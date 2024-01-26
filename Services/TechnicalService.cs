using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Common;
using EQC.Models;
namespace EQC.Services
{
    public class TechnicalService
    {

        DBConn db = new DBConn();
        private string saveDir = HttpContext.Current.Server.MapPath("~/FileUploads/Technical");

        public class extendTechnicalArtical : TechnicalArtical
        {
            public int ArticalReplyCount { get; set; }
            public List<object> Tags { get; set; }

            public string AuthorName
            {
                get;set;
            }
  
           
        }
        public List<extendTechnicalArtical> getAllTechnicalArtical()
        {
            

           string sql = @"select 
                TechnicalArtical.Seq as Seq,
                TechnicalArtical.Title as Title,
                TechnicalArtical.Text as Text,
                TechnicalArtical.Author as Author,
                TechnicalArtical.ModifyTime as ModifyTime,
                TechnicalArtical.CreateTime as CreateTime,
                TechnicalArtical.Click as Click,
                TechnicalArtical.Url as Url,
                u.UserNo AuthorName
                ,
				case when c.ArticalReplyCount is NULL then 0 Else ArticalReplyCount END as ArticalReplyCount
				from
                (
                    select TechnicalArtical.Seq as ArticalSeq , Count(*) as ArticalReplyCount
                    from TechnicalArtical 
                    inner join TechnicalComment 
                    on TechnicalArtical.Seq = TechnicalComment.TechnicalArticalSeq
                    inner join TechnicalReply 
                    on TechnicalComment.Seq =  TechnicalReply.TechnicalCommentSeq   
                    group by TechnicalArtical.Seq
					
                ) as c 
                    right join TechnicalArtical on  TechnicalArtical.Seq = ArticalSeq
                    inner join UserMain  u on  u.Seq = TechnicalArtical.Author
                order by ModifyTime  desc";
            List<extendTechnicalArtical>  list = db.GetDataTableWithClass<extendTechnicalArtical>(db.GetCommand(sql));
            foreach (extendTechnicalArtical artical in list)
            {
                artical.Tags = getArticalTags(artical.Seq);
            }
            return list;

        }
        public Dictionary<string, string> getTechnicalArticalAuthorName()
        {

            Dictionary<string, string> articalAuthorName = new Dictionary<string, string>();
            string sql = "Select TechnicalArtical.Seq as TechnicalArticalSeq, DisplayName from TechnicalArtical inner join UserMain On UserMain.Seq = TechnicalArtical.Author ";
            return db.GetDataTable(db.GetCommand(sql)).Rows.Cast<DataRow>().ToDictionary<DataRow, string, string>(row => row.Field<int>("TechnicalArticalSeq").ToString() , row => row.Field<string>("DisplayName")) ;
        }
        public List<object> getArticalCommentWithReply(int seq)
        {
            string sql = @"select 
                Seq,
                TechnicalComment.Author as CommentAuthor, 
                TechnicalReply.Author as ReplyAuthor,
                TechnicalComment.Text as CommentText,
                TechnicalReply.Text as ReplyText,
                TechnicalComment.ModifyTime as CommentModifyTime,
                TechnicalReply.ModifyTime as ReplyModifyTime
                from TechnicalComment left join TechnicalReply on Seq = TechnicalCommentSeq where TechnicalArticalSeq=" + seq;

            string clickSql = @"Update TechnicalArtical Set Click = Click+1 where Seq=" + seq;
            db.ExecuteNonQuery(clickSql);
            return db.GetDataTable(db.GetCommand(sql)).Rows.Cast<DataRow>().Select(row => new
            {
                Seq = row.Field<int>("Seq"),
                CommentAuthor = db.ExecuteScalar("select DisplayName from UserMain where Seq="+row.Field<int?>("CommentAuthor")),
                ReplyAuthor = db.ExecuteScalar("select DisplayName from UserMain where Seq=" + (row.Field<int?>("ReplyAuthor") ?? -1)),
                CommentAuthorSeq = row.Field<int?>("CommentAuthor"),
                ReplyAuthorSeq = row.Field<int?>("ReplyAuthor"),
                CommentText = row.Field<string>("CommentText"),
                ReplyText = row.Field<string>("ReplyText"),
                CommentModifyTime = row.Field<DateTime?>("CommentModifyTime"),
                ReplyModifyTime = row.Field<DateTime?>("ReplyModifyTime"),
                CommentModifyTimeStr = row.Field<DateTime?>("CommentModifyTime")?.ToString("yyyy-MM-dd HH:mm:ss"),
                ReplyModifyTimeStr = row.Field<DateTime?>("ReplyModifyTime")?.ToString("yyyy-MM-dd HH:mm:ss")
            }
            ).ToList<object>();

           


        }


