using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class SchEngProgressService : BaseService
    {//前置作業
        public int FillCompleted(SchEngProgressHeaderModel sepHeader, decimal co2Total)
        {
            db.BeginTransaction();
            try
            {
                string sql = @"update SchEngProgressHeader set
                    SPState=1,
                    ModifyTime=GetDate(),
                    ModifyUserSeq=@ModifyUserSeq
                where Seq=@Seq";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", sepHeader.Seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                db.ExecuteNonQuery(cmd);
                //s20230420
                sql = @"update EngMain set
                    CarbonConstructionQuantity=@CarbonConstructionQuantity,
                    ModifyTime=GetDate(),
                    ModifyUserSeq=@ModifyUserSeq
                where Seq=@Seq";

                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", sepHeader.EngMainSeq);
                cmd.Parameters.AddWithValue("@CarbonConstructionQuantity", co2Total);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return 1;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchEngProgressService.FillCompleted: " + e.Message);
                //log.Info(sql);
                return -1;
            }
        }
        //還原刪除 s20230721
        public int UnDelRecord(int seq)
        {
            db.BeginTransaction();
            try
            {
                string sql = @"
                    SET IDENTITY_INSERT SchEngProgressPayItem ON;

                    insert into SchEngProgressPayItem (
                        Seq, SchEngProgressHeaderSeq, PayItem, Description, Unit, Quantity, Price, Amount, ItemKey, ItemNo,
                        RefItemCode, KgCo2e, ItemKgCo2e, Memo, RStatus, CreateTime, CreateUserSeq, ModifyTime, ModifyUserSeq,
                        RStatusCode, OrderNo, GreenFundingSeq, GreenFundingMemo
                    )
                    select 
                        Seq, SchEngProgressHeaderSeq, PayItem, Description, Unit, Quantity, Price, Amount, ItemKey, ItemNo,
                        RefItemCode, KgCo2e, ItemKgCo2e, Memo, RStatus, CreateTime, CreateUserSeq, ModifyTime, ModifyUserSeq,
                        RStatusCode, OrderNo, GreenFundingSeq, GreenFundingMemo
                    from SchEngProgressPayItemDel
                    where Seq=@Seq;

                    SET IDENTITY_INSERT SchEngProgressPayItem OFF;

                    SET IDENTITY_INSERT SchEngProgressWorkItem ON;

                    insert into SchEngProgressWorkItem (
                        Seq, SchEngProgressPayItemSeq, WorkItemQuantity, ItemCode, ItemKind, Description, Unit, Quantity,
                        Price, Amount, Remark, LabourRatio, EquipmentRatio, MaterialRatio, MiscellaneaRatio, OrderNo
                    )
                    select
                        Seq, SchEngProgressPayItemSeq, WorkItemQuantity, ItemCode, ItemKind, Description, Unit, Quantity,
                        Price, Amount, Remark, LabourRatio, EquipmentRatio, MaterialRatio, MiscellaneaRatio, OrderNo
                    from SchEngProgressWorkItemDel
                    where SchEngProgressPayItemSeq=@Seq;

                    SET IDENTITY_INSERT SchEngProgressWorkItem OFF;

                    delete from SchEngProgressWorkItemDel where SchEngProgressPayItemSeq=@Seq;

                    delete from SchEngProgressPayItemDel where Seq=@Seq;
                ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchEngProgressService.UnDelRecord: " + e.Message);
                return -1;
            }
        }
        //刪除 s20230721
        public int DelRecord(int seq)
        {
            db.BeginTransaction();
            try
            {
                string sql = @"
                    insert into SchEngProgressPayItemDel ( --s20230721
                        Seq, SchEngProgressHeaderSeq, PayItem, Description, Unit, Quantity, Price, Amount, ItemKey, ItemNo,
                        RefItemCode, KgCo2e, ItemKgCo2e, Memo, RStatus, CreateTime, CreateUserSeq, ModifyTime, ModifyUserSeq,
                        RStatusCode, OrderNo, GreenFundingSeq, GreenFundingMemo
                    )
                    select 
                        Seq, SchEngProgressHeaderSeq, PayItem, Description, Unit, Quantity, Price, Amount, ItemKey, ItemNo,
                        RefItemCode, KgCo2e, ItemKgCo2e, Memo, RStatus, CreateTime, CreateUserSeq, ModifyTime, ModifyUserSeq,
                        RStatusCode, OrderNo, GreenFundingSeq, GreenFundingMemo
                    from SchEngProgressPayItem
                    where Seq=@Seq;

                    insert into SchEngProgressWorkItemDel ( --s20230721
                        Seq, SchEngProgressPayItemSeq, WorkItemQuantity, ItemCode, ItemKind, Description, Unit, Quantity,
                        Price, Amount, Remark, LabourRatio, EquipmentRatio, MaterialRatio, MiscellaneaRatio, OrderNo
                    )
                    select
                        Seq, SchEngProgressPayItemSeq, WorkItemQuantity, ItemCode, ItemKind, Description, Unit, Quantity,
                        Price, Amount, Remark, LabourRatio, EquipmentRatio, MaterialRatio, MiscellaneaRatio, OrderNo
                    from SchEngProgressWorkItem
                    where SchEngProgressPayItemSeq=@Seq;

                    delete from SchEngProgressWorkItem where SchEngProgressPayItemSeq=@Seq;

                    delete from SchEngProgressPayItem where Seq=@Seq;
                ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchEngProgressService.DelRecord: " + e.Message);
                return -1;
            }
        }
        public int Update(SchEngProgressPayItemModel m)
        {
            Null2Empty(m);
            try
            {
                string sql = @"update SchEngProgressPayItem set
                    Unit=@Unit,
                    Quantity=@Quantity,
                    Price=@Price,
                    Amount=@Quantity*@Price,
                    ModifyTime=GetDate(),
                    ModifyUserSeq=@ModifyUserSeq
                where Seq=@Seq";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@Unit", m.Unit);
                cmd.Parameters.AddWithValue("@Quantity", m.Quantity);
                cmd.Parameters.AddWithValue("@Price", m.Price);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("SchEngProgressService.Update: " + e.Message);
                //log.Info(sql);
                return -1;
            }
        }

        //清單總筆數
        public int GetListTotal(int engMainSeq, string fLevel, string keyWord)
        {
            if (!String.IsNullOrEmpty(fLevel)) fLevel += "%";
            if (String.IsNullOrEmpty(keyWord))
                keyWord = "";
            else
                keyWord = String.Format("%{0}%", keyWord);
            string sql = @"
                select
                    count(a.Seq) total
                from SchEngProgressHeader b
                inner join SchEngProgressPayItem a on(a.SchEngProgressHeaderSeq=b.Seq
                    and (@fLevel='' or a.PayItem like @fLevel)
                    and (@keyWord='' or a.Description like @keyWord)
                )
                where b.EngMainSeq=@EngMainSeq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@fLevel", fLevel);
            cmd.Parameters.AddWithValue("@keyWord", keyWord);

            DataTable dt = db.GetDataTable(cmd);
            int cnt = Convert.ToInt32(dt.Rows[0]["total"].ToString());

            return cnt;
        }
        //清單
        public List<T> GetList<T>(int engMainSeq, int pageRecordCount, int pageIndex, string fLevel, string keyWord)
        {
            if (!String.IsNullOrEmpty(fLevel)) fLevel += "%";
            if (String.IsNullOrEmpty(keyWord))
                keyWord = "";
            else
                keyWord = String.Format("%{0}%", keyWord);

            string sql = @"
                select
                    a.Seq,
                    a.SchEngProgressHeaderSeq,
                    a.PayItem,
                    a.Description,
                    a.Unit,
                    a.Quantity,
                    a.Price,
                    a.Amount,
                    a.ItemKey,
                    a.ItemNo,
                    a.KgCo2e,
                    a.GreenFundingSeq,
                    a.GreenFundingMemo,
                    --a.RStatus,
                    --a.RStatusCode,
                    a.Memo
                from SchEngProgressHeader b
                inner join SchEngProgressPayItem a on(a.SchEngProgressHeaderSeq=b.Seq
                    and (@fLevel='' or a.PayItem like @fLevel)
                    and (@keyWord='' or a.Description like @keyWord)
                )
                where b.EngMainSeq=@EngMainSeq
                Order by a.OrderNo
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY
             ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@fLevel", fLevel);
            cmd.Parameters.AddWithValue("@keyWord", keyWord);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //刪除清單 s20230721
        public List<T> GetDelList<T>(int engMainSeq)
        {
            string sql = @"
                select
                    a.Seq,
                    a.SchEngProgressHeaderSeq,
                    a.PayItem,
                    a.Description,
                    a.Unit,
                    a.Quantity,
                    a.Price,
                    a.Amount,
                    a.ItemKey,
                    a.ItemNo,
                    a.KgCo2e,
                    a.GreenFundingSeq,
                    a.GreenFundingMemo,
                    --a.RStatus,
                    --a.RStatusCode,
                    a.Memo
                from SchEngProgressHeader b
                inner join SchEngProgressPayItemDel a on(a.SchEngProgressHeaderSeq=b.Seq)
                where b.EngMainSeq=@EngMainSeq
                Order by a.OrderNo
             ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //取得第一層大綱資料
        public List<T> GetLevel1Options<T>(int engMainSeq)
        {
            string sql = @"
                select
                    a.Seq,
                    trim(a.PayItem) Value,
                    a.Description Text
                from SchEngProgressHeader b
                inner join SchEngProgressPayItem a on(a.SchEngProgressHeaderSeq=b.Seq)
                where b.EngMainSeq=@EngMainSeq
                and LEN(a.PayItem)=1
                order by a.Seq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //取得第二層大綱資料
        public List<T> GetLevel2Options<T>(int engMainSeq, string key)
        {
            string sql = @"
                select
                    a.Seq,
                    trim(a.PayItem) Value,
                    a.Description Text
                from SchEngProgressHeader b
                inner join SchEngProgressPayItem a on(a.SchEngProgressHeaderSeq=b.Seq)
                where b.EngMainSeq=@EngMainSeq
                and (
                    SELECT COUNT(*) FROM STRING_SPLIT(a.PayItem, ',')
                    where a.PayItem like @LevelKey
                    and a.PayItem not like '%=%'
    			)=2
                order by a.Seq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@LevelKey", key+'%');

            return db.GetDataTableWithClass<T>(cmd);
        }
        //前置作業 PayItem 清單 s20230406
        public List<T> GetPayItemList<T>(int engMainSeq)
        {
            string sql = @"
                select
                    a.Seq,
                    a.SchEngProgressHeaderSeq,
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
                    a.OrderNo,
                    a.GreenFundingSeq,
                    a.GreenFundingMemo
                from SchEngProgressHeader b
                inner join SchEngProgressPayItem a on(a.SchEngProgressHeaderSeq=b.Seq)
                where b.EngMainSeq=@EngMainSeq
                Order by a.OrderNo,a.Seq
             ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //前置作業 WorkItem 清單 s20230406
        public void GetWorkItemList(List<SchEngProgressPayItem2Model> items)
        {
            string sql = @"
                select
                    Seq,
                    SchEngProgressPayItemSeq,
                    WorkItemQuantity,
                    ItemCode,
                    ItemKind,
                    Description,
                    Unit,
                    Quantity,
                    Price,
                    Amount,
                    Remark,
                    LabourRatio,
                    EquipmentRatio,
                    MaterialRatio,
                    MiscellaneaRatio,
                    OrderNo
                from SchEngProgressWorkItem
                where SchEngProgressPayItemSeq=@SchEngProgressPayItemSeq
             ";
            SqlCommand cmd;
            foreach (SchEngProgressPayItem2Model m in items)
            {
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@SchEngProgressPayItemSeq", m.Seq);
                m.workItems = db.GetDataTableWithClass<SchEngProgressWorkItemModel>(cmd);
            }
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
                    from SchEngProgressHeader b
                    inner join SchEngProgressPayItem a on(a.SchEngProgressHeaderSeq=b.Seq)
                    where b.EngMainSeq=@EngMainSeq
                    and a.KgCo2e is not null and a.ItemKgCo2e is not null

                    union all
                
                    select
                        ROUND(sum(ISNULL(a.Amount, 0)), 0) GreenFunding,
                        0 Co2Total,
                        0 Co2ItemTotal
                    from SchEngProgressHeader b
                    inner join SchEngProgressPayItem a on(a.SchEngProgressHeaderSeq=b.Seq and a.GreenFundingSeq is not null)
                    where b.EngMainSeq=@EngMainSeq
                    -- s20230524取消 and a.KgCo2e is not null and a.ItemKgCo2e is not null
                ) z
            "; //s20230528
            /*string sql =
                @"select
                    ROUND(sum(ISNULL(a.ItemKgCo2e,0)),0) Co2Total,
                    ROUND(sum(ISNULL(a.Quantity * a.Price, 0)), 0) Co2ItemTotal
                from SchEngProgressHeader b
                inner join SchEngProgressPayItem a on(a.SchEngProgressHeaderSeq=b.Seq)
                where b.EngMainSeq=@EngMainSeq
                and a.KgCo2e is not null and a.ItemKgCo2e is not null
                --and (a.RStatusCode>50 and a.RStatusCode<200 or a.RStatusCode=201)
                ";*/
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
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
        //工程進度-主檔
        public List<T> GetHeaderList<T>(int engMainSeq)
        {
            string sql = @"
                select
                    Seq,
                    EngMainSeq,
                    SPState,
                    PccesXMLFile,
                    PccesXMLDate
                from SchEngProgressHeader
                where EngMainSeq=@EngMainSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //碳排清單
        public List<T> GetCarbonEmissionList<T>(int engMainSeq)
        {
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
                    a.RStatusCode,
                    a.GreenFundingSeq,
                    a.GreenFundingMemo,
                    (select COUNT(seq) from CarbonEmissionWorkItem where CarbonEmissionPayItemSeq=a.Seq) WorkItemCnt
                from CarbonEmissionHeader b
                inner join CarbonEmissionPayItem a on(a.CarbonEmissionHeaderSeq=b.Seq)
                where b.EngMainSeq=@EngMainSeq
                Order by a.Seq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //碳排 WorkItem 清單
        public void GetCarbonEmissionWorkItemList(List<CarbonEmissionPayItemV2Model> items)
        {
            string sql = @"
                select
                    Seq,
                    CarbonEmissionPayItemSeq,
                    WorkItemQuantity,
                    ItemCode,
                    ItemKind,
                    Description,
                    Unit,
                    Quantity,
                    Price,
                    Amount,
                    Remark,
                    LabourRatio,
                    EquipmentRatio,
                    MaterialRatio,
                    MiscellaneaRatio
                from CarbonEmissionWorkItem
                where CarbonEmissionPayItemSeq=@CarbonEmissionPayItemSeq
                Order by Seq
             ";
            SqlCommand cmd;
            foreach (CarbonEmissionPayItemV2Model m in items) {
                if (m.WorkItemCnt > 0)
                {
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@CarbonEmissionPayItemSeq", m.Seq);
                    m.workItems = db.GetDataTableWithClass<CarbonEmissionWorkItemModel>(cmd);
                }
            }
        }
        //初始化碳排資料
        public bool InitData(int engMainSeq, List<CarbonEmissionPayItemV2Model> items)
        {
            SqlCommand cmd;
            string sql;
            int userSeq = getUserSeq();
            db.BeginTransaction();
            try
            {
                sql = @"
                    insert into SchEngProgressHeader (
                        EngMainSeq,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EngMainSeq,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('SchEngProgressHeader') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);
                int schEngProgressHeaderSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                int payItemOrderNo = 1;
                foreach (CarbonEmissionPayItemV2Model item in items)
                {
                    Null2Empty(item);
                    sql = @"
                        insert into SchEngProgressPayItem (
                            SchEngProgressHeaderSeq
                            ,PayItem,[Description],Unit,Quantity,Price,Amount,ItemKey,ItemNo,RefItemCode
                            ,RStatusCode,KgCo2e,ItemKgCo2e,Memo,RStatus,OrderNo,GreenFundingSeq,GreenFundingMemo
                            ,CreateTime,CreateUserSeq,ModifyTime,ModifyUserSeq
                        ) values (
                            @SchEngProgressHeaderSeq
                            ,@PayItem,@Description,@Unit,@Quantity,@Price,@Amount,@ItemKey,@ItemNo,@RefItemCode
                            ,@RStatusCode,@KgCo2e,@ItemKgCo2e,@Memo,@RStatus,@OrderNo,@GreenFundingSeq,@GreenFundingMemo
                            ,GetDate(),@ModifyUserSeq,GetDate(),@ModifyUserSeq
                        )
                        ";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@SchEngProgressHeaderSeq", schEngProgressHeaderSeq);
                    cmd.Parameters.AddWithValue("@PayItem", item.PayItem);
                    cmd.Parameters.AddWithValue("@Description", item.Description);
                    cmd.Parameters.AddWithValue("@Unit", item.Unit);
                    cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    cmd.Parameters.AddWithValue("@Price", item.Price);
                    cmd.Parameters.AddWithValue("@Amount", item.Amount);
                    cmd.Parameters.AddWithValue("@ItemKey", item.ItemKey);
                    cmd.Parameters.AddWithValue("@ItemNo", item.ItemNo);
                    cmd.Parameters.AddWithValue("@RefItemCode", item.RefItemCode);
                    cmd.Parameters.AddWithValue("@RStatusCode", item.RStatusCode);
                    cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(item.KgCo2e));
                    cmd.Parameters.AddWithValue("@ItemKgCo2e", this.NulltoDBNull(item.ItemKgCo2e));
                    cmd.Parameters.AddWithValue("@Memo", item.Memo);
                    cmd.Parameters.AddWithValue("@RStatus", item.RStatus);
                    cmd.Parameters.AddWithValue("@OrderNo", payItemOrderNo++);
                    cmd.Parameters.AddWithValue("@GreenFundingSeq", this.NulltoDBNull(item.GreenFundingSeq));//s20230418
                    cmd.Parameters.AddWithValue("@GreenFundingMemo", item.GreenFundingMemo);//s20230418
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);

                    cmd.Parameters.Clear();
                    sql1 = @" SELECT IDENT_CURRENT('SchEngProgressPayItem') AS NewSeq";
                    cmd = db.GetCommand(sql1);
                    dt = db.GetDataTable(cmd);
                    int payItemSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
                    if (item.WorkItemCnt > 0)
                    {
                        sql = @"
				        INSERT INTO SchEngProgressWorkItem (
                            SchEngProgressPayItemSeq, WorkItemQuantity, ItemCode, ItemKind, Description, Unit, Quantity,
                            Price, Amount, Remark, LabourRatio, EquipmentRatio, MaterialRatio, MiscellaneaRatio,OrderNo
                        )values(
                            @SchEngProgressPayItemSeq, @WorkItemQuantity, @ItemCode, @ItemKind, @Description, @Unit, @Quantity,
                            @Price, @Amount, @Remark, @LabourRatio, @EquipmentRatio, @MaterialRatio, @MiscellaneaRatio,@OrderNo
                        )";
                        int workItemOrderNo = 1;
                        foreach (WorkItemModel wi in item.workItems)
                        {
                            Null2Empty(wi);
                            cmd = db.GetCommand(sql);
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@SchEngProgressPayItemSeq", payItemSeq);
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
                            cmd.Parameters.AddWithValue("@OrderNo", workItemOrderNo++);
                            db.ExecuteNonQuery(cmd);
                        }
                    }
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchEngProgressService.InitData: " + e.Message);
                return false;
            }
        }
        
        //更新 PCCES.xml 20230417
        public int UpdatePCCES(EngMainModel engMainModel, List<PCCESPayItemModel> payItems, SchEngProgressHeaderModel headerModel, ref string errMsg)
        {
            return insertPCCES(engMainModel, payItems, headerModel, ref errMsg);
        }
        public int insertPCCES(EngMainModel engMainModel, List<PCCESPayItemModel> payItems, SchEngProgressHeaderModel headerModel, ref string errMsg)
        {
            string sql = "", sql1 = "";
            SqlCommand cmd;
            DataTable dt;
            int commandTimeout = 600; //s20230809

            db.BeginTransaction();
            try
            {
                sql = @"
                    DELETE SchEngProgressWorkItemDel where SchEngProgressPayItemSeq in(
                        select Seq from SchEngProgressPayItemDel where SchEngProgressHeaderSeq=@SchEngProgressHeaderSeq
                    ); --s20230721

                    DELETE SchEngProgressPayItemDel where SchEngProgressHeaderSeq=@SchEngProgressHeaderSeq; --s20230721

                    DELETE SchEngProgressWorkItem where SchEngProgressPayItemSeq in(
                        select Seq from SchEngProgressPayItem where SchEngProgressHeaderSeq=@SchEngProgressHeaderSeq
                    );

                    DELETE SchEngProgressPayItem where SchEngProgressHeaderSeq=@SchEngProgressHeaderSeq;
                ";
                cmd = db.GetCommand(sql);
                cmd.CommandTimeout = commandTimeout; //s20230809
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@SchEngProgressHeaderSeq", headerModel.Seq);
                db.ExecuteNonQuery(cmd);

                sql = @"update SchEngProgressHeader set
                            PccesXMLFile=@PccesXMLFile,
                            PccesXMLDate=GetDate(),
                            ModifyTime=GetDate(),
                            ModifyUserSeq=@ModifyUserSeq
                        where Seq=@Seq";
                cmd = db.GetCommand(sql);
                cmd.CommandTimeout = commandTimeout; //s20230809
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", headerModel.Seq);
                cmd.Parameters.AddWithValue("@PccesXMLFile", headerModel.PccesXMLFile);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                db.ExecuteNonQuery(cmd);

                int orderNo = 1;
                foreach (PCCESPayItemModel item in payItems)
                {
                    sql = @"
                        INSERT INTO SchEngProgressPayItem (
                            SchEngProgressHeaderSeq,
                            PayItem, Description, Unit, Quantity, Price, Amount, ItemKey, ItemNo, RefItemCode, OrderNo,
                            RStatusCode, CreateTime, CreateUserSeq, ModifyTime, ModifyUserSeq
                        )values(
                            @SchEngProgressHeaderSeq,
                            @PayItem, @Description, @Unit, @Quantity, @Price, @Amount, @ItemKey, @ItemNo, @RefItemCode, @OrderNo,
                            @RStatusCode, GetDate(), @ModifyUserSeq, GetDate(), @ModifyUserSeq
                        )";
                    cmd = db.GetCommand(sql);
                    cmd.CommandTimeout = commandTimeout; //s20230809
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@SchEngProgressHeaderSeq", headerModel.Seq);
                    cmd.Parameters.AddWithValue("@PayItem", item.PayItem);
                    cmd.Parameters.AddWithValue("@Description", item.Description);
                    cmd.Parameters.AddWithValue("@Unit", item.Unit);
                    cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    cmd.Parameters.AddWithValue("@Price", item.Price);
                    cmd.Parameters.AddWithValue("@Amount", item.Amount);
                    cmd.Parameters.AddWithValue("@ItemKey", item.ItemKey);
                    cmd.Parameters.AddWithValue("@ItemNo", item.ItemNo);
                    cmd.Parameters.AddWithValue("@OrderNo", orderNo++);
                    string refItemCode = item.RefItemCode == null ? "" : item.RefItemCode.Trim();
                    cmd.Parameters.AddWithValue("@RefItemCode", refItemCode);
                    int RStatusCode = CarbonEmissionPayItemService._None;//不須匹配
                    if (!String.IsNullOrEmpty(refItemCode))
                    {
                        if (refItemCode.Length < 10) RStatusCode = CarbonEmissionPayItemService._NotLongEnough;//不足10碼
                        else RStatusCode = CarbonEmissionPayItemService._Init;
                    }

                    cmd.Parameters.AddWithValue("@RStatusCode", RStatusCode);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                    db.ExecuteNonQuery(cmd);

                    cmd.Parameters.Clear();
                    sql1 = @" SELECT IDENT_CURRENT('SchEngProgressPayItem') AS NewSeq";
                    cmd = db.GetCommand(sql1);
                    dt = db.GetDataTable(cmd);
                    int payItemSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                    sql = @"
				        INSERT INTO SchEngProgressWorkItem (
                            SchEngProgressPayItemSeq, WorkItemQuantity, ItemCode, ItemKind, Description, Unit, Quantity, OrderNo,
                            Price, Amount, Remark, LabourRatio, EquipmentRatio, MaterialRatio, MiscellaneaRatio
                        )values(
                            @SchEngProgressPayItemSeq, @WorkItemQuantity, @ItemCode, @ItemKind, @Description, @Unit, @Quantity, @OrderNo,
                            @Price, @Amount, @Remark, @LabourRatio, @EquipmentRatio, @MaterialRatio, @MiscellaneaRatio
                        )";
                    int wInx = 1;
                    foreach (WorkItemModel wi in item.workItems)
                    {
                        Null2Empty(wi);
                        cmd = db.GetCommand(sql);
                        cmd.CommandTimeout = commandTimeout; //s20230809
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@SchEngProgressPayItemSeq", payItemSeq);
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
                        cmd.Parameters.AddWithValue("@OrderNo", wInx++);
                        db.ExecuteNonQuery(cmd);
                    }
                }

                //綠色經費 s20230418
                sql = @"
                    Update SchEngProgressPayItem set
	                    GreenFundingSeq=z.GreenFundingSeq
                    from (
                        select b.Seq SchEngProgressPayItemSeq, c.Seq GreenFundingSeq
                        from SchEngProgressHeader a
                        inner join SchEngProgressPayItem b on(b.SchEngProgressHeaderSeq=a.Seq)
                        left outer join GreenFunding c on( c.MatchCode like '%'+SUBSTRING(b.RefItemCode,len(b.RefItemCode)-9, 5)+'%')
                        where a.EngMainSeq=@EngMainSeq
                        and len(b.RefItemCode)>=10
                    ) z
                    where z.SchEngProgressPayItemSeq=SchEngProgressPayItem.Seq
                    ";
                cmd = db.GetCommand(sql);
                cmd.CommandTimeout = commandTimeout; //s20230809
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", engMainModel.Seq);
                db.ExecuteNonQuery(cmd);

                /*//取消更新 s20230528
                if (engMainModel.TotalBudget.HasValue)
                {
                    sql = @"update EngMain set
                            TotalBudget=@TotalBudget,
                            ModifyTime=GetDate(),
                            ModifyUserSeq=@ModifyUserSeq
                        where Seq=@Seq";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", engMainModel.Seq);
                    cmd.Parameters.AddWithValue("@TotalBudget", this.NulltoDBNull(engMainModel.TotalBudget));
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }*/

                db.TransactionCommit();
                return 1;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                errMsg = e.Message;
                log.Info("SchEngProgressService.UpdatePCCES: " + e.Message);
                //log.Info(sql);
                return -1;
            }
        }
    }
}