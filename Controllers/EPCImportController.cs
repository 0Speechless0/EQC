using EQC.Common;
using EQC.Models;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Xml;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EPCImportController : Controller
    {//工程履約子系統 - 標案資料匯入
        private EPCImportService iService = new EPCImportService();

        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }
        //匯入清單
        public JsonResult GetImportList(int pageRecordCount, int pageIndex)
        {
            List<PrjXMLImportVModel> engList = new List<PrjXMLImportVModel>();
            int total = iService.GetPrjXMLImportCount();
            if (total > 0)
            {
                try
                {
                    engList = iService.GetPrjXMLImportList<PrjXMLImportVModel>(pageRecordCount, pageIndex);
                }
                catch (Exception e)
                {
                    total = 0;
                }
            }
            return Json( new
            {
                pTotal = total,
                items = engList
            });
        }

        //上傳 PCCESS xml
        public JsonResult UploadXML(int processMode)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                try
                {
                    string uuid = System.Guid.NewGuid().ToString("B").ToUpper();
                    string tempPath = Path.GetTempPath();
                    string fullPath = Path.Combine(tempPath, uuid+".xml");
                    file.SaveAs(fullPath);
                    int total = processXML(fullPath, uuid);
                    if (total > 0) {
                        return Json(new
                        {
                            result = 0,
                            message = "上傳檔案完成",
                        });
                    } else {
                        return Json(new
                        {
                            result = 0,
                            message = "錯誤!! 檔案內無法解析標案資料",
                        });
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                    return Json(new
                    {
                        result = -1,
                        message = "檔案讀取錯誤:\n"+ e.Message
                    });
                }
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }


        private int processXML(string fileName, string uuid)
        {
            System.Diagnostics.Debug.WriteLine(fileName);
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            System.Diagnostics.Debug.WriteLine(doc.ToString());

            List<TenderModel> prjList = new List<TenderModel>();
            int total = 0, xmlErr = 0, saveErr = 0, saveOK = 0;

            XmlNode root = doc.ChildNodes.Item(1);
            XmlElement eRoot = (XmlElement)root;
            XmlNodeList prjNodes = eRoot.GetElementsByTagName("標案");
            foreach (XmlNode node in prjNodes)
            {
                total++;
                TenderModel m = new TenderModel();
                m.UpdateState = -1;
                m.UpdateMsg = "XML解析錯誤";
                try
                {
                    m.TenderNo = node.Attributes["編號"].Value;
                    m.ExecUnitCd = node.Attributes["執行機關代碼"].Value;
                    BaseData(node.SelectSingleNode("基本資料"), m);
                    ContractorQualityControlData(node.SelectSingleNode("廠商聘用品管人員"), m);
                    SupervisorData(node.SelectSingleNode("監造單位聘用現場人員"), m);
                    FullTimeEngineerData(node.SelectSingleNode("專任工程人員"), m);
                    SiteRelateData(node.SelectSingleNode("工地相關人員"), m);
                    BudgetingData(node.SelectSingleNode("預算編列"), m);
                    ChangeDesignDataData(node.SelectSingleNode("變更設計資料"), m);
                    ProgressData(node.SelectSingleNode("進度資料"), m);
                    BackwardData(node.SelectSingleNode("落後資料"), m);
                    PaymentRecordData(node.SelectSingleNode("歷次付款資料"), m);

                    if (!String.IsNullOrEmpty(m.TenderNo) && !String.IsNullOrEmpty(m.ExecUnitCd))
                    {
                        m.UpdateState = 0;
                        m.UpdateMsg = "";
                    } else
                    {
                        m.UpdateMsg = "無標案編號或執行機關代碼";
                    }
                }
                catch {
                    xmlErr++;
                }
                prjList.Add(m);
            }
            //
            foreach (TenderModel item in prjList)
            {

                if ( (item.UpdateState == 0) && iService.updatePrjFromXML(item) )
                {
                    saveOK++;
                    item.UpdateState = 1;
                }
                else
                {
                    saveErr++;
                }
            }
            if(total > 0)
            {
                iService.SavePrjXMLImport(prjList, saveOK, total- saveOK);
            }
            return total;
        }
        //解析 歷次付款資料
        private void PaymentRecordData(XmlNode n, TenderModel m)
        {
            if (n == null) return;
            int inx = 0, iv;
            decimal v;
            foreach (XmlNode node in n.ChildNodes)
            {
                try
                {
                    PaymentRecordModel item = new PaymentRecordModel();
                    if (int.TryParse(node.Attributes["次別"].Value, out iv)) item.PRItemNo = iv;
                    if (decimal.TryParse(getNodeStr("應付總金額", node), out v)) item.PRTotalAmountPayable = v;
                    item.PRPayDate = getNodeStr("請付日期", node);
                    if (decimal.TryParse(getNodeStr("請付金額", node), out v)) item.PRPayAmount = v;
                    item.PRSchePayDate = getNodeStr("預定付款日期", node);
                    if (decimal.TryParse(getNodeStr("預定付款金額", node), out v)) item.PRSchePayAmount = v;
                    item.PRActualPayDate = getNodeStr("實際付款日期", node);
                    if (decimal.TryParse(getNodeStr("實際付款金額", node), out v)) item.PRActualPayAmount = v;
                    if (decimal.TryParse(getNodeStr("累計已付總金額", node), out v)) item.PRAccuPayAmount = v;
                    item.PRMemo = getNodeStr("備註", node);
                    item.OrderNo = inx;
                    m.PaymentRecord.Add(item);
                    inx++;
                }
                catch { }
            }
        }
        //解析 落後資料
        private void BackwardData(XmlNode n, TenderModel m)
        {
            if (n == null) return;
            int inx = 0;
            int v;
            foreach (XmlNode node in n.ChildNodes)
            {
                try
                {
                    int year;
                    if (int.TryParse(node.Attributes["民國年"].Value, out year))
                    {
                        foreach (XmlNode month in node.ChildNodes)
                        {
                            BackwardDataModel item = new BackwardDataModel();
                            item.BDYear = year;
                            if (int.TryParse(month.Attributes["日曆月"].Value, out v)) item.BDMonth = v;
                            item.BDBackwardFactor = getNodeStr("落後因素", month);
                            item.BDAnalysis = getNodeStr("原因分析", month);
                            item.BDSolution = getNodeStr("解決辦法", month);
                            item.OrderNo = inx;
                            m.BackwardData.Add(item);
                            inx++;
                        }
                    }
                }
                catch { }
            }
        }
        //解析 進度資料
        private void ProgressData(XmlNode n, TenderModel m)
        {
            if (n == null) return;
            int inx = 0;
            int v;
            decimal dv;
            foreach (XmlNode node in n.ChildNodes)
            {
                try
                {
                    int year;
                    if (int.TryParse(node.Attributes["民國年"].Value, out year))
                    {
                        foreach (XmlNode month in node.ChildNodes)
                        {
                            ProgressDataModel item = new ProgressDataModel();
                            item.PDYear = year;
                            if (int.TryParse(month.Attributes["日曆月"].Value, out v)) item.PDMonth = v;
                            if (decimal.TryParse(getNodeStr("總累計預定進度", month), out dv)) item.PDAccuScheProgress = dv;
                            if (decimal.TryParse(getNodeStr("總累計實際進度", month), out dv)) item.PDAccuActualProgress = dv;
                            if (decimal.TryParse(getNodeStr("總累計預定完成金額", month), out dv)) item.PDAccuScheCloseAmount = dv;
                            if (decimal.TryParse(getNodeStr("總累計實際完成金額", month), out dv)) item.PDAccuActualCloseAmount = dv;
                            if (decimal.TryParse(getNodeStr("年累計預定進度", month), out dv)) item.PDYearAccuScheProgress = dv;
                            if (decimal.TryParse(getNodeStr("年累計實際進度", month), out dv)) item.PDYearAccuActualProgress = dv;
                            if (decimal.TryParse(getNodeStr("年累計預定完成金額", month), out dv)) item.PDYearAccuScheCloseAmount = dv;
                            if (decimal.TryParse(getNodeStr("年累計實際完成金額", month), out dv)) item.PDYearAccuActualCloseAmount = dv;
                            if (decimal.TryParse(getNodeStr("總累計估驗計價金額", month), out dv)) item.PDAccuEstValueAmount = dv;
                            if (int.TryParse(getNodeStr("估驗計價保留款", month), out v)) item.PDEstValueRetentAmount = v;
                            item.PDExecState = getNodeStr("執行狀態", month);
                            item.PDActualExecMemo = getNodeStr("實際執行摘要", month);
                            if (decimal.TryParse(getNodeStr("應付未付數", month), out dv)) item.PDAccountPayable = dv;
                            item.OrderNo = inx;
                            m.ProgressData.Add(item);
                            inx++;
                        }
                    }
                }
                catch { }
            }
        }
        //解析 變更設計資料
        private void ChangeDesignDataData(XmlNode n, TenderModel m)
        {
            if (n == null) return;
            int inx = 0;
            Decimal v;
            int iv;
            foreach (XmlNode node in n.ChildNodes)
            {
                try
                {
                    ChangeDesignDataModel item = new ChangeDesignDataModel();
                    item.CDChangeDate = node.Attributes["日期"].Value;
                    item.CDAnnoNo = getNodeStr("公告編號", node);
                    item.CDApprovalNo = getNodeStr("核准文號", node);
                    if (Decimal.TryParse(getNodeStr("變更前金額", node), out v)) item.CDBeforeAmount = v;
                    if (Decimal.TryParse(getNodeStr("變更後金額", node), out v)) item.CDAfterAmount = v;
                    item.CDBeforeCloseDate = getNodeStr("變更前竣工日", node);
                    item.CDAfterCloseDate = getNodeStr("變更後竣工日", node);
                    if (int.TryParse(getNodeStr("準延天數", node), out iv)) item.CDApprovalDays = iv;
                    item.OrderNo = inx;
                    m.ChangeDesignData.Add(item);
                    inx++;
                }
                catch { }
            }
        }
        //解析 預算編列
        private void BudgetingData(XmlNode n, TenderModel m)
        {
            if (n == null) return;
            int inx = 0;
            Decimal v;
            foreach (XmlNode node in n.ChildNodes)
            {
                try
                {
                    BudgetingModel budgeting = new BudgetingModel();
                    budgeting.BudgetYear = node.Attributes["民國年"].Value;
                    budgeting.OrderNo = inx;
                    int sInx = 0;
                    foreach (XmlNode sub in node.ChildNodes)
                    {
                        BudgetKindModel bk = new BudgetKindModel();
                        bk.BudgetType = sub.Attributes["類別"].Value;
                        bk.BudgetSource = getNodeStr("預算來源", sub);
                        bk.BudgetAccount = getNodeStr("預算科目", sub);
                        if (Decimal.TryParse(getNodeStr("法定預算數", sub), out v)) bk.LegalBudget = v;
                        bk.OrderNo = sInx;
                        sInx++;
                        budgeting.BudgetKindList.Add(bk);
                    }
                    m.Budgeting.Add(budgeting);
                    inx++;
                }
                catch { }
            }
        }
        //解析 工地相關人員
        private void SiteRelateData(XmlNode n, TenderModel m)
        {
            if (n == null) return;
            int inx = 0;
            foreach (XmlNode node in n.ChildNodes)
            {
                try
                {
                    SiteRelateModel user = new SiteRelateModel();
                    user.SRName = node.Attributes["姓名"].Value;
                    user.SRLicenseKind = getNodeStr("職務類別", node);
                    user.SRLicenseNo = getNodeStr("證照號碼", node);
                    user.SRStartDate = getNodeStr("受聘本工程起始日期", node);
                    user.SREndDate = getNodeStr("受聘本工程結束日期", node);
                    user.SRMemo = getNodeStr("備註", node);
                    user.OrderNo = inx;
                    m.SiteRelate.Add(user);
                    inx++;
                }
                catch { }
            }
        }
        //解析 專任工程人員
        private void FullTimeEngineerData(XmlNode n, TenderModel m)
        {
            if (n == null) return;
            int inx = 0;
            foreach (XmlNode node in n.ChildNodes)
            {
                try
                {
                    FullTimeEngineerModel user = new FullTimeEngineerModel();
                    user.FEName = node.Attributes["姓名"].Value;
                    user.FELicenseKind = getNodeStr("證照種類", node);
                    user.FELicenseNo = getNodeStr("證照號碼", node);
                    user.FEStartDate = getNodeStr("負責本工程起始日期", node);
                    user.FEEndDate = getNodeStr("負責本工程結束日期", node);
                    user.FEMemo = getNodeStr("備註", node);
                    user.OrderNo = inx;
                    m.FullTimeEngineer.Add(user);
                    inx++;
                }
                catch { }
            }
        }
        //解析 監造單位聘用監工人員
        private void SupervisorData(XmlNode n, TenderModel m)
        {
            if (n == null) return;
            int inx = 0;
            foreach (XmlNode node in n.ChildNodes)
            {
                try
                {
                    SupervisorModel user = new SupervisorModel();
                    user.SPName = node.Attributes["姓名"].Value;
                    user.SPLicenseNo = getNodeStr("證照號碼", node);
                    user.SPSkill = getNodeStr("專長", node);
                    user.SPMoveinDate = getNodeStr("進駐工地日期", node);
                    user.SPDismissalDate = getNodeStr("解職日期", node);
                    user.SPState = getNodeStr("目前狀況", node);
                    user.OrderNo = inx;
                    m.Supervisor.Add(user);
                    inx++;
                }
                catch { }
            }
        }
        //解析 廠商聘用品管人員
        private void ContractorQualityControlData(XmlNode n, TenderModel m)
        {
            if (n == null) return;
            int inx = 0;
            foreach (XmlNode node in n.ChildNodes)
            {
                try
                {
                    ContractorQualityControlModel user = new ContractorQualityControlModel();
                    user.QCName = node.Attributes["姓名"].Value;
                    user.QCLicenseNo = getNodeStr("證照號碼", node);
                    user.QCSkill = getNodeStr("專長", node);
                    user.QCMoveinDate = getNodeStr("進駐工地日期", node);
                    user.QCDismissalDate = getNodeStr("解職日期", node);
                    user.QCState = getNodeStr("目前狀況", node);
                    user.OrderNo = inx;
                    m.ContractorQualityControl.Add(user);
                    inx++;
                } catch { }
            }
        }
        //解析 基本資料
        private void BaseData(XmlNode n, TenderModel m)
        {
            if (n == null) return;
            int v; decimal d;
            string b;
            XmlNode temp;

            m.ExecUnitName = Utils.filterUnitName(getNodeStr("執行機關", n));
            m.TenderName = getNodeStr("標案名稱", n);
            m.PrjName = getNodeStr("專案項目", n);
            m.EngType = getNodeStr("工程類別", n);
            m.TownName = getNodeStr("縣市鄉鎮", n);

            string[] coord = getNodeStr("座標", n).Split(',');
            if (coord.Length == 2)
            {
                decimal coordV;
                if (decimal.TryParse(coord[0], out coordV)) m.CoordX = coordV;
                if (decimal.TryParse(coord[1], out coordV)) m.CoordY = coordV;
            }

            m.Location = getNodeStr("施工地點", n);
            m.PlanOrganizerName = Utils.filterUnitName(getNodeStr("計畫主辦機關", n));
            m.PlanNo = getNodeStr("計畫編號", n);
            m.CompetentAuthority = Utils.filterUnitName(getNodeStr("主管機關", n));
            m.OrganizerName = Utils.filterUnitName(getNodeStr("主辦機關", n));
            m.FundingSourceName = Utils.filterUnitName(getNodeStr("經費來源機關", n));
            m.TenderNoticeUnit = getNodeStr("招標公告上線單位", n);

            m.ContactName = getNodeStr("聯絡人員/姓名", n);
            m.ContactPhone = getNodeStr("聯絡人員/電話", n);
            m.ContactEmail = getNodeStr("聯絡人員/電子郵件信箱", n);

            m.Weights = getNodeStr("權重", n);
            m.EngOverview = getNodeStr("工程概要", n);
            if (int.TryParse(getNodeStr("鋼材需求", n), out v)) m.SteelDemand = v;
            if (int.TryParse(getNodeStr("混凝土需求", n), out v)) m.ConcreteDemand = v;
            if (int.TryParse(getNodeStr("填方砂石需求", n), out v)) m.EarchworkDemand = v;
            m.DurationCategory = getNodeStr("工期類別", n);
            if (int.TryParse(getNodeStr("總天數", n), out v)) m.TotalDays = v;
            m.DurationDesc = getNodeStr("工期說明", n);
            m.BudgetAccount = getNodeStr("預算科目", n);
            if (decimal.TryParse(getNodeStr("工程總預算", n), out d)) m.TotalEngBudget = d;
            if (decimal.TryParse(getNodeStr("發包預算", n), out d)) m.OutsourcingBudget = d;
            if (int.TryParse(getNodeStr("供給材料費", n), out v)) m.SupplyMaterialCost = v;
            if (int.TryParse(getNodeStr("購地補償費", n), out v)) m.LandPurCompen = v;
            if (decimal.TryParse(getNodeStr("工程管理費", n), out d)) m.EngManageFee = d;
            if (decimal.TryParse(getNodeStr("空污費", n), out d)) m.AirPollutionFee = d;
            if (decimal.TryParse(getNodeStr("其他費用", n), out d)) m.OtherFee = d;
            m.PlanningUnitName = getNodeStr("規劃/單位名稱", n);

            m.DesignUnitName = getNodeStr("設計/單位名稱", n);
            if (decimal.TryParse(getNodeStr("設計/費用", n), out d)) m.DesignFee = d;
            m.DesignMemo = getNodeStr("設計/備註", n);

            m.SupervisionUnitName = getNodeStr("監造/單位名稱", n);
            if (decimal.TryParse(getNodeStr("監造/費用", n), out d)) m.SupervisionFee = d;
            m.SupervisionMemo = getNodeStr("監造/備註", n);

            m.ContractorName1 = getNodeStr("承攬/廠商名稱", n);
            m.ContractorName2 = getNodeStr("承攬/廠商名稱2", n);

            m.InsuranceDate = getNodeStr("保險/保險日期", n);
            if (decimal.TryParse(getNodeStr("保險/保險金額", n), out d)) m.InsuranceAmount = d;
            m.InsuranceNo = getNodeStr("保險/保險號碼", n);

            m.ActualAnnoDate = getNodeStr("實際公告日期", n);
            m.ScheBidReviewDate = getNodeStr("預定審標日期", n);
            m.ActualBidReviewDate = getNodeStr("實際審標日期", n);
            m.ScheBidOpeningDate = getNodeStr("預定開標日期", n);
            m.ActualBidOpeningDate = getNodeStr("實際開標日期", n);
            m.ScheBidAwardDate = getNodeStr("預定決標日期", n);
            m.ActualBidAwardDate = getNodeStr("實際決標日期", n);
            if(!String.IsNullOrEmpty(m.ActualBidAwardDate))
            {
                System.Diagnostics.Debug.WriteLine(m.ActualBidAwardDate.Substring(0, 3));
                Int16 i16;
                if (Int16.TryParse(m.ActualBidAwardDate.Substring(0,3), out i16)) m.TenderYear = i16;
            }
            m.ScheBiddingMethod = getNodeStr("預定招標方式", n);
            m.ActualBiddingMethod = getNodeStr("實際招標方式", n);
            m.BidAwardMethod = getNodeStr("決標方式", n);
            m.ContractFeePayMethod = getNodeStr("契約費用給付方式", n);
            if (decimal.TryParse(getNodeStr("預估底價", n), out d)) m.EstimateBasePrice = d;
            if (decimal.TryParse(getNodeStr("會合底價", n), out d)) m.RendBasePrice = d;
            if (decimal.TryParse(getNodeStr("決標金額", n), out d)) m.BidAmount = d;
            m.ContractNo = getNodeStr("合約編號", n);
            if (int.TryParse(getNodeStr("預付款", n), out v)) m.Prepayment = v;
            m.ScheStartDate = getNodeStr("預定開工日期", n);
            m.ActualStartDate = getNodeStr("實際開工日期", n);
            m.ScheCompletionDate = getNodeStr("預定完工日期", n);
            m.ScheCompletCloseDate = getNodeStr("預定竣工決算日期", n);
            if (decimal.TryParse(getNodeStr("品管費用", n), out d)) m.QualityControlFee = d;
            m.QualityPlanApproveUnit = Utils.filterUnitName(getNodeStr("品質計畫書核定單位", n));
            m.QualityPlanApproveDate = getNodeStr("品質計畫書核定日期", n);
            m.QualityPlanApproveNo = getNodeStr("品質計畫書核定文號", n);
            m.SupervisionPlanApproveUnit = Utils.filterUnitName(getNodeStr("監造計畫書核定單位", n));
            m.SupervisionPlanApproveDate = getNodeStr("監造計畫書核定日期", n);
            m.SupervisionPlanApproveNo = getNodeStr("監造計畫書核定文號", n);
            m.SiteContactMemo = getNodeStr("工地聯絡備註", n);
        }
        private string getNodeStr(string key, XmlNode n)
        {
            try
            {
                XmlNode node = n.SelectSingleNode(key);
                if (node != null) return node.InnerText.Trim();
            } catch { }
            return "";
        }
    }
}