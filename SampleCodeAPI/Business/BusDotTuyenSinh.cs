using DpsLibs.Data;
using SampleCodeAPI.Model;
using System.Collections;
using System.Data;

namespace SampleCodeAPI.Business
{
    public class BusDotTuyenSinh
    {
        public static async Task<object> GetList(QueryParams query, string connect)
        {
            BaseModel<object> model = new BaseModel<object>();
            PageModel pageModel = new PageModel();
            using (DpsConnection cnn = new DpsConnection(connect))
            {

                SqlConditions Conds = new SqlConditions();
                string sqlq = "", orderByStr = " TenDotTS ", whereStr = " dts.IsDel = 0 ";
                Dictionary<string, string> sortableFields = new Dictionary<string, string>
                {
                    { "TenDotTS", "TenDotTS"},
                    { "Dot", "Dot"},
                    { "NamHoc", "NamHoc"},
                    { "KhoaHoc", "KhoaHoc"}
                };

                if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                {
                    orderByStr = sortableFields[query.sortField] + ("desc".Equals(query.sortOrder) ? " desc" : " asc");
                }
                if (!string.IsNullOrEmpty(query.filter["keyword"]))
                {
                    whereStr += " and (Code like @kw or Title like @kw)";
                    Conds.Add("kw", "%" + query.filter["keyword"] + "%");
                }
                sqlq = $@"select count(*) AS tong from (select * from DotTuyenSinh dts
                                  where {whereStr} ) as a";
                DataTable dt = cnn.CreateDataTable(sqlq, Conds);
                var total = int.Parse(dt.Rows[0]["tong"].ToString());
                if (cnn.LastError != null || dt == null)
                {
                    model.status = 0;
                    model.error = new ErrorModel
                    {
                        message = "Không có dữ liệu"
                    };
                    return model;
                }
                if (total == 0)
                {
                    model.status = 0;
                    model.error = new ErrorModel
                    {
                        message = "Không có dữ liệu"
                    };
                    return model;
                }
                pageModel.Total = total;
                pageModel.AllPage = (int)Math.Ceiling(total / (decimal)query.record);
                pageModel.Size = query.record;
                pageModel.Page = query.page;

                if (!query.more)
                {

                    if (query.page > 1)
                    {
                        sqlq = $@"  select kh.TenKhoaHoc, nh.NamHoc as TenNamHoc, dts.* from DotTuyenSinh dts inner join DanhSachKhoaHoc kh on dts.KhoaHoc = kh.id inner join DanhSachNamHoc nh on dts.NamHoc = nh.id
                                  where {whereStr} order by {orderByStr} 
                                  OFFSET @firstRecord ROWS FETCH NEXT @record ROWS ONLY";
                    }
                    else if (query.page == 1)
                    {
                        sqlq = $@"  select top(@record) kh.TenKhoaHoc, nh.NamHoc as TenNamHoc, dts.* from DotTuyenSinh dts inner join DanhSachKhoaHoc kh on dts.KhoaHoc = kh.id inner join DanhSachNamHoc nh on dts.NamHoc = nh.id
                                  where {whereStr} order by {orderByStr} ";

                    }
                    Conds.Add("firstRecord", (query.page - 1) * query.record);
                    Conds.Add("record", query.record);

                }
                dt = cnn.CreateDataTable(sqlq, Conds);

                var data = (from r in dt.AsEnumerable()
                            select new
                            {
                                Id = r["Id"] != DBNull.Value ? Convert.ToInt32(r["Id"]) : 0,

                                NamHoc = r["NamHoc"] != DBNull.Value ? Convert.ToInt16(r["NamHoc"]) : (short?)null,

                                Dot = r["Dot"] != DBNull.Value ? Convert.ToByte(r["Dot"]) : (byte?)null,

                                TenDotTS = r["TenDotTS"]?.ToString(), // String tự động nhận null nếu r["TenDotTS"] là DBNull

                                KhoaHoc = r["KhoaHoc"] != DBNull.Value ? Convert.ToInt16(r["KhoaHoc"]) : (short?)null,

                                ThoiGianNhanHSTuNgay = r["ThoiGianNhanHSTuNgay"] != DBNull.Value ? Convert.ToDateTime(r["ThoiGianNhanHSTuNgay"]) : (DateTime?)null,

                                ThoiGianNhanHSDenNgay = r["ThoiGianNhanHSDenNgay"] != DBNull.Value ? Convert.ToDateTime(r["ThoiGianNhanHSDenNgay"]) : (DateTime?)null,

                                NgayInGBTT = r["NgayInGBTT"] != DBNull.Value ? Convert.ToDateTime(r["NgayInGBTT"]) : (DateTime?)null,

                                ThoiGianLayHSTuNgay = r["ThoiGianLayHSTuNgay"] != DBNull.Value ? Convert.ToDateTime(r["ThoiGianLayHSTuNgay"]) : (DateTime?)null,

                                ThoiGianLayHSDenNgay = r["ThoiGianLayHSDenNgay"] != DBNull.Value ? Convert.ToDateTime(r["ThoiGianLayHSDenNgay"]) : (DateTime?)null,

                                NgayNhapHocDK = r["NgayNhapHocDK"] != DBNull.Value ? Convert.ToDateTime(r["NgayNhapHocDK"]) : (DateTime?)null,

                                GhiChu = r["GhiChu"]?.ToString() ?? "",

                                CreatedBy = r["CreatedBy"]?.ToString() ?? "",

                                CreatedDate = r["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(r["CreatedDate"]) : (DateTime?)null,

                                HienThi = r["HienThi"] != DBNull.Value ? Convert.ToBoolean(r["HienThi"]) : false,

                                KichHoat = r["KichHoat"] != DBNull.Value ? Convert.ToBoolean(r["KichHoat"]) : false,

                                TenKhoaHoc = r["TenKhoaHoc"]?.ToString() ?? "",

                                TenNamHoc = r["TenNamHoc"]?.ToString() ?? "",

                            }).ToList();

                model.data = data;
                model.status = 1;
                model.page = pageModel;
                return model;
            }
        }
        public static async Task<BaseModel<object>> GetDetail(long id, string connect)
        {
            BaseModel<object> model = new BaseModel<object>();
            PageModel pageModel = new PageModel();
            ErrorModel error = new ErrorModel();
            using (DpsConnection cnn = new DpsConnection(connect))
            {

                SqlConditions Conds = new SqlConditions();
                string sqlq = "";
                sqlq = $@" select * from DotTuyenSinh
                                  where id=@id ";
                Conds.Add("id", id);
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
                        message = "lỗi dữ liệu"
                    };
                    return model;
                }

