using EQC.Common;
using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EADPlaneWeaknessService : BaseService
    {//品質管制弱面追蹤與分析
        //工程年分清單 20230726
        public List<EngYearVModel> GetEngYearList()
        {
            string sql = @"
                SELECT DISTINCT cast(a.TenderYear as integer) EngYear FROM PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d--20230505
                where a.TenderYear > 106 and ISNULL(b.ActualCompletionDate,'')= ''
                and ISNULL(d.PDAccuActualProgress,0)< 100 and d.PDExecState <> '已結案' and d.PDExecState <> '驗收完成'";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<EngYearVModel>(cmd);
        }
        private string handleUnitListCodition(string unitList, string sql)
        {
            List<string> list = new List<string>(unitList.Split(','));

            if (list.Exists(e => e == "'0'"))
            {
                sql += " or " + Utils.getExecUnitTenderCoditionSql("a.", 0);
            }
            if (list.Exists(e => e == "'1'"))
            {
                sql += " or " + Utils.getExecUnitTenderCoditionSql("a.", 1);
            }
            return sql;
        }
        //工程清單
        public List<T> GetList<T>(string unitList, int sYear, int eYear,List<string> places, bool isSupervise, string pw, string minBid, string maxBid)
        {
            string unitListCoditionSql = "";
            if (unitList.Length >0) unitListCoditionSql = String.Format(" a.ExecUnitName in ({0}) ", unitList);

            string placeList = "";
            if (places != null)
            {
                string sp = "";
                foreach (string item in places)
                {
                    placeList += String.Format("{0}(a.Location like '%{1}%')", sp, item);
                    sp = " OR ";
                }
            }
            if (placeList.Length > 0) placeList = String.Format(" and ({0}) ", placeList);

            string bidAmount = "";
            if(!String.IsNullOrEmpty(minBid) && !String.IsNullOrEmpty(maxBid))
            {
                bidAmount = String.Format(" and ( a.BidAmount>={0} and a.BidAmount<={1} ) ", minBid, maxBid);
            }

            string supervise = "";
            if (!isSupervise) supervise = @" and (a.TenderNo in (select ContractNo from AuditCaseList)) ";

            string sql = @"
                select a.Seq, a.TenderYear, a.TenderName, a.ExecUnitName, a.BidAmount, a.Location,
	                (
    	                CASE b.W1 when 1 then '1,' else '' end
                        + CASE b.W2 when 1 then '2,' else '' end
                        + CASE b.W3 when 1 then '3,' else '' end
                        + CASE b.W4 when 1 then '4,' else '' end
                        + CASE b.W5 when 1 then '5,' else '' end
                        + CASE b.W6 when 1 then '6,' else '' end
                        + CASE b.W7 when 1 then '7,' else '' end
                        + CASE b.W8 when 1 then '8,' else '' end
                        + CASE b.W9 when 1 then '9,' else '' end
                        + CASE b.W10 when 1 then '10,' else '' end
                        + CASE b.W11 when 1 then '11,' else '' end
                        + CASE b.W12 when 1 then '12,' else '' end
                        + CASE b.W13 when 1 then '13,' else '' end
                        + CASE b.W14 when 1 then '14,' else '' end 
	                ) as PlaneWeakness
                from PrjXML a
                inner join viewPrjXMLPlaneWeakness b on(
                    b.PrjXMLSeq=a.Seq"
                    + String.Format(" and b.{0} >0", pw) +
                @")
                left outer join PrjXMLExt zb on(zb.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230505
                where a.TenderYear>106 and a.TenderYear >=@startYear and a.TenderYear <=@endYear and ISNULL(zb.ActualCompletionDate,'')=''
                and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                " +
                supervise + bidAmount + placeList + " and (" +
                unitListCoditionSql;
                sql = handleUnitListCodition(unitList, sql) + ")";
                sql += @" order by a.TenderYear desc, a.TenderName ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}