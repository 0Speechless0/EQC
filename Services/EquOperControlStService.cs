using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EquOperControlStService : BaseService
    {//設備運轉抽查標準
        //s20230415
        public int ImportStdList(int equOperTestStSeq, List<QCStdModel> items)
        {
            string sql;
            db.BeginTransaction();
            try
            {
                sql = @"
                delete from EquOperControlSt where EquOperTestStSeq=@Seq
                ";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", equOperTestStSeq);
                db.ExecuteNonQuery(cmd);

                sql = @"
                insert into EquOperControlSt (
                    DataKeep,
                    EquOperTestStSeq,
                    EPCheckItem1,
                    EPStand1,
                    EPCheckTiming,
                    EPCheckMethod,
                    EPCheckFeq,
                    EPIncomp,
                    EPManageRec,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    1,
                    @EquOperTestStSeq,
                    @EPCheckItem1,
                    @EPStand1,
                    @EPCheckTiming,
                    @EPCheckMethod,
                    @EPCheckFeq,
                    @EPIncomp,
                    @EPManageRec,
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
                    cmd.Parameters.AddWithValue("@EquOperTestStSeq", equOperTestStSeq);
                    cmd.Parameters.AddWithValue("@EPCheckItem1", m.ManageItem);
                    cmd.Parameters.AddWithValue("@EPStand1", m.Stand);
                    cmd.Parameters.AddWithValue("@EPCheckTiming", m.CheckTiming);
                    cmd.Parameters.AddWithValue("@EPCheckMethod", m.CheckMethod);
                    cmd.Parameters.AddWithValue("@EPCheckFeq", m.CheckFeq);
                    cmd.Parameters.AddWithValue("@EPIncomp", m.Incomp);
                    cmd.Parameters.AddWithValue("@EPManageRec", m.ManageRec);
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
                log.Info("EquOperControlStService.ImportStdList: " + e.Message);
                return -1;
            }
        }
        //s20230414
        public List<T> GetStdList<T>(int EquOperTestStSeq)
        {
            string sql = @"
                SELECT
                    b.EPCheckItem1 ManageItem,
                    b.EPStand1 Stand,
                    b.EPCheckTiming CheckTiming,
                    b.EPCheckMethod CheckMethod,
                    b.EPCheckFeq CheckFeq,
                    b.EPIncomp Incomp,
                    b.EPManageRec ManageRec
                FROM EquOperTestList a
                inner join EquOperControlSt b on(b.EquOperTestStSeq=a.Seq)
                where a.Seq=@Seq
                order by b.DataKeep desc, b.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", EquOperTestStSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetList<T>(int EquOperTestStSeq)
        {
            string sql = @"SELECT * FROM EquOperControlSt
                where EquOperTestStSeq=@EquOperTestStSeq
                and DataKeep=1
                order by OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EquOperTestStSeq", EquOperTestStSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        public int GetEquOperControlStCount(int EquOperTestStSeq)
        {
            string sql = @"
                SELECT count(a.Seq) total FROM EquOperTestList a
                inner join EquOperControlSt b on(b.EquOperTestStSeq=a.Seq)
                where a.Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", EquOperTestStSeq);
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
        public List<T> GetList<T>(int EquOperTestStSeq, int pageIndex, int perPage)
        {
            string sql = @"
                SELECT
                    b.Seq,
                    b.DataType,
                    b.DataKeep,
                    b.EquOperTestStSeq,
                    b.EPCheckItem1,
                    b.EPCheckItem2,
                    b.EPStand1,
                    b.EPStand2,
                    b.EPStand3,
                    b.EPStand4,
                    b.EPStand5,
                    b.EPCheckTiming,
                    b.EPCheckMethod,
                    b.EPCheckFeq,
                    b.EPIncomp,
                    b.EPManageRec,
                    b.EPType,
                    b.EPMemo,
                    b.EPCheckFields,
                    b.EPManageFields,
                    b.OrderNo 
                FROM EquOperTestList a
                inner join EquOperControlSt b on(b.EquOperTestStSeq=a.Seq)
                where a.Seq=@Seq
                order by b.DataKeep desc, b.OrderNo
                OFFSET @pageIndex ROWS
				FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", EquOperTestStSeq);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //s20260626
        public List<T> GetListAll<T>(int equOperTestStSeq)
        {
            string sql = @"
                SELECT
                    b.Seq,
                    b.DataType,
                    b.DataKeep,
                    b.EquOperTestStSeq,
                    b.EPCheckItem1,
                    b.EPCheckItem2,
                    b.EPStand1,
                    b.EPStand2,
                    b.EPStand3,
                    b.EPStand4,
                    b.EPStand5,
                    b.EPCheckTiming,
                    b.EPCheckMethod,
                    b.EPCheckFeq,
                    b.EPIncomp,
                    b.EPManageRec,
                    b.EPType,
                    b.EPMemo,
                    b.EPCheckFields,
                    b.EPManageFields,
                    b.OrderNo 
                FROM EquOperControlSt b
                where b.EquOperTestStSeq=@EquOperTestStSeq
                order by b.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EquOperTestStSeq", equOperTestStSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        public List<T> GetItem<T>(int seq)
        {
            string sql = @"
                SELECT
                    Seq,
                    DataType,
                    DataKeep,
                    EquOperTestStSeq,
                    EPCheckItem1,
                    EPCheckItem2,
                    EPStand1,
                    EPStand2,
                    EPStand3,
                    EPStand4,
                    EPStand5,
                    EPCheckTiming,
                    EPCheckMethod,
                    EPCheckFeq,
                    EPIncomp,
                    EPManageRec,
                    EPType,
                    EPMemo,
                    EPCheckFields,
                    EPManageFields,
                    OrderNo 
                FROM EquOperControlSt
                where Seq=@Seq
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public int Insert(int EquOperTestStSeq, EquOperControlTpModel m)
        {
            //Null2Empty(m);
            string sql = @"
                insert into EquOperControlSt (
                    DataType,
                    EquOperTestStSeq,
                    EPCheckItem1,
                    EPCheckItem2,
                    EPStand1,
                    EPStand2,
                    EPStand3,
                    EPStand4,
                    EPStand5,
                    EPCheckTiming,
                    EPCheckMethod,
                    EPCheckFeq,
                    EPIncomp,
                    EPManageRec,
                    EPType,
                    EPMemo,
                    EPCheckFields,
                    EPManageFields,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    1,
                    @EquOperTestStSeq,
                    @EPCheckItem1,
                    @EPCheckItem2,
                    @EPStand1,
                    @EPStand2,
                    @EPStand3,
                    @EPStand4,
                    @EPStand5,
                    @EPCheckTiming,
                    @EPCheckMethod,
                    @EPCheckFeq,
                    @EPIncomp,
                    @EPManageRec,
                    @EPType,
                    @EPMemo,
                    @EPCheckFields,
                    @EPManageFields,
                    (select COALESCE(max(a.OrderNo),0)+1 from EquOperControlSt a where a.EquOperTestStSeq=@EquOperTestStSeq),
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EquOperTestStSeq", EquOperTestStSeq);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@EPCheckItem1", m.EPCheckItem1);
            cmd.Parameters.AddWithValue("@EPCheckItem2", m.EPCheckItem2);
            cmd.Parameters.AddWithValue("@EPStand1", m.EPStand1);
            cmd.Parameters.AddWithValue("@EPStand2", m.EPStand2);
            cmd.Parameters.AddWithValue("@EPStand3", m.EPStand3);
            cmd.Parameters.AddWithValue("@EPStand4", m.EPStand4);
            cmd.Parameters.AddWithValue("@EPStand5", m.EPStand5);
            cmd.Parameters.AddWithValue("@EPCheckTiming", m.EPCheckTiming);
            cmd.Parameters.AddWithValue("@EPCheckMethod", m.EPCheckMethod);
            cmd.Parameters.AddWithValue("@EPCheckFeq", m.EPCheckFeq);
            cmd.Parameters.AddWithValue("@EPIncomp", m.EPIncomp);
            cmd.Parameters.AddWithValue("@EPManageRec", m.EPManageRec);
            cmd.Parameters.AddWithValue("@EPType", this.NulltoDBNull(m.EPType));//s20230920
            cmd.Parameters.AddWithValue("@EPMemo", this.NulltoDBNull(m.EPMemo));//s20230920
            cmd.Parameters.AddWithValue("@EPCheckFields", this.NulltoDBNull(m.EPCheckFields));//s20230920
            cmd.Parameters.AddWithValue("@EPManageFields", this.NulltoDBNull(m.EPManageFields));//s20230920

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('EquOperControlSt') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
        }
        public int NewAdd(int EquOperTestStSeq)
        {
            //Null2Empty(m);
            string sql = @"
                insert into EquOperControlSt (
                    DataType,
                    EquOperTestStSeq,
                    EPType,
                    EPMemo,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    1,
                    @EquOperTestStSeq,
                    1,
                    0,
                    (select COALESCE(max(a.OrderNo),0)+1 from EquOperControlSt a where a.EquOperTestStSeq=@EquOperTestStSeq),
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EquOperTestStSeq", EquOperTestStSeq);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('EquOperControlSt') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
        }

        public bool Updates(List<EquOperControlStModel> items)
        {
            string sql = @"";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                foreach (EquOperControlStModel m in items)
                {
                    Null2Empty(m);
                    sql = @"
                        update EquOperControlSt set
                            DataType=@DataType,
                            DataKeep=@DataKeep,
                            EPCheckItem1 = @EPCheckItem1,
                            EPCheckItem2 = @EPCheckItem2,
                            EPStand1 = @EPStand1,
                            EPStand2 = @EPStand2,
                            EPStand3 = @EPStand3,
                            EPStand4 = @EPStand4,
                            EPStand5 = @EPStand5,
                            EPCheckTiming = @EPCheckTiming,
                            EPCheckMethod = @EPCheckMethod,
                            EPCheckFeq = @EPCheckFeq,
                            EPIncomp = @EPIncomp,
                            EPManageRec = @EPManageRec,
                            EPType = @EPType,
                            EPMemo = @EPMemo,
                            EPCheckFields = @EPCheckFields,
                            EPManageFields = @EPManageFields,
                            OrderNo = @OrderNo,
                            ModifyTime = GETDATE(),
                            ModifyUserSeq = @ModifyUserSeq
                        where Seq=@Seq";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@DataType", m.DataType);
                    cmd.Parameters.AddWithValue("@DataKeep", m.DataKeep);
                    cmd.Parameters.AddWithValue("@EPCheckItem1", m.EPCheckItem1);
                    cmd.Parameters.AddWithValue("@EPCheckItem2", m.EPCheckItem2);
                    cmd.Parameters.AddWithValue("@EPStand1", m.EPStand1);
                    cmd.Parameters.AddWithValue("@EPStand2", m.EPStand2);
                    cmd.Parameters.AddWithValue("@EPStand3", m.EPStand3);
                    cmd.Parameters.AddWithValue("@EPStand4", m.EPStand4);
                    cmd.Parameters.AddWithValue("@EPStand5", m.EPStand5);
                    cmd.Parameters.AddWithValue("@EPCheckTiming", m.EPCheckTiming);
                    cmd.Parameters.AddWithValue("@EPCheckMethod", m.EPCheckMethod);
                    cmd.Parameters.AddWithValue("@EPCheckFeq", m.EPCheckFeq);
                    cmd.Parameters.AddWithValue("@EPIncomp", m.EPIncomp);
                    cmd.Parameters.AddWithValue("@EPManageRec", m.EPManageRec);
                    cmd.Parameters.AddWithValue("@EPType", this.NulltoDBNull(m.EPType));//s20230920
                    cmd.Parameters.AddWithValue("@EPMemo", this.NulltoDBNull(m.EPMemo));//s20230920
                    cmd.Parameters.AddWithValue("@EPCheckFields", this.NulltoDBNull(m.EPCheckFields));
                    cmd.Parameters.AddWithValue("@EPManageFields", this.NulltoDBNull(m.EPManageFields));
                    cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    cmd.Parameters.AddWithValue("@Seq", m.Seq);

                    db.ExecuteNonQuery(cmd);
                }
                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("EquOperControlStService.Updates: " + e.Message);
                log.Info(sql);
                return false;
            }
        }

        public int Delete(int seq)
        {
            string sql = @"delete EquOperControlSt where Seq=@Seq";

            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }
        /*public int Add(EquOperControlStModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into EquOperControlSt (
                    DataType,
                    DataKeep,
                    EquOperTestStSeq,
                    EPCheckItem1,
                    EPCheckItem2,
                    EPStand1,
                    EPStand2,
                    EPStand3,
                    EPStand4,
                    EPStand5,
                    EPCheckTiming,
                    EPCheckMethod,
                    EPCheckFeq,
                    EPIncomp,
                    EPManageRec,
                    EPType,
                    EPMemo,
                    EPCheckFields,
                    EPManageFields,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @DataType,
                    @DataKeep,
                    @EquOperTestStSeq,
                    @EPCheckItem1,
                    @EPCheckItem2,
                    @EPStand1,
                    @EPStand2,
                    @EPStand3,
                    @EPStand4,
                    @EPStand5,
                    @EPCheckTiming,
                    @EPCheckMethod,
                    @EPCheckFeq,
                    @EPIncomp,
                    @EPManageRec,
                    @EPType,
                    @EPMemo,
                    @EPCheckFields,
                    @EPManageFields,
                    @OrderNo,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@DataType", m.DataType);
            cmd.Parameters.AddWithValue("@DataKeep", m.EPCheckItem2);
            cmd.Parameters.AddWithValue("@EquOperTestStSeq", m.EquOperTestStSeq);
            cmd.Parameters.AddWithValue("@EPCheckItem1", m.EPCheckItem1);
            cmd.Parameters.AddWithValue("@EPCheckItem2", m.EPCheckItem2);
            cmd.Parameters.AddWithValue("@EPStand1", m.EPStand1);
            cmd.Parameters.AddWithValue("@EPStand2", m.EPStand2);
            cmd.Parameters.AddWithValue("@EPStand3", m.EPStand3);
            cmd.Parameters.AddWithValue("@EPStand4", m.EPStand4);
            cmd.Parameters.AddWithValue("@EPStand5", m.EPStand5);
            cmd.Parameters.AddWithValue("@EPCheckTiming", m.EPCheckTiming);
            cmd.Parameters.AddWithValue("@EPCheckMethod", m.EPCheckMethod);
            cmd.Parameters.AddWithValue("@EPCheckFeq", m.EPCheckFeq);
            cmd.Parameters.AddWithValue("@EPIncomp", m.EPIncomp);
            cmd.Parameters.AddWithValue("@EPManageRec", m.EPManageRec);
            cmd.Parameters.AddWithValue("@EPType", m.EPType);
            cmd.Parameters.AddWithValue("@EPMemo", m.EPMemo);
            cmd.Parameters.AddWithValue("@EPCheckFields", this.NulltoDBNull(m.EPCheckFields));
            cmd.Parameters.AddWithValue("@EPManageFields", this.NulltoDBNull(m.EPManageFields));
            cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('EquOperControlSt') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["NewSeq"].TEPtring());
        }*/
    }
}