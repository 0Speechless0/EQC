using EQC.Common;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Models
{
    public class MasterEnvirConsListTpModel : EnvirConsListTpModel
    {//環境保育清單範本
        public List<EnvirConsControlTpModel> detail { get; set; }

        public void GetDetail()
        {
            detail = new EnvirConsControlTpService().GetList<EnvirConsControlTpModel>(Seq);
        }

        //寫入至工程專案
        public void clone(DBConn db, int engMainSeq, List<FlowChartFileModel> copyFileList)
        {
            if (!String.IsNullOrEmpty(FlowCharUniqueFileName))
            {
                copyFileList.Add(new FlowChartFileModel()
                {
                    Seq = this.Seq,
                    FlowCharOriginFileName = this.FlowCharUniqueFileName,
                    FlowCharUniqueFileName = String.Format("{0}-{1}", copyFileList.Count + 1, this.FlowCharUniqueFileName)
                });
            }
            string sql = @"
                insert into EnvirConsList (
                    EngMainSeq,
                    ExcelNo,
                    ItemName,
                    FlowCharOriginFileName,
                    FlowCharUniqueFileName,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @EngMainSeq,
                    @ExcelNo,
                    @ItemName,
                    @FlowCharOriginFileName,
                    @FlowCharUniqueFileName,
                    @OrderNo,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ExcelNo", ExcelNo);
            cmd.Parameters.AddWithValue("@ItemName", ItemName);
            cmd.Parameters.AddWithValue("@FlowCharOriginFileName", FlowCharOriginFileName);
            cmd.Parameters.AddWithValue("@FlowCharUniqueFileName", FlowCharUniqueFileName);
            cmd.Parameters.AddWithValue("@OrderNo", Utils.NulltoDBNull(OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", Utils.getUserSeq());

            int result = db.ExecuteNonQuery(cmd);

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('EnvirConsList') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            int Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

            foreach (EnvirConsControlTpModel m in detail)
            {
                Utils.Null2Empty(m);
                sql = @"
                insert into EnvirConsControlSt (
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
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EnvirConsListSeq", Seq);
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
                cmd.Parameters.AddWithValue("@ECCType", Utils.NulltoDBNull(m.ECCType)); //s20230830
                cmd.Parameters.AddWithValue("@ECCMemo", m.ECCMemo);
                cmd.Parameters.AddWithValue("@ECCCheckFields", Utils.NulltoDBNull(m.ECCCheckFields));
                cmd.Parameters.AddWithValue("@ECCManageFields", Utils.NulltoDBNull(m.ECCManageFields));
                cmd.Parameters.AddWithValue("@OrderNo", Utils.NulltoDBNull(m.OrderNo));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", Utils.getUserSeq());

                db.ExecuteNonQuery(cmd);
            }
        }
    }
}
