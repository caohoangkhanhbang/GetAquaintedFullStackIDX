namespace SampleCodeAPI.Model
{
    public class NamHocModel
    {
        public int? id { get; set; }
        public int NamHoc { get; set; }
        public string NienHoc { get; set; }
        public bool? Disable { get; set; }
        public int? DeletedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
}
