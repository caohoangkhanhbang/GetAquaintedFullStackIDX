using API_JeeSale.Services;
using DPSinfra.ConnectionCache;
using DPSinfra.Kafka;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SampleCodeAPI.Business;
using SampleCodeAPI.Classes;
using SampleCodeAPI.Model;
using SampleCodeAPI.Services;

namespace SampleCodeAPI.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/khoahoc")]
    [ApiController]
    [ApiVersion("1.0")]

    public class QLKhoaHocController(IConfiguration configuration, IConnectionCache connectionCache, ILogger<SampleController> logger, IProducer producer, INotifyService notifyService, IBoxEvent boxEvent, MinioObject minioClient, IConnectionService connectionService) : ControllerBase
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
                var result = await BusKhoaHoc.GetList(query, connect);
                return result;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(loginData.UserName, "QLKhoaHocController", message, ex);
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
            var model = await BusKhoaHoc.GetDetail(Id, connect);
            return model;
        }

        [HttpPost]
        [Route("insert")]
        public async Task<object> Insert(KhoaHocModel data)
        {
            UserJWT loginData = _ulities.GetUserByHeader(HttpContext.Request.Headers);
            if (loginData == null)
                return JsonResultCommon.DangNhap();

            var message = "Thêm mới";
            try
            {
                string connect = _connection.getConnectionString(loginData.customerID);
                var result = await BusKhoaHoc.Insert(data, connect, loginData);
                return result;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(loginData.UserName, "QLKhoaHocController", message, ex);
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<object> Update(KhoaHocModel data)
        {
            UserJWT loginData = _ulities.GetUserByHeader(HttpContext.Request.Headers);
            if (loginData == null)
                return JsonResultCommon.DangNhap();

            var message = "Cập nhật";
            try
            {
                string connect = _connection.getConnectionString(loginData.customerID);
                var result = await BusKhoaHoc.Update(data, connect, loginData);
                return result;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(loginData.UserName, "QLKhoaHocController", message, ex);
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
            var model = await BusKhoaHoc.Delete(Id, connect, loginData);
            return model;
        }

        //Hàm xuất excel
        [HttpGet]
        [Route("export-excel")]
        public async Task<IActionResult> ExportExcel([FromQuery] QueryParams query)
        {
            var loginData = _ulities.GetUserByHeader(HttpContext.Request.Headers);
            if (loginData == null)
                return Unauthorized(JsonResultCommon.DangNhap());

            const string message = "Xuất danh sách khóa học ra Excel";
            try
            {
                string connect = _connection.getConnectionString(loginData.customerID);
                byte[] fileBytes = await BusKhoaHoc.ExportToExcel(loginData, query, connect);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Danh_Sach_Nam_Hoc_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                _logHelper.LogError(loginData.UserName, "QLDiemQuatrinh_ExportExcel", message, ex);
                return BadRequest(JsonResultCommon.Exception(ex));
            }
        }
    }
}


