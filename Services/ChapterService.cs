using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class ChapterService : BaseService
    {
        //
        public List<T> GetChapterList<T>()
        {
            string sql = @"SELECT Seq, ChapterName FROM Chapter order by ChapterNo";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //
        public int GetListCount(int chapterSeq)
        {
            string sql = @"
                SELECT count(Seq) total FROM ChartMaintainTp
                where (@ChapterSeq=-1 or ChapterSeq=@ChapterSeq)";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@ChapterSeq", chapterSeq);

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
        //
        public List<CMEditModel> GetChartMaintainTpByChapterSeq(int chapterSeq, int pageIndex, int perPage)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    a.ChapterSeq,
                    a.OrderNo,
                    a.ExcelNo,
                    a.ChartKind,
                    a.ChartName,
                    a.OriginFileName,
                    a.CreateTime,
                    a.ModifyTime,
                    b.ChapterName
                FROM ChartMaintainTp a
                inner join Chapter b on(b.Seq=a.ChapterSeq)
                where (@ChapterSeq=-1 or a.ChapterSeq=@ChapterSeq)
                order by b.ChapterName, a.OrderNo
                OFFSET @pageIndex ROWS
				FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ChapterSeq", chapterSeq);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            return db.GetDataTableWithClass<CMEditModel>(cmd);
        }

        public List<CMEditModel> GetItemBySeq(int seq)
        {
            string sql = @"SELECT
                a.Seq,
                a.OrderNo,
                a.ExcelNo,
                a.ChartName,
                a.CreateTime,
                a.ModifyTime,
                b.ChapterName
                FROM ChartMaintainTp a
                inner join Chapter b on(b.Seq=a.ChapterSeq)
                where a.Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<CMEditModel>(cmd);
        }

        public List<CMEditModel> GetItemFileInfoBySeq(int seq)
        {
            string sql = @"SELECT
                Seq,
                OriginFileName,
                UniqueFileName
                FROM ChartMaintainTp
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<CMEditModel>(cmd);
        }

        public int Add(CMEditModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into ChartMaintainTp (
                    ChapterSeq,
                    ExcelNo,
                    ChartKind,
                    ChartName,
                    OriginFileName,
                    UniqueFileName,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @ChapterSeq,
                    @ExcelNo,
                    @ChartKind,
                    @ChartName,
                    @OriginFileName,
                    @UniqueFileName,
                    @OrderNo,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ChapterSeq", this.NulltoDBNull(m.ChapterSeq));
            cmd.Parameters.AddWithValue("@ExcelNo", this.NulltoDBNull(m.ExcelNo));
            cmd.Parameters.AddWithValue("@ChartKind", this.NulltoDBNull(m.ChartKind));
            cmd.Parameters.AddWithValue("@ChartName", m.ChartName);
            cmd.Parameters.AddWithValue("@OriginFileName", m.OriginFileName);
            cmd.Parameters.AddWithValue("@UniqueFileName", m.UniqueFileName);
            cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('ChartMaintainTp') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
            return m.Seq;
        }

        public int Update(CMEditModel m)
        {
            Null2Empty(m);
            string sql = @"
                update ChartMaintainTp set
                    ExcelNo = @ExcelNo,
                    ChartKind = @ChartKind,
                    ChartName = @ChartName,
                    OriginFileName = @OriginFileName,
                    UniqueFileName = @UniqueFileName,
                    OrderNo = @OrderNo,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ExcelNo", this.NulltoDBNull(m.ExcelNo));
            cmd.Parameters.AddWithValue("@ChartKind", this.NulltoDBNull(m.ChartKind));
            cmd.Parameters.AddWithValue("@ChartName", m.ChartName);
            cmd.Parameters.AddWithValue("@OriginFileName", m.OriginFileName);
            cmd.Parameters.AddWithValue("@UniqueFileName", m.UniqueFileName);
            cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@Seq", m.Seq);

            return db.ExecuteNonQuery(cmd);
        }

        public int Delete(int seq)
        {
            string sql = @"delete ChartMaintainTp where Seq=@Seq";

            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }

        public int UpdateUploadFile(int seq, string originFileName, string uniqueFileName)
        {
            string sql = @"
                update ChartMaintainTp set
                    OriginFileName = @originFileName,
                    UniqueFileName = @uniqueFileName,
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