using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class ESPreWorkController : Controller
    {//工程督導 - 前置作業
        SupervisePhaseService iServce = new SupervisePhaseService();
        public ActionResult Index()
        {
            Utils.setUserClass(this, 1);
            return View("Index");
        }
        //期別查詢
        public JsonResult SearchPhase(string keyWord)
        {
            List<SupervisePhaseModel> list = iServce.GetPhaseCode(keyWord);
            if(list.Count==1)
            {
                return Json(new
                {
                    result = 0,
                    item = list[0]
                });
            }
            return Json(new
            {
                result = -1,
                msg = "查無此期別"
            });
        }
        //期間工程清單
        public JsonResult GetPhaseEngList(int id, int pageRecordCount, int pageIndex)
        {
            List<SuperviseEng1VModel> engList = new List<SuperviseEng1VModel>();
            int total = iServce.GetPhaseEngList1Count(id);
            
            if (total > 0)
            {
                engList = iServce.GetPhaseEngList1<SuperviseEng1VModel>(id, pageRecordCount, pageIndex);
            }
            return Json(new
            {
                pTotal = total,
                items = engList
            });
        }

        //期間工程
        public JsonResult GetEng(int id)
        {
            List<SuperviseEngPreWordVModel> engList = iServce.GetEngForPrework<SuperviseEngPreWordVModel>(id);
            if (engList.Count == 1)
            {
                return Json(new
                {
                    result = 0,
                    item = engList[0]
                });
            } else
            {
                return Json(new
                {
                    result = -1,
                    msg = "無法取得資料"
                });
            }
            
        }
        public JsonResult GetCtlPlanList()
        {
            return Json(new
            {
                result = 0,
                items = iServce.GetControlPlanNoList()
            });
        }
        //edit ====
        public JsonResult SaveEng(SuperviseEngPreWordVModel m)
        {
            int state = iServce.SaveEngForPrework(m);
            if (state >0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "儲存成功"
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "儲存失敗"
                });
            }

        }
        //職稱清單
        public JsonResult GetPositions()
        {
            List<SelectIntOptionModel> lists = iServce
                .GetPositions<SelectIntOptionModel>()
                .SortListByMap(r=> r.Text, Order.PositionOrderMap);
            return Json(new
            {
                items = lists
            });
        }
        public JsonResult GetPositionsUser(int id)
        {
            List<SelectIntOptionModel> lists = iServce.GetPositionsUser<SelectIntOptionModel>(id);
            return Json(new
            {
                items = lists
            });
        }
        //幹事
        public JsonResult SearchOfficerCommitte(int id, string keyWord)
        {
            List<ESCommitteeVModel> list = iServce.GetOfficerCommitte<ESCommitteeVModel>(id, keyWord);
            return Json(new
            {
                result = 0,
                items = list
            });
        }
        public JsonResult AddOfficerCommitte(int id, int committe)
        {
            int state = iServce.AddOfficerCommitte(id, committe);
            if (state == 1)
            {
                return Json(new
                {
                    result = 0,
                    msg = "加入成功"
                }); ;
            }
            return Json(new
            {
                result = -1,
                msg = "加入失敗"
            });
        }
        public JsonResult DelOfficerCommitte(int id)
        {
            int state = iServce.DelOfficerCommitte(id);
            if (state == 1)
            {
                return Json(new
                {
                    result = 0,
                    msg = "移除成功"
                }); ;
            }
            return Json(new
            {
                result = -1,
                msg = "移除失敗"
            });
        }

        //督導委員(內聘)
        public JsonResult SearchInsideCommitte(int id, string keyWord)
        {
            List<ESCommitteeVModel> list = iServce.GetInsideCommitte<ESCommitteeVModel>(id, keyWord);
            return Json(new
            {
                result = 0,
                items = list
            });
        }
        public JsonResult AddInsideCommitte(int id, int committe)
        {
            int state = iServce.AddInsideCommitte(id, committe);
            if (state == 1)
            {
                return Json(new
                {
                    result = 0,
                    msg = "加入成功"
                }); ;
            }
            return Json(new
            {
                result = 0,
                msg = "加入失敗"
            });
        }
        public JsonResult DelInsideCommitte(int id)
        {
            int state = iServce.DelInsideCommitte(id);
            if (state == 1)
            {
                return Json(new
                {
                    result = 0,
                    msg = "移除成功"
                }); ;
            }
            return Json(new
            {
                result = -1,
                msg = "移除失敗"
            });
        }

        //督導委員(外聘)
        public JsonResult SearchOutCommitte(int id, string keyWord)
        {
            List<ESCommitteeVModel> list = iServce.GetOutCommitte<ESCommitteeVModel>(id, keyWord);
            return Json(new
            {
                result = 0,
                items = list
            });
        }
        public JsonResult AddOutCommitte(int id, int committe)
        {
            List<ExpertCommitteeModel> lists = new ExpertCommitteeService().GetCommittee<ExpertCommitteeModel>(committe);
            if(lists.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "委員資料讀取錯誤"
                });
            }
            ExpertCommitteeModel cm = lists[0];
            if(string.IsNullOrEmpty(cm.ECId) || cm.ECId.IndexOf("XX")>-1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "委員資料錯誤"
                });
            }
            int state = iServce.checkAccount(UnitService.type_OutCommitteeUnit, cm.ECId, cm.ECName, cm.ECEmail, cm.ECTel, cm.ECMobile);
            if (state != 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "委員帳號建立錯誤"
                });
            }
            state = iServce.AddOutCommitte(id, committe);
            if(state == 1)
            {
                return Json(new
                {
                    result = 0,
                    msg = "加入成功"
                }); ;
            }
            return Json(new
            {
                result = -1,
                msg = "加入失敗"
            });
        }
        public JsonResult DelOutCommitte(int id)
        {
            int state = iServce.DelOutCommitte(id);
            if (state == 1)
            {
                return Json(new
                {
                    result = 0,
                    msg = "移除成功"
                }); ;
            }
            return Json(new
            {
                result = -1,
                msg = "移除失敗"
            });
        }
    }
}