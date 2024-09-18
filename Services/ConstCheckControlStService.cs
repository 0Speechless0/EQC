using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class ConstCheckControlStService : BaseService
    {//施工抽查標準
        //s20230415
        public int ImportStdList(int constCheckListSeq, List<QCStdModel> items)
        {
            string sql;
            db.BeginTransaction();
            try
            {
                sql = @"
                delete from ConstCheckControlSt where ConstCheckListSeq=@Seq
                ";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", constCheckListSeq);
                db.ExecuteNonQuery(cmd);

                sql = @"
                insert into ConstCheckControlSt (
                    DataKeep,
                    ConstCheckListSeq,
                    CCFlow1,
                    CCFlow2,
                    CCManageItem1,
                    CCCheckStand1,
                    CCCheckTiming,
                    CCCheckMethod,
                    CCCheckFeq,
                    CCIncomp,
                    CCManageRec,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    1,
                    @ConstCheckListSeq,
                    @CCFlow1,
                    @CCFlow2,
                    @CCManageItem1,
                    @CCCheckStand1,
                    @CCCheckTiming,
                    @CCCheckMethod,
                    @CCCheckFeq,
                    @CCIncomp,
                    @CCManageRec,
                    @OrderNo,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
                int orderNo = 1;
                foreach (QCStdModel m in items)
                {
                    this.Null2Empty(m);

                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@ConstCheckListSeq", constCheckListSeq);
                    cmd.Parameters.AddWithValue("@CCFlow1", m.FlowType);
                    cmd.Parameters.AddWithValue("@CCFlow2", m.Flow);
                    cmd.Parameters.AddWithValue("@CCManageItem1", m.ManageItem);
                    cmd.Parameters.AddWithValue("@CCCheckStand1", m.Stand);
                    cmd.Parameters.AddWithValue("@CCCheckTiming", m.CheckTiming);
                    cmd.Parameters.AddWithValue("@CCCheckMethod", m.CheckMethod);
                    cmd.Parameters.AddWithValue("@CCCheckFeq", m.CheckFeq);
                    cmd.Parameters.AddWithValue("@CCIncomp", m.Incomp);
                    cmd.Parameters.AddWithValue("@CCManageRec", m.ManageRec);
                    cmd.Parameters.AddWithValue("@OrderNo", orderNo++);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                    int result = db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("ConstCheckControlStService.ImportStdList: " + e.Message);
                return -1;
            }
        }
        //s20230414
        public List<T> GetStdList<T>(int ConstCheckListSeq)
        {
            string sql = @"
                SELECT
                    CCFlow1 FlowType,
                    CCFlow2 Flow,
                    CCManageItem1 ManageItem,
                    CCCheckStand1 Stand,
                    CCCheckTiming CheckTiming,
                    CCCheckMethod CheckMethod,
                    CCCheckFeq CheckFeq,
                    CCIncomp Incomp,
                    CCManageRec ManageRec
                FROM ConstCheckControlSt
                where ConstCheckListSeq=@ConstCheckListSeq
                and DataKeep=1
                order by OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ConstCheckListSeq", ConstCheckListSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetList<T>(int ConstCheckListSeq)
        {
            string sql = @"SELECT * FROM ConstCheckControlSt
                where ConstCheckListSeq=@ConstCheckListSeq
                and DataKeep=1
                order by OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ConstCheckListSeq", ConstCheckListSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        public int GetConstCheckControlStCount(int ConstCheckListSeq)
        {
            string sql = @"
                SELECT count(a.Seq) total FROM ConstCheckList a
                inner join ConstCheckControlSt b on(b.ConstCheckListSeq=a.Seq)
                where a.Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", ConstCheckListSeq);
            DataTable dt = db.GetDataTable(cmd);
            if (dt.Rows.Count == 1)
            {
                return Convert.ToInt32(dt.Rows[0]["total"].ToString());
            }
            else
            {
                return 0;
            }
        }
        public List<T> GetList<T>(int ConstCheckListSeq, int pageIndex, int perPage)
        {
            string sql = @"
                SELECT
                    b.Seq,
                    b.DataType,
                    b.DataKeep,
                    b.ConstCheckListSeq,
                    b.CCFlow1,
                    b.CCFlow2,
                    b.CCManageItem1,
                    b.CCManageItem2,
                    b.CCCheckStand1,
                    b.CCCheckStand2,
                    b.CCCheckTiming,
                    b.CCCheckMethod,
                    b.CCCheckFeq,
                    b.CCIncomp,
                    b.CCManageRec,
                    b.CCType,
                    b.CCMemo,
                    b.CCCheckFields,
                    b.CCManageFields,
                    b.OrderNo 
                FROM ConstCheckList a
                inner join ConstCheckControlSt b on(b.ConstCheckListSeq=a.Seq)
                where a.Seq=@Seq
                order by b.DataKeep desc, b.OrderNo
                OFFSET @pageIndex ROWS
				FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", ConstCheckListSeq);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //s20230626
        public List<T> GetListAll<T>(int ConstCheckListSeq)
        {
            string sql = @"
                SELECT
                    b.Seq,
                    b.DataType,
                    b.DataKeep,
                    b.ConstCheckListSeq,
                    b.CCFlow1,
                    b.CCFlow2,
                    b.CCManageItem1,
                    b.CCManageItem2,
                    b.CCCheckStand1,
                    b.CCCheckStand2,
                    b.CCCheckTiming,
                    b.CCCheckMethod,
                    b.CCCheckFeq,
                    b.CCIncomp,
                    b.CCManageRec,
                    b.CCType,
                    b.CCMemo,
                    b.CCCheckFields,
                    b.CCManageFields,
                    b.OrderNo 
                FROM ConstCheckControlSt b
                where b.ConstCheckListSeq=@ConstCheckListSeq
                order by b.OrderNo
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ConstCheckListSeq", ConstCheckListSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetItem<T>(int seq)
        {
            string sql = @"
                SELECT
                    Seq,
                    DataType,
                    DataKeep,
                    CCFlow1,
                    CCFlow2,
                    ConstCheckListSeq,
                    CCManageItem1,
                    CCManageItem2,
                    CCCheckStand1,
                    CCCheckStand2,
                    CCCheckTiming,
                    CCCheckMethod,
                    CCCheckFeq,
                    CCIncomp,
                    CCManageRec,
                    CCType,
                    CCMemo,
                    CCCheckFields,
                    CCManageFields,
                    OrderNo 
                FROM ConstCheckControlSt
                where Seq=@Seq
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
         public int Insert(int ConstCheckListSeq, ConstCheckControlTpModel m)
         {
            //Null2Empty(m);
            string sql = @"
                insert into ConstCheckControlSt (
                    DataType,
                    ConstCheckListSeq,
                    CCFlow1,
                    CCFlow2,
                    CCManageItem1,
                    CCManageItem2,
                    CCCheckStand1,
                    CCCheckStand2,
                    CCCheckTiming,
                    CCCheckMethod,
                    CCCheckFeq,
                    CCIncomp,
                    CCManageRec,
                    CCType,
                    CCMemo,
                    CCCheckFields,
                    CCManageFields,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    1,
                    @ConstCheckListSeq,
                    @CCFlow1,
                    @CCFlow2,
                    @CCManageItem1,
                    @CCManageItem2,
                    @CCCheckStand1,
                    @CCCheckStand2,
                    @CCCheckTiming,
                    @CCCheckMethod,
                    @CCCheckFeq,
                    @CCIncomp,
                    @CCManageRec,
                    @CCType,
                    @CCMemo,
                    @CCCheckFields,
                    @CCManageFields,
                    (select COALESCE(max(a.OrderNo),0)+1 from ConstCheckControlSt a where a.ConstCheckListSeq=@ConstCheckListSeq),
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ConstCheckListSeq", ConstCheckListSeq);
            cmd.Parameters.AddWithValue("@CCFlow1", m.CCFlow1 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CCFlow2", m.CCFlow2 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CCManageItem1", m.CCManageItem1 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CCManageItem2", m.CCManageItem2 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CCCheckStand1", m.CCCheckStand1 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CCCheckStand2", m.CCCheckStand2 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CCCheckTiming", m.CCCheckTiming ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CCCheckMethod", m.CCCheckMethod ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CCCheckFeq", m.CCCheckFeq ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CCIncomp", m.CCIncomp ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CCManageRec", m.CCManageRec ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CCType", this.NulltoDBNull(m.CCType));//s20230912
            cmd.Parameters.AddWithValue("@CCMemo", m.CCMemo );
            cmd.Parameters.AddWithValue("@CCCheckFields", m.CCCheckFields ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CCManageFields", m.CCManageFields ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            try
            {
                int result = db.ExecuteNonQuery(cmd);
                if (result == 0) return 0;

                cmd.Parameters.Clear();

                string sql1 = @"SELECT IDENT_CURRENT('ConstCheckControlSt') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);
                return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
            }
            catch(Exception e)
            {

            }
            return 0;

        }
        public int NewAdd(int ConstCheckListSeq)
        {
            //Null2Empty(m);
            string sql = @"
                insert into ConstCheckControlSt (
                    DataType,
                    ConstCheckListSeq,
                    CCFlow1,
                    CCType,
                    CCMemo,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    1,
                    @ConstCheckListSeq,
                    1,
                    1,
                    0,
                    (select COALESCE(max(a.OrderNo),0)+1 from ConstCheckControlSt a where a.ConstCheckListSeq=@ConstCheckListSeq),
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ConstCheckListSeq", ConstCheckListSeq);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('ConstCheckControlSt') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
        }

        public bool Updates(List<ConstCheckControlStModel> items)
        {
            string sql = @"";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                foreach (ConstCheckControlStModel m in items)
                {
                    Null2Empty(m);
                    sql = @"
                        update ConstCheckControlSt set
                            DataType=@DataType,
                            DataKeep=@DataKeep,
                            CCFlow1=@CCFlow1,
                            CCFlow2=@CCFlow2,
                            CCManageItem1 = @CCManageItem1,
                            CCManageItem2 = @CCManageItem2,
                            CCCheckStand1 = @CCCheckStand1,
                            CCCheckStand2 = @CCCheckStand2,
                            CCCheckTiming = @CCCheckTiming,
                            CCCheckMethod = @CCCheckMethod,
                            CCCheckFeq = @CCCheckFeq,
                            CCIncomp = @CCIncomp,
                            CCManageRec = @CCManageRec,
                            CCType = @CCType,
                            CCMemo = @CCMemo,
                            CCCheckFields = @CCCheckFields,
                            CCManageFields = @CCManageFields,
                            OrderNo = @OrderNo,
                            ModifyTime = GETDATE(),
                            ModifyUserSeq = @ModifyUserSeq
                        Where Seq = @Seq";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@DataType", m.DataType);
                    cmd.Parameters.AddWithValue("@DataKeep", m.DataKeep);
                    cmd.Parameters.AddWithValue("@CCFlow1", m.CCFlow1);
                    cmd.Parameters.AddWithValue("@CCFlow2", m.CCFlow2);
                    cmd.Parameters.AddWithValue("@CCManageItem1", m.CCManageItem1);
                    cmd.Parameters.AddWithValue("@CCManageItem2", m.CCManageItem2);
                    cmd.Parameters.AddWithValue("@CCCheckStand1", m.CCCheckStand1);
                    cmd.Parameters.AddWithValue("@CCCheckStand2", m.CCCheckStand2);
                    cmd.Parameters.AddWithValue("@CCCheckTiming", m.CCCheckTiming);
                    cmd.Parameters.AddWithValue("@CCCheckMethod", m.CCCheckMethod);
                    cmd.Parameters.AddWithValue("@CCCheckFeq", m.CCCheckFeq);
                    cmd.Parameters.AddWithValue("@CCIncomp", m.CCIncomp);
                    cmd.Parameters.AddWithValue("@CCManageRec", m.CCManageRec);
                    cmd.Parameters.AddWithValue("@CCType", this.NulltoDBNull(m.CCType)); //s20230912
                    cmd.Parameters.AddWithValue("@CCMemo", m.CCMemo);
                    cmd.Parameters.AddWithValue("@CCCheckFields", this.NulltoDBNull(m.CCCheckFields));
                    cmd.Parameters.AddWithValue("@CCManageFields", this.NulltoDBNull(m.CCManageFields));
                    cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    cmd.Parameters.AddWithValue("@Seq", m.Seq);

                    db.ExecuteNonQuery(cmd);
                }
                    db.TransactionCommit();
                    return true;
            } catch (Exception e) {
                db.TransactionRollback();
                log.Info("ConstCheckControlStService.Updates: " + e.Message);
                log.Info(sql);
                return false;
            }
        }

        public int Delete(int seq)
        {
            string sql = @"delete cc from ConstCheckControlSt  cc 
                left join  ConstCheckRecResult cs
                on cc.Seq = cs.ControllStSeq
                left join ConstCheckRec cr
                on ( cr.Seq = cs.ConstCheckRecSeq　and  cr.CCRCheckType1 = 1 )
                where cc.Seq = @Seq　and cs.Seq is null";

            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }
        /*public int Add(ConstCheckControlStModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into ConstCheckControlSt (
                    DataType,
                    DataKeep,
                    ConstCheckListSeq,
                    CCManageItem1,
                    CCManageItem2,
                    CCCheckStand1,
                    CCCheckStand2,
                    CCCheckStand3,
                    CCCheckStand4,
                    CCCheckStand5,
                    CCCheckTiming,
                    CCCheckMethod,
                    CCCheckFeq,
                    CCIncomp,
                    CCManageRec,
                    CCType,
                    CCMemo,
                    CCCheckFields,
                    CCManageFields,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @DataType,
                    @DataKeep,
                    @ConstCheckListSeq,
                    @CCManageItem1,
                    @CCManageItem2,
                    @CCCheckStand1,
                    @CCCheckStand2,
                    @CCCheckStand3,
                    @CCCheckStand4,
                    @CCCheckStand5,
                    @CCCheckTiming,
                    @CCCheckMethod,
                    @CCCheckFeq,
                    @CCIncomp,
                    @CCManageRec,
                    @CCType,
                    @CCMemo,
                    @CCCheckFields,
                    @CCManageFields,
                    @OrderNo,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@DataType", m.DataType);
            cmd.Parameters.AddWithValue("@DataKeep", m.CCManageItem2);
            cmd.Parameters.AddWithValue("@ConstCheckListSeq", m.ConstCheckListSeq);
            cmd.Parameters.AddWithValue("@CCManageItem1", m.CCManageItem1);
            cmd.Parameters.AddWithValue("@CCManageItem2", m.CCManageItem2);
            cmd.Parameters.AddWithValue("@CCCheckStand1", m.CCCheckStand1);
            cmd.Parameters.AddWithValue("@CCCheckStand2", m.CCCheckStand2);
            cmd.Parameters.AddWithValue("@CCCheckStand3", m.CCCheckStand3);
            cmd.Parameters.AddWithValue("@CCCheckStand4", m.CCCheckStand4);
            cmd.Parameters.AddWithValue("@CCCheckStand5", m.CCCheckStand5);
            cmd.Parameters.AddWithValue("@CCCheckTiming", m.CCCheckTiming);
            cmd.Parameters.AddWithValue("@CCCheckMethod", m.CCCheckMethod);
            cmd.Parameters.AddWithValue("@CCCheckFeq", m.CCCheckFeq);
            cmd.Parameters.AddWithValue("@CCIncomp", m.CCIncomp);
            cmd.Parameters.AddWithValue("@CCManageRec", m.CCManageRec);
            cmd.Parameters.AddWithValue("@CCType", m.CCType);
            cmd.Parameters.AddWithValue("@CCMemo", m.CCMemo);
            cmd.Parameters.AddWithValue("@CCCheckFields", this.NulltoDBNull(m.CCCheckFields));
            cmd.Parameters.AddWithValue("@CCManageFields", this.NulltoDBNull(m.CCManageFields));
            cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('ConstCheckControlSt') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["NewSeq"].TCCtring());
        }*/
    }
}