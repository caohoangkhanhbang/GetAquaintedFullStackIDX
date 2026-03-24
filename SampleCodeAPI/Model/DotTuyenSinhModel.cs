namespace SampleCodeAPI.Model
{
    public class DotTuyenSinhModel
    {
        public int? Id { get; set; }
        public int NamHoc { get; set; } 
        public int Dot { get; set; }    
        public string TenDotTS { get; set; }
        public int KhoaHoc { get; set; }
        public DateTime? ThoiGianNhanHSTuNgay { get; set; }
        public DateTime? ThoiGianNhanHSDenNgay { get; set; }
        public DateTime? NgayInGBTT { get; set; } 
        public DateTime? ThoiGianLayHSTuNgay { get; set; }
        public DateTime? ThoiGianLayHSDenNgay { get; set; }
        public DateTime? NgayNhapHocDK { get; set; } 
        public string? GhiChu { get; set; }
        public bool? HienThi { get; set; }
        public bool? KichHoat { get; set; }
        public int? DeletedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? TenKhoaHoc { get; set; }
        public string? TenNamHoc { get; set; }
    }
}
