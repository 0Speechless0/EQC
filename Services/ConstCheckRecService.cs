using EQC.Common;
using EQC.EDMXModel;
using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace EQC.Services
{
    public class ConstCheckRecService : BaseService
    {//抽驗紀錄填報, 已建立範本
     //public const int docstate_Edit = -1;//編輯
     //public const int docstate_SettingComplete = 0;//設置完成

        //有檢驗單之分項工程清單 s20230520
        public List<SelectIntOptionModel> GetRecEngConstruction(int mode, int engMainSeq, int constCheckListSeq)
        {
            string sql = @"
                SELECT distinct e.Seq Value, e.ItemName Text
                FROM EngMain a
                inner join EngConstruction e on(e.EngMainSeq=a.Seq)
                inner join ConstCheckRec f on(f.EngConstructionSeq=e.Seq and f.CCRCheckType1=@mode and f.ItemSeq=@ItemSeq)
                where a.Seq=@EngMainSeq
                ";
            
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@mode", mode);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemSeq", constCheckListSeq);
            return db.GetDataTableWithClass<SelectIntOptionModel>(cmd);
        }
        //檢驗單清單 s20230520
        public List<T> GetList1<T>(int engConstructionSeq, int itemSeq)
        {
            string sql = @"SELECT c.*, e.EngName FROM ConstCheckRec c


                inner join EngConstruction ec on ec.Seq = c.EngConstructionSeq
                inner join EngMain e on e.Seq = ec.EngMainSeq
                where c.EngConstructionSeq=@EngConstructionSeq and c.ItemSeq=@ItemSeq
                order by c.CCRCheckType1, c.CCRCheckDate
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngConstructionSeq", engConstructionSeq);
            cmd.Parameters.AddWithValue("@ItemSeq", itemSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //工程-分項工程資訊 s20230520
        public List<T> GetEngConstruction<T>(int engMainSeq, int mode, int constCheckListSeq, bool auth = true)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    a.EngNo,
                    a.EngName,
                    a.ApproveDate,
                    a.ApproveNo,
                    a.SupervisorUnitName,
                    a.SupervisorContact,
                    c.Name organizerUnitName,
                    c1.Name execUnitName,
                    c2.Name execSubUnitName,
                    d.DocState,
                    e.ItemName subEngName,
                    e.ItemNo subEngItemNo,
                    e.Seq subEngNameSeq
                FROM EngConstruction e
                inner join EngMain a on(a.Seq=e.EngMainSeq)
                left outer join Unit c on(c.Seq=a.OrganizerUnitSeq)
                left outer join Unit c1 on(c1.Seq=a.ExecUnitSeq)
                left outer join Unit c2 on(c2.Seq=a.ExecSubUnitSeq)
                left outer join SupervisionProjectList d on(
                    d.EngMainSeq=a.Seq
                    and d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                )
                where e.Seq in (
                    SELECT distinct f.EngConstructionSeq FROM EngMain a
                    inner join EngConstruction e on(e.EngMainSeq=a.Seq)
                    inner join ConstCheckRec f on(f.EngConstructionSeq=e.Seq and f.CCRCheckType1=@mode and f.ItemSeq=@ItemSeq)
                    where a.Seq=@EngMainSeq
                )"
                +( auth ? Utils.getAuthoritySql("a.") :  "");
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@mode", mode);
            cmd.Parameters.AddWithValue("@ItemSeq", constCheckListSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //1 施工抽查清單 s20230520
        public List<T> GetConstCheckList1<T>(int engMain)
        {
            string sql = @"
                SELECT
                    a.Seq, a.OrderNo, a.ItemName
                    ,(
                        select COUNT(z1.Seq) from ConstCheckRec z1
                        where z1.EngConstructionSeq in(
                            select zb.Seq from EngMain za
                            inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)                                
                            where za.Seq=@EngMainSeq
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
                            where za.Seq=@EngMainSeq
                            and z1.ItemSeq=a.Seq
                        )
                        and z1.CCRCheckType1=1
                    ) missingCount
                FROM ConstCheckList a
                where a.EngMainSeq=@EngMainSeq
                and a.DataKeep=1
                order by a.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMain);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //2 設備運轉測試清單 s20230520
        public List<T> GetEquOperTestList1<T>(int engMain)
        {
            string sql = @"
                SELECT
                    a.Seq, a.OrderNo, a.ItemName
                    ,(
                        select COUNT(z1.Seq) from ConstCheckRec z1
                        where z1.EngConstructionSeq in(
                            select zb.Seq from EngMain za
                            inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)                                
                            where za.Seq=@EngMainSeq
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
                            where za.Seq=@EngMainSeq
                            and z1.ItemSeq=a.Seq
                        )
                        and z1.CCRCheckType1=2
                    ) missingCount
                FROM EquOperTestList a
                where a.EngMainSeq=@EngMainSeq
                and a.DataKeep=1
                order by a.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMain);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //3 職業安全衛生清單 s20230520
        public List<T> GetOccuSafeHealthList1<T>(int engMain)
        {
            string sql = @"
                SELECT
                    a.Seq, a.OrderNo, a.ItemName
                    ,(
                        select COUNT(z1.Seq) from ConstCheckRec z1
                        where z1.EngConstructionSeq in(
                            select zb.Seq from EngMain za
                            inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)                                
                            where za.Seq=@EngMainSeq
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
                            where za.Seq=@EngMainSeq
                            and z1.ItemSeq=a.Seq
                        )
                        and z1.CCRCheckType1=3
                    ) missingCount
                FROM OccuSafeHealthList a
                where a.EngMainSeq=@EngMainSeq
                and a.DataKeep=1
                order by a.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMain);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //4 環境保育清單 s20230520
        public List<T> GetEnvirConsList1<T>(int engMain)
        {
            string sql = @"
                SELECT
                    a.Seq, a.OrderNo, a.ItemName
                    ,(
                        select COUNT(z1.Seq) from ConstCheckRec z1
                        where z1.EngConstructionSeq in(
                            select zb.Seq from EngMain za
                            inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)                                
                            where za.Seq=@EngMainSeq
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
                            where za.Seq=@EngMainSeq
                            and z1.ItemSeq=a.Seq
                        )
                        and z1.CCRCheckType1=4
                    ) missingCount
                FROM EnvirConsList a
                where a.EngMainSeq=@EngMainSeq
                and a.DataKeep=1
                order by a.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMain);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //工程年分清單
        public List<EngYearVModel> GetEngYearList()
        {
            string sql = @"
                SELECT DISTINCT
                    cast(a.EngYear as integer) EngYear
                FROM EngMain a
                inner join SupervisionProjectList z on(z.EngMainSeq=a.Seq)
                where 1=1"
                + Utils.getAuthoritySql("a.") //a.CreateUserSeq=@CreateUserSeq
                + @" and exists (select EngMainSeq from EngConstruction where EngMainSeq=a.Seq)
                order by EngYear DESC";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<EngYearVModel>(cmd);
        }
        //工程執行機關清單
        public List<EngExecUnitsVModel> GetEngExecUnitList(string engYear)
        {
            string sql = @"
                SELECT DISTINCT
                    b.OrderNo,
                    a.ExecUnitSeq UnitSeq,
                    b.Name UnitName
                FROM EngMain a
                inner join SupervisionProjectList z on(z.EngMainSeq=a.Seq)            
                inner join Unit b on(b.Seq=a.ExecUnitSeq and b.parentSeq is null)
                where a.EngYear=@EngYear"
                + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @" and exists (select EngMainSeq from EngConstruction where EngMainSeq=a.Seq)
                order by b.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngYear", engYear);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<EngExecUnitsVModel>(cmd);
        }
        //工程執行單位清單
        public List<EngExecUnitsVModel> GetEngExecSubUnitList(string engYear, int parentSeq)
        {
            string sql = @"
                SELECT DISTINCT
                    b.OrderNo,
                    a.ExecSubUnitSeq UnitSeq,
                    b.Name UnitName
                FROM EngMain a
                inner join SupervisionProjectList z on(z.EngMainSeq=a.Seq)
                inner join Unit b on(b.Seq=a.ExecSubUnitSeq and @ParentSeq=b.parentSeq)
                where a.EngYear=@EngYear"
                + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @" and exists (select EngMainSeq from EngConstruction where EngMainSeq=a.Seq)
                order by b.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngYear", engYear);
            cmd.Parameters.AddWithValue("@ParentSeq", parentSeq);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<EngExecUnitsVModel>(cmd);
        }
        //工程主要施工項目
        public List<T> GetSubEngList<T>(int engMain)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    a.ItemName
                FROM EngConstruction a
                inner join SupervisionProjectList d on(
                    d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.EngMainSeq)
                )
                where a.EngMainSeq=@EngMainSeq
                order by a.OrderNo
				"; 

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMain);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //工程主要施工項目 s20230520
        public List<T> GetSubEngList1<T>(int engMain)
        {
            string sql = @"
                select
	                z.*
                from (    
                    SELECT
                        a.OrderNo,
                        a.Seq,
                        a.ItemName
                    FROM EngConstruction a
                    inner join SupervisionProjectList d on(
                        d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.EngMainSeq)
                    )
                    where a.EngMainSeq=@EngMainSeq
                    and a.OrderNo>0

                    union all

                    SELECT
                        a.OrderNo* -1000 OrderNo,
                        a.Seq,
                        a.ItemName
                    FROM EngConstruction a
                    inner join SupervisionProjectList d on(
                        d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.EngMainSeq)
                    )
                    where a.EngMainSeq=@EngMainSeq
                    and a.OrderNo<0
                ) z
                order by z.OrderNo
				";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMain);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //工程名稱清單
        //public List<T> GetEngCreatedNameList<T>(string year, int unitSeq, int subUnitSeq)
        //{
        //    string sql = @"";
        //    if (subUnitSeq == -1)
        //    {
        //        sql = @"
        //            SELECT
        //                a.Seq,
        //                a.EngName
        //            FROM EngMain a
        //            inner join SupervisionProjectList d on(
        //                d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
        //            )


        //            where a.EngYear=@EngYear
        //            and exists (select EngMainSeq from EngConstruction where EngMainSeq=a.Seq)
        //            and a.ExecUnitSeq=@ExecUnitSeq"
        //            + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
        //            + @" order by EngNo DESC
        //            ";
        //    }
        //    else
        //    {
        //        sql = @"
        //            SELECT
        //                a.Seq,
        //                a.EngName
        //            FROM EngMain a
        //            inner join SupervisionProjectList d on(
        //                d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
        //            )
        //            where a.EngYear=@EngYear
        //            and exists (select EngMainSeq from EngConstruction where EngMainSeq=a.Seq)
        //            and a.ExecSubUnitSeq=@ExecSubUnitSeq"
        //            + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
        //            + @" order by EngNo DESC
        //            ";
        //    }
        //    SqlCommand cmd = db.GetCommand(sql);
        //    cmd.Parameters.AddWithValue("@EngYear", year);
        //    cmd.Parameters.AddWithValue("@ExecUnitSeq", unitSeq);
        //    cmd.Parameters.AddWithValue("@ExecSubUnitSeq", subUnitSeq);
        //    cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
        //    return db.GetDataTableWithClass<T>(cmd);
        //}
        public List<T> GetEngCreatedNameList<T>(string year, int unitSeq, int subUnitSeq)
        {
            string sql = @"";
            if (subUnitSeq == -1)
            {
                sql = @"
                    SELECT
                        a.Seq,
                        a.EngName
                    FROM EngMain a
                    inner join Unit b on(b.Seq=a.ExecUnitSeq)
                    inner join SupervisionProjectList d on(
                        d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                    )
                    left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)


                    where a.EngYear=@EngYear
                    and a.PrjXMLSeq is not null ---shioulo 20220618
                    and a.ExecUnitSeq=@ExecUnitSeq "
                    + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                    + @" order by EngNo DESC
                    ";
            }
            else
            {
                sql = @"
                    SELECT
                        a.Seq,
                        a.EngName
                    FROM EngMain a
                    inner join Unit b on(b.Seq=a.ExecUnitSeq and a.ExecUnitSeq=@ExecUnitSeq)
                    inner join SupervisionProjectList d on(
                        d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                    )
                    left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)
                    where a.EngYear=@EngYear
                    and a.EngYear=@EngYear
                    and a.PrjXMLSeq is not null ---shioulo 20220618
                    and a.ExecSubUnitSeq=@ExecSubUnitSeq "
                    + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                    + @" order by EngNo DESC
                    ";
            }
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngYear", year);
            cmd.Parameters.AddWithValue("@ExecUnitSeq", unitSeq);
            cmd.Parameters.AddWithValue("@ExecSubUnitSeq", subUnitSeq);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<T>(cmd);
        }

        internal List<object> GetEngCreatedList(string engNo)
        {
            string sql = @"
                    SELECT
                        a.Seq,
                        a.EngNo,
                        a.EngName,
                        b.Name ExecUnit, 
                        c.Name ExecSubUnit,
                        a.SupervisorUnitName,
                        a.ApproveDate,
                        d.DocState,
                        f.ItemNo subEngNo,
                        f.ItemName subEngName,
                        f.Seq subEngNameSeq,
						cast((
									select COUNT(z1.Seq)  from ConstCheckRec z1
									where z1.EngConstructionSeq=f.Seq
								)as int) constCheckRecCount,
                        cast((
                            select COUNT(z2.Seq)  from ConstCheckRec z1
                            inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq=z1.Seq and z2.CCRCheckResult=2)
                            where z1.EngConstructionSeq=f.Seq
						)as int) missingCount
                    FROM EngMain a
                    inner join EngConstruction f on(f.EngMainSeq=a.Seq  )
                    inner join Unit b on(b.Seq=a.ExecUnitSeq)
                    inner join SupervisionProjectList d on(
                        d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                    )
                    left outer join Unit c on(c.Seq=a.ExecSubUnitSeq) 
                    where a.EngNo = @engNo
                    ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@engNo", engNo);

            return db.GetDataTable(cmd).Rows.Cast<DataRow>()
            .Where(row => row.Field<int>("constCheckRecCount") > 0 )
            .Select(row => new
            {
                subEngSeq = row.Field<int>("subEngNameSeq"),
                subEngNo = row.Field<string>("subEngNo"),
                subEngName = row.Field<string>("subEngName"),
                missingCount = row.Field<int>("missingCount"),
            }).ToList<object>();
        }

        //
        public int GetEngCreatedListCount(string year, int unitSeq, int subUnitSeq, int engMain, int subEngMain)
        {
            string sql = @"";
            if (subUnitSeq == -1)
            {
                sql = @"
                SELECT
                    count(a.Seq) total
                FROM EngMain a
                inner join EngConstruction f on(f.EngMainSeq=a.Seq and( (-1=@subEngMain)or(f.Seq=@subEngMain)) )
                inner join Unit b on(b.Seq=a.ExecUnitSeq)
                inner join SupervisionProjectList d on(
                    d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                )
                where a.Seq=@Seq
                and a.EngYear=@EngYear
                and a.ExecUnitSeq=@ExecUnitSeq"
                + Utils.getAuthoritySql("a."); //and a.CreateUserSeq=@CreateUserSeq";
            }
            else
            {
                sql = @"
                SELECT
                    count(a.Seq) total
                FROM EngMain a
                inner join EngConstruction f on(f.EngMainSeq=a.Seq and( (-1=@subEngMain)or(f.Seq=@subEngMain)) )
                inner join Unit b on(b.Seq=a.ExecSubUnitSeq and a.ExecUnitSeq=@ExecUnitSeq)
                inner join SupervisionProjectList d on(
                    d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                )
                where a.Seq=@Seq
                and a.EngYear=@EngYear
                and a.ExecSubUnitSeq=@ExecSubUnitSeq"
                + Utils.getAuthoritySql("a."); //and a.CreateUserSeq=@CreateUserSeq";
            }
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", engMain);
            cmd.Parameters.AddWithValue("@EngYear", year);
            cmd.Parameters.AddWithValue("@ExecUnitSeq", unitSeq);
            cmd.Parameters.AddWithValue("@ExecSubUnitSeq", subUnitSeq);
            cmd.Parameters.AddWithValue("@subEngMain", subEngMain);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());

            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetEngCreatedList<T>(int engMain)
        {
            string sql = @"
                    SELECT
                            a.Seq,
                            a.EngNo,
                            a.EngName,
                            b.Name ExecUnit, 
                            c.Name ExecSubUnit,
                            a.SupervisorUnitName,
                            a.ApproveDate,
                            d.DocState,
                            f.ItemNo subEngNo,
                            f.ItemName subEngName,
                            f.Seq subEngNameSeq,
                            cast((
                                select COUNT(z1.Seq)  from ConstCheckRec z1
                                where z1.EngConstructionSeq=f.Seq
						    )as int) constCheckRecCount, -- shioulo 20221216
                            cast((
                                select COUNT(z2.Seq)  from ConstCheckRec z1
                                inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq=z1.Seq and z2.CCRCheckResult=2)
                                where z1.EngConstructionSeq=f.Seq
						    )as int) missingCount
                        FROM EngMain a
                        inner join EngConstruction f on(f.EngMainSeq=a.Seq )
                        inner join Unit b on(b.Seq=a.ExecUnitSeq)
                        inner join SupervisionProjectList d on(
                            d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                        )
                        left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)
                    
                        where a.Seq=@Seq
                        order by a.EngNo DESC,f.ItemNo
                        ";
            /*if (subUnitSeq == -1)
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
                            d.DocState,
                            f.ItemNo subEngNo,
                            f.ItemName subEngName,
                            f.Seq subEngNameSeq,
                            cast((
                                select COUNT(z1.Seq)  from ConstCheckRec z1
                                where z1.EngConstructionSeq=f.Seq
						    )as int) constCheckRecCount, -- shioulo 20221216
                            cast((
                                select COUNT(z2.Seq)  from ConstCheckRec z1
                                inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq=z1.Seq and z2.CCRCheckResult=2)
                                where z1.EngConstructionSeq=f.Seq
						    )as int) missingCount
                        FROM EngMain a
                        inner join EngConstruction f on(f.EngMainSeq=a.Seq and( (-1=@subEngMain)or(f.Seq=@subEngMain)) )
                        inner join Unit b on(b.Seq=a.ExecUnitSeq)
                        inner join SupervisionProjectList d on(
                            d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                        )
                        left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)
                    
                        where a.Seq=@Seq
                        --and a.EngYear=@EngYear
                        --and a.ExecUnitSeq=@ExecUnitSeq
                        "
                        + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                        + @" order by a.EngNo DESC,f.ItemNo
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
                        d.DocState,
                        f.ItemNo subEngNo,
                        f.ItemName subEngName,  
                        f.Seq subEngNameSeq,
                        cast((
                            select COUNT(z2.Seq)  from ConstCheckRec z1
                            inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq=z1.Seq and z2.CCRCheckResult=2)
                            where z1.EngConstructionSeq=f.Seq
						)as int) missingCount
                    FROM EngMain a
                    inner join EngConstruction f on(f.EngMainSeq=a.Seq and( (-1=@subEngMain)or(f.Seq=@subEngMain)) )
                    inner join Unit b on(b.Seq=a.ExecUnitSeq and a.ExecUnitSeq=@ExecUnitSeq)
                    inner join SupervisionProjectList d on(
                        d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                    )
                    left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)
                    where a.Seq=@Seq
                    --and a.EngYear=@EngYear
                    --and a.ExecSubUnitSeq=@ExecSubUnitSeq
                    "
                    + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                    + @" order by a.EngNo DESC, f.ItemNo
                    OFFSET @pageIndex ROWS
				    FETCH FIRST @pageRecordCount ROWS ONLY";
            }*/
            SqlCommand cmd = db.GetCommand(sql);
            //cmd.Parameters.AddWithValue("@EngYear", year);
            //cmd.Parameters.AddWithValue("@ExecUnitSeq", unitSeq);
            //cmd.Parameters.AddWithValue("@ExecSubUnitSeq", subUnitSeq);
            cmd.Parameters.AddWithValue("@Seq", engMain);
            //cmd.Parameters.AddWithValue("@subEngMain", subEngMain);
            //cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            //cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            //cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<T>(cmd);
        }


        //Android get 分項工程

        public List<T> GetEngPostCreatedList<T>( string EnItenSeq, string EngNo)
        {
            string sql = @"";


            sql = @"
                   SELECT
                        a.Seq,
                        a.EngNo,
                        a.EngName,
                        b.Name ExecUnit,
                        c.Name ExecSubUnit,
                        a.SupervisorUnitName,
                        a.ApproveDate,
                        d.DocState,
                        f.ItemName subEngName,
                        f.Seq subEngNameSeq,
                        cast((
                            select COUNT(z2.Seq)  from ConstCheckRec z1
                            inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq=z1.Seq and z2.CCRCheckResult=2)
                            where z1.EngConstructionSeq=f.Seq
                        )as int) missingCount   
                    FROM EngMain a
                    inner join EngConstruction f on(f.EngMainSeq=a.Seq )
                    inner join Unit b on(b.Seq=a.ExecUnitSeq)
                    inner join SupervisionProjectList d on(d.EngMainSeq=a.Seq)
                    left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)
                    where 
                    f.Seq = @EnItenSeq
                    and a.EngNo = @EngNo";
            

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EnItenSeq", EnItenSeq);
            cmd.Parameters.AddWithValue("@EngNo", EngNo);

            return db.GetDataTableWithClass<T>(cmd);
        }


        //由 工程主要施工項目 取得工程資訊
        public List<T> GetEngMainByEngConstructionSeq<T>(int seq)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    a.EngNo,
                    a.EngName,
                    a.ApproveDate,
                    a.ApproveNo,
                    a.SupervisorUnitName,
                    a.SupervisorContact,
                    c.Name organizerUnitName,
                    c1.Name execUnitName,
                    c2.Name execSubUnitName,
                    d.DocState,
                    e.ItemName subEngName,
                    e.ItemNo subEngItemNo,
                    e.Seq subEngNameSeq
                FROM EngConstruction e
                inner join EngMain a on(a.Seq=e.EngMainSeq)
                left outer join Unit c on(c.Seq=a.OrganizerUnitSeq)
                left outer join Unit c1 on(c1.Seq=a.ExecUnitSeq)
                left outer join Unit c2 on(c2.Seq=a.ExecSubUnitSeq)
                left outer join SupervisionProjectList d on(
                    d.EngMainSeq=a.Seq
                    and d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                )
                where e.Seq=@Seq"
                + Utils.getAuthoritySql("a.");
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //設備運轉測試清單
        public List<T> GetEquOperTestOption<T>(int engMain)
        {
            string sql = @"
                SELECT Seq as [Value], ItemName as [Text]
                FROM EquOperTestList
                where EngMainSeq=@EngMainSeq
                and DataKeep=1
                order by OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMain);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //已有檢驗單之檢驗項目
        public List<T> GetRecCheckTypeOption<T>(int constructionSeq)
        {
            string sql = @"
                SELECT distinct CCRCheckType1 Value from ConstCheckRec
                where EngConstructionSeq=@EngConstructionSeq 
                order by CCRCheckType1";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngConstructionSeq", constructionSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //設備運轉測試 標準清單
        public List<T> GetEquOperControlSt<T>(int stdType)
        {
            string sql = @"
                SELECT
                    a.Seq ControllStSeq,
                    a.EPCheckItem1 CheckItem1,
                    a.EPCheckItem2 CheckItem2,
                    a.EPStand1 Stand1,
                    a.EPStand2 Stand2,
                    a.EPStand3 Stand3,
                    a.EPStand4 Stand4,
                    a.EPStand5 Stand5,
                    a.EPCheckFields CheckFields
                FROM EquOperControlSt a
                where a.EquOperTestStSeq = @EquOperTestStSeq
                and a.DataKeep=1
                order by a.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EquOperTestStSeq", stdType);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //施工抽查清單
        public List<T> GetConstCheckList<T>(int engMain)
        {
            string sql = @"
                SELECT Seq as [Value], ItemName as [Text]
                FROM ConstCheckList
                where EngMainSeq=@EngMainSeq
                and DataKeep=1
                order by OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMain);
            return db.GetDataTableWithClass<T>(cmd);
        }
        
        //施工抽查清單 標準清單
        public List<T> GetConstCheckControlSt<T>(int stdType, int checkFlow)
        {
            string sql = @"
                SELECT
                    a.Seq ControllStSeq,
                    a.CCManageItem1 CheckItem1,
                    a.CCManageItem2 CheckItem2,
                    a.CCCheckStand1 Stand1,
                    a.CCCheckStand2 Stand2,
                    '' Stand3,
                    '' Stand4,
                    '' Stand5,
                    a.CCCheckFields CheckFields
                FROM ConstCheckControlSt a
                where a.ConstCheckListSeq = @ConstCheckListSeq
                and a.DataKeep=1
                and a.CCFlow1=@CCFlow1
                order by a.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@ConstCheckListSeq", stdType);
            cmd.Parameters.AddWithValue("@CCFlow1", checkFlow);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //職業安全衛生清單
        public List<T> GetOccuSafeHealthList<T>(int engMain)
        {
            string sql = @"
                SELECT Seq as [Value], ItemName as [Text]
                FROM OccuSafeHealthList
                where EngMainSeq=@EngMainSeq
                and DataKeep=1
                order by OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMain);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //職業安全衛生 標準清單
        public List<T> GetOccuSafeHealthControlSt<T>(int stdType)
        {
            string sql = @"
                SELECT
                    a.Seq ControllStSeq,
                    a.OSCheckItem1 CheckItem1,
                    a.OSCheckItem2 CheckItem2,
                    a.OSStand1 Stand1,
                    a.OSStand2 Stand2,
                    a.OSStand3 Stand3,
                    a.OSStand4 Stand4,
                    a.OSStand5 Stand5,
                    a.OSCheckFields CheckFields
                FROM OccuSafeHealthControlSt a
                where a.OccuSafeHealthListSeq = @OccuSafeHealthListSeq
                and a.DataKeep=1
                order by a.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@OccuSafeHealthListSeq", stdType);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //環境保育清單
        public List<T> GetEnvirConsList<T>(int engMain)
        {
            string sql = @"
                SELECT Seq as [Value], ItemName as [Text]
                FROM EnvirConsList
                where EngMainSeq=@EngMainSeq
                and DataKeep=1
                order by OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMain);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //環境保育 標準清單
        public List<T> GetEnvirConsControlSt<T>(int stdType, int checkFlow)
        {
            string sql = @"
                SELECT
                    a.Seq ControllStSeq,
                    a.ECCCheckItem1 CheckItem1,
                    a.ECCCheckItem2 CheckItem2,
                    a.ECCStand1 Stand1,
                    a.ECCStand2 Stand2,
                    a.ECCStand3 Stand3,
                    a.ECCStand4 Stand4,
                    a.ECCStand5 Stand5,
                    a.ECCCheckFields CheckFields
                FROM EnvirConsControlSt a
                where a.EnvirConsListSeq = @EnvirConsListSeq
                and a.DataKeep=1
                and a.ECCFlow1=@ECCFlow1
                order by a.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EnvirConsListSeq", stdType);
            cmd.Parameters.AddWithValue("@ECCFlow1", checkFlow);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //新增
        public bool Add(ConstCheckRecModel m)
        {
            string sql = "";
            Null2Empty(m);
            db.BeginTransaction();
            try { 
                sql = @"
                    insert into ConstCheckRec(
                        EngConstructionSeq,
                        CCRCheckType1,
                        ItemSeq,
                        CCRCheckFlow,
                        CCRCheckDate,
                        CCRPosLati,
                        CCRPosLong,
                        CCRPosDesc,
                        IsManageConfirm,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EngConstructionSeq,
                        @CCRCheckType1,
                        @ItemSeq,
                        @CCRCheckFlow,
                        @CCRCheckDate,
                        @CCRPosLati,
                        @CCRPosLong,
                        @CCRPosDesc,
                        0,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngConstructionSeq", m.EngConstructionSeq);
                cmd.Parameters.AddWithValue("@CCRCheckType1", m.CCRCheckType1);
                cmd.Parameters.AddWithValue("@ItemSeq", m.ItemSeq);
                cmd.Parameters.AddWithValue("@CCRCheckFlow", m.CCRCheckFlow);
                cmd.Parameters.AddWithValue("@CCRCheckDate", m.CCRCheckDate);
                cmd.Parameters.AddWithValue("@CCRPosLati", this.NulltoDBNull(m.CCRPosLati));
                cmd.Parameters.AddWithValue("@CCRPosLong", this.NulltoDBNull(m.CCRPosLong));
                cmd.Parameters.AddWithValue("@CCRPosDesc", m.CCRPosDesc);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('ConstCheckRec') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);
                m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
                //檢驗項目
                sql = null;
                if (m.CCRCheckType1 == 1)//1:施工抽查
                {
                    sql = @"insert into ConstCheckRecResult(
	                        ConstCheckRecSeq,ControllStSeq,CCRCheckResult,CCRIsNCR
                        ) 
                        SELECT @ConstCheckRecSeq,Seq,1,0 from ConstCheckControlSt a where a.ConstCheckListSeq=@ItemSeq and a.CCFlow1=@CheckFlow";
                }
                else if (m.CCRCheckType1 == 2)//2:設備運轉測試
                {
                    sql = @"insert into ConstCheckRecResult(
	                        ConstCheckRecSeq,ControllStSeq,CCRCheckResult,CCRIsNCR
                        ) 
                        SELECT @ConstCheckRecSeq,Seq,1,0 from EquOperControlSt a where a.EquOperTestStSeq=@ItemSeq";
                }
                else if (m.CCRCheckType1 == 3)//3:職業安全
                {
                    sql = @"insert into ConstCheckRecResult(
	                        ConstCheckRecSeq,ControllStSeq,CCRCheckResult,CCRIsNCR
                        ) 
                        SELECT @ConstCheckRecSeq,Seq,1,0 from OccuSafeHealthControlSt a where a.OccuSafeHealthListSeq=@ItemSeq";
                }
                else if (m.CCRCheckType1 == 4)//4:環境保育
                {
                    sql = @"insert into ConstCheckRecResult(
	                        ConstCheckRecSeq,ControllStSeq,CCRCheckResult,CCRIsNCR
                        ) 
                        SELECT @ConstCheckRecSeq,Seq,1,0 from EnvirConsControlSt a where a.EnvirConsListSeq=@ItemSeq and a.ECCFlow1=@CheckFlow";
                }
                else
                {
                    db.TransactionRollback();
                    log.Info("ConstCheckRecService.Add 檢驗標準項目對應錯誤:"+m.CCRCheckType1);
                    return false;
                }
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ConstCheckRecSeq", m.Seq);
                cmd.Parameters.AddWithValue("@ItemSeq", m.ItemSeq);
                cmd.Parameters.AddWithValue("@CheckFlow", m.CCRCheckFlow);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();

                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("ConstCheckRecService.Add: " + e.Message);
                log.Info(sql);
                return false;
            }
        }

        public void UpdateRecResultStandard(List<ControlStVModel> items)
        {
            using(var context = new EQC_NEW_Entities())
            {
                var originResultGroup = context.ConstCheckRecResultStandard
                    .GroupBy(r => r.ConstCheckRecResultSeq)
                    .ToDictionary(r => r.Key, r => r.ToList()) ;
                items.ForEach(item =>
                {
                    if (originResultGroup.TryGetValue(item.Seq, out List<ConstCheckRecResultStandard> standardValues))
                    {
                        int i = 0;
                        item.StandardValuesStr.Split(',')
                        .ToList()
                        .ForEach(targetValue =>
                        {
                            standardValues[i++].Value = targetValue;
                        });
                    }
                    else
                    {
                        item.StandardValuesStr?.Split(',')
                         .Where(r => r != "")
                         .ToList()
                         .ForEach(targetValue =>
                         {
                             context.ConstCheckRecResultStandard.Add(new ConstCheckRecResultStandard
                             {
                                 ConstCheckRecResultSeq = item.Seq,
                                 Value = targetValue
                             });
                         });
                        
                    }
                });
                context.SaveChanges();
            }
        }
        //更新
        public bool Update(ConstCheckRecModel recItem, List<ControlStVModel> items)
        {
            string sql = "";
            Null2Empty(recItem);
            foreach (ControlStVModel m in items) Null2Empty(m);

            db.BeginTransaction();
            try
            {
                /*sql = @"delete ConstCheckRecResult where ConstCheckRecSeq=@ConstCheckRecSeq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ConstCheckRecSeq", recItem.Seq);
                db.ExecuteNonQuery(cmd);*/

                sql = @"update ConstCheckRecResult set
	                        ConstCheckRecSeq=@ConstCheckRecSeq,
                            ControllStSeq=@ControllStSeq,
                            CCRRealCheckCond=@CCRRealCheckCond,
                            CCRCheckResult=@CCRCheckResult,
                            RecResultRemark= @RecResultRemark,
                            CCRIsNCR=@CCRIsNCR,
                            ResultItem=@ResultItem
                        where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                foreach (ControlStVModel m in items)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", m.Seq);
                    cmd.Parameters.AddWithValue("@ConstCheckRecSeq", m.ConstCheckRecSeq);
                    cmd.Parameters.AddWithValue("@ControllStSeq", m.ControllStSeq);
                    cmd.Parameters.AddWithValue("@CCRRealCheckCond", m.CCRRealCheckCond);
                    cmd.Parameters.AddWithValue("@CCRCheckResult", this.NulltoDBNull(m.CCRCheckResult));
                    cmd.Parameters.AddWithValue("@RecResultRemark", this.NulltoDBNull(m.RecResultRemark));
                    cmd.Parameters.AddWithValue("@CCRIsNCR", this.NulltoDBNull(m.CCRIsNCR));
                    cmd.Parameters.AddWithValue("@ResultItem", m.ResultItem);
                    db.ExecuteNonQuery(cmd);
                }

                sql = @"update ConstCheckRec set
                            CCRCheckDate=@CCRCheckDate, 
                            CCRPosLati=@CCRPosLati,
                            CCRPosLong=@CCRPosLong,
                            CCRPosDesc=@CCRPosDesc,
                            ModifyTime=GetDate(),
                            ModifyUserSeq=@ModifyUserSeq,
                            SupervisionDirectorSignature = @SupervisionDirectorSignature,
                            SupervisionComSignature = @SupervisionComSignature
	                    where Seq=@Seq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", recItem.Seq);
                cmd.Parameters.AddWithValue("@CCRCheckDate", recItem.CCRCheckDate);
                cmd.Parameters.AddWithValue("@CCRPosLati", this.NulltoDBNull(recItem.CCRPosLati));
                cmd.Parameters.AddWithValue("@CCRPosLong", this.NulltoDBNull(recItem.CCRPosLong));
                cmd.Parameters.AddWithValue("@CCRPosDesc", recItem.CCRPosDesc);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                cmd.Parameters.AddWithValue("@SupervisionComSignature", recItem.SupervisionComSignature);
                cmd.Parameters.AddWithValue("@SupervisionDirectorSignature", recItem.SupervisionDirectorSignature);
                db.ExecuteNonQuery(cmd);
                db.TransactionCommit();

                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("ConstCheckRecService.Update: " + e.Message);
                log.Info(sql);
                return false;
            }
        }
        //確認 抽查單
        public int FormConfirm(int seq, int state)
        {
            string sql = @"update ConstCheckRec set
                        FormConfirm=@FormConfirm,
                        SupervisorUserSeq=@ModifyUserSeq,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            cmd.Parameters.AddWithValue("@FormConfirm", state);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
            return db.ExecuteNonQuery(cmd);
        }
        //刪除檢驗單
        public bool DelRec(int seq)
        {
            string sql = "";
            db.BeginTransaction();
            try
            {
                sql = @"delete ConstCheckRecResult where ConstCheckRecSeq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                sql = @"delete ConstCheckRecFile where ConstCheckRecSeq=@Seq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                sql = @"delete ConstCheckRec where Seq=@Seq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();

                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("ConstCheckRecService.Add: " + e.Message);
                log.Info(sql);
                return false;
            }
        }

        public List<T> GetItem<T>(int seq)
        {
            string sql = @"SELECT
                Seq,
                EngConstructionSeq,
                CCRCheckType1,
                ItemSeq,
                CCRCheckFlow,
                CCRCheckDate,
                CCRPosLati,
                CCRPosLong,
                CCRPosDesc,
                FormConfirm,
                SupervisionComSignature,
                SupervisionDirectorSignature
                FROM ConstCheckRec
                where Seq=@Seq
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //檢驗單清單
        public List<T> GetList<T>(int engConstructionSeq)
        {
            string sql = @"SELECT * FROM ConstCheckRec
                where EngConstructionSeq=@EngConstructionSeq
                order by CCRCheckType1,CCRCheckDate
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngConstructionSeq", engConstructionSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //檢驗單清單 單一項目
        public List<T> GetListByCheckType<T>(int engConstructionSeq, int checkType)
        {
            string sql = @"SELECT
                Seq,
                EngConstructionSeq,
                CCRCheckType1,
                ItemSeq,
                CCRCheckFlow,
                CCRCheckDate,
                CCRPosLati,
                CCRPosLong,
                CCRPosDesc,
                FormConfirm
                FROM ConstCheckRec
                where EngConstructionSeq=@EngConstructionSeq
                and CCRCheckType1=@CCRCheckType1
                order by CCRCheckDate DESC
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngConstructionSeq", engConstructionSeq);
            cmd.Parameters.AddWithValue("@CCRCheckType1", checkType);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //檢驗單清單 不分分項工程
        public List<T> GetListByCheckType1<T>(int engMainSeq, int checkType, int cCRCheckFlow, int itemSeq, int pageRecordCount, int pageIndex)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    a.EngConstructionSeq,
                    a.CCRCheckType1,
                    a.ItemSeq,
                    a.CCRCheckFlow,
                    a.CCRCheckDate,
                    a.CCRPosLati,
                    a.CCRPosLong,  
                    a.CCRPosDesc,
                    a.FormConfirm,
                    a.IsFromMobile,
                    b.ItemName
                FROM EngConstruction b
                inner join ConstCheckRec a on(a.EngConstructionSeq=b.Seq)
                where b.EngMainSeq=@EngMainSeq
                and a.CCRCheckType1=@CCRCheckType1
                and a.ItemSeq=@ItemSeq
                and (@CCRCheckFlow=-1 or @CCRCheckType1=2 or @CCRCheckType1=3 or a.CCRCheckFlow=@CCRCheckFlow)
                order by a.CCRCheckDate DESC
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@CCRCheckType1", checkType);
            cmd.Parameters.AddWithValue("@CCRCheckFlow", cCRCheckFlow);
            cmd.Parameters.AddWithValue("@ItemSeq", itemSeq);//s20230608
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public int GetListByCheckType1Count(int engMainSeq, int checkType, int cCRCheckFlow, int itemSeq)
        {
            string sql = @"
                SELECT
                    count(a.Seq) total
                FROM EngConstruction b
                inner join ConstCheckRec a on(a.EngConstructionSeq=b.Seq)
                where b.EngMainSeq=@EngMainSeq
                and a.CCRCheckType1=@CCRCheckType1
                and a.ItemSeq=@ItemSeq
                and (@CCRCheckFlow=-1 or @CCRCheckType1=2 or @CCRCheckType1=3 or a.CCRCheckFlow=@CCRCheckFlow)
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@CCRCheckType1", checkType);
            cmd.Parameters.AddWithValue("@CCRCheckFlow", cCRCheckFlow);
            cmd.Parameters.AddWithValue("@ItemSeq", itemSeq);//s20230608

            DataTable dt = db.GetDataTable(cmd);
            int cnt = Convert.ToInt32(dt.Rows[0]["total"].ToString());

            return cnt;
        }

        public List<string> GetRecOption(int engConstructionSeq, int checkType)
        {
            List<ConstCheckRecOptionVModel> list = GetListByCheckType<ConstCheckRecOptionVModel>(engConstructionSeq, checkType);
            return list.Select(row => row.Text).ToList();
        }
        public Dictionary<string, int> GetRecSeqMap(int engConstructionSeq, int checkType)
        {

            List<ConstCheckRecOptionVModel> list = GetListByCheckType<ConstCheckRecOptionVModel>(engConstructionSeq, checkType);
            return list.Select(row => new { Text = row.Text, Seq = row.Seq }).Distinct(row => row.Text).ToDictionary(row => row.Text, row => row.Seq);
        }
        //搜索 檢驗項目名稱
        public List<T> SearchCheckTypeByName<T>(int engMainSeq, string itemName)
        {
            string sql = @"select top 1 z.* from
                (
                    SELECT cast(1 as tinyint) CCRCheckType1, Seq ItemSeq, ItemName FROM ConstCheckList
                    where EngMainSeq=@EngMainSeq and ItemName like @ItemName
                    union ALL
                    SELECT cast(2 as tinyint) CCRCheckType1, Seq ItemSeq, ItemName FROM EquOperTestList
                    where EngMainSeq=@EngMainSeq and ItemName like @ItemName
                    union ALL
                    SELECT cast(3 as tinyint) CCRCheckType1, Seq ItemSeq, ItemName FROM OccuSafeHealthList
                    where EngMainSeq=@EngMainSeq and ItemName like @ItemName
                    union ALL
                    SELECT cast(4 as tinyint) CCRCheckType1, Seq ItemSeq, ItemName FROM EnvirConsList
                    where EngMainSeq=@EngMainSeq and ItemName like @ItemName
                ) z
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemName", "%"+ itemName + "%");
            return db.GetDataTableWithClass<T>(cmd);
        }
        //施工抽查 照片群組清單
        public List<T> GetConstCheckPhotoGroupOption<T>(int recSeq)
        {
            string sql = @"
                select
                    DISTINCT (c.CCManageItem1+c.CCManageItem2) [Text], b.ControllStSeq, c.Seq [Value], c.OrderNo
                from ConstCheckRec a
                inner join ConstCheckRecFile b on(b.ConstCheckRecSeq=a.Seq)
                inner join ConstCheckControlSt c on(c.ConstCheckListSeq=a.ItemSeq and c.Seq=b.ControllStSeq )
                where a.Seq=@Seq
                order by c.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", recSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //設備運轉 照片群組清單
        public List<T> GetEquOperPhotoGroupOption<T>(int recSeq)
        {
            string sql = @"
                select
                    DISTINCT (c.EPCheckItem1+c.EPCheckItem2) [Text], b.ControllStSeq, c.Seq [Value], c.OrderNo
                from ConstCheckRec a
                inner join ConstCheckRecFile b on(b.ConstCheckRecSeq=a.Seq)
                inner join EquOperControlSt c on(c.EquOperTestStSeq=a.ItemSeq and c.Seq=b.ControllStSeq )
                where a.Seq=@Seq
                order by c.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", recSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //職業安全衛生 照片群組清單
        public List<T> GetOccuSafeHealthGroupOption<T>(int recSeq)
        {
            string sql = @"
                select
                    DISTINCT (c.OSCheckItem1+c.OSCheckItem2) [Text], b.ControllStSeq, c.Seq [Value], c.OrderNo
                from ConstCheckRec a
                inner join ConstCheckRecFile b on(b.ConstCheckRecSeq=a.Seq)
                inner join OccuSafeHealthControlSt c on(c.OccuSafeHealthListSeq=a.ItemSeq and c.Seq=b.ControllStSeq )
                where a.Seq=@Seq
                order by c.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", recSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //環境保育 照片群組清單
        public List<T> GetEnvirConsGroupOption<T>(int recSeq)
        {
            string sql = @"
                select
                    DISTINCT (c.ECCCheckItem1+c.ECCCheckItem2) [Text], b.ControllStSeq, c.Seq [Value], c.OrderNo
                from ConstCheckRec a
                inner join ConstCheckRecFile b on(b.ConstCheckRecSeq=a.Seq)
                inner join EnvirConsControlSt c on(c.EnvirConsListSeq=a.ItemSeq and c.Seq=b.ControllStSeq )
                where a.Seq=@Seq
                order by c.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", recSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}