using EQC.Common;
using EQC.EDMXModel;
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
            var phaseOption = iServce.GetPhaseOptions(keyWord.Substring(0, 3));
            if (list.Count > 0)
            {
                return Json(new
                {
                    result = 0,
                    item = list[0],
                    phaseOption =phaseOption
                });
            }
            return Json(new
            {
                result = -1,
                msg = "查無此期別"
            });
        }

        public JsonResult SyncWeakness(int prjSeq, int pharseSeq)
        {
            string description =  iServce.GetWeaknessDescription(prjSeq);
            if(iServce.SyncSuperviseEngWeakness(prjSeq, pharseSeq) > 0)
            {
                return Json(description);
            }
            else
            {
                return Json(null);
            }
        }
        //期間工程清單
        public JsonResult GetPhaseEngList(int id, int pageRecordCount, int pageIndex)
        {
            List<SuperviseEng1VModel> engList = new List<SuperviseEng1VModel>();
            int total = iServce.GetPhaseEngList1Count(id);
            
            if (total > 0)
            {
                engList = iServce.GetPhaseEngList1<SuperviseEng1VModel>(id, pageRecordCount, pageIndex);
                /* s20240913 取消
                using(var context = new EQC_NEW_Entities())
                {
                    engList =  context.SuperviseEng.Where(r => r.SupervisePhaseSeq == id).ToList().Join(
                        context.viewPrjXMLPlaneWeakness,
                        r1 => r1.PrjXMLSeq,
                        r2 => r2.PrjXMLSeq,
                        (r1, r2) =>
                        {
                            r1.Updated =
                                !(r1.W1 == r2.W1 &&
                                r1.W2 == r2.W2 &&
                                r1.W3 == r2.W3 &&
                                r1.W4 == r2.W4 &&
                                r1.W5 == r2.W5 &&
                                r1.W6 == r2.W6 &&
                                r1.W7 == r2.W7 &&
                                r1.W8 == r2.W8 &&
                                r1.W9 == r2.W9 &&
                                r1.W10 == r2.W10 &&
                                r1.W11 == r2.W11 &&
                                r1.W12 == r2.W12 &&
                                r1.W13 == r2.W13 &&
                                r1.W14 == r2.W14);
                            return r1;

                        })
                        .Join(engList,
                            r1 => r1.PrjXMLSeq,
                            r2 => r2.PrjXMLSeq,
                            (r1, r2) =>
                            {
                                r2.Updated = r1.Updated;
                                return r2;
                            }).ToList();
                        
                }*/
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
        //移動期別儲存
        public JsonResult SaveChangeEng(SuperviseEngPreWordVModel m ,int PhaseSeq)
        {
            int state = iServce.SaveChangePhaseForPrework(m, PhaseSeq);
            if (state > 0)
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