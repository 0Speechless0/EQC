using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EnvirConsControlTpService : BaseService
    {//環境保育抽查標準範本
        public List<T> GetList<T>(int envirConsListSeq)
        {
            string sql = @"SELECT * FROM EnvirConsControlTp where EnvirConsListSeq=@EnvirConsListSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EnvirConsListSeq", envirConsListSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        public int Update(ECCTpModel m)
        {
            Null2Empty(m);
            string sql = @"
                update EnvirConsControlTp set
                    ECCFlow1 = @ECCFlow1,
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
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ECCFlow1", m.ECCFlow1);
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
            cmd.Parameters.AddWithValue("@Seq", m.Seq);

            return db.ExecuteNonQuery(cmd);
        }
        public int Add(ECCTpModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into EnvirConsControlTp (
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
                    @OrderNo,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EnvirConsListSeq", m.EnvirConsListSeq);
            cmd.Parameters.AddWithValue("@ECCFlow1", m.ECCFlow1);
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

            string sql1 = @"SELECT IDENT_CURRENT('EnvirConsControlTp') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
        }

        public int Delete(int seq)
        {
            string sql = @"delete EnvirConsControlTp where Seq=@Seq";

            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }
    }
}