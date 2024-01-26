using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EPCCalendarService : BaseService
    {//工程行事月曆
        //停復工
        public List<T> GetWorkByPrjSeq<T>(int engMainSeq)
        {
            string sql = @"
				SELECT
                    a.Seq,
					a.SStopWorkDate,
                    a.EStopWorkDate,
                    a.StopWorkReason,
                    a.StopWorkNo,
                    a.StopWorkApprovalFile,
                    a.BackWorkDate,
                    a.BackWorkNo,
                    a.BackWorkApprovalFile,
                    b.Seq EC_SchEngProgressHeaderSeq
				FROM SupDailyReportWork a
                left outer join EC_SchEngProgressHeader b on(b.SupDailyReportWorkSeq=a.Seq) --s20230526
				WHERE a.EngMainSeq=@EngMainSeq
				ORDER BY a.Seq desc";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetWorkBySeq<T>(int seq)
        {
            string sql = @"
				SELECT
                    Seq,
                    EngMainSeq,
					SStopWorkDate,
                    EStopWorkDate,
                    StopWorkReason,
                    StopWorkNo,
                    StopWorkApprovalFile,
                    BackWorkDate,
                    BackWorkNo,
                    BackWorkApprovalFile
				FROM SupDailyReportWork
				WHERE Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public bool WorkAdd(SupDailyReportWorkModel m)
        {
            Null2Empty(m);
            try
            {
                string sql = @"
                    insert into SupDailyReportWork(
                        EngMainSeq,
                        SStopWorkDate,
                        EStopWorkDate,
                        StopWorkReason,
                        StopWorkNo,
                        StopWorkApprovalFile,
                        BackWorkDate,
                        BackWorkNo,
                        BackWorkApprovalFile,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EngMainSeq,
                        @SStopWorkDate,
                        @EStopWorkDate,
                        @StopWorkReason,
                        @StopWorkNo,
                        @StopWorkApprovalFile,
                        @BackWorkDate,
                        @BackWorkNo,
                        @BackWorkApprovalFile,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", m.EngMainSeq);
                cmd.Parameters.AddWithValue("@SStopWorkDate", m.SStopWorkDate);
                cmd.Parameters.AddWithValue("@EStopWorkDate", m.EStopWorkDate);
                cmd.Parameters.AddWithValue("@StopWorkReason", m.StopWorkReason);
                cmd.Parameters.AddWithValue("@StopWorkNo", m.StopWorkNo);
                cmd.Parameters.AddWithValue("@StopWorkApprovalFile", m.StopWorkApprovalFile);
                cmd.Parameters.AddWithValue("@BackWorkDate", this.NulltoDBNull(m.BackWorkDate));
                cmd.Parameters.AddWithValue("@BackWorkNo", m.BackWorkNo);
                cmd.Parameters.AddWithValue("@BackWorkApprovalFile", m.BackWorkApprovalFile);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                db.ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("EPCCalenderService.WorkAdd:" + e.Message);
                return false;
            }
        }
        public bool WorkUpdate(SupDailyReportWorkModel m)
        {
            Null2Empty(m);
            try
            {
                string sql = @"
                    update SupDailyReportWork set
                        SStopWorkDate=@SStopWorkDate,
                        EStopWorkDate=@EStopWorkDate,
                        StopWorkReason=@StopWorkReason,
                        StopWorkNo=@StopWorkNo,
                        StopWorkApprovalFile=@StopWorkApprovalFile,
                        BackWorkDate=@BackWorkDate,
                        BackWorkNo=@BackWorkNo,
                        BackWorkApprovalFile=@BackWorkApprovalFile,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@SStopWorkDate", m.SStopWorkDate);
                cmd.Parameters.AddWithValue("@EStopWorkDate", m.EStopWorkDate);
                cmd.Parameters.AddWithValue("@StopWorkReason", m.StopWorkReason);
                cmd.Parameters.AddWithValue("@StopWorkNo", m.StopWorkNo);
                cmd.Parameters.AddWithValue("@StopWorkApprovalFile", m.StopWorkApprovalFile);
                cmd.Parameters.AddWithValue("@BackWorkDate", this.NulltoDBNull(m.BackWorkDate));
                cmd.Parameters.AddWithValue("@BackWorkNo", m.BackWorkNo);
                cmd.Parameters.AddWithValue("@BackWorkApprovalFile", m.BackWorkApprovalFile);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                db.ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("EPCCalenderService.WorkUpdate:" + e.Message);
                return false;
            }
        }

        //設定展延工期
        public int ExtensionDel(int seq)
        {
            string sql = @"delete SupDailyReportExtension where Seq=@Seq";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }
        public List<T> GetExtensionByPrjSeq<T>(int engMainSeq)
        {
            string sql = @"
				SELECT
                    a.Seq,
					a.ExtendDays,
                    a.ApprovalNo,
                    a.ApprovalDate,
                    a.ExtendReason,
                    a.ExtendReasonOther,
                    b.SupDailyReportExtensionSeq
				FROM SupDailyReportExtension a
                left outer join EC_SchEngProgressHeader b on(b.SupDailyReportExtensionSeq=a.Seq)
				WHERE a.EngMainSeq=@EngMainSeq
				ORDER BY a.Seq desc";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);

        }
        //s20230310
        public bool ExtensionUpdate(SupDailyReportExtensionModel m)
        {
            Null2Empty(m);
            try
            {
                string sql = @"
                    update SupDailyReportExtension set
                        ExtendDays = @ExtendDays,
                        ApprovalNo = @ApprovalNo,
                        ApprovalDate = @ApprovalDate,
                        ExtendReason = @ExtendReason,
                        ExtendReasonOther = @ExtendReasonOther,
                        ModifyTime = GetDate(),
                        ModifyUserSeq = @ModifyUserSeq
                    where Seq = @Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@ExtendDays", m.ExtendDays);
                cmd.Parameters.AddWithValue("@ApprovalNo", m.ApprovalNo);
                cmd.Parameters.AddWithValue("@ApprovalDate", m.ApprovalDate);
                cmd.Parameters.AddWithValue("@ExtendReason", m.ExtendReason);
                cmd.Parameters.AddWithValue("@ExtendReasonOther", m.ExtendReasonOther);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                db.ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("EPCCalenderService.ExtensionUpdate:" + e.Message);
                return false;
            }
        }
        public bool ExtensionAdd(SupDailyReportExtensionModel m)
        {
            Null2Empty(m);
            try
            {
                string sql = @"
                    insert into SupDailyReportExtension(
                        EngMainSeq,
                        ExtendDays,
                        ApprovalNo,
                        ApprovalDate,
                        ExtendReason,
                        ExtendReasonOther,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EngMainSeq,
                        @ExtendDays,
                        @ApprovalNo,
                        @ApprovalDate,
                        @ExtendReason,
                        @ExtendReasonOther,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", m.EngMainSeq);
                cmd.Parameters.AddWithValue("@ExtendDays", m.ExtendDays);
                cmd.Parameters.AddWithValue("@ApprovalNo", m.ApprovalNo);
                cmd.Parameters.AddWithValue("@ApprovalDate", m.ApprovalDate);
                cmd.Parameters.AddWithValue("@ExtendReason", m.ExtendReason);
                cmd.Parameters.AddWithValue("@ExtendReasonOther", m.ExtendReasonOther);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                db.ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("EPCCalenderService.ExtensionAdd:" + e.Message);
                return false;
            }
        }

        //設定不計工期
        public int NoDurationDel(int seq)
        {
            string sql = @"delete SupDailyReportNoDuration where Seq=@Seq";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }
        public List<T> GetNoDurationByPrjSeq<T>(int engMainSeq)
        {
            string sql = @"
				SELECT
                    Seq,
					StartDate,
                    EndDate,
                    DaySet,
                    Descript
				FROM SupDailyReportNoDuration
				WHERE EngMainSeq=@EngMainSeq
				ORDER BY Seq desc";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);

        }
        public bool NoDurationAdd(SupDailyReportNoDurationModel m)
        {
            Null2Empty(m);
            try
            {
                string sql = @"
                    insert into SupDailyReportNoDuration(
                        EngMainSeq,
                        StartDate,
                        EndDate,
                        DaySet,
                        Descript,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EngMainSeq,
                        @StartDate,
                        @EndDate,
                        @DaySet,
                        @Descript,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", m.EngMainSeq);
                cmd.Parameters.AddWithValue("@StartDate", m.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", m.EndDate);
                cmd.Parameters.AddWithValue("@DaySet", m.DaySet);
                cmd.Parameters.AddWithValue("@Descript", m.Descript);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                db.ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("EPCCalenderService. NoDurationAdd:" + e.Message);
                return false;
            }
        }

        //設定假日計工期
        public int HolidayDel(int seq)
        {
            string sql = @"delete SupDailyReportHoliday where Seq=@Seq";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }
        public List<T> GetHolidayByPrjSeq<T>(int engMainSeq)
        {
            string sql = @"
				SELECT
                    Seq,
					StartDate,
                    EndDate,
                    DaySet,
                    Descript
				FROM SupDailyReportHoliday
				WHERE EngMainSeq=@EngMainSeq
				ORDER BY Seq desc";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);

        }
        public bool HolidayAdd(SupDailyReportHolidayModel m)
        {
            Null2Empty(m);
            try
            {
                string sql = @"
                    insert into SupDailyReportHoliday(
                        EngMainSeq,
                        StartDate,
                        EndDate,
                        DaySet,
                        Descript,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EngMainSeq,
                        @StartDate,
                        @EndDate,
                        @DaySet,
                        @Descript,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", m.EngMainSeq);
                cmd.Parameters.AddWithValue("@StartDate", m.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", m.EndDate);
                cmd.Parameters.AddWithValue("@DaySet", m.DaySet);
                cmd.Parameters.AddWithValue("@Descript", m.Descript);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                db.ExecuteNonQuery(cmd);
                return true;
            } catch(Exception e) {
                db.TransactionRollback();
                log.Info("EPCCalenderService.HolidayAdd:" + e.Message);
                return false;
            }
        }
    }
}