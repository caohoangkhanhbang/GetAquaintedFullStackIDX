using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using SampleCodeAPI.Model;
using System.IdentityModel.Tokens.Jwt;
using DPSinfra.Redis;

namespace SampleCodeAPI.Classes
{
    public class CusAuthorizeAttribute : TypeFilterAttribute
    {
        public CusAuthorizeAttribute(string permission = "")
        : base(typeof(CusAuthorize))
        {
            Arguments = new object[] { permission };
        }
    }
    public class CusAuthorize : IAuthorizationFilter
    {
        private readonly string _permission;
        private readonly IRedisService _redisService;
        public CusAuthorize(string permission, IRedisService redisService)
        {
            _permission = permission;
            _redisService = redisService;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (string.IsNullOrEmpty(_permission))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            else
            {
                try
                {
                    IHeaderDictionary _d = context.HttpContext.Request.Headers;

                    if (!_d.ContainsKey(HeaderNames.Authorization))
                        context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };

                    string _bearer_token, _customdata;

                    _bearer_token = _d[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
                    var handler = new JwtSecurityTokenHandler();
                    var tokenS = handler.ReadToken(_bearer_token) as JwtSecurityToken;
                    var claims = tokenS.Claims.Where(x => x.Type == "customdata").FirstOrDefault();
                    if (claims == null)
                    {
                        context.Result = new UnauthorizedResult();
                        return;
                    }
                    _customdata = claims.Value;
                    CustomData cusData = new CustomData();
                    cusData = JsonConvert.DeserializeObject<CustomData>(_customdata);
                    if (DateTime.UtcNow > tokenS.ValidTo)
                    {
                        context.Result = new JsonResult(new { message = "Expired" }) { StatusCode = StatusCodes.Status408RequestTimeout };
                    }
                    else
                    {
                        //Kiểm tra quyền ở đây
                        var requiredPermissions = _permission.Split(","); // Nhận nhiều mã quyền từ controller, xóa "," để lấy từng quyền
                        var listQuyen = new List<string>();// Lấy ra danh sách quyền trên redis tương ứng
                        string appID = "1"; // AppID tương ứng ở từng ứng dụng
                        if (cusData.RoleHR == null)
                        {
                            context.Result = new JsonResult(new { message = "Forbidden" }) { StatusCode = StatusCodes.Status403Forbidden };
                            return;
                        }
                        var data = _redisService.GetRedis(appID, cusData.RoleHR.roles);
                        listQuyen = data.Result.Split(",").ToList();
                        foreach (var x in requiredPermissions)
                        {
                            if (listQuyen.Contains(x))
                                return; //User Authorized
                        }
                        context.Result = new JsonResult(new { message = "Forbidden" }) { StatusCode = StatusCodes.Status403Forbidden };
                        return;
                    }

                }
                catch (Exception ex)
                {
                    context.Result = new JsonResult(new { message = "Forbidden" }) { StatusCode = StatusCodes.Status403Forbidden };
                }
            }
        }
    }
}
