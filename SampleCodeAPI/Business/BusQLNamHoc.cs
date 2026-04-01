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

        public static async Task<object> GetListNH(QueryParams query, string connect)
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

                var r = dt.Rows[0];

                var data = new
                {
                    Header = new
                    {
                        id = r["id"] != DBNull.Value ? int.Parse(r["id"].ToString()) : (int?)null,
                        NamHoc = r["NamHoc"] != DBNull.Value ? int.Parse(r["NamHoc"].ToString()) : (int?)null,
                        NienHoc = !String.IsNullOrEmpty(r["NienHoc"].ToString()) ? r["NienHoc"].ToString() : "",
                        Disable = r["Disable"] != DBNull.Value ? Boolean.Parse(r["Disable"].ToString()) : false,
                        CreatedBy = !String.IsNullOrEmpty(r["CreatedBy"].ToString()) ? r["CreatedBy"].ToString() : "",
                        CreatedDate = r["CreatedDate"] != DBNull.Value ? DateTime.Parse(r["CreatedDate"].ToString()) : (DateTime?)null,
                    },
                    Rows = new
                    {
                        id = r["id"] != DBNull.Value ? int.Parse(r["id"].ToString()) : (int?)null,
                        NamHoc = r["NamHoc"] != DBNull.Value ? int.Parse(r["NamHoc"].ToString()) : (int?)null,
                        NienHoc = !String.IsNullOrEmpty(r["NienHoc"].ToString()) ? r["NienHoc"].ToString() : "",
                        Disable = r["Disable"] != DBNull.Value ? Boolean.Parse(r["Disable"].ToString()) : false,
                        CreatedBy = !String.IsNullOrEmpty(r["CreatedBy"].ToString()) ? r["CreatedBy"].ToString() : "",
                        CreatedDate = r["CreatedDate"] != DBNull.Value ? DateTime.Parse(r["CreatedDate"].ToString()) : (DateTime?)null,
                    }
                };

                //var data = (from r in dt.AsEnumerable()
                //            select new
                //            {
                //                    id = r["id"] != DBNull.Value ? int.Parse(r["id"].ToString()) : (int?)null,
                //                    NamHoc = r["NamHoc"] != DBNull.Value ? int.Parse(r["NamHoc"].ToString()) : (int?)null,
                //                    NienHoc = !String.IsNullOrEmpty(r["NienHoc"].ToString()) ? r["NienHoc"].ToString() : "",
                //                    Disable = r["Disable"] != DBNull.Value ? Boolean.Parse(r["Disable"].ToString()) : false,
                //                    CreatedBy = !String.IsNullOrEmpty(r["CreatedBy"].ToString()) ? r["CreatedBy"].ToString() : "",
                //                    CreatedDate = r["CreatedDate"] != DBNull.Value ? DateTime.Parse(r["CreatedDate"].ToString()) : (DateTime?)null,
                                
                //            }).ToList();


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
            var response = await GetListNH(query, connect) as BaseModel<object>;

            if (response == null || response.status != 1 || response.data == null)
                throw new Exception(response?.error?.message ?? "Không lấy được dữ liệu");

            // Parse dữ liệu từ response
            dynamic data = response.data;
            var header = data.Header;
            var ros = data.Rows;
            var rows = data.Rows as IEnumerable<object>;

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

                // Tạo header (rows 1-10) với dữ liệu từ GetList
                BuildExcelHeader(sheetData, mergeCells, null);

                //Thêm code ghi dữ liệu

                // Insert logo
                var drawingsPart = InsertLogo(worksheetPart, mergeCells);

                // Setup worksheet
                var worksheet = new Worksheet();
                worksheet.Append(CreateColumns());
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

        #region Code hỗ trợ xuất excel

        private static void BuildExcelHeader(SheetData sheetData, MergeCells mergeCells, dynamic? headerData = null)
        {
            // Row 3: Tạo dòng 3
            var row3 = new Row { RowIndex = 3, Height = 20, CustomHeight = true };
            row3.AppendChild(new Cell { CellReference = "A3", CellValue = new CellValue("DANH SÁCH NĂM HỌC"), DataType = CellValues.String, StyleIndex = 2U });
            sheetData.AppendChild(row3);
            for (int i = 1; i <= 10; i++)
            {
                row3.AppendChild(new Cell { CellReference = $"{GetColumnName(i)}3", StyleIndex = 2U });
            }
            sheetData.AppendChild(new Row { RowIndex = 4, Height = 20, CustomHeight = true });

            // Row 5: Title
            var row5 = new Row { RowIndex = 5, Height = 30, CustomHeight = true };
            var cellValues = new Dictionary<string, (string text, uint styleIndex)>
            {
                { "A", ("STT", 4U) },
                { "B", ("Năm học", 4U) },
                { "C", ("Niên học", 4U) },
                { "D", ("Người tạo", 4U) },
                { "E", ("Ngày tạo", 4U) },
            };
            for (int i = 0; i < 5; i++)
            {
                string colName = GetColumnName(i);
                if (cellValues.TryGetValue(colName, out var val))
                    row5.AppendChild(new Cell { CellReference = $"{colName}5", CellValue = new CellValue(val.text), DataType = CellValues.String, StyleIndex = val.styleIndex });
                else
                    row5.AppendChild(new Cell { CellReference = $"{colName}5", StyleIndex = 5U });
            }
            sheetData.AppendChild(row5);
            mergeCells.Append(new MergeCell { Reference = "A3:E3" });
        }

        //Hàm render ra tên cột Excel từ số cột (0 -> A, 1 -> B,... 25 -> Z, 26 -> AA,...)
        private static string GetColumnName(int columnNumber)
        {
            string columnName = "";
            int num = columnNumber;
            while (num >= 0)
            {
                columnName = (char)('A' + (num % 26)) + columnName;
                num = num / 26 - 1;
            }
            return columnName;
        }

        private static Columns CreateColumns()
        {
            return new Columns(
                new Column { Min = 1, Max = 1, Width = 5, CustomWidth = true },
                new Column { Min = 2, Max = 2, Width = 12, CustomWidth = true },
                new Column { Min = 3, Max = 4, Width = 18, CustomWidth = true },
                new Column { Min = 5, Max = 5, Width = 12, CustomWidth = true },
                new Column { Min = 6, Max = 26, Width = 6, CustomWidth = true },
                new Column { Min = 27, Max = 30, Width = 6, CustomWidth = true }
            );
        }

        private static DrawingsPart? InsertLogo(WorksheetPart worksheetPart, MergeCells mergeCells)
        {
            try
            {
                string logoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Common", "Images", "logo.excel.png");
                if (!File.Exists(logoPath)) logoPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Common", "Images", "logo.excel.png");
                if (!File.Exists(logoPath)) return null;

                var drawingsPart = worksheetPart.AddNewPart<DrawingsPart>();
                var worksheetDrawing = new DocumentFormat.OpenXml.Drawing.Spreadsheet.WorksheetDrawing();
                var imagePart = drawingsPart.AddImagePart(ImagePartType.Png);
                using (var fileStream = new FileStream(logoPath, FileMode.Open, FileAccess.Read)) imagePart.FeedData(fileStream);

                var twoCellAnchor = new TwoCellAnchor(
                    new DocumentFormat.OpenXml.Drawing.Spreadsheet.FromMarker(new ColumnId("0"), new ColumnOffset("0"), new RowId("0"), new RowOffset("0")),
                    new DocumentFormat.OpenXml.Drawing.Spreadsheet.ToMarker(new ColumnId("3"), new ColumnOffset("0"), new RowId("4"), new RowOffset("0")),
                    new DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture(
                        new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureProperties(
                            new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualDrawingProperties { Id = 1U, Name = "Logo" },
                            new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureDrawingProperties(new A.PictureLocks() { NoChangeAspect = true })),
                        new DocumentFormat.OpenXml.Drawing.Spreadsheet.BlipFill(new Blip { Embed = drawingsPart.GetIdOfPart(imagePart) }, new Stretch(new FillRectangle())),
                        new DocumentFormat.OpenXml.Drawing.Spreadsheet.ShapeProperties(
                            new A.Transform2D(new A.Offset() { X = 0, Y = 0 }, new A.Extents() { Cx = 20923250L, Cy = 4318000L }),
                            new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle })),
                    new ClientData())
                { EditAs = EditAsValues.OneCell };

                worksheetDrawing.Append(twoCellAnchor);
                drawingsPart.WorksheetDrawing = worksheetDrawing;
                mergeCells.Append(new MergeCell { Reference = "A1:C4" });
                return drawingsPart;
            }
            catch
            {
                return null;
            }
        }
        #endregion Hết Code hỗ trợ xuất excel

    }
}
