using DpsLibs.Data;
using SampleCodeAPI.Model;
using System.Collections;
using System.Data;

namespace SampleCodeAPI.Business
{
    public class BusKhoaHoc
    {
        public static async Task<object> GetList(QueryParams query, string connect)
        {
            BaseModel<object> model = new BaseModel<object>();
            PageModel pageModel = new PageModel();
            using (DpsConnection cnn = new DpsConnection(connect))
            {
                SqlConditions Conds = new SqlConditions();
                string sqlq = "", orderByStr = " TenKhoaHoc ", whereStr = " Disable = 0 ";
                Dictionary<string, string> sortableFields = new Dictionary<string, string>
                {
                    { "TenKhoaHoc", "TenKhoaHoc"},
                    { "NamHoc", "NamHoc"},
                };

                if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                {
                    orderByStr = sortableFields[query.sortField] + ("desc".Equals(query.sortOrder) ? " desc" : " asc");
                }
                if (!string.IsNullOrEmpty(query.filter["keyword"]))
                {
                    whereStr += " and (TenKhoaHoc like @kw or NamHoc like @kw)";
                    Conds.Add("kw", "%" + query.filter["keyword"] + "%");
                }
                sqlq = $@"select count(*) AS tong from (select * from DanhSachKhoaHoc
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
                        sqlq = $@"  select DanhSachKhoaHoc.* from DanhSachKhoaHoc
                                  where {whereStr} order by {orderByStr} 
                                  OFFSET @firstRecord ROWS FETCH NEXT @record ROWS ONLY";
                    }
                    else if (query.page == 1)
                    {
                        sqlq = $@"  select top(@record) DanhSachKhoaHoc.* from DanhSachKhoaHoc
                                  where {whereStr} order by {orderByStr} ";

                    }
                    Conds.Add("firstRecord", (query.page - 1) * query.record);
                    Conds.Add("record", query.record);

                }
                dt = cnn.CreateDataTable(sqlq, Conds);

                var data = (from r in dt.AsEnumerable()
                            select new
                            {
                                id = r["id"] != DBNull.Value ? int.Parse(r["id"].ToString()) : (int?)null,
                                TenKhoaHoc = !String.IsNullOrEmpty(r["TenKhoaHoc"].ToString())? r["TenKhoaHoc"].ToString():"",
                                NamHoc = r["NamHoc"] != DBNull.Value ? int.Parse(r["NamHoc"].ToString()) : (int?)null,
                                CachViet = !String.IsNullOrEmpty(r["CachViet"].ToString())? r["CachViet"].ToString(): "",
                                Disable = r["Disable"] != DBNull.Value ? Boolean.Parse(r["Disable"].ToString()): (bool?)null,
                                CreatedBy = r["CreatedBy"] != DBNull.Value ? r["CreatedBy"].ToString() : "",
                                CreatedDate = r["CreatedDate"] != DBNull.Value ? DateTime.Parse(r["CreatedDate"].ToString()) : (DateTime?)null,                              
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
                sqlq = $@" select * from DanhSachKhoaHoc
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
                                id = r["id"] != DBNull.Value ? int.Parse(r["id"].ToString()) : (int?)null,
                                TenKhoaHoc = !String.IsNullOrEmpty(r["TenKhoaHoc"].ToString()) ? r["TenKhoaHoc"].ToString() : "",
                                NamHoc = r["NamHoc"] != DBNull.Value ? int.Parse(r["NamHoc"].ToString()) : (int?)null,
                                CachViet = !String.IsNullOrEmpty(r["CachViet"].ToString()) ? r["CachViet"].ToString() : "",
                                Disable = r["Disable"] != DBNull.Value ? Boolean.Parse(r["Disable"].ToString()) : (bool?)null,
                                CreatedBy = r["CreatedBy"] != DBNull.Value ? r["CreatedBy"].ToString() : "",
                                CreatedDate = r["CreatedDate"] != DBNull.Value ? DateTime.Parse(r["CreatedDate"].ToString()) : (DateTime?)null,
                            }).FirstOrDefault();

                model.data = data;
                model.status = 1;
                return model;
            }
        }
        public static async Task<BaseModel<object>> Insert(KhoaHocModel data, string connect, UserJWT loginData)
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

            if (!CheckTrungTen(connect, data.TenKhoaHoc.ToString()))
            {
                model.status = 0;
                model.error = new ErrorModel
                {
                    message = "Tên khóa học không được trùng"
                };
                return model;
            }

            using (DpsConnection cnn = new DpsConnection(connect))
            {
                val.Add("TenKhoaHoc", data.TenKhoaHoc);
                val.Add("NamHoc", data.NamHoc);
                val.Add("CachViet", (object)data.CachViet ?? DBNull.Value);
                val.Add("Disable", (object)data.Disable ?? DBNull.Value);
                val.Add("CreatedBy",  loginData.UserName);
                val.Add("CreatedDate", DateTime.UtcNow);

                if (cnn.Insert(val, "DanhSachKhoaHoc") == 1)
                {
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
        public static async Task<BaseModel<object>> Update(KhoaHocModel data, string connect, UserJWT loginData)
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

            if (!CheckTrungTen(connect, data.NamHoc.ToString(), data.id.ToString()))
            {
                model.status = 0;
                model.error = new ErrorModel
                {
                    message = "Mã loại đào tạo không được trùng"
                };
                return model;
            }
            using (DpsConnection cnn = new DpsConnection(connect))
            {
                val.Add("TenKhoaHoc", data.TenKhoaHoc);
                val.Add("NamHoc", data.NamHoc);
                val.Add("CachViet", (object)data.CachViet ?? DBNull.Value);
                val.Add("Disable", (object)data.Disable ?? DBNull.Value);
                val.Add("UpdatedDate", DateTime.UtcNow);
                val.Add("UpdatedBy",  loginData.UserName);

                if (cnn.Update(val, new SqlConditions { { "id", data.id } }, "DanhSachKhoaHoc") == 1)
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
                val.Add("Disable", 1);
                val.Add("DeletedDate", DateTime.UtcNow);
                val.Add("DeletedBy",  loginData.UserName);

                if (cnn.Update(val, new SqlConditions { { "id", id } }, "DanhSachKhoaHoc") == 1)
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
                string sql = "select * from DanhSachKhoaHoc where TenKhoaHoc = @Code  and Disable=0";
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