        public void saveUploadFile(HttpPostedFileBase file, int id, string dir)
        {
            if(file.ContentType.Split('/')[0] == "video")
            {
                dir += "\\" + "vdo";
            }
            else if (file.ContentType.Split('/')[0] == "image")
            {
                dir += "\\" + "img";
            }
            else
            {
                dir += "\\" + "doc";
            }
            Directory.CreateDirectory(Path.Combine(saveDir, dir, id.ToString()));
            string fullPath = Path.Combine(saveDir, dir, id.ToString(), file.FileName);
            file.SaveAs(fullPath);
        }

        public List<string> getAllFileNameInDir(int id, string dir)
        {

            List<string> articalFilesPath = new List<string>();
            try
            {

                foreach (string classDir in Directory.GetDirectories(Path.Combine(saveDir, dir)))
                {
                    try
                    {
                        var getFilesPath = Directory.GetFiles(Path.Combine(classDir, id.ToString())).Select(path => path.Replace(saveDir, ""));
                        articalFilesPath.AddRange(getFilesPath);
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        continue;
                    }

                }
            }
            catch (DirectoryNotFoundException e)
            {

            }




            return articalFilesPath;

        }


        public List<object> getCommentAndReplyPathByArtical(int id)
        {
            string sql = @"select Seq from TechnicalComment where TechnicalArticalSeq=" + id;
            List<int> CommentSeqs = db.GetDataTable(db.GetCommand(sql)).Rows.Cast<DataRow>().Select(row => row.Field<int>("Seq")).ToList();
            List<object> filePathList = new List<object>();
            foreach (int seq in CommentSeqs)
            {
                string path = Path.Combine(saveDir, "TechnicalComments");
                string path2 = Path.Combine(saveDir, "TechnicalReplys");

                List<object> list1 = new List<object>();
                List<object> list2 = new List<object>();
                try
                {

                    list2.AddRange ( Directory.GetFiles(Path.Combine(path2, "vdo", seq.ToString())).Select(file => new { filePath = file.Replace(saveDir, "").Replace(@"\\", "/"), fileType = "video" }).ToList<object>() );
                }
                catch (DirectoryNotFoundException e)
                {

                }
                try
                {
                    list1.AddRange ( Directory.GetFiles(Path.Combine(path, "vdo", seq.ToString())).Select(file => new { filePath = file.Replace(saveDir, "").Replace(@"\\", "/"), fileType = "video" }).ToList<object>() );
                }
                catch (DirectoryNotFoundException e)
                {
                  
                }

                try
                {
                    list2.AddRange( Directory.GetFiles(Path.Combine(path2, "img", seq.ToString())).Select(file => new { filePath = file.Replace(saveDir, "").Replace(@"\\", "/"), fileType = "image" }).ToList<object>() );                   

                }
                catch (DirectoryNotFoundException e)
                {

                }

                try
                {

                    list1.AddRange ( Directory.GetFiles(Path.Combine(path, "img", seq.ToString())).Select(file => new { filePath = file.Replace(saveDir, "").Replace(@"\\", "/"), fileType = "image" }).ToList<object>() );

                }
                catch (DirectoryNotFoundException e)
                {
               
                }

                filePathList.Add(new
                {
                    CommentSeqs = seq,
                    CommentPaths = list1,
                    ReplyPaths = list2
                });
               

            }
            return filePathList;
        }

        internal void updateReply(int id, TechnicalReply value)
        {
            string sql = @"Update TechnicalReply Set
                Text = @Text,
                ModifyTime = GetDate()
            where TechnicalCommentSeq=" + id;
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Text", value.Text);
            db.ExecuteNonQuery(cmd);
        }

