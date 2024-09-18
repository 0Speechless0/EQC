using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class ECSupDailyReportService : BaseService
    {//監造/施工 日誌
        public const int _Supervise = 1; //監造
        public const int _Construction = 2; //施工

        //日誌填報完成 s20231116
        public bool DailyLogCompleted(int supDailyItemSeq)
        {
            try
            {
                string sql = @"
                        update EC_SupDailyDate set
                        ItemState=1
                        where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", supDailyItemSeq);
                db.ExecuteNonQuery(cmd);

                return true;
            }
            catch (Exception e)
            {
                log.Info("ECSupDailyReportService.CopyConstructionMiscData:" + e.Message);
                return false;
            }
        }
        //複製前日材料,人員,機具資料 s20231116
        public bool CopyConstructionMiscData(ECSupDailyDateVModel supDailyItem, List<EC_SupDailyReportConstructionEquipmentModel> equipments, List<EC_SupDailyReportConstructionPersonModel> persons, List<EC_SupDailyReportConstructionMaterialModel> materials)
        {
            Null2Empty(supDailyItem);
            string sql = "";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                //機具資料
                if (equipments.Count > 0)
                {
                    sql = @"delete from EC_SupDailyReportConstructionEquipment where EC_SupDailyDateSeq=@EC_SupDailyDateSeq";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyItem.Seq);
                    db.ExecuteNonQuery(cmd);

                    sql = @"
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
                    foreach (EC_SupDailyReportConstructionEquipmentModel m in equipments)
                    {
                        Null2Empty(m);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyItem.Seq);
                        cmd.Parameters.AddWithValue("@EquipmentName", m.EquipmentName);
                        cmd.Parameters.AddWithValue("@EquipmentModel", m.EquipmentModel);
                        cmd.Parameters.AddWithValue("@TodayQuantity", m.TodayQuantity);
                        cmd.Parameters.AddWithValue("@TodayHours", this.NulltoDBNull(m.TodayHours));
                        cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(m.KgCo2e)); //s20230502
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                        db.ExecuteNonQuery(cmd);
                    }
                }
                //工地人員資料
                if (persons.Count > 0)
                {
                    sql = @"delete from EC_SupDailyReportConstructionPerson where EC_SupDailyDateSeq=@EC_SupDailyDateSeq";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyItem.Seq);
                    db.ExecuteNonQuery(cmd);

                    sql = @"
                    insert into EC_SupDailyReportConstructionPerson (
                        EC_SupDailyDateSeq,
                        KindName,
                        TodayQuantity,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    )values(
                        @EC_SupDailyDateSeq,
                        @KindName,
                        @TodayQuantity,
                        GetDate(),
                        @ModifyUserSeq,
                        0,
                        @ModifyUserSeq
                    )";

                    foreach (EC_SupDailyReportConstructionPersonModel m in persons)
                    {
                        Null2Empty(m);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyItem.Seq);
                        cmd.Parameters.AddWithValue("@KindName", m.KindName);
                        cmd.Parameters.AddWithValue("@TodayQuantity", m.TodayQuantity);
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                        db.ExecuteNonQuery(cmd);
                    }
                }

                //工地材料資料
                if (materials.Count > 0)
                {
                    sql = @"delete from EC_SupDailyReportConstructionMaterial where EC_SupDailyDateSeq=@EC_SupDailyDateSeq";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyItem.Seq);
                    db.ExecuteNonQuery(cmd);

                    sql = @"
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

                    foreach (EC_SupDailyReportConstructionMaterialModel m in materials)
                    {
                        Null2Empty(m);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyItem.Seq);
                        cmd.Parameters.AddWithValue("@MaterialName", m.MaterialName);
                        cmd.Parameters.AddWithValue("@Unit", m.Unit);
                        cmd.Parameters.AddWithValue("@ContractQuantity", this.NulltoDBNull(m.ContractQuantity));
                        cmd.Parameters.AddWithValue("@TodayQuantity", m.TodayQuantity);
                        //cmd.Parameters.AddWithValue("@AccQuantity", m.AccQuantity+ m.TodayQuantity);
                        cmd.Parameters.AddWithValue("@Memo", m.Memo);
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                        db.ExecuteNonQuery(cmd);
                    }
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("ECSupDailyReportService.CopyConstructionMiscData:" + e.Message);
                log.Info(sql);
                return false;
            }
        }
        //新增施工日誌 ====================================
        public bool ConstructionAdd(EC_SupDailyDateModel supDailyItem, EC_SupDailyReportMiscConstructionModel miscItem, List<EC_SupPlanOverviewModel> planItems,
            List<EC_SupDailyReportConstructionEquipmentModel> equipments, List<EC_SupDailyReportConstructionPersonModel> persons, List<EC_SupDailyReportConstructionMaterialModel> materials)
        {
            Null2Empty(supDailyItem);
            string sql = "";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                sql = @"
                    insert into EC_SupDailyDate(
                        EngMainSeq,
                        ItemDate,
                        DataType,
                        Weather1,
                        Weather2,
                        FillinDate,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EngMainSeq,
                        @ItemDate,
                        @DataType,
                        @Weather1,
                        @Weather2,
                        @FillinDate,
                        GetDate(),
                        @ModifyUserSeq,
                        DATEADD(second, 1, getdate()),
                        @ModifyUserSeq
                    )";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", supDailyItem.EngMainSeq);
                cmd.Parameters.AddWithValue("@ItemDate", supDailyItem.ItemDate);
                cmd.Parameters.AddWithValue("@DataType", supDailyItem.DataType);
                cmd.Parameters.AddWithValue("@Weather1", supDailyItem.Weather1);
                cmd.Parameters.AddWithValue("@Weather2", supDailyItem.Weather2);
                cmd.Parameters.AddWithValue("@FillinDate", this.NulltoDBNull(supDailyItem.FillinDate));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('EC_SupDailyDate') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);
                int supDailyDateSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                //施工日誌_雜項
                if(miscItem != null)
                {
                    Null2Empty(miscItem);
                    sql = @"
                    insert into EC_SupDailyReportMiscConstruction(
                        EC_SupDailyDateSeq,
                        IsFollowSkill,
                        SafetyHygieneMattersOther,
                        SafetyHygieneMatters01,
                        SafetyHygieneMatters02,
                        SafetyHygieneMatters03,
                        SamplingTest,
                        NoticeManufacturers,
                        ImportantNotes,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EC_SupDailyDateSeq,
                        @IsFollowSkill,
                        @SafetyHygieneMattersOther,
                        @SafetyHygieneMatters01,
                        @SafetyHygieneMatters02,
                        @SafetyHygieneMatters03,
                        @SamplingTest,
                        @NoticeManufacturers,
                        @ImportantNotes,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyDateSeq);
                    cmd.Parameters.AddWithValue("@IsFollowSkill", this.NulltoDBNull(miscItem.IsFollowSkill));
                    cmd.Parameters.AddWithValue("@SafetyHygieneMattersOther", miscItem.SafetyHygieneMattersOther);
                    cmd.Parameters.AddWithValue("@SafetyHygieneMatters01", this.NulltoDBNull(miscItem.SafetyHygieneMatters01));
                    cmd.Parameters.AddWithValue("@SafetyHygieneMatters02", this.NulltoDBNull(miscItem.SafetyHygieneMatters02));
                    cmd.Parameters.AddWithValue("@SafetyHygieneMatters03", this.NulltoDBNull(miscItem.SafetyHygieneMatters03));
                    cmd.Parameters.AddWithValue("@SamplingTest", miscItem.SamplingTest);
                    cmd.Parameters.AddWithValue("@NoticeManufacturers", miscItem.NoticeManufacturers);
                    cmd.Parameters.AddWithValue("@ImportantNotes", miscItem.ImportantNotes);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }


                //依施工計畫執行按圖施工概況
                sql = @"insert into EC_SupPlanOverview(
                        EC_SupDailyDateSeq,
                        EC_SchEngProgressPayItemSeq,
                        TodayConfirm,
                        Memo,
                        DayProgress,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    )values(
                        @EC_SupDailyDateSeq,
                        @EC_SchEngProgressPayItemSeq,
                        @TodayConfirm,
                        @Memo,
                        @DayProgress,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                )";
                foreach (EC_SupPlanOverviewModel m in planItems)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyDateSeq);
                    cmd.Parameters.AddWithValue("@EC_SchEngProgressPayItemSeq", m.EC_SchEngProgressPayItemSeq);
                    cmd.Parameters.AddWithValue("@TodayConfirm", this.NulltoDBNull(m.TodayConfirm));
                    cmd.Parameters.AddWithValue("@Memo", m.Memo);
                    cmd.Parameters.AddWithValue("@DayProgress", m.DayProgress);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                //預設機具資料 s20230831
                sql = @"
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
                        GetDate(),
                        @ModifyUserSeq
                    )";
                foreach (EC_SupDailyReportConstructionEquipmentModel m in equipments)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyDateSeq);
                    cmd.Parameters.AddWithValue("@EquipmentName", m.EquipmentName);
                    cmd.Parameters.AddWithValue("@EquipmentModel", m.EquipmentModel);
                    cmd.Parameters.AddWithValue("@TodayQuantity", m.TodayQuantity);
                    cmd.Parameters.AddWithValue("@TodayHours", this.NulltoDBNull(m.TodayHours));
                    cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(m.KgCo2e)); //s20230502
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }
                //預設工地人員資料 s20230831
                sql = @"
                    insert into EC_SupDailyReportConstructionPerson (
                        EC_SupDailyDateSeq,
                        KindName,
                        TodayQuantity,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    )values(
                        @EC_SupDailyDateSeq,
                        @KindName,
                        @TodayQuantity,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";

                foreach (EC_SupDailyReportConstructionPersonModel m in persons)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyDateSeq);
                    cmd.Parameters.AddWithValue("@KindName", m.KindName);
                    cmd.Parameters.AddWithValue("@TodayQuantity", m.TodayQuantity);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }
                //預設工地材料資料 s20230831
                sql = @"
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
                        GetDate(),
                        @ModifyUserSeq
                    )";

                foreach (EC_SupDailyReportConstructionMaterialModel m in materials)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyDateSeq);
                    cmd.Parameters.AddWithValue("@MaterialName", m.MaterialName);
                    cmd.Parameters.AddWithValue("@Unit", m.Unit);
                    cmd.Parameters.AddWithValue("@ContractQuantity", this.NulltoDBNull(m.ContractQuantity));
                    cmd.Parameters.AddWithValue("@TodayQuantity", m.TodayQuantity);
                    //cmd.Parameters.AddWithValue("@AccQuantity", m.AccQuantity+ m.TodayQuantity);
                    cmd.Parameters.AddWithValue("@Memo", m.Memo);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("ECSupDailyReportService.ConstructionAdd:" + e.Message);
                log.Info(sql);
                return false;
            }
        }
        //更新施工日誌
        public bool ConstructionUpdate(EC_SupDailyDateModel supDailyItem, EC_SupDailyReportMiscConstructionModel miscItem, List<EC_SupPlanOverviewModel> planItems)
        {
            string sql = "";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                //施工日誌_日誌日期 20230408
                Null2Empty(supDailyItem);
                sql = @"
                    update EC_SupDailyDate set
                        Weather1=@Weather1,
                        Weather2=@Weather2,
                        FillinDate=@FillinDate,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", supDailyItem.Seq);
                cmd.Parameters.AddWithValue("@Weather1", supDailyItem.Weather1);
                cmd.Parameters.AddWithValue("@Weather2", supDailyItem.Weather2);
                cmd.Parameters.AddWithValue("@FillinDate", this.NulltoDBNull(supDailyItem.FillinDate));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                //施工日誌_雜項
                if (miscItem != null)
                {
                    Null2Empty(miscItem);
                    sql = @"
                    update EC_SupDailyReportMiscConstruction set
                        IsFollowSkill=@IsFollowSkill,
                        SafetyHygieneMattersOther=@SafetyHygieneMattersOther,
                        SafetyHygieneMatters01=@SafetyHygieneMatters01,
                        SafetyHygieneMatters02=@SafetyHygieneMatters02,
                        SafetyHygieneMatters03=@SafetyHygieneMatters03,
                        SamplingTest=@SamplingTest,
                        NoticeManufacturers=@NoticeManufacturers,
                        ImportantNotes=@ImportantNotes,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", miscItem.Seq);
                    cmd.Parameters.AddWithValue("@IsFollowSkill", this.NulltoDBNull(miscItem.IsFollowSkill));
                    cmd.Parameters.AddWithValue("@SafetyHygieneMattersOther", miscItem.SafetyHygieneMattersOther);
                    cmd.Parameters.AddWithValue("@SafetyHygieneMatters01", this.NulltoDBNull(miscItem.SafetyHygieneMatters01));
                    cmd.Parameters.AddWithValue("@SafetyHygieneMatters02", this.NulltoDBNull(miscItem.SafetyHygieneMatters02));
                    cmd.Parameters.AddWithValue("@SafetyHygieneMatters03", this.NulltoDBNull(miscItem.SafetyHygieneMatters03));
                    cmd.Parameters.AddWithValue("@SamplingTest", miscItem.SamplingTest);
                    cmd.Parameters.AddWithValue("@NoticeManufacturers", miscItem.NoticeManufacturers);
                    cmd.Parameters.AddWithValue("@ImportantNotes", miscItem.ImportantNotes);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }


                //依施工計畫執行按圖施工概況
                sql = @"update EC_SupPlanOverview set
                        TodayConfirm=@TodayConfirm,
                        Memo=@Memo,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
                foreach (EC_SupPlanOverviewModel m in planItems)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", m.Seq);
                    cmd.Parameters.AddWithValue("@TodayConfirm", this.NulltoDBNull(m.TodayConfirm));
                    cmd.Parameters.AddWithValue("@Memo", m.Memo);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("ECSupDailyReportService.ConstructionUpdate:" + e.Message);
                log.Info(sql);
                return false;
            }
        }
        //日誌 每日紀錄
        public bool UpdateTodayConfirm(List<EC_SupPlanOverviewModel> planItems, ECSupDailyDateVModel supDailyItem)
        {
            string sql = "";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                Null2Empty(supDailyItem);
                sql = @"
                    update EC_SupDailyDate set
                        FillinDate=@FillinDate,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", supDailyItem.Seq);
                cmd.Parameters.AddWithValue("@FillinDate", this.NulltoDBNull(supDailyItem.FillinDate));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);//s20230408

                //依施工計畫執行按圖施工概況
                sql = @"update EC_SupPlanOverview set
                        TodayConfirm=@TodayConfirm,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
                foreach (EC_SupPlanOverviewModel m in planItems)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", m.Seq);
                    cmd.Parameters.AddWithValue("@TodayConfirm", this.NulltoDBNull(m.TodayConfirm));
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("ECSupDailyReportService.UpdateTodayConfirm:" + e.Message);
                log.Info(sql);
                return false;
            }
        }
        //取得日誌
        public List<T> GetSupDailyDate<T>(int dataType, int engMainSeq, DateTime itemDate)
        {
            return GetSupDailyDate<T>(dataType, engMainSeq, itemDate, itemDate);
        }
        public List<T> GetSupDailyDate<T>(int dataType, int engMainSeq, DateTime sDate, DateTime eDate)
        {
            string sql = @"
				SELECT
                    a.Seq,
					a.EngMainSeq,
                    a.ItemDate,
                    a.DataType,
                    a.ItemState,
                    a.Weather1,
                    a.Weather2,
                    a.FillinDate,
                    a.ModifyTime,
                    b.DisplayName
				FROM EC_SupDailyDate a
                left outer join UserMain b on(b.Seq=a.ModifyUserSeq)
				WHERE a.engMainSeq=@engMainSeq
                and a.DataType=@DataType
				and a.ItemDate>=@sDate
                and a.ItemDate<=@eDate
                order by a.ItemDate
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@engMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@DataType", dataType);
            cmd.Parameters.AddWithValue("@sDate", sDate);
            cmd.Parameters.AddWithValue("@eDate", eDate);//s20230228
            return db.GetDataTableWithClass<T>(cmd);
        }
        //含已填寫天數 s20230228
        public List<T> GetSupDailyDateAndCount<T>(int dataType, int engMainSeq, DateTime sDate, DateTime eDate)
        {
            string sql = @"
			    declare @DayCount as int = 
			    (
					    select Count(*) from SupDailyDate sp
					    where sp.EngMainSeq = @engMainSeq and sp.DataType = @DataType 
				    )
				declare @MinItemDate as datetime = 
				(
                    select Min(ItemDate) from EC_SupDailyDate
				    WHERE engMainSeq=@engMainSeq
                    and DataType=@DataType and CreateTime != ModifyTime
                )
				SELECT
                    a.Seq,
					a.EngMainSeq,
                    a.ItemDate,
                    a.DataType,
                    a.ItemState,
                    a.Weather1,
                    a.Weather2,
                    a.FillinDate,
					DATEDIFF(DAY,  @MinItemDate, a.ItemDate) +1 + @DayCount  OrderNo
				FROM EC_SupDailyDate a
                inner join EC_SupDailyDate b on(b.EngMainSeq=a.EngMainSeq)
				WHERE a.engMainSeq=@engMainSeq
                and a.DataType=@DataType
				and a.ItemDate>=@sDate
                and a.ItemDate<=@eDate
                group by a.Seq,a.EngMainSeq,a.ItemDate,a.DataType,a.ItemState,a.Weather1,a.Weather2,a.FillinDate
                order by a.ItemDate
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@engMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@DataType", dataType);
            cmd.Parameters.AddWithValue("@sDate", sDate);
            cmd.Parameters.AddWithValue("@eDate", eDate);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetSupDailyDateBySeq<T>(int seq)
        {
            string sql = @"
				SELECT
                    Seq,
					EngMainSeq,
                    ItemDate,
                    DataType,
                    ItemState,
                    Weather1,
                    Weather2,
                    FillinDate
				FROM EC_SupDailyDate
				WHERE Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //含已填寫天數
        public List<T> GetSupDailyDateAndCount<T>(int seq)
        {
            string sql = @"
				declare @MinItemDate as datetime = 
				(
                    select Min(ec.ItemDate) from EC_SupDailyDate e
					inner join EC_SupDailyDate ec on  (ec.EngMainSeq = e.EngMainSeq and ec.DataType=e.DataType)
				    WHERE e.Seq = @Seq
                     and e.CreateTime != e.ModifyTime
                )
			    declare @DayCount as int = 
			    (
					    select Count(*) from EC_SupDailyDate sp
					    inner join SupDailyDate spp on (sp.EngMainSeq = spp.EngMainSeq and sp.DataType = spp.DataType )
					    where sp.Seq = @Seq
				    )
				SELECT
                    a.Seq,
					a.EngMainSeq,
                    a.ItemDate,
                    a.DataType,
                    a.ItemState,
                    a.Weather1,
                    a.Weather2,
                    a.FillinDate,
					DATEDIFF(DAY,  @MinItemDate, a.ItemDate) +1 + @DayCount  OrderNo
				FROM EC_SupDailyDate a
                inner join EC_SupDailyDate b on(b.EngMainSeq=a.EngMainSeq)
				WHERE a.Seq=@Seq
                group by a.Seq,a.EngMainSeq,a.ItemDate,a.DataType,a.ItemState,a.Weather1,a.Weather2,a.FillinDate
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //施工日誌_雜項
        public List<T> GetMiscConstruction<T>(int supDailyDateSeq)
        {
            string sql = @"
				SELECT
                    Seq,
                    IsFollowSkill,
                    SafetyHygieneMattersOther,
                    SafetyHygieneMatters01,
                    SafetyHygieneMatters02,
                    SafetyHygieneMatters03,
                    SamplingTest,
                    NoticeManufacturers,
                    ImportantNotes
				FROM EC_SupDailyReportMiscConstruction
				WHERE EC_SupDailyDateSeq=@EC_SupDailyDateSeq
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyDateSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //新增監造日誌 ====================================
        public bool MiscAdd(EC_SupDailyDateModel supDailyItem, EC_SupDailyReportMiscModel miscItem, List<EC_SupPlanOverviewModel> planItems)
        {
            Null2Empty(supDailyItem);
            string sql = "";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                sql = @"
                    insert into EC_SupDailyDate(
                        EngMainSeq,
                        ItemDate,
                        DataType,
                        Weather1,
                        Weather2,
                        FillinDate,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EngMainSeq,
                        @ItemDate,
                        @DataType,
                        @Weather1,
                        @Weather2,
                        @FillinDate,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", supDailyItem.EngMainSeq);
                cmd.Parameters.AddWithValue("@ItemDate", supDailyItem.ItemDate);
                cmd.Parameters.AddWithValue("@DataType", supDailyItem.DataType);
                cmd.Parameters.AddWithValue("@Weather1", supDailyItem.Weather1);
                cmd.Parameters.AddWithValue("@Weather2", supDailyItem.Weather2);
                cmd.Parameters.AddWithValue("@FillinDate", this.NulltoDBNull(supDailyItem.FillinDate));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('EC_SupDailyDate') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);
                int supDailyDateSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                //施工日誌_雜項
                Null2Empty(miscItem);
                sql = @"
                    insert into EC_SupDailyReportMisc(
                        EC_SupDailyDateSeq,
                        DesignDrawingConst,
                        SpecAndQuality,
                        SafetyHygieneMattersOther,
                        SafetyHygieneMatters01,
                        OtherMatters,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EC_SupDailyDateSeq,
                        @DesignDrawingConst,
                        @SpecAndQuality,
                        @SafetyHygieneMattersOther,
                        @SafetyHygieneMatters01,
                        @OtherMatters,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyDateSeq);
                cmd.Parameters.AddWithValue("@DesignDrawingConst", miscItem.DesignDrawingConst);
                cmd.Parameters.AddWithValue("@SpecAndQuality", miscItem.SpecAndQuality);
                cmd.Parameters.AddWithValue("@SafetyHygieneMattersOther", miscItem.SafetyHygieneMattersOther);
                cmd.Parameters.AddWithValue("@SafetyHygieneMatters01", this.NulltoDBNull(miscItem.SafetyHygieneMatters01));
                cmd.Parameters.AddWithValue("@OtherMatters", miscItem.OtherMatters);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                //依施工計畫執行按圖施工概況
                sql = @"insert into EC_SupPlanOverview(
                        EC_SupDailyDateSeq,
                        EC_SchEngProgressPayItemSeq,
                        TodayConfirm,
                        Memo,
                        DayProgress,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    )values(
                        @EC_SupDailyDateSeq,
                        @EC_SchEngProgressPayItemSeq,
                        @TodayConfirm,
                        @Memo,
                        @DayProgress,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                )";
                foreach (EC_SupPlanOverviewModel m in planItems)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyDateSeq);
                    cmd.Parameters.AddWithValue("@EC_SchEngProgressPayItemSeq", m.EC_SchEngProgressPayItemSeq);
                    cmd.Parameters.AddWithValue("@TodayConfirm", this.NulltoDBNull(m.TodayConfirm));
                    cmd.Parameters.AddWithValue("@Memo", m.Memo);
                    cmd.Parameters.AddWithValue("@DayProgress", m.DayProgress);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("ECSupDailyReportService.ConstructionAdd:" + e.Message);
                log.Info(sql);
                return false;
            }
        }
        //更新監造日誌
        public bool MiscUpdate(EC_SupDailyDateModel supDailyItem, EC_SupDailyReportMiscModel miscItem, List<EC_SupPlanOverviewModel> planItems)
        {
            string sql = "";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                //日誌日期 20230408
                Null2Empty(supDailyItem);
                sql = @"
                    update EC_SupDailyDate set
                        Weather1=@Weather1,
                        Weather2=@Weather2,
                        FillinDate=@FillinDate,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", supDailyItem.Seq);
                cmd.Parameters.AddWithValue("@Weather1", supDailyItem.Weather1);
                cmd.Parameters.AddWithValue("@Weather2", supDailyItem.Weather2);
                cmd.Parameters.AddWithValue("@FillinDate", this.NulltoDBNull(supDailyItem.FillinDate));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                //雜項
                Null2Empty(miscItem);
                sql = @"
                    update EC_SupDailyReportMisc set
                        DesignDrawingConst=@DesignDrawingConst,
                        SpecAndQuality=@SpecAndQuality,
                        SafetyHygieneMattersOther=@SafetyHygieneMattersOther,
                        SafetyHygieneMatters01=@SafetyHygieneMatters01,
                        OtherMatters=@OtherMatters,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", miscItem.Seq);
                cmd.Parameters.AddWithValue("@DesignDrawingConst", miscItem.DesignDrawingConst);
                cmd.Parameters.AddWithValue("@SpecAndQuality", miscItem.SpecAndQuality);
                cmd.Parameters.AddWithValue("@SafetyHygieneMattersOther", miscItem.SafetyHygieneMattersOther);
                cmd.Parameters.AddWithValue("@SafetyHygieneMatters01", this.NulltoDBNull(miscItem.SafetyHygieneMatters01));
                cmd.Parameters.AddWithValue("@OtherMatters", miscItem.OtherMatters);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                //依施工計畫執行按圖施工概況
                sql = @"update EC_SupPlanOverview set
                        TodayConfirm=@TodayConfirm,
                        Memo=@Memo,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
                foreach (EC_SupPlanOverviewModel m in planItems)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", m.Seq);
                    cmd.Parameters.AddWithValue("@TodayConfirm", this.NulltoDBNull(m.TodayConfirm));
                    cmd.Parameters.AddWithValue("@Memo", m.Memo);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("ECSupDailyReportService.ConstructionUpdate:" + e.Message);
                log.Info(sql);
                return false;
            }
        }
        //監造日誌_雜項
        public List<T> GetMisc<T>(int supDailyDateSeq)
        {
            string sql = @"
				SELECT
                    Seq,
					DesignDrawingConst,
                    SpecAndQuality,
                    SafetyHygieneMattersOther,
                    SafetyHygieneMatters01,
                    OtherMatters
				FROM EC_SupDailyReportMisc
				WHERE EC_SupDailyDateSeq=@EC_SupDailyDateSeq
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyDateSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //監造日誌_雜項 s20230831
        public List<T> GetMiscByDate<T>(int engMainSeq, DateTime tarDate)
        {
            string sql = @"
				SELECT
                    -1 Seq,
					DesignDrawingConst,
                    SpecAndQuality,
                    SafetyHygieneMattersOther,
                    SafetyHygieneMatters01,
                    OtherMatters
				FROM EC_SupDailyReportMisc
                WHERE EC_SupDailyDateSeq in (
	                select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq and dataType=1 and ItemDate=@ItemDate
                )
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", tarDate);
            return db.GetDataTableWithClass<T>(cmd);
        }

        // 施工/監造 已記錄日誌日期
        public List<T> GetCalendarInfo<T>(int engMainSeq, DateTime itemDate, int dataType)
        {
            string sql = @"
				SELECT
                    ItemDate
				FROM EC_SupDailyDate
				WHERE EngMainSeq=@EngMainSeq
                and ItemDate>=@startDate
                and ItemDate<=@endDate
                and DataType=@DataType";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@startDate", itemDate);
            cmd.Parameters.AddWithValue("@endDate", itemDate.AddDays(31));
            cmd.Parameters.AddWithValue("DataType", dataType);
            return db.GetDataTableWithClass<T>(cmd);
        }

        // 施工/監造 依施工計畫執行按圖施工概況
        public List<T> GetPlanOverview<T>(int supDailyDateSeq)
        {
            string sql = @"
				SELECT
                    a.Seq,
                    a.EC_SchEngProgressPayItemSeq,
                    a1.OrderNo,
					a1.PayItem,
                    a1.Description,
                    a1.Unit,
                    a1.Quantity,
                    a1.Price,
                    a1.Amount,
                    a.TodayConfirm,
                    Cast(0 as decimal(20,4)) TotalAccConfirm,
                    a.Memo,
                    a.DayProgress
				FROM EC_SupPlanOverview a
                inner join EC_SchEngProgressPayItem a1 on(a1.Seq=a.EC_SchEngProgressPayItemSeq)
				WHERE a.EC_SupDailyDateSeq=@EC_SupDailyDateSeq
                order by a1.OrderNo
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyDateSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //含累計數量 不含進度為 -1
        public List<T> GetPlanOverviewAndTotalFilter<T>(int supDailyDateSeq, string exportMode)
        {
            string sql = @"
                select
                    z1.Seq,
                    z1.EC_SchEngProgressPayItemSeq,
                    z2.OrderNo,
                    z2.PayItem,
                    z2.Description,
                    z2.Unit,
                    z2.Quantity,
                    z2.Price,
                    z2.Amount,
                    Z1.TodayConfirm,
                    z1.Memo,
                    z1.DayProgress,
                    (
                      ISNULL((
                        SELECT sum(b.TodayConfirm) FROM EC_SupDailyDate a
                        inner join EC_SupPlanOverview b on(b.EC_SupDailyDateSeq=a.Seq and b.TodayConfirm>0 )
                        inner join EC_SchEngProgressPayItem b1 on(b1.Seq=b.EC_SchEngProgressPayItemSeq and b1.RootSeq=z2.RootSeq)
                        where a.EngMainSeq=z.EngMainSeq
                        and a.DataType=z.DataType
                        and a.ItemDate<z.ItemDate
                      ),0)
                       +
                      ISNULL((
                          SELECT sum(b.TodayConfirm) FROM SupDailyDate a
                          inner join SupPlanOverview b on(b.SupDailyDateSeq=a.Seq and b.TodayConfirm>0 )
                          inner join SchEngProgressPayItem b1 on(b1.Seq=b.SchEngProgressPayItemSeq and b1.Seq=z2.ParentSchEngProgressPayItemSeq)
                          where a.EngMainSeq=z.EngMainSeq
                          and a.DataType=z.DataType
                      ),0) 
                    ) TotalAccConfirm
                FROM EC_SupDailyDate z
                inner join EC_SupPlanOverview z1 on(z1.EC_SupDailyDateSeq=z.Seq)
                inner join EC_SchEngProgressPayItem z2 on(z2.Seq=z1.EC_SchEngProgressPayItemSeq)
                where z.Seq=@EC_SupDailyDateSeq
                and z1.DayProgress != -1
                and( @exportMode='0' or (@exportMode='1' and z1.TodayConfirm>0) ) --s20230831
                order by z2.OrderNo
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyDateSeq);
            cmd.Parameters.AddWithValue("@exportMode", exportMode);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //含累計數量
        public List<T> GetPlanOverviewAndTotal<T>(int supDailyDateSeq)
        {
            string sql = @"
                select
                    z1.Seq,
                    z1.EC_SchEngProgressPayItemSeq,
                    z2.OrderNo,
                    z2.PayItem,
                    z2.Description,
                    z2.Unit,
                    z2.Quantity,
                    z2.Price,
                    z2.Amount,
                    Z1.TodayConfirm,
                    z1.Memo,
                    z1.DayProgress,
                    (
                      ISNULL((
                        SELECT sum(b.TodayConfirm) FROM EC_SupDailyDate a
                        inner join EC_SupPlanOverview b on(b.EC_SupDailyDateSeq=a.Seq and b.TodayConfirm>0 )
                        inner join EC_SchEngProgressPayItem b1 on(b1.Seq=b.EC_SchEngProgressPayItemSeq and b1.RootSeq=z2.RootSeq)
                        where a.EngMainSeq=z.EngMainSeq
                        and a.DataType=z.DataType
                        and a.ItemDate<z.ItemDate
                      ),0)
                       +
                      ISNULL((
                          SELECT sum(b.TodayConfirm) FROM SupDailyDate a
                          
                         inner join SupPlanOverview b on(b.SupDailyDateSeq=a.Seq and b.TodayConfirm>0 )
                          inner join SchEngProgressPayItem b1 on(b1.Seq=b.SchEngProgressPayItemSeq and b1.Seq=z2.ParentSchEngProgressPayItemSeq)
                        left join EC_SupDailyDate e on e.ItemDate = a.ItemDate and e.DataType = a.DataType and e.EngMainSeq = a.EngMainSeq  
                        where a.EngMainSeq=z.EngMainSeq
                          and a.DataType=z.DataType and e.Seq is null
                      ),0) 
                    ) TotalAccConfirm
                FROM EC_SupDailyDate z
                inner join EC_SupPlanOverview z1 on(z1.EC_SupDailyDateSeq=z.Seq)
                inner join EC_SchEngProgressPayItem z2 on(z2.Seq=z1.EC_SchEngProgressPayItemSeq)
                where z.Seq=@EC_SupDailyDateSeq
                order by z2.OrderNo
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EC_SupDailyDateSeq", supDailyDateSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //預定進度 Payitem 清單
        public List<T> GetPayitemList<T>(int engMainSeq, DateTime tarDate)
        {
            string sql = @"
            select zz.* from (
                select DISTINCT
                    a.EC_SchEngProgressPayItemSeq,
                    a1.OrderNo,
                    -1 Seq,
                    a1.PayItem,
                    a1.Description,
                    a1.Unit,
                    a1.Quantity,
                    a1.Price,
                    a1.Amount,
                    cast(0 as decimal(20, 4)) TodayConfirm,
                    cast(0 as decimal(20, 4)) TotalAccConfirm,
                    a.DayProgress
                from EC_SchEngProgressHeader b
                inner join EC_SchEngProgressPayItem b1 on(b1.EC_SchEngProgressHeaderSeq=b.Seq)
                inner join EC_SchProgressPayItem a on(a.EC_SchEngProgressPayItemSeq=b1.Seq)
                inner join EC_SchEngProgressPayItem a1 on(a1.Seq=a.EC_SchEngProgressPayItemSeq)
                where b.EngMainSeq=@EngMainSeq
                and (b.StartDate<=@SPDate and (b.EndDate is null or b.EndDate>=@SPDate)) --s20230527
                and a.SPDate = (
                	select min(z.SPDate) from EC_SchEngProgressPayItem z1
                    inner join EC_SchEngProgressHeader z2 on(z2.Seq=z1.EC_SchEngProgressHeaderSeq)
                    inner join EC_SchProgressPayItem z on(z.EC_SchEngProgressPayItemSeq=z1.Seq)
                    where z2.EngMainSeq=@EngMainSeq
                    and z.SPDate >= @SPDate
                )

                union ALL
                
                select DISTINCT
                    a.EC_SchEngProgressPayItemSeq,
                    a1.OrderNo,
                    -1 Seq,
                    a1.PayItem,
                    a1.Description,
                    a1.Unit,
                    a1.Quantity,
                    a1.Price,
                    a1.Amount,
                    cast(0 as decimal(20, 4)) TodayConfirm,
                    cast(0 as decimal(20, 4)) TotalAccConfirm,
                    DayProgressAfter DayProgress
                from EC_SchEngProgressHeader b
                inner join EC_SchEngProgressPayItem b1 on(b1.EC_SchEngProgressHeaderSeq=b.Seq)
                inner join EC_SchProgressPayItem a on(a.EC_SchEngProgressPayItemSeq=b1.Seq)
                inner join EC_SchEngProgressPayItem a1 on(a1.Seq=a.EC_SchEngProgressPayItemSeq)
                where b.EngMainSeq=@EngMainSeq
                and (b.StartDate<=@SPDate and (b.EndDate is null or b.EndDate>=@SPDate)) --s20230527
                and (
                    select count(z1.Seq) from EC_SchEngProgressPayItem z1
                    inner join EC_SchEngProgressHeader z2 on(z2.Seq=z1.EC_SchEngProgressHeaderSeq)
                    inner join EC_SchProgressPayItem z on(z.EC_SchEngProgressPayItemSeq=z1.Seq)
                    where z2.EngMainSeq=@EngMainSeq
                    and z.SPDate >= @SPDate
                    ) = 0
                and a.SPDate = (
                    select max(z.SPDate) from EC_SchEngProgressPayItem z1
                    inner join EC_SchEngProgressHeader z2 on(z2.Seq=z1.EC_SchEngProgressHeaderSeq)
                    inner join EC_SchProgressPayItem z on(z.EC_SchEngProgressPayItemSeq=z1.Seq)
                    where z2.EngMainSeq=@EngMainSeq
                )
            ) zz
            Order by zz.OrderNo
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@SPDate", tarDate);

            return db.GetDataTableWithClass<T>(cmd);
        }

        /// <summary>
        /// 工程進度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seq">EngMain.Seq</param>
        /// <param name="mode">1:監造 2:施工</param>
        /// <param name="tarDate"></param>
        /// <returns></returns>
        public List<T> GetEngProgress<T>(int seq, int mode, string tarDate)
        {
            string sql = @"
                SELECT 
                    CAST(sum(z1.Price * z1.Quantity * z1.SchProgress) / ISNULL(sum(z1.Amount),1) as decimal(6,2)) SchProgress,
                    CAST(sum(z1.Price * z1.actualQuantity * 100) / ISNULL(sum(z1.Amount),1) as decimal(6,2)) AcualProgress
                from fECPayItemProgress(@Seq, @mode, @tarDate) z1
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            cmd.Parameters.AddWithValue("@mode", mode);
            cmd.Parameters.AddWithValue("@tarDate", tarDate);
            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}