                dt = cnn.CreateDataTable(sqlq, Conds);

                var data = (from r in dt.AsEnumerable()
                            select new
                            {
                                Id = r["Id"] != DBNull.Value ? Convert.ToInt32(r["Id"]) : 0,

                                NamHoc = r["NamHoc"] != DBNull.Value ? Convert.ToInt16(r["NamHoc"]) : (short?)null,

                                Dot = r["Dot"] != DBNull.Value ? Convert.ToByte(r["Dot"]) : (byte?)null,

                                TenDotTS = r["TenDotTS"]?.ToString(), // String tự động nhận null nếu r["TenDotTS"] là DBNull

                                KhoaHoc = r["KhoaHoc"] != DBNull.Value ? Convert.ToInt16(r["KhoaHoc"]) : (short?)null,

                                ThoiGianNhanHSTuNgay = r["ThoiGianNhanHSTuNgay"] != DBNull.Value ? Convert.ToDateTime(r["ThoiGianNhanHSTuNgay"]) : (DateTime?)null,

                                ThoiGianNhanHSDenNgay = r["ThoiGianNhanHSDenNgay"] != DBNull.Value ? Convert.ToDateTime(r["ThoiGianNhanHSDenNgay"]) : (DateTime?)null,

                                NgayInGBTT = r["NgayInGBTT"] != DBNull.Value ? Convert.ToDateTime(r["NgayInGBTT"]) : (DateTime?)null,

                                ThoiGianLayHSTuNgay = r["ThoiGianLayHSTuNgay"] != DBNull.Value ? Convert.ToDateTime(r["ThoiGianLayHSTuNgay"]) : (DateTime?)null,

                                ThoiGianLayHSDenNgay = r["ThoiGianLayHSDenNgay"] != DBNull.Value ? Convert.ToDateTime(r["ThoiGianLayHSDenNgay"]) : (DateTime?)null,

                                NgayNhapHocDK = r["NgayNhapHocDK"] != DBNull.Value ? Convert.ToDateTime(r["NgayNhapHocDK"]) : (DateTime?)null,

                                GhiChu = r["GhiChu"]?.ToString() ?? "",

                                CreatedBy = r["CreatedBy"]?.ToString() ?? "",

                                CreatedDate = r["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(r["CreatedDate"]) : (DateTime?)null,

                                HienThi = r["HienThi"] != DBNull.Value ? Convert.ToBoolean(r["HienThi"]) : false,

                                KichHoat = r["KichHoat"] != DBNull.Value ? Convert.ToBoolean(r["KichHoat"]) : false,

                            }).FirstOrDefault();

                model.data = data;
                model.status = 1;
                return model;
            }
        }
        public static async Task<BaseModel<object>> Insert(DotTuyenSinhModel data, string connect, UserJWT loginData)
        {
            ErrorModel error = new ErrorModel();
            BaseModel<object> model = new BaseModel<object>();
            BaseModel<string> result_up = new BaseModel<string>();
            Hashtable val = new Hashtable();
            if (data == null)
            {
                model.status = 0;
                model.error = new ErrorModel
                {
                    message = "Lỗi dữ liệu"
                };

                return model;
            }

            if (!CheckTrungTen(connect, data.TenDotTS))
            {
                model.status = 0;
                model.error = new ErrorModel
                {
                    message = "Dữ liệu đã tồn tại" //_954
                };
                return model;
            }

            using (DpsConnection cnn = new DpsConnection(connect))
            {
                val.Add("NamHoc", data.NamHoc);
                val.Add("Dot", data.Dot);
                val.Add("TenDotTS", data.TenDotTS);
                val.Add("KhoaHoc", data.KhoaHoc);
                val.Add("ThoiGianNhanHSTuNgay", (object)data.ThoiGianNhanHSTuNgay??DBNull.Value);
                val.Add("ThoiGianNhanHSDenNgay", (object)data.ThoiGianNhanHSDenNgay??DBNull.Value);
                val.Add("NgayInGBTT", (object)data.NgayInGBTT??DBNull.Value);
                val.Add("ThoiGianLayHSTuNgay", (object)data.ThoiGianLayHSTuNgay??DBNull.Value);
                val.Add("ThoiGianLayHSDenNgay", (object)data.ThoiGianLayHSDenNgay??DBNull.Value);
                val.Add("NgayNhapHocDK", (object)data.NgayNhapHocDK??DBNull.Value);
                val.Add("GhiChu", data.GhiChu);
                val.Add("HienThi", data.HienThi);
                val.Add("KichHoat", data.KichHoat);
                val.Add("CreatedBy", loginData.customdata.jeeAccount.customerID);
                val.Add("CreatedDate", DateTime.UtcNow);
                val.Add("IsDel", false);

                if (cnn.Insert(val, "DotTuyenSinh") == 1)
                {
                    int id = Convert.ToInt32(cnn.ExecuteScalar("SELECT IDENT_CURRENT ('DotTuyenSinh') AS Current_Identity;  ").ToString());
                    data.Id = id;

                    model.status = 1;
                    model.error = new ErrorModel
                    {
                        message = "Thêm mới thành công"
                    };
                }
                else
                {
                    model.status = 0;
                    model.error = new ErrorModel
                    {
                        message = "Thêm mới thất bại"
                    };
                    //Bổ sung ghi log
                    return model;
                }
            }
            //Bổ sung ghi log
            return model;
        }
        public static async Task<BaseModel<object>> Update(DotTuyenSinhModel data, string connect, UserJWT loginData)
        {
            BaseModel<object> model = new BaseModel<object>();
            BaseModel<string> result_up = new BaseModel<string>();
            ErrorModel error = new ErrorModel();
            Hashtable val = new Hashtable();
            if (data == null)
            {
                model.status = 0;
                model.error = new ErrorModel
                {
                    message = "Lỗi dữ liệu"
                };

                return model;
            }

            if (!CheckTrungTen(connect, data.TenDotTS, data.Id.ToString()))
            {
                model.status = 0;
                model.error = new ErrorModel
                {
                    message = "Dữ liệu đã tồn tại"
                };
                return model;
            }
            using (DpsConnection cnn = new DpsConnection(connect))
            {
                val.Add("NamHoc", data.NamHoc);
                val.Add("Dot", data.Dot);
                val.Add("TenDotTS", data.TenDotTS);
                val.Add("KhoaHoc", data.KhoaHoc);
                val.Add("ThoiGianNhanHSTuNgay", (object)data.ThoiGianNhanHSTuNgay ?? DBNull.Value);
                val.Add("ThoiGianNhanHSDenNgay", (object)data.ThoiGianNhanHSDenNgay ?? DBNull.Value);
                val.Add("NgayInGBTT", (object)data.NgayInGBTT ?? DBNull.Value);
                val.Add("ThoiGianLayHSTuNgay", (object)data.ThoiGianLayHSTuNgay ?? DBNull.Value);
                val.Add("ThoiGianLayHSDenNgay", (object)data.ThoiGianLayHSDenNgay ?? DBNull.Value);
                val.Add("NgayNhapHocDK", (object)data.NgayNhapHocDK ?? DBNull.Value);
                val.Add("GhiChu", (object)data.GhiChu ?? DBNull.Value);
                val.Add("HienThi", data.HienThi);
                val.Add("KichHoat", data.KichHoat);
                val.Add("UpdatedDate", DateTime.UtcNow);
                val.Add("UpdatedBy", loginData.customdata.jeeAccount.customerID);

                if (cnn.Update(val, new SqlConditions { { "id", data.Id } }, "DotTuyenSinh") == 1)
                {
                    model.status = 1;
                    model.error = new ErrorModel
                    {
                        message = "Cập nhật thành công"
                    };
                }
                else
                {
                    model.status = 0;
                    model.error = new ErrorModel
                    {
                        message = "Cập nhật thất bại"
                    };
                    //Bổ sung ghi log
                    return model;
                }
            }
            //Bổ sung ghi log
            return model;
        }
        public static async Task<BaseModel<object>> Delete(long id, string connect, UserJWT loginData)
        {
            BaseModel<object> model = new BaseModel<object>();
            using (DpsConnection cnn = new DpsConnection(connect))
            {
                Hashtable val = new Hashtable();
                val.Add("HienThi", 0);
                val.Add("IsDel", true);
                val.Add("DeletedDate", DateTime.UtcNow);
                val.Add("DeletedBy", loginData.customdata.jeeAccount.customerID);

                if (cnn.Update(val, new SqlConditions { { "id", id } }, "DotTuyenSinh") == 1)
                {
                    model.status = 1;
                    model.error = new ErrorModel
                    {
                        message = "Xóa thành công"
                    };
                    //Bổ sung ghi log
                }
                else
                {
                    model.status = 0;
                    model.error = new ErrorModel
                    {
                        message = "Xóa thất bại"
                    };
                    //Bổ sung ghi log
                    return model;
                }
            }

            return model;
        }
        public static bool CheckTrungTen(string _ConnectionString, string name, string Id = "")
        {
            using (DpsConnection cnn = new DpsConnection(_ConnectionString))
            {
                SqlConditions conds = new SqlConditions();
                string sql = "select * from DotTuyenSinh where TenDotTS = @Code  and HienThi=0";
                conds.Add("Code", name);

                if (!string.IsNullOrEmpty(Id))
                {
                    sql += " and id <> @RowId";
                    conds.Add("RowId", Id);
                }

                var dt = cnn.CreateDataTable(sql, conds);

                if (cnn.LastError == null && dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return false; // có trùng
                    }
                    else
                    {
                        return true; // không bị trùng
                    }
                }

                return false;
            }
        }
    }
}
