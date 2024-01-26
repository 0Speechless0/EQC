using System;

using System.Web;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Linq;
using NPOI.HSSF.Util;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;
using Aspose.Cells;
using Workbook = Aspose.Cells.Workbook;
using EQC.Services;

namespace EQC.Common
{

    public class NpoiMemoryStream :MemoryStream
    {
        public NpoiMemoryStream()
        {
            AllowClose = false;
        }
        public bool AllowClose { get; set; } 

        public override void Close()
        {
            if(AllowClose)
                base.Close();
        }
    }
    public class ExcelProcesser
    {
        public ISheet sheet;
        private int width;
        private string[] dbColumns;
        private bool startProcess = false;
        private int headerRow = 1;
        private int startRow = 0;
        private bool isRowAdj = false;
        private Dictionary<string, int> headerIndexMap = new Dictionary<string, int>();
        private Dictionary<string, string> dbColumnMap = new Dictionary<string, string>();
        private Dictionary<string, int> IndexMapForDbColumn ;
        private Dictionary<int, int> numberOfWidth = new Dictionary<int, int>();
        private Dictionary<string, int[]> posDictionary = new Dictionary<string, int[]>();

        Dictionary<string, string> tableColTypeMap;
        ///<summary>
        ///資料表欄位名稱和匯入excel位置之對應
        ///</summary>
        private Dictionary<string, int[]> declareColQueue = new Dictionary<string, int[]>();

        ///<summary>
        ///資料表欄位名稱和匯入excel索引序之對應 
        ///</summary>
        private Dictionary<string, int> declareColQueueV2 = new Dictionary<string, int>();

        ///<summary>
        ///資料表欄位名稱和值的對應
        ///</summary>
        private Dictionary<string, string> declareColQueueV3 = new Dictionary<string, string>();


        private string indexCol;
        private string _tableName;
        public ExcelProcesser(int _headerRow, Action<XSSFWorkbook> doInit = null)
        {


            var workbook = new XSSFWorkbook();

            sheet = workbook.CreateSheet();

            startRow = _headerRow + 1;

            sheet.Workbook.SetActiveSheet(0);
            doInit?.Invoke(workbook);
        }
        public ExcelProcesser(string filePath, int _headerRow, int activeSheet = 0)
        {


            var workbook = new XSSFWorkbook(filePath);

            if (workbook.NumberOfSheets == 0)
            {
                throw new Exception("No Sheet");
            }

            sheet = workbook.GetSheetAt(0);

            startRow = _headerRow + 1;

            sheet.Workbook.SetActiveSheet(activeSheet);

        }
        public ExcelProcesser(MemoryStream inputStream, int? _headerRow, int activeSheet = 0)
        {
            inputStream.Position = 0;

            var workbook = new XSSFWorkbook(inputStream);

            if (workbook.NumberOfSheets == 0)
            {
                throw new Exception("No Sheet");
            }

            sheet = workbook.GetSheetAt(activeSheet);

            if (_headerRow is int __headerRow)
            {
                startRow = __headerRow + 1;

            }
            else findStartRow();

            sheet.ForceFormulaRecalculation = true;

        }
        public ExcelProcesser(Stream inputStream, int? _headerRow, int activeSheet=0)
        {
            MemoryStream ms = new MemoryStream();

            inputStream.CopyTo(ms);

            ms.Position = 0;

            var workbook = new XSSFWorkbook(ms);
            if (workbook.NumberOfSheets == 0)
            {
                throw new Exception("No Sheet");
            }

            sheet = workbook.GetSheetAt(activeSheet);

            if (_headerRow is int __headerRow) 
            {
                startRow = __headerRow + 1;

            }
            else findStartRow();

            sheet.ForceFormulaRecalculation = true;
            ms.Dispose();
            inputStream.Position = 0;

        }

 
        public ExcelProcesser(HttpPostedFileBase file, string fileName, string tableName = null)
        {
            _tableName = tableName;
            tableColTypeMap = _tableName != null ? SqlDB.getTableColumnType(_tableName) : null;
            if (!file.FileName.EndsWith(".xlsx"))
            {
                throw new Exception("file type error");
            }
            string tempPath = Path.GetTempPath();
            string fullPath = Path.Combine(tempPath, fileName);
            var workbook = new XSSFWorkbook(file.InputStream);
            if (workbook.NumberOfSheets == 0)
            {
                throw new Exception("No Sheet");
            }

            sheet = workbook.GetSheetAt(0);
            



            //findTableWidth();
            findStartRow();
            findHeaderRow();


        }

