using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class SchProgressPayItemService : BaseService
    {//預定進度
        //更新單項進度 s2023070
        public bool UpdateProgress(int seq, SchProgressPayItemModel previousProgress, SchProgressPayItemModel editProgress)
        {
            SqlCommand cmd;
            string sql;
            if (seq == 0)
            {
                sql = @"
                    update SchProgressPayItem set
                        SchProgress=@SchProgress,
                        DayProgress=@DayProgress,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq
                 ";
            }
            else
            {
                sql = @"
                    update EC_SchProgressPayItem set
                        SchProgress=@SchProgress,
                        DayProgress=@DayProgress,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq
                 ";
            }
            db.BeginTransaction();
            try {
                cmd = db.GetCommand(sql);
                if (previousProgress != null)
                {
                    
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", previousProgress.Seq);
                    cmd.Parameters.AddWithValue("@SchProgress", previousProgress.SchProgress);
                    cmd.Parameters.AddWithValue("@DayProgress", previousProgress.DayProgress);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }
                if (editProgress != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", editProgress.Seq);
                    cmd.Parameters.AddWithValue("@SchProgress", editProgress.SchProgress);
                    cmd.Parameters.AddWithValue("@DayProgress", editProgress.DayProgress);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchProgressPayItemService.UpdateProgress: " + e.Message);
                return false;
            }
}
        //進度清單 s20230720
        public List<T> GetProgressList<T>(int engMainSeq, string tarDate, int seq)
        {
            string sql;
            if (seq == 0)
            {
                sql = @"
                    select
                        a.Seq,
                        a.SchEngProgressPayItemSeq,
                        a1.OrderNo,
                        a1.PayItem,
                        a1.Description,
                        a.SchProgress,
                        a.DayProgress,
                        a.Days,
                        a.DayProgressAfter
                    from SchProgressHeader b
                    inner join SchProgressPayItem a on(a.SchProgressHeaderSeq=b.Seq)
                    inner join SchEngProgressPayItem a1 on(a1.Seq=a.SchEngProgressPayItemSeq)
                    where b.EngMainSeq=@EngMainSeq
                    and a.SPDate=@SPDate
                    Order by a1.OrderNo
                 ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                cmd.Parameters.AddWithValue("@SPDate", tarDate);

                return db.GetDataTableWithClass<T>(cmd);
            }
            else
            {
                sql = @"
                    select
                        a.Seq,
                        a.EC_SchEngProgressPayItemSeq SchEngProgressPayItemSeq,
                        a1.OrderNo,
                        a1.PayItem,
                        a1.Description,
                        a.SchProgress,
                        a.DayProgress,
                        a.Days,
                        a.DayProgressAfter
                    from EC_SchEngProgressHeader b
                    inner join EC_SchEngProgressPayItem a1 on(a1.EC_SchEngProgressHeaderSeq=b.Seq)
                    inner join EC_SchProgressPayItem a on(a.EC_SchEngProgressPayItemSeq=a1.Seq and a.SPDate=@SPDate)
                    where b.Seq=@Version
                    Order by a1.OrderNo
                 ";
            
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                cmd.Parameters.AddWithValue("@SPDate", tarDate);
                cmd.Parameters.AddWithValue("@Version", seq);

                return db.GetDataTableWithClass<T>(cmd);
            }
        }

        //當日預定進度 s20230408
        public List<T> GetPayItemSchProgress<T>(int engMainSeq, DateTime tarDate)
        {
            string sql = @"
                select
                    z.SchProgressHeaderSeq
                    ,z.SPDate
                    ,IIF(z.DayProgress=-1,-1,z.SchProgress) SchProgress --預定進度
                    ,z.DayProgress
                    ,z.DayProgressAfter
                    ,z.Days
                    ,z.SchEngProgressPayItemSeq
                from (
                    select
                        c.OrderNo,
                        a.SchProgressHeaderSeq
                        ,@tarDate SPDate
                        ,(
                            a.SchProgress - a.DayProgress * DATEDIFF(Day, IIF(@tarDate>a.SPDate, a.SPDate, @tarDate), a.SPDate)
                        ) SchProgress --預定進度
                        ,a.DayProgress
                        ,a.DayProgressAfter
                        ,(
                            a.Days - DATEDIFF(Day, IIF(@tarDate>a.SPDate, a.SPDate, @tarDate), a.SPDate)
                        ) Days
                        ,a.SchEngProgressPayItemSeq
                    from SchProgressHeader b
                    inner join SchProgressPayItem a on(a.SchProgressHeaderSeq=b.Seq)
                    inner join SchEngProgressPayItem c on(c.Seq=a.SchEngProgressPayItemSeq)
                    where b.EngMainSeq=@engMainSeq
                    and a.SPDate = (
                        select top 1 z1.SPDate from (
                          select top 1 z.SPDate from SchProgressPayItem z
                          where z.SchProgressHeaderSeq = b.Seq
                          and z.SPDate >= @tarDate
                          order by z.SPDate
              
                          union all
              
                          select top 1 z.SPDate from SchProgressPayItem z
                          where z.SchProgressHeaderSeq = b.Seq
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
        //工程決標日期 shioulo20221216
        public List<T> GetEngMainBySeq<T>(int seq)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    ISNULL(a.StartDate, dbo.ChtDate2Date(e.ActualStartDate)) StartDate,
                    ISNULL(a.SchCompDate, dbo.ChtDate2Date(e.ScheCompletionDate)) SchCompDate,
                    a.StartDate srcStartDate,
                    a.SchCompDate srcSchCompDate,
                    a.EngChangeStartDate,
                    dbo.ChtDate2Date(b.ScheChangeCloseDate) ScheChangeCloseDate
                FROM EngMain a
                inner join PrjXML e on(e.Seq=a.PrjXMLSeq)
                left outer join PrjXMLExt b on(b.PrjXMLSeq=e.Seq)
                where a.Seq=@Seq
            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //更新工程決標日期 shioulo20221216
        public int UpdateEngDates(EPCSchProgressV1Model m)
        {
            Null2Empty(m);
            string sql = @"
                update EngMain set
                    StartDate = @StartDate,
                    SchCompDate = @SchCompDate,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq
            ";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@StartDate", this.NulltoDBNull(m.StartDate));
                cmd.Parameters.AddWithValue("@SchCompDate", this.NulltoDBNull(m.SchCompDate));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("SchProgressPayItemService.UpdateEngDates: " + e.Message);
                return -1;
            }
        }
        //更新工程變更日期 shioulo20221228
        public int UpdateEngChangeStartDate(EPCSchProgressV1Model m)
        {
            Null2Empty(m);
            string sql = @"
                update EngMain set
                    EngChangeStartDate = @EngChangeStartDate,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq
            ";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngChangeStartDate", this.NulltoDBNull(m.EngChangeStartDate));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("SchProgressPayItemService.UpdateEngChangeStartDate: " + e.Message);
                return -1;
            }
        }

        //刪除進度資料
        public bool DelProgress(int engMainSeq)
        {
            string sql = @"
                delete SchProgressPayItem where SchProgressHeaderSeq in (select Seq from SchProgressHeader where EngMainSeq=@EngMainSeq);
                    
                delete SchProgressHeader where EngMainSeq=@EngMainSeq;
                ";
            db.BeginTransaction();
            try { 
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                db.ExecuteNonQuery(cmd);

                //shioulo 20230217
                sql = @"
                Update EngMain set
                    StartDate=null,
                    SchCompDate=null
                where Seq=@EngMainSeq;
                ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchProgressPayItemService.DelProgress: " + e.Message);
                return false;
            }
        }
        //日誌已填寫數
        public List<T> GetSupDailyDateCount<T>(int engMainSeq)
        {
            string sql = @"
				SELECT
                    a.EngMainSeq,
                    count(a.Seq) dailyCount
				FROM SupDailyDate a
				WHERE a.EngMainSeq=@EngMainSeq
                group by a.EngMainSeq
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //工程變更進度清單調整
        public bool EngChangeAddItems(SchProgressHeaderModel spHeader, List<SchProgressPayItemModel> payItemList, List<DateTime> dateList, List<SchProgressHeaderHistoryProgressModel> engSchProgress)
        {
            string sql = "";
            db.BeginTransaction();
            try
            {
                sql = @"
                    delete AskPaymentPayItem where AskPaymentHeaderSeq in (select Seq from AskPaymentHeader where EngMainSeq=@EngMainSeq);

                    delete AskPaymentHeader where EngMainSeq=@EngMainSeq;
 
                    delete SchProgressPayItem
                    where SchProgressHeaderSeq=@SchProgressHeaderSeq
                    and SPDate>@SPDate;
                    ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", spHeader.EngMainSeq);
                cmd.Parameters.AddWithValue("@SchProgressHeaderSeq", spHeader.Seq);
                cmd.Parameters.AddWithValue("@SPDate", spHeader.EngChangeStartDate.Value);
                db.ExecuteNonQuery(cmd);

                sql = @"
                    Update SchProgressHeader set
                        SPState=0,
                        EngChangeState=1,
                        EngChangeCount = EngChangeCount+1,
                        EngChangeStartDate=@EngChangeStartDate,
                        EngChangeSchCompDate=@EngChangeSchCompDate,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", spHeader.Seq);
                cmd.Parameters.AddWithValue("@EngChangeStartDate", spHeader.EngChangeStartDate.Value);
                cmd.Parameters.AddWithValue("@EngChangeSchCompDate", spHeader.EngChangeSchCompDate.Value);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                sql = @"insert into SchProgressPayItem(
                        SchProgressHeaderSeq,
                        SchEngProgressPayItemSeq,
                        SPDate,
                        /*PayItem,
                        Description,
                        Unit,
                        Quantity,
                        Price,
                        Amount,
                        ItemKey,
                        ItemNo,
                        RefItemCode,
                        OrderNo,*/
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    )values(
                        @SchProgressHeaderSeq,
                        @SchEngProgressPayItemSeq,
                        @SPDate,
                        /*@PayItem,
                        @Description,
                        @Unit,
                        @Quantity,
                        @Price,
                        @Amount,
                        @ItemKey,
                        @ItemNo,
                        @RefItemCode,
                        @OrderNo,*/
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                foreach (DateTime dt in dateList)
                {
                    int orderNo = 1;
                    foreach (SchProgressPayItemModel m in payItemList)
                    {
                        Null2Empty(m);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@SchProgressHeaderSeq", spHeader.Seq);
                        cmd.Parameters.AddWithValue("@SchEngProgressPayItemSeq", m.SchEngProgressPayItemSeq);
                        cmd.Parameters.AddWithValue("@SPDate", dt);
                        /*cmd.Parameters.AddWithValue("@PayItem", m.PayItem);
                        cmd.Parameters.AddWithValue("@Description", m.Description);
                        cmd.Parameters.AddWithValue("@Unit", m.Unit);
                        cmd.Parameters.AddWithValue("@Quantity", this.NulltoDBNull(m.Quantity));
                        cmd.Parameters.AddWithValue("@Price", this.NulltoDBNull(m.Price));
                        cmd.Parameters.AddWithValue("@Amount", this.NulltoDBNull(m.Amount));
                        cmd.Parameters.AddWithValue("@ItemKey", m.ItemKey);
                        cmd.Parameters.AddWithValue("@ItemNo", m.ItemNo);
                        cmd.Parameters.AddWithValue("@RefItemCode", m.RefItemCode);
                        cmd.Parameters.AddWithValue("@OrderNo", m.OrderNo);*/
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                        db.ExecuteNonQuery(cmd);
                    }
                }

                if(engSchProgress.Count>0)
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
                    cmd.Parameters.AddWithValue("@EngChangeCount", spHeader.EngChangeCount+1);
                    cmd.Parameters.AddWithValue("@EngChangeStartDate", spHeader.EngChangeStartDate.Value);
                    cmd.Parameters.AddWithValue("@EngChangeSchCompDate", spHeader.EngChangeSchCompDate.Value);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
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
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchProgressPayItemService.EngChangeAddItems: " + e.Message);
                return false;
            }
        }
        //工程變更日期
        public List<T> GetEngChangeDate<T>(int engMainSeq)
        {
            string sql = @"
                select
                    a.EngChangeStartDate,
                    dbo.ChtDate2Date(b.ScheChangeCloseDate) ScheChangeCloseDate
                FROM EngMain a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.PrjXMLSeq)
                where a.Seq=@EngMainSeq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //最後一天日否 100% s20230227
        public bool CheckLastProgress(int engMainSeq)
        {
            string sql = @"
                select b.Seq from SchProgressHeader b
                inner join SchProgressPayItem a on(a.SchProgressHeaderSeq=b.Seq and a.SchProgress>-1
                    and a.SPDate=(select max(z.SPDate) from SchProgressPayItem z where z.SchProgressHeaderSeq = b.Seq)
                    and a.SchProgress<100
                )
                where b.EngMainSeq=@EngMainSeq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            DataTable dt = db.GetDataTable(cmd);
            return (dt.Rows.Count == 0);
        }
        //最後一天日否 100% s20230412
        public bool CheckLastProgressForEngChange(int seq)
        {
            string sql = @"
                select a.Seq from EC_SchEngProgressHeader b
                inner join EC_SchEngProgressPayItem b1 on(b1.EC_SchEngProgressHeaderSeq=b.Seq)
                inner join EC_SchProgressPayItem a on(a.EC_SchEngProgressPayItemSeq=b1.Seq and a.SchProgress>-1
                    and a.SPDate=(
    	                select max(z.SPDate) from EC_SchProgressPayItem z
                        inner join EC_SchEngProgressPayItem z1 on(z1.Seq=z.EC_SchEngProgressPayItemSeq)
                        where z1.EC_SchEngProgressHeaderSeq = b.Seq
                    )
                    and a.SchProgress<100
                )
                where b.Seq=@Seq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            DataTable dt = db.GetDataTable(cmd);
            return (dt.Rows.Count == 0);
        }
        //預定進度狀態
        public int SetState(int engMainSeq, int state, int engChangeState)
        {
            string sql = @"
                update SchProgressHeader set
                SPState=@SPState,
                EngChangeState=@EngChangeState
                where EngMainSeq=@EngMainSeq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@SPState", state);
            cmd.Parameters.AddWithValue("@EngChangeState", engChangeState);

            return db.ExecuteNonQuery(cmd);
        }
        //預定進度日期清單
        public List<T> GetDateList<T>(int engMainSeq)
        {
            string sql = @"
                    select Version, SPDate from (
                        select DISTINCT 0 Version, a.SPDate
                        from SchProgressHeader b
                        inner join SchProgressPayItem a on(a.SchProgressHeaderSeq=b.Seq)
                        where b.EngMainSeq=@EngMainSeq
                    ) z                            
                    Order by z.SPDate 
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //預定進度日期清單含工程變更 s20230411
        public List<T> GetDateListVer<T>(int engMainSeq)
        {
            string sql = @"
                    select Version, SPDate from (
                        select DISTINCT 0 Version, a.SPDate
                        from SchProgressHeader b
                        inner join SchProgressPayItem a on(a.SchProgressHeaderSeq=b.Seq)
                        where b.EngMainSeq=@EngMainSeq
                
                        union all
                        --s20230411
                        select DISTINCT b.Seq Version, a.SPDate
                        from EC_SchEngProgressHeader b
                        inner join EC_SchEngProgressPayItem c on(c.EC_SchEngProgressHeaderSeq=b.Seq)
                        inner join EC_SchProgressPayItem a on(a.EC_SchEngProgressPayItemSeq=c.Seq)
                        where b.EngMainSeq=@EngMainSeq
                    ) z                            
                    Order by z.SPDate 
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //建立初始進度清單
        public bool AddItems(int engMainSeq, List<SchEngProgressPayItemModel> payItemList, List<DateTime> dateList)
        {
            string sql;
            db.BeginTransaction();
            try
            {
                sql = @"
                    insert into SchProgressHeader (
                        EngMainSeq,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EngMainSeq,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('SchProgressHeader') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dTable = db.GetDataTable(cmd);
                int schProgressHeaderSeq = Convert.ToInt32(dTable.Rows[0]["NewSeq"].ToString());

                sql = @"insert into SchProgressPayItem(
                    SchProgressHeaderSeq,
                    SchEngProgressPayItemSeq,
                    SPDate,
                    /*PayItem,
                    Description,
                    Unit,
                    Quantity,
                    Price,
                    Amount,
                    ItemKey,
                    ItemNo,
                    RefItemCode,
                    OrderNo,*/
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                    )values(
                    @SchProgressHeaderSeq,
                    @SchEngProgressPayItemSeq,
                    @SPDate,
                    /*@PayItem,
                    @Description,
                    @Unit,
                    @Quantity,
                    @Price,
                    @Amount,
                    @ItemKey,
                    @ItemNo,
                    @RefItemCode,
                    @OrderNo,*/
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                )";
                foreach (DateTime dt in dateList)
                {
                    int orderNo = 1;
                    foreach (SchEngProgressPayItemModel m in payItemList)
                    {
                        Null2Empty(m);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@SchProgressHeaderSeq", schProgressHeaderSeq);
                        cmd.Parameters.AddWithValue("@SchEngProgressPayItemSeq", m.Seq);
                        cmd.Parameters.AddWithValue("@SPDate", dt);
                        /*cmd.Parameters.AddWithValue("@PayItem", m.PayItem);
                        cmd.Parameters.AddWithValue("@Description", m.Description);
                        cmd.Parameters.AddWithValue("@Unit", m.Unit);
                        cmd.Parameters.AddWithValue("@Quantity", this.NulltoDBNull(m.Quantity));
                        cmd.Parameters.AddWithValue("@Price", this.NulltoDBNull(m.Price));
                        cmd.Parameters.AddWithValue("@Amount", this.NulltoDBNull(m.Amount));
                        cmd.Parameters.AddWithValue("@ItemKey", m.ItemKey);
                        cmd.Parameters.AddWithValue("@ItemNo", m.ItemNo);
                        cmd.Parameters.AddWithValue("@RefItemCode", m.RefItemCode);
                        cmd.Parameters.AddWithValue("@OrderNo", orderNo);*/
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                        db.ExecuteNonQuery(cmd);

                        orderNo++;
                    }
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchProgressPayItemService.AddItems: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }
        //清單
        public List<T> GetList<T>(int engMainSeq, string tarDate)
        {
            string sql = @"
                select
                    a.Seq,
                    a.SchEngProgressPayItemSeq,
                    a1.OrderNo,
                    a1.PayItem,
                    a1.Description,
                    a1.Unit,
                    a1.Quantity,
                    a1.Price,
                    a1.Amount,
                    --a1.ItemKey,
                    --a1.ItemNo,
                    --a1.RefItemCode,
                    a.SchProgress,
                    a.DayProgress
                from SchProgressHeader b
                inner join SchProgressPayItem a on(a.SchProgressHeaderSeq=b.Seq)
                inner join SchEngProgressPayItem a1 on(a1.Seq=a.SchEngProgressPayItemSeq)
                where b.EngMainSeq=@EngMainSeq
                and a.SPDate=@SPDate
                Order by a1.OrderNo
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@SPDate", tarDate);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //清單含工程變更 s20230411
        public List<T> GetListVer<T>(int engMainSeq, string tarDate, int seq)
        {
            string sql;
            if (seq == 0)
            {
                return GetList<T>(engMainSeq, tarDate);
            } else
            {
                sql = @"
                    select
                        a.Seq,
                        a.EC_SchEngProgressPayItemSeq SchEngProgressPayItemSeq,
                        a1.OrderNo,
                        a1.PayItem,
                        a1.Description,
                        a1.Unit,
                        a1.Quantity,
                        a1.Price,
                        a1.Amount,
                        --a1.ItemKey,
                        --a1.ItemNo,
                        --a1.RefItemCode,
                        a.SchProgress,
                        a.DayProgress
                    from EC_SchEngProgressHeader b
                    inner join EC_SchEngProgressPayItem a1 on(a1.EC_SchEngProgressHeaderSeq=b.Seq)
                    inner join EC_SchProgressPayItem a on(a.EC_SchEngProgressPayItemSeq=a1.Seq and a.SPDate=@SPDate)
                    where b.Seq=@Version
                    Order by a1.OrderNo
                 ";
            }
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@SPDate", tarDate);
            cmd.Parameters.AddWithValue("@Version", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetListMinDate<T>(int engMainSeq)
        {
            string sql = @"
                select
                    a.Seq,
                    --a.SchProgressHeaderSeq,
                    a.SchEngProgressPayItemSeq,
                    a1.OrderNo,
                    a1.PayItem,
                    a1.Description,
                    a1.Unit,
                    a1.Quantity,
                    a1.Price,
                    a1.Amount,
                    a1.ItemKey,
                    a1.ItemNo,
                    a1.RefItemCode,
                    a.SchProgress,
                    a.DayProgress
                from SchProgressHeader b
                inner join SchProgressPayItem a on(a.SchProgressHeaderSeq=b.Seq)
                inner join SchEngProgressPayItem a1 on(a1.Seq=a.SchEngProgressPayItemSeq) 
                where b.EngMainSeq=@EngMainSeq
                and a.SPDate=(
                	select min(SPDate) from SchProgressPayItem where SchProgressHeaderSeq=(
                    	select Seq from SchProgressHeader where EngMainSeq=@EngMainSeq
                    )
                )
                Order by a1.OrderNo
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetHeaderList<T>(int engMainSeq)
        {
            string sql = @"
                select
                    Seq,
                    EngMainSeq,
                    SPState,
                    PccesXMLFile,
                    PccesXMLDate,
                    EngChangeState,
                    EngChangeStartDate,
                    EngChangeSchCompDate,
                    EngChangeCount
                from SchProgressHeader
                where EngMainSeq=@EngMainSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        /*public List<T> GetHeaderList1<T>(int engMainSeq)
        {
            string sql = @"
                select
                    Seq,
                    EngMainSeq,
                    SPState,
                    EngChangeState,
                    EngChangeStartDate,
                    EngChangeSchCompDate,
                    EngChangeCount
                from SchProgressHeader
                where EngMainSeq=@EngMainSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }*/
        //Payitem 清單
        public List<T> GetPayitemList<T>(int engMainSeq)
        {
            string sql = @"
                select DISTINCT
                    a1.OrderNo,
                    a1.PayItem,
                    a1.Description,
                    a1.Unit,
                    a1.RefItemCode
                from SchProgressHeader b
                inner join SchProgressPayItem a on(a.SchProgressHeaderSeq=b.Seq)
                inner join SchEngProgressPayItem a1 on(a1.Seq=a.SchEngProgressPayItemSeq) 
                where b.EngMainSeq=@EngMainSeq
                Order by a1.OrderNo
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //更新 預定進度
        public bool UpdateSchProgress(List<SchProgressPayItemModel> items)
        {
            SqlCommand cmd;

            string sql = @"
                    update SchProgressPayItem set
                        SchProgress=@SchProgress,
                        DayProgress=@DayProgress,
                        DayProgressAfter=@DayProgressAfter,
                        Days=@Days,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq
                    ";
            db.BeginTransaction();
            try
            {
                foreach (SchProgressPayItemModel item in items)
                {
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", item.Seq);
                    cmd.Parameters.AddWithValue("@SchProgress", item.SchProgress);
                    cmd.Parameters.AddWithValue("@DayProgress", item.DayProgress);
                    cmd.Parameters.AddWithValue("@DayProgressAfter", item.DayProgressAfter);
                    cmd.Parameters.AddWithValue("@Days", item.Days);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchProgressPayItemService.UpdateSchProgress: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }
        //更新 工程變更-預定進度 s20230411
        public bool UpdateSchProgressForEngChange(List<EC_SchProgressPayItemModel> items)
        {
            SqlCommand cmd;

            string sql = @"
                    update EC_SchProgressPayItem set
                        SchProgress=@SchProgress,
                        DayProgress=@DayProgress,
                        DayProgressAfter=@DayProgressAfter,
                        Days=@Days,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq
                    ";
            db.BeginTransaction();
            try
            {
                foreach (EC_SchProgressPayItemModel item in items)
                {
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", item.Seq);
                    cmd.Parameters.AddWithValue("@SchProgress", item.SchProgress);
                    cmd.Parameters.AddWithValue("@DayProgress", item.DayProgress);
                    cmd.Parameters.AddWithValue("@DayProgressAfter", item.DayProgressAfter);
                    cmd.Parameters.AddWithValue("@Days", item.Days);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchProgressPayItemService.UpdateSchProgressForEngChange: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }

        //更新 PCCES.xml
        public int UpdatePCCES(List<SchProgressPayItemModel> payItems, SchProgressHeaderModel headerModel, ref string errMsg)
        {
            int result = 0;
            SqlCommand cmd;

            string sql;
            db.BeginTransaction();
            try
            {
                sql = @"update SchProgressHeader set
                            PccesXMLFile=@PccesXMLFile,
                            PccesXMLDate=GetDate(),
                            ModifyTime=GetDate(),
                            ModifyUserSeq=@ModifyUserSeq
                        where Seq=@Seq;
                      ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", headerModel.Seq);
                cmd.Parameters.AddWithValue("@PccesXMLFile", headerModel.PccesXMLFile);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                db.ExecuteNonQuery(cmd);

                sql = @"
                    update SchProgressPayItem set
                        Price=@Price,
                        Quantity=@Quantity,
                        Amount=@Amount,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where SchProgressHeaderSeq=@SchProgressHeaderSeq
                    and OrderNo=@OrderNo;
                    ";
                if (headerModel.EngChangeState == 1)
                {//工程變更 20220903
                    sql += @"

                        update SupPlanOverview set
                            Price=@Price,
                            Quantity=@Quantity,
                            Amount=@Amount,
                            ModifyTime=GetDate(),
                            ModifyUserSeq=@ModifyUserSeq
                        where SupDailyDateSeq in (
                            select Seq from SupDailyDate where EngMainSeq=@EngMainSeq
                        )
                        and OrderNo=@OrderNo;
                      ";
                }
                foreach (SchProgressPayItemModel item in payItems)
                {
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@SchProgressHeaderSeq", headerModel.Seq);
                    cmd.Parameters.AddWithValue("@OrderNo", item.OrderNo);
                    cmd.Parameters.AddWithValue("@Price", item.Price);
                    cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    cmd.Parameters.AddWithValue("@Amount", item.Amount);
                    cmd.Parameters.AddWithValue("@EngMainSeq", headerModel.EngMainSeq);

                    cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());

                    result = db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return 1;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                errMsg = e.Message;
                log.Info("SchProgressPayItemService.UpdatePCCES: " + e.Message);
                //log.Info(sql);
                return -3;
            }
        }
    }
}