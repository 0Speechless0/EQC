using EQC.Common;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Models
{
    public class MasterEquOperTestListTpModel : EquOperTestListTpModel
    {//設備運轉測試清單範本
        public List<EquOperControlTpModel> detail { get; set; }

        public void GetDetail()
        {
            detail = new EquOperControlTpService().GetList<EquOperControlTpModel>(Seq);
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
                insert into EquOperTestList (
                    EngMainSeq,
                    EPKind,
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
                    @EPKind,
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
            cmd.Parameters.AddWithValue("@EPKind", Utils.NulltoDBNull(EPKind));
            cmd.Parameters.AddWithValue("@ExcelNo", ExcelNo);
            cmd.Parameters.AddWithValue("@ItemName", ItemName);
            cmd.Parameters.AddWithValue("@FlowCharOriginFileName", FlowCharOriginFileName);
            cmd.Parameters.AddWithValue("@FlowCharUniqueFileName", FlowCharUniqueFileName);
            cmd.Parameters.AddWithValue("@OrderNo", Utils.NulltoDBNull(OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq",Utils.getUserSeq());

            int result = db.ExecuteNonQuery(cmd);

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('EquOperTestList') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            int Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

            foreach (EquOperControlTpModel m in detail)
            {
                Utils.Null2Empty(m);
                sql = @"
                insert into EquOperControlSt (
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
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EquOperTestStSeq", Seq);
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
                cmd.Parameters.AddWithValue("@EPType", Utils.NulltoDBNull(m.EPType));//s20230830
                cmd.Parameters.AddWithValue("@EPMemo", m.EPMemo);
                cmd.Parameters.AddWithValue("@EPCheckFields", Utils.NulltoDBNull(m.EPCheckFields));
                cmd.Parameters.AddWithValue("@EPManageFields", Utils.NulltoDBNull(m.EPManageFields));
                cmd.Parameters.AddWithValue("@OrderNo", Utils.NulltoDBNull(m.OrderNo));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", Utils.getUserSeq());

                db.ExecuteNonQuery(cmd);
            }
        }
    }
}
