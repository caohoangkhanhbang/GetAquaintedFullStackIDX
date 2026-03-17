using API_JeeSale.Services;
using DPSinfra.ConnectionCache;
using DPSinfra.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SampleCodeAPI.Business;
using SampleCodeAPI.Classes;
using SampleCodeAPI.Model;
using SampleCodeAPI.Services;

namespace SampleCodeAPI.Controllers
{
    [Route("api/loaidaotao")]
    [ApiController]
    public class LoaiDaoTaoController(IConfiguration configuration, IConnectionCache connectionCache, ILogger<SampleController> logger, IProducer producer, INotifyService notifyService, IBoxEvent boxEvent, MinioObject minioClient, IConnectionService connectionService) : ControllerBase
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
                var result = await BusLoaiDaoTao.GetList(query, connect);
                return result;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(loginData.UserName, "QLBacDaoTaoController", message, ex);
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
            var model = await BusLoaiDaoTao.GetDetail(Id, connect);
            return model;
        }

        [HttpPost]
        [Route("insert")]
        public async Task<object> Insert(LoaiDaoTaoModel data)
        {
            UserJWT loginData = _ulities.GetUserByHeader(HttpContext.Request.Headers);
            if (loginData == null)
                return JsonResultCommon.DangNhap();


            var message = "Thêm mới";
            try
            {
                string connect = _connection.getConnectionString(loginData.customerID);
                var result = await BusLoaiDaoTao.Insert(data, connect, loginData);
                return result;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(loginData.UserName, "QLBacDaoTaoController", message, ex);
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<object> Update(LoaiDaoTaoModel data)
        {
            UserJWT loginData = _ulities.GetUserByHeader(HttpContext.Request.Headers);
            if (loginData == null)
                return JsonResultCommon.DangNhap();

            var message = "Cập nhật";
            try
            {
                string connect = _connection.getConnectionString(loginData.customerID);
                var result = await BusLoaiDaoTao.Update(data, connect, loginData);
                return result;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(loginData.UserName, "QLBacDaoTaoController", message, ex);
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
            var model = await BusLoaiDaoTao.Delete(Id, connect, loginData);
            return model;
        }

    }
}

