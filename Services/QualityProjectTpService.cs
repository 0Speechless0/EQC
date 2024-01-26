using EQC.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class QualityProjectTpService : BaseService
    {//監造計畫書範本
        public int Add(QualityProjectTpModel m)
        {
            string sql = @"
                insert into QualityProjectTp(
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

        public List<QualityProjectEditModel> ListAll()
        {
            string sql = @"SELECT
                Seq,
                RevisionDate,
                Name,
                OriginFileName
                FROM QualityProjectTp";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<QualityProjectEditModel>(cmd);
        }

        public List<QualityProjectEditModel> GetItemBySeq(int seq)
        {
            string sql = @"SELECT
                Seq,
                RevisionDate,
                Name
                FROM QualityProjectTp
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<QualityProjectEditModel>(cmd);
        }

        public List<QualityProjectEditModel> GetItemFileInfoBySeq(int seq)
        {
            string sql = @"SELECT
                Seq,
                OriginFileName,
                UniqueFileName
                FROM QualityProjectTp
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<QualityProjectEditModel>(cmd);
        }

        public int UpdateUploadFile(int seq, string originFileName, string uniqueFileName)
        {
            string sql = @"
                update QualityProjectTp set
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