        private void findHeaderRow()
        {
            for (int i = startRow - 1; i >= 0; i--)
            {
                int cellNum = sheet.GetRow(i).PhysicalNumberOfCells;
                int j = 0;
                for (j = 0; j < cellNum; j++)
                {
                    string value = sheet.GetRow(i).GetCell(j).ToString();
                    if (value == "" && j > 0) break;
                }
                if (j == cellNum)
                {
                    headerRow = i;
                    break;
                }
            }
        }

        private void findTableWidth()
        {
            int max = 0;
            numberOfWidth.Add(max, 0);
            for (int i = 0; i < sheet.LastRowNum; i++)
            {
                int width = sheet.GetRow(i).Cells.Count;
                int value = 0;
                numberOfWidth.TryGetValue(width, out value);
                numberOfWidth[width] = value + 1;
                if (numberOfWidth[width] > numberOfWidth[max]) max = width;
            }
            this.width = max;
        }

        private void findStartRow()
        {
            int i = 0;
            if (!startProcess)
            {
                for (i = 0; i <= sheet.LastRowNum; i++)
                {
                    if (sheet.GetRow(i) == null) continue;
                    var value = (sheet.GetRow(i).GetCell(0)?.ToString());
                    if (Int32.TryParse(value, out int j)) break;
                }
                startProcess = true;
                startRow = i;
            }
            if (i > sheet.LastRowNum)
            {
                startRow = 1;
                isRowAdj = true;
            }
        }
        public void declareCol(string colName, int[] pos, string[] replacements)
        {
            int row = pos[0];
            int col = pos[1];
            string str = sheet.GetRow(row).Cells[col].ToString();

            foreach (string r in replacements)
            {
                str = str.Replace(r, "");
            }
            sheet.GetRow(row).GetCell(col).SetCellValue(str);
            declareColQueue[colName] = pos;
        }
        public void declareCol(string[,] alternativeHeaderMap)
        {
            int length = alternativeHeaderMap.GetLength(0);
            for (int i = 0; i < length; i++)
            {
                int col = headerIndexMap[alternativeHeaderMap[i, 1]];
                declareColQueueV2[alternativeHeaderMap[i, 0]] = col;
            }


        }
        public bool checkRowVaild(int row, int key = 0)
        {
            string strKey = sheet.GetRow(row).Cells[key].ToString();
            int i;
            if (!startProcess)
            {
                throw new Exception("Process have not find starting row");
            }
            return isRowAdj || Int32.TryParse(strKey, out i);
        }


        private void createHeaderMapToIndex()
        {
            IRow headers = sheet.GetRow(headerRow);
            int headerNum = sheet.GetRow(headerRow).LastCellNum;
            for (int i = 0; i < headerNum; i++)
            {
                string headerName = headers.Cells[i].ToString().Replace("\n", "");

                if (headerIndexMap.ContainsKey(headerName))
                {
                    // headerIndexMap[headerName] = i;
                    continue;
                }
                else
                {
                    headerIndexMap.Add(headers.Cells[i].ToString().Replace("\n", ""), i);
                }
            }

        }

