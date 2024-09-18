using EQC.Common;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Models
{//施工抽查清單範本
    public class MasterConstCheckListTpModel : ConstCheckListTpModel
    {
        public List<ConstCheckControlTpModel> detail { get; set; }

        public void GetDetail()
        {
            detail = new ConstCheckControlTpService().GetList<ConstCheckControlTpModel>(Seq);
        }
        //寫入至工程專案
        public void clone(DBConn db, int engMainSeq, List<MasterPCCESPayItemModel> payItems, List<FlowChartFileModel> copyFileList)
        {
            string oFile = FlowCharOriginFileName;
            string uFile = FlowCharUniqueFileName;

            if (this.ExcelNo.Length == 0) return;
            bool fSave = (this.ExcelNo == "A01");
            if (!fSave)
            {
                int len = this.ExcelNo.Length;
                if (len >= 5)
                {
                    foreach (MasterPCCESPayItemModel item in payItems)
                    {
                        if (item.RefItemCode != null && item.RefItemCode.Length >= 5)
                        {
                            try
                            {
                                if (this.ExcelNo == item.RefItemCode.Substring(0, len))
                                {
                                    fSave = true;
                                    break;
                                }
                            }
                            catch(Exception e)
                            {

                            }

                        }
                    }
                }
            }
            if (!fSave) return;

            if (!String.IsNullOrEmpty(uFile))
            {
                oFile = uFile;
                uFile = String.Format("{0}-{1}", copyFileList.Count + 1, uFile);
                copyFileList.Add(new FlowChartFileModel()
                {
                    Seq = this.Seq,
                    FlowCharOriginFileName = oFile,
                    FlowCharUniqueFileName = uFile
                });
            }

            string sql = @"
                insert into ConstCheckList (
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

            string sql1 = @"SELECT IDENT_CURRENT('ConstCheckList') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            int Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

            foreach (ConstCheckControlTpModel m in detail)
            {
                Utils.Null2Empty(m);
                sql = @"
                insert into ConstCheckControlSt (
                    ConstCheckListSeq,
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
                    @ConstCheckListSeq,
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
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ConstCheckListSeq", Seq);
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
                cmd.Parameters.AddWithValue("@CCCheckFields", Utils.NulltoDBNull(m.CCCheckFields));
                cmd.Parameters.AddWithValue("@CCManageFields", Utils.NulltoDBNull(m.CCManageFields));
                cmd.Parameters.AddWithValue("@OrderNo", Utils.NulltoDBNull(m.OrderNo));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", Utils.getUserSeq());

                db.ExecuteNonQuery(cmd);
            }
        }
    }
}
