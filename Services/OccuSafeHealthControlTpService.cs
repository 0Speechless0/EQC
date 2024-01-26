using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class OccuSafeHealthControlTpService : BaseService
    {//職業安全衛生抽查標準範本
        public List<T> GetList<T>(int occuSafeHealthListSeq)
        {
            string sql = @"SELECT * FROM OccuSafeHealthControlTp where OccuSafeHealthListSeq=@OccuSafeHealthListSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@OccuSafeHealthListSeq", occuSafeHealthListSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        public int Update(OSHCTpModel m)
        {
            Null2Empty(m);
            string sql = @"
                update OccuSafeHealthControlTp set
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
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            
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
            cmd.Parameters.AddWithValue("@Seq", m.Seq);

            return db.ExecuteNonQuery(cmd);
        }
        public int Add(OSHCTpModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into OccuSafeHealthControlTp (
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

            string sql1 = @"SELECT IDENT_CURRENT('OccuSafeHealthControlTp') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
        }

        public int Delete(int seq)
        {
            string sql = @"delete OccuSafeHealthControlTp where Seq=@Seq";

            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }
    }
}