using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class SchEngProgressSubService : BaseService
    {//前置作業 - 分項工程

        //加入分項工程 PayItem
        public bool DelSubPayItem(int schEngProgressSubSeq, int schEngProgressPayItemSeq)
        {
            db.BeginTransaction();
            try
            {
                string sql = @"
                delete from SchEngProgressSubPayItem where Seq=@Seq
                ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", schEngProgressPayItemSeq);
                db.ExecuteNonQuery(cmd);
                //
                updateWeights(schEngProgressSubSeq);//s20230623
                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchEngProgressSubService.DelSubPayItem: " + e.Message);
                return false;
            }
        }
        //加入分項工程 PayItem
        public bool AddSubPayItem(int schEngProgressSubSeq, int schEngProgressPayItemSeq)
        {
            db.BeginTransaction();
            try
            {
                string sql = @"
                insert into SchEngProgressSubPayItem(
                    SchEngProgressSubSeq,
                    SchEngProgressPayItemSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @SchEngProgressSubSeq,
                    @SchEngProgressPayItemSeq,
                    GetDate(),
                    @ModifyUserSeq        
                )
                ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@SchEngProgressSubSeq", schEngProgressSubSeq);
                cmd.Parameters.AddWithValue("@SchEngProgressPayItemSeq", schEngProgressPayItemSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                db.ExecuteNonQuery(cmd);

                updateWeights(schEngProgressSubSeq);//s20230623

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchEngProgressSubService.AddSubPayItem: " + e.Message);
                return false;
            }
        }
        //更新權重 s20230623
        private void updateWeights(int schEngProgressSubSeq)
        {
            string sql = @"
                update SchEngProgressSub set
                    Weights=round(
    	                (select ISNULL(sum(b.Amount),0)*100 from SchEngProgressSubPayItem a
		                inner join SchEngProgressPayItem b on(b.Seq=a.SchEngProgressPayItemSeq)
		                where a.SchEngProgressSubSeq=@SchEngProgressSubSeq) / (
                            select c.SubContractingBudget from SchEngProgressSub a
                            inner join SchEngProgressHeader b on(b.Seq=a.SchEngProgressHeaderSeq)
                            inner join EngMain c on(c.Seq=b.EngMainSeq)
                            where a.Seq=@SchEngProgressSubSeq
                        )
                    ,0),
                    ModifyTime=GetDate(),
                    ModifyUserSeq=@ModifyUserSeq
                where Seq=@SchEngProgressSubSeq
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SchEngProgressSubSeq", schEngProgressSubSeq);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
            db.ExecuteNonQuery(cmd);
        }

        //分項工程 PayItem清單
        public List<T> GetSubPayItemList<T>(int schEngProgressSubSeq)
        {

            string sql = @"
                select
                    b.Seq,
                    a.PayItem,
                    a.Description,
                    a.Unit,
                    a.Quantity,
                    a.Price,
                    a.Amount
                from SchEngProgressSubPayItem b
                inner join SchEngProgressPayItem a on(a.Seq=b.SchEngProgressPayItemSeq)
                where b.SchEngProgressSubSeq=@SchEngProgressSubSeq
             ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@SchEngProgressSubSeq", schEngProgressSubSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //更新 分項工程
        public int Update(SchEngProgressSubModel m)
        {
            Null2Empty(m);
            try
            {
                string sql = @"update SchEngProgressSub set
                    EngName=@EngName,
                    Weights=@Weights,
                    ModifyTime=GetDate(),
                    ModifyUserSeq=@ModifyUserSeq
                where Seq=@Seq";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@EngName", m.EngName);
                cmd.Parameters.AddWithValue("@Weights", m.Weights);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("SchEngProgressSubService.Update: " + e.Message);
                //log.Info(sql);
                return -1;
            }
        }
        //新增分項工程
        public bool AddSubEng(int schEngProgressHeaderSeq)
        {
            try
            {
                string sql = @"
                insert into SchEngProgressSub(
                    SchEngProgressHeaderSeq,
                    EngName,
                    Weights,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @SchEngProgressHeaderSeq,
                    '',
                    0,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq        
                )
                ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@SchEngProgressHeaderSeq", schEngProgressHeaderSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                db.ExecuteNonQuery(cmd);

                return true;
            }
            catch (Exception e)
            {
                log.Info("SchEngProgressSubService.AddSubEng: " + e.Message);
                return false;
            }
        }
        //清單
        public List<T> GetList<T>(int engMainSeq)
        {
            string sql = @"
                DECLARE @tmp_PayItemProgress table (SchEngProgressPayItemSeq INT, SchProgress decimal(6,2), ActualProgress decimal(6,2));

                insert into @tmp_PayItemProgress (SchEngProgressPayItemSeq, SchProgress, ActualProgress)
                select
                    z.SchEngProgressPayItemSeq, 
                    CAST((z.Price * z.Quantity * z.SchProgress) / z.Amount as decimal(6,2)) SchProgress,
                    CAST((z.Price * z.ActualQuantity * 100) / z.Amount as decimal(6,2)) ActualProgress
                from fPayItemProgress(@EngMainSeq, 1, GetDate()) z;     

                select
	                b.Seq, b.EngName, b.Weights,
                    sum(c1.Amount) Amount,
	                IIF(a.SPState=2, b.Weights*sum(d.SchProgress)/COUNT(b.Seq), null) SchProgress,
                    IIF(a.SPState=2, b.Weights*sum(d.ActualProgress)/COUNT(b.Seq), null) ActualProgress
                from SchEngProgressHeader a
                inner join SchEngProgressSub b on(b.SchEngProgressHeaderSeq=a.Seq)
                left outer join SchEngProgressSubPayItem c on(c.SchEngProgressSubSeq=b.Seq)
                left outer join SchEngProgressPayItem c1 on(c1.Seq=c.SchEngProgressPayItemSeq)
                left outer join SchProgressHeader c2 on(c2.EngMainSeq=@EngMainSeq)
                left outer join @tmp_PayItemProgress d on(a.SPState=2 and c2.SPState=1 and d.SchEngProgressPayItemSeq=c.SchEngProgressPayItemSeq)
                where a.EngMainSeq=@EngMainSeq
                group by a.SPState, b.Seq, b.EngName, b.Weights
                order by b.EngName
             ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public int FillCompleted(int seq)
        {
            try
            {
                string sql = @"update SchEngProgressHeader set
                    SPState=2,
                    ModifyTime=GetDate(),
                    ModifyUserSeq=@ModifyUserSeq
                where Seq=@Seq";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("SchEngProgressSubService.FillCompleted: " + e.Message);
                //log.Info(sql);
                return -1;
            }
        }
        //刪除
        public bool DelRecord(int seq)
        {
            db.BeginTransaction();
            try
            {
                string sql = @"
                delete from SchEngProgressSubPayItem where SchEngProgressSubSeq in(
                    select Seq from SchEngProgressSub where Seq=@Seq
                );

                delete from SchEngProgressSub where Seq=@Seq;
                ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SchEngProgressSubService.DelRecord: " + e.Message);
                return false;
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
                and a.Seq not in(
                    select SchEngProgressPayItemSeq from SchEngProgressSubPayItem where SchEngProgressSubSeq in(
                        select Seq from SchEngProgressSub where SchEngProgressHeaderSeq=b.Seq
                    )
                )
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
        //未勾稽清單
        public List<T> GetPayItemList<T>(int engMainSeq, int pageRecordCount, int pageIndex, string fLevel, string keyWord)
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
                    a.Memo
                from SchEngProgressHeader b
                inner join SchEngProgressPayItem a on(a.SchEngProgressHeaderSeq=b.Seq
                    and (@fLevel='' or a.PayItem like @fLevel)
                    and (@keyWord='' or a.Description like @keyWord)
                )
                where b.EngMainSeq=@EngMainSeq
                and a.Seq not in(
                    select SchEngProgressPayItemSeq from SchEngProgressSubPayItem where SchEngProgressSubSeq in(
                        select Seq from SchEngProgressSub where SchEngProgressHeaderSeq=b.Seq
                    )
                )
                Order by a.Seq
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
                and a.Seq not in(
                    select SchEngProgressPayItemSeq from SchEngProgressSubPayItem where SchEngProgressSubSeq in(
                        select Seq from SchEngProgressSub where SchEngProgressHeaderSeq=b.Seq
                    )
                )
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
                and a.Seq not in(
                    select SchEngProgressPayItemSeq from SchEngProgressSubPayItem where SchEngProgressSubSeq in(
                        select Seq from SchEngProgressSub where SchEngProgressHeaderSeq=b.Seq
                    )
                )
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
    }
}