using SPCoder.Core.Plugins;
using OfficeOpenXml;
using System.IO;

/// <summary>
/// SPCoder plugin that reads the first sheet of an excel file and shows the data in SPCoder's viewer.
/// This plugin can be extended by adding the popup window for setting the order of the sheet.
/// </summary>
public class SPExcelWorksheetDataGetter : BasePlugin
{
    public SPExcelWorksheetDataGetter()
    {
        this.TargetType = typeof(System.IO.FileInfo);
        this.Name = "Show data in grid";
        this.Filter = ".xlsx";
    }

    public override void Execute(Object target)
    {
        //if (target is typeof(this.TargetType))            
        List<string> errorList = new List<string>();
        DataTable dt = this.ExcelToDataTable(((System.IO.FileInfo)target).FullName, ref errorList);
        Result = dt;
        ExecuteCallback(dt);
    }

    public override bool IsActive(Object target)
    {
        bool active = base.IsActive(target);
        if (!active)
            return false;
        if (((System.IO.FileInfo)target).Extension == this.Filter)
            return true;
        return false;
    }

    public DataTable ExcelToDataTable(string path, ref List<string> errorList, bool hasHeaderRow = true, int worksheetOrder = 1)
    {
        DataTable dt = new DataTable();
        errorList = new List<string>();

        //create a new Excel package           
        using (ExcelPackage excelPackage = new ExcelPackage())
        {
            using (var stream = System.IO.File.OpenRead(path))
            {
                excelPackage.Load(stream);
            }

            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[worksheetOrder];

            //check if the worksheet is completely empty
            if (worksheet.Dimension == null)
            {
                return dt;
            }

            //add the columns to the datatable
            for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
            {
                string columnName = "Column " + j;


                //Build hashset with all types in the row
                var columnTypes = new HashSet<Type>();
                for (int i = worksheet.Dimension.Start.Row + Convert.ToInt32(hasHeaderRow); i <= worksheet.Dimension.End.Row; i++)
                {
                    //Only add type if cell value not empty
                    if (worksheet.Cells[i, j].Value != null)
                    {
                        columnTypes.Add(worksheet.Cells[i, j].Value.GetType());
                    }
                }

                var excelCell = worksheet.Cells[1, j].Value;


                if (excelCell != null)
                {
                    Type excelCellDataType = null;

                    //if there is a headerrow, set the next cell for the datatype and set the column name
                    if (hasHeaderRow == true)
                    {

                        columnName = excelCell.ToString();

                        //check if the column name already exists in the datatable, if so make a unique name
                        if (dt.Columns.Contains(columnName) == true)
                        {
                            columnName = columnName + "_" + j;
                        }
                    }

                    //Select  input type for the column
                    if (columnTypes.Count == 1)
                    {
                        excelCellDataType = columnTypes.First();
                    }
                    else
                    {
                        excelCellDataType = typeof(string);
                    }

                    //try to determine the datatype for the column (by looking at the next column if there is a header row)
                    if (excelCellDataType == typeof(DateTime))
                    {
                        dt.Columns.Add(columnName, typeof(DateTime));
                    }
                    else if (excelCellDataType == typeof(Boolean))
                    {
                        dt.Columns.Add(columnName, typeof(Boolean));
                    }
                    else if (excelCellDataType == typeof(Double))
                    {
                        //determine if the value is a decimal or int by looking for a decimal separator
                        //not the cleanest of solutions but it works since excel always gives a double
                        if (excelCellDataType.ToString().Contains(".") || excelCellDataType.ToString().Contains(","))
                        {
                            dt.Columns.Add(columnName, typeof(Decimal));
                        }
                        else
                        {
                            dt.Columns.Add(columnName, typeof(Int64));
                        }
                    }
                    else
                    {
                        dt.Columns.Add(columnName, typeof(String));
                    }
                }
                else
                {
                    dt.Columns.Add(columnName, typeof(String));
                }
            }

            //start adding data the datatable here by looping all rows and columns
            for (int i = worksheet.Dimension.Start.Row + Convert.ToInt32(hasHeaderRow); i <= worksheet.Dimension.End.Row; i++)
            {
                //create a new datatable row
                DataRow row = dt.NewRow();

                //loop all columns
                for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                {
                    var excelCell = worksheet.Cells[i, j].Value;

                    //add cell value to the datatable
                    if (excelCell != null)
                    {
                        try
                        {
                            row[j - 1] = excelCell;
                        }
                        catch
                        {
                            errorList.Add("Row " + (i - 1) + ", Column " + j + ". Invalid " + dt.Columns[j - 1].DataType.ToString().Replace("System.", "") + " value:  " + excelCell.ToString());
                        }
                    }
                }

                //add the new row to the datatable
                dt.Rows.Add(row);
            }
        }

        return dt;
    }
}

//registration code
SPExcelWorksheetDataGetter excelFileDataGetter = new SPExcelWorksheetDataGetter();
excelFileDataGetter.Callback += DoShowObjectInGrid;
PluginContainer.Register(excelFileDataGetter);
logger.LogInfo("Registered plugin SPExcelWorksheetDataGetter");

//listItemGetter.Execute(list);
