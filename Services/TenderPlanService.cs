using EQC.Common;
using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class TenderPlanService : BaseService
    {
        private UnitService unitService = new UnitService();

        //由核定工程來新增工程 s20231006
        public int CreateApprovalEng(int engApprovalImportSeq, EngMainEditVModel engMain, ref string errMsg)
        {
            string sql = "", sql1 = "";
            SqlCommand cmd;
            int newSeq = 0;
            DataTable dt;
            int userSeq = getUserSeq();

            db.BeginTransaction();
            try
            {
                sql = @"
				    INSERT INTO PCCESSMain (
                        --ProcuringEntity,
                        --ProcuringEntityId,
                        ContractTitle,
                        contractNo,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    )values(
                        --@ProcuringEntity,
                        --@ProcuringEntityId,
                        @ContractTitle,
                        @contractNo,
                        GETDATE(),
                        @CreateUserSeq,
                        GETDATE(),
                        @ModifyUserSeq
                    )";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                //cmd.Parameters.AddWithValue("@ProcuringEntity", pcessMain.ProcuringEntity);
                //cmd.Parameters.AddWithValue("@ProcuringEntityId", pcessMain.ProcuringEntityId);
                cmd.Parameters.AddWithValue("@ContractTitle", engMain.EngName);
                cmd.Parameters.AddWithValue("@contractNo", engMain.EngNo);
                cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                /*cmd.Parameters.Clear();
                sql1 = @"
					SELECT IDENT_CURRENT('PCCESSMain') AS NewSeq";
                cmd = db.GetCommand(sql1);
                dt = db.GetDataTable(cmd);
                newSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());*/

                sql = @"
				    INSERT INTO EngMain (
                        EngYear,
                        EngNo,
                        EngName,
                        OrganizerUnitSeq,
                        OrganizerUserSeq,
                        ExecUnitSeq,
                        ExecSubUnitSeq,
                        TotalBudget,
                        SubContractingBudget,
                        EngTownSeq,
                        CarbonDemandQuantity,
                        ApprovedCarbonQuantity,
                        AwardDate,
                        AwardAmount,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    )values(
                        @EngYear,
                        @EngNo,
                        @EngName,
                        @OrganizerUnitSeq,
                        @OrganizerUserSeq,
                        @ExecUnitSeq,
                        @ExecSubUnitSeq,
                        @TotalBudget,
                        @SubContractingBudget,
                        @EngTownSeq,
                        @CarbonDemandQuantity,
                        @ApprovedCarbonQuantity,
                        @AwardDate,
                        @AwardAmount,
                        GETDATE(),
                        @ModifyUserSeq,
                        GETDATE(),
                        @ModifyUserSeq
                    )";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngYear", engMain.EngYear);
                cmd.Parameters.AddWithValue("@EngNo", engMain.EngNo);
                cmd.Parameters.AddWithValue("@EngName", engMain.EngName);
                cmd.Parameters.AddWithValue("@OrganizerUnitSeq", engMain.OrganizerUnitSeq);
                cmd.Parameters.AddWithValue("@OrganizerUserSeq", engMain.OrganizerUserSeq);
                cmd.Parameters.AddWithValue("@ExecUnitSeq", engMain.ExecUnitSeq);
                cmd.Parameters.AddWithValue("@ExecSubUnitSeq", engMain.ExecSubUnitSeq);
                cmd.Parameters.AddWithValue("@TotalBudget", engMain.TotalBudget);
                cmd.Parameters.AddWithValue("@SubContractingBudget", engMain.SubContractingBudget);
                cmd.Parameters.AddWithValue("@EngTownSeq", this.NulltoDBNull(engMain.EngTownSeq));//s20231106
                cmd.Parameters.AddWithValue("@CarbonDemandQuantity", this.NulltoDBNull(engMain.CarbonDemandQuantity));
                cmd.Parameters.AddWithValue("@ApprovedCarbonQuantity", this.NulltoDBNull(engMain.ApprovedCarbonQuantity));
                cmd.Parameters.AddWithValue("@AwardAmount", this.NulltoDBNull(engMain.BidAmount));
                cmd.Parameters.AddWithValue("@AwardDate", this.NulltoDBNull(engMain.AwardDate));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                sql1 = @"SELECT IDENT_CURRENT('EngMain') AS NewSeq";
                cmd = db.GetCommand(sql1);
                dt = db.GetDataTable(cmd);
                newSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                /*sql = @"
				    Update EngApprovalImport set
                        EngMainSeq = @EngMainSeq,
                        ModifyTime = GETDATE(),
                        ModifyUserSeq = @ModifyUserSeq
                    where Seq=@Seq
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", newSeq);
                cmd.Parameters.AddWithValue("@Seq", engApprovalImportSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);*/
                //s20231105 改為刪除
                sql = @"delete from EngApprovalImport where Seq=@Seq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", engApprovalImportSeq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return newSeq;
            }
            catch (Exception ex)
            {
                db.TransactionRollback();
                errMsg = ex.Message;
                log.Info("TenderPlanService.CreateApprovalEng: " + ex.Message);
                //log.Info(sql);
                return -1;
            }
        }
        public int checkAccount(int itemType, string taxId, string unitName, string userName, string email, string engNo)
        {
            Int16 unitSeq = -1;
            int accountSeq = -1;
            Int16 unitParentSeq = 0;
            byte roleSeq = 0;
            string uType = "*";
            if (itemType==UnitService.type_BuildContractor)
            {
                unitParentSeq = ConfigManager.BuildContractor_UnitParentSeq;
                roleSeq = ConfigManager.BuildContractor_RoleSeq;
                uType = "C";//s20220812
            } else if (itemType == UnitService.type_SupervisorUnit)
            {
                unitParentSeq = ConfigManager.SupervisorUnit_UnitParentSeq;
                roleSeq = ConfigManager.SupervisorUnit_RoleSeq;
                uType = "S";//s20220812
            }
            else if (itemType == UnitService.type_DesignUnit)
            {
                unitParentSeq = ConfigManager.DesignUnit_UnitParentSeq;
                roleSeq = ConfigManager.OutsourceDesign_RoleSeq;
                uType = "D";//s20220812
            }

            if (unitParentSeq == 0 || roleSeq == 0) return -1;//系統資料錯誤

            string sql;
            SqlCommand cmd;
            DataTable dt;
            
            sql = @"SELECT Seq FROM Unit where Code=@taxId and ParentSeq=@ParentSeq";
            cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@taxId", taxId);
            cmd.Parameters.AddWithValue("@ParentSeq", unitParentSeq);//s20220812 增加讓同一公司可分別建立於 設計,監造,施工清單下

            dt = db.GetDataTable(cmd);
            if (dt.Rows.Count > 0) unitSeq = Convert.ToInt16(dt.Rows[0]["Seq"].ToString());

            sql = @"SELECT Seq FROM UserMain where UserNo=@taxId";
            cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@taxId", uType + engNo); //s20230316 cmd.Parameters.AddWithValue("@taxId", uType+taxId);
            dt = db.GetDataTable(cmd);
            if (dt.Rows.Count > 0) accountSeq = Convert.ToInt16(dt.Rows[0]["Seq"].ToString());

            if(unitSeq == -1 || accountSeq == -1)
            {

                db.BeginTransaction();
                try
                {
                    if (unitSeq == -1)
                    {
                        //新增單位
                        sql = @"
				        INSERT INTO Unit (
					        ParentSeq
					        ,Code
					        ,[Name]
					        ,CreateTime
					        ,CreateUserSeq
					        ,ModifyTime
					        ,ModifyUserSeq
					        )
				        VALUES (
					        @ParentSeq
					        ,@Code
					        ,@Name
					        ,GETDATE()
					        ,@ModifyUser
					        ,GETDATE()
					        ,@ModifyUser
					        )";
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.AddWithValue("@ParentSeq", unitParentSeq);
                        cmd.Parameters.AddWithValue("@Code", taxId);
                        cmd.Parameters.AddWithValue("@Name", unitName);
                        cmd.Parameters.AddWithValue("@ModifyUser", getUserSeq());
                        db.ExecuteNonQuery(cmd);

                        sql = @"SELECT IDENT_CURRENT('Unit') AS NewSeq";
                        cmd = db.GetCommand(sql);
                        dt = db.GetDataTable(cmd);
                        unitSeq = Convert.ToInt16(dt.Rows[0]["NewSeq"].ToString());
                    }
                    if(accountSeq == -1)
                    {
                        //新增人員
                        sql = @"
                        INSERT INTO [UserMain]
                               ([UserNo]
                               ,[PassWord]
                               ,[DisplayName]
                               ,[Email]
                               ,[IsEnabled]
                               ,[CreateTime]
                               ,[CreateUserSeq]
                               ,[ModifyTime]
                               ,[ModifyUserSeq])
                         VALUES
                               (@UserNo
                               ,@PassWord
                               ,@DisplayName
                               ,@Email
                               ,1
                               ,GETDATE()
                               ,@ModifyUserSeq
                               ,GETDATE()
                               ,@ModifyUserSeq)";
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.AddWithValue("@UserNo", uType+ engNo);
                        cmd.Parameters.AddWithValue("@PassWord", "!"+ taxId+"!");
                        cmd.Parameters.AddWithValue("@DisplayName", userName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                        db.ExecuteNonQuery(cmd);

                        sql = @"SELECT IDENT_CURRENT('UserMain') AS NewSeq";
                        cmd = db.GetCommand(sql);
                        dt = db.GetDataTable(cmd);
                        accountSeq = Convert.ToInt16(dt.Rows[0]["NewSeq"].ToString());

                        //使用者、單位、職務關聯
                        sql = @"
                        INSERT INTO [UserUnitPosition]
                               ([UnitSeq]
                               ,[UserMainSeq]
                               ,[IsEnabled]
                               ,[CreateTime]
                               ,[CreateUserSeq]
                               ,[ModifyTime]
                               ,[ModifyUserSeq])
                         VALUES
                               (@UnitSeq
                               ,@UserMainSeq
                               ,1
                               ,GETDATE()
                               ,@ModifyUserSeq
                               ,GETDATE()
                               ,@ModifyUserSeq)";
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.AddWithValue("@UnitSeq", unitSeq);
                        cmd.Parameters.AddWithValue("@UserMainSeq", accountSeq);
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                        db.ExecuteNonQuery(cmd);

                        sql = @"SELECT IDENT_CURRENT('UserUnitPosition') AS NewSeq";
                        cmd = db.GetCommand(sql);
                        dt = db.GetDataTable(cmd);
                        int userUnitPositionSeq = Convert.ToInt16(dt.Rows[0]["NewSeq"].ToString());

                        sql = @"
                        INSERT INTO [UserRole] (
                               UserUnitPositionSeq,
                               RoleSeq
                        )VALUES(
                               @UserUnitPositionSeq,
                               @RoleSeq
                        )";
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.AddWithValue("@UserUnitPositionSeq", userUnitPositionSeq);
                        cmd.Parameters.AddWithValue("@RoleSeq", roleSeq);
                        db.ExecuteNonQuery(cmd);
                    }

                    db.TransactionCommit();
                } 
                catch (Exception ex)
                {
                    db.TransactionRollback();
                    log.Info("TenderPlanService.checkAccount");
                    log.Info(ex.Message);
                    log.Info(sql);
                    return -999;
                }
            }
            return 0;
        }

        //工程年分清單
        public List<EngYearVModel> GetEngYearList()
        {
            int userSeq = new SessionManager().GetUser().Seq;
            string sql;
            if (UserRoleCheckService.checkSupervisor(userSeq) )
            {
                sql = @" SELECT DISTINCT
                    cast(a.EngYear as integer) EngYear
                FROM EngMain a
                inner join EngSupervisor es
                on es.EngMainSeq = a.Seq

                where es.UserMainSeq =" + userSeq+
                @" 
                order by EngYear DESC";
            }
            else
            {
                sql = @"
                SELECT DISTINCT
                    cast(a.EngYear as integer) EngYear
                FROM EngMain a

                where 1=1 "//CreateUserSeq=@CreateUserSeq"
                + Utils.getAuthoritySql("a.")
                + @" 
                order by EngYear DESC";
            }
            SqlCommand cmd = db.GetCommand(sql);
            //cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<EngYearVModel>(cmd);
        }
        //工程執行機關清單
        public List<EngExecUnitsVModel> GetEngExecUnitList(string engYear)
        {
            int userSeq = new SessionManager().GetUser().Seq;
            string sql;
            int year = Int32.Parse(engYear);
            if (UserRoleCheckService.checkSupervisor(userSeq))
            {
                sql = @"                
                SELECT DISTINCT
                    b.OrderNo,
                    a.ExecUnitSeq UnitSeq,
                    b.Name UnitName
                FROM EngMain a
                inner join Unit b on(b.Seq=a.ExecUnitSeq and b.parentSeq is null)
                inner join EngSupervisor es
                on es.EngMainSeq = a.Seq

                where 1 = 1
                " + ((year == -1) ? "" : " and a.EngYear=@EngYear ") + @"
                and es.UserMainSeq =" + userSeq+
                " order by b.OrderNo";
            }
            else
            {
                sql = @"
                SELECT DISTINCT
                    b.OrderNo,
                    a.ExecUnitSeq UnitSeq,
                    b.Name UnitName
                FROM EngMain a
                inner join Unit b on(b.Seq=a.ExecUnitSeq and b.parentSeq is null)

                where 1=1
                " + ((year == -1) ? "" : " and a.EngYear=@EngYear ") + @"
                "
                + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @" order by b.OrderNo";
            }
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngYear", engYear);
            //cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<EngExecUnitsVModel>(cmd);
        }

        //工程年分清單 依決標日期 s20230719
        public List<EngYearVModel> GetEngYearListByAwardDate()
        {
            int userSeq = new SessionManager().GetUser().Seq;
            string sql = @"
                SELECT DISTINCT YEAR(a.AwardDate)-1911 EngYear
                FROM EngMain a

                where a.AwardDate is not null "//CreateUserSeq=@CreateUserSeq"
                + Utils.getAuthoritySql("a.")
                + @" 
                order by EngYear DESC";
            SqlCommand cmd = db.GetCommand(sql);
            //cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<EngYearVModel>(cmd);
        }
        //工程執行機關清單 依決標日期 s20230719
        public List<EngExecUnitsVModel> GetEngExecUnitListAwardDate(string engYear)
        {
            //int userSeq = new SessionManager().GetUser().Seq;
            int year = Int32.Parse(engYear);
            string sql = @"
                SELECT DISTINCT
                    b.OrderNo,
                    a.ExecUnitSeq UnitSeq,
                    b.Name UnitName
                FROM EngMain a
                inner join Unit b on(b.Seq=a.ExecUnitSeq and b.parentSeq is null)

                where a.AwardDate is not null
                " + ((year == -1) ? "" : " and (YEAR(a.AwardDate)-1911)=@EngYear ") + @"
                "
                + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @" order by b.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngYear", engYear);
            //cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<EngExecUnitsVModel>(cmd);
        }

        //工程執行單位清單
        public List<EngExecUnitsVModel> GetEngExecSubUnitList(string engYear, int parentSeq)
        {
            int userSeq = new SessionManager().GetUser().Seq;
            string sql;
            int year = Int32.Parse(engYear);
            if (UserRoleCheckService.checkSupervisor(userSeq))
            {
                sql = @"

                SELECT DISTINCT
                    b.OrderNo,
                    a.ExecSubUnitSeq UnitSeq,
                    b.Name UnitName
                FROM EngMain a

                inner join Unit b on(b.Seq=a.ExecSubUnitSeq and @ParentSeq=b.parentSeq)
                inner join EngSupervisor es
                on es.EngMainSeq = a.Seq
                where 1 =1 
                " + ((year == -1) ? "" : " and a.EngYear=@EngYear ") + @"
                and es.UserMainSeq =" + userSeq +
                " order by b.OrderNo";
            }
            else
            {
                sql = @"
                SELECT DISTINCT
                    b.OrderNo,
                    a.ExecSubUnitSeq UnitSeq,
                    b.Name UnitName
                FROM EngMain a

                inner join Unit b on(b.Seq=a.ExecSubUnitSeq and @ParentSeq=b.parentSeq)
                where 1=1
                " + ((year == -1) ? "": " and a.EngYear=@EngYear " ) 
                + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @" order by b.OrderNo";
            }
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngYear", engYear);
            cmd.Parameters.AddWithValue("@ParentSeq", parentSeq);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<EngExecUnitsVModel>(cmd);
        }
        //工程清單總筆數
        public int GetEngListCount(int year, int hasCouncil, int unitSeq, int subUnitSeq)
        {
            string sql = @"";
            int userSeq = new SessionManager().GetUser().Seq;
            bool isSuperviosr = UserRoleCheckService.checkSupervisor(userSeq);
            if (subUnitSeq == -1) {
                sql = @"
                SELECT distinct
                    a.Seq
                FROM EngMain a
                inner join Unit b on(b.Seq=a.ExecUnitSeq)

                " + ((isSuperviosr) ? "left join EngSupervisor es on es.EngMainSeq = a.Seq" : "") + @"
                where a.ExecUnitSeq=@ExecUnitSeq
                "
                + ((year == -1) ? "" : "  and a.EngYear=" + year)
                + ((isSuperviosr) ? " and es.UserMainSeq=" + userSeq : Utils.getAuthoritySql("a."))
                + ((hasCouncil >= 0) ? ((hasCouncil == 0) ? " and a.PrjXMLSeq is not null"
                  : "  and a.PrjXMLSeq is null") : "" );///and a.CreateUserSeq=@CreateUserSeq";
            }
            else
            {
                sql = @"
                SELECT distinct
                    a.Seq
                FROM EngMain a
                inner join Unit b on(b.Seq=a.ExecUnitSeq)

                " + ((isSuperviosr) ? "left join EngSupervisor es on es.EngMainSeq = a.Seq" : "") + @"
                where a.ExecSubUnitSeq=@ExecSubUnitSeq
                "
                + ((year == -1) ? "" : " and a.EngYear=" + year)
                + ((isSuperviosr) ? " and es.UserMainSeq=" + userSeq : Utils.getAuthoritySql("a."))
                + ((hasCouncil >= 0) ? ((hasCouncil == 0) ? " and a.PrjXMLSeq is not null"
                  : "  and a.PrjXMLSeq is null") : "");///and a.CreateUserSeq=@CreateUserSeq";
            }
            string sql2 = @"
                SELECT count(*) as total from ("+ sql + @") b
            ";
            SqlCommand cmd = db.GetCommand(sql2);
            cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@Seq", engMain);
            cmd.Parameters.AddWithValue("@ExecUnitSeq", unitSeq);
            cmd.Parameters.AddWithValue("@ExecSubUnitSeq", subUnitSeq);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());

            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        //工程清單
        public List<T> GetEngList<T>(int year, int hasCouncil, int unitSeq, int subUnitSeq, int pageRecordCount, int pageIndex)
        {
            string sql = @"";
            int userSeq = new SessionManager().GetUser().Seq;
            bool isSuperviosr = UserRoleCheckService.checkSupervisor(userSeq);
            if (subUnitSeq == -1)
            {
                sql = @"
                    SELECT distinct
                        a.Seq
                    FROM EngMain a

                    " + ((isSuperviosr) ? " left join EngSupervisor es on es.EngMainSeq = a.Seq" : "") + @"
                    where a.ExecUnitSeq=@ExecUnitSeq
                    "
                    + ((year == -1) ? "" : " and a.EngYear=" + year)
                    + ((isSuperviosr) ? " and es.UserMainSeq=" + userSeq : Utils.getAuthoritySql("a."))
                    + ((hasCouncil >= 0) ? ((hasCouncil == 0) ? " and a.PrjXMLSeq is not null"
                      : "  and a.PrjXMLSeq is null") : "");///and a.CreateUserSeq=@CreateUserSeq";

            }
            else
            {
                sql = @"
                    SELECT distinct
                        a.Seq
                    FROM EngMain a

                    " + ((isSuperviosr) ? "left join EngSupervisor es on es.EngMainSeq = a.Seq" : "") + @"
                    where a.ExecSubUnitSeq=@ExecSubUnitSeq
                    "
                    + ((year == -1) ? "" : " and a.EngYear=" + year)
                    + ((isSuperviosr) ? " and es.UserMainSeq=" + userSeq : Utils.getAuthoritySql("a.")) //and a.CreateUserSeq=@CreateUserSeq
                    + ((hasCouncil >= 0) ? ((hasCouncil == 0) ? " and a.PrjXMLSeq is not null"
                      : "  and a.PrjXMLSeq is null") : "");///and a.CreateUserSeq=@CreateUserSeq";
            }
            string sql2 = @"
                     SELECT
                        aa.Seq,
                        aa.EngNo,
                        aa.EngName,
                        b.Name ExecUnit, 
                        c.Name ExecSubUnit,
                        aa.SupervisorUnitName,
                        aa.ApproveDate,
                        aa.PccesXMLDate,
                        d.DocState,
                        aa.PrjXMLSeq
                    FROM EngMain aa
                    inner join Unit b on(b.Seq=aa.ExecUnitSeq)
                    left outer join Unit c on(c.Seq=aa.ExecSubUnitSeq)
                    left outer join SupervisionProjectList d on(
                        d.EngMainSeq=aa.Seq
                        and d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=aa.Seq)
                    )
                    where aa.Seq in (" + sql + @")
                    order by aa.EngNo DESC
                    OFFSET @pageIndex ROWS
                    FETCH FIRST @pageRecordCount ROWS ONLY";

            SqlCommand cmd = db.GetCommand(sql2);
            cmd.Parameters.AddWithValue("@ExecUnitSeq", unitSeq);
            cmd.Parameters.AddWithValue("@ExecSubUnitSeq", subUnitSeq);
            //cmd.Parameters.AddWithValue("@Seq", engMain);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex-1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<T>(cmd);
        }
        //新增工程
        public int AddEngItem(PCCESSMainModel pcessMain, List<PCCESPayItemModel> payItems, EngMainModel engMain, ref string errMsg)
        {
            string sql="", sql1="";
            SqlCommand cmd;
            int newSeq = 0;
            DataTable dt;
            int userSeq = getUserSeq();
            engMain.OrganizerUserSeq = userSeq;//承辦人預設登入者
            int? engTownSeq = GetEngTownSeq(pcessMain.ContractLocation);
            int? organizerUnitSeq = unitService.GetUnitSeq(pcessMain.ProcuringEntityId);
            int? organizerSubUnitSeq = null;
            List<VUserMain> users = new UserService().GetUserInfo(userSeq);
            VUserMain user = users[0];
            if (!organizerUnitSeq.HasValue || (organizerUnitSeq.HasValue && organizerUnitSeq.Value== user.UnitSeq1))
            {
                organizerUnitSeq = user.UnitSeq1;
                organizerSubUnitSeq = user.UnitSeq2;
            }

            db.BeginTransaction();
            try
            {
                sql = @"
				    INSERT INTO PCCESSMain (
                        ProcuringEntity,
                        ProcuringEntityId,
                        ContractTitle,
                        ContractLocation,
                        contractNo,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    )values(
                        @ProcuringEntity,
                        @ProcuringEntityId,
                        @ContractTitle,
                        @ContractLocation,
                        @contractNo,
                        GETDATE(),
                        @CreateUserSeq,
                        GETDATE(),
                        @ModifyUserSeq
                    )";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ProcuringEntity", pcessMain.ProcuringEntity);
                cmd.Parameters.AddWithValue("@ProcuringEntityId", pcessMain.ProcuringEntityId);
                cmd.Parameters.AddWithValue("@ContractTitle", pcessMain.ContractTitle);
                cmd.Parameters.AddWithValue("@ContractLocation", pcessMain.ContractLocation);
                cmd.Parameters.AddWithValue("@contractNo", pcessMain.contractNo); 
                cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                sql1 = @"
					SELECT IDENT_CURRENT('PCCESSMain') AS NewSeq";
                cmd = db.GetCommand(sql1);
                dt = db.GetDataTable(cmd);
                newSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                foreach (PCCESPayItemModel item in payItems)
                {
                    sql = @"
				    INSERT INTO PCCESPayItem (
                        PCCESSMainSeq, PayItem, Description, Unit, Quantity,
                        Price, Amount, ItemKey, ItemNo, RefItemCode
                    )values(
                        @PCCESSMainSeq, @PayItem, @Description, @Unit, @Quantity,
                        @Price, @Amount, @ItemKey, @ItemNo, @RefItemCode
                    )";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@PCCESSMainSeq", newSeq);
                    cmd.Parameters.AddWithValue("@PayItem", item.PayItem);
                    cmd.Parameters.AddWithValue("@Description", item.Description);
                    cmd.Parameters.AddWithValue("@Unit", item.Unit);
                    cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    cmd.Parameters.AddWithValue("@Price", item.Price);
                    cmd.Parameters.AddWithValue("@Amount", item.Amount);
                    cmd.Parameters.AddWithValue("@ItemKey", item.ItemKey);
                    cmd.Parameters.AddWithValue("@ItemNo", item.ItemNo);
                    cmd.Parameters.AddWithValue("@RefItemCode", item.RefItemCode);
                    db.ExecuteNonQuery(cmd);

                    cmd.Parameters.Clear();
                    sql1 = @" SELECT IDENT_CURRENT('PCCESPayItem') AS NewSeq";
                    cmd = db.GetCommand(sql1);
                    dt = db.GetDataTable(cmd);
                    int payItemSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                    sql = @"
				        INSERT INTO PCCESWorkItem (
                            PCCESPayItemSeq, WorkItemQuantity, ItemCode, ItemKind, Description, Unit, Quantity,
                            Price, Amount, Remark, LabourRatio, EquipmentRatio, MaterialRatio, MiscellaneaRatio
                        )values(
                            @PCCESPayItemSeq, @WorkItemQuantity, @ItemCode, @ItemKind, @Description, @Unit, @Quantity,
                            @Price, @Amount, @Remark, @LabourRatio, @EquipmentRatio, @MaterialRatio, @MiscellaneaRatio
                        )";
                    foreach (PCCESWorkItemModel wi in item.workItems)
                    {
                        Null2Empty(wi);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@PCCESPayItemSeq", payItemSeq);
                        cmd.Parameters.AddWithValue("@WorkItemQuantity", wi.WorkItemQuantity);
                        cmd.Parameters.AddWithValue("@ItemCode", wi.ItemCode);
                        cmd.Parameters.AddWithValue("@ItemKind", wi.ItemKind); 
                        cmd.Parameters.AddWithValue("@Description", wi.Description);
                        cmd.Parameters.AddWithValue("@Unit", wi.Unit);
                        cmd.Parameters.AddWithValue("@Quantity", wi.Quantity);
                        cmd.Parameters.AddWithValue("@Price", wi.Price);
                        cmd.Parameters.AddWithValue("@Amount", wi.Amount);
                        cmd.Parameters.AddWithValue("@Remark", wi.Remark);
                        cmd.Parameters.AddWithValue("@LabourRatio", wi.LabourRatio);
                        cmd.Parameters.AddWithValue("@EquipmentRatio", wi.EquipmentRatio);
                        cmd.Parameters.AddWithValue("@MaterialRatio", wi.MaterialRatio);
                        cmd.Parameters.AddWithValue("@MiscellaneaRatio", wi.MiscellaneaRatio);
                        db.ExecuteNonQuery(cmd);
                    }
                }

                sql = @"
				    INSERT INTO EngMain (
                        EngYear,
                        EngNo,
                        EngName,
                        OrganizerUnitCode,
                        OrganizerUnitSeq,
                        OrganizerSubUnitSeq,
                        OrganizerUserSeq,
                        ExecUnitSeq,
                        ExecSubUnitSeq,
                        TotalBudget,
                        SubContractingBudget,
                        PurchaseAmount,
                        EngTownSeq,
                        PccesXMLFile,
                        PccesXMLDate,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    )values(
                        @EngYear,
                        @EngNo,
                        @EngName,
                        @OrganizerUnitCode,
                        @OrganizerUnitSeq,
                        @OrganizerSubUnitSeq,
                        @ModifyUserSeq,
                        @ExecUnitSeq,
                        @ExecSubUnitSeq,
                        @TotalBudget,
                        @SubContractingBudget,
                        @PurchaseAmount,
                        @EngTownSeq,
                        @PccesXMLFile,
                        GETDATE(),
                        GETDATE(),
                        @ModifyUserSeq,
                        GETDATE(),
                        @ModifyUserSeq
                    )";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngYear", engMain.EngYear);
                cmd.Parameters.AddWithValue("@EngNo", engMain.EngNo);
                cmd.Parameters.AddWithValue("@EngName", engMain.EngName);
                cmd.Parameters.AddWithValue("@OrganizerUnitCode", engMain.OrganizerUnitCode);
                cmd.Parameters.AddWithValue("@OrganizerUnitSeq", this.NulltoDBNull(organizerUnitSeq));
                cmd.Parameters.AddWithValue("@OrganizerSubUnitSeq", this.NulltoDBNull(organizerSubUnitSeq));
                cmd.Parameters.AddWithValue("@ExecUnitSeq", this.NulltoDBNull(organizerUnitSeq));
                cmd.Parameters.AddWithValue("@ExecSubUnitSeq", this.NulltoDBNull(organizerSubUnitSeq));
                cmd.Parameters.AddWithValue("@TotalBudget", engMain.TotalBudget);
                cmd.Parameters.AddWithValue("@SubContractingBudget", engMain.SubContractingBudget);
                cmd.Parameters.AddWithValue("@PurchaseAmount", engMain.PurchaseAmount);
                cmd.Parameters.AddWithValue("@EngTownSeq", this.NulltoDBNull(engTownSeq));
                cmd.Parameters.AddWithValue("@PccesXMLFile", engMain.PccesXMLFile);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                sql1 = @"SELECT IDENT_CURRENT('EngMain') AS NewSeq";
                cmd = db.GetCommand(sql1);
                dt = db.GetDataTable(cmd);
                newSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                db.TransactionCommit();
                return newSeq;
            }
            catch (Exception ex)
            {
                db.TransactionRollback();
                errMsg = ex.Message;
                log.Info(ex.Message);
                //log.Info(sql);
                return 0;
            }
        }
        //由 xml 更新工程
        public int UpdateEngItem(PCCESSMainModel pcessMain, List<PCCESPayItemModel> payItems, EngMainModel engMain, ref string errMsg)
        {
            string sql = "", sql1 = "";
            SqlCommand cmd;
            int newSeq = 0;
            DataTable dt;
            //int? engTownSeq = GetEngTownSeq(pcessMain.ContractLocation);
            //int? organizerUnitSeq = unitService.GetUnitSeq(pcessMain.ProcuringEntityId);

            int userSeq = getUserSeq();
            engMain.OrganizerUserSeq = userSeq;//承辦人預設登入者
            int? engTownSeq = GetEngTownSeq(pcessMain.ContractLocation);
            int? organizerUnitSeq = unitService.GetUnitSeq(pcessMain.ProcuringEntityId);
            int? organizerSubUnitSeq = null;
            List<VUserMain> users = new UserService().GetUserInfo(userSeq);
            VUserMain user = users[0];
            if (!organizerUnitSeq.HasValue || (organizerUnitSeq.HasValue && organizerUnitSeq.Value == user.UnitSeq1))
            {
                organizerUnitSeq = user.UnitSeq1;
                organizerSubUnitSeq = user.UnitSeq2;
            }

            db.BeginTransaction();
            try
            {
                sql = @"
				    update PCCESSMain set
                        ProcuringEntity = @ProcuringEntity,
                        ProcuringEntityId = @ProcuringEntityId,
                        ContractTitle = @ContractTitle,
                        ContractLocation = @ContractLocation,
                        ModifyTime = GETDATE(),
                        ModifyUserSeq = @ModifyUserSeq
                    where Seq=@Seq                
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ProcuringEntity", pcessMain.ProcuringEntity);
                cmd.Parameters.AddWithValue("@ProcuringEntityId", pcessMain.ProcuringEntityId);
                cmd.Parameters.AddWithValue("@ContractTitle", pcessMain.ContractTitle);
                cmd.Parameters.AddWithValue("@ContractLocation", pcessMain.ContractLocation);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@Seq", pcessMain.Seq);
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                sql = @"
                    delete PCCESWorkItem where PCCESPayItemSeq in (
                        select seq from PCCESPayItem where PCCESSMainSeq=@PCCESSMainSeq
                    );
                    delete PCCESPayItem where PCCESSMainSeq=@PCCESSMainSeq;
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PCCESSMainSeq", pcessMain.Seq);
                db.ExecuteNonQuery(cmd);

                foreach (PCCESPayItemModel item in payItems)
                {
                    sql = @"
				    INSERT INTO PCCESPayItem (
                        PCCESSMainSeq, PayItem, Description, Unit, Quantity,
                        Price, Amount, ItemKey, ItemNo, RefItemCode
                    )values(
                        @PCCESSMainSeq, @PayItem, @Description, @Unit, @Quantity,
                        @Price, @Amount, @ItemKey, @ItemNo, @RefItemCode
                    )";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@PCCESSMainSeq", pcessMain.Seq);
                    cmd.Parameters.AddWithValue("@PayItem", item.PayItem);
                    cmd.Parameters.AddWithValue("@Description", item.Description);
                    cmd.Parameters.AddWithValue("@Unit", item.Unit);
                    cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    cmd.Parameters.AddWithValue("@Price", item.Price);
                    cmd.Parameters.AddWithValue("@Amount", item.Amount);
                    cmd.Parameters.AddWithValue("@ItemKey", item.ItemKey);
                    cmd.Parameters.AddWithValue("@ItemNo", item.ItemNo);
                    cmd.Parameters.AddWithValue("@RefItemCode", item.RefItemCode);
                    db.ExecuteNonQuery(cmd);

                    cmd.Parameters.Clear();
                    sql1 = @" SELECT IDENT_CURRENT('PCCESPayItem') AS NewSeq";
                    cmd = db.GetCommand(sql1);
                    dt = db.GetDataTable(cmd);
                    int payItemSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                    sql = @"
				        INSERT INTO PCCESWorkItem (
                            PCCESPayItemSeq, WorkItemQuantity, ItemCode, ItemKind, Description, Unit, Quantity,
                            Price, Amount, Remark, LabourRatio, EquipmentRatio, MaterialRatio, MiscellaneaRatio
                        )values(
                            @PCCESPayItemSeq, @WorkItemQuantity, @ItemCode, @ItemKind, @Description, @Unit, @Quantity,
                            @Price, @Amount, @Remark, @LabourRatio, @EquipmentRatio, @MaterialRatio, @MiscellaneaRatio
                        )";
                    foreach (PCCESWorkItemModel wi in item.workItems)
                    {
                        Null2Empty(wi);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@PCCESPayItemSeq", payItemSeq);
                        cmd.Parameters.AddWithValue("@WorkItemQuantity", wi.WorkItemQuantity);
                        cmd.Parameters.AddWithValue("@ItemCode", wi.ItemCode);
                        cmd.Parameters.AddWithValue("@ItemKind", wi.ItemKind); 
                        cmd.Parameters.AddWithValue("@Description", wi.Description);
                        cmd.Parameters.AddWithValue("@Unit", wi.Unit);
                        cmd.Parameters.AddWithValue("@Quantity", wi.Quantity);
                        cmd.Parameters.AddWithValue("@Price", wi.Price);
                        cmd.Parameters.AddWithValue("@Amount", wi.Amount);
                        cmd.Parameters.AddWithValue("@Remark", wi.Remark);
                        cmd.Parameters.AddWithValue("@LabourRatio", wi.LabourRatio);
                        cmd.Parameters.AddWithValue("@EquipmentRatio", wi.EquipmentRatio);
                        cmd.Parameters.AddWithValue("@MaterialRatio", wi.MaterialRatio);
                        cmd.Parameters.AddWithValue("@MiscellaneaRatio", wi.MiscellaneaRatio);
                        db.ExecuteNonQuery(cmd);
                    }
                }

                sql = @"
				    update EngMain set
                        EngYear = @EngYear,
                        EngName = @EngName,
                        OrganizerUnitCode = @OrganizerUnitCode,
                        OrganizerUnitSeq = @OrganizerUnitSeq,
                        OrganizerSubUnitSeq = @OrganizerSubUnitSeq,
                        ExecUnitSeq = @ExecUnitSeq,
                        SubContractingBudget = @SubContractingBudget,
                        TotalBudget = @TotalBudget,
                        PurchaseAmount = @PurchaseAmount,
                        EngTownSeq = @EngTownSeq,
                        PccesXMLFile = @PccesXMLFile,
                        PccesXMLDate = GETDATE(),
                        ModifyTime = GETDATE(),
                        ModifyUserSeq = @ModifyUserSeq
                    where EngNo=@EngNo
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngYear", engMain.EngYear);
                cmd.Parameters.AddWithValue("@EngName", engMain.EngName);
                cmd.Parameters.AddWithValue("@OrganizerUnitCode", engMain.OrganizerUnitCode);
                cmd.Parameters.AddWithValue("@OrganizerUnitSeq", this.NulltoDBNull(organizerUnitSeq));
                cmd.Parameters.AddWithValue("@OrganizerSubUnitSeq", this.NulltoDBNull(organizerSubUnitSeq));
                cmd.Parameters.AddWithValue("@ExecUnitSeq", this.NulltoDBNull(organizerUnitSeq));
                cmd.Parameters.AddWithValue("@SubContractingBudget", engMain.SubContractingBudget);//s20231116
                cmd.Parameters.AddWithValue("@TotalBudget", engMain.TotalBudget);
                cmd.Parameters.AddWithValue("@PurchaseAmount", engMain.PurchaseAmount);
                cmd.Parameters.AddWithValue("@EngTownSeq", this.NulltoDBNull(engTownSeq));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@EngNo", engMain.EngNo);
                cmd.Parameters.AddWithValue("@PccesXMLFile", engMain.PccesXMLFile);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                db.Connection.Close();
                return engMain.Seq;
            }
            catch (Exception ex)
            {
                db.TransactionRollback();
                errMsg = ex.Message;
                log.Info(ex.Message);
                log.Info(sql);
                return 0;
            }
        }
        //取得鄉鎮 Seq
        public int? GetEngTownSeq(string cityTown)
        {
            string sql = @"
                SELECT
                    a.seq EngTownSeq from Town a
                inner join City b on(b.Seq=a.CitySeq)
                where (b.CityName+a.TownName)=@cityTown
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@cityTown", cityTown.Replace("台","臺"));
            DataTable dt = db.GetDataTable(cmd);
            if (dt.Rows.Count == 1)
            {
                return Convert.ToInt32(dt.Rows[0]["EngTownSeq"].ToString());
            } else
            {
                return null;
            }
        }
    }
}