        internal List<object> getAllTag()
        {
            string sql = @"Select * from TechnicalTag order by CreateTime desc";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTable(cmd).Rows.Cast<DataRow>().Select(row => new { Seq = row.Field<int>("Seq"), Text = row.Field<string>("Text") }).ToList<object>();

        }

        private List<object> getArticalTags(int id)
        {
            string sql = @"Select TechnicalTag.Seq ,TechnicalTag.Text from TechnicalArticalMTag 
                inner join TechnicalTag on TechnicalTagSeq = TechnicalTag.Seq
				where TechnicalArticalSeq = @ArticalSeq 
                order by TechnicalTag.CreateTime desc
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@ArticalSeq", id);
            return db.GetDataTable(cmd).Rows.Cast<DataRow>().Select(row => new { Seq = row.Field<int>("Seq"), Text = row.Field<string>("Text") }).ToList<object>();
        }


        internal void storeTag(string text)
        {
            string sql = @"Insert into TechnicalTag(
                Text,
                Author,
                CreateTime
            ) values (
                @Text,
                @Author,
                GetDate()
            )";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Text", text);
            cmd.Parameters.AddWithValue("@Author", new SessionManager().GetUser().Seq);

            db.ExecuteNonQuery(cmd);

        }
        internal void deleteTag(int id)
        {
            string sql = @"Delete from TechnicalTag where Seq=" + id;

            db.ExecuteNonQuery(sql);

        }
        internal void deleteComment(int id)
        {
            string sql = @"Delete from TechnicalComment where Seq=" + id;


            int affect = db.ExecuteNonQuery(sql);
            if (affect > 0)
            {
                deleteUploadFile(id, "TechnicalComments");
            }
        }

        internal void updateComment(int id, TechnicalComment value)
        {
            string sql = @"Update TechnicalComment Set
                Text = @Text,
                ModifyTime = GetDate() 
             where Seq="+id;
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Text", value.Text);
            db.ExecuteNonQuery(cmd);
        }

        public int addArtical(FormCollection artical)
        {

            string sql = @"Insert into TechnicalArtical(
                Text,
                Title,
                CreateTime,
                ModifyTime,
                Author,
                Url

            )output Inserted.Seq values
            (
                @Text,
                @Title,
                GetDate(),
                GetDate(),
                @Author,
                " + (artical["Url"] == " " ? "NULL" : "'"+ artical["Url"] + "'" )+ @")";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Text", artical["Text"]);
            cmd.Parameters.AddWithValue("@Title", artical["Title"]);
            cmd.Parameters.AddWithValue("@Author", new SessionManager().GetUser().Seq );
            return (int)db.ExecuteScalar(cmd);
        }
        public void storeArticalTags(int articalSeq, List<int> tags)
        {
            if (tags.Count == 0) return;
            string sql = @"Insert into TechnicalArticalMTag(TechnicalArticalSeq, TechnicalTagSeq) values";
            foreach(int tagSeq in tags)
            {
                sql += "("+articalSeq+", "+tagSeq+"),";
            }
            sql = sql.Substring(0, sql.Length - 1);
            db.ExecuteNonQuery(sql);
        }
        internal void deleteReply(int id)
        {
            string sql = @"Delete from TechnicalReply where TechnicalCommentSeq=" + id;

            int affect = db.ExecuteNonQuery(sql);
            if (affect > 0)
            {
                deleteUploadFile(id, "TechnicalReplys");
            }
        }

        public int addReply(TechnicalReply value)
        {
            string sql = @"Insert into TechnicalReply(
                TechnicalCommentSeq,
                Text,
                Author,
                CreateTime,
                ModifyTime
            ) output Inserted.TechnicalCommentSeq values (
                @CommentSeq,
                @Text,
                @Author,
                GetDate(),
                GetDate()
            )";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@CommentSeq", value.TechnicalCommentSeq);
            cmd.Parameters.AddWithValue("@Text", value.Text);
            cmd.Parameters.AddWithValue("@Author", new SessionManager().GetUser().Seq);

            return (int)db.ExecuteScalar(cmd);
        }

        public void giveThumb(int replyId)
        {
            string sql = @"Insert into Thumb([User], Reply) values(@User, @Reply)";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@User", new SessionManager().GetUser().Seq);
            cmd.Parameters.AddWithValue("@Reply", replyId);
            db.ExecuteNonQuery(cmd);
        }

        public void recoveryThumb(int id)
        {
            string sql = @"Delete Thumb where [User]=" + new SessionManager().GetUser().Seq;
            db.ExecuteNonQuery(sql);
        }

        internal bool checkReplyThumb(int id)
        {
            string sql = @"Select Count(*) from Thumb where Reply = @Reply and [User] = @User";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Reply", id);
            cmd.Parameters.AddWithValue("@User", new SessionManager().GetUser().Seq);
            return (int)db.ExecuteScalar(cmd) > 0;
        }

        public int getReplyThumb(int id)
        {
            string sql = @"Select Count(*) as totalThumb from Thumb where Reply ="+id;
            return (int)db.ExecuteScalar(sql);

        }

        public int addComment(TechnicalComment value)
        {
            string sql = @"Insert into TechnicalComment(
                TechnicalArticalSeq,
                Text,
                Author,
                CreateTime,
                ModifyTime
            ) output Inserted.Seq values (
                @TechnicalArticalSeq,
                @Text,
                @Author,
                GetDate(),
                GetDate()
            )";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@TechnicalArticalSeq", value.TechnicalArticalSeq);
            cmd.Parameters.AddWithValue("@Text", value.Text);
            cmd.Parameters.AddWithValue("@Author", new SessionManager().GetUser().Seq);

            return (int)db.ExecuteScalar(cmd);
        }

        public void updateArtical(int seq, TechnicalArtical artical)
        {
            string sql= @"Update TechnicalArtical Set
                Text = @Text,
                Title = @Title,
                ModifyTime = GetDate()
            where Seq=" + seq;
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Text", artical.Text);
            cmd.Parameters.AddWithValue("@Title", artical.Title);
            db.ExecuteNonQuery(cmd);
        }

        public void resetArticalTags(int articalSeq, List<int> tags)
        {
            string sql = @"Delete TechnicalArticalMTag where TechnicalArticalSeq=" + articalSeq;
            db.ExecuteNonQuery(sql); ;
            storeArticalTags(articalSeq, tags);


        }
        public void deleteTags(List<int> seqs)
        {
            string sql = @"Delete TechnicalTag where Seq in (";
            foreach(int seq in seqs)
            {
                sql += seq + ",";
            }
            sql = sql.Remove(sql.Length - 1) + ")";

            db.ExecuteNonQuery(sql);
        }

        public void deleteArtical(int seq)
        {
            string sql = @"Delete from TechnicalArtical where Seq=" + seq;
            
            int affect = db.ExecuteNonQuery(sql);
            if(affect >  0)
            {
                deleteUploadFile(seq, "TechnicalArticals");
            }
        }

        public int getArticalReplyCount(int id)
        {
            string sql = @"select Count(*) from TechnicalComment inner join TechnicalReply on TechnicalComment.Seq =  TechnicalReply.TechnicalCommentSeq where TechnicalArticalSeq = " + id;
            return (int)db.ExecuteScalar(sql);

        }
        private void deleteUploadFile(int seq, string path)
        {
            if (!Directory.Exists(Path.Combine(saveDir, path))) return;
            foreach (string typeDir in Directory.GetDirectories(Path.Combine(saveDir, path) ))
            {
                if (!Directory.Exists(Path.Combine(saveDir, path, typeDir))) continue;
                foreach (string seqDir in Directory.GetDirectories(Path.Combine(saveDir, path, typeDir)) )
                {
                    if(
                        Path.Combine(saveDir, path, typeDir, seqDir.ToString()) == 
                        Path.Combine(saveDir, path, typeDir, seq.ToString() ) )
                    {
                        Directory.Delete(Path.Combine(saveDir, path, typeDir, seq.ToString() ), true);
                    }
                }
            }
        }
    }
}