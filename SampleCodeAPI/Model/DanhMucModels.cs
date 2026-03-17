using Newtonsoft.Json;
using System.Net;
using System.Text.Json.Serialization;

namespace SampleCodeAPI.Model
{
    public class BacDaoTaoModels
    {
        public int RowId { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string TenTiengAnh { get; set; }
        public string NoiDung { get; set; }
        public string GhiChu { get; set; }
        public int HinhThucDaoTao { get; set; }
        public int SoThuTu { get; set; }
    }
}
