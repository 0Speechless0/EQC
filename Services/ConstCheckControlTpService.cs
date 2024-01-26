using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class ConstCheckControlTpService : BaseService
    {//材料設備抽查管理標準範本
        public List<T> GetList<T>(int constCheckListTpSeq)
        {
            string sql = @"SELECT * FROM ConstCheckControlTp where ConstCheckListTpSeq=@ConstCheckListTpSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ConstCheckListTpSeq", constCheckListTpSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        public int Update(CCCTpModel m)
        {
            Null2Empty(m);
            string sql = @"
                update ConstCheckControlTp set
                    CCFlow1 = @CCFlow1,
                    CCFlow2 = @CCFlow2,
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
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            
            cmd.Parameters.AddWithValue("@CCFlow1", this.NulltoDBNull(m.CCFlow1));
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
            cmd.Parameters.AddWithValue("@CCType", m.CCType);
            cmd.Parameters.AddWithValue("@CCMemo", m.CCMemo);
            cmd.Parameters.AddWithValue("@CCCheckFields", this.NulltoDBNull(m.CCCheckFields));
            cmd.Parameters.AddWithValue("@CCManageFields", this.NulltoDBNull(m.CCManageFields));
            cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@Seq", m.Seq);

            return db.ExecuteNonQuery(cmd);
        }
        public int Add(CCCTpModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into ConstCheckControlTp(
                    ConstCheckListTpSeq,
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
                    @ConstCheckListTpSeq,
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
                    @OrderNo,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ConstCheckListTpSeq", m.ConstCheckListTpSeq);
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
            cmd.Parameters.AddWithValue("@CCType", m.CCType);
            cmd.Parameters.AddWithValue("@CCMemo", m.CCMemo);
            cmd.Parameters.AddWithValue("@CCCheckFields", this.NulltoDBNull(m.CCCheckFields));
            cmd.Parameters.AddWithValue("@CCManageFields", this.NulltoDBNull(m.CCManageFields));
            cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('ConstCheckControlTp') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
        }

        public int Delete(int seq)
        {
            string sql = @"delete ConstCheckControlTp where Seq=@Seq";

            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }
    }
}