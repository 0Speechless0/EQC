using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EPCQualityVerifyController : Controller
    {//工程管理 - 品質查證
        EPCQualityVerifyService iService = new EPCQualityVerifyService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }
        //工程資料表
        public ActionResult DnDoc2(int id)
        {
            List<EPCQualityVerifyDoc2VMode> items = iService.GetDoc2<EPCQualityVerifyDoc2VMode>(id);
            if(items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料讀取錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EPCQualityVerifyDoc2VMode m = items[0];

            Microsoft.Office.Interop.Word.Application wordApp = null;
            Document doc = null;
            string filename = String.Format("{0} 工程施工執行資料表.docx", m.TenderNo);
            try
            {
                string tarfile = Utils.CopyTemplateFile("B-7_工程施工執行資料表_102.12.版.docx", ".docx");
                wordApp = new Microsoft.Office.Interop.Word.Application();
                doc = wordApp.Documents.Open(tarfile);
                Table table = doc.Tables[1];
                table.Cell(2, 2).Range.Text = m.BelongPrj;
                table.Cell(3, 2).Range.Text = m.TenderName;
                table.Cell(4, 2).Range.Text = decimal2Str(m.OutsourcingBudget);
                table.Cell(5, 2).Range.Text = decimal2Str(m.RendBasePrice);
                table.Cell(5, 4).Range.Text = decimal2Str(m.BidAmount);
                table.Cell(6, 2).Range.Text = m.EngType;
                table.Cell(6, 4).Range.Text = m.AuditDate;
                table.Cell(7, 2).Range.Text = m.ActualStartDate;
                table.Cell(7, 4).Range.Text = m.ScheCompletionDate;
                table.Cell(8, 2).Range.Text = String.Format("{0}{1}",m.TotalDays, m.DurationCategory);
                if (m.BidAmount.HasValue && m.PDAccuScheProgress.HasValue)
                {
                    table.Cell(9, 2).Range.Text = String.Format("{0}", m.BidAmount.Value * m.PDAccuScheProgress.Value / 100);
                }
                else
                    table.Cell(9, 2).Range.Text = "0";
                table.Cell(9, 3).Range.Text = decimal2Str(m.PDAccuEstValueAmount);
                table.Cell(10, 2).Range.Text = m.CompetentAuthority;
                table.Cell(11, 2).Range.Text = m.OrganizerName;
                table.Cell(12, 2).Range.Text = m.PrjManageUnit;
                table.Cell(13, 2).Range.Text = m.DesignUnitName;
                table.Cell(14, 2).Range.Text = m.SupervisionUnitName;
                table.Cell(15, 2).Range.Text = m.ContractorName1;
                table.Cell(17, 1).Range.Text = m.EngOverview;
                table.Cell(19, 3).Range.Text = m.PDAccuScheProgress.HasValue ? m.PDAccuScheProgress.ToString() : "";//shioulo20221025 int2Str(m.PDAccuScheProgress);
                table.Cell(20, 3).Range.Text = m.PDAccuActualProgress.HasValue ? m.PDAccuActualProgress.ToString() : "";//shioulo20221025 int2Str(m.PDAccuActualProgress);
                table.Cell(21, 3).Range.Text = DateTime.Now.ToString("yyyy-m-d");
                table.Cell(23, 2).Range.Text = String.Format("落後因素:{0}\n原因分析:{1}\n解決辦法:{2}", m.BDBackwardFactor, m.BDAnalysis, m.BDSolution);
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
        }
        private string int2Str(int? v)
        {
            return v.HasValue? v.Value.ToString() : "";
        }
        //s20230315
        private string decimal2Str(decimal? v)
        {
            return v.HasValue ? v.Value.ToString() : "";
        }
        //施工抽查成果
        public ActionResult DnDoc1(int id)
        {
            List<EngMainEditVModel> engs = new EngMainService().GetItemBySeq<EngMainEditVModel>(id);//20220602
            if (engs.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料讀取錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EngMainEditVModel eng = engs[0];
            List<EPCQualityVerifyDoc1VMode> items = iService.GetDoc1<EPCQualityVerifyDoc1VMode>(id);
            Microsoft.Office.Interop.Word.Application wordApp = null;
            Document doc = null;
            string filename = String.Format("{0} 施工抽查成果統計總表.docx", eng.EngNo);
            try
            {
                string tarfile = Utils.CopyTemplateFile("施工抽查成果統計總表.docx", ".docx");
                wordApp = new Microsoft.Office.Interop.Word.Application();
                doc = wordApp.Documents.Open(tarfile);
                Table table = doc.Tables[1];

                table.Cell(1, 1).Range.Text = String.Format("工程編號:{0}", eng.EngNo);
                table.Cell(1, 2).Range.Text = String.Format("工程名稱:{0}", eng.EngName);
                int rowInx = 4;
                int sTotalRec = 0, sOkCount = 0;
                foreach (EPCQualityVerifyDoc1VMode m in items) {
                    table.Cell(rowInx, 2).Range.Text = String.Format("{0}:{1}", m.subEngName, m.checkName);
                    table.Cell(rowInx, 3).Range.Text = m.totalRec.ToString();
                    table.Cell(rowInx, 4).Range.Text = m.okCount.ToString();
                    table.Cell(rowInx, 5).Range.Text = (m.totalRec - m.okCount).ToString();
                    table.Cell(rowInx, 6).Range.Text = String.Format("{0}%", m.okCount*100 / m.totalRec);
                    sTotalRec += m.totalRec;
                    sOkCount += m.okCount;
                    rowInx++;
                }
                if (sTotalRec > 0)
                {
                    table.Cell(18, 2).Range.Text = sTotalRec.ToString();
                    table.Cell(18, 3).Range.Text = sOkCount.ToString();
                    table.Cell(18, 4).Range.Text = (sTotalRec - sOkCount).ToString();
                    table.Cell(18, 5).Range.Text = String.Format("{0}%", sOkCount * 100 / sTotalRec);
                }

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
        }
        //督導
        public virtual JsonResult GetFillList(int id)
        {
            List<EPCSuperviseFillVModel> items = iService.GetFillList<EPCSuperviseFillVModel>(id);
            return Json(new
            {
                result = 0,
                items = items
            });
        }
        //督導更新
        public virtual JsonResult UpdateItem(EPCSuperviseFillVModel m)
        {
            if(iService.UpdateItem(m))
            {
                return Json(new
                {
                    result = 0,
                    msg = "儲存成功"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }

        //標案 EngMain
        public virtual JsonResult GetEngMain(int id)
        {
            //List<EngMainEditVModel> items = new EngMainService().GetItemByPrjXMLSeq<EngMainEditVModel>(id);
            List<EngMainEditVModel> items = new EngMainService().GetItemBySeq<EngMainEditVModel>(id);//20220602
            if (items.Count == 1)
            {
                return Json(new
                {
                    result = 0,
                    item = items[0]
                });
            }
            else if (items.Count > 1)
            {
                return Json(new
                {
                    result = 1,
                    msg = "資料錯誤: 對應到多個工程資料"
                });
            }
            else
            {
                return Json(new
                {
                    result = 2,
                    msg = "讀取資料錯誤"
                });
            }
        }
    }
}