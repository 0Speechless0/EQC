using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class ECSupDailyReportConstructionEquipmentService : BaseService
    {//工程變更-施工日誌-機具管理
        //清單
        public List<T> GetList<T>(int supDailyDateSeq)
        {
            string sql = @"SELECT
                    a.Seq,
                    a.EC_SupDailyDateSeq,
                    a.EquipmentName,
                    a.EquipmentModel,
                    a.TodayQuantity,
                    cast(
                        ISNULL((
                            select sum(zc.TodayQuantity) from EC_SupDailyDate zb 
                            inner join EC_SupDailyReportConstructionEquipment zc on(zc.EC_SupDailyDateSeq=zb.Seq and zc.EquipmentName=a.EquipmentName and zc.EquipmentModel=a.EquipmentModel)
                            where zb.DataType=b.DataType and zb.EngMainSeq=b.EngMainSeq and zb.ItemDate<=b.ItemDate
    	                ),0)
                        +
                        ISNULL((
                            select sum(zc.TodayQuantity) from SupDailyDate zb
                            inner join SupDailyReportConstructionEquipment zc on(zc.SupDailyDateSeq=zb.Seq and zc.EquipmentName=a.EquipmentName and zc.EquipmentModel=a.EquipmentModel)
                            where zb.DataType=b.DataType and zb.EngMainSeq=b.EngMainSeq and zb.ItemDate<=b.ItemDate
    	                ),0)
                    as decimal(20,4)) AccQuantity,
                    a.TodayHours,
                    (a.KgCo2e*a.TodayHours) KgCo2eAmount, --s20230502
                    cast(
    	                ISNULL((
                            select sum(zc.TodayHours) from EC_SupDailyDate zb
                            inner join EC_SupDailyReportConstructionEquipment zc on(zc.EC_SupDailyDateSeq=zb.Seq and zc.EquipmentName=a.EquipmentName and zc.EquipmentModel=a.EquipmentModel)
                            where zb.DataType=b.DataType and zb.EngMainSeq=b.EngMainSeq and zb.ItemDate<=b.ItemDate
                        ),0)
                        +
                        ISNULL((
        	                select sum(zc.TodayHours) from SupDailyDate zb
                            inner join SupDailyReportConstructionEquipment zc on(zc.SupDailyDateSeq=zb.Seq and zc.EquipmentName=a.EquipmentName and zc.EquipmentModel=a.EquipmentModel)
                            where zb.DataType=b.DataType and zb.EngMainSeq=b.EngMainSeq and zb.ItemDate<=b.ItemDate
                        ),0)
                    as decimal(20,4)) AccHours
                FROM EC_SupDailyReportConstructionEquipment a
                inner join EC_SupDailyDate b on (b.Seq=a.EC_SupDailyDateSeq)
				where a.EC_SupDailyDateSeq=@EC_SupDailyDateSeq
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyDateSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //新增
        public int AddRecord(EC_SupDailyReportConstructionEquipmentModel m)
        {
            Null2Empty(m);
            string sql = @"
            insert into EC_SupDailyReportConstructionEquipment (
                EC_SupDailyDateSeq,
                EquipmentName,
                EquipmentModel,
                TodayQuantity,
                TodayHours,
                KgCo2e,
                CreateTime,
                CreateUserSeq,
                ModifyTime,
                ModifyUserSeq
            )values(
                @EC_SupDailyDateSeq,
                @EquipmentName,
                @EquipmentModel,
                @TodayQuantity,
                @TodayHours,
                @KgCo2e,
                GetDate(),
                @ModifyUserSeq,
                0,
                @ModifyUserSeq
            )";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", m.EC_SupDailyDateSeq);
                cmd.Parameters.AddWithValue("@EquipmentName", m.EquipmentName);
                cmd.Parameters.AddWithValue("@EquipmentModel", m.EquipmentModel);
                cmd.Parameters.AddWithValue("@TodayQuantity", m.TodayQuantity);
                cmd.Parameters.AddWithValue("@TodayHours", this.NulltoDBNull(m.TodayHours));
                cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(m.KgCo2e)); //s20230502
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('EC_SupDailyReportConstructionEquipment') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);

                m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                return 0;
            }
            catch (Exception e)
            {
                log.Info("ECSupDailyReportConstructionEquipmentService.AddRecords: " + e.Message);
                return -1;
            }
        }
        //更新
        public int UpdateRecord(EC_SupDailyReportConstructionEquipmentModel m)
        {
            Null2Empty(m);
            string sql = @"
            update EC_SupDailyReportConstructionEquipment set 
                --EquipmentName = @EquipmentName,
                EquipmentModel = @EquipmentModel,
                TodayQuantity = @TodayQuantity,
                TodayHours = @TodayHours,
                ModifyTime = GetDate(),
                ModifyUserSeq = @ModifyUserSeq
            where Seq=@Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                //cmd.Parameters.AddWithValue("@EquipmentName", m.EquipmentName);
                cmd.Parameters.AddWithValue("@EquipmentModel", m.EquipmentModel);
                cmd.Parameters.AddWithValue("@TodayQuantity", m.TodayQuantity);
                cmd.Parameters.AddWithValue("@TodayHours", this.NulltoDBNull(m.TodayHours));

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("ECSupDailyReportConstructionEquipmentService.UpdateRecords: " + e.Message);
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
                sql = @"delete from EC_SupDailyReportConstructionEquipment where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("ECSupDailyReportConstructionEquipmentService.DelDelRecords: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
    }
}