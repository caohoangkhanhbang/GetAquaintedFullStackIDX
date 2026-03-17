namespace SampleCodeAPI.Model
{
    public class LoaiDaoTaoModel
    {
        public int id { get; set; }
        public string MaLoaiDT { get; set; }
        public string TenLoaiDT { get; set; }
        public string TenTiengAnh { get; set; }
        public string NoiDung { get; set; }
        public int STT { get; set; }
        public string GhiChu { get; set; }
        public bool IsDel { get; set; }
        public string NguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
