using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class ECSupDailyReportConstructionMaterialService : BaseService
    {//工程變更-施工日誌-工地材料管理概況
        //清單
        public List<T> GetList<T>(int supDailyDateSeq)
        {
            string sql = @"SELECT
                    a.Seq,
                    a.EC_SupDailyDateSeq,
                    a.MaterialName,
                    a.Unit,
                    a.ContractQuantity,
                    a.TodayQuantity,
                    a.Memo,
                    ISNULL((
	                    select sum(TodayQuantity) from EC_SupDailyReportConstructionMaterial
                        where EC_SupDailyDateSeq in (
    	                    select Seq from EC_SupDailyDate 
                            where EC_SupDailyDate.EngMainSeq = b.EngMainSeq
                            and EC_SupDailyDate.ItemDate <= b.ItemDate
                        )
                        and EC_SupDailyReportConstructionMaterial.MaterialName = a.MaterialName
                        and EC_SupDailyReportConstructionMaterial.Unit = a.Unit
                    ),0)+ISNULL((
	                    select sum(TodayQuantity) from SupDailyReportConstructionMaterial
                        where SupDailyDateSeq in (
    	                    select Seq from SupDailyDate 
                            where SupDailyDate.EngMainSeq = b.EngMainSeq
                            and SupDailyDate.ItemDate <= b.ItemDate
                        )
                        and SupDailyReportConstructionMaterial.MaterialName = a.MaterialName
                        and SupDailyReportConstructionMaterial.Unit = a.Unit
                    ),0) AccQuantity --s20230908
                FROM EC_SupDailyReportConstructionMaterial a
                inner join EC_SupDailyDate b on(b.Seq=a.EC_SupDailyDateSeq)
				where a.EC_SupDailyDateSeq=@EC_SupDailyDateSeq
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyDateSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //新增
        public int AddRecord(EC_SupDailyReportConstructionMaterialModel m)
        {
            Null2Empty(m);
            string sql = @"
            insert into EC_SupDailyReportConstructionMaterial (
                EC_SupDailyDateSeq,
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
                @EC_SupDailyDateSeq,
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
                cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", m.EC_SupDailyDateSeq);
                cmd.Parameters.AddWithValue("@MaterialName", m.MaterialName);
                cmd.Parameters.AddWithValue("@Unit", m.Unit);
                cmd.Parameters.AddWithValue("@ContractQuantity", this.NulltoDBNull(m.ContractQuantity));
                cmd.Parameters.AddWithValue("@TodayQuantity", m.TodayQuantity);
                //cmd.Parameters.AddWithValue("@AccQuantity", this.NulltoDBNull(m.AccQuantity));
                cmd.Parameters.AddWithValue("@Memo", m.Memo);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('EC_SupDailyReportConstructionMaterial') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);

                m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                return 0;
            }
            catch (Exception e)
            {
                log.Info("ECSupDailyReportConstructionMaterialService.AddRecords: " + e.Message);
                return -1;
            }
        }
        //更新
        public int UpdateRecord(EC_SupDailyReportConstructionMaterialModel m)
        {
            Null2Empty(m);
            string sql = @"
            update EC_SupDailyReportConstructionMaterial set 
                Unit = @Unit,
                ContractQuantity = @ContractQuantity,
                TodayQuantity = @TodayQuantity,
                --AccQuantity = @AccQuantity,
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
                log.Info("ECSupDailyReportConstructionMaterialService.UpdateRecords: " + e.Message);
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
                sql = @"delete from EC_SupDailyReportConstructionMaterial where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("ECSupDailyReportConstructionMaterialService.DelDelRecords: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
    }
}