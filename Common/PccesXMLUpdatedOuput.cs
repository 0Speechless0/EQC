using DocumentFormat.OpenXml.Spreadsheet;
using EQC.Models;
using NPOI.OpenXmlFormats.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace EQC_CarbonEmissionCal.Common
{
    public class PccesXMLUpdatedOuput
    {
        private XmlDocument doc;
        private XmlNode eRoot;
        private string _saveFilePath;
        private XmlNamespaceManager manager;

        public PccesXMLUpdatedOuput(string fileName, string spaceStr= null)
        {
            doc = new XmlDocument();
            doc.Load(fileName);
            eRoot = doc.ChildNodes.Item(1);
            _saveFilePath = fileName;
            manager = new XmlNamespaceManager(eRoot.OwnerDocument.NameTable);
            manager.AddNamespace("ns", spaceStr ?? "http://pcstd.pcc.gov.tw/2003/eTender");
        }
        public FileStream exportUpdatedPayItemPCCESFile(List<CarbonEmissionPayItemModel> originPayItems)
        {
            var payItems = originPayItems;
            //    .Where(row => row.CarbonEmissionHeaderSeq == carbonHeaderSeq).ToList();
 
            //var belongWorkItems = payItems.Join(originWorkItems,
            //    row => row.Seq,
            //    row2 => row2.CarbonEmissionPayItemSeq,
            //    (row, row2) => row2)
            //    .OrderBy(row => row.ItemCode);


            try
            {

                var xmlPayItems = eRoot.SelectNodes("ns:DetailList//ns:PayItem", manager);
                var xmlRootPayItems = eRoot.SelectNodes("ns:DetailList/ns:PayItem", manager);
                var xmlPayItemUnits = eRoot.SelectNodes("//ns:Unit[@language='en'] | //ns:Unit[@language='zh-TW']", manager);
                
                //單位空字串重設
                foreach (XmlNode unitNode in xmlPayItemUnits)
                {

                    if(unitNode.InnerText == "")
                        unitNode.InnerText = " ";
                }


                for (int i = 0; i < xmlPayItems.Count; i++)
                {
                    var xmlPayItemNode = xmlPayItems[i];
                    if (payItems[i].Memo?.Length == 0) continue;
                    xmlPayItemNode.Attributes.GetNamedItem("refItemCode").InnerText = payItems[i].Memo;
                    var xmlPayItemUnit = xmlPayItemNode.SelectSingleNode("ns:Unit[@language='zh-TW']", manager);
                    //var xmlPayItemPrice = xmlPayItemNode.SelectSingleNode("ns:Price", manager);
                    //var xmlPayItemAmount = xmlPayItemNode.SelectSingleNode("ns:Amount", manager);

                    xmlPayItemUnit.InnerText = payItems[i].Unit == ""  || payItems[i].Unit == null ? " " : payItems[i].Unit;
                    //var xmlPayItemQuantity = xmlPayItemNode.SelectSingleNode("ns:Quantity", manager);
                   
                    //xmlPayItemQuantity.InnerText = decimal.ToInt32(payItems[i].Quantity ?? 0).ToString();
                    //xmlPayItemPrice.InnerText = payItems[i].Price.ToString();
                    //xmlPayItemAmount.InnerText = (payItems[i].Price * decimal.ToInt32(payItems[i].Quantity ?? 0)).ToString();

                }

                //根據數量的更動連動複價...總價
                //int totalAmount = 0;
                //for(int i = 0; i < xmlRootPayItems.Count - 1; i ++)
                //{
                //    totalAmount += calChildXmlNodesAmount(xmlRootPayItems[i]);
                //}
                //var totoalXmlPrice = xmlRootPayItems[xmlRootPayItems.Count - 1].SelectSingleNode("ns:Price");
                //var totoalXmlAmount = xmlRootPayItems[xmlRootPayItems.Count - 1].SelectSingleNode("ns:Amount");
                //totoalXmlPrice.InnerText = totalAmount.ToString();
                //totoalXmlAmount.InnerText = totalAmount.ToString(); 


            }
            catch (Exception e)
            {
                throw new Exception("XML格式錯誤，無法更新");
            }
            doc.Save(_saveFilePath);
            return new FileStream(_saveFilePath, FileMode.Open, FileAccess.Read,  FileShare.Read);

        }

        private int calChildXmlNodesAmount(XmlNode node)
        {
            int totalAmount = 0;
            var childPayItems = node.SelectNodes("ns:PayItem", manager);
            foreach (XmlNode child in childPayItems)
            {
                var ofspring = child.SelectNodes("ns:PayItem", manager);
                if(ofspring.Count > 0 )
                {
                    totalAmount += calChildXmlNodesAmount(child);
                }
                else
                {
                    var XmlAmount = child.SelectSingleNode("ns:Amount", manager);
                    totalAmount += Int32.Parse(XmlAmount.InnerText);
                }
            }
            var xmlPayItemAmount = node.SelectSingleNode("ns:Amount", manager);
            xmlPayItemAmount.InnerText = totalAmount.ToString();
            return totalAmount;
        }
        public FileStream exportUpdatedPayItemPCCESFile(int carbonHeaderSeq, List<CarbonEmissionPayItemModel> originPayItems)
        {
            var payItems = originPayItems
                .Where(row => row.CarbonEmissionHeaderSeq == carbonHeaderSeq).ToList();

            //var belongWorkItems = payItems.Join(originWorkItems,
            //    row => row.Seq,
            //    row2 => row2.CarbonEmissionPayItemSeq,
            //    (row, row2) => row2)
            //    .OrderBy(row => row.ItemCode);


            try
            {

                var xmlPayItems = eRoot.SelectNodes("ns:DetailList//ns:PayItem", manager);
                var xmlRootPayItems = eRoot.SelectNodes("ns:DetailList/ns:PayItem", manager);
                var xmlPayItemUnits = eRoot.SelectNodes("//ns:Unit[@language='en'] | //ns:Unit[@language='zh-TW']", manager);

                //單位空字串重設
                foreach (XmlNode unitNode in xmlPayItemUnits)
                {

                    if (unitNode.InnerText == "")
                        unitNode.InnerText = " ";
                }


                for (int i = 0; i < xmlPayItems.Count; i++)
                {
                    var xmlPayItemNode = xmlPayItems[i];
                    if (payItems[i].Memo?.Length == 0) continue;
                    xmlPayItemNode.Attributes.GetNamedItem("refItemCode").InnerText = payItems[i].Memo;
                    var xmlPayItemUnit = xmlPayItemNode.SelectSingleNode("ns:Unit[@language='zh-TW']", manager);
                    //var xmlPayItemPrice = xmlPayItemNode.SelectSingleNode("ns:Price", manager);
                    //var xmlPayItemAmount = xmlPayItemNode.SelectSingleNode("ns:Amount", manager);

                    xmlPayItemUnit.InnerText = payItems[i].Unit == "" || payItems[i].Unit == null ? " " : payItems[i].Unit;
                    //var xmlPayItemQuantity = xmlPayItemNode.SelectSingleNode("ns:Quantity", manager);

                    //xmlPayItemQuantity.InnerText = decimal.ToInt32(payItems[i].Quantity ?? 0).ToString();
                    //xmlPayItemPrice.InnerText = payItems[i].Price.ToString();
                    //xmlPayItemAmount.InnerText = (payItems[i].Price * decimal.ToInt32(payItems[i].Quantity ?? 0)).ToString();

                }

                //根據數量的更動連動複價...總價
                //int totalAmount = 0;
                //for(int i = 0; i < xmlRootPayItems.Count - 1; i ++)
                //{
                //    totalAmount += calChildXmlNodesAmount(xmlRootPayItems[i]);
                //}
                //var totoalXmlPrice = xmlRootPayItems[xmlRootPayItems.Count - 1].SelectSingleNode("ns:Price");
                //var totoalXmlAmount = xmlRootPayItems[xmlRootPayItems.Count - 1].SelectSingleNode("ns:Amount");
                //totoalXmlPrice.InnerText = totalAmount.ToString();
                //totoalXmlAmount.InnerText = totalAmount.ToString(); 


            }
            catch (Exception e)
            {
                throw new Exception("XML格式錯誤，無法更新");
            }
            doc.Save(_saveFilePath);
            return new FileStream(_saveFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        }
    }
}
