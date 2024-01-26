using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EPCQualityVerifyService : BaseService
    {//工程管理 - 品質查證
        //施工抽查成果
        public List<T> GetDoc1<T>(int engMainSeq)
        {
            string sql = @"
                select count(z1.OrderNo) totalRec,
	                sum(IIF(missingCount=0, 1, 0)) okCount,
	                z1.subEngName, z1.checkName	
                from (	
                    select z.subEngName,z.OrderNo,z.checkName,
                        sum(okCount) okCount, SUM(missingCount) missingCount,
                        z.CCRPosDesc
                    from (					
                        SELECT
                            f.ItemName subEngName,
                            e3.ItemName checkName,
                            e3.OrderNo,
                            1 okCount,
                            0 missingCount,
                            e1.CCRPosDesc
                        FROM EngMain a
                        inner join EngConstruction f on(f.EngMainSeq=a.Seq)
                        inner join ConstCheckRec e1 on(e1.EngConstructionSeq=f.Seq and e1.CCRCheckType1=1)
 		                inner join ConstCheckRecResult e2 on(e2.ConstCheckRecSeq=e1.Seq and e2.ResultItem=1 and e2.CCRCheckResult=1)
                        inner join ConstCheckList e3 on(e3.Seq=e1.ItemSeq)
                    
                        where a.Seq=@Seq
                    
                        union all
                    
                        SELECT
                            f.ItemName subEngName,
                            e3.ItemName checkName,
                            e3.OrderNo,
                            0 okCount,
                            1 missingCount,
                            e1.CCRPosDesc
                        FROM EngMain a
                        inner join EngConstruction f on(f.EngMainSeq=a.Seq)
                        inner join ConstCheckRec e1 on(e1.EngConstructionSeq=f.Seq and e1.CCRCheckType1=1)
 		                inner join ConstCheckRecResult e2 on(e2.ConstCheckRecSeq=e1.Seq and e2.ResultItem=1 and e2.CCRCheckResult=2)
                        inner join ConstCheckList e3 on(e3.Seq=e1.ItemSeq)
                    
                        where a.Seq=@Seq
                    ) z
                    group by z.subEngName,z.checkName,z.OrderNo,z.CCRPosDesc
                ) z1
                group by z1.subEngName,z1.OrderNo,z1.checkName
                order by z1.subEngName,z1.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //工程施工執行資料表
        public List<T> GetDoc2<T>(int prjXMLSeq)
        {
            string sql = @"
                select
                    a.TenderNo,
	                b.BelongPrj,
	                a.TenderName,
                    a.OutsourcingBudget,
                    a.RendBasePrice,
                    ISNULL(b.DesignChangeContractAmount, a.BidAmount) BidAmount,
	                a.EngType,
                    b.AuditDate,
                    a.ActualStartDate,
                    ISNULL(b.ScheChangeCloseDate, a.ScheCompletionDate) ScheCompletionDate,
                    a.TotalDays,
                    a.DurationCategory,
                    a.CompetentAuthority,
                    a.OrganizerName,
                    b.PrjManageUnit,
                    a.DesignUnitName,
                    a.SupervisionUnitName,
                    a.ContractorName1,
                    a.EngOverview,
                    c.BDBackwardFactor,	
                    c.BDAnalysis,
                    c.BDSolution,
                    d.PDAccuScheProgress,
                    d.PDAccuActualProgress,
                    d.PDAccuEstValueAmount
                from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                left outer join BackwardData c on( c.Seq =
	                (
    	                select top 1 seq from BackwardData
                        where PrjXMLSeq=a.Seq
                        order by (BDYEAR*100+BDMonth) desc
                    )
                )
                left outer join ProgressData d on( d.Seq =
	                (
    	                select top 1 seq from ProgressData
                        where PrjXMLSeq=a.Seq
                        order by (PDYEAR*100+PDMonth) desc
                    )
                )
                where a.Seq=@Seq
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", prjXMLSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //填報清單
        public List<T> GetFillList<T>(int prjXMLSeq)
        {
            string sql = @"
                SELECT
	                a.Seq SEngSeq, a.SuperviseDate, a.SuperviseMode,
                    b.Seq, (b.MissingNo+':'+b.MissingLoc) Missing,
                    b.SchImprovDate, b.ActImprovDate, b.CloseDate
                FROM SuperviseEng a
                inner join SuperviseFill b ON(b.SuperviseEngSeq=a.Seq)
                where a.PrjXMLSeq=@PrjXMLSeq
                order by a.SuperviseDate desc, b.MissingNo 
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@PrjXMLSeq", prjXMLSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public bool UpdateItem(EPCSuperviseFillVModel m)
        {
            Null2Empty(m);
            string sql = @"
                update SuperviseFill set
	                SchImprovDate=@SchImprovDate,
                    ActImprovDate=@ActImprovDate,
                    CloseDate=@CloseDate
                where Seq=@Seq
                ";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@SchImprovDate", this.NulltoDBNull(m.SchImprovDate));
                cmd.Parameters.AddWithValue("@ActImprovDate", this.NulltoDBNull(m.ActImprovDate));
                cmd.Parameters.AddWithValue("@CloseDate", this.NulltoDBNull(m.CloseDate));
                db.ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception e)
            {
                log.Info("EPCQualityVerifyService.UpdateItem" + e.Message);
                log.Info(sql);
                return false;
            }
        }
    }
}