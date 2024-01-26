using EQC.Common;
using EQC.EDMXModel;
using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace EQC.Services
{
	public class UnitCitiesService : ExcelImportService
	{
		private DBConn db = new DBConn();
		private string[,] excelHeaderMap = new string[,]
		{
			{ "RiverBureau" , "單位" },
			{ "Country", "縣市"}

		};
		public List<Country2WRAMapping> GetList(int page, int per_page, string keyWord)
		{
			string sql = @"Select
				Seq,
				RiverBureau,
				Country
                from Country2WRAMapping
				where RiverBureau Like @keyWord
				or Country Like @keyWord
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
			cmd.Parameters.AddWithValue("@keyWord", "%" + keyWord + "%");
			return db.GetDataTableWithClass<Country2WRAMapping>(cmd);
		}
		public Object GetCount()
		{
			string sql = @"
				SELECT COUNT(*)
				FROM Country2WRAMapping";
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
				sql = processer.getInsertSql("Country2WRAMapping", currentRow);
				string updateSql = processer.getUpdateSql("Country2WRAMapping"
					, "Country", currentRow);
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
		//public int Delete(int SNo)
		//{
		//	string sql = @"
		//		DELETE Country2WRAMapping WHERE SNo = @SNo";
		//	SqlCommand cmd = db.GetCommand(sql);
		//	cmd.Parameters.AddWithValue("@SNo", SNo);
		//	return db.ExecuteNonQuery(cmd);
		//}
		//public int Add(Country2WRAMapping collection)
		//{
		//	string sql = @"
		//		INSERT INTO  Country2WRAMapping(
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
		//	cmd.Parameters.AddWithValue("@CreateUserSNo", new SessionManager().GetUser().SNo);
		//	cmd.Parameters.AddWithValue("@ModifyUserSNo", new SessionManager().GetUser().SNo);
		//	return (Int32)db.ExecuteScalar(cmd);
		//}
		//public int Update(Country2WRAMapping collection)
		//{
		//	string sql = @"
		//		UPDATE Country2WRAMapping
		//		SET
		//			vendorname = @vendorname,
		//			Location = @Location,
		//			TW97_X = @TW97_X,
		//			TW97_Y = @TW97_Y,
		//			longitude = @longitude,
		//			latitude = @latitude
		//			,ModifyTime = GETDATE()
		//			,ModifyUserSNo = @ModifyUser
		//		WHERE SNo = @SNo";
		//	SqlCommand cmd = db.GetCommand(sql);
		//	cmd.Parameters.AddWithValue("@vendorname", collection.vendorname);
		//	cmd.Parameters.AddWithValue("@Location", collection.Location);
		//	cmd.Parameters.AddWithValue("@TW97_X", collection.TW97_X);
		//	cmd.Parameters.AddWithValue("@TW97_Y", collection.TW97_Y);
		//	cmd.Parameters.AddWithValue("@longitude", collection.longitude);
		//	cmd.Parameters.AddWithValue("@latitude", collection.latitude);
		//	cmd.Parameters.AddWithValue("@SNo", collection.SNo);
		//	cmd.Parameters.AddWithValue("@ModifyUser", new SessionManager().GetUser().SNo);
		//	return db.ExecuteNonQuery(cmd);

		//}

	}
}
