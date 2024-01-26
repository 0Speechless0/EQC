using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EADManufacturerService : BaseService
    {//廠商履歷評估分析
        //廠商
        public string GetManufacturer(string keyword)
        {
            string result = "";
            string sql = @"
                select top 1 a.ContractorName1 from PrjXML a
                where a.ContractorName1 Like @WinningBidder  or a.ContractorName1 Like @WinningBidder
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@WinningBidder", keyword+"%");
            DataTable dt = db.GetDataTable(cmd);
            try
            {
                if(dt.Rows.Count == 1)
                    result = dt.Rows[0]["ContractorName1"].ToString();
            }
            catch { }
            return result;
        }
        //工程清單
        public List<T> GetEngList<T>(string winningBidder)
        {
            string sql = @"
                select
                    a.Seq,
	                a.TenderYear,
                    a.ExecUnitName,
                    a.TenderName,
                    a.BidAmount,
                    a.Location,
                    b.ActualPerformDesignDate,
                    b.ActualAacceptanceCompletionDate,
                    c.PSTotalScore
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                left outer join PerformanceScore c on (c.PrjXMLSeq=a.Seq)
                where a.ContractorName1 = @WinningBidder  or a.ContractorName1 = @WinningBidder
                order by a.TenderYear desc, a.ExecUnitName, a.TenderName
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@WinningBidder",  winningBidder );
            return db.GetDataTableWithClass<T>(cmd);
        }
        //履約計分
        public List<T> GetEngScoreList<T>(string winningBidder)
        {
            string sql = @"
                select
	                b.Seq, a.ExecUnitName, a.TenderName, a.BidAmount, c.ActualAacceptanceCompletionDate, b.PSTotalScore
                from PrjXML a
                inner join PrjXMLExt c on(c.PrjXMLSeq=a.Seq )
                inner join PerformanceScore b on (b.PrjXMLSeq=a.Seq)
                where a.ContractorName1 = @WinningBidder  or a.ContractorName1 = @WinningBidder
                order by b.PSIssueDate desc
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@WinningBidder", winningBidder);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //重大事故
        public List<T> GetEngSafetyList<T>(string winningBidder)
        {
            string sql = @"
                select
	                b.Seq, a.ExecUnitName, a.TenderName, b.WSDeadCnt, b.WSHurtCnt
                from PrjXML a
                inner join PrjXMLExt c on(c.PrjXMLSeq=a.Seq )
                inner join WorkSafetyTribunal b on (b.PrjXMLSeq=a.Seq)
                where a.ContractorName1 = @WinningBidder  or a.ContractorName1 = @WinningBidder
                order by b.WSDate desc
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@WinningBidder", winningBidder);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //司法院裁判書
        public List<T> GetEngVerdictList<T>(string winningBidder)
        {
            string sql = @"
                select
	                b.Seq, a.ExecUnitName, a.TenderName, b.WSDeadCnt, b.WSHurtCnt
                from PrjXML a
                inner join PrjXMLExt c on(c.PrjXMLSeq=a.Seq )
                inner join WorkSafetyTribunal b on (b.PrjXMLSeq=a.Seq)
                where a.ContractorName1 = @WinningBidder  or a.ContractorName1 = @WinningBidder
                order by b.WSDate desc
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@WinningBidder", winningBidder);
            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}