using EQC.Common;
using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace EQC.Services
{
    public class ConstCheckRecApproveService : BaseService
    {//抽驗紀錄填報-審核, 已建立範本

        //工程年分清單
        public List<EngYearVModel> GetEngYearList()
        {
            string sql = @"
                SELECT DISTINCT
                    cast(a.EngYear as integer) EngYear
                FROM EngMain a
                inner join SupervisionProjectList z on(z.EngMainSeq=a.Seq)
                where exists (
                    select EngMainSeq from EngConstruction s
                    inner join ConstCheckRec s1 on(s1.EngConstructionSeq=s.Seq and s1.FormConfirm>0)
                    where s.EngMainSeq=a.Seq
                ) "
                + Utils.getAuthoritySql("a.") //a.CreateUserSeq=@CreateUserSeq
                + @" order by EngYear DESC";
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
                where a.EngYear=@EngYear
                and exists (
                    select EngMainSeq from EngConstruction s
                    inner join ConstCheckRec s1 on(s1.EngConstructionSeq=s.Seq and s1.FormConfirm>0)
                    where s.EngMainSeq=a.Seq
                ) "
                + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @" order by b.OrderNo";
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
                where a.EngYear=@EngYear
                and exists (
                    select EngMainSeq from EngConstruction s
                    inner join ConstCheckRec s1 on(s1.EngConstructionSeq=s.Seq and s1.FormConfirm>0)
                    where s.EngMainSeq=a.Seq
                ) "
                + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @" order by b.OrderNo";
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
                SELECT DISTINCT 
                    a.Seq,
                    a.ItemName,
                    a.OrderNo
                FROM EngConstruction a
                inner join ConstCheckRec s1 on(s1.EngConstructionSeq=a.Seq and s1.FormConfirm>0)
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
        //工程名稱清單
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
                    inner join SupervisionProjectList d on(
                        d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                    )
                    where a.EngYear=@EngYear
                    and exists (
                        select EngMainSeq from EngConstruction s
                        inner join ConstCheckRec s1 on(s1.EngConstructionSeq=s.Seq and s1.FormConfirm>0)
                        where s.EngMainSeq=a.Seq
                    )
                    and a.ExecUnitSeq=@ExecUnitSeq"
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
                    inner join SupervisionProjectList d on(
                        d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                    )
                    where a.EngYear=@EngYear
                    and exists (
                        select EngMainSeq from EngConstruction s
                        inner join ConstCheckRec s1 on(s1.EngConstructionSeq=s.Seq and s1.FormConfirm>0)
                        where s.EngMainSeq=a.Seq
                    )
                    and a.ExecSubUnitSeq=@ExecSubUnitSeq"
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

        //分項工程清單
        public int GetEngCreatedListCount(int engMain, int subEngMain)
        {
            string sql = @"
                SELECT Distinct
                    a.Seq 
                FROM EngMain a
                inner join EngConstruction f on(f.EngMainSeq=a.Seq and( (-1=@subEngMain)or(f.Seq=@subEngMain)) )
                inner join Unit b on(b.Seq=a.ExecUnitSeq)
                inner join ConstCheckRec s1 on(s1.EngConstructionSeq=f.Seq and s1.FormConfirm>0)
                inner join SupervisionProjectList d on(
                    d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                )
                left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)
                where a.Seq=@Seq
                and exists (
                    select EngMainSeq from EngConstruction s
                    inner join ConstCheckRec s1 on(s1.EngConstructionSeq=s.Seq and s1.FormConfirm>0)
                    where s.EngMainSeq=a.Seq
                ) "
                + Utils.getAuthoritySql("a."); //and a.CreateUserSeq=@CreateUserSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", engMain);
            cmd.Parameters.AddWithValue("@subEngMain", subEngMain);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());

            DataTable dt = db.GetDataTable(cmd);

            return dt.Rows.Cast<DataRow>().ToList().Count;
        }
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
                and a.ExecUnitSeq=@ExecUnitSeq
                and exists (
                    select EngMainSeq from EngConstruction s
                    inner join ConstCheckRec s1 on(s1.EngConstructionSeq=s.Seq and s1.FormConfirm>0)
                    where s.EngMainSeq=a.Seq
                ) "
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
                and a.ExecSubUnitSeq=@ExecSubUnitSeq
                and exists (
                    select EngMainSeq from EngConstruction s
                    inner join ConstCheckRec s1 on(s1.EngConstructionSeq=s.Seq and s1.FormConfirm>0)
                    where s.EngMainSeq=a.Seq
                ) "
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
        public List<T> GetEngCreatedList<T>(int engMain, int subEngMain, int pageRecordCount, int pageIndex)
        {
            string sql = @"
                      SELECT DISTINCT
                          a.Seq,
                          a.EngNo,
                          a.EngName,
                          b.Name ExecUnit, 
                          c.Name ExecSubUnit,
                          a.SupervisorUnitName,
                          a.SupervisorDirector,
                          a.ApproveDate,
                          d.DocState,
                          f.ItemName subEngName,
                          f.Seq subEngNameSeq,
                          cast((
                              select COUNT(z2.Seq)  from ConstCheckRec z1
                              inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq=z1.Seq and z2.CCRCheckResult=2)
                              where z1.EngConstructionSeq=f.Seq
        )as int) missingCount,
                          COALESCE((
                              select max(y1.FormConfirm) from ConstCheckRec y1
                              where y1.EngConstructionSeq=f.Seq and y1.FormConfirm=1
                          ),0) hasUnderReview,
                          COALESCE((
                              select max(x1.FormConfirm) from ConstCheckRec x1
                              where x1.EngConstructionSeq=f.Seq and x1.FormConfirm=2
                          ),0) hasApproved
                      FROM EngMain a
                      inner join EngConstruction f on(f.EngMainSeq=a.Seq and( (-1=@subEngMain)or(f.Seq=@subEngMain)) )
                      inner join ConstCheckRec s1 on(s1.EngConstructionSeq=f.Seq and s1.FormConfirm>0)
                      inner join Unit b on(b.Seq=a.ExecUnitSeq)
                      inner join SupervisionProjectList d on(
                          d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                      )
                      left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)

                      where a.Seq=@Seq 
   
                      "
                + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @" order by EngNo DESC
                      OFFSET @pageIndex ROWS
          FETCH FIRST @pageRecordCount ROWS ONLY";
        
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", engMain);
            cmd.Parameters.AddWithValue("@subEngMain", subEngMain);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<T>(cmd);
        }
        //  public List<T> GetEngCreatedList<T>(string year, int unitSeq, int subUnitSeq, int engMain, int subEngMain, int pageRecordCount, int pageIndex)
        //  {
        //      string sql = @"";
        //      if (subUnitSeq == -1)
        //      {
        //          sql = @"
        //              SELECT DISTINCT
        //                  a.Seq,
        //                  a.EngNo,
        //                  a.EngName,
        //                  b.Name ExecUnit, 
        //                  c.Name ExecSubUnit,
        //                  a.SupervisorUnitName,
        //                  a.SupervisorDirector,
        //                  a.ApproveDate,
        //                  d.DocState,
        //                  f.ItemName subEngName,
        //                  f.Seq subEngNameSeq,
        //                  cast((
        //                      select COUNT(z2.Seq)  from ConstCheckRec z1
        //                      inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq=z1.Seq and z2.CCRCheckResult=2)
        //                      where z1.EngConstructionSeq=f.Seq
        //)as int) missingCount,
        //                  COALESCE((
        //                      select max(y1.FormConfirm) from ConstCheckRec y1
        //                      where y1.EngConstructionSeq=f.Seq and y1.FormConfirm=1
        //                  ),0) hasUnderReview,
        //                  COALESCE((
        //                      select max(x1.FormConfirm) from ConstCheckRec x1
        //                      where x1.EngConstructionSeq=f.Seq and x1.FormConfirm=2
        //                  ),0) hasApproved
        //              FROM EngMain a
        //              inner join EngConstruction f on(f.EngMainSeq=a.Seq and( (-1=@subEngMain)or(f.Seq=@subEngMain)) )
        //              inner join ConstCheckRec s1 on(s1.EngConstructionSeq=f.Seq and s1.FormConfirm>0)
        //              inner join Unit b on(b.Seq=a.ExecUnitSeq)
        //              inner join SupervisionProjectList d on(
        //                  d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
        //              )
        //              left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)

        //              where a.Seq=@Seq
        //              and a.EngYear=@EngYear
        //              and a.ExecUnitSeq=@ExecUnitSeq
        //              "
        //              + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
        //              + @" order by EngNo DESC
        //              OFFSET @pageIndex ROWS
        //  FETCH FIRST @pageRecordCount ROWS ONLY";
        //      }
        //      else
        //      {
        //          sql = @"
        //              SELECT DISTINCT
        //                  a.Seq,
        //                  a.EngNo,
        //                  a.EngName,
        //                  b.Name ExecUnit, 
        //                  c.Name ExecSubUnit,
        //                  a.SupervisorUnitName,
        //                  a.SupervisorDirector,
        //                  a.ApproveDate,
        //                  d.DocState,
        //                  f.ItemName subEngName,
        //                  f.Seq subEngNameSeq,
        //                  cast((
        //                      select COUNT(z2.Seq)  from ConstCheckRec z1
        //                      inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq=z1.Seq and z2.CCRCheckResult=2)
        //                      where z1.EngConstructionSeq=f.Seq
        //)as int) missingCount,
        //                  COALESCE((
        //                      select max(y1.FormConfirm) from ConstCheckRec y1
        //                      where y1.EngConstructionSeq=f.Seq and y1.FormConfirm=1
        //                  ),0) hasUnderReview,
        //                  COALESCE((
        //                      select max(x1.FormConfirm) from ConstCheckRec x1
        //                      where x1.EngConstructionSeq=f.Seq and x1.FormConfirm=2
        //                  ),0) hasApproved
        //              FROM EngMain a
        //              inner join EngConstruction f on(f.EngMainSeq=a.Seq and( (-1=@subEngMain)or(f.Seq=@subEngMain)) )
        //              inner join ConstCheckRec s1 on(s1.EngConstructionSeq=f.Seq and s1.FormConfirm>0)
        //              inner join Unit b on(b.Seq=a.ExecUnitSeq and a.ExecUnitSeq=@ExecUnitSeq)
        //              inner join SupervisionProjectList d on(
        //                  d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
        //              )
        //              left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)
        //              where a.Seq=@Seq
        //              and a.EngYear=@EngYear
        //              and a.ExecSubUnitSeq=@ExecSubUnitSeq"
        //              + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
        //              + @" order by EngNo DESC
        //              OFFSET @pageIndex ROWS
        //  FETCH FIRST @pageRecordCount ROWS ONLY";
        //      }
        //      SqlCommand cmd = db.GetCommand(sql);
        //      cmd.Parameters.AddWithValue("@EngYear", year);
        //      cmd.Parameters.AddWithValue("@ExecUnitSeq", unitSeq);
        //      cmd.Parameters.AddWithValue("@ExecSubUnitSeq", subUnitSeq);
        //      cmd.Parameters.AddWithValue("@Seq", engMain);
        //      cmd.Parameters.AddWithValue("@subEngMain", subEngMain);
        //      cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
        //      cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
        //      cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
        //      return db.GetDataTableWithClass<T>(cmd);
        //  }


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
                    c.Name organizerUnitName,
                    c1.Name execUnitName,
                    c2.Name execSubUnitName,
                    d.DocState,
                    e.ItemName subEngName,
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

        //分項工程之檢驗單審核 同意
        public int EngConstructionFormApproved(int engConstructionSeq)
        {
            string sql = @"update ConstCheckRec set
                        FormConfirm=2,
                        SupervisorDirectorSeq=@ModifyUserSeq,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where EngConstructionSeq=@EngConstructionSeq
                    and FormConfirm=1";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngConstructionSeq", engConstructionSeq);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
            return db.ExecuteNonQuery(cmd);
        }
        //分項工程之檢驗單審核 不同意
        public int EngConstructionFormDisagree(int engConstructionSeq)
        {
            string sql = @"update ConstCheckRec set
                        FormConfirm=0,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where EngConstructionSeq=@EngConstructionSeq
                    and FormConfirm=1";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngConstructionSeq", engConstructionSeq);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
            return db.ExecuteNonQuery(cmd);
        }

        //已同意檢驗單清單
        public List<T> GetList<T>(int engConstructionSeq)
        {
            string sql = @"SELECT
                    a.Seq,
                    a.ItemSeq,
                    a.CCRCheckType1,
                    a.CCRCheckFlow,
                    a.CCRCheckDate,
                    a.CCRPosLati,
                    a.CCRPosLong,
                    a.CCRPosDesc,
                    a.SupervisorUserSeq,
                    a.SupervisorDirectorSeq
                FROM ConstCheckRec a
                where a.EngConstructionSeq=@EngConstructionSeq
                and a.FormConfirm=2
                order by a.CCRCheckType1, a.CCRCheckDate
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngConstructionSeq", engConstructionSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}