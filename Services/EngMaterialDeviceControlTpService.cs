using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EngMaterialDeviceControlTpService: BaseService
    {//材料設備抽查管理標準範本
        public List<T> GetList<T>(int engMaterialDeviceListTpSeq)
        {
            string sql = @"SELECT * FROM EngMaterialDeviceControlTp where EngMaterialDeviceListTpSeq=@EngMaterialDeviceListTpSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMaterialDeviceListTpSeq",engMaterialDeviceListTpSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        public int Update(EMDCTpModel m)
        {
            Null2Empty(m);
            string sql = @"
                update EngMaterialDeviceControlTp set
                    MDTestItem = @MDTestItem,
                    MDTestStand1 = @MDTestStand1,
                    MDTestStand2 = @MDTestStand2,
                    MDTestTime = @MDTestTime,
                    MDTestMethod = @MDTestMethod,
                    MDTestFeq = @MDTestFeq,
                    MDIncomp = @MDIncomp,
                    MDManageRec = @MDManageRec,
                    MDMemo = @MDMemo,
                    OrderNo = @OrderNo,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@MDTestItem", m.MDTestItem);
            cmd.Parameters.AddWithValue("@MDTestStand1", m.MDTestStand1);
            cmd.Parameters.AddWithValue("@MDTestStand2", m.MDTestStand2);
            cmd.Parameters.AddWithValue("@MDTestTime", m.MDTestTime);
            cmd.Parameters.AddWithValue("@MDTestMethod", m.MDTestMethod);
            cmd.Parameters.AddWithValue("@MDTestFeq", m.MDTestFeq);
            cmd.Parameters.AddWithValue("@MDIncomp", m.MDIncomp);
            cmd.Parameters.AddWithValue("@MDManageRec", m.MDManageRec);
            cmd.Parameters.AddWithValue("@MDMemo", m.MDMemo);
            cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@Seq", m.Seq);

            return db.ExecuteNonQuery(cmd);
        }
        public int Add(EMDCTpModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into EngMaterialDeviceControlTp(
                    EngMaterialDeviceListTpSeq,
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
                    @EngMaterialDeviceListTpSeq,
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
            SqlCommand cmd = db.GetCommand(sql);
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMaterialDeviceListTpSeq", m.EngMaterialDeviceListTpSeq);
            cmd.Parameters.AddWithValue("@MDTestItem", m.MDTestItem);
            cmd.Parameters.AddWithValue("@MDTestStand1", m.MDTestStand1);
            cmd.Parameters.AddWithValue("@MDTestStand2", m.MDTestStand2);
            cmd.Parameters.AddWithValue("@MDTestTime", m.MDTestTime);
            cmd.Parameters.AddWithValue("@MDTestMethod", m.MDTestMethod);
            cmd.Parameters.AddWithValue("@MDTestFeq", m.MDTestFeq);
            cmd.Parameters.AddWithValue("@MDIncomp", m.MDIncomp);
            cmd.Parameters.AddWithValue("@MDManageRec", m.MDManageRec);
            cmd.Parameters.AddWithValue("@MDMemo", m.MDMemo);
            cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('EngMaterialDeviceControlTp') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
        }

        public int Delete(int seq)
        {
            string sql = @"delete EngMaterialDeviceControlTp where Seq=@Seq";

            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }
    }
}