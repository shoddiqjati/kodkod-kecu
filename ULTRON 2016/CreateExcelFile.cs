using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using System.Diagnostics;
using System.Reflection;


namespace ULTRON_2016
{
    public class CreateExcelFile
    {
        public static bool CreateExcelDocument<T>(List<T> list, string xlsxFilePath)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(ListToDataTable(list));

            return CreateExcelDocument(ds, xlsxFilePath);
        }

        //  This function is taken from: http://www.codeguru.com/forum/showthread.php?t=450171
        public static DataTable ListToDataTable<T>(List<T> list)
        {
            DataTable dt = new DataTable();

            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            foreach (T t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(T).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null);
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        public static bool CreateExcelDocument(DataTable dt, string xlsxFilePath)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            return CreateExcelDocument(ds, xlsxFilePath);
        }

        public static bool CreateExcelDocument(DataSet ds, string xlsxFilePath)
        {
            using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Create(xlsxFilePath, SpreadsheetDocumentType.Workbook, true))
            {
                // Create the Workbook
                spreadSheet.AddWorkbookPart();
                spreadSheet.WorkbookPart.Workbook = new Workbook();

                // A Workbook must only have exactly one <Sheets> section
                spreadSheet.WorkbookPart.Workbook.AppendChild(new Sheets());

                //  Loop through each of the DataTables in our DataSet, and create a new Excel Worksheet for each.
                uint worksheetNumber = 1;
                foreach (DataTable dt in ds.Tables)
                {
                    WorksheetPart newWorksheetPart = spreadSheet.WorkbookPart.AddNewPart<WorksheetPart>();
                    newWorksheetPart.Worksheet = new Worksheet();

                    //  Create a new Excel worksheet
                    SheetData sheetData = newWorksheetPart.Worksheet.AppendChild(new SheetData());

                    //  Create a Header Row in our Excel file, containing one header for each Column of data in our DataTable.
                    //
                    //  We'll also create an array, showing which type each column of data is (Text or Numeric), so when we come to write the actual
                    //  cells of data, we'll know if to write Text values or Numeric cell values.
                    int numberOfColumns = dt.Columns.Count;
                    bool[] IsNumericColumn = new bool[numberOfColumns];

                    string[] excelColumnNames = new string[numberOfColumns];
                    for (int n = 0; n < numberOfColumns; n++)
                        excelColumnNames[n] = GetExcelColumnName(n);

                    int rowIndex = 1;
                    Row newRow = sheetData.AppendChild(new Row());
                    for (int colInx = 0; colInx < numberOfColumns; colInx++)
                    {
                        DataColumn col = dt.Columns[colInx];
                        Cell newHeaderCell = CreateTextCell(excelColumnNames[colInx], rowIndex, col.ColumnName);
                        IsNumericColumn[colInx] = (col.DataType.FullName == "System.Decimal");     //  eg "System.String" or "System.Decimal"
                        newRow.AppendChild(newHeaderCell);
                    }

                    //  Now, step through each row of data in our DataTable...
                    Cell newCell;
                    foreach (DataRow dr in dt.Rows)
                    {
                        // ...create a new row, and append a set of this row's data to it.
                        newRow = sheetData.AppendChild(new Row());
                        ++rowIndex;

                        for (int colInx = 0; colInx < numberOfColumns; colInx++)
                        {
                            string cellValue = dr.ItemArray[colInx].ToString();

                            // Create cell with data
                            if (IsNumericColumn[colInx] && !string.IsNullOrEmpty(cellValue))
                                newCell = CreateNumericCell(excelColumnNames[colInx], rowIndex, cellValue);
                            else
                                newCell = CreateTextCell(excelColumnNames[colInx], rowIndex, cellValue);

                            newRow.AppendChild(newCell);
                        }
                    }
                    newWorksheetPart.Worksheet.Save();

                    //  Link this worksheet to our workbook
                    spreadSheet.WorkbookPart.Workbook.GetFirstChild<Sheets>().AppendChild(new Sheet()
                    {
                        Id = spreadSheet.WorkbookPart.GetIdOfPart(newWorksheetPart),
                        SheetId = worksheetNumber++,
                        Name = dt.TableName
                    });
                }   // foreach (DataTable..)

                spreadSheet.WorkbookPart.Workbook.Save();

            }   // using (SpreadsheetDocument spreadSheet..)

            return true;
        }

        private static Cell CreateTextCell(string excelColumnName, int excelRowIndex, string cellValue)
        {
            //  Create an Excel cell, containing a Text value
            Cell c = new Cell();
            c.DataType = CellValues.InlineString;
            c.CellReference = excelColumnName + excelRowIndex.ToString();

            //Add text to the text cell.
            InlineString inlineString = new InlineString();
            Text t = new Text();
            t.Text = cellValue.ToString();
            inlineString.AppendChild(t);
            c.AppendChild(inlineString);

            return c;
        }

        private static Cell CreateNumericCell(string excelColumnName, int excelRowIndex, string cellValue)
        {
            //  Create an Excel cell, containing a numeric value
            Cell c = new Cell();
            c.DataType = CellValues.Number;
            c.CellReference = excelColumnName + excelRowIndex.ToString();

            if (string.IsNullOrEmpty(cellValue))
                return c;

            decimal decimalValue = 0;
            if (!decimal.TryParse(cellValue, out decimalValue))
                return c;

            //  If we don't specifically set a number of decimal places, sometimes it'll create invalid Excel
            //  files ("Do you wish to repair..")
            CellValue v = new CellValue();
            v.Text = decimalValue.ToString("0.000000");
            c.AppendChild(v);

            return c;
        }

        private static string GetExcelColumnName(int columnIndex)
        {
            //  Convert a zero-based column index into an Excel column reference (A, B, C.. Y, Y, AA, AB, AC... AY, AZ, B1, B2..)
            //  Each Excel cell we write must have the cell name stored with it.
            //
            if (columnIndex < 26)
                return ((char)('A' + columnIndex)).ToString();

            //char firstChar = (char)('A' + (columnIndex / 26));
            //If columnIndex  more than 26 (columnIndex / 26)=1 so It gives B, so A+((columnIndex / 26)-1 will give the currect value.
            char firstChar = (char)('A' + (columnIndex / 26) - 1);
            char secondChar = (char)('A' + (columnIndex % 26));

            return string.Format("{0}{1}", firstChar, secondChar);
        }
    }

}
