using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using System.Data;
using DPSinfra.ConnectionCache;
using DpsLibs.Data;
using SampleCodeAPI.Classes;
using SampleCodeAPI.Model;
using Microsoft.IdentityModel.Logging;
using Confluent.Kafka;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using DPSinfra.UploadFile;
using DPSinfra.Kafka;
using Microsoft.Extensions.Logging;
using API_JeeSale.Services;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Minio.DataModel;
using Minio;
using System.Security.AccessControl;
using SampleCodeAPI.Services;
using SampleCodeAPI.Business;
using Microsoft.AspNetCore.Razor.TagHelpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SampleCodeAPI.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/tutorial")]
    [ApiController]
    [ApiVersion("1.0")]
    public class TutorialController(IConfiguration configuration, IConnectionCache connectionCache, ILogger<SampleController> logger, IProducer producer, INotifyService notifyService, IBoxEvent boxEvent, MinioObject minioClient, IConnectionService connectionService) : ControllerBase
    {
        private IConfiguration _configuration = configuration;
        private IConnectionCache _cache = connectionCache;
        private readonly Ulities _ulities = new Ulities(configuration);
        private readonly IProducer _producer = producer;
        private readonly LoggerHelper _logHelper = new LoggerHelper(logger);
        private readonly INotifyService _notifyService = notifyService;
        private readonly IBoxEvent _boxEvent = boxEvent;
        private readonly MinioObject _minioClient = minioClient;
        private readonly IConnectionService _connection = connectionService;

        [HttpGet]
        [Route("list")]
        public async Task<object> GetList([FromQuery] QueryParams query)
        {
            UserJWT loginData = _ulities.GetUserByHeader(HttpContext.Request.Headers);
            if (loginData == null)
                return JsonResultCommon.DangNhap();

            var message = "Load danh sách";
            try
            {
                string connect = _connection.getConnectionString(loginData.customerID);
                var result = await TutorialAPI.GetList(query, connect);
                return result;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(loginData.UserName, "TutorialController", message, ex);
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpGet]
        [Route("detail/{Id}")]
        public async Task<object> GetDetail(long Id)
        {
            UserJWT loginData = _ulities.GetUserByHeader(HttpContext.Request.Headers);
            if (loginData == null)
                return JsonResultCommon.DangNhap();

            string connect = _connection.getConnectionString(loginData.customerID);
            var model = await TutorialAPI.GetDetail(Id, connect);
            return model;
        }

        [HttpPost]
        [Route("insert")]
        public async Task<object> Insert(TutorialModels data)
        {
            UserJWT loginData = _ulities.GetUserByHeader(HttpContext.Request.Headers);
            if (loginData == null)
                return JsonResultCommon.DangNhap();

            var message = "Thêm mới";
            try
            {
                string connect = _connection.getConnectionString(loginData.customerID);
                var result = await TutorialAPI.Insert(data, connect);
                return result;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(loginData.UserName, "TutorialController", message, ex);
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<object> Update(TutorialModels data)
        {
            UserJWT loginData = _ulities.GetUserByHeader(HttpContext.Request.Headers);
            if (loginData == null)
                return JsonResultCommon.DangNhap();

            var message = "Cập nhật";
            try
            {
                string connect = _connection.getConnectionString(loginData.customerID);
                var result = await TutorialAPI.Update(data, connect);
                return result;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(loginData.UserName, "TutorialController", message, ex);
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpDelete]
        [Route("delete/{Id}")]
        public async Task<object> Delete(long Id)
        {
            UserJWT loginData = _ulities.GetUserByHeader(HttpContext.Request.Headers);
            if (loginData == null)
                return JsonResultCommon.DangNhap();

            string connect = _connection.getConnectionString(loginData.customerID);
            var model = await TutorialAPI.Delete(Id, connect);
            return model;
        }

    }
}
