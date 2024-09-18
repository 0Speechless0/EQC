using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EADCarbonEmissionService : BaseService
    {//水利工程淨零碳排分析
        public int GetListCount(string unitList, int sYear, int eYear, string refCodeKeyWord ,ref decimal co2Total)
        {
            if (!String.IsNullOrEmpty(unitList)) unitList = String.Format(" and b.Name in ({0}) ", unitList);
            string sql = @"
            select a.Seq into #temp from 
                EngMain a 
				inner join CarbonEmissionHeader b on a.Seq = b.EngMainSeq
                inner join CarbonEmissionPayItem za on(za.CarbonEmissionHeaderSeq=b.Seq)
            where
                (
                    (za.RefItemCode Like  @refCodeKeyWord +'%'  or @refCodeKeyWord = '') or
                    (za.Description Like  '%' + @refCodeKeyWord +'%'  or @refCodeKeyWord = '') 
                )   group by a.Seq;
                select count(z.Seq) total, sum(z.Co2Total) Co2Total
                from (
                    SELECT
                        a.Seq,
                        a.EngYear,
                        a.EngNo,
                        a.EngName,
                        a.AwardAmount,
                        b.Name ExecUnit,
                        (
                            select
                                ROUND(sum(ISNULL(za.ItemKgCo2e,0)),0) Co2Total
                            from CarbonEmissionHeader b
                            inner join CarbonEmissionPayItem za on(za.CarbonEmissionHeaderSeq=b.Seq)
                            where b.EngMainSeq=a.Seq
                            and za.KgCo2e is not null and za.ItemKgCo2e is not null --s20230310
                            and (
                                (za.RefItemCode Like  @refCodeKeyWord +'%'  or @refCodeKeyWord = '') or
                                (za.Description Like  '%' + @refCodeKeyWord +'%'  or @refCodeKeyWord = '') 
                            )
                            --and (za.RStatusCode>50 and za.RStatusCode<200 or za.RStatusCode=201)
                        ) Co2Total
                    FROM EngMain a
                    inner join Unit b on(b.Seq=a.ExecUnitSeq)
                    inner join #temp e on e.Seq = a.Seq
                    where a.EngYear>=@startYear and a.EngYear<=@endYear

 
"

                    + unitList +
                    @"
                ) z
                where z.Co2Total is not null;
                drop table #temp;
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            cmd.Parameters.AddWithValue("@refCodeKeyWord", refCodeKeyWord);

            DataTable dt = db.GetDataTable(cmd);
            try
            {
                co2Total = Convert.ToDecimal(dt.Rows[0]["Co2Total"].ToString());
            }
            catch { }

            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetList<T>(string unitList, int sYear, int eYear, string refCodeKeyWord, int pageRecordCount, int pageIndex)
        {
            if(!String.IsNullOrEmpty(unitList)) unitList = String.Format(" and b.Name in ({0}) ", unitList);
            string sql = @"
                        select a.Seq into #temp from 
                            EngMain a 
							inner join CarbonEmissionHeader b on a.Seq = b.EngMainSeq
                            inner join CarbonEmissionPayItem za on(za.CarbonEmissionHeaderSeq=b.Seq)
                        where
                           (
                                (za.RefItemCode Like  @refCodeKeyWord +'%'  or @refCodeKeyWord = '') or
                                (za.Description Like  '%' + @refCodeKeyWord +'%'  or @refCodeKeyWord = '') 
                            )
                        group by a.Seq;

                select z.* from (
                    SELECT
                        a.Seq,
                        a.EngYear,
                        a.EngNo,
                        a.EngName,
                        a.AwardAmount,
                        b.Name ExecUnit,
                        px.BelongPrj,
                        p.EngType,
                        (
                            select
                                ROUND(sum(ISNULL(za.ItemKgCo2e,0)),0) Co2Total
                            from CarbonEmissionHeader b
                            inner join CarbonEmissionPayItem za on(za.CarbonEmissionHeaderSeq=b.Seq)
                            where b.EngMainSeq=a.Seq
                            and za.KgCo2e is not null and za.ItemKgCo2e is not null --s20230310
                            and (
                                (za.RefItemCode Like  @refCodeKeyWord +'%'  or @refCodeKeyWord = '') or
                                (za.Description Like  '%' + @refCodeKeyWord +'%'  or @refCodeKeyWord = '') 
                            )
                            --and (za.RStatusCode>50 and za.RStatusCode<200 or za.RStatusCode=201)
                        ) Co2Total
                    FROM EngMain a
                    inner join Unit b on(b.Seq=a.ExecUnitSeq)
                    inner join #temp e on (e.Seq = a.Seq)
                    left join PrjXML p on ( p.Seq = a.PrjXMLSeq)
                    left join PrjXMLExt px on ( px.PrjXMLSeq = p.Seq)
                    where a.EngYear>=@startYear and a.EngYear<=@endYear
                     "
                    + unitList +
                    @"
                ) z
                where z.Co2Total is not null
                order by z.EngNo
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY;

                drop table #temp;
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            cmd.Parameters.AddWithValue("@refCodeKeyWord", refCodeKeyWord);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //碳排清單
        public List<T> GetCEList<T>(int engMainSeq, string keyWord, string refCodeKeyWord)
        {
            if (!String.IsNullOrEmpty(keyWord)) keyWord += "%";
            string sql = @"
                select
                    a.Seq,
                    a.CarbonEmissionHeaderSeq,
                    a.PayItem,
                    a.Description,
                    a.Unit,
                    a.Quantity,
                    a.Price,
                    a.Amount,
                    a.ItemKey,
                    a.ItemNo,
                    a.RefItemCode,
                    a.KgCo2e,
                    a.ItemKgCo2e,
                    a.Memo,
                    a.RStatus,
                    a.RStatusCode
                from CarbonEmissionHeader b
                inner join CarbonEmissionPayItem a on(a.CarbonEmissionHeaderSeq=b.Seq and (@keyWord='' or a.PayItem like @keyWord))
                where b.EngMainSeq=@EngMainSeq
                and (
                    (a.RefItemCode Like  @refCodeKeyWord +'%'  or @refCodeKeyWord = '') or
                    (a.Description Like  '%' + @refCodeKeyWord +'%'  or @refCodeKeyWord = '') 
                )
                Order by b.Seq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@refCodeKeyWord", refCodeKeyWord);
            cmd.Parameters.AddWithValue("@keyWord", keyWord);

            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}