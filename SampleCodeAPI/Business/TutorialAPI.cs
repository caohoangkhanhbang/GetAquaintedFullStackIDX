using Confluent.Kafka;
using DpsLibs.Data;
using Newtonsoft.Json.Linq;
using SampleCodeAPI.Model;
using static Azure.Core.HttpHeader;
using System.Data;
using System.Reflection;
using System.Collections;

namespace SampleCodeAPI.Business
{
    public class TutorialAPI
    {
        public static async Task<object> GetList(QueryParams query, string connect)
        {
            BaseModel<object> model = new BaseModel<object>();
            PageModel pageModel = new PageModel();
            using (DpsConnection cnn = new DpsConnection(connect))
            {

                SqlConditions Conds = new SqlConditions();
                string sqlq = "", orderByStr = " EventID ", whereStr = " Isdel = 0 ";
                Dictionary<string, string> sortableFields = new Dictionary<string, string>
                {
                    { "RowID", "EventID"},
                    { "EventCode", "EventCode"},
                    { "EventName", "EventName"},
                };

                if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                {
                    orderByStr = sortableFields[query.sortField] + ("desc".Equals(query.sortOrder) ? " desc" : " asc");
                }
                if (!string.IsNullOrEmpty(query.filter["keyword"]))
                {
                    whereStr += " and (EventCode like @kw or EventName like @kw)";
                    Conds.Add("kw", "%" + query.filter["keyword"] + "%");
                }
                sqlq = $@" select * from EventSource
                                  where {whereStr} order by {orderByStr} ";
                DataTable dt = cnn.CreateDataTable(sqlq, Conds);
                if (cnn.LastError != null || dt == null)
                {
                    model.status = 0;
                    model.error = new ErrorModel
                    {
                        message = "Không có dữ liệu"
                    };
                    return model;
                }
                if (dt.Rows.Count == 0)
                {
                    model.status = 0;
                    model.error = new ErrorModel
                    {
                        message = "Không có dữ liệu"
                    };
                    return model;
                }
                int total = dt.Rows.Count;
                pageModel.Total = total;
                pageModel.AllPage = (int)Math.Ceiling(total / (decimal)query.record);
                pageModel.Size = query.record;
                pageModel.Page = query.page;
                pageModel.Page = query.page;

                if (!query.more)
                {

                    if (query.page > 1)
                    {
                        sqlq = $@"  select * from EventSource
                                  where {whereStr} order by {orderByStr} 
                                  OFFSET @firstRecord ROWS FETCH NEXT @record ROWS ONLY";
                    }
                    else if (query.page == 1)
                    {
                        sqlq = $@"  select top(@record) * from EventSource
                                  where {whereStr} order by {orderByStr} ";

                    }
                    Conds.Add("firstRecord", (query.page - 1) * query.record);
                    Conds.Add("record", query.record);

                }
                dt = cnn.CreateDataTable(sqlq, Conds);

                var data = (from r in dt.AsEnumerable()
                            select new
                            {
                                RowID = r["EventID"],
                                EventCode = r["EventCode"],
                                EventName = r["EventName"],
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
                sqlq = $@" select * from EventSource
                                  where EventID=@id ";
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
                                RowID = r["EventID"],
                                EventCode = r["EventCode"],
                                EventName = r["EventName"],
                            }).FirstOrDefault();

                model.data = data;
                model.status = 1;
                return model;
            }
        }
        public static async Task<BaseModel<object>> Insert(TutorialModels data, string connect)
        {
            ErrorModel error = new ErrorModel();
            string url_icon = "";
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

            if (!CheckTrungTen(connect, data.EventCode))
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
                val.Add("EventCode", data.EventCode);
                val.Add("EventName", data.EventName);
                val.Add("IsDel", 0);

                if (cnn.Insert(val, "EventSource") == 1)
                {
                    int id = Convert.ToInt32(cnn.ExecuteScalar("SELECT IDENT_CURRENT ('EventSource') AS Current_Identity;  ").ToString());
                    data.RowID = id;

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
        public static async Task<BaseModel<object>> Update(TutorialModels data, string connect)
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

            if (!CheckTrungTen(connect, data.EventCode, data.RowID.ToString()))
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
                string sqlq = "select * from EventSource where EventID=@id";
                DataTable dt = cnn.CreateDataTable(sqlq, new SqlConditions() { { "id", data.RowID } });

                val.Add("EventCode", data.EventCode);
                val.Add("EventName", data.EventName);

                if (cnn.Update(val, new SqlConditions { { "EventID", data.RowID } }, "EventSource") == 1)
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
        public static async Task<BaseModel<object>> Delete(long id, string connect)
        {
            BaseModel<object> model = new BaseModel<object>();
            using (DpsConnection cnn = new DpsConnection(connect))
            {
                Hashtable val = new Hashtable();
                val.Add("IsDel", 1);

                if (cnn.Update(val, new SqlConditions { { "EventID", id } }, "EventSource") == 1)
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
                string sql = "select * from EventSource where EventCode = @EventCode  and IsDel=0";
                conds.Add("EventCode", name);

                if (!string.IsNullOrEmpty(Id))
                {
                    sql += " and EventID <> @EventID";
                    conds.Add("EventID", Id);
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
