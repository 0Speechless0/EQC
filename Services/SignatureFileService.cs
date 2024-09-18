using EQC.Common;
using EQC.EDMXModel;
using EQC.Models;
using NPOI.XWPF.UserModel;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace EQC.Services
{
    public class SignatureFileService : BaseService
    {

        public bool isTextRender { get; set; }

        public void RenderConstCheckDoc(XWPFDocument doc, ConstCheckRecSheetModel recItem, 
            Action<XWPFTableCell, int? , SignatureFileService> addSignature,
             Action<XWPFTableCell, string, PictureType> addSignatureAlter
            )
        {

                using(var context = new EQC_NEW_Entities())
                {



                var roleSignatures = context.constCheckSignatures
                        .Where(r => r.ConstCheckSeq == recItem.Seq)
                        .ToDictionary(r => r.SignatureRole , r => r);
                    constCheckSignatures value;

                    if (roleSignatures.TryGetValue(2, out  value) && value.SignatureImgeBase64 == "")
                    {
                        doc.Tables[1].GetRow(0).GetCell(1).SetText(roleSignatures[2].SignatureVal);
                    }
                    else if (recItem.SupervisionComSignature != null &&
                    recItem.SupervisionComSignature.Length > 0
                    )
                    {

                        addSignatureAlter.Invoke(doc.Tables[1].GetRow(0).GetCell(3),
                            Utils.GetTempFile(".png").SaveImageByBase64(recItem.SupervisionComSignature, 1, false),
                            PictureType.PNG
                        );
                    }
                    else
                    {
                        addSignature.Invoke(doc.Tables[1].GetRow(0).GetCell(1), recItem.SupervisorUserSeq, this);
                    }



                    if (roleSignatures.TryGetValue(3, out value) && value.SignatureImgeBase64 == "")
                    {
                        doc.Tables[1].GetRow(0).GetCell(3).SetText(roleSignatures[3].SignatureVal);
                    }

                    else if (recItem.SupervisionDirectorSignature != null &&
                    recItem.SupervisionDirectorSignature.Length > 0
                    )
                    {

                        addSignatureAlter.Invoke(doc.Tables[1].GetRow(0).GetCell(1),
                             Utils.GetTempFile(".png").SaveImageByBase64(recItem.SupervisionDirectorSignature, 1, false),
                            PictureType.PNG
                        );
                    }
                
                    else
                    {
                        addSignature.Invoke(doc.Tables[1].GetRow(0).GetCell(3), recItem.SupervisorDirectorSeq, this);
                    }

         
                }
              

        }
        public int AddSignatureFile(int userMainSeq, string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string sql = @"
                insert into SignatureFile(DisplayName, FileName, FilePath, UserMainSeq)
                    values
                    (
                        '簽名.png',
                        @FileName, 
                        @FilePath, 
                        @UserMainSeq
                    )";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@UserMainSeq", userMainSeq);
            cmd.Parameters.AddWithValue("@FileName", fileName);
            cmd.Parameters.AddWithValue("@FilePath", filePath);
            return db.ExecuteNonQuery(cmd);
        }
        public int UpdateSignatureFile(int userMainSeq, string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string sql = @"
                update SignatureFile set
                    FileName = @FileName,
                    FilePath = @FilePath
                where UserMainSeq = @UserMainSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@UserMainSeq", userMainSeq);
            cmd.Parameters.AddWithValue("@FileName", fileName);
            cmd.Parameters.AddWithValue("@FilePath", filePath);
            return db.ExecuteNonQuery(cmd);
        }
        public string GetFileNameByUser(int userSeq)
        {
            string sql = @"
                SELECT FilePath+'\'+FileName FullName from SignatureFile
                where UserMainSeq = @UserMainSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@UserMainSeq", userSeq);
            DataTable dt = db.GetDataTable(cmd);
            if (dt.Rows.Count == 1)
            {
                return dt.Rows[0]["FullName"].ToString();
            }
            else
                return null;
        }

        public string GetPicFileName(int userSeq)
        {
            string sql = @"
                SELECT FilePath+'\'+FileName FullName from SignatureFile
                where UserMainSeq = @UserMainSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@UserMainSeq", userSeq);
            DataTable dt = db.GetDataTable(cmd);
            if (dt.Rows.Count == 1)
            {
                return dt.Rows[0]["FullName"].ToString();
            }
            else
                return null;
        }
    }
}