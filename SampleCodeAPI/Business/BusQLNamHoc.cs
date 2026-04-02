using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DpsLibs.Data;
using Newtonsoft.Json;
using SampleCodeAPI.Classes;
using SampleCodeAPI.Model;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using A = DocumentFormat.OpenXml.Drawing;

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
                string sqlq = "", orderByStr = " NamHoc ", whereStr = " IsDel = 0 ";
                Dictionary<string, string> sortableFields = new Dictionary<string, string>
                {
                    { "NamHoc", "NamHoc"},
                    { "NienHoc", "NienHoc"},
                };

                if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                {
                    orderByStr = sortableFields[query.sortField] + ("desc".Equals(query.sortOrder) ? " desc" : " asc");
                }
                if (!string.IsNullOrEmpty(query.filter["keyword"]))
                {
                    whereStr += " and (NamHoc like @kw or NienHoc like @kw)";
                    Conds.Add("kw", "%" + query.filter["keyword"] + "%");
                }
                sqlq = $@"select count(*) AS tong from (select * from DanhSachNamHoc
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
                        sqlq = $@"  select DanhSachNamHoc.* from DanhSachNamHoc
                                  where {whereStr} order by {orderByStr} 
                                  OFFSET @firstRecord ROWS FETCH NEXT @record ROWS ONLY";
                    }
                    else if (query.page == 1)
                    {
                        sqlq = $@"  select top(@record) DanhSachNamHoc.* from DanhSachNamHoc
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
                                NamHoc = r["NamHoc"] != DBNull.Value ? int.Parse(r["NamHoc"].ToString()) : (int?)null,
                                NienHoc = !String.IsNullOrEmpty(r["NienHoc"].ToString()) ? r["NienHoc"].ToString() : "",
                                Disable = r["Disable"] != DBNull.Value ? Boolean.Parse(r["Disable"].ToString()):false,
                                CreatedBy = !String.IsNullOrEmpty(r["CreatedBy"].ToString()) ? r["CreatedBy"].ToString() : "",
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
                sqlq = $@" select * from DanhSachNamHoc
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
                                NamHoc = r["NamHoc"] != DBNull.Value ? int.Parse(r["NamHoc"].ToString()) : (int?)null,
                                NienHoc = !String.IsNullOrEmpty(r["NienHoc"].ToString()) ? r["NienHoc"].ToString() : "",
                                Disable = r["Disable"] != DBNull.Value ? Boolean.Parse(r["Disable"].ToString()) : false,
                                CreatedBy = !String.IsNullOrEmpty(r["CreatedBy"].ToString()) ? r["CreatedBy"].ToString() : "",
                                CreatedDate = r["CreatedDate"] != DBNull.Value ? DateTime.Parse(r["CreatedDate"].ToString()) : (DateTime?)null,

                            }).FirstOrDefault();

                model.data = data;
                model.status = 1;
                return model;
            }
        }
        public static async Task<BaseModel<object>> Insert(NamHocModel data, string connect, UserJWT loginData)
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

            if (!CheckTrungTen(connect, data.NamHoc.ToString()))
            {
                model.status = 0;
                model.error = new ErrorModel
                {
                    message = "Năm học không được trùng"
                };
                return model;
            }

            using (DpsConnection cnn = new DpsConnection(connect))
            {
                val.Add("NamHoc", data.NamHoc);
                val.Add("NienHoc", data.NienHoc);
                val.Add("Disable", (object)data.Disable ?? DBNull.Value);
                val.Add("CreatedBy", loginData.UserName);
                val.Add("CreatedDate", DateTime.UtcNow);
                val.Add("IsDel", false);

                if (cnn.Insert(val, "DanhSachNamHoc") == 1)
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
        public static async Task<BaseModel<object>> Update(NamHocModel data, string connect, UserJWT loginData)
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
                    message = "Năm học không được trùng"
                };
                return model;
            }
            using (DpsConnection cnn = new DpsConnection(connect))
            {
                val.Add("NamHoc", data.NamHoc);
                val.Add("NienHoc", data.NienHoc);
                val.Add("Disable", (object)data.Disable ?? DBNull.Value);
                val.Add("UpdatedDate", DateTime.UtcNow);
                val.Add("UpdatedBy", loginData.UserName);
                val.Add("IsDel", false);

                if (cnn.Update(val, new SqlConditions { { "id", data.id } }, "DanhSachNamHoc") == 1)
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
                val.Add("DeletedDate", DateTime.UtcNow);
                val.Add("DeletedBy", loginData.UserName);

                if (cnn.Update(val, new SqlConditions { { "id", id } }, "DanhSachNamHoc") == 1)
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
                string sql = "select * from DanhSachNamHoc where NamHoc = @Code  and IsDel =0";
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

        //Hàm xuất excel
        public static async Task<byte[]> ExportToExcel(UserJWT loginData, QueryParams query, string connect)
        {
            // Lấy tất cả dữ liệu (không phân trang)
            query.more = false;
            var response = await GetList(query, connect) as BaseModel<object>;

            if (response == null || response.status != 1 || response.data == null)
                throw new Exception(response?.error?.message ?? "Không lấy được dữ liệu");

            // Parse dữ liệu từ response
            dynamic data = response.data;

            using var stream = new MemoryStream();
            using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

                var exportExcel = new ExportExcelHelper();
                var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                stylesPart.Stylesheet = exportExcel.GenerateStylesheet03A_CDR();
                stylesPart.Stylesheet.Save();

                var sheetData = new SheetData();
                var mergeCells = new MergeCells();

                var cellValues = new Dictionary<string, (string text, uint styleIndex)>
            {
                { "A", ("STT", 4U) },
                { "B", ("Năm học", 4U) },
                { "C", ("Niên học", 4U) },
                { "D", ("Người tạo", 4U) },
                { "E", ("Ngày tạo", 4U) },
            };

                // Tạo Tiêu đề côt cho excel
                ExcelUtil.BuildExcelHeader(sheetData, mergeCells, cellValues, "Danh Sách Năm Học");

                //Nơi chưa thông tin nội dung chính
                uint rowIndex = 6;
                int stt = 1;
                foreach(dynamic item in data)
                {
                    var dataRow = new Row { RowIndex = rowIndex, Height = 20, CustomHeight = true };
                    dataRow.AppendChild(new Cell { CellReference = $"A{rowIndex}", CellValue = new CellValue(stt), DataType = CellValues.Number, StyleIndex = 11U });
                    dataRow.AppendChild(new Cell { CellReference = $"B{rowIndex}" , CellValue = new CellValue(item.NamHoc?.ToString() ?? ""), DataType = CellValues.String, StyleIndex = 11U });
                    dataRow.AppendChild(new Cell { CellReference = $"C{rowIndex}", CellValue = new CellValue(item.NienHoc?.ToString() ?? ""), DataType = CellValues.String, StyleIndex = 11U });
                    dataRow.AppendChild(new Cell { CellReference = $"D{rowIndex}", CellValue = new CellValue(item.CreatedBy?.ToString() ?? ""), DataType = CellValues.String, StyleIndex = 11U });
                    dataRow.AppendChild(new Cell { CellReference = $"E{rowIndex}", CellValue = new CellValue(item.CreatedDate?.ToString("dd/MM/yyyy") ?? ""), DataType = CellValues.String, StyleIndex = 11U });
                    sheetData.AppendChild(dataRow);
                    stt++;
                    rowIndex++;
                }

                // Insert logo
                var drawingsPart = ExcelUtil.InsertLogo(worksheetPart, mergeCells);

                // Setup worksheet
                var worksheet = new Worksheet();
                worksheet.Append(ExcelUtil.CreateColumns());
                worksheet.Append(sheetData);
                mergeCells.Count = (uint)mergeCells.ChildElements.Count;
                worksheet.Append(mergeCells);
                if (drawingsPart != null)
                    worksheet.Append(new Drawing { Id = worksheetPart.GetIdOfPart(drawingsPart) });

                worksheetPart.Worksheet = worksheet;
                worksheetPart.Worksheet.Save();

                var sheets = document.WorkbookPart!.Workbook.AppendChild(new Sheets());
                sheets.Append(new Sheet { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "DiemQuaTrinh" });
                workbookPart.Workbook.Save();
            }

            return stream.ToArray();
        }
    }
}
