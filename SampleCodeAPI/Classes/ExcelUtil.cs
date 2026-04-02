using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using A = DocumentFormat.OpenXml.Drawing;

namespace SampleCodeAPI.Classes
{
    public class ExcelUtil
    {
        #region Code hỗ trợ xuất excel

        public static void BuildExcelHeader(SheetData sheetData, MergeCells mergeCells, Dictionary<string, (string text, uint styleIndex)> cellValues, string TieuDe)
        {
            // Row 3: Nơi chứa tiêu đề chính.
            var row3 = new Row { RowIndex = 3, Height = 20, CustomHeight = true };
            row3.AppendChild(new Cell { CellReference = "A3", CellValue = new CellValue(TieuDe), DataType = CellValues.String, StyleIndex = 2U });
            sheetData.AppendChild(row3);
            for (int i = 1; i <= 10; i++)
            {
                row3.AppendChild(new Cell { CellReference = $"{GetColumnName(i)}3", StyleIndex = 2U });
            }
            sheetData.AppendChild(new Row { RowIndex = 4, Height = 20, CustomHeight = true });

            // Row 5: Nơi chứa tiêu đề cột
            var row5 = new Row { RowIndex = 5, Height = 30, CustomHeight = true };

            int SoCell = cellValues.Count;
            for (int i = 0; i < SoCell; i++)
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
        public static string GetColumnName(int columnNumber)
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

        public static Columns CreateColumns()
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

        public static DrawingsPart? InsertLogo(WorksheetPart worksheetPart, MergeCells mergeCells)
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
