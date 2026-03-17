using DpsLibs.Data;
using SampleCodeAPI.Model;
using System.Data;
using System.Data.SqlClient;

namespace SampleCodeAPI.Business
{
    public class BusQLNamHoc
    {
        public static async Task<object> GetList(QueryParams query, string connect)
        {
            BaseModel<object> model = new BaseModel<object>();
            PageModel pageModel = new PageModel();
            using (DpsConnection cnn = new DpsConnection(connect))
            {

                SqlConditions Conds = new SqlConditions();
                string sqlq = "", orderByStr = " Title ", whereStr = " Isdel = 0 ";
                Dictionary<string, string> sortableFields = new Dictionary<string, string>
                {
                    { "RowId", "RowId"},
                    { "Code", "Code"},
                    { "Title", "Title"},
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
                sqlq = $@"select count(*) AS tong from (select * from DM_BacDaoTao
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
                        sqlq = $@"  select DM_BacDaoTao.* from DM_BacDaoTao
                                  where {whereStr} order by {orderByStr} 
                                  OFFSET @firstRecord ROWS FETCH NEXT @record ROWS ONLY";
                    }
                    else if (query.page == 1)
                    {
                        sqlq = $@"  select top(@record) DM_BacDaoTao.* from DM_BacDaoTao
                                  where {whereStr} order by {orderByStr} ";

                    }
                    Conds.Add("firstRecord", (query.page - 1) * query.record);
                    Conds.Add("record", query.record);

                }
                dt = cnn.CreateDataTable(sqlq, Conds);

                var data = (from r in dt.AsEnumerable()
                            select new
                            {
                                RowId = r["RowId"],
                                Code = r["Code"],
                                Title = r["Title"],
                                TenTiengAnh = !String.IsNullOrEmpty(r["TenTiengAnh"].ToString()) ? r["TenTiengAnh"].ToString() : "",
                                NoiDung = r["NoiDung"],
                                GhiChu = !String.IsNullOrEmpty(r["GhiChu"].ToString()) ? r["GhiChu"].ToString() : "",
                                HinhThucDaoTao = r["HinhThucDaoTao"],
                                SoThuTu = r["SoThuTu"],
                                FullName = "",
                                CreatedDate = ((DateTime)r["CreatedDate"]).ToString("u"),
                            }).ToList();

                model.data = data;
                model.status = 1;
                model.page = pageModel;
                return model;
            }
        }

        //public static async Task<object> GetList1(QueryParams query, string connect)
        //{
        //    List<NamHocModel> lst = new List<NamHocModel>();
        //    using(SqlConnection con = new SqlConnection(connect))
        //    {
        //        string sqlq = "select * from DM_NamHoc where Isdel = 0";
        //        SqlCommand cmd = new SqlCommand(sqlq, con);
        //        cmd.CommandType = CommandType.Text;
        //        con.Open();
        //        SqlDataReader dr = cmd.ExecuteReader();
        //        if (dr.HasRows)
        //        {
        //            while (dr.Read())
        //            {
        //                NamHocModel model = new NamHocModel
        //                {
        //                    id = int.Parse(dr["id"].ToString()),
        //                    STT = int.Parse(dr["STT"].ToString()),
        //                    NamHoc = int.Parse(dr["NamHoc"].ToString()),
        //                    NienHoc = dr["NienHoc"].ToString(),
        //                    HienThi = bool.Parse(dr["HienThi"].ToString()),
        //                    NguoiTao = dr["NguoiTao"].ToString(),
        //                    NgayTao = DateTime.Parse(dr["NgayTao"].ToString())
        //                };
        //                lst.Add(model);
        //            }
        //        }
        //    }
        //    return lst;
        //}
    }
}