        public void createExcelPosMap(Dictionary<string, int[]> dictinary)
        {
            posDictionary = dictinary;
        }
        public bool createDbColumnMapToHeader(string[,] columnMap)
        {
            int columnNum = columnMap.GetLength(0);
            dbColumns = new string[columnNum];
            for (int i = 0; i < columnNum; i++)
            {
                dbColumnMap.Add(columnMap[i, 0], columnMap[i, 1]);
                dbColumns[i] = columnMap[i, 0];
            }
            createHeaderMapToIndex();
            return true;
        }
        public bool createDbColumnMapToIndex(object[,] columnMap)
        {
            int columnNum = columnMap.GetLength(0);
            IndexMapForDbColumn = new Dictionary<string, int>();
            dbColumns = new string[columnNum];
            for (int i = 0; i < columnNum; i++)
            {
                IndexMapForDbColumn.Add((string)columnMap[i, 0], (int)columnMap[i, 1]);
                dbColumns[i] = (string)columnMap[i, 0];
            }
            createHeaderMapToIndex();
            return true;
        }
        private string DateTimeToStr(DateTime date)
        {
            if (date.Year.ToString().Length > 3)
                return (date.Year - 1911).ToString("D2") + (date.Month).ToString("D2") + (date.Day).ToString("D2");
            return (date.Year).ToString() + (date.Month).ToString("D2") + (date.Day).ToString("D2");
        }
        public string getInsertSql(string tableName)
        {
            System.Globalization.CultureInfo tc = new System.Globalization.CultureInfo("zh-TW");
            tc.DateTimeFormat.Calendar = new System.Globalization.TaiwanCalendar();
            string sql = @"Insert Into " + tableName;
            string colSection = @"(";
            string valueSection = @"VALUES (";
            foreach (KeyValuePair<string, int[]> item in posDictionary)
            {
                int row = item.Value[0];
                int col = item.Value[1];
                string colValue = (sheet.GetRow(row).Cells[col]).ToString();
                bool isDate = DateTime.TryParse(colValue, tc, DateTimeStyles.None, out DateTime DateTimeResult);
                bool isDecimal = decimal.TryParse(colValue, out decimal i);
                if (isDate && !isDecimal)
                {
                    colValue = DateTimeToStr(DateTimeResult);
                }

                colSection += item.Key + @",";

                if (colValue == "--")
                {
                    valueSection += "NULL" + @",";

                }
                else
                {
                    valueSection += "'" + colValue + "'" + @",";
                }

            }
            colSection = colSection.Remove(colSection.Length - 1, 1) + @")";
            valueSection = valueSection.Remove(valueSection.Length - 1, 1) + @")";
            sql += colSection + valueSection + ";";

            return remarkInsert(sql);
        }


        //private void findMappingIndex(string colName,int row, out int index)
        //{
        //    index = headerIndexMap[dbColumnMap[colName]];
        //    string headerName = sheet.GetRow(headerRow).Cells[index].ToString();

        //    if (headerName == dbColumnMap[colName]) return ;

        //    while(++index < sheet.GetRow(row).LastCellNum )
        //    {
        //        headerName = sheet.GetRow(headerRow).Cells[index].ToString();
        //        if (headerName == dbColumnMap[colName]) return ;
        //    }
        //    throw new Exception("can't find mapping header");
        //}  


