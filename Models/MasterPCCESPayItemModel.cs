using EQC.Common;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Models
{
    public class MasterPCCESPayItemModel : PCCESPayItemModel
    {//PCCESS詳細表
        //public int deviceListSeq { get; set; }

        public static List<T> GetList<T>(DBConn db, int engMainSeq)
        {
            string sql = @"
                    select a.PayItem,a.[Description],a.Unit,a.Quantity,a.Price,a.Amount,a.RefItemCode
                    from PCCESPayItem a
                    inner join PCCESSMain b on(
	                    b.Seq=a.PCCESSMainSeq
                        and b.contractNo=(select c.EngNo from EngMain c where c.Seq=@EngMainSeq)
                    )
                    -- where a.RefItemCode like 'M%'";
            /*string sql = @"
                    select d.Seq deviceListSeq, a.PayItem,a.[Description],a.Unit,a.Quantity,a.Price,a.Amount,a.RefItemCode
                    from PCCESPayItem a
                    inner join PCCESSMain b on(
	                    b.Seq=a.PCCESSMainSeq
                        and b.contractNo=(select c.EngNo from EngMain c where c.Seq=@EngMainSeq)
                    )
                    left outer join EngMaterialDeviceListTp d on(d.ExcelNo=a.RefItemCode)
                    where a.RefItemCode like 'M%'";*/
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //寫入至工程專案
        public void SaveEngMaterialDeviceSummary(DBConn db, int engMainSeq, List<MasterEngMaterialDeviceListTpModel> mList5, MasterPCCESPayItemModel m, ref int orderNo, List<FlowChartFileModel> copyFileList)
        {
            SqlCommand cmd;
            int dataType = 1;
            string sql = @"";
            Boolean fSave = false;
            MasterEngMaterialDeviceListTpModel masterItem = null;
            foreach (MasterEngMaterialDeviceListTpModel item in mList5)
            {//上層 Seq
                if (m.RefItemCode.IndexOf(item.ExcelNo) == 0)
                {
                    dataType = 0;

                    if (String.IsNullOrEmpty(item.FlowCharUniqueFileName))
                    {
                        m.FlowCharOriginFileName = item.FlowCharOriginFileName;
                        m.FlowCharUniqueFileName = item.FlowCharUniqueFileName;
                    } else {
                        m.FlowCharOriginFileName = item.FlowCharUniqueFileName;
                        m.FlowCharUniqueFileName = String.Format("{0}-{1}", copyFileList.Count+1, item.FlowCharUniqueFileName);
                        copyFileList.Add(new FlowChartFileModel()
                        {
                            Seq = m.Seq,
                            FlowCharOriginFileName = m.FlowCharOriginFileName,
                            FlowCharUniqueFileName = m.FlowCharUniqueFileName
                        });
                    }
                    masterItem = item;
                    fSave = true;
                    break;
                }
            }
            if (!fSave) return;
            orderNo++;

            sql = @"
            insert into EngMaterialDeviceList(
                DataType,
                EngMainSeq,
                ExcelNo,
                ItemNo,
                MDName,
                OrderNo,
                FlowCharOriginFileName,
                FlowCharUniqueFileName,
                CreateTime,
                CreateUserSeq,
                ModifyTime,
                ModifyUserSeq
            ) values (
                @DataType,
                @EngMainSeq,
                @ExcelNo,
                @ItemNo,
                @MDName,
                @OrderNo,
                @FlowCharOriginFileName,
                @FlowCharUniqueFileName,
                GETDATE(),
                @ModifyUserSeq,
                GETDATE(),
                @ModifyUserSeq
            )";
            cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@DataType", dataType);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ExcelNo", m.RefItemCode);
            cmd.Parameters.AddWithValue("@ItemNo", m.PayItem);
            cmd.Parameters.AddWithValue("@MDName", m.Description);
            cmd.Parameters.AddWithValue("@OrderNo", orderNo);
            cmd.Parameters.AddWithValue("@FlowCharOriginFileName", Utils.NulltoEmpty(m.FlowCharOriginFileName));
            cmd.Parameters.AddWithValue("@FlowCharUniqueFileName", Utils.NulltoEmpty(m.FlowCharUniqueFileName));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", Utils.getUserSeq());

            int result = db.ExecuteNonQuery(cmd);

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('EngMaterialDeviceList') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            int engMaterialDeviceListSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

            if (masterItem != null) masterItem.cloneDetail(db, engMaterialDeviceListSeq);
            sql = @"
                insert into EngMaterialDeviceSummary(
                    EngMaterialDeviceListSeq,
                    OrderNo,
                    ItemNo,
                    MDName,
                    ContactQty,
                    ContactUnit,
                    IsSampleTest,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values(
                    @EngMaterialDeviceListSeq,
                    @OrderNo,
                    @ItemNo,
                    @MDName,
                    @ContactQty,
                    @ContactUnit,
                    1,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", engMaterialDeviceListSeq);
            cmd.Parameters.AddWithValue("@OrderNo", orderNo);
            cmd.Parameters.AddWithValue("@ItemNo", m.PayItem);
            cmd.Parameters.AddWithValue("@MDName", m.Description);
            cmd.Parameters.AddWithValue("@ContactQty", m.Quantity);
            cmd.Parameters.AddWithValue("@ContactUnit", m.Unit);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", Utils.getUserSeq());
            db.ExecuteNonQuery(cmd);
        }
    }
}
