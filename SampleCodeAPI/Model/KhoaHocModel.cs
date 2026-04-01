using System.Text.Json.Serialization;

namespace SampleCodeAPI.Model
{
    public class KhoaHocModel
    {
        public int? id { get; set; }
        public String TenKhoaHoc { get; set; }
        public String? TenNamHoc { get; set; }
        public int NamHoc { get; set; }
        public string? CachViet { get; set; }
        public bool? Disable { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
