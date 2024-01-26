using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class CEExportController : Controller
    {//碳排量計算-匯出 s20230531
        CEExportService iService = new CEExportService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View();
        }
        //標案年分
        public JsonResult GetYearOptions()
        {
            List<EngYearVModel> items = iService.GetEngYearList();

            return Json(items);
        }
        //碳報表-總表 XML
        public ActionResult dnCEExport(int year)
        {//s20230630
            string filename = Utils.GetTempFile(".xml");
            System.Diagnostics.Debug.WriteLine("read SQL: " + DateTime.Now.ToLocalTime());
            List<CarbonEmissionPayItemV3Model> items = iService.GetList<CarbonEmissionPayItemV3Model>(year);
            System.Diagnostics.Debug.WriteLine("read finish: " + DateTime.Now.ToLocalTime());
            var csv = new StringBuilder();
            try
            {
                csv.AppendLine(@"<?xml version='1.0'?>
<?mso-application progid='Excel.Sheet'?>
<Workbook xmlns='urn:schemas-microsoft-com:office:spreadsheet' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:x='urn:schemas-microsoft-com:office:excel'
    xmlns:ss='urn:schemas-microsoft-com:office:spreadsheet' xmlns:html='http://www.w3.org/TR/REC-html40'>
    <OfficeDocumentSettings xmlns='urn:schemas-microsoft-com:office:office'><AllowPNG/></OfficeDocumentSettings>
    <ExcelWorkbook xmlns='urn:schemas-microsoft-com:office:excel'>
        <WindowHeight>11610</WindowHeight>
        <WindowWidth>24555</WindowWidth>
        <WindowTopX>720</WindowTopX>
        <WindowTopY>360</WindowTopY>
        <ProtectStructure>False</ProtectStructure>
        <ProtectWindows>False</ProtectWindows>
    </ExcelWorkbook>
    <Styles>
        <Style ss:ID='Default' ss:Name='Normal'>
            <Alignment ss:Vertical='Center'/>
            <Borders/>
            <Font ss:FontName='新細明體' x:CharSet='136' x:Family='Roman' ss:Size='12' ss:Color='#000000'/>
            <Interior/>
            <NumberFormat/>
            <Protection/>
        </Style>
        <Style ss:ID='s62'><NumberFormat ss:Format='@'/></Style>
    </Styles>
    <Worksheet ss:Name='清單'>
        <Table ss:ExpandedColumnCount='17' x:FullColumns='1' x:FullRows='1' ss:DefaultColumnWidth='54' ss:DefaultRowHeight='16.5'>
        <Column ss:StyleID='s62' ss:AutoFitWidth='0' ss:Width='144'/>
        <Column ss:StyleID='s62' ss:AutoFitWidth='0' ss:Width='150'/>
        <Column ss:StyleID='s62' ss:AutoFitWidth='0' ss:Width='248.25'/>
        <Column ss:StyleID='s62' ss:AutoFitWidth='0' ss:Width='84'/>
        <Column ss:StyleID='s62' ss:AutoFitWidth='0' ss:Width='35.25'/>
        <Column ss:StyleID='s62' ss:AutoFitWidth='0' ss:Width='224.25'/>
        <Column ss:AutoFitWidth='0' ss:Width='83.25'/>
        <Column ss:AutoFitWidth='0' ss:Width='84.75'/>
        <Column ss:Index='10' ss:Width='54.75'/>
        <Column ss:Index='12' ss:AutoFitWidth='0' ss:Width='83.25'/>
        <Column ss:StyleID='s62' ss:AutoFitWidth='0' ss:Width='114'/>
        <Column ss:StyleID='s62' ss:AutoFitWidth='0' ss:Width='114'/>
        <Column ss:StyleID='s62' ss:AutoFitWidth='0' ss:Width='166.5'/>
        <Column ss:StyleID='s62' ss:AutoFitWidth='0' ss:Width='146.25'/>
        <Column ss:StyleID='s62' ss:AutoFitWidth='0' ss:Width='126'/>
        <Row ss:AutoFitHeight='0'>
            <Cell><Data ss:Type='String'>機關/單位</Data></Cell>
            <Cell><Data ss:Type='String'>標案編號</Data></Cell>
            <Cell><Data ss:Type='String'>標案名稱</Data></Cell>
            <Cell><Data ss:Type='String'>工程項目代號</Data></Cell>
            <Cell><Data ss:Type='String'>項次</Data></Cell>
            <Cell><Data ss:Type='String'>項目及說明</Data></Cell>
            <Cell><Data ss:Type='String'>單價</Data></Cell>
            <Cell><Data ss:Type='String'>金額</Data></Cell>
            <Cell><Data ss:Type='String'>單位</Data></Cell>
            <Cell><Data ss:Type='String'>數量</Data></Cell>
            <Cell><Data ss:Type='String'>碳排系數</Data></Cell>
            <Cell><Data ss:Type='String'>工項碳排放量</Data></Cell>
            <Cell><Data ss:Type='String'>編碼</Data></Cell>
            <Cell><Data ss:Type='String'>修改說明</Data></Cell>
            <Cell><Data ss:Type='String'>結果</Data></Cell>
            <Cell><Data ss:Type='String'>綠色經費類型</Data></Cell>
            <Cell><Data ss:Type='String'>綠色經費修改說明</Data></Cell>
        </Row>");
                string result;
                foreach (CarbonEmissionPayItemV3Model m in items)
                {
                    csv.AppendLine(@"        <Row ss:AutoFitHeight='0'>");
                    if (String.IsNullOrEmpty(m.RStatus))
                        result = m.RStatusCodeStr.Replace("<font color=\"red\">", "").Replace("</font>", "").Replace("<br>", ",");
                    else
                        result = m.RStatus;
                    csv.AppendLine(String.Format(@"         <Cell><Data ss:Type='String'>{0}</Data></Cell>", m.execUnitName));
                    csv.AppendLine(String.Format(@"         <Cell><Data ss:Type='String'>{0}</Data></Cell>", m.EngNo));
                    csv.AppendLine(String.Format(@"         <Cell><Data ss:Type='String'>{0}</Data></Cell>", m.EngName));
                    csv.AppendLine(String.Format(@"         <Cell><Data ss:Type='String'>{0}</Data></Cell>", m.PayItem));
                    csv.AppendLine(String.Format(@"         <Cell><Data ss:Type='String'>{0}</Data></Cell>", m.ItemNo));
                    csv.AppendLine(String.Format(@"         <Cell><Data ss:Type='String'>{0}</Data></Cell>", xmlCheck(m.Description)));
                    csv.AppendLine(String.Format(@"         <Cell><Data ss:Type='Number'>{0}</Data></Cell>", m.Price));
                    csv.AppendLine(String.Format(@"         <Cell><Data ss:Type='Number'>{0}</Data></Cell>", m.Amount));
                    csv.AppendLine(String.Format(@"         <Cell><Data ss:Type='String'>{0}</Data></Cell>", m.Unit));
                    csv.AppendLine(String.Format(@"         <Cell><Data ss:Type='Number'>{0}</Data></Cell>", m.Quantity));
                    csv.AppendLine(String.Format(@"         <Cell><Data ss:Type='Number'>{0}</Data></Cell>", m.KgCo2e));
                    csv.AppendLine(String.Format(@"         <Cell><Data ss:Type='Number'>{0}</Data></Cell>", m.ItemKgCo2e));
                    csv.AppendLine(String.Format(@"         <Cell><Data ss:Type='String'>{0}</Data></Cell>", m.RefItemCode));
                    csv.AppendLine(String.Format(@"         <Cell><Data ss:Type='String'>{0}</Data></Cell>", xmlCheck(m.Memo)));
                    csv.AppendLine(String.Format(@"         <Cell><Data ss:Type='String'>{0}</Data></Cell>", xmlCheck(result)));
                    csv.AppendLine(String.Format(@"         <Cell><Data ss:Type='String'>{0}</Data></Cell>", xmlCheck(m.GreenItemName)));
                    csv.AppendLine(String.Format(@"         <Cell><Data ss:Type='String'>{0}</Data></Cell>", xmlCheck(m.GreenFundingMemo)));
                    csv.AppendLine(@"        </Row>");
                }
                csv.AppendLine(@"       </Table>
        <WorksheetOptions xmlns='urn:schemas-microsoft-com:office:excel'>
            <PageSetup>
                <Header x:Margin='0.3'/>
                <Footer x:Margin='0.3'/>
                <PageMargins x:Bottom='0.75' x:Left='0.7' x:Right='0.7' x:Top='0.75'/>
            </PageSetup>
            <Unsynced/>
            <Print>
            <ValidPrinterInfo/>
            <PaperSizeIndex>9</PaperSizeIndex>
            <HorizontalResolution>600</HorizontalResolution>
            <VerticalResolution>600</VerticalResolution>
            </Print>
            <Selected/>
            <Panes>
                <Pane>
                    <Number>3</Number>
                    <ActiveRow>15</ActiveRow>
                    <ActiveCol>13</ActiveCol>
                </Pane>
            </Panes>
            <ProtectObjects>False</ProtectObjects>
            <ProtectScenarios>False</ProtectScenarios>
        </WorksheetOptions>
    </Worksheet>
</Workbook>");
                System.IO.File.WriteAllText(filename, csv.ToString());

                System.Diagnostics.Debug.WriteLine("write finish: " + DateTime.Now.ToLocalTime());
                Stream iStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", String.Format("碳排量計算匯出資料-{0}.xml", year));
                //return DownloadFile(filename, eng.EngNo);
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = -1,
                    message = "Excel 製表失敗\n" + e.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        private string xmlCheck(string d)
        {
            if (String.IsNullOrEmpty(d)) return d;
            return d.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
        }

        //碳報表-總表
        /*public ActionResult dnCEExport(int year)
        {
            string filename = Utils.CopyTemplateFile("碳排量計算匯出資料.xlsx", ".xlsx");
            Dictionary<string, Microsoft.Office.Interop.Excel.Worksheet> dict = new Dictionary<string, Microsoft.Office.Interop.Excel.Worksheet>();
            Microsoft.Office.Interop.Excel.Application appExcel = null;
            Microsoft.Office.Interop.Excel.Workbook workbook = null;
            //開啟 Excel 檔案
            CarbonEmissionPayItemV3Model tar = null;
            try
            {
                appExcel = new Microsoft.Office.Interop.Excel.Application();
                workbook = appExcel.Workbooks.Open(filename);

                foreach (Microsoft.Office.Interop.Excel.Worksheet worksheet in workbook.Worksheets)
                {
                    dict.Add(worksheet.Name, worksheet);
                }
                Microsoft.Office.Interop.Excel.Worksheet sheet = dict["清單"];
                System.Diagnostics.Debug.Write(DateTime.Now.ToLocalTime());
                List<CarbonEmissionPayItemV3Model> items = iService.GetList<CarbonEmissionPayItemV3Model>(year);
                System.Diagnostics.Debug.Write(DateTime.Now.ToLocalTime());
                string execUnitName = "";
                int row = 2;
                foreach (CarbonEmissionPayItemV3Model m in items)
                {
                    tar = m;
                    //System.Diagnostics.Debug.WriteLine(row);
                    if (m.execUnitName != execUnitName)
                    {
                        
                        execUnitName = m.execUnitName;
                    }
                    sheet.Cells[row, 1] = m.execUnitName;
                    sheet.Cells[row, 2] = m.EngNo;
                    sheet.Cells[row, 3] = m.EngName;
                    sheet.Cells[row, 4] = m.PayItem;
                    sheet.Cells[row, 5] = m.ItemNo;
                    sheet.Cells[row, 6] = m.Description;
                    sheet.Cells[row, 7] = m.Price;
                    sheet.Cells[row, 8] = m.Amount;
                    sheet.Cells[row, 9] = m.Unit;
                    sheet.Cells[row, 10] = m.Quantity;
                    sheet.Cells[row, 11] = m.KgCo2e;
                    sheet.Cells[row, 12] = m.ItemKgCo2e;
                    sheet.Cells[row, 13] = m.RefItemCode;
                    sheet.Cells[row, 14] = m.Memo;
                    if (String.IsNullOrEmpty(m.RStatus))
                        sheet.Cells[row, 15] = m.RStatusCodeStr.Replace("<font color=\"red\">", "").Replace("</font>", "").Replace("<br>", "\n");
                    else
                        sheet.Cells[row, 15] = m.RStatus;
                    sheet.Cells[row, 16] = m.GreenItemName;
                    sheet.Cells[row, 17] = m.GreenFundingMemo;
                    row++;
                }
                

                workbook.Save();
                workbook.Close();
                appExcel.Quit();
                System.Diagnostics.Debug.Write(DateTime.Now.ToLocalTime());
                Stream iStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", String.Format("碳排量計算匯出資料-{0}.xlsx", year));
                //return DownloadFile(filename, eng.EngNo);
            }
            catch (Exception e)
            {
                if (workbook != null) workbook.Close(false);
                if (appExcel != null) appExcel.Quit();
                System.Diagnostics.Debug.WriteLine(Json(tar).ToString());
                return Json(new
                {
                    result = -1,
                    message = "Excel 製表失敗\n" + e.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }*/
    }
}