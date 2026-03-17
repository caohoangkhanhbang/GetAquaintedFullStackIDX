using Newtonsoft.Json;
using System.Net;
using System.Text.Json.Serialization;

namespace SampleCodeAPI.Model
{
    public class EventTestMessage
    {
        public int AppID { get; set; }
        public int CustomerID { get; set; }
        public int UserID { get; set; } = 0;
        public string? EventCode { get; set; } //Giống với khai báo ở EventSource
        public string? EventName { get; set; }
        public Input1Model? EventData { get; set; }
    }
    public class Input1Model
    {
        public float a { get; set; }
        public float b { get; set; }
    }

    public class Action1Model
    {
        public int CustomerID { get; set; }
        public int UserID { get; set; } = 0;
        public long ActionID { get; set; }
        public string? ActionCode { get; set; }
        public string? ActionName { get; set; }
        public Input1Model? ActionData { get; set; }
    }

    public class Output1Model
    {
        public float c { get; set; }
    }

    public class KhoaPhongBanModel
    {
        public int CustomerID { get; set; }
        public int UserID { get; set; } = 0;
        public long ActionID { get; set; }
        public string? ActionCode { get; set; }
        public string? ActionName { get; set; }
        public InputKhoaPhongBanModel? ActionData { get; set; }
    }
    public class InputKhoaPhongBanModel
    {
        public int ParentID { get; set; }
        public int RowID { get; set; }
        public int Level { get; set; }
        public int Position { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public int WorkingModeID { get; set; }
        public int Type { get; set; }//0: Thêm mới/Cập nhật ; 1: Xóa
    }
}
