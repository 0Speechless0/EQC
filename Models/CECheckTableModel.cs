using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EQC.Models
{
    public class CECheckTableModel
    {//碳排放量檢核表
        public int Seq { get; set; }
        public int CarbonEmissionHeaderSeq { get; set; }
        public byte? F1101 { get; set; }
        public byte? F1102 { get; set; }
        public byte? F1103 { get; set; }
        public byte? F1104 { get; set; }
        public byte? F1105 { get; set; }
        public byte? F1106 { get; set; }
        public string F1106TreeJson { get; set; }
        public List<CECheckTableTreeModel> TreeList { get; set; }
        public byte? F1107 { get; set; }
        public string F1107Desc { get; set; }
        public string F1107TreeJson { get; set; } //s20231006
        public int F1107TreeTotal { get; set; } //s20231006
        public List<CECheckTableTreeModel> ShrubList { get; set; } //s20231006 灌木
        public byte? F1108 { get; set; } //s20230518
        public int? F1108Area { get; set; }
        public byte? F1109 { get; set; } //s20230518
        public int? F1109Length { get; set; }
        public byte? F1110 { get; set; } //s20230518
        public string F1110Desc { get; set; }
        public byte? F1201 { get; set; }
        public byte? F1201Mix { get; set; }
        public string F1201Other { get; set; }
        public byte? F1202 { get; set; }
        public byte? F1203 { get; set; }
        public byte? F1204 { get; set; }
        public byte? F1205 { get; set; }
        public byte? F1206 { get; set; }
        public string F1206Desc { get; set; }
        public byte? F1301 { get; set; }
        public byte? F1302 { get; set; }
        public byte? F1303 { get; set; }
        public byte? F1304 { get; set; }
        public byte? F1305 { get; set; }
        public byte? F1306 { get; set; }
        public byte? F1306Mode { get; set; }
        public byte? F1307 { get; set; }
        public byte? F1307Mode { get; set; }
        public byte? F1308 { get; set; }
        public byte? F1309 { get; set; }
        public byte? F1309Mode { get; set; }
        public byte? F1310 { get; set; }
        public byte? F1310Mode { get; set; }
        public byte? F1311 { get; set; }
        public byte? F1312 { get; set; }
        public string F1312Desc { get; set; }
        public byte? F1401 { get; set; }
        public byte? F1402 { get; set; }
        public byte? F1403 { get; set; }
        public byte? F1404 { get; set; }
        public string F1404Desc { get; set; }
        public byte? F2101 { get; set; }
        public byte? F2102 { get; set; }
        public byte? F2103 { get; set; }
        public string F2103Desc { get; set; }
        public byte? F2201 { get; set; }
        public byte? F2202 { get; set; }
        public byte? F2203 { get; set; }
        public byte? F2204 { get; set; }
        public string F2204Desc { get; set; }
        public byte? F2301 { get; set; }
        public byte? F2302 { get; set; }
        public byte? F2303 { get; set; }
        public string F2303Desc { get; set; }
        public byte? F3101 { get; set; }
        public byte? F3102 { get; set; }
        public byte? F3103 { get; set; }
        public byte? F3104 { get; set; }
        public string F3104Desc { get; set; }
        public byte? F3201 { get; set; }
        public byte? F3202 { get; set; }
        public byte? F3203 { get; set; }
        public byte? F3204 { get; set; }
        public string F3204Desc { get; set; }
        public byte? F3301 { get; set; }
        public byte? F3302 { get; set; }
        public string F3302Desc { get; set; }
        public byte? F3401 { get; set; }
        public byte? F3402 { get; set; }
        public byte? F3403 { get; set; }
        public string F3403Desc { get; set; }
        public string Signature { get; set; }
        public string Remark { get; set; } //s20231006

        public CECheckTableModel()
        {
            TreeList = InitTreeList();
            ShrubList = InitShrubList(); //s20231006
        }
        //樹木清單
        private List<CECheckTableTreeModel> InitTreeList()
        {
            List<CECheckTableTreeModel> items = new List<CECheckTableTreeModel>();
            items.Add(new CECheckTableTreeModel() { Seq = 1, TreeName = "相思樹" });
            items.Add(new CECheckTableTreeModel() { Seq = 2, TreeName = "臺灣杉" });
            items.Add(new CECheckTableTreeModel() { Seq = 3, TreeName = "柳杉" });
            items.Add(new CECheckTableTreeModel() { Seq = 4, TreeName = "杉木" });
            items.Add(new CECheckTableTreeModel() { Seq = 5, TreeName = "光臘樹" });
            items.Add(new CECheckTableTreeModel() { Seq = 6, TreeName = "肖楠" });
            items.Add(new CECheckTableTreeModel() { Seq = 7, TreeName = "楓香" });
            items.Add(new CECheckTableTreeModel() { Seq = 8, TreeName = "樟樹" });
            items.Add(new CECheckTableTreeModel() { Seq = 9, TreeName = "台灣櫸" });
            items.Add(new CECheckTableTreeModel() { Seq = 10, TreeName = "烏心石" });
            items.Add(new CECheckTableTreeModel() { Seq = 11, TreeName = "檜木" });
            items.Add(new CECheckTableTreeModel() { Seq = 12, TreeName = "松類" });
            items.Add(new CECheckTableTreeModel() { Seq = 13, TreeName = "木油桐" });
            items.Add(new CECheckTableTreeModel() { Seq = 9999, TreeName = "" });

            return items;
        }

        public void TreeJsonToList()
        {
            if (!String.IsNullOrEmpty(F1106TreeJson))
            {
                List<CETreeModel> trees = JsonConvert.DeserializeObject<List<CETreeModel>>(F1106TreeJson);
                foreach (CETreeModel tree in trees)
                {
                    foreach (CECheckTableTreeModel m in TreeList)
                    {
                        if (tree.S == 9999)
                        {
                            if (m.Seq == 9999)
                            {
                                m.Checked = true;
                                m.Amount = tree.A;
                                m.TreeName = tree.T;
                                break;
                            }
                        }
                        else if (m.TreeName == tree.T)
                        {
                            m.Checked = true;
                            m.Amount = tree.A;
                            break;
                        }
                    }
                }
            }
            //s20231006
            if (!String.IsNullOrEmpty(F1107TreeJson))
            {
                List<CETreeModel> trees = JsonConvert.DeserializeObject<List<CETreeModel>>(F1107TreeJson);
                foreach (CETreeModel tree in trees)
                {
                    foreach (CECheckTableTreeModel m in ShrubList)
                    {
                        if (tree.S == 9999)
                        {
                            if (m.Seq == 9999)
                            {
                                m.Checked = true;
                                m.Amount = tree.A;
                                m.TreeName = tree.T;
                                break;
                            }
                        }
                        else if (m.TreeName == tree.T)
                        {
                            m.Checked = true;
                            m.Amount = tree.A;
                            break;
                        }
                    }
                }
            }
        }
        public void TreeListToJson()
        {
            List<CETreeModel> trees = new List<CETreeModel>();
            foreach (CECheckTableTreeModel m in TreeList)
            {
                if (m.Checked) trees.Add(new CETreeModel() { S = m.Seq, T = m.TreeName, A = m.Amount });
            }
            if (trees.Count == 0)
            {
                F1106TreeJson = "[]";
            }
            else
            {
                F1106TreeJson = JsonConvert.SerializeObject(trees);
            }
            //s20231006
            trees = new List<CETreeModel>();
            int total = 0;
            foreach (CECheckTableTreeModel m in ShrubList)
            {
                if (m.Checked)
                {
                    trees.Add(new CETreeModel() { S = m.Seq, T = m.TreeName, A = m.Amount });
                    if(m.Amount.HasValue)
                    {
                        total += m.Amount.Value;
                    }
                }
            }
            F1107TreeTotal = total;
            if (trees.Count == 0)
            {
                F1107TreeJson = "[]";
            }
            else
            {
                F1107TreeJson = JsonConvert.SerializeObject(trees);
            }
        }

        //灌木清單 s20231006
        private List<CECheckTableTreeModel> InitShrubList()
        {
            List<CECheckTableTreeModel> items = new List<CECheckTableTreeModel>();
            items.Add(new CECheckTableTreeModel() { Seq = 1, TreeName = "圓柏" });
            items.Add(new CECheckTableTreeModel() { Seq = 2, TreeName = "小葉羅漢松" });
            items.Add(new CECheckTableTreeModel() { Seq = 3, TreeName = "台灣枇杷" });
            items.Add(new CECheckTableTreeModel() { Seq = 4, TreeName = "鵝掌藤" });
            items.Add(new CECheckTableTreeModel() { Seq = 5, TreeName = "垂榕" });
            items.Add(new CECheckTableTreeModel() { Seq = 6, TreeName = "九重葛" });
            items.Add(new CECheckTableTreeModel() { Seq = 7, TreeName = "海桐" });
            items.Add(new CECheckTableTreeModel() { Seq = 8, TreeName = "台灣海桐" });
            items.Add(new CECheckTableTreeModel() { Seq = 9, TreeName = "黃秋葵" });
            items.Add(new CECheckTableTreeModel() { Seq = 10, TreeName = "風鈴花" });
            items.Add(new CECheckTableTreeModel() { Seq = 11, TreeName = "山芙蓉" });
            items.Add(new CECheckTableTreeModel() { Seq = 12, TreeName = "木芙蓉" });
            items.Add(new CECheckTableTreeModel() { Seq = 13, TreeName = "黃槿" });
            items.Add(new CECheckTableTreeModel() { Seq = 14, TreeName = "油茶" });
            items.Add(new CECheckTableTreeModel() { Seq = 15, TreeName = "杜仲" });
            items.Add(new CECheckTableTreeModel() { Seq = 16, TreeName = "台灣樹蘭" });
            items.Add(new CECheckTableTreeModel() { Seq = 17, TreeName = "大葉樹蘭" });
            items.Add(new CECheckTableTreeModel() { Seq = 9999, TreeName = "" });

            return items;
        }
    }
}
