namespace SampleCodeAPI.Classes
{
    public class NotifyHelper
    {
        
    }
    public class NotificationMess
    {
        /// <summary>
        /// Nội dung thông báo
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// Avatar người gửi
        /// </summary>
        public string Img { get; set; }
        public string Icon { get; set; } = "https://cdn.jee.vn/jee-account/images/icons/Jee_Sale.png"; //kèm icon thông báo
        public string AppCode { get; set; }
        /// <summary>
        /// Link domain web
        /// </summary>
        public string Domain { get; set; }
        /// <summary>
        /// Link web chuyển nếu có
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// Link app chuyển nếu có
        /// </summary>
        public string OsLink { get; set; }
        /// <summary>
        /// Link flatform app nếu có
        /// </summary>
        public string LinkDesktop { get; set; }
        public int Loai { get; set; }

        //Desktop
        public int IdObject { get; set; }
        public int MenuID { get; set; } = 6;
        public int SubmenuID { get; set; }

        public long customerID { get; set; }
    }
    public class MobileLinkModel
    {
        public string screen { get; set; }
        public object param { get; set; }
    }
}
