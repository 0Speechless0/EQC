using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EADCommitteeService : BaseService
    {//工程採購評選委員分析
        //工程年分清單 20230726
        public List<EngYearVModel> GetEngYearList()
        {
            string sql = @"
                select DISTINCT cast(a.TenderYear as integer) EngYear FROM PrjXML a
                inner join PrjXMLCommittee a1 on(a1.PrjXMLSeq=a.Seq and a1.IsPresence=1)
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                order by EngYear desc
                ";
            
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<EngYearVModel>(cmd);
        }
        //工程清單
        public List<T> GetEngList<T>(string cName, byte kind, int sYear, int eYear)
        {

            string sql = @"
                select a.Seq, a.TenderYear, a.TenderName, a.ExecUnitName, a.BidAmount, a.Location,
                (
                    SELECT STUFF(
                        (
                            SELECT ',' + z.cName FROM (
                                select za.CName from PrjXMLCommittee za
                                where za.PrjXMLSeq=a.Seq
                                ) z
                            FOR XML PATH('')
                        ) ,1,1,'')
                ) AS committees
                FROM PrjXMLCommittee za
                inner join PrjXML a on(
                    a.Seq=za.PrjXMLSeq and a.TenderYear>=@startYear and a.TenderYear<=@endYear
                )
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230726
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                and za.CName=@CName and za.Kind=@Kind
                order by a.TenderYear desc, a.TenderName
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            cmd.Parameters.AddWithValue("@CName", cName);
            cmd.Parameters.AddWithValue("@Kind", kind);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //委員清單
        public List<T> GetList<T>(string unitList, int sYear, int eYear, string committee)
        {
            if (String.IsNullOrEmpty(committee))
                committee = "";
            else
                committee = String.Format("%{0}%", committee);
            if (unitList.Length >0) unitList = String.Format(" and b.ExecUnitName in ({0}) ", unitList);
            string sql = @"
                select * from (
                    SELECT
                        z.CName, z.Kind,
                        count(z.Kind) totalCount,
                        sum(z.Presence) presence,
	                    cast(cast(sum(z.Presence)*100 as decimal)/count(z.Kind) as decimal(6,2)) presenceRate,
                        (select top 1 Profession from PrjXMLCommittee where CName=z.CName and Kind=z.Kind) Profession
                    from (
                        select za.CName, za.Kind, 1 Presence, 0 NoPresence
                        FROM PrjXML a
                        inner join PrjXMLCommittee za on(
                            a.Seq=za.PrjXMLSeq and za.IsPresence=1
                            and (@committee='' or za.CName like @committee)
                        )
                        left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                        CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230726
                        where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                        and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                        and a.TenderYear>=@startYear and a.TenderYear<=@endYear " + unitList + @"

                        union all

                        select za.CName, za.Kind, 0 Presence, 1 NoPresence
                        FROM PrjXML a
                        inner join PrjXMLCommittee za on(
                            a.Seq=za.PrjXMLSeq and za.IsPresence=0
                            and (@committee='' or za.CName like @committee)
                        )
                        left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                        CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230726
                        where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                        and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                        and a.TenderYear>=@startYear and a.TenderYear<=@endYear " + unitList + @"
                    ) z
                    group by z.CName, z.Kind
                ) z1
                order by z1.totalCount desc, z1.CName
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", sYear);
            cmd.Parameters.AddWithValue("@endYear", eYear);
            cmd.Parameters.AddWithValue("@committee", committee);
            //cmd.Parameters.AddWithValue("@All", getAll);
            //cmd.Parameters.AddWithValue("@unitList", unitList);
            //cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}