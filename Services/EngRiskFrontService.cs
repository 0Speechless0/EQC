using EQC.Common;
using EQC.EDMXModel;
using EQC.Models;
using EQC.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace EQC.Services
{
    public class EngRiskFrontService : BaseService
    {//施工風險評估報告產製
        public List<T> GetItem<T>(int id)
        {
            string sql = @"
                select *
	                  ,dbo.DelGuidStr(ISNULL([PlanScopeFile],'')) AS [PlanScopeFileName]
	                  ,dbo.DelGuidStr(ISNULL([DesignConceptFile],'')) AS [DesignConceptFileName]
	                  ,dbo.DelGuidStr(ISNULL([DesignSelectionFile],'')) AS [DesignSelectionFileName]
	                  ,dbo.DelGuidStr(ISNULL([DesignStageRiskResultFile],'')) AS [DesignStageRiskResultFileName]
	                  ,dbo.DelGuidStr(ISNULL([RiskTrackingFile],'')) AS [RiskTrackingFileName]
	                  ,dbo.DelGuidStr(ISNULL([ConclusionFile],'')) AS [ConclusionFileName]
	                  ,dbo.DelGuidStr(ISNULL([FinishFile],'')) AS [FinishFileName]
                from EngRiskFrontList
                where EngNo in (select EngNo from EngMain where Seq = @id )
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@id", id);

            return db.GetDataTableWithClass<T>(cmd);
        }

        //新增施工風險評估主檔
        public int AddEngRiskMain(int id)
        {
            try
            {
                string unitSubSeq = "";
                string unitSeq = "";
                Utils.GetUserUnit(ref unitSeq, ref unitSubSeq);

                string sql = @"
                    DECLARE @EngRiskFrontSeq INT;
                    DECLARE @ExecUnitSeq INT, @ExecSubUnitSeq INT, @OrganizerUserSeq INT

                    SELECT @ExecUnitSeq = [ExecUnitSeq]
                          ,@ExecSubUnitSeq = [ExecSubUnitSeq]
                          ,@OrganizerUserSeq = [OrganizerUserSeq]
                    FROM [dbo].[EngMain] 
                    WHERE [Seq] = @id

                    INSERT INTO [dbo].[EngRiskFrontList]
                           ([EngNo],[CreateTime],[CreateUserSeq],[ModifyTime],[ModifyUserSeq])
                    SELECT EngNo,GETDATE() ,@CreateUserSeq ,GETDATE() ,@ModifyUserSeq 
                    FROM [dbo].[EngMain] 
                    WHERE Seq = @id
                    SET @EngRiskFrontSeq = @@IDENTITY;

	                --施工風險評估小組
	                IF((SELECT COUNT(*) FROM [dbo].[EngRiskFrontEvaluationTeam] WHERE [EngRiskFrontSeq]=@EngRiskFrontSeq ) = 0 )
	                BEGIN
		                INSERT INTO [dbo].[EngRiskFrontEvaluationTeam]
					                ([EngRiskFrontSeq],[JobTitle],[OrganizerUnitSeq],[UnitSeq],[PrincipalSeq],[CreateTime],[CreateUserSeq],[ModifyTime],[ModifyUserSeq])
			                SELECT @EngRiskFrontSeq ,'召集人' ,@ExecUnitSeq ,@ExecSubUnitSeq ,@OrganizerUserSeq ,GETDATE() ,@CreateUserSeq ,GETDATE() ,@ModifyUserSeq UNION ALL
			                SELECT @EngRiskFrontSeq ,'專案主辦人員' ,@ExecUnitSeq ,null ,null ,GETDATE() ,@CreateUserSeq ,GETDATE() ,@ModifyUserSeq UNION ALL
			                SELECT @EngRiskFrontSeq ,'職業安全衛生人員' ,@ExecUnitSeq ,null ,null ,GETDATE() ,@CreateUserSeq ,GETDATE() ,@ModifyUserSeq UNION ALL
			                SELECT @EngRiskFrontSeq ,'工址環境現況調查人員' ,@ExecUnitSeq ,null ,null ,GETDATE() ,@CreateUserSeq ,GETDATE() ,@ModifyUserSeq UNION ALL
			                SELECT @EngRiskFrontSeq ,'工程設計主辦人員' ,@ExecUnitSeq ,null ,null ,GETDATE() ,@CreateUserSeq ,GETDATE() ,@ModifyUserSeq UNION ALL
			                SELECT @EngRiskFrontSeq ,'施工規劃人員' ,@ExecUnitSeq ,null ,null ,GETDATE() ,@CreateUserSeq ,GETDATE() ,@ModifyUserSeq UNION ALL
			                SELECT @EngRiskFrontSeq ,'規範編訂人員' ,@ExecUnitSeq ,null ,null ,GETDATE() ,@CreateUserSeq ,GETDATE() ,@ModifyUserSeq UNION ALL
			                SELECT @EngRiskFrontSeq ,'預算編製人員' ,@ExecUnitSeq ,null ,null ,GETDATE() ,@CreateUserSeq ,GETDATE() ,@ModifyUserSeq UNION ALL
			                SELECT @EngRiskFrontSeq ,'繪圖人員' ,@ExecUnitSeq ,null ,null ,GETDATE() ,@CreateUserSeq ,GETDATE() ,@ModifyUserSeq;  
	                END

	                --設計階段施工方法檢討評選表
	                IF((SELECT COUNT(*) FROM [dbo].[EngRiskFrontProjectSelection] WHERE [EngRiskFrontSeq]=@EngRiskFrontSeq AND [PSType]=1 ) = 0 )
	                BEGIN
		                INSERT INTO [dbo].[EngRiskFrontProjectSelection]
				                   ([EngRiskFrontSeq],[PSType],[PlanOverview],[Weight1],[Weight2],[Weight3],[Weight4],[Weight5],[Weight6],[Weight7],[WeightSort]
				                   ,[CreateTime],[CreateUserSeq],[ModifyTime],[ModifyUserSeq])
		                SELECT @EngRiskFrontSeq ,1 ,'' ,'功能' ,'技術' ,'成本' ,'工期' ,'工址環境' ,'安全' ,'維護' ,0 ,GETDATE() ,@CreateUserSeq ,GETDATE() ,@ModifyUserSeq UNION ALL
		                SELECT @EngRiskFrontSeq ,2 ,'' ,'25％' ,'10％' ,'15％' ,'10％' ,'10％' ,'15％' ,'15％' ,0 ,GETDATE() ,@CreateUserSeq ,GETDATE() ,@ModifyUserSeq 
	                END

	                --施工風險分項工程-主檔
                    DECLARE @SubProjectSeq INT;
                    DECLARE @Seq INT
                    DECLARE CS Cursor FOR SELECT [Seq] FROM [dbo].[EngRiskFrontSubProjectListTp]
                    Open CS 
                    Fetch NEXT FROM CS INTO @Seq
                    While (@@FETCH_STATUS <> -1)
                    BEGIN
	                    INSERT INTO [dbo].[EngRiskFrontSubProjectList]
				                    ([EngRiskFrontSeq],[ExcelNo],[SubProjectName],[SubProjectJson],[ExistingProtectiveFacilities]
				                    ,[Equipment],[EngControl],[ManagementControl],[PersonalProtectiveEquipment],[IsEnabled]
				                    ,[CreateTime],[CreateUserSeq],[ModifyTime],[ModifyUserSeq])
	                    SELECT @EngRiskFrontSeq,[ExcelNo],[SubProjectName],[SubProjectJson],[ExistingProtectiveFacilities]
			                    ,[Equipment],[EngControl],[ManagementControl],[PersonalProtectiveEquipment],1
			                    ,GETDATE(),@CreateUserSeq,GETDATE(),@ModifyUserSeq
	                    FROM [dbo].[EngRiskFrontSubProjectListTp]
                        WHERE [Seq]=@Seq
	                    SET @SubProjectSeq = @@IDENTITY;

	                    INSERT INTO [dbo].[EngRiskFrontSubProjectDetail]
				                    ([SubProjectSeq],[Level],[ParentSeq],[StepNo],[StepName]
				                    ,[HazardTypeSeq],[PossibleRiskSituation],[Possibility],[Severity],[IsAcceptable]
				                    ,[RisksAndOpportunitiesMeasure],[PrincipalSeq],[SummaryOfExecutiveResults],[IsEffect]
				                    ,[CreateTime],[CreateUser],[ModifyTime],[ModifyUser])
	                    SELECT @SubProjectSeq,[Level],[ParentSeq],[StepNo],[StepName]
			                    ,[HazardTypeSeq],[PossibleRiskSituation],[Possibility],[Severity],[IsAcceptable]
			                    ,[RisksAndOpportunitiesMeasure],[PrincipalSeq],[SummaryOfExecutiveResults],[IsEffect]
			                    ,GETDATE(),@CreateUserSeq,GETDATE(),@ModifyUserSeq
	                    FROM [dbo].[EngRiskFrontSubProjectDetailTp]
	                    WHERE [SubProjectSeq]=@Seq
                    Fetch NEXT FROM CS INTO @Seq
                    END
                    CLOSE CS
                    DEALLOCATE CS   

                    ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id", id);
                //cmd.Parameters.AddWithValue("@UnitSeq", unitSeq);
                //cmd.Parameters.AddWithValue("@UnitSubSeq", unitSubSeq);
                cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngRiskFrontService.AddEngRiskMain " + e.Message);
                return -1;
            }
        }

        //更新施工風險評估主檔
        public int UpdateEngRiskMain(EngRiskFrontListVModel m)
        {
            Null2Empty(m);
            try
            {
                string sql = @"
                    UPDATE [dbo].[EngRiskFrontList]
                       SET [PlanOriginAndTarget] = @PlanOriginAndTarget
                          ,[PlanScope] = @PlanScope
                          --,[PlanScopeFile] = @PlanScopeFile
                          ,[PlanEnvironment] = @PlanEnvironment
                          ,[DesignConcept] = @DesignConcept
                          --,[DesignConceptFile] = @DesignConceptFile
                          ,[DesignStudy] = @DesignStudy
                          ,[DesignPrecautions] = @DesignPrecautions
                          ,[DesignSelection] = @DesignSelection
                          --,[DesignSelectionFile] = @DesignSelectionFile
                          ,[DesignStageRiskResult] = @DesignStageRiskResult
                          --,[DesignStageRiskResultFile] = @DesignStageRiskResultFile
                          ,[RiskTracking] = @RiskTracking
                          --,[RiskTrackingFile] = @RiskTrackingFile
                          ,[Conclusion] = @Conclusion
                          --,[ConclusionFile] = @ConclusionFile
                          --,[FinishFile] = @FinishFile
                          ,[IsFinish] = @IsFinish
                          ,[ModifyTime] = GETDATE()
                          ,[ModifyUserSeq] = @ModifyUserSeq
                     WHERE [EngNo] = @EngNo
                    ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngNo", m.EngNo);
                cmd.Parameters.AddWithValue("@PlanOriginAndTarget", m.PlanOriginAndTarget);
                cmd.Parameters.AddWithValue("@PlanScope", m.PlanScope);
                cmd.Parameters.AddWithValue("@PlanScopeFile", m.PlanScopeFile);
                cmd.Parameters.AddWithValue("@PlanEnvironment", m.PlanEnvironment);
                cmd.Parameters.AddWithValue("@DesignConcept", m.DesignConcept);
                cmd.Parameters.AddWithValue("@DesignConceptFile", m.DesignConceptFile);
                cmd.Parameters.AddWithValue("@DesignStudy", m.DesignStudy);
                cmd.Parameters.AddWithValue("@DesignPrecautions", m.DesignPrecautions);
                cmd.Parameters.AddWithValue("@DesignSelection", m.DesignSelection);
                cmd.Parameters.AddWithValue("@DesignSelectionFile", m.DesignSelectionFile);
                cmd.Parameters.AddWithValue("@DesignStageRiskResult", m.DesignStageRiskResult);
                cmd.Parameters.AddWithValue("@DesignStageRiskResultFile", m.DesignStageRiskResultFile);
                cmd.Parameters.AddWithValue("@RiskTracking", m.RiskTracking);
                cmd.Parameters.AddWithValue("@RiskTrackingFile", m.RiskTrackingFile);
                cmd.Parameters.AddWithValue("@Conclusion", m.Conclusion);
                cmd.Parameters.AddWithValue("@ConclusionFile", m.ConclusionFile);
                cmd.Parameters.AddWithValue("@FinishFile", m.FinishFile);
                cmd.Parameters.AddWithValue("@IsFinish", m.IsFinish);
                cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngRiskFrontService.UpdateEngRiskMain " + e.Message);
                return -1;
            }
        }

        //更新施工風險評估主檔-上傳附件
        public int UpdateEngRiskMain(EngRiskFrontListVModel m, string fileType)
        {
            Null2Empty(m);
            string sql = @"
            update EngRiskFrontList set 
                ModifyTime = GetDate(),
                ModifyUserSeq = @ModifyUserSeq"
                + ((fileType == "A1") ? " , PlanScopeFile='" + m.PlanScopeFile + "'" : "")
                + ((fileType == "A2") ? " , DesignConceptFile='" + m.DesignConceptFile + "'" : "")
                + ((fileType == "A3") ? " , DesignSelectionFile='" + m.DesignSelectionFile + "'" : "")
                + ((fileType == "A4") ? " , DesignStageRiskResultFile='" + m.DesignStageRiskResultFile + "'" : "")
                + ((fileType == "A5") ? " , RiskTrackingFile='" + m.RiskTrackingFile + "'" : "")
                + ((fileType == "A6") ? " , ConclusionFile='" + m.ConclusionFile + "'" : "")
                + ((fileType == "A7") ? " , FinishFile='" + m.FinishFile + "'" : "")
            + " where Seq=@Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngRiskFrontService.UpdateEngReportForUA: " + e.Message);
                return -1;
            }
        }

        //更新施工風險評估主檔
        public int UpdateEngRiskMainByLock(EngRiskFrontListVModel m)
        {
            Null2Empty(m);
            try
            {
                string sql = @"
                    UPDATE [dbo].[EngRiskFrontList]
                       SET [LockState] = @LockState
                     WHERE [EngNo] = @EngNo
                    ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngNo", m.EngNo);
                cmd.Parameters.AddWithValue("@LockState", m.LockState);
                db.ExecuteNonQuery(cmd);
                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngRiskFrontService.UpdateEngRiskMainByLock " + e.Message);
                return -1;
            }
        }

        //下載
        public List<T> GetDownloadFile<T>(int seq, string fileNo)
        {
            string col = "";
            switch (fileNo)
            {
                case "A1": col = ",PlanScopeFile as FileName"; break;
                case "A2": col = ",DesignConceptFile as FileName"; break;
                case "A3": col = ",DesignSelectionFile as FileName"; break;
                case "A4": col = ",DesignStageRiskResultFile as FileName"; break;
                case "A5": col = ",RiskTrackingFile as FileName"; break;
                case "A6": col = ",ConclusionFile as FileName"; break;
                case "A7": col = ",FinishFile as FileName"; break;
                default: col = ""; break;
            }
            string sql = @"
                select
                    Seq " + col + @"
                from EngRiskFrontList
                where EngNo in (select EngNo from EngMain where Seq = @Seq )
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        //讀取對策處置人員
        internal List<SelectVM> GetUserPrincipalList(int EngRiskFrontSeq)
        {
            string sql = @"
                SELECT CAST(FN1.[PrincipalSeq] AS VARCHAR(10)) AS Value
	                  ,FN2.DisplayName AS Text
                  FROM [dbo].[EngRiskFrontEvaluationTeam] FN1 INNER JOIN [dbo].[UserMain] FN2 ON FN1.[PrincipalSeq]=FN2.[Seq]
                WHERE FN1.[EngRiskFrontSeq] = @EngRiskFrontSeq
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngRiskFrontSeq", EngRiskFrontSeq);
            return db.GetDataTableWithClass<SelectVM>(cmd);
        }

        #region 工程計畫概要 - 工程功能需求

        public List<T> GetEngRiskFrontProjectOutlineFunction<T>(int EngRiskFrontSeq)
        {
            string sql = @"
                SELECT FN1.[Seq]
                      ,FN1.[EngRiskFrontSeq]
                      ,FN1.[EngFunction]
                      ,FN1.[PotentialHazard]
                      ,FN1.[HazardCountermeasures]
                      ,FN1.[PrincipalSeq]
	                  ,FN2.[DisplayName] AS [PrincipalName]
                      ,FN1.[EngMemo]
                      ,FN1.[CreateTime]
                      ,FN1.[CreateUserSeq]
                      ,FN1.[ModifyTime]
                      ,FN1.[ModifyUserSeq]
                FROM [dbo].[EngRiskFrontProjectOutlineFunction] FN1
	                LEFT OUTER JOIN [dbo].[UserMain] FN2 ON FN1.[PrincipalSeq]=FN2.[Seq]
                WHERE FN1.[EngRiskFrontSeq] = @EngRiskFrontSeq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngRiskFrontSeq", EngRiskFrontSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //新增
        public int AddEngRiskFrontProjectOutlineFunction(EngRiskFrontProjectOutlineFunctionVModel m)
        {
            try
            {
                string sql = @"
                    INSERT INTO [dbo].[EngRiskFrontProjectOutlineFunction]
                               ([EngRiskFrontSeq],[EngFunction],[PotentialHazard],[HazardCountermeasures],[PrincipalSeq],[EngMemo]
                               ,[CreateTime],[CreateUserSeq],[ModifyTime],[ModifyUserSeq])
                         VALUES
                               (@EngRiskFrontSeq ,@EngFunction ,@PotentialHazard  ,@HazardCountermeasures  ,@PrincipalSeq ,@EngMemo 
                               ,GETDATE() ,@CreateUserSeq ,GETDATE() ,@ModifyUserSeq)
                    ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngRiskFrontSeq", m.EngRiskFrontSeq);
                cmd.Parameters.AddWithValue("@EngFunction", m.EngFunction);
                cmd.Parameters.AddWithValue("@PotentialHazard", m.PotentialHazard);
                cmd.Parameters.AddWithValue("@HazardCountermeasures", m.HazardCountermeasures);
                cmd.Parameters.AddWithValue("@PrincipalSeq", m.PrincipalSeq);
                cmd.Parameters.AddWithValue("@EngMemo", m.EngMemo);
                cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngRiskFrontService.AddEngRiskFrontProjectOutlineFunction " + e.Message);
                return -1;
            }
        }
        //更新
        public int UpdateEngRiskFrontProjectOutlineFunction(EngRiskFrontProjectOutlineFunctionVModel m)
        {
            Null2Empty(m);
            string sql = @"
            update EngRiskFrontProjectOutlineFunction 
            set ModifyTime = GetDate()
                ,ModifyUserSeq = @ModifyUserSeq

                ,[EngFunction] = @EngFunction
                ,[PotentialHazard] = @PotentialHazard
                ,[HazardCountermeasures] = @HazardCountermeasures
                ,[PrincipalSeq] = @PrincipalSeq
                ,[EngMemo] = @EngMemo
            WHERE [Seq] = @Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@EngFunction", m.EngFunction);
                cmd.Parameters.AddWithValue("@PotentialHazard", m.PotentialHazard);
                cmd.Parameters.AddWithValue("@HazardCountermeasures", m.HazardCountermeasures);
                cmd.Parameters.AddWithValue("@PrincipalSeq", m.PrincipalSeq);
                cmd.Parameters.AddWithValue("@EngMemo", m.EngMemo);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngRiskFrontService.UpdateEngRiskFrontProjectOutlineFunction: " + e.Message);
                return -1;
            }
        }
        //刪除
        public int DelEngRiskFrontProjectOutlineFunction(int Seq)
        {
            try
            {
                string sql = @"
                    DELETE FROM [dbo].[EngRiskFrontProjectOutlineFunction] WHERE [Seq] = @Seq;";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", Seq);
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                //db.TransactionRollback();
                log.Info("EngRiskFrontService.DelEngRiskFrontProjectOutlineFunction" + e.Message);
                return -1;
            }
        }

        #endregion 

        #region 工程計畫概要 - 工址環境現況

        public List<T> GetEngRiskFrontProjectOutlineSiteEnvironment<T>(int EngRiskFrontSeq)
        {
            string sql = @"
                SELECT FN1.[Seq]
                      ,FN1.[EngRiskFrontSeq]
                      ,FN1.[SiteEnvironment]
                      ,FN1.[PotentialHazard]
                      ,FN1.[HazardCountermeasures]
                      ,FN1.[PrincipalSeq]
	                  ,FN2.[DisplayName] AS [PrincipalName]
                      ,FN1.[EngMemo]
                      ,FN1.[CreateTime]
                      ,FN1.[CreateUserSeq]
                      ,FN1.[ModifyTime]
                      ,FN1.[ModifyUserSeq]
                FROM [dbo].[EngRiskFrontProjectOutlineSiteEnvironment] FN1
	                LEFT OUTER JOIN [dbo].[UserMain] FN2 ON FN1.[PrincipalSeq]=FN2.[Seq]
                WHERE FN1.[EngRiskFrontSeq] = @EngRiskFrontSeq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngRiskFrontSeq", EngRiskFrontSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //新增
        public int AddEngRiskFrontProjectOutlineSiteEnvironment(EngRiskFrontProjectOutlineSiteEnvironmentVModel m)
        {
            try
            {
                string sql = @"
                    INSERT INTO [dbo].[EngRiskFrontProjectOutlineSiteEnvironment]
                               ([EngRiskFrontSeq],[SiteEnvironment],[PotentialHazard],[HazardCountermeasures],[PrincipalSeq],[EngMemo]
                               ,[CreateTime],[CreateUserSeq],[ModifyTime],[ModifyUserSeq])
                         VALUES
                               (@EngRiskFrontSeq ,@SiteEnvironment ,@PotentialHazard  ,@HazardCountermeasures  ,@PrincipalSeq ,@EngMemo 
                               ,GETDATE() ,@CreateUserSeq ,GETDATE() ,@ModifyUserSeq)
                    ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngRiskFrontSeq", m.EngRiskFrontSeq);
                cmd.Parameters.AddWithValue("@SiteEnvironment", m.SiteEnvironment);
                cmd.Parameters.AddWithValue("@PotentialHazard", m.PotentialHazard);
                cmd.Parameters.AddWithValue("@HazardCountermeasures", m.HazardCountermeasures);
                cmd.Parameters.AddWithValue("@PrincipalSeq", m.PrincipalSeq);
                cmd.Parameters.AddWithValue("@EngMemo", m.EngMemo);
                cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngRiskFrontService.AddEngRiskFrontProjectOutlineSiteEnvironment " + e.Message);
                return -1;
            }
        }
        //更新
        public int UpdateEngRiskFrontProjectOutlineSiteEnvironment(EngRiskFrontProjectOutlineSiteEnvironmentVModel m)
        {
            Null2Empty(m);
            string sql = @"
            update EngRiskFrontProjectOutlineSiteEnvironment 
            set ModifyTime = GetDate()
                ,ModifyUserSeq = @ModifyUserSeq

                ,[SiteEnvironment] = @SiteEnvironment
                ,[PotentialHazard] = @PotentialHazard
                ,[HazardCountermeasures] = @HazardCountermeasures
                ,[PrincipalSeq] = @PrincipalSeq
                ,[EngMemo] = @EngMemo
            WHERE [Seq] = @Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@SiteEnvironment", m.SiteEnvironment);
                cmd.Parameters.AddWithValue("@PotentialHazard", m.PotentialHazard);
                cmd.Parameters.AddWithValue("@HazardCountermeasures", m.HazardCountermeasures);
                cmd.Parameters.AddWithValue("@PrincipalSeq", m.PrincipalSeq);
                cmd.Parameters.AddWithValue("@EngMemo", m.EngMemo);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngRiskFrontService.UpdateEngRiskFrontProjectOutlineSiteEnvironment: " + e.Message);
                return -1;
            }
        }
        //刪除
        public int DelEngRiskFrontProjectOutlineSiteEnvironment(int Seq)
        {
            try
            {
                string sql = @"
                    DELETE FROM [dbo].[EngRiskFrontProjectOutlineSiteEnvironment] WHERE [Seq] = @Seq;";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", Seq);
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                //db.TransactionRollback();
                log.Info("EngRiskFrontService.DelEngRiskFrontProjectOutlineSiteEnvironment" + e.Message);
                return -1;
            }
        }

        #endregion 

        #region 準備作業 - 施工風險評估小組

        public List<T> GetEngRiskFrontEvaluationTeam<T>(int EngRiskFrontSeq)
        {
            string sql = @"
                SELECT FN1.[Seq]
                      ,FN1.[EngRiskFrontSeq]
                      ,FN1.[JobTitle]
                      ,FN1.[OrganizerUnitSeq]
	                  ,FN2.[Name] AS [OrganizerUnitName]
                      ,FN1.[UnitSeq]
	                  ,FN3.[Name] AS [UnitName]
                      ,FN1.[PrincipalSeq]
	                  ,FN4.[DisplayName] AS [PrincipalName]
                      ,FN1.[Memo]
                FROM [dbo].[EngRiskFrontEvaluationTeam] FN1
	                LEFT OUTER JOIN [dbo].[Unit] FN2 ON FN1.[OrganizerUnitSeq]=FN2.[Seq]
	                LEFT OUTER JOIN [dbo].[Unit] FN3 ON FN1.[UnitSeq]=FN3.[Seq]
	                LEFT OUTER JOIN [dbo].[UserMain] FN4 ON FN1.[PrincipalSeq]=FN4.[Seq]
                WHERE FN1.[EngRiskFrontSeq] = @EngRiskFrontSeq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngRiskFrontSeq", EngRiskFrontSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //新增
        public int AddEngRiskFrontEvaluationTeam(EngRiskFrontEvaluationTeamModel m)
        {
            try
            {
                string sql = @"
                    INSERT INTO [dbo].[EngRiskFrontEvaluationTeam]
                               ([EngRiskFrontSeq],[JobTitle],[OrganizerUnitSeq],[UnitSeq],[PrincipalSeq],[Memo]
                               ,[CreateTime],[CreateUserSeq],[ModifyTime],[ModifyUserSeq])
                         VALUES
                               (@EngRiskFrontSeq ,@JobTitle ,@OrganizerUnitSeq  ,@UnitSeq  ,@PrincipalSeq  ,@Memo 
                               ,GETDATE() ,@CreateUserSeq ,GETDATE() ,@ModifyUserSeq)
                    ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngRiskFrontSeq", m.EngRiskFrontSeq);
                cmd.Parameters.AddWithValue("@JobTitle", m.JobTitle);
                cmd.Parameters.AddWithValue("@OrganizerUnitSeq", m.OrganizerUnitSeq);
                cmd.Parameters.AddWithValue("@UnitSeq", m.UnitSeq);
                cmd.Parameters.AddWithValue("@PrincipalSeq", m.PrincipalSeq);
                cmd.Parameters.AddWithValue("@Memo", m.Memo);
                cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngRiskFrontService.AddEngRiskFrontEvaluationTeam " + e.Message);
                return -1;
            }
        }
        //更新
        public int UpdateEngRiskFrontEvaluationTeam(EngRiskFrontEvaluationTeamModel m)
        {
            Null2Empty(m);
            string sql = @"
            update EngRiskFrontEvaluationTeam 
            set ModifyTime = GetDate()
                ,ModifyUserSeq = @ModifyUserSeq

                ,[OrganizerUnitSeq] = @OrganizerUnitSeq
                ,[UnitSeq] = @UnitSeq
                ,[PrincipalSeq] = @PrincipalSeq
                ,[Memo] = @Memo
            WHERE [Seq] = @Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@EngRiskFrontSeq", m.EngRiskFrontSeq);
                cmd.Parameters.AddWithValue("@JobTitle", m.JobTitle);
                cmd.Parameters.AddWithValue("@OrganizerUnitSeq", m.OrganizerUnitSeq);
                cmd.Parameters.AddWithValue("@UnitSeq", m.UnitSeq);
                cmd.Parameters.AddWithValue("@PrincipalSeq", m.PrincipalSeq);
                cmd.Parameters.AddWithValue("@Memo", m.Memo);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngRiskFrontService.UpdateEngRiskFrontEvaluationTeam: " + e.Message);
                return -1;
            }
        }
        //刪除
        public int DelEngRiskFrontEvaluationTeam(int Seq)
        {
            try
            {
                string sql = @"
                    DELETE FROM [dbo].[EngRiskFrontEvaluationTeam] WHERE [Seq] = @Seq;";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", Seq);
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                //db.TransactionRollback();
                log.Info("EngRiskFrontService.DelEngRiskFrontEvaluationTeam" + e.Message);
                return -1;
            }
        }

        #endregion 

        #region 設計方案評選 - 方案項目權重

        public List<T> GetEngRiskFrontProjectSelection<T>(int EngRiskFrontSeq)
        {
            string sql = @"
                select *
                from EngRiskFrontProjectSelection
                where EngRiskFrontSeq = @EngRiskFrontSeq
                order by [WeightSort]
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngRiskFrontSeq", EngRiskFrontSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //新增
        public int AddEngRiskFrontProjectSelection(EngRiskFrontProjectSelectionVModel m)
        {
            try
            {
                string sql = @"
                    INSERT INTO [dbo].[EngRiskFrontProjectSelection]
                               ([EngRiskFrontSeq],[PSType],[PlanOverview],[Weight1],[Weight2],[Weight3],[Weight4],[Weight5],[Weight6],[Weight7],[TWeight],[WeightSort]
                               ,[CreateTime],[CreateUserSeq],[ModifyTime],[ModifyUserSeq])
                         VALUES
                               (@EngRiskFrontSeq ,@PSType ,@PlanOverview  ,@Weight1  ,@Weight2  ,@Weight3  ,@Weight4  ,@Weight5  ,@Weight6  ,@Weight7  ,@TWeight  ,@WeightSort 
                               ,GETDATE() ,@CreateUserSeq ,GETDATE() ,@ModifyUserSeq);

                    SELECT [Seq],[EngRiskFrontSeq],ROW_NUMBER() OVER (ORDER BY [TWeight] DESC) as [ROW_NUMBER_SORT]
                    INTO #TMP
                    FROM [dbo].[EngRiskFrontProjectSelection] FN1
                    WHERE [PSType]=3 AND [EngRiskFrontSeq]=@EngRiskFrontSeq

                    UPDATE FN1
                    SET FN1.[WeightSort] = FN2.[ROW_NUMBER_SORT]
                    FROM [dbo].[EngRiskFrontProjectSelection] FN1
	                    INNER JOIN #TMP FN2 ON FN1.Seq = FN2.Seq 
                    ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngRiskFrontSeq", m.EngRiskFrontSeq);
                cmd.Parameters.AddWithValue("@PSType", m.PSType);
                cmd.Parameters.AddWithValue("@PlanOverview", m.PlanOverview);
                cmd.Parameters.AddWithValue("@Weight1", m.Weight1);
                cmd.Parameters.AddWithValue("@Weight2", m.Weight2);
                cmd.Parameters.AddWithValue("@Weight3", m.Weight3);
                cmd.Parameters.AddWithValue("@Weight4", m.Weight4);
                cmd.Parameters.AddWithValue("@Weight5", m.Weight5);
                cmd.Parameters.AddWithValue("@Weight6", m.Weight6);
                cmd.Parameters.AddWithValue("@Weight7", m.Weight7);
                cmd.Parameters.AddWithValue("@TWeight", m.TWeight);
                cmd.Parameters.AddWithValue("@WeightSort", m.WeightSort);
                cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngRiskFrontService.AddEngRiskFrontProjectSelection " + e.Message);
                return -1;
            }
        }
        //更新
        public int UpdateEngRiskFrontProjectSelection(EngRiskFrontProjectSelectionVModel m)
        {
            Null2Empty(m);
            string sql = @"
            update EngRiskFrontProjectSelection 
            set ModifyTime = GetDate()
                ,ModifyUserSeq = @ModifyUserSeq

                ,[PSType] = @PSType
                ,[PlanOverview] = @PlanOverview
                ,[Weight1] = @Weight1
                ,[Weight2] = @Weight2
                ,[Weight3] = @Weight3
                ,[Weight4] = @Weight4
                ,[Weight5] = @Weight5
                ,[Weight6] = @Weight6
                ,[Weight7] = @Weight7
                ,[TWeight] = @TWeight
                ,[WeightSort] = @WeightSort
            WHERE [Seq] = @Seq;

            SELECT [Seq],[EngRiskFrontSeq],ROW_NUMBER() OVER (ORDER BY [TWeight] DESC) as [ROW_NUMBER_SORT]
            INTO #TMP
            FROM [dbo].[EngRiskFrontProjectSelection] FN1
            WHERE [PSType]=3 AND [EngRiskFrontSeq]=@EngRiskFrontSeq

            UPDATE FN1
            SET FN1.[WeightSort] = FN2.[ROW_NUMBER_SORT]
            FROM [dbo].[EngRiskFrontProjectSelection] FN1
	            INNER JOIN #TMP FN2 ON FN1.Seq = FN2.Seq ";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@EngRiskFrontSeq", m.EngRiskFrontSeq);
                cmd.Parameters.AddWithValue("@PSType", m.PSType);
                cmd.Parameters.AddWithValue("@PlanOverview", m.PlanOverview);
                cmd.Parameters.AddWithValue("@Weight1", m.Weight1);
                cmd.Parameters.AddWithValue("@Weight2", m.Weight2);
                cmd.Parameters.AddWithValue("@Weight3", m.Weight3);
                cmd.Parameters.AddWithValue("@Weight4", m.Weight4);
                cmd.Parameters.AddWithValue("@Weight5", m.Weight5);
                cmd.Parameters.AddWithValue("@Weight6", m.Weight6);
                cmd.Parameters.AddWithValue("@Weight7", m.Weight7);
                cmd.Parameters.AddWithValue("@TWeight", m.TWeight);
                cmd.Parameters.AddWithValue("@WeightSort", m.WeightSort);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngRiskFrontService.UpdateEngRiskFrontProjectSelection: " + e.Message);
                return -1;
            }
        }
        //刪除
        public int DelEngRiskFrontProjectSelection(int Seq)
        {
            try
            {
                string sql = @"
                    DECLARE @EngRiskFrontSeq INT
                    SET @EngRiskFrontSeq = ISNULL((SELECT EngRiskFrontSeq FROM [dbo].[EngRiskFrontProjectSelection] WHERE [Seq] = @Seq),0);

                    DELETE FROM [dbo].[EngRiskFrontProjectSelection] WHERE [Seq] = @Seq;

                    SELECT [Seq],[EngRiskFrontSeq],ROW_NUMBER() OVER (ORDER BY [TWeight] DESC) as [ROW_NUMBER_SORT]
                    INTO #TMP
                    FROM [dbo].[EngRiskFrontProjectSelection] FN1
                    WHERE [PSType]=3 AND [EngRiskFrontSeq]=@EngRiskFrontSeq

                    UPDATE FN1
                    SET FN1.[WeightSort] = FN2.[ROW_NUMBER_SORT]
                    FROM [dbo].[EngRiskFrontProjectSelection] FN1
	                    INNER JOIN #TMP FN2 ON FN1.Seq = FN2.Seq";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", Seq);
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                //db.TransactionRollback();
                log.Info("EngRiskFrontService.DelEngRiskFrontProjectSelection" + e.Message);
                return -1;
            }
        }

        #endregion

        #region 檔案上傳(1.設計方案評選，2.設計階段施工風險評估成果之運用，3.風險資訊傳遞及風險追蹤管理，4.結論)

        //清單
        public List<T> GetEngRiskFrontFileList<T>(int Seq, int ERFType)
        {
            int userSeq = new SessionManager().GetUser().Seq;

            string sql = @"
                SELECT FN1.[Seq]
                      ,FN1.[EngRiskFrontSeq]
                      ,FN1.[FilePath]
                      ,FN1.[CreateTime]
                      ,FN1.[CreateUserSeq]
                      ,FN1.[ModifyTime]
                      ,FN1.[ModifyUserSeq]
	                  ,ISNULL(FN2.DisplayName,'') AS CreateUser
	                  ,ISNULL(FN3.DisplayName,'') AS ModifyUser
                      ,dbo.DelGuidStr(FN1.[FilePath]) FileName
                FROM [dbo].[EngRiskFrontFile] FN1
	                LEFT OUTER JOIN [dbo].[UserMain] FN2 ON FN2.Seq = FN1.[CreateUserSeq]
	                LEFT OUTER JOIN [dbo].[UserMain] FN3 ON FN3.Seq = FN1.[ModifyUserSeq]
                WHERE FN1.[EngRiskFrontSeq] = @Seq AND FN1.[ERFType] = @ERFType";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", Seq);
            cmd.Parameters.AddWithValue("@ERFType", ERFType);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //單筆
        public List<T> GetEngRiskFrontFileBySeq<T>(int Seq)
        {
            int userSeq = new SessionManager().GetUser().Seq;

            string sql = @"
                SELECT FN1.[Seq]
                      ,FN1.[EngRiskFrontSeq]
                      ,FN1.[FilePath]
                      ,FN1.[CreateTime]
                      ,FN1.[CreateUserSeq]
                      ,FN1.[ModifyTime]
                      ,FN1.[ModifyUserSeq]
	                  ,ISNULL(FN2.DisplayName,'') AS CreateUser
	                  ,ISNULL(FN3.DisplayName,'') AS ModifyUser
                      ,dbo.DelGuidStr(FN1.[FilePath]) FileName
                FROM [dbo].[EngRiskFrontFile] FN1
	                LEFT OUTER JOIN [dbo].[UserMain] FN2 ON FN2.Seq = FN1.[CreateUserSeq]
	                LEFT OUTER JOIN [dbo].[UserMain] FN3 ON FN3.Seq = FN1.[ModifyUserSeq]
                WHERE FN1.[Seq] = @Seq";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", Seq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //新增
        public int AddEngRiskFrontFile(EngRiskFrontFileVModel m)
        {
            Null2Empty(m);
            try
            {
                string sql = @"
                    INSERT INTO [dbo].[EngRiskFrontFile]
                               ([EngRiskFrontSeq],[ERFType],[FilePath]
                               ,[CreateTime],[CreateUserSeq],[ModifyTime],[ModifyUserSeq])
                         VALUES
                               (@EngRiskFrontSeq,@ERFType,@FilePath
                               ,GETDATE(),@CreateUserSeq,GETDATE(),@ModifyUserSeq)";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngRiskFrontSeq", m.EngRiskFrontSeq);
                cmd.Parameters.AddWithValue("@ERFType", m.ERFType);
                cmd.Parameters.AddWithValue("@FilePath", m.FilePath);
                cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngRiskFrontService.AddEngRiskFrontFile " + e.Message);
                return -1;
            }
        }

        //更新
        public int UpdateEngRiskFrontFile(EngRiskFrontFileVModel m)
        {
            Null2Empty(m);
            string sql = @"
                UPDATE [dbo].[EngRiskFrontFile]
                   SET [FilePath] = @FilePath
                      ,[ModifyTime] = GetDate()
                      ,[ModifyUserSeq] = @ModifyUserSeq
                 WHERE [Seq] = @Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@FilePath", m.FilePath);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngRiskFrontService.UpdateEngRiskFrontFile: " + e.Message);
                return -1;
            }
        }

        //刪除
        public int DelEngRiskFrontFile(int Seq)
        {
            try
            {
                string sql = @"DELETE FROM [dbo].[EngRiskFrontFile] WHERE [Seq] = @Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", Seq);
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                //db.TransactionRollback();
                log.Info("EngRiskFrontService.DelEngRiskFrontFile" + e.Message);
                return -1;
            }
        }

        //下載
        public List<T> DownloadEngRiskFrontFile<T>(int seq)
        {
            string sql = @"
                select
                    Seq ,EngRiskFrontSeq ,FilePath as FileName
                from [dbo].[EngRiskFrontFile]
                where Seq=@Seq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        #endregion 

        #region 施工風險內容及施作順序

        public List<T> GetEngRiskFrontSubProjectList<T>(int EngRiskFrontSeq)
        {
            string sql = @"
                select *
                from EngRiskFrontSubProjectList
                where EngRiskFrontSeq = @EngRiskFrontSeq and IsEnabled=1
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngRiskFrontSeq", EngRiskFrontSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        #endregion 

        #region 施工風險評估 - 明細

        public List<T> GetEngRiskFrontSubProjectDetail<T>(int SubProjectSeq)
        {
            string sql = @"
                select *
                from EngRiskFrontSubProjectDetail
                where SubProjectSeq = @SubProjectSeq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@SubProjectSeq", SubProjectSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        #endregion 

    }
}