using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace EQC.Services
{
    public class EngMaterialDeviceListTpService : BaseService
    {//材料設備清冊範本
        public List<T> GetList<T>()
        {
            string sql = @"SELECT * FROM EngMaterialDeviceListTp";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<T>(cmd);
        }

        public int GetListCount()
        {
            string sql = @"
                SELECT count(Seq) total FROM EngMaterialDeviceListTp";
            SqlCommand cmd = db.GetCommand(sql);
            DataTable dt = db.GetDataTable(cmd);
            if (dt.Rows.Count == 1)
            {
                return Convert.ToInt32(dt.Rows[0]["total"].ToString());
            }
            else
            {
                return 0;
            }
        }
        public List<EMDListTpEditModel> ListAll(int pageIndex, int perPage)
        {
            string sql = @"SELECT
                Seq,
                OrderNo,
                ExcelNo,
                MDName,
                FlowCharOriginFileName,
                CreateTime,
                ModifyTime,
                (
                    select count(a.EngMaterialDeviceListTpSeq)
                    from EngMaterialDeviceControlTp a
                    where a.EngMaterialDeviceListTpSeq=EngMaterialDeviceListTp.Seq
                ) detailCount
                FROM EngMaterialDeviceListTp
                order by OrderNo
                OFFSET @pageIndex ROWS
				FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);

            return db.GetDataTableWithClass<EMDListTpEditModel>(cmd);
        }





        public List<EMDListTpEditModel> GetItemBySeq(int seq)
        {
            string sql = @"SELECT
                Seq,
                OrderNo,
                ExcelNo,
                MDName,
                CreateTime,
                ModifyTime
                FROM EngMaterialDeviceListTp
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<EMDListTpEditModel>(cmd);
        }

        public List<EMDListTpEditModel> GetItemFileInfoBySeq(int seq)
        {
            string sql = @"SELECT
                Seq,
                FlowCharOriginFileName,
                FlowCharUniqueFileName
                FROM EngMaterialDeviceListTp
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<EMDListTpEditModel>(cmd);
        }

        public int Add(EMDListTpEditModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into EngMaterialDeviceListTp (
                    ParentSeq,
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
                    @ParentSeq,
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
            cmd.Parameters.AddWithValue("@ParentSeq", this.NulltoDBNull(m.ParentSeq));
            cmd.Parameters.AddWithValue("@ExcelNo", m.ExcelNo);
            cmd.Parameters.AddWithValue("@MDName", m.MDName);
            cmd.Parameters.AddWithValue("@FlowCharOriginFileName", m.FlowCharOriginFileName);
            cmd.Parameters.AddWithValue("@FlowCharUniqueFileName", m.FlowCharUniqueFileName);
            cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('EngMaterialDeviceListTp') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
            return m.Seq;
        }

        public int Update(EMDListTpEditModel m)
        {
            Null2Empty(m);
            string sql = @"
                update EngMaterialDeviceListTp set
                    ParentSeq = @ParentSeq,
                    ExcelNo = @ExcelNo,
                    MDName = @MDName,
                    FlowCharOriginFileName = @FlowCharOriginFileName,
                    OrderNo = @OrderNo,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ParentSeq", this.NulltoDBNull(m.ParentSeq));
            cmd.Parameters.AddWithValue("@ExcelNo", m.ExcelNo);
            cmd.Parameters.AddWithValue("@MDName", m.MDName);
            cmd.Parameters.AddWithValue("@FlowCharOriginFileName", m.FlowCharOriginFileName);
            cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@Seq", m.Seq);

            return db.ExecuteNonQuery(cmd);
        }

        public int Delete(int seq)
        {
            string sql = @"delete EngMaterialDeviceListTp where Seq=@Seq";

            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }

        public int UpdateUploadFile(int seq, string originFileName, string uniqueFileName)
        {
            string sql = @"
                update EngMaterialDeviceListTp set
                    FlowCharOriginFileName = @originFileName,
                    FlowCharUniqueFileName = @uniqueFileName,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@originFileName", originFileName);
            cmd.Parameters.AddWithValue("@uniqueFileName", uniqueFileName);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }
    }
}