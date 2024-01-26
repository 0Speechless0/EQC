using EQC.Common;
using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace EQC.Services
{
    public class NSActivityService : ExcelImportService
	{
        private DBConn db = new DBConn();
        private string[,] excelHeaderMap = new string[,]
        {
            {"PublicBulletinNo", "通報案件編號"},
            {"PublicBulletinDate" , "通報日期"},
            {"PublicBulletinPerson" , "通報人"},
            {"ConstructionName" , "通報工程"},
            {"ConstructionSubject" , "通報主題"},
            {"AssignDate" , "分文日期"},
            {"ProcessingECUnit" , "目前處理機關"},
            {"ExpectDateCaseClosed" , "應結案日期"},
            {"ActualDateCaseClosed" , "結案日期"},
            {"PhotoQuantity" , "照片數"},


        };
		public List<NSActivityModel> GetList(int page, int per_page, string keyWord)
		{
			string sql = @"Select 
				Seq,
				PublicBulletinNo,
				PublicBulletinDate,
				PublicBulletinPerson,
				ConstructionName,
				ConstructionSubject,
				AssignDate,
				ProcessingECUnit,
				ExpectDateCaseClosed,
				ActualDateCaseClosed,
				PhotoQuantity
                from NationalSupervisedActivity
				where ConstructionName Like @keyWord
				ORDER BY CASE @Sort_by
					WHEN 'Seq'
					THEN Seq
					END
				OFFSET @Page ROWS
				FETCH FIRST @Per_page ROWS ONLY";


			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@Sort_by", "Seq");
			cmd.Parameters.AddWithValue("@Page", page * per_page);
			cmd.Parameters.AddWithValue("@Per_page", per_page);
			cmd.Parameters.AddWithValue("@keyWord", "%"+keyWord+"%");
			return db.GetDataTableWithClass<NSActivityModel>(cmd);
		}
		public int Add(NSActivityModel collection)
		{
			string sql = @"
				INSERT INTO  NationalSupervisedActivity(
						PublicBulletinNo,
						PublicBulletinDate,
						PublicBulletinPerson,
						ConstructionName,
						ConstructionSubject,
						AssignDate,
						ProcessingECUnit,
						ExpectDateCaseClosed,
						ActualDateCaseClosed,
						PhotoQuantity,
						CreateUser,
						ModifyUser,
						CreateTime,
						ModifyTime
					)
				VALUES (
						@PublicBulletinNo,
						@PublicBulletinDate,
						@PublicBulletinPerson,
						@ConstructionName,
						@ConstructionSubject,
						@AssignDate,
						@ProcessingECUnit,
						@ExpectDateCaseClosed,
						@ActualDateCaseClosed,
						@PhotoQuantity,
						@CreateUser,
						@ModifyUser,
						GETDATE(),
						GETDATE()
					) "
					 + "SELECT CAST(scope_identity() AS int)"; ;
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@PublicBulletinNo", collection.PublicBulletinNo);
			cmd.Parameters.AddWithValue("@PublicBulletinDate", collection.PublicBulletinDate);
			cmd.Parameters.AddWithValue("@PublicBulletinPerson", collection.PublicBulletinPerson);
			cmd.Parameters.AddWithValue("@ConstructionName", collection.ConstructionName);
			cmd.Parameters.AddWithValue("@ConstructionSubject", collection.ConstructionSubject);
			cmd.Parameters.AddWithValue("@AssignDate", collection.AssignDate);
			cmd.Parameters.AddWithValue("@ProcessingECUnit", collection.ProcessingECUnit);
			cmd.Parameters.AddWithValue("@ExpectDateCaseClosed", collection.ExpectDateCaseClosed);
			cmd.Parameters.AddWithValue("@ActualDateCaseClosed", collection.ActualDateCaseClosed);
			cmd.Parameters.AddWithValue("@PhotoQuantity", collection.PhotoQuantity);

			cmd.Parameters.AddWithValue("@CreateUserSeq", new SessionManager().GetUser().Seq);
			cmd.Parameters.AddWithValue("@ModifyUserSeq", new SessionManager().GetUser().Seq);
			return (Int32)db.ExecuteScalar(cmd);
		}
		public int Update(NSActivityModel collection)
		{
			string sql = @"
				UPDATE NationalSupervisedActivity
				SET
					PublicBulletinNo  = @PublicBulletinNo,
					PublicBulletinDate  = @PublicBulletinDate,
					PublicBulletinPerson  = @PublicBulletinPerson,
					ConstructionName  = @ConstructionName,
					ConstructionSubject  = @ConstructionSubject,
					AssignDate  = @AssignDate,
					ProcessingECUnit  = @ProcessingECUnit,
					ExpectDateCaseClosed  = @ExpectDateCaseClosed,
					ActualDateCaseClosed  = @ActualDateCaseClosed,
					PhotoQuantity  = @PhotoQuantity
					,ModifyTime = GETDATE()
					,ModifyUserSeq = @ModifyUser
				WHERE Seq = @Seq";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@PublicBulletinNo", collection.PublicBulletinNo);
			cmd.Parameters.AddWithValue("@PublicBulletinDate", collection.PublicBulletinDate);
			cmd.Parameters.AddWithValue("@PublicBulletinPerson", collection.PublicBulletinPerson);
			cmd.Parameters.AddWithValue("@ConstructionName", collection.ConstructionName);
			cmd.Parameters.AddWithValue("@ConstructionSubject", collection.ConstructionSubject);
			cmd.Parameters.AddWithValue("@AssignDate", collection.AssignDate);
			cmd.Parameters.AddWithValue("@ProcessingECUnit", collection.ProcessingECUnit);
			cmd.Parameters.AddWithValue("@ExpectDateCaseClosed", collection.ExpectDateCaseClosed);
			cmd.Parameters.AddWithValue("@ActualDateCaseClosed", collection.ActualDateCaseClosed);
			cmd.Parameters.AddWithValue("@PhotoQuantity", collection.PhotoQuantity);
			cmd.Parameters.AddWithValue("@Seq", collection.Seq);
			cmd.Parameters.AddWithValue("@ModifyUser", new SessionManager().GetUser().Seq);
			return db.ExecuteNonQuery(cmd);

		}
		public Object GetCount()
		{
			string sql = @"
				SELECT COUNT(*)
				FROM NationalSupervisedActivity";
			SqlCommand cmd = db.GetCommand(sql);
			return db.ExecuteScalar(cmd);
		}
		public int Delete(int seq)
		{
			string sql = @"
				DELETE NationalSupervisedActivity WHERE Seq = @Seq";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@Seq", seq);
			return db.ExecuteNonQuery(cmd);
		}


		public void importExcel(ExcelProcesser processer)
        {
            string sql = "";
            processer.createDbColumnMapToHeader(excelHeaderMap);
            int currentRow = processer.getStartRow();
            int lastRow = processer.getEndRow();
			StringBuilder SqlStringBuilder = new StringBuilder();


			while (currentRow < lastRow)
            {

                if (!processer.checkRowVaild(currentRow))
                {
                    currentRow++;
                    continue;
                }
				sql = processer.getInsertSql("NationalSupervisedActivity", currentRow) ;
				string updateSql = processer.getUpdateSql("NationalSupervisedActivity"
					, "PublicBulletinNo", currentRow);
				int update = db.ExecuteNonQuery(db.GetCommand(updateSql));
				if (update == 0)
				{
					SqlCommand cmd = db.GetCommand(sql);
					db.ExecuteNonQuery(cmd);
				}
                currentRow++;
            }

        }

        public List<object> getExcelImportFields()
        {
            int rowNum = excelHeaderMap.GetLength(0);
			List<object> list = new List<object>();
            for (int i = 0; i < rowNum; i++)
            {
				list.Add(new { fieldName = excelHeaderMap[i, 0], fieldShowName = excelHeaderMap[i, 1] });
            }

			return list;
        }
		public string[,] getExcelImportFieldsStr()
        {
			return excelHeaderMap;
        }

	}
}