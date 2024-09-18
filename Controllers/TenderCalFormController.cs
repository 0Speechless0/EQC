using EQC.Common;
using EQC.Services;
using EQC.ViewModel;
using Newtonsoft.Json;
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
        public void Download(string itemsStr, string year)
        {
            var items = JsonConvert.DeserializeObject<List<TenderCalForm>>(itemsStr);
            string srcFileName = Path.Combine(Utils.GetTemplateFilePath(), "管制填報表詳細.xlsx");
            string tempFileName = Path.Combine(Utils.GetTempFile("管制填報表詳細.xlsx"));

            XSSFWorkbook workbook;
            //copyFile 
            System.IO.File.Copy(srcFileName, tempFileName);

            workbook = new XSSFWorkbook(tempFileName);



            //開啟 Excel 檔案
            try
            {

                var sheet = workbook.GetSheetAt(1);

                int row = 3, bkChange = 0, inx = 0;
                string execUnitName = "";
                Microsoft.Office.Interop.Excel.Range excelRange;
                
                sheet.GetRow(0).Cells[0].SetCellValue($"{year}年度發包工程數位轉型填報情形明細表");
                foreach (TenderCalForm m in items)
                {
                    int MaterialSummaryTotal = tenderCalService.GetMeterailSummaryTotal(m.EngNo); ;
                    inx++;
                    sheet.GetRow(row).Cells[1].SetCellValue(inx);
                    sheet.GetRow(row).Cells[2].SetCellValue(m.ExecUnitName);
                    sheet.GetRow(row).Cells[4].SetCellValue(m.EngName);
                    sheet.GetRow(row).Cells[7].SetCellValue(m.AwardDate.ToChsDate() );
                    sheet.GetRow(row).Cells[8].SetCellValue(m.createEng >0 ? "已完成" :"未完成" );
                    sheet.GetRow(row).Cells[9].SetCellValue(m.CECheckResult);
                    sheet.GetRow(row).Cells[10].SetCellValue(m.DismantlingRate+"%");
                    sheet.GetRow(row).Cells[11].SetCellValue(m.ReductionResult);
                    sheet.GetRow(row).Cells[12].SetCellValue($"{m.MaterialSummaryNum}/{m.MaterialSummaryTotal}");
                    sheet.GetRow(row).Cells[13].SetCellValue(
                        MaterialSummaryTotal == 0 ? "已完成" : m.MetarialAddrResult);
                    sheet.GetRow(row).Cells[14].SetCellValue(
                        $"{m.MachineLoadingResult}/{m.FillConstructionDayShouldNum}/{m.FillDayTotal}");
                    sheet.GetRow(row).Cells[15].SetCellValue($"{m.FillConstructionDayNum}/{m.FillConstructionDayShouldNum}/{m.FillDayTotal}");
                    sheet.GetRow(row).Cells[16].SetCellValue($"{m.FillSupervisionDayNum}/{m.FillSupervisionDayShouldNum}/{m.FillDayTotal}");
                    sheet.GetRow(row).Cells[17].SetCellValue(m.EnergySavingCarbonResult != null ?
                        Int32.TryParse(m.EnergySavingCarbonResult, out int result ) ?
                            $"{result}/{m.energySavingCheckMustNum}/{m.energySavingCheckShouldNum}" : m.EnergySavingCarbonResult
                        : "");
                    sheet.GetRow(row).Cells[18].SetCellValue($"{m.constCheckShouldNum}/{m.neededConstCheckNum}");

                    //for (int i = 0; i < 13; i++)
                    //{
                    //    // set color
                    //    XSSFCellStyle cellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
                    //    cellStyle.SetFillForegroundColor(new XSSFColor(new byte[3] { 200, 226, 232 }));
                    //    cellStyle.FillPattern = FillPattern.SolidForeground;
                    //    sheet.GetRow(row).Cells[i].CellStyle = cellStyle;
                    //}
                    
                    sheet.copyRowToNext(row);
                    row++;
                }


                row = 6;


                sheet = workbook.GetSheetAt(0);
                sheet.GetRow(0).Cells[0].SetCellValue($"{year}年度發包工程數位轉型填報情形統計總表");
                TenderCalForm total = new TenderCalForm();
                var groupedItem = items.GroupBy(r => r.ExecUnitName);
                sheet.GetRow(1).Cells[0].SetCellValue($"資料統計截止日期：{DateTime.Now.Year - 1911}年{DateTime.Now.Month }月{DateTime.Now.Day}日");
                foreach (var m in groupedItem)
                {
                    sheet.GetRow(row).Cells[0].SetCellValue(m.Key);
                    sheet.GetRow(row).Cells[1].SetCellValue(m.Where(r => r.awardStatus > 0).Count());
                    sheet.GetRow(row).Cells[2].SetCellValue(m.Sum(r => r.createEng));
                    sheet.GetRow(row).Cells[3].SetCellValue(m.Where(r => r.CECheckResult != "未開工").Count());
                    sheet.GetRow(row).Cells[4].SetCellValue(m.Where(r => r.DismantlingRate >= 70).Count());
                    sheet.GetRow(row).Cells[5].SetCellValue(m.Where(r => r.ReductionResult != "未開工").Count());
                    sheet.GetRow(row).Cells[6].SetCellValue(m.Where(r => r.MaterialSummaryNum == r.MaterialSummaryTotal).Count());
                    sheet.GetRow(row).Cells[7].SetCellValue(m.Where(r => r.MetarialAddrResult != "未開工").Count());
                    sheet.GetRow(row).Cells[8].SetCellValue(
                        m.Where(
                            r => Int32.TryParse(r.MachineLoadingResult, out int result) ? result > 0 : false ).Count()
                        );
                    sheet.GetRow(row).Cells[9].SetCellValue(m.Where(r => r.FillConstructionDayNum > 0).Count());
                    sheet.GetRow(row).Cells[10].SetCellValue(m.Where(r => r.FillSupervisionDayNum > 0).Count());
                    sheet.GetRow(row).Cells[11].SetCellValue(m.Where(r => r.EnergySavingCarbonResult!= null ? Int32.Parse(r.EnergySavingCarbonResult) > 0 : false).Count());
                    sheet.GetRow(row).Cells[12].SetCellValue(m.Where(r => r.constCheckShouldNum > 0).Count());
                    sheet.GetRow(row).Cells[13].SetCellValue("");
                    //for (int i = 0; i < 8; i++)
                    //{
                    //    // set color
                    //    XSSFCellStyle cellStyle = (XSSFCellStyle)workbook.CreateCellStyle();

                    //    cellStyle.BorderBottom = BorderStyle.Thin;
                    //    cellStyle.BorderLeft = BorderStyle.Thin;
                    //    cellStyle.BorderRight = BorderStyle.Thin;
                    //    cellStyle.BorderTop = BorderStyle.Thin;
                    //    sheet.GetRow(row).Cells[i].CellStyle = cellStyle;
                    //}


                    sheet.ShiftRows(row+1, sheet.LastRowNum, 1, true, false);
                    sheet.copyRowToNext(row);
                    row++;

                }
                sheet.GetRow(row).Cells[0].SetCellValue("合計");
                sheet.GetRow(row).Cells[1].SetCellFormula($"SUM(B7:B{7 + groupedItem.Count() - 1})");
                sheet.GetRow(row).Cells[2].SetCellFormula($"SUM(C7:C{7 + groupedItem.Count() - 1})");
                sheet.GetRow(row).Cells[3].SetCellFormula($"SUM(D7:D{7 + groupedItem.Count() - 1})");
                sheet.GetRow(row).Cells[4].SetCellFormula($"SUM(E7:E{7 + groupedItem.Count() - 1})");
                sheet.GetRow(row).Cells[5].SetCellFormula($"SUM(F7:F{7 + groupedItem.Count() - 1})");
                sheet.GetRow(row).Cells[6].SetCellFormula($"SUM(G7:G{7 + groupedItem.Count() - 1})");
                sheet.GetRow(row).Cells[7].SetCellFormula($"SUM(H7:H{7 + groupedItem.Count() - 1 })");
                sheet.GetRow(row).Cells[8].SetCellFormula($"SUM(I7:I{7 + groupedItem.Count() - 1 })");
                sheet.GetRow(row).Cells[9].SetCellFormula($"SUM(J7:J{7 + groupedItem.Count() - 1 })");
                sheet.GetRow(row).Cells[10].SetCellFormula($"SUM(K7:K{7 + groupedItem.Count() - 1 })");
                sheet.GetRow(row).Cells[11].SetCellFormula($"SUM(L7:L{7 + groupedItem.Count() - 1 })");
                sheet.GetRow(row).Cells[12].SetCellFormula($"SUM(M7:M{7 + groupedItem.Count() - 1 })");

                sheet.ForceFormulaRecalculation = true;
                //for (int i = 0; i < 8; i++)
                //{
                //    // set color
                //    XSSFCellStyle cellStyle = (XSSFCellStyle)workbook.CreateCellStyle();

                //    cellStyle.BorderBottom = BorderStyle.Medium;
                //    cellStyle.BorderLeft = BorderStyle.Medium;
                //    cellStyle.BorderRight = BorderStyle.Medium;
                //    cellStyle.BorderTop = BorderStyle.Medium;
                //    sheet.GetRow(row).Cells[i].CellStyle = cellStyle;
                //}

                using (var ms = new MemoryStream())
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
        public void GetList(int year, int uId, bool update = false)
        {


            string tempPath = Utils.GetTempFile(".json", $"eqc_tender_cal_form_{year}");
            if(System.IO.File.Exists(tempPath) && !update)
            {
                var resultLightly = ReadFromJsonFile<TenderCalForm>(tempPath)
                        .Where(r => r.EngYear == year || year == -1)
                        .Where(r => r.ExecUnitSeq == uId || uId == -1)
                        .ToList();
                if(resultLightly.Count() > 0 )
                {
                    ResponseJson(resultLightly
                    );
                    return;
                }

            }

            var engList = new CarbonEmissionFactorService().GetCarbonControlTable<CarbonEmissionCTVModel>(year, uId);

            engList
                .GetValueByJoin(tenderCalService.GetEngCECheckResult(year),
                    (row) => row.EngNo,
                    (row, joinValue) =>
                    {
                        row.CECheckResult = joinValue;
                    })
                .GetValueByJoin(tenderCalService.GetEngEnergySavingCarbonResult(year),
                    (row) => row.EngNo,
                    (row, joinValue) =>
                    {
                        row.EnergySavingCarbonResult = joinValue;
                    })
                .GetValueByJoin(tenderCalService.GetEngMachineLoadingResult(year),
                    (row) => row.EngNo,
                    (row, joinValue) =>
                    {
                        row.MachineLoadingResult = joinValue;
                    })
                .GetValueByJoin(tenderCalService.GetEngMetarialAddrResult(year),
                    (row) => row.EngNo,
                    (row, joinValue) =>
                    {
                        row.MetarialAddrResult = joinValue;
                    })
                .GetValueByJoin(tenderCalService.GetEngReductionResult(year),
                    (row) => row.EngNo,
                    (row, joinValue) =>
                    {
                        row.ReductionResult = joinValue;
                    })
                ;
            var result = engList.Select(r =>
            {
                int meterailSummaryTotal = tenderCalService.GetMeterailSummaryTotal(r.EngNo);
                int meterailTestTotal = tenderCalService.GetMeterailTestTotal(r.EngNo);
                decimal? Co2Total = null;
                decimal? Co2ItemTotal = null;
                tenderCalService.GetCarbonCo2Total(r.EngNo, ref Co2Total, ref Co2ItemTotal);
                var constructionDayNum = tenderCalService.GetReportFillCount(r.EngNo, 2);
                var supervisionDayNum = tenderCalService.GetReportFillCount(r.EngNo, 1);
                DateTime?[] engDate = TenderCalFormService.EngNoDic[r.EngNo].GetEngDate();
                int EnergySavingCheckShouldNum = 0;
                int EnergySavingCheckMustNum = 0;
                if (engDate[0] != null && engDate[1] != null)
                {
                    EnergySavingCheckShouldNum =
                    ((engDate[1].Value.Year - engDate[0].Value.Year) * 12) + engDate[1].Value.Month - engDate[0].Value.Month + 1;
                }
                if (engDate[0] != null)
                {
                    EnergySavingCheckMustNum =
                    ((DateTime.Now.Year - engDate[0].Value.Year) * 12) + DateTime.Now.Month - engDate[0].Value.Month + 1;
                    EnergySavingCheckMustNum = EnergySavingCheckMustNum > EnergySavingCheckShouldNum
                    ? EnergySavingCheckShouldNum : EnergySavingCheckMustNum;
                }
                decimal dismantlingRate = 0;
                decimal greenRate = 0;
                if (TenderCalFormService.EngNoDic.TryGetValue(r.EngNo, out EDMXModel.EngMain eng))
                {
                    Utils.GetEngDismantlingRate(eng.Seq, (e) => e.CarbonEmissionHeader.FirstOrDefault()?.CarbonEmissionPayItem  , ref  dismantlingRate, ref greenRate);
                }
   
                return new TenderCalForm
                {
                    awardStatus = r.awardStatus,
                    AwardDate = r.AwardDate,
                    TenderNo = r.TenderNo,
                    TenderName = r.TenderName,
                    EngName = r.EngName,
                    EngNo = r.EngNo,
                    EngYear = r.EngYear,
                    ExecUnitSeq = r.ExecUnitSeq,
                    ExecUnitName = r.execUnitName,
                    createEng = r.createEng,
                    CECheckResult = r.CECheckResult,
                    EnergySavingCarbonResult = r.EnergySavingCarbonResult,
                    MachineLoadingResult = r.MachineLoadingResult,
                    MetarialAddrResult = meterailSummaryTotal == 0 ? "已完成" : r.MetarialAddrResult,
                    ReductionResult = r.ReductionResult,
                    TotalKgCo2e = Co2Total ?? 0,
                    DismantlingRate = (int)dismantlingRate,
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
                    constCheckShouldNum = tenderCalService.GetContCheckShouldNum(r.EngNo),
                    energySavingCheckShouldNum = EnergySavingCheckShouldNum,
                    energySavingCheckMustNum = EnergySavingCheckMustNum
                };
            });
            Dictionary<string, TenderCalForm> EngsFromJsonDic =  null;
            if (System.IO.File.Exists(tempPath))
            {
                    EngsFromJsonDic = ReadFromJsonFile<TenderCalForm>(tempPath).ToDictionary(r => r.EngNo, r => r);

                result.ToList()
                    .ForEach(e => {
                        if (EngsFromJsonDic.ContainsKey(e.EngNo))
                        {
                            EngsFromJsonDic[e.EngNo] = e;
                        }
                        else
                        {
                            EngsFromJsonDic.Add(e.EngNo, e);
                        }
                    });
            }

            var mergedResult = EngsFromJsonDic?.Select(r => r.Value) ?? result.ToList();
            WriteToJsonFile<TenderCalForm>(mergedResult
                , tempPath);
            ResponseJson(result.ToList()
            );
            

        }
    }
}