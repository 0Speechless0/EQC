using EQC.Common;
using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EADRiskService : BaseService
    {//水利工程履約風險分析
        //工程年分清單 s20230726
        public List<EngYearVModel> GetEngYearList()
        {
            string sql = @"
                SELECT DISTINCT cast(a.TenderYear as integer) EngYear FROM PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                order by EngYear desc
                ";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<EngYearVModel>(cmd);
        }
        //終止解約工程
        public int GetA8Count(string unitList, int sYear, int eYear)
        {
            string unitListCoditionSql = "";
            if (unitList.Length > 0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);
            string sql = @"
                select count(a.Seq) total
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq and b.Status like '%解約%')
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                and ( @startYear=-1 OR (a.TenderYear>=@startYear and a.TenderYear<=@endYear) ) and (
                "
                + unitListCoditionSql;
            sql = handleUnitListCodition(unitList, sql) +")";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            DataTable dt = db.GetDataTable(cmd);

            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<EADRiskEngVModel> GetA8(string unitList, int sYear, int eYear, int pageRecordCount, int pageIndex)
        {
            string unitListCoditionSql = "";
            if (unitList.Length > 0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);
            string sql = @"
                select a.Seq, a.ExecUnitName, a.TenderName, a.BidAmount
                    ,b.BelongPrj, b.DiffProgress
                    ,c.PDAccuScheProgress ,c.PDAccuActualProgress
	                ,d.BDAnalysis ,d.BDSolution
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq and b.Status like '%解約%')
                left join ProgressData c on (
	                c.Seq = ( select top 1 Seq from ProgressData where PrjXMLSeq=a.Seq order by (PDYear*100+PDMonth) desc )
                )
                left join BackwardData d on (
	                d.Seq = ( select top 1 Seq from BackwardData where PrjXMLSeq=a.Seq order by (BDYear*100 + BDMonth) desc )
                )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                and ( @startYear=-1 OR (a.TenderYear>=@startYear and a.TenderYear<=@endYear) ) and (
                "
                + unitListCoditionSql;
            sql = handleUnitListCodition(unitList, sql);
            sql +=
            @")
                order by b.BelongPrj
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            return db.GetDataTableWithClass<EADRiskEngVModel>(cmd);
        }

        //超過5個月未估驗工程
        public int GetA7Count(string unitList, int sYear, int eYear)
        {
            string unitListCoditionSql = "";
            if (unitList.Length > 0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);
            string sql = @"
                select count(a.Seq) total
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq
                    and ISNULL(b.ActualAacceptanceCompletionDate,'')=''
                    and DATEDIFF(day, dbo.ChtDate2Date(b.ReportContDate), GETDATE()) >= 150 --5個月
                )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                and ( @startYear=-1 OR (a.TenderYear>=@startYear and a.TenderYear<=@endYear) ) and (
                
                "
                + unitListCoditionSql;
            sql = handleUnitListCodition(unitList, sql) + ")";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            DataTable dt = db.GetDataTable(cmd);

            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<EADRiskEngVModel> GetA7(string unitList, int sYear, int eYear, int pageRecordCount, int pageIndex)
        {
            string unitListCoditionSql = "";
            if (unitList.Length > 0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);
            string sql = @"
                select a.Seq, a.ExecUnitName, a.TenderName, a.BidAmount
                    ,b.BelongPrj, b.DiffProgress
                    ,c.PDAccuScheProgress ,c.PDAccuActualProgress
	                ,d.BDAnalysis ,d.BDSolution
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq
                    and ISNULL(b.ActualAacceptanceCompletionDate,'')=''
                    and DATEDIFF(day, dbo.ChtDate2Date(b.ReportContDate), GETDATE()) >= 150 --5個月
                )
                left join ProgressData c on (
	                c.Seq = ( select top 1 Seq from ProgressData where PrjXMLSeq=a.Seq order by (PDYear*100+PDMonth) desc )
                )
                left join BackwardData d on (
	                d.Seq = ( select top 1 Seq from BackwardData where PrjXMLSeq=a.Seq order by (BDYear*100 + BDMonth) desc )
                )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                and ( @startYear=-1 OR (a.TenderYear>=@startYear and a.TenderYear<=@endYear) ) and (
                "
                + unitListCoditionSql;
            sql = handleUnitListCodition(unitList, sql) + ")";
            sql +=
            @"

                order by b.BelongPrj
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            return db.GetDataTableWithClass<EADRiskEngVModel>(cmd);
        }

        //超過4個月未估驗工程
        public int GetA6Count(string unitList, int sYear, int eYear)
        {
            string unitListCoditionSql = "";
            if (unitList.Length > 0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);
            string sql = @"
                select count(a.Seq) total
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq
                    and ISNULL(b.ActualAacceptanceCompletionDate,'')=''
                    and DATEDIFF(day, dbo.ChtDate2Date(b.ReportContDate), GETDATE()) >= 120 --4個月
                )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                and ( @startYear=-1 OR (a.TenderYear>=@startYear and a.TenderYear<=@endYear) ) and (
                "
                + unitListCoditionSql;
            sql = handleUnitListCodition(unitList, sql) +")";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            DataTable dt = db.GetDataTable(cmd);

            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<EADRiskEngVModel> GetA6(string unitList, int sYear, int eYear, int pageRecordCount, int pageIndex)
        {
            string unitListCoditionSql = "";
            if (unitList.Length > 0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);
            string sql = @"
                select a.Seq, a.ExecUnitName, a.TenderName, a.BidAmount
                    ,b.BelongPrj, b.DiffProgress
                    ,c.PDAccuScheProgress ,c.PDAccuActualProgress
	                ,d.BDAnalysis ,d.BDSolution
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq
                    and ISNULL(b.ActualAacceptanceCompletionDate,'')=''
                    and DATEDIFF(day, dbo.ChtDate2Date(b.ReportContDate), GETDATE()) >= 120 --4個月
                )
                left join ProgressData c on (
	                c.Seq = ( select top 1 Seq from ProgressData where PrjXMLSeq=a.Seq order by (PDYear*100+PDMonth) desc )
                )
                left join BackwardData d on (
	                d.Seq = ( select top 1 Seq from BackwardData where PrjXMLSeq=a.Seq order by (BDYear*100 + BDMonth) desc )
                )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                and ( @startYear=-1 OR (a.TenderYear>=@startYear and a.TenderYear<=@endYear) ) and (
                "
                + unitListCoditionSql;
                sql = handleUnitListCodition(unitList, sql);
                sql +=
            @")
                order by b.BelongPrj
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            return db.GetDataTableWithClass<EADRiskEngVModel>(cmd);
        }
        
        //完工5個月未完成驗收工程
        public int GetA5Count(string unitList, int sYear, int eYear)
        {
            string unitListCoditionSql = "";
            if (unitList.Length > 0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);
            string sql = @"
                select count(a.Seq) total
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq
                    and ISNULL(b.ActualCompletionDate, '') <> '' and ISNULL(b.ActualAacceptanceCompletionDate, '')=''
                    and DATEDIFF(day, dbo.ChtDate2Date(b.ActualCompletionDate), GETDATE()) >= 150 --5個月
                )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                and ( @startYear=-1 OR (a.TenderYear>=@startYear and a.TenderYear<=@endYear) ) and (
                "
                + unitListCoditionSql;
            sql = handleUnitListCodition(unitList, sql) + ")";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            DataTable dt = db.GetDataTable(cmd);

            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<EADRiskEngVModel> GetA5(string unitList, int sYear, int eYear, int pageRecordCount, int pageIndex)
        {
            string unitListCoditionSql = "";
            if (unitList.Length > 0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);
            string sql = @"
                select a.Seq, a.ExecUnitName, a.TenderName, a.BidAmount
                    ,b.BelongPrj, b.DiffProgress
                    ,c.PDAccuScheProgress ,c.PDAccuActualProgress
	                ,d.BDAnalysis ,d.BDSolution
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq
                    and ISNULL(b.ActualCompletionDate, '') <> '' and ISNULL(b.ActualAacceptanceCompletionDate, '')=''
                    and DATEDIFF(day, dbo.ChtDate2Date(b.ActualCompletionDate), GETDATE()) >= 150 --5個月
                )
                left join ProgressData c on (
	                c.Seq = ( select top 1 Seq from ProgressData where PrjXMLSeq=a.Seq order by (PDYear*100+PDMonth) desc )
                )
                left join BackwardData d on (
	                d.Seq = ( select top 1 Seq from BackwardData where PrjXMLSeq=a.Seq order by (BDYear*100 + BDMonth) desc )
                )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                and ( @startYear=-1 OR (a.TenderYear>=@startYear and a.TenderYear<=@endYear) ) and (
                "
                + unitListCoditionSql;
                sql = handleUnitListCodition(unitList, sql);
                sql +=
            @")
                order by b.BelongPrj
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            return db.GetDataTableWithClass<EADRiskEngVModel>(cmd);
        }

        //完工4個月未完成驗收工程
        public int GetA4Count(string unitList, int sYear, int eYear)
        {
            string unitListCoditionSql = "";
            if (unitList.Length > 0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);
            string sql = @"
                select count(a.Seq) total
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq
                    and ISNULL(b.ActualCompletionDate, '') <> '' and ISNULL(b.ActualAacceptanceCompletionDate, '')=''
                    and DATEDIFF(day, dbo.ChtDate2Date(b.ActualCompletionDate), GETDATE()) >= 120 --4個月
                )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                and ( @startYear=-1 OR (a.TenderYear>=@startYear and a.TenderYear<=@endYear) ) and (
                "
                + unitListCoditionSql;
            sql = handleUnitListCodition(unitList, sql) + ")";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            DataTable dt = db.GetDataTable(cmd);

            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<EADRiskEngVModel> GetA4(string unitList, int sYear, int eYear, int pageRecordCount, int pageIndex)
        {
            string unitListCoditionSql = "";
            if (unitList.Length > 0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);
            string sql = @"
                select a.Seq, a.ExecUnitName, a.TenderName, a.BidAmount
                    ,b.BelongPrj, b.DiffProgress
                    ,c.PDAccuScheProgress ,c.PDAccuActualProgress
	                ,d.BDAnalysis ,d.BDSolution
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq
                    and ISNULL(b.ActualCompletionDate, '') <> '' and ISNULL(b.ActualAacceptanceCompletionDate, '')=''
                    and DATEDIFF(day, dbo.ChtDate2Date(b.ActualCompletionDate), GETDATE()) >= 120 --4個月
                )
                left join ProgressData c on (
	                c.Seq = ( select top 1 Seq from ProgressData where PrjXMLSeq=a.Seq order by (PDYear*100+PDMonth) desc )
                )
                left join BackwardData d on (
	                d.Seq = ( select top 1 Seq from BackwardData where PrjXMLSeq=a.Seq order by (BDYear*100 + BDMonth) desc )
                )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                and ( @startYear=-1 OR (a.TenderYear>=@startYear and a.TenderYear<=@endYear) ) and (
                "
                + unitListCoditionSql;
                sql = handleUnitListCodition(unitList, sql);
                sql +=
            @")
                order by b.BelongPrj
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            return db.GetDataTableWithClass<EADRiskEngVModel>(cmd);
        }
        
        //完工3個月未完成驗收工程
        public int GetA3Count(string unitList, int sYear, int eYear)
        {
            string unitListCoditionSql = "";
            if (unitList.Length > 0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);
            string sql = @"
                select count(a.Seq) total
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq
                    and ISNULL(b.ActualCompletionDate, '') <> '' and ISNULL(b.ActualAacceptanceCompletionDate, '')=''
                    and DATEDIFF(day, dbo.ChtDate2Date(b.ActualCompletionDate), GETDATE()) >= 90 --3個月
                )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                and ( @startYear=-1 OR (a.TenderYear>=@startYear and a.TenderYear<=@endYear) ) and (
                "
                + unitListCoditionSql;
            sql = handleUnitListCodition(unitList, sql) + ")";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            DataTable dt = db.GetDataTable(cmd);

            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<EADRiskEngVModel> GetA3(string unitList, int sYear, int eYear, int pageRecordCount, int pageIndex)
        {
            string unitListCoditionSql = "";
            if (unitList.Length > 0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);
            string sql = @"
                select a.Seq, a.ExecUnitName, a.TenderName, a.BidAmount
                    ,b.BelongPrj, b.DiffProgress
                    ,c.PDAccuScheProgress ,c.PDAccuActualProgress
	                ,d.BDAnalysis ,d.BDSolution
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq
                    and ISNULL(b.ActualCompletionDate, '') <> '' and ISNULL(b.ActualAacceptanceCompletionDate, '')=''
                    and DATEDIFF(day, dbo.ChtDate2Date(b.ActualCompletionDate), GETDATE()) >= 90 --3個月
                )
                left join ProgressData c on (
	                c.Seq = ( select top 1 Seq from ProgressData where PrjXMLSeq=a.Seq order by (PDYear*100+PDMonth) desc )
                )
                left join BackwardData d on (
	                d.Seq = ( select top 1 Seq from BackwardData where PrjXMLSeq=a.Seq order by (BDYear*100 + BDMonth) desc )
                )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                and ( @startYear=-1 OR (a.TenderYear>=@startYear and a.TenderYear<=@endYear) ) and (
                "
                + unitListCoditionSql;
                sql = handleUnitListCodition(unitList, sql);
                sql +=
            @")
                order by b.BelongPrj
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            return db.GetDataTableWithClass<EADRiskEngVModel>(cmd);
        }

        //停工案件
        public int GetA2Count(string unitList, int sYear, int eYear)
        {
            string unitListCoditionSql = "";
            if (unitList.Length > 0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);
            string sql = @"
                select count(a.Seq) total
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq and b.Status like '%停工%')
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                and ( @startYear=-1 OR (a.TenderYear>=@startYear and a.TenderYear<=@endYear) ) and (
                "
                + unitListCoditionSql;
            sql = handleUnitListCodition(unitList, sql) + ")";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            DataTable dt = db.GetDataTable(cmd);

            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<EADRiskEngVModel> GetA2(string unitList, int sYear, int eYear, int pageRecordCount, int pageIndex)
        {
            string unitListCoditionSql = "";
            if (unitList.Length > 0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);

            string sql = @"
                select a.Seq, a.ExecUnitName, a.TenderName, a.BidAmount
                    ,b.BelongPrj, b.DiffProgress
                    ,c.PDAccuScheProgress ,c.PDAccuActualProgress
	                ,d.BDAnalysis ,d.BDSolution
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq and b.Status like '%停工%')
                left join ProgressData c on (
	                c.Seq = ( select top 1 Seq from ProgressData where PrjXMLSeq=a.Seq order by (PDYear*100+PDMonth) desc )
                )
                left join BackwardData d on (
	                d.Seq = ( select top 1 Seq from BackwardData where PrjXMLSeq=a.Seq order by (BDYear*100 + BDMonth) desc )
                )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                and ( @startYear=-1 OR (a.TenderYear>=@startYear and a.TenderYear<=@endYear) ) and (
                "
                + unitListCoditionSql;
                sql = handleUnitListCodition(unitList, sql); 
                sql +=
                @")
                order by b.BelongPrj
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            return db.GetDataTableWithClass<EADRiskEngVModel>(cmd);
        }

        private string handleUnitListCodition(string unitList, string sql)
        {
            List<string> list = new List<string>(unitList.Split(','));

            if( list.Exists(e => e == "'0'") )
            {
                sql += " or " + Utils.getExecUnitTenderCoditionSql("a.", 0);
            }
            if (list.Exists(e => e == "'1'"))
            {
                sql += " or " +  Utils.getExecUnitTenderCoditionSql("a.", 1);
            }
            return sql;
        }
        //巨額工程落後2%:是指決標金額大於2億元
        public int GetA1Count(string unitList, int sYear, int eYear)
        {
            string unitListCoditionSql = "";
            if (unitList.Length > 0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);
            string sql = @"
                select count(a.Seq) total
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq and b.DiffProgress < -2)
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                and ( @startYear=-1 OR (a.TenderYear>=@startYear and a.TenderYear<=@endYear) )
                and (ISNULL(b.DesignChangeContractAmount,a.BidAmount) > 200000) and (
                "
                + unitListCoditionSql;
            sql = handleUnitListCodition(unitList, sql) + ")";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            DataTable dt = db.GetDataTable(cmd);

            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<EADRiskEngVModel> GetA1(string unitList, int sYear, int eYear, int pageRecordCount, int pageIndex)
        {
            string unitListCoditionSql = "";
            if (unitList.Length > 0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);
            string sql = @"
                select a.Seq, a.ExecUnitName, a.TenderName, ISNULL(b.DesignChangeContractAmount,a.BidAmount) BidAmount
                    ,b.BelongPrj, b.DiffProgress
                    ,c.PDAccuScheProgress ,c.PDAccuActualProgress
	                ,d.BDAnalysis ,d.BDSolution
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq and b.DiffProgress < -2)
                left join ProgressData c on (
	                c.Seq = ( select top 1 Seq from ProgressData where PrjXMLSeq=a.Seq order by (PDYear*100+PDMonth) desc )
                )
                left join BackwardData d on (
	                d.Seq = ( select top 1 Seq from BackwardData where PrjXMLSeq=a.Seq order by (BDYear*100 + BDMonth) desc )
                )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                and ( @startYear=-1 OR (a.TenderYear>=@startYear and a.TenderYear<=@endYear) )
                and ISNULL(b.DesignChangeContractAmount,a.BidAmount) >= 200000 and (
                "
                    + unitListCoditionSql;
                sql = handleUnitListCodition(unitList, sql);
                sql +=
            @")
                order by b.BelongPrj
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            return db.GetDataTableWithClass<EADRiskEngVModel>(cmd);
        }

        //5千萬以上未達2億元(落後8%以上)
        public int GetA9Count(string unitList, int sYear, int eYear)
        {
            string unitListCoditionSql = "";
            if (unitList.Length > 0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);
            string sql = @"
                select count(a.Seq) total
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq and b.DiffProgress < -8)
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                and ( @startYear=-1 OR (a.TenderYear>=@startYear and a.TenderYear<=@endYear) )
                and (
                    (ISNULL(b.DesignChangeContractAmount,a.BidAmount) >= 50000 and ISNULL(b.DesignChangeContractAmount,a.BidAmount) < 200000)
                )
                and ("
                + unitListCoditionSql;
            sql = handleUnitListCodition(unitList, sql) +")";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            DataTable dt = db.GetDataTable(cmd);

            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<EADRiskEngVModel> GetA9(string unitList, int sYear, int eYear, int pageRecordCount, int pageIndex)
        {
            string unitListCoditionSql = "";
            if (unitList.Length > 0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);
            string sql = @"
                select a.Seq, a.ExecUnitName, a.TenderName, ISNULL(b.DesignChangeContractAmount,a.BidAmount) BidAmount
                    ,b.BelongPrj, b.DiffProgress
                    ,c.PDAccuScheProgress ,c.PDAccuActualProgress
	                ,d.BDAnalysis ,d.BDSolution
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq and b.DiffProgress < -8)
                left join ProgressData c on (
	                c.Seq = ( select top 1 Seq from ProgressData where PrjXMLSeq=a.Seq order by (PDYear*100+PDMonth) desc )
                )
                left join BackwardData d on (
	                d.Seq = ( select top 1 Seq from BackwardData where PrjXMLSeq=a.Seq order by (BDYear*100 + BDMonth) desc )
                )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                and ( @startYear=-1 OR (a.TenderYear>=@startYear and a.TenderYear<=@endYear) )
                and (
                    (ISNULL(b.DesignChangeContractAmount,a.BidAmount) >= 50000 and ISNULL(b.DesignChangeContractAmount,a.BidAmount) < 200000)
                ) and (
                "
                + unitListCoditionSql;
            sql = handleUnitListCodition(unitList, sql);
            sql +=
                @")
                order by b.BelongPrj
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            return db.GetDataTableWithClass<EADRiskEngVModel>(cmd);
        }

        //未達查核金額工程 (落後8%以上)
        public int GetA10Count(string unitList, int sYear, int eYear)
        {
            string unitListCoditionSql = "";
            if (unitList.Length > 0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);
            string sql = @"
                select count(a.Seq) total
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq and b.DiffProgress < -8)
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                and ( @startYear=-1 OR (a.TenderYear>=@startYear and a.TenderYear<=@endYear) )
                and (
                    ISNULL(b.DesignChangeContractAmount,a.BidAmount) < 50000
                    --or (b.DesignChangeContractAmount < 50000) 
                ) and (
                "
                + unitListCoditionSql;
            sql = handleUnitListCodition(unitList, sql)+")";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            DataTable dt = db.GetDataTable(cmd);

            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<EADRiskEngVModel> GetA10(string unitList, int sYear, int eYear, int pageRecordCount, int pageIndex)
        {
            string unitListCoditionSql = "";
            if (unitList.Length > 0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);
            string sql = @"
                select a.Seq, a.ExecUnitName, a.TenderName, ISNULL(b.DesignChangeContractAmount,a.BidAmount) BidAmount
                    ,b.BelongPrj, b.DiffProgress
                    ,c.PDAccuScheProgress ,c.PDAccuActualProgress
	                ,d.BDAnalysis ,d.BDSolution
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq and b.DiffProgress < -8)
                left join ProgressData c on (
	                c.Seq = ( select top 1 Seq from ProgressData where PrjXMLSeq=a.Seq order by (PDYear*100+PDMonth) desc )
                )
                left join BackwardData d on (
	                d.Seq = ( select top 1 Seq from BackwardData where PrjXMLSeq=a.Seq order by (BDYear*100 + BDMonth) desc )
                )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) zd --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(zd.PDAccuActualProgress,0)<100 and zd.PDExecState<>'已結案' and zd.PDExecState<>'驗收完成'
                and ( @startYear=-1 OR (a.TenderYear>=@startYear and a.TenderYear<=@endYear) )
                and (
                    ISNULL(b.DesignChangeContractAmount,a.BidAmount) < 50000
                    --or (b.DesignChangeContractAmount < 50000)
                ) and (
                "
                + unitListCoditionSql;
            sql = handleUnitListCodition(unitList, sql);
            sql +=
            @")
                order by b.BelongPrj
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            return db.GetDataTableWithClass<EADRiskEngVModel>(cmd);
        }
    }
}