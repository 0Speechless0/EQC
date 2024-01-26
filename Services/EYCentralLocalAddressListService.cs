using EQC.Common;
using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace EQC.Services
{
    public class EYCentralLocalAddressListService : ExcelImportService
	{
		private DBConn db = new DBConn();
		private string[,] excelHeaderMap = new string[,]
		{
			{"OrganizerId" ,    "機關代碼" },
			{"OrganizerName" ,  "機關名稱" },
			{"OrganizerAddress" ,   "機關地址" },
			{"OrganizerTEL" ,   "機關電話" },
			{"OrganizerArea" ,  "所屬區域" }

		};
		public List<EYCentralLocalAddressListModel> GetList(int page, int per_page, string keyWord, string location)
		{
			string sql = @"Select
				Seq,
				OrganizerId,
				OrganizerName,
				OrganizerAddress,
				OrganizerTEL,
				OrganizerArea
                from EYCentralLocalAddressList
				where OrganizerName Like @keyWord and
						OrganizerArea Like @location
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
			cmd.Parameters.AddWithValue("@location", "%" + location + "%");
			return db.GetDataTableWithClass<EYCentralLocalAddressListModel>(cmd);
		}

		public List<string> getOrganizerArea()
        {
			string sql = @"select OrganizerArea from EYCentralLocalAddressList group by  OrganizerArea";
			return db.GetDataTable(db.GetCommand(sql)).Rows.Cast<DataRow>().Select(row => row.Field<string>("OrganizerArea")).ToList();

		}
		public Object GetCount()
		{
			string sql = @"
				SELECT COUNT(*)
				FROM EYCentralLocalAddressList";
			SqlCommand cmd = db.GetCommand(sql);
			return db.ExecuteScalar(cmd);
		}

        internal List<string> GetOrganizerList()
        {
            throw new NotImplementedException();
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
				sql = processer.getInsertSql("EYCentralLocalAddressList", currentRow);
				string updateSql = processer.getUpdateSql("EYCentralLocalAddressList"
					, "OrganizerId", currentRow);
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
		//		DELETE EYCentralLocalAddressList WHERE Seq = @Seq";
		//	SqlCommand cmd = db.GetCommand(sql);
		//	cmd.Parameters.AddWithValue("@Seq", seq);
		//	return db.ExecuteNonQuery(cmd);
		//}
		//public int Add(EYCentralLocalAddressListModel collection)
		//{
		//	string sql = @"
		//		INSERT INTO  EYCentralLocalAddressList(
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
		//public int Update(EYCentralLocalAddressListModel collection)
		//{
		//	string sql = @"
		//		UPDATE EYCentralLocalAddressList
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
