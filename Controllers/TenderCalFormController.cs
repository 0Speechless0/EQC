using EQC.Common;
using EQC.Services;
using EQC.ViewModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    public class TenderCalFormController : MyController
    {
        CarbonEmissionFactorService factorService;
        TenderCalFormService tenderCalService;
        // GET: TenderCalForm
        public ActionResult Index()
        {

            return View();
        }
        public TenderCalFormController()
        {
            factorService = new CarbonEmissionFactorService();
            tenderCalService = new TenderCalFormService();
        }
        public void Download(List<TenderCalForm> items)
        {
            
            string srcFileName = Path.Combine(Utils.GetTemplateFilePath(), "水利署碳排管制表(詳細).xlsx");
            string tempFileName = Path.Combine(Utils.GetTempFile("水利署碳排管制表(詳細).xlsx"));

            XSSFWorkbook workbook;
            //copyFile 
            System.IO.File.Copy(srcFileName, tempFileName);

            workbook = new XSSFWorkbook(tempFileName);



            //開啟 Excel 檔案
            try
            {

                var sheet = workbook.GetSheetAt(0);

                int row = 1, bkChange = 0, inx = 0;
                string execUnitName = "";
                Microsoft.Office.Interop.Excel.Range excelRange;
                foreach (TenderCalForm m in items)
                {
                    inx++;
                    var newRow =  sheet.CreateRow(row);
                    for (int i = 0; i < 13; i++) newRow.CreateCell(i);
                    sheet.GetRow(row).Cells[0].SetCellValue(inx);
                    sheet.GetRow(row).Cells[1].SetCellValue(m.ExecUnitName);
                    sheet.GetRow(row).Cells[2].SetCellValue(m.EngNo);
                    sheet.GetRow(row).Cells[3].SetCellValue(m.EngName);
                    sheet.GetRow(row).Cells[4].SetCellValue(m.TenderNo);
                    sheet.GetRow(row).Cells[5].SetCellValue(m.TenderName);
                    sheet.GetRow(row).Cells[6].SetCellValue(m.isBuild);
                    sheet.GetRow(row).Cells[7].SetCellValue($"{m.TotalKgCo2e} ({m.DismantlingRate}%)");
                    sheet.GetRow(row).Cells[8].SetCellValue($"{m.MaterialSummaryNum}/{m.MaterialSummaryTotal}");
                    sheet.GetRow(row).Cells[9].SetCellValue($"{m.MaterialTestNum}/{m.MaterialTestTotal}");
                    sheet.GetRow(row).Cells[10].SetCellValue($"{m.FillConstructionDayNum}/{m.FillConstructionDayShouldNum}/{m.FillDayTotal}");
                    sheet.GetRow(row).Cells[11].SetCellValue($"{m.FillSupervisionDayNum}/{m.FillSupervisionDayShouldNum}/{m.FillDayTotal}");
                    sheet.GetRow(row).Cells[12].SetCellValue($"{m.constCheckShouldNum}/{m.neededConstCheckNum}");

                    for (int i = 0; i < 13; i++)
                    {
                        // set color
                        XSSFCellStyle cellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
                        cellStyle.SetFillForegroundColor(new XSSFColor(new byte[3] { 200, 226, 232 }));
                        cellStyle.FillPattern = FillPattern.SolidForeground;
                        sheet.GetRow(row).Cells[i].CellStyle = cellStyle;
                    }
                    row++;
                }


                row = 1;


                sheet = workbook.GetSheetAt(1);
                TenderCalForm total = new TenderCalForm();
                var groupedItem = items.GroupBy(r => r.ExecUnitName);

                foreach (var m in groupedItem)
                {
                    var newRow = sheet.CreateRow(row);
                    for (int i = 0; i < 8; i++) newRow.CreateCell(i);
                    sheet.GetRow(row).Cells[0].SetCellValue(m.Key);
                    sheet.GetRow(row).Cells[1].SetCellValue(m.Sum(r => r.isBuild));
                    sheet.GetRow(row).Cells[2].SetCellValue(m.Sum(r => (int)r.TotalKgCo2e));
                    sheet.GetRow(row).Cells[3].SetCellValue(m.Where(r => r.MaterialSummaryNum == r.MaterialSummaryTotal).Count());
                    sheet.GetRow(row).Cells[4].SetCellValue(m.Where(r => r.MaterialTestNum == r.MaterialTestTotal).Count());
                    sheet.GetRow(row).Cells[5].SetCellValue(m.Where(r => r.FillConstructionDayNum == r.FillDayTotal).Count());
                    sheet.GetRow(row).Cells[6].SetCellValue(m.Where(r => r.FillSupervisionDayNum == r.FillDayTotal).Count());
                    sheet.GetRow(row).Cells[7].SetCellValue(m.Where(r => r.constCheckShouldNum == r.neededConstCheckNum).Count());
                    for (int i = 0; i < 8; i++)
                    {
                        // set color
                        XSSFCellStyle cellStyle = (XSSFCellStyle)workbook.CreateCellStyle();

                        cellStyle.BorderBottom = BorderStyle.Thin;
                        cellStyle.BorderLeft = BorderStyle.Thin;
                        cellStyle.BorderRight = BorderStyle.Thin;
                        cellStyle.BorderTop = BorderStyle.Thin;
                        sheet.GetRow(row).Cells[i].CellStyle = cellStyle;
                    }
                    row++;
                }
                var sumRow = sheet.CreateRow(row);
                for (int i = 0; i < 8; i++) sumRow.CreateCell(i);
                sheet.GetRow(row).Cells[0].SetCellValue("總計");
                sheet.GetRow(row).Cells[1].SetCellFormula($"SUM(B2:B{2 + groupedItem.Count() -1})");
                sheet.GetRow(row).Cells[2].SetCellFormula($"SUM(C2:C{2 + groupedItem.Count() -1})");
                sheet.GetRow(row).Cells[3].SetCellFormula($"SUM(D2:D{2 + groupedItem.Count() -1})");
                sheet.GetRow(row).Cells[4].SetCellFormula($"SUM(E2:E{2 + groupedItem.Count() -1})");
                sheet.GetRow(row).Cells[5].SetCellFormula($"SUM(F2:F{2 + groupedItem.Count() -1})");
                sheet.GetRow(row).Cells[6].SetCellFormula($"SUM(G2:G{2 + groupedItem.Count() -1})");
                sheet.GetRow(row).Cells[7].SetCellFormula($"SUM(H2:H{2 + groupedItem.Count() -1 })");

                sheet.ForceFormulaRecalculation = true;
                for (int i = 0; i < 8; i++)
                {
                    // set color
                    XSSFCellStyle cellStyle = (XSSFCellStyle)workbook.CreateCellStyle();

                    cellStyle.BorderBottom = BorderStyle.Medium;
                    cellStyle.BorderLeft = BorderStyle.Medium;
                    cellStyle.BorderRight = BorderStyle.Medium;
                    cellStyle.BorderTop = BorderStyle.Medium;
                    sheet.GetRow(row).Cells[i].CellStyle = cellStyle;
                }

                using(var ms = new MemoryStream())
                {
                    workbook.Write(ms);
                    DownloadFile(ms, String.Format("水利署碳排管制表-[{0}].xlsx", DateTime.Now.ToString("yyyy-MM-dd")));
                }
                workbook.Close();
                
                //return DownloadFile(filename, eng.EngNo);
            }
            catch (Exception e)
            {
                if (workbook != null) workbook.Close();
            }
            System.IO.File.Delete(tempFileName);
        }
        public void GetList(int year, int uId)
        {

            var engList  = new CarbonEmissionFactorService().GetCarbonControlTable<CarbonEmissionCTVModel>(year, uId);


            var result = engList.Select(r =>
            {
                int meterailSummaryTotal = tenderCalService.GetMeterailSummaryTotal(r.EngNo);
                int meterailTestTotal = tenderCalService.GetMeterailTestTotal(r.EngNo);
                decimal? Co2Total = null;
                decimal? Co2ItemTotal = null;
                tenderCalService.GetCarbonCo2Total(r.EngNo, ref  Co2Total, ref Co2ItemTotal);
                var constructionDayNum = tenderCalService.GetReportFillCount(r.EngNo, 2);
                var supervisionDayNum = tenderCalService.GetReportFillCount(r.EngNo, 1);
                return new TenderCalForm
                {
                    TenderNo = r.TenderNo,
                    TenderName = r.TenderName,
                    EngName = r.EngName,
                    EngNo = r.EngNo,
                    ExecUnitName = r.execUnitName,
                    isBuild = r.createEng,
                    TotalKgCo2e = Co2Total ?? 0,
                    DismantlingRate = tenderCalService.DismantlingRate(r.EngNo, Co2ItemTotal ?? 0),
                    FillDayTotal = tenderCalService.GetReportCount(r.EngNo),
                    FillConstructionDayNum = constructionDayNum[1],
                    FillSupervisionDayNum = supervisionDayNum[1],
                    FillConstructionDayShouldNum = constructionDayNum[0],
                    FillSupervisionDayShouldNum = supervisionDayNum[0],
                    MaterialTestTotal = meterailTestTotal,
                    MaterialSummaryTotal = meterailSummaryTotal,
                    MaterialSummaryNum = tenderCalService.GetMetrailSummaryNum(r.EngNo, meterailSummaryTotal),
                    MaterialTestNum = tenderCalService.GetMetrailTestNum(r.EngNo, meterailTestTotal),
                    neededConstCheckNum = tenderCalService.GetNeededContCheckNum(r.EngNo),
                    constCheckShouldNum = tenderCalService.GetContCheckShouldNum(r.EngNo)

                };
            });

            ResponseJson(result);
        }
    }
}