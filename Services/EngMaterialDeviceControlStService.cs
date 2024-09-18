using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EngMaterialDeviceControlStService : BaseService
    {//材料設備抽查管理標準
        //s20230415
        public int ImportStdList(int engMaterialDeviceListSeq, List<QCStdModel> items)
        {
            string sql;
            db.BeginTransaction();
            try
            {
                sql = @"
                delete from EngMaterialDeviceControlSt where EngMaterialDeviceListSeq=@Seq
                ";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", engMaterialDeviceListSeq);
                db.ExecuteNonQuery(cmd);

                sql = @"
                insert into EngMaterialDeviceControlSt (
                    DataKeep,
                    EngMaterialDeviceListSeq,
                    MDTestItem,
                    MDTestStand1,
                    MDTestTime,
                    MDTestMethod,
                    MDTestFeq,
                    MDIncomp,
                    MDManageRec,
                    MDMemo,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    1,
                    @EngMaterialDeviceListSeq,
                    @MDTestItem,
                    @MDTestStand1,
                    @MDTestTime,
                    @MDTestMethod,
                    @MDTestFeq,
                    @MDIncomp,
                    @MDManageRec,
                    @MDMemo,
                    @OrderNo,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
                int orderNo = 1;
                foreach(QCStdModel m in items) {
                    this.Null2Empty(m);

                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", engMaterialDeviceListSeq);
                    cmd.Parameters.AddWithValue("@MDTestItem", m.ManageItem);
                    cmd.Parameters.AddWithValue("@MDTestStand1", m.Stand);
                    cmd.Parameters.AddWithValue("@MDTestTime", m.CheckTiming);
                    cmd.Parameters.AddWithValue("@MDTestMethod", m.CheckMethod);
                    cmd.Parameters.AddWithValue("@MDTestFeq", m.CheckFeq);
                    cmd.Parameters.AddWithValue("@MDIncomp", m.Incomp);
                    cmd.Parameters.AddWithValue("@MDManageRec", m.ManageRec);
                    cmd.Parameters.AddWithValue("@MDMemo", m.Memo);
                    cmd.Parameters.AddWithValue("@OrderNo", orderNo++);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                    int result = db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return 0;
            } catch(Exception e)
            {
                db.TransactionRollback();
                log.Info("EngMaterialDeviceControlStService.ImportStdList: " + e.Message);
                return -1;
            }
        }
        //s20230414
        public List<T> GetStdList<T>(int EngMaterialDeviceListSeq)
        {
            string sql = @"
                SELECT
                    b.MDTestItem ManageItem,
                    b.MDTestStand1 Stand,
                    b.MDTestTime CheckTiming,
                    b.MDTestMethod CheckMethod,
                    b.MDTestFeq CheckFeq,
                    b.MDIncomp Incomp,
                    b.MDManageRec ManageRec,
                    b.MDMemo Memo
                FROM EngMaterialDeviceList a
                inner join EngMaterialDeviceControlSt b on(b.EngMaterialDeviceListSeq=a.Seq)
                where a.Seq=@Seq
                order by b.DataKeep desc, b.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", EngMaterialDeviceListSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetList<T>(int EngMaterialDeviceListSeq)
        {
            string sql = @"SELECT * FROM EngMaterialDeviceControlSt
                where EngMaterialDeviceListSeq=@EngMaterialDeviceListSeq
                order by OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", EngMaterialDeviceListSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        public int GetEngMaterialDeviceControlStCount(int EngMaterialDeviceListSeq)
        {
            string sql = @"
                SELECT count(a.Seq) total FROM EngMaterialDeviceList a
                inner join EngMaterialDeviceControlSt b on(b.EngMaterialDeviceListSeq=a.Seq)
                where a.Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", EngMaterialDeviceListSeq);
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
        public List<T> GetList<T>(int EngMaterialDeviceListSeq, int pageIndex, int perPage)
        {
            string sql = @"
                SELECT
                    b.Seq,
                    b.DataType,
                    b.DataKeep,
                    b.EngMaterialDeviceListSeq,
                    b.MDTestItem,
                    b.MDTestStand1,
                    b.MDTestStand2,
                    b.MDTestTime,
                    b.MDTestMethod,
                    b.MDTestFeq,
                    b.MDIncomp,
                    b.MDManageRec,
                    b.MDMemo,
                    b.OrderNo 
                FROM EngMaterialDeviceList a
                inner join EngMaterialDeviceControlSt b on(b.EngMaterialDeviceListSeq=a.Seq)
                where a.Seq=@Seq
                order by b.DataKeep desc, b.OrderNo
                OFFSET @pageIndex ROWS
				FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", EngMaterialDeviceListSeq);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //s20230624
        public List<T> GetListAll<T>(int EngMaterialDeviceListSeq)
        {
            string sql = @"
                SELECT
                    b.Seq,
                    b.DataType,
                    b.DataKeep,
                    b.EngMaterialDeviceListSeq,
                    b.MDTestItem,
                    b.MDTestStand1,
                    b.MDTestStand2,
                    b.MDTestTime,
                    b.MDTestMethod,
                    b.MDTestFeq,
                    b.MDIncomp,
                    b.MDManageRec,
                    b.MDMemo,
                    b.OrderNo 
                FROM EngMaterialDeviceControlSt b
                where b.EngMaterialDeviceListSeq=@EngMaterialDeviceListSeq
                order by b.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", EngMaterialDeviceListSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        public List<T> GetItem<T>(int seq)
        {
            string sql = @"
                SELECT
                    Seq,
                    DataType,
                    DataKeep,
                    EngMaterialDeviceListSeq,
                    MDTestItem,
                    MDTestStand1,
                    MDTestStand2,
                    MDTestTime,
                    MDTestMethod,
                    MDTestFeq,
                    MDIncomp,
                    MDManageRec,
                    MDMemo,
                    OrderNo 
                FROM EngMaterialDeviceControlSt
                where Seq=@Seq
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public int Insert(int EngMaterialDeviceListSeq, EngMaterialDeviceControlTpModel m)
        {
            //Null2Empty(m);
            string sql = @"
                insert into EngMaterialDeviceControlSt (
                    DataType,
                    EngMaterialDeviceListSeq,
                    OrderNo,
                    MDTestItem,
                    MDTestStand1,
                    MDTestStand2,
                    MDTestTime,
                    MDTestMethod,
                    MDTestFeq,
                    MDIncomp,
                    MDManageRec,
                    MDMemo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    1,
                    @EngMaterialDeviceListSeq,
                    (select COALESCE(max(a.OrderNo),0)+1 from EngMaterialDeviceControlSt a where a.EngMaterialDeviceListSeq=@EngMaterialDeviceListSeq),
                    @MDTestItem,
                    @MDTestStand1,
                    @MDTestStand2,
                    @MDTestTime,
                    @MDTestMethod,
                    @MDTestFeq,
                    @MDIncomp,
                    @MDManageRec,
                    @MDMemo,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", EngMaterialDeviceListSeq);
            cmd.Parameters.AddWithValue("@MDTestItem", m.MDTestItem ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@MDTestStand1", m.MDTestStand1 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@MDTestStand2", m.MDTestStand2 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("MDTestTime", m.MDTestTime ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@MDTestMethod", m.MDTestMethod ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@MDTestFeq", m.MDTestFeq ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@MDIncomp", m.MDIncomp ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@MDManageRec", m.MDManageRec ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@MDMemo", m.MDMemo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('EngMaterialDeviceControlSt') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
        }
        public int NewAdd(int EngMaterialDeviceListSeq)
        {
            //Null2Empty(m);
            string sql = @"
                insert into EngMaterialDeviceControlSt (
                    DataType,
                    EngMaterialDeviceListSeq,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    1,
                    @EngMaterialDeviceListSeq,
                    (select COALESCE(max(a.OrderNo),0)+1 from EngMaterialDeviceControlSt a where a.EngMaterialDeviceListSeq=@EngMaterialDeviceListSeq),
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", EngMaterialDeviceListSeq);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('EngMaterialDeviceControlSt') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
        }

        public bool Updates(List<EngMaterialDeviceControlStModel> items)
        {
            string sql = @"";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                foreach (EngMaterialDeviceControlStModel m in items)
                {
                    Null2Empty(m);
                    sql = @"
                        update EngMaterialDeviceControlSt set
                            DataType=@DataType,
                            DataKeep=@DataKeep,
                            MDTestItem = @MDTestItem,
                            MDTestStand1 = @MDTestStand1,
                            MDTestStand2 = @MDTestStand2,
                            MDTestTime = @MDTestTime,
                            MDTestMethod = @MDTestMethod,
                            MDTestFeq = @MDTestFeq,
                            MDIncomp = @MDIncomp,
                            MDManageRec = @MDManageRec,
                            MDMemo = @MDMemo,
                            OrderNo = @OrderNo,
                            ModifyTime = GETDATE(),
                            ModifyUserSeq = @ModifyUserSeq
                            Where Seq = @Seq";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@DataType", m.DataType);
                    cmd.Parameters.AddWithValue("@DataKeep", m.DataKeep);
                    cmd.Parameters.AddWithValue("@MDTestItem", m.MDTestItem);
                    cmd.Parameters.AddWithValue("@MDTestStand1", m.MDTestStand1);
                    cmd.Parameters.AddWithValue("@MDTestStand2", m.MDTestStand2);
                    cmd.Parameters.AddWithValue("@MDTestTime", m.MDTestTime);
                    cmd.Parameters.AddWithValue("@MDTestMethod", m.MDTestMethod);
                    cmd.Parameters.AddWithValue("@MDTestFeq", m.MDTestFeq);
                    cmd.Parameters.AddWithValue("@MDIncomp", m.MDIncomp);
                    cmd.Parameters.AddWithValue("@MDManageRec", m.MDManageRec);
                    cmd.Parameters.AddWithValue("@MDMemo", m.MDMemo);
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
                log.Info("EngMaterialDeviceControlStService.Updates: " + e.Message);
                log.Info(sql);
                return false;
            }
        }

        public int Delete(int seq)
        {
            string sql = @"delete cc from EngMaterialDeviceControlSt cc 
                left join  EngMaterialDeviceTestSummary es on es.EngMaterialDeviceListSeq = cc.EngMaterialDeviceListSeq
                where cc.Seq=@Seq and es.Seq is null";

            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }
        /*public int Add(EngMaterialDeviceControlStModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into EngMaterialDeviceControlSt (
                    DataType,
                    DataKeep,
                    EngMaterialDeviceListSeq,
                    MDTestItem1,
                    MDTestItem2,
                    MDTestStand1,
                    MDTestStand2,
                    MDTestStand3,
                    MDTestStand4,
                    MDTestStand5,
                    MDTestTime,
                    MDTestMethod,
                    MDTestFeq,
                    MDIncomp,
                    MDManageRec,
                    MDType,
                    MDMemo,
                    MDTestFields,
                    MDTestFields,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @DataType,
                    @DataKeep,
                    @EngMaterialDeviceListSeq,
                    @MDTestItem1,
                    @MDTestItem2,
                    @MDTestStand1,
                    @MDTestStand2,
                    @MDTestStand3,
                    @MDTestStand4,
                    @MDTestStand5,
                    @MDTestTime,
                    @MDTestMethod,
                    @MDTestFeq,
                    @MDIncomp,
                    @MDManageRec,
                    @MDType,
                    @MDMemo,
                    @MDTestFields,
                    @MDTestFields,
                    @OrderNo,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@DataType", m.DataType);
            cmd.Parameters.AddWithValue("@DataKeep", m.MDTestItem2);
            cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", m.EngMaterialDeviceListSeq);
            cmd.Parameters.AddWithValue("@MDTestItem1", m.MDTestItem1);
            cmd.Parameters.AddWithValue("@MDTestItem2", m.MDTestItem2);
            cmd.Parameters.AddWithValue("@MDTestStand1", m.MDTestStand1);
            cmd.Parameters.AddWithValue("@MDTestStand2", m.MDTestStand2);
            cmd.Parameters.AddWithValue("@MDTestStand3", m.MDTestStand3);
            cmd.Parameters.AddWithValue("@MDTestStand4", m.MDTestStand4);
            cmd.Parameters.AddWithValue("@MDTestStand5", m.MDTestStand5);
            cmd.Parameters.AddWithValue("@MDTestTime", m.MDTestTime);
            cmd.Parameters.AddWithValue("@MDTestMethod", m.MDTestMethod);
            cmd.Parameters.AddWithValue("@MDTestFeq", m.MDTestFeq);
            cmd.Parameters.AddWithValue("@MDIncomp", m.MDIncomp);
            cmd.Parameters.AddWithValue("@MDManageRec", m.MDManageRec);
            cmd.Parameters.AddWithValue("@MDType", m.MDType);
            cmd.Parameters.AddWithValue("@MDMemo", m.MDMemo);
            cmd.Parameters.AddWithValue("@MDTestFields", this.NulltoDBNull(m.MDTestFields));
            cmd.Parameters.AddWithValue("@MDTestFields", this.NulltoDBNull(m.MDTestFields));
            cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('EngMaterialDeviceControlSt') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["NewSeq"].TMDtring());
        }*/
    }
}