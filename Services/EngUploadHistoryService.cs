using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EngUploadHistoryService : BaseService
    {//監造計畫上傳歷程

        public int AddRevision(EngUploadHistoryModel m)
        {
            string sql = @"
                insert into EngUploadHistory(
                    EngMainSeq,
                    Memo,
                    OriginFileName,
                    UniqueFileName,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq,
                    VersionNo,
                    ApproveNo
                ) values (
                    @EngMainSeq,    
                    @Memo,
                    @OriginFileName,
                    @UniqueFileName,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq,
                    (select ISNULL(max(VersionNo),0)+1 from EngUploadHistory where EngMainSeq=@EngMainSeq),
                    @ApproveNo
                )";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", m.EngMainSeq);
            cmd.Parameters.AddWithValue("@Memo", m.Memo);
            cmd.Parameters.AddWithValue("@ApproveNo", m.ApproveNo);
            cmd.Parameters.AddWithValue("@originFileName", m.OriginFileName);
            cmd.Parameters.AddWithValue("@uniqueFileName", m.UniqueFileName);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            return db.ExecuteNonQuery(cmd);
        }

        public List<T> ListByEngMainSeq<T>(int engMainSeq)
        {
            string sql = @"SELECT
                Seq,
                VersionNo,
                Memo,
                ApproveNo,
                ModifyTime
                FROM EngUploadHistory
                where EngMainSeq=@EngMainSeq
                order by VersionNo DESC";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> ListByEngNo<T>(string engNo)
        {
            string sql = @"SELECT
                a.Seq,
                a.VersionNo,
                a.Memo,
                a.ModifyTime
                FROM EngUploadHistory a
                inner join EngMain b on(b.EngNo=@EngNo and b.Seq=a.EngMainSeq)
                order by a.VersionNo DESC";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngNo", engNo);
            return db.GetDataTableWithClass<T>(cmd);
        }

        public List<T> GetItemBySeq<T>(int seq)
        {
            string sql = @"
                SELECT
                    Seq,
                    VersionNo,
                    Memo,
                    ApproveNo,
                    ModifyTime
                FROM EngUploadHistory
                where Seq=@Seq
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        public List<T> GetLastItemByEngMain<T>(int engMainSeq)
        {
            string sql = @"SELECT TOP 1
                Seq,
                VersionNo,
                Memo,
                ModifyTime
                FROM EngUploadHistory
                where EngMainSeq=@EngMainSeq
                order by VersionNo DESC
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        public int UpdateName(EngUploadHistoryModel m)
        {
            string sql = @"
                update EngUploadHistory set
                    Memo = @Memo,
                    ApproveNo = @ApproveNo,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", m.Seq);
            cmd.Parameters.AddWithValue("@Memo", m.Memo);
            cmd.Parameters.AddWithValue("@ApproveNo", m.ApproveNo);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            return db.ExecuteNonQuery(cmd);
        }

        public List<EngUploadHistoryModel> GetItemFileInfoBySeq(int seq)
        {
            string sql = @"SELECT
                Seq,
                EngMainSeq,
                OriginFileName,
                UniqueFileName
                FROM EngUploadHistory
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<EngUploadHistoryModel>(cmd);
        }
    }
}