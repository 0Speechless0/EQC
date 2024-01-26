using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class ERProposalWorkController : Controller
    {//工程提報 - 提案工作會議

        EngReportService engReportService = new EngReportService();


        public ActionResult Index()
        {
            Utils.setUserClass(this, 1);
            return View();
        }
        
        //工程清單
        public JsonResult GetList(int year, int unit, int subUnit, int rptType, int pageRecordCount, int pageIndex)
        {
            List<EngReportVModel> engList = new List<EngReportVModel>();
            int total = engReportService.GetEngListCount(year, unit, subUnit, rptType, 2);
            if (total > 0)
            {
                engList = engReportService.GetEngList<EngReportVModel>(year, unit, subUnit, rptType, pageRecordCount, pageIndex, 2);

                int iCount = 0;
                foreach (EngReportVModel vm in engList) 
                {
                    engList[iCount].IsShow = false;
                    if ((vm.RptTypeSeq == 1 || vm.RptTypeSeq == 3) && vm.NeedAssessmenApproval == 2)
                        engList[iCount].IsShow = true;
                    iCount++;
                }
            }
            return Json(new
            {
                pTotal = total,
                items = engList
            });
        }
        
        //更新評估結果
        public ActionResult UpdateEngReport(EngReportVModel m)
        {
            int state;
            switch (m.EvaluationResult) 
            {
                case 1:
                case 2:
                    m.RptTypeSeq = 2;
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                    m.RptTypeSeq = 3;
                    break;
            }
            if (m.EvaluationResult != 1)
            {
                m.ER1_1 = "";
                m.ER1_2 = "";
            }
            if (m.EvaluationResult != 2)
            {
                m.ER2_1 = "";
                m.ER2_2 = "";
            }
            if (m.EvaluationResult != 3)
            {
                m.ER3 = "";
            }
            if (m.EvaluationResult != 4)
            {
                m.ER4 = "";
            }
            if (m.EvaluationResult != 6)
            {
                m.ER6 = "";
            }
            state = engReportService.UpdateEngReportForPW(m);
            if (state == 0)
            {
                //switch (m.EvaluationResult)
                //{
                //    case 3:
                //    case 4:
                //    case 5:
                //    case 6:
                //        EngReportSignOffProcessModel sop = new EngReportSignOffProcessModel();
                //        sop.EngReportSeq = m.Seq;
                //        sop.SignOffState = 1;
                //        sop.Memo = "需求重新評估";
                //        //新增簽核歷程
                //        sop.ContentType = 1;
                //        new EngReportSignOffProcessService().Insert(sop);
                //        //新增簽核歷程
                //        sop.ContentType = 2;
                //        new EngReportSignOffProcessService().Insert(sop);
                //        //新增簽核歷程
                //        sop.ContentType = 3;
                //        new EngReportSignOffProcessService().Insert(sop);
                //        //新增簽核歷程
                //        sop.ContentType = 4;
                //        new EngReportSignOffProcessService().Insert(sop);
                //        break;
                //}
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
    }
}