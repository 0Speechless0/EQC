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
                SELECT *
                FROM [dbo].[view_EngReportApprove]
                WHERE [EngReportSeq] = @EngReportSeq
                ORDER BY [GroupId] DESC, [Seq] ASC
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
                ----簽核職稱清單：依簽核流程更新資料
                --UPDATE FN1
                --SET FN1.[SubUnitSeq]=ISNULL(FN5.Seq,0)
	               -- ,FN1.[PositionSeq]=ISNULL(FN4.[PositionSeq],0)
	               -- ,FN1.[UserMainSeq]=0
                --FROM [dbo].[EngReportApprovePosition] FN1
	               -- INNER JOIN [dbo].[ApprovalModuleList] FN2 ON FN1.[ApprovalModuleListSeq] = FN2.[Seq] 
	               -- LEFT OUTER JOIN [dbo].[ApproverList] FN3 ON FN2.[Approver] = FN3.[Seq]
	               -- LEFT OUTER JOIN [dbo].[ApprovalPosition] FN4 ON FN3.Seq = FN4.ApproverListSeq 
	               -- LEFT OUTER JOIN [dbo].[Unit] FN5 ON FN1.[UnitSeq] = FN5.[ParentSeq] AND FN3.[Name] = FN5.[Name]
                --WHERE FN1.[EngReportSeq] = @EngReportSeq AND FN3.[Name] NOT IN ('申請人','提案單位主管')

                ----簽核職稱清單：依簽核流程更新資料(提案單位主管)
                --UPDATE FN1
                --SET FN1.[SubUnitSeq]=@SubUnitSeq
	               -- ,FN1.[PositionSeq]=ISNULL(FN4.[PositionSeq],0)
	               -- ,FN1.[UserMainSeq]=0
                --FROM [dbo].[EngReportApprovePosition] FN1
	               -- INNER JOIN [dbo].[ApprovalModuleList] FN2 ON FN1.[ApprovalModuleListSeq] = FN2.[Seq] 
	               -- LEFT OUTER JOIN [dbo].[ApproverList] FN3 ON FN2.[Approver] = FN3.[Seq]
	               -- LEFT OUTER JOIN [dbo].[ApprovalPosition] FN4 ON FN3.Seq = FN4.ApproverListSeq 
                --WHERE FN1.[EngReportSeq] = @EngReportSeq AND RTRIM(FN3.[Name]) IN ('提案單位主管')

                ----簽核清單：更新尚未簽核的資料
                --UPDATE FN1
                --SET FN1.[SubUnitSeq]=FN2.[SubUnitSeq], FN1.[PositionSeq]=FN2.[PositionSeq], FN1.[UserMainSeq]=FN2.[UserMainSeq]
                --FROM [dbo].[EngReportApprove] FN1
	               -- INNER JOIN (SELECT * FROM [EngReportApprovePosition] WHERE Seq IN (SELECT MIN(Seq) FROM [EngReportApprovePosition] WHERE EngReportSeq = @EngReportSeq GROUP BY [ApprovalModuleListSeq])) FN2 
		              --  ON FN1.[ApprovalModuleListSeq] = FN2.[ApprovalModuleListSeq]
                --WHERE FN1.EngReportSeq = @EngReportSeq AND ISNULL(FN1.ApproveUserSeq,0)=0

                DECLARE @IsOWN INT = ISNULL((SELECT COUNT(SEQ) FROM [dbo].[EngReportList] WHERE [Seq] = @EngReportSeq AND [CreateUserSeq] = @UserMainSeq ),0)
                DECLARE @Seq INT = ISNULL((SELECT MIN(Seq) AS Seq FROM [dbo].[EngReportApprove] WHERE ISNULL([ApproveTime],'')='' AND [EngReportSeq] = @EngReportSeq ),0)
                IF(@IsOWN=0)
                BEGIN
	                DECLARE @GroupId INT = ISNULL((SELECT GroupId FROM [dbo].[view_EngReportApprove] WHERE [Seq] = @Seq),0)
	                SELECT FN1.*
	                FROM [dbo].[view_EngReportApprove] FN1
		                INNER JOIN (SELECT * FROM [dbo].[EngReportApprovePosition] 
					                WHERE [EngReportSeq] IN (SELECT [EngReportSeq] FROM [dbo].[view_EngReportApprove] WHERE [Seq] = @Seq) 
						                AND [UnitSeq] = @UnitSeq AND [SubUnitSeq] = @SubUnitSeq AND [PositionSeq] = @PositionSeq) FN2 ON FN1.ApprovalModuleListSeq = FN2.ApprovalModuleListSeq 
	                WHERE FN1.GroupId=@GroupId AND FN1.[EngReportSeq] = @EngReportSeq
	                --SELECT *
	                --FROM [dbo].[view_EngReportApprove]
	                --WHERE [Seq] = @Seq AND [UnitSeq] = @UnitSeq AND [SubUnitSeq] = @SubUnitSeq AND [PositionSeq] = @PositionSeq
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
            string sql = "";
            if (Utils.getUserInfo().IsAdmin || m.CreateUserSeq == getUserSeq()) 
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

                    SET @PSeq = ISNULL((SELECT TOP 1 FN1.[Seq]
                                        FROM [dbo].[EngReportApprovePosition] FN1
	                                        INNER JOIN [dbo].[UserUnitPosition] FN2 ON FN1.SubUnitSeq = FN2.UnitSeq AND FN1.PositionSeq = FN2.PositionSeq 
                                        WHERE FN1.[EngReportSeq]=@EngReportSeq AND FN2.UserMainSeq = @ApproveUserSeq),0)

                    UPDATE [dbo].[EngReportApprovePosition] 
                    SET ApproveUserSeq = @ApproveUserSeq, ApproveTime=GETDATE()
                    WHERE Seq = @PSeq

                    UPDATE [dbo].[EngReportApprove]
                    SET [PositionSeq] = ISNULL((SELECT [PositionSeq] FROM [dbo].[EngReportApprovePosition] WHERE Seq = @PSeq),[PositionSeq])
	                    ,[ApproveUserSeq] = ISNULL((SELECT [ApproveUserSeq] FROM [dbo].[EngReportApprovePosition] WHERE Seq = @PSeq),[ApproveUserSeq])
	                    ,[ApproveTime] = GETDATE()
	                    ,[ModifyUserSeq] = @ModifyUserSeq
	                    ,[ModifyTime] = GETDATE()
	                    ,[Signature] = @Signature
                    WHERE [Seq] = @Seq

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