        public string getInsertSql(string tableName, int row) {
            System.Globalization.CultureInfo tc = new System.Globalization.CultureInfo("zh-TW");
            tc.DateTimeFormat.Calendar = new System.Globalization.TaiwanCalendar();
            string sql = @"Insert Into " + tableName;
            string colSection = @"(";
            string valueSection = @"VALUES (";
            if (dbColumns.Length == 0) throw new Exception("No map, plead create it ");
            foreach (string colName in dbColumns)
            {
                int index = IndexMapForDbColumn == null ?
                    headerIndexMap[dbColumnMap[colName]] : IndexMapForDbColumn[colName];
                string colValue = (sheet.GetRow(row).Cells[index]).ToString();
                bool isDate = DateTime.TryParse(colValue, tc, DateTimeStyles.None, out DateTime DateTimeResult);
                bool isDecimal = decimal.TryParse(colValue, out decimal i);
                if (isDate && !isDecimal)
                {
                    colValue = DateTimeToStr(DateTimeResult);
                }

                colSection += colName + @",";
                
                if (colValue == "--" || (!isDecimal && tableColTypeMap != null &&
                    ( (tableColTypeMap[colName].StartsWith("decimal")) ||
                    (tableColTypeMap[colName].StartsWith("int") ) 
                    ) )
                )
                {
                    valueSection += "NULL" + @",";
                }
                else
                {
                    valueSection += "'" + colValue + "'" + @",";
                }


            }
            colSection = colSection.Remove(colSection.Length - 1, 1) + @")";
            valueSection = valueSection.Remove(valueSection.Length - 1, 1) + @")";
            sql += colSection + valueSection + ";";
            return remarkInsert(sql);
        }
        public string getUpdateSql(string tableName, string target)
        {
            System.Globalization.CultureInfo tc = new System.Globalization.CultureInfo("zh-TW");
            tc.DateTimeFormat.Calendar = new System.Globalization.TaiwanCalendar();
            string sql = @"Update " + tableName + " SET ";
            string section = "";
            string targetValue = "";
            foreach (KeyValuePair<string, int[]> item in posDictionary)
            {

                int row = item.Value[0];
                int col = item.Value[1];
                string colValue = (sheet.GetRow(row).Cells[col]).ToString();
                if (item.Key == target)
                    targetValue = colValue;
                bool isDate = DateTime.TryParse(colValue, tc, DateTimeStyles.None, out DateTime DateTimeResult);
                bool isDecimal = decimal.TryParse(colValue, out decimal i);
                if (isDate && !isDecimal)
                {
                    colValue = DateTimeToStr(DateTimeResult);
                }
                if (colValue == "--")
                {
                    section += item.Key + "=" + "NULL" + @",";
                }
                else
                {
                    section += item.Key + "=" + "'" + colValue + "'" + @",";
                }

            }
            section = section.Remove(section.Length - 1, 1);
            if (declareColQueue.ContainsKey(target))
            {
                string value = sheet.GetRow(declareColQueue[target][0]).GetCell(declareColQueue[target][1]).ToString();
                sql += section + " where " + target + "=" + "'" + value + "'";
            }
            else
            {
                sql += section + " where " + target + "=" + "'" + targetValue + "'";
            }
            return remarkUpdate(sql);
        }
        public string getUpdateSql(string tableName, string target, int row)
        {
            System.Globalization.CultureInfo tc = new System.Globalization.CultureInfo("zh-TW");
            tc.DateTimeFormat.Calendar = new System.Globalization.TaiwanCalendar();
            string sql = @"Update " + tableName + " SET ";
            string section = "";
            string targetValue = "";
            if (dbColumns.Length == 0) throw new Exception("No map, plead create it ");
            foreach (string colName in dbColumns)
            {

                int index = IndexMapForDbColumn == null ?
                     headerIndexMap[dbColumnMap[colName]] : IndexMapForDbColumn[colName];
                string colValue = (sheet.GetRow(row).Cells[index]).ToString();
                if (colName == target)
                    targetValue = colValue;
                bool isDate = DateTime.TryParse(colValue, tc, DateTimeStyles.None, out DateTime DateTimeResult);
                bool isDecimal = decimal.TryParse(colValue, out decimal i);
                if (isDate && !isDecimal)
                {
                    colValue = DateTimeToStr(DateTimeResult);
                }
                if (colValue == "--" || (!isDecimal && tableColTypeMap != null &&
                    ((tableColTypeMap[colName].StartsWith("decimal")) ||
                    (tableColTypeMap[colName].StartsWith("int"))
                    ))
                )
                {
                    section += colName + "=" + "NULL" + @",";
                }
                else
                {
                    section += colName + "=" + "'" + colValue + "'" + @",";
                }

            }

            section = section.Remove(section.Length - 1, 1);
            if (declareColQueue.ContainsKey(target))
            {
                string value = sheet.GetRow(declareColQueue[target][0]).GetCell(declareColQueue[target][1]).ToString();
                sql += section + " where " + target + "=" + "'" + value + "'";
            }
            else if (declareColQueueV2.ContainsKey(target))
            {
                string value = sheet.GetRow(row).GetCell(declareColQueueV2[target]).ToString();
                sql += section + " where " + target + "=" + "'" + value + "'";
            }
            else
            {
                sql += section + " where " + target + "=" + "'" + targetValue + "'";
            }

            this.indexCol = target;
            return remarkUpdate(sql);
        }


        public string addDeclareColToInsertSql(string sql)
        {
            int cutting_pos = sql.IndexOf(")");
            string firstSplit = sql.Substring(0, cutting_pos);
            string lastSplit = sql.Substring(cutting_pos, sql.Length - cutting_pos);
            lastSplit = lastSplit.Remove(lastSplit.Length - 2, 2);
            foreach (KeyValuePair<string, int[]> item in declareColQueue)
            {
                firstSplit += "," + item.Key;
                lastSplit += ", '" + sheet.GetRow(item.Value[0]).GetCell(item.Value[1]).ToString() + "'";

            }
            foreach (KeyValuePair<string, string> item in declareColQueueV3)
            {
                firstSplit += "," + item.Key;
                lastSplit += ", '" + item.Value +"'";

            }
                return firstSplit + lastSplit + ");";

        }

