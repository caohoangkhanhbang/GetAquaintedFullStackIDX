using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SampleCodeAPI.Classes
{
    public class ExportExcelHelper
    {
        public static Stylesheet GenerateStylesheet()
        {
            Stylesheet styleSheet = null;


            Fonts fonts = new Fonts(
                new Font( // Index 0 - default
                    new FontSize() { Val = 14 }


                ),
                new Font( // Index 1 - header
                    new FontSize() { Val = 14 },
                    new Color() { Rgb = "FFFFFF" }

                ),
                 new Font( // Index 2 - body
                    new FontSize() { Val = 16 },
                      new Bold()
                ),
                 new Font( // Index 3 - header BÁO CÁO
                    new FontSize() { Val = 20 },
                      new Bold()
                ),
                 new Font( // Index 4 - header TỪ NGÀY - ĐẾN NGÀY
                    new FontSize() { Val = 11 }
                ),
                 new Font( // Index 5 - header table
                    new FontSize() { Val = 12 },
                    new Bold()
                //new FontName() { Val="" }
                )

            );

            Fills fills = new Fills(
                    new Fill(new PatternFill() { PatternType = PatternValues.None }), // Index 0 - default
                    new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }), // Index 1 - default
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "66666666" } })
                    { PatternType = PatternValues.Solid }),// Index 2 - header
                     new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "6600CCFF" } })
                     { PatternType = PatternValues.Solid })// Index 3 - header table bg
            );

            Borders borders = new Borders(
                    new Border(), // index 0 default
                    new Border( // index 1 black border
                        new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new DiagonalBorder())
                );

            CellFormats cellFormats = new CellFormats(
                    new CellFormat { Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } }, // default                    
                    new CellFormat { FontId = 3, FillId = 0, BorderId = 0, ApplyBorder = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } }, // body
                    new CellFormat { FontId = 1, FillId = 2, BorderId = 1, ApplyFill = true, Alignment = new Alignment { WrapText = true } }, // header
                    new CellFormat { FontId = 1, FillId = 2, BorderId = 1, ApplyFill = true, Alignment = new Alignment { WrapText = true } },
                    new CellFormat { FontId = 4, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center } }, // cho cell
                    new CellFormat { FontId = 4, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Right }, }, // cho số tiền
                    new CellFormat { FontId = 5, FillId = 3, BorderId = 1, ApplyBorder = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } }, // body
                    new CellFormat { FontId = 4, FillId = 0, BorderId = 0, ApplyBorder = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } } // header từ ngày đến ngày

                );



            styleSheet = new Stylesheet(fonts, fills, borders, cellFormats);

            return styleSheet;
        }

        public Cell ConstructCell(string value, CellValues dataType, uint styleIndex = 0)
        {
            return new Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType),
                StyleIndex = styleIndex,
            };
        }
        public static void AddListToSheet(SpreadsheetDocument document, WorksheetPart worksheetPart, List<string> list, string rangeName, string columnLetter = "A")
        {
            SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
            for (int i = 0; i < list.Count; i++)
            {
                Row row;
                if (sheetData.Elements<Row>().Count() > i)
                {
                    row = sheetData.Elements<Row>().ElementAt(i);
                }
                else
                {
                    row = new Row { RowIndex = (uint)(i + 1) };
                    sheetData.Append(row);
                }

                // Create a new cell with the specified column letter
                Cell cell = new Cell { CellReference = $"{columnLetter}{i + 1}", CellValue = new CellValue(list[i]), DataType = CellValues.String };
                row.Append(cell);
            }

            WorkbookPart workbookPart = document.WorkbookPart;
            DefinedNames definedNames = workbookPart.Workbook.GetFirstChild<DefinedNames>();
            if (definedNames == null)
            {
                definedNames = new DefinedNames();
                workbookPart.Workbook.Append(definedNames);
            }

            // Update the reference to use the specified column letter
            string reference = $"HiddenLists!${columnLetter}$1:${columnLetter}${list.Count}";
            definedNames.Append(new DefinedName { Name = rangeName, Text = reference });
        }

        public static string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            if (types.ContainsKey(ext))
                return types[ext];
            return "";
        }
        private static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
        {
            {".txt", "text/plain"},
            {".pdf", "application/pdf"},
            {".doc", "application/vnd.ms-word"},
            {".docx", "application/vnd.ms-word"},
            {".ppt", "application/vnd.ms-powerpoint"},
            {".pptx","application/vnd.openxmlformats-officedocument.presentationml.presentation" },
            {".xls", "application/vnd.ms-excel"},
            {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
            {".png", "image/png"},
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".gif", "image/gif"},
            {".csv", "text/csv"},
            {".rar","application/vnd.rar" },
            {".zip","application/zip" },
            {".7z","application/x-7z-compressed" },
            {".sql","application/sql" }

        };
        }

        public static byte[] GetByteArrayFromImageAsync(IFormFile file)
        {
            using (var target = new MemoryStream())
            {
                file.CopyToAsync(target);
                var fileBytes = target.ToArray();
                // string s = Convert.ToBase64String(fileBytes);
                return fileBytes;
            }
        }

        public Stylesheet GenerateStylesheet03A_CDR()
        {
            Stylesheet styleSheet = null;
            Fonts fonts = new Fonts(
                new Font( // Index 0 - chữ bình thường màu đỏ size 12
                    new FontSize() { Val = 12 },
                    new Color() { Rgb = "FFFF0000" },
                    new FontName() { Val = "Times New Roman" }
                ),
                new Font( // Index 1 - chữ đậm size 16
                    new FontSize() { Val = 16 },
                    new Bold(),
                    new FontName() { Val = "Times New Roman" }
                ),
                new Font( // Index 2 - chữ đậm size 11
                    new FontSize() { Val = 11 },
                    new Bold(),
                    new FontName() { Val = "Times New Roman" }
                ),
                new Font( // Index 3 - chữ đậm size 10
                    new FontSize() { Val = 10 },
                    new Bold(),
                    new FontName() { Val = "Times New Roman" }
                ),
                new Font( // Index 4 - chữ bình thường size 10
                    new FontSize() { Val = 10 },
                    new FontName() { Val = "Times New Roman" }
                )
            );
            Fills fills = new Fills(
                    new Fill(new PatternFill() { PatternType = PatternValues.None }), // Index 0 - default
                    new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }), // Index 1 - default
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "FFDAEEF3" } })
                    { PatternType = PatternValues.Solid }),// Index 2 - xanh nhạt (daeef3)
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "FF9BBB59" } })
                    { PatternType = PatternValues.Solid }),// Index 3 - xanh lá (9bbb59)
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "FF92CDDC" } })
                    { PatternType = PatternValues.Solid }),// Index 4 - xanh dương nhạt (92cddc)
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "FFFFC000" } })
                    { PatternType = PatternValues.Solid }),// Index 5 - vàng cam (ffc000)
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "FFD8E4BC" } })
                    { PatternType = PatternValues.Solid }),// Index 6 - xanh lá nhạt (d8e4bc)
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "FFEBF1DE" } })
                    { PatternType = PatternValues.Solid })// Index 7 - xanh lá rất nhạt (ebf1de)
                );

            Borders borders = new Borders(
                    new Border(), // index 0 default
                    new Border( // index 1 black border
                        new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new DiagonalBorder())
                );

            // Fonts:
            // Index 0 - chữ bình thường màu đỏ size 12
            // Index 1 - chữ đậm size 16
            // Index 2 - chữ đậm size 11
            // Index 3 - chữ đậm size 10
            // Index 4 - chữ bình thường size 10

            // Fills:
            // Index 0 - None (default)
            // Index 1 - Gray125 (default)
            // Index 2 - DAEEF3 (xanh nhạt)
            // Index 3 - 9BBB59 (xanh lá)
            // Index 4 - 92CDDC (xanh dương nhạt)
            // Index 5 - FFC000 (vàng cam)
            // Index 6 - D8E4BC (xanh lá nhạt)
            // Index 7 - EBF1DE (xanh lá rất nhạt)

            CellFormats cellFormats = new CellFormats(
                    // Index 0 - default
                    new CellFormat { Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                    // Index 1 - body (chữ thường size 10, no border)
                    new CellFormat { FontId = 4, FillId = 0, BorderId = 0, ApplyBorder = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                    // Index 2 - in đậm size 16 (tiêu đề row 5)
                    new CellFormat { FontId = 1, FillId = 0, BorderId = 0, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                    // Index 3 - in đậm size 11 (row 6, 7, 8)
                    new CellFormat { FontId = 2, FillId = 0, BorderId = 0, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Left } },
                    // Index 4 - in đậm size 10, BorderId = 1
                    new CellFormat { FontId = 3, FillId = 0, BorderId = 1, ApplyBorder = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                    // Index 5 - in đậm size 10, BorderId = 1, màu 92CDDC (xanh dương nhạt - header row 10)
                    new CellFormat { FontId = 3, FillId = 4, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                    // Index 6 - in đậm size 10, BorderId = 1, màu D8E4BC (xanh lá nhạt - cột Z-AB data)
                    new CellFormat { FontId = 3, FillId = 6, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                    // Index 7 - in đậm size 10, BorderId = 1, màu 92CDDC (duplicate)
                    new CellFormat { FontId = 3, FillId = 4, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                    // Index 8 - chữ thường size 10, BorderId = 1, màu EBF1DE (xanh lá rất nhạt - cột F-U data)
                    new CellFormat { FontId = 4, FillId = 7, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                    // Index 9 - chữ thường size 10, BorderId = 1, màu FFC000 (vàng cam - cột V, AF header & data)
                    new CellFormat { FontId = 4, FillId = 5, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                    // Index 10 - chữ đỏ size 12, nền 9BBB59, text xoay 90 độ từ dưới lên, sát đáy (cột Z, AA, AB header row 1-9)
                    new CellFormat { FontId = 0, FillId = 3, BorderId = 0, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = false, Vertical = VerticalAlignmentValues.Bottom, Horizontal = HorizontalAlignmentValues.Center, TextRotation = 90U } },
                    // Index 11 - chữ thường size 10, BorderId = 1, center (data cells mặc định)
                    new CellFormat { FontId = 4, FillId = 0, BorderId = 1, ApplyBorder = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                    // Index 12 - chữ thường size 10, BorderId = 1, canh trái (cột C-D, AG)
                    new CellFormat { FontId = 4, FillId = 0, BorderId = 1, ApplyBorder = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Left } },

                    new CellFormat { FontId = 3, FillId = 2, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                    new CellFormat { FontId = 4, FillId = 0, BorderId = 0, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Bottom, Horizontal = HorizontalAlignmentValues.Center, TextRotation = 90U } }
                );

            styleSheet = new Stylesheet(fonts, fills, borders, cellFormats);

            return styleSheet;
        }

        public Stylesheet GenerateStylesheetCDR()
        {
            Stylesheet styleSheet = null;
            Fonts fonts = new Fonts(
                new Font( // 0
                    new FontSize() { Val = 11 },
                    new FontName() { Val = "Times New Roman" }
                ),
                new Font( // 1
                    new FontSize() { Val = 12 },
                    new FontName() { Val = "Times New Roman" },
                    new Bold()
                ),
                new Font( // 2
                    new FontSize() { Val = 11 },
                    new FontName() { Val = "Times New Roman" },
                    new Bold()
                ),
                new Font( // 3
                    new FontSize() { Val = 14 },
                    new FontName() { Val = "Times New Roman" },
                    new Bold()
                ),
                new Font( // 4
                    new FontSize() { Val = 12 },
                    new FontName() { Val = "Times New Roman" },
                    new Bold(),
                    new Color() { Rgb = "FFFF0000" }
                ),
                new Font( // 5
                    new FontSize() { Val = 12 },
                    new FontName() { Val = "Times New Roman" },
                    new Italic(),
                    new Bold(),
                    new Color() { Rgb = "FFFF0000" }
                ),
                new Font( // 6
                    new FontSize() { Val = 11 },
                    new FontName() { Val = "Times New Roman" },
                    new Italic(),
                    new Bold()
                ),
                new Font( // 7
                    new FontSize() { Val = 11 },
                    new FontName() { Val = "Times New Roman" },
                    new Color() { Rgb = "FFFF0000" }
                )
            );
            Fills fills = new Fills(
                new Fill(new PatternFill() { PatternType = PatternValues.None }), // 0 default
                new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "FFB7DEE8" } })
                { PatternType = PatternValues.Solid }),// 1 #B7DEE8 (xanh nhạt cho header CDR)
                new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "FFD9D9D9" } })
                { PatternType = PatternValues.Solid }),// 2 #D9D9D9 (xám nhạt)
                new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "FFEBF1DE" } })
                { PatternType = PatternValues.Solid }),// 3 #EBF1DE (xanh lá rất nhạt)
                new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "FFB7DEE8" } })
                { PatternType = PatternValues.Solid }),// 4 #B7DEE8 (xanh nhạt cho header CDR)
                new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "FFE4DFEC" } })
                { PatternType = PatternValues.Solid })// 5 #E4DFEC (tím nhạt cho header CDR)
            );

            Borders borders = new Borders(
                new Border(), // 0
                new Border( // 1
                    new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.DashDotDot },
                    new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.DashDotDot },
                    new DiagonalBorder()),
                new Border( // 2
                    new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.DashDotDot },
                    new DiagonalBorder()),
                new Border( // 3
                    new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.DashDotDot },
                    new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new DiagonalBorder()),
                new Border( // 4 - toàn bộ Thin (solid)
                    new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new DiagonalBorder()),
                new Border( // 5
                    new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.None },
                    new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.DashDotDot },
                    new DiagonalBorder()),
                new Border( // 6
                    new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.None },
                    new DiagonalBorder())
            );

            CellFormats cellFormats = new CellFormats(
                // 0 default
                new CellFormat { FontId = 0, FillId = 0, BorderId = 0, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = false, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                // 1 default in table boder
                new CellFormat { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                // 2 top thin - size 12
                new CellFormat { FontId = 1, FillId = 4, BorderId = 2, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                // 3 default - size 12
                new CellFormat { FontId = 1, FillId = 4, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                // 4 default - size 11
                new CellFormat { FontId = 0, FillId = 4, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                // 5 default - size 11 - bold
                new CellFormat { FontId = 2, FillId = 4, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                // 6 default - size 11 - fill none
                new CellFormat { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                // 7 default - size 11 - fill #D9D9D9 (xám nhạt) - border solid
                new CellFormat { FontId = 0, FillId = 2, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                // 8 default - size 11 - fill #EBF1DE (xanh lá rất nhạt) - border solid
                new CellFormat { FontId = 0, FillId = 3, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                // 9 top thin - bold - size 11
                new CellFormat { FontId = 2, FillId = 4, BorderId = 2, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                // 10 default - bold - size 11
                new CellFormat { FontId = 2, FillId = 0, BorderId = 0, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = false, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Left } },
                // 11 default - size 11 - fill none - left
                new CellFormat { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Left } },
                // 12 default - size 14 - fill none - center
                new CellFormat { FontId = 3, FillId = 0, BorderId = 0, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                // 13 default - size 12 - fill none - center
                new CellFormat { FontId = 1, FillId = 0, BorderId = 0, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = false, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Left } },
                // 14 top thin un bottom - bold - size 11 - color xanh lá nhạt
                new CellFormat { FontId = 2, FillId = 3, BorderId = 6, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                // 15 un top - bold - size 11 - color xanh lá nhạt
                new CellFormat { FontId = 2, FillId = 3, BorderId = 5, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                // 16 default - size 11 - fill (tím nhạt) - border solid
                new CellFormat { FontId = 0, FillId = 5, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                // 17 default - size 11 - fill (tím nhạt) - border solid - bold
                new CellFormat { FontId = 2, FillId = 5, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                // 18 default - size 11 - fill (tím nhạt) - top border solid - bold
                new CellFormat { FontId = 2, FillId = 5, BorderId = 2, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                // 19 default - size 11 - fill #EBF1DE (xanh lá rất nhạt) - border solid
                new CellFormat { FontId = 2, FillId = 3, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                // 20 default - size 12 - fill none - center
                new CellFormat { FontId = 4, FillId = 0, BorderId = 0, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = false, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Left } },
                // 21 default - size 11 - fill none - left
                new CellFormat { FontId = 0, FillId = 5, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Left } },
                // 22 default - size 12 - fill none - right - italic
                new CellFormat { FontId = 6, FillId = 5, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Right } },
                // 23 default - size 12 - fill none - center - italic - red
                new CellFormat { FontId = 5, FillId = 5, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                // 24 default - size 11 - fill (tím nhạt) - border solid
                new CellFormat { FontId = 7, FillId = 5, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } },
                // 25 default - size 11 - fill none - left
                new CellFormat { FontId = 7, FillId = 5, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Left } },
                // 26 default - size 11 - fill #EBF1DE (xanh lá rất nhạt) - border solid
                new CellFormat { FontId = 7, FillId = 3, BorderId = 1, ApplyBorder = true, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } }
            );

            styleSheet = new Stylesheet(fonts, fills, borders, cellFormats);

            return styleSheet;
        }
    }
}
