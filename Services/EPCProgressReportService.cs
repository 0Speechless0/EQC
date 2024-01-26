using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EPCProgressReportService : BaseService
    {//工程管理-進度報表
        //工程完成進度
        public List<T> GetComProgress<T>(int engMainSeq, DateTime startDate)
        {
            string sql = @"
                select  
	                z.ItemDate itemDate
                    , cast(sum(z.Price * 100 * z.TodayConfirm)/sum(z.Amount) as decimal(6,2)) rate
                from (
                    select
                        a.ItemDate
                        ,b1.Price, b.TodayConfirm, b1.Amount
                    from SupDailyDate a
                    inner join SupPlanOverview b on(b.SupDailyDateSeq=a.Seq and b.DayProgress>-1)
                    inner join SchEngProgressPayItem b1 on(b1.Seq=b.SchEngProgressPayItemSeq)
                    where a.EngMainSeq=@EngMainSeq
                    and a.DataType=1
                    and a.ItemDate>=@ItemDate

                    union all

                    select
                        a.ItemDate
                        ,b1.Price, b.TodayConfirm, b1.Amount
                    from EC_SupDailyDate a
                    inner join EC_SupPlanOverview b on(b.EC_SupDailyDateSeq=a.Seq and b.DayProgress>-1)
                    inner join EC_SchEngProgressPayItem b1 on(b1.Seq=b.EC_SchEngProgressPayItemSeq)
                    where a.EngMainSeq=@EngMainSeq
                    and a.DataType=1
                    and a.ItemDate>=@ItemDate
                ) z
                GROUP by z.ItemDate
                order by z.ItemDate             
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", startDate);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //工程預定進度
        public List<T> GetSchProgress<T>(int engMainSeq)
        {
            string sql = @"
                select  
	                z.SPDate itemDate
	                , cast(sum(z.Amount * z.SchProgress)/sum(z.Amount) as decimal(6,2)) rate
                from (
                    select
                        b.SPDate, b1.Amount, b.SchProgress
                    from SchProgressHeader a
                    inner join SchProgressPayItem b on(b.SchProgressHeaderSeq=a.Seq and b.SchProgress>-1)
                    inner join SchEngProgressPayItem b1 on(b1.Seq=b.SchEngProgressPayItemSeq)
                    where a.EngMainSeq=@EngMainSeq

                    union all
                    --s20230426
                    select
                        b.SPDate ,b1.Amount,b.SchProgress
                    from EC_SchEngProgressHeader a
                    inner join EC_SchEngProgressPayItem b1 on(b1.EC_SchEngProgressHeaderSeq=a.Seq)
                    inner join EC_SchProgressPayItem b on(b.EC_SchEngProgressPayItemSeq=b1.Seq and b.SchProgress>-1)
                    where a.EngMainSeq=@EngMainSeq
                ) z
                GROUP by z.SPDate
                order by z.SPDate              
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //初始工程預定進度
        public List<T> GetIinitSchProgress<T>(int engMainSeq)
        {
            string sql = @"
                select
	                c.ProgressDate,
                    c.SchProgress
                from SchProgressHeader b
                inner join SchProgressHeaderHistory a on(a.SchProgressHeaderSeq=b.Seq and a.EngChangeCount=1)
                inner join SchProgressHeaderHistoryProgress c on(c.SchProgressHeaderHistorySeq=a.Seq)
                where b.EngMainSeq=@EngMainSeq
                order by c.ProgressDate            
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //人員
        public List<T> GetChart3Person<T>(int engMainSeq, DateTime startDate)
        {
            string sql = @"
                select * from (
                    select
                        a.ItemDate, b.KindName itemName, b.TodayQuantity quantity
                    from SupDailyDate a
                    inner join SupDailyReportConstructionPerson b on(b.SupDailyDateSeq=a.Seq)
                    where a.EngMainSeq=@EngMainSeq
                    and a.DataType=2
                    and a.ItemDate>=@ItemDate

                    union all
                    --s20230426
                    select
                        a.ItemDate, b.KindName itemName, b.TodayQuantity quantity
                    from EC_SupDailyDate a
                    inner join EC_SupDailyReportConstructionPerson b on(b.EC_SupDailyDateSeq=a.Seq)
                    where a.EngMainSeq=@EngMainSeq
                    and a.DataType=2
                    and a.ItemDate>=@ItemDate
                ) z
                order by z.itemName,z.ItemDate               
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", startDate);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //機具
        public List<T> GetChart3Equipment<T>(int engMainSeq, DateTime startDate)
        {
            string sql = @"
                select * from (
                    select
                        a.ItemDate, (b.EquipmentName+'/'+b.EquipmentModel) itemName, b.TodayQuantity quantity
                    from SupDailyDate a
                    inner join SupDailyReportConstructionEquipment b on(b.SupDailyDateSeq=a.Seq)
                    where a.EngMainSeq=@EngMainSeq
                    and a.DataType=2
                    and a.ItemDate>=@ItemDate

                    union all
                    --s20230426
                    select
                        a.ItemDate, (b.EquipmentName+'/'+b.EquipmentModel) itemName, b.TodayQuantity quantity
                    from EC_SupDailyDate a
                    inner join EC_SupDailyReportConstructionEquipment b on(b.EC_SupDailyDateSeq=a.Seq)
                    where a.EngMainSeq=@EngMainSeq
                    and a.DataType=2
                    and a.ItemDate>=@ItemDate
                ) z
                order by z.itemName,z.ItemDate               
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", startDate);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //機具
        public List<T> GetChart4Equipment<T>(int engMainSeq, DateTime startDate)
        {
            string sql = @"
                select * from (
                    select
                        a.ItemDate, (b.EquipmentName+'/'+b.EquipmentModel) itemName, (b.KgCo2e*b.TodayHours) quantity
                    from SupDailyDate a
                    inner join SupDailyReportConstructionEquipment b on(b.SupDailyDateSeq=a.Seq)
                    where a.EngMainSeq=@EngMainSeq
                    and a.DataType=2
                    and a.ItemDate>=@ItemDate

                    union all
                    --s20230426
                    select
                        a.ItemDate, (b.EquipmentName+'/'+b.EquipmentModel) itemName, (b.KgCo2e*b.TodayHours) quantity
                    from EC_SupDailyDate a
                    inner join EC_SupDailyReportConstructionEquipment b on(b.EC_SupDailyDateSeq=a.Seq)
                    where a.EngMainSeq=@EngMainSeq
                    and a.DataType=2
                    and a.ItemDate>=@ItemDate
                ) z
                order by z.itemName,z.ItemDate               
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", startDate);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //細部工程預定進度 起訖日期
        public List<T> GetChart2<T>(int engMainSeq)
        {
            /*string sql = @"
                SELECT
	                max(z.minDate) minDate, max(z.maxDate) maxDate, z.OrderNo, z.Description
                from (
                    select
                        min(a.SPDate) minDate, null maxDate, a1.OrderNo, a1.Description
                    from SchProgressHeader b
                    inner join SchProgressPayItem a on(a.SchProgressHeaderSeq=b.Seq and a.SchProgress>0)
                    inner join SchEngProgressPayItem a1 on(a1.Seq=a.SchEngProgressPayItemSeq)
                    where b.EngMainSeq=@EngMainSeq
                    group by a1.OrderNo, a1.Description

                    union all

                    select
                        null minDate, min(a.SPDate) maxDate, a1.OrderNo, a1.Description
                    from SchProgressHeader b
                    inner join SchProgressPayItem a on(a.SchProgressHeaderSeq=b.Seq and a.SchProgress=100)
                    inner join SchEngProgressPayItem a1 on(a1.Seq=a.SchEngProgressPayItemSeq)
                    where b.EngMainSeq=@EngMainSeq
                    group by a1.OrderNo, a1.Description
                ) z
                group by z.OrderNo, z.Description
                order by z.OrderNo
                ";*/
            //s20230426 
            string sql = @"
                DECLARE @tmp_SchProgressPayItem table (SPDate DATE, SchProgress decimal(18, 4), Seq int, Description NVARCHAR(200))

                INSERT INTO @tmp_SchProgressPayItem(SPDate , SchProgress, Seq, Description)
                select
	                z.SPDate, z.SchProgress, z.Seq, z.Description
                from (
                    select
                        a.SPDate, a1.Seq, a.SchProgress, a1.Description
                    from SchProgressHeader b
                    inner join SchProgressPayItem a on(a.SchProgressHeaderSeq=b.Seq and a.SchProgress>-1)
                    inner join SchEngProgressPayItem a1 on(a1.Seq=a.SchEngProgressPayItemSeq)
                    where b.EngMainSeq=@EngMainSeq
                    or a1.Seq in(
                        select
                            a1.ParentSchEngProgressPayItemSeq
                        from EC_SchEngProgressHeader b
                        inner join EC_SchEngProgressPayItem a1 on(a1.EC_SchEngProgressHeaderSeq=b.Seq)
                        inner join EC_SchProgressPayItem a on(a.EC_SchEngProgressPayItemSeq=a1.Seq and a.SchProgress>-1)
                        where b.EngMainSeq=@EngMainSeq
                    )
                    union all
                    select
                        a.SPDate, ISNULL(a1.ParentSchEngProgressPayItemSeq, a1.RootSeq) Seq, a.SchProgress, a1.Description
                    from EC_SchEngProgressHeader b
                    inner join EC_SchEngProgressPayItem a1 on(a1.EC_SchEngProgressHeaderSeq=b.Seq)
                    inner join EC_SchProgressPayItem a on(a.EC_SchEngProgressPayItemSeq=a1.Seq and a.SchProgress>-1)
                    where b.EngMainSeq=@EngMainSeq
                ) z
                order by z.SPDate, z.Description;

                select
	                max(z.minDate) minDate, max(z.maxDate) maxDate, z.Seq OrderNo, z.Description
                from (
                    select
                        max(a.SPDate) minDate, null maxDate, a.Seq, a.Description
                    from @tmp_SchProgressPayItem a
                    inner join (
                        select
                            Seq, min(SchProgress) SchProgress
                        from @tmp_SchProgressPayItem
                        group by Seq
                    ) b on (b.Seq=a.Seq and b. SchProgress=a.SchProgress)
                    group by a.Seq, a.Description

                    union all

                    select
                        null minDate, min(a.SPDate) maxDate, a.Seq, a.Description
                    from @tmp_SchProgressPayItem a
                    inner join (
                        select
                            Seq, max(SchProgress) SchProgress
                        from @tmp_SchProgressPayItem
                        group by Seq
                    ) b on (b.Seq=a.Seq and b. SchProgress=a.SchProgress)
                    group by a.Seq, a.Description
                ) z
                group by z.Description, z.Seq 
                order by z.Description
            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //細部工程 預定進度/實際進度 s20230228
        public List<T> GetEngProgress<T>(int engMainSeq, DateTime tarDate)
        {
            /*string sql = @"
                select 
	                z.OrderNo,
                    z.Description,
                    CAST((z.Price * z.Quantity * z.SchProgress) / Amount as decimal(6,2)) SchProgress,
                    CAST((z.Price * z.ActualQuantity * 100) / Amount as decimal(6,2)) ActualProgress
                from fPayItemProgress(@EngMainSeq, @mode, @tarDate) z
                order by z.OrderNo
                ";*/
            //s20230426
            string sql = @"
                select 
	                za.OrderNo, za.Description,
                    ISNULL(max(za.ECSchProgress), max(za.SchProgress)) SchProgress,
                    ISNULL(max(za.ECActualProgress), max(za.ActualProgress)) ActualProgress
                from (
                    select 
    	
                        ISNULL(z1.ParentSchEngProgressPayItemSeq, z1.RootSeq) OrderNo,
                        z.Description,
                        null SchProgress,
                        null ActualProgress,
                        CAST((z.Price * z.Quantity * z.SchProgress) / z.Amount as decimal(6,2)) ECSchProgress,
                        CAST((z.Price * z.ActualQuantity * 100) / z.Amount as decimal(6,2)) ECActualProgress
                    from fECPayItemProgress(@EngMainSeq, 1, GetDate()) z
                    inner join EC_SchEngProgressPayItem z1 on(z1.Seq=z.EC_SchEngProgressPayItemSeq)

                    union all

                    select 
                        z.SchEngProgressPayItemSeq OrderNo,
                        z.Description,
                        CAST((z.Price * z.Quantity * z.SchProgress) / z.Amount as decimal(6,2)) SchProgress,
                        CAST((z.Price * z.ActualQuantity * 100) / z.Amount as decimal(6,2)) ActualProgress,
    	                null ECSchProgress,
                        null ECActualProgress
                    from fPayItemProgress(@EngMainSeq, 1, GetDate()) z
                ) za
                group by za.OrderNo, za.Description
            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@mode", SupDailyReportService._Supervise);
            cmd.Parameters.AddWithValue("@tarDate", tarDate);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //工程停工天數
        public int GetStopWorkDays(int engMainSeq)
        {
            string sql = @"
                select
                    ISNULL(sum(DATEDIFF(day, a.SStopWorkDate, a.EStopWorkDate)+1), 0) total
                from SupDailyReportWork a
                where a.EngMainSeq=@EngMainSeq             
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
    }
}