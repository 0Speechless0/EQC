using EQC.Common;
using EQC.EDMXModel;
using EQC.Models;
using EQC.ViewModel;
using EQC.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;

namespace EQC.Services
{
    public class TreePlantService : BaseService
    {

        Dictionary<int, TreeList> treeDic;
        Dictionary<string, int> treeRefCodeDic;
        EQC_NEW_Entities dbContext;

        ~TreePlantService()
        {
            dbContext.Dispose();
        }
        public TreePlantService(int eng = 0)
        {
            dbContext = new EQC_NEW_Entities();

            treeDic = dbContext.TreeList.ToDictionary(row => row.Seq, row => row);
            treeRefCodeDic = treeDic
                .ToDictionary(row => $"{row.Value.Type}{row.Value.Code}", row => row.Value.Seq);
            treeDic[0] = new TreeList();

            if (eng > 0)
            {
                payItemTemp = getPayItem(eng);
                payItemSchTemp = getSchPayItem(eng);
                payItemOverviewTemp = getPayItemOverview(eng);
                SchProgressDicTemp = getSchProgress(eng, payItemTemp);
            }
        }
        public List<EngMainTreeVModel> GetTenderListByUser(int createType, int Seq = 0)
        {
            string sql = "";
            if (createType == 1)
            {
                sql = @"
                select a.*  from (
                    SELECT 
                        a.Seq,
                        a.EngNo ,
                        a.EngName ,
                        a.EngYear, 
                        a.BuildContractorName,
                        a.BuildContractorTaxId,
                        a.ExecUnitSeq execUnitSeq,
                        a.ModifyTime,
                        p.ActualBidAwardDate ActualBidAwardDate,
                        (select Name from Unit where Unit.Seq = a.ExecUnitSeq) execUnitName,
                        c.Seq TreePlantSeq
                    FROM  EngMain a
                    inner join PrjXML p on p.Seq = a.PrjXMLSeq
                    left join TreePlantMain c
                    on (
                        c.EngSeq = a.Seq 
                    )

				    where  ( ( @Seq = 0 and c.Seq is null ) or (@Seq = a.Seq) )
                 " + Utils.getAuthoritySql("a.", null, null, false, new List<int>() { 6, 7 }) + @"
                ) a

				order by a.ModifyTime desc
            ";
            }
            else if (createType == 2)
            {

                sql = @"
        select * from ( 
		    SELECT 
                    a.Seq,
                    a.TenderNo EngNo ,
                    a.TenderName EngName ,
                    a.TenderYear EngYear,
                    a.ContractorName1 BuildContractorName,
                    a.ExecUnitName execUnitName,
					ISNULL(c1.RiverBureau, a.ExecUnitName) RiverBureau,
                    cc.Seq ExecUnitSeq,
                    cc.Seq OrganizerUnitSeq,
                    a.ActualBidAwardDate ActualBidAwardDate,
				    pr.PDExecState,
                    [PDExecStateRow] = ROW_NUMBER() over(partition by a.Seq order by pr.CreateTime desc ) ,
					a.ModifyTime,
                    c.Seq TreePlantSeq
                FROM  PrjXML a

                left join EngMain b on a.Seq = b.PrjXMLSeq
 
                inner join ProgressData pr on pr.PrjXMLSeq = a.Seq
				left outer join Unit c2 on(c2.ParentSeq is null and c2.Name=a.ExecUnitName)
				left outer join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
				left outer join Unit cc on cc.Name = ISNULL(c1.RiverBureau, a.ExecUnitName)
                left join TreePlantMain c
				 on (
                    c.EngSeq = a.Seq 
                )

			where 
					b.Seq is null
					and cc.Seq is not null
 					and ( ( c.Seq is null and (@Seq= 0 ) and b.PrjXMLSeq is null ) or (a.Seq = @Seq ) )

 
			)　d　
            where  
			d.PDExecStateRow = 1
            and d.PDExecState != '已結案'
            " + Utils.getAuthoritySql("d.", null, "0", false, new List<int>() { 6, 7 }) + @"
            order by d.ModifyTime　desc; 

            ";
            }



            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", Seq);
            return db.GetDataTableWithClass<EngMainTreeVModel>(cmd);
        }

        public TreeEngMainEditVModel GetTender(int engSeq, int createType)
        {

            string sql = "";
            if (createType == 1)
            {
                sql = @"
                SELECT
                    a.Seq,
                    a.EngYear,
                    a.EngNo,
                    a.EngName,
                    (select  cc.CityName + c.TownName from Town c inner join City cc on cc.Seq = c.CitySeq 
						where  c.Seq = a.EngTownSeq) engTownName,

                    (select c.Name from Unit c where  c.Seq = a.OrganizerUnitSeq ) organizerUnitName,
					(select c.Name from Unit c where  c.Seq = a.ExecUnitSeq ) execUnitName,
					(select c.Name from Unit c where  c.Seq = a.ExecSubUnitSeq ) execSubUnitName,
					(select c.DisplayName from UserMain c where  c.Seq = a.OrganizerUserSeq ) organizerUserName,
                    a.TotalBudget,
                    a.ExecUnitSeq,
                    a.ExecSubUnitSeq,
                    a.SubContractingBudget,
                    a.PurchaseAmount
                FROM EngMain a
                where a.Seq = @engSeq";
            }
            else if (createType == 2)
            {
                sql = @"
                  SELECT
                    a.Seq,
                    a.TenderNo EngNo,
                    a.TenderName EngName,
                    a.TenderYear EngYear,
                    a.TownName engTownName,
                    a.OrganizerName organizerUnitName,
					ISNULL(ct.RiverBureau, a.ExecUnitName) execUnitName,
                    u.Seq ExecUnitSeq,
                    CAST(a.TotalEngBudget*1000 as decimal) TotalBudget,
                    CAST(a.OutsourcingBudget*1000  as decimal) SubContractingBudget
                FROM PrjXML a
                left join Country2WRAMapping ct on (Replace(a.TownName, '台','臺')　Like '%'+ substring(ct.Country, 1, 2) +'%')
                left join Unit u on (u.Name = ct.RiverBureau　and u.ParentSeq is null)
                where a.Seq = @engSeq";
            }
            else
            {
                return null;
            }

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@engSeq", engSeq);
            return db.GetDataTableWithClass<TreeEngMainEditVModel>(cmd).FirstOrDefault();
        }

        public List<SelectVM> TPEngTypeOption(List<TreePlantMainPrj> list)
        {
            using (var context = new EQC_NEW_Entities())
            {
                var TPEngTypeNameDic = context
                    .TreePlantEngType
                    .ToDictionary(row => row.Seq, row => row.Name);
                TPEngTypeNameDic[0] = "";

                return list
                    .Where(row => row.TPEngTypeSeq != null)
                    .GroupBy(row => row.TPEngTypeSeq)
                    .ToList()
                    .Select(g => new SelectVM
                    {
                        Text = TPEngTypeNameDic[g.Key ?? 0],
                        Value = g.Key.ToString()
                    }).OrderBy(row => row.Text).ToList();
            }
        }

