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
    public class GravelFieldCoordService : ExcelImportService
	{
		private DBConn db = new DBConn();
		private string[,] excelHeaderMap = new string[,]
		{
			{"vendorname", "廠商"},
			{"Location", "所在地" },
			{"TW97_X", "TW97_X"},
			{"TW97_Y", "TW97_Y"},
			{"longitude", "經度"},
			{"latitude", "緯度" }
		};
		public List<GravelFieldCoordModel> GetList(int page, int per_page, string keyWord)
		{
			string sql = @"Select
				Seq,
				vendorname,
				Location,
				TW97_X,
				TW97_Y,
				longitude,
				latitude
                from Gravelfieldcoord
				where vendorname Like @keyWord
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
			return db.GetDataTableWithClass<GravelFieldCoordModel>(cmd);
		}
		public Object GetCount()
		{
			string sql = @"
				SELECT COUNT(*)
				FROM Gravelfieldcoord";
			SqlCommand cmd = db.GetCommand(sql);
			return db.ExecuteScalar(cmd);
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
				sql = processer.getInsertSql("Gravelfieldcoord", currentRow);
				string updateSql = processer.getUpdateSql("Gravelfieldcoord"
					, "vendorname", currentRow);
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
		//public int Delete(int seq)
		//{
		//	string sql = @"
		//		DELETE Gravelfieldcoord WHERE Seq = @Seq";
		//	SqlCommand cmd = db.GetCommand(sql);
		//	cmd.Parameters.AddWithValue("@Seq", seq);
		//	return db.ExecuteNonQuery(cmd);
		//}
		//public int Add(GravelFieldCoordModel collection)
		//{
		//	string sql = @"
		//		INSERT INTO  Gravelfieldcoord(
		//				vendorname,
		//				Location,
		//				TW97_X,
		//				TW97_Y,
		//				longitude,
		//				latitude,
		//				CreateUser,
		//				ModifyUser,
		//				CreateTime,
		//				ModifyTime
		//			)
		//		VALUES (
		//				@vendorname,
		//				@Location,
		//				@TW97_X,
		//				@TW97_Y,
		//				@longitude,
		//				@latitude,
		//				@CreateUser,
		//				@ModifyUser,
		//				GETDATE(),
		//				GETDATE()
		//			) "
		//			 + "SELECT CAST(scope_identity() AS int)"; ;
		//	SqlCommand cmd = db.GetCommand(sql);


		//	cmd.Parameters.AddWithValue("@vendorname", collection.vendorname);
		//	cmd.Parameters.AddWithValue("@Location", collection.Location);
		//	cmd.Parameters.AddWithValue("@TW97_X", collection.TW97_X);
		//	cmd.Parameters.AddWithValue("@TW97_Y", collection.TW97_Y);
		//	cmd.Parameters.AddWithValue("@longitude", collection.longitude);
		//	cmd.Parameters.AddWithValue("@latitude", collection.latitude);
		//	cmd.Parameters.AddWithValue("@CreateUserSeq", new SessionManager().GetUser().Seq);
		//	cmd.Parameters.AddWithValue("@ModifyUserSeq", new SessionManager().GetUser().Seq);
		//	return (Int32)db.ExecuteScalar(cmd);
		//}
		//public int Update(GravelFieldCoordModel collection)
		//{
		//	string sql = @"
		//		UPDATE Gravelfieldcoord
		//		SET
		//			vendorname = @vendorname,
		//			Location = @Location,
		//			TW97_X = @TW97_X,
		//			TW97_Y = @TW97_Y,
		//			longitude = @longitude,
		//			latitude = @latitude
		//			,ModifyTime = GETDATE()
		//			,ModifyUserSeq = @ModifyUser
		//		WHERE Seq = @Seq";
		//	SqlCommand cmd = db.GetCommand(sql);
		//	cmd.Parameters.AddWithValue("@vendorname", collection.vendorname);
		//	cmd.Parameters.AddWithValue("@Location", collection.Location);
		//	cmd.Parameters.AddWithValue("@TW97_X", collection.TW97_X);
		//	cmd.Parameters.AddWithValue("@TW97_Y", collection.TW97_Y);
		//	cmd.Parameters.AddWithValue("@longitude", collection.longitude);
		//	cmd.Parameters.AddWithValue("@latitude", collection.latitude);
		//	cmd.Parameters.AddWithValue("@Seq", collection.Seq);
		//	cmd.Parameters.AddWithValue("@ModifyUser", new SessionManager().GetUser().Seq);
		//	return db.ExecuteNonQuery(cmd);

		//}

	}
}
