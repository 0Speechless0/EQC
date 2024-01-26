using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EnvirConsControlStService : BaseService
    {//環境保育抽查標準
        //s20230415
        public int ImportStdList(int envirConsListSeq, List<QCStdModel> items)
        {
            string sql;
            db.BeginTransaction();
            try
            {
                sql = @"
                delete from EnvirConsControlSt where EnvirConsListSeq=@Seq
                ";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", envirConsListSeq);
                db.ExecuteNonQuery(cmd);

                sql = @"
                insert into EnvirConsControlSt (
                    DataKeep,
                    EnvirConsListSeq,
                    ECCFlow1,
                    ECCCheckItem1,
                    ECCStand1,
                    ECCCheckTiming,
                    ECCCheckMethod,
                    ECCCheckFeq,
                    ECCIncomp,
                    ECCManageRec,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    1,
                    @EnvirConsListSeq,
                    @ECCFlow1,
                    @ECCCheckItem1,
                    @ECCStand1,
                    @ECCCheckTiming,
                    @ECCCheckMethod,
                    @ECCCheckFeq,
                    @ECCIncomp,
                    @ECCManageRec,
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
                    cmd.Parameters.AddWithValue("@EnvirConsListSeq", envirConsListSeq);
                    cmd.Parameters.AddWithValue("@ECCFlow1", m.FlowType);
                    cmd.Parameters.AddWithValue("@ECCCheckItem1", m.ManageItem);
                    cmd.Parameters.AddWithValue("@ECCStand1", m.Stand);
                    cmd.Parameters.AddWithValue("@ECCCheckTiming", m.CheckTiming);
                    cmd.Parameters.AddWithValue("@ECCCheckMethod", m.CheckMethod);
                    cmd.Parameters.AddWithValue("@ECCCheckFeq", m.CheckFeq);
                    cmd.Parameters.AddWithValue("@ECCIncomp", m.Incomp);
                    cmd.Parameters.AddWithValue("@ECCManageRec", m.ManageRec);
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
                log.Info("EnvirConsControlStService.ImportStdList: " + e.Message);
                return -1;
            }
        }
        //s20230414
        public List<T> GetStdList<T>(int EnvirConsListSeq)
        {
            string sql = @"
                SELECT
                    b.ECCFlow1 FlowType,
                    b.ECCCheckItem1 ManageItem,
                    b.ECCStand1 Stand,
                    b.ECCCheckTiming CheckTiming,
                    b.ECCCheckMethod CheckMethod,
                    b.ECCCheckFeq CheckFeq,
                    b.ECCIncomp Incomp,
                    b.ECCManageRec ManageRec
                FROM EnvirConsList a
                inner join EnvirConsControlSt b on(b.EnvirConsListSeq=a.Seq)
                where a.Seq=@Seq
                order by b.DataKeep desc, b.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", EnvirConsListSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetList<T>(int EnvirConsListSeq)
        {
            string sql = @"SELECT * FROM EnvirConsControlSt
                where EnvirConsListSeq=@EnvirConsListSeq
                and DataKeep=1
                order by OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EnvirConsListSeq", EnvirConsListSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        public int GetEnvirConsControlStCount(int EnvirConsListSeq)
        {
            string sql = @"
                SELECT count(a.Seq) total FROM EnvirConsList a
                inner join EnvirConsControlSt b on(b.EnvirConsListSeq=a.Seq)
                where a.Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", EnvirConsListSeq);
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
        public List<T> GetList<T>(int EnvirConsListSeq, int pageIndex, int perPage)
        {
            string sql = @"
                SELECT
                    b.Seq,
                    b.DataType,
                    b.DataKeep,
                    b.EnvirConsListSeq,
                    b.ECCFlow1,
                    b.ECCCheckItem1,
                    b.ECCCheckItem2,
                    b.ECCStand1,
                    b.ECCStand2,
                    b.ECCStand3,
                    b.ECCStand4,
                    b.ECCStand5,
                    b.ECCCheckTiming,
                    b.ECCCheckMethod,
                    b.ECCCheckFeq,
                    b.ECCIncomp,
                    b.ECCManageRec,
                    b.ECCType,
                    b.ECCMemo,
                    b.ECCCheckFields,
                    b.ECCManageFields,
                    b.OrderNo 
                FROM EnvirConsList a
                inner join EnvirConsControlSt b on(b.EnvirConsListSeq=a.Seq)
                where a.Seq=@Seq
                order by b.DataKeep desc, b.OrderNo
                OFFSET @pageIndex ROWS
				FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", EnvirConsListSeq);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //20230626
        public List<T> GetListAll<T>(int envirConsListSeq)
        {
            string sql = @"
                SELECT
                    b.Seq,
                    b.DataType,
                    b.DataKeep,
                    b.EnvirConsListSeq,
                    b.ECCFlow1,
                    b.ECCCheckItem1,
                    b.ECCCheckItem2,
                    b.ECCStand1,
                    b.ECCStand2,
                    b.ECCStand3,
                    b.ECCStand4,
                    b.ECCStand5,
                    b.ECCCheckTiming,
                    b.ECCCheckMethod,
                    b.ECCCheckFeq,
                    b.ECCIncomp,
                    b.ECCManageRec,
                    b.ECCType,
                    b.ECCMemo,
                    b.ECCCheckFields,
                    b.ECCManageFields,
                    b.OrderNo 
                FROM EnvirConsControlSt b
                where b.EnvirConsListSeq=@EnvirConsListSeq
                order by b.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EnvirConsListSeq", envirConsListSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        public List<T> GetItem<T>(int seq)
        {
            string sql = @"
                SELECT
                    Seq,
                    DataType,
                    DataKeep,
                    ECCFlow1,
                    EnvirConsListSeq,
                    ECCCheckItem1,
                    ECCCheckItem2,
                    ECCStand1,
                    ECCStand2,
                    ECCStand3,
                    ECCStand4,
                    ECCStand5,
                    ECCCheckTiming,
                    ECCCheckMethod,
                    ECCCheckFeq,
                    ECCIncomp,
                    ECCManageRec,
                    ECCType,
                    ECCMemo,
                    ECCCheckFields,
                    ECCManageFields,
                    OrderNo 
                FROM EnvirConsControlSt
                where Seq=@Seq
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public int Insert(int EnvirConsListSeq, EnvirConsControlTpModel m)
        {
            //Null2Empty(m);
            string sql = @"
                insert into EnvirConsControlSt (
                    DataType,
                    EnvirConsListSeq,
                    ECCFlow1,
                    ECCCheckItem1,
                    ECCCheckItem2,
                    ECCStand1,
                    ECCStand2,
                    ECCStand3,
                    ECCStand4,
                    ECCStand5,
                    ECCCheckTiming,
                    ECCCheckMethod,
                    ECCCheckFeq,
                    ECCIncomp,
                    ECCManageRec,
                    ECCType,
                    ECCMemo,
                    ECCCheckFields,
                    ECCManageFields,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    1,
                    @EnvirConsListSeq,
                    @ECCFlow1,
                    @ECCCheckItem1,
                    @ECCCheckItem2,
                    @ECCStand1,
                    @ECCStand2,
                    @ECCStand3,
                    @ECCStand4,
                    @ECCStand5,
                    @ECCCheckTiming,
                    @ECCCheckMethod,
                    @ECCCheckFeq,
                    @ECCIncomp,
                    @ECCManageRec,
                    @ECCType,
                    @ECCMemo,
                    @ECCCheckFields,
                    @ECCManageFields,
                    (select COALESCE(max(a.OrderNo),0)+1 from EnvirConsControlSt a where a.EnvirConsListSeq=@EnvirConsListSeq),
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ECCFlow1", m.ECCFlow1);
            cmd.Parameters.AddWithValue("@ECCCheckItem1", m.ECCCheckItem1);
            cmd.Parameters.AddWithValue("@ECCCheckItem2", this.NulltoDBNull(m.ECCCheckItem2));//s20230920
            cmd.Parameters.AddWithValue("@ECCStand1", this.NulltoDBNull(m.ECCStand1));//s20231006
            cmd.Parameters.AddWithValue("@ECCStand2", this.NulltoDBNull(m.ECCStand2));//s20231006
            cmd.Parameters.AddWithValue("@ECCStand3", this.NulltoDBNull(m.ECCStand3));//s20231006
            cmd.Parameters.AddWithValue("@ECCStand4", this.NulltoDBNull(m.ECCStand4));//s20231006
            cmd.Parameters.AddWithValue("@ECCStand5", this.NulltoDBNull(m.ECCStand5));//s20231006
            cmd.Parameters.AddWithValue("@ECCCheckTiming", m.ECCCheckTiming);
            cmd.Parameters.AddWithValue("@ECCCheckMethod", m.ECCCheckMethod);
            cmd.Parameters.AddWithValue("@ECCCheckFeq", m.ECCCheckFeq);
            cmd.Parameters.AddWithValue("@ECCIncomp", m.ECCIncomp);
            cmd.Parameters.AddWithValue("@ECCManageRec", m.ECCManageRec);
            cmd.Parameters.AddWithValue("@ECCType", this.NulltoDBNull(m.ECCType));//s20230920
            cmd.Parameters.AddWithValue("@ECCMemo", m.ECCMemo);
            cmd.Parameters.AddWithValue("@ECCCheckFields", this.NulltoDBNull(m.ECCCheckFields));//s20230920
            cmd.Parameters.AddWithValue("@ECCManageFields", this.NulltoDBNull(m.ECCManageFields));//s20230920
            cmd.Parameters.AddWithValue("@EnvirConsListSeq", EnvirConsListSeq);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('EnvirConsControlSt') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
        }
        public int NewAdd(int EnvirConsListSeq)
        {
            //Null2Empty(m);
            string sql = @"
                insert into EnvirConsControlSt (
                    DataType,
                    EnvirConsListSeq,
                    ECCFlow1,
                    ECCType,
                    ECCMemo,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    1,
                    @EnvirConsListSeq,
                    1,
                    1,
                    0,
                    (select COALESCE(max(a.OrderNo),0)+1 from EnvirConsControlSt a where a.EnvirConsListSeq=@EnvirConsListSeq),
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EnvirConsListSeq", EnvirConsListSeq);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('EnvirConsControlSt') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
        }

        public bool Updates(List<EnvirConsControlStModel> items)
        {
            string sql = @"";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                foreach (EnvirConsControlStModel m in items)
                {
                    Null2Empty(m);
                    sql = @"
                        update EnvirConsControlSt set
                            DataType=@DataType,
                            DataKeep=@DataKeep,
                            ECCFlow1=@ECCFlow1,
                            ECCCheckItem1 = @ECCCheckItem1,
                            ECCCheckItem2 = @ECCCheckItem2,
                            ECCStand1 = @ECCStand1,
                            ECCStand2 = @ECCStand2,
                            ECCStand3 = @ECCStand3,
                            ECCStand4 = @ECCStand4,
                            ECCStand5 = @ECCStand5,
                            ECCCheckTiming = @ECCCheckTiming,
                            ECCCheckMethod = @ECCCheckMethod,
                            ECCCheckFeq = @ECCCheckFeq,
                            ECCIncomp = @ECCIncomp,
                            ECCManageRec = @ECCManageRec,
                            ECCType = @ECCType,
                            ECCMemo = @ECCMemo,
                            ECCCheckFields = @ECCCheckFields,
                            ECCManageFields = @ECCManageFields,
                            OrderNo = @OrderNo,
                            ModifyTime = GETDATE(),
                            ModifyUserSeq = @ModifyUserSeq
                        Where Seq = @Seq";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@DataType", m.DataType);
                    cmd.Parameters.AddWithValue("@DataKeep", m.DataKeep);
                    cmd.Parameters.AddWithValue("@ECCFlow1", m.ECCFlow1); 
                    cmd.Parameters.AddWithValue("@ECCCheckItem1", m.ECCCheckItem1);
                    cmd.Parameters.AddWithValue("@ECCCheckItem2", this.NulltoDBNull(m.ECCCheckItem2));//s20231006
                    cmd.Parameters.AddWithValue("@ECCStand1", this.NulltoDBNull(m.ECCStand1));//s20231006
                    cmd.Parameters.AddWithValue("@ECCStand2", this.NulltoDBNull(m.ECCStand2));//s20231006
                    cmd.Parameters.AddWithValue("@ECCStand3", this.NulltoDBNull(m.ECCStand3));//s20231006
                    cmd.Parameters.AddWithValue("@ECCStand4", this.NulltoDBNull(m.ECCStand4));//s20231006
                    cmd.Parameters.AddWithValue("@ECCStand5", this.NulltoDBNull(m.ECCStand5));//s20231006
                    cmd.Parameters.AddWithValue("@ECCCheckTiming", m.ECCCheckTiming);
                    cmd.Parameters.AddWithValue("@ECCCheckMethod", m.ECCCheckMethod);
                    cmd.Parameters.AddWithValue("@ECCCheckFeq", m.ECCCheckFeq);
                    cmd.Parameters.AddWithValue("@ECCIncomp", m.ECCIncomp);
                    cmd.Parameters.AddWithValue("@ECCManageRec", m.ECCManageRec);
                    cmd.Parameters.AddWithValue("@ECCType", this.NulltoDBNull(m.ECCType));//s20230920
                    cmd.Parameters.AddWithValue("@ECCMemo", m.ECCMemo);
                    cmd.Parameters.AddWithValue("@ECCCheckFields", this.NulltoDBNull(m.ECCCheckFields));
                    cmd.Parameters.AddWithValue("@ECCManageFields", this.NulltoDBNull(m.ECCManageFields));
                    cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    cmd.Parameters.AddWithValue("@Seq", m.Seq);

                    db.ExecuteNonQuery(cmd);
                }
                db.TransactionCommit();
                return true;
            } catch (Exception e) {
                db.TransactionRollback();
                log.Info("EnvirConsControlStService.Updates: " + e.Message);
                log.Info(sql);
                return false;
            }
        }

        public int Delete(int seq)
        {
            string sql = @"delete cc from EnvirConsControlSt  cc 
                left join  ConstCheckRecResult cs
                on cc.Seq = cs.ControllStSeq
                left join ConstCheckRec cr
                on ( cr.Seq = cs.ConstCheckRecSeq　and  cr.CCRCheckType1 = 4 )
                where cc.Seq = @Seq　and cs.Seq is null";
            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }
        /*public int Add(EnvirConsControlStModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into EnvirConsControlSt (
                    DataType,
                    DataKeep,
                    EnvirConsListSeq,
                    ECCCheckItem1,
                    ECCCheckItem2,
                    ECCStand1,
                    ECCStand2,
                    ECCStand3,
                    ECCStand4,
                    ECCStand5,
                    ECCCheckTiming,
                    ECCCheckMethod,
                    ECCCheckFeq,
                    ECCIncomp,
                    ECCManageRec,
                    ECCType,
                    ECCMemo,
                    ECCCheckFields,
                    ECCManageFields,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @DataType,
                    @DataKeep,
                    @EnvirConsListSeq,
                    @ECCCheckItem1,
                    @ECCCheckItem2,
                    @ECCStand1,
                    @ECCStand2,
                    @ECCStand3,
                    @ECCStand4,
                    @ECCStand5,
                    @ECCCheckTiming,
                    @ECCCheckMethod,
                    @ECCCheckFeq,
                    @ECCIncomp,
                    @ECCManageRec,
                    @ECCType,
                    @ECCMemo,
                    @ECCCheckFields,
                    @ECCManageFields,
                    @OrderNo,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@DataType", m.DataType);
            cmd.Parameters.AddWithValue("@DataKeep", m.ECCCheckItem2);
            cmd.Parameters.AddWithValue("@EnvirConsListSeq", m.EnvirConsListSeq);
            cmd.Parameters.AddWithValue("@ECCCheckItem1", m.ECCCheckItem1);
            cmd.Parameters.AddWithValue("@ECCCheckItem2", m.ECCCheckItem2);
            cmd.Parameters.AddWithValue("@ECCStand1", m.ECCStand1);
            cmd.Parameters.AddWithValue("@ECCStand2", m.ECCStand2);
            cmd.Parameters.AddWithValue("@ECCStand3", m.ECCStand3);
            cmd.Parameters.AddWithValue("@ECCStand4", m.ECCStand4);
            cmd.Parameters.AddWithValue("@ECCStand5", m.ECCStand5);
            cmd.Parameters.AddWithValue("@ECCCheckTiming", m.ECCCheckTiming);
            cmd.Parameters.AddWithValue("@ECCCheckMethod", m.ECCCheckMethod);
            cmd.Parameters.AddWithValue("@ECCCheckFeq", m.ECCCheckFeq);
            cmd.Parameters.AddWithValue("@ECCIncomp", m.ECCIncomp);
            cmd.Parameters.AddWithValue("@ECCManageRec", m.ECCManageRec);
            cmd.Parameters.AddWithValue("@ECCType", m.ECCType);
            cmd.Parameters.AddWithValue("@ECCMemo", m.ECCMemo);
            cmd.Parameters.AddWithValue("@ECCCheckFields", this.NulltoDBNull(m.ECCCheckFields));
            cmd.Parameters.AddWithValue("@ECCManageFields", this.NulltoDBNull(m.ECCManageFields));
            cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('EnvirConsControlSt') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["NewSeq"].TECCtring());
        }*/
    }
}