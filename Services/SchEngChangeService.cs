using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class SchEngChangeService : BaseService
    {//工程變更
        public const int _cyExtensionWork = 2; //展延工期
        public const int _cyBackWork = 3; //復工
        public const int _cyStopWork = 100; //停工
        public const int _cyTerminateContract = 200; //200:解除契約

        //刪除工程變更
        public bool DelEngChange(EPCProgressEngChangeListVModel ec)
        {
            SqlCommand cmd;
            db.BeginTransaction();
            string sql;
            try
            {
                sql = @"
                    delete EC_SchEngProgressWorkItem where EC_SchEngProgressPayItemSeq in (
                        select seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq = @EC_SchEngProgressHeaderSeq
                    )
                    delete EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq =@EC_SchEngProgressHeaderSeq;
                    delete EC_SchEngProgressHeader where Seq=@EC_SchEngProgressHeaderSeq;
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EC_SchEngProgressHeaderSeq", ec.Seq);
                db.ExecuteNonQuery(cmd);

                if(ec.ChangeType == _cyStopWork) {//s20231014
                    sql = @"delete SupDailyReportWork where Seq=@Seq";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", ec.SupDailyReportWorkSeq);
                    db.ExecuteNonQuery(cmd);
                } else if (ec.ChangeType == _cyBackWork) {//s20231014
                    sql = @"
                        update SupDailyReportWork set
                            BackWorkDate = null,
                            BackWorkNo = '',
                            BackWorkApprovalFile = ''
                        where Seq=@Seq
                    ";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", ec.SupDailyReportWorkSeq);
                    db.ExecuteNonQuery(cmd);
                } else if (ec.ChangeType == _cyExtensionWork) {//s20231014
                    sql = @"delete SupDailyReportExtension where Seq=@Seq";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", ec.SupDailyReportExtensionSeq);
                    db.ExecuteNonQuery(cmd);
                }

                sql = @"
                    Update SchProgressHeader set
                        SPState=1,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where EngMainSeq=@EngMainSeq
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", ec.EngMainSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchEngChangeService.DelEngChange: " + e.Message);
                return false;
            }
        }

        //拒絕往來廠商
        public List<T> GetRejectCompany<T>(int engMainSeq)
        {
            string sql = @"
                select
	                a.Seq,
                    a.Corporation_Number,
                    a.Corporation_Name,
                    a.Case_no,
                    a.Case_Name,
                    a.Effective_Date,
                    a.Expire_Date
                from EngMain b
                inner join RVFile a on(a.Case_no=b.EngNo)
                where b.Seq=@engMainSeq
                order by a.Corporation_Number, a.Effective_Date DESC";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@engMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //當日預定進度 s20230408
        public List<T> GetPayItemSchProgress<T>(int engMainSeq, DateTime tarDate)
        {
            string sql = @"
                select
                    z.EC_SchEngProgressPayItemSeq
                    ,z.SPDate
                    ,IIF(z.DayProgress=-1,-1,z.SchProgress) SchProgress --預定進度
                    ,z.DayProgress
                    ,z.DayProgressAfter
                    ,z.Days
                from (
                    select
    	                a1.OrderNo,
                        a.EC_SchEngProgressPayItemSeq
                        ,@tarDate SPDate
                        ,(
                            a.SchProgress - a.DayProgress * DATEDIFF(Day, IIF(@tarDate>a.SPDate, a.SPDate, @tarDate), a.SPDate)
                        ) SchProgress --預定進度 %
                        ,a.DayProgress
                        ,a.DayProgressAfter
                        ,(
                            a.Days - DATEDIFF(Day, IIF(@tarDate>a.SPDate, a.SPDate, @tarDate), a.SPDate)
                        ) Days
                    from EC_SchEngProgressHeader b
                    inner join EC_SchEngProgressPayItem b1 on(b1.EC_SchEngProgressHeaderSeq=b.Seq)
                    inner join EC_SchProgressPayItem a on(a.EC_SchEngProgressPayItemSeq=b1.Seq)
                    inner join EC_SchEngProgressPayItem a1 on(a1.Seq=a.EC_SchEngProgressPayItemSeq)
                    where b.EngMainSeq=@engMainSeq
                    and (b.StartDate<=@tarDate and (b.EndDate is null or b.EndDate>=@tarDate)) --s20230527
                    and a.SPDate = (
                        select top 1 z1.SPDate from (
                          select top 1 z.SPDate from EC_SchEngProgressPayItem z1
                          inner join EC_SchProgressPayItem z on(z.EC_SchEngProgressPayItemSeq=z1.Seq)
                          where z1.EC_SchEngProgressHeaderSeq = b.Seq
                          and z.SPDate >= @tarDate
                          order by z.SPDate
          
                          union all
          
                          select top 1 z.SPDate from EC_SchEngProgressPayItem z1
                          inner join EC_SchProgressPayItem z on(z.EC_SchEngProgressPayItemSeq=z1.Seq)
                          where z1.EC_SchEngProgressHeaderSeq = b.Seq
                          and z.SPDate < @tarDate
                          order by z.SPDate desc
                        ) z1
                        order by z1.SPDate desc
                    )
                ) z
                order by z.OrderNo
            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@engMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@tarDate", tarDate);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //預定進度清單
        public List<T> GetSchProgressPayItemList<T>(int seq, string tarDate)
        {
            string sql = @"
                select
                    a.Seq,
                    a.EC_SchEngProgressPayItemSeq,
                    a1.OrderNo,
                    a1.PayItem,
                    a1.Description,
                    a1.Unit,
                    a1.Quantity,
                    a1.Price,
                    a1.Amount,
                    a.SchProgress,
                    a.DayProgress
                from EC_SchEngProgressHeader b
                inner join EC_SchEngProgressPayItem a1 on(a1.EC_SchEngProgressHeaderSeq=b.Seq)
                inner join EC_SchProgressPayItem a on(a.EC_SchEngProgressPayItemSeq=a1.Seq and a.SPDate=@SPDate)
                where b.Seq=@Seq
                Order by a1.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            cmd.Parameters.AddWithValue("@SPDate", tarDate);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //預定進度清單(初次變更)
        public List<T> GetSchProgressPayItemsAndDayBefore<T>(int seq, string tarDate)
        {
            string sql = @"
                select
                    a.Seq,
                    a.EC_SchEngProgressPayItemSeq,
                    a1.OrderNo,
                    a1.PayItem,
                    a1.Description,
                    a1.Unit,
                    a1.Quantity,
                    a1.Price,
                    a1.Amount,
                    a.SchProgress,
                    a.DayProgress,
                    cast(c.SchProgress as decimal(18, 4)) SchProgressDayBefore
                from EC_SchEngProgressHeader b
                inner join EC_SchEngProgressPayItem a1 on(a1.EC_SchEngProgressHeaderSeq=b.Seq)
                inner join EC_SchProgressPayItem a on(a.EC_SchEngProgressPayItemSeq=a1.Seq and a.SPDate=@SPDate)
                left outer join SchProgressPayItem c on(c.SPDate=DATEADD(day,-1,a.SPDate) and c.SchEngProgressPayItemSeq=a1.ParentSchEngProgressPayItemSeq)
                where b.Seq=@Seq
                Order by a1.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            cmd.Parameters.AddWithValue("@SPDate", tarDate);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //預定進度清單(初次變更)
        public List<T> GetSchProgressPayItemsAndDayBefore2<T>(int seq, string tarDate)
        {
            string sql = @"
                select
                    a.Seq,
                    a.EC_SchEngProgressPayItemSeq,
                    a1.OrderNo,
                    a1.PayItem,
                    a1.Description,
                    a1.Unit,
                    a1.Quantity,
                    a1.Price,
                    a1.Amount,
                    a.SchProgress,
                    a.DayProgress,
                    cast(c.SchProgress as decimal(18, 4)) SchProgressDayBefore
                from EC_SchEngProgressHeader b
                inner join EC_SchEngProgressPayItem a1 on(a1.EC_SchEngProgressHeaderSeq=b.Seq)
                inner join EC_SchProgressPayItem a on(a.EC_SchEngProgressPayItemSeq=a1.Seq and a.SPDate=@SPDate)
                left outer join EC_SchProgressPayItem c on(c.SPDate=DATEADD(day,-1,a.SPDate) and c.EC_SchEngProgressPayItemSeq=a1.ParentEC_SchEngProgressPayItemSeq)
                where b.Seq=@Seq
                Order by a1.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            cmd.Parameters.AddWithValue("@SPDate", tarDate);

            return db.GetDataTableWithClass<T>(cmd);
        }

        //預定進度日期清單
        public List<T> GetDateList<T>(int seq)
        {
            string sql = @"
                    select DISTINCT a.SPDate
                    from EC_SchEngProgressHeader b
                    inner join EC_SchEngProgressPayItem c on(c.EC_SchEngProgressHeaderSeq=b.Seq)
                    inner join EC_SchProgressPayItem a on(a.EC_SchEngProgressPayItemSeq=c.Seq)
                    where b.Seq=@Seq
                    Order by a.SPDate 
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        //工程變更進度清單調整(初次)
        public int EngChangeAddItems(SchProgressHeaderModel spHeader, List<SchProgressPayItemModel> payItemList, List<DateTime> dateList, List<SchProgressHeaderHistoryProgressModel> engSchProgress, EC_SchEngProgressHeaderModel engChange, List<EC_SchEngProgressPayItemModel> sepPayItems)
        {
            string sql = "";
            int userSeq = getUserSeq();
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                sql = @"
                    delete AskPaymentPayItem where AskPaymentHeaderSeq in (select Seq from AskPaymentHeader where EngMainSeq=@EngMainSeq and APDate>=@SPDate);

                    delete AskPaymentHeader where EngMainSeq=@EngMainSeq and APDate>=@SPDate;
 
                    delete SchProgressPayItem where SchProgressHeaderSeq=@SchProgressHeaderSeq and SPDate>=@SPDate1;
                    
                    delete SupDailyReportMiscConstruction where SupDailyDateSeq in (select Seq from SupDailyDate where EngMainSeq=@EngMainSeq and ItemDate>=@SPDate);
                    delete SupDailyReportMisc where SupDailyDateSeq in (select Seq from SupDailyDate where EngMainSeq=@EngMainSeq and ItemDate>=@SPDate);
                    delete SupDailyReportConstructionPerson where SupDailyDateSeq in (select Seq from SupDailyDate where EngMainSeq=@EngMainSeq and ItemDate>=@SPDate);
                    delete SupDailyReportConstructionMaterial where SupDailyDateSeq in (select Seq from SupDailyDate where EngMainSeq=@EngMainSeq and ItemDate>=@SPDate);
                    delete SupDailyReportConstructionEquipment where SupDailyDateSeq in (select Seq from SupDailyDate where EngMainSeq=@EngMainSeq and ItemDate>=@SPDate);
                    delete SupPlanOverview where SupDailyDateSeq in (select Seq from SupDailyDate where EngMainSeq=@EngMainSeq and ItemDate>=@SPDate);
                    delete SupDailyDate where EngMainSeq=@EngMainSeq and ItemDate>=@SPDate;
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", spHeader.EngMainSeq);
                cmd.Parameters.AddWithValue("@SchProgressHeaderSeq", spHeader.Seq);
                cmd.Parameters.AddWithValue("@SPDate", spHeader.EngChangeStartDate.Value);
                cmd.Parameters.AddWithValue("@SPDate1", spHeader.EngChangeStartDate.Value.AddDays(-1));
                db.ExecuteNonQuery(cmd);

                sql = @"
                    Update SchProgressHeader set
                        SPState=@SPState,
                        EngChangeState=@EngChangeState,
                        EngChangeCount = EngChangeCount+1,
                        EngChangeStartDate=@EngChangeStartDate,
                        EngChangeSchCompDate=@EngChangeSchCompDate,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq
                    ";
                bool fChangeType = (engChange.ChangeType == SchEngChangeService._cyStopWork  || engChange.ChangeType == SchEngChangeService._cyTerminateContract);
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", spHeader.Seq);
                cmd.Parameters.AddWithValue("@SPState", fChangeType ? 1 : 0); //s20230527
                cmd.Parameters.AddWithValue("@EngChangeState", fChangeType ? 0 : 1); //s20230527
                cmd.Parameters.AddWithValue("@EngChangeStartDate", spHeader.EngChangeStartDate.Value);
                cmd.Parameters.AddWithValue("@EngChangeSchCompDate", spHeader.EngChangeSchCompDate.Value);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                db.ExecuteNonQuery(cmd);

                sql = @"insert into SchProgressPayItem(
                        SchProgressHeaderSeq,
                        SchEngProgressPayItemSeq,
                        SPDate,
                        SchProgress,
                        DayProgress,
                        DayProgressAfter,
                        Days,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    )values(
                        @SchProgressHeaderSeq,
                        @SchEngProgressPayItemSeq,
                        @SPDate,
                        @SchProgress,
                        @DayProgress,
                        @DayProgressAfter,
                        @Days,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                foreach (SchProgressPayItemModel m in payItemList)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@SchProgressHeaderSeq", spHeader.Seq);
                    cmd.Parameters.AddWithValue("@SchEngProgressPayItemSeq", m.SchEngProgressPayItemSeq);
                    cmd.Parameters.AddWithValue("@SPDate", m.SPDate);
                    cmd.Parameters.AddWithValue("@SchProgress", m.SchProgress);
                    cmd.Parameters.AddWithValue("@DayProgress", m.DayProgress);
                    cmd.Parameters.AddWithValue("@DayProgressAfter", m.DayProgressAfter);
                    cmd.Parameters.AddWithValue("@Days", m.Days);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                    db.ExecuteNonQuery(cmd);
                }

                if (engSchProgress.Count > 0)
                {
                    sql = @"
                    insert into SchProgressHeaderHistory (
                        SchProgressHeaderSeq,
                        EngChangeCount,
                        EngChangeStartDate,
                        EngChangeSchCompDate,
                        CreateUserSeq
                    ) values (
                        @SchProgressHeaderSeq,
                        @EngChangeCount,
                        @EngChangeStartDate,
                        @EngChangeSchCompDate,
                        @ModifyUserSeq
                    )";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@SchProgressHeaderSeq", spHeader.Seq);
                    cmd.Parameters.AddWithValue("@EngChangeCount", spHeader.EngChangeCount + 1);
                    cmd.Parameters.AddWithValue("@EngChangeStartDate", spHeader.EngChangeStartDate.Value);
                    cmd.Parameters.AddWithValue("@EngChangeSchCompDate", spHeader.EngChangeSchCompDate.Value);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                    db.ExecuteNonQuery(cmd);

                    cmd.Parameters.Clear();
                    string sql1 = @"SELECT IDENT_CURRENT('SchProgressHeaderHistory') AS NewSeq";
                    cmd = db.GetCommand(sql1);
                    DataTable dTable = db.GetDataTable(cmd);
                    int schProgressHeaderHistorySeq = Convert.ToInt32(dTable.Rows[0]["NewSeq"].ToString());

                    sql = @"insert into SchProgressHeaderHistoryProgress(
                            SchProgressHeaderHistorySeq,
                            ProgressDate,
                            SchProgress
                        )values(
                            @SchProgressHeaderHistorySeq,
                            @ProgressDate,
                            @SchProgress
                        )
                    ";
                    foreach (SchProgressHeaderHistoryProgressModel m in engSchProgress)
                    {
                        Null2Empty(m);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@SchProgressHeaderHistorySeq", schProgressHeaderHistorySeq);
                        cmd.Parameters.AddWithValue("@ProgressDate", m.ProgressDate);
                        cmd.Parameters.AddWithValue("@SchProgress", m.SchProgress);
                        db.ExecuteNonQuery(cmd);
                    }
                }

                sql = @"insert into EC_SchProgressPayItem(
                        EC_SchEngProgressPayItemSeq,
                        SPDate,
                        SchProgress,
                        DayProgress,
                        DayProgressAfter,
                        Days,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    )values(
                        @EC_SchEngProgressPayItemSeq,
                        @SPDate,
                        @SchProgress,
                        @DayProgress,
                        @DayProgressAfter,
                        @Days,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                DateTime? itemDate = null;
                if (payItemList.Count > 0) itemDate = payItemList[0].SPDate;
                foreach (DateTime dt in dateList)
                {
                    int day = dt.Subtract(itemDate.Value).Days;
                    foreach (EC_SchEngProgressPayItemModel m in sepPayItems)
                    {
                        decimal schProgress = 0;
                        decimal dayProgress = 0;
                        decimal dayProgressAfter = 0;
                        Null2Empty(m);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@EC_SchEngProgressPayItemSeq", m.Seq);
                        cmd.Parameters.AddWithValue("@SPDate", dt);
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                        if (!itemDate.HasValue || !fChangeType) //s20230527
                        {
                            cmd.Parameters.AddWithValue("@Days", DBNull.Value);
                        }
                        else
                        {//停工直接設定進度 s20230527
                            cmd.Parameters.AddWithValue("@Days", day);
                            if (m.ParentSchEngProgressPayItemSeq.HasValue)
                            {
                                foreach (SchProgressPayItemModel item in payItemList)
                                {
                                    if (m.ParentSchEngProgressPayItemSeq == item.SchEngProgressPayItemSeq)
                                    {
                                        schProgress = item.SchProgress;
                                        dayProgress = item.DayProgress;
                                        dayProgressAfter = item.DayProgressAfter;
                                        break;
                                    }
                                }
                            }
                        }
                        cmd.Parameters.AddWithValue("@SchProgress", schProgress);
                        cmd.Parameters.AddWithValue("@DayProgress", dayProgress);
                        cmd.Parameters.AddWithValue("@DayProgressAfter", dayProgressAfter);
                        db.ExecuteNonQuery(cmd);
                    }
                }

                sql = @"
                    Update EngMain set
                        EngChangeSchCompDate=@EngChangeSchCompDate,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", spHeader.EngMainSeq);
                cmd.Parameters.AddWithValue("@EngChangeSchCompDate", spHeader.EngChangeSchCompDate.Value);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                db.ExecuteNonQuery(cmd);
                
                sql = @"
                    Update EC_SchEngProgressPayItem set
                        OrderNo=@OrderNo,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq
                    ";
                foreach (EC_SchEngProgressPayItemModel m in sepPayItems)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", m.Seq);
                    cmd.Parameters.AddWithValue("@OrderNo", m.OrderNo);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                    db.ExecuteNonQuery(cmd);
                }

                sql = @"
                    Update EC_SchEngProgressHeader set
                        SPState=1,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", engChange.Seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                db.ExecuteNonQuery(cmd);

                //s20231014
                string scheChangeCloseDate = String.Format("{0}{1}", spHeader.EngChangeSchCompDate.Value.Year - 1911, spHeader.EngChangeSchCompDate.Value.ToString("MMdd"));
                sql = @"
                    update PrjXMLExt set
                        ScheChangeCloseDate=@ScheChangeCloseDate,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where PrjXMLSeq=(select PrjXMLSeq from EngMain where Seq=@EngMainSeq)
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", spHeader.EngMainSeq);
                cmd.Parameters.AddWithValue("@ScheChangeCloseDate", scheChangeCloseDate);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                int result = db.ExecuteNonQuery(cmd);
                if (result == 0)
                {
                    db.TransactionRollback();
                    return -2;
                }

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchEngChangeService.EngChangeAddItems: " + e.Message);
                return -1;
            }
        }
        //工程變更進度清單調整
        public int EngChangeAddItems2(SchProgressHeaderModel spHeader, List<EC_SchProgressPayItemModel> payItemList, List<DateTime> dateList, EC_SchEngProgressHeaderModel engChange, List<EC_SchEngProgressPayItemModel> sepPayItems)
        {
            string sql = "";
            int userSeq = getUserSeq();
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                sql = @"
                    delete EC_AskPaymentPayItem where EC_AskPaymentHeaderSeq in (select Seq from EC_AskPaymentHeader where EngMainSeq=@EngMainSeq and APDate>=@SPDate);

                    delete EC_AskPaymentHeader where EngMainSeq=@EngMainSeq and APDate>=@SPDate;
 
                    delete EC_SchProgressPayItem
                    where EC_SchEngProgressPayItemSeq in (
                        select Seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in(
                            select Seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                        )
                    )
                    and SPDate>=@SPDate1;
                    
                    delete EC_SupDailyReportMiscConstruction where EC_SupDailyDateSeq in (select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq and ItemDate>=@SPDate);
                    delete EC_SupDailyReportMisc where EC_SupDailyDateSeq in (select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq and ItemDate>=@SPDate);
                    delete EC_SupDailyReportConstructionPerson where EC_SupDailyDateSeq in (select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq and ItemDate>=@SPDate);
                    delete EC_SupDailyReportConstructionMaterial where EC_SupDailyDateSeq in (select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq and ItemDate>=@SPDate);
                    delete EC_SupDailyReportConstructionEquipment where EC_SupDailyDateSeq in (select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq and ItemDate>=@SPDate);
                    delete EC_SupPlanOverview where EC_SupDailyDateSeq in (select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq and ItemDate>=@SPDate);
                    delete EC_SupDailyDate where EngMainSeq=@EngMainSeq and ItemDate>=@SPDate;
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", spHeader.EngMainSeq);
                //cmd.Parameters.AddWithValue("@EC_SchEngProgressHeaderSeq", engChange.Seq);
                cmd.Parameters.AddWithValue("@SPDate", engChange.StartDate.Value);
                cmd.Parameters.AddWithValue("@SPDate1", engChange.StartDate.Value.AddDays(-1));
                db.ExecuteNonQuery(cmd);

                sql = @"
                    Update SchProgressHeader set
                        SPState=@SPState,
                        EngChangeState=@EngChangeState,
                        EngChangeCount = EngChangeCount+1,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq
                    ";
                bool fChangeType = (engChange.ChangeType == SchEngChangeService._cyStopWork || engChange.ChangeType == SchEngChangeService._cyTerminateContract);
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", spHeader.Seq);
                cmd.Parameters.AddWithValue("@SPState", fChangeType ? 1 : 0); //s20230527
                cmd.Parameters.AddWithValue("@EngChangeState", fChangeType ? 0 : 1); //s20230527
                cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                db.ExecuteNonQuery(cmd);

                sql = @"insert into EC_SchProgressPayItem(
                        SPDate,
                        EC_SchEngProgressPayItemSeq,
                        SchProgress,
                        DayProgress,
                        DayProgressAfter,
                        Days,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    )values(
                        @SPDate,
                        @EC_SchEngProgressPayItemSeq,
                        @SchProgress,
                        @DayProgress,
                        @DayProgressAfter,
                        @Days,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                foreach (EC_SchProgressPayItemModel m in payItemList)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EC_SchEngProgressPayItemSeq", m.EC_SchEngProgressPayItemSeq);
                    cmd.Parameters.AddWithValue("@SPDate", m.SPDate);
                    cmd.Parameters.AddWithValue("@SchProgress", m.SchProgress);
                    cmd.Parameters.AddWithValue("@DayProgress", m.DayProgress);
                    cmd.Parameters.AddWithValue("@DayProgressAfter", m.DayProgressAfter);
                    cmd.Parameters.AddWithValue("@Days", m.Days);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                    db.ExecuteNonQuery(cmd);
                }

                sql = @"insert into EC_SchProgressPayItem(
                        EC_SchEngProgressPayItemSeq,
                        SPDate,
                        SchProgress,
                        DayProgress,
                        DayProgressAfter,
                        Days,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    )values(
                        @EC_SchEngProgressPayItemSeq,
                        @SPDate,
                        @SchProgress,
                        @DayProgress,
                        @DayProgressAfter,
                        @Days,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                //
                DateTime? itemDate = null;
                if (payItemList.Count > 0) itemDate = payItemList[0].SPDate;
                foreach (DateTime dt in dateList)
                {
                    int day = dt.Subtract(itemDate.Value).Days;
                    foreach (EC_SchEngProgressPayItemModel m in sepPayItems)
                    {
                        decimal schProgress = 0;
                        decimal dayProgress = 0;
                        decimal dayProgressAfter = 0;
                        Null2Empty(m);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@EC_SchEngProgressPayItemSeq", m.Seq);
                        cmd.Parameters.AddWithValue("@SPDate", dt);
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                        if (!itemDate.HasValue || !fChangeType) //s20230527
                        {
                            cmd.Parameters.AddWithValue("@Days", DBNull.Value);
                        }
                        else
                        {//停工直接設定進度 s20230527
                            cmd.Parameters.AddWithValue("@Days", day);
                            if (m.ParentEC_SchEngProgressPayItemSeq.HasValue)
                            {
                                foreach (EC_SchProgressPayItemModel item in payItemList)
                                {
                                    if(m.ParentEC_SchEngProgressPayItemSeq == item.EC_SchEngProgressPayItemSeq)
                                    {
                                        schProgress = item.SchProgress;
                                        dayProgress = item.DayProgress;
                                        dayProgressAfter = item.DayProgressAfter;
                                        break;
                                    }
                                }
                            }
                        }
                        cmd.Parameters.AddWithValue("@SchProgress", schProgress);
                        cmd.Parameters.AddWithValue("@DayProgress", dayProgress);
                        cmd.Parameters.AddWithValue("@DayProgressAfter", dayProgressAfter);
                        db.ExecuteNonQuery(cmd);
                    }
                    itemDate = dt;
                }

                sql = @"
                    Update EngMain set
                        EngChangeSchCompDate=@EngChangeSchCompDate,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", engChange.EngMainSeq);
                cmd.Parameters.AddWithValue("@EngChangeSchCompDate", engChange.SchCompDate.Value);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                db.ExecuteNonQuery(cmd);

                sql = @"
                    Update EC_SchEngProgressPayItem set
                        OrderNo=@OrderNo,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq
                    ";
                foreach (EC_SchEngProgressPayItemModel m in sepPayItems)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", m.Seq);
                    cmd.Parameters.AddWithValue("@OrderNo", m.OrderNo);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                    db.ExecuteNonQuery(cmd);
                }

                sql = @"
                    Update EC_SchEngProgressHeader set
                        EndDate=@EndDate
                    where EngMainSeq=@EngMainSeq
                    and Version=@Version
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", engChange.EngMainSeq);
                cmd.Parameters.AddWithValue("@Version", engChange.Version-1);
                cmd.Parameters.AddWithValue("@EndDate", engChange.StartDate.Value.AddDays(-1));
                db.ExecuteNonQuery(cmd);

                sql = @"
                    Update EC_SchEngProgressHeader set
                        SPState=1,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", engChange.Seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                db.ExecuteNonQuery(cmd);

                //s20231014
                string scheChangeCloseDate = String.Format("{0}{1}", spHeader.EngChangeSchCompDate.Value.Year - 1911, spHeader.EngChangeSchCompDate.Value.ToString("MMdd"));
                sql = @"
                    update PrjXMLExt set
                        ScheChangeCloseDate=@ScheChangeCloseDate,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where PrjXMLSeq=(select PrjXMLSeq from EngMain where Seq=@EngMainSeq)
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", spHeader.EngMainSeq);
                cmd.Parameters.AddWithValue("@ScheChangeCloseDate", scheChangeCloseDate);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                int result = db.ExecuteNonQuery(cmd);
                if (result == 0)
                {
                    db.TransactionRollback();
                    return -2;
                }

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchEngChangeService.EngChangeAddItems2: " + e.Message);
                return -1;
            }
        }
        //新增工程變更
        public int AddEngChange(EPCProgressEngChangeListVModel m)
        {
            return AddEngChange(m, null, null);
        }
        public int AddEngChange(EPCProgressEngChangeListVModel m, SupDailyReportExtensionModel extensionModel)
        {
            return AddEngChange(m, extensionModel, null);
        }
        public int AddEngChange(EPCProgressEngChangeListVModel m, SupDailyReportWorkVModel workModel)
        {
            return AddEngChange(m, null, workModel);
        }

        /// <summary>
        /// 更新工程變更日期 s20231014
        /// </summary>
        /// <param name="m">更新工程變更日期</param>
        /// <returns></returns>
        public int UpdateEngDate(EPCProgressEngChangeListVModel m, int ec_SchEngProgressHeaderSeq, int mode)
        {
            SqlCommand cmd;
            string sql;
            Null2Empty(m);
            int userSeq = getUserSeq();

            db.BeginTransaction();
            try
            {
                sql = @"
                    update EC_SchEngProgressHeader set
                        StartDate=@StartDate,
                        SchCompDate=@SchCompDate,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", ec_SchEngProgressHeaderSeq);
                cmd.Parameters.AddWithValue("@StartDate", m.StartDate);
                cmd.Parameters.AddWithValue("@SchCompDate", m.SchCompDate);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                db.ExecuteNonQuery(cmd);
                
                //延展工期
                /*if (mode == 2)
                {
                    Null2Empty(extensionModel);
                    sql = @"
                    insert into SupDailyReportExtension(
                        EngMainSeq,
                        ExtendDays,
                        ApprovalNo,
                        ApprovalDate,
                        ExtendReason,
                        ExtendReasonOther,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EngMainSeq,
                        @ExtendDays,
                        @ApprovalNo,
                        @ApprovalDate,
                        @ExtendReason,
                        @ExtendReasonOther,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EngMainSeq", extensionModel.EngMainSeq);
                    cmd.Parameters.AddWithValue("@ExtendDays", extensionModel.ExtendDays);
                    cmd.Parameters.AddWithValue("@ApprovalNo", extensionModel.ApprovalNo);
                    cmd.Parameters.AddWithValue("@ApprovalDate", extensionModel.ApprovalDate);
                    cmd.Parameters.AddWithValue("@ExtendReason", extensionModel.ExtendReason);
                    cmd.Parameters.AddWithValue("@ExtendReasonOther", extensionModel.ExtendReasonOther);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);

                    cmd.Parameters.Clear();
                    sql1 = @"SELECT IDENT_CURRENT('SupDailyReportExtension') AS NewSeq";
                    cmd = db.GetCommand(sql1);
                    dt = db.GetDataTable(cmd);
                    int supDailyReportExtensionSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                    sql = @"
                    update EC_SchEngProgressHeader set
                        SupDailyReportExtensionSeq=@SupDailyReportExtensionSeq
                    where Seq=@Seq
                    ";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", schEngProgressHeaderSeq);
                    cmd.Parameters.AddWithValue("@SupDailyReportExtensionSeq", supDailyReportExtensionSeq);
                    db.ExecuteNonQuery(cmd);
                }*/
                
                //停工
                if (mode == _cyStopWork)
                {
                    sql = @"
                    update SupDailyReportWork set
                        SStopWorkDate = @SStopWorkDate
                    where Seq = @Seq
                    ";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", m.SupDailyReportWorkSeq);
                    cmd.Parameters.AddWithValue("@SStopWorkDate", m.StartDate);
                    db.ExecuteNonQuery(cmd);
                }
                //復工
                if (mode == _cyBackWork)
                {
                    sql = @"
                        update SupDailyReportWork set
                            BackWorkDate = @BackWorkDate
                        where Seq=@Seq
                    ";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", m.SupDailyReportWorkSeq);
                    cmd.Parameters.AddWithValue("@BackWorkDate", m.StartDate);
                    db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return ec_SchEngProgressHeaderSeq;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchEngChangeService.UpdateEngDate: " + e.Message);
                return -1;
            }
        }

        /// <summary>
        /// 新增工程變更 s20230523
        /// </summary>
        /// <param name="m">工程變更</param>
        /// <param name="extensionModel">延展工期</param>
        /// <returns></returns>
        public int AddEngChange(EPCProgressEngChangeListVModel m, SupDailyReportExtensionModel extensionModel, SupDailyReportWorkVModel workModel)
        {
            SqlCommand cmd;
            string sql;
            Null2Empty(m);
            int userSeq = getUserSeq();

            db.BeginTransaction();
            try
            {
                sql = @"
                    insert into EC_SchEngProgressHeader (
                        EngMainSeq,
                        Version,
                        StartDate,
                        SchCompDate,
                        ChangeType,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EngMainSeq,
                        @Version,
                        @StartDate,
                        @SchCompDate,
                        @ChangeType,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", m.Seq);
                cmd.Parameters.AddWithValue("@Version", m.Version);
                cmd.Parameters.AddWithValue("@StartDate", m.StartDate);
                cmd.Parameters.AddWithValue("@SchCompDate", m.SchCompDate);
                cmd.Parameters.AddWithValue("@ChangeType", m.ChangeType);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('EC_SchEngProgressHeader') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);
                int schEngProgressHeaderSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
                //
                sql = @"
                    update SchProgressHeader set
                        SPState=0,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where EngMainSeq=@EngMainSeq
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", m.Seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                db.ExecuteNonQuery(cmd);

                string scheChangeCloseDate = String.Format("{0}{1}", m.SchCompDate.Value.Year - 1911, m.SchCompDate.Value.ToString("MMdd"));
                sql = @"
                    update PrjXMLExt set
                        ScheChangeCloseDate=@ScheChangeCloseDate,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where PrjXMLSeq=(select PrjXMLSeq from EngMain where Seq=@EngMainSeq)
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", m.Seq);
                cmd.Parameters.AddWithValue("@ScheChangeCloseDate", scheChangeCloseDate);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                int result = db.ExecuteNonQuery(cmd);
                if(result==0)
                {
                    db.TransactionRollback();
                    return -2;
                }

                //延展工期
                if (extensionModel !=null)
                {
                    Null2Empty(extensionModel);
                    sql = @"
                    insert into SupDailyReportExtension(
                        EngMainSeq,
                        ExtendDays,
                        ApprovalNo,
                        ApprovalDate,
                        ExtendReason,
                        ExtendReasonOther,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EngMainSeq,
                        @ExtendDays,
                        @ApprovalNo,
                        @ApprovalDate,
                        @ExtendReason,
                        @ExtendReasonOther,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EngMainSeq", extensionModel.EngMainSeq);
                    cmd.Parameters.AddWithValue("@ExtendDays", extensionModel.ExtendDays);
                    cmd.Parameters.AddWithValue("@ApprovalNo", extensionModel.ApprovalNo);
                    cmd.Parameters.AddWithValue("@ApprovalDate", extensionModel.ApprovalDate);
                    cmd.Parameters.AddWithValue("@ExtendReason", extensionModel.ExtendReason);
                    cmd.Parameters.AddWithValue("@ExtendReasonOther", extensionModel.ExtendReasonOther);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);

                    cmd.Parameters.Clear();
                    sql1 = @"SELECT IDENT_CURRENT('SupDailyReportExtension') AS NewSeq";
                    cmd = db.GetCommand(sql1);
                    dt = db.GetDataTable(cmd);
                    int supDailyReportExtensionSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                    sql = @"
                    update EC_SchEngProgressHeader set
                        SupDailyReportExtensionSeq=@SupDailyReportExtensionSeq
                    where Seq=@Seq
                    ";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", schEngProgressHeaderSeq);
                    cmd.Parameters.AddWithValue("@SupDailyReportExtensionSeq", supDailyReportExtensionSeq);
                    db.ExecuteNonQuery(cmd);
                }
                
                //停工
                if(workModel != null && workModel.Seq == -1)
                {
                    Null2Empty(workModel);
                    sql = @"
                    insert into SupDailyReportWork(
                        EngMainSeq,
                        SStopWorkDate,
                        EStopWorkDate,
                        StopWorkReason,
                        StopWorkNo,
                        StopWorkApprovalFile,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EngMainSeq,
                        @SStopWorkDate,
                        @EStopWorkDate,
                        @StopWorkReason,
                        @StopWorkNo,
                        @StopWorkApprovalFile,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EngMainSeq", workModel.EngMainSeq);
                    cmd.Parameters.AddWithValue("@SStopWorkDate", workModel.SStopWorkDate);
                    cmd.Parameters.AddWithValue("@EStopWorkDate", workModel.EStopWorkDate);
                    cmd.Parameters.AddWithValue("@StopWorkReason", workModel.StopWorkReason);
                    cmd.Parameters.AddWithValue("@StopWorkNo", workModel.StopWorkNo);
                    cmd.Parameters.AddWithValue("@StopWorkApprovalFile", workModel.StopWorkApprovalFile);
                    //cmd.Parameters.AddWithValue("@BackWorkDate", this.NulltoDBNull(m.BackWorkDate));
                    //cmd.Parameters.AddWithValue("@BackWorkNo", m.BackWorkNo);
                    //cmd.Parameters.AddWithValue("@BackWorkApprovalFile", m.BackWorkApprovalFile);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);

                    cmd.Parameters.Clear();
                    sql1 = @"SELECT IDENT_CURRENT('SupDailyReportWork') AS NewSeq";
                    cmd = db.GetCommand(sql1);
                    dt = db.GetDataTable(cmd);
                    int supDailyReportWorkSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                    sql = @"
                    update EC_SchEngProgressHeader set
                        SupDailyReportWorkSeq=@SupDailyReportWorkSeq
                    where Seq=@Seq
                    ";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", schEngProgressHeaderSeq);
                    cmd.Parameters.AddWithValue("@SupDailyReportWorkSeq", supDailyReportWorkSeq);
                    db.ExecuteNonQuery(cmd);
                    
                }
                //復工
                if (workModel != null && workModel.Seq > 0)
                {
                    Null2Empty(workModel);
                    sql = @"
                        update SupDailyReportWork set
                            BackWorkDate = @BackWorkDate,
                            BackWorkNo = @BackWorkNo,
                            BackWorkApprovalFile = @BackWorkApprovalFile,
                            ModifyTime = GetDate(),
                            ModifyUserSeq = @ModifyUserSeq
                        where Seq=@Seq
                    ";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", workModel.Seq);
                    cmd.Parameters.AddWithValue("@BackWorkDate", workModel.BackWorkDate);
                    cmd.Parameters.AddWithValue("@BackWorkNo", workModel.BackWorkNo);
                    cmd.Parameters.AddWithValue("@BackWorkApprovalFile", workModel.BackWorkApprovalFile);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);

                    //s20231014
                    sql = @"
                    update EC_SchEngProgressHeader set
                        SupDailyReportWorkSeq=@SupDailyReportWorkSeq
                    where Seq=@Seq
                    ";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", schEngProgressHeaderSeq);
                    cmd.Parameters.AddWithValue("@SupDailyReportWorkSeq", workModel.Seq);
                    db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return schEngProgressHeaderSeq;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchEngChangeService.AddEngChange: " + e.Message);
                return -1;
            }
        }
        //工程變更履歷清單
        public List<T> GetEngChangeList<T>(int engMainSeq)
        {
            string sql = @"
                select
                    Seq,
                    EngMainSeq,
                    Version,
                    StartDate,
                    EndDate,
                    SchCompDate,
                    SPState,
                    ChangeType,
                    SupDailyReportExtensionSeq,
                    SupDailyReportWorkSeq,
                    ModifyTime
                from EC_SchEngProgressHeader
                where EngMainSeq=@EngMainSeq
                order by Version
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetEngChange<T>(int seq)
        {
            string sql = @"
                select
                    Seq,
                    EngMainSeq,
                    Version,
                    StartDate,
                    EndDate,
                    SchCompDate,
                    SPState,
                    ChangeType,
                    ModifyTime
                from EC_SchEngProgressHeader
                where Seq=@Seq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //確認 預定進度 狀態可否進行工變
        public bool CheckEngState(int engMainSeq)
        {
            string sql = @"
                select count(z.Seq) total from (
                  select Seq from SchProgressHeader where EngMainSeq=@EngMainSeq and SPState=1
                  union all
                  select top 1 Seq from SupDailyDate where EngMainSeq=@EngMainSeq and DataType=1
                ) z
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString()) == 2;
        }
        //清單總筆數
        public int GetListTotal(int ec_SchEngProgressHeaderSeq)
        {
            string sql = @"
                select
                    count(a.Seq) total
                from EC_SchEngProgressPayItem a
                where a.EC_SchEngProgressHeaderSeq=@EC_SchEngProgressHeaderSeq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EC_SchEngProgressHeaderSeq", ec_SchEngProgressHeaderSeq);

            DataTable dt = db.GetDataTable(cmd);
            int cnt = Convert.ToInt32(dt.Rows[0]["total"].ToString());

            return cnt;
        }
        //PayItem 清單
        public List<T> GetList<T>(int ec_SchEngProgressHeaderSeq, int pageRecordCount, int pageIndex)
        {
            string sql = @"
                select
                    a.Seq,
                    a.PayItem,
                    a.Description,
                    a.Unit,
                    a.Quantity,
                    a.Price,
                    a.Amount,
                    a.KgCo2e,
                    a.Memo,
                    a.OrderNo,
                    a.ItemMode,
                    a.RootSeq,
                    a.GreenFundingSeq,
                    a.GreenFundingMemo
                from EC_SchEngProgressPayItem a
                where a.EC_SchEngProgressHeaderSeq=@EC_SchEngProgressHeaderSeq
                Order by a.OrderNo,a.Seq
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY
             ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EC_SchEngProgressHeaderSeq", ec_SchEngProgressHeaderSeq);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetList<T>(int ec_SchEngProgressHeaderSeq)
        {//s20230527
            string sql = @"
                select
                    a.Seq,
                    a.PayItem,
                    a.Description,
                    a.Unit,
                    a.Quantity,
                    a.Price,
                    a.Amount,
                    a.Memo,
                    a.OrderNo,
                    a.ItemMode,
                    a.RootSeq,
                    a.ParentEC_SchEngProgressPayItemSeq,
                    a.ParentSchEngProgressPayItemSeq
                from EC_SchEngProgressPayItem a
                where a.EC_SchEngProgressHeaderSeq=@EC_SchEngProgressHeaderSeq
                Order by a.OrderNo,a.Seq
             ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EC_SchEngProgressHeaderSeq", ec_SchEngProgressHeaderSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //計算總碳量,金額
        public void CalCarbonTotal(int engMainSeq, ref decimal? Co2Total, ref decimal? Co2ItemTotal, ref decimal? GreenFunding)
        {
            string sql = @"
                SELECT
	                sum(z.GreenFunding) GreenFunding, sum(z.Co2Total) Co2Total, sum(z.Co2ItemTotal) Co2ItemTotal
                from (
                    select
	                    0 GreenFunding,
                        ROUND(sum(ISNULL(a.ItemKgCo2e,0)),0) Co2Total,
                        ROUND(sum(ISNULL(a.Quantity * a.Price, 0)), 0) Co2ItemTotal
                    from EC_SchEngProgressHeader b
                    inner join EC_SchEngProgressPayItem a on(a.EC_SchEngProgressHeaderSeq=b.Seq)
                    where b.EngMainSeq=@EngMainSeq
                    and a.KgCo2e is not null and a.ItemKgCo2e is not null

                    union all
                
                    select
                        ROUND(sum(ISNULL(a.Amount, 0)), 0) GreenFunding,
                        0 Co2Total,
                        0 Co2ItemTotal
                    from EC_SchEngProgressHeader b
                    inner join EC_SchEngProgressPayItem a on(a.EC_SchEngProgressHeaderSeq=b.Seq and a.GreenFundingSeq is not null)
                    where b.EngMainSeq=@EngMainSeq
                    -- s20230524取消 and a.KgCo2e is not null and a.ItemKgCo2e is not null
                ) z
            "; //s20230528
            /*string sql =
                @"select
                    ROUND(sum(ISNULL(a.ItemKgCo2e,0)),0) Co2Total,
                    ROUND(sum(ISNULL(a.Quantity * a.Price, 0)), 0) Co2ItemTotal
                from SchEngProgressHeader b
                inner join SchEngProgressPayItem a on(a.SchEngProgressHeaderSeq=b.Seq)
                where b.EngMainSeq=@EngMainSeq
                and a.KgCo2e is not null and a.ItemKgCo2e is not null
                --and (a.RStatusCode>50 and a.RStatusCode<200 or a.RStatusCode=201)
                ";*/
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            DataTable dt = db.GetDataTable(cmd);
            try
            {
                Co2Total = Convert.ToDecimal(dt.Rows[0]["Co2Total"].ToString());
            }
            catch { }
            try
            {
                Co2ItemTotal = Convert.ToDecimal(dt.Rows[0]["Co2ItemTotal"].ToString());
            }
            catch { }
            try
            {
                GreenFunding = Convert.ToDecimal(dt.Rows[0]["GreenFunding"].ToString());
            }
            catch { }
        }
        //初始化資料 從 前置作業
        public bool InitFromSchEngProgress(int ec_SchEngProgressHeaderSeq, List<SchEngProgressPayItem2Model> items)
        {
            SqlCommand cmd;
            string sql;
            int userSeq = getUserSeq();
            db.BeginTransaction();
            try
            {
                foreach (SchEngProgressPayItem2Model item in items)
                {
                    Null2Empty(item);
                    sql = @"
                        insert into EC_SchEngProgressPayItem (
                            EC_SchEngProgressHeaderSeq
                            ,PayItem,[Description],Unit,Quantity,Price,Amount,ItemKey,ItemNo,RefItemCode
                            ,RStatusCode,KgCo2e,ItemKgCo2e,Memo,RStatus,OrderNo,ParentSchEngProgressPayItemSeq
                            ,GreenFundingSeq, GreenFundingMemo
                            ,CreateTime,CreateUserSeq,ModifyTime,ModifyUserSeq
                        ) values (
                            @EC_SchEngProgressHeaderSeq
                            ,@PayItem,@Description,@Unit,@Quantity,@Price,@Amount,@ItemKey,@ItemNo,@RefItemCode
                            ,@RStatusCode,@KgCo2e,@ItemKgCo2e,@Memo,@RStatus,@OrderNo,@ParentSchEngProgressPayItemSeq
                            ,@GreenFundingSeq, @GreenFundingMemo
                            ,GetDate(),@ModifyUserSeq,GetDate(),@ModifyUserSeq
                        )
                        ";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EC_SchEngProgressHeaderSeq", ec_SchEngProgressHeaderSeq);
                    cmd.Parameters.AddWithValue("@PayItem", item.PayItem);
                    cmd.Parameters.AddWithValue("@Description", item.Description);
                    cmd.Parameters.AddWithValue("@Unit", item.Unit);
                    cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    cmd.Parameters.AddWithValue("@Price", item.Price);
                    cmd.Parameters.AddWithValue("@Amount", item.Amount);
                    cmd.Parameters.AddWithValue("@ItemKey", item.ItemKey);
                    cmd.Parameters.AddWithValue("@ItemNo", item.ItemNo);
                    cmd.Parameters.AddWithValue("@RefItemCode", item.RefItemCode);
                    cmd.Parameters.AddWithValue("@RStatusCode", item.RStatusCode);
                    cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(item.KgCo2e));
                    cmd.Parameters.AddWithValue("@ItemKgCo2e", this.NulltoDBNull(item.ItemKgCo2e));
                    cmd.Parameters.AddWithValue("@Memo", item.Memo);
                    cmd.Parameters.AddWithValue("@RStatus", item.RStatus);
                    cmd.Parameters.AddWithValue("@OrderNo", item.OrderNo);
                    cmd.Parameters.AddWithValue("@GreenFundingSeq", this.NulltoDBNull(item.GreenFundingSeq));
                    cmd.Parameters.AddWithValue("@GreenFundingMemo", item.GreenFundingMemo);
                    cmd.Parameters.AddWithValue("@ParentSchEngProgressPayItemSeq", item.Seq);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                    db.ExecuteNonQuery(cmd);

                    cmd.Parameters.Clear();
                    string sql1 = @" SELECT IDENT_CURRENT('EC_SchEngProgressPayItem') AS NewSeq";
                    cmd = db.GetCommand(sql1);
                    DataTable dt = db.GetDataTable(cmd);
                    int payItemSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                    sql = @"
				        update EC_SchEngProgressPayItem set
                            RootSeq=@Seq
                        where Seq=@Seq
                        ";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", payItemSeq);
                    db.ExecuteNonQuery(cmd);

                    sql = @"
				    INSERT INTO EC_SchEngProgressWorkItem (
                        EC_SchEngProgressPayItemSeq, WorkItemQuantity, ItemCode, ItemKind, Description, Unit, Quantity,
                        Price, Amount, Remark, LabourRatio, EquipmentRatio, MaterialRatio, MiscellaneaRatio,OrderNo,
                        CreateTime, CreateUserSeq, ModifyTime, ModifyUserSeq
                    )values(
                        @EC_SchEngProgressPayItemSeq, @WorkItemQuantity, @ItemCode, @ItemKind, @Description, @Unit, @Quantity,
                        @Price, @Amount, @Remark, @LabourRatio, @EquipmentRatio, @MaterialRatio, @MiscellaneaRatio,@OrderNo,
                        GetDate(), @ModifyUserSeq, GetDate(), @ModifyUserSeq
                    )";
                    foreach (SchEngProgressWorkItemModel wi in item.workItems)
                    {
                        Null2Empty(wi);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@EC_SchEngProgressPayItemSeq", payItemSeq);
                        cmd.Parameters.AddWithValue("@WorkItemQuantity", wi.WorkItemQuantity);
                        cmd.Parameters.AddWithValue("@ItemCode", wi.ItemCode);
                        cmd.Parameters.AddWithValue("@ItemKind", wi.ItemKind);
                        cmd.Parameters.AddWithValue("@Description", wi.Description);
                        cmd.Parameters.AddWithValue("@Unit", wi.Unit);
                        cmd.Parameters.AddWithValue("@Quantity", wi.Quantity);
                        cmd.Parameters.AddWithValue("@Price", wi.Price);
                        cmd.Parameters.AddWithValue("@Amount", wi.Amount);
                        cmd.Parameters.AddWithValue("@Remark", wi.Remark);
                        cmd.Parameters.AddWithValue("@LabourRatio", wi.LabourRatio);
                        cmd.Parameters.AddWithValue("@EquipmentRatio", wi.EquipmentRatio);
                        cmd.Parameters.AddWithValue("@MaterialRatio", wi.MaterialRatio);
                        cmd.Parameters.AddWithValue("@MiscellaneaRatio", wi.MiscellaneaRatio);
                        cmd.Parameters.AddWithValue("@OrderNo", wi.OrderNo);
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                        db.ExecuteNonQuery(cmd);
                    }
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchEngChangeService.InitFromSchEngProgress: " + e.Message);
                return false;
            }
        }
        //初始化資料 從 工程變更
        public bool InitFromEngChange(int ec_SchEngProgressHeaderSeq, List<EC_SchEngProgressPayItem2Model> items)
        {
            SqlCommand cmd;
            string sql;
            int userSeq = getUserSeq();
            db.BeginTransaction();
            try
            {
                foreach (EC_SchEngProgressPayItem2Model item in items)
                {
                    Null2Empty(item);
                    sql = @"
                        insert into EC_SchEngProgressPayItem (
                            EC_SchEngProgressHeaderSeq
                            ,PayItem,[Description],Unit,Quantity,Price,Amount,ItemKey,ItemNo,RefItemCode
                            ,RStatusCode,KgCo2e,ItemKgCo2e,Memo,RStatus,OrderNo,ParentSchEngProgressPayItemSeq
                            ,ParentEC_SchEngProgressPayItemSeq,RootSeq
                            ,GreenFundingSeq, GreenFundingMemo
                            ,CreateTime,CreateUserSeq,ModifyTime,ModifyUserSeq
                        ) values (
                            @EC_SchEngProgressHeaderSeq
                            ,@PayItem,@Description,@Unit,@Quantity,@Price,@Amount,@ItemKey,@ItemNo,@RefItemCode
                            ,@RStatusCode,@KgCo2e,@ItemKgCo2e,@Memo,@RStatus,@OrderNo,@ParentSchEngProgressPayItemSeq
                            ,@ParentEC_SchEngProgressPayItemSeq,@RootSeq
                            ,@GreenFundingSeq, @GreenFundingMemo
                            ,GetDate(),@ModifyUserSeq,GetDate(),@ModifyUserSeq
                        )
                        ";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EC_SchEngProgressHeaderSeq", ec_SchEngProgressHeaderSeq);
                    cmd.Parameters.AddWithValue("@PayItem", item.PayItem);
                    cmd.Parameters.AddWithValue("@Description", item.Description);
                    cmd.Parameters.AddWithValue("@Unit", item.Unit);
                    cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    cmd.Parameters.AddWithValue("@Price", item.Price);
                    cmd.Parameters.AddWithValue("@Amount", item.Amount);
                    cmd.Parameters.AddWithValue("@ItemKey", item.ItemKey);
                    cmd.Parameters.AddWithValue("@ItemNo", item.ItemNo);
                    cmd.Parameters.AddWithValue("@RefItemCode", item.RefItemCode);
                    cmd.Parameters.AddWithValue("@RStatusCode", item.RStatusCode);
                    cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(item.KgCo2e));
                    cmd.Parameters.AddWithValue("@ItemKgCo2e", this.NulltoDBNull(item.ItemKgCo2e));
                    cmd.Parameters.AddWithValue("@Memo", item.Memo);
                    cmd.Parameters.AddWithValue("@RStatus", item.RStatus);
                    cmd.Parameters.AddWithValue("@OrderNo", item.OrderNo);
                    cmd.Parameters.AddWithValue("@ParentSchEngProgressPayItemSeq", this.NulltoDBNull(item.ParentSchEngProgressPayItemSeq));
                    cmd.Parameters.AddWithValue("@ParentEC_SchEngProgressPayItemSeq", item.Seq);
                    cmd.Parameters.AddWithValue("@RootSeq", item.RootSeq);
                    cmd.Parameters.AddWithValue("@GreenFundingSeq", this.NulltoDBNull(item.GreenFundingSeq));
                    cmd.Parameters.AddWithValue("@GreenFundingMemo", item.GreenFundingMemo);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                    db.ExecuteNonQuery(cmd);

                    cmd.Parameters.Clear();
                    string sql1 = @" SELECT IDENT_CURRENT('EC_SchEngProgressPayItem') AS NewSeq";
                    cmd = db.GetCommand(sql1);
                    DataTable dt = db.GetDataTable(cmd);
                    int payItemSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                    sql = @"
				    INSERT INTO EC_SchEngProgressWorkItem (
                        EC_SchEngProgressPayItemSeq, WorkItemQuantity, ItemCode, ItemKind, Description, Unit, Quantity,
                        Price, Amount, Remark, LabourRatio, EquipmentRatio, MaterialRatio, MiscellaneaRatio,OrderNo,
                        CreateTime, CreateUserSeq, ModifyTime, ModifyUserSeq
                    )values(
                        @EC_SchEngProgressPayItemSeq, @WorkItemQuantity, @ItemCode, @ItemKind, @Description, @Unit, @Quantity,
                        @Price, @Amount, @Remark, @LabourRatio, @EquipmentRatio, @MaterialRatio, @MiscellaneaRatio,@OrderNo,
                        GetDate(), @ModifyUserSeq, GetDate(), @ModifyUserSeq
                    )";
                    foreach (EC_SchEngProgressWorkItemModel wi in item.workItems)
                    {
                        Null2Empty(wi);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@EC_SchEngProgressPayItemSeq", payItemSeq);
                        cmd.Parameters.AddWithValue("@WorkItemQuantity", wi.WorkItemQuantity);
                        cmd.Parameters.AddWithValue("@ItemCode", wi.ItemCode);
                        cmd.Parameters.AddWithValue("@ItemKind", wi.ItemKind);
                        cmd.Parameters.AddWithValue("@Description", wi.Description);
                        cmd.Parameters.AddWithValue("@Unit", wi.Unit);
                        cmd.Parameters.AddWithValue("@Quantity", wi.Quantity);
                        cmd.Parameters.AddWithValue("@Price", wi.Price);
                        cmd.Parameters.AddWithValue("@Amount", wi.Amount);
                        cmd.Parameters.AddWithValue("@Remark", wi.Remark);
                        cmd.Parameters.AddWithValue("@LabourRatio", wi.LabourRatio);
                        cmd.Parameters.AddWithValue("@EquipmentRatio", wi.EquipmentRatio);
                        cmd.Parameters.AddWithValue("@MaterialRatio", wi.MaterialRatio);
                        cmd.Parameters.AddWithValue("@MiscellaneaRatio", wi.MiscellaneaRatio);
                        cmd.Parameters.AddWithValue("@OrderNo", wi.OrderNo);
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                        db.ExecuteNonQuery(cmd);
                    }
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchEngChangeService.InitFromEngChange: " + e.Message);
                return false;
            }
        }
        //取得工程變更版本 PayItem 清單
        public List<T> GetListByVer<T>(int engMainSeq, int version)
        {
            string sql = @"
                select
                    a.Seq,
                    a.PayItem,
                    a.Description,
                    a.Unit,
                    a.Quantity,
                    a.Price,
                    a.Amount,
                    a.ItemKey,
                    a.ItemNo,
                    a.Memo,
                    a.OrderNo,
                    a.RefItemCode,
                    a.KgCo2e,
                    a.ItemKgCo2e,
                    a.RStatus,
                    a.RStatusCode,
                    a.ParentEC_SchEngProgressPayItemSeq,
                    a.ParentSchEngProgressPayItemSeq,
                    a.ItemMode,
                    a.RootSeq,
                    a.GreenFundingSeq,
                    a.GreenFundingMemo
                from EC_SchEngProgressHeader b
                inner join EC_SchEngProgressPayItem a on(a.EC_SchEngProgressHeaderSeq=b.Seq)
                where b.EngMainSeq=@EngMainSeq
                and b.Version=@Version
                Order by a.OrderNo,a.Seq
             ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@Version", version);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //取得所屬 WorkItem
        public void GetWorkItemListFromPayItem(List<EC_SchEngProgressPayItem2Model> items)
        {
            string sql = @"
                select
                    Seq,
                    EC_SchEngProgressPayItemSeq,
                    WorkItemQuantity,
                    ItemCode,
                    ItemKind,
                    Description,
                    Unit,
                    Quantity,
                    Price,
                    Amount,
                    Remark,
                    LabourRatio,
                    EquipmentRatio,
                    MaterialRatio,
                    MiscellaneaRatio,
                    OrderNo
                from EC_SchEngProgressWorkItem
                where EC_SchEngProgressPayItemSeq=@EC_SchEngProgressPayItemSeq
             ";
            SqlCommand cmd;
            foreach (EC_SchEngProgressPayItem2Model m in items)
            {
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EC_SchEngProgressPayItemSeq", m.Seq);
                m.workItems = db.GetDataTableWithClass<EC_SchEngProgressWorkItemModel>(cmd);
            }
        }
        public List<EC_SchEngProgressWorkItemModel> GetWorkItemList(int ec_SchEngProgressPayItemSeq)
        {
            string sql = @"
                select
                    Seq,
                    EC_SchEngProgressPayItemSeq,
                    WorkItemQuantity,
                    ItemCode,
                    ItemKind,
                    Description,
                    Unit,
                    Quantity,
                    Price,
                    Amount,
                    Remark,
                    LabourRatio,
                    EquipmentRatio,
                    MaterialRatio,
                    MiscellaneaRatio,
                    OrderNo
                from EC_SchEngProgressWorkItem
                where EC_SchEngProgressPayItemSeq=@EC_SchEngProgressPayItemSeq
             ";
            SqlCommand cmd;
            cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EC_SchEngProgressPayItemSeq", ec_SchEngProgressPayItemSeq);
            return db.GetDataTableWithClass<EC_SchEngProgressWorkItemModel>(cmd);
        }
        //更新 WorkItem
        public int UpdateWorkItem(EC_SchEngProgressWorkItemModel m)
        {
            Null2Empty(m);
            string sql;
            try
            {
                if(m.Seq == -1)
                {
                    sql = @"
                    insert into EC_SchEngProgressWorkItem (
                        EC_SchEngProgressPayItemSeq,
                        ItemCode,
                        Description,
                        Unit,
                        Quantity,
                        Price,
                        Amount,
                        Remark,
                        OrderNo,
                        ItemKind,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EC_SchEngProgressPayItemSeq,
                        @ItemCode,
                        @Description,
                        @Unit,
                        @Quantity,
                        @Price,
                        @Amount,
                        @Remark,
                        (select ISNULL(max(OrderNo),0)+1 from EC_SchEngProgressWorkItem where EC_SchEngProgressPayItemSeq=@EC_SchEngProgressPayItemSeq),
                        'customAdd',
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                } else
                {
                    sql = @"
                    update EC_SchEngProgressWorkItem set
                        ItemCode = @ItemCode,
                        Description = @Description,
                        Unit = @Unit,
                        Quantity = @Quantity,
                        Price = @Price,
                        Amount = @Amount,
                        Remark = @Remark,
                        ModifyTime = GetDate(),
                        ModifyUserSeq = @ModifyUserSeq
                    where Seq=@Seq
                    ";
                }

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@EC_SchEngProgressPayItemSeq", m.EC_SchEngProgressPayItemSeq);
                cmd.Parameters.AddWithValue("@ItemCode", m.ItemCode);
                cmd.Parameters.AddWithValue("@Description", m.Description);
                cmd.Parameters.AddWithValue("@Unit", m.Unit);
                cmd.Parameters.AddWithValue("@Quantity", m.Quantity);
                cmd.Parameters.AddWithValue("@Price", m.Price);
                cmd.Parameters.AddWithValue("@Amount", m.Amount);
                cmd.Parameters.AddWithValue("@Remark", m.Remark);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("SchEngProgressService.UpdateWorkItem: " + e.Message);
                //log.Info(sql);
                return -1;
            }
        }
        //刪除 PayItem
        public int DelWorkItem(int seq)
        {
            db.BeginTransaction();
            try
            {
                string sql = @"
                delete from EC_SchEngProgressWorkItem where Seq=@Seq;
                ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchEngProgressService.DelWorkItem: " + e.Message);
                return -1;
            }
        }

        //新增 PayItem
        public bool AddPayItem(int ec_SchEngProgressHeaderSeq, EC_SchEngProgressPayItemModel item)
        {
            try
            {
                Null2Empty(item);
                db.BeginTransaction();

                string sql = @"
                    insert into EC_SchEngProgressPayItem (
                        ItemMode,EC_SchEngProgressHeaderSeq
                        ,PayItem,[Description],Unit,Quantity,Price,Amount,OrderNo,Memo
                        ,CreateTime,CreateUserSeq,ModifyTime,ModifyUserSeq
                        ,KgCo2e,ItemKgCo2e
                    ) values (
                        1,@EC_SchEngProgressHeaderSeq
                        ,@PayItem,@Description,@Unit,@Quantity,@Price,@Quantity*@Price,@OrderNo,@Memo
                        ,GetDate(),@ModifyUserSeq,GetDate(),@ModifyUserSeq
                        ,@KgCo2e,@KgCo2e*@Quantity
                    )
                    ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EC_SchEngProgressHeaderSeq", ec_SchEngProgressHeaderSeq);
                cmd.Parameters.AddWithValue("@PayItem", item.PayItem);
                cmd.Parameters.AddWithValue("@Description", item.Description);
                cmd.Parameters.AddWithValue("@Unit", item.Unit); 
                cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(item.KgCo2e));
                cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                cmd.Parameters.AddWithValue("@Price", item.Price);
                cmd.Parameters.AddWithValue("@Memo", item.Memo);
                cmd.Parameters.AddWithValue("@OrderNo", item.OrderNo);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @" SELECT IDENT_CURRENT('EC_SchEngProgressPayItem') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);
                int payItemSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                sql = @"
				        update EC_SchEngProgressPayItem set
                            RootSeq=@Seq
                        where Seq=@Seq
                        ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", payItemSeq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchEngChangeService.AddPayItem: " + e.Message);
                return false;
            }
        }
        //更新 PayItem
        public int UpdatePayItem(EC_SchEngProgressPayItemModel m)
        {
            Null2Empty(m);
            try
            {//Amount 四捨五入 s20231017
                string sql = @"update EC_SchEngProgressPayItem set
                    PayItem=@PayItem,
                    [Description]=@Description,
                    Unit=@Unit,
                    Quantity=@Quantity,
                    Price=@Price,
                    Amount=round(@Quantity*@Price,0),
                    KgCo2e=@KgCo2e,
                    ItemKgCo2e=@KgCo2e*@Quantity,
                    ModifyTime=GetDate(),
                    ModifyUserSeq=@ModifyUserSeq
                where Seq=@Seq";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@PayItem", m.PayItem);
                cmd.Parameters.AddWithValue("@Description", m.Description);
                cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(m.KgCo2e));
                cmd.Parameters.AddWithValue("@Unit", m.Unit);
                cmd.Parameters.AddWithValue("@Quantity", m.Quantity);
                cmd.Parameters.AddWithValue("@Price", m.Price);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("SchEngProgressService.UpdatePayItem: " + e.Message);
                //log.Info(sql);
                return -1;
            }
        }
        //刪除 PayItem
        public int DelRecord(int seq)
        {
            db.BeginTransaction();
            try
            {
                string sql = @"
                delete from EC_SchEngProgressWorkItem where EC_SchEngProgressPayItemSeq=@Seq;

                delete from EC_SchEngProgressPayItem where Seq=@Seq;
                ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchEngProgressService.DelRecord: " + e.Message);
                return -1;
            }
        }
        //刪除 PayItems s20230530
        public int DelRecords(List<int> seqs)
        {
            if (seqs.Count == 0) return 0;
            string seqList = String.Join(",", seqs.ToArray());
            db.BeginTransaction();
            try
            {
                string sql = @"
                delete from EC_SchEngProgressWorkItem where EC_SchEngProgressPayItemSeq in ("+ seqList + @");

                delete from EC_SchEngProgressPayItem where Seq in ("+ seqList + @");
                ";
                SqlCommand cmd = db.GetCommand(sql);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchEngProgressService.DelRecord: " + e.Message);
                return -1;
            }
        }
    }
}