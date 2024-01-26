using EQC.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class SupervisionProjectTpService : BaseService
    {//監造計畫書範本
        public int Add(SupervisionProjectTpModel m)
        {
            string sql = @"
                insert into SupervisionProjectTp(
                    Name,
                    CreateTime,
                    CreateUserSeq
                ) values (
                    @Name,
                    GetDate(),
                    @CreateUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Name", m.Name);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());

            return db.ExecuteNonQuery(cmd);
        }

        public List<SPTpEditModel> ListAll()
        {
            string sql = @"SELECT
                Seq,
                RevisionDate,
                Name,
                OriginFileName
                FROM SupervisionProjectTp";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<SPTpEditModel>(cmd);
        }

        public List<SPTpEditModel> GetItemBySeq(int seq)
        {
            string sql = @"SELECT
                Seq,
                RevisionDate,
                Name
                FROM SupervisionProjectTp
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<SPTpEditModel>(cmd);
        }

        public List<SPTpEditModel> GetItemFileInfoBySeq(int seq)
        {
            string sql = @"SELECT
                Seq,
                OriginFileName,
                UniqueFileName
                FROM SupervisionProjectTp
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<SPTpEditModel>(cmd);
        }

        public int UpdateUploadFile(int seq, string originFileName, string uniqueFileName)
        {
            string sql = @"
                update SupervisionProjectTp set
                    OriginFileName = @originFileName,
                    UniqueFileName = @uniqueFileName,
                    RevisionDate = GETDATE(),
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