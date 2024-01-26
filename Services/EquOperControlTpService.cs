using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EquOperControlTpService : BaseService
    {//設備運轉抽查標準範本
        public List<T> GetList<T>(int equOperTestTpSeq)
        {
            string sql = @"SELECT * FROM EquOperControlTp where EquOperTestTpSeq=@EquOperTestTpSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EquOperTestTpSeq", equOperTestTpSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        public int Update(EOCTpModel m)
        {
            Null2Empty(m);
            string sql = @"
                update EquOperControlTp set
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
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
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
            cmd.Parameters.AddWithValue("@Seq", m.Seq);

            return db.ExecuteNonQuery(cmd);
        }
        public int Add(EOCTpModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into EquOperControlTp(
                    EquOperTestTpSeq,
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
                    @EquOperTestTpSeq,
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
            cmd.Parameters.AddWithValue("@EquOperTestTpSeq", m.EquOperTestTpSeq);
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

            string sql1 = @"SELECT IDENT_CURRENT('EquOperControlTp') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
        }

        public int Delete(int seq)
        {
            string sql = @"delete EquOperControlTp where Seq=@Seq";

            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }
    }
}