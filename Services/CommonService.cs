using EQC.Models;

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Services
{
    public static class CommonService
    {
        private static DBConn db = new DBConn();
        public static string ToJSON(FormCollection collection)
        {
            var list = new Dictionary<string, string>();
            foreach(string key in collection.Keys)
            {
                list.Add(key, collection[key]);
            }
            return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(list);
        }

        public static string GetQueryType(string sourceControl, string fromBtn, int type)
        {
            string qtype = "00";
            switch (sourceControl.ToUpper())
            {
                case "TOTALDIST":
                    switch (fromBtn.ToUpper())
                    {
                        case "QUERYBTN":    // 快速按鈕
                            switch (Convert.ToInt32(type))
                            {
                                case 1:
                                    qtype = "11";  //未分派
                                    break;
                                case 2:
                                    qtype = "12";  //已分派
                                    break;
                                case 3:
                                    qtype = "14";  //已逾期
                                    break;
                                default:
                                    qtype = "00";
                                    break;
                            }
                            break;
                        case "INQUIRYBTN":  // 查詢
                            qtype = "11";   // 預設未分派
                            break;
                    }
                    break;
                case "UNITDIST":
                    switch (fromBtn.ToUpper())
                    {
                        case "QUERYBTN":

                            switch (Convert.ToInt32(type))
                            {
                                case 1:     // 待處理(已派到該登記桌，還未分案件列表)
                                    qtype = "12";
                                    break;
                                case 2:     // 處理中(已派到該登記桌，還未結案件列表)
                                    qtype = "22";
                                    break;
                                case 3:     // 已逾期(已派到該登記桌，已逾期案件列表)
                                    qtype = "24";
                                    break;
                                case 4:     // 退件(該登記桌派至單位承辦，承辦退件列表)
                                    qtype = "25";
                                    break;
                                default:
                                    qtype = "00";
                                    break;
                            }
                            break;
                        case "INQUIRYBTN":
                            switch(Convert.ToInt32(type)) {
                                case 1: // 待分案
                                    qtype = "12";
                                    break;
                                case 2: // 已分案
                                    qtype = "12";
                                    break;
                                case 3: // 已逾期
                                    qtype = "14";
                                    break;
                                case 4: // 未派案
                                    qtype = "21";
                                    break;
                                case 5: // 未結案
                                    qtype = "22";
                                    break;
                                case 6: // 退件
                                    qtype = "25";
                                    break;
                                case 7: // 未處理
                                    qtype = "31";
                                    break;
                                case 8: // 已結案
                                    qtype = "33";
                                    break;
                                default:
                                    qtype = "12";
                                    break;
                            }
                            break;
                    }
                    break;
                case "CASEMANAGEMENT":
                    switch(fromBtn.ToUpper())
                    {
                        case "QUERYBTN":
                            switch(Convert.ToInt32(type))
                            {
                                case 1:     // 待處理(已派至該單位承辦，未第一次回覆民眾案件列表)
                                    qtype = "31";
                                    break;
                                case 2:     // 處理中(已派至該單位承辦，未結案件列表)
                                    qtype = "32";
                                    break;
                                case 3:     // 已逾期(已派至該單位承辦，已逾期案件列表)
                                    qtype = "35";
                                    break;
                            }
                            break;
                        case "INQUIRYBTN":
                            switch (Convert.ToInt32(type))
                            {
                                case 1: // 待分案
                                    qtype = "11";
                                    break;
                                case 2: // 已分案
                                    qtype = "12";
                                    break;
                                case 3: // 已逾期
                                    qtype = "14";
                                    break;
                                case 4: // 未派案
                                    qtype = "21";
                                    break;
                                case 5: // 未結案
                                    qtype = "22";
                                    break;
                                case 7: // 未處理
                                    qtype = "31";
                                    break;
                                case 8: // 已結案
                                    qtype = "33";
                                    break;
                                default:
                                    qtype = "12";
                                    break;
                            }
                            break;
                    }
                    break;
                case "INQUIRY":
                    switch(fromBtn.ToUpper())
                    {
                        case "INQUIRYBTN":
                            switch (Convert.ToInt32(type))
                            {
                                case 1: // 待分案
                                    qtype = "11";
                                    break;
                                case 2: // 已分案
                                    qtype = "12";
                                    break;
                                case 3: // 已逾期
                                    qtype = "14";
                                    break;
                                case 4: // 未派案
                                    qtype = "21";
                                    break;
                                case 5: // 未結案
                                    qtype = "22";
                                    break;
                                case 6: // 退件
                                    qtype = "25";
                                    break;
                                case 7: // 未處理
                                    qtype = "31";
                                    break;
                                case 8: // 已結案
                                    qtype = "33";
                                    break;
                            }
                            break;
                    }
                    break;
                case "EXPEDITING":
                    switch (fromBtn.ToUpper())
                    {
                        case "INQUIRYBTN":
                            switch (Convert.ToInt32(type))
                            {
                                case 2: // 已分案
                                    qtype = "12";
                                    break;
                                case 3: // 已逾期
                                    qtype = "14";
                                    break;
                                case 4: // 未派案
                                    qtype = "21";
                                    break;
                                case 5: // 未結案
                                    qtype = "22";
                                    break;
                                case 6: // 退件
                                    qtype = "25";
                                    break;
                                case 7: // 未處理
                                    qtype = "31";
                                    break;
                            }
                            break;
                    }
                    break;
                case "UNFOUNDEDCASE":
                    switch (fromBtn.ToUpper())
                    {
                        case "INQUIRYBTN":
                            switch (Convert.ToInt32(type))
                            {
                                case 3: // 已逾期
                                    qtype = "02";
                                    break;
                                case 9: // 未逾期
                                    qtype = "03";
                                    break;
                            }
                            break;
                        default:
                            qtype = "00";
                            break;
                    }
                    break;
            }
            return qtype;
        }

        public static List<Stage> GetStageList(int stageType)
        {
            string sql = @"
                SELECT StageCode, StageName
                FROM FN_GetPetitionStage(@StageType)
                ORDER BY StageCode";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@StageType", stageType);
            return db.GetDataTableWithClass<Stage>(cmd);
        }
    }
}