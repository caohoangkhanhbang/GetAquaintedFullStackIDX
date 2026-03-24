using API_JeeSale.Services;
using DPSinfra.ConnectionCache;
using DPSinfra.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Minio;
using SampleCodeAPI.Business;
using SampleCodeAPI.Classes;
using SampleCodeAPI.Model;
using SampleCodeAPI.Services;

namespace SampleCodeAPI.Controllers
{
    [Route("api/share")]
    [ApiController]
    public class SharedController(IConfiguration configuration, IConnectionCache connectionCache, ILogger<SampleController> logger, IProducer producer, INotifyService notifyService, IBoxEvent boxEvent, MinioObject minioClient, IConnectionService connectionService) : ControllerBase
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
        [Route("list-nam-hoc")]
        public async Task<object> GetListNamHoc()//[FromQuery] QueryParams query
        {
            UserJWT loginData = _ulities.GetUserByHeader(HttpContext.Request.Headers);
            if (loginData == null)
                return JsonResultCommon.DangNhap();

            var message = "Load danh sách";
            try
            {
                string connect = _connection.getConnectionString(loginData.customerID);
                var result = await BusShare.GetListNamHoc( connect);//query,
                return result;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(loginData.UserName, "SharedController", message, ex);
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpGet]
        [Route("list-khoa-hoc")]
        public async Task<object> GetListKhoaHoc()//[FromQuery] QueryParams query
        {
            UserJWT loginData = _ulities.GetUserByHeader(HttpContext.Request.Headers);
            if (loginData == null)
                return JsonResultCommon.DangNhap();

            var message = "Load danh sách";
            try
            {
                string connect = _connection.getConnectionString(loginData.customerID);
                var result = await BusShare.GetListKhoaHoc(connect);//query,
                return result;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(loginData.UserName, "SharedController", message, ex);
                return JsonResultCommon.Exception(ex);
            }
        }

    }
}