        public string addDeclareColToUpdateSql(string sql)
        {
            int cutting_pos = sql.IndexOf("where");
            string beforceWhere = sql.Substring(0, cutting_pos);
            string afterWhere = sql.Substring(cutting_pos, sql.Length - beforceWhere.Length);
            foreach (KeyValuePair<string, int[]> item in declareColQueue)
            {
                string value = sheet.GetRow(item.Value[0]).GetCell(item.Value[1]).ToString();
                beforceWhere += "," + item.Key + "=" + "'" + value + "'";

            }
            foreach (KeyValuePair<string, string> item in declareColQueueV3)
            {
                beforceWhere += "," + item.Key + "=" + "'" + item.Value + "'";

            }

            return beforceWhere + " " + afterWhere;

        }

        public string addDeclareColToInsertSql(string sql, int row)
        {
            int cutting_pos = sql.IndexOf(")");
            string firstSplit = sql.Substring(0, cutting_pos);
            string lastSplit = sql.Substring(cutting_pos, sql.Length - cutting_pos);
            lastSplit = lastSplit.Remove(lastSplit.Length - 2, 2);
            foreach (KeyValuePair<string, int> item in declareColQueueV2)
            {
                firstSplit += "," + item.Key;
                lastSplit += ", '" + sheet.GetRow(row).GetCell(item.Value).ToString() + "'";

            }

            return firstSplit + lastSplit + ");";

        }

        public string addDeclareColToUpdateSql(string sql, int row)
        {
            int cutting_pos = sql.IndexOf("where");
            string beforceWhere = sql.Substring(0, cutting_pos);
            string afterWhere = sql.Substring(cutting_pos, sql.Length - beforceWhere.Length);
            foreach (KeyValuePair<string, int> item in declareColQueueV2)
            {
                if (item.Key == indexCol) continue;
                string value = sheet.GetRow(row).GetCell(item.Value).ToString();
                beforceWhere += "," + item.Key + "=" + "'" + value + "'";

            }

            return beforceWhere + " " + afterWhere;
        }
        private string remarkInsert(string sql)
        {
            int cutting_pos = sql.IndexOf(")");
            string firstSplit = sql.Substring(0, cutting_pos);
            string lastSplit = sql.Substring(cutting_pos, sql.Length - cutting_pos);
            SessionManager session = new SessionManager();
            lastSplit = lastSplit.Remove(lastSplit.Length - 2, 2);
            firstSplit += ", CreateTime, CreateUserSeq, ModifyTime, ModifyUserSeq";
            lastSplit += ", GETDATE(), " + session.GetUser().Seq + ", GETDATE()," + session.GetUser().Seq;
            return firstSplit + lastSplit + ");";
        }
        private string remarkUpdate(string sql)
        {
            int cutting_pos = sql.IndexOf("where");
            string beforceWhere = sql.Substring(0, cutting_pos);
            SessionManager session = new SessionManager();
            string afterWhere = sql.Substring(cutting_pos, sql.Length - beforceWhere.Length);

            beforceWhere += ", ModifyTime= GETDATE(), ModifyUserSeq=" + session.GetUser().Seq;

            return beforceWhere + " " + afterWhere + ";";
        }
        public int getStartRow()
        {
            return startRow;
        }

        public int getEndRow()
        {
            return sheet.LastRowNum + 1;
        }
        public int getEndRowBySeq(int seqColNum = 0)
        {
            int i = startRow;
            do
            {
                var v = sheet.GetRow(i)?.GetCell(seqColNum)?.ToString();
                if (v == null) break;
                var isNum = Int32.TryParse(v, out int result);
                if (!isNum) break;
                i++;
            } while (true);
            return i;
        }

        public void declareCol(string colName, Dictionary<string, int[]> posMap, Func<Dictionary<string, string>, string> converter) 
        {
            Dictionary<string, string> colValueMap = new Dictionary<string, string>();

            foreach(KeyValuePair<string, int[]> item in posMap)
            {
                string value = sheet.GetRow(item.Value[0]).GetCell(item.Value[1]).ToString();
                colValueMap[item.Key] = value;
            }

            string resulut = converter.Invoke(colValueMap);

            declareColQueueV3[colName] = resulut;
        }

        //可能之後用得到
        public void declareCol(string colName, int row, Func<Dictionary<string, string>, string> converter)
        {
            Dictionary<string, string> colValueMap = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> item in dbColumnMap)
            {
                int index = headerIndexMap[item.Value];

                string value = sheet.GetRow(row).GetCell(index).ToString();
                colValueMap[item.Key] = value;
            }

