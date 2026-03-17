using DPSinfra.Notifier;
using SampleCodeAPI.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_JeeSale.Services
{
    public interface INotifyService
    {
        void sendNotify(string sender, string[] receivers, NotificationMess noti_mess, string mess_text = "", string mess_html = "", bool isLanding = false, bool isAppJee = true);
        void sendEmail(long CustomerID, string email, string title, string contents);
        void sendEmails(long CustomerID, string Username, string[] receivers, string[] cc_receivers, string[] bcc_receivers, string title, string contents);
    }
}
