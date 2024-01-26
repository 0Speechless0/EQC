using EQC.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EngHistoryInfoService : BaseService
    {//水利工程履歷管理
        //督導紀錄
        public int GetSupervisionListCount(string buildContractorTaxId, string buildContractorName)
        {
            string sql = @"
                SELECT
	                count(a.Seq) total
                FROM EngMain a
                inner join SuperviseEng b on(b.PrjXMLSeq=a.PrjXMLSeq)
                where a.BuildContractorName like @BuildContractorName
                or a.BuildContractorTaxId like @BuildContractorTaxId
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@BuildContractorTaxId", buildContractorTaxId);
            cmd.Parameters.AddWithValue("@BuildContractorName", buildContractorName);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetSupervisionList<T>(string buildContractorTaxId, string buildContractorName, int perPage, int pageIndex)
        {
            string sql = @"
                SELECT
	                a.Seq,
                    a.EngYear,
                    a.EngName,
                    a1.Name execUnitName,
                    b.SuperviseDate,
                    b.CommitteeAverageScore
                    ,(
    	                select sum(DeductPoint) from SuperviseFill za
                        where za.SuperviseEngSeq=b.Seq
                    ) DeductPoint
                FROM EngMain a
                inner join SuperviseEng b on(b.PrjXMLSeq=a.PrjXMLSeq)
                left outer join Unit a1 on(a1.Seq=a.ExecUnitSeq)
                where a.BuildContractorName like @BuildContractorName
                or a.BuildContractorTaxId like @BuildContractorTaxId
                order by a.EngYear desc
                OFFSET @pageIndex ROWS
		        FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@BuildContractorTaxId", buildContractorTaxId);
            cmd.Parameters.AddWithValue("@BuildContractorName", buildContractorName);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //施工抽查紀錄
        public int GetConstructionCheckListCount(string buildContractorTaxId, string buildContractorName)
        {
            string sql = @"
                select
	                count(z1.Seq) total
                from (          
                    select
                        z.Seq
                    from (
                        SELECT
                            a.EngMainSeq Seq
                        FROM ConstCheckList a
                        where a.DataKeep=1
                        and a.EngMainSeq in(
                            select seq from EngMain
                            where BuildContractorName like @BuildContractorName
                            or BuildContractorTaxId like @BuildContractorTaxId
                        )

                        union all
                        SELECT
                            a.EngMainSeq Seq
                        FROM EquOperTestList a
                        where a.DataKeep=1
                        and a.EngMainSeq in(
                            select seq from EngMain
                            where BuildContractorName like @BuildContractorName
                            or BuildContractorTaxId like @BuildContractorTaxId
                        )

                        union all
                        SELECT
                            a.EngMainSeq Seq
                        FROM OccuSafeHealthList a
                        where a.DataKeep=1
                        and a.EngMainSeq in(
                            select seq from EngMain
                            where BuildContractorName like @BuildContractorName
                            or BuildContractorTaxId like @BuildContractorTaxId
                        )

                        union all
                        SELECT
                            a.EngMainSeq Seq
                        FROM EnvirConsList a
                        where a.DataKeep=1
                        and a.EngMainSeq in(
                            select seq from EngMain
                            where BuildContractorName like @BuildContractorName
                            or BuildContractorTaxId like @BuildContractorTaxId
                        )
                    ) z
                    group by z.Seq
                ) z1
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@BuildContractorTaxId", buildContractorTaxId);
            cmd.Parameters.AddWithValue("@BuildContractorName", buildContractorName);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetConstructionCheckList<T>(string buildContractorTaxId, string buildContractorName, int perPage, int pageIndex)
        {
            string sql = @"
                select
	                z1.Seq,
	                z1.constCheckRecCount,
	                z1.missingCount,
                    za.EngYear,
                    za.EngName,
                    zb.Name execUnitName
                from (          
                    select
                        z.Seq,
                        sum(z.constCheckRecCount) constCheckRecCount,
                        sum(z.missingCount) missingCount
                    from (
                        SELECT
                            a.EngMainSeq Seq
                            ,(
                                select COUNT(z1.Seq) from ConstCheckRec z1
                                where z1.EngConstructionSeq in(
                                    select zb.Seq from EngMain za
                                    inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)                                
                                    where za.Seq=a.EngMainSeq
                                    and z1.ItemSeq=a.Seq
                                )
                                and z1.CCRCheckType1=1
                            ) constCheckRecCount
                            ,(
                                select COUNT(z1.Seq) from ConstCheckRec z1
                                inner join ConstCheckRecResult zc on(zc.ConstCheckRecSeq=z1.Seq and zc.CCRCheckResult=2)
                                where z1.EngConstructionSeq in(
                                    select zb.Seq from EngMain za
                                    inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
                                    where za.Seq=a.EngMainSeq
                                    and z1.ItemSeq=a.Seq
                                )
                                and z1.CCRCheckType1=1
                            ) missingCount
                        FROM ConstCheckList a
                        where a.DataKeep=1
                        and a.EngMainSeq in(
                            select seq from EngMain
                            where BuildContractorName like @BuildContractorName
                            or BuildContractorTaxId like @BuildContractorTaxId
                        )

                        union all
                        SELECT
                            a.EngMainSeq Seq
                            ,(
                                select COUNT(z1.Seq) from ConstCheckRec z1
                                where z1.EngConstructionSeq in(
                                    select zb.Seq from EngMain za
                                    inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)                                
                                    where za.Seq=a.EngMainSeq
                                    and z1.ItemSeq=a.Seq
                                )
                                and z1.CCRCheckType1=2
                            ) constCheckRecCount
                            ,(
                                select COUNT(z1.Seq) from ConstCheckRec z1
                                inner join ConstCheckRecResult zc on(zc.ConstCheckRecSeq=z1.Seq and zc.CCRCheckResult=2)
                                where z1.EngConstructionSeq in(
                                    select zb.Seq from EngMain za
                                    inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
                                    where za.Seq=a.EngMainSeq
                                    and z1.ItemSeq=a.Seq
                                )
                                and z1.CCRCheckType1=2
                            ) missingCount
                        FROM EquOperTestList a
                        where a.DataKeep=1
                        and a.EngMainSeq in(
                            select seq from EngMain
                            where BuildContractorName like @BuildContractorName
                            or BuildContractorTaxId like @BuildContractorTaxId
                        )

                        union all
                        SELECT
                            a.EngMainSeq Seq
                            ,(
                                select COUNT(z1.Seq) from ConstCheckRec z1
                                where z1.EngConstructionSeq in(
                                    select zb.Seq from EngMain za
                                    inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)                                
                                    where za.Seq=a.EngMainSeq
                                    and z1.ItemSeq=a.Seq
                                )
                                and z1.CCRCheckType1=3
                            ) constCheckRecCount
                            ,(
                                select COUNT(z1.Seq) from ConstCheckRec z1
                                inner join ConstCheckRecResult zc on(zc.ConstCheckRecSeq=z1.Seq and zc.CCRCheckResult=2)
                                where z1.EngConstructionSeq in(
                                    select zb.Seq from EngMain za
                                    inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
                                    where za.Seq=a.EngMainSeq
                                    and z1.ItemSeq=a.Seq
                                )
                                and z1.CCRCheckType1=3
                            ) missingCount
                        FROM OccuSafeHealthList a
                        where a.DataKeep=1
                        and a.EngMainSeq in(
                            select seq from EngMain
                            where BuildContractorName like @BuildContractorName
                            or BuildContractorTaxId like @BuildContractorTaxId
                        )

                        union all
                        SELECT
                            a.EngMainSeq Seq
                            ,(
                                select COUNT(z1.Seq) from ConstCheckRec z1
                                where z1.EngConstructionSeq in(
                                    select zb.Seq from EngMain za
                                    inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)                                
                                    where za.Seq=a.EngMainSeq
                                    and z1.ItemSeq=a.Seq
                                )
                                and z1.CCRCheckType1=4
                            ) constCheckRecCount
                            ,(
                                select COUNT(z1.Seq) from ConstCheckRec z1
                                inner join ConstCheckRecResult zc on(zc.ConstCheckRecSeq=z1.Seq and zc.CCRCheckResult=2)
                                where z1.EngConstructionSeq in(
                                    select zb.Seq from EngMain za
                                    inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
                                    where za.Seq=a.EngMainSeq
                                    and z1.ItemSeq=a.Seq
                                )
                                and z1.CCRCheckType1=4
                            ) missingCount
                        FROM EnvirConsList a
                        where a.DataKeep=1
                        and a.EngMainSeq in (
                            select seq from EngMain
                            where BuildContractorName like @BuildContractorName
                            or BuildContractorTaxId like @BuildContractorTaxId
                        )
                    ) z
                    group by z.Seq
                ) z1
                inner join EngMain za on(za.Seq=z1.Seq)
                left outer join Unit zb on(zb.Seq=za.ExecUnitSeq)
                order by za.EngYear desc
                OFFSET @pageIndex ROWS
		        FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@BuildContractorTaxId", buildContractorTaxId);
            cmd.Parameters.AddWithValue("@BuildContractorName", buildContractorName);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //重大事故資料
        public int GetWorkSafetyTribunalListCount(string buildContractorTaxId, string buildContractorName)
        {
            string sql = @"
                SELECT
	                count(z.Seq) total
                from (
                    select a.TenderName, a.Seq
                    from EngMain b            
                    inner join PrjXML a on(b.PrjXMLSeq=a.Seq)
                    inner join WorkSafetyTribunal c on(c.PrjXMLSeq=a.Seq)
                    where b.BuildContractorName like @BuildContractorName
                    or b.BuildContractorTaxId like @BuildContractorTaxId
                    group by a.ExecUnitName, a.TenderName, a.Seq
                ) z
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@BuildContractorTaxId", buildContractorTaxId);
            cmd.Parameters.AddWithValue("@BuildContractorName", buildContractorName);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetWorkSafetyTribunalList<T>(string buildContractorTaxId, string buildContractorName, int perPage, int pageIndex)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    a.TenderName,
                    a.ExecUnitName,
                    sum(c.WSDeadCnt) WSDeadCnt,
                    sum(c.WSHurtCnt) WSHurtCnt
                from EngMain b            
                inner join PrjXML a on(b.PrjXMLSeq=a.Seq)
                inner join WorkSafetyTribunal c on(c.PrjXMLSeq=a.Seq)
                where b.BuildContractorName like @BuildContractorName
                or b.BuildContractorTaxId like @BuildContractorTaxId
                group by a.ExecUnitName, a.TenderName, a.Seq
                order by a.ExecUnitName, a.TenderName, a.Seq
                OFFSET @pageIndex ROWS
		        FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@BuildContractorTaxId", buildContractorTaxId);
            cmd.Parameters.AddWithValue("@BuildContractorName", buildContractorName);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }
        
        //碳排放量
        public int GetCarbonEemissionListCount(string buildContractorTaxId, string buildContractorName)
        {
            string sql = @"
                SELECT
	                count(a.Seq) total
                from EngMain a            
                where a.BuildContractorName like @BuildContractorName
                or a.BuildContractorTaxId like @BuildContractorTaxId
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@BuildContractorTaxId", buildContractorTaxId);
            cmd.Parameters.AddWithValue("@BuildContractorName", buildContractorName);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetCarbonEemissionList<T>(string buildContractorTaxId, string buildContractorName, int perPage, int pageIndex)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    a.EngYear,
	                a.EngNo,
	                a.EngName,
                    b.Name execUnitName,
                    b1.Name execSubUnitName
                    ,(
                        select ROUND(sum(ISNULL(za.ItemKgCo2e,0)),0)
                        from CarbonEmissionHeader zb
                        inner join CarbonEmissionPayItem za on(za.CarbonEmissionHeaderSeq=zb.Seq)
                        where zb.EngMainSeq=a.Seq
                        and za.KgCo2e is not null and za.ItemKgCo2e is not null
                    ) co2Total
                from EngMain a
                left outer join Unit b on(b.Seq=a.ExecUnitSeq)
                left outer join Unit b1 on(b1.Seq=a.ExecSubUnitSeq)
                where a.BuildContractorName like @BuildContractorName
                or a.BuildContractorTaxId like @BuildContractorTaxId
                order by a.EngNo desc
                OFFSET @pageIndex ROWS
		        FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@BuildContractorTaxId", buildContractorTaxId);
            cmd.Parameters.AddWithValue("@BuildContractorName", buildContractorName);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //履約計分資訊
        public int GetPerformanceScoreListCount(string buildContractorTaxId, string buildContractorName)
        {
            string sql = @"
                SELECT
	                count(a.Seq) total
                from EngMain b            
                inner join PrjXML a on(b.PrjXMLSeq=a.Seq)
                where b.BuildContractorName like @BuildContractorName
                or b.BuildContractorTaxId like @BuildContractorTaxId
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@BuildContractorTaxId", buildContractorTaxId);
            cmd.Parameters.AddWithValue("@BuildContractorName", buildContractorName);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetPerformanceScoreList<T>(string buildContractorTaxId, string buildContractorName, int perPage, int pageIndex)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    a.TenderYear,
                    a.TenderNo,
                    a.TenderName,
                    a.ExecUnitName,
                    c.PSTotalScore
                from EngMain b            
                inner join PrjXML a on(b.PrjXMLSeq=a.Seq)
                left outer join PerformanceScore c on(c.PrjXMLSeq=a.Seq)
                where b.BuildContractorName like @BuildContractorName
                or b.BuildContractorTaxId like @BuildContractorTaxId
                order by c.PSTotalScore desc
                OFFSET @pageIndex ROWS
		        FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@BuildContractorTaxId", buildContractorTaxId);
            cmd.Parameters.AddWithValue("@BuildContractorName", buildContractorName);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //品質弱面資訊
        public int GetWeaknessesListCount(string buildContractorTaxId, string buildContractorName)
        {
            string sql = @"
                SELECT
	                count(a.Seq) total
                from EngMain b            
                inner join PrjXML a on(b.PrjXMLSeq=a.Seq)
                where b.BuildContractorName like @BuildContractorName
                or b.BuildContractorTaxId like @BuildContractorTaxId
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@BuildContractorTaxId", buildContractorTaxId);
            cmd.Parameters.AddWithValue("@BuildContractorName", buildContractorName);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetWeaknessesList<T>(string buildContractorTaxId, string buildContractorName, int perPage, int pageIndex)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    a.TenderYear,
                    a.TenderNo,
                    a.TenderName,
                    a.ExecUnitName,
                    ISNULL(a1.DesignChangeContractAmount, a.BidAmount) BidAmount,
                    a.Location,
                    e.W1,
                    e.W2,
                    e.W3,
                    e.W4,
                    e.W5,
                    e.W6,
                    e.W7,
                    e.W8,
                    e.W9,
                    e.W10,
                    e.W11,
                    e.W12,
                    e.W13,
                    e.W14
                from EngMain b            
                inner join PrjXML a on(b.PrjXMLSeq=a.Seq)
                left outer join PrjXMLExt a1 on(a1.PrjXMLSeq=a.Seq)
                left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                where b.BuildContractorName like @BuildContractorName
                or b.BuildContractorTaxId like @BuildContractorTaxId
                order by a.TenderYear desc, a.TenderName
                OFFSET @pageIndex ROWS
		        FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@BuildContractorTaxId", buildContractorTaxId);
            cmd.Parameters.AddWithValue("@BuildContractorName", buildContractorName);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //廠商查詢
        public int GetSearchVenderEngListCount(string keyword)
        {
            string sql = @"
                SELECT
	                count(a.Seq) total
                from EngMain a
                inner join PrjXML a1 on(a1.Seq=a.PrjXMLSeq)
                where a.BuildContractorName like @keyword
                or a.BuildContractorTaxId like @keyword
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@keyword", keyword);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetSearchVenderEngList<T>(string keyword, int perPage, int pageIndex)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    a.EngYear,
                    a.EngNo,
                    a.EngName,
                    a.BuildContractorName,
                    a.BuildContractorTaxId,
                    b.Name execUnitName,
                    b1.Name execSubUnitName,
                    a.TotalBudget,
                    a1.Location,
                    a2.ActualCompletionDate,
                    a2.ActualAacceptanceCompletionDate
                from  EngMain a
                inner join PrjXML a1 on(a1.Seq=a.PrjXMLSeq)
                left outer join PrjXMLExt a2 on(a2.PrjXMLSeq=a.PrjXMLSeq)
                left outer join Unit b on(b.Seq=a.ExecUnitSeq)
                left outer join Unit b1 on(b1.Seq=a.ExecSubUnitSeq)
                where a.BuildContractorName like @keyword
                or a.BuildContractorTaxId like @keyword
                order by a.EngNo
                OFFSET @pageIndex ROWS
		        FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@keyword", keyword);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //工程查詢
        public int GetSearchEngListCount(string keyword)
        {
            string sql = @"
                SELECT
	                count(a.Seq) total
                from  EngMain a
                where a.EngName like @keyword
                or a.EngNo like @keyword
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@keyword", keyword);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetSearchEngList<T>(string keyword, int perPage, int pageIndex)
        {
            string sql = @"
                SELECT
                    a.Seq,
	                a.EngNo,
	                a.EngName,
                    b.Name execUnitName,
                    b1.Name execSubUnitName
                    ,(
                        select ROUND(sum(ISNULL(za.ItemKgCo2e,0)),0)
                        from CarbonEmissionHeader zb
                        inner join CarbonEmissionPayItem za on(za.CarbonEmissionHeaderSeq=zb.Seq)
                        where zb.EngMainSeq=a.Seq
                        and za.KgCo2e is not null and za.ItemKgCo2e is not null
                    ) co2Total
                from  EngMain a
                left outer join Unit b on(b.Seq=a.ExecUnitSeq)
                left outer join Unit b1 on(b1.Seq=a.ExecSubUnitSeq)
                where a.EngName like @keyword
                or a.EngNo like @keyword
                order by a.EngNo
                OFFSET @pageIndex ROWS
		        FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@keyword", keyword);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //抽查照片
        public int GetPhotoListCount(int engMainSeq, DateTime itemDate)
        {
            string sql = @"
                SELECT
                    COUNT(a.Seq) total
                FROM EngConstruction b
                inner join ConstCheckRec a on(a.EngConstructionSeq=b.Seq)
                left outer join ConstCheckRecImprove c on(c.ConstCheckRecSeq=a.Seq)
                left outer join ConstCheckRecFile a1 on(a1.ConstCheckRecSeq=a.Seq)
                left outer join ConstCheckRecImproveFile c1 on(c1.ConstCheckRecImproveSeq=c.Seq and c1.ItemGroup=1)
                left outer join ConstCheckRecImproveFile c2 on(c2.ConstCheckRecImproveSeq=c.Seq and c2.ItemGroup=2)
                where b.EngMainSeq=@EngMainSeq
                and (a1.ControllStSeq is not null or c1.ControllStSeq is not null or c1.ControllStSeq is not null)
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", itemDate);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetPhotoList<T>(int engMainSeq, DateTime itemDate, int perPage, int pageIndex)
        {
            string sql = @"
                SELECT
	                --b.ItemName,
                    a.Seq Seq,
                    --c.Seq improveSeq,
                    --a.EngConstructionSeq,
                    --a.CCRCheckType1,
                    --a.ItemSeq,
                    --a.CCRPosDesc,
                    --a1.ControllStSeq,
                    a1.UniqueFileName p1,
                    --c1.ControllStSeq,
                    c1.UniqueFileName p2,
                    --c2.ControllStSeq,
                    c2.UniqueFileName p3
                FROM EngConstruction b
                inner join ConstCheckRec a on(a.EngConstructionSeq=b.Seq)
                left outer join ConstCheckRecImprove c on(c.ConstCheckRecSeq=a.Seq)
                left outer join ConstCheckRecFile a1 on(a1.ConstCheckRecSeq=a.Seq)
                left outer join ConstCheckRecImproveFile c1 on(c1.ConstCheckRecImproveSeq=c.Seq and c1.ItemGroup=1)
                left outer join ConstCheckRecImproveFile c2 on(c2.ConstCheckRecImproveSeq=c.Seq and c2.ItemGroup=2)
                where b.EngMainSeq=@EngMainSeq
                and (a1.ControllStSeq is not null or c1.ControllStSeq is not null or c1.ControllStSeq is not null)
                order by a.CCRCheckType1 ,a.ItemSeq, a.CCRCheckDate
                OFFSET @pageIndex ROWS
		        FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", itemDate);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //機具
        public int GetEquipmentCount(int engMainSeq, DateTime itemDate)
        {
            string sql = @"
                select COUNT(z.Seq) total
                from (
                    SELECT
                        a.Seq
                    FROM SupDailyReportConstructionEquipment a
                    where a.SupDailyDateSeq=(
                        select top 1 za.Seq FROM SupDailyDate za
                        where za.EngMainSeq = @EngMainSeq
                        and za.ItemDate=@ItemDate
                        and za.DataType=2    
                    )
                    union all
                    SELECT
                        a.Seq
                    FROM EC_SupDailyReportConstructionEquipment a
                    where a.EC_SupDailyDateSeq=(
                        select top 1 za.Seq FROM EC_SupDailyDate za
                        where za.EngMainSeq = @EngMainSeq
                        and za.ItemDate=@ItemDate
                        and za.DataType=2    
                    )
                ) z
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", itemDate);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetEquipmentList<T>(int engMainSeq, DateTime itemDate, int perPage, int pageIndex)
        {
            string sql = @"
                select z.* from(
                    SELECT
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
                        (a.KgCo2e*a.TodayHours) KgCo2eAmount,
                        cast(
                        (
                            select sum(zc.TodayHours) from SupDailyDate za
                            inner join SupDailyDate zb on(zb.DataType=za.DataType and zb.EngMainSeq=za.EngMainSeq and zb.ItemDate<=za.ItemDate)
                            inner join SupDailyReportConstructionEquipment zc on(zc.SupDailyDateSeq=zb.Seq and zc.EquipmentName=a.EquipmentName and zc.EquipmentModel=a.EquipmentModel)
                            where za.Seq=a.SupDailyDateSeq
                        ) as decimal(20,4)) AccHours
                    FROM SupDailyReportConstructionEquipment a
                    where a.SupDailyDateSeq=(
                        select top 1 za.Seq FROM SupDailyDate za
                        where za.EngMainSeq = @EngMainSeq
                        and za.ItemDate=@ItemDate
                        and za.DataType=2    
                    )
                    union all
                    SELECT
                        a.Seq,
                        a.EC_SupDailyDateSeq SupDailyDateSeq,
                        a.EquipmentName,
                        a.EquipmentModel,
                        a.TodayQuantity,
                        cast(
                        (
                            select sum(zc.TodayQuantity) from EC_SupDailyDate za
                            inner join EC_SupDailyDate zb on(zb.DataType=za.DataType and zb.EngMainSeq=za.EngMainSeq and zb.ItemDate<=za.ItemDate)
                            inner join EC_SupDailyReportConstructionEquipment zc on(zc.EC_SupDailyDateSeq=zb.Seq and zc.EquipmentName=a.EquipmentName and zc.EquipmentModel=a.EquipmentModel)
                            where za.Seq=a.EC_SupDailyDateSeq
                        ) as decimal(20,4)) AccQuantity,
                        a.TodayHours,
                        (a.KgCo2e*a.TodayHours) KgCo2eAmount,
                        cast(
                        (
                            select sum(zc.TodayHours) from EC_SupDailyDate za
                            inner join EC_SupDailyDate zb on(zb.DataType=za.DataType and zb.EngMainSeq=za.EngMainSeq and zb.ItemDate<=za.ItemDate)
                            inner join EC_SupDailyReportConstructionEquipment zc on(zc.EC_SupDailyDateSeq=zb.Seq and zc.EquipmentName=a.EquipmentName and zc.EquipmentModel=a.EquipmentModel)
                            where za.Seq=a.EC_SupDailyDateSeq
                        ) as decimal(20,4)) AccHours
                    FROM EC_SupDailyReportConstructionEquipment a
                    where a.EC_SupDailyDateSeq=(
                        select top 1 za.Seq FROM EC_SupDailyDate za
                        where za.EngMainSeq = @EngMainSeq
                        and za.ItemDate=@ItemDate
                        and za.DataType=2    
                    )
                ) z
                order by z.Seq
                OFFSET @pageIndex ROWS
		        FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", itemDate);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //工地人員
        public int GetPersonListCount(int engMainSeq, DateTime itemDate)
        {
            string sql = @"
                select COUNT(z.Seq) total
                from (
                    SELECT
                        a.Seq
                    FROM SupDailyReportConstructionPerson a
                    where a.SupDailyDateSeq=(
                        select top 1 za.Seq FROM SupDailyDate za
                        where za.EngMainSeq = @EngMainSeq
                        and za.ItemDate=@ItemDate
                        and za.DataType=2    
                    )
                    union all
                    SELECT
                        a.Seq
                    FROM EC_SupDailyReportConstructionPerson a
                    where a.EC_SupDailyDateSeq=(
                        select top 1 za.Seq FROM EC_SupDailyDate za
                        where za.EngMainSeq = @EngMainSeq
                        and za.ItemDate=@ItemDate
                        and za.DataType=2    
                    )
                ) z
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", itemDate);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetPersonList<T>(int engMainSeq, DateTime itemDate, int perPage, int pageIndex)
        {
            string sql = @"
                select z.* from(
                    SELECT
                        a.Seq,
                        a.SupDailyDateSeq,
                        a.KindName,
                        a.TodayQuantity,
                        cast(
                        (
                            select sum(zc.TodayQuantity) from SupDailyDate za
                            inner join SupDailyDate zb on(zb.DataType=za.DataType and zb.EngMainSeq=za.EngMainSeq and zb.ItemDate<=za.ItemDate)
                            inner join SupDailyReportConstructionPerson zc on(zc.SupDailyDateSeq=zb.Seq and zc.KindName=a.KindName)
                            where za.Seq=a.SupDailyDateSeq
                        ) as decimal(20,4)) AccQuantity
                    FROM SupDailyReportConstructionPerson a
                    where a.SupDailyDateSeq=(
                        select top 1 za.Seq FROM SupDailyDate za
                        where za.EngMainSeq = @EngMainSeq
                        and za.ItemDate=@ItemDate
                        and za.DataType=2    
                    )
                    union all
                    SELECT
                        a.Seq,
                        a.EC_SupDailyDateSeq SupDailyDateSe,
                        a.KindName,
                        a.TodayQuantity,
                        cast(
                        (
                            select sum(zc.TodayQuantity) from EC_SupDailyDate za
                            inner join EC_SupDailyDate zb on(zb.DataType=za.DataType and zb.EngMainSeq=za.EngMainSeq and zb.ItemDate<=za.ItemDate)
                            inner join EC_SupDailyReportConstructionPerson zc on(zc.EC_SupDailyDateSeq=zb.Seq and zc.KindName=a.KindName)
                            where za.Seq=a.EC_SupDailyDateSeq
                        ) as decimal(20,4)) AccQuantity
                    FROM EC_SupDailyReportConstructionPerson a
                    where a.EC_SupDailyDateSeq=(
                        select top 1 za.Seq FROM EC_SupDailyDate za
                        where za.EngMainSeq = @EngMainSeq
                        and za.ItemDate=@ItemDate
                        and za.DataType=2    
                    )
                ) z
                order by z.Seq
                OFFSET @pageIndex ROWS
		        FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", itemDate);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //工地材料管制概況
        public int GetMaterialListCount(int engMainSeq, DateTime itemDate)
        {
            string sql = @"
                select COUNT(z.Seq) total
                from (
                    SELECT
                        a.Seq
                    FROM SupDailyReportConstructionMaterial a
                    where a.SupDailyDateSeq=(
	                    select top 1 za.Seq FROM SupDailyDate za
                        where za.EngMainSeq = @EngMainSeq
                        and za.ItemDate=@ItemDate
                        and za.DataType=2    
                    )

                    union all

                    SELECT
                        a.Seq
                    FROM EC_SupDailyReportConstructionMaterial a
                    where a.EC_SupDailyDateSeq=(
	                    select top 1 za.Seq FROM EC_SupDailyDate za
                        where za.EngMainSeq = @EngMainSeq
                        and za.ItemDate=@ItemDate
                        and za.DataType=2    
                    )
                ) z
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", itemDate);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetMaterialList<T>(int engMainSeq, DateTime itemDate, int perPage, int pageIndex)
        {
            string sql = @"
                select z.* from(
                    SELECT
                        a.Seq,
                        a.SupDailyDateSeq,
                        a.MaterialName,
                        a.Unit,
                        a.ContractQuantity,
                        a.TodayQuantity,
                        a.AccQuantity,
                        a.Memo
                    FROM SupDailyReportConstructionMaterial a
                    where a.SupDailyDateSeq=(
	                    select top 1 za.Seq FROM SupDailyDate za
                        where za.EngMainSeq = @EngMainSeq
                        and za.ItemDate=@ItemDate
                        and za.DataType=2    
                    )

                    union all

                    SELECT
                        a.Seq,
                        a.EC_SupDailyDateSeq SupDailyDateSeq,
                        a.MaterialName,
                        a.Unit,
                        a.ContractQuantity,
                        a.TodayQuantity,
                        a.AccQuantity,
                        a.Memo
                    FROM EC_SupDailyReportConstructionMaterial a
                    where a.EC_SupDailyDateSeq=(
	                    select top 1 za.Seq FROM EC_SupDailyDate za
                        where za.EngMainSeq = @EngMainSeq
                        and za.ItemDate=@ItemDate
                        and za.DataType=2    
                    )
                ) z
                order by z.Seq
                OFFSET @pageIndex ROWS
		        FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemDate", itemDate);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }        
        
        //材料設備送審管制總表
        public int GetEMDSummaryListCount(int engMainSeq)
        {
            string sql = @"
                SELECT
	                count(z.ItemNo) total
                from (
                    SELECT 
                        b.[EngMaterialDeviceListSeq]
                        ,b.[ItemNo]
                        ,b.[MDName]
                        ,b.[ContactQty]
                        ,b.[ContactUnit]
                    from EngMaterialDeviceList a                  
                    inner join EngMaterialDeviceSummary b on(b.EngMaterialDeviceListSeq=a.Seq)
                    WHERE a.EngMainSeq = @EngMainSeq
                    group by b.[EngMaterialDeviceListSeq], b.[ItemNo], b.[MDName] ,b.[ContactQty], b.[ContactUnit]
                ) z
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetEMDSummaryList<T>(int engMainSeq, int perPage, int pageIndex)
        {
            string sql = @"
                SELECT
	                z.[EngMaterialDeviceListSeq], z.[ItemNo], z.[MDName] ,z.[ContactQty], z.ContactUnit,
                    sum(z.IsSampleTest) IsSampleTestCnt, --取樣次數
                    sum(z.IsFactoryInsp) IsFactoryInspCnt, --驗廠次數
                    sum(z.AuditDate) AuditDateCnt, --送審次數
                    sum(z.AuditResult) AuditResultCnt --通過次數
                from (
                    SELECT 
                        --b.ItemType
                        --b.Seq
                        b.[EngMaterialDeviceListSeq]
                        --,b.[OrderNo]
                        ,b.[ItemNo]
                        ,b.[MDName]
                        ,b.[ContactQty]
                        ,b.[ContactUnit]
                        ,IIF(b.[IsSampleTest]=1,1,0) IsSampleTest
                        ,IIF(b.[IsFactoryInsp]=1,1,0) IsFactoryInsp
                        ,IIF(b.[AuditDate]=NULL, 0, 1) AuditDate
                        ,IIF(b.[AuditResult]=1,1 ,0) AuditResult
                    from EngMaterialDeviceList a                  
                    inner join EngMaterialDeviceSummary b on(b.EngMaterialDeviceListSeq=a.Seq)
                    WHERE a.EngMainSeq = @EngMainSeq
                ) z
                group by z.[EngMaterialDeviceListSeq], z.[ItemNo], z.[MDName], z.[ContactQty], z.ContactUnit
                order by z.[EngMaterialDeviceListSeq], z.[ItemNo], z.[MDName], z.[ContactQty], z.ContactUnit
                OFFSET @pageIndex ROWS
		        FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //材料設備送審管制總表 - 協力廠商資料
        public List<T> GetEMDVendorList<T>(int engMaterialDeviceListSeq)
        {
            string sql = @"
                SELECT 
                    b.VendorName,
                    b.VendorTaxId,
                    b.VendorAddr
                from EngMaterialDeviceSummary b
                WHERE b.EngMaterialDeviceListSeq = @EngMaterialDeviceListSeq
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", engMaterialDeviceListSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //材料設備送審管制總表 - 型錄
        public List<T> GetEMDCatalogList<T>(int engMaterialDeviceListSeq)
        {
            string sql = @"
                SELECT
	                a.Seq,
                    a.OriginFileName,
                    a.CreateTime
                FROM EngMaterialDeviceSummary b
                inner join Resume a on(a.EngMaterialDeviceSummarySeq=b.Seq)
                WHERE b.EngMaterialDeviceListSeq = @EngMaterialDeviceListSeq
                order by a.CreateTime DESC
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", engMaterialDeviceListSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //材料設備送審管制總表 - 相關試驗報告
        public List<T> GetEMDTestReportList<T>(int engMaterialDeviceListSeq)
        {
            string sql = @"
                SELECT
	                a.Seq,
                    a.OriginFileName,
                    a.CreateTime
                FROM EngMaterialDeviceSummary b
                inner join UploadAuditFile a on(a.EngMaterialDeviceSummarySeq=b.Seq)
                WHERE b.EngMaterialDeviceListSeq = @EngMaterialDeviceListSeq
                order by a.CreateTime DESC
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", engMaterialDeviceListSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //材料設備檢(試)驗管制總表 筆數
        public int GetEMDTestSummaryListCount(int engMainSeq, int engMaterialDeviceListSeq)
        {
            string sql = @"
                SELECT
                    count(Seq) total
                FROM [dbo].[EngMaterialDeviceTestSummary]
                WHERE [EngMaterialDeviceListSeq] in (
	                SELECT [Seq] FROM [dbo].[EngMaterialDeviceList] WHERE [EngMainSeq]=@Seq
                )
                and EngMaterialDeviceListSeq=@EngMaterialDeviceListSeq
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", engMainSeq);
            cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", engMaterialDeviceListSeq);

            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        //材料設備檢(試)驗管制總表
        public List<T> GetEMDTestSummaryList<T>(int engMainSeq, int engMaterialDeviceListSeq, int pageIndex, int perPage)
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
	                SELECT [Seq] FROM EngMaterialDeviceList WHERE [EngMainSeq] = @Seq
                )
                and FN1.EngMaterialDeviceListSeq=@EngMaterialDeviceListSeq
                ORDER BY FN1.OrderNo, FN1.ItemType, FN1.Seq ASC
                OFFSET @pageIndex ROWS
		        FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", engMainSeq);
            cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", engMaterialDeviceListSeq);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}