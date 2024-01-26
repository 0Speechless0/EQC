using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EngHistoryInfoController : Controller
    {//水利工程履歷管理
        private EngHistoryInfoService iService = new EngHistoryInfoService();
        private DateTime tarDate = DateTime.Parse("2022-3-4");// DateTime.Now.Date;
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }
        //督導紀錄
        public JsonResult GetSupervisionList(string taxId, string bName, int perPage, int pageIndex)
        {
            List<EHI_SupervisionVModel> items = new List<EHI_SupervisionVModel>();
            int total = iService.GetSupervisionListCount(taxId, bName);
            if (total > 0)
            {
                try
                {
                    items = iService.GetSupervisionList<EHI_SupervisionVModel>(taxId, bName, perPage, pageIndex);
                }
                catch
                {
                    total = 0;
                }
            }
            return Json(new
            {
                result = 0,
                total = total,
                items = items
            });
        }

        //施工抽查紀錄
        public JsonResult GetConstructionCheckList(string taxId, string bName, int perPage, int pageIndex)
        {
            List<EHI_ConstructionCheckVModel> items = new List<EHI_ConstructionCheckVModel>();
            int total = iService.GetConstructionCheckListCount(taxId, bName);
            if (total > 0)
            {
                try
                {
                    items = iService.GetConstructionCheckList<EHI_ConstructionCheckVModel>(taxId, bName, perPage, pageIndex);
                }
                catch
                {
                    total = 0;
                }
            }
            return Json(new
            {
                result = 0,
                total = total,
                items = items
            });
        }

        //重大事故資料
        public JsonResult GetWorkSafetyTribunalList(string taxId, string bName, int perPage, int pageIndex)
        {
            List<EHI_WorkSafetyTribunalVModel> items = new List<EHI_WorkSafetyTribunalVModel>();
            int total = iService.GetWorkSafetyTribunalListCount(taxId, bName);
            if (total > 0)
            {
                try
                {
                    items = iService.GetWorkSafetyTribunalList<EHI_WorkSafetyTribunalVModel>(taxId, bName, perPage, pageIndex);
                }
                catch
                {
                    total = 0;
                }
            }
            return Json(new
            {
                result = 0,
                total = total,
                items = items
            });
        }

        //碳排放量
        public JsonResult GetCarbonEemissionList(string taxId, string bName, int perPage, int pageIndex)
        {
            List<EHI_EngListVModel> items = new List<EHI_EngListVModel>();
            int total = iService.GetCarbonEemissionListCount(taxId, bName);
            if (total > 0)
            {
                try
                {
                    items = iService.GetCarbonEemissionList<EHI_EngListVModel>(taxId, bName, perPage, pageIndex);
                }
                catch
                {
                    total = 0;
                }
            }
            return Json(new
            {
                result = 0,
                total = total,
                items = items
            });
        }
        
        //履約計分資訊 清單
        public JsonResult GetPerformanceScoreList(string taxId, string bName, int perPage, int pageIndex)
        {
            List<EHI_PerformanceScoreVModel> items = new List<EHI_PerformanceScoreVModel>();
            int total = iService.GetPerformanceScoreListCount(taxId, bName);
            if (total > 0)
            {
                try
                {
                    items = iService.GetPerformanceScoreList<EHI_PerformanceScoreVModel>(taxId, bName, perPage, pageIndex);
                }
                catch
                {
                    total = 0;
                }
            }
            return Json(new
            {
                result = 0,
                total = total,
                items = items
            });
        }

        //品質弱面資訊 清單
        public JsonResult GetWeaknessesList(string taxId, string bName, int perPage, int pageIndex)
        {
            List<EHI_PlaneWeaknessVModel> items = new List<EHI_PlaneWeaknessVModel>();
            int total = iService.GetWeaknessesListCount(taxId, bName);
            if (total > 0)
            {
                try
                {
                    items = iService.GetWeaknessesList<EHI_PlaneWeaknessVModel>(taxId, bName, perPage, pageIndex);
                }
                catch
                {
                    total = 0;
                }
            }
            return Json(new
            {
                result = 0,
                total = total,
                items = items
            });
        }

        //廠商查詢
        public JsonResult SearchVender(string keyword, int perPage, int pageIndex)
        {
            if (!String.IsNullOrEmpty(keyword)) keyword = "%" + keyword + "%";
            List<EHI_EngList2VModel> items = new List<EHI_EngList2VModel>();
            int total = iService.GetSearchVenderEngListCount(keyword);
            if (total > 0)
            {
                try
                {
                    items = iService.GetSearchVenderEngList<EHI_EngList2VModel>(keyword, perPage, pageIndex);
                }
                catch (Exception e)
                {
                    total = 0;
                }
            }
            return Json(new
            {
                result = 0,
                total = total,
                items = items
            });
        }

        //工程查詢 *****************
        public JsonResult SearchEng(string keyword, int perPage, int pageIndex)
        {
            if (!String.IsNullOrEmpty(keyword)) keyword = "%" + keyword + "%";
            List<EHI_EngListVModel> items = new List<EHI_EngListVModel>();
            int total = iService.GetSearchEngListCount(keyword);
            if (total > 0)
            {
                try
                {
                    items = iService.GetSearchEngList<EHI_EngListVModel>(keyword, perPage, pageIndex);
                }
                catch (Exception e)
                {
                    total = 0;
                }
            }
            return Json(new
            {
                result = 0,
                total = total,
                items = items
            });
        }

        //抽查照片 清單
        public JsonResult GetPhotoList(int id, int perPage, int pageIndex)
        {
            List<EHI_CheckRecPhotoVModel> items = new List<EHI_CheckRecPhotoVModel>();
            int total = iService.GetPhotoListCount(id, tarDate);
            if (total > 0)
            {
                try
                {
                    items = iService.GetPhotoList<EHI_CheckRecPhotoVModel>(id, tarDate, perPage, pageIndex);
                }
                catch
                {
                    total = 0;
                }
            }
            return Json(new
            {
                result = 0,
                total = total,
                items = items
            });
        }

        //機具
        public JsonResult GetEquipmentList(int id, int perPage, int pageIndex)
        {
            //DateTime tarDate = DateTime.Now.Date;
            List<EPCSupDailyReportConstructionEquipmentModelVModel> items = new List<EPCSupDailyReportConstructionEquipmentModelVModel>();
            int total = iService.GetEquipmentCount(id, tarDate);
            if (total > 0)
            {
                try
                {
                    items = iService.GetEquipmentList<EPCSupDailyReportConstructionEquipmentModelVModel>(id, tarDate, perPage, pageIndex);
                }
                catch
                {
                    total = 0;
                }
            }
            return Json(new
            {
                result = 0,
                total = total,
                items = items
            });
        }

        //工地人員
        public JsonResult GetPersonList(int id, int perPage, int pageIndex)
        {
            //DateTime tarDate = DateTime.Now.Date;
            List<SupDailyReportConstructionPersonModel> items = new List<SupDailyReportConstructionPersonModel>();
            int total = iService.GetPersonListCount(id, tarDate);
            if (total > 0)
            {
                try
                {
                    items = iService.GetPersonList<SupDailyReportConstructionPersonModel>(id, tarDate, perPage, pageIndex);
                }
                catch
                {
                    total = 0;
                }
            }
            return Json(new
            {
                result = 0,
                total = total,
                items = items
            });
        }

        //工地材料管制概況 清單
        public JsonResult GetMaterialList(int id, int perPage, int pageIndex)
        {
            //DateTime tarDate = DateTime.Now.Date;
            List<SupDailyReportConstructionMaterialModel> items = new List<SupDailyReportConstructionMaterialModel>();
            int total = iService.GetMaterialListCount(id, tarDate);
            if (total > 0)
            {
                try
                {
                    items = iService.GetMaterialList<SupDailyReportConstructionMaterialModel>(id, tarDate, perPage, pageIndex);
                }
                catch
                {
                    total = 0;
                }
            }
            return Json(new
            {
                result = 0,
                total = total,
                items = items
            });
        }
        
        //工程-材料設備送審
        public JsonResult GetEquipmentReviewList(int id, int pageRecordCount, int pageIndex)
        {
            List<EHI_EquipmentReviewVModel> items = new List<EHI_EquipmentReviewVModel>();
            int total = iService.GetEMDSummaryListCount(id);
            if (total > 0)
            {
                try
                {
                    items = iService.GetEMDSummaryList<EHI_EquipmentReviewVModel>(id, pageRecordCount, pageIndex);
                }
                catch
                {
                    total = 0;
                }
            }
            return Json(new
            {
                result = 0,
                total = total,
                items = items
            });
        }
        //材料設備送審 - 協力廠商資料
        public JsonResult GetEMDVendorList(int id)
        {
            return Json(new
            {
                result = 0,
                items = iService.GetEMDVendorList<EHI_EMDVendorVModel>(id)
            });
        }
        //材料設備送審 - 型錄
        public JsonResult GetEMDCatalogList(int id)
        {
            return Json(new
            {
                result = 0,
                items = iService.GetEMDCatalogList<EHI_EMDCatalogVModel>(id)
            });
        }
        //材料設備送審 - 相關試驗報告
        public JsonResult GetEMDTestReportList(int id)
        {
            return Json(new
            {
                result = 0,
                items = iService.GetEMDTestReportList<EHI_EMDCatalogVModel>(id)
            });
        }

        //材料設備檢(試)驗管制總表
        public JsonResult GetEMDTestSummaryList(int id, int emdSeq, int pageRecordCount, int pageIndex)
        {
            List<EngMaterialDeviceTestSummaryVModel> EMDTestSummaryList = new List<EngMaterialDeviceTestSummaryVModel>();
            int total = iService.GetEMDTestSummaryListCount(id, emdSeq);
            if (total > 0)
            {
                EMDTestSummaryList = iService.GetEMDTestSummaryList<EngMaterialDeviceTestSummaryVModel>(id, emdSeq, pageIndex, pageRecordCount);
            }
            return Json(new
            {
                result = 0,
                total = total,
                items = EMDTestSummaryList
            });
        }
    }
}