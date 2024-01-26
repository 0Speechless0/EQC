using EQC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace EQC.Common
{
    public class PccseXML
    {
        public PCCESSMainModel pccessMainModel = new PCCESSMainModel();
        public List<PCCESPayItemModel> payItems = new List<PCCESPayItemModel>();
        public EngMainModel engMainModel = null;
        public XmlElement eRoot;
        private XmlNodeList costBreakdownList;
        public PccseXML(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            //System.Diagnostics.Debug.WriteLine(doc.ToString());
            //
            XmlNode root = doc.ChildNodes.Item(1);
            eRoot = (XmlElement)root;
            costBreakdownList = eRoot.GetElementsByTagName("CostBreakdownList");
            TagTenderInformation(eRoot.GetElementsByTagName("TenderInformation"), pccessMainModel);
            TagDetailList(eRoot.GetElementsByTagName("DetailList"), payItems);
        }
        public PccseXML(Stream inputStream)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(inputStream);
            //System.Diagnostics.Debug.WriteLine(doc.ToString());
            //
            XmlNode root = doc.ChildNodes.Item(1);
            eRoot = (XmlElement)root;
            costBreakdownList = eRoot.GetElementsByTagName("CostBreakdownList");
            TagTenderInformation(eRoot.GetElementsByTagName("TenderInformation"), pccessMainModel);
            TagDetailList(eRoot.GetElementsByTagName("DetailList"), payItems);
        }
        public EngMainModel CreateEngMainModel(ref string errMsg)
        {
            if (pccessMainModel.contractNo == null || pccessMainModel.contractNo.Length == 0) return null;

            engMainModel = new EngMainModel();
            engMainModel.EngNo = pccessMainModel.contractNo.Trim();
            engMainModel.EngName = pccessMainModel.ContractTitle.Trim();
            try
            {
                engMainModel.EngYear = Int16.Parse(engMainModel.EngNo.Substring(0, 3));
            }catch(Exception e)
            {
                errMsg = "工程編號錯誤";
                return null;
            }
            engMainModel.OrganizerUnitCode = pccessMainModel.ProcuringEntityId.Trim();
            engMainModel.OrganizerUserSeq = Utils.getUserSeq();//承辦人預設登入者
            

            return engMainModel;
        }

        public void GetGrandTotalInformation()
        {
            if (engMainModel == null) return;
            TagGrandTotalInformation(eRoot.GetElementsByTagName("GrandTotalInformation"), engMainModel);//工程總預算

            engMainModel.SubContractingBudget = 0;//發包工作費
            
            bool flag = false;
            foreach (PCCESPayItemModel payItem in payItems)
            {
                if (payItem.Description == "發包工程費" || payItem.Description == "發包工作費")//20210820
                {
                    flag = true;
                    engMainModel.SubContractingBudget = payItem.Price;
                    break;
                }
            }
            //if(!flag) return -6;//20211213 不檢查 無發包工作費

            engMainModel.PurchaseAmount = 0;
            if (engMainModel.TotalBudget >= 50000000)
            {
                engMainModel.PurchaseAmount = 1;
            }
            else if (engMainModel.TotalBudget >= 10000000)
            {
                engMainModel.PurchaseAmount = 2;
            }
            else if (engMainModel.TotalBudget > 1000000)
            {
                engMainModel.PurchaseAmount = 3;
            }
        }
        //解析 總金額
        public void TagGrandTotalInformation(XmlNodeList nodes, EngMainModel m)
        {
            foreach (XmlNode node in nodes)
            {
                foreach (XmlNode item in node.ChildNodes)
                {
                    if (item.Name == "TotalAmount") //工程總預算(PCCESS帶入)
                    {
                        m.TotalBudget = int.Parse(item.InnerText);
                    }
                }
            }
        }

        //解析 標案基本資料元素
        public void TagTenderInformation(XmlNodeList nodes, PCCESSMainModel m)
        {
            XmlElement xe;

            foreach (XmlNode node in nodes)
            {
                //標案編號(工程編號)
                m.contractNo = node.Attributes["contractNo"].Value.Trim();

                if (m.contractNo == null || m.contractNo.Length == 0) break;

                xe = (XmlElement)node;
                //招標機關名稱(主辦機關
                XmlNodeList items = xe.GetElementsByTagName("ProcuringEntity");
                foreach (XmlNode item in items)
                {
                    if (item.Attributes["language"].Value == "zh-TW")
                    {
                        m.ProcuringEntity = item.InnerText.Trim();
                    }
                }
                //機關代碼(主辦機關代碼)
                items = xe.GetElementsByTagName("ProcuringEntityId");
                foreach (XmlNode item in items)
                {
                    m.ProcuringEntityId = item.InnerText.Trim();
                }
                //採購標案名稱(工程名稱)
                items = xe.GetElementsByTagName("ContractTitle");
                foreach (XmlNode item in items)
                {
                    if (item.Attributes["language"].Value == "zh-TW")
                    {
                        m.ContractTitle = item.InnerText.Trim();
                    }
                }
                //屐約地點(施工地點)
                items = xe.GetElementsByTagName("ContractLocation");
                foreach (XmlNode item in items)
                {
                    m.ContractLocation = item.InnerText.Trim();
                }
            }
        }

        //解析 詳細表元素
        public void TagDetailList(XmlNodeList nodes, List<PCCESPayItemModel> items)
        {
            foreach (XmlNode item in nodes)
            {
                foreach (XmlNode node in item.ChildNodes)
                {
                    if (node.Name == "PayItem")
                    {
                        PCCESPayItemModel m = new PCCESPayItemModel();
                        items.Add(m);
                        m.ItemKey = node.Attributes["itemKey"].Value.Trim();
                        string itemNo = node.Attributes["itemNo"].Value.Trim();//項次代碼
                        m.ItemNo = itemNo;
                        m.RefItemCode = node.Attributes["refItemCode"].Value.Trim();
                        if (itemNo.Trim().Length == 0)
                        {
                            itemNo = "===";
                        }
                        m.PayItem = itemNo;
                        TagDetailList_PayItem(itemNo + ",", node.ChildNodes, items, m);
                    }
                }
            }
            foreach(PCCESPayItemModel m in items)
            {
                getWorkItemRoot(m);
            }
        }
        //解析 PayItem
        public void TagDetailList_PayItem(string pItemNo, XmlNodeList nodes, List<PCCESPayItemModel> items, PCCESPayItemModel model)
        {
            //System.Diagnostics.Debug.WriteLine("PayItem item: " + model.PayItem);

            decimal decimalV = 0;
            int intV = 0;
            foreach (XmlNode node in nodes)
            {
                string nodeName = node.Name;
                string nodeText = node.InnerText.Trim();

                if (nodeName == "PayItem")
                {
                    string itemNo = node.Attributes["itemNo"].Value.Trim().Replace(".", ",").Replace(pItemNo, "");//項次代碼,去除包含父項次的內容
                    string subPItemNo = pItemNo + itemNo + ",";
                    //System.Diagnostics.Debug.Print(itemNo);
                    //if (itemNo != null && itemNo.Trim().Length > 0) {
                    PCCESPayItemModel m = new PCCESPayItemModel();
                    items.Add(m);
                    m.ItemNo = itemNo;
                    m.ItemKey = node.Attributes["itemKey"].Value.Trim();//項次編碼
                    m.RefItemCode = node.Attributes["refItemCode"].Value.Trim();//參照工項編號
                    if (int.TryParse(itemNo, out intV))//項次為數值,前補0以便排序, 補三位則須加大欄位
                    {
                        itemNo = String.Format("{0:00}", intV);
                    }
                    else if (itemNo.Trim().Length == 0)//小計 item
                    {
                        itemNo = "===";
                    }
                    if (model.PayItem.Length == 0)
                    {
                        m.PayItem = itemNo;
                    }
                    else
                    {
                        m.PayItem = model.PayItem + "," + itemNo;
                    }
                    TagDetailList_PayItem(subPItemNo, node.ChildNodes, items, m);
                    //}
                }
                else if (nodeName == "Description")//項目說明(材料設備名稱)
                {
                    if (node.Attributes["language"].Value == "zh-TW")
                    {
                        model.Description = nodeText;
                    }
                }
                else if (nodeName == "Unit")//單位
                {
                    if (node.Attributes["language"].Value == "zh-TW")
                    {
                        model.Unit = nodeText;
                    }
                }
                else if (nodeName == "Quantity" && decimal.TryParse(nodeText, out decimalV))//數量
                {
                    model.Quantity = decimalV;
                }
                else if (nodeName == "Price" && decimal.TryParse(nodeText, out decimalV))//單價
                {
                    model.Price = decimalV;
                }
                else if (nodeName == "Amount" && decimal.TryParse(nodeText, out decimalV))//複價(總價)
                {
                    model.Amount = decimalV;
                }
            }
        }

        private void getWorkItemRoot(PCCESPayItemModel m)
        {
            //System.Diagnostics.Debug.WriteLine(m.PayItem + " , " + m.RefItemCode);
            if (String.IsNullOrEmpty(m.RefItemCode)) return;
            int level = 0;
            foreach (XmlNode item in costBreakdownList)
            {
                foreach (XmlNode rootWorkItem in item.ChildNodes)
                {
                    //System.Diagnostics.Debug.WriteLine("root:" + rootWorkItem.Attributes["itemCode"].Value + " , " + rootWorkItem.Attributes["refItemNo"].Value);
                    XmlElement rWorkItem = (XmlElement)rootWorkItem;
                    if (m.RefItemCode == rWorkItem.Attributes["itemCode"].Value.Trim() )
                    {
                        XmlNodeList wiItems = rWorkItem.GetElementsByTagName("WorkItem");
                        foreach (XmlNode cItem in wiItems)
                        {
                            if(cItem.Name == "WorkItem")
                            {
                                level++;
                                //System.Diagnostics.Debug.WriteLine("wk: " + cItem.Attributes["itemCode"].Value);
                                //System.Diagnostics.Debug.WriteLine(workItem["Description"].InnerText);
                                PCCESWorkItemModel wi = new PCCESWorkItemModel();
                                wi.WorkItemQuantity = decimal.Parse(rWorkItem["Quantity"].InnerText.Trim());

                                XmlElement wItem = (XmlElement)cItem;
                                wi.ItemCode = wItem.Attributes["itemCode"].Value.Trim();
                                wi.ItemKind = wItem.Attributes["itemKind"].Value.Trim();
                                wi.Description = wItem["Description"].InnerText.Trim();
                                wi.Unit = wItem["Unit"].InnerText.Trim();
                                wi.Quantity = decimal.Parse(wItem["Quantity"].InnerText.Trim());
                                wi.Price = decimal.Parse(wItem["Price"].InnerText.Trim());
                                wi.Amount = decimal.Parse(wItem["Amount"].InnerText.Trim());
                                wi.Remark = wItem["Remark"].InnerText.Trim();
                                wi.LabourRatio = decimal.Parse(wItem["LabourRatio"].InnerText.Trim());
                                wi.EquipmentRatio = decimal.Parse(wItem["EquipmentRatio"].InnerText.Trim());
                                wi.MaterialRatio = decimal.Parse(wItem["MaterialRatio"].InnerText.Trim());
                                wi.MiscellaneaRatio = decimal.Parse(wItem["MiscellaneaRatio"].InnerText.Trim());
                                //
                                m.workItems.Add(wi);
                            }
                        }
                        return;
                    }
                }
            }
        }
    }
}