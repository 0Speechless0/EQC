using EQC.Common;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Models
{
    public class MasterEngMaterialDeviceListTpModel : EngMaterialDeviceListTpModel
    {//材料設備清冊範本
        public List<EngMaterialDeviceControlTpModel> detail { get; set; }

        public void GetDetail()
        {
            detail = new EngMaterialDeviceControlTpService().GetList<EngMaterialDeviceControlTpModel>(Seq);
        }
        //寫入至工程專案
        public void clone(DBConn db, int engMainSeq)
        {
            string sql = @"
                insert into EngMaterialDeviceList(
                    EngMainSeq,
                    ExcelNo,
                    MDName,
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
                    @MDName,
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
            cmd.Parameters.AddWithValue("@MDName", MDName);
            cmd.Parameters.AddWithValue("@FlowCharOriginFileName", FlowCharOriginFileName);
            cmd.Parameters.AddWithValue("@FlowCharUniqueFileName", FlowCharUniqueFileName);
            cmd.Parameters.AddWithValue("@OrderNo", Utils.NulltoDBNull(OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", Utils.getUserSeq());

            int result = db.ExecuteNonQuery(cmd);

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('EngMaterialDeviceList') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            this.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

            foreach (EngMaterialDeviceControlTpModel m in detail)
            {
                Utils.Null2Empty(m);
                sql = @"
                insert into EngMaterialDeviceControlSt(
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
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @EngMaterialDeviceListSeq,
                    @MDTestItem,
                    @MDTestStand1,
                    @MDTestStand2,
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
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", this.Seq);
                cmd.Parameters.AddWithValue("@MDTestItem", m.MDTestItem);
                cmd.Parameters.AddWithValue("@MDTestStand1", m.MDTestStand1);
                cmd.Parameters.AddWithValue("@MDTestStand2", m.MDTestStand2);
                cmd.Parameters.AddWithValue("@MDTestTime", m.MDTestTime);
                cmd.Parameters.AddWithValue("@MDTestMethod", m.MDTestMethod);
                cmd.Parameters.AddWithValue("@MDTestFeq", m.MDTestFeq);
                cmd.Parameters.AddWithValue("@MDIncomp", m.MDIncomp);
                cmd.Parameters.AddWithValue("@MDManageRec", m.MDManageRec);
                cmd.Parameters.AddWithValue("@MDMemo", m.MDMemo);
                cmd.Parameters.AddWithValue("@OrderNo", Utils.NulltoDBNull(m.OrderNo));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", Utils.getUserSeq());

                db.ExecuteNonQuery(cmd);
            }
        }

        //寫入至工程專案
        public void cloneDetail(DBConn db, int engMaterialDeviceListSeq)
        {
            string sql = "";
            SqlCommand cmd;

            foreach (EngMaterialDeviceControlTpModel m in detail)
            {
                Utils.Null2Empty(m);
                sql = @"
                insert into EngMaterialDeviceControlSt(
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
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @EngMaterialDeviceListSeq,
                    @MDTestItem,
                    @MDTestStand1,
                    @MDTestStand2,
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
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", engMaterialDeviceListSeq);
                cmd.Parameters.AddWithValue("@MDTestItem", m.MDTestItem);
                cmd.Parameters.AddWithValue("@MDTestStand1", m.MDTestStand1);
                cmd.Parameters.AddWithValue("@MDTestStand2", m.MDTestStand2);
                cmd.Parameters.AddWithValue("@MDTestTime", m.MDTestTime);
                cmd.Parameters.AddWithValue("@MDTestMethod", m.MDTestMethod);
                cmd.Parameters.AddWithValue("@MDTestFeq", m.MDTestFeq);
                cmd.Parameters.AddWithValue("@MDIncomp", m.MDIncomp);
                cmd.Parameters.AddWithValue("@MDManageRec", m.MDManageRec);
                cmd.Parameters.AddWithValue("@MDMemo", m.MDMemo);
                cmd.Parameters.AddWithValue("@OrderNo", Utils.NulltoDBNull(m.OrderNo));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", Utils.getUserSeq());

                db.ExecuteNonQuery(cmd);
            }
        }
    }
}
