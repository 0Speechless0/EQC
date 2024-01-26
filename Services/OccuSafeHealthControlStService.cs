using EQC.Common;
using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class OccuSafeHealthControlStService : BaseService
    {//職業安全衛生抽查標準範本
        //s20230415
        public int ImportStdList(int occuSafeHealthListSeq, List<QCStdModel> items)
        {
            string sql;
            db.BeginTransaction();
            try
            {
                sql = @"
                delete from OccuSafeHealthControlSt where OccuSafeHealthListSeq=@Seq
                ";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", occuSafeHealthListSeq);
                db.ExecuteNonQuery(cmd);

                sql = @"
                insert into OccuSafeHealthControlSt (
                    DataKeep,
                    OccuSafeHealthListSeq,
                    OSCheckItem1,
                    OSStand1,
                    OSCheckTiming,
                    OSCheckMethod,
                    OSCheckFeq,
                    OSIncomp,
                    OSManageRec,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    1,
                    @OccuSafeHealthListSeq,
                    @OSCheckItem1,
                    @OSStand1,
                    @OSCheckTiming,
                    @OSCheckMethod,
                    @OSCheckFeq,
                    @OSIncomp,
                    @OSManageRec,
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
                    cmd.Parameters.AddWithValue("@OccuSafeHealthListSeq", occuSafeHealthListSeq);
                    cmd.Parameters.AddWithValue("@OSCheckItem1", m.ManageItem);
                    cmd.Parameters.AddWithValue("@OSStand1", m.Stand);
                    cmd.Parameters.AddWithValue("@OSCheckTiming", m.CheckTiming);
                    cmd.Parameters.AddWithValue("@OSCheckMethod", m.CheckMethod);
                    cmd.Parameters.AddWithValue("@OSCheckFeq", m.CheckFeq);
                    cmd.Parameters.AddWithValue("@OSIncomp", m.Incomp);
                    cmd.Parameters.AddWithValue("@OSManageRec", m.ManageRec);
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
                log.Info("OccuSafeHealthControlStService.ImportStdList: " + e.Message);
                return -1;
            }
        }
        //s20230414
        public List<T> GetStdList<T>(int OccuSafeHealthListSeq)
        {
            string sql = @"
                SELECT
                    b.OSCheckItem1 ManageItem,
                    b.OSStand1 Stand,
                    b.OSCheckTiming CheckTiming,
                    b.OSCheckMethod CheckMethod,
                    b.OSCheckFeq CheckFeq,
                    b.OSIncomp Incomp,
                    b.OSManageRec ManageRec
                FROM OccuSafeHealthList a
                inner join OccuSafeHealthControlSt b on(b.OccuSafeHealthListSeq=a.Seq)
                where a.Seq=@Seq
                order by b.DataKeep desc, b.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", OccuSafeHealthListSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetList<T>(int occuSafeHealthListSeq)
        {
            string sql = @"SELECT * FROM OccuSafeHealthControlSt
                where OccuSafeHealthListSeq=@OccuSafeHealthListSeq
                and DataKeep=1
                order by OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@OccuSafeHealthListSeq", occuSafeHealthListSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        public int GetOccuSafeHealthControlStCount(int OccuSafeHealthListSeq)
        {
            string sql = @"
                SELECT count(a.Seq) total FROM OccuSafeHealthList a
                inner join OccuSafeHealthControlSt b on(b.OccuSafeHealthListSeq=a.Seq)
                where a.Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", OccuSafeHealthListSeq);
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
        public List<T> GetList<T>(int OccuSafeHealthListSeq, int pageIndex, int perPage)
        {
            string sql = @"
                SELECT
                    b.Seq,
                    b.DataType,
                    b.DataKeep,
                    b.OccuSafeHealthListSeq,
                    b.OSCheckItem1,
                    b.OSCheckItem2,
                    b.OSStand1,
                    b.OSStand2,
                    b.OSStand3,
                    b.OSStand4,
                    b.OSStand5,
                    b.OSCheckTiming,
                    b.OSCheckMethod,
                    b.OSCheckFeq,
                    b.OSIncomp,
                    b.OSManageRec,
                    b.OSType,
                    b.OSMemo,
                    b.OSCheckFields,
                    b.OSManageFields,
                    b.OrderNo 
                FROM OccuSafeHealthList a
                inner join OccuSafeHealthControlSt b on(b.OccuSafeHealthListSeq=a.Seq)
                where a.Seq=@Seq
                order by b.DataKeep desc, b.OrderNo
                OFFSET @pageIndex ROWS
				FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", OccuSafeHealthListSeq);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //s20230626
        public List<T> GetListAll<T>(int occuSafeHealthListSeq)
        {
            string sql = @"
                SELECT
                    b.Seq,
                    b.DataType,
                    b.DataKeep,
                    b.OccuSafeHealthListSeq,
                    b.OSCheckItem1,
                    b.OSCheckItem2,
                    b.OSStand1,
                    b.OSStand2,
                    b.OSStand3,
                    b.OSStand4,
                    b.OSStand5,
                    b.OSCheckTiming,
                    b.OSCheckMethod,
                    b.OSCheckFeq,
                    b.OSIncomp,
                    b.OSManageRec,
                    b.OSType,
                    b.OSMemo,
                    b.OSCheckFields,
                    b.OSManageFields,
                    b.OrderNo 
                FROM OccuSafeHealthControlSt b
                where b.OccuSafeHealthListSeq=@OccuSafeHealthListSeq
                order by b.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@OccuSafeHealthListSeq", occuSafeHealthListSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        public List<T> GetItem<T>(int seq)
        {
            string sql = @"
                SELECT
                    Seq,
                    DataType,
                    DataKeep,
                    OccuSafeHealthListSeq,
                    OSCheckItem1,
                    OSCheckItem2,
                    OSStand1,
                    OSStand2,
                    OSStand3,
                    OSStand4,
                    OSStand5,
                    OSCheckTiming,
                    OSCheckMethod,
                    OSCheckFeq,
                    OSIncomp,
                    OSManageRec,
                    OSType,
                    OSMemo,
                    OSCheckFields,
                    OSManageFields,
                    OrderNo 
                FROM OccuSafeHealthControlSt
                where Seq=@Seq
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public int Insert(int occuSafeHealthListSeq, OccuSafeHealthControlTpModel m)
        {
            //Null2Empty(m);
            string sql = @"
                insert into OccuSafeHealthControlSt (
                    DataType,
                    OccuSafeHealthListSeq,
                    OSCheckItem1,
                    OSCheckItem2,
                    OSStand1,
                    OSStand2,
                    OSStand3,
                    OSStand4,
                    OSStand5,
                    OSCheckTiming,
                    OSCheckMethod,
                    OSCheckFeq,
                    OSIncomp,
                    OSManageRec,
                    OSType,
                    OSMemo,
                    OSCheckFields,
                    OSManageFields,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    1,
                    @OccuSafeHealthListSeq,
                    @OSCheckItem1,
                    @OSCheckItem2,
                    @OSStand1,
                    @OSStand2,
                    @OSStand3,
                    @OSStand4,
                    @OSStand5,
                    @OSCheckTiming,
                    @OSCheckMethod,
                    @OSCheckFeq,
                    @OSIncomp,
                    @OSManageRec,
                    @OSType,
                    @OSMemo,
                    @OSCheckFields,
                    @OSManageFields,
                    (select COALESCE(max(a.OrderNo),0)+1 from OccuSafeHealthControlSt a where a.OccuSafeHealthListSeq=@OccuSafeHealthListSeq),
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@OccuSafeHealthListSeq", Utils.NulltoDBNull(occuSafeHealthListSeq));
            cmd.Parameters.AddWithValue("@OSCheckItem1", Utils.NulltoDBNull(m.OSCheckItem1));
            cmd.Parameters.AddWithValue("@OSCheckItem2", Utils.NulltoDBNull( m.OSCheckItem2) );
            cmd.Parameters.AddWithValue("@OSStand1", Utils.NulltoDBNull(m.OSStand1) );
            cmd.Parameters.AddWithValue("@OSStand2", Utils.NulltoDBNull(m.OSStand2));
            cmd.Parameters.AddWithValue("@OSStand3", Utils.NulltoDBNull(m.OSStand3));
            cmd.Parameters.AddWithValue("@OSStand4", Utils.NulltoDBNull(m.OSStand4));
            cmd.Parameters.AddWithValue("@OSStand5", Utils.NulltoDBNull(m.OSStand5));
            cmd.Parameters.AddWithValue("@OSCheckTiming", Utils.NulltoDBNull(m.OSCheckTiming));
            cmd.Parameters.AddWithValue("@OSCheckMethod", Utils.NulltoDBNull(m.OSCheckMethod));
            cmd.Parameters.AddWithValue("@OSCheckFeq", Utils.NulltoDBNull(m.OSCheckFeq));
            cmd.Parameters.AddWithValue("@OSIncomp", Utils.NulltoDBNull(m.OSIncomp));
            cmd.Parameters.AddWithValue("@OSManageRec", Utils.NulltoDBNull(m.OSManageRec));
            cmd.Parameters.AddWithValue("@OSType", Utils.NulltoDBNull(m.OSType));
            cmd.Parameters.AddWithValue("@OSMemo", Utils.NulltoDBNull(m.OSMemo));
            cmd.Parameters.AddWithValue("@OSCheckFields", Utils.NulltoDBNull(m.OSCheckFields));
            cmd.Parameters.AddWithValue("@OSManageFields", Utils.NulltoDBNull(m.OSManageFields));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('OccuSafeHealthControlSt') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
        }
        public int NewAdd(int occuSafeHealthListSeq)
        {
            //Null2Empty(m);
            string sql = @"
                insert into OccuSafeHealthControlSt (
                    DataType,
                    OccuSafeHealthListSeq,
                    OSType,
                    OSMemo,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    1,
                    @OccuSafeHealthListSeq,
                    1,
                    0,
                    (select COALESCE(max(a.OrderNo),0)+1 from OccuSafeHealthControlSt a where a.OccuSafeHealthListSeq=@OccuSafeHealthListSeq),
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@OccuSafeHealthListSeq", occuSafeHealthListSeq);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('OccuSafeHealthControlSt') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
        }

        public bool Updates(List<OccuSafeHealthControlStModel> items)
        {
            string sql = @"";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                foreach (OccuSafeHealthControlStModel m in items)
                {
                    Null2Empty(m);
                    sql = @"
                        update OccuSafeHealthControlSt set
                            DataType=@DataType,
                            DataKeep=@DataKeep,
                            OSCheckItem1 = @OSCheckItem1,
                            OSCheckItem2 = @OSCheckItem2,
                            OSStand1 = @OSStand1,
                            OSStand2 = @OSStand2,
                            OSStand3 = @OSStand3,
                            OSStand4 = @OSStand4,
                            OSStand5 = @OSStand5,
                            OSCheckTiming = @OSCheckTiming,
                            OSCheckMethod = @OSCheckMethod,
                            OSCheckFeq = @OSCheckFeq,
                            OSIncomp = @OSIncomp,
                            OSManageRec = @OSManageRec,
                            OSType = @OSType,
                            OSMemo = @OSMemo,
                            OSCheckFields = @OSCheckFields,
                            OSManageFields = @OSManageFields,
                            OrderNo = @OrderNo,
                            ModifyTime = GETDATE(),
                            ModifyUserSeq = @ModifyUserSeq
                        Where Seq = @Seq";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@DataType", m.DataType);
                    cmd.Parameters.AddWithValue("@DataKeep", m.DataKeep);
                    cmd.Parameters.AddWithValue("@OSCheckItem1", m.OSCheckItem1);
                    cmd.Parameters.AddWithValue("@OSCheckItem2", m.OSCheckItem2);
                    cmd.Parameters.AddWithValue("@OSStand1", m.OSStand1);
                    cmd.Parameters.AddWithValue("@OSStand2", m.OSStand2);
                    cmd.Parameters.AddWithValue("@OSStand3", m.OSStand3);
                    cmd.Parameters.AddWithValue("@OSStand4", m.OSStand4);
                    cmd.Parameters.AddWithValue("@OSStand5", m.OSStand5);
                    cmd.Parameters.AddWithValue("@OSCheckTiming", m.OSCheckTiming);
                    cmd.Parameters.AddWithValue("@OSCheckMethod", m.OSCheckMethod);
                    cmd.Parameters.AddWithValue("@OSCheckFeq", m.OSCheckFeq);
                    cmd.Parameters.AddWithValue("@OSIncomp", m.OSIncomp);
                    cmd.Parameters.AddWithValue("@OSManageRec", m.OSManageRec);
                    cmd.Parameters.AddWithValue("@OSType", this.NulltoDBNull(m.OSType));//s20230920
                    cmd.Parameters.AddWithValue("@OSMemo", m.OSMemo);
                    cmd.Parameters.AddWithValue("@OSCheckFields", this.NulltoDBNull(m.OSCheckFields));
                    cmd.Parameters.AddWithValue("@OSManageFields", this.NulltoDBNull(m.OSManageFields));
                    cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    cmd.Parameters.AddWithValue("@Seq", m.Seq);

                    db.ExecuteNonQuery(cmd);
                }
                    db.TransactionCommit();
                    return true;
            } catch (Exception e) {
                db.TransactionRollback();
                log.Info("OccuSafeHealthControlStService.Updates: " + e.Message);
                log.Info(sql);
                return false;
            }
        }

        public int Delete(int seq)
        {
            string sql = @"delete cc from OccuSafeHealthControlSt  cc 
                left join  ConstCheckRecResult cs
                on cc.Seq = cs.ControllStSeq
                left join ConstCheckRec cr
                on ( cr.Seq = cs.ConstCheckRecSeq　and  cr.CCRCheckType1 = 3 )
                where cc.Seq = @Seq　and cs.Seq is null";
            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }
        /*public int Add(OccuSafeHealthControlStModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into OccuSafeHealthControlSt (
                    DataType,
                    DataKeep,
                    OccuSafeHealthListSeq,
                    OSCheckItem1,
                    OSCheckItem2,
                    OSStand1,
                    OSStand2,
                    OSStand3,
                    OSStand4,
                    OSStand5,
                    OSCheckTiming,
                    OSCheckMethod,
                    OSCheckFeq,
                    OSIncomp,
                    OSManageRec,
                    OSType,
                    OSMemo,
                    OSCheckFields,
                    OSManageFields,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @DataType,
                    @DataKeep,
                    @OccuSafeHealthListSeq,
                    @OSCheckItem1,
                    @OSCheckItem2,
                    @OSStand1,
                    @OSStand2,
                    @OSStand3,
                    @OSStand4,
                    @OSStand5,
                    @OSCheckTiming,
                    @OSCheckMethod,
                    @OSCheckFeq,
                    @OSIncomp,
                    @OSManageRec,
                    @OSType,
                    @OSMemo,
                    @OSCheckFields,
                    @OSManageFields,
                    @OrderNo,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@DataType", m.DataType);
            cmd.Parameters.AddWithValue("@DataKeep", m.OSCheckItem2);
            cmd.Parameters.AddWithValue("@OccuSafeHealthListSeq", m.OccuSafeHealthListSeq);
            cmd.Parameters.AddWithValue("@OSCheckItem1", m.OSCheckItem1);
            cmd.Parameters.AddWithValue("@OSCheckItem2", m.OSCheckItem2);
            cmd.Parameters.AddWithValue("@OSStand1", m.OSStand1);
            cmd.Parameters.AddWithValue("@OSStand2", m.OSStand2);
            cmd.Parameters.AddWithValue("@OSStand3", m.OSStand3);
            cmd.Parameters.AddWithValue("@OSStand4", m.OSStand4);
            cmd.Parameters.AddWithValue("@OSStand5", m.OSStand5);
            cmd.Parameters.AddWithValue("@OSCheckTiming", m.OSCheckTiming);
            cmd.Parameters.AddWithValue("@OSCheckMethod", m.OSCheckMethod);
            cmd.Parameters.AddWithValue("@OSCheckFeq", m.OSCheckFeq);
            cmd.Parameters.AddWithValue("@OSIncomp", m.OSIncomp);
            cmd.Parameters.AddWithValue("@OSManageRec", m.OSManageRec);
            cmd.Parameters.AddWithValue("@OSType", m.OSType);
            cmd.Parameters.AddWithValue("@OSMemo", m.OSMemo);
            cmd.Parameters.AddWithValue("@OSCheckFields", this.NulltoDBNull(m.OSCheckFields));
            cmd.Parameters.AddWithValue("@OSManageFields", this.NulltoDBNull(m.OSManageFields));
            cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('OccuSafeHealthControlSt') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
        }*/
    }
}
