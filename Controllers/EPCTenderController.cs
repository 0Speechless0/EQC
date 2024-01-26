using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Xml;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EPCTenderController : Controller
    {//工程履約子系統 - 工程標案清單
        public ActionResult Index3()
        {
            Utils.setUserClass(this);
            return View("Index", "_MainLayout");
        }
        public ActionResult Index9()
        {
            Utils.setUserClass(this);
            return View("Index", "_MainLayout");
        }
        //編輯標案
        public ActionResult EditTender(int seq, string tarEdit)
        {
            var redirectUrl = new UrlHelper(Request.RequestContext).Action(tarEdit, "EPCTender");
            string tarUrl = redirectUrl.ToString() + "?id=" + seq;
            return Json(new { Url = tarUrl });
        }
        //編輯標案
        public ActionResult EditTenderPlan(int seq, string tarEdit)
        {
            var redirectUrl = new UrlHelper(Request.RequestContext).Action(tarEdit+"Plan", "EPCTender");
            string tarUrl = redirectUrl.ToString() + "?id=" + seq;
            return Json(new { Url = tarUrl });
        }
        //編輯標案
        public ActionResult Edit3Plan()
        {
            //ViewBag.Title = "監造計畫-編輯";
            /*List<VMenu> menu = new List<VMenu>();
            var link = new UrlHelper(Request.RequestContext).Action("Index", "SupervisionPlan");
            menu.Add(new VMenu() { Name = "監造計畫", Url = link.ToString() });
            menu.Add(new VMenu() { Name = "監造計畫-編輯", Url = "" });
            ViewBag.breadcrumb = menu;*/

            Utils.setUserClass(this);
            return View("TenderPlanEdit", "_MainLayout");
        }
        public ActionResult Edit9Plan()
        {
            Utils.setUserClass(this);
            return View("TenderPlanEdit", "_MainLayout");
        }
        //編輯標案
        public ActionResult Edit3()
        {
            //ViewBag.Title = "監造計畫-編輯";
            /*List<VMenu> menu = new List<VMenu>();
            var link = new UrlHelper(Request.RequestContext).Action("Index", "SupervisionPlan");
            menu.Add(new VMenu() { Name = "監造計畫", Url = link.ToString() });
            menu.Add(new VMenu() { Name = "監造計畫-編輯", Url = "" });
            ViewBag.breadcrumb = menu;*/

            Utils.setUserClass(this);
            return View("TenderEdit", "_MainLayout");
        }
        public ActionResult Edit9()
        {
            Utils.setUserClass(this);
            return View("TenderEdit", "_MainLayout");
        }
        //工程標案清單 by PrjXML.Seq
        public virtual JsonResult GetItem(int id)
        {
            List<TenderModel> tender = new EPCTenderService().GetItemDetail(id);
            if (tender.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "查無資料"
                });
                
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    item = tender[0]
                });
            }
        }
        //工程標案 by EngMain.Seq
        public JsonResult GetTrender(int id)
        {
            //List<EPCTendeVMode> tender = new EPCTenderService().GetTrender<EPCTendeVMode>(id);
            List<EPCTendeVModel> tender = new EPCProgressManageService().GetEngLinkTenderBySeq<EPCTendeVModel>(id);//20220602
            //
            if (tender.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "查無資料"
                });

            }
            else
            {
                return Json(new
                {
                    result = 0,
                    item = tender[0]
                });
            }
        }
        //工程標案清單
        public virtual JsonResult GetList(string year, int unit, string keyWord, int pageRecordCount, int pageIndex)
        {
            int engMain = -1;
            EPCTenderService tService = new EPCTenderService();
            List<EPCEngMainVModel> engList = new List<EPCEngMainVModel>();
            //List<EngNameOptionsVModel> engNames = new List<EngNameOptionsVModel>();
            int total = tService.GetEngCreatedListCount(year, unit, -1, engMain, keyWord);
            if (total > 0)
            {
                engList = tService.GetEngCreatedList<EPCEngMainVModel>(year, unit, -1, engMain, pageRecordCount, pageIndex, keyWord);
               /* engNames = tService.GetEngCreatedList<EngNameOptionsVModel>(year, unit, -1, engMain, 9999, 1);
                engNames.Sort((x, y) => x.CompareTo(y));
                EngNameOptionsVModel m = new EngNameOptionsVModel();
                m.Seq = -1;
                m.EngName = "全部工程";
                engNames.Insert(0, m);*/
            }
            return Json(new
            {
                pTotal = total,
                items = engList
                //engNameItems = engNames
            });
            /*keyWord = (String.IsNullOrEmpty(keyWord) ? "" : "%"+keyWord + "%");
            List <EPCEngTenderListVMode> engList = new List<EPCEngTenderListVMode>();
            int total = iService.GetTenderListCount(year, unit, keyWord);
            if (total > 0)
            {
                engList = iService.GetTenderList<EPCEngTenderListVMode>(year, unit, keyWord, pageRecordCount, pageIndex);
            }
            return Json(new
            {
                pTotal = total,
                items = engList,
            });*/
       }
        //使用者所屬機關
        public virtual ActionResult GetUserUnit()
        {
            string unitSubSeq = "";
            string unitSeq = "";
            Utils.GetUserUnit(ref unitSeq, ref unitSubSeq);
            return Json(new
            {
                result = 0,
                unit = unitSeq,
                unitSub = unitSubSeq
            });
        }
        //標案年分
        public virtual JsonResult GetYearOptions()
        {
            //List<EngYearVModel> years = new PrjXMLService().GetTenderYearList();
            List<EngYearVModel> years = new EPCTenderService().GetTenderYearList();//shioulo 20220712
            return Json(years);
        }
        //依年分取執行機關
        public virtual JsonResult GetUnitOptions(string year)
        {
            //List<EngExecUnitsVModel> items = new PrjXMLService().GetTenderExecUnitList(year);
            List<EngExecUnitsVModel> items = new EPCTenderService().GetTenderExecUnitList(year);//shioulo 20220712
            return Json(items);
        }

        //委員資料
        //新增
        public virtual JsonResult AddCommittee(PrjXMLCommitteeModel item)
        {
            if (new PrjXMLCommitteeService().Add(item) > 0) {
                return Json(new
                {
                    result = 0,
                    msg = "新增成功"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "新增失敗"
            });
        }
        //更新
        public virtual JsonResult UpdateCommittee(PrjXMLCommitteeModel item)
        {
            if (new PrjXMLCommitteeService().Update(item) > 0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "更新成功"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "更新失敗"
            });
        }
        //刪除
        public virtual JsonResult DelCommittee(int id)
        {
            if (new PrjXMLCommitteeService().Del(id) > 0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "刪除成功"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "刪除失敗"
            });
        }
        public virtual JsonResult GetCommitteeList(int id)
        {
            List<PrjXMLCommitteeModel> items = new PrjXMLCommitteeService().GetCommittee<PrjXMLCommitteeModel>(id);
            return Json(new
            {
                result = 0,
                items = items
            });
        }
        //上傳 PCCESS xml
        public JsonResult UploadCommitteeXML(int id)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                try
                {
                    string fullPath = Utils.GetTempFile(".xml");// Path.Combine(tempPath, uuid + ".xml");
                    //string tempPath = Path.GetTempPath();
                    //string fullPath = Path.Combine(tempPath, "SchProgress-" + file.FileName);
                    file.SaveAs(fullPath);
                    List<EPCEngPrjVModel> items = new EngMainService().GetItemBySeq<EPCEngPrjVModel>(id);
                    if (items.Count != 1)
                    {
                        return Json(new
                        {
                            result = -1,
                            msg = "工程資料讀取錯誤"
                        });
                    }
                    //
                    string errCnt = "";
                    EPCEngPrjVModel eng = items[0];
                    List<PrjXMLCommitteeModel> updateList = new List<PrjXMLCommitteeModel>();
                    string actualBiddingMethod = "";
                    string bidAwardMethod = "";
                    int result = processXML(fullPath, eng, updateList, ref actualBiddingMethod, ref bidAwardMethod, ref errCnt);
                    if (result == -1)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "工程/主辦單位/執行單位 錯誤\n"+errCnt
                        });
                    }

                    int iCnt = 0, uCnt = 0;
                    
                    result = new PrjXMLCommitteeService().ImportData(updateList, eng.PrjXMLSeq.Value, actualBiddingMethod, bidAwardMethod, ref iCnt, ref uCnt, ref errCnt);
                    if (result == -1)
                    {
                        return Json(new
                        {
                            result = 0,
                            message = "實際招標方式 資料錯誤"
                        });
                    } else if (result == -2)
                    {
                        return Json(new
                        {
                            result = 0,
                            message = "決標方式 資料錯誤"
                        });
                    }

                    string msg = String.Format("上傳檔案更新完成\n新增:{0}, 更新:{1}", iCnt, uCnt);
                    if(!String.IsNullOrEmpty(errCnt))
                    {
                        msg += "\n錯誤:" + errCnt;
                    }
                    //if (result > 0)
                    //{
                        return Json(new
                        {
                            result = 0,
                            message = msg
                        });
                    //}
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                }
                return Json(new
                {
                    result = -1,
                    message = "更新失敗"
                });
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }
        private int processXML(string fileName, EPCEngPrjVModel eng, List<PrjXMLCommitteeModel> updateList, ref string actualBiddingMethod, ref string bidAwardMethod, ref string errCnt)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            //System.Diagnostics.Debug.WriteLine(doc.ToString());
            //
            XmlNode root = doc.ChildNodes.Item(1);
            XmlElement eRoot = (XmlElement)root;
            String tenderName = eRoot.GetElementsByTagName("TENDER_NAME")[0].InnerText;

            String tenderOrgName = Utils.filterUnitName(eRoot.GetElementsByTagName("TENDER_ORG_NAME")[0].InnerText);
            /*if (tenderOrgName == "經濟部水利署")
                tenderOrgName = "水利署";
            else
                tenderOrgName = tenderOrgName.Replace("經濟部水利署", "");*/

            String execOrgName = Utils.filterUnitName(eRoot.GetElementsByTagName("EXEC_ORG_NAME")[0].InnerText);
            /*if (execOrgName == "經濟部水利署")
                execOrgName = "水利署";
            else
                execOrgName = execOrgName.Replace("經濟部水利署", "");*/

            if (eng.TenderName != tenderName || eng.tenderOrgUnitName != tenderOrgName || eng.tenderExecUnitName != execOrgName)
            {
                errCnt = String.Format("工程:\n{0}\n{1}, {2}\n\n匯入:\n{3}\n{4}, {5}",
                    eng.TenderName, eng.tenderOrgUnitName, eng.tenderExecUnitName
                    , tenderName, tenderOrgName, execOrgName);
                return -1;
            }

            XmlNodeList temp = eRoot.GetElementsByTagName("TENDER_WAY"); //實際招標方式
            if (temp.Count == 1) actualBiddingMethod = temp[0].InnerText;
            temp = eRoot.GetElementsByTagName("TENDER_AWARD_WAY"); //決標方式
            if (temp.Count == 1) bidAwardMethod = temp[0].InnerText;

            List<PrjXMLCommitteeModel> committeeList = new List<PrjXMLCommitteeModel>();
            XmlNodeList externalExperts3List = eRoot.GetElementsByTagName("externalExperts3List");//外部專家
            foreach (XmlNode node in externalExperts3List) {
                XmlElement xe = (XmlElement)node;
                temp = xe.GetElementsByTagName("name");
                if (temp.Count == 1)
                {
                    PrjXMLCommitteeModel m = new PrjXMLCommitteeModel() { Seq = -1, Kind = 1, PrjXMLSeq = eng.PrjXMLSeq.Value, Profession = "", Experience = "" };
                    m.CName = temp[0].InnerText;
                    temp = xe.GetElementsByTagName("isPresence");//出席
                    if (temp.Count == 1 && temp[0].InnerText == "Y")
                        m.IsPresence = true;
                    else
                        m.IsPresence = false;
                    temp = xe.GetElementsByTagName("position");//職業
                    if (temp.Count == 1) m.Profession = temp[0].InnerText;
                    temp = xe.GetElementsByTagName("educationInstitution1");//學經歷
                    if (temp.Count == 1) m.Experience = temp[0].InnerText;
                    committeeList.Add(m);
                }
            }
            XmlNodeList internalExpertsList = eRoot.GetElementsByTagName("internalExpertsList");//內部委員
            foreach (XmlNode node in internalExpertsList)
            {
                XmlElement xe = (XmlElement)node;
                temp = xe.GetElementsByTagName("name");
                if (temp.Count == 1)
                {
                    PrjXMLCommitteeModel m = new PrjXMLCommitteeModel() { Seq = -1, Kind = 2, PrjXMLSeq = eng.PrjXMLSeq.Value, Profession = "", Experience = "" };
                    m.CName = temp[0].InnerText;
                    temp = xe.GetElementsByTagName("isPresence");//出席
                    if (temp.Count == 1 && temp[0].InnerText == "Y")
                        m.IsPresence = true;
                    else
                        m.IsPresence = false;
                    temp = xe.GetElementsByTagName("position");//職業
                    if (temp.Count == 1) m.Profession = temp[0].InnerText;
                    temp = xe.GetElementsByTagName("experienceAgency1");//學經歷
                    if (temp.Count == 1) m.Experience = temp[0].InnerText;
                    temp = xe.GetElementsByTagName("experienceTitle1");//學經歷
                    if (temp.Count == 1) m.Experience += temp[0].InnerText;
                    committeeList.Add(m);
                }
            }
            List<PrjXMLCommitteeModel> oldList = new PrjXMLCommitteeService().GetCommittee<PrjXMLCommitteeModel>(eng.PrjXMLSeq.Value);
            Dictionary<string, PrjXMLCommitteeModel> dic = new Dictionary<string, PrjXMLCommitteeModel>();
            foreach(PrjXMLCommitteeModel item in oldList)
            {
                dic.Add(item.Kind.ToString()+item.CName, item);
            }
            foreach (PrjXMLCommitteeModel item in committeeList)
            {
                string key = item.Kind.ToString() + item.CName;
                if (dic.ContainsKey(key))
                {
                    item.Seq = dic[key].Seq;
                    updateList.Add(item);
                    dic.Remove(key);
                }
                else
                    updateList.Add(item);
            }

            return 1;
        }
    }
}