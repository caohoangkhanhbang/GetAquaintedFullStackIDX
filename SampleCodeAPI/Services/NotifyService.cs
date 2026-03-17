using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Confluent.Kafka;
using DPSinfra.Notifier;
using DpsLibs.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SampleCodeAPI.Classes;

namespace API_JeeSale.Services
{
    public class NotifyService : INotifyService
    {
        private readonly INotifier _notifier;
        private readonly IConfiguration _configuration;
        public NotifyService(INotifier notifier, IConfiguration configuration)
            => (_notifier, _configuration) = (notifier, configuration);

        public void sendNotify(string sender, string[] receivers, NotificationMess noti_mess,
            string mess_text = "", string mess_html = "", bool isLanding = false, bool isAppJee = false)
        {
            string linkBE = _configuration.GetValue<string>("Host:LinkBackend") ?? "";
            string linkLanding = _configuration.GetValue<string>("Host:JeeLanding_Link") ?? "";
            string appCode = _configuration.GetValue<string>("AppConfig:AppCode") ?? "";
            string linkMobile = _configuration.GetValue<string>("Mobile:code") ?? ""; //thêm link mobile bms

            string mobile_link = linkMobile + "/";

            noti_mess.AppCode = isLanding ? "LANDING" : appCode;
            noti_mess.Domain = isLanding ? linkLanding : linkBE;
            if (string.IsNullOrEmpty(noti_mess.Img))
            {
                noti_mess.Img = "https://cdn.jee.vn/jee-account/images/icons/Jee_Sale.png";//Thay đổi theo ứng dụng tương ứng
            }
            noti_mess.Link = linkBE + noti_mess.Link;
            noti_mess.OsLink = mobile_link + noti_mess.OsLink;

            socketMessage asyncnotice = new socketMessage()
            {
                sender = sender,
                receivers = receivers,
                message_text = noti_mess.Content, //mess_text
                message_html = mess_html,
                message_json = JsonConvert.SerializeObject(noti_mess),

                // Các field dưới đây là của OneSignal
                osTitle = "Thông báo từ hệ thống",
                osMessage = noti_mess.Content,
                osWebURL = noti_mess.Link,
                osAppURL = noti_mess.OsLink,
                osIcon = noti_mess.Icon,

            };
            _notifier.sendSocket(asyncnotice);
        }

        public void sendEmail(long CustomerID, string email, string title, string contents)
        {
            emailMessage asyncnotice = new emailMessage()
            {

                CustomerID = CustomerID,
                to = email,
                subject = title,
                html = contents
            };
            _notifier.sendEmail(asyncnotice);
        }
        public void sendEmails(long CustomerID, string Username, string[] receivers, string[] cc_receivers, string[] bcc_receivers, string title, string contents)
        {
            emailMessage asyncnotice = new emailMessage()
            {
                CustomerID = 1, 
                sender = Username, //Username người gửi
                receivers = receivers, 
                cc_receivers = cc_receivers, 
                bcc_receivers = bcc_receivers, 
                subject = title,
                html = contents,
            };
            _notifier.sendEmail(asyncnotice);
        }
    }
}
