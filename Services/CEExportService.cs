using EQC.Common;
using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class CEExportService : BaseService
    {//碳排量計算-匯出
        public List<EngYearVModel> GetEngYearList()
        {
            string sql= @"
                SELECT DISTINCT
                    cast(a.EngYear as integer) EngYear
                FROM EngMain a
                order by EngYear desc
                ";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<EngYearVModel>(cmd);
        }
        //清單
        public List<T> GetList<T>(int year)
        {
            string sql = @"
                SELECT
                    --top 1000
	                a.EngNo,
                    a.EngName,
                    b.Name execUnitName,
                    d.PayItem,
                    d.Description,
                    d.Price,
                    d.Amount,
                    d.Unit,
                    d.Quantity,
                    --d.ItemKey,
                    d.ItemNo,
                    d.KgCo2e,
                    d.ItemKgCo2e,
                    d.RefItemCode,
                    d.Memo,
                    d.RStatus,
                    d.RStatusCode,
                    e.ItemName GreenItemName,
                    d.GreenFundingMemo
                from EngMain a
                inner join Unit b on(b.Seq=a.ExecUnitSeq)
                inner join CarbonEmissionHeader c on(c.EngMainSeq=a.Seq)
                inner join CarbonEmissionPayItem d on(d.CarbonEmissionHeaderSeq=c.Seq)
                left outer join GreenFunding e on(e.Seq=d.GreenFundingSeq)
                where a.EngYear=@year
                order by a.ExecUnitSeq, a.EngNo
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@year", @year);

            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}