using EQC.Common;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Models
{
    public class MasterOccuSafeHealthListTpModel : OccuSafeHealthListTpModel
    {//職業安全衛生清單範本
        public List<OccuSafeHealthControlTpModel> detail { get; set; }

        public void GetDetail()
        {
            detail = new OccuSafeHealthControlTpService().GetList<OccuSafeHealthControlTpModel>(Seq);
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
                insert into OccuSafeHealthList (
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

            string sql1 = @"SELECT IDENT_CURRENT('OccuSafeHealthList') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            int Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

            foreach (OccuSafeHealthControlTpModel m in detail)
            {
                Utils.Null2Empty(m);
                sql = @"
                insert into OccuSafeHealthControlSt (
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
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@OccuSafeHealthListSeq", Seq);
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
                cmd.Parameters.AddWithValue("@OSType", Utils.NulltoDBNull(m.OSType)); //s20230830
                cmd.Parameters.AddWithValue("@OSMemo", m.OSMemo);
                cmd.Parameters.AddWithValue("@OSCheckFields", Utils.NulltoDBNull(m.OSCheckFields));
                cmd.Parameters.AddWithValue("@OSManageFields", Utils.NulltoDBNull(m.OSManageFields));
                cmd.Parameters.AddWithValue("@OrderNo", Utils.NulltoDBNull(m.OrderNo));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", Utils.getUserSeq());

                db.ExecuteNonQuery(cmd);
            }
        }
    }
}
