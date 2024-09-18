using EQC.Common;
using EQC.Models;
using EQC.ViewModel;
using EQC.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EngReportApproveService : BaseService
    {//簽核歷程
        private UnitService unitService = new UnitService();

        //工程簽核流程清單
        public List<T> GetEngReportApproveList<T>(int engReportSeq)
        {
            string sql = @"
                SELECT v.*, u.DisplayName ApproveUser
                FROM [dbo].[view_EngReportApprove] v
                left join UserMain u on u.Seq = ApproveUserSeq
                WHERE v.[EngReportSeq] = @EngReportSeq
                ORDER BY v.[GroupId] DESC, v.[Seq] ASC
                ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngReportSeq", engReportSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //工程簽核流程清單-Doc
        public List<T> GetEngReportApproveListDoc<T>(int engReportSeq)
        {
            string sql = @"
                SELECT *
                FROM [dbo].[view_EngReportApprove]
                WHERE [EngReportSeq] = @EngReportSeq 
	                AND [GroupId] IN (SELECT Max([GroupId]) FROM [dbo].[view_EngReportApprove] WHERE [EngReportSeq] = @EngReportSeq)
                ORDER BY [GroupId] DESC, [Seq] ASC
                ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngReportSeq", engReportSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //工程簽核流程
        public List<T> GetEngReportApprove<T>(int engReportSeq)
        {
            string positionSeq = "";
            string unitSubSeq = "";
            string unitSeq = "";
            Utils.GetUserUnitPosition(ref unitSeq, ref unitSubSeq, ref positionSeq);
            string sql = @"

                DECLARE @IsOWN INT = ISNULL((SELECT COUNT(SEQ) FROM [dbo].[EngReportList] WHERE [Seq] = @EngReportSeq AND [CreateUserSeq] = @UserMainSeq ),0)
                DECLARE @Seq INT = ISNULL((SELECT MIN(Seq) AS Seq FROM [dbo].[EngReportApprove] WHERE ISNULL([ApproveTime],'')='' AND [EngReportSeq] = @EngReportSeq ),0)
                DECLARE @FlowSeq INT = ISNULL(
	                (SELECT MIN(ApprovalWorkFlow) AS Seq 
		                FROM EngReportApprove  ep
		                inner join ApprovalModuleList am on ep.ApprovalModuleListSeq = am.Seq
		                WHERE ISNULL([ApproveTime],'')='' AND [EngReportSeq] = @EngReportSeq )
	
                ,0)
                IF(@IsOWN=0)
                BEGIN
	                DECLARE @GroupId INT = ISNULL((SELECT GroupId FROM [dbo].[view_EngReportApprove] WHERE [Seq] = @Seq),0)
					SELECT FN1.* , ap.* FROM [dbo].[view_EngReportApprove] FN1
					inner join ApprovalPosition ap on ap.ApproverListSeq = FN1.ApprovalModuleListSeq and FN1.EngReportSeq = @EngReportSeq
					where FN1.GroupId = @GroupId 
                                and  ( FN1.[ApprovalWorkFlow] = @FlowSeq  or @FlowSeq > 2 )
                                and (FN1.[UnitSeq] = @UnitSeq or FN1.[UnitSeq] is null) 
                                AND ( FN1.[SubUnitSeq] = @SubUnitSeq or FN1.[SubUnitSeq] is null )
                                AND ap.[PositionSeq] = @PositionSeq

                END
                ELSE
                BEGIN
	                --建立者
	                SELECT *
	                FROM [dbo].[view_EngReportApprove]
	                WHERE [Seq] = @Seq AND [UserMainSeq] = @UserMainSeq
                END
                ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngReportSeq", engReportSeq);
            cmd.Parameters.AddWithValue("@UnitSeq", unitSeq);
            cmd.Parameters.AddWithValue("@SubUnitSeq", unitSubSeq);
            cmd.Parameters.AddWithValue("@PositionSeq", positionSeq);
            cmd.Parameters.AddWithValue("@UserMainSeq", getUserSeq());
            return db.GetDataTableWithClass<T>(cmd);
        }

        //更新工程 FOR 需求評估填報 - 送簽或簽核
        public int Update(EngReportVModel m, EngReportApproveVModel era)
        {
            Null2Empty(m);
            Null2Empty(era);
            string sql = $@"
                DECLARE @FlowSeq INT = ISNULL(
	                (SELECT MIN(ApprovalWorkFlow) AS Seq 
		                FROM EngReportApprove  ep
		                inner join ApprovalModuleList am on ep.ApprovalModuleListSeq = am.Seq
		                WHERE ISNULL([ApproveTime],'')='' AND [EngReportSeq] = {era.EngReportSeq} )
	
                ,0);
                select @FlowSeq;
            ";
            int FlowSeq =0;
            try
            {
                FlowSeq = (int)db.ExecuteScalar(db.GetCommand(sql));
            }
            catch(Exception e)
            {

            }
            var userInfo = Utils.getUserInfo();


            if (userInfo.RoleSeq <= 2 || ( m.CreateUserSeq == getUserSeq() && FlowSeq == 1)) 
            {
                sql = @"
                    update EngReportApprove set 
                        ModifyTime = GetDate()
                        ,ModifyUserSeq = @ModifyUserSeq
                        ,[ApproveTime] = GetDate()
                        ,[ApproveUserSeq] = @ApproveUserSeq
                        ,[Signature] = @Signature
                    where Seq = @Seq; ";
            }
            else 
            {
                sql = @"



                    DECLARE @PSeq INT

                    -- 查詢人員簽和狀態 代表 @PSeq，依此更新 EngReportApprovePosition 時間、人Seq

                    SET @PSeq = ISNULL((SELECT TOP 1 FN1.[Seq]
                                        FROM [dbo].[EngReportApprovePosition] FN1
	                                        INNER JOIN [dbo].[UserUnitPosition] FN2 ON FN1.SubUnitSeq = FN2.UnitSeq AND FN1.PositionSeq = FN2.PositionSeq 
                                        WHERE FN1.[EngReportSeq]=@EngReportSeq AND FN2.UserMainSeq = @ApproveUserSeq),0)

                    UPDATE [dbo].[EngReportApprovePosition] 
                    SET ApproveUserSeq = @ApproveUserSeq, ApproveTime=GETDATE()
                    WHERE Seq = @PSeq

                    -- 依 EngReportApprovePosition 更新 EngReportApprove 簽核狀態

                    UPDATE [dbo].[EngReportApprove]
                    SET [PositionSeq] = ISNULL((SELECT [PositionSeq] FROM [dbo].[EngReportApprovePosition] WHERE Seq = @PSeq),[PositionSeq])
	                    ,[ApproveUserSeq] = ISNULL((SELECT [ApproveUserSeq] FROM [dbo].[EngReportApprovePosition] WHERE Seq = @PSeq),[ApproveUserSeq])
	                    ,[ApproveTime] = GETDATE()
	                    ,[ModifyUserSeq] = @ModifyUserSeq
	                    ,[ModifyTime] = GETDATE()
	                    ,[Signature] = @Signature
                    WHERE [Seq] = @Seq
                        
                    --如果有重新評估

                    UPDATE FN1
                    SET FN1.[ApproveTime]=FN2.[ApproveTime] ,FN1.[ApproveUserSeq]=FN2.[ApproveUserSeq] ,FN1.[Signature]=FN2.[Signature],FN1.ModifyTime = GetDate(),ModifyUserSeq = @ModifyUserSeq
                    FROM [dbo].[EngReportApprove] FN1
	                    INNER JOIN (SELECT * FROM [dbo].[EngReportApprove] WHERE [Seq]=@Seq) FN2 ON FN1.[GroupId]=FN2.[GroupId] AND FN1.[UnitSeq]=FN2.[UnitSeq] AND FN1.[SubUnitSeq]=FN2.[SubUnitSeq] AND FN1.[PositionSeq]=FN2.[PositionSeq]
                    WHERE FN1.[EngReportSeq]=@EngReportSeq AND ISNULL(FN1.[ApproveUserSeq],0)=0	
                    ";
            }

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", era.Seq);
                cmd.Parameters.AddWithValue("@EngReportSeq", era.EngReportSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@ApproveUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@Signature", era.Signature);
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportApproveService.Update: " + e.Message);
                return -1;
            }
        }

        //新增
        //public int Insert(EngReportSignOffProcessModel s)
        //{
        //    Null2Empty(s);
        //    string sql = @"
        //            INSERT INTO [dbo].[EngReportSignOffProcess]
        //                       ([EngReportSeq]
        //                       ,[ContentType]
        //                       ,[SignOffState]
        //                       ,[Memo]
        //                       ,[CreateUserSeq]
        //                       ,[CreateTime])
        //            VALUES(@EngReportSeq
        //             ,@ContentType
        //             ,@SignOffState
        //             ,@Memo
        //             ,@CreateUserSeq
        //             ,GETDATE() );
        //        ";

        //    try
        //    {
        //        SqlCommand cmd = db.GetCommand(sql);
        //        cmd.Parameters.AddWithValue("@EngReportSeq", s.EngReportSeq);
        //        cmd.Parameters.AddWithValue("@ContentType", s.ContentType);
        //        cmd.Parameters.AddWithValue("@SignOffState", s.SignOffState);
        //        cmd.Parameters.AddWithValue("@Memo", s.Memo);
        //        cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
        //        db.ExecuteNonQuery(cmd);

        //        return 0;
        //    }
        //    catch (Exception e)
        //    {
        //        log.Info("EngReportSignOffProcessService.InsertEngReportSignOffProcess: " + e.Message);
        //        return -1;
        //    }
        //}
    }
}