        public EngMainTreeVModel GetSelectTender(int engSeq, int createType)
        {
            var user = new SessionManager().GetUser();
            int unitSeq = (int)user.UnitSeq1;
            string sql = "";
            /*(int)(user.UnitSeq3 ?? (user.UnitSeq2 ?? user.UnitSeq1));*/

            if (createType == 1)
            {
                sql = @"
                SELECT 
                    a.Seq,
                    a.EngNo ,
                    a.EngName ,
                    a.BuildContractorName ,
                    a.BuildContractorTaxId,
                    c.Seq TreePlantSeq
                FROM  EngMain a
                left join TreePlantMain c
                on (
                    c.EngSeq = a.Seq 
                )
				where  a.Seq = @Seq
				order by a.ModifyTime desc
            ";
            }
            else if (createType == 2)
            {
                sql = @"
                SELECT
                    a.TenderNo EngNo,
                    a.TenderName EngName,
                    a.TownName engTownName,
                    a.OrganizerName organizerUserName,
					a.ExecUnitName execUnitName,
                    a.TotalEngBudget TotalBudget,
                    a.OutsourcingBudget SubContractingBudget
                FROM PrjXML a
                where a.Seq = @engSeq";
            }

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", engSeq);
            return db.GetDataTableWithClass<EngMainTreeVModel>(cmd).FirstOrDefault();
        }

        public void removeTreePlantMonthsByEng(int engSeq)
        {
            using (var context = new EQC_NEW_Entities())
            {
                context.TreePlantMonth.RemoveRange(
                    context.TreePlantMonth.Where(row => row.TreePlantMain.EngSeq == engSeq)
                    );
                context.SaveChanges();
            }
        }

