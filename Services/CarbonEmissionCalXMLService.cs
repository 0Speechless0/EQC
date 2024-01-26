using EQC.Common;
using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EQC.Services
{
    public class CarbonEmissionCalXMLService : CarbonEmissionPayItemService
    {

        UnitService unitService;
        TenderPlanService tenderService;
        public CarbonEmissionCalXMLService()
        {
            unitService = new UnitService();
            tenderService = new TenderPlanService();
        }
        //Copy PCCESPayItem to CarbonEmissionPayItem
        public List<CarbonEmissionPayItemModel> GetAllPayItem(int headerSeq)
        {
            string sql = @"select * from CarbonEmissionCalXMLPayItem a where a.CarbonEmissionCalXMLHeaderSeq = @headerSeq";
            var cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@headerSeq", headerSeq);
            return db.GetDataTableWithClass<CarbonEmissionPayItemModel>(cmd);
        }
        public int deletePayItemWithWorkItem(int carbonHeaderSeq)
        {
            string sql = @"
                delete from CarbonEmissionCalXMLWorkItem where CarbonEmissionCalXMLWorkItem.CarbonEmissionCalXMLPayItemSeq 

                in (select a.Seq from CarbonEmissionCalXMLPayItem a where a.CarbonEmissionCalXMLHeaderSeq = @carbonHeaderSeq)


                delete from CarbonEmissionCalXMLPayItem where CarbonEmissionCalXMLPayItem .CarbonEmissionCalXMLHeaderSeq = @carbonHeaderSeq;
            ";
            var cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@carbonHeaderSeq", carbonHeaderSeq);
            return db.ExecuteNonQuery(cmd);
        }
        public int updatePayItem(CarbonEmissionPayItemVModel m)
        {
            string sql = @"
                update CarbonEmissionCalXMLPayItem  
                    set 
                    Memo = @memo ,
                    Unit = @unit ,
                    Quantity = @quantity 
                where Seq = @payItemSeq
            ";
            var cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@memo", m.Memo ?? "");
            cmd.Parameters.AddWithValue("@unit", m.Unit ?? "");
            cmd.Parameters.AddWithValue("@quantity", m.Quantity ?? 0);
            cmd.Parameters.AddWithValue("@payItemSeq", m.Seq);
            return db.ExecuteNonQuery(cmd);
        }
        //更新 PCCES.xml
        public int UpdatePCCES(List<PCCESPayItemModel> payItems, CarbonEmissionHeaderModel headerModel, ref string errMsg)
        {
            return insertPCCES(payItems, headerModel, ref errMsg);
        }
        public int insertPCCES(List<PCCESPayItemModel> payItems, CarbonEmissionHeaderModel headerModel, ref string errMsg)
        {
            string sql = "", sql1 = "";
            SqlCommand cmd;
            DataTable dt;

            db.BeginTransaction();
            try
            {
                sql = @"
                    DELETE CarbonEmissionCalXMLWorkItem where CarbonEmissionCalXMLPayItemSeq in(
                        select Seq from CarbonEmissionCalXMLPayItem where CarbonEmissionCalXMLHeaderSeq=@CarbonEmissionCalXMLHeaderSeq
                    );
                    DELETE CarbonEmissionCalXMLPayItem where CarbonEmissionCalXMLHeaderSeq=@CarbonEmissionCalXMLHeaderSeq;
                ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CarbonEmissionCalXMLHeaderSeq", headerModel.Seq);
                db.ExecuteNonQuery(cmd);

                sql = @"update CarbonEmissionCalXMLHeader set
                            PccesXMLFile=@PccesXMLFile,
                            PccesXMLDate=GetDate(),
                            ModifyTime=GetDate(),
                            ModifyUserSeq=@ModifyUserSeq
                        where Seq=@Seq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", headerModel.Seq);
                cmd.Parameters.AddWithValue("@PccesXMLFile", headerModel.PccesXMLFile);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", 0);
                db.ExecuteNonQuery(cmd);

                foreach (PCCESPayItemModel item in payItems)
                {
                    sql = @"
                        INSERT INTO CarbonEmissionCalXMLPayItem (
                            CarbonEmissionCalXMLHeaderSeq,
                            PayItem, Description, Unit, Quantity, Price, Amount, ItemKey, ItemNo, RefItemCode,
                            RStatusCode, CreateTime, CreateUserSeq, ModifyTime, ModifyUserSeq
                        )values(
                            @CarbonEmissionCalXMLHeaderSeq,
                            @PayItem, @Description, @Unit, @Quantity, @Price, @Amount, @ItemKey, @ItemNo, @RefItemCode,
                            @RStatusCode, GetDate(), @ModifyUserSeq, GetDate(), @ModifyUserSeq
                        )";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@CarbonEmissionCalXMLHeaderSeq", headerModel.Seq);
                    cmd.Parameters.AddWithValue("@PayItem", item.PayItem);
                    cmd.Parameters.AddWithValue("@Description", item.Description);
                    cmd.Parameters.AddWithValue("@Unit", item.Unit);
                    cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    cmd.Parameters.AddWithValue("@Price", item.Price);
                    cmd.Parameters.AddWithValue("@Amount", item.Amount);
                    cmd.Parameters.AddWithValue("@ItemKey", item.ItemKey);
                    cmd.Parameters.AddWithValue("@ItemNo", item.ItemNo);
                    string refItemCode = item.RefItemCode == null ? "" : item.RefItemCode.Trim();
                    cmd.Parameters.AddWithValue("@RefItemCode", refItemCode);
                    int RStatusCode = _None;//不須匹配
                    if (!String.IsNullOrEmpty(refItemCode))
                    {
                        if (refItemCode.Length < 10) RStatusCode = _NotLongEnough;//不足10碼
                        else RStatusCode = _Init;
                    }

                    cmd.Parameters.AddWithValue("@RStatusCode", RStatusCode);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", 0);
                    db.ExecuteNonQuery(cmd);

                    cmd.Parameters.Clear();
                    sql1 = @" SELECT IDENT_CURRENT('CarbonEmissionCalXMLPayItem') AS NewSeq";
                    cmd = db.GetCommand(sql1);
                    dt = db.GetDataTable(cmd);
                    int payItemSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                    sql = @"
				        INSERT INTO CarbonEmissionCalXMLWorkItem (
                            CarbonEmissionCalXMLPayItemSeq, WorkItemQuantity, ItemCode, ItemKind, Description, Unit, Quantity,
                            Price, Amount, Remark, LabourRatio, EquipmentRatio, MaterialRatio, MiscellaneaRatio
                        )values(
                            @CarbonEmissionCalXMLPayItemSeq, @WorkItemQuantity, @ItemCode, @ItemKind, @Description, @Unit, @Quantity,
                            @Price, @Amount, @Remark, @LabourRatio, @EquipmentRatio, @MaterialRatio, @MiscellaneaRatio
                        )";
                    foreach (WorkItemModel wi in item.workItems)
                    {
                        Utils.Null2Empty(wi);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@CarbonEmissionCalXMLPayItemSeq", payItemSeq);
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

                db.TransactionCommit();
                return 1;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                errMsg = e.Message;
                //log.Info("CarbonEmissionCalXMLPayItemService.UpdatePCCES: " + e.Message);
                //log.Info(sql);
                return -1;
            }
        }
        public bool CalCarbonEmissions(int engMainSeq)
        {
            string sql = @"
                select z.RStatusCode, z.Seq, z.RefItemCode, z.Code, z.KgCo2e, z.Memo RStatus
                    ---,z.KeyCode1, a.Description, z.item
                from ( 
                    --shioulo 20230202
                    select " + _C10NotMatchUnit + @" RStatusCode,a.Seq, a.RefItemCode, null Code, null KgCo2e, null KeyCode1, null Memo
                    from CarbonEmissionCalXMLHeader b
                    inner join CarbonEmissionCalXMLPayItem a on(a.CarbonEmissionCalXMLHeaderSeq=b.Seq)
                    where b.EngMainCalXMLSeq=@EngMainCalXMLSeq
                    and a.RefItemCode like 'L%'
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300) 
                    and case (a.Unit)
					    when '時' THEN  '1'
					    when '工' THEN  '2'
					    when '月' THEN  '3'
					    when '式' THEN  '4'
					    when '年' THEN  '5'
                        ELSE '-1' END
				        != SUBSTRING(a.RefItemCode,len(a.RefItemCode), 1)
                    and SUBSTRING(a.RefItemCode,len(a.RefItemCode), 1) in ('1','2','3','4','5')

                    --shioulo 20221228
                    union all
                    select " + _C10NotMatchUnit + @" RStatusCode,a.Seq, a.RefItemCode, null Code, null KgCo2e, null KeyCode1, null Memo
                    from CarbonEmissionCalXMLHeader b
                    inner join CarbonEmissionCalXMLPayItem a on(a.CarbonEmissionCalXMLHeaderSeq=b.Seq)
                    where b.EngMainCalXMLSeq=@EngMainCalXMLSeq
                    and a.RefItemCode like 'E%'
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300) 
                    and case (a.Unit)
					    when '時' THEN  '1'
					    when '天' THEN  '2'
					    when '月' THEN  '3'
					    when '式' THEN  '4'
					    when '年' THEN  '5'
					    when '趟' THEN  '6'
					    when '半天' THEN  '7'
                        ELSE '-1' END
				        != SUBSTRING(a.RefItemCode,len(a.RefItemCode), 1)
                    and SUBSTRING(a.RefItemCode,len(a.RefItemCode), 1) in ('1','2','3','4','5','6','7')

                    union all
                    select " + _C10NotMatchUnit + @" RStatusCode,a.Seq, a.RefItemCode, null Code, null KgCo2e, null KeyCode1, null Memo
                    from CarbonEmissionCalXMLHeader b
                    inner join CarbonEmissionCalXMLPayItem a on(a.CarbonEmissionCalXMLHeaderSeq=b.Seq)
                    where b.EngMainCalXMLSeq=@EngMainCalXMLSeq
                    and a.RefItemCode not like 'E%'
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)
                    and case (a.Unit)
					    when 'M' THEN  '1'
					    when 'M2' THEN  '2'
					    when 'M3' THEN  '3'
					    when '式' THEN  '4'
					    when 'T' THEN  '5'
					    when '只' THEN  '6'
					    when '個' THEN  '7'
					    when '組' THEN  '8'
					    when 'KG' THEN  '9' 
                        ELSE '-1' END
				        != SUBSTRING(a.RefItemCode,len(a.RefItemCode), 1)
                    and SUBSTRING(a.RefItemCode,len(a.RefItemCode), 1) in ('1','2','3','4','5','6','7','8','9')

                    union all

                    select " + _FullMatch + @" RStatusCode,a.Seq, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo
                    from CarbonEmissionCalXMLHeader b
                    inner join CarbonEmissionCalXMLPayItem a on(a.CarbonEmissionCalXMLHeaderSeq=b.Seq) 
                    inner join CarbonEmissionFactor x1 on (a.RefItemCode=x1.Code )
                    where b.EngMainCalXMLSeq=@EngMainCalXMLSeq
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)

                    union all

                    select " + _Match + @" RStatusCode,a.Seq, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo
                    from CarbonEmissionCalXMLHeader b
                    inner join CarbonEmissionCalXMLPayItem a on(a.CarbonEmissionCalXMLHeaderSeq=b.Seq)
                    inner join CarbonEmissionFactor x1 on (SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 10) like x1.KeyCode1 )
                    where b.EngMainCalXMLSeq=@EngMainCalXMLSeq
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)

                    union all
                    select " + _MatchC10_0 + @" RStatusCode,a.Seq, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo
                    from CarbonEmissionCalXMLHeader b
                    inner join CarbonEmissionCalXMLPayItem a on(a.CarbonEmissionCalXMLHeaderSeq=b.Seq)
                    inner join (
                    	select * from CarbonEmissionFactor 
                        where KeyCode1 like '%0' and KeyCode2 <> '-1' /*and KeyCode3 = '-1'*/) x1 on (SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 10) like x1.KeyCode2 )
                    where b.EngMainCalXMLSeq=@EngMainCalXMLSeq
                    and SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 5) in ('02931','02932') -- shioulo 20230111
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)

                    union all -- shioulo 20230111
                    select " + _Tree0Match + @" RStatusCode,a.Seq, a.RefItemCode, '末碼0' Code, 0 KgCo2e, '_________0' KeyCode1, '歸類到樹木類' Memo
                    from CarbonEmissionCalXMLHeader b
                    inner join CarbonEmissionCalXMLPayItem a on(a.CarbonEmissionCalXMLHeaderSeq=b.Seq)
                    --inner join CarbonEmissionFactor x1 on (SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 10) like x1.KeyCode2 )
                    where b.EngMainCalXMLSeq=@EngMainCalXMLSeq
                    and SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 5) in ('02931','02932')
                    and a.RefItemCode like '%0'
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)

                    union all -- shioulo 20230107
                    select " + _C10NotMatch + @" RStatusCode,a.Seq, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo
                    from CarbonEmissionCalXMLHeader b
                    inner join CarbonEmissionCalXMLPayItem a on(a.CarbonEmissionCalXMLHeaderSeq=b.Seq)
                    inner join CarbonEmissionFactor x1 on (SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 10) like x1.KeyCode2 )
                    where b.EngMainCalXMLSeq=@EngMainCalXMLSeq
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)

                    union all

                    select 2 RStatusCode,a.Seq, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo
                    from CarbonEmissionCalXMLHeader b
                    inner join CarbonEmissionCalXMLPayItem a on(a.CarbonEmissionCalXMLHeaderSeq=b.Seq)
                    inner join CarbonEmissionFactor x1 on (SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 10) like x1.KeyCode3 )
                    where b.EngMainCalXMLSeq=@EngMainCalXMLSeq
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)

                    union all 

                    select " + _NotMatch + @" RStatusCode,a.Seq, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo
                    from CarbonEmissionCalXMLHeader b
                    inner join CarbonEmissionCalXMLPayItem a on(a.CarbonEmissionCalXMLHeaderSeq=b.Seq)
                    left outer join CarbonEmissionFactor x1 on (a.RefItemCode=x1.Code )
                    where b.EngMainCalXMLSeq=@EngMainCalXMLSeq
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)
                ) z";
            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainCalXMLSeq", engMainSeq);

            List<EQMCalCarbonEmissionsVModel> list = db.GetDataTableWithClass<EQMCalCarbonEmissionsVModel>(cmd);

            Dictionary<int, string> updateItem = new Dictionary<int, string>();

            db.BeginTransaction();
            try
            {
                foreach (EQMCalCarbonEmissionsVModel m in list)
                {
                    if (!updateItem.ContainsKey(m.Seq))
                    {
                        Null2Empty(m);
                        if (m.RStatusCode == _Match || m.RStatusCode == _FullMatch || (m.RStatusCode == 2) || m.RStatusCode == _MatchC10_0 || m.RStatusCode == _Tree0Match)
                        {
                            sql = @"update CarbonEmissionCalXMLPayItem set
                                KgCo2e=@KgCo2e,
                                ItemKgCo2e=@KgCo2e*Quantity,
                                Memo=@Memo,
                                RStatus=@RStatus,
                                RStatusCode=@RStatusCode,
                                ModifyTime=GetDate(),
                                ModifyUserSeq=@ModifyUserSeq
                            where Seq=@Seq";
                        }
                        else
                        {
                            sql = @"update CarbonEmissionCalXMLPayItem set
                                KgCo2e=null,
                                ItemKgCo2e=null,
                                Memo='',
                                RStatus='',
                                RStatusCode=@RStatusCode,
                                ModifyTime=GetDate(),
                                ModifyUserSeq=@ModifyUserSeq
                            where Seq=@Seq";
                        }
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@Seq", m.Seq);
                        cmd.Parameters.AddWithValue("@KgCo2e", m.KgCo2e);
                        cmd.Parameters.AddWithValue("@Memo", m.Code);
                        cmd.Parameters.AddWithValue("@RStatus", m.RStatus);
                        cmd.Parameters.AddWithValue("@RStatusCode", m.RStatusCode == 2 ? _NonTypeMatch : m.RStatusCode);
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                        db.ExecuteNonQuery(cmd);

                        updateItem.Add(m.Seq, m.Code);
                    }
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("CarbonEmissionCalXMLPayItemService.CalCarbonEmissions: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }
        public List<T> GetItemBySeq<T>(int seq)
        {
            string sql = @"
                SELECT
                    a.ExecType,
                    a.SupervisorExecType,
                    a.Seq,
                    a.EngYear,
                    a.EngNo,
                    a.EngName,
                    a.OrganizerUnitSeq,
                    a.ExecUnitSeq,
                    a.DesignUnitName,
                    a.DesignManName,
                    a.DesignUnitTaxId,
                    a.DesignUnitEmail,
                    a.TotalBudget,
                    a.SubContractingBudget,
                    a.ContractAmountAfterDesignChange,
                    a.PurchaseAmount,
                    a.ProjectScope,
                    a.EngTownSeq,
                    a.EngPeriod,
                    a.StartDate,
                    a.SchCompDate,
                    a.PostCompDate,
                    a.ApproveDate,
                    a.ApproveNo,
                    a.AwardAmount,
                    a.BuildContractorName,
                    a.BuildContractorContact,
                    a.BuildContractorTaxId,
                    a.BuildContractorEmail,
                    a.IsNeedElecDevice,
                    a.SupervisorUnitName,
                    a.SupervisorDirector,
                    a.SupervisorTechnician,
                    a.SupervisorTaxid,
                    a.SupervisorContact,
                    a.SupervisorSelfPerson1,
                    a.SupervisorSelfPerson2,
                    a.SupervisorCommPerson4,
                    a.SupervisorCommPersion2,
                    a.SupervisorCommPerson3,
                    a.SupervisorCommPerson4LicenseExpires,
                    a.SupervisorCommPerson3LicenseExpires,
                    a.EngChangeStartDate,
                    a.WarrantyExpires,
                    a.ConstructionDirector,
                    a.ConstructionPerson1,
                    a.ConstructionPerson2,
                    a.OrganizerSubUnitSeq,
                    a.ExecSubUnitSeq,
                    a.OrganizerUserSeq,
                    a.AwardDate,
                    a.CarbonDemandQuantity,
                    a.ApprovedCarbonQuantity,
                    a.OfficialApprovedCarbonQuantity,
                    b.CitySeq,
                    c.Name organizerUnitName,
                    c1.Name execUnitName,
                    c2.Name execSubUnitName,
                    d.DocState,
                    a.PrjXMLSeq,
                    e.TenderNo,
                    e.TenderName,
                    e.OrganizerName tenderOrgUnitName,
                    e.ExecUnitName tenderExecUnitName,
                    e.DurationCategory,
                    e.BidAmount
                FROM EngMainCalXML a
                left outer join Town b on(b.Seq=a.EngTownSeq)
                left outer join Unit c on(c.Seq=a.OrganizerUnitSeq)
                left outer join Unit c1 on(c1.Seq=a.ExecUnitSeq)
                left outer join Unit c2 on(c2.Seq=a.ExecSubUnitSeq)
                left outer join SupervisionProjectList d on(
                    d.EngMainSeq=a.Seq
                    and d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                )
                left outer join PrjXML e on(e.Seq=a.PrjXMLSeq)
                where
                    a.Seq=@Seq
            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public int GetListTotal(int engMainSeq, string keyWord)
        {
            if (!String.IsNullOrEmpty(keyWord)) keyWord += "%";
            string sql = @"
                select
                    count(a.Seq) total
                from CarbonEmissionCalXMLHeader b
                inner join CarbonEmissionCalXMLPayItem a on(a.CarbonEmissionCalXMLHeaderSeq=b.Seq)
                where b.EngMainCalXMLSeq=@EngMainCalXMLSeq
                and (@keyWord='' or a.PayItem like @keyWord)
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainCalXMLSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@keyWord", keyWord);

            DataTable dt = db.GetDataTable(cmd);
            int cnt = Convert.ToInt32(dt.Rows[0]["total"].ToString());

            return cnt;
        }
                //計算總碳量,金額
        public void CalCarbonTotal(int engMainSeq, ref decimal? Co2Total, ref decimal? Co2ItemTotal)
        {
            decimal? GreenFunding = null;
            CalCarbonTotal(engMainSeq, ref Co2Total, ref Co2ItemTotal, ref GreenFunding);
        }
        public void CalCarbonTotal(int engMainSeq, ref decimal? Co2Total, ref decimal? Co2ItemTotal, ref decimal? GreenFunding)
        {
            string sql = @"
                SELECT
	                sum(z.GreenFunding) GreenFunding, sum(z.Co2Total) Co2Total, sum(z.Co2ItemTotal) Co2ItemTotal
                from (
                select
	                0 GreenFunding,
                    ROUND(sum(ISNULL(a.ItemKgCo2e,0)),0) Co2Total,
                    ROUND(sum(ISNULL(a.Quantity * a.Price, 0)), 0) Co2ItemTotal
                from CarbonEmissionCalXMLHeader b
                inner join CarbonEmissionCalXMLPayItem a on(a.CarbonEmissionCalXMLHeaderSeq=b.Seq)
                where b.EngMainCalXMLSeq=@EngMainCalXMLSeq
                and a.KgCo2e is not null and a.ItemKgCo2e is not null

                union all
                
                select
                    ROUND(sum(ISNULL(a.Amount, 0)), 0) GreenFunding,
                    0 Co2Total,
                    0 Co2ItemTotal
                from CarbonEmissionCalXMLHeader b
                inner join CarbonEmissionCalXMLPayItem a on(a.CarbonEmissionCalXMLHeaderSeq=b.Seq and a.GreenFundingSeq is not null)
                where b.EngMainCalXMLSeq=@EngMainCalXMLSeq
                -- s20230524取消 and a.KgCo2e is not null and a.ItemKgCo2e is not null
                ) z
            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainCalXMLSeq", engMainSeq);
            DataTable dt = db.GetDataTable(cmd);
            try
            {
                Co2Total = Convert.ToDecimal(dt.Rows[0]["Co2Total"].ToString());
            }
            catch { }
            try
            {
                Co2ItemTotal = Convert.ToDecimal(dt.Rows[0]["Co2ItemTotal"].ToString());
            }
            catch { }
            try
            {
                GreenFunding = Convert.ToDecimal(dt.Rows[0]["GreenFunding"].ToString());
            }
            catch { }
        }
        public List<T> GetList<T>(int engMainSeq, int pageRecordCount, int pageIndex, string keyWord)
        {
            if (!String.IsNullOrEmpty(keyWord)) keyWord += "%";
            string sql = @"
                select
                    a.Seq,
                    a.CarbonEmissionCalXMLHeaderSeq,
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
                    a.RStatusCode,
                    a.GreenFundingSeq,
                    a.GreenFundingMemo
                from CarbonEmissionCalXMLHeader b
                inner join CarbonEmissionCalXMLPayItem a on(a.CarbonEmissionCalXMLHeaderSeq=b.Seq and (@keyWord='' or a.PayItem like @keyWord))
                where b.EngMainCalXMLSeq=@EngMainCalXMLSeq
                Order by a.Seq
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainCalXMLSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@keyWord", keyWord);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);

            return db.GetDataTableWithClass<T>(cmd);
        }
        internal void ReFresh()
        {
            string sql = $@"
                delete from CarbonEmissionCalXMLWorkItem;
                delete from CarbonEmissionCalXMLPayItem;
                delete from CarbonEmissionCalXMLHeader;
                delete from EngMainCalXML;
                DBCC CHECKIDENT('EngMainCalXML', RESEED, 0);
                DBCC CHECKIDENT('CarbonEmissionCalXMLHeader', RESEED, 0);
                DBCC CHECKIDENT('CarbonEmissionCalXMLPayItem', RESEED, 0);
                DBCC CHECKIDENT('CarbonEmissionCalXMLWorkItem', RESEED, 0);

        
            ";
            SqlCommand cmd = db.GetCommand(sql);
            db.ExecuteNonQuery(cmd);
        }
        public int AddEngItem(PCCESSMainModel pcessMain, EngMainModel engMain, ref string errMsg)
        {
            string sql = "", sql1 = "";
            SqlCommand cmd;
            int newSeq = 0;
            DataTable dt;
            int userSeq = getUserSeq();
            engMain.OrganizerUserSeq = userSeq;//承辦人預設登入者
            int? engTownSeq = tenderService.GetEngTownSeq(pcessMain.ContractLocation);
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
				    INSERT INTO EngMainCalXML (
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
                sql1 = @"SELECT IDENT_CURRENT('EngMainCalXML') AS NewSeq";
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
        public List<T> GetEngSeqByEngNo<T>(string engNo)
        {
            string sql = @"
                SELECT * FROM EngMainCalXML
                where EngNo=@engNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@engNo", engNo);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public bool CreatePayItems(int engMainSeq,  PccseXML pccseXML, string xmlPath = null)
        {
            //shioulo 20221006
            string sql = @"select a.Seq,
                        trim(a.PayItem) PayItem,a.[Description],a.Unit,a.Quantity,a.Price,a.Amount,a.ItemKey,a.ItemNo,a.RefItemCode
                        ,case
                            when len(trim(a.RefItemCode))>=10 then " + _Init + @"
                            when len(trim(a.RefItemCode))<10 and len(trim(a.RefItemCode))>0 then " + _NotLongEnough + @"
                            else " + _None + @"
                        end RStatusCode
                    from PCCESPayItem a
                    inner join PCCESSMain b on(
	                    b.Seq=a.PCCESSMainSeq
                        and b.contractNo=(select c.EngNo from EngMain c where c.Seq=@EngMainCalXMLSeq)
                    )
                    ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainCalXMLSeq", engMainSeq);
            List<CarbonEmissionPayItemInsertModel> pccesPayItemModel = db.GetDataTableWithClass<CarbonEmissionPayItemInsertModel>(cmd);

            sql = @"select * from PCCESWorkItem where PCCESPayItemSeq=@PCCESPayItemSeq";
            foreach (CarbonEmissionPayItemInsertModel m in pccesPayItemModel)
            {
                Utils.Null2Empty(m);
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PCCESPayItemSeq", m.Seq);
                m.workItems = db.GetDataTableWithClass<CarbonEmissionWorkItemModel>(cmd);
            }

            db.BeginTransaction();
            try
            {
                sql = @"
                    insert into CarbonEmissionCalXMLHeader (
                        PccesXMLFile,
                        EngMainCalXMLSeq,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @PccesXMLFile,
                        @EngMainCalXMLSeq,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainCalXMLSeq", engMainSeq);
                cmd.Parameters.AddWithValue("@PccesXMLFile", NulltoDBNull(xmlPath));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", 0);
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('CarbonEmissionCalXMLHeader') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);
                int carbonEmissionHeaderSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                foreach (PCCESPayItemModel item in pccseXML.payItems)
                {
                    sql = @"
                        insert into CarbonEmissionCalXMLPayItem (
                            CarbonEmissionCalXMLHeaderSeq
                            ,PayItem,[Description],Unit,Quantity,Price,Amount,ItemKey,ItemNo,RefItemCode
                            ,RStatusCode
                            ,CreateTime,CreateUserSeq,ModifyTime,ModifyUserSeq
                        ) values (
                            @CarbonEmissionCalXMLHeaderSeq
                            ,@PayItem,@Description,@Unit,@Quantity,@Price,@Amount,@ItemKey,@ItemNo,@RefItemCode
                            ,@RStatusCode
                            ,GetDate(),@ModifyUserSeq,GetDate(),@ModifyUserSeq
                        )
                        ";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@CarbonEmissionCalXMLHeaderSeq", carbonEmissionHeaderSeq);
                    cmd.Parameters.AddWithValue("@PayItem", item.PayItem);
                    cmd.Parameters.AddWithValue("@Description", item.Description);
                    cmd.Parameters.AddWithValue("@Unit", item.Unit);
                    cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    cmd.Parameters.AddWithValue("@Price", item.Price);
                    cmd.Parameters.AddWithValue("@Amount", item.Amount);
                    cmd.Parameters.AddWithValue("@ItemKey", item.ItemKey);
                    cmd.Parameters.AddWithValue("@ItemNo", item.ItemNo);
                    cmd.Parameters.AddWithValue("@RefItemCode", item.RefItemCode);
                    cmd.Parameters.AddWithValue("@RStatusCode", 0);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", 0);
                    db.ExecuteNonQuery(cmd);

                    cmd.Parameters.Clear();
                    sql1 = @" SELECT IDENT_CURRENT('CarbonEmissionCalXMLPayItem') AS NewSeq";
                    cmd = db.GetCommand(sql1);
                    dt = db.GetDataTable(cmd);
                    int payItemSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                    sql = @"
				        INSERT INTO CarbonEmissionCalXMLWorkItem (
                            CarbonEmissionCalXMLPayItemSeq, WorkItemQuantity, ItemCode, ItemKind, Description, Unit, Quantity,
                            Price, Amount, Remark, LabourRatio, EquipmentRatio, MaterialRatio, MiscellaneaRatio
                        )values(
                            @CarbonEmissionCalXMLPayItemSeq, @WorkItemQuantity, @ItemCode, @ItemKind, @Description, @Unit, @Quantity,
                            @Price, @Amount, @Remark, @LabourRatio, @EquipmentRatio, @MaterialRatio, @MiscellaneaRatio
                        )";
                    foreach (PCCESWorkItemModel wi in item.workItems)
                    {
                        Utils.Null2Empty(wi);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@CarbonEmissionCalXMLPayItemSeq", payItemSeq);
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

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();

                return false;
            }
        }
    }
}