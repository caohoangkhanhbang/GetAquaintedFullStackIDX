using DpsLibs.Data;
using SampleCodeAPI.Model;
using System;
using System.Data;

namespace SampleCodeAPI.Business
{
    public class BusShare
    {
        public static async Task<object> GetListNamHoc( string connect)//QueryParams query,
        {
            BaseModel<object> model = new BaseModel<object>();
            PageModel pageModel = new PageModel();
            ErrorModel error = new ErrorModel();
            using (DpsConnection cnn = new DpsConnection(connect))
            {

                SqlConditions Conds = new SqlConditions();
                string sqlq = "";
                sqlq = @"
                        SELECT *
                        FROM DanhSachNamHoc 
                        WHERE DATEDIFF(YEAR, CreatedDate, GETDATE()) <= 10 and isDel = 0 and Disable = 1
                        ORDER BY CreatedDate DESC;
                        ";
                DataTable dt = cnn.CreateDataTable(sqlq, Conds);
                if (cnn.LastError != null || dt == null)
                {
                    model.status = 0;
                    model.error = new ErrorModel
                    {
                        message = "lỗi dữ liệu"
                    };
                    return model;
                }
                if (dt.Rows.Count == 0)
                {
                    model.status = 1;
                    model.error = new ErrorModel
                    {
                        message = "không có dữ liệu"
                    };
                    return model;
                }

                var data = (from r in dt.AsEnumerable()
                            select new
                            {
                                Id = r["Id"],
                                NamHoc = r["NamHoc"] != DBNull.Value ? r["NamHoc"] : "",
                                NienHoc = r["NienHoc"] != DBNull.Value ? r["NienHoc"] : "",
                            }).ToList();

                model.data = data;
                model.status = 1;
                return model;
            }
        }

        public static async Task<object> GetListKhoaHoc(string connect)
        {
            BaseModel<object> model = new BaseModel<object>();
            PageModel pageModel = new PageModel();
            ErrorModel error = new ErrorModel();
            using (DpsConnection cnn = new DpsConnection(connect))
            {

                SqlConditions Conds = new SqlConditions();
                string sqlq = "";
                sqlq = $@"
                        SELECT *
                        FROM DanhSachKHoaHoc 
                        WHERE DATEDIFF(YEAR, CreatedDate, GETDATE()) <= 10 and isDel = 0 and Disable = 1
                        ORDER BY CreatedDate DESC;
                        ";
                DataTable dt = cnn.CreateDataTable(sqlq, Conds);
                if (cnn.LastError != null || dt == null)
                {
                    model.status = 0;
                    model.error = new ErrorModel
                    {
                        message = "lỗi dữ liệu"
                    };
                    return model;
                }
                if (dt.Rows.Count == 0)
                {
                    model.status = 1;
                    model.error = new ErrorModel
                    {
                        message = "không có dữ liệu"
                    };
                    return model;
                }

                var data = (from r in dt.AsEnumerable()
                            select new
                            {
                                Id = r["id"],
                                TenKhoaHoc = r["TenKhoaHoc"] != DBNull.Value ? r["TenKhoaHoc"] : "",
                                NamHoc = r["NamHoc"] != DBNull.Value ? r["NamHoc"] : "",
                            }).ToList();

                model.data = data;
                model.status = 1;
                return model;
            }
        }
    }
}