        public void deleteTreeMain(int id)
        {
            using (var context = new EQC_NEW_Entities())
            {
                context.TreePlantNumList.RemoveRange(
                context.TreePlantNumList.Where(row => row.TreePlantSeq == id));
                context.TreePlantMonth.RemoveRange(
                context.TreePlantMonth.Where(row => row.TreePlantSeq == id));
                context.Entry(context.TreePlantMain.Find(id)).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }

        Func<string, bool> treeCheck =
            (row) => row != null && (row.StartsWith("02931") ||
            row.StartsWith("02932"));



        List<SchProgressPayItem> payItemSchTemp;
        /// <summary>
        /// 從pcces取得標案計價項目最新預定進度資料查詢
        /// </summary>
        public List<SchProgressPayItem> getSchPayItem(int engSeq)
        {

            return payItemSchTemp ?? dbContext.SchProgressPayItem
                    .Where(row => row.SchProgressHeader.EngMainSeq == engSeq && row.SchProgress != -1)

                    .Where(
                                (row) => row.SchEngProgressPayItem.RefItemCode != null && (row.SchEngProgressPayItem.RefItemCode.StartsWith("02931") ||
                            row.SchEngProgressPayItem.RefItemCode.StartsWith("02932"))
                    )
                    .Include(row => row.SchEngProgressPayItem)
                    .OrderBy(row => row.SchEngProgressPayItem.PayItem)
                    .GroupBy(row => row.SchEngProgressPayItem.PayItem)
                    .ToList()
                    .Select(row => row.OrderBy(r => r.SPDate).Last())
                    .ToList() ?? new List<SchProgressPayItem>();

        }

        List<SchEngProgressPayItem> payItemTemp;

        /// <summary>
        /// 從pcces取得標案決標後屬於樹的計價項目查詢
        /// </summary>
        public List<SchEngProgressPayItem> getPayItem(int engSeq)
        {
            return payItemTemp ?? dbContext.SchEngProgressPayItem
               .Where(row => row.SchEngProgressHeader.EngMainSeq == engSeq)
               .Where(
                           (row) => row.RefItemCode != null && (row.RefItemCode.StartsWith("02931") ||
                       row.RefItemCode.StartsWith("02932"))
               )
               .OrderBy(row => row.PayItem)
               .ToList();

        }


        List<SupPlanOverview> payItemOverviewTemp;
        /// <summary>
        /// 取得標案屬於樹的監造填報資料查詢
        /// </summary>
        public List<SupPlanOverview> getPayItemOverview(int engSeq)
        {
            return payItemOverviewTemp ?? dbContext.SupPlanOverview

                    .Where(row => row.SupDailyDate.EngMainSeq == engSeq)
                    .Where(
                                (row) => row.SchEngProgressPayItem.RefItemCode != null && (row.SchEngProgressPayItem.RefItemCode.StartsWith("02931") ||
                            row.SchEngProgressPayItem.RefItemCode.StartsWith("02932"))
                    )
                    .OrderBy(row => row.SchEngProgressPayItem.PayItem)
                    .Include(row => row.SupDailyDate)
                    .Include(row => row.SchEngProgressPayItem)
                    .ToList();

        }
        public List<TreePlantMainVM> getAllTreePlantMain()
        {

            return dbContext.TreePlantMain
                .ToList()
                .Select(row => new TreePlantMainVM().SetTreeMainSourceEntity(row, dbContext) )
                .ToList();
        }

        /// <summary>
        ///從pcces取得標案當前已完成實際數量
        /// </summary>
        public List<int> getTreePlantXMLMonthsAclNum(int engSeq, int count)
        {

            int[] result = new int[count];

            var payItemOverviewTemp = getPayItemOverview(engSeq);
            payItemOverviewTemp
                .Where(row => row.SupDailyDate.ItemDate <= DateTime.Now)
                .GroupBy(row => row.SchEngProgressPayItem.PayItem)
                .OrderBy(row => row.Key)
                .Select(row => row.Sum(r => (int)r.TodayConfirm))
                .ToArray()
                .CopyTo(result, 0);
            return new List<int>(result);




        }

        /// <summary>
        ///從pcces取得標案每月已完成實際數量字串
        /// </summary>
        public List<string> getTreePlantXMLMonthsAcl(int engSeq, List<SchEngProgressPayItem> list, int itemCount)
        {
            string[] result = new string[itemCount];
            var SchDateProgressDic = getSchProgress(engSeq, list);

            var AclDateNumDic = SchDateProgressDic
                    .ToDictionary(row => $"{row.Key}", row => new int[row.Value.Count]);


            Dictionary<string, int[]> accList = getAclNumProgress(engSeq);

            foreach (var dateAcc in accList)
            {
                var dateAccKey = dateAcc.Key;
                if (AclDateNumDic.ContainsKey(dateAccKey)) accList[dateAcc.Key].CopyTo(AclDateNumDic[dateAccKey], 0);
                else
                {
                    AclDateNumDic.Add(dateAccKey, new int[itemCount]);
                    accList[dateAcc.Key].CopyTo(AclDateNumDic[dateAccKey], 0);
                }
            }


            List<string> resultList = new List<string>();

            int acc = 0;
            for (int i = 0; i < itemCount; i++)
            {
                //acc = 0;
                resultList.Add(
                    AclDateNumDic.Aggregate("", (last, current) =>
                    {
                        //if (current.Value[i] == 0) return last;
                        //acc += current.Value[i];
                        return $"{last}，{current.Key} : {current.Value[i]}";
                    })

                );
            }
            resultList.CopyTo(result);

            return new List<string>(result);


        }

        /// <summary>
        ///從pccess取得標案當前預定數量資料
        /// </summary>
        public List<int> getTreePlantXMLMonthsSchNum(int engSeq, int count)
        {
            using (var context = new EQC_NEW_Entities())
            {
                var list = getSchPayItem(engSeq);


                int[] result = new int[count];

                list
                     .Select(row => (int)(row.SchEngProgressPayItem.Quantity * row.SchProgress / 100))
                     .ToArray().CopyTo(result, 0);
                return new List<int>(result);
            }

        }

        /// <summary>
        ///取得標案pccess各個月份預定數量進度字串
        /// </summary>
        public List<string> getTreePlantXMLMonthsSch(int engSeq, List<SchEngProgressPayItem> list, int count)
        {

            List<string> resultList = new List<string>();
            Dictionary<string, Dictionary<string, int>> schProgress =
                getSchProgress(engSeq, list);

            string[] result = new string[count];
            foreach (var item in list)
            {
                StringBuilder value2 = new StringBuilder();
                schProgress.Aggregate(
                    list.ToDictionary(row => row.PayItem, row => 0), (lastProgress, progress) =>
                    {
                        //if (progress.Value[item.PayItem] * item.Quantity == 0) return value;

                        value2.Append($"，{progress.Key}: {(int)((progress.Value[item.PayItem] - lastProgress[item.PayItem]) * item.Quantity) / 100 }");
                        return progress.Value;
                    });
                resultList.Add(value2.ToString());
            }
            resultList.CopyTo(result);
            return new List<string>(result);

        }


        /// <summary>
        ///取得標案pccess每月實際已完成數量
        /// </summary>
        public Dictionary<string, int[]> getAclNumProgress(int engSeq)
        {

            var payItemOverviewTemp = getPayItemOverview(engSeq);
            return payItemOverviewTemp

                    .ToList()
                    .OrderBy(row => row.SupDailyDate.ItemDate)

                    .GroupBy(row => $"{row.SupDailyDate.ItemDate.Year - 1911}/{row.SupDailyDate.ItemDate.Month}")
                    .ToDictionary(row => row.Key,
                        GroupRow =>


                            GroupRow

                                .GroupBy(row => row.SchEngProgressPayItem.PayItem)
                                .OrderBy(row => row.Key)
                                .Select(row => row.Sum(r => (int)r.TodayConfirm))
                            .ToArray()
                    );
        }

        Dictionary<string, Dictionary<string, int>> SchProgressDicTemp;

        /// <summary>
        ///取得標案pccess每月預定完成度
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> getSchProgress(int engSeq, List<SchEngProgressPayItem> list)
        {
            if (SchProgressDicTemp != null) return SchProgressDicTemp;

            using (var context = new EQC_NEW_Entities())
            {

                var schProgress = context.SchProgressPayItem
                 .Where(row => row.SchProgressHeader.EngMainSeq == engSeq)
                 .Where(row =>
                        row.SPDate != null &&
                        row.SchEngProgressPayItem.RefItemCode != null && (row.SchEngProgressPayItem.RefItemCode.StartsWith("02931") ||
                        row.SchEngProgressPayItem.RefItemCode.StartsWith("02932") && row.SchProgress != -1))
                 .ToList()
                 .GroupBy(row => row.SPDate?.ToString("yyyy/MM"))
                 .OrderBy(row => row.Key)
                 .ToDictionary(row => row.LastOrDefault()?.SPDate,
                     dateGroup => dateGroup
                        .OrderBy(row => row.SchEngProgressPayItem.PayItem)
                         .GroupBy(row => row.SchEngProgressPayItem.PayItem)
                         .ToDictionary(row => row.Key, row => (int)row.LastOrDefault()?.SchProgress)
                 );
                if (schProgress == null) return new Dictionary<string, Dictionary<string, int>>();
                var schProgressSet = schProgress.ToDictionary(row => row.Key?.ToString("yyyy/MM"), row => row.Value);
                DateTime? startSchDateTime = schProgress.Select(r => r.Key).FirstOrDefault();
                DateTime? endSchDateTime = schProgress.Select(r => r.Key).LastOrDefault();
                int MonthDiff = (((endSchDateTime?.Year - startSchDateTime?.Year) * 12) + endSchDateTime?.Month - startSchDateTime?.Month) ?? 0;
                var schProgressDateStrArr = schProgress.Select(row => row.Key?.ToString("yyyy/MM")).ToHashSet();
                for (int i = 1; i < MonthDiff; i++)
                {
                    var current = startSchDateTime?.AddMonths(i);
                    var last = startSchDateTime?.AddMonths(i - 1);
                    var dateStr = current?.ToString("yyyy/MM");
                    var lastDateStr = last?.ToString("yyyy/MM");
                    if (
                        dateStr != null &&
                        !schProgressDateStrArr.Contains(dateStr))
                    {
                        schProgressSet.Add(dateStr,
                            list.ToDictionary(row => row.PayItem, row => schProgressSet[lastDateStr][row.PayItem]));
                    }
                }
                return schProgressSet.OrderBy(row => row.Key).ToDictionary(
                    row =>
                    {
                        DateTime date = DateTime.Parse(row.Key);
                        return $"{date.Year - 1911}/{date.Month}";

                    },

                    row => row.Value);
            }
        }


        /// <summary>
        ///取得標案pccess某年度每月預定數量
        /// </summary>
        public int[] getAclNumProgressMonthSumFromPcces(int engSeq, int year)
        {
            int[] result = new int[12];
            var progress = getAclNumProgress(engSeq)
                .Where(row => Int32.Parse(row.Key.Split('/')[0]) == year);
            progress
                .Select(row => row.Value.Sum())
                .ToArray().CopyTo(result, 0);
            return result;
        }

        public List<TreeList> getTreeList()
        {
            UserInfo user = new SessionManager().GetUser();
            using (var context = new EQC_NEW_Entities())
            {
                return context.TreeList.ToList();

            }
        }

        public void updateTreePlantNumList(List<TreePlantNumList> list, int engSeq = 0, int treePlantSeq = 0)
        {
            using (var context = new EQC_NEW_Entities())
            {
                int i = 0;
                treePlantSeq = treePlantSeq == 0 ?
                    context.EngMain
                    .Where(row => row.Seq == engSeq)
                    .FirstOrDefault()?
                    .TreePlantMain
                    .FirstOrDefault()?.Seq ?? 0 : treePlantSeq;

                List<TreePlantNumList> origin = context.TreePlantNumList
                    .Where(row => row.TreePlantSeq == treePlantSeq)
                    .OrderBy(row => row.TreeTypeSeq)
                    .ToList();

                List<TreePlantNumList>
                    insertList = list.Count > origin.Count ?
                        list.GetRange(origin.Count, list.Count - origin.Count) : new List<TreePlantNumList>();
                List<TreePlantNumList>
                    updateList = list
                    .GetRange(0, origin.Count)
                    .OrderBy(row => row.TreeTypeSeq).ToList();


                int userSeq = new SessionManager().GetUser().Seq;

                insertTreePlantNumList(insertList, engSeq, treePlantSeq);
                foreach (var item in updateList)
                {
                    item.Seq = origin[i].Seq;
                    item.CreateTime = origin[i].CreateTime;
                    item.ModifyTime = DateTime.Now;
                    item.ModifyUserSeq = userSeq;
                    item.Seq = item.Seq;
                    context.Entry(origin[i])
                        .CurrentValues.SetValues(item);
                    i++;
                }


                context.SaveChanges();
            }
        }

        public void deleteTreePlantNumList(int treeNumSeq)
        {
            using (var context = new EQC_NEW_Entities())
            {
                context.Entry(context.TreePlantNumList.Find(treeNumSeq))
                    .State = EntityState.Deleted;
                context.SaveChanges();

            }
        }
        public void insertTreePlantNumList(List<TreePlantNumList> list, int engSeq = 0, int treePlantSeq = 0)
        {
            using (var context = new EQC_NEW_Entities())
            {
                treePlantSeq =
                    treePlantSeq == 0 ? context.EngMain
                        .Where(row => row.Seq == engSeq)
                        .FirstOrDefault()?
                        .TreePlantMain
                        .FirstOrDefault()?
                        .Seq ?? 0 : treePlantSeq;

                foreach (var item in list)
                {
                    item.CreateTime = DateTime.Now;
                    item.ModifyTime = DateTime.Now;
                    item.CreateUserSeq = new SessionManager().GetUser().Seq;
                    item.ModifyUserSeq = new SessionManager().GetUser().Seq;
                    item.TreePlantSeq = treePlantSeq;

                    context.TreePlantNumList.Add(item);
                }

                context.SaveChanges();

            }
        }

        public TreePlantMainPrj getTreePlantMain(int createType, int treePlantMainEngSeq, int treeMainSeq = 0)
        {
            SqlCommand cmd;
            string sql = "";
            if (treeMainSeq == 0)
            {
                if (createType == 2)
                {
                    sql = @"
                select 
                e0.Seq treeMainPrjSeq, 
                e2.*
                from
                PrjXML e0
                inner join TreePlantMain e2 on e2.EngSeq = e0.Seq
                where e0.Seq = @treeMainPrjSeq
                
            ";
                }
                else if (createType == 1)
                {
                    sql = @"
                        select 
                        e2.*
                        from
                        EngMain e0
                        inner join TreePlantMain e2 on e2.EngSeq = e0.Seq
                        where e0.Seq = @treeMainPrjSeq
                
                    ";
                }

                cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@treeMainPrjSeq", treePlantMainEngSeq);
            }
            else
            {
                sql = @"
                    select 
                    e2.*
                    from
                    TreePlantMain e2
                    where e2.Seq = @treeMainSeq
                
                ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@treeMainSeq", treeMainSeq);
            }

            var target = db.GetDataTableWithClass<TreePlantMainPrj>(cmd).FirstOrDefault();
            using (var context = new EQC_NEW_Entities())
            {
               
                var targetVM = new TreePlantMainVM().SetTreeMainSourceEntity(target, context);
                target.Contractor= targetVM.ContractorNameResult;
                target.ContractorUniformNumber = targetVM.ContractorUniformNumberResult;
                target.ContractDate = targetVM.ContractDateResult;
            }

            return target;
         
        }
        public List<TreePlantMainPrj> getTreePlantMainList()
        {
            var user = new SessionManager().GetUser();
            string sql = @"
                select 
                e1.EngNo,
                (select top 1 Name from TreePlantEngType c where c.Seq = e2.TPEngTypeSeq ) TPEngTypeName,
                ( select  top 1　Name from Unit c where c.Seq = e2.ExecUnitSeq) execUnitName,
                ( select  top 1　Name from Unit c where c.Seq = e2.ExecSubUnitSeq) execSubUnitName,
                e2.*
                from
                TreePlantMain e2
                inner join EngMain e1 on (e2.EngSeq = e1.Seq and e2.EngCreatType = 1)

                where 1=1

            " + Utils.getAuthoritySql("e2.", null, "e2.EngSeq", false, new List<int>() { 6, 7 })
            + @" or (e2.EngSeq is null and e2.OrganizerUserSeq = @userSeq ) 
                
            ";

            string sql2 = @"
                select 
                (select top 1 Name from TreePlantEngType c where c.Seq = e2.TPEngTypeSeq ) TPEngTypeName,
                ( select  top 1　Name from Unit c where c.Seq = e2.ExecUnitSeq) execUnitName,
                ( select  top 1　Name from Unit c where c.Seq = e2.ExecSubUnitSeq) execSubUnitName,
                e2.*
                from
                TreePlantMain e2
                inner join PrjXML e1 on ( e2.EngSeq =  e1.Seq and e2.EngCreatType = 2)
                where  1=1 
            " + Utils.getAuthoritySql("e2.", null, "e2.EngSeq", false, new List<int>() { 6, 7 })
            + @" 
                or (e2.EngSeq is null and e2.OrganizerUserSeq = @userSeq ) 
            ";
            string sql3 = @"
                select 
                (select top 1 Name from TreePlantEngType c where c.Seq = e2.TPEngTypeSeq ) TPEngTypeName,
                ( select  top 1　Name from Unit c where c.Seq = e2.ExecUnitSeq) execUnitName,
                ( select  top 1　Name from Unit c where c.Seq = e2.ExecSubUnitSeq) execSubUnitName,
                e2.*
                from
                TreePlantMain e2
                where e2.EngCreatType = 3

            " + Utils.getAuthoritySql("e2.", null, "e2.EngSeq", false, new List<int>() { 6, 7 })
            + @" or (e2.EngSeq is null and e2.OrganizerUserSeq = @userSeq) 
                
            ";
            SqlCommand cmd = db.GetCommand(sql);
            SqlCommand cmd2 = db.GetCommand(sql2);
            SqlCommand cmd3 = db.GetCommand(sql3);
            cmd.Parameters.AddWithValue("@userSeq", user.Seq);
            cmd2.Parameters.AddWithValue("@userSeq", user.Seq);
            cmd3.Parameters.AddWithValue("@userSeq", user.Seq);

            var list = db.GetDataTableWithClass<TreePlantMainPrj>(cmd);
            list.AddRange(db.GetDataTableWithClass<TreePlantMainPrj>(cmd2));
            list.AddRange(db.GetDataTableWithClass<TreePlantMainPrj>(cmd3));
            return list;
        }
        public List<TreePlantMainPrj> getTreePlantMainList(int year, int TPEngTypeSeq, string[] subUnit)
        {
            var user = new SessionManager().GetUser();

            string sql = @"
                select 
                e1.EngNo,
                (select top 1 Name from TreePlantEngType c where c.Seq = e2.TPEngTypeSeq ) TPEngTypeName,
                ( select  top 1　Name from Unit c where c.Seq = e2.ExecUnitSeq) execUnitName,
                ( select  top 1　Name from Unit c where c.Seq = e2.ExecSubUnitSeq) execSubUnitName,
                e2.*
                from
                TreePlantMain e2
                inner join EngMain e1 on (e2.EngSeq = e1.Seq and e2.EngCreatType = 1)
                where (e2.EngYear = @Year or @Year = 0)
                and 
                (e2.TPEngTypeSeq = @TPEngTypeSeq or @TPEngTypeSeq = 0)
                and (
                    (e2.ExecUnitSeq  = ( select top 1 Seq from Unit c where c.Name = @parentUnit )  or  @parentUnit =''  ) 
                    and
                    (e2.ExecSubUnitSeq  in ( select Seq from Unit c where c.Name = @subUnit )  or @subUnit = '' or  @parentUnit ='') 
                )  
	
                and ( 1=1 
            " + Utils.getAuthoritySql("e2.", null, "e2.EngSeq", false, new List<int>() { 6, 7 })
            + @" or (e2.EngSeq is null and e2.CreateUserSeq = @userSeq ) 
                )
            ";

            string sql2 = @"
                select 
                (select top 1 Name from TreePlantEngType c where c.Seq = e2.TPEngTypeSeq ) TPEngTypeName,
                e1.ExecUnitName execUnitName,
                null execSubUnitName,
                e2.*
                from
                TreePlantMain e2
                inner join PrjXML e1 on ( e2.EngSeq =  e1.Seq and e2.EngCreatType = 2)
                where (e2.EngYear = @Year or @Year = 0)
                and 
                (e2.TPEngTypeSeq = @TPEngTypeSeq or @TPEngTypeSeq = 0)
                and (
                    (e2.ExecUnitSeq  = ( select top 1 Seq from Unit c where c.Name = @parentUnit )  or  @parentUnit =''  ) 
                    and
                    (e2.ExecSubUnitSeq  in ( select Seq from Unit c where c.Name = @subUnit )  or @subUnit = '' or  @parentUnit ='') 
                )  
	
                and ( 1=1 
            " + Utils.getAuthoritySql("e2.", null, "e2.EngSeq", false, new List<int>() { 6, 7 })
            + @" 
                )
            ";
            SqlCommand cmd = db.GetCommand(sql);
            SqlCommand cmd2 = db.GetCommand(sql2);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@subUnit", subUnit[1]);
            cmd.Parameters.AddWithValue("@parentUnit", subUnit[0]);
            cmd.Parameters.AddWithValue("@userSeq", user.Seq);
            cmd.Parameters.AddWithValue("@TPEngTypeSeq", TPEngTypeSeq);

            cmd2.Parameters.AddWithValue("@Year", year);
            cmd2.Parameters.AddWithValue("@subUnit", subUnit[1]);
            cmd2.Parameters.AddWithValue("@parentUnit", subUnit[0]);
            cmd2.Parameters.AddWithValue("@userSeq", user.Seq);
            cmd2.Parameters.AddWithValue("@TPEngTypeSeq", TPEngTypeSeq);

            //using (var context = new EQC_NEW_Entities())
            //{
            //    TreePlantMainPrj.unitList = context.Unit.ToList();
            //}
            var list = db.GetDataTableWithClass<TreePlantMainPrj>(cmd);
            list.AddRange(db.GetDataTableWithClass<TreePlantMainPrj>(cmd2));
            return list;
        }

        //public List<TreePlantMainPrj> getTreePlantMainList(int page, int perPage, int year, string subUnit)
        //{
        //    var user = new SessionManager().GetUser();
        //    string sql = @"
        //        select 
        //        e0.Seq treeMainPrjSeq, 
        //        e1.EngNo,
        //        (select top 1 Name from TreePlantEngType c where c.Seq = e2.TPEngTypeSeq ) TPEngTypeName,
        //        ( select  top 1　Name from Unit c where c.Seq = e2.ExecUnitSeq) execUnitName,
        //        ( select  top 1　Name from Unit c where c.Seq = e2.ExecSubUnitSeq) execSubUnitName,
        //        e2.*
        //        from
        //        TreePlantMain e2
        //        left join EngMain e1 on e2.EngSeq = e1.Seq
        //        left join PrjXML e0 on e1.engSeq = e0.Seq
        //        where (e1.EngYear = @Year or @Year = 0)
        //        and (
        //            (e2.ExecUnitSeq  = ( select top 1 Seq from Unit c where c.Name = @subUnit )  or @subUnit = '') 
        //            or
        //            (e2.ExecSubUnitSeq  = ( select top 1 Seq from Unit c where c.Name = @subUnit )  or @subUnit = '') 
        //        )  
        //         order by e0.Seq
        //            OFFSET @pageIndex ROWS
        //            FETCH FIRST @pageRecordCount ROWS ONLY

        //    ";

        //    SqlCommand cmd = db.GetCommand(sql);
        //    cmd.Parameters.AddWithValue("@Year", year);
        //    cmd.Parameters.AddWithValue("@subUnit", subUnit == "" ? user.UnitName1 : subUnit);
        //    cmd.Parameters.AddWithValue("@pageIndex", perPage * (page - 1));
        //    cmd.Parameters.AddWithValue("@pageRecordCount", perPage);
        //    return db.GetDataTableWithClass<TreePlantMainPrj>(cmd);
        //}
        //public int getTreePlantMainListCount(int year, string subUnit)
        //{
        //    var user = new SessionManager().GetUser();
        //    string sql = @"
        //        select
        //        Count(*)
        //        from
        //        PrjXML e0
        //        inner join EngMain e1 on e1.engSeq =  e0.Seq
        //        inner join TreePlantMain e2 on e2.EngSeq = e1.Seq
        //        where (e1.EngYear = @Year or @Year = 0)
        //        and (e1.ExecSubUnitSeq = ( select top 1 Seq from Unit c where c.Name = @subUnit ) or @subUnit = '')

        //    ";
        //    SqlCommand cmd = db.GetCommand(sql);
        //    cmd.Parameters.AddWithValue("@subUnit", subUnit == "" ? user.UnitName1 : subUnit);
        //    cmd.Parameters.AddWithValue("@Year", year);
        //    return (int)db.ExecuteScalar(cmd);
        //}
        public List<int> GetTenderListYearOption(List<EngMainTreeVModel> list, int createType)
        {
            if (createType == 1)
            {
                return list
                .Where(row => row.EngYear != 0)
                .Select(row => row.EngYear)
                .OrderByDescending(row => row)
                .Distinct().ToList();
            }
            else if (createType == 2)
            {
                return list
                    .Where(row => DateTime.TryParse(row.ActualBidAwardDate, out DateTime result))
                    .Select(row => Convert.ToDateTime(row.ActualBidAwardDate).Year - 1911)
                    .OrderByDescending(row => row)
                    .Distinct()
                    .ToList();
            }
            return new List<int>();

        }
        public List<int?> TreePlantYearOption(List<TreePlantMainPrj> list)
        {
            return list
                    .Where(row => row.EngYear != null)
                    .Select(row => row.EngYear)
                    .OrderByDescending(row => row)
                    .Distinct().ToList();
        }
        public List<string> TreePlantUnitOption(List<TreePlantMainPrj> list, string[] subUnit)
        {
            using (var context = new EQC_NEW_Entities())
            {
                if (subUnit[0] == "")
                {
                    return list
                        .Where(row => row.execUnitName != null)
                        .OrderBy(row => row.ExecUnitSeq)
                        .Select(row => row.execUnitName)
                        .Distinct()
                        .ToList();
                }
                else
                {
                    return list.Where(row => row.execUnitName == subUnit[0] && row.execSubUnitName != null)
                         .OrderBy(row => row.ExecSubUnitSeq)
                        .Select(row => row.execSubUnitName)
                        .Distinct()
                        .ToList();
                }


            }

        }

        public List<TreePlantType> GetTreePlantTypes()
        {
            using (var context = new EQC_NEW_Entities())
            {
                return context.TreePlantType.ToList();
            }
        }
        public List<TreePlantEngType> GetTreePlantEngTypes(int createType)
        {
            using (var context = new EQC_NEW_Entities())
            {
                if (createType == 3)
                {
                    return context.TreePlantEngType.ToList();
                }
                else
                {
                    return context.TreePlantEngType
                        .Where(row => row.EngCreatType == 1  || row.EngCreatType == 2)
                        .ToList();
                }

            }
        }
        public List<TreePlantNumList> getTreePlantNumList(int engSeq = 0, int treePlantMainSeq = 0)
        {
            using (var context = new EQC_NEW_Entities())
            {
                return
                    treePlantMainSeq == 0 ? context
                    .EngMain
                    .Where(row => row.Seq == engSeq)
                    .FirstOrDefault()?
                    .TreePlantMain
                    .FirstOrDefault()?
                    .TreePlantNumList.ToList() ?? new List<TreePlantNumList>() :

                     context.TreePlantMain.Find(treePlantMainSeq)?.TreePlantNumList.ToList()
                        ?? new List<TreePlantNumList>()
                    ;
            }
        }
        public List<TreePlantMonth> getTreePlantMonths(int engSeq = 0, int treePlantMainSeq = 0)
        {
            using (var context = new EQC_NEW_Entities())
            {
                return treePlantMainSeq == 0 ?
                    context
                    .EngMain
                    .Where(row => row.Seq == engSeq)
                    .FirstOrDefault()?
                    .TreePlantMain
                    .FirstOrDefault()?
                    .TreePlantMonth.ToList() ?? new List<TreePlantMonth>() :

                     context.TreePlantMain.Find(treePlantMainSeq)?.TreePlantMonth.ToList()
                    ?? new List<TreePlantMonth>();

            }
        }
        public void updateTreePlantMonth(List<TreePlantMonth> monthList, int engSeq = 0, int treePlantSeq = 0)
        {
            var user = new SessionManager().GetUser();
            using (var context = new EQC_NEW_Entities())
            {
                int i = 0;
                List<TreePlantMonth> treePlantMonths = treePlantSeq == 0 ?
                    context.TreePlantMonth
                    .Where(row => row.TreePlantMain.EngMain.Seq == engSeq).ToList() :
                    context.TreePlantMonth.Where(row => row.TreePlantSeq == treePlantSeq).ToList();

                List<TreePlantMonth> deleteList
                    = monthList.Count < treePlantMonths.Count ?
                    treePlantMonths.GetRange(monthList.Count, treePlantMonths.Count - monthList.Count) : new List<TreePlantMonth>();

                List<TreePlantMonth>
                    insertList =
                        monthList.Count > treePlantMonths.Count ?
                        monthList.GetRange(treePlantMonths.Count, monthList.Count - treePlantMonths.Count) : new List<TreePlantMonth>();

                List<TreePlantMonth>
                     updateList = monthList.GetRange(0, monthList.Count);

                insertTreePlantMonth(insertList, engSeq, treePlantSeq);

                foreach (TreePlantMonth item in deleteList)
                {
                    context.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                }

                foreach (TreePlantMonth item in updateList)
                {
                    if (i >= treePlantMonths.Count) break;
                    item.Seq = treePlantMonths[i].Seq;
                    item.TreePlantSeq = treePlantMonths[i].TreePlantSeq;
                    item.CreateTime = treePlantMonths[i].CreateTime;
                    item.CreateUserSeq = treePlantMonths[i].CreateUserSeq;
                    item.ModifyTime = DateTime.Now;
                    item.ModifyUserSeq = user.Seq;
                    context.Entry(treePlantMonths[i]).CurrentValues.SetValues(item);
                    i++;

                }
                context.SaveChanges();
            }
        }

        public void insertTreePlantMonth(List<TreePlantMonth> list, int engSeq = 0, int treePlantSeq = 0)
        {
            using (var context = new EQC_NEW_Entities())
            {
                var user = new SessionManager().GetUser();
                treePlantSeq =
                    treePlantSeq == 0 ? context.TreePlantMain
                        .Where(row => row.EngMain.Seq == engSeq)
                        .FirstOrDefault()?.Seq ?? 0 : treePlantSeq;
                foreach (var item in list)
                {

                    item.TreePlantSeq = treePlantSeq;

                    item.CreateTime = DateTime.Now;
                    item.ModifyTime = DateTime.Now;
                    item.CreateUserSeq = user.Seq;
                    item.ModifyUserSeq = user.Seq;
                    context.TreePlantMonth.Add(item);

                }

                context.SaveChanges();
            }

        }
        public void setCitySeq(TreePlantMain m, string cityName)
        {

        }
            public void setExecUnitSeq(TreePlantMain m, string townCity)
        {
            using (var context = new EQC_NEW_Entities())
            {
                foreach (var city in context.City)
                {
                    if (context.Unit.Where(r => r.Name == townCity).FirstOrDefault() is EDMXModel.Unit  execUnit) 
                    {
                        m.ExecUnitSeq = execUnit.Seq;
                    }
                    if (townCity.Contains(city.CityName)
                        )
                    {
                        string unitName = context.Country2WRAMapping
                            .Where(row => city.CityName == row.Country)
                            .FirstOrDefault()?.RiverBureau ?? "";

                        m.ExecUnitSeq = (context
                            .Unit
                            .Where(unit =>
                                unit.Name == unitName
                            )
                            .FirstOrDefault()?.Seq ?? 0);
                    }
                    

                }

            }
        }
        public void updateTreeMain(TreePlantMain m)
        {
            using (var context = new EQC_NEW_Entities())
            {
                var item = context.TreePlantMain.Find(m.Seq);
                var user = new SessionManager().GetUser();
                m.CreateTime = item.CreateTime;
                m.CreateUserSeq = item.CreateUserSeq;
                m.ModifyTime = DateTime.Now;
                m.ModifyUserSeq = user.Seq;
                context.Entry(item).CurrentValues.SetValues(m);
                context.SaveChanges();
            }

        }

        public int insertTreeMain(TreePlantMain m)
        {
            int insertSeq;
            var user = new SessionManager().GetUser();
            using (var context = new EQC_NEW_Entities())
            {
                m.CreateTime = DateTime.Now;
                m.ModifyTime = DateTime.Now;
                m.ModifyUserSeq = user.Seq;
                m.CreateUserSeq = user.Seq;
                context.TreePlantMain.Add(m);
                context.SaveChanges();
                insertSeq = context.TreePlantMain
                    .ToList()
                    .LastOrDefault()?.Seq ?? 0;
            }
            return insertSeq;
        }
        public int insertTreeMain(TreePlantMain m, int engSeq)
        {
            int insertSeq;
            using (var context = new EQC_NEW_Entities())
            {

                if (m.EngCreatType == 1)
                {
                    EQC.EDMXModel.EngMain eng = context.EngMain
                    .Find(engSeq);

                    m.EngNo = eng.EngNo;
                    m.EngYear = eng.EngYear;
                    m.EngSeq = eng.Seq;
                    m.EngName = eng.EngName;
                    m.EngTownSeq = eng.EngTownSeq;
                    m.OrganizerUnitSeq = eng.OrganizerUnitSeq;
                    m.ExecUnitSeq = eng.ExecUnitSeq;
                    m.ExecSubUnitSeq = eng.ExecSubUnitSeq;
                    m.OrganizerUserSeq = eng.OrganizerUserSeq;

                    m.CitySeq = context.Town.Find(eng.EngTownSeq)?.City.Seq;

                }
                else if (m.EngCreatType == 2)
                {
                    m.EngSeq = engSeq;
                    var townName = context.PrjXML.Find(engSeq).TownName;
                    context.City.ToList().ForEach(e => {
                        if (townName.Contains(e.CityName))
                        {
                            m.CitySeq = e.Seq;
                        }

                    });
                    context.Town.ToList().ForEach(e =>
                    {
                        if (townName.Contains(e.TownName))
                        {
                            m.EngTownSeq = e.Seq;
                        }

                    });



                }



                m.CreateTime = DateTime.Now;
                m.ModifyTime = DateTime.Now;
                m.CreateUserSeq = new SessionManager().GetUser().Seq;
                m.ModifyUserSeq = new SessionManager().GetUser().Seq;
                context.TreePlantMain.Add(m);
                context.SaveChanges();
                insertSeq = context.TreePlantMain
                    .ToList()
                    .LastOrDefault()?.Seq ?? 0;

            }
            return insertSeq;
        }
        /// <summary>
        ///取得標案pccess某年度每月預定面積進度
        /// </summary>
        public decimal[] getTreePlantSchMonthsAreaSumFromPcces(int eng, int year, TreePlantMain _treeMain = null)
        {
            decimal[] result = new decimal[12];
            TreePlantMain treeMain;
            using (var context = new EQC_NEW_Entities())
            {

                treeMain = _treeMain ?? context.TreePlantMain
                .Where(row => row.EngSeq == eng)
                .FirstOrDefault();
            }
            var standardPayItem =
                getPayItem(eng);

            if (standardPayItem == null) return result;
            var treeNumRatio = getTreePlantRatio(eng);

            var payItemDic = standardPayItem.ToDictionary(row => row.PayItem, row => row);

            var schMonthProgress = new Dictionary<string, Dictionary<string, int>>(
                getSchProgress(eng, standardPayItem)
                    ).ToArray();

            var payItemList = schMonthProgress.Length > 0 ? schMonthProgress
                .FirstOrDefault().Value.Select(itemProgress => itemProgress.Key) : new List<string>();

            var currentSchMonthPrgress = new Dictionary<string, Dictionary<string, int>>();


            for (int j = schMonthProgress.Length - 1; j > 0; j--)
            {
                var payItemsProgress = new Dictionary<string, int>(schMonthProgress[j].Value);
                foreach (var itemNo in payItemList)
                {
                    payItemsProgress[itemNo]
                        -= schMonthProgress[j - 1].Value[itemNo];
                }
                currentSchMonthPrgress.Add(schMonthProgress[j].Key, payItemsProgress);
            }
            if (schMonthProgress.Length > 0)
                currentSchMonthPrgress.Add(schMonthProgress[0].Key, new Dictionary<string, int>(schMonthProgress[0].Value));
            var currentSchMonthPrgressArr
                = currentSchMonthPrgress
                    .Where(row => row.Key.StartsWith(year.ToString()))
                    .Reverse()
                    .ToArray();


            int startMonth = currentSchMonthPrgressArr.Length > 0
                ? Int32.Parse(
                    currentSchMonthPrgressArr
                    .First().Key.Split('/')[1]) : 1;

            currentSchMonthPrgressArr
                .Where(dateGroup => dateGroup.Key.StartsWith(year.ToString()))
                .Select(monthProgress =>
                    monthProgress
                        .Value.Sum(itemProgress =>
                                (treeMain.ScheduledPlantTotalArea ?? 0)
                                * treeNumRatio[itemProgress.Key]
                                * itemProgress.Value / 100
                            )
                        )
                .ToList()
                .CopyTo(result, startMonth - 1);
            return result;
        }
        /// <summary>
        ///從植樹專區儲存區取得標案某年度每月預定面積進度
        /// </summary>
        public decimal[] getTreePlantSchMonthsAreaSum(int treeMainSeq, int year)
        {
            decimal[] result = new decimal[12];
            TreePlantMain treeMain;
            using (var context = new EQC_NEW_Entities())
            {

                treeMain = context.TreePlantMain
                .Where(row => row.Seq == treeMainSeq)
                .FirstOrDefault();
            }
            using (var context = new EQC_NEW_Entities())
            {
                var monthsList = context.TreePlantMonth
                    .Where(row => row.TreePlantMain.Seq == treeMainSeq && row.Year == year)
                    .Select(row => row.ScheduledArea ?? 0)
                    .ToArray();
                int startMonth = getTreePlantYearStartMonth(treeMain, year);
                int endMonth = getTreePlantYearEndMonth(treeMain, year);

                monthsList
                    .ToArray()
                    .CopyTo(result, startMonth - 1);

            }
            return result;
        }
        public int getTreePlantYearEndMonth(TreePlantMain treeMain, int year)
        {
            return treeMain.ScheduledCompletionDate?.Year == year + 1911 ?
                treeMain.ScheduledCompletionDate?.Month ?? 12 : 12;
        }
        public int getTreePlantYearStartMonth(TreePlantMain treeMain, int year)
        {
            return treeMain.ScheduledPlantDate?.Year == year + 1911 ?
                treeMain.ScheduledPlantDate?.Month ?? 1 : 1;
        }

        /// <summary>
        ///取得標案pccess某年度每月已完成面積進度
        /// </summary>
        public decimal[] getTreePlantAclMonthsAreaSumFromPcces(int? eng, int year, TreePlantMain _treeMain = null)
        {
            decimal[] result = new decimal[12];
            TreePlantMain treeMain;
            var PayItem =
                getPayItem(eng ?? 0);
            var qualitySum = PayItem != null ? getTreePlantSum(PayItem) : 0;

            using (var context = new EQC_NEW_Entities())
            {

                treeMain = _treeMain ?? context.TreePlantMain
                .Where(row => row.EngSeq == eng)
                .FirstOrDefault();
            }

            getAclNumProgress(eng ?? 0)

            .Where(dateGroup => dateGroup.Key?.StartsWith(year.ToString()) ?? false)
                .ToDictionary(dateGroup => Int32.Parse(dateGroup.Key.Split('/')[1]),
                    row => qualitySum != 0 ? (int)Math.Round(
                        (treeMain.ScheduledPlantTotalArea ?? 0) * ((decimal)row.Value.Sum() / qualitySum)
                   ) : 0)
                .ToList()
                .ForEach(e => result[e.Key - 1] = e.Value);

            return result;

        }

        /// <summary>
        ///從植樹專區儲存區取得標案某年度每月已完成面積進度
        /// </summary>
        public decimal[] getTreePlantAclMonthsAreaSum(int? eng, int year, int createType)
        {
            decimal[] result = new decimal[12];
            TreePlantMain treeMain;
            var PayItem =
                getPayItem(eng ?? 0);
            var qualitySum = PayItem != null ? getTreePlantSum(PayItem) : 0;

            using (var context = new EQC_NEW_Entities())
            {

                treeMain = context.TreePlantMain
                .Where(row => row.EngSeq == eng)
                .FirstOrDefault();
                var treeSum = context.TreePlantNumList
                    .Where(row => row.TreePlantMain.EngSeq == eng)
                    .Sum(row => row.ScheduledPlantNum);


                int startMonth = treeMain?.ScheduledPlantDate?.Month ?? 0;
                int endMonth = treeMain?.ScheduledCompletionDate?.Month ?? -1;
                if (startMonth <= endMonth
                    && treeMain?.ScheduledPlantDate?.Year == treeMain?.ScheduledCompletionDate?.Year)
                {
                    int i = startMonth - 1;
                    context.TreePlantMonth
                        .Where(row => row.TreePlantMain.EngSeq == eng)
                        .Select(row => (int)(row.ActualArea ?? 0))
                        .Take(endMonth - startMonth + 1)
                        .ToList()
                        .Aggregate(0, (cur, item) =>
                        {

                            cur = cur + item;
                            result[i] = cur;
                            i++;
                            return cur;
                        });
                }
            }


            return result;
        }

        /// <summary>
        ///從植樹專區儲存區取得標案某年度已完成面積進度
        /// </summary>
        public decimal getTreePlantComplateArea(int treeMainSeq, int year)
        {
            TreePlantMain treeMain;
            using (var context = new EQC_NEW_Entities())
            {
                return context
                    .TreePlantMonth
                    .Where(row => row.TreePlantMain.Seq == treeMainSeq && row.Year == year)
                    .ToList()
                    .Sum(row => (decimal)(row.ActualArea ?? 0));
            }
        }

        /// <summary>
        ///計算標案pcces資料並取得樹木預定總數
        /// </summary>
        public decimal getTreePlantSum(List<SchEngProgressPayItem> list)
        {
            return list.Sum(row => (int)(row.Quantity ?? 0));
        }


        /// <summary>
        ///計算標案pcces資料並取得喬木、灌木已完成總數
        /// </summary>
        //public Dictionary<string, int> getTreePlantTypeAclNumFromPcces(int eng)
        //{

        //    var payItemOverviewTemp = getPayItemOverview(eng)
        //        .Where(row => row.SupDailyDate.ItemDate <= DateTime.Now);
        //    return payItemOverviewTemp
        //        .GroupBy(row => row.RefItemCode.Substring(0, row.RefItemCode.Length < 5 ? row.RefItemCode.Length : 5))
        //        .ToDictionary(row => row.Key, row => row.Sum(r => (int)r.TodayConfirm));
        //}

        /// <summary>
        ///從植樹專區儲存區計算喬木、灌木預定數量
        /// </summary>
        public Dictionary<string, int> getTreePlantTypeSchNum(int treeMainSeq)
        {

            return getTreePlantNumList(0, treeMainSeq)
                    .Where(r => r.TreeType != null)
                .GroupBy(row => row.TreeType)
                .ToDictionary(row => row.Key, row => row.Sum(r => (int)(r.ScheduledPlantNum ?? 0)));

        }

        /// <summary>
        ///從植樹專區儲存區計算喬木、灌木已完成數量
        /// </summary>
        public Dictionary<string, int> getTreePlantTypeAclNum(int treeMainSeq)
        {

            return getTreePlantNumList(0, treeMainSeq)
                .Where(r => r.TreeType != null)
                .GroupBy(row => row.TreeType)
                .ToDictionary(row => row.Key, row => row.Sum(r => (int)(r.ActualPlantNum ?? 0) ));

        }

        /// <summary>
        ///從植樹專區儲存區計算樹木預定數量並組成字串回傳
        /// </summary>
        public Dictionary<string, string> getTreeNumExport(int treeMainSeq)
        {

            return
                getTreePlantNumList(0, treeMainSeq)
                .Where(r => r.TreeType != null)
                .GroupBy(row => row.TreeType)
                .ToDictionary(row => row.Key,
                    row => row.Aggregate("", (last, cur) => last + $"{cur.TreeType}-{cur.TreeTypeName}*{cur.ScheduledPlantNum ?? 0}\r\n")
                );


        }

        /// <summary>
        ///從植樹專區儲存區計算樹木(自行創建)預定數量並組成字串回傳
        /// </summary>
        public string getNonNormalTreeNumExport(int treeMainSeq)
        {

            return
                getTreePlantNumList(0, treeMainSeq)
                .Where(r => r.TreeTypeName != null)
                .Aggregate("", (last, cur) => cur.ScheduledPlantNum > 0 ? last + $"{cur.TreeTypeName}*{cur.ScheduledPlantNum ?? 0}\r\n" : last + "");


        }
        /// <summary>
        ///從植樹專區儲存區計算已完成面積
        /// </summary>
        public decimal getTreeAclArea(int treeMainSeq, int year)
        {
            using (var context = new EQC_NEW_Entities())
            {
                return context.TreePlantMonth
                    .Where(row => row.TreePlantSeq == treeMainSeq && row.Year == year)
                    .Sum(row => row.ActualArea) ?? 0;
            }
        }

        /// <summary>
        ///從植樹專區儲存區計算已完成面積
        /// </summary>
        public decimal getTreeSchArea(int treeMainSeq, int year)
        {
            using (var context = new EQC_NEW_Entities())
            {
                return context.TreePlantMonth
                    .Where(row => row.TreePlantSeq == treeMainSeq && row.Year == year)
                    .Sum(row => row.ScheduledArea) ?? 0;
            }
        }

        /// <summary>
        ///從pccess取得標案計價項目數量占比
        /// </summary>
        public Dictionary<string, decimal> getTreePlantRatio(int eng)
        {
            var list = getPayItem(eng);
            decimal qualitySum = getTreePlantSum(list);
            if (qualitySum == 0)
            {
                return list.ToDictionary(row => row.PayItem, row => (decimal)0);
            }
            return list
                .ToDictionary(row => row.PayItem, row => qualitySum != 0 ? ((row.Quantity ?? 0) / qualitySum) : 0);
        }





        private int convertRefCodeToTreeSeq(string refCode, int start, int? end = null)
        {
            int _end = end ?? refCode.Length;

            for (int i = _end; i >= start; i--)
            {
                if (treeRefCodeDic.TryGetValue(refCode.Substring(0, i), out int resultTreeSeq))
                    return resultTreeSeq;
            }
            return 0;
        }

        /// <summary>
        ///從pccess計算標案各個樹種預定數量
        /// </summary>
        public Dictionary<int, int> getTreePlantNumFromPcces(int eng)
        {
            var list = getPayItem(eng);
            var a = list


                 .GroupBy(row =>
                     convertRefCodeToTreeSeq(row.RefItemCode, 5)
                  );

            return a.ToDictionary(row => row.Key, row => (int)(row.Sum(r => r.Quantity)));

        }


        /// <summary>
        ///從pccess計算標案各個樹種已完成數量
        /// </summary>
        public Dictionary<int, int> getTreePlantAclNumFromPcces(int eng)
        {
            var list = getPayItemOverview(eng);
            return list

                .Where(row => row.SupDailyDate.ItemDate <= DateTime.Now)
                .GroupBy(row =>
                    convertRefCodeToTreeSeq(row.SchEngProgressPayItem.RefItemCode, 5)
                 )
                .Where(row => row.Key != 0)

                .ToDictionary(row => row.Key, row => (int)(row.Sum(r => r.TodayConfirm)));
        }

    }
}
