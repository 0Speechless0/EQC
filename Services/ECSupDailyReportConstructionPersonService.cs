using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class ECSupDailyReportConstructionPersonService : BaseService
    {//工程變更-施工日誌-工地人員
        //清單
        public List<T> GetList<T>(int supDailyDateSeq)
        {
            string sql = @"SELECT
                    a.Seq,
                    a.EC_SupDailyDateSeq,
                    a.KindName,
                    a.TodayQuantity,
                    cast(
                        ISNULL((
                            select sum(zc.TodayQuantity) from EC_SupDailyDate zb
                            inner join EC_SupDailyReportConstructionPerson zc on(zc.EC_SupDailyDateSeq=zb.Seq and zc.KindName=a.KindName)
                            where zb.DataType=b.DataType and zb.EngMainSeq=b.EngMainSeq and zb.ItemDate<=b.ItemDate
                        ), 0)
                        +
                        ISNULL((
                            select sum(zc.TodayQuantity) from SupDailyDate zb
                            inner join SupDailyReportConstructionPerson zc on(zc.SupDailyDateSeq=zb.Seq and zc.KindName=a.KindName)
                            where zb.DataType=b.DataType and zb.EngMainSeq=b.EngMainSeq and zb.ItemDate<=b.ItemDate
                        ),0) --s20230425
                    as decimal(20,4) ) AccQuantity
                FROM EC_SupDailyReportConstructionPerson a
                inner join EC_SupDailyDate b on (b.Seq=a.EC_SupDailyDateSeq)
				where a.EC_SupDailyDateSeq=@EC_SupDailyDateSeq
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyDateSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //新增
        public int AddRecord(EC_SupDailyReportConstructionPersonModel m)
        {
            Null2Empty(m);
            string sql = @"
            insert into EC_SupDailyReportConstructionPerson (
                EC_SupDailyDateSeq,
                KindName,
                TodayQuantity,
                --AccQuantity,
                CreateTime,
                CreateUserSeq,
                ModifyTime,
                ModifyUserSeq
            )values(
                @EC_SupDailyDateSeq,
                @KindName,
                @TodayQuantity,
                --@AccQuantity,
                GetDate(),
                @ModifyUserSeq,
                0,
                @ModifyUserSeq
            )";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", m.EC_SupDailyDateSeq);
                cmd.Parameters.AddWithValue("@KindName", m.KindName);
                cmd.Parameters.AddWithValue("@TodayQuantity", m.TodayQuantity);
                //cmd.Parameters.AddWithValue("@AccQuantity", this.NulltoDBNull(m.AccQuantity));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('EC_SupDailyReportConstructionPerson') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);

                m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                return 0;
            }
            catch (Exception e)
            {
                log.Info("ECSupDailyReportConstructionPersonService.AddRecords: " + e.Message);
                return -1;
            }
        }
        //更新
        public int UpdateRecord(EC_SupDailyReportConstructionPersonModel m)
        {
            Null2Empty(m);
            string sql = @"
            update EC_SupDailyReportConstructionPerson set 
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
                log.Info("ECSupDailyReportConstructionPersonService.UpdateRecords: " + e.Message);
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
                sql = @"delete from EC_SupDailyReportConstructionPerson where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("ECSupDailyReportConstructionPersonService.DelDelRecords: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
    }
}