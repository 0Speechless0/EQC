using EQC.Common;
using EQC.EDMXModel;
using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace EQC.Services
{
    public class CarbonEmissionPayItemService : BaseService
    {//碳排量計算
        public const int _None = 300;               //300:無須匹配

        public const int _NotLongEnoughEdit = 201;  //200:不足10碼-修改 shioulo 20230204
        public const int _NotLongEnough = 200;      //200:不足10碼

        public const int _FullMatchEdit = 151;      //151:全匹配-修改 shioulo 20230111
        public const int _MatchEdit = 150;          //150:匹配-修改 shioulo 20230111
        public const int _NonTypeMatchEdit = 149;    //99:不分類匹配-修改 shioulo 20230204
        public const int _MatchC10_0Edit = 148;      //98:匹配 資料庫第10碼是0-修改 shioulo 20230204
        public const int _Tree0MatchEdit = 147;      //97:樹木類 末碼0-修改 shioulo 20230204

        public const int _FullMatch = 101;          //101:全匹配
        public const int _Match = 100;              //100:匹配
        public const int _NonTypeMatch = 99;        //99:不分類匹配
        public const int _MatchC10_0 = 98;          //98:匹配 資料庫第10碼是0
        public const int _Tree0Match = 97;          //97:樹木類 末碼0

        public const int _C10NotMatchUnitEdit = 56; //6:單位文字錯誤-修改 shioulo 20230204
        public const int _C10NotMatchReason = 55;   //52:第10碼不同(理由)
        public const int _NotMatchReason = 51;      //51:無匹配(理由)

        public const int _C10NotMatchUnit = 6;      //6:單位文字錯誤
        public const int _C10NotMatch = 5;          //5:第10碼不同
        public const int _NotMatch = 1;             //1:無匹配
        public const int _Init = 0;                 //0:未比對
        public bool unLockData(int engMainSeq)
        {
            using (var context = new EQC_NEW_Entities())
            {
                if ((context.EngMain.Find(engMainSeq)
                    .CarbonEmissionHeader.FirstOrDefault() is CarbonEmissionHeader target))
                {
                    target.State = 0;
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }
        public bool LockData(int engMainSeq)
        {
            using(var context = new EQC_NEW_Entities())
            {
                if ( (context.EngMain.Find(engMainSeq)
                    .CarbonEmissionHeader.FirstOrDefault() is CarbonEmissionHeader target)  )  
                {
                    target.State = 2;
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        //資料檢查
        public void CheckData(int engMainSeq, ref bool c10NotMatch, ref bool notMatch, ref bool notLongEnough, ref bool c10NotMatchUnit)
        {
            string sql =
                @"select distinct a.RStatusCode
                    from CarbonEmissionHeader b
                    inner join CarbonEmissionPayItem a on(a.CarbonEmissionHeaderSeq=b.Seq)
                    where b.EngMainSeq=@EngMainSeq
                    and (a.RStatusCode<50 or a.RStatusCode=200)";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            DataTable dt = db.GetDataTable(cmd);
            foreach(DataRow row in dt.Rows) {
                int code = Convert.ToInt32(row["RStatusCode"].ToString());
                if (code == _NotMatch)
                    notMatch = true;
                else if (code == _C10NotMatch)
                    c10NotMatch = true;
                else if (code == _NotLongEnough)
                    notLongEnough = true;
                else if (code == _C10NotMatchUnit)
                    c10NotMatchUnit = true;
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
                from CarbonEmissionHeader b
                inner join CarbonEmissionPayItem a on(a.CarbonEmissionHeaderSeq=b.Seq)
                where b.EngMainSeq=@EngMainSeq
                and a.KgCo2e is not null and a.ItemKgCo2e is not null

                union all
                
                select
                    ROUND(sum(ISNULL(a.Amount, 0)), 0) GreenFunding,
                    0 Co2Total,
                    0 Co2ItemTotal
                from CarbonEmissionHeader b
                inner join CarbonEmissionPayItem a on(a.CarbonEmissionHeaderSeq=b.Seq and a.GreenFundingSeq is not null)
                where b.EngMainSeq=@EngMainSeq
                -- s20230524取消 and a.KgCo2e is not null and a.ItemKgCo2e is not null
                ) z
            ";
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
                //Co2ItemTotal = Convert.ToDecimal(dt.Rows[0]["Co2ItemTotal"].ToString());
                Co2ItemTotal = 0;
            }
            catch { }
            try
            {
                GreenFunding = Convert.ToDecimal(dt.Rows[0]["GreenFunding"].ToString());
            }
            catch { }
        }
        //
        public int Update(CarbonEmissionPayItemModel m)
        {
            Null2Empty(m);
            try
            {
                string sql = @"update CarbonEmissionPayItem set
                    KgCo2e=@KgCo2e,
                    ItemKgCo2e=IIF(@KgCo2e is null, null, @KgCo2e*Quantity),
                    Memo=@Memo,
                    RStatusCode=@RStatusCode,
                    GreenFundingSeq=@GreenFundingSeq,
                    GreenFundingMemo=@GreenFundingMemo,
                    Suggestion = @Suggestion,
                    ModifyTime=GetDate(),
                    ModifyUserSeq=@ModifyUserSeq
                where Seq=@Seq";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(m.KgCo2e));
                cmd.Parameters.AddWithValue("@Memo", m.Memo);
                cmd.Parameters.AddWithValue("@Suggestion", m.Suggestion);
                cmd.Parameters.AddWithValue("@GreenFundingSeq", this.NulltoDBNull(m.GreenFundingSeq));//s20230418
                cmd.Parameters.AddWithValue("@GreenFundingMemo", m.GreenFundingMemo);//s20230418
                cmd.Parameters.AddWithValue("@RStatusCode", m.RStatusCode);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("CarbonEmissionPayItemService.Update: " + e.Message);
                //log.Info(sql);
                return -1;
            }
        }


        public T CompareCalCarbonEmissions<T>(string code, string unit) where T : EQMCalCarbonEmissionsVModel
        {
            string sql = @"
                Declare @item as Table
                (
                    RefItemCode varchar(20),
                    Unit varchar(10)
                )
                INSERT INTO @item(RefItemCode, Unit)
                select @Code, @Unit;
                select z.RStatusCode, z.RefItemCode, z.Code, z.KgCo2e ResultKgCo2e, z.Memo Memo, z.Unit ResultUnit, z.Item Description
                    ---,z.KeyCode1, a.Description, z.item
                from ( 
                    --shioulo 20230530
                    select " + _C10NotMatchUnit + @" RStatusCode, a.RefItemCode, null Code, null KgCo2e, null KeyCode1, null Memo, null Unit, null Item
                    from @item a
                    where a.RefItemCode like 'L%' 
                    and case (
						a.Unit
					)
					    when '時' THEN  '1'
					    when '工' THEN  '2'
					    when '月' THEN  '3'
					    when '式' THEN  '4'
					    when '年' THEN  '5'
                        ELSE '-1' END
				        != SUBSTRING(a.RefItemCode,len(a.RefItemCode), 1)
                    and SUBSTRING(a.RefItemCode,len(a.RefItemCode), 1) in ('1','2','3','4','5')

                    union all
                    --shioulo 20221228
                    select " + _C10NotMatchUnit + @" RStatusCode, a.RefItemCode, null Code, null KgCo2e, null KeyCode1, null Memo, null Unit, null Item
                    from @item a
                    where a.RefItemCode like 'E%' 
                    and case (
						a.Unit
					)
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
                    select " + _C10NotMatchUnit + @" RStatusCode, a.RefItemCode, null Code, null KgCo2e, null KeyCode1, null Memo, null Unit, null Item
                    from @item a
                    where a.RefItemCode not like 'E%' --shioulo 20221228
                    and case (
						a.Unit
					)
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

                    select " + _FullMatch + @" RStatusCode, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo, x1.Unit Unit, x1.Item Item
                    from @item a
                    inner join CarbonEmissionFactor x1 on (a.RefItemCode=x1.Code )
                    --and len(a.RefItemCode)>9

                    union all

                    select " + _Match + @" RStatusCode, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo, x1.Unit Unit, x1.Item Item
                    from @item a
                    inner join CarbonEmissionFactor x1 on (SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 10) like x1.KeyCode1 )
                    --and len(a.RefItemCode)>9

                    union all

                    select " + _MatchC10_0 + @" RStatusCode, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo, x1.Unit Unit, x1.Item Item
                    from @item a
                    inner join (
                    	select * from CarbonEmissionFactor 
                        where KeyCode1 like '%0' and KeyCode2 <> '-1' /*and KeyCode3 = '-1'*/) x1 on (SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 10) like x1.KeyCode2 )
                    where SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 5) in ('02931','02932') -- shioulo 20230111

                    union all -- shioulo 20230111
                    select " + _Tree0Match + @" RStatusCode, a.RefItemCode, '' Code, 0 KgCo2e, '' KeyCode1, '歸類到樹木類' Memo, '' Unit, '末碼0' Item
                    from @item a
                    where SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 5) in ('02931','02932')
                    and a.RefItemCode like '%0'

                    union all -- shioulo 20230107
                    select " + _C10NotMatch + @" RStatusCode, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo, x1.Unit Unit, x1.Item Item
                    from @item a
                    inner join CarbonEmissionFactor x1 on (SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 10) like x1.KeyCode2 )

                    union all

                    select 99 RStatusCode,a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo, x1.Unit Unit, x1.Item Item
                    from @item a
                    inner join CarbonEmissionFactor x1 on (SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 10) like x1.KeyCode3 )
                    --and len(a.RefItemCode)>9

                    union all 

                    select " + _NotMatch + @" RStatusCode, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo, x1.Unit Unit, x1.Item Item
                    from @item a
                    left outer join CarbonEmissionFactor x1 on (a.RefItemCode=x1.Code )

                    --and len(a.RefItemCode)>9
                ) z ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Code", code);
            cmd.Parameters.AddWithValue("@Unit", unit);
            return db.GetDataTableWithClass<T>(cmd).FirstOrDefault();

        }
        
        //計算碳排量
        public bool CalCarbonEmissions(int engMainSeq)
        {
            string sql = @"
                select z.RStatusCode, z.Seq, z.RefItemCode, z.Code, z.KgCo2e, z.Memo RStatus
                    ---,z.KeyCode1, a.Description, z.item
                from ( 
                    --shioulo 20230202
                    select " + _C10NotMatchUnit + @" RStatusCode,a.Seq, a.RefItemCode, null Code, null KgCo2e, null KeyCode1, null Memo
                    from CarbonEmissionHeader b
                    inner join CarbonEmissionPayItem a on(a.CarbonEmissionHeaderSeq=b.Seq)
                    where b.EngMainSeq=@EngMainSeq
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
                    from CarbonEmissionHeader b
                    inner join CarbonEmissionPayItem a on(a.CarbonEmissionHeaderSeq=b.Seq)
                    where b.EngMainSeq=@EngMainSeq
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
                    from CarbonEmissionHeader b
                    inner join CarbonEmissionPayItem a on(a.CarbonEmissionHeaderSeq=b.Seq)
                    where b.EngMainSeq=@EngMainSeq
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
                    from CarbonEmissionHeader b
                    inner join CarbonEmissionPayItem a on(a.CarbonEmissionHeaderSeq=b.Seq) 
                    inner join CarbonEmissionFactor x1 on (a.RefItemCode=x1.Code )
                    where b.EngMainSeq=@EngMainSeq
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)

                    union all

                    select " + _Match + @" RStatusCode,a.Seq, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo
                    from CarbonEmissionHeader b
                    inner join CarbonEmissionPayItem a on(a.CarbonEmissionHeaderSeq=b.Seq)
                    inner join CarbonEmissionFactor x1 on (SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 10) like x1.KeyCode1 )
                    where b.EngMainSeq=@EngMainSeq
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)

                    union all
                    select " + _MatchC10_0 + @" RStatusCode,a.Seq, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo
                    from CarbonEmissionHeader b
                    inner join CarbonEmissionPayItem a on(a.CarbonEmissionHeaderSeq=b.Seq)
                    inner join (
                    	select * from CarbonEmissionFactor 
                        where KeyCode1 like '%0' and KeyCode2 <> '-1' /*and KeyCode3 = '-1'*/) x1 on (SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 10) like x1.KeyCode2 )
                    where b.EngMainSeq=@EngMainSeq
                    and SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 5) in ('02931','02932') -- shioulo 20230111
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)

                    union all -- shioulo 20230111
                    select " + _Tree0Match + @" RStatusCode,a.Seq, a.RefItemCode, '末碼0' Code, 0 KgCo2e, '_________0' KeyCode1, '歸類到樹木類' Memo
                    from CarbonEmissionHeader b
                    inner join CarbonEmissionPayItem a on(a.CarbonEmissionHeaderSeq=b.Seq)
                    --inner join CarbonEmissionFactor x1 on (SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 10) like x1.KeyCode2 )
                    where b.EngMainSeq=@EngMainSeq
                    and SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 5) in ('02931','02932')
                    and a.RefItemCode like '%0'
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)

                    union all -- shioulo 20230107
                    select " + _C10NotMatch + @" RStatusCode,a.Seq, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo
                    from CarbonEmissionHeader b
                    inner join CarbonEmissionPayItem a on(a.CarbonEmissionHeaderSeq=b.Seq)
                    inner join CarbonEmissionFactor x1 on (SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 10) like x1.KeyCode2 )
                    where b.EngMainSeq=@EngMainSeq
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)

                    union all

                    select 2 RStatusCode,a.Seq, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo
                    from CarbonEmissionHeader b
                    inner join CarbonEmissionPayItem a on(a.CarbonEmissionHeaderSeq=b.Seq)
                    inner join CarbonEmissionFactor x1 on (SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 10) like x1.KeyCode3 )
                    where b.EngMainSeq=@EngMainSeq
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)

                    union all 

                    select " + _NotMatch + @" RStatusCode,a.Seq, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo
                    from CarbonEmissionHeader b
                    inner join CarbonEmissionPayItem a on(a.CarbonEmissionHeaderSeq=b.Seq)
                    left outer join CarbonEmissionFactor x1 on (a.RefItemCode=x1.Code )
                    where b.EngMainSeq=@EngMainSeq
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)
                ) z";
            SqlCommand cmd = db.GetCommand(sql);
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            List<EQMCalCarbonEmissionsVModel> list = db.GetDataTableWithClass<EQMCalCarbonEmissionsVModel>(cmd);

            Dictionary<int, string> updateItem = new Dictionary<int, string>();

            db.BeginTransaction();
            try
            {
                foreach (EQMCalCarbonEmissionsVModel m in list)
                {
                    if(!updateItem.ContainsKey(m.Seq))
                    {
                        Null2Empty(m);
                        if (m.RStatusCode == _Match || m.RStatusCode == _FullMatch || (m.RStatusCode == 2) || m.RStatusCode == _MatchC10_0 || m.RStatusCode == _Tree0Match)
                        {
                            sql = @"update CarbonEmissionPayItem set
                                KgCo2e=@KgCo2e,
                                ItemKgCo2e=@KgCo2e*Quantity,
                                Memo=@Memo,
                                RStatus=@RStatus,
                                RStatusCode=@RStatusCode,
                                ModifyTime=GetDate(),
                                ModifyUserSeq=@ModifyUserSeq
                            where Seq=@Seq";
                        } else
                        {
                            sql = @"update CarbonEmissionPayItem set
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
                        cmd.Parameters.AddWithValue("@RStatusCode", m.RStatusCode==2 ? _NonTypeMatch : m.RStatusCode);
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
                log.Info("CarbonEmissionPayItemService.CalCarbonEmissions: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }
        //計算碳排量 進度管理-前置作業 s20230417
        public bool CalCarbonEmissionsForSchEngProgress(int engMainSeq)
        {
            string sql = @"
                select z.RStatusCode, z.Seq, z.RefItemCode, z.Code, z.KgCo2e, z.Memo RStatus
                    ---,z.KeyCode1, a.Description, z.item
                from ( 
                    --shioulo 20230202
                    select " + _C10NotMatchUnit + @" RStatusCode,a.Seq, a.RefItemCode, null Code, null KgCo2e, null KeyCode1, null Memo
                    from SchEngProgressHeader b
                    inner join SchEngProgressPayItem a on(a.SchEngProgressHeaderSeq=b.Seq)
                    where b.EngMainSeq=@EngMainSeq
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
                    from SchEngProgressHeader b
                    inner join SchEngProgressPayItem a on(a.SchEngProgressHeaderSeq=b.Seq)
                    where b.EngMainSeq=@EngMainSeq
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
                    from SchEngProgressHeader b
                    inner join SchEngProgressPayItem a on(a.SchEngProgressHeaderSeq=b.Seq)
                    where b.EngMainSeq=@EngMainSeq
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
                    from SchEngProgressHeader b
                    inner join SchEngProgressPayItem a on(a.SchEngProgressHeaderSeq=b.Seq) 
                    inner join CarbonEmissionFactor x1 on (a.RefItemCode=x1.Code )
                    where b.EngMainSeq=@EngMainSeq
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)

                    union all

                    select " + _Match + @" RStatusCode,a.Seq, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo
                    from SchEngProgressHeader b
                    inner join SchEngProgressPayItem a on(a.SchEngProgressHeaderSeq=b.Seq)
                    inner join CarbonEmissionFactor x1 on (SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 10) like x1.KeyCode1 )
                    where b.EngMainSeq=@EngMainSeq
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)

                    union all
                    select " + _MatchC10_0 + @" RStatusCode,a.Seq, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo
                    from SchEngProgressHeader b
                    inner join SchEngProgressPayItem a on(a.SchEngProgressHeaderSeq=b.Seq)
                    inner join (
                    	select * from CarbonEmissionFactor 
                        where KeyCode1 like '%0' and KeyCode2 <> '-1' /*and KeyCode3 = '-1'*/) x1 on (SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 10) like x1.KeyCode2 )
                    where b.EngMainSeq=@EngMainSeq
                    and SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 5) in ('02931','02932') -- shioulo 20230111
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)

                    union all -- shioulo 20230111
                    select " + _Tree0Match + @" RStatusCode,a.Seq, a.RefItemCode, '末碼0' Code, 0 KgCo2e, '_________0' KeyCode1, '歸類到樹木類' Memo
                    from SchEngProgressHeader b
                    inner join SchEngProgressPayItem a on(a.SchEngProgressHeaderSeq=b.Seq)
                    --inner join CarbonEmissionFactor x1 on (SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 10) like x1.KeyCode2 )
                    where b.EngMainSeq=@EngMainSeq
                    and SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 5) in ('02931','02932')
                    and a.RefItemCode like '%0'
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)

                    union all -- shioulo 20230107
                    select " + _C10NotMatch + @" RStatusCode,a.Seq, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo
                    from SchEngProgressHeader b
                    inner join SchEngProgressPayItem a on(a.SchEngProgressHeaderSeq=b.Seq)
                    inner join CarbonEmissionFactor x1 on (SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 10) like x1.KeyCode2 )
                    where b.EngMainSeq=@EngMainSeq
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)

                    union all

                    select 2 RStatusCode,a.Seq, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo
                    from SchEngProgressHeader b
                    inner join SchEngProgressPayItem a on(a.SchEngProgressHeaderSeq=b.Seq)
                    inner join CarbonEmissionFactor x1 on (SUBSTRING(a.RefItemCode,len(a.RefItemCode)-9, 10) like x1.KeyCode3 )
                    where b.EngMainSeq=@EngMainSeq
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)

                    union all 

                    select " + _NotMatch + @" RStatusCode,a.Seq, a.RefItemCode, x1.Code, x1.KgCo2e, x1.KeyCode1, x1.Memo
                    from SchEngProgressHeader b
                    inner join SchEngProgressPayItem a on(a.SchEngProgressHeaderSeq=b.Seq)
                    left outer join CarbonEmissionFactor x1 on (a.RefItemCode=x1.Code )
                    where b.EngMainSeq=@EngMainSeq
                    and a.RStatusCode not in(51,55,56,147,148,149,150,151,200,201,300)
                ) z";
            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

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
                            sql = @"update SchEngProgressPayItem set
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
                            sql = @"update SchEngProgressPayItem set
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
                log.Info("SchEngProgressPayItemService.CalSchEngProgresssForSchEngProgress: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }
        //Copy PCCESPayItem to CarbonEmissionPayItem
        public bool CreatePayItems(int engMainSeq)
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
                        and b.contractNo=(select c.EngNo from EngMain c where c.Seq=@EngMainSeq)
                    )
                    ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            List<CarbonEmissionPayItemInsertModel> pccesPayItemModel = db.GetDataTableWithClass<CarbonEmissionPayItemInsertModel>(cmd);
            if (pccesPayItemModel.Count == 0) return false;

            sql = @"select * from PCCESWorkItem where PCCESPayItemSeq=@PCCESPayItemSeq";
            foreach (CarbonEmissionPayItemInsertModel m in pccesPayItemModel)
            {
                Null2Empty(m);
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PCCESPayItemSeq", m.Seq);
                m.workItems = db.GetDataTableWithClass<CarbonEmissionWorkItemModel>(cmd);
            }

            //string engFolder = Utils.GetEngMainFolder(engMainSeq);
            //string xmlFileName = new System.IO.DirectoryInfo(engFolder)
            //    .GetFiles()
            //    .OrderByDescending(row => row.CreationTime).FirstOrDefault()?.Name ?? "";
            db.BeginTransaction();
            try {
                //s20231109
                sql = @"
                    delete from CarbonEmissionHeader where EngMainSeq=@EngMainSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                db.ExecuteNonQuery(cmd);

                sql = @"
                    insert into CarbonEmissionHeader (
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
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('CarbonEmissionHeader') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);
                int carbonEmissionHeaderSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                //shioulo 20221006
                foreach (CarbonEmissionPayItemInsertModel item in pccesPayItemModel)
                {
                    sql = @"
                        insert into CarbonEmissionPayItem (
                            CarbonEmissionHeaderSeq
                            ,PayItem,[Description],Unit,Quantity,Price,Amount,ItemKey,ItemNo,RefItemCode
                            ,RStatusCode
                            ,CreateTime,CreateUserSeq,ModifyTime,ModifyUserSeq
                        ) values (
                            @CarbonEmissionHeaderSeq
                            ,@PayItem,@Description,@Unit,@Quantity,@Price,@Amount,@ItemKey,@ItemNo,@RefItemCode
                            ,@RStatusCode
                            ,GetDate(),@ModifyUserSeq,GetDate(),@ModifyUserSeq
                        )
                        ";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@CarbonEmissionHeaderSeq", carbonEmissionHeaderSeq);
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
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);

                    cmd.Parameters.Clear();
                    sql1 = @" SELECT IDENT_CURRENT('CarbonEmissionPayItem') AS NewSeq";
                    cmd = db.GetCommand(sql1);
                    dt = db.GetDataTable(cmd);
                    int payItemSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                    sql = @"
				        INSERT INTO CarbonEmissionWorkItem (
                            CarbonEmissionPayItemSeq, WorkItemQuantity, ItemCode, ItemKind, Description, Unit, Quantity,
                            Price, Amount, Remark, LabourRatio, EquipmentRatio, MaterialRatio, MiscellaneaRatio
                        )values(
                            @CarbonEmissionPayItemSeq, @WorkItemQuantity, @ItemCode, @ItemKind, @Description, @Unit, @Quantity,
                            @Price, @Amount, @Remark, @LabourRatio, @EquipmentRatio, @MaterialRatio, @MiscellaneaRatio
                        )";
                    foreach (WorkItemModel wi in item.workItems)
                    {
                        Null2Empty(wi);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@CarbonEmissionPayItemSeq", payItemSeq);
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

                //綠色經費 s20230421
                sql = @"
                    Update CarbonEmissionPayItem set
	                    GreenFundingSeq=z.GreenFundingSeq
                    from (
                        select b.Seq CarbonEmissionPayItemSeq, c.Seq GreenFundingSeq
                        from CarbonEmissionHeader a
                        inner join CarbonEmissionPayItem b on(b.CarbonEmissionHeaderSeq=a.Seq)
                        left outer join GreenFunding c on( c.MatchCode like '%'+SUBSTRING(b.RefItemCode,len(b.RefItemCode)-9, 5)+'%')
                        where a.EngMainSeq=@EngMainSeq
                        and len(b.RefItemCode)>=10
                    ) z
                    where z.CarbonEmissionPayItemSeq=CarbonEmissionPayItem.Seq
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("CarbonEmissionPayItemService.CreatePayItems: " + e.Message);
                return false;
            }
        }
        //清單總筆數
        public int GetListTotal(int engMainSeq, string keyWord)
        {
            if (!String.IsNullOrEmpty(keyWord)) keyWord += "%";
            string sql = @"
                select
                    count(a.Seq) total
                from CarbonEmissionHeader b
                inner join CarbonEmissionPayItem a on(a.CarbonEmissionHeaderSeq=b.Seq)
                where b.EngMainSeq=@EngMainSeq
                and (@keyWord='' or a.PayItem like @keyWord)
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@keyWord", keyWord);

            DataTable dt = db.GetDataTable(cmd);
            int cnt = Convert.ToInt32(dt.Rows[0]["total"].ToString());

            return cnt;
        }
        public List<CarbonEmissionPayItemModel> GetPayItemByHeaderSeq(int headerSeq)
        {
            string sql = @"select * from CarbonEmissionPayItem a where a.CarbonEmissionHeaderSeq = @headerSeq";
            var cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@headerSeq", headerSeq);
            return db.GetDataTableWithClass<CarbonEmissionPayItemModel>(cmd);
        }
        //清單
        public List<T> GetList<T>(int engMainSeq, int pageRecordCount, int pageIndex, string keyWord)
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
                    a.RStatusCode,
                    a.Suggestion,
                    a.GreenFundingSeq,
                    a.GreenFundingMemo
                from CarbonEmissionHeader b
                inner join CarbonEmissionPayItem a on(a.CarbonEmissionHeaderSeq=b.Seq and (@keyWord='' or a.PayItem like @keyWord))
                where b.EngMainSeq=@EngMainSeq
                Order by a.Seq
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@keyWord", keyWord);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);

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
                from CarbonEmissionHeader b
                inner join CarbonEmissionPayItem a on(a.CarbonEmissionHeaderSeq=b.Seq)
                where b.EngMainSeq=@EngMainSeq
                and LEN(a.PayItem)=1
                order by a.Seq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //
        public List<T> GetHeaderList<T>(int engMainSeq)
        {
            string sql = @"
                select
                    Seq,
                    EngMainSeq,
                    State,
                    PccesXMLFile,
                    PccesXMLDate
                from CarbonEmissionHeader
                where EngMainSeq=@EngMainSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //取得工程
        public List<T> GetEng<T>(int seq)
        {
            string sql = @"SELECT
                    a.EngNo,
                    a.EngName,
                    a.TotalBudget,
                    a.ApprovedCarbonQuantity,
                    a.SubContractingBudget,
                    c1.Name ExecUnitName,
                    (d1.CityName +d.TownName) EngPlace,
                    (
                        select ROUND(sum(ISNULL(b1.ItemKgCo2e,0)),0) Co2Total
                        from CarbonEmissionHeader b
                        inner join CarbonEmissionPayItem b1 on(b1.CarbonEmissionHeaderSeq=b.Seq)
                        where b.EngMainSeq=a.Seq
                        and b1.KgCo2e is not null and b1.ItemKgCo2e is not null --s20230310
                        --and (b1.RStatusCode>50 and b1.RStatusCode<200 or b1.RStatusCode=201)
                    ) Co2Total,
                    (
                        select ROUND(sum(ISNULL(b1.Quantity * b1.Price, 0)), 0) Co2ItemTotal
                        from CarbonEmissionHeader b
                        inner join CarbonEmissionPayItem b1 on(b1.CarbonEmissionHeaderSeq=b.Seq)
                        where b.EngMainSeq=a.Seq
                        and b1.KgCo2e is not null and b1.ItemKgCo2e is not null --s20230310
                        --and (b1.RStatusCode>50 and b1.RStatusCode<200 or b1.RStatusCode=201)
                    ) Co2ItemTotal
                    
                    ---c3.ExecUnitName,
                    ---c3.OrganizerName,
                    ---c3.ContactName,
                    ---c3.ContactPhone,
                    ---c2.Name ExecSubUnitName
                FROM EngMain a
                left outer join Town d on(d.Seq=a.EngTownSeq)
                left outer join City d1 on(d1.Seq=d.CitySeq)
                left outer join Unit c1 on(c1.Seq=a.ExecUnitSeq)
                ---left outer join Unit c2 on(c2.Seq=a.ExecSubUnitSeq)
                -- left outer join PrjXML c3 on(c.Seq=a.PrjXMLSeq)"
                + @"
                where a.Seq=@Seq
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        //更新 PCCES.xml
        public int UpdatePCCES(EngMainModel engMainModel, List<PCCESPayItemModel> payItems, CarbonEmissionHeaderModel headerModel, ref string errMsg)
        {
            return insertPCCES(engMainModel, payItems, headerModel, ref errMsg);
        }
        public int insertPCCES(EngMainModel engMainModel, List<PCCESPayItemModel> payItems, CarbonEmissionHeaderModel headerModel, ref string errMsg)
        {
            string sql = "", sql1 = "";
            SqlCommand cmd;
            DataTable dt;

            db.BeginTransaction();
            try
            {
                sql = @"
                    DELETE CarbonEmissionWorkItem where CarbonEmissionPayItemSeq in(
                        select Seq from CarbonEmissionPayItem where CarbonEmissionHeaderSeq=@CarbonEmissionHeaderSeq
                    );
                    DELETE CarbonEmissionPayItem where CarbonEmissionHeaderSeq=@CarbonEmissionHeaderSeq;
                ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CarbonEmissionHeaderSeq", headerModel.Seq);
                db.ExecuteNonQuery(cmd);

                sql = @"update CarbonEmissionHeader set
                            PccesXMLFile=@PccesXMLFile,
                            PccesXMLDate=GetDate(),
                            ModifyTime=GetDate(),
                            ModifyUserSeq=@ModifyUserSeq
                        where Seq=@Seq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", headerModel.Seq);
                cmd.Parameters.AddWithValue("@PccesXMLFile", headerModel.PccesXMLFile);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                db.ExecuteNonQuery(cmd);
                
                foreach (PCCESPayItemModel item in payItems)
                {
                    sql = @"
                        INSERT INTO CarbonEmissionPayItem (
                            CarbonEmissionHeaderSeq,
                            PayItem, Description, Unit, Quantity, Price, Amount, ItemKey, ItemNo, RefItemCode,
                            RStatusCode, CreateTime, CreateUserSeq, ModifyTime, ModifyUserSeq
                        )values(
                            @CarbonEmissionHeaderSeq,
                            @PayItem, @Description, @Unit, @Quantity, @Price, @Amount, @ItemKey, @ItemNo, @RefItemCode,
                            @RStatusCode, GetDate(), @ModifyUserSeq, GetDate(), @ModifyUserSeq
                        )";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@CarbonEmissionHeaderSeq", headerModel.Seq);
                    cmd.Parameters.AddWithValue("@PayItem", item.PayItem);
                    cmd.Parameters.AddWithValue("@Description", item.Description);
                    cmd.Parameters.AddWithValue("@Unit", item.Unit);
                    cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    cmd.Parameters.AddWithValue("@Price", item.Price);
                    cmd.Parameters.AddWithValue("@Amount", item.Amount);
                    cmd.Parameters.AddWithValue("@ItemKey", item.ItemKey);
                    cmd.Parameters.AddWithValue("@ItemNo", item.ItemNo);
                    string refItemCode = item.RefItemCode==null ? "" : item.RefItemCode.Trim();
                    cmd.Parameters.AddWithValue("@RefItemCode", refItemCode);
                    int RStatusCode = _None;//不須匹配
                    if(!String.IsNullOrEmpty(refItemCode))
                    {
                        if (refItemCode.Length < 10) RStatusCode = _NotLongEnough;//不足10碼
                        else RStatusCode = _Init;
                    }

                    cmd.Parameters.AddWithValue("@RStatusCode", RStatusCode);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                    db.ExecuteNonQuery(cmd);

                    cmd.Parameters.Clear();
                    sql1 = @" SELECT IDENT_CURRENT('CarbonEmissionPayItem') AS NewSeq";
                    cmd = db.GetCommand(sql1);
                    dt = db.GetDataTable(cmd);
                    int payItemSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                    sql = @"
				        INSERT INTO CarbonEmissionWorkItem (
                            CarbonEmissionPayItemSeq, WorkItemQuantity, ItemCode, ItemKind, Description, Unit, Quantity,
                            Price, Amount, Remark, LabourRatio, EquipmentRatio, MaterialRatio, MiscellaneaRatio
                        )values(
                            @CarbonEmissionPayItemSeq, @WorkItemQuantity, @ItemCode, @ItemKind, @Description, @Unit, @Quantity,
                            @Price, @Amount, @Remark, @LabourRatio, @EquipmentRatio, @MaterialRatio, @MiscellaneaRatio
                        )";
                    foreach (WorkItemModel wi in item.workItems)
                    {
                        Null2Empty(wi);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@CarbonEmissionPayItemSeq", payItemSeq);
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
                //綠色經費 s20230418
                sql = @"
                    Update CarbonEmissionPayItem set
	                    GreenFundingSeq=z.GreenFundingSeq
                    from (
                        select b.Seq CarbonEmissionPayItemSeq, c.Seq GreenFundingSeq
                        from CarbonEmissionHeader a
                        inner join CarbonEmissionPayItem b on(b.CarbonEmissionHeaderSeq=a.Seq)
                        left outer join GreenFunding c on( c.MatchCode like '%'+SUBSTRING(b.RefItemCode,len(b.RefItemCode)-9, 5)+'%')
                        where a.EngMainSeq=@EngMainSeq
                        and len(b.RefItemCode)>=10
                    ) z
                    where z.CarbonEmissionPayItemSeq=CarbonEmissionPayItem.Seq
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", engMainModel.Seq);
                db.ExecuteNonQuery(cmd);

                if (engMainModel.TotalBudget.HasValue)
                {//s20230410
                    sql = @"update EngMain set
                            TotalBudget=@TotalBudget,
                            SubContractingBudget=@TotalBudget, --s20230620
                            ModifyTime=GetDate(),
                            ModifyUserSeq=@ModifyUserSeq
                        where Seq=@Seq";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", engMainModel.Seq);
                    cmd.Parameters.AddWithValue("@TotalBudget", this.NulltoDBNull(engMainModel.TotalBudget));
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return 1;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                errMsg = e.Message;
                log.Info("CarbonEmissionPayItemService.UpdatePCCES: " + e.Message);
                //log.Info(sql);
                return -1;
            }
        }
        //
        //綠色經費 s20230418
        public List<T> GreenFundingList<T>()
        {
            string sql = @"
                select
                    Cast(Seq as VarChar) Value,
                    ItemName Text
                from GreenFunding 
                order by OrderNo
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();

            return db.GetDataTableWithClass<T>(cmd);
        }
        //可交易碳工程清單 s20230419
        public List<T> GetTradeList<T>(int engMainSeq)
        {
            string sql = @"
                select *
                    from (
                        select 
                            0 mode,
                            ISNULL(c1.Seq, -1) Seq,
                            a.EngMainSeq,
                            b.EngNo,
                            b.EngName,
                            b1.Name ExecUnitName,
                            b.ApprovedCarbonQuantity,
                            b.CarbonDesignQuantity,
                            ISNULL((
                                select sum(z1.Quantity) from (
                                    select ISNULL(Quantity,0) Quantity from CarbonEmissionTrading z
                                    inner join CarbonEmissionHeader z1 on(z1.Seq=z.CarbonEmissionHeaderSeq and z1.State=0)
                                    where z.EngMainSeq=a.EngMainSeq
                                    union all
                                    select ISNULL(Quantity,0) Quantity from CarbonEmissionTradingAdj z
                                    inner join CarbonEmissionHeader z1 on(z1.Seq=z.CarbonEmissionHeaderSeq and z1.AdjState=0)
                                    where z.EngMainSeq=a.EngMainSeq
                                    and z.Quantity > z.SrcQuantity
                                ) z1
                            ),0) + CarbonTradedQuantity - ISNULL(c1.Quantity,0) TradingTotalQuantity,
                            c1.Quantity
                        from CarbonEmissionHeader a
                        inner join EngMain b on(b.Seq=a.EngMainSeq
                            and b.EngYear in (select EngYear from EngMain where Seq=@EngMainSeq)
                            and b.ExecUnitSeq in (select ExecUnitSeq from EngMain where Seq=@EngMainSeq)
                        )
                        inner join Unit b1 on(b1.Seq=b.ExecUnitSeq)
                        left outer join CarbonEmissionHeader c on(c.EngMainSeq=@EngMainSeq)
                        left outer join CarbonEmissionTrading c1 on(c1.CarbonEmissionHeaderSeq=c.Seq and c1.EngMainSeq=a.EngMainSeq)
                        where a.State=1
                        and b.ApprovedCarbonQuantity > b.CarbonDesignQuantity

                        union all --s20230524
                        select
                            1 mode,
                            -2 Seq,
                            a.EngMainSeq,
                            b.EngNo,
                            b.EngName,
                            b1.Name ExecUnitName,
                            b.ApprovedCarbonQuantity,
                            b.CarbonDesignQuantity,
                            0 TradingTotalQuantity,
                            null Quantity
                        from CarbonEmissionHeader a
                        inner join EngMain b on(b.Seq=a.EngMainSeq
                            and b.EngYear in (select EngYear from EngMain where Seq=@EngMainSeq)
                            and b.ExecUnitSeq in (select ExecUnitSeq from EngMain where Seq=@EngMainSeq)
                        )
                        inner join Unit b1 on(b1.Seq=b.ExecUnitSeq)
                        where a.State<>1
                        and a.EngMainSeq<>@EngMainSeq
                    ) z
                    order by z.mode, z.Quantity desc, z.EngNo
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        //s20230419
        public List<T> GetTradeHeaders<T>(int engMainSeq)
        {
            string sql = @"
                select
                    a.Seq,
                    a.EngMainSeq,
                    a.State,
                    a.AdjState,
                    a.CarbonTradingDesc
                from CarbonEmissionHeader a
                where a.EngMainSeq=@EngMainSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetTradeHeadersAndDoc<T>(int engMainSeq)
        {
            string sql = @"
                select
                    a.Seq,
                    a.EngMainSeq,
                    a.State,
                    a.CarbonTradingDesc,
                    b.CarbonTradingApprovedDate,
  	                b.CarbonTradingNo
                from CarbonEmissionHeader a
                left outer join (
	                select top 1 z1.CarbonTradingNo, z1.CarbonTradingApprovedDate from CarbonEmissionHeader z
                    inner join CarbonEmissionTradingDoc z1 on(z1.CarbonEmissionHeaderSeq=z.Seq)
                    where z.EngMainSeq=@EngMainSeq
                    order by z1.Seq desc
                ) b on(1=1)
                where a.EngMainSeq=@EngMainSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public bool RemoveTradeDoc(string fileName, int engMainSeq)
        {
            try
            {
                string sql = @"
                    delete cd From CarbonEmissionTradingDoc cd
                    inner join  CarbonEmissionHeader ch on ch.Seq = cd.CarbonEmissionHeaderSeq
                    where ch.EngMainSeq = @EngMainSeq and cd.OriginFileName = @FileName;
                    ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                cmd.Parameters.AddWithValue("@FileName", fileName);

                db.ExecuteNonQuery(cmd);

                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("CarbonEmissionPayItemService.AppendTradeDoc: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }

        //更新碳交易核定文件 s20230428
        public bool AppendTradeDoc(CarbonEmissionTradingDocModel m)
        {
            try {
                Null2Empty(m);
                string sql = @"
                    insert into CarbonEmissionTradingDoc(
                        CarbonEmissionHeaderSeq,
                        CarbonTradingApprovedDate,
                        CarbonTradingNo,
                        OriginFileName,
                        UniqueFileName,
                        CreateTime,
                        CreateUserSeq
                    )values(
                        @CarbonEmissionHeaderSeq,
                        @CarbonTradingApprovedDate,
                        @CarbonTradingNo,
                        @OriginFileName,
                        @UniqueFileName,
                        GetDate(),
                        @ModifyUserSeq
                    )
                    ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@CarbonEmissionHeaderSeq", m.CarbonEmissionHeaderSeq);
                cmd.Parameters.AddWithValue("@CarbonTradingApprovedDate", m.CarbonTradingApprovedDate);
                cmd.Parameters.AddWithValue("@CarbonTradingNo", m.CarbonTradingNo);
                cmd.Parameters.AddWithValue("@OriginFileName", m.OriginFileName);
                cmd.Parameters.AddWithValue("@UniqueFileName", m.UniqueFileName);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                db.ExecuteNonQuery(cmd);

                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("CarbonEmissionPayItemService.AppendTradeDoc: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }
        //核定文件清單
        public List<T> GetApproveDocs<T>(int carbonEmissionHeaderSeq)
        {
            string sql = @"
                select 
	                a.Seq,
	                a.CarbonEmissionHeaderSeq,
	                a.CarbonTradingApprovedDate,
	                a.CarbonTradingNo,
                    a.OriginFileName
                from CarbonEmissionTradingDoc a
                where a.CarbonEmissionHeaderSeq=@CarbonEmissionHeaderSeq
                order by a.Seq desc
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CarbonEmissionHeaderSeq", carbonEmissionHeaderSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetApproveDocBySeq<T>(int seq)
        {
            string sql = @"
                select 
	                a.Seq,
	                a.CarbonEmissionHeaderSeq,
	                a.CarbonTradingApprovedDate,
	                a.CarbonTradingNo,
                    a.OriginFileName,
                    a.UniqueFileName
                from CarbonEmissionTradingDoc a
                where a.Seq=@Seq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        //更新交易量 s20230419
        public bool UpdateEngTrade(int carbonEmissionHeaderSeq, List<CarbonTradeEngsVModel> items, string desc)
        {
            db.BeginTransaction();
            try
            {
                if (items != null)
                {
                    foreach (CarbonTradeEngsVModel m in items)
                    {
                        Null2Empty(m);
                        if (m.Seq.Value > -1)
                        {
                            if (!m.Quantity.HasValue)
                            {
                                delEngTrade(m);
                            }
                            else if (updateEngTrade(m) == 0)
                            {
                                insertEngTrade(carbonEmissionHeaderSeq, m);
                            }
                        }
                        else
                        {
                            insertEngTrade(carbonEmissionHeaderSeq, m);
                        }
                    }
                }
                string sql = @"
                    update CarbonEmissionHeader set
                        CarbonTradingDesc=@CarbonTradingDesc,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", carbonEmissionHeaderSeq);
                cmd.Parameters.AddWithValue("@CarbonTradingDesc", desc);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("CarbonEmissionPayItemService.UpdateEngTrade: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }
        private int insertEngTrade(int carbonEmissionHeaderSeq, CarbonTradeEngsVModel m)
        {
            string sql = @"
                insert into CarbonEmissionTrading(
                    CarbonEmissionHeaderSeq,
                    EngMainSeq,
                    Quantity
                )values(
                    @CarbonEmissionHeaderSeq,
                    @EngMainSeq,
                    @Quantity
                )";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CarbonEmissionHeaderSeq", carbonEmissionHeaderSeq);
            cmd.Parameters.AddWithValue("@EngMainSeq", m.EngMainSeq);
            cmd.Parameters.AddWithValue("@Quantity", m.Quantity);
            return db.ExecuteNonQuery(cmd);
        }
        private int updateEngTrade(CarbonTradeEngsVModel m)
        {
            string sql = @"
                Update CarbonEmissionTrading set
                    Quantity=@Quantity
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("Seq", m.Seq);
            cmd.Parameters.AddWithValue("@Quantity", this.NulltoDBNull(m.Quantity));
            return db.ExecuteNonQuery(cmd);
        }
        private int delEngTrade(CarbonTradeEngsVModel m)
        {
            string sql = @"
                delete from CarbonEmissionTrading where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("Seq", m.Seq);
            return db.ExecuteNonQuery(cmd);
        }
        //交易清單
        public List<T> GetEngTradeList<T>(int carbonEmissionHeaderSeq)
        {
            string sql = @"
                select 
	                a.Seq,
	                a.EngMainSeq,
                    a.Quantity,
                    b.EngNo,
                    b.EngName,
                    b1.Name ExecUnitName,
                    b.ApprovedCarbonQuantity,
                    b.CarbonDesignQuantity
                from CarbonEmissionTrading a
                inner join EngMain b on(b.Seq=a.EngMainSeq)
                inner join Unit b1 on(b1.Seq=b.ExecUnitSeq)
                where a.CarbonEmissionHeaderSeq=@CarbonEmissionHeaderSeq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CarbonEmissionHeaderSeq", carbonEmissionHeaderSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //更新交易量 s20230419
        public bool ConfirmTrade(EQMCarbonEmissionHeaderVModel ceHaeder, List<CarbonTradeEngsVModel> items, decimal co2Total)
        {
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                int uId = this.getUserSeq();
                string sql = @"
                    update EngMain set
                        CarbonTradedQuantity=CarbonTradedQuantity+@Quantity,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
                foreach (CarbonTradeEngsVModel m in items)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", m.EngMainSeq);
                    cmd.Parameters.AddWithValue("@Quantity", m.Quantity);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", uId);
                    db.ExecuteNonQuery(cmd);
                }

                sql = @"
                    update EngMain set
                        CarbonDesignQuantity=@CarbonDesignQuantity,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", ceHaeder.EngMainSeq); 
                cmd.Parameters.AddWithValue("@CarbonDesignQuantity", co2Total);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", uId);
                db.ExecuteNonQuery(cmd);

                sql = @"
                    update CarbonEmissionHeader set
                        State=1,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", ceHaeder.Seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", uId);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("CarbonEmissionPayItemService.ConfirmTrade: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }

        //碳交易工程清單 s20230508
        public List<T> GetCarbonTradeEngList<T>(int year, int unitSeq, int subUnitSeq)
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
                    + ((isSuperviosr) ? " and es.UserMainSeq=" + userSeq : Utils.getAuthoritySql("a."));

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
                    + ((isSuperviosr) ? " and es.UserMainSeq=" + userSeq : Utils.getAuthoritySql("a."));
            }
            string sql2 = @"
                     SELECT
                        aa.Seq,
                        aa.EngNo,
                        aa.EngName,
                        aa.EngYear,
                        b.Name ExecUnitName, 
                        c.Name ExecSubUnitName,
                        aa.AwardDate,
                        aa.ApprovedCarbonQuantity,
                        aa.CarbonDesignQuantity,
                        d.State CarbonEmissionHeaderState,
                        (
                            select sum(Quantity) from CarbonEmissionTrading where CarbonEmissionHeaderSeq=d.Seq
                        ) CarbonTradeQuantity
                    FROM EngMain aa
                    inner join Unit b on(b.Seq=aa.ExecUnitSeq)
                    left outer join Unit c on(c.Seq=aa.ExecSubUnitSeq)
                    left outer join CarbonEmissionHeader d on(d.EngMainSeq=aa.Seq)
                    where aa.Seq in (" + sql + @")
                    order by aa.EngNo DESC
                    ";

            SqlCommand cmd = db.GetCommand(sql2);
            cmd.Parameters.AddWithValue("@ExecUnitSeq", unitSeq);
            cmd.Parameters.AddWithValue("@ExecSubUnitSeq", subUnitSeq);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<T>(cmd);
        }
        //初始碳交易調整s20230508
        public bool InitEngTradeAdj(int engMainSeq)
        {
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                string sql = @"
                delete from CarbonEmissionTradingAdj
                where CarbonEmissionHeaderSeq = (select Seq from CarbonEmissionHeader where EngMainSeq=@EngMainSeq)
                ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                db.ExecuteNonQuery(cmd);

                sql = @"
                insert into CarbonEmissionTradingAdj(
                    CarbonEmissionHeaderSeq, EngMainSeq, Quantity, SrcQuantity,
                    CreateTime, CreateUserSeq ,ModifyTime ,ModifyUserSeq
                )
                select 
                    b.CarbonEmissionHeaderSeq, b.EngMainSeq, b.Quantity, b.Quantity,
                    GetDate(), @ModifyUserSeq, GetDate(), @ModifyUserSeq
                from CarbonEmissionHeader a
                inner join CarbonEmissionTrading b on(b.CarbonEmissionHeaderSeq=a.Seq)
                where a.EngMainSeq=@EngMainSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                db.ExecuteNonQuery(cmd);

                sql = @"
                update CarbonEmissionHeader set
                    AdjState=0,
                    ModifyTime=GetDate(),
                    ModifyUserSeq=@ModifyUserSeq
                where EngMainSeq=@EngMainSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("CarbonEmissionPayItemService.InitEngTradeAdj: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }
        //交易碳工程清單 s20230508
        public List<T> GetTradeAdjList<T>(int carbonEmissionHeaderSeq)
        {
            string sql = @"
                select 
                    c1.Seq,
                    c1.EngMainSeq,
                    b.EngNo,
                    b.EngName,
                    b1.Name ExecUnitName,
                    b.ApprovedCarbonQuantity,
                    b.CarbonDesignQuantity,
                    ISNULL((
                        select sum(z1.Quantity) from (
                            select ISNULL(Quantity,0) Quantity from CarbonEmissionTrading z
                            inner join CarbonEmissionHeader z1 on(z1.Seq=z.CarbonEmissionHeaderSeq and z1.State=0)
                            where z.EngMainSeq=c1.EngMainSeq
                            union all
                            select ISNULL(Quantity,0) Quantity from CarbonEmissionTradingAdj z
                            inner join CarbonEmissionHeader z1 on(z1.Seq=z.CarbonEmissionHeaderSeq and z1.AdjState=0)
        		            where z.EngMainSeq=c1.EngMainSeq
                            and z.Quantity > z.SrcQuantity
                        ) z1
                    ),0) + b.CarbonTradedQuantity - ISNULL(c1.Quantity,0) TradingTotalQuantity,
                    c1.Quantity,
                    c1.SrcQuantity
                from CarbonEmissionTradingAdj c1
                inner join EngMain b on(b.Seq=c1.EngMainSeq)
                inner join Unit b1 on(b1.Seq=b.ExecUnitSeq)
                where c1.CarbonEmissionHeaderSeq=@CarbonEmissionHeaderSeq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CarbonEmissionHeaderSeq", carbonEmissionHeaderSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //更新交易量 s20230419
        public bool DelTradeEng(int seq)
        {
            try
            {
                string sql = @" delete from CarbonEmissionTradingAdj where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                return true;
            }
            catch (Exception e)
            {
                log.Info("CarbonEmissionPayItemService.DelTradeEng: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }
        //可交易工程 s20230508
        public List<SelectOptionModel> GetTradeEngs(int engMainSeq)
        {
            string sql = @"
                select
	                z.EngNo Text, 
                    z.EngNo Value
                from (
	                select
                        b.EngNo,
                        b.ApprovedCarbonQuantity,
                        b.CarbonDesignQuantity,
                        ISNULL((
                            select sum(z1.Quantity) from (
                                select ISNULL(Quantity,0) Quantity from CarbonEmissionTrading z
                                inner join CarbonEmissionHeader z1 on(z1.Seq=z.CarbonEmissionHeaderSeq and z1.State=0)
                                where z.EngMainSeq=a.EngMainSeq
                                union all
                                select ISNULL(Quantity,0) Quantity from CarbonEmissionTradingAdj z
                                inner join CarbonEmissionHeader z1 on(z1.Seq=z.CarbonEmissionHeaderSeq and z1.AdjState=0)
        		                where z.EngMainSeq=a.EngMainSeq
                                and z.Quantity > z.SrcQuantity
                            ) z1
                        ),0) + CarbonTradedQuantity TradingTotalQuantity
                    from CarbonEmissionHeader a
                    inner join EngMain b on(b.Seq=a.EngMainSeq
                        and b.EngYear in (select EngYear from EngMain where Seq=@EngMainSeq)
                        and b.Seq not in (
        	                select b2.EngMainSeq from CarbonEmissionHeader b1
                            inner join CarbonEmissionTradingAdj b2 on(b2.CarbonEmissionHeaderSeq=b1.Seq)
                            where b1.EngMainSeq=@EngMainSeq
                        )                    
                    )
                    inner join Unit b1 on(b1.Seq=b.ExecUnitSeq)
                    where a.State=1
                    and b.ApprovedCarbonQuantity > b.CarbonDesignQuantity
                ) z
                where z.ApprovedCarbonQuantity - z.CarbonDesignQuantity - z.TradingTotalQuantity > 0
                order by z.EngNo
            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<SelectOptionModel>(cmd);
        }
        //可交易工程 s20230508
        public List<T> GetTradeEng<T>(string engNo)
        {
            string sql = @"
                select
	                -1 Seq,
                    z.EngMainSeq,
                    z.EngNo,
                    z.EngName,
                    z.ExecUnitName,
                    z.ApprovedCarbonQuantity,
                    z.CarbonDesignQuantity,
	                z.TradingTotalQuantity,
                    0 Quantity
                from (
	                select
                        a.EngMainSeq,
                        b.EngNo,
                        b.EngName,
                        b1.Name ExecUnitName,
                        b.ApprovedCarbonQuantity,
                        b.CarbonDesignQuantity,
                        ISNULL((
                            select sum(z1.Quantity) from (
                                select ISNULL(Quantity,0) Quantity from CarbonEmissionTrading z
                                inner join CarbonEmissionHeader z1 on(z1.Seq=z.CarbonEmissionHeaderSeq and z1.State=0)
                                where z.EngMainSeq=a.EngMainSeq
                                union all
                                select ISNULL(Quantity,0) Quantity from CarbonEmissionTradingAdj z
                                inner join CarbonEmissionHeader z1 on(z1.Seq=z.CarbonEmissionHeaderSeq and z1.AdjState=0)
        		                where z.EngMainSeq=a.EngMainSeq
                                and z.Quantity > z.SrcQuantity
                            ) z1
                        ),0) + CarbonTradedQuantity TradingTotalQuantity
                    from CarbonEmissionHeader a
                    inner join EngMain b on(b.Seq=a.EngMainSeq and b.EngNo=@EngNo)
                    inner join Unit b1 on(b1.Seq=b.ExecUnitSeq)
                    where a.State=1
                    and b.ApprovedCarbonQuantity > b.CarbonDesignQuantity
                ) z
                where z.ApprovedCarbonQuantity - z.CarbonDesignQuantity - z.TradingTotalQuantity > 0
            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngNo", engNo);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //更新交易量 s20230508
        public bool UpdateEngTradeAdj(int carbonEmissionHeaderSeq, CarbonTradeEngsVModel m)
        {
            try
            {
                Null2Empty(m);
                if (m.Seq.Value == -1)
                {
                    insertEngTradeAdj(carbonEmissionHeaderSeq, m); 
                }
                else
                {
                    updateEngTradeAdj(m);
                }

                return true;
            }
            catch (Exception e)
            {
                log.Info("CarbonEmissionPayItemService.UpdateEngTradeAdj: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }
        private int insertEngTradeAdj(int carbonEmissionHeaderSeq, CarbonTradeEngsVModel m)
        {
            string sql = @"
                insert into CarbonEmissionTradingAdj(
                    CarbonEmissionHeaderSeq,
                    EngMainSeq,
                    Quantity,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                )values(
                    @CarbonEmissionHeaderSeq,
                    @EngMainSeq,
                    @Quantity,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CarbonEmissionHeaderSeq", carbonEmissionHeaderSeq);
            cmd.Parameters.AddWithValue("@EngMainSeq", m.EngMainSeq);
            cmd.Parameters.AddWithValue("@Quantity", m.Quantity);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
            return db.ExecuteNonQuery(cmd);
        }
        private int updateEngTradeAdj(CarbonTradeEngsVModel m)
        {
            string sql = @"
                Update CarbonEmissionTradingAdj set
                    Quantity=@Quantity,
                    ModifyTime=GetDate(),
                    ModifyUserSeq=@ModifyUserSeq
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("Seq", m.Seq);
            cmd.Parameters.AddWithValue("@Quantity", this.NulltoDBNull(m.Quantity));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
            return db.ExecuteNonQuery(cmd);
        }
        //更新交易量 s20230509
        public bool ConfirmTradeAdj(EQMCarbonEmissionHeaderVModel ceHaeder, List<CarbonTradeEngsVModel> items, List<CarbonTradeEngsVModel> srcItems)
        {
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                int uId = this.getUserSeq();
                string sql = @"
                    update EngMain set
                        CarbonTradedQuantity=CarbonTradedQuantity-@Quantity,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
                foreach (CarbonTradeEngsVModel m in srcItems)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", m.EngMainSeq);
                    cmd.Parameters.AddWithValue("@Quantity", m.Quantity);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", uId);
                    db.ExecuteNonQuery(cmd);
                }

                sql = @"
                    delete from CarbonEmissionTrading
                    where CarbonEmissionHeaderSeq=@CarbonEmissionHeaderSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CarbonEmissionHeaderSeq", ceHaeder.Seq);
                db.ExecuteNonQuery(cmd);

                sql = @"
                    insert into CarbonEmissionTrading (
                        CarbonEmissionHeaderSeq, EngMainSeq, Quantity
                    )
                    select
                        CarbonEmissionHeaderSeq, EngMainSeq, Quantity
                    from CarbonEmissionTradingAdj
                    where CarbonEmissionHeaderSeq=@CarbonEmissionHeaderSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CarbonEmissionHeaderSeq", ceHaeder.Seq);
                db.ExecuteNonQuery(cmd);

                sql = @"
                    update CarbonEmissionHeader set
                        State=0,
                        AdjState=1,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", ceHaeder.Seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", uId);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("CarbonEmissionPayItemService.ConfirmTradeAdj: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }
    }
}