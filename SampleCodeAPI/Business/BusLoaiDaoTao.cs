using DpsLibs.Data;
using SampleCodeAPI.Model;
using System.Collections;
using System.Data;

namespace SampleCodeAPI.Business
{
    public class BusLoaiDaoTao
    {
        public static async Task<object> GetList(QueryParams query, string connect)
        {
            BaseModel<object> model = new BaseModel<object>();
            PageModel pageModel = new PageModel();
            using (DpsConnection cnn = new DpsConnection(connect))
            {

                SqlConditions Conds = new SqlConditions();
                string sqlq = "", orderByStr = " TenLoaiDT ", whereStr = " Isdel = 0 ";
                Dictionary<string, string> sortableFields = new Dictionary<string, string>
                {
                    { "MaLoaiDT", "MaLoaiDT"},
                    { "TenTiengAnh", "TenTiengAnh"},
                    { "TenLoaiDaoTao", "TenLoaiDaoTao"},
                };

                if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                {
                    orderByStr = sortableFields[query.sortField] + ("desc".Equals(query.sortOrder) ? " desc" : " asc");
                }
                if (!string.IsNullOrEmpty(query.filter["keyword"]))
                {
                    whereStr += " and (MaLoaiDT like @kw or TenLoaiDT like @kw or TenTiengAnh like @kw)";
                    Conds.Add("kw", "%" + query.filter["keyword"] + "%");
                }
                sqlq = $@"select count(*) AS tong from (select * from LoaiDaoTao
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
                        sqlq = $@"  select LoaiDaoTao.* from LoaiDaoTao
                                  where {whereStr} order by {orderByStr} 
                                  OFFSET @firstRecord ROWS FETCH NEXT @record ROWS ONLY";
                    }
                    else if (query.page == 1)
                    {
                        sqlq = $@"  select top(@record) LoaiDaoTao.* from LoaiDaoTao
                                  where {whereStr} order by {orderByStr} ";

                    }
                    Conds.Add("firstRecord", (query.page - 1) * query.record);
                    Conds.Add("record", query.record);

                }
                dt = cnn.CreateDataTable(sqlq, Conds);

                var data = (from r in dt.AsEnumerable()
                            select new
                            {
                                //RowId = r["RowId"],
                                //Code = r["Code"],
                                id = r["id"].ToString(),
                                MaLoaiDT = r["MaLoaiDT"].ToString(),
                                TenLoaiDT = r["TenLoaiDT"].ToString(),
                                TenTiengAnh = !String.IsNullOrEmpty(r["TenTiengAnh"].ToString()) ? r["TenTiengAnh"].ToString() : "",
                                NoiDung = r["NoiDung"].ToString(),
                                SoThuTu = r["SoThuTu"].ToString(),                                
                                GhiChu = !String.IsNullOrEmpty(r["GhiChu"].ToString()) ? r["GhiChu"].ToString() : "",
                                NguoiTao = r["NguoiTao"].ToString(),
                                NgayTao = r["NgayTao"].ToString(),
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
                sqlq = $@" select * from LoaiDaoTao
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
                                id = r["id"].ToString(),
                                MaLoaiDT = r["MaLoaiDT"].ToString(),
                                TenLoaiDT = r["TenLoaiDT"].ToString(),
                                TenTiengAnh = !String.IsNullOrEmpty(r["TenTiengAnh"].ToString()) ? r["TenTiengAnh"].ToString() : "",
                                NoiDung = r["NoiDung"].ToString(),
                                SoThuTu = r["SoThuTu"].ToString(),
                                GhiChu = !String.IsNullOrEmpty(r["GhiChu"].ToString()) ? r["GhiChu"].ToString() : "",
                                NguoiTao = r["NguoiTao"].ToString(),
                                NgayTao = r["NgayTao"].ToString(),
                            }).FirstOrDefault();

                model.data = data;
                model.status = 1;
                return model;
            }
        }
        public static async Task<BaseModel<object>> Insert(LoaiDaoTaoModel data, string connect, UserJWT loginData)
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

            if (!CheckTrungTen(connect, data.MaLoaiDT.ToString()))
            {
                model.status = 0;
                model.error = new ErrorModel
                {
                    message = "Mã loại đào tạo không được trùng" //_954
                };
                return model;
            }

            using (DpsConnection cnn = new DpsConnection(connect))
            {
                val.Add("MaLoaiDT", data.MaLoaiDT);
                val.Add("TenLoaiDT", data.TenLoaiDT);
                val.Add("TenTiengAnh", data.TenTiengAnh);
                val.Add("NoiDung", data.NoiDung);
                val.Add("SoThuTu", data.SoThuTu);
                val.Add("GhiChu", data.GhiChu);
                val.Add("IsDel", data.IsDel);
                val.Add("NguoiTao", loginData.customdata.jeeAccount.customerID);
                val.Add("NgayTao", DateTime.UtcNow);

                if (cnn.Insert(val, "LoaiDaoTao") == 1)
                {
                    //int id = Convert.ToInt32(cnn.ExecuteScalar("SELECT IDENT_CURRENT ('LoaiDaoTao') AS Current_Identity;  ").ToString());
                    //data.RowId = id;

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
        public static async Task<BaseModel<object>> Update(LoaiDaoTaoModel data, string connect, UserJWT loginData)
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

            if (!CheckTrungTen(connect, data.MaLoaiDT, data.id.ToString()))
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
                val.Add("MaLoaiDT", data.MaLoaiDT);
                val.Add("TenLoaiDT", data.TenLoaiDT);
                val.Add("TenTiengAnh", data.TenTiengAnh);
                val.Add("NoiDung", data.NoiDung);
                val.Add("SoThuTu", data.SoThuTu);
                val.Add("GhiChu", data.GhiChu);

                if (cnn.Update(val, new SqlConditions { { "id", data.id } }, "LoaiDaoTao") == 1)
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
                val.Add("IsDel", 1);
                //val.Add("DeletedBy", loginData.customdata.jeeAccount.userID);
                //val.Add("DeletedDate", DateTime.UtcNow);

                if (cnn.Update(val, new SqlConditions { { "id", id } }, "LoaiDaoTao") == 1)
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
                string sql = "select * from LoaiDaoTao where TenLoaiDT = @Code  and IsDel=0";
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
