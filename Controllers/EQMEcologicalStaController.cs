using EQC.Common;
using EQC.Services;
using EQC.ViewModel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EQMEcologicalStaController : Controller
    {//生態檢核統計
        EQMEcologicalStaService iService = new EQMEcologicalStaService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }

        //清單
        public JsonResult GetRecords(int mode)
        {
            if (mode == 1)
            {
                return Json(new
                {
                    result = 0,
                    items = iService.GetPlanList()
                });
            } else
            {
                return Json(new
                {
                    result = 0,
                    items = iService.GetExecList()
                });
            }
        }
        public ActionResult Download(int mode)
        {
            string filename; // String.Format("{0} 公共工程施工日誌[{1}].docx", eng.EngNo, supDailyDate.ItemDateStr);
            List<EQMEcologicalStaVModel> items;
            if (mode == 1)
            {
                filename = "規劃設計階段之工程案件-生態檢核.docx";
                items = iService.GetPlanList();
            }
            else
            {
                filename = "施工階段之工程案件-生態檢核.docx";
                items = iService.GetExecList();
            }

            Microsoft.Office.Interop.Word.Application wordApp = null;
            Document doc = null;
            try
            {
                string tarfile = Utils.CopyTemplateFile(filename, ".docx");
                wordApp = new Microsoft.Office.Interop.Word.Application();
                doc = wordApp.Documents.Open(tarfile);
                Table table = doc.Tables[1];
                table.Cell(1, 2).Range.Text = String.Format("{0}", DateTime.Now.Year-1911);

                int row = 4;
                int itemCnt = items.Count;
                Row templateRow = table.Rows[row];
                templateRow.Range.Copy();
                for (int i = 1; i < items.Count; i++)
                {
                    templateRow.Range.Paste();
                }
                itemCnt = 0;
                foreach (EQMEcologicalStaVModel m in items)
                {
                    table.Cell(row + itemCnt, 1).Range.Text = m.execUnitName;
                    table.Cell(row + itemCnt, 2).Range.Text = String.Format("{0}", m.engCount);
                    table.Cell(row + itemCnt, 3).Range.Text = String.Format("{0}", m.notChcek);
                    table.Cell(row + itemCnt, 4).Range.Text = String.Format("{0}", m.needChcek);
                    table.Cell(row + itemCnt, 5).Range.Text = String.Format("{0}", m.execChcek);
                    table.Cell(row + itemCnt, 6).Range.Text = String.Format("{0}", m.lostChcek);
                    table.Cell(row + itemCnt, 10).Range.Text = String.Format("{0:0.##}", m.lostChcekRate);
                    itemCnt++;
                }

                /*table.Cell(1, 4).Range.Text = tender.ContractorName1;
                table.Cell(2, 2).Range.Text = String.Format("{0}天", tender.TotalDays);
                table.Cell(2, 4).Range.Text = String.Format("{0}天", supDailyDate.dailyCount);
                table.Cell(2, 6).Range.Text = String.Format("{0}天", tender.TotalDays - supDailyDate.dailyCount);
                table.Cell(3, 2).Range.Text = tender.ActualStartDate;
                table.Cell(3, 4).Range.Text = tender.ScheCompletionDate;
                string check = "□有 □無";
                if (daily.miscList[0].IsFollowSkill.HasValue)
                {
                    if (daily.miscList[0].IsFollowSkill.Value)
                        check = "■有 □無";
                    else
                        check = "□有 ■無";
                }
                table.Cell(17, 1).Range.Text = String.Format("四、本日施工項目是否有須依「營造業專業工程特定施工項目應置之技術士種類、比率或人數標準表」規定應設置技術士之專業工程：{0}（此項如勾選”有”，則應填寫後附「公共工程施工日誌之技術士簽章表」）", check);

                check = "□有 □無";
                if (daily.miscList[0].SafetyHygieneMatters01.HasValue)
                {
                    if (daily.miscList[0].SafetyHygieneMatters01.Value)
                        check = "■有 □無";
                    else
                        check = "□有 ■無";
                }
                table.Cell(19, 1).Range.Text = String.Format("   1.實施勤前教育(含工地預防災變及危害告知)：{0}", check);

                check = "□有 □無 □無新進勞工";
                if (daily.miscList[0].SafetyHygieneMatters02.HasValue)
                {
                    if (daily.miscList[0].SafetyHygieneMatters02.Value == 0)
                        check = "■有 □無 □無新進勞工";
                    else if (daily.miscList[0].SafetyHygieneMatters02.Value == 1)
                        check = "□有 ■無 □無新進勞工";
                    else if (daily.miscList[0].SafetyHygieneMatters02.Value == 2)
                        check = "□有 □無 ■無新進勞工";
                }
                table.Cell(20, 1).Range.Text = String.Format("   2.確認新進勞工是否提報勞工保險(或其他商業保險)資料及安全衛生教育訓練紀錄：{0}", check);

                check = "□有 □無";
                if (daily.miscList[0].SafetyHygieneMatters03.HasValue)
                {
                    if (daily.miscList[0].SafetyHygieneMatters03.Value)
                        check = "■有 □無";
                    else
                        check = "□有 ■無";
                }
                table.Cell(21, 1).Range.Text = String.Format("   3.檢查勞工個人防護具：{0}", check);
                table.Cell(23, 2).Range.Text = daily.miscList[0].SafetyHygieneMattersOther;
                table.Cell(25, 1).Range.Text = daily.miscList[0].SamplingTest;
                table.Cell(27, 1).Range.Text = daily.miscList[0].NoticeManufacturers;
                table.Cell(29, 1).Range.Text = daily.miscList[0].ImportantNotes;
                //三、工地人員及機具管理
                int row = 16;
                int itemCnt = daily.personList.Count;
                if (itemCnt < daily.equipmentList.Count)
                    itemCnt = daily.equipmentList.Count;
                Row templateRow = table.Rows[row];
                templateRow.Range.Copy();
                for (int i = 1; i < itemCnt; i++)
                {
                    templateRow.Range.Paste();
                }
                itemCnt = 0;
                foreach (SupDailyReportConstructionPersonModel m in daily.personList)
                {
                    table.Cell(row + itemCnt, 1).Range.Text = m.KindName;
                    table.Cell(row + itemCnt, 2).Range.Text = m.TodayQuantity.ToString();
                    table.Cell(row + itemCnt, 3).Range.Text = m.AccQuantity.ToString();
                    itemCnt++;
                }
                itemCnt = 0;
                foreach (SupDailyReportConstructionEquipmentModel m in daily.equipmentList)
                {
                    table.Cell(row + itemCnt, 4).Range.Text = m.EquipmentName + " " + m.EquipmentModel;
                    table.Cell(row + itemCnt, 5).Range.Text = m.TodayQuantity.ToString();
                    table.Cell(row + itemCnt, 6).Range.Text = m.AccQuantity.ToString();
                    itemCnt++;
                }
                //二、工地材料管理概況
                row = 13;
                itemCnt = daily.materialList.Count;
                templateRow = table.Rows[row];
                templateRow.Range.Copy();
                for (int i = 1; i < itemCnt; i++)
                {
                    templateRow.Range.Paste();
                }
                itemCnt = 0;
                foreach (SupDailyReportConstructionMaterialModel m in daily.materialList)
                {
                    table.Cell(row + itemCnt, 1).Range.Text = m.MaterialName;
                    table.Cell(row + itemCnt, 2).Range.Text = m.Unit;
                    table.Cell(row + itemCnt, 3).Range.Text = m.ContractQuantity.ToString();
                    table.Cell(row + itemCnt, 4).Range.Text = m.TodayQuantity.ToString();
                    table.Cell(row + itemCnt, 5).Range.Text = m.AccQuantity.ToString();
                    table.Cell(row + itemCnt, 6).Range.Text = m.Memo;
                    itemCnt++;
                }
                //一、依施工計畫書執行按圖施工概況
                row = 7;
                itemCnt = daily.planOverviewList.Count;
                templateRow = table.Rows[row];
                templateRow.Range.Copy();
                for (int i = 1; i < itemCnt; i++)
                {
                    templateRow.Range.Paste();
                }
                itemCnt = 0;
                foreach (EPCSupPlanOverviewVModel m in daily.planOverviewList)
                {
                    table.Cell(row + itemCnt, 1).Range.Text = m.Description;
                    table.Cell(row + itemCnt, 2).Range.Text = m.Unit;
                    table.Cell(row + itemCnt, 3).Range.Text = String.Format("{0:0.####}", m.Quantity);
                    table.Cell(row + itemCnt, 4).Range.Text = String.Format("{0:0.####}", m.TodayConfirm);
                    table.Cell(row + itemCnt, 5).Range.Text = String.Format("{0:0.####}", m.TodayConfirm + m.TotalAccConfirm);
                    table.Cell(row + itemCnt, 6).Range.Text = m.Memo;
                    itemCnt++;
                }
                //
                table = doc.Tables[3];
                table.Cell(1, 2).Range.Text = eng.EngName;
                table.Cell(2, 2).Range.Text = tender.ContractorName1;

                if (docMode == 2)
                {
                    filename = filename.Replace(".docx", ".pdf");
                    tarfile = tarfile.Replace(".docx", ".pdf");
                    doc.SaveAs2(tarfile, WdExportFormat.wdExportFormatPDF);

                }*/

                if (doc != null) doc.Close();
                if (wordApp != null) wordApp.Quit();

                Stream iStream = new FileStream(tarfile, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", filename);
            }
            catch (Exception e)
            {
                if (doc != null) doc.Close();
                if (wordApp != null) wordApp.Quit();
                return Json(new
                {
                    result = -1,
                    message = "製表失敗"
                }, JsonRequestBehavior.AllowGet);
            }

            if (doc != null) doc.Close();
            if (wordApp != null) wordApp.Quit();
            return Json(new
            {
                result = -1,
                message = "請求錯誤"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}