            string resulut = converter.Invoke(colValueMap);

            declareColQueueV3[colName] = resulut;
        }

        public ExcelProcesser insertTemplate<T>(List<T> insertList)
        {
            int colCount = 0;
            int row = headerRow;
            foreach(T item in insertList)
            {
                row++;
                for(int i= 0; i< colCount; i ++)
                {
                    string headerName = sheet.GetRow(headerRow).Cells[i].ToString();
                    string ColValue = item.GetType().GetProperty(dbColumnMap[headerName] ).GetValue(item)?.ToString() ?? "";
                    sheet.GetRow(row).Cells[i].SetCellValue(ColValue);
                }


            }
            return this;

        }
        public ExcelProcesser insertOneCol<T>(IEnumerable<T> insertList, int col)
        {
            int colCount = 0;
            int row = startRow;
            foreach (T item in insertList)
            {



                setCell(row, col, item?.ToString() ?? "");
                row++;

            }
            return this;

        }

        public MemoryStream getTemplateStream(MemoryStream mss = null)
        {
            var ms = mss ?? new MemoryStream();
            sheet.Workbook.Write(ms);

            return ms;

        }

        public ExcelProcesser insertSection<T>(IEnumerable<T[]> data, int startCol)
        {

            int i = 0;
            foreach(var item in data)
            {
                for (int j = 0; j < item.Length; j++)
                {

                    setCell(i + startRow, j + startCol,item[j].ToString() );
                }
                i++;

            }
            return this;
        }

        public ExcelProcesser insertRowContent(string[] col, 
            short cellColor, 
            short fontColor
        )
        {
            int i = 0 ;
            foreach (string val in col)
            {


                ICell cell = setCell(startRow, i, val);
                var style = cell?.CellStyle ?? sheet.Workbook.CreateCellStyle();
                var font = sheet.Workbook.CreateFont();
                style.FillForegroundColor = cellColor;
                style.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                font.Color = fontColor;
                font.FontHeightInPoints = 16;
                font.FontName = "微軟正黑體";
                font.IsBold = true;
                style.SetFont(font);
                cell.CellStyle = style;
                i++;
            }
            startRow++;
            return this;
        }
        public ExcelProcesser insertRowContentV2(string[] col,
            short cellColor,
            short fontColor
        )
        {
            int i = 0;
            var lastRow = sheet.GetRow(startRow - 1);
            foreach (string val in col)
            {

                ICell lastRowCell = lastRow.GetCell(i) ?? lastRow.CreateCell(i);

                var lastRowCellFont = lastRowCell.CellStyle.GetFont(sheet.Workbook);
                ICell cell = setCell(startRow, i, val);
                var style = cell?.CellStyle ?? sheet.Workbook.CreateCellStyle();
                var font = sheet.Workbook.CreateFont();
                
                style.FillForegroundColor = cellColor;
                style.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                font.Color = fontColor;
                font.FontHeightInPoints = lastRowCellFont.FontHeightInPoints;
                font.FontName = lastRowCellFont.FontName;
                font.IsBold = true;
                style.SetFont(font);
                cell.CellStyle = style;
                i++;
            }
            startRow++;
            return this;
        }
        public ExcelProcesser setStartRow(int row)
        {
            startRow = row;
            return this;
        }

        public ExcelProcesser setSheetCell(int row, int col, string value)
        {
            setCell(row, col, value);
            return this;
        }

        private ICell setCell(int row, int col, string value)
        {
            IRow currentRow = sheet.GetRow(row);
            IRow newRow = null;
     

            if (currentRow == null)
            {
                newRow = sheet.CreateRow(row);
            }

            if (col >= (currentRow ?? newRow).Cells.Count)
            {
                currentRow = (currentRow ?? newRow);
                for (int i = currentRow.Cells.Count; i <= col; i++) currentRow.CreateCell(i);

            }


            if (value != null && value.StartsWith("="))
            {

                currentRow.Cells[col].SetCellFormula(value.Remove(0, 1));
                currentRow.Cells[col].SetCellType(CellType.Formula);
            }
            else
            {

                if(double.TryParse(value, out double result) )
                {
                    currentRow.Cells[col].SetCellValue(result);
                    currentRow.Cells[col].SetCellType(CellType.Numeric);
                }
                else
                {
                    currentRow.Cells[col].SetCellValue(value);
                    currentRow.Cells[col].SetCellType(CellType.String);
                }
            }

            return sheet.GetRow(row).Cells[col];
        }
        public ExcelProcesser setSheet(int n)
        {
            sheet = sheet.Workbook.GetSheetAt(n);
            return this;
        }
        public ExcelProcesser fowardStartRow(int n)
        {
            startRow += n;
            return this;
        }

        public void removeSheet(int n)
        {
            sheet.Workbook.RemoveSheetAt(n);
        }

        public ExcelProcesser copyOutSideRowStyle(int sheetIndex2, int sourceSheetRow, int copyCount)
        {
            var sheet1 = sheet;
            var sheet2 = sheet.Workbook.GetSheetAt(sheetIndex2);
            for (int j = 0; j < copyCount; j++)
            {
                var sourceRow = sheet2.GetRow(sourceSheetRow);
                var targetRow = sheet1.GetRow(startRow+j) ?? sheet1.CreateRow(startRow+j);
                int i;
                for (i = 0; i < sourceRow.LastCellNum; i++)
                {
                    // Grab a copy of the old/new cell
                    ICell oldCell = sourceRow.GetCell(i);
                    ICell newCell = targetRow.GetCell(i);

                    if (newCell == null)
                        newCell = targetRow.CreateCell(i) as ICell;

                    // If the old cell is null jump to next cell
                    if (oldCell == null)
                    {
                        newCell = null;
                        continue;
                    }
                    var newCellStyle = sheet.Workbook.CreateCellStyle();
                    newCellStyle.CloneStyleFrom(oldCell.CellStyle);
                    newCell.CellStyle = newCellStyle;

                }
            }


            return this;
        }
        public ExcelProcesser copyOutSideRow(int sheetIndex2, int sourceSheetRow)
        {
            var sheet1 = sheet;
            var sheet2 = sheet.Workbook.GetSheetAt(sheetIndex2);
            var sourceRow = sheet2.GetRow(sourceSheetRow);
            var targetRow = sheet1.GetRow(startRow) ?? sheet1.CreateRow(startRow);
            int i;
            
            for (i = 0; i < sourceRow.LastCellNum; i++)
            {
                // Grab a copy of the old/new cell
                ICell oldCell = sourceRow.GetCell(i) ;
                ICell newCell = targetRow.GetCell(i) ;

                if (newCell == null)
                    newCell = targetRow.CreateCell(i) as ICell;

                // If the old cell is null jump to next cell
                if (oldCell == null)
                {
                    newCell = null;
                    continue;
                }

                // Copy style from old cell and apply to new cell
                newCell.CellStyle = oldCell.CellStyle;

                // If there is a cell comment, copy
                if (newCell.CellComment != null) newCell.CellComment = oldCell.CellComment;

                // If there is a cell hyperlink, copy
                if (oldCell.Hyperlink != null) newCell.Hyperlink = oldCell.Hyperlink;

                // Set the cell data value
                switch (oldCell.CellType)
                {
                    case CellType.Blank:
                        newCell.SetCellValue(oldCell.StringCellValue);
                        break;
                    case CellType.Boolean:
                        newCell.SetCellValue(oldCell.BooleanCellValue);
                        break;
                    case CellType.Error:
                        newCell.SetCellErrorValue(oldCell.ErrorCellValue);
                        break;
                    case CellType.Formula:
                        newCell.CellFormula = oldCell.CellFormula;
                        break;
                    case CellType.Numeric:
                        newCell.SetCellValue(oldCell.NumericCellValue);
                        break;
                    case CellType.String:
                        newCell.SetCellValue(oldCell.RichStringCellValue);
                        break;
                    case CellType.Unknown:
                        newCell.SetCellValue(oldCell.StringCellValue);
                        break;
                }
            }
            startRow++;
            return this;
        }

        public string getPlusFormula(List<int> sumRowIndex, string letter)
        {

            var formula = sumRowIndex.Aggregate("", (str, index) => str += $"{letter}{index}+");
            return $"={formula}".Remove(formula.Length, 1);
        }
        public string getSumFormula(int startRow, int RowCount, string letter)
        {

            return $"=SUM({letter}{startRow}:{letter}{startRow+RowCount-1})";
        }

