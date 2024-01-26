using EQC.Common;
using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EMDAuditService : BaseService
    {
        //private UnitService unitService = new UnitService();

        //工程座標 s20230503
        public List<T> GetEngLatLng<T>(int seq)
        {
            string sql = @"
                SELECT
                    CoordX, CoordY
                FROM EngMain a 
	            INNER JOIN PrjXML b on(b.Seq=A.PrjXMLSeq) 
                WHERE a.Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //工程年分清單
        public List<EngYearVModel> GetEngYearList()
        {
            string sql = @"
                SELECT DISTINCT 
                    CAST(FN1.EngYear AS Integer) AS EngYear
                FROM EngMain FN1 
	                INNER JOIN SupervisionProjectList FN2 ON FN1.Seq = FN2.EngMainSeq 
                WHERE FN2.DocState >= 0  "
                    //AND FN1.CreateUserSeq = @CreateUserSeq
                + Utils.getAuthoritySql("FN1.")
                + @" ORDER BY CAST(FN1.EngYear AS Integer) DESC";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<EngYearVModel>(cmd);
        }
        //工程執行機關清單
        public List<EngExecUnitsVModel> GetEngExecUnitList(string engYear)
        {
            string sql = @"
                SELECT DISTINCT
                    FN2.OrderNo,
                    FN1.ExecUnitSeq UnitSeq,
                    FN2.Name UnitName
                FROM EngMain FN1
	                INNER JOIN Unit FN2 ON FN2.Seq = FN1.ExecUnitSeq AND FN2.parentSeq IS NULL
	                INNER JOIN SupervisionProjectList FN3 ON FN1.Seq = FN3.EngMainSeq 
                WHERE FN2.parentSeq is null 
                    AND FN3.DocState >= 0
	                AND FN1.EngYear = @EngYear "
                //AND FN1.CreateUserSeq = @CreateUserSeq"
                + Utils.getAuthoritySql("FN1.")
                + @" ORDER BY FN2.OrderNo";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngYear", engYear);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<EngExecUnitsVModel>(cmd);
        }
        //工程執行機關清單
        public List<EngExecUnitsVModel> GetEngExecSubUnitList(string engYear, int parentSeq)
        {
            string sql = @"
                SELECT DISTINCT
                    FN2.OrderNo,
                    FN1.ExecSubUnitSeq UnitSeq,
                    FN2.Name UnitName
                FROM EngMain FN1
	                INNER JOIN Unit FN2 ON FN2.Seq = FN1.ExecUnitSeq AND @ParentSeq = FN2.parentSeq
	                INNER JOIN SupervisionProjectList FN3 ON FN3.EngMainSeq = FN1.Seq
                WHERE FN2.parentSeq is null 
                    AND FN3.DocState >= 0
	                AND FN1.EngYear = @EngYear "
                //AND FN1.CreateUserSeq = @CreateUserSeq"
                + Utils.getAuthoritySql("FN1.")
                + @" ORDER BY FN2.OrderNo";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngYear", engYear);
            cmd.Parameters.AddWithValue("@ParentSeq", parentSeq);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<EngExecUnitsVModel>(cmd);
        }

        public List<T> GetEngCreatedList<T>(string year, int unitSeq, int subUnitSeq, int engMain, int pageRecordCount, int pageIndex)
        {
            string sql = @"";
            if (subUnitSeq == -1)
            {
                sql = @"
                    SELECT
                        a.Seq,
                        a.EngNo,
                        a.EngName,
                        b.Name ExecUnit, 
                        c.Name ExecSubUnit,
                        a.SupervisorUnitName,
                        a.ApproveDate,
                        d.DocState
                    FROM EngMain a
                    inner join Unit b on(b.Seq=a.ExecUnitSeq)
                    inner join SupervisionProjectList d on(
                        d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                    )
                    left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)
                    
                    where ( (@Seq=-1) or (a.Seq=@Seq) )
                    and a.EngYear=@EngYear
                    and a.ExecUnitSeq=@ExecUnitSeq"
                    + Utils.getAuthoritySql("a.")  //and a.CreateUserSeq=@CreateUserSeq
                    + @" order by EngNo DESC
                    OFFSET @pageIndex ROWS
				    FETCH FIRST @pageRecordCount ROWS ONLY";
            }
            else
            {
                sql = @"
                    SELECT
                        a.Seq,
                        a.EngNo,
                        a.EngName,
                        b.Name ExecUnit, 
                        c.Name ExecSubUnit,
                        a.SupervisorUnitName,
                        a.ApproveDate,
                        d.DocState
                    FROM EngMain a
                    inner join Unit b on(b.Seq=a.ExecUnitSeq and a.ExecUnitSeq=@ExecUnitSeq)
                    inner join SupervisionProjectList d on(
                        d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                    )
                    left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)
                    where ( (@Seq=-1) or (a.Seq=@Seq) )
                    and a.EngYear=@EngYear
                    and a.ExecSubUnitSeq=@ExecSubUnitSeq"
                    + Utils.getAuthoritySql("a.")  //and a.CreateUserSeq=@CreateUserSeq
                    + @" order by EngNo DESC
                    OFFSET @pageIndex ROWS
				    FETCH FIRST @pageRecordCount ROWS ONLY";
            }
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngYear", year);
            cmd.Parameters.AddWithValue("@ExecUnitSeq", unitSeq);
            cmd.Parameters.AddWithValue("@ExecSubUnitSeq", subUnitSeq);
            cmd.Parameters.AddWithValue("@Seq", engMain);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<T>(cmd);
        }

        //工程清單總筆數
        public int GetEngListCount(string year, int unitSeq, int subUnitSeq, int engMain)
        {
            string sql = @"";
            if (subUnitSeq == -1)
            {
                sql = @"
                SELECT
                    count(a.Seq) total
                FROM EngMain a
                inner join Unit b on(b.Seq=a.ExecUnitSeq)
                inner join SupervisionProjectList d on(
                    d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                )
                where d.DocState >= 0 
                    and ( (@Seq=-1) or (a.Seq=@Seq) )
                    and a.EngYear=@EngYear
                    and a.ExecUnitSeq=@ExecUnitSeq"
                    + Utils.getAuthoritySql("a.");  //and a.CreateUserSeq=@CreateUserSeq";
            }
            else
            {
                sql = @"
                SELECT
                    count(a.Seq) total
                FROM EngMain a
                inner join Unit b on(b.Seq=a.ExecSubUnitSeq and a.ExecUnitSeq=@ExecUnitSeq)
                inner join SupervisionProjectList d on(
                    d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                )
                where d.DocState >= 0 
                    and ( (@Seq=-1) or (a.Seq=@Seq) )
                    and a.EngYear=@EngYear
                    and a.ExecSubUnitSeq=@ExecSubUnitSeq"
                    + Utils.getAuthoritySql("a.");  //and a.CreateUserSeq=@CreateUserSeq";
            }
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", engMain);
            cmd.Parameters.AddWithValue("@EngYear", year);
            cmd.Parameters.AddWithValue("@ExecUnitSeq", unitSeq);
            cmd.Parameters.AddWithValue("@ExecSubUnitSeq", subUnitSeq);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());

            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        //工程清單
        public List<EngMainVModel> GetEngList(string year, int unitSeq, int subUnitSeq, int engMain, int pageRecordCount, int pageIndex)
        {
            string sql = @"
                WITH FN4 AS (
	                SELECT EngMainSeq,COUNT(*) AS EMDCount FROM [dbo].[EngMaterialDeviceList] GROUP BY EngMainSeq
                ),
                FN5 AS (
	                SELECT FN51.EngMainSeq, 
		                CASE WHEN ISNULL(FN52.TOTAL,0) = 0 
			                THEN '尚未' 
			                ELSE (CASE WHEN ISNULL(FN52.TOTAL,0) - ISNULL(FN53.YOK,0) = 0 
						                THEN '完成' 
						                ELSE '尚未' END) 
			                END AS AuditProgress 
	                FROM [dbo].[EngMaterialDeviceList] FN51
		                LEFT OUTER JOIN (SELECT A.EngMainSeq, COUNT(*) AS TOTAL 
							                FROM [dbo].[EngMaterialDeviceList] A INNER JOIN [dbo].[EngMaterialDeviceSummary] B ON A.Seq = B.EngMaterialDeviceListSeq
							                GROUP BY A.EngMainSeq) FN52 ON FN51.Seq = FN52.EngMainSeq
		                LEFT OUTER JOIN (SELECT A.EngMainSeq, COUNT(*) AS YOK 
							                FROM [dbo].[EngMaterialDeviceList] A INNER JOIN [dbo].[EngMaterialDeviceSummary] B ON A.Seq = B.EngMaterialDeviceListSeq
							                WHERE B.AuditDate IS NOT NULL 
							                GROUP BY A.EngMainSeq) FN53 ON FN51.Seq = FN53.EngMainSeq
	                GROUP BY FN51.EngMainSeq, FN52.TOTAL, FN53.YOK
                ) ";

            if (subUnitSeq == -1)
            {
                sql = sql + @"
                    SELECT
                        a.Seq,
                        a.EngNo,
                        a.EngName,
                        b.Name ExecUnit, 
                        c.Name ExecSubUnit,
                        isnull(a.SupervisorUnitName,'') as SupervisorUnitName,
                        FN5.AuditProgress,
                        isnull(FN4.EMDCount,0) as EMDCount
                    FROM EngMain a
                        inner join Unit b on(b.Seq=a.ExecUnitSeq)
                        inner join SupervisionProjectList d on(
                            d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                        )
                        left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)
                        LEFT OUTER JOIN FN4 ON a.Seq = FN4.EngMainSeq
	                    LEFT OUTER JOIN FN5 ON a.Seq = FN5.EngMainSeq
                    where ( (@Seq=-1) or (a.Seq=@Seq) )
                        AND d.DocState >= 0
                        and a.EngYear=@EngYear
                        and a.ExecUnitSeq=@ExecUnitSeq"
                        + Utils.getAuthoritySql("a.")  //and a.CreateUserSeq=@CreateUserSeq
                    + @" order by EngNo DESC
                    OFFSET @pageIndex ROWS
				    FETCH FIRST @pageRecordCount ROWS ONLY";
            }
            else
            {
                sql = sql + @"
                    SELECT
                        a.Seq,
                        a.EngNo,
                        a.EngName,
                        b.Name ExecUnit, 
                        c.Name ExecSubUnit,
                        isnull(a.SupervisorUnitName,'') as SupervisorUnitName,
                        FN5.AuditProgress,
                        isnull(FN4.EMDCount,0) as EMDCount
                    FROM EngMain a
                        inner join Unit b on(b.Seq=a.ExecUnitSeq and a.ExecUnitSeq=@ExecUnitSeq)
                        inner join SupervisionProjectList d on(
                            d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                        )
                        left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)
                        LEFT OUTER JOIN FN4 ON a.Seq = FN4.EngMainSeq
	                    LEFT OUTER JOIN FN5 ON a.Seq = FN5.EngMainSeq
                    where ( (@Seq=-1) or (a.Seq=@Seq) )
                        AND d.DocState >= 0
                        and a.EngYear=@EngYear
                        and a.ExecSubUnitSeq=@ExecSubUnitSeq"
                        + Utils.getAuthoritySql("a.")  //and a.CreateUserSeq=@CreateUserSeq
                    + @" order by EngNo DESC
                    OFFSET @pageIndex ROWS
				    FETCH FIRST @pageRecordCount ROWS ONLY";
            }

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngYear", year);
            cmd.Parameters.AddWithValue("@ExecUnitSeq", unitSeq);
            cmd.Parameters.AddWithValue("@ExecSubUnitSeq", subUnitSeq);
            cmd.Parameters.AddWithValue("@Seq", engMain);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<EngMainVModel>(cmd);
        }
        public List<T> GetItemBySeq<T>(int seq)
        {
            //shioulo 取消所有 [EQC]. 關鍵字
            string sql = @"
                MERGE [dbo].[EngMaterialDeviceSummary] AS T
                USING (SELECT * FROM [dbo].[EngMaterialDeviceList] WHERE [EngMainSeq] = @Seq) AS S
                ON T.[EngMaterialDeviceListSeq] = S.[Seq]
                WHEN NOT MATCHED THEN  
                    INSERT ([EngMaterialDeviceListSeq], [OrderNo], [ItemNo], [MDName], [CreateTime], [CreateUserSeq], [ModifyTime], [ModifyUserSeq])  
                    VALUES (S.[Seq], S.[OrderNo], S.[ItemNo], S.[MDName], S.[CreateTime], S.[CreateUserSeq], S.[ModifyTime], S.[ModifyUserSeq]); 

                MERGE [dbo].[EngMaterialDeviceTestSummary] AS T
                USING (SELECT * FROM [dbo].[EngMaterialDeviceList] WHERE [EngMainSeq] = @Seq) AS S
                ON T.[EngMaterialDeviceListSeq] = S.[Seq]
                WHEN NOT MATCHED THEN  
                    INSERT ([EngMaterialDeviceListSeq], [OrderNo], [ItemNo], [MDName], [CreateTime], [CreateUserSeq], [ModifyTime], [ModifyUserSeq])  
                    VALUES (S.[Seq], S.[OrderNo], S.[ItemNo], S.[MDName], S.[CreateTime], S.[CreateUserSeq], S.[ModifyTime], S.[ModifyUserSeq]); 

                WITH EMD AS (
	                SELECT EngMainSeq,COUNT(*) AS EMDCount FROM [dbo].[EngMaterialDeviceList] GROUP BY EngMainSeq
                )
                SELECT
                    a.Seq,
                    a.EngNo,
                    a.EngName,
                    b.Name OrganizerUnit, 
                    c.Name ExecUnit, 
                    isnull(a.SupervisorUnitName,'') as SupervisorUnitName,
					'' as AuditProgress,
					isnull(d.EMDCount,0) as EMDCount,
					a.ApproveDate,
					a.ApproveNo
                FROM EngMain a
                    inner join Unit b on(b.Seq=a.OrganizerUnitSeq)
					inner join Unit c on(c.Seq=a.ExecUnitSeq)
                    left outer join EMD d on a.Seq = d.EngMainSeq
                where
                    a.Seq = @Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //材料設備送審管制總表筆數
        public int GetEMDSummaryListCount(int seq)
        {
            string sql = @"
                SELECT count(b.Seq) total
                FROM EngMaterialDeviceList a
                inner join EngMaterialDeviceSummary b on (b.EngMaterialDeviceListSeq=a.Seq)
                WHERE a.EngMainSeq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        //材料設備送審管制總表
        public List<T> GetEMDSummaryList<T>(int seq, int pageIndex, int perPage)
        {
            string sql = @"
                SELECT 
                    b.ItemType
                    ,b.Seq
                    ,b.[EngMaterialDeviceListSeq]
                    ,b.[OrderNo]
                    ,b.[ItemNo]
                    ,b.[MDName]
                    ,b.[ContactQty]
                    ,b.[ContactUnit]
                    ,b.[IsSampleTest]
                    ,b.[SchAuditDate]
                    ,b.[RealAutitDate]
                    ,b.[IsFactoryInsp]
                    ,b.[FactoryInspDate]
                    ,b.[IsAuditVendor]
                    ,b.[VendorName]
                    ,b.[VendorTaxId]
                    ,b.[VendorAddr]
                    ,b.[VendorLng]
                    ,b.[VendorLat]
                    ,b.[VendorDistance]
                    ,b.[IsAuditCatalog]
                    ,b.[IsAuditReport]
                    ,b.[IsAuditSample]
                    ,b.[OtherAudit]
                    ,b.[AuditDate]
                    ,b.[AuditResult]
                    ,b.[ArchiveNo]
                    ,b.[CreateTime]
                    ,b.[CreateUserSeq]
                    ,b.[ModifyTime]
                    ,b.[ModifyUserSeq]
                    ,ISNULL((SELECT TOP 1 ResumeUrl FROM [dbo].[Resume] WHERE ResumeType = 1 AND EngMaterialDeviceSummarySeq = b.Seq),'') AS QRCodeImageURL
                from EngMaterialDeviceList a                  
                inner join EngMaterialDeviceSummary b on(b.EngMaterialDeviceListSeq=a.Seq)
                WHERE a.EngMainSeq=@Seq
                ORDER BY b.OrderNo, b.ItemType, b.Seq
                OFFSET @pageIndex ROWS
		        FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetEMDSummaryListByEngMainSeq<T>(int seq)
        {
            string sql = @"
                SELECT 
                   [Seq]
                    ,ItemType
                  ,[EngMaterialDeviceListSeq]
                  ,[OrderNo]
                  ,[ItemNo]
                  ,[MDName]
                  ,[ContactQty]
                  ,[ContactUnit]
                  ,[IsSampleTest]
                  ,[SchAuditDate]
                  ,[RealAutitDate]
                  ,[IsFactoryInsp]
                  ,[FactoryInspDate]
                  ,[IsAuditVendor]
                  ,[VendorName]
                  ,[VendorTaxId]
                  ,[VendorAddr]
                  ,[IsAuditCatalog]
                  ,[IsAuditReport]
                  ,[IsAuditSample]
                  ,[OtherAudit]
                  ,[AuditDate]
                  ,[AuditResult]
                  ,[ArchiveNo]
                  ,[CreateTime]
                  ,[CreateUserSeq]
                  ,[ModifyTime]
                  ,[ModifyUserSeq]
                  ,ISNULL((SELECT TOP 1 ResumeUrl FROM [dbo].[Resume] WHERE ResumeType = 1 AND EngMaterialDeviceSummarySeq = [dbo].[EngMaterialDeviceSummary].[Seq]),'') AS QRCodeImageURL
              FROM [dbo].[EngMaterialDeviceSummary]
              WHERE [EngMaterialDeviceListSeq] in (
	              SELECT [Seq]
	              FROM [dbo].[EngMaterialDeviceList]
	              WHERE [EngMainSeq]=@Seq)
              ORDER BY OrderNo, ItemType, Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public string GetEMDSummaryListBySeq(int seq)
        {
            string sql = @"";
            sql = @"
                SELECT
                    ISNULL((SELECT TOP 1 ResumeUrl FROM [dbo].[Resume] WHERE ResumeType = 1 AND EngMaterialDeviceSummarySeq = [dbo].[EngMaterialDeviceSummary].[Seq]),'') AS QRCodeImageURL
                FROM [dbo].[EngMaterialDeviceSummary]
              WHERE [Seq] = @Seq ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            DataTable dt = db.GetDataTable(cmd);
            return dt.Rows[0]["QRCodeImageURL"].ToString();
        }
        //材料設備檢(試)驗管制總表 筆數
        public int GetEMDTestSummaryListCount(int seq)
        {
            string sql = @"";
            sql = @"
                SELECT
                    count(Seq) total
                FROM [dbo].[EngMaterialDeviceTestSummary]
              WHERE [EngMaterialDeviceListSeq] in (
	              SELECT [Seq]
	              FROM [dbo].[EngMaterialDeviceList]
	              WHERE [EngMainSeq]=@Seq)";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        //材料設備檢(試)驗管制總表
        public List<T> GetEMDTestSummaryList<T>(int seq, int pageIndex, int perPage)
        {
            string sql = @"
                WITH EMDListSt AS (
	                SELECT FN1.[EngMaterialDeviceListSeq], FN1.[MDTestFeq]
                    FROM [dbo].[EngMaterialDeviceControlSt] FN1
	                    INNER JOIN (SELECT [EngMaterialDeviceListSeq],MAX(Seq) AS Seq FROM [dbo].[EngMaterialDeviceControlSt] GROUP BY [EngMaterialDeviceListSeq]) FN2 
		                    ON FN1.[EngMaterialDeviceListSeq] = FN2.[EngMaterialDeviceListSeq] AND FN1.Seq = FN2.Seq
                )
                SELECT 
                    FN1.[Seq]
                    ,FN1.ItemType
                    ,FN1.[EngMaterialDeviceListSeq]
                    ,FN1.[OrderNo]
                    ,FN1.[ItemNo]
                    ,FN1.[MDName]
                    ,FN1.[SchTestDate]
                    ,FN1.[RealTestDate]
                    ,FN1.[TestQty]
                    ,FN1.[SampleDate]
                    ,FN1.[SampleQty]
                    --,FN1.[SampleFeq]
	                ,FN2.[MDTestFeq] AS [SampleFeq]
                    ,FN1.[AccTestQty]
                    ,FN1.[AccSampleQty]
                    ,FN1.[TestResult]
                    ,FN1.[Coworkers]
                    ,FN1.[ArchiveNo]
                    ,FN1.[CreateTime]
                    ,FN1.[CreateUserSeq]
                    ,FN1.[ModifyTime]
                    ,FN1.[ModifyUserSeq]
                FROM [dbo].[EngMaterialDeviceTestSummary] FN1
	                LEFT OUTER JOIN EMDListSt FN2 ON FN1.[EngMaterialDeviceListSeq] = FN2.[EngMaterialDeviceListSeq]
                WHERE FN1.[EngMaterialDeviceListSeq] in (
	                SELECT [Seq]
	                FROM [dbo].[EngMaterialDeviceList]
	                WHERE [EngMainSeq] = @Seq)
                ORDER BY FN1.OrderNo, FN1.ItemType, FN1.Seq ASC
                OFFSET @pageIndex ROWS
		        FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetEMDTestSummaryListByEngMainSeq<T>(int seq)
        {
            string sql = @"
                WITH EMDListSt AS (
	                SELECT FN1.[EngMaterialDeviceListSeq], FN1.[MDTestFeq]
                    FROM [dbo].[EngMaterialDeviceControlSt] FN1
	                    INNER JOIN (SELECT [EngMaterialDeviceListSeq],MAX(Seq) AS Seq FROM [dbo].[EngMaterialDeviceControlSt] GROUP BY [EngMaterialDeviceListSeq]) FN2 
		                    ON FN1.[EngMaterialDeviceListSeq] = FN2.[EngMaterialDeviceListSeq] AND FN1.Seq = FN2.Seq
                )
                SELECT 
                    FN1.[Seq]
                    ,FN1.ItemType
                    ,FN1.[EngMaterialDeviceListSeq]
                    ,FN1.[OrderNo]
                    ,FN1.[ItemNo]
                    ,FN1.[MDName]
                    ,FN1.[SchTestDate]
                    ,FN1.[RealTestDate]
                    ,FN1.[TestQty]
                    ,FN1.[SampleDate]
                    ,FN1.[SampleQty]
                    --,FN1.[SampleFeq]
	                ,FN2.[MDTestFeq] AS [SampleFeq]
                    ,FN1.[AccTestQty]
                    ,FN1.[AccSampleQty]
                    ,FN1.[TestResult]
                    ,FN1.[Coworkers]
                    ,FN1.[ArchiveNo]
                    ,FN1.[CreateTime]
                    ,FN1.[CreateUserSeq]
                    ,FN1.[ModifyTime]
                    ,FN1.[ModifyUserSeq]
                FROM [dbo].[EngMaterialDeviceTestSummary] FN1
	                LEFT OUTER JOIN EMDListSt FN2 ON FN1.[EngMaterialDeviceListSeq] = FN2.[EngMaterialDeviceListSeq]
                WHERE FN1.[EngMaterialDeviceListSeq] in (
	                SELECT [Seq]
	                FROM [dbo].[EngMaterialDeviceList]
	                WHERE [EngMainSeq] = @Seq)
                ORDER BY FN1.OrderNo, FN1.ItemType, FN1.Seq ASC";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //複製Item s20230308
        public bool CopyEMDSummary(EngMaterialDeviceSummaryVModel m)
        {
            Null2Empty(m);
            string sql;
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                sql = @"
                insert into EngMaterialDeviceSummary (
                    RefSeq,
	                EngMaterialDeviceListSeq,
                    ItemType,
                    OrderNo,
	                ItemNo,
                    MDName,
                    ContactQty,
                    ContactUnit,
	                CreateTime,
	                CreateUserSeq,
	                ModifyTime,
	                ModifyUserSeq
                )
                SELECT
                    Seq,
	                EngMaterialDeviceListSeq,
                    1,
                    OrderNo,
	                ItemNo,
	                MDName,
	                ContactQty,
	                ContactUnit,
	                GetDate(),
	                @ModifyUserSeq,
	                GetDate(),
                    @ModifyUserSeq
                from EngMaterialDeviceSummary                  
                WHERE Seq=@Seq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
                //s20230720
                sql = @"
                insert into EngMaterialDeviceTestSummary (
                    RefSeq,
	                EngMaterialDeviceListSeq,
                    ItemType,
                    OrderNo,
	                ItemNo,
                    MDName,
	                CreateTime,
	                CreateUserSeq,
	                ModifyTime,
	                ModifyUserSeq
                )
                SELECT
                    Seq,
	                EngMaterialDeviceListSeq,
                    1,
                    OrderNo,
	                ItemNo,
	                MDName,
	                GetDate(),
	                1,
	                GetDate(),
                    1
                from EngMaterialDeviceTestSummary                  
                WHERE EngMaterialDeviceListSeq=@EngMaterialDeviceListSeq
                and ItemType=0 ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", m.EngMaterialDeviceListSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                db.Connection.Close();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                db.Connection.Close();
                log.Info("EMDAuditService.CopyEMDSummary: " + e.Message);
                return false;
            }
        }
        //刪除Item s20230308
        public bool DelEMDSummary(int seq)
        {
            string sql;
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                sql = @"
                delete from EngMaterialDeviceSummary WHERE Seq=@Seq and ItemType=1";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                db.Connection.Close();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                db.Connection.Close();
                log.Info("EMDAuditService.DelEMDSummary: " + e.Message);
                return false;
            }
        }
        public bool UpdateEMDSummary(EngMaterialDeviceSummaryVModel m)
        {
            Null2Empty(m);
            string sql = @"";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                sql = @"
                update EngMaterialDeviceSummary set
                    OrderNo = @OrderNo,
                    ItemNo = @ItemNo,
                    MDName = @MDName,
                    ContactQty = @ContactQty,
                    ContactUnit = @ContactUnit,
                    IsSampleTest = @IsSampleTest,
                    SchAuditDate = @SchAuditDate,
                    RealAutitDate = @RealAutitDate,
                    IsFactoryInsp = @IsFactoryInsp,
                    FactoryInspDate = @FactoryInspDate,
                    IsAuditVendor = @IsAuditVendor,
                    IsAuditCatalog = @IsAuditCatalog,
                    IsAuditReport = @IsAuditReport,
                    IsAuditSample = @IsAuditSample,
                    OtherAudit = @OtherAudit,
                    AuditDate = @AuditDate,
                    AuditResult = @AuditResult,
                    ArchiveNo = @ArchiveNo,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@OrderNo", m.OrderNo);
                cmd.Parameters.AddWithValue("@ItemNo", m.ItemNo);
                cmd.Parameters.AddWithValue("@MDName", m.MDName);
                cmd.Parameters.AddWithValue("@ContactQty", m.ContactQty);
                cmd.Parameters.AddWithValue("@ContactUnit", m.ContactUnit);
                cmd.Parameters.AddWithValue("@IsSampleTest", m.IsSampleTest);
                cmd.Parameters.AddWithValue("@SchAuditDate", this.NulltoDBNull(m.SchAuditDate));
                cmd.Parameters.AddWithValue("@RealAutitDate", this.NulltoDBNull(m.RealAutitDate));
                cmd.Parameters.AddWithValue("@IsFactoryInsp", m.IsFactoryInsp);
                cmd.Parameters.AddWithValue("@FactoryInspDate", this.NulltoDBNull(m.FactoryInspDate));
                cmd.Parameters.AddWithValue("@IsAuditVendor", m.IsAuditVendor);
                cmd.Parameters.AddWithValue("@IsAuditCatalog", m.IsAuditCatalog);
                cmd.Parameters.AddWithValue("@IsAuditReport", m.IsAuditReport);
                cmd.Parameters.AddWithValue("@IsAuditSample", m.IsAuditSample);
                cmd.Parameters.AddWithValue("@OtherAudit", m.OtherAudit);
                cmd.Parameters.AddWithValue("@AuditDate", this.NulltoDBNull(m.AuditDate));
                cmd.Parameters.AddWithValue("@AuditResult", this.NulltoDBNull(m.AuditResult));
                cmd.Parameters.AddWithValue("@ArchiveNo", m.ArchiveNo);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                int result = db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                db.Connection.Close();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                db.Connection.Close();
                log.Info("EngMaterialDeviceSummary.Update: " + e.Message);
                log.Info(sql);
                return false;
            }
        }

        public bool UpdateEMDSummary_1(EngMaterialDeviceSummaryVModel m)
        {
            Null2Empty(m);
            string sql = @"";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                sql = @"
                update EngMaterialDeviceSummary set
                    VendorName = @VendorName,
                    VendorTaxId = @VendorTaxId,
                    VendorAddr = @VendorAddr,
                    VendorLng = @VendorLng,
                    VendorLat = @VendorLat,
                    VendorDistance = @VendorDistance,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@VendorName", m.VendorName);
                cmd.Parameters.AddWithValue("@VendorTaxId", m.VendorTaxId);
                cmd.Parameters.AddWithValue("@VendorAddr", m.VendorAddr);//s20230502
                cmd.Parameters.AddWithValue("@VendorLng", this.NulltoDBNull(m.VendorLng));//s20231106
                cmd.Parameters.AddWithValue("@VendorLat", this.NulltoDBNull(m.VendorLat));//s20231106
                cmd.Parameters.AddWithValue("@VendorDistance", m.VendorDistance);//s20231106
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                int result = db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                db.Connection.Close();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                db.Connection.Close();
                log.Info("EMDAuditService.Update: " + e.Message);
                log.Info(sql);
                return false;
            }
        }

        public bool UpdateEMDSummary_2(EngMaterialDeviceSummaryVModel m)
        {
            Null2Empty(m);
            string sql = @"";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                sql = @"
                update EngMaterialDeviceSummary set
                    ArchiveNo = @ArchiveNo,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ArchiveNo", m.ArchiveNo);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                int result = db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                db.Connection.Close();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                db.Connection.Close();
                log.Info("EngMaterialDeviceSummary.Update: " + e.Message);
                log.Info(sql);
                return false;
            }
        }
        //複製Item s20230308
        public bool CopyEMDTestSummary(EngMaterialDeviceTestSummaryVModel m)
        {
            Null2Empty(m);
            string sql = @"";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                sql = @"
                insert into EngMaterialDeviceTestSummary (
                    RefSeq,
	                EngMaterialDeviceListSeq,
                    ItemType,
                    OrderNo,
	                ItemNo,
                    MDName,
	                CreateTime,
	                CreateUserSeq,
	                ModifyTime,
	                ModifyUserSeq
                )
                SELECT
                    Seq,
	                EngMaterialDeviceListSeq,
                    1,
                    OrderNo,
	                ItemNo,
	                MDName,
	                GetDate(),
	                1,
	                GetDate(),
                    1
                from EngMaterialDeviceTestSummary                  
                WHERE Seq=@Seq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                int result = db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                db.Connection.Close();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                db.Connection.Close();
                log.Info("EMDAuditService.CopyEMDTestSummary: " + e.Message);
                log.Info(sql);
                return false;
            }
        }
        //刪除Item s20230308
        public bool DelEMDTestSummary(int seq)
        {
            string sql;
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                sql = @"
                delete UploadAuditFileResult where EngMaterialDeviceTestSummarySeq=@Seq; --s20230626

                delete from EngMaterialDeviceTestSummary WHERE Seq=@Seq and ItemType=1;
                ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                db.Connection.Close();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                db.Connection.Close();
                log.Info("EMDAuditService.DelEMDTestSummary: " + e.Message);
                return false;
            }
        }
        public bool UpdateEMDTestSummary(EngMaterialDeviceTestSummaryVModel m)
        {
            Null2Empty(m);
            string sql = @"";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                sql = @"
                update EngMaterialDeviceTestSummary set
                    OrderNo = @OrderNo,
                    ItemNo = @ItemNo,
                    MDName = @MDName,

                    SchTestDate = @SchTestDate,
                    RealTestDate = @RealTestDate,
                    TestQty = @TestQty,
                    SampleDate = @SampleDate,
                    SampleQty = @SampleQty,
                    SampleFeq = @SampleFeq,
                    AccTestQty = @AccTestQty,
                    AccSampleQty = @AccSampleQty,
                    TestResult = @TestResult,
                    Coworkers = @Coworkers,
                    ArchiveNo = @ArchiveNo,

                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@OrderNo", m.OrderNo);
                cmd.Parameters.AddWithValue("@ItemNo", m.ItemNo);
                cmd.Parameters.AddWithValue("@MDName", m.MDName);
                cmd.Parameters.AddWithValue("@SchTestDate", this.NulltoDBNull(m.SchTestDate));
                cmd.Parameters.AddWithValue("@RealTestDate", this.NulltoDBNull(m.RealTestDate));
                cmd.Parameters.AddWithValue("@TestQty", m.TestQty);
                cmd.Parameters.AddWithValue("@SampleDate", this.NulltoDBNull(m.SampleDate));
                cmd.Parameters.AddWithValue("@SampleQty", m.SampleQty);
                cmd.Parameters.AddWithValue("@SampleFeq", m.SampleFeq);
                cmd.Parameters.AddWithValue("@AccTestQty", m.AccTestQty);
                cmd.Parameters.AddWithValue("@AccSampleQty", m.AccSampleQty);
                cmd.Parameters.AddWithValue("@TestResult", m.TestResult);
                cmd.Parameters.AddWithValue("@Coworkers", m.Coworkers);
                cmd.Parameters.AddWithValue("@ArchiveNo", m.ArchiveNo);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                int result = db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                db.Connection.Close();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                db.Connection.Close();
                log.Info("EngMaterialDeviceTestSummary.Update: " + e.Message);
                log.Info(sql);
                return false;
            }
        }

        //送審檔案 清單
        public List<T> UploadAuditFileListBySeq<T>(int EngMaterialDeviceSummarySeq, int FileType)
        {
            string sql = @"
                SELECT
                        [Seq]
                       ,[EngMaterialDeviceSummarySeq]
                       ,[FileType]
                       ,[OriginFileName]
                       ,[UniqueFileName]
                       ,[OrderNo]
                       ,[CreateTime]
                       ,[CreateUserSeq]
                       ,[ModifyTime]
                       ,[ModifyUserSeq]
                FROM [dbo].[UploadAuditFile]
                where [EngMaterialDeviceSummarySeq]=@EngMaterialDeviceSummarySeq AND [FileType]=@FileType
                order by OrderNo DESC";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMaterialDeviceSummarySeq", EngMaterialDeviceSummarySeq);
            cmd.Parameters.AddWithValue("@FileType", FileType);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //送審結果檔案 清單 s20230626
        public List<T> GetAuditFileResultList<T>(int engMaterialDeviceTestSummarySeq, int FileType)
        {
            string sql = @"
                SELECT
                        [Seq]
                       ,[EngMaterialDeviceTestSummarySeq]
                       ,[FileType]
                       ,[OriginFileName]
                       ,[UniqueFileName]
                       ,[OrderNo]
                       ,[CreateTime]
                       ,[CreateUserSeq]
                       ,[ModifyTime]
                       ,[ModifyUserSeq]
                FROM [dbo].[UploadAuditFileResult]
                where [EngMaterialDeviceTestSummarySeq]=@EngMaterialDeviceTestSummarySeq AND [FileType]=@FileType
                order by OrderNo DESC";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMaterialDeviceTestSummarySeq", engMaterialDeviceTestSummarySeq);
            cmd.Parameters.AddWithValue("@FileType", FileType);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //履歷 清單
        public List<T> ResumeListBySeq<T>(int EngMaterialDeviceSummarySeq)
        {
            string sql = @"
                SELECT
                        [Seq]
                       ,[EngMaterialDeviceSummarySeq]
                       ,[ResumeType]
                       ,[ResumeUrl]
                       ,[OriginFileName]
                       ,[UniqueFileName]
                       ,[OrderNo]
                       ,[CreateTime]
                       ,[CreateUserSeq]
                       ,[ModifyTime]
                       ,[ModifyUserSeq]
                FROM [dbo].[Resume]
                where [EngMaterialDeviceSummarySeq]=@EngMaterialDeviceSummarySeq
                order by OrderNo DESC";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMaterialDeviceSummarySeq", EngMaterialDeviceSummarySeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //送審檔案 單筆
        public List<UploadAuditFileVModel> GetUploadAuditFileInfoBySeq(int seq)
        {
            string sql = @"
                SELECT FN1.[Seq]
                    ,FN1.[EngMaterialDeviceSummarySeq]
	                ,FN3.[EngMainSeq]
                    ,FN1.[OriginFileName]
                    ,FN1.[UniqueFileName]
                FROM [dbo].[UploadAuditFile] FN1 
	                INNER JOIN [EngMaterialDeviceSummary] FN2 ON FN1.EngMaterialDeviceSummarySeq=FN2.Seq
	                INNER JOIN [EngMaterialDeviceList] FN3 ON FN2.EngMaterialDeviceListSeq=FN3.Seq
                WHERE FN1.Seq = @Seq ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<UploadAuditFileVModel>(cmd);
        }
        //送審結果檔案 s20230626
        public List<UploadAuditFileResultModel> GetUploadAuditFileResultItem(int seq)
        {
            string sql = @"
                SELECT  
                    Seq,
                    EngMaterialDeviceTestSummarySeq,
	                FileType,
                    OriginFileName,
                    UniqueFileName,
                    OrderNo
                FROM UploadAuditFileResult 
                WHERE Seq = @Seq
            ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<UploadAuditFileResultModel>(cmd);
        }
        //履歷 單筆
        public List<ResumeVModel> GetResumeInfoBySeq(int seq)
        {
            string sql = @"
                SELECT FN1.[Seq]
                    ,FN1.[EngMaterialDeviceSummarySeq]
	                ,FN3.[EngMainSeq]
                    ,FN1.[OriginFileName]
                    ,FN1.[UniqueFileName]
                FROM [dbo].[Resume] FN1 
	                INNER JOIN [EngMaterialDeviceSummary] FN2 ON FN1.EngMaterialDeviceSummarySeq=FN2.Seq
	                INNER JOIN [EngMaterialDeviceList] FN3 ON FN2.EngMaterialDeviceListSeq=FN3.Seq
                WHERE FN1.Seq = @Seq ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<ResumeVModel>(cmd);
        }
        //新增 送審檔案
        public int AddUploadAuditFile(UploadAuditFileModel m)
        {
            string sql = @"
                INSERT INTO [dbo].[UploadAuditFile]
                       ([EngMaterialDeviceSummarySeq]
                       ,[FileType]
                       ,[OriginFileName]
                       ,[UniqueFileName]
                       ,[OrderNo]
                       ,[CreateTime]
                       ,[CreateUserSeq]
                       ,[ModifyTime]
                       ,[ModifyUserSeq])
                 VALUES
                       (@EngMaterialDeviceSummarySeq
                       ,@FileType
                       ,@OriginFileName
                       ,@UniqueFileName
                       ,(select ISNULL(max(OrderNo),0)+1 from [dbo].[UploadAuditFile] where EngMaterialDeviceSummarySeq=@EngMaterialDeviceSummarySeq)
                       ,GetDate()
                       ,@CreateUserSeq
                       ,GetDate()
                       ,@ModifyUserSeq)";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMaterialDeviceSummarySeq", m.EngMaterialDeviceSummarySeq);
            cmd.Parameters.AddWithValue("@FileType", m.FileType);
            cmd.Parameters.AddWithValue("@OriginFileName", m.OriginFileName);
            cmd.Parameters.AddWithValue("@uniqueFileName", m.UniqueFileName);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            return db.ExecuteNonQuery(cmd);
        }
        //新增 送審結果檔案 s20230626
        public int AddUploadAuditFileResult(UploadAuditFileModel m)
        {
            try
            {
                string sql = @"
                INSERT INTO [dbo].[UploadAuditFileResult]
                       ([EngMaterialDeviceTestSummarySeq]
                       ,[FileType]
                       ,[OriginFileName]
                       ,[UniqueFileName]
                       ,[OrderNo]
                       ,[CreateTime]
                       ,[CreateUserSeq]
                       ,[ModifyTime]
                       ,[ModifyUserSeq])
                 VALUES
                       (@EngMaterialDeviceTestSummarySeq
                       ,@FileType
                       ,@OriginFileName
                       ,@UniqueFileName
                       ,(select ISNULL(max(OrderNo),0)+1 from [dbo].[UploadAuditFileResult] where EngMaterialDeviceTestSummarySeq=@EngMaterialDeviceTestSummarySeq)
                       ,GetDate()
                       ,@CreateUserSeq
                       ,GetDate()
                       ,@ModifyUserSeq)";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMaterialDeviceTestSummarySeq", m.EngMaterialDeviceSummarySeq);
                cmd.Parameters.AddWithValue("@FileType", m.FileType);
                cmd.Parameters.AddWithValue("@OriginFileName", m.OriginFileName);
                cmd.Parameters.AddWithValue("@uniqueFileName", m.UniqueFileName);
                cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                return db.ExecuteNonQuery(cmd);
            } catch(Exception e) {
                log.Info("EMDAuditService.AddUploadAuditFileResult: " + e.Message);
                return 0;
            }
        }

        //新增 履歷
        public int AddResume(ResumeModel m)
        {
            string sql = @"
                INSERT INTO [dbo].[Resume]
                       ([EngMaterialDeviceSummarySeq]
                       ,[ResumeType]
                       ,[ResumeUrl]
                       ,[OriginFileName]
                       ,[UniqueFileName]
                       ,[OrderNo]
                       ,[CreateTime]
                       ,[CreateUserSeq]
                       ,[ModifyTime]
                       ,[ModifyUserSeq])
                 VALUES
                       (@EngMaterialDeviceSummarySeq
                       ,@ResumeType
                       ,@ResumeUrl
                       ,@OriginFileName
                       ,@UniqueFileName
                       ,(select ISNULL(max(OrderNo),0)+1 from [dbo].[Resume] where EngMaterialDeviceSummarySeq=@EngMaterialDeviceSummarySeq)
                       ,GetDate()
                       ,@CreateUserSeq
                       ,GetDate()
                       ,@ModifyUserSeq)";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMaterialDeviceSummarySeq", m.EngMaterialDeviceSummarySeq);
            cmd.Parameters.AddWithValue("@ResumeType", m.ResumeType);
            cmd.Parameters.AddWithValue("@ResumeUrl", m.ResumeUrl);
            cmd.Parameters.AddWithValue("@OriginFileName", m.OriginFileName);
            cmd.Parameters.AddWithValue("@uniqueFileName", m.UniqueFileName);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            return db.ExecuteNonQuery(cmd);
        }
        //刪除 送審檔案
        public int DelUploadAuditFile(int Seq)
        {
            string sql = @"
                DELETE FROM [dbo].[UploadAuditFile] WHERE Seq = @Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", Seq);

            return db.ExecuteNonQuery(cmd);
        }
        //刪除 送審結果檔案
        public int DelUploadAuditFileResult(int Seq)
        {
            try { 
                string sql = @"
                    DELETE FROM UploadAuditFileResult WHERE Seq = @Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", Seq);

                return db.ExecuteNonQuery(cmd);
            } catch(Exception e) {
                log.Info("EMDAuditService.DelUploadAuditFileResult: " + e.Message);
                return 0;
            }
        }

        //新增 履歷
        public int DelResume(int Seq)
        {
            string sql = @"
                DELETE FROM [dbo].[Resume] WHERE Seq = @Seq";
            
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", Seq);

            return db.ExecuteNonQuery(cmd);
        }


    }
}