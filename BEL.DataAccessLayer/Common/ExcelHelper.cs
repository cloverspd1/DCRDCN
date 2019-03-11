namespace BEL.DataAccessLayer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.IO;
    using System.Configuration;
    using DocumentFormat.OpenXml.Packaging;
    using Microsoft.Office.Interop.Excel;

    /// <summary>
    /// Excel Helper
    /// </summary>
    public static class ExcelHelper
    {
        /// <summary>
        /// This method takes DataSet as input paramenter and it exports the same to excel
        /// </summary>
        /// <param name="ds">Data Set</param>
        /// <returns>
        /// byte array of excel file
        /// </returns>
        public static byte[] GetByteArrayExcel(DataSet ds)
        {
            try
            {
                byte[] returnBytes = null;
                if (ds != null && ds.Tables.Count != 0)
                {
                    using (MemoryStream mem = new MemoryStream())
                    {
                        var workbook = SpreadsheetDocument.Create(mem, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook);
                        {
                            workbook.AddWorkbookPart();
                            workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
                            workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();
                            for (int i = ds.Tables.Count - 1; i >= 0; i--)
                            {
                                System.Data.DataTable table = ds.Tables[i];
                                var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                                var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                                sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                                DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
                                string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                                uint sheetId = 1;
                                if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
                                {
                                    sheetId =
                                        sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                                }

                                DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = table.TableName };
                                sheets.Append(sheet);

                                List<string> columns = new List<string>();
                                foreach (System.Data.DataColumn column in table.Columns)
                                {
                                    columns.Add(column.ColumnName);
                                }

                                foreach (System.Data.DataRow dsrow in table.Rows)
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                                    foreach (string col in columns)
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dsrow[col].ToString());
                                        newRow.AppendChild(cell);
                                    }
                                    sheetData.AppendChild(newRow);
                                }
                            }
                        }
                        workbook.Close();
                        workbook.Dispose();
                        returnBytes = mem.ToArray();
                    }
                }
                return returnBytes;
            }
            catch (Exception ex)
            {
                Logger.Info("Error while generate LSMW Excel file - , Message : {0}  ,Stack:{1}", ex.Message, ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// This method takes DataSet as input paramenter and it exports the same to excel
        /// </summary>
        /// <param name="ds">Data Set</param>
        /// <returns>
        /// byte array of excel file
        /// </returns>
        public static byte[] GetByteArrayOfficeExcel(DataSet ds)
        {
            byte[] byteArray = null;
            if (ds != null)
            {
                //Creae an Excel application instance
                Application excelApp = new Application();

                //Create an Excel workbook instance and open it from the predefined location
                string physicalPath = System.Web.HttpContext.Current.Server.MapPath("~/Temp");
                if (Directory.Exists(physicalPath))
                {
                    Directory.CreateDirectory(physicalPath);
                }
                string fileId = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 10) + ".xlsx";
                string filePath = physicalPath.Trim('\\') + "\\" + fileId;
                Workbook excelWorkBook = excelApp.Workbooks.Add(1);
                foreach (System.Data.DataTable table in ds.Tables)
                {
                    ////Add a new worksheet to workbook with the Datatable name
                    Worksheet excelWorkSheet = excelWorkBook.Sheets.Add();
                    excelWorkSheet.Name = table.TableName;
                    ////for (int i = 1; i < table.Columns.Count + 1; i++)
                    ////{
                    ////    excelWorkSheet.Cells[1, i] = table.Columns[i - 1].ColumnName;
                    ////}
                    for (int j = 0; j < table.Rows.Count; j++)
                    {
                        for (int k = 0; k < table.Columns.Count; k++)
                        {
                            excelWorkSheet.Cells[j + 1, k + 1] = table.Rows[j].ItemArray[k].ToString();
                        }
                    }
                }
                excelWorkBook.SaveCopyAs(filePath);
                excelWorkBook.Close(true);
                excelApp.Quit();
                byteArray = File.ReadAllBytes(filePath);
                File.Delete(filePath);
            }
            return byteArray;
        }

        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <param name="noOfColumns">The no of columns.</param>
        /// <returns>Data table</returns>
        public static System.Data.DataTable GetDataTable(string sheetName, int noOfColumns)
        {
            using (System.Data.DataTable dt = new System.Data.DataTable())
            {
                dt.TableName = sheetName;
                for (int i = 1; i <= noOfColumns; i++)
                {
                    dt.Columns.Add("Column" + i);
                }
                return dt;
            }
        }

        /// <summary>
        /// s this instance.
        /// </summary>
        /// <param name="templateName"> template name</param>
        /// <returns>Template body</returns>
        public static string GetTemplateBody(string templateName)
        {
            return System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(string.Concat(ConfigurationManager.AppSettings["TemplatePath"], templateName, ".html")));
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>byte array</returns>
        public static byte[] GetBytes(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                byte[] bytes = new byte[str.Length * sizeof(char)];
                System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
                return bytes;
            }
            return null;
        }
    }
}