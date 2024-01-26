using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class ConstCheckListTpService : BaseService
    {//材料設備清冊範本
        public List<T> GetList<T>()
        {
            string sql = @"SELECT * FROM ConstCheckListTp";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<T>(cmd);
        }

        public int GetListCount()
        {
            string sql = @"
                SELECT count(Seq) total FROM ConstCheckListTp";
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
        public List<CCListTpEditModel> ListAll(int pageIndex, int perPage)
        {
            string sql = @"SELECT
                Seq,
                OrderNo,
                ExcelNo,
                ItemName,
                FlowCharOriginFileName,
                CreateTime,
                ModifyTime,
                (
                    select count(a.ConstCheckListTpSeq)
                    from ConstCheckControlTp a
                    where a.ConstCheckListTpSeq=ConstCheckListTp.Seq
                ) detailCount
                FROM ConstCheckListTp
                order by OrderNo
                OFFSET @pageIndex ROWS
				FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<CCListTpEditModel>(cmd);
        }

        public List<CCListTpEditModel> GetItemBySeq(int seq)
        {
            string sql = @"SELECT
                Seq,
                OrderNo,
                ExcelNo,
                ItemName,
                CreateTime,
                ModifyTime
                FROM ConstCheckListTp
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<CCListTpEditModel>(cmd);
        }

        public List<CCListTpEditModel> GetItemFileInfoBySeq(int seq)
        {
            string sql = @"SELECT
                Seq,
                FlowCharOriginFileName,
                FlowCharUniqueFileName
                FROM ConstCheckListTp
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<CCListTpEditModel>(cmd);
        }

        public int Add(CCListTpEditModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into ConstCheckListTp (
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
            cmd.Parameters.AddWithValue("@ExcelNo", m.ExcelNo);
            cmd.Parameters.AddWithValue("@ItemName", m.ItemName);
            cmd.Parameters.AddWithValue("@FlowCharOriginFileName", m.FlowCharOriginFileName);
            cmd.Parameters.AddWithValue("@FlowCharUniqueFileName", m.FlowCharUniqueFileName);
            cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('ConstCheckListTp') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
            return m.Seq;
        }

        public int Update(CCListTpEditModel m)
        {
            Null2Empty(m);
            string sql = @"
                update ConstCheckListTp set
                    ExcelNo = @ExcelNo,
                    ItemName = @ItemName,
                    FlowCharOriginFileName = @FlowCharOriginFileName,
                    OrderNo = @OrderNo,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ExcelNo", m.ExcelNo);
            cmd.Parameters.AddWithValue("@ItemName", m.ItemName);
            cmd.Parameters.AddWithValue("@FlowCharOriginFileName", m.FlowCharOriginFileName);
            cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@Seq", m.Seq);

            return db.ExecuteNonQuery(cmd);
        }

        public int Delete(int seq)
        {
            string sql = @"delete ConstCheckListTp where Seq=@Seq";

            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }

        public int UpdateUploadFile(int seq, string originFileName, string uniqueFileName)
        {
            string sql = @"
                update ConstCheckListTp set
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