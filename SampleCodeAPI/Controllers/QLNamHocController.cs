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
using System.Data;
using System.Data.SqlClient;

namespace SampleCodeAPI.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/namhoc")]
    [ApiController]
    [ApiVersion("1.0")]
    public class QLNamHocController(IConfiguration configuration, IConnectionCache connectionCache, ILogger<SampleController> logger, IProducer producer, INotifyService notifyService, IBoxEvent boxEvent, MinioObject minioClient, IConnectionService connectionService) : ControllerBase
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
        [Route("dsnamhoc")]
        public async Task<object> GetList1(string connect)
        {
            List<NamHocModel> lst = new List<NamHocModel>();
            using (SqlConnection con = new SqlConnection(connect))
            {
                string sqlq = "select * from DanhSachNamHoc";
                SqlCommand cmd = new SqlCommand(sqlq, con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        NamHocModel modelnh = new NamHocModel
                        {
                            id = int.Parse(dr["id"].ToString()),
                            STT = int.Parse(dr["STT"].ToString()),
                            NamHoc = int.Parse(dr["NamHoc"].ToString()),
                            NienHoc = dr["NienHoc"].ToString(),
                            HienThi = bool.Parse(dr["HienThi"].ToString()),
                            NguoiTao = dr["NguoiTao"].ToString(),
                            NgayTao = DateTime.Parse(dr["NgayTao"].ToString())
                        };
                        lst.Add(modelnh);
                    }
                }
            }
          
            return lst;
        }
    
        //[HttpGet]
        //[Route("namhoc")]
        //public async Task<NamHocModel> GetListADO()
        //{
        //    try
        //    {
        //        string connect = _configuration.GetConnectionString("ConnectionString");
        //        var result = await BusQLNamHoc.GetList1(connect);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logHelper.LogError("System", "QLNamHocController", "Load danh sách", ex);
        //        return null;
        //    }
        //}

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
                var result = await BusQLNamHoc.GetList(query, connect);
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
            var model = await BusQLBacDaoTao.GetDetail(Id, connect);
            return model;
        }

        [HttpPost]
        [Route("insert")]
        public async Task<object> Insert(BacDaoTaoModels data)
        {
            UserJWT loginData = _ulities.GetUserByHeader(HttpContext.Request.Headers);
            if (loginData == null)
                return JsonResultCommon.DangNhap();

            var message = "Thêm mới";
            try
            {
                string connect = _connection.getConnectionString(loginData.customerID);
                var result = await BusQLBacDaoTao.Insert(data, connect, loginData);
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
        public async Task<object> Update(BacDaoTaoModels data)
        {
            UserJWT loginData = _ulities.GetUserByHeader(HttpContext.Request.Headers);
            if (loginData == null)
                return JsonResultCommon.DangNhap();

            var message = "Cập nhật";
            try
            {
                string connect = _connection.getConnectionString(loginData.customerID);
                var result = await BusQLBacDaoTao.Update(data, connect, loginData);
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
            var model = await BusQLBacDaoTao.Delete(Id, connect, loginData);
            return model;
        }
    }
}
