using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class NcrService : BaseService
    {//NCR程序追蹤改善表

        public List<T> GetItemByConstCheckRecSeq<T>(int constCheckRecSeq)
        {
            string sql = @"SELECT
                        Seq,
                        ConstCheckRecSeq,
                        MissingItem,
                        CauseAnalysis,
                        CorrectiveAction,
                        PreventiveAction,
                        CorrPrevImproveResult,
                        ImproveAuditResult,
                        ProcessTrackDate,
                        TrackCont,
                        CanClose,
                        CloseMemo,
                        FormConfirm
                    FROM NCR
                    where ConstCheckRecSeq=@ConstCheckRecSeq
                        ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@ConstCheckRecSeq", constCheckRecSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //更新
        public bool Update(ConstCheckRecModel recItem, NcrModel report)
        {
            string sql = "";
            SqlCommand cmd;
            Null2Empty(recItem);
            Null2Empty(report);

            db.BeginTransaction();
            try
            {
                if (report.Seq == null)
                {
                    sql = @"insert into NCR(
                            ConstCheckRecSeq,
                            MissingItem,
                            CauseAnalysis,
                            CorrectiveAction,
                            PreventiveAction,
                            CorrPrevImproveResult,
                            --ImproveAuditResult,
                            --ProcessTrackDate,
                            --TrackCont,
                            CanClose,
                            --CloseMemo,
                            FormConfirm,
                            CreateTime,
                            CreateUserSeq,
                            ModifyTime,
                            ModifyUserSeq
                        ) values(
                            @ConstCheckRecSeq,
                            @MissingItem,
                            @CauseAnalysis,
                            @CorrectiveAction,
                            @PreventiveAction,
                            @CorrPrevImproveResult,
                            --@ImproveAuditResult,
                            --@ProcessTrackDate,
                            --@TrackCont,
                            0,
                            --@CloseMemo,
                            0,
                            GetDate(),
                            @ModifyUserSeq,
                            GetDate(),
                            @ModifyUserSeq                        
                        )";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@ConstCheckRecSeq", report.ConstCheckRecSeq);
                }
                else
                {
                    sql = @"update NCR set
                            MissingItem=@MissingItem,
                            CauseAnalysis=@CauseAnalysis,
                            CorrectiveAction=@CorrectiveAction,
                            PreventiveAction=@PreventiveAction,
                            CorrPrevImproveResult=@CorrPrevImproveResult,
                            --ImproveAuditResult=@ImproveAuditResult,
                            --ProcessTrackDate=@ProcessTrackDate,
                            --TrackCont=@TrackCont,
                            --CanClose=@CanClose,
                            --CloseMemo=@CloseMemo,
                            ModifyTime=GetDate(),
                            ModifyUserSeq=@ModifyUserSeq
                        where Seq=@Seq";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", report.Seq);
                }

                cmd.Parameters.AddWithValue("@MissingItem", report.MissingItem);
                cmd.Parameters.AddWithValue("@CauseAnalysis", report.CauseAnalysis);
                cmd.Parameters.AddWithValue("@CorrectiveAction", report.CorrectiveAction);
                cmd.Parameters.AddWithValue("@PreventiveAction", report.PreventiveAction);
                cmd.Parameters.AddWithValue("@CorrPrevImproveResult", report.CorrPrevImproveResult);
                //cmd.Parameters.AddWithValue("@ImproveAuditResult", this.NulltoDBNull(report.ImproveAuditResult));
                //cmd.Parameters.AddWithValue("@ProcessTrackDate", report.ProcessTrackDate);
                //cmd.Parameters.AddWithValue("@TrackCont", report.TrackCont);
                //cmd.Parameters.AddWithValue("@CanClose", this.NulltoDBNull(report.CanClose));
                //cmd.Parameters.AddWithValue("@CloseMemo", report.CloseMemo);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                db.ExecuteNonQuery(cmd);

                if (report.Seq == null)
                {
                    sql = @"SELECT IDENT_CURRENT('NCR') AS NewSeq";
                    cmd = db.GetCommand(sql);
                    DataTable dt = db.GetDataTable(cmd);
                    report.Seq = Convert.ToInt16(dt.Rows[0]["NewSeq"].ToString());
                }

                db.TransactionCommit();

                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("NcrService.Update: " + e.Message);
                log.Info(sql);
                return false;
            }
        }

        //表單確認
        public int NCRConfirm(int seq, int state)
        {
            string sql = sql = @"update NCR set
                            FormConfirm=@FormConfirm,
                            ImproveUserSeq=@ModifyUserSeq,
                            ModifyTime=GetDate(),
                            ModifyUserSeq=@ModifyUserSeq
                        where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            cmd.Parameters.AddWithValue("@FormConfirm", state);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
            return db.ExecuteNonQuery(cmd);
        }

        //審核更新
        public int UpdateApprove(NcrModel ncr)
        {
            Null2Empty(ncr);
            ncr.FormConfirm = (byte)(ncr.ImproveAuditResult.Value == 1 ? 2 : 0);
            string sql = @"
                update NCR set
                    ImproveAuditResult=@ImproveAuditResult,
                    ProcessTrackDate=@ProcessTrackDate,
                    TrackCont=@TrackCont,
                    CanClose=@CanClose,
                    --CloseMemo=@CloseMemo,
                    ApproveUserSeq=@ModifyUserSeq,
                    ApproveDate=GetDate(),
                    FormConfirm=@FormConfirm
                    --ModifyTime=GetDate(),
                    --ModifyUserSeq=@ModifyUserSeq
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", ncr.Seq);

            cmd.Parameters.AddWithValue("@ImproveAuditResult", ncr.ImproveAuditResult);
            cmd.Parameters.AddWithValue("@FormConfirm", ncr.FormConfirm);
            cmd.Parameters.AddWithValue("@ProcessTrackDate", ncr.ProcessTrackDate);
            cmd.Parameters.AddWithValue("@TrackCont", ncr.TrackCont);
            cmd.Parameters.AddWithValue("@CanClose", ncr.CanClose);
            //cmd.Parameters.AddWithValue("@CloseMemo", report.CloseMemo);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
            return db.ExecuteNonQuery(cmd);
    }
    }
}