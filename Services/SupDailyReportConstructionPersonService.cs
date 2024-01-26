using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class SupDailyReportConstructionPersonService : BaseService
    {//施工日誌-工地人員
        //前一日工地人員資料 s20231113
        public List<T> GetDayBeforeItems<T>(int engMainSeq, DateTime dayBefore)
        {
            string sql = @"
                SELECT
                    KindName,
                    TodayQuantity      
                FROM SupDailyReportConstructionPerson
                where SupDailyDateSeq in (
  	                select Seq from SupDailyDate where EngMainSeq=@EngMainSeq and dataType=2 and ItemDate=@ItemDate
                )
                order by KindName
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", dayBefore);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //預設工地人員資料 s20230831
        public List<T> GetDefaultItems<T>(int engMainSeq, DateTime dayBefore)
        {
            string sql = @"
                select a.ItemName KindName,
                    ISNULL(b.TodayQuantity,0) TodayQuantity
                from OptionList a
                left outer join (
                    SELECT
                        KindName,
                        TodayQuantity      
                    FROM SupDailyReportConstructionPerson
                    where SupDailyDateSeq in (
  	                select Seq from SupDailyDate where EngMainSeq=@EngMainSeq and dataType=2 and ItemDate=@ItemDate
                    )
                ) b on(b.KindName=a.ItemName)
                where a.ItemType=2
                and a.ItemName in('鋼筋工','模板工','泥水工')
                order by ItemName
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", dayBefore);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //前一日工地人員資料-工變 s20231113
        public List<T> GetDayBeforeItemsEC<T>(int engMainSeq, DateTime dayBefore)
        {
            string sql = @"
                select * from (
                    SELECT
                        KindName,
                        TodayQuantity      
                    FROM SupDailyReportConstructionPerson
                    where SupDailyDateSeq in (
  	                    select Seq from SupDailyDate where EngMainSeq=@EngMainSeq and dataType=2 and ItemDate=@ItemDate
                    )
                    union all
                    SELECT
                        KindName,
                        TodayQuantity      
                    FROM EC_SupDailyReportConstructionPerson
                    where EC_SupDailyDateSeq in (
  	                    select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq and dataType=2 and ItemDate=@ItemDate
                    )
                ) z
                order by z.KindName
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", dayBefore);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //預設工地人員資料-工變 s20230831
        public List<T> GetDefaultItemsEC<T>(int engMainSeq, DateTime dayBefore)
        {
            string sql = @"
                select a.ItemName KindName,
                    ISNULL(b.TodayQuantity,0) TodayQuantity
                from OptionList a
                left outer join (
                    SELECT
                        KindName,
                        TodayQuantity      
                    FROM EC_SupDailyReportConstructionPerson
                    where EC_SupDailyDateSeq in (
  	                select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq and dataType=2 and ItemDate=@ItemDate
                    )
                ) b on(b.KindName=a.ItemName)
                where a.ItemType=2
                and a.ItemName in('鋼筋工','模板工','泥水工')
                order by ItemName
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", dayBefore);

            return db.GetDataTableWithClass<T>(cmd);
        }

        //清單
        public List<T> GetList<T>(int supDailyDateSeq)
        {
            string sql = @"SELECT
                    a.Seq,
                    a.SupDailyDateSeq,
                    a.KindName,
                    a.TodayQuantity,
                    cast(
                    (
                        select sum(zc.TodayQuantity) from SupDailyDate za
                        inner join SupDailyDate zb on(zb.DataType=za.DataType and zb.EngMainSeq=za.EngMainSeq and zb.ItemDate<=za.ItemDate)
                        inner join SupDailyReportConstructionPerson zc on(zc.SupDailyDateSeq=zb.Seq and zc.KindName=a.KindName)
                        where za.Seq=a.SupDailyDateSeq
                    ) as decimal(20,4)) AccQuantity
                FROM SupDailyReportConstructionPerson a
				where a.SupDailyDateSeq=@SupDailyDateSeq
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyDateSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //新增
        public int AddRecord(SupDailyReportConstructionPersonModel m)
        {
            Null2Empty(m);
            string sql = @"
            insert into SupDailyReportConstructionPerson (
                SupDailyDateSeq,
                KindName,
                TodayQuantity,
                --AccQuantity,
                CreateTime,
                CreateUserSeq,
                ModifyTime,
                ModifyUserSeq
            )values(
                @SupDailyDateSeq,
                @KindName,
                @TodayQuantity,
                --@AccQuantity,
                GetDate(),
                @ModifyUserSeq,
                GetDate(),
                @ModifyUserSeq
            )";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@SupDailyDateSeq", m.SupDailyDateSeq);
                cmd.Parameters.AddWithValue("@KindName", m.KindName);
                cmd.Parameters.AddWithValue("@TodayQuantity", m.TodayQuantity);
                //cmd.Parameters.AddWithValue("@AccQuantity", this.NulltoDBNull(m.AccQuantity));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('SupDailyReportConstructionPerson') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);

                m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                return 0;
            }
            catch (Exception e)
            {
                log.Info("SupDailyReportConstructionPersonService.AddRecords: " + e.Message);
                return -1;
            }
        }
        //更新
        public int UpdateRecord(SupDailyReportConstructionPersonModel m)
        {
            Null2Empty(m);
            string sql = @"
            update SupDailyReportConstructionPerson set 
                --KindName = @KindName,
                TodayQuantity = @TodayQuantity,
                --AccQuantity = @AccQuantity,
                ModifyTime = GetDate(),
                ModifyUserSeq = @ModifyUserSeq
            where Seq=@Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                //cmd.Parameters.AddWithValue("@KindName", m.KindName);
                cmd.Parameters.AddWithValue("@TodayQuantity", m.TodayQuantity);
                //cmd.Parameters.AddWithValue("@AccQuantity", this.NulltoDBNull(m.AccQuantity));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("SupDailyReportConstructionPersonService.UpdateRecords: " + e.Message);
                return -1;
            }
        }
        //刪除
        public int DelRecord(int seq)
        {
            string sql = "";
            db.BeginTransaction();
            try
            {
                sql = @"delete from SupDailyReportConstructionPerson where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupDailyReportConstructionPersonService.DelDelRecords: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
    }
}