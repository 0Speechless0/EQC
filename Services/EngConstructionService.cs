using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EngConstructionService : BaseService
    {//工程主要施工項目及數量
        public List<T> GetListByEngMainSeq<T>(int engMainSeq)
        {
            string sql = @"
                SELECT
                    Seq,
                    EngMainSeq,
                    ItemName,
                    ItemQty,
                    ItemUnit
                FROM EngConstruction
                where EngMainSeq=@EngMainSeq
                and OrderNo>0
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //s20230624
        public List<T> GetListAllByEngMainSeq<T>(int engMainSeq)
        {
            string sql = @"
                SELECT
                    Seq,
                    EngMainSeq,
                    ItemName,
                    ItemQty,
                    ItemUnit,
                    OrderNo,
                    ItemNo
                FROM EngConstruction
                where EngMainSeq=@EngMainSeq
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //是否有預設 item
        public int GetDefaultItem(int engMainSeq)
        {
            string sql = @"
                SELECT count(Seq) total FROM EngConstruction
                where EngMainSeq=@EngMainSeq
                and OrderNo<0";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
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

        public int Add(EngConstructionModel m, int count)
        {
            Null2Empty(m);
            string sql = "";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                if(count==0)
                {//初始預設項目 職業安全、1、式, 生態保育、1、式
                    sql = @"
                    insert into EngConstruction (
                        EngMainSeq,
                        ItemNo,
                        ItemName,
                        ItemQty,
                        ItemUnit,
                        OrderNo,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EngMainSeq,
                        @ItemNo,
                        @ItemName,
                        @ItemQty,
                        @ItemUnit,
                        @OrderNo,
                        GETDATE(),
                        @ModifyUserSeq,
                        GETDATE(),
                        @ModifyUserSeq
                    )";
                    cmd = db.GetCommand(sql);

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EngMainSeq", m.EngMainSeq);
                    cmd.Parameters.AddWithValue("@ItemNo", "E01");
                    cmd.Parameters.AddWithValue("@ItemName", "職業安全");
                    cmd.Parameters.AddWithValue("@ItemQty", 1);
                    cmd.Parameters.AddWithValue("@ItemUnit", "式");
                    cmd.Parameters.AddWithValue("@OrderNo", -1);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EngMainSeq", m.EngMainSeq);
                    cmd.Parameters.AddWithValue("@ItemNo", "E02");
                    cmd.Parameters.AddWithValue("@ItemName", "生態保育");
                    cmd.Parameters.AddWithValue("@ItemQty", 1);
                    cmd.Parameters.AddWithValue("@ItemUnit", "式");
                    cmd.Parameters.AddWithValue("@OrderNo", -2);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                sql = @"
                insert into EngConstruction (
                    EngMainSeq,
                    ItemNo,
                    ItemName,
                    ItemQty,
                    ItemUnit,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @EngMainSeq,
                    Right('000' + Cast((select ISNULL(max(OrderNo),0)+1 from EngConstruction where EngMainSeq=@EngMainSeq and OrderNo>0) as varchar), 3),
                    @ItemName,
                    @ItemQty,
                    @ItemUnit,
                    (select ISNULL(max(OrderNo),0)+1 from EngConstruction where EngMainSeq=@EngMainSeq and OrderNo>0),
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
                cmd = db.GetCommand(sql);

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", m.EngMainSeq);
                //cmd.Parameters.AddWithValue("@ItemNo", m.ItemNo);
                cmd.Parameters.AddWithValue("@ItemName", m.ItemName);
                cmd.Parameters.AddWithValue("@ItemQty", this.NulltoDBNull(m.ItemQty));
                cmd.Parameters.AddWithValue("@ItemUnit", m.ItemUnit);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('EngConstruction') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);

                db.TransactionCommit();
                return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString()); ;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("EngConstructionService.Add: " + e.Message);
                log.Info(sql);
                return 0;
            }
        }

        public int Update(EngConstructionModel m)
        {
            string sql = @"
                update EngConstruction set
                    ItemName = @ItemName,
                    ItemQty = @ItemQty,
                    ItemUnit = @ItemUnit,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ItemName", m.ItemName);
            cmd.Parameters.AddWithValue("@ItemQty", this.NulltoDBNull(m.ItemQty));
            cmd.Parameters.AddWithValue("@ItemUnit", m.ItemUnit);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@Seq", m.Seq);
            try
            {
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("EngConstructionService.Update: " + e.Message);
                return -1;
            }
        }

        public int Delete(int seq)
        {
            string sql = @"delete EngConstruction where Seq=@Seq";

            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }
    }
}