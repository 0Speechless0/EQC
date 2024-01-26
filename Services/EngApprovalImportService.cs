using EQC.Common;
using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EngApprovalImportService : BaseService
    {//工程核定資料匯入 s20231006
        public bool DelItem(int seq)
        {
            string sql = @"
                delete from EngApprovalImport where Seq=@Seq";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                return true;
            }
            catch (Exception e)
            {
                log.Info("EngApprovalImportService.DelItem: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }
        public bool UpdateUnit(EngApprovalImportModel m)
        {
            Null2Empty(m);
            string sql = @"
                update EngApprovalImport set
                    ExecUnitSeq = @ExecUnitSeq,
                    ExecSubUnitSeq = @ExecSubUnitSeq,
                    OrganizerUserSeq = @OrganizerUserSeq,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ExecUnitSeq", this.NulltoDBNull(m.ExecUnitSeq));
                cmd.Parameters.AddWithValue("@ExecSubUnitSeq", this.NulltoDBNull(m.ExecSubUnitSeq));
                cmd.Parameters.AddWithValue("@OrganizerUserSeq", this.NulltoDBNull(m.OrganizerUserSeq));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                db.ExecuteNonQuery(cmd);

                return true;
            }
            catch (Exception e)
            {
                log.Info("EngApprovalImportService.UpdateUnit: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }
        public int GetListCount()
        {
            string sql = @"SELECT
                    count(a.Seq) total
                FROM EngApprovalImport a 
                ";

            SqlCommand cmd = db.GetCommand(sql);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetList<T>(int pageRecordCount, int pageIndex)
        {
            string sql = @"SELECT
                    a.Seq,
                    a.EngYear,
                    a.EngNo,
                    a.EngName,
                    a.TotalBudget,
                    a.SubContractingBudget,
                    a.CarbonDemandQuantity,
                    a.ApprovedCarbonQuantity,
                    (SELECT top 1 Seq FROM EngMain where EngNo=a.EngNo) engMainSeq, --20231105
                    (
                    	SELECT top 1 Unit.Name FROM EngMain
                        inner join Unit on(Unit.Seq=EngMain.ExecUnitSeq)
                        where EngMain.EngNo=a.EngNo
                    ) engExecUnit, --20231106
                    (SELECT top 1 Seq FROM PCCESSMain where contractNo=a.EngNo) pccessMainSeq --20231105
                FROM EngApprovalImport a
                order by EngYear desc, EngNo, EngName
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public int GetEngListCount(int unit) //s20231102
        {
            UserInfo userInfo = Utils.getUserInfo();
            if(userInfo.RoleSeq !=1 && userInfo.RoleSeq != 3)//系統管理員,分屬管理員
            {
                return 0;
            }
            string sql = @"
                SELECT count(a.Seq) total FROM EngApprovalImport a
                where (@RoleSeq=1 or (@RoleSeq=3 and a.ExecUnitSeq=@ExecUnitSeq))
                and (a.ExecUnitSeq=@unitSeq or (@RoleSeq=1 and a.ExecUnitSeq is null))
            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@RoleSeq", userInfo.RoleSeq); //s20231102
            cmd.Parameters.AddWithValue("@ExecUnitSeq", userInfo.UnitSeq1); //s20231102
            cmd.Parameters.AddWithValue("@unitSeq", unit);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetEngList<T>(int pageRecordCount, int pageIndex, int unit)
        {
            UserInfo userInfo = Utils.getUserInfo();
            string sql = @"SELECT
                    a.Seq,
                    a.EngYear,
                    a.EngNo,
                    a.EngName,
                    a.TotalBudget,
                    a.SubContractingBudget,
                    a.CarbonDemandQuantity,
                    a.ApprovedCarbonQuantity,
                    a.ExecUnitSeq,
                    a.ExecSubUnitSeq,
                    a.OrganizerUserSeq,
                    c.Name ExecUnitName,
                    c1.Name ExecSubUnitName,
                    d.DisplayName ExecUserName
                FROM EngApprovalImport a
                left outer join Unit c on(c.Seq=a.ExecUnitSeq)
                left outer join Unit c1 on(c1.Seq=a.ExecSubUnitSeq)
                left outer join UserMain d on(d.Seq=a.OrganizerUserSeq)
                where (@RoleSeq=1 or (@RoleSeq=3 and a.ExecUnitSeq=@ExecUnitSeq))
                and (a.ExecUnitSeq=@unitSeq or (@RoleSeq=1 and a.ExecUnitSeq is null))
                order by EngYear desc, EngNo, EngName
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@RoleSeq", userInfo.RoleSeq); //s20231102
            cmd.Parameters.AddWithValue("@ExecUnitSeq", userInfo.UnitSeq1); //s20231102
            cmd.Parameters.AddWithValue("@unitSeq", unit);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);

            return db.GetDataTableWithClass<T>(cmd);
        }

        public int Update(EngApprovalImportModel m)
        {
            Null2Empty(m);
            string sql = @"
                update EngApprovalImport set
                    EngYear = @EngYear,
                    TotalBudget = @TotalBudget,
                    SubContractingBudget = @SubContractingBudget,
                    CarbonDemandQuantity = @CarbonDemandQuantity,
                    ApprovedCarbonQuantity = @ApprovedCarbonQuantity,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
            try { 
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngYear", this.NulltoDBNull(m.EngYear));
                //cmd.Parameters.AddWithValue("@EngNo", m.EngNo);
                //cmd.Parameters.AddWithValue("@EngName", m.EngName);
                cmd.Parameters.AddWithValue("@TotalBudget", this.NulltoDBNull(m.TotalBudget));
                cmd.Parameters.AddWithValue("@SubContractingBudget", this.NulltoDBNull(m.SubContractingBudget));
                cmd.Parameters.AddWithValue("@CarbonDemandQuantity", this.NulltoDBNull(m.CarbonDemandQuantity));
                cmd.Parameters.AddWithValue("@ApprovedCarbonQuantity", this.NulltoDBNull(m.ApprovedCarbonQuantity));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@Seq", m.Seq);

                return db.ExecuteNonQuery(cmd);
            } catch(Exception e) {
                log.Info("EngApprovalImportService.Update: " + e.Message);
                //log.Info(sql);
                return -1;
            }
        }
        public int Add(EngApprovalImportModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into EngApprovalImport (
                    EngYear,
                    EngNo,
                    EngName,
                    TotalBudget,
                    SubContractingBudget,
                    CarbonDemandQuantity,
                    ApprovedCarbonQuantity,
                    CreateUserSeq,
                    ModifyUserSeq
                ) values (
                    @EngYear,
                    @EngNo,
                    @EngName,
                    @TotalBudget,
                    @SubContractingBudget,
                    @CarbonDemandQuantity,
                    @ApprovedCarbonQuantity,
                    @ModifyUserSeq,
                    @ModifyUserSeq
                )";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngYear", this.NulltoDBNull(m.EngYear));
                cmd.Parameters.AddWithValue("@EngNo", m.EngNo);
                cmd.Parameters.AddWithValue("@EngName", m.EngName);
                cmd.Parameters.AddWithValue("@TotalBudget", this.NulltoDBNull(m.TotalBudget));
                cmd.Parameters.AddWithValue("@SubContractingBudget", this.NulltoDBNull(m.SubContractingBudget));
                cmd.Parameters.AddWithValue("@CarbonDemandQuantity", this.NulltoDBNull(m.CarbonDemandQuantity));
                cmd.Parameters.AddWithValue("@ApprovedCarbonQuantity", this.NulltoDBNull(m.ApprovedCarbonQuantity));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                int result = db.ExecuteNonQuery(cmd);
                if (result == 0) return 0;

                cmd.Parameters.Clear();

                string sql1 = @"SELECT IDENT_CURRENT('ExpertCommittee') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);
                return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
            } catch(Exception e) {
                log.Info("EngApprovalImportService.Add: " + e.Message);
                //log.Info(sql);
                return 0;
            }
        }

        //批次處裡
        public void ImportData(List<EngApprovalImportModel> items, ref int iCnt, ref int uCnt, ref string errCnt)
        {
            SqlCommand cmd;
            string sql;
            int inx = 1;
            foreach(EngApprovalImportModel m in items)
            {
                if (String.IsNullOrEmpty(m.EngNo) || String.IsNullOrEmpty(m.EngName))
                {
                    errCnt += inx.ToString() + ",";
                } else
                {
                    sql = @"SELECT Seq FROM EngApprovalImport where EngNo=@EngNo and EngName=@EngName";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.AddWithValue("@EngNo", m.EngNo);
                    cmd.Parameters.AddWithValue("@EngName", m.EngName);
                    DataTable dt = db.GetDataTable(cmd);
                    if (dt.Rows.Count == 1) {
                        m.Seq = Convert.ToInt32(dt.Rows[0]["Seq"].ToString());
                        if (Update(m) == -1)
                            errCnt += inx.ToString() + ",";
                        else
                            uCnt++;
                    } else
                    {
                        if (Add(m) == 0)
                            errCnt += inx.ToString() + ",";
                        else
                            iCnt++;
                    }
                }
                inx++;
            }
        }
    }
}