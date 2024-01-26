using EQC.Common;
using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class QualityProjectListService : BaseService
    {//品質計畫書範本, 已建立範本
        //監造計畫書範本, 已建立範本
        public const int docstate_Edit = -1;//編輯
        public const int docstate_SettingComplete = 0;//設置完成
        public const int docstate_PlanMaking = 1;//產製監造計畫書


        //工程年分清單
        public List<EngYearVModel> GetEngYearList()
        {
            string sql = @"
                SELECT DISTINCT
                    cast(a.EngYear as integer) EngYear
                FROM EngMain a
                inner join QualityProjectList z on(z.EngMainSeq=a.Seq)
                where 1=1"
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
                inner join QualityProjectList z on(z.EngMainSeq=a.Seq)            
                inner join Unit b on(b.Seq=a.ExecUnitSeq and b.parentSeq is null)
                where a.EngYear=@EngYear"
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
                inner join QualityProjectList z on(z.EngMainSeq=a.Seq)
                inner join Unit b on(b.Seq=a.ExecSubUnitSeq and @ParentSeq=b.parentSeq)
                where a.EngYear=@EngYear"
                + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @" order by b.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngYear", engYear);
            cmd.Parameters.AddWithValue("@ParentSeq", parentSeq);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<EngExecUnitsVModel>(cmd);
        }

        public List<EngMainVModel> getEng(int engMain)
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
                        d.DocState
                    FROM EngMain a
                    inner join Unit b on(b.Seq=a.ExecUnitSeq)
                    inner join QualityProjectList d on(
                        d.EngMainSeq=a.Seq
                        and d.Seq=(select max(Seq) from QualityProjectList where EngMainSeq=a.Seq)
                    )
                    left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)
                    
                    where ( (@Seq=-1) or (a.Seq=@Seq) )"
                    + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                    + @" order by EngNo DESC";
            SqlCommand cmd = db.GetCommand(sql); ;
            cmd.Parameters.AddWithValue("@Seq", engMain);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());

            return db.GetDataTableWithClass<EngMainVModel>(cmd);
        }

        public int GetEngCreatedListCount(string year, int unitSeq, int subUnitSeq, int engMain)
        {
            string sql = @"";
            if (subUnitSeq == -1)
            {
                sql = @"
                SELECT
                    count(a.Seq) total
                FROM EngMain a
                inner join Unit b on(b.Seq=a.ExecUnitSeq)
                inner join QualityProjectList d on(
                    d.EngMainSeq=a.Seq
                    and d.Seq=(select max(Seq) from QualityProjectList where EngMainSeq=a.Seq)
                )
                where ( (@Seq=-1) or (a.Seq=@Seq) )
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
                inner join Unit b on(b.Seq=a.ExecSubUnitSeq and a.ExecUnitSeq=@ExecUnitSeq)
                inner join QualityProjectList d on(
                    d.EngMainSeq=a.Seq
                    and d.Seq=(select max(Seq) from QualityProjectList where EngMainSeq=a.Seq)
                )
                where ( (@Seq=-1) or (a.Seq=@Seq) )
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
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());

            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetEngCreatedList<T>(string year, int unitSeq, int subUnitSeq, int engMain, int pageRecordCount, int pageIndex)
        {
            string sql = @"";
            if (subUnitSeq == -1)
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
                        d.DocState
                    FROM EngMain a
                    inner join Unit b on(b.Seq=a.ExecUnitSeq)
                    inner join QualityProjectList d on(
                        d.EngMainSeq=a.Seq
                        and d.Seq=(select max(Seq) from QualityProjectList where EngMainSeq=a.Seq)
                    )
                    left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)
                    
                    where ( (@Seq=-1) or (a.Seq=@Seq) )
                    and a.EngYear=@EngYear
                    and a.ExecUnitSeq=@ExecUnitSeq"
                    + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                    + @" order by EngNo DESC
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
                        d.DocState
                    FROM EngMain a
                    inner join Unit b on(b.Seq=a.ExecUnitSeq and a.ExecUnitSeq=@ExecUnitSeq)
                    inner join QualityProjectList d on(
                        d.EngMainSeq=a.Seq
                        and d.Seq=(select max(Seq) from QualityProjectList where EngMainSeq=a.Seq)
                    )
                    left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)
                    where ( (@Seq=-1) or (a.Seq=@Seq) )
                    and a.EngYear=@EngYear
                    and a.ExecSubUnitSeq=@ExecSubUnitSeq"
                    + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                    + @" order by EngNo DESC
                    OFFSET @pageIndex ROWS
				    FETCH FIRST @pageRecordCount ROWS ONLY";
            }
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngYear", year);
            cmd.Parameters.AddWithValue("@ExecUnitSeq", unitSeq);
            cmd.Parameters.AddWithValue("@ExecSubUnitSeq", subUnitSeq);
            cmd.Parameters.AddWithValue("@Seq", engMain);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<T>(cmd);
        }

        public int AddRevision(QualityProjectListModel m)
        {
            string sql = @"
                insert into QualityProjectList(
                    EngMainSeq,
                    Name,
                    DocState,
                    RevisionDate,
                    OriginFileName,
                    UniqueFileName,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq,
                    RevisionNo
                ) values (
                    @EngMainSeq,    
                    @Name,
                    -1,
                    GetDate(),
                    @OriginFileName,
                    @UniqueFileName,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq,
                    (select ISNULL(max(RevisionNo),0)+1 from QualityProjectList where EngMainSeq=@EngMainSeq)
                )";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", m.EngMainSeq);
            cmd.Parameters.AddWithValue("@Name", m.Name);
            cmd.Parameters.AddWithValue("@originFileName", m.OriginFileName);
            cmd.Parameters.AddWithValue("@uniqueFileName", m.UniqueFileName);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            return db.ExecuteNonQuery(cmd);
        }

        public List<T> ListByEngMainSeq<T>(int engMainSeq)
        {
            string sql = @"SELECT
                Seq,
                RevisionNo,
                RevisionDate,
                Name,
                DocState,
                ModifyTime
                FROM QualityProjectList
                where EngMainSeq=@EngMainSeq
                order by RevisionNo DESC";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> ListByEngNo<T>(string engNo)
        {
            string sql = @"SELECT
                a.Seq,
                a.RevisionNo,
                a.RevisionDate,
                a.Name,
                a.DocState,
                a.ModifyTime
                FROM QualityProjectList a
                inner join EngMain b on(b.EngNo=@EngNo and b.Seq=a.EngMainSeq)
                order by a.RevisionNo DESC
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngNo", engNo);
            return db.GetDataTableWithClass<T>(cmd);
        }

        public List<T> GetItemBySeq<T>(int seq)
        {
            string sql = @"SELECT
                Seq,
                RevisionNo,
                RevisionDate,
                Name,
                DocState,
                ModifyTime
                FROM QualityProjectList
                where Seq=@Seq
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        public List<T> GetLastItemByEngMain<T>(int engMainSeq)
        {
            string sql = @"SELECT TOP 1
                Seq,
                RevisionNo,
                RevisionDate,
                Name,
                DocState,
                ModifyTime
                FROM QualityProjectList
                where EngMainSeq=@EngMainSeq
                order by RevisionNo DESC
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        public int UpdateName(QualityProjectListModel m)
        {
            string sql = @"
                update QualityProjectList set
                    Name = @Name,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", m.Seq);
            cmd.Parameters.AddWithValue("@Name", m.Name);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            return db.ExecuteNonQuery(cmd);
        }

        public int UpdateDocState(int engMainSeq, int docState)
        {
            string sql = @"
                update QualityProjectList set
                    DocState = @DocState,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=(select max(Seq) from QualityProjectList where EngMainSeq=@EngMainSeq)
                and DocState<10";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@DocState", docState);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            return db.ExecuteNonQuery(cmd);
        }
        //解鎖
        public int UnlockDocState(int engMainSeq, int docState)
        {
            string sql = @"
                update QualityProjectList set
                    DocState = @DocState,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=(select max(Seq) from QualityProjectList where EngMainSeq=@EngMainSeq)
                and DocState<10";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@DocState", docState);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            return db.ExecuteNonQuery(cmd);
        }

        public List<QualityProjectListModel> GetItemFileInfoBySeq(int seq)
        {
            string sql = @"SELECT
                Seq,
                EngMainSeq,
                OriginFileName,
                UniqueFileName
                FROM QualityProjectList
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<QualityProjectListModel>(cmd);
        }

        /*public int UpdateUploadFile(int seq, string originFileName, string uniqueFileName)
        {
            string sql = @"
                update QualityProjectList set
                    OriginFileName = @originFileName,
                    UniqueFileName = @uniqueFileName,
                    RevisionDate = GETDATE(),
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@originFileName", originFileName);
            cmd.Parameters.AddWithValue("@uniqueFileName", uniqueFileName);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }*/

       //勾選與設置完成
       public bool SettingComplateDocState(int engMainSeq)
       {
           string sql = @"select a.DocState from QualityProjectList a where a.Seq=(select max(Seq) from QualityProjectList where EngMainSeq=@EngMainSeq)";
           SqlCommand cmd = db.GetCommand(sql);
           cmd.Parameters.Clear();
           cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
           DataTable dt = db.GetDataTable(cmd);
           if (dt.Rows.Count == 1)
           {
               if (Convert.ToInt32(dt.Rows[0]["DocState"].ToString()) >= 10) return false;
           }
           else
           {
               return false;
           }
    
           db.BeginTransaction();
           try
           {
               //刪除未勾選項目
               //5 材料設備清冊範本 EngMaterialDeviceListTp, 材料設備送審管制總表 EngMaterialDeviceSummary
               sql = @"
               delete EngMaterialDeviceSummary where EngMaterialDeviceListSeq in(
                   select Seq from EngMaterialDeviceList where EngMainSeq=@EngMainSeq and DataKeep=0
               );
               delete EngMaterialDeviceControlSt where EngMaterialDeviceListSeq in(
                   select Seq from EngMaterialDeviceList where EngMainSeq=@EngMainSeq and DataKeep=0
               );
               delete EngMaterialDeviceList where EngMainSeq=@EngMainSeq and DataKeep=0;";
    
               cmd = db.GetCommand(sql);
               cmd.Parameters.Clear();
               cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
               db.ExecuteNonQuery(cmd);
    
               //6 設備運轉測試清單範本 EquOperTestListTp
               sql = @"
               delete EquOperControlSt where EquOperTestStSeq in(
                   select Seq from EquOperTestList where EngMainSeq=@EngMainSeq and DataKeep=0
               );
               delete EquOperTestList where EngMainSeq=@EngMainSeq and DataKeep=0;";
    
               cmd = db.GetCommand(sql);
               cmd.Parameters.Clear();
               cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
               db.ExecuteNonQuery(cmd);
    
               // 701 施工抽查清單範本 ConstCheckListTp
               sql = @"
               delete ConstCheckControlSt where ConstCheckListSeq in(
                   select Seq from ConstCheckList where EngMainSeq=@EngMainSeq and DataKeep=0
               );
               delete ConstCheckList where EngMainSeq=@EngMainSeq and DataKeep=0;";
    
               cmd = db.GetCommand(sql);
               cmd.Parameters.Clear();
               cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
               db.ExecuteNonQuery(cmd);
    
               //702 環境保育清單範本 EnvirConsListTp
               sql = @"
               delete EnvirConsControlSt where EnvirConsListSeq in(
                   select Seq from EnvirConsList where EngMainSeq=@EngMainSeq and DataKeep=0
               );
               delete EnvirConsList where EngMainSeq=@EngMainSeq and DataKeep=0;";
    
               cmd = db.GetCommand(sql);
               cmd.Parameters.Clear();
               cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
               db.ExecuteNonQuery(cmd);
    
               //703 職業安全衛生清單範本 OccuSafeHealthListTp
               sql = @"
               delete OccuSafeHealthControlSt where OccuSafeHealthListSeq in(
                   select Seq from OccuSafeHealthList where EngMainSeq=@EngMainSeq and DataKeep=0
               );
               delete OccuSafeHealthList where EngMainSeq=@EngMainSeq and DataKeep=0;";
    
               cmd = db.GetCommand(sql);
               cmd.Parameters.Clear();
               cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
               db.ExecuteNonQuery(cmd);
    
               sql = @"
               update QualityProjectList set
                   DocState = @DocState,
                   ModifyTime = GETDATE(),
                   ModifyUserSeq = @ModifyUserSeq
               where Seq=(select max(Seq) from QualityProjectList where EngMainSeq=@EngMainSeq)
               and DocState<10";
               cmd = db.GetCommand(sql);
               cmd.Parameters.Clear();
               cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
               cmd.Parameters.AddWithValue("@DocState", QualityProjectListService.docstate_SettingComplete);
               cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
               db.ExecuteNonQuery(cmd);
    
               db.TransactionCommit();
               return true;
           }
           catch (Exception e)
           {
               db.TransactionRollback();
               log.Info("QualityProjectListService.SettingComplateDocState: " + e.Message);
               log.Info(sql);
               return false;
           }
       }

    }
}