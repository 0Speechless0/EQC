using DocumentFormat.OpenXml.Spreadsheet;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using EQC.Services;
using System.Linq;
using System.Configuration;
using System.Drawing;
using EQC.Interface;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using EQC.ViewModel.Interface;
using EQC.Models;
using NPOI.SS.UserModel;
using System.Text;


namespace EQC.Common
{
    public static class Utils
    {
        public static string rootPath;

        public static bool isNumber(string s)
        {
            int Flag = 0;
            char[] str = s.ToCharArray();
            for (int i = 0; i < str.Length; i++)
            {
                if (Char.IsNumber(str[i]))
                {
                    Flag++;
                }
                else
                {
                    Flag = -1;
                    break;
                }
            }

            if (Flag > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void GetEngDismantlingRate<T>(int engSeq, Func<EDMXModel.EngMain, IEnumerable<T> > payItemsGetter,ref decimal rate, ref decimal greenRate)
        where T : EDMXModel.InterFace.PayItem
        {
            using(var context = new EQC.EDMXModel.EQC_NEW_Entities())
            {
                rate = 0;
                greenRate = 0;
                var eng = context.EngMain.Find(engSeq);

                    
                var payItems = payItemsGetter(eng)?.Where(r => r.RStatusCode != 300).ToList() ;
                if (payItems == null) return;
                var bascAmount = payItems.Sum(r => r.Amount);
                if(bascAmount.HasValue && bascAmount > 0)
                {
                    var listA = payItems.Where(r => r.KgCo2e != null);
                    if (listA.Sum(r => r.Amount) is decimal d)
                    {
                        rate = decimal.Round(d / bascAmount.Value * 100, 2);
 
                    }
                    if (listA.Where(r => r.GreenFundingSeq != null ).Sum(r => r.Amount) is decimal dd)
                    {
                        greenRate = decimal.Round(dd / bascAmount.Value * 100, 2);

                    }
                }
            }
        }

        public static void importToOther<T>(this EQC.EDMXModel.EQC_NEW_Entities db, ICollection<T> source, ICollection<T> other)  where T : class, new()
        {

            source.ToList().ForEach(e =>
            {
                var t  = new T();
                other.Add(t);
                db.Entry(t).CurrentValues.SetValues(e);
            });
        }

        public static bool CheckDirectorOfSupervision(string userNo)
        {
            using (var context = new EDMXModel.EQC_NEW_Entities())
            {
                // 委辦監造主任判別
                var engNo = userNo.Substring(1);
                if (
                    context.EngMain.Select(r => r.EngNo)
                    .Contains(engNo)
                )
                    return true;

                // 自辦判別
                return context.EngSupervisor
                    .Where(r => r.UserKind == 0 && r.UserMain.UserNo == userNo).Count() > 0;
            }
        }

        public static IRow copyRowToNext(this ISheet sheet, int row)
        {
            var newRow = sheet.CreateRow(row +1);
            var standardRow = sheet.GetRow(row);
            int i = 0;
            while( i < standardRow.LastCellNum)
            {
                var newCell = newRow.CreateCell(i);
                var standardCell = standardRow.GetCell(i);
                var cellStyle = sheet.Workbook.CreateCellStyle();
                cellStyle.CloneStyleFrom(standardCell.CellStyle);
                if (standardCell.CellStyle != null)
                    newCell.CellStyle = cellStyle;
                newCell.CellStyle.SetFont(standardCell.CellStyle.GetFont(sheet.Workbook) );
                i++;
            }
            newRow.Height = standardRow.Height;
            if(standardRow.RowStyle != null)
                newRow.RowStyle.CloneStyleFrom(standardRow.RowStyle );
            return newRow;
        }
        public static DateTime?[] GetEngDate(this EDMXModel.EngMain eng)
        {
            var engStartDate = eng.EngChangeStartDate ?? eng.StartDate;
            var engEndDate = eng.EngChangeSchCompDate ?? eng.SchCompDate;
            return new DateTime?[] {
                engStartDate,
                engEndDate
            };
        }
        public static List<T> GetValueByJoin<T>(
            this List<T> originList,
            List<SelectOptionModel> externalList,
            Func<T, string> joinCondition,
            Action<T, string> joinAction
        )
        {
            return originList
                .GroupJoin(externalList,
                r1 => joinCondition.Invoke(r1),
                r2 => r2.Value,
                (r1, r2) => {
                    joinAction.Invoke(r1, r2.FirstOrDefault()?.Text);
                    return r1;
                }

                ).ToList();
        }
        public static void InteropWordAddPicFromBase64(this Microsoft.Office.Interop.Word.Application wordApp, Microsoft.Office.Interop.Word.Cell cell, string base64)
        {
            cell.Select();
            byte[] imageBytes = Convert.FromBase64String(base64.Replace("data:image/png;base64,", ""));
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                string fileName = Utils.GetTempFile(".png");
                image.Save(fileName);
                object linkToFile = false;
                object saveWithDocument = true;
                object range = wordApp.Selection.Range;
                Microsoft.Office.Interop.Word.InlineShape shape = wordApp.ActiveDocument.InlineShapes.AddPicture(fileName, ref linkToFile, ref saveWithDocument, ref range);
                shape.Width = 90f;
                shape.Height = 30f;
                shape.ConvertToShape().WrapFormat.Type = Microsoft.Office.Interop.Word.WdWrapType.wdWrapFront;
            }
        }
        public static string getEscapeDataString(this string str)
        {
            return Uri.EscapeDataString(Path.GetFileName(str));
        }
        public static string getHost()
        {
            Uri myuri = new Uri(System.Web.HttpContext.Current.Request.Url.AbsoluteUri);
            string pathQuery = myuri.PathAndQuery;
            return myuri.ToString().Replace(pathQuery, "");
        }
        public static void insert<T>(
            this DbSet<T> dbSet,
            IEnumerable<T> targetList

            ) where T : class, StandardRecord
        {
            var currentUser = new SessionManager().GetUser();
            foreach(var item in targetList)
            {
                item.CreateTime = DateTime.Now;
                item.CreateUser = currentUser.Seq;
                dbSet.Add(item);
            }

        }
        public static void delete<T>(
            this DbSet<T> dbSet, 
            List<T> targetList, 
            List<T> sourceList, 
            Func<T, object> keySelector
        
            ) where T : class
        {

            var targetIndexSet = targetList
                .Select(keySelector)
                .ToHashSet();

            foreach (var originItem in sourceList)
            {
                if (!targetIndexSet.Contains(keySelector.Invoke(originItem)))
                    dbSet.Remove(originItem);
            }
            
        }
        public static void update<T>(
            this DbContext dbconetxt, 
            List<T> targetList,
            List<T> sourceList,
            Func<T, object> keySelector

            ) where T : class, StandardRecord
        {
            var currentUser = new SessionManager().GetUser();
            var targetDic = targetList
                .ToDictionary(keySelector, row => row);
            var sourceDic = sourceList
                .ToDictionary(keySelector, row => row);

            foreach(var target in targetList)
            {
                if (sourceDic.TryGetValue(keySelector.Invoke(target), out T source))
                {
                    target.ModifyTime = DateTime.Now;
                    target.ModifyUser = currentUser.Seq;
                    target.CreateTime = source.CreateTime;
                    target.CreateUser = source.CreateUser;
                    dbconetxt.Entry(source).CurrentValues.SetValues(target);
                }

            }

        }
        public static List<T> getInsertList<T>(List<T> targetList, List<T> sourceList)
        {
            return targetList.Count > sourceList.Count ?
                    targetList.GetRange(sourceList.Count, targetList.Count - sourceList.Count) : new List<T>();
        }

        public static List<T> getDeleteList<T>(List<T> targetList, List<T> sourceList)
        {
            return targetList.Count < sourceList.Count ?
                    sourceList.GetRange(targetList.Count, sourceList.Count - targetList.Count ) : new List<T>();
        }

        public static int getPageCount<T>(this List<T> list,int perPage)
        {
            return (list.Count % perPage == 0) ?
                list.Count / perPage : list.Count / perPage + 1;
        }
        public static  List<T> getPagination<T>(this List<T> list, int page, int perPage)
        {
            var pageCount = list.getPageCount(perPage);
            if (page > pageCount ) page --;
            if(page > 0 )
                return list.GetRange((page - 1) * perPage,
                    (page - 1) * perPage + perPage > list.Count
                    ? list.Count - (page - 1) * perPage : perPage);
            else
            {
                return new List<T>();
            }
        }

        //各角色套版使用色系
        public static void setUserClass(Controller ctl, int classInx = -1)
        {
            ctl.ViewBag.gotopBtn = "btn-color1";
            UserInfo userInfo = getUserInfo();
            string userUnitName = "";
            using( var context = new  EQC.EDMXModel.EQC_NEW_Entities())
            {
                userUnitName =  context.UserMain.Find(userInfo.Seq).UserUnitPosition.FirstOrDefault()?.Role.FirstOrDefault()?.Name;
            }
            if (userInfo.IsOutsourceDesign)
            {
                //設計單位
                ctl.ViewBag.userRole = "6";
                ctl.ViewBag.nvaClass = "navbar navbar-expand-lg fixed-top navbar-bg-B";
                ctl.ViewBag.mainLeftClass = "main-left menu_bg_B";
                ctl.ViewBag.userUnit = userUnitName;
                ctl.ViewBag.cardClass = "card whiteBG mb-4 colorset_B";
                ctl.ViewBag.footerClass = "inside_footer footer-B";
                ctl.ViewBag.userIcon = "~/Content/images1/icon-003-design.png";
                ctl.ViewBag.gotopBtn = "btn-color12";
            }
            else if (userInfo.IsBuildContractor)
            {
                //施工單位
                ctl.ViewBag.userRole = "4";
                ctl.ViewBag.nvaClass = "navbar navbar-expand-lg fixed-top navbar-bg-R";
                ctl.ViewBag.mainLeftClass = "main-left menu_bg_R";
                ctl.ViewBag.userUnit = userUnitName;
                ctl.ViewBag.cardClass = "card whiteBG pattern-F colorset_R";
                ctl.ViewBag.footerClass = "inside_footer footer-R";
                ctl.ViewBag.userIcon = "~/Content/images1/icon-005-construction.png";
            }
            else if (userInfo.IsSupervisorUnit)
            {
                //監造主任
                ctl.ViewBag.userRole = "5";
                ctl.ViewBag.nvaClass = "navbar navbar-expand-lg fixed-top navbar-bg-G";
                ctl.ViewBag.mainLeftClass = "main-left menu_bg_G";
                ctl.ViewBag.userUnit = userUnitName;
                ctl.ViewBag.cardClass = "card whiteBG mb-4 pattern-F colorset_G";
                ctl.ViewBag.footerClass = "inside_footer footer-G";
                ctl.ViewBag.userIcon = "~/Content/images1/icon-004-supervisor.png";
            }
            else if (userInfo.IsDepartmentUser)
            {
                //署使用者
                ctl.ViewBag.userRole = "3";
                if (classInx == 2)
                {//工程品管
                    ctl.ViewBag.nvaClass = "navbar navbar-expand-lg fixed-top navbar-bg-B";
                    ctl.ViewBag.mainLeftClass = "main-left menu_bg_B";
                    ctl.ViewBag.userUnit = userUnitName;
                    ctl.ViewBag.cardClass = "card whiteBG mb-4 colorset_B";
                    ctl.ViewBag.footerClass = "inside_footer footer-B";
                    ctl.ViewBag.userIcon = "~/Content/images1/icon-002-agency.png";
                }
                else
                {
                    ctl.ViewBag.nvaClass = "navbar navbar-expand-lg fixed-top navbar-bg-1";
                    ctl.ViewBag.mainLeftClass = "main-left menu_bg_1";
                    ctl.ViewBag.userUnit = userUnitName;
                    ctl.ViewBag.cardClass = "card whiteBG mb-4 pattern-F colorset_1";
                    ctl.ViewBag.footerClass = "inside_footer footer-1";
                    ctl.ViewBag.userIcon = "~/Content/images1/icon-002-agency.png";
                }
            }
            else if (userInfo.IsCommittee)
            {
                //委員
                ctl.ViewBag.userRole = "7";
                ctl.ViewBag.nvaClass = "navbar navbar-expand-lg fixed-top navbar-bg-P";
                ctl.ViewBag.mainLeftClass = "main-left menu_bg_P";
                ctl.ViewBag.userUnit = userUnitName;
                ctl.ViewBag.cardClass = "card whiteBG mb-4 pattern-F colorset_P";
                ctl.ViewBag.footerClass = "inside_footer footer-P";
                ctl.ViewBag.userIcon = "~/Content/images1/icon-006-committee.png";
            }
            else if (userInfo.IsAdmin)
            {
                //系統管理者
                ctl.ViewBag.userRole = "1";
                ctl.ViewBag.nvaClass = "navbar navbar-expand-lg fixed-top navbar-bg-R";
                ctl.ViewBag.mainLeftClass = "main-left menu_bg_R";
                ctl.ViewBag.userUnit = "管理者";
                ctl.ViewBag.cardClass = "card whiteBG mb-4 colorset_R";
                ctl.ViewBag.footerClass = "inside_footer footer-R";
                ctl.ViewBag.userIcon = "~/Content/images1/icon-001-manager.png";
            }
            else
            {
                //署管理者
                ctl.ViewBag.userRole = "2";
                if (classInx == 1)
                {//工程督導
                    ctl.ViewBag.nvaClass = "navbar navbar-expand-lg fixed-top navbar-bg-3";
                    ctl.ViewBag.mainLeftClass = "main-left menu_bg_3";
                    ctl.ViewBag.userUnit = "管理者";
                    ctl.ViewBag.cardClass = "card whiteBG mb-4 colorset_3";
                    ctl.ViewBag.footerClass = "inside_footer footer-3";
                    ctl.ViewBag.userIcon = "~/Content/images1/icon-001-manager.png";
                }
                else
                {
                    ctl.ViewBag.nvaClass = "navbar navbar-expand-lg fixed-top";
                    ctl.ViewBag.mainLeftClass = "main-left";
                    ctl.ViewBag.userUnit = "";
                    ctl.ViewBag.cardClass = "card whiteBG mb-4 pattern-F";
                    ctl.ViewBag.footerClass = "inside_footer";
                    ctl.ViewBag.userIcon = "~/Content/images1/icon-001-manager.png";
                }
            }
            if (userInfo.IsDepartmentExec)
            {
                ctl.ViewBag.userRole = "20";
                ctl.ViewBag.nvaClass = "navbar navbar-expand-lg fixed-top navbar-bg-P";
                ctl.ViewBag.mainLeftClass = "main-left menu_bg_P";
                ctl.ViewBag.userUnit = userUnitName;
                ctl.ViewBag.cardClass = "card whiteBG mb-4 pattern-F colorset_P";
                ctl.ViewBag.footerClass = "inside_footer footer-P";
                ctl.ViewBag.userIcon = "~/Content/images1/icon-006-committee.png";
            }
            if( ConfigurationManager.AppSettings.Get("Developement")  != null)
            {
                ctl.ViewBag.nvaClass += "-developement";
            }
        }
        //標案權限管控 shioulo 20220816
        public static string getAuthoritySqlForTender(string alias, string engAlias)
        {
            string sql = " and 1=0";


            System.Web.SessionState.HttpSessionState _session = null;

            if (System.Web.HttpContext.Current != null)
            {
                _session = System.Web.HttpContext.Current.Session;
            }
            if (_session == null) return " and 1=1";
            if (  _session["UserInfo"] == null) return sql;

            UserInfo userInfo = (UserInfo)_session["UserInfo"];
            if (userInfo.UnitSeq1 == null && userInfo.UnitSeq2 == null) return sql;

            if (userInfo.IsAdmin) return " ";//系統管理者

            //
            List<Models.Role> roles = userInfo.Role;
            if (roles.Count == 0) return sql;

            int roleSeq = roles[0].Seq;

            if (roleSeq == ConfigManager.DepartmentAdmin_RoleSeq) //署管理者
            {
                sql = " ";
            }
            else if (roleSeq == ConfigManager.DepartmentUser_RoleSeq) //署使用者
            {
                if (userInfo.UnitName1 == "水利署")
                {
                    sql = " ";
                }
                else
                {
                    sql = String.Format(@" and (
                        {0}ExecUnitName like '{1}' or {0}PlanOrganizerName like '{1}' 
                        or (
                        {2}Seq in (select EngMainSeq from EngSupervisor where UserMainSeq={3})
                        )
                    ) ", alias, "%"+userInfo.UnitName1, engAlias, getUserSeq());
                }
            }
            else if (roleSeq == ConfigManager.BuildContractor_RoleSeq) //施工廠商
            {
                if (userInfo.UnitCode2 != null)
                {
                    sql = String.Format(@" and (
                        {0}BuildContractorTaxId='{1}'
                    ) ", engAlias, userInfo.UnitCode2);
                }
            }
            else if (roleSeq == ConfigManager.SupervisorUnit_RoleSeq) //監造單位
            {
                if (userInfo.UnitCode2 != null)
                {
                    sql = String.Format(@" and (
                        {0}SupervisorTaxid='{1}'
                    ) ", engAlias, userInfo.UnitCode2);
                }
            }
            else if (roleSeq == ConfigManager.OutsourceDesign_RoleSeq) //委外設計
            {
                if (userInfo.UnitCode2 != null)
                {
                    sql = String.Format(@" and (
                        {0}DesignUnitTaxId='{1}'
                    ) ", engAlias, userInfo.UnitCode2);
                }
            }

            return sql;
        }
        //標案權限管控 shioulo 20220816
        public static string getAuthoritySqlForTender1(string alias)
        {
            string sql = " and 1=0";
            System.Web.SessionState.HttpSessionState _session = System.Web.HttpContext.Current.Session;
            if (_session["UserInfo"] == null) return sql;

            UserInfo userInfo = (UserInfo)_session["UserInfo"];
            if (userInfo.UnitSeq1 == null && userInfo.UnitSeq2 == null) return sql;

            if (userInfo.IsAdmin) return " ";//系統管理者

            //
            List<Models.Role> roles = userInfo.Role;
            if (roles.Count == 0) return sql;

            int roleSeq = roles[0].Seq;

            if (roleSeq == ConfigManager.DepartmentAdmin_RoleSeq) //署管理者
            {
                sql = " ";
            }
            else if (roleSeq == ConfigManager.DepartmentUser_RoleSeq) //署使用者
            {
                if (userInfo.UnitName1 == "水利署")
                {
                    sql = " ";
                }
                else
                {
                    sql = String.Format(@" and (
                        {0}ExecUnitName like '{1}' or {0}PlanOrganizerName like '{1}' 
                    ) ", alias, "%" + userInfo.UnitName1);
                }
            }
            return sql;
        }

        public static string getExecUnitTenderCoditionSql(string alias, int codition)
        {
            switch (codition){
                //縣市政府
                case 0: 
                    return String.Format(@"substring({0}ExecUnitName,1,3) in (select Country from Country2WRAMapping)
                        ", alias); 
                //不
                case 1:
                    return String.Format(@"{0}ExecUnitName not in (select name from unit where ParentSeq is null)
                          and substring({0}ExecUnitName,1,3) not in (select Country from Country2WRAMapping)
                        ", alias);
                default: return "";
            }


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitName"> 要取得所有屬單位的單位</param>
        /// <param name="units"> 要搜尋的單位清單</param>
        /// <returns></returns>
        ///
        public static string getUnitBelonging(string unitName ,List<Unit> units)
        {
            string unitStr = "";
            int levelSeq = (units.FirstOrDefault(row => row.Name == unitName) ?? new Unit() ).Seq;
            unitStr += levelSeq + ",";
            Queue<Unit> queue = new Queue<Unit>();
            foreach(Unit unit in units)
            {
                if(unit.ParentSeq == levelSeq)
                {
                    queue.Enqueue(unit);
                    unitStr += unit.Seq +",";
                }
            }

            while(queue.Count > 0) 
            {
                levelSeq = queue.Dequeue().Seq;

                foreach (Unit unit in units)
                {
                    if(unit.ParentSeq == levelSeq)
                    {
                        unitStr += unit.Seq + ",";
                        queue.Enqueue(unit);
                    }
                }
            }
            return unitStr.Remove(unitStr.Length - 1 , 1);

        }

        public static IEnumerable<T> getEngMainForRole<T>(this IEnumerable<T> list
                      ) where T : EngMainRole
        {


            System.Web.SessionState.HttpSessionState _session = System.Web.HttpContext.Current.Session;

            int roleSeq; string roleSeqId; UserInfo userInfo = new UserInfo(); VUserMain user = new VUserMain();

            if (_session["UserInfo"] == null) return new List<T>();

            userInfo = (UserInfo)_session["UserInfo"];
            if (userInfo.UnitSeq1 == null && userInfo.UnitSeq2 == null) return new List<T>();


            List<Models.Role> roles = userInfo.Role;

            if (roles.Count == 0) return new List<T>();

            roleSeq = roles[0].Seq;

            roleSeqId = roles[0].Id;

            using(var context = new EQC.EDMXModel.EQC_NEW_Entities())
            {
                if (roleSeq == 1)
                {
                    return list;
                }

                if (roleSeq == ConfigManager.DepartmentAdmin_RoleSeq) //署管理者
                {

                    return list;
                }
                else if (roleSeq == ConfigManager.DepartmentUser_RoleSeq) //署使用者
                {
                    
                    return list.Except(
                        context.Country2WRAMapping
                            .Where(row => row.RiverBureau == userInfo.UnitName1 )
                            .ToList()
                            .Aggregate(list, (current , item ) =>
                                current
                                .Where(row => !row.execUnitName.Contains(item.Country) )
                            )
                    );

                }
            }



            /*else if (roleSeq == ConfigManager.Committee_RoleSeq) //委員
            {
                sql = " ";
            }*/

            return new List<T>();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"> EngMain 代號 </param>
        /// <returns></returns>
        /// 

        public static string getAuthoritySql(
            string alias,
            string userNo = null,
            string SupervisorAlias = null,
            bool subOrgMatch = true,
            List<int> skipRole = null )
        {
            skipRole = skipRole ?? new List<int>();

            string sql = " and 1=0 ";
            System.Web.SessionState.HttpSessionState _session = null;

            if(System.Web.HttpContext.Current != null)
                _session=
                System.Web.HttpContext.Current.Session ;

            int roleSeq; string roleSeqId; UserInfo userInfo = new UserInfo(); VUserMain user = new VUserMain();

            if(_session == null)
            {
                return " and 1=1";
            }
            else if (userNo == null)
            {
                userInfo = new SessionManager().GetUser();
                if (ConfigurationManager.AppSettings.Get("Debug") == null)
                {

                    if (_session["UserInfo"] == null) return sql;

                }

                if (userInfo.UnitSeq1 == null && userInfo.UnitSeq2 == null) return sql;

                if (userInfo.IsAdmin) return " and 1=1";//系統管理者

                List<Models.Role> roles = userInfo.Role;

                if (roles.Count == 0) return sql;

                roleSeq = roles[0].Seq;

                roleSeqId = roles[0].Id;

                if (skipRole.Exists(row => row == Int32.Parse(roleSeqId)))
                {
                    return sql;
                }
            }
            else
            {
                UserService userService = new UserService();

                userInfo = userService.GetUserByAccount(userNo).FirstOrDefault(row => true) ;
               
                if (userInfo is null) return sql;

                user  = userService.GetUserInfo(userInfo.Seq).FirstOrDefault(row => true);
                if (user is null) return sql;

                roleSeq = user.RoleSeq;

                roleSeqId = Convert.ToString (roleSeq);
               

            }


            if(roleSeq == 1)
            {
                sql = " and 1=1";
            }

            if (roleSeq == ConfigManager.DepartmentAdmin_RoleSeq) //署管理者
            {
                sql = " ";
                /*sql = String.Format(@" and (
                        {0}OrganizerUnitSeq={1}
                        or
                        {0}ExecUnitSeq={1}
                    ) ", alias, userInfo.UnitSeq1);*/
            } else if (roleSeq == ConfigManager.DepartmentUser_RoleSeq) //署使用者
            {
                if (userInfo.UnitSeq2 == null || !subOrgMatch)
                {
                    sql = String.Format(@" and (
                        {0}OrganizerUnitSeq={1}
                        or
                        {0}ExecUnitSeq={1}
                        or (
                         " +(SupervisorAlias ?? "{0}Seq" )+ @" in (select EngMainSeq from EngSupervisor where UserMainSeq={2})
                        )
                        111%99
                    ) ", alias, userInfo.UnitSeq1, userNo != null ? user.Seq : getUserSeq());
                }
                else
                {
                    sql = String.Format(@" and (
                        ( {0}OrganizerUnitSeq={1} and {0}OrganizerSubUnitSeq = {2} )
                        or
                        ( {0}ExecUnitSeq={1} ) 
                        or (
                        " + (SupervisorAlias ?? "{0}Seq") + @" in (select EngMainSeq from EngSupervisor where UserMainSeq={3})
                        )
                        111%99
                    ) ", alias, userInfo.UnitSeq1, userInfo.UnitSeq2, userNo != null ? user.Seq : getUserSeq());
                }
            } else if (roleSeq == ConfigManager.BuildContractor_RoleSeq) //施工廠商
            {
                if(userInfo.UnitCode2!=null)
                {
                    sql = String.Format(@" and (
                        {0}BuildContractorTaxId='{1}' 111%99
                    ) ", alias, userInfo.UnitCode2);
                }
            }
            else if (roleSeq == ConfigManager.SupervisorUnit_RoleSeq) //監造單位
            {
                if (userInfo.UnitCode2 != null)
                {
                    sql = String.Format(@" and (
                        {0}SupervisorTaxid='{1}' 111%99
                    ) ", alias, userInfo.UnitCode2);
                }
            }
            else if (roleSeq == ConfigManager.OutsourceDesign_RoleSeq) //委外設計
            {
                if (userInfo.UnitCode2 != null)
                {
                    sql = String.Format(@" and (
                        {0}DesignUnitTaxId='{1}' 111%99
                    ) ", alias, userInfo.UnitCode2);
                }
            }
            else if(roleSeqId == "20" ) //署內執行者
            {
                sql = String.Format(@" and (
                        " + (SupervisorAlias ?? "{0}Seq") + @" in (select EngMainSeq from EngSupervisor where UserMainSeq={1}) 
                        or " + alias + @"OrganizerUserSeq = {1} 111%99
                    ) ", alias, userNo != null ? user.Seq : getUserSeq()) ;

            }
            
            /*else if (roleSeq == ConfigManager.Committee_RoleSeq) //委員
            {
                sql = " ";
            }*/

            return sql.Replace("111%99", $" or ( {alias}EngNo Like '111%99' and {alias}ExecUnitSeq={userInfo.UnitSeq1})");
        }
        //取得使用者 單位/機關
        public static void GetUserUnit(ref string unitSeq, ref string unitSubSeq)
        {
            string unitName = "";
            string unitSubName = "";
            GetUserUnit(ref unitSeq, ref unitName, ref unitSubSeq, ref unitSubName);
            /*SessionManager sessionManager = new SessionManager();
            UserInfo user = sessionManager.GetUser();
            if (user.UnitSeq.HasValue)
            {
                int parentSeq = new UnitService().GetParentUnitSeq(user.UnitSeq.Value);
                if (parentSeq == -1)
                {
                    unitSeq = user.UnitSeq.ToString();
                    unitSubSeq = "-1";
                }
                else
                {
                    unitSeq = parentSeq.ToString();
                    unitSubSeq = user.UnitSeq.ToString();
                }
            }*/
        }
        //取得使用者 單位/機關
        public static void GetUserUnit(ref string unitSeq, ref string unitName, ref string unitSubSeq, ref string unitSubName)
        {
            SessionManager sessionManager = new SessionManager();
            UserInfo user = sessionManager.GetUser();
            if (user !=null)
            {
                List<VUserMain> users = new UserService().GetUserInfo(user.Seq);
                if(users.Count==1)
                {
                    unitSubSeq = "-1";
                    unitSubName = "";
                    if (users[0].UnitSeq1.HasValue)
                    {
                        unitSeq = users[0].UnitSeq1.ToString();
                        unitName = users[0].UnitName1;
                    }
                    if (users[0].UnitSeq2.HasValue)
                    {
                        unitSubSeq = users[0].UnitSeq2.ToString();
                        unitSubName = users[0].UnitName2;
                    }
                    unitSeq = users[0].UnitSeq1.ToString();
                }
            }
        }

        //取得使用者 單位/機關/職稱
        public static void GetUserUnitPosition(ref string unitSeq, ref string unitSubSeq, ref string positionSeq)
        {
            SessionManager sessionManager = new SessionManager();
            UserInfo user = sessionManager.GetUser();
            if (user != null)
            {
                List<VUserMain> users = new UserService().GetUserInfo(user.Seq);
                if (users.Count == 1)
                {
                    unitSubSeq = "-1";
                    if (users[0].UnitSeq1.HasValue)
                    {
                        unitSeq = users[0].UnitSeq1.ToString();
                    }
                    if (users[0].UnitSeq2.HasValue)
                    {
                        unitSubSeq = users[0].UnitSeq2.ToString();
                    }
                    if (users[0].PositionSeq.HasValue)
                    {
                        positionSeq = users[0].PositionSeq.ToString();
                    }
                    unitSeq = users[0].UnitSeq1.ToString();
                }
            }
        }

        //取得使用者 機關 20220726
        public static string GetUserUnitName(int userSeq = 0)
        {
            string unitName = "";
            SessionManager sessionManager = new SessionManager();
            UserInfo user = userSeq == 0 ? sessionManager.GetUser() : new UserInfo();
            if (user != null)
            {
                List<VUserMain> users = new UserService().GetUserInfo(userSeq == 0 ? user.Seq : userSeq );
                if (users.Count == 1)
                {
                    if (users[0].UnitSeq1.HasValue)
                    {
                        unitName = users[0].UnitName1;
                    }
                }
            }
            return unitName;
        }
        //發送 email
        public static bool Email(string toMailAddress, string subject, string mailBody, string token = null, string loginHost = null)
        {
            try
            {
                MailMessage message = new MailMessage();
                string loginUrl = loginHost ?? "(https://eqc.wra.gov.tw)";
                if (token != null) loginUrl += "?token=" + token;
                message.From = new MailAddress(ConfigManager.MailAdress);
                message.To.Add(new MailAddress(toMailAddress));
                message.Subject = subject;
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = $"工程品管數位化系統{loginUrl} <br>" + mailBody;
                message.BodyEncoding = System.Text.Encoding.UTF8;

                SmtpClient smtp = new SmtpClient();
                smtp.Port = ConfigManager.MailPort;
                smtp.Host = ConfigManager.SMTP; //for gmail host  
                //smtp.EnableSsl = true;
                smtp.EnableSsl = ConfigManager.EnableSsl;
                smtp.UseDefaultCredentials = ConfigManager.UseDefaultCredentials;
                smtp.Credentials = new NetworkCredential(ConfigManager.MailAdress, ConfigManager.MailPd);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                smtp = null;
                message.Dispose();
                return true;
            }
            catch (Exception e) {
                BaseService.log.Info("Send Email:"+ toMailAddress+" Err: " + e.Message);
                return false;
            }
        }

        //for NPOI
        public static void insertRow(XWPFTable table, int copyrowIndex, int newrowIndex)
        {
            // Add a new row at the specified position in the table
            XWPFTableRow targetRow = table.InsertNewTableRow(newrowIndex);
            // Get the object that needs to be copied
            XWPFTableRow copyRow = table.GetRow(copyrowIndex);
            //Copy row object
            targetRow.GetCTRow().trPr = copyRow.GetCTRow().trPr;
            //May need to copy the column of the row
            List<XWPFTableCell> copyCells = copyRow.GetTableCells();
            //Copy column object
            XWPFTableCell targetCell = null;
            for (int i = 0; i < copyCells.Count; i++)
            {
                XWPFTableCell copyCell = copyCells[i];
                targetCell = targetRow.AddNewTableCell();
                targetCell.GetCTTc().tcPr = copyCell.GetCTTc().tcPr;
                if (copyCell.Paragraphs != null && copyCell.Paragraphs.Count > 0)
                {
                    targetCell.Paragraphs[0].GetCTP().pPr = copyCell.Paragraphs[0].GetCTP().pPr;
                    if (copyCell.Paragraphs[0].Runs != null
                            && copyCell.Paragraphs[0].Runs.Count > 0)
                    {
                        XWPFRun cellR = targetCell.Paragraphs[0].CreateRun();
                        cellR.IsBold = copyCell.Paragraphs[0].Runs[0].IsBold;
                    }
                }
            }

        }

        public static void rowMergeStart(XWPFTableCell cell)
        {
            rowMergeStart(cell, null);
        }
        public static void rowMergeStart(XWPFTableCell cell, string colSpan)
        {
            CT_Tc cttc = cell.GetCTTc();
            CT_TcPr ctPrFirstofThird = cttc.AddNewTcPr();
            if (colSpan != null)
            {
                CT_DecimalNumber gridSpan = ctPrFirstofThird.AddNewGridspan();
                gridSpan.val = colSpan;
            }
            ctPrFirstofThird.AddNewVMerge().val = ST_Merge.restart;//開始合併
            ctPrFirstofThird.AddNewVAlign().val = ST_VerticalJc.center;//垂直置中
            cttc.GetPList()[0].AddNewPPr().AddNewJc().val = ST_Jc.center;
        }
        public static void rowMergeContinue(XWPFTableCell cell)
        {
            rowMergeContinue(cell, null);
        }
        public static void rowMergeContinue(XWPFTableCell cell, string colSpan)
        {
            CT_Tc cttc = cell.GetCTTc();
            CT_TcPr ctPr = cttc.AddNewTcPr();
            
            if (colSpan != null)
            {
                CT_DecimalNumber gridSpan = ctPr.AddNewGridspan();
                gridSpan.val = colSpan;
            }
            ctPr.AddNewVMerge().val = ST_Merge.@continue;//續合併

        }
        public static void cellMergeStart(XWPFTableCell cell)
        {
            CT_Tc cttc = cell.GetCTTc();
            CT_TcPr ctPrFirstofThird = cttc.AddNewTcPr();
            ctPrFirstofThird.AddNewHMerge().val = ST_Merge.restart;//開始合併
        }
        public static void cellMergeContinue(XWPFTableCell cell)
        {
            CT_Tc cttc = cell.GetCTTc();
            CT_TcPr ctPrfirstofRow = cttc.AddNewTcPr();
            ctPrfirstofRow.AddNewHMerge().val = ST_Merge.@continue;//續合併
        }
        public static XWPFParagraph setCellTextLeft(XWPFDocument doc, XWPFTable table)
        {
            CT_P para = new CT_P();
            XWPFParagraph pCell = new XWPFParagraph(para, table.Body);
            pCell.Alignment = ParagraphAlignment.LEFT;//文字靠右  
            pCell.VerticalAlignment = TextAlignment.CENTER;//水平置  

            /*XWPFRun r1c1 = pCell.CreateRun();
            r1c1.SetText(setText);
            r1c1.FontSize = 12;
            r1c1.FontFamily = "標楷體";
            //r1c1.SetTextPosition(20);//高度  */
            return pCell;
        }
        /// <summary>
        /// 取得當前 user info
        /// </summary>
        /// <returns></returns>
        public static UserInfo getUserInfo()
        {
            return new SessionManager().GetUser();
        }
        /// <summary>
        /// 取得當前 user Seq
        /// </summary>
        /// <returns></returns>
        public static int getUserSeq()
        {

            return new SessionManager().GetUser().Seq;
        }
        /// <summary>
        /// null 轉成 "NULL" 字串
        /// </summary>
        /// <param name="source"></param>
        public static object NulltoNullString(object value)
        {
            if (value == null)
                return "NULL";
            else
                return value;
        }

        /// <summary>
        /// null 轉成 DBNull
        /// </summary>
        /// <param name="source"></param>
        public static object NulltoDBNull(object value)
        {
            if (value == null)
                return DBNull.Value;
            else
                return value;
        }

        /// <summary>
        /// null 轉成 String.Empty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string NulltoEmpty(string value)
        {
            if (value==null)
                return string.Empty;
            else
                return value;
        }

        /// <summary>
        /// 將物件中字串屬性為null的值轉成string.Empty
        /// </summary>
        /// <param name="source"></param>
        public static void Null2Empty(object source)
        {
            //設定其他欄位為空字串
            foreach (PropertyInfo prop in source.GetType().GetProperties())
            {

                if (prop.PropertyType == typeof(string))
                {
                    string propValue = prop.GetValue(source, null) as string;
                    try
                    {
                        if (propValue == null)
                            prop.SetValue(source, string.Empty, null);
                    }
                    catch { }
                }
                if (prop.PropertyType == typeof(int))
                {
                    string propValue = prop.GetValue(source, null) as string;
                    try
                    {
                        if (propValue == null)
                            prop.SetValue(source, (object)DBNull.Value, null);
                    }
                    catch { }
                }
            }
        }
        //後台上傳模板檔案目錄
        public static string GetSignatureFile(string srcFile)
        {
            string webRootPath = WebRootPath ?? HttpContext.Current.Server.MapPath("~");
            if (srcFile.IndexOf("\\") == 0) srcFile = srcFile.Substring(1);
            //System.Diagnostics.Debug.WriteLine(Path.Combine(webRootPath, srcFile));
            //return webRootPath + srcFile;
            return Path.Combine(webRootPath, srcFile);
        }

        public static string WebRootPath;
        //套版檔案路徑
        public static string GetTemplateFilePath()
        {
            string folderName = "TemplateFile";
            string webRootPath = WebRootPath ?? HttpContext.Current.Server.MapPath("~");
            return Path.Combine(webRootPath, folderName);
        }
        //後台上傳模板檔案目錄
        public static string GetTemplateFolder()
        {
            string folderName = "FileUploads/Tp";
            string webRootPath = WebRootPath ?? HttpContext.Current.Server.MapPath("~");
            return Path.Combine(webRootPath, folderName);
        }
        //工程專案檔案目錄
        public static string GetEngMainFolder(int engMainSeq)
        {
            string folderName = String.Format("FileUploads/Eng/{0}", engMainSeq);
            string webRootPath = WebRootPath ?? HttpContext.Current.Server.MapPath("~");
            return Path.Combine(webRootPath, folderName);
        }
        //監造計畫書目錄
        public static string GetSupervisionPlanFolder(int engMainSeq)
        {
            string folderName = String.Format("FileUploads/Eng/{0}/SupervisionPlan", engMainSeq);
            string webRootPath = WebRootPath ?? HttpContext.Current.Server.MapPath("~");
            return Path.Combine(webRootPath, folderName);
        }

        //品質計畫書目錄
        public static string GetQualityPlanFolder(int engMainSeq)
        {
            string folderName = String.Format("FileUploads/Eng/{0}/QualityPlan", engMainSeq);
            string webRootPath = WebRootPath ?? HttpContext.Current.Server.MapPath("~");
            return Path.Combine(webRootPath, folderName);
        }
        //標案PrjXML檔案目錄
        public static string GetTenderFolder(int engMainSeq)
        {
            string folderName = String.Format("FileUploads/Tender/{0}", engMainSeq);
            string webRootPath = WebRootPath ?? HttpContext.Current.Server.MapPath("~");
            return Path.Combine(webRootPath, folderName);
        }
        /// <summary>
        /// 設計階段施工風險評估
        /// </summary>
        /// <param name="engMainSeq"></param>
        /// <returns></returns>
        public static string GetConstRiskEvalFolder(int engMainSeq)
        {
            string fPath = Path.Combine(WebRootPath ?? HttpContext.Current.Server.MapPath("~"), String.Format("FileUploads/ConstRiskEval/{0}", engMainSeq));
            if (!Directory.Exists(fPath)) Directory.CreateDirectory(fPath);
            return fPath;
        }
        /// <summary>
        /// 生態檢核(設計階段)上傳
        /// </summary>
        /// <param name="engMainSeq"></param>
        /// <returns></returns>
        public static string GetEcologicalCheckRemoteFolder(int engMainSeq, string folder= "FileUploads/EcologicalCheck")
        {
            string fPath;
            if (ConfigurationManager
                    .AppSettings["SelfEvalPath"]?.ToString() == null)
            {
                return GetEcologicalCheckFolder(engMainSeq, folder);
            }
            if (ConfigurationManager
                .AppSettings["SelfEvalHost"]?.ToString() == null)
            {
                fPath =
                    Path.Combine(
                        WebRootPath ?? HttpContext.Current.Server.MapPath("~")
                        ,
                        ConfigurationManager
                        .AppSettings["SelfEvalPath"].ToString()
                        ,
                        String.Format(folder + "/{0}", engMainSeq)
                    );
            }
            else
            {
                fPath =
                    Path.Combine(
                        ConfigurationManager
                        .AppSettings["SelfEvalPath"].ToString()
                        ,
                        String.Format(folder + "/{0}", engMainSeq)
                    );
            }


            if (!Directory.Exists(fPath)) Directory.CreateDirectory(fPath);
            return fPath;
        }
        public static string GetEcologicalCheckFolder(int engMainSeq, string folder)
        {
            string fPath = Path.Combine(WebRootPath ?? HttpContext.Current.Server.MapPath("~"), String.Format(folder+"/{0}", engMainSeq));
            //string fPath =
            //    Path.Combine(
            //        ConfigurationManager
            //        .AppSettings["SelfEvalPath"].ToString()
            //        , 
            //        String.Format("FileUploads/EcologicalCheck/{0}", engMainSeq)
            //    );

            if (!Directory.Exists(fPath)) Directory.CreateDirectory(fPath);
            return fPath;
        }
        /// <summary>
        /// 生態檢核(施工階段)上傳
        /// </summary>
        /// <param name="engMainSeq"></param>
        /// <returns></returns>
        public static string GetEcologicalCheck2Folder(int engMainSeq, string folder)
        {
            string fPath = Path.Combine(WebRootPath ??HttpContext.Current.Server.MapPath("~"), String.Format("FileUploads/EcologicalCheck2/{0}", engMainSeq));
            if (!Directory.Exists(fPath)) Directory.CreateDirectory(fPath);
            return fPath;
        }
        /// <summary>
        /// 工程提報
        /// </summary>
        /// <param name="engMainSeq"></param>
        /// <returns></returns>
        public static string GetEngReportFolder(int engReportSeq)
        {
            string fPath = Path.Combine(WebRootPath ?? HttpContext.Current.Server.MapPath("~"), String.Format("FileUploads/EngReport/{0}", engReportSeq));
            if (!Directory.Exists(fPath)) Directory.CreateDirectory(fPath);
            return fPath;
        }
        /// <summary>
        /// 施工風險
        /// </summary>
        /// <param name="engMainSeq"></param>
        /// <returns></returns>
        public static string GetEngRiskFrontFolder(int engSeq)
        {
            string fPath = Path.Combine(WebRootPath ?? HttpContext.Current.Server.MapPath("~"), String.Format("FileUploads/EngRiskFront/{0}", engSeq));
            if (!Directory.Exists(fPath)) Directory.CreateDirectory(fPath);
            return fPath;
        }
        /// <summary>
        /// 複製範本擋道暫存目錄
        /// </summary>
        /// <returns></returns>
        public static string CopyTemplateFile(string filename, string extFile)
        {
            string tempFile = Utils.GetTempFile(extFile);
            string srcFile = Path.Combine(Utils.GetTemplateFilePath(), filename);
            System.IO.File.Copy(srcFile, tempFile);
            return tempFile;
        }

        /// <summary>
        /// 建立使用者暫存目錄
        /// </summary>
        /// <returns></returns>
        public static string GetTempFolderForUser(string uuid ="", int? userSeq = null)
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), 
                "EQCUserTempFile",
                (userSeq  ?? Utils.getUserSeq() ).ToString(), uuid);
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            return tempFolder;
        }

        /// <summary>
        /// 建立暫存目錄
        /// </summary>
        /// <returns></returns>
        public static string GetTempFolder(string uuid = null)
        {
            string tempFolder = uuid ?? Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("B").ToUpper());
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            return tempFolder;
        }

        /// <summary>
        /// 建立暫存檔
        /// </summary>
        /// <param name="extFile"></param>
        /// <returns></returns>
        public static string GetTempFile(string extFile, string _uuid = null)
        {
            string uuid = _uuid ?? Guid.NewGuid().ToString("B").ToUpper();
            string tempPath = Path.GetTempPath();
            return Path.Combine(tempPath, uuid + extFile);
        }

        //轉為中曆時間
        /// <summary>
        /// 轉為中曆字串 {0}/{1}/{2}
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToChsDateTime(this DateTime? dt)
        {
            if (dt.HasValue)
            {
                DateTime tar = dt.Value;
                int year = tar.Year - 1911;
                return String.Format("{0}/{1}/{2} {3}:{4}:{5}", year, tar.Month, tar.Day, tar.Hour, tar.Minute, tar.Second);
            }
            else
            {
                return string.Empty;
            }
        }

        //轉為中曆字串
        /// <summary>
        /// 轉為中曆字串 {0}/{1}/{2}
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToChsDate(this DateTime? dt)
        {
            if (dt.HasValue)
            {
                DateTime tar = dt.Value;
                int year = tar.Year - 1911;
                return String.Format("{0}/{1}/{2}", year, tar.Month, tar.Day);
            }
            else
            {
                return string.Empty;
            }
        }
        //轉為中曆字串
        /// <summary>
        /// 轉為中曆字串 {0}/{1}/{2}
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ChsDate(DateTime? dt)
        {
            if (dt.HasValue)
            {
                DateTime tar = dt.Value;
                int year = tar.Year - 1911;
                return String.Format("{0}/{1}/{2}", year, tar.Month, tar.Day);
            }
            else
            {
                return string.Empty;
            }
        }

        //中曆字串格式化為DateTime
        /// <summary>
        /// yyyMMdd 轉 DateTime
        /// </summary>
        /// <param name="dateStr">yyyMMdd</param>
        /// <returns>yyy/MM/dd</returns>
        public static DateTime? ChsDateStrToDate(string dateStr)
        {
            if (String.IsNullOrEmpty(dateStr) || dateStr.Length != 7)
            {
                return null;
            }
            else
            {
                return new DateTime(1911 + Int32.Parse(dateStr.Substring(0, 3)), Int32.Parse(dateStr.Substring(3, 2)), Int32.Parse(dateStr.Substring(5, 2)));

            }
        }
        //中曆字串格式化 s20230519
        /// <summary>
        /// yyyMMdd 轉 yyy/MM/dd
        /// </summary>
        /// <param name="dateStr">yyyMMdd</param>
        /// <returns>yyy/MM/dd</returns>
        public static string ChsDateFormat(string dateStr)
        {
            if (String.IsNullOrEmpty(dateStr) || dateStr.Length != 7)
            {
                return "";
            }
            else
            {
                return String.Format("{0}/{1}/{2}", dateStr.Substring(0,3), dateStr.Substring(3, 2), dateStr.Substring(5, 2));
            }
        }
        public static string GetDayOfWeek(DateTime? dt)
        {
            if (dt.HasValue)
            {
                DateTime tar = dt.Value;
                switch( tar.DayOfWeek)
                {
                    case DayOfWeek.Sunday: return "星期天";break;
                    case DayOfWeek.Monday : return "星期一"; break;
                    case DayOfWeek.Tuesday : return "星期二"; break;
                    case DayOfWeek.Wednesday : return "星期三"; break;
                    case DayOfWeek.Thursday : return "星期四"; break;
                    case DayOfWeek.Friday : return "星期五"; break;
                    case DayOfWeek.Saturday : return "星期六"; break;
                    default: return "";
                }

            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 轉為中曆字串 {0}年{1}月{2}日
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string Chs1Date(DateTime? dt)
        {
            if (dt.HasValue)
            {
                DateTime tar = dt.Value;
                int year = tar.Year - 1911;
                return String.Format("{0}年{1}月{2}日", year, tar.Month, tar.Day);
            }
            else
            {
                return string.Empty;
            }
        }

        //轉為中曆字串
        /// <summary>
        /// 轉為中曆字串 {0}/{1}/{2}
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ChsDateTime(DateTime? dt)
        {
            if (dt.HasValue)
            {
                DateTime tar = dt.Value;
                int year = tar.Year - 1911;
                return $"{year}/{tar.Month}/{tar.Day} {tar.Hour}:{tar.Minute}";
            }
            else
            {
                return string.Empty;
            }
        }

        //轉為西元字串
        public static string EngDateTime(DateTime? dt)
        {
            if (dt.HasValue)
            {
                DateTime tar = dt.Value;
                return String.Format("{0}{1:00}{2:00} {3:00}:{4:00}", tar.Year, tar.Month, tar.Day, tar.Hour, tar.Minute);
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 中曆字串 yyy/m/d 轉DateTime
        /// </summary>
        public static DateTime? StringChsDateToDateTime(string date)
        {
            if (String.IsNullOrEmpty(date))
            {
                return null;
            } else {
                string[] subs = date.Split('/');
                if (subs.Length!=3)
                {
                    return null;
                } else
                {
                    int year, month, day;
                    if( int.TryParse(subs[0], out year) && int.TryParse(subs[1], out month) && int.TryParse(subs[2], out day) )
                    {
                        try
                        {
                            return new DateTime(year + 1911, month, day);
                        }catch(Exception e)
                        {
                            return null;
                        }
                    } else
                    {
                        return null;
                    }
                }
            }
        }
        /// <summary>
        /// 中曆字串 yyyMMdd  轉DateTime
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime? StringChs2DateToDateTime(string date)
        {
            if (!String.IsNullOrEmpty(date) && date.Length != 9)
            {
                
                int year, month, day;
                if (int.TryParse(date.Substring(0,3), out year) && int.TryParse(date.Substring(3, 2), out month) && int.TryParse(date.Substring(5, 2), out day))
                {
                    try
                    {
                        return new DateTime(year + 1911, month, day);
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
            }
            return null;
        }
        //OpenXML Cell text
        public static string oxCellStr(Cell cell, SharedStringTable strTable)
        {
            if (cell.ChildElements.Count == 0) return null;
            string val = cell.CellValue.InnerText;
            if (cell.DataType != null && cell.DataType == CellValues.SharedString)
                val = strTable.ChildElements[int.Parse(val)].InnerText;

            return val;
        }

        //機關名稱過濾 20220725
        public static string filterUnitName(string unitName)
        {
            if (String.IsNullOrEmpty(unitName)) return unitName;

            unitName = unitName.Trim();
            unitName = unitName.Replace("台", "臺"); //shioulo 20220730
            if (unitName.Length > 3 && unitName.Length <= 6) return unitName.Replace("經濟部", "");
            else if (unitName.Length > 6) return unitName.Replace("經濟部水利署", "");
            else return unitName;
        }

        public static List<string> getCarbonUnit()
        {
            return (new string[]{
                "M",
                "M2",
                "M3",
                "式",
                "T",
                "只",
                "個",
                "組",
                "KG"
            }).ToList();
        }
        /// <summary>
        /// Excel index欄位轉名稱欄位代碼
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        public static string GetExcelColumnName(int columnNumber)
        {
            string columnName = "";

            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }

            return columnName;
        }
        /// <summary>
        /// 調整圖片尺寸
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <param name="stPhotoPath"></param>
        /// <returns></returns>
        public static Image ResizeImage(int newWidth, int newHeight, string stPhotoPath)
        {
            Image imgPhoto = Image.FromFile(stPhotoPath);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            //Consider vertical pics
            if (sourceWidth < sourceHeight)
            {
                int buff = newWidth;

                newWidth = newHeight;
                newHeight = buff;
            }

            int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
            float nPercent = 0, nPercentW = 0, nPercentH = 0;

            nPercentW = ((float)newWidth / (float)sourceWidth);
            nPercentH = ((float)newHeight / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((newWidth - (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((newHeight - (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);


            Bitmap bmPhoto = new Bitmap(newWidth, newHeight,
                          System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                         imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(System.Drawing.Color.Black);
            grPhoto.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new System.Drawing.Rectangle(destX, destY, destWidth, destHeight),
                new System.Drawing.Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            imgPhoto.Dispose();
            return bmPhoto;
        }


        
    }
}