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

namespace SampleCodeAPI.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/sample")]
    [ApiController]
    [ApiVersion("1.0")]
    public class SampleController(IConfiguration configuration, IConnectionCache connectionCache, ILogger<SampleController> logger, IProducer producer, INotifyService notifyService, IBoxEvent boxEvent, MinioObject minioClient) : ControllerBase
    {
        private IConfiguration _configuration = configuration;
        private IConnectionCache _cache = connectionCache;
        private readonly Ulities _ulities = new Ulities(configuration);
        private readonly IProducer _producer = producer;
        private readonly LoggerHelper _logHelper = new LoggerHelper(logger);
        private readonly INotifyService _notifyService = notifyService;
        private readonly IBoxEvent _boxEvent = boxEvent;
        private readonly MinioObject _minioClient = minioClient;

        /// <summary>
        /// Code mẫu check token 
        /// </summary>
        /// <returns></returns>
        [HttpGet("CheckToken")]
        public object CheckToken()
        {
            UserJWT loginData = _ulities.GetUserByHeader(HttpContext.Request.Headers);

            if (loginData == null)
                return JsonResultCommon.DangNhap();
            try
            {
                return JsonResultCommon.ThanhCong();
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, _configuration, loginData.customerID);
            }
        }

        /// <summary>
        /// Code mẫu check CusAuthorizeAttribute - Xư lý nẫu cho ứng dụng HR - AppID là 1, các ứng dụng khác thi tùy biến 
        /// </summary>
        /// <returns></returns>
        [CusAuthorizeAttribute("37")]
        [HttpGet("CheckAuthor")]
        public object CheckAuthor()
        {
            UserJWT loginData = _ulities.GetUserByHeader(HttpContext.Request.Headers);

            if (loginData == null)
                return JsonResultCommon.DangNhap();
            try
            {
                return JsonResultCommon.ThanhCong();
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, _configuration, loginData.customerID);
            }
        }

        /// <summary>
        /// Upload file và get Link file lên cdn - Dùng appcode ứng dụng HR làm ví dụ
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [Route("UpLoad")]
        [HttpPost]
        public async Task<object> UploadImg(IFormFile file)
        {
            UserJWT loginData = _ulities.GetUserByHeader(HttpContext.Request.Headers);
            if (loginData == null)
                return JsonResultCommon.DangNhap();

            string imgext = Path.GetExtension(file.FileName);
            try
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    string str = Convert.ToBase64String(fileBytes);
                    byte[] imageBytes = Convert.FromBase64String(str.ToString());
                    upLoadFileModel up = new upLoadFileModel()
                    {
                        bs = imageBytes,
                        FileName = file.FileName,
                        Linkfile = $"{loginData.customerID}/File/{DateTime.Now.Ticks}",
                        CustomerID = loginData.customerID,
                        UserID = loginData.customdata.jeeAccount.userID,
                        AppCode = _configuration.GetValue<string>("AppConfig:AppCode")
                    };
                    var result = await UploadFile.UploadFileAllTypeMinio(up, _configuration, _producer, file.ContentType);
                    if (result.status)
                    {
                        return (new
                        {
                            succeeded = true,
                            imageUrl = _configuration.GetValue<string>("MinioConfig:MinioServer") + result.link
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return new
                {
                    succeeded = false,
                };
            }
            return new
            {
                succeeded = false,
            };
        }

        /// <summary>
        /// Get file upload từ cdn
        /// </summary>
        /// <returns></returns>
        [HttpGet("getFileUpload")]
        public async Task<object> getFileUpload()
        {
            try
            {
                string link = "/jee-tutorial/25/File/638747999616304277/Hướng dẫn sử dụng Automation Service.docx";
                var result = await _minioClient.GetUrl_CDN(_configuration.GetValue<string>("AppConfig:BucKetID") ?? "", link);
                result = result.ToString().Replace($":443/{_configuration.GetValue<string>("AppConfig:BucKetID")}/", "");
                return (new
                {
                    imageUrl = _configuration.GetValue<string>("MinioConfig:MinioServer") + link, //Không thời hạn nếu bucket là public
                    imageUrl_Token = result,//Có thời hạn token nếu bucket là private
                });
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        /// <summary>
        /// Code mẫu phần ghi log lỗi và ghi log thông tin
        /// Chi tiết tham khảo tại link https://docs.jee.vn/fordev/autolog-server.html#gioi-thieu
        /// </summary>
        /// <returns></returns>
        [HttpGet("Log")]
        public object Log()
        {
            UserJWT loginData = _ulities.GetUserByHeader(HttpContext.Request.Headers);
            if (loginData == null)
                return JsonResultCommon.DangNhap();

            try
            {
                //Ghi log lỗi
                _logHelper.LogError(loginData.UserName, "SampleController", "Ghi log lỗi", "Data lỗi");

                //ghi log thông tin
                _logHelper.LogInfo(loginData.UserName, "SampleController", "Ghi log thông tin", "Data");

                return JsonResultCommon.ThanhCong();
            }
            catch (Exception ex)
            {
                _logHelper.LogError(loginData.UserName, "SampleController", "Xem", ex);
                return JsonResultCommon.Exception(ex, _configuration, loginData.customerID);
            }
        }

        /// <summary>
        /// Code mẫu gửi notify
        /// </summary>
        /// <returns></returns>
        [HttpGet("SendNotify")]
        public object SendNotify()
        {
            UserJWT loginData = _ulities.GetUserByHeader(HttpContext.Request.Headers);
            if (loginData == null)
                return JsonResultCommon.DangNhap();

            try
            {
                NotificationMess noti_mess = new()
                {
                    Content = loginData.customdata.personalInfo.Fullname + " thao tác ",
                    Img = loginData.customdata.personalInfo.Avatar,
                    Link = $"",
                    OsLink = $"",
                    IdObject = 1,
                    customerID = loginData.customerID,
                };
                _notifyService.sendNotify(loginData.UserName, ["thienng"], noti_mess, "", "", true);
                return JsonResultCommon.ThanhCong();
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, _configuration, loginData.customerID);
            }
        }

        /// <summary>
        /// Code mẫu gửi mail
        /// </summary>
        /// <returns></returns>
        [HttpGet("SendMail")]
        public object SendMail()
        {
            UserJWT loginData = _ulities.GetUserByHeader(HttpContext.Request.Headers);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            try
            {
                //Gửi 1 người - gửi theo email
                //_notifyService.sendEmail(loginData.customerID, "thienng@dps.com.vn", "Chúc mừng năm mới", "An khang thịnh vượng");
                //Gửi nhiều người - gửi theo username
                _notifyService.sendEmails(loginData.customerID, loginData.UserName, ["thienng"], [""], [""], "Chúc mừng năm mới", "An khang thịnh vượng");
                return JsonResultCommon.ThanhCong();
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, _configuration, loginData.customerID);
            }
        }

        /// <summary>
        /// Code mẫu sử dụng cho cập nhật nhắc nhở
        /// </summary>
        /// <returns></returns>
        [HttpGet("Update_Reminder")]
        public object Update_Reminder()
        {
            var demo = new
            {
                PhanLoaiID = 804,//căn cứ bảng ReminderTypes db landingpage
                SoLuong = 1,
                UserID = 77116,//Giá trị UserID
                CustomerID = 25,//Giá trị CustomerID
                DataField = "Sophu2",//Cập nhật cột trong bảng Reminders (SoLuong, SoPhu1, Sophu2, Sophu3)
                FieldChange = "+",//(+ là tăng ; - là giảm)
                Email = "thienng@dps.com.vn",
                Username = "thienng@dps.com.vn",
            };
            string TopicAddNewCustomer = _configuration.GetValue<string>("KafkaConfig:TopicProduce:JeeplatformUpdatReminder") ?? "";
            _producer.PublishAsync(TopicAddNewCustomer, Newtonsoft.Json.JsonConvert.SerializeObject(demo));
            return JsonResultCommon.ThanhCong();
        }

        /// <summary>
        /// Code mẫu gửi message vào topic eventlist (khi có sự kiện xảy ra ở ứng dụng)
        /// Dùng cho cách tính phương trình bậc 2
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Route("SendMessage")]
        [HttpPost]
        public async Task<object> SendMessage([FromBody] EventTestMessage data)
        {
            try
            {
                string guid = Guid.NewGuid().ToString();
                OutboxEventMessage outbox = new()
                {
                    ID = guid,
                    AggregateType = "AutoEventList",
                    AggregateID = 0,
                    EventType = "DongBo",
                    Payload = JsonConvert.SerializeObject(data),
                    Status = 0,
                    Retry_count = 0,
                    SourceID = "HR",
                    ErrorMessage = ""
                };
                //save to outbox
                var connectString = _configuration.GetValue<string>("AppConfig:ConnectionString") ?? "";
                await _boxEvent.changeOutboxEvent(outbox, connectString, true);
                //push to kafka
                var topicEventList = _configuration.GetValue<string>("KafkaConfig:TopicConsume:AutoEventList");
                var result1 = await _producer.PublishProducerAsync(topicEventList, JsonConvert.SerializeObject(outbox));
                if (result1 != null) //chưa thấy trả lỗi
                {
                    outbox.ID = result1.id;
                    outbox.Retry_count = result1.retry;
                    outbox.Status = result1.statusid;
                    outbox.ErrorMessage = result1.error ?? "";
                    outbox.EventType = "AutoEventList";
                    var k = await _boxEvent.changeOutboxEvent(outbox, connectString); //update status outbox
                }
                return 1;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
