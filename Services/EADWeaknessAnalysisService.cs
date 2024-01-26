using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EADWeaknessAnalysisService : BaseService
    {//品質管制弱面分析
        //廠商工程案件數
        public List<ChartWordcloudVModel> GetContractorList(int mode, string unit)
        {
            string sql = @"
                SELECT
                    cast(count(a.Seq) as decimal(10,0)) weight,
                    a.ContractorName1 name";

            if (mode == 1)
            {//所屬機關
                sql += @"
                    FROM PrjXML a
                    inner join Unit a2 on(a2.ParentSeq is null and a2.Name=a.ExecUnitName)
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230726
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and a.TenderYear >= (YEAR(GetDate())-1916) --前5年案件 
                    and (e.W1+e.W2+e.W3+e.W4+e.W5+e.W6+e.W7+e.W8+e.W9+e.W10+e.W11+e.W12+e.W13+e.W14)>0
                    and (@fUnit='' or a.ExecUnitName=@fUnit)
                    group by a.ContractorName1
                    order by a.ContractorName1";
            }
            else if (mode == 2)
            {//縣市政府
                sql += @"
                    FROM PrjXML a
                    inner join Country2WRAMapping aa on(aa.Country=substring(a.ExecUnitName,1,3) "
                    //+ Utils.getAuthoritySqlForTender("a.", "aa.RiverBureau")
                    + @")
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230726
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and a.TenderYear >= (YEAR(GetDate())-1916) --前5年案件
                    and (e.W1+e.W2+e.W3+e.W4+e.W5+e.W6+e.W7+e.W8+e.W9+e.W10+e.W11+e.W12+e.W13+e.W14)>0
                    and (@fUnit='' or aa.Country=@fUnit)
                    group by a.ContractorName1
                    order by a.ContractorName1";
            }
            else if (mode == 3)
            {//其他補助
                sql += @"
                    FROM PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230726
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and a.TenderYear >= (YEAR(GetDate())-1916) --前5年案件
                    and (e.W1+e.W2+e.W3+e.W4+e.W5+e.W6+e.W7+e.W8+e.W9+e.W10+e.W11+e.W12+e.W13+e.W14)>0 "
                    //+ Utils.getAuthoritySqlForTender1("a.")
                    + @"
                    and not exists (select Name from Unit where ParentSeq is null and Name=a.ExecUnitName)
                    and not exists (select Country from Country2WRAMapping where Country=substring(a.ExecUnitName,1,3) ) 
                    and (@fUnit='' or a.ExecUnitName=@fUnit)
                    group by a.ContractorName1
                    order by a.ContractorName1";
            }
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@fUnit", String.IsNullOrEmpty(unit) ? "" : unit);
            return db.GetDataTableWithClass<ChartWordcloudVModel>(cmd);
        }
        //弱面構面分析
        public List<T> GetOrientedSta<T>(int mode, string unit)
        {
            string sql = @"
                SELECT
                    count(a.Seq) EngCount,
                    sum(IIF((e.W1+e.W2+e.W3+e.W4+e.W5+e.W6+e.W7+e.W8+e.W9+e.W10+e.W11+e.W12+e.W13+e.W14)>0,1,0)) WeaknessTotal,
					sum(e.W1) W1,
                    sum(e.W2) W2,
                    sum(e.W3) W3,
                    sum(e.W4) W4,
                    sum(e.W5) W5,
                    sum(e.W6) W6,
                    sum(e.W7) W7,
                    sum(e.W8) W8,
                    sum(e.W9) W9,
                    sum(e.W10) W10,
                    sum(e.W11) W11,
                    sum(e.W12) W12,
                    sum(e.W13) W13,
                    sum(e.W14) W14";

            if (mode == 1)
            {//所屬機關
                sql += @"
                    FROM PrjXML a
                    inner join Unit a2 on(a2.ParentSeq is null and a2.Name=a.ExecUnitName)
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230726
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and a.TenderYear >= (YEAR(GetDate())-1916) --前5年案件 
                    and (@fUnit='' or a.ExecUnitName=@fUnit)
                    ";
            }
            else if (mode == 2)
            {//縣市政府
                sql += @"
                    FROM PrjXML a
                    inner join Country2WRAMapping aa on(aa.Country=substring(a.ExecUnitName,1,3) "
                    //+ Utils.getAuthoritySqlForTender("a.", "aa.RiverBureau")
                    + @")
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230726
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and a.TenderYear >= (YEAR(GetDate())-1916) --前5年案件
                    and (@fUnit='' or aa.Country=@fUnit)";
            }
            else if (mode == 3)
            {//其他補助
                sql += @"
                    FROM PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230726
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and a.TenderYear >= (YEAR(GetDate())-1916) --前5年案件 "
                    //+ Utils.getAuthoritySqlForTender1("a.")
                    + @"
                    and not exists (select Name from Unit where ParentSeq is null and Name=a.ExecUnitName)
                    and not exists (select Country from Country2WRAMapping where Country=substring(a.ExecUnitName,1,3) ) 
                    and (@fUnit='' or a.ExecUnitName=@fUnit)";
            }
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@fUnit", String.IsNullOrEmpty(unit) ? "" : unit);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //品質管制弱面構面 清單
        public List<WeaknessOrientedModel> GetWeaknessOriented()
        {
            string sql = @"
                    SELECT
                        Id,
                        ItemName,
                        ItemType
                    from WeaknessOriented
                    order by id";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<WeaknessOrientedModel>(cmd);
        }
        //十四項指標統計
        public List<T> GetWeaknessSta<T>(int mode, string unit)
        {
            string sql = @"
                    SELECT
                        sum(e.W1) W1,
                        sum(e.W2) W2,
                        sum(e.W3) W3,
                        sum(e.W4) W4,
                        sum(e.W5) W5,
                        sum(e.W6) W6,
                        sum(e.W7) W7,
                        sum(e.W8) W8,
                        sum(e.W9) W9,
                        sum(e.W10) W10,
                        sum(e.W11) W11,
                        sum(e.W12) W12,
                        sum(e.W13) W13,
                        sum(e.W14) W14";

            if (mode == 1)
            {//所屬機關
                sql += @"
                    FROM PrjXML a
                    inner join Unit a2 on(a2.ParentSeq is null and a2.Name=a.ExecUnitName)
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230726
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and a.TenderYear >= (YEAR(GetDate())-1916) --前5年案件
                    and (@fUnit='' or a.ExecUnitName=@fUnit)
                    ";
            }
            else if (mode == 2)
            {//縣市政府
                sql += @"
                    FROM PrjXML a
                    inner join Country2WRAMapping aa on(aa.Country=substring(a.ExecUnitName,1,3) "
                    //+ Utils.getAuthoritySqlForTender("a.", "aa.RiverBureau")
                    + @")
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230726
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and a.TenderYear >= (YEAR(GetDate())-1916) --前5年案件
                    and (@fUnit='' or aa.Country=@fUnit)";
            }
            else if (mode == 3)
            {//其他補助
                sql += @"
                    FROM PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230726
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and a.TenderYear >= (YEAR(GetDate())-1916) --前5年案件 "
                    //+ Utils.getAuthoritySqlForTender1("a.")
                    + @"
                    and not exists (select Name from Unit where ParentSeq is null and Name=a.ExecUnitName)
                    and not exists (select Country from Country2WRAMapping where Country=substring(a.ExecUnitName,1,3) ) 
                    and (@fUnit='' or a.ExecUnitName=@fUnit)";
            }
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@fUnit", String.IsNullOrEmpty(unit) ? "" : unit);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //單位工程案件數
        public List<T> GetTenderList<T>(int mode)
        {
            string sql = @"";

            if (mode == 1)
            {//所屬機關
                sql = @"
                    SELECT
                        count(a.Seq) Amount,
                        a.ExecUnitName UnitName,
                	    a2.OrderNo
                    FROM PrjXML a
                    inner join Unit a2 on(a2.ParentSeq is null and a2.Name=a.ExecUnitName)
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230726
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and a.TenderYear >= (YEAR(GetDate())-1916) --前5年案件
                    group by a2.OrderNo, a.ExecUnitName
                    order by a2.OrderNo, a.ExecUnitName";
            }
            else if (mode == 2)
            {//縣市政府
                sql = @"
                    SELECT
                        count(a.Seq) Amount,
                        aa.Country UnitName
                    FROM PrjXML a
                    inner join Country2WRAMapping aa on(aa.Country=substring(a.ExecUnitName,1,3) "
                    //+ Utils.getAuthoritySqlForTender("a.", "aa.RiverBureau")
                    + @")
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230726
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and a.TenderYear >= (YEAR(GetDate())-1916) --前5年案件
                    group by aa.Country
                    order by aa.Country";
            }
            else if (mode == 3)
            {//其他補助
                sql = @"
                    SELECT
                        count(a.Seq) Amount,
                        a.ExecUnitName UnitName
                    FROM PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230726
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and a.TenderYear >= (YEAR(GetDate())-1916) --前5年案件 "
                    //+ Utils.getAuthoritySqlForTender1("a.")
                    + @"
                    and not exists (select Name from Unit where ParentSeq is null and Name=a.ExecUnitName)
                    and not exists (select Country from Country2WRAMapping where Country=substring(a.ExecUnitName,1,3) ) 
                    group by a.ExecUnitName
                    order by a.ExecUnitName";
            }
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}