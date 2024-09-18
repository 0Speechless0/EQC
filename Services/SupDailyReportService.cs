using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class SupDailyReportService : BaseService
    {//監造/施工 日誌
        public const int _Supervise = 1; //監造
        public const int _Construction = 2; //施工

        //日誌填報完成 s20231116
        public bool DailyLogCompleted(int supDailyItemSeq)
        {
            try
            {
                string sql = @"
                        update SupDailyDate set
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
        public bool CopyConstructionMiscData(SupDailyDateModel supDailyItem, List<SupDailyReportConstructionEquipmentModel> equipments, List<SupDailyReportConstructionPersonModel> persons, List<SupDailyReportConstructionMaterialModel> materials)
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
                    sql = @"delete from SupDailyReportConstructionEquipment where SupDailyDateSeq=@SupDailyDateSeq";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyItem.Seq);
                    db.ExecuteNonQuery(cmd);

                    
                    sql = @"
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
                            0,
                            @ModifyUserSeq
                        )";
                    foreach (SupDailyReportConstructionEquipmentModel m in equipments)
                    {
                        Null2Empty(m);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyItem.Seq);
                        cmd.Parameters.AddWithValue("@EquipmentName", m.EquipmentName);
                        cmd.Parameters.AddWithValue("@EquipmentModel", m.EquipmentModel);
                        cmd.Parameters.AddWithValue("@TodayQuantity", m.TodayQuantity);
                        cmd.Parameters.AddWithValue("@TodayHours", this.NulltoDBNull(m.TodayHours));
                        cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(m.KgCo2e));
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                        db.ExecuteNonQuery(cmd);
                    }
                }
                //工地人員資料
                if (persons.Count > 0)
                {
                    sql = @"delete from SupDailyReportConstructionPerson where SupDailyDateSeq=@SupDailyDateSeq";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyItem.Seq);
                    db.ExecuteNonQuery(cmd);

                    sql = @"
                        insert into SupDailyReportConstructionPerson (
                            SupDailyDateSeq,
                            KindName,
                            TodayQuantity,
                            CreateTime,
                            CreateUserSeq,
                            ModifyTime,
                            ModifyUserSeq
                        )values(
                            @SupDailyDateSeq,
                            @KindName,
                            @TodayQuantity,
                            GetDate(),
                            @ModifyUserSeq,
                            0,
                            @ModifyUserSeq
                        )";

                    foreach (SupDailyReportConstructionPersonModel m in persons)
                    {
                        Null2Empty(m);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyItem.Seq);
                        cmd.Parameters.AddWithValue("@KindName", m.KindName);
                        cmd.Parameters.AddWithValue("@TodayQuantity", m.TodayQuantity);
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                        db.ExecuteNonQuery(cmd);
                    }
                }

                //工地材料資料
                if (materials.Count > 0)
                {
                    sql = @"delete from SupDailyReportConstructionMaterial where SupDailyDateSeq=@SupDailyDateSeq";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyItem.Seq);
                    db.ExecuteNonQuery(cmd);

                    sql = @"
                        insert into SupDailyReportConstructionMaterial (
                            SupDailyDateSeq,
                            MaterialName,
                            Unit,
                            ContractQuantity,
                            TodayQuantity,
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
                            @Memo,
                            GetDate(),
                            @ModifyUserSeq,
                            0,
                            @ModifyUserSeq
                        )";

                    foreach (SupDailyReportConstructionMaterialModel m in materials)
                    {
                        Null2Empty(m);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyItem.Seq);
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
                log.Info("SupDailyReportService.CopyConstructionMiscData:" + e.Message);
                log.Info(sql);
                return false;
            }
        }
        //新增施工日誌 ====================================
        public bool ConstructionAdd(SupDailyDateModel supDailyItem, SupDailyReportMiscConstructionModel miscItem, List<SupPlanOverviewModel> planItems,
            List<SupDailyReportConstructionEquipmentModel> equipments, List<SupDailyReportConstructionPersonModel> persons, List<SupDailyReportConstructionMaterialModel> materials,
            bool excelGenerate = false
            )
        {
            Null2Empty(supDailyItem);
            string sql = "";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                sql = @"
                    insert into SupDailyDate(
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
                        @CreateTime,
                        @ModifyUserSeq,
                        @ModifyTime,
                        @ModifyUserSeq
                    )";
                DateTime createTime = DateTime.Now;
                object modifyTime;
                if (!excelGenerate)
                {
                    modifyTime = createTime.AddSeconds(1);
                }
                else
                {
                    modifyTime = DateTime.Now;
                }
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", supDailyItem.EngMainSeq);
                cmd.Parameters.AddWithValue("@ItemDate", supDailyItem.ItemDate);
                cmd.Parameters.AddWithValue("@DataType", supDailyItem.DataType);
                cmd.Parameters.AddWithValue("@Weather1", supDailyItem.Weather1);
                cmd.Parameters.AddWithValue("@Weather2", supDailyItem.Weather2);
                cmd.Parameters.AddWithValue("@ModifyTime", modifyTime);
                cmd.Parameters.AddWithValue("@CreateTime", createTime);
                cmd.Parameters.AddWithValue("@FillinDate", this.NulltoDBNull(supDailyItem.FillinDate));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('SupDailyDate') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);
                int supDailyDateSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                //施工日誌_雜項
                Null2Empty(miscItem);
                sql = @"
                    insert into SupDailyReportMiscConstruction(
                        SupDailyDateSeq,
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
                        @SupDailyDateSeq,
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
                cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyDateSeq);
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

                //依施工計畫執行按圖施工概況
                sql = @"insert into SupPlanOverview(
                        SupDailyDateSeq,
                        SchEngProgressPayItemSeq,
                        TodayConfirm,
                        Memo,
                        DayProgress,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    )values(
                        @SupDailyDateSeq,
                        @SchEngProgressPayItemSeq,
                        @TodayConfirm,
                        @Memo,
                        @DayProgress,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                )";
                foreach (SupPlanOverviewModel m in planItems)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyDateSeq);
                    cmd.Parameters.AddWithValue("@SchEngProgressPayItemSeq", m.SchEngProgressPayItemSeq);
                    cmd.Parameters.AddWithValue("@TodayConfirm", this.NulltoDBNull(m.TodayConfirm));
                    cmd.Parameters.AddWithValue("@Memo", m.Memo);
                    cmd.Parameters.AddWithValue("@DayProgress", m.DayProgress);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }
                //預設機具資料 s20230831
                sql = @"
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
                foreach (SupDailyReportConstructionEquipmentModel m in equipments) {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyDateSeq);
                    cmd.Parameters.AddWithValue("@EquipmentName", m.EquipmentName);
                    cmd.Parameters.AddWithValue("@EquipmentModel", m.EquipmentModel);
                    cmd.Parameters.AddWithValue("@TodayQuantity", m.TodayQuantity);
                    cmd.Parameters.AddWithValue("@TodayHours", this.NulltoDBNull(m.TodayHours));
                    cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(m.KgCo2e));
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                //預設工地人員資料 s20230831
                sql = @"
                    insert into SupDailyReportConstructionPerson (
                        SupDailyDateSeq,
                        KindName,
                        TodayQuantity,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    )values(
                        @SupDailyDateSeq,
                        @KindName,
                        @TodayQuantity,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";

                foreach (SupDailyReportConstructionPersonModel m in persons)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyDateSeq);
                    cmd.Parameters.AddWithValue("@KindName", m.KindName);
                    cmd.Parameters.AddWithValue("@TodayQuantity", m.TodayQuantity);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                //預設工地材料資料 s20230831
                sql = @"
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
                        GetDate(),
                        @ModifyUserSeq
                    )";

                foreach (SupDailyReportConstructionMaterialModel m in materials) {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyDateSeq);
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
                log.Info("SupDailyReportService.ConstructionAdd:" + e.Message);
                log.Info(sql);
                return false;
            }
        }
        //更新施工日誌
        public bool ConstructionUpdate(SupDailyDateModel supDailyItem, SupDailyReportMiscConstructionModel miscItem, List<SupPlanOverviewModel> planItems)
        {
            string sql = "";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                //施工日誌_日誌日期 20230408
                Null2Empty(supDailyItem);
                sql = @"
                    update SupDailyDate set
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
                Null2Empty(miscItem);
                sql = @"
                    update SupDailyReportMiscConstruction set
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

                //依施工計畫執行按圖施工概況
                sql = @"update SupPlanOverview set
                        TodayConfirm=@TodayConfirm,
                        Memo=@Memo,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
                foreach (SupPlanOverviewModel m in planItems)
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
                log.Info("SupDailyReportService.ConstructionUpdate:" + e.Message);
                log.Info(sql);
                return false;
            }
        }
        //日誌 每日紀錄
        public bool UpdateTodayConfirm(List<SupPlanOverviewModel> planItems, EPCSupDailyDateVModel supDailyItem)
        {
            string sql = "";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                Null2Empty(supDailyItem);
                sql = @"
                    update SupDailyDate set
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
                sql = @"update SupPlanOverview set
                        TodayConfirm=@TodayConfirm,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
                foreach (SupPlanOverviewModel m in planItems)
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
                log.Info("SupDailyReportService.UpdateTodayConfirm:" + e.Message);
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
				FROM SupDailyDate a
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
    //        string sql = @"
				//SELECT
    //                a.Seq,
				//	a.EngMainSeq,
    //                a.ItemDate,
    //                a.DataType,
    //                a.ItemState,
    //                a.Weather1,
    //                a.Weather2,
    //                a.FillinDate,
    //                count(b.Seq) dailyCount
				//FROM SupDailyDate a
    //            inner join SupDailyDate b on(b.EngMainSeq=a.EngMainSeq)
				//WHERE a.engMainSeq=@engMainSeq
    //            and a.DataType=@DataType
				//and a.ItemDate>=@sDate
    //            and a.ItemDate<=@eDate
    //            group by a.Seq,a.EngMainSeq,a.ItemDate,a.DataType,a.ItemState,a.Weather1,a.Weather2,a.FillinDate
    //            order by a.ItemDate
    //            ";
            string sql = @"
				declare @MinItemDate as datetime = 
				(
                    select Min(ItemDate) from SupDailyDate
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
					DATEDIFF(DAY,  @MinItemDate, a.ItemDate) +1  OrderNo
				FROM SupDailyDate a
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
				FROM SupDailyDate
				WHERE Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //含已填寫天數
        public List<T> GetSupDailyDateAndCount<T>(int seq, int dataType)
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
					Min(b.ItemDate),
					Max( DATEDIFF(DAY, b.ItemDate, a.ItemDate) +1)  OrderNo,
                    count(b.Seq) dailyCount
				FROM SupDailyDate a
                inner join SupDailyDate b on(b.EngMainSeq=a.EngMainSeq )
				WHERE a.Seq=@Seq 
                    and b.ItemDate <= a.ItemDate
                    and b.CreateTime != b.ModifyTime
                group by a.Seq,a.EngMainSeq,a.ItemDate,a.DataType,a.ItemState,a.Weather1,a.Weather2,a.FillinDate
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            cmd.Parameters.AddWithValue("@DataType", dataType);
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
				FROM SupDailyReportMiscConstruction
				WHERE SupDailyDateSeq=@SupDailyDateSeq
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyDateSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //新增監造日誌 ====================================
        public bool MiscAdd(SupDailyDateModel supDailyItem, SupDailyReportMiscModel miscItem, List<SupPlanOverviewModel> planItems)
        {
            Null2Empty(supDailyItem);
            string sql = "";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                sql = @"
                    insert into SupDailyDate(
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
                string sql1 = @"SELECT IDENT_CURRENT('SupDailyDate') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);
                int supDailyDateSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                //施工日誌_雜項
                Null2Empty(miscItem);
                sql = @"
                    insert into SupDailyReportMisc(
                        SupDailyDateSeq,
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
                        @SupDailyDateSeq,
                        @DesignDrawingConst,
                        @SpecAndQuality,
                        @SafetyHygieneMattersOther,
                        @SafetyHygieneMatters01,
                        @OtherMatters,
                        null,
                        @ModifyUserSeq,
                        @ModifyTime,
                        @ModifyUserSeq
                    )";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyDateSeq);
                cmd.Parameters.AddWithValue("@DesignDrawingConst", miscItem.DesignDrawingConst);
                cmd.Parameters.AddWithValue("@SpecAndQuality", miscItem.SpecAndQuality);
                cmd.Parameters.AddWithValue("@SafetyHygieneMattersOther", miscItem.SafetyHygieneMattersOther);
                cmd.Parameters.AddWithValue("@SafetyHygieneMatters01", this.NulltoDBNull(miscItem.SafetyHygieneMatters01));
                cmd.Parameters.AddWithValue("@OtherMatters", miscItem.OtherMatters);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@ModifyTime", DateTime.Now);
                db.ExecuteNonQuery(cmd);

                //依施工計畫執行按圖施工概況
                sql = @"insert into SupPlanOverview(
                        SupDailyDateSeq,
                        SchEngProgressPayItemSeq,
                        /*OrderNo,
                        PayItem,
                        Description,
                        Unit,
                        Quantity,
                        Price,
                        Amount,
                        ItemKey,
                        ItemNo,
                        RefItemCode,*/
                        TodayConfirm,
                        Memo,
                        DayProgress,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    )values(
                        @SupDailyDateSeq,
                        @SchEngProgressPayItemSeq,
                        /*@OrderNo,
                        @PayItem,
                        @Description,
                        @Unit,
                        @Quantity,
                        @Price,
                        @Amount,
                        @ItemKey,
                        @ItemNo,
                        @RefItemCode,*/
                        @TodayConfirm,
                        @Memo,
                        @DayProgress,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                )";
                foreach (SupPlanOverviewModel m in planItems)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyDateSeq);
                    cmd.Parameters.AddWithValue("@SchEngProgressPayItemSeq", m.SchEngProgressPayItemSeq);
                    /*cmd.Parameters.AddWithValue("@OrderNo", m.OrderNo);
                    cmd.Parameters.AddWithValue("@PayItem", m.PayItem);
                    cmd.Parameters.AddWithValue("@Description", m.Description);
                    cmd.Parameters.AddWithValue("@Unit", m.Unit);
                    cmd.Parameters.AddWithValue("@Quantity", this.NulltoDBNull(m.Quantity));
                    cmd.Parameters.AddWithValue("@Price", this.NulltoDBNull(m.Price));
                    cmd.Parameters.AddWithValue("@Amount", this.NulltoDBNull(m.Amount));
                    cmd.Parameters.AddWithValue("@ItemKey", m.ItemKey);
                    cmd.Parameters.AddWithValue("@ItemNo", m.ItemNo);
                    cmd.Parameters.AddWithValue("@RefItemCode", m.RefItemCode);*/
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
                log.Info("SupDailyReportService.ConstructionAdd:" + e.Message);
                log.Info(sql);
                return false;
            }
        }
        //更新監造日誌
        public bool MiscUpdate(SupDailyDateModel supDailyItem, SupDailyReportMiscModel miscItem, List<SupPlanOverviewModel> planItems)
        {
            string sql = "";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                //日誌日期 20230408
                Null2Empty(supDailyItem);
                sql = @"
                    update SupDailyDate set
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
                    update SupDailyReportMisc set
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
                sql = @"update SupPlanOverview set
                        TodayConfirm=@TodayConfirm,
                        Memo=@Memo,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
                foreach (SupPlanOverviewModel m in planItems)
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
                log.Info("SupDailyReportService.ConstructionUpdate:" + e.Message);
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
				FROM SupDailyReportMisc
				WHERE SupDailyDateSeq=@SupDailyDateSeq
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyDateSeq);
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
				FROM SupDailyReportMisc
				WHERE SupDailyDateSeq in (
	                select Seq from SupDailyDate where EngMainSeq=@EngMainSeq and dataType=1 and ItemDate=@ItemDate
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
				sp.Seq,
                sd.CreateTime MiscConstructionCreateTime,
                sd.ModifyTime MiscConstructionModifyTime,
                    sp.ItemDate,
                    sp.CreateTime,
                    sp.ModifyTime,

                    -- 數字與查詢專區同步
					cast (case when sp2.Seq is null then 0 else 1 end　　as bit)  　machineNumFormFilled
                FROM SupDailyDate　sp
                inner join EngMain a on  a.Seq = sp.EngMainSeq 
				left join (
					select sp.Seq from 

						--SupDailyDate 是儲存工程施工日誌及監造報表填寫狀況的資料表

						--以下資料表顯示於 工程履約 > 進度管理 > 施工日誌下方其他頁簽
						--SupDailyReportConstructionEquipment 機具資料表
						--SupDailyReportConstructionMaterial 材料資料表
						--SupDailyReportConstructionPerson 人員資料表
						SupDailyDate sp

						--這裡left join 出來的資料筆數並不重要， 只看這些資料中有無 不為null且大於0 的欄位，有則代表已填寫過
						left join SupDailyReportConstructionEquipment sdr on (sp.Seq = sdr.SupDailyDateSeq ) 
						left join SupDailyReportConstructionMaterial sdm on (sdm.SupDailyDateSeq = sp.Seq  ) 
						left join SupDailyReportConstructionPerson sdp on (sdp.SupDailyDateSeq = sp.Seq )
                        left join  SupDailyReportMiscConstruction sdd on sdd.SupDailyDateSeq = sp.Seq
						--若三者其中之一TodayQuantity大於0代表填寫過
						where (
                            sdr.TodayQuantity > 0 or 
                            sdm.TodayQuantity > 0 or 
                            sdp.TodayQuantity > 0  or
                            sdr.CreateTime != sdr.ModifyTime or
                            sdm.CreateTime != sdm.ModifyTime or
                            sdp.CreateTime != sdp.ModifyTime or
                            sdd.CreateTime != sdd.ModifyTime
                        ) 
                        and  sp.DataType = @DataType
						group by sp.Seq
				)　 sp2 on sp.Seq = sp2.Seq
                left join  SupDailyReportMiscConstruction sd on sd.SupDailyDateSeq = sp.Seq
                WHERE EngMainSeq=@EngMainSeq
                and sp.DataType=@DataType
                and sp.ItemDate>=@startDate and sp.ItemDate<=@endDate
                and sp.ItemDate >= a.StartDate
                and sp.ItemDate <= a.SchCompDate
                and sp.CreateTime != sp.ModifyTime
                union all
                SELECT
				sp.Seq,
                sd.CreateTime MiscConstructionCreateTime,
                sd.ModifyTime MiscConstructionModifyTime,
                    sp.ItemDate,
                    sp.CreateTime,
                    sp.ModifyTime,
					cast (case when sp2.Seq is null then 0 else 1 end　　as bit)  　machineNumFormFilled
                FROM EC_SupDailyDate sp
                inner join EngMain a on  a.Seq = sp.EngMainSeq 
				left join (
					select sp.Seq from 

						--SupDailyDate 是儲存工程施工日誌及監造報表填寫狀況的資料表

						--以下資料表顯示於 工程履約 > 進度管理 > 施工日誌下方其他頁簽
						--SupDailyReportConstructionEquipment 機具資料表
						--SupDailyReportConstructionMaterial 材料資料表
						--SupDailyReportConstructionPerson 人員資料表
						EC_SupDailyDate sp

						--這裡left join 出來的資料筆數並不重要， 只看這些資料中有無 不為null且大於0 的欄位，有則代表已填寫過
						left join EC_SupDailyReportConstructionEquipment sdr on (sp.Seq = sdr.EC_SupDailyDateSeq  ) 
						left join EC_SupDailyReportConstructionMaterial sdm on (sdm.EC_SupDailyDateSeq = sp.Seq ) 
						left join EC_SupDailyReportConstructionPerson sdp on (sdp.EC_SupDailyDateSeq = sp.Seq )
                        left join EC_SupDailyReportMiscConstruction sdd on sdd.EC_SupDailyDateSeq = sp.Seq
						--若三者其中之一TodayQuantity大於0代表填寫過
						where (
                            sdr.TodayQuantity > 0 or 
                            sdm.TodayQuantity > 0 or 
                            sdp.TodayQuantity > 0  or
                            sdr.CreateTime != sdr.ModifyTime or
                            sdm.CreateTime != sdm.ModifyTime or
                            sdp.CreateTime != sdp.ModifyTime or
                            sdd.CreateTime != sdd.ModifyTime
                        ) and  sp.DataType = @DataType　
						group by sp.Seq
				)　 sp2 on sp.Seq = sp2.Seq
                left join  EC_SupDailyReportMiscConstruction sd on sd.EC_SupDailyDateSeq = sp.Seq
				WHERE EngMainSeq=@EngMainSeq
                and sp.DataType=@DataType
                and sp.ItemDate>=@startDate and sp.ItemDate<=  @endDate
                and sp.ItemDate >= ISNULL(a.EngChangeStartDate, a.StartDate)
                and sp.ItemDate <= ISNULL(a.EngChangeSchCompDate, a.SchCompDate)
                and sp.CreateTime != sp.ModifyTime
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@startDate", itemDate);
            cmd.Parameters.AddWithValue("@endDate", itemDate.AddMonths(1).AddDays(-1));//s20230627
            cmd.Parameters.AddWithValue("DataType", dataType);
            return db.GetDataTableWithClass<T>(cmd);
        }

        // 施工/監造 依施工計畫執行按圖施工概況
        public List<T> GetPlanOverview<T>(int supDailyDateSeq)
        {
            string sql = @"
				SELECT
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
                    a.TodayConfirm,
                    Cast(0 as decimal(20,4)) TotalAccConfirm,
                    a.Memo,
                    a.DayProgress
				FROM SupPlanOverview a
                inner join SchEngProgressPayItem a1 on(a1.Seq=a.SchEngProgressPayItemSeq)
				WHERE a.SupDailyDateSeq=@SupDailyDateSeq
                order by a1.OrderNo
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyDateSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //含累計數量 不含進度為 -1
        /*public List<T> GetPlanOverviewAndTotalFilter<T>(int supDailyDateSeq)
        {
            return GetPlanOverviewAndTotalFilter<T>(supDailyDateSeq, "0");
        }*/
        //exportMode 0:全部, 1:有數量 s20230830
        public List<T> GetPlanOverviewAndTotalFilter<T>(int supDailyDateSeq, string exportMode)
        {
            string sql = @"
				SELECT
                    z1.Seq,
                    z1.SchEngProgressPayItemSeq,
                    z11.OrderNo,
                    z11.PayItem,
                    z11.Description,
                    z11.Unit,
                    z11.Quantity,
                    z11.Price,
                    z11.Amount,
                    --z11.ItemKey,
                    --z11.ItemNo,
                    --z11.RefItemCode,
                    z1.TodayConfirm,
                    z2.TotalAccConfirm,
                    z1.Memo,
                    z1.DayProgress
                FROM SupPlanOverview z1
                inner join SchEngProgressPayItem z11 on(z11.Seq=z1.SchEngProgressPayItemSeq)
                inner join (
                    SELECT b.SupDailyDateSeq, b.Seq, sum(c.TodayConfirm) TotalAccConfirm FROM SupDailyDate a
                    inner join SupPlanOverview b on(b.SupDailyDateSeq=a.Seq)
                    left outer join (
                        SELECT bb.SchEngProgressPayItemSeq, bb.TodayConfirm FROM SupDailyDate ba
                        inner join SupDailyDate ba1 on(ba1.EngMainSeq = ba.EngMainSeq AND ba1.ItemDate < ba.ItemDate AND ba1.DataType = ba.DataType)
                        inner join SupPlanOverview bb on(bb.SupDailyDateSeq=ba1.Seq)
                        where ba.Seq=@SupDailyDateSeq
                    ) c on (b.SchEngProgressPayItemSeq=c.SchEngProgressPayItemSeq)
                    where a.Seq=@SupDailyDateSeq
                    group by b.SupDailyDateSeq, b.Seq
                ) z2 on (z2.SupDailyDateSeq=z1.SupDailyDateSeq and z2.Seq=z1.Seq)
                where z1.DayProgress != -1
                and( @exportMode='0' or (@exportMode='1' and z1.TodayConfirm>0) ) --s20230830
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyDateSeq);
            cmd.Parameters.AddWithValue("@exportMode", exportMode);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //含累計數量
        public List<T> GetPlanOverviewAndTotal<T>(int supDailyDateSeq)
        {
            string sql = @"
				SELECT
                    z1.Seq,
                    z1.SchEngProgressPayItemSeq,
                    z11.OrderNo,
                    z11.PayItem,
                    z11.Description,
                    z11.Unit,
                    z11.Quantity,
                    z11.Price,
                    z11.Amount,
                    --z11.ItemKey,
                    --z11.ItemNo,
                    --z11.RefItemCode,
                    z1.TodayConfirm,
                    z2.TotalAccConfirm,
                    z1.Memo,
                    z1.DayProgress
                FROM SupPlanOverview z1
                inner join SchEngProgressPayItem z11 on(z11.Seq=z1.SchEngProgressPayItemSeq)
                inner join (
                    SELECT b.SupDailyDateSeq, b.Seq, sum(c.TodayConfirm) TotalAccConfirm FROM SupDailyDate a
                    inner join SupPlanOverview b on(b.SupDailyDateSeq=a.Seq)
                    left outer join (
                        SELECT bb.SchEngProgressPayItemSeq, bb.TodayConfirm FROM SupDailyDate ba
                        inner join SupDailyDate ba1 on(ba1.EngMainSeq = ba.EngMainSeq AND ba1.ItemDate < ba.ItemDate AND ba1.DataType = ba.DataType)
                        inner join SupPlanOverview bb on(bb.SupDailyDateSeq=ba1.Seq)
                        where ba.Seq=@SupDailyDateSeq
                    ) c on (b.SchEngProgressPayItemSeq=c.SchEngProgressPayItemSeq)
                    where a.Seq=@SupDailyDateSeq
                    group by b.SupDailyDateSeq, b.Seq
                ) z2 on (z2.SupDailyDateSeq=z1.SupDailyDateSeq and z2.Seq=z1.Seq)
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyDateSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //預定進度 Payitem 清單
        public List<T> GetPayitemList<T>(int engMainSeq, DateTime tarDate)
        {
            string sql = @"
            select zz.* from (
                select DISTINCT
                    a.SchEngProgressPayItemSeq,
                    a1.OrderNo,
                    -1 Seq,
                    a1.PayItem,
                    a1.Description,
                    a1.Unit,
                    a1.Quantity,
                    a1.Price,
                    a1.Amount,
                    --a1.ItemKey,
                    --a1.ItemNo,
                    --a1.RefItemCode,
                    cast(0 as decimal(20, 4)) TodayConfirm,
                    cast(0 as decimal(20, 4)) TotalAccConfirm,
                    a.DayProgress
                from SchProgressHeader b
                inner join SchProgressPayItem a on(a.SchProgressHeaderSeq=b.Seq)
                inner join SchEngProgressPayItem a1 on(a1.Seq=a.SchEngProgressPayItemSeq)
                where b.EngMainSeq=@EngMainSeq
                and a.SPDate = (
                	select top 1 z.SPDate from SchProgressPayItem z
                    where z.SchProgressHeaderSeq = b.Seq
                    and z.SPDate >= @SPDate
                    order by z.SPDate
                )

                union ALL
                
                select DISTINCT
                    a.SchEngProgressPayItemSeq,
                    a1.OrderNo,
                    -1 Seq,
                    a1.PayItem,
                    a1.Description,
                    a1.Unit,
                    a1.Quantity,
                    a1.Price,
                    a1.Amount,
                    --a1.ItemKey,
                    --a1.ItemNo,
                    --a1.RefItemCode,
                    cast(0 as decimal(20, 4)) TodayConfirm,
                    cast(0 as decimal(20, 4)) TotalAccConfirm,
                    DayProgressAfter DayProgress
                from SchProgressHeader b
                inner join SchProgressPayItem a on(a.SchProgressHeaderSeq=b.Seq)
                inner join SchEngProgressPayItem a1 on(a1.Seq=a.SchEngProgressPayItemSeq)
                where b.EngMainSeq=@EngMainSeq
                and (
                    select count(z.Seq) from SchProgressPayItem z
                    where z.SchProgressHeaderSeq = b.Seq
                    and z.SPDate >= @SPDate
                    ) = 0
                and a.SPDate = (select max(z.SPDate) from SchProgressPayItem z where z.SchProgressHeaderSeq=b.Seq)
            ) zz
            Order by zz.OrderNo
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@SPDate", tarDate);

            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}
