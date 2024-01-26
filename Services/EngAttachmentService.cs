using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EngAttachmentService : BaseService
    {//上傳監造計畫附件
        public List<T> GetItemBySeq<T>(int seq)
        {
            string sql = @"
                SELECT
                    Seq,
                    EngMainSeq,
                    OriginFileName,
                    UniqueFileName,
                    Chapter,
                    FileType,
                    Description,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                FROM EngAttachment
                where Seq=@Seq
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        public List<T> GetList<T>(int engMainSeq, int chapter)
        {
            string sql = @"
                SELECT
                    Seq,
                    FileType,
                    [Description],
                    ModifyTime
                FROM EngAttachment
                where EngMainSeq=@EngMainSeq
                and Chapter=@Chapter
                order by CreateTime desc";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@Chapter", chapter);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //s20230624
        public List<T> GetListAll<T>(int engMainSeq)
        {
            string sql = @"
                SELECT
                    Seq,
                    EngMainSeq,
                    OriginFileName,
                    UniqueFileName,
                    Chapter,
                    FileType,
                    [Description]
                FROM EngAttachment
                where EngMainSeq=@EngMainSeq
                order by Chapter";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        public int Add(EngAttachmentModel m)
        {
            string sql = @"
                insert into EngAttachment (
                    EngMainSeq,
                    OriginFileName,
                    UniqueFileName,
                    Chapter,
                    FileType,
                    Description,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @EngMainSeq,
                    @OriginFileName,
                    @UniqueFileName,
                    @Chapter,
                    @FileType,
                    @Description,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", m.EngMainSeq);
            cmd.Parameters.AddWithValue("@Chapter", m.Chapter);
            cmd.Parameters.AddWithValue("@FileType", m.FileType);
            cmd.Parameters.AddWithValue("@Description", m.Description);
            cmd.Parameters.AddWithValue("@OriginFileName", m.OriginFileName);
            cmd.Parameters.AddWithValue("@UniqueFileName", m.UniqueFileName);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            return db.ExecuteNonQuery(cmd);
            /*int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('ChartMaintain') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
            return m.Seq;*/
        }

        public int UpdateDescription(EngAttachmentModel m)
        {
            Null2Empty(m);
            string sql = @"
                update EngAttachment set
                    Description = @Description,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Description", m.Description);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@Seq", m.Seq);

            return db.ExecuteNonQuery(cmd);
        }

        public int Delete(int seq)
        {
            string sql = @"delete EngAttachment where Seq=@Seq";

            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }

        public List<T> GetItemFileInfoBySeq<T>(int seq)
        {
            string sql = @"SELECT
                Seq,
                EngMainSeq,
                OriginFileName,
                UniqueFileName
                FROM EngAttachment
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}