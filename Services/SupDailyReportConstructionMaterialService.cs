using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class SupDailyReportConstructionMaterialService : BaseService
    {//施工日誌-工地材料管理概況
        //前一日工地材料資料 s20231113
        public List<T> GetDayBeforeItems<T>(int engMainSeq, DateTime dayBefore)
        {
            string sql = @"
                SELECT
    	            MaterialName,
                    Unit,
                    ContractQuantity,
                    TodayQuantity
                FROM SupDailyReportConstructionMaterial
                where SupDailyDateSeq in (
                    select Seq from SupDailyDate where EngMainSeq=@EngMainSeq and dataType=2 and ItemDate=@ItemDate
                )
                order by MaterialName
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", dayBefore);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //預設工地材料資料 s20230831
        public List<T> GetDefaultItems<T>(int engMainSeq, DateTime dayBefore)
        {
            string sql = @"
                select a.ItemName MaterialName,
                    ISNULL(b.Unit,'式') Unit,
                    ISNULL(b.ContractQuantity, 0) ContractQuantity,
                    ISNULL(b.TodayQuantity, 0) TodayQuantity
                from OptionList a
                left outer join (
                    SELECT
    	                MaterialName,
                        Unit,
                        ContractQuantity,
                        TodayQuantity
                    FROM SupDailyReportConstructionMaterial
                    where SupDailyDateSeq in (
                      select Seq from SupDailyDate where EngMainSeq=@EngMainSeq and dataType=2 and ItemDate=@ItemDate
                    )
                ) b on(b.MaterialName=a.ItemName)
                where a.ItemType=1
                and a.ItemName in('預拌混凝土','鋼筋','模板')
                order by a.ItemName
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", dayBefore);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //前一日工地材料資料-工變 s20230831
        public List<T> GetDayBeforeItemsEC<T>(int engMainSeq, DateTime dayBefore)
        {
            string sql = @"
                select * from (
                    SELECT
    	                MaterialName,
                        Unit,
                        ContractQuantity,
                        TodayQuantity
                    FROM SupDailyReportConstructionMaterial
                    where SupDailyDateSeq in (
                        select Seq from SupDailyDate where EngMainSeq=@EngMainSeq and dataType=2 and ItemDate=@ItemDate
                    )
                    union all
                    SELECT
    	                MaterialName,
                        Unit,
                        ContractQuantity,
                        TodayQuantity
                    FROM EC_SupDailyReportConstructionMaterial
                    where EC_SupDailyDateSeq in (
                        select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq and dataType=2 and ItemDate=@ItemDate
                    )
                ) z
                order by z.MaterialName
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", dayBefore);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //預設工地材料資料-工變 s20230831
        public List<T> GetDefaultItemsEC<T>(int engMainSeq, DateTime dayBefore)
        {
            string sql = @"
                select a.ItemName MaterialName,
                    ISNULL(b.Unit,'式') Unit,
                    ISNULL(b.ContractQuantity, 0) ContractQuantity,
                    ISNULL(b.TodayQuantity, 0) TodayQuantity
                from OptionList a
                left outer join (
                    SELECT
    	                MaterialName,
                        Unit,
                        ContractQuantity,
                        TodayQuantity
                    FROM EC_SupDailyReportConstructionMaterial
                    where EC_SupDailyDateSeq in (
                      select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq and dataType=2 and ItemDate=@ItemDate
                    )
                ) b on(b.MaterialName=a.ItemName)
                where a.ItemType=1
                and a.ItemName in('預拌混凝土','鋼筋','模板')
                order by a.ItemName
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
                    a.MaterialName,
                    a.Unit,
                    a.ContractQuantity,
                    a.TodayQuantity,
                    a.Memo,
                    ISNULL((
	                    select sum(TodayQuantity) from SupDailyReportConstructionMaterial
                        where SupDailyDateSeq in (
    	                    select Seq from SupDailyDate 
                            where SupDailyDate.EngMainSeq = b.EngMainSeq
                            and SupDailyDate.ItemDate <= b.ItemDate
                        )
                        and SupDailyReportConstructionMaterial.MaterialName = a.MaterialName
                    ),0) AccQuantity --s20230908
                    
                FROM SupDailyReportConstructionMaterial a
                inner join SupDailyDate b on(b.Seq=a.SupDailyDateSeq)
				where a.SupDailyDateSeq=@SupDailyDateSeq
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyDateSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //新增
        public int AddRecord(SupDailyReportConstructionMaterialModel m)
        {
            Null2Empty(m);
            string sql = @"
            insert into SupDailyReportConstructionMaterial (
                SupDailyDateSeq,
                MaterialName,
                Unit,
                ContractQuantity,
                TodayQuantity,
                --AccQuantity,
                Memo,
                CreateTime,
                CreateUserSeq,
                ModifyTime,
                ModifyUserSeq
            )values(
                @SupDailyDateSeq,
                @MaterialName,
                @Unit,
                @ContractQuantity,
                @TodayQuantity,
                --@AccQuantity,
                @Memo,
                GetDate(),
                @ModifyUserSeq,
                0,
                @ModifyUserSeq
            )";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@SupDailyDateSeq", m.SupDailyDateSeq);
                cmd.Parameters.AddWithValue("@MaterialName", m.MaterialName);
                cmd.Parameters.AddWithValue("@Unit", m.Unit);
                cmd.Parameters.AddWithValue("@ContractQuantity", this.NulltoDBNull(m.ContractQuantity));
                cmd.Parameters.AddWithValue("@TodayQuantity", m.TodayQuantity);
                //cmd.Parameters.AddWithValue("@AccQuantity", this.NulltoDBNull(m.AccQuantity));
                cmd.Parameters.AddWithValue("@Memo", m.Memo);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('SupDailyReportConstructionMaterial') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);

                m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                return 0;
            }
            catch (Exception e)
            {
                log.Info("SupDailyReportConstructionMaterialService.AddRecords: " + e.Message);
                return -1;
            }
        }
        //更新
        public int UpdateRecord(SupDailyReportConstructionMaterialModel m)
        {
            Null2Empty(m);
            string sql = @"
            update SupDailyReportConstructionMaterial set 
                Unit = @Unit,
                ContractQuantity = @ContractQuantity,
                TodayQuantity = @TodayQuantity,
                Memo = @Memo,
                ModifyTime = GetDate(),
                ModifyUserSeq = @ModifyUserSeq
            where Seq=@Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                //cmd.Parameters.AddWithValue("@MaterialName", m.MaterialName);
                cmd.Parameters.AddWithValue("@Unit", m.Unit);
                cmd.Parameters.AddWithValue("@ContractQuantity", this.NulltoDBNull(m.ContractQuantity));
                cmd.Parameters.AddWithValue("@TodayQuantity", m.TodayQuantity);
                //cmd.Parameters.AddWithValue("@AccQuantity", this.NulltoDBNull(m.AccQuantity));
                cmd.Parameters.AddWithValue("@Memo", m.Memo);

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("SupDailyReportConstructionMaterialService.UpdateRecords: " + e.Message);
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
                sql = @"delete from SupDailyReportConstructionMaterial where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupDailyReportConstructionMaterialService.DelDelRecords: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
    }
}