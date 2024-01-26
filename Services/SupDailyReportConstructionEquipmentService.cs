using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class SupDailyReportConstructionEquipmentService : BaseService
    {//施工日誌-機具管理
        //前一日機具資料 s20231113
        public List<T> GetDayBeforeItems<T>(int engMainSeq, DateTime dayBefore)
        {
            string sql = @"
                SELECT
                    EquipmentName,
                    EquipmentModel,   
                    KgCo2e,
                    TodayQuantity,
                    TodayHours
                FROM SupDailyReportConstructionEquipment
                where SupDailyDateSeq in (
  	                select Seq from SupDailyDate where EngMainSeq=@EngMainSeq and dataType=2 and ItemDate=@ItemDate
                )
                order by EquipmentName, EquipmentModel
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", dayBefore);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //預設機具資料 s20230831
        public List<T> GetDefaultItems<T>(int engMainSeq, DateTime dayBefore)
        {
            string sql = @"
                select a.Kind EquipmentName,
	                a.NameSpec EquipmentModel,
                    a.KgCo2e,
                    ISNULL(b.TodayQuantity, 0) TodayQuantity,
                    ISNULL(b.TodayHours, 0) TodayHours
                from CarbonEmissionMachine a
                left outer join (
                    SELECT
                        EquipmentName,
                        EquipmentModel,      
                        TodayQuantity,
                        TodayHours
                    FROM SupDailyReportConstructionEquipment
                    where SupDailyDateSeq in (
  	                    select Seq from SupDailyDate where EngMainSeq=@EngMainSeq and dataType=2 and ItemDate=@ItemDate
                    )
                ) b on(b.EquipmentName=a.Kind and b.EquipmentModel=a.NameSpec)
                where a.Kind in('抽水機','推土機')
                and a.NameSpec in('抽水機，1000L/min','推土機，履帶式，120~129kW','推土機，履帶式帶犁刀，340~349kW')
                order by a.Kind,a.NameSpec
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", dayBefore);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //前一日機具資料-工變 s20231113
        public List<T> GetDayBeforeItemsEC<T>(int engMainSeq, DateTime dayBefore)
        {
            string sql = @"
                select * from (
                    SELECT
                        EquipmentName,
                        EquipmentModel,   
                        KgCo2e,
                        TodayQuantity,
                        TodayHours
                    FROM SupDailyReportConstructionEquipment
                    where SupDailyDateSeq in (
  	                    select Seq from SupDailyDate where EngMainSeq=@EngMainSeq and dataType=2 and ItemDate=@ItemDate
                    )
                    union all
                    SELECT
                        EquipmentName,
                        EquipmentModel, 
                        KgCo2e,
                        TodayQuantity,
                        TodayHours
                    FROM EC_SupDailyReportConstructionEquipment
                    where EC_SupDailyDateSeq in (
  	                    select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq and dataType=2 and ItemDate=@ItemDate
                    )
                ) z
                order by z.EquipmentName, z.EquipmentModel
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", dayBefore);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //預設機具資料-工變 s20230831
        public List<T> GetDefaultItemsEC<T>(int engMainSeq, DateTime dayBefore)
        {
            string sql = @"
                select a.Kind EquipmentName,
	                a.NameSpec EquipmentModel,
                    a.KgCo2e,
                    ISNULL(b.TodayQuantity, 0) TodayQuantity,
                    ISNULL(b.TodayHours, 0) TodayHours
                from CarbonEmissionMachine a
                left outer join (
                    SELECT
                        EquipmentName,
                        EquipmentModel,      
                        TodayQuantity,
                        TodayHours
                    FROM EC_SupDailyReportConstructionEquipment
                    where EC_SupDailyDateSeq in (
  	                    select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq and dataType=2 and ItemDate=@ItemDate
                    )
                ) b on(b.EquipmentName=a.Kind and b.EquipmentModel=a.NameSpec)
                where a.Kind in('抽水機','推土機')
                and a.NameSpec in('抽水機，1000L/min','推土機，履帶式，120~129kW','推土機，履帶式帶犁刀，340~349kW')
                order by a.Kind,a.NameSpec
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
                    a.EquipmentName,
                    a.EquipmentModel,
                    a.TodayQuantity,
                    cast(
                    (
                        select sum(zc.TodayQuantity) from SupDailyDate za
                        inner join SupDailyDate zb on(zb.DataType=za.DataType and zb.EngMainSeq=za.EngMainSeq and zb.ItemDate<=za.ItemDate)
                        inner join SupDailyReportConstructionEquipment zc on(zc.SupDailyDateSeq=zb.Seq and zc.EquipmentName=a.EquipmentName and zc.EquipmentModel=a.EquipmentModel)
                        where za.Seq=a.SupDailyDateSeq
                    ) as decimal(20,4)) AccQuantity,
                    a.TodayHours,
                    (a.KgCo2e*a.TodayHours) KgCo2eAmount, --s20230502
                    cast(
                    (
                        select sum(zc.TodayHours) from SupDailyDate za
                        inner join SupDailyDate zb on(zb.DataType=za.DataType and zb.EngMainSeq=za.EngMainSeq and zb.ItemDate<=za.ItemDate)
                        inner join SupDailyReportConstructionEquipment zc on(zc.SupDailyDateSeq=zb.Seq and zc.EquipmentName=a.EquipmentName and zc.EquipmentModel=a.EquipmentModel)
                        where za.Seq=a.SupDailyDateSeq
                    ) as decimal(20,4)) AccHours
                FROM SupDailyReportConstructionEquipment a
				where a.SupDailyDateSeq=@SupDailyDateSeq
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyDateSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //新增
        public int AddRecord(SupDailyReportConstructionEquipmentModel m)
        {
            Null2Empty(m);
            string sql = @"
            insert into SupDailyReportConstructionEquipment (
                SupDailyDateSeq,
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
                @SupDailyDateSeq,
                @EquipmentName,
                @EquipmentModel,
                @TodayQuantity,
                @TodayHours,
                @KgCo2e,
                GetDate(),
                @ModifyUserSeq,
                GetDate(),
                @ModifyUserSeq
            )";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@SupDailyDateSeq", m.SupDailyDateSeq);
                cmd.Parameters.AddWithValue("EquipmentName", m.EquipmentName);
                cmd.Parameters.AddWithValue("EquipmentModel", m.EquipmentModel);
                cmd.Parameters.AddWithValue("@TodayQuantity", m.TodayQuantity);
                cmd.Parameters.AddWithValue("@TodayHours", this.NulltoDBNull(m.TodayHours));
                cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(m.KgCo2e));//s20230502
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('SupDailyReportConstructionEquipment') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);

                m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                return 0;
            }
            catch (Exception e)
            {
                log.Info("SupDailyReportConstructionEquipmentService.AddRecords: " + e.Message);
                return -1;
            }
        }
        //更新
        public int UpdateRecord(SupDailyReportConstructionEquipmentModel m)
        {
            Null2Empty(m);
            string sql = @"
            update SupDailyReportConstructionEquipment set 
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
                cmd.Parameters.AddWithValue("EquipmentModel", m.EquipmentModel);
                cmd.Parameters.AddWithValue("@TodayQuantity", m.TodayQuantity);
                cmd.Parameters.AddWithValue("@TodayHours", this.NulltoDBNull(m.TodayHours));

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("SupDailyReportConstructionEquipmentService.UpdateRecords: " + e.Message);
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
                sql = @"delete from SupDailyReportConstructionEquipment where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupDailyReportConstructionEquipmentService.DelDelRecords: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
    }
}