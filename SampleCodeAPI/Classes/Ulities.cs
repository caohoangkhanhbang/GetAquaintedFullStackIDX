using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using SampleCodeAPI.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SampleCodeAPI.Classes
{
    public class Ulities
    {
        public IConfiguration config;
        public Ulities(IConfiguration _config)
        {
            config = _config;
        }
        public UserJWT GetUserByHeader(IHeaderDictionary pHeader)
        {
            try
            {
                if (pHeader == null) return null;
                if (!pHeader.ContainsKey(HeaderNames.Authorization)) return null;

                IHeaderDictionary _d = pHeader;
                string _bearer_token, _username, _customdata, _id;
                _bearer_token = _d[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();

                var secretKey = Encoding.ASCII.GetBytes(config["Jwt:access_secret"]);

                var valParams = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true, // Validate signature
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ValidateLifetime = true,  // Validate life time of token
                    ValidateAudience = false, // Because there is no audiance in the generated token
                    ValidateIssuer = false,   // Because there is no issuer in the generated token
                    ClockSkew = TimeSpan.Zero
                };

                #region Phần code dùng cho public live để validate Token
                SecurityToken tokenS;
                var claimsPrincipal = handler.ValidateToken(_bearer_token, valParams, out tokenS);
                var claims = claimsPrincipal.Claims;

                if (tokenS.ValidTo <= DateTime.UtcNow) return null;


                if (claims == null) return null;

                _id = claims.First(x => x.Type == "userId").Value;
                _username = claims.First(x => x.Type == "username").Value;
                _customdata = claims.First(x => x.Type == "customdata").Value;
                #endregion

                #region Phần code dùng cho dev để tránh validate Token
                //var tokenS = handler.ReadToken(_bearer_token) as JwtSecurityToken;
                //var claims = tokenS.Claims.Where(x => x.Type == "userId").FirstOrDefault();
                //if (claims == null) return null;
                //_id = claims.Value;

                //claims = tokenS.Claims.Where(x => x.Type == "username").FirstOrDefault();
                //if (claims == null) return null;
                //_username = claims.Value;

                //claims = tokenS.Claims.Where(x => x.Type == "customdata").FirstOrDefault();
                //if (claims == null) return null;
                //_customdata = claims.Value;
                #endregion


                UserJWT q = new UserJWT();
                q._id = _id;
                q.UserName = _username;
                q.customdata = JsonConvert.DeserializeObject<CustomData>(_customdata);
                if (q.customdata.jeeAccount != null)
                {
                    q.staffID = q.customdata.jeeAccount.staffID;
                    q.customerID = long.Parse(q.customdata.jeeAccount.customerID);
                }
                return q;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
