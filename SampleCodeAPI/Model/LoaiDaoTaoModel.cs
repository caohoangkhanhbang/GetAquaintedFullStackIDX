namespace SampleCodeAPI.Model
{
    public class LoaiDaoTaoModel
    {
        public int id { get; set; }
        public string MaLoaiDT { get; set; }
        public string TenLoaiDT { get; set; }
        public string? TenTiengAnh { get; set; }
        public string? NoiDung { get; set; }
        public long? SoThuTu { get; set; }
        public string GhiChu { get; set; }
        public bool? IsDel { get; set; }
        public int? DeletedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        

    }
}