        internal void InsertRow(int insertCount, ISheet _sheet = null, int? _startRow = null )
        {
            _sheet = _sheet ?? sheet;
            _startRow = _startRow ?? startRow;

            for (int j = _startRow.Value; j < _startRow+insertCount; j++)
            {


                // If the row exist in destination, push down all rows by 1 else create a new row
                var oldRow = _sheet.GetRow(j ) ?? _sheet.CreateRow(j);
                if (oldRow != null)
                {
                    _sheet.ShiftRows(j, _sheet.LastRowNum, 1, true, false);
                    _sheet.CreateRow(j);
                }
                else
                {
                    oldRow = _sheet.CreateRow(j);
                }
                var newRow = _sheet.CreateRow(j);
                //var sourceRow = sheet.GetRow(startRow) ;
                //var targetRow = sheet.GetRow(startRow + j+1) ?? sheet.CreateRow(startRow + j+1);
                //int i;
                for (int i = 0; i < oldRow.LastCellNum; i++)
                {
                    // Grab a copy of the old/new cell
                    ICell newCell = newRow.GetCell(i) ?? newRow.CreateCell(i);
                    ICell oldCell = oldRow.GetCell(i) ?? oldRow.CreateCell(i);


                    var newCellStyle = sheet.Workbook.CreateCellStyle();
                    newCellStyle.CloneStyleFrom(oldCell.CellStyle);
                    newCell.CellStyle = newCellStyle;


                }
            }
        }
        public void insertRange(CellRangeAddress range, ISheet sourceSheet, int? destStartRow = null, ISheet destSheet = null)
        {

            destSheet = destSheet ?? sheet;
            int row = destStartRow ?? startRow;
            InsertRow(range.LastRow - range.FirstRow + 1, destSheet, destStartRow);



            //合併儲存格轉移
            sourceSheet.MergedRegions.ForEach(region => destSheet.AddMergedRegion(
                new CellRangeAddress(
                    row,
                    row + region.LastRow - region.FirstRow ,
                    region.FirstColumn,
                    region.LastColumn
            )));

            for (var rowNum = range.FirstRow; rowNum <= range.LastRow; rowNum++)
            {



                IRow sourceRow = sourceSheet.GetRow(rowNum);
                
                if (destSheet.GetRow(row) == null)
                    destSheet.CreateRow(row);

                if (sourceRow != null)
                {
                    IRow destinationRow = destSheet.GetRow(row);

                    for (var col = range.FirstColumn; col < sourceRow.LastCellNum && col <= range.LastColumn; col++)
                    {
    
                        destinationRow.CreateCell(col);
                        CopyCell(sourceSheet, sourceRow.GetCell(col), destinationRow.GetCell(col));
                    }
                }
                row++;
            }
        }
        public ISheet getSheet(int index)
        {
            return sheet.Workbook.GetSheetAt(index);
        }

        void CopyCell(ISheet sourceSheet, ICell source, ICell destination)
        {
            if (destination != null && source != null)
            {
                //you can comment these out if you don't want to copy the style ...
                //destination.CellComment = source.CellComment;
                var newCellStyle = sourceSheet.Workbook.CreateCellStyle();
                newCellStyle.CloneStyleFrom(source.CellStyle);
                destination.CellStyle = newCellStyle;
                //destination.Hyperlink = source.Hyperlink;

                switch (source.CellType)
                {
                    case CellType.Formula:
                        destination.CellFormula = source.CellFormula; break;
                    case CellType.Numeric:
                        destination.SetCellValue(source.NumericCellValue); break;
                    case CellType.String:
                        destination.SetCellValue(source.StringCellValue); break;
                }
            }
        }


        internal void evaluateSheet(int i)
        {
            sheet.Workbook.SetActiveSheet(i);
            sheet.ForceFormulaRecalculation = true;
        }

        public MemoryStream getConvertedTemplateStream()
        {
            var ms = new NpoiMemoryStream();
            sheet.Workbook.Write(ms);

            var asposeWorkBook = new Workbook(ms);
            string tempfilePath = Path.Combine(Utils.GetTempFolder(), "temp");
            asposeWorkBook.Save(tempfilePath, SaveFormat.Ods);
            using (var returnMS = new NpoiMemoryStream())
            {
                using (var fileMS = new FileStream(tempfilePath, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    fileMS.CopyTo(returnMS);
                }

                ms.AllowClose = true;
                ms.Close();
                return returnMS;
            }


            

        }

    }
}