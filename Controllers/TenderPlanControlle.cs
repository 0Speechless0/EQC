using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace EQC.Controllers
{
    [SessionFilter]
    public class TenderPlanController : Controller
    {
        private TenderPlanService tenderPlanService = new TenderPlanService();
        private EngMainService engMainService = new EngMainService();

        public ActionResult TenderPlanListTest()
        {
            Utils.setUserClass(this, 2);
            //ViewBag.Title = "工程立號";
            return View("TenderPlanListTest");
        }
        public ActionResult Index()
        {
            return TenderPlanList();
        }
        //s20231006
        //新增 自辦監造人員
        public ActionResult CreateApprovalEng(int id, EngMainEditVModel eng)
        {
            List<EngMainEditVModel> engs = engMainService.CheckEngNo<EngMainEditVModel>(eng.EngNo);
            if (engs.Count > 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "錯誤:工程編號已存在不能重複建立"
                });
            }
            else
            {
                eng.updateDate();
                string errMsg = "";
                int result = tenderPlanService.CreateApprovalEng(id, eng, ref errMsg);
                if (result == -1)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "儲存失敗"
                    });
                }
                else
                {
                    return Json(new
                    {
                        result = 0,
                        id = result,
                        msg = "儲存成功"
                    });
                }
            }
        }
        public ActionResult SupervisorUserAdd(int eng, int kind, int subUnit, int id)
        {
            int result = engMainService.SupervisorUserAdd(eng, kind, subUnit, id);
            if (result == -1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "新增失敗"
                });
            } else
            {
                return Json(new
                {
                    result = 0,
                    items = engMainService.SupervisorUserList<EngSupervisorVModel>(eng),
                    msg = "新增成功"
                });
            }
        }
        //刪除 自辦監造人員
        public ActionResult SupervisorUserDel(int eng, int id)
        {
            if (engMainService.SupervisorUserDel(id) == -1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "刪除失敗"
                });
            } else
            {
                return Json(new
                {
                    result = 0,
                    items = engMainService.SupervisorUserList<EngSupervisorVModel>(eng),
                    msg = "刪除完成"
                });
            }
        }
        //自辦監造人員清單
        public ActionResult GetSupervisorUser(int id)
        {
            return Json(new
            {
                result = 0,
                items = engMainService.SupervisorUserList<EngSupervisorVModel>(id)
        });
        }

        public ActionResult TenderPlanList()
        {
            Utils.setUserClass(this, 2);
            //ViewBag.Title = "工程立號";
            return View("TenderPlanList");
        }
        //新增 標案(PrjXML) s20230327
        public JsonResult AddPrjXMLEng(PrjXMLModel eng)
        {
            if(new PrjXMLService().AddEng(eng))
            {
                return Json(new
                {
                    result = 0,
                    msg = "新增成功"
                });
            }
            return Json(new
            {
                result = 1,
                msg = "新增失敗"
            });
        }
        //取得 標案清單(PrjXML) shioulo 20220504
        public JsonResult GetTenderList(int id)
        {
            List<PrjXMLVModel> items = new PrjXMLService().GetTenderListByEng<PrjXMLVModel>(id);

            return Json(items);
        }
        //工程連結標案(PrjXML) shioulo 20220518
        public JsonResult SetEngLinkTender(int id, int prj)
        {
            int state = engMainService.SetEngLinkTender(id, prj);
            if(state == -1)
            {
                return Json(new
                {
                    result = 1,
                    msg = "該標案已被指定在其它工程, 不能重複指定"
                });
            } else if(state == 0)
            {
                return Json(new
                {
                    result = 1,
                    msg = "儲存失敗"
                });
            }

            return Json(new
            {
                result = 0,
                msg = "儲存成功"
            });
        }
        public JsonResult CancelEngLinkTender(int id)
        {
            int state = engMainService.CancelEngLinkTender(id);
            if (state == 0)
            {
                return Json(new
                {
                    result = 1,
                    msg = "儲存失敗"
                });
            }

            return Json(new
            {
                result = 0,
                msg = "儲存成功"
            });
        }
        public ActionResult GetUserUnit()
        {
            string unitSubSeq = "";
            string unitSeq = "";
            string unitSubN = "";
            string unitN = "";
            Utils.GetUserUnit(ref unitSeq, ref unitN, ref unitSubSeq, ref unitSubN);
            return Json(new
            {
                result = 0,
                unit = unitSeq,
                unitSub = unitSubSeq,
                unitName = unitN,
                unitSubName = unitSubN
            });
        }

        public ActionResult EditEng(int seq)
        {
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Edit", "TenderPlan");
            string tarUrl = redirectUrl.ToString() + "?id=" + seq;
            return Json(new { Url = tarUrl });
        }
        public ActionResult Edit()
        {
            Utils.setUserClass(this, 2);
            //ViewBag.Title = "工程立號-編輯";
            List<VMenu> menu = new List<VMenu>();
            var link = new UrlHelper(Request.RequestContext).Action("Index", "TenderPlan");
            menu.Add(new VMenu() { Name = "工程立號", Url = link.ToString() });
            menu.Add(new VMenu() { Name = "建立標案", Url = "" });
            ViewBag.breadcrumb = menu;
            return View("TenderPlanEdit");
        }

        //標案年分
        public JsonResult GetYearOptions()
        {
            List<EngYearVModel> items = tenderPlanService.GetEngYearList();

            return Json(items);
        }
        //依年分取執行機關
        public JsonResult GetUnitOptions(string year)
        {
            List<EngExecUnitsVModel> items = tenderPlanService.GetEngExecUnitList(year);
            /*if(items.Count>0)
            {
                EngUnitsVModel m = new EngUnitsVModel();
                m.OrganizerUnitSeq = -1;
                m.OrganizerUnit = "全部河川局";
                items.Insert(0, m);
            }*/
            return Json(items);
        }
        //依年分,機關取執行單位
        public JsonResult GetSubUnitOptions(string year, int parentSeq)
        {
            List<EngExecUnitsVModel> items = tenderPlanService.GetEngExecSubUnitList(year, parentSeq);
            //if(items.Count>0)
            //{
                EngExecUnitsVModel m = new EngExecUnitsVModel();
                m.UnitSeq = -1;
                m.UnitName = "全部單位";
                items.Insert(0, m);
            //}
            return Json(items);
        }
        //工程清單
        public JsonResult GetList(int year, int hasCouncil , int unit, int subUnit, int pageRecordCount, int pageIndex)
        {
            List<EngMainVModel> engList = new List<EngMainVModel>();
            int total = tenderPlanService.GetEngListCount(year, hasCouncil, unit, subUnit);
            if (total > 0)
            {
                engList = tenderPlanService.GetEngList<EngMainVModel>(year, hasCouncil, unit, subUnit, pageRecordCount, pageIndex);
            }
            return Json( new
            {
                pTotal = total,
                items = engList
            });
        }
        //for edit =========================
        //單位清單
        public JsonResult GetUnitList(int parentUnit)
        {
            UnitService s = new UnitService();
            List<UModel> items = s.GetListForOption<UModel>(parentUnit);
            return Json(items);
        }
        //行政區(縣市)
        public JsonResult GetCityList()
        {
            CityService s = new CityService();
            List<VCityModel> items = s.GetCityForOption<VCityModel>();
            return Json(items);
        }
        //行政區(鄉鎮)
        public JsonResult GetTownList(int id)
        {
            TownService s = new TownService();
            List<VTownModel> items = s.GetTownForOption<VTownModel>(id);
            return Json(items);
        }
        //人員清單
        public JsonResult GetUserList(int organizerUnitSeq, int organizerSubUnitSeq)
        {
            UserService s = new UserService();
            List<VUserMain> users = s.GetList(1,"%", 0, 9999);

            List<SelectOptionModel> items = new List<SelectOptionModel>();
            foreach(VUserMain user in users)
            {
                if (organizerUnitSeq > -1 && organizerSubUnitSeq > -1)
                {
                    if (user.UnitSeq1 == organizerUnitSeq && user.UnitSeq2 == organizerSubUnitSeq)
                    {
                        items.Add(new SelectOptionModel()
                        {
                            Text = user.DisplayName,
                            Value = user.Seq.ToString()
                        });
                    }
                }
                else if (organizerUnitSeq > -1 && organizerSubUnitSeq <= -1)
                {
                    if (user.UnitSeq1 == organizerUnitSeq)
                    {
                        items.Add(new SelectOptionModel()
                        {
                            Text = user.DisplayName,
                            Value = user.Seq.ToString()
                        });
                    }
                }
                else
                {
                    items.Add(new SelectOptionModel()
                    {
                        Text = user.DisplayName,
                        Value = user.Seq.ToString()
                    });

                }
            }
            return Json(items);
        }

        public JsonResult GetEngItem(int id)
        {
            List<EngMainEditVModel> items = engMainService.GetItemBySeq<EngMainEditVModel>(id);
            if (items.Count == 1)
            {
                return Json(new
                {
                    result = 0,
                    item = items[0],
                    ur = Utils.getUserInfo().RoleSeq
                });
            } else
            {
                return Json(new
                {
                    result = 1,
                    message = "讀取資料錯誤"
                });
            }
        }
        //工程主要施工項目及數量 清單
        public JsonResult GetECList(int engMain)
        {
            EngConstructionService engConstructionService = new EngConstructionService();
            List<EngConstructionVModel> items = engConstructionService.GetListByEngMainSeq<EngConstructionVModel>(engMain);
            return Json(items);
        }
        //
        public JsonResult ConstructionAdd(EngConstructionVModel item)
        {
            if (!item.ItemQty.HasValue)
            {//s20230912
                return Json(new
                {
                    result = 1,
                    message = "數量必須輸入, 且為整數值"
                });
            }
            EngConstructionService engConstructionService = new EngConstructionService();
            //List<EngConstructionVModel> items = engConstructionService.GetListByEngMainSeq<EngConstructionVModel>(item.EngMainSeq);
            int seq = engConstructionService.Add(item, engConstructionService.GetDefaultItem(item.EngMainSeq));
            if (seq > 0)
            {
                return Json(new
                {
                    result = 0,
                    message = "新增成功",
                    Seq = seq
                });
            }
            else
            {
                return Json(new
                {
                    result = 1,
                    message = "讀取資料錯誤"
                });
            }
        }
        //
        public JsonResult ConstructionUpdate(EngConstructionVModel item)
        {
            if(!item.ItemQty.HasValue)
            {//s20230906
                return Json(new
                {
                    result = 1,
                    message = "數量必須輸入, 且為整數值"
                });
            }
            EngConstructionService engConstructionService = new EngConstructionService();
            int result = engConstructionService.Update(item);
            if (result==1)
            {
                return Json(new
                {
                    result = 0,
                    message = "新增成功",
                });
            }
            else
            {
                return Json(new
                {
                    result = 1,
                    message = "儲存錯誤"
                });
            }
        }
        //
        public JsonResult ConstructionDel(int seq)
        {
            EngConstructionService engConstructionService = new EngConstructionService();
            int result = engConstructionService.Delete(seq);
            if (result == 1)
            {
                return Json(new
                {
                    result = 0,
                    message = "刪除成功",
                });
            }
            else
            {
                return Json(new
                {
                    result = 1,
                    message = "刪除錯誤"
                });
            }
        }

        //上傳監造計畫附件 圖/表
        public JsonResult EngAttachmentList(int engMain, int chapter)
        {
            List<EngAttachmentVModel> result = new List<EngAttachmentVModel>();
            EngAttachmentService service = new EngAttachmentService();
            result = service.GetList<EngAttachmentVModel>(engMain, chapter);
            return Json(result);
        }
        public JsonResult EngAttachmentUpload(int engMain, int chapter, byte fileType, string description)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                EngAttachmentModel model = new EngAttachmentVModel();
                model.EngMainSeq = engMain;
                model.Chapter = chapter;
                model.FileType = fileType;
                model.Description = description;
                try
                {
                    if (SaveFile(file, model, "Attr-"))
                    {
                        EngAttachmentService service = new EngAttachmentService();
                        if (service.Add(model) == 0)
                        {
                            return Json(new
                            {
                                result = -1,
                                message = "新增圖表失敗"
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                result = 0,
                                message = "新增圖表成功",
                            });

                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                }
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }
        public ActionResult EngAttachmentDownload(int seq)
        {
            EngAttachmentService service = new EngAttachmentService();
            List<EngAttachmentModel> items = service.GetItemFileInfoBySeq<EngAttachmentModel>(seq);
            if (items.Count == 0 || items[0].UniqueFileName == null || items[0].UniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            return DownloadFile(items[0]);
        }
        private bool SaveFile(HttpPostedFileBase file, EngAttachmentModel m, string fileHeader)
        {
            try
            {
                string filePath = Utils.GetEngMainFolder(m.EngMainSeq);// GetFlowChartPath();
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                m.OriginFileName = file.FileName.ToString().Trim();

                int inx = m.OriginFileName.LastIndexOf(".");
                m.UniqueFileName = String.Format("{0}{1}{2}", fileHeader, Guid.NewGuid(), m.OriginFileName.Substring(inx));
                string fullPath = Path.Combine(filePath, m.UniqueFileName);
                file.SaveAs(fullPath);

                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                return false;
            }
        }
        private ActionResult DownloadFile(EngAttachmentModel m)
        {
            string filePath = Utils.GetEngMainFolder(m.EngMainSeq);

            string uniqueFileName = m.UniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
                if (System.IO.File.Exists(fullPath))
                {
                    Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", m.OriginFileName);
                }
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }
        //
        public JsonResult EngAttachmentUpdate(EngAttachmentModel item)
        {
            EngAttachmentService service = new EngAttachmentService();
            int result = service.UpdateDescription(item);
            if (result == 1)
            {
                return Json(new
                {
                    result = 0,
                    message = "新增成功",
                });
            }
            else
            {
                return Json(new
                {
                    result = 1,
                    message = "儲存錯誤"
                });
            }
        }
        //
        public JsonResult EngAttachmentDel(int seq)
        {
            EngAttachmentService service = new EngAttachmentService();
            List<EngAttachmentModel> items = service.GetItemBySeq<EngAttachmentModel>(seq);
            if (items.Count == 0)
            {
                return Json(new
                {
                    result = 0,
                    message = "資料刪除完成"
                });
            }
            else
            {
                int result = service.Delete(seq);
                if (result == 1)
                {
                    //刪除已儲存檔案
                    string uniqueFileName = items[0].UniqueFileName;
                    if (uniqueFileName != null && uniqueFileName.Length > 0)
                    {
                        System.IO.File.Delete(Path.Combine(Utils.GetEngMainFolder(items[0].EngMainSeq), uniqueFileName));
                    }
                    return Json(new
                    {
                        result = 0,
                        message = "刪除成功",
                    });
                }
                else
                {
                    return Json(new
                    {
                        result = 1,
                        message = "刪除錯誤"
                    });
                }
            }
        }

        //儲存
        public JsonResult UpdateTenderPlan(EngMainEditVModel engMain)
        {
            engMain.updateDate();
            
            int result = engMainService.Update(engMain);
            if (result >= 1)
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存成功"
                });
            } else
            {
                return Json(new
                {
                    result = -1,
                    message = "儲存失敗"
                });
            }
        }
        //設計單位建立帳號與發送mail
        public JsonResult SendMailToDesignUnit(int engMain)
        {
            List<EngMainEditVModel> l = engMainService.GetItemBySeq<EngMainEditVModel>(engMain);
            if (l.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "工程資料錯誤, Email傳送失敗"
                });
            }
            EngMainEditVModel eng = l[0];
            int result = tenderPlanService.checkAccount(UnitService.type_DesignUnit, eng.DesignUnitTaxId, eng.DesignUnitName, eng.DesignManName, eng.DesignUnitEmail, eng.EngNo);
            if (result == 0)
            {
                if (sendMail(eng.DesignUnitEmail, eng.DesignManName, "設計單位"))
                {
                    return Json(new
                    {
                        result = 0,
                        message = "已發送帳密通知設計單位進行資料填報"
                    });
                }
                else
                {
                    return Json(new
                    {
                        result = -1,
                        message = "Email傳送失敗"
                    });
                }
            }
            else if (result == -1)
            {
                return Json(new
                {
                    result = -1,
                    message = "系統資料錯誤, 請洽系統管理員"
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "帳號建立失敗, 請洽系統管理員"
                });
            }
            /**/
        }
        //施工廠商建立帳號與發送mail
        public JsonResult SendMailToBuildContractor(int engMain)
        {
            List<EngMainEditVModel> l = engMainService.GetItemBySeq<EngMainEditVModel>(engMain);
            if(l.Count==0)
            {
                return Json(new
                {
                    result = -1,
                    message = "工程資料錯誤, Email傳送失敗"
                });
            }
            EngMainEditVModel eng = l[0];
            int result = tenderPlanService.checkAccount(UnitService.type_BuildContractor, eng.BuildContractorTaxId, eng.BuildContractorName, eng.BuildContractorContact, eng.BuildContractorEmail, eng.EngNo);
            if (result == 0)
            {
                if (sendMail(eng.BuildContractorEmail, eng.BuildContractorContact, "施工廠商"))
                {
                    return Json(new
                    {
                        result = 0,
                        message = "已發送帳密通知施工廠商進行資料填報"
                    });
                }
                else
                {
                    return Json(new
                    {
                        result = -1,
                        message = "Email傳送失敗"
                    });
                }
            } else if (result == -1)
            {
                return Json(new
                {
                    result = -1,
                    message = "系統資料錯誤, 請洽系統管理員"
                });
            } else
            {
                return Json(new
                {
                    result = -1,
                    message = "帳號建立失敗, 請洽系統管理員"
                });
            }
            /**/
        }
        //監造單位建立帳號與發送mail
        public JsonResult SendMailToSupervisor(int engMain)
        {
            List<EngMainEditVModel> l = engMainService.GetItemBySeq<EngMainEditVModel>(engMain);
            if (l.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "工程資料錯誤, Email傳送失敗"
                });
            }
            EngMainEditVModel eng = l[0];
            int result = tenderPlanService.checkAccount(UnitService.type_SupervisorUnit, eng.SupervisorTaxid, eng.SupervisorUnitName, eng.SupervisorDirector, eng.SupervisorContact, eng.EngNo);
            if (result == 0)
            {
                if (sendMail(eng.SupervisorContact, eng.SupervisorDirector, "監造單位"))
                {
                    return Json(new
                    {
                        result = 0,
                        message = "已發送帳密通知監造單位進行資料填報"
                    });
                }
                else
                {
                    return Json(new
                    {
                        result = -1,
                        message = "Email傳送失敗"
                    });
                }
            }
            else if (result == -1)
            {
                return Json(new
                {
                    result = -1,
                    message = "系統資料錯誤, 請洽系統管理員"
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "帳號建立失敗, 請洽系統管理員"
                });
            }
        }
        private bool sendMail(string toMail, string user, string unit)
        {//s20230623
            string body = String.Format(@"
                {0}<{1}>您好，<br>
                水利工程品管系統已新增帳號及密碼。<br>
                系統預設帳號、密碼規則如下:<br>
                帳號:代號+工程編號<br>
                密碼:!統編!<br>
                如果你是設計單位，代號為D；如果你是監造單位，代號為S；如果你是施工廠商，代號為C；登入系統後，請自行變更密碼。<br>
                請依水利署工程品管作業規範，登入系統填報工程抽驗及缺失改善狀況", user, unit);
            return Utils.Email(toMail, "水利工程品管系統,帳號通知", body);
        }
        //產製監造計畫書範例
        public JsonResult CreateSupervisionProject(int engMain, string im)
        {
            if (engMainService.CreateSupervisionProject(engMain, im))
            {
                return Json(new
                {
                    result = 0,
                    message = "產製完成"
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "產製失敗"
                });
            }
        }

        //上傳 PCCESS xml
        public JsonResult UploadXML(int processMode)
        {
            var file = Request.Files[0];
            string errMsg = "";
            if (file.ContentLength > 0)
            {
                try
                {
                    //string uuid = System.Guid.NewGuid().ToString("B").ToUpper();
                    string tempPath = Path.GetTempPath();
                    string fullPath = Path.Combine(tempPath, "Eng-" + file.FileName);
                    file.SaveAs(fullPath);
                    int result = processXML(fullPath, processMode, ref errMsg);
                    if(result>0)
                    {
                        return Json(new
                        {
                            result = 0,
                            message = "上傳檔案完成",
                            item = result
                        });
                    }
                    else if (result == -1)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "工程編號錯誤(或前三碼不是年分)"
                        });
                    }
                    else if (result == -2)
                    {
                        return Json(new
                        {
                            result = -2,
                            message = "工程編號已存在"
                        });
                    }
                    else if (result == -3)
                    {
                        return Json(new
                        {
                            result = -3,
                            message = "工程編號存在多筆資料, 請洽管理人員"
                        });
                    }
                    else if (result == -4)
                    {
                        return Json(new
                        {
                            result = -4,
                            message = "獲取 工程 資料錯誤"
                        });
                    }
                    else if (result == -5)
                    {
                        return Json(new
                        {
                            result = -5,
                            message = "工程編號已存在 且 已產製過計劃書,不能再上傳xml"
                        });
                    }
                    else if (result == -6)
                    {
                        return Json(new
                        {
                            result = -6,
                            message = "無 發包工作費/發包工程費 項目"
                        });
                    }
                    else if (result == -7)
                    {
                        return Json(new
                        {
                            result = -7,
                            message = "預算項目表格式錯誤"
                        });
                    }
                    else
                    {
                        errMsg = "資料儲存失敗: " + errMsg;
                        return Json(new
                        {
                            result = -3,
                            message = errMsg
                        });
                    }
                }
                catch (Exception e)
                {
                    //System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                    errMsg = "上傳檔案失敗: " + e.Message;
                }
            }

            return Json(new
            {
                result = -1,
                message = errMsg.Length > 0 ? errMsg : "上傳失敗"
            });
        }


        private int processXML(string fileName, int processMode, ref string errMsg)
        {
            //System.Diagnostics.Debug.WriteLine(fileName);          
            PccseXML pccseXML = new PccseXML(fileName);
            EngMainModel engMainModel = pccseXML.CreateEngMainModel(ref errMsg);
            if (engMainModel == null) return -1;
            pccseXML.GetGrandTotalInformation();
            PCCESSMainModel pccessMainModel = pccseXML.pccessMainModel;
            List<PCCESPayItemModel> payItems = pccseXML.payItems;
            //return -1;
            string xmlFileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
            engMainModel.PccesXMLFile = xmlFileName;
            engMainModel.PostCompDate = DateTime.Now;
            PCCESSMainService pmS = new PCCESSMainService();
            if (processMode == 0)
            {
                int cnt = pmS.GetCountByContractNo(pccessMainModel.contractNo);
                if (cnt == 1)
                {
                    List<SupervisionProjectListModel> sList = new SupervisionProjectListService().ListByEngNo<SupervisionProjectListModel>(pccessMainModel.contractNo);
                    if (sList.Count == 0) {//尚未產製計畫書
                        return -2;
                    }
                    else
                    {
                        return -5;//已產製過計劃書
                    }
                } else if (cnt > 1)
                {
                    return -3;
                }
                else
                {
                    int engMainSeq = tenderPlanService.AddEngItem(pccessMainModel, payItems, engMainModel,ref errMsg);
                    if(engMainSeq > 0)
                    {
                        string dir = Utils.GetEngMainFolder(engMainSeq);
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                        System.IO.File.Copy(fileName, Path.Combine(dir, xmlFileName), true);
                    }
                    return engMainSeq;
                }
            } else {//更新模式
                int engMainSeq = -1;
                int pccessMainSeq = pmS.GetPCCESSMainSeqByContractNo(pccessMainModel.contractNo, ref engMainSeq);
                if (pccessMainSeq > 0 && engMainSeq >0)
                {
                    if (engMainSeq > 0)
                    {
                        string dir = Utils.GetEngMainFolder(engMainSeq);
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                        System.IO.File.Copy(fileName, Path.Combine(dir, xmlFileName), true);
                    }

                    pccessMainModel.Seq = pccessMainSeq;
                    engMainModel.Seq = engMainSeq;
                    return tenderPlanService.UpdateEngItem(pccessMainModel, payItems, engMainModel, ref errMsg);
                }
                else
                {
                    return -4;
                }
            }
        }

        //下載 PccesXML shioulo 20220711
        public ActionResult DownloadPccesXML(int id)
        {
            List<EngMainEditVModel> items = engMainService.GetItemPccesFileBySeq<EngMainEditVModel>(id);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EngMainEditVModel m = items[0];
            string fName = Path.Combine(Utils.GetEngMainFolder(m.Seq), m.PccesXMLFile);
            if (!System.IO.File.Exists(fName))
            {
                return Json(new
                {
                    result = -1,
                    message = "未發現檔案"
                }, JsonRequestBehavior.AllowGet);
            }

            Stream iStream = new FileStream(fName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, "application/blob", m.PccesXMLFile);
        }
        //複製工程 s20230623
        public JsonResult CopyEng(string sNo, string eNo)
        {
            if(String.IsNullOrEmpty(eNo))
            {
                return Json(new
                {
                    result = -1,
                    msg = "錯誤, 沒有新工程編號"
                });
            }
            List<EngMainEditVModel> es = engMainService.GetEngSeqByEngNo<EngMainEditVModel>(sNo);
            if (es.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "錯誤, 要複製工程編號不存在"
                });
            }
            int id = es[0].Seq;
            es = engMainService.GetEngSeqByEngNo<EngMainEditVModel>(eNo);
            if(es.Count>0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "錯誤, 新工程編號已存在"
                });
            }
            List<EngMainCopyModel> engs = engMainService.GetItemBySeq<EngMainCopyModel>(id);
            if(engs.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }
            EngMainCopyModel eng = engs[0];
            
            eng.engConstructionItems = new EngConstructionService().GetListAllByEngMainSeq<EngConstructionModel>(eng.Seq); 
            eng.engSupervisorItems = engMainService.SupervisorUserAllList<EngSupervisorModel>(eng.Seq); //自辦監造人員清單
            eng.engAttachmentItems = new EngAttachmentService().GetListAll<EngAttachmentModel>(eng.Seq); //監造計畫附件 圖/表
            //碳排量清單
            SchEngProgressService schEngProgressService = new SchEngProgressService();
            eng.carbonEmissionPayItems = schEngProgressService.GetCarbonEmissionList<CarbonEmissionPayItemV2Model>(eng.Seq);
            schEngProgressService.GetCarbonEmissionWorkItemList(eng.carbonEmissionPayItems);
            //第五章 材料設備清冊
            EngMaterialDeviceListService engMaterialDeviceListService = new EngMaterialDeviceListService();
            eng.engMaterialDeviceItems = engMaterialDeviceListService.GetListAll<EngMaterialDeviceList2VModel>(eng.Seq);
            foreach(EngMaterialDeviceList2VModel m in eng.engMaterialDeviceItems)
            {
                m.GetQCStd();
                m.engMaterialDeviceSummaryItems = engMaterialDeviceListService.GetEMDSummaryList<EngMaterialDeviceSummaryModel>(m.Seq);
            }
            //第六章 設備功能運轉測試抽驗程序及標準
            eng.equOperTestItems = new EquOperTestListService().GetListAll<EquOperTestListV2Model>(eng.Seq);
            foreach (EquOperTestListV2Model m in eng.equOperTestItems)
            {
                m.GetQCStd();
                m.GetFlowChart();
            }
            //第七章 701 施工抽查程序及標準
            eng.constCheckItems = new ConstCheckListService().GetListAll<ConstCheckListV2Model>(eng.Seq);
            foreach (ConstCheckListV2Model m in eng.constCheckItems)
            {
                m.GetQCStd();
                m.GetFlowChart();
            }
            //第七章 702 環境保育抽查標準
            eng.envirConsItems = new EnvirConsListService().GetListAll<EnvirConsListV2Model>(eng.Seq);
            foreach(EnvirConsListV2Model m in eng.envirConsItems)
            {
                m.GetQCStd();
                m.GetFlowChart();
            }
            //第七章 703 職業安全衛生抽查標準
            eng.occuSafeHealthItems = new OccuSafeHealthListService().GetListAll<OccuSafeHealthListV2Model>(eng.Seq);
            foreach (OccuSafeHealthListV2Model m in eng.occuSafeHealthItems)
            {
                m.GetQCStd();
                m.GetFlowChart();
            }

            if (engMainService.CopyEng(id, eNo, eng))
            {
                return Json(new
                {
                    result = 0,
                    msg = "複製完成"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "複製失敗"
            });
        }
    }
}