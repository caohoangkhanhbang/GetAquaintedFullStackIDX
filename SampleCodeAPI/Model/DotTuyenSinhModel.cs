namespace SampleCodeAPI.Model
{
    public class DotTuyenSinhModel
    {
        public int Id { get; set; }
        public short? NamHoc { get; set; } 
        public byte? Dot { get; set; }    
        public string TenDotTS { get; set; }
        public string KhoaHoc { get; set; }
        public DateTime? ThoiGianNhanHSTuNgay { get; set; }
        public DateTime? ThoiGianNhanHSDenNgay { get; set; }
        public DateTime? NgayInGBTT { get; set; } 
        public DateTime? ThoiGianLayHSTuNgay { get; set; }
        public DateTime? ThoiGianLayHSDenNgay { get; set; }
        public DateTime? NgayNhapHocDK { get; set; } 
        public string GhiChu { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public bool HienThi { get; set; }
    }
}
