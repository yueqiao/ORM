using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ORM
{
    public class ExcelHelper
    {
        public static void OutPutExcel(DataTable dt, string dirPath, string fileName)
        {
            SaveToFile(RenderToExcel(dt), dirPath, fileName);
        }

        public static DataTable ReadExcelToDataTable(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {

                var workbook = default(IWorkbook);
                if (fileName.IndexOf(".xlsx", StringComparison.OrdinalIgnoreCase) > 0)
                    workbook = new XSSFWorkbook(stream);
                else
                    workbook = new HSSFWorkbook(stream);

                var sheet = workbook.GetSheetAt(workbook.ActiveSheetIndex);
                if (Equals(sheet, null))
                {
                    return null;
                }

                var dt = new DataTable();
                CreateDataColumn(sheet, dt);
                CreateDataTable(sheet, dt);
                return dt;
            }
        }

        private static void CreateDataTable(ISheet sheet, DataTable dt)
        {
            for (var i = 1; i < sheet.LastRowNum; i++)
            {
                var sheetRow = sheet.GetRow(i);
                var dataRow = dt.NewRow();
                for (var j = 0; j < sheetRow.LastCellNum; j++)
                {
                    dataRow[j] = sheetRow.GetCell(j).ToString();
                }

                dt.Rows.Add(dataRow);
            }
        }

        private static void CreateDataColumn(ISheet sheet, DataTable dt)
        {
            var firstRow = sheet.GetRow(0);
            for (var i = 0; i < firstRow.LastCellNum; i++)
            {
                var column = new DataColumn(firstRow.GetCell(i).StringCellValue);
                dt.Columns.Add(column);
            }
        }

        private static void SaveToFile(MemoryStream ms, string dirPath, string fileName)
        {
            if (string.IsNullOrEmpty(dirPath) || string.IsNullOrEmpty(fileName))
            {
                return;
            }

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            using (var fs = new FileStream(string.Format("{0}{1}", dirPath, fileName), FileMode.Create))
            {
                byte[] data = ms.ToArray();
                fs.Write(data, 0, data.Length);
                fs.Flush();

                data = null;
            }
        }

        private static IFont GetFont(IWorkbook workbook,short fontSize)
        {
            var font = workbook.CreateFont();
            font.FontHeightInPoints = fontSize;
            return font;
        }

        private static MemoryStream RenderToExcel(DataTable dt)
        {
            if (Equals(dt, null))
            {
                return null;
            }

            using (var ms = new MemoryStream())
            {
                using (dt)
                {
                    IWorkbook workbook = new HSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet();
                    IRow headerRow = sheet.CreateRow(0);

                    foreach (DataColumn column in dt.Columns)
                    {
                        var cell = headerRow.CreateCell(column.Ordinal);
                        var cellStyle = workbook.CreateCellStyle();
                        cell.SetCellValue(column.Caption);
                        cellStyle.SetFont(GetFont(workbook, 10));
                        cell.CellStyle = cellStyle;
                    }

                    int rowIndex = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        IRow dataRow = sheet.CreateRow(rowIndex);

                        foreach (DataColumn column in dt.Columns)
                        {
                            ICell cell = dataRow.CreateCell(column.Ordinal);
                            cell.SetCellValue(row[column.Caption].ToString());
                        }
                        rowIndex++;
                    }

                    workbook.Write(ms);
                    ms.Flush();
                    ms.Position = 0;

                }

                return ms;
            }
        }
    }
}
