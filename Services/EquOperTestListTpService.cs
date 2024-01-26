using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EquOperTestListTpService : BaseService
    {//職業安全衛生清單範本
        public List<T> GetList<T>()
        {
            string sql = @"SELECT * FROM EquOperTestListTp";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<T>(cmd);
        }

        public int GetListCount()
        {
            string sql = @"
                SELECT count(Seq) total FROM EquOperTestListTp";
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
        public List<EOTListTpEditModel> ListAll(int pageIndex, int perPage)
        {
            string sql = @"SELECT
                Seq,
                EPKind,
                OrderNo,
                ExcelNo,
                ItemName,
                FlowCharOriginFileName,
                CreateTime,
                ModifyTime,
                (
                    select count(a.EquOperTestTpSeq)
                    from EquOperControlTp a
                    where a.EquOperTestTpSeq=EquOperTestListTp.Seq
                ) detailCount
                FROM EquOperTestListTp
                order by OrderNo
                OFFSET @pageIndex ROWS
				FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<EOTListTpEditModel>(cmd);
        }

        public List<EOTListTpEditModel> GetItemBySeq(int seq)
        {
            string sql = @"SELECT
                Seq,
                EPKind,
                OrderNo,
                ExcelNo,
                ItemName,
                CreateTime,
                ModifyTime
                FROM EquOperTestListTp
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<EOTListTpEditModel>(cmd);
        }

        public List<EOTListTpEditModel> GetItemFileInfoBySeq(int seq)
        {
            string sql = @"SELECT
                Seq,
                FlowCharOriginFileName,
                FlowCharUniqueFileName
                FROM EquOperTestListTp
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<EOTListTpEditModel>(cmd);
        }

        public int Add(EOTListTpEditModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into EquOperTestListTp (
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
            cmd.Parameters.AddWithValue("@EPKind", this.NulltoDBNull(m.EPKind));
            cmd.Parameters.AddWithValue("@ExcelNo", m.ExcelNo);
            cmd.Parameters.AddWithValue("@ItemName", m.ItemName);
            cmd.Parameters.AddWithValue("@FlowCharOriginFileName", m.FlowCharOriginFileName);
            cmd.Parameters.AddWithValue("@FlowCharUniqueFileName", m.FlowCharUniqueFileName);
            cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('EquOperTestListTp') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
            return m.Seq;
        }

        public int Update(EOTListTpEditModel m)
        {
            Null2Empty(m);
            string sql = @"
                update EquOperTestListTp set
                    EPKind = @EPKind,
                    ExcelNo = @ExcelNo,
                    ItemName = @ItemName,
                    FlowCharOriginFileName = @FlowCharOriginFileName,
                    OrderNo = @OrderNo,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EPKind", this.NulltoDBNull(m.EPKind));
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
            string sql = @"delete EquOperTestListTp where Seq=@Seq";

            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }

        public int UpdateUploadFile(int seq, string originFileName, string uniqueFileName)
        {
            string sql = @"
                update EquOperTestListTp set
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