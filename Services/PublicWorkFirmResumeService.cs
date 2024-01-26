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
    public class PublicWorkFirmResumeService : ExcelImportService
	{
		private DBConn db = new DBConn();

		private Dictionary<string, int[]> excelPosMap = new Dictionary<string, int[]>() 
		{
			{"GrdAPlusAuditCnt200million5Yrs", new int[2]{8,2} },
			{"GrdAAuditCnt200million5Yrs", new int[2]{9,2} },
			{"GrdBAuditCnt200million5Yrs", new int[2]{10,2} },
			{"GrdCAuditCnt200million5Yrs", new int[2]{11,2} },
			{"GrdAPlusAuditCnt50To200million5Yrs", new int[2]{8,4} },
			{"GrdAAuditCnt50To200million5Yrs", new int[2]{9,4} },
			{"GrdBAuditCnt50To200million5Yrs", new int[2]{10,4} },
			{"GrdCAuditCnt50To200million5Yrs", new int[2]{11,4} },
			{"GrdAPlusAuditCnt10To50million5Yrs", new int[2]{8,6} },
			{"GrdAAuditCnt10To50million5Yrs", new int[2]{9,6} },
			{"GrdBAuditCnt10To50million5Yrs", new int[2]{10,6} },
			{"GrdCAuditCnt10To50million5Yrs", new int[2]{11,6} },
			{"GrdAPlusAuditCnt1To10million5Yrs", new int[2]{8,8} },
			{"GrdAAuditCnt1To10million5Yrs", new int[2]{9,8} },
			{"GrdBAuditCnt1To10million5Yrs", new int[2]{10,8} },
			{"GrdCAuditCnt1To10million5Yrs", new int[2]{11,8} }

		};
		private string[,] excelHeaderMap = 
		{
			{"CorporationName", "公司" },
			{"GrdAPlusAuditCnt200million5Yrs",  "2億以上優等" },
			{"GrdAAuditCnt200million5Yrs",  "2億以上甲等" },
			{"GrdBAuditCnt200million5Yrs",  "2億以上乙等" },
			{"GrdCAuditCnt200million5Yrs",  "2億以上丙等" },
			{"GrdAPlusAuditCnt50To200million5Yrs", "5000萬至2億優等" },
			{"GrdAAuditCnt50To200million5Yrs",  "5000萬至2億甲等" },
			{"GrdBAuditCnt50To200million5Yrs",  "5000萬至2億乙等" },
			{"GrdCAuditCnt50To200million5Yrs",  "5000萬至2億丙等" },
			{"GrdAPlusAuditCnt10To50million5Yrs",  "1000萬至5000萬優等" },
			{"GrdAAuditCnt10To50million5Yrs",  "1000萬至5000萬甲等" },
			{"GrdBAuditCnt10To50million5Yrs",  "1000萬至5000萬乙等" },
			{"GrdCAuditCnt10To50million5Yrs",  "1000萬至5000萬丙等" },
			{"GrdAPlusAuditCnt1To10million5Yrs", "100萬至1000萬優等" },
			{"GrdAAuditCnt1To10million5Yrs", "100萬至1000萬甲等" },
			{"GrdBAuditCnt1To10million5Yrs", "100萬至1000萬乙等" },
			{"GrdCAuditCnt1To10million5Yrs", "100萬至1000萬丙等" }
		};
		public List<PublicWorkFirmResumeModel> GetList(int page, int per_page, string keyWord)
		{
			string sql = @"Select
				Seq,
				CorporationName,
				GrdAPlusAuditCnt200million5Yrs,
				GrdAAuditCnt200million5Yrs,
				GrdBAuditCnt200million5Yrs,
				GrdCAuditCnt200million5Yrs,
				GrdAPlusAuditCnt50To200million5Yrs,
				GrdAAuditCnt50To200million5Yrs,
				GrdBAuditCnt50To200million5Yrs,
				GrdCAuditCnt50To200million5Yrs,
				GrdAPlusAuditCnt10To50million5Yrs,
				GrdAAuditCnt10To50million5Yrs,
				GrdBAuditCnt10To50million5Yrs,
				GrdCAuditCnt10To50million5Yrs,
				GrdAPlusAuditCnt1To10million5Yrs,
				GrdAAuditCnt1To10million5Yrs,
				GrdBAuditCnt1To10million5Yrs,
				GrdCAuditCnt1To10million5Yrs
                from PublicWorkFirmResume
				where @keyWord ='' or CorporationName Like @keyWord
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
			cmd.Parameters.AddWithValue("@keyWord", $"%{keyWord}%");
			return db.GetDataTableWithClass<PublicWorkFirmResumeModel>(cmd);
		}
		public Object GetCount()
		{
			string sql = @"
				SELECT COUNT(*)
				FROM PublicWorkFirmResume";
			SqlCommand cmd = db.GetCommand(sql);
			return db.ExecuteScalar(cmd);
		}


		public void importExcel(ExcelProcesser processer)
		{
			string sql = "";
			processer.createExcelPosMap(excelPosMap);
			StringBuilder SqlStringBuilder = new StringBuilder();
			processer.declareCol("CorporationName", new int[] { 0, 0}, new string[] { "工程有限公司公共工程履歷資料"});
			processer.declareCol("ListDate", new int[] { 1, 0 }, new string[] { "列表日期： ", "年", "月", "日" });
			processer.declareCol("GrdAAuditRatio", excelPosMap, (Dictionary<string, string> colValueMap) =>
			{
				decimal v1 = Int32.Parse(colValueMap["GrdAAuditCnt200million5Yrs"]) +
					Int32.Parse(colValueMap["GrdAAuditCnt50To200million5Yrs"]) +
					Int32.Parse(colValueMap["GrdAAuditCnt10To50million5Yrs"]) +
					Int32.Parse(colValueMap["GrdAAuditCnt1To10million5Yrs"]);

				decimal v2 = Int32.Parse(colValueMap["GrdAPlusAuditCnt200million5Yrs"]) +
					Int32.Parse(colValueMap["GrdBAuditCnt200million5Yrs"]) +
					Int32.Parse(colValueMap["GrdCAuditCnt200million5Yrs"]) +
					Int32.Parse(colValueMap["GrdAPlusAuditCnt50To200million5Yrs"]) +
					Int32.Parse(colValueMap["GrdBAuditCnt50To200million5Yrs"]) +
					Int32.Parse(colValueMap["GrdCAuditCnt50To200million5Yrs"]) +
					Int32.Parse(colValueMap["GrdAPlusAuditCnt10To50million5Yrs"]) +
					Int32.Parse(colValueMap["GrdBAuditCnt10To50million5Yrs"]) +
					Int32.Parse(colValueMap["GrdCAuditCnt10To50million5Yrs"]) +
					Int32.Parse(colValueMap["GrdAPlusAuditCnt1To10million5Yrs"]) +
					Int32.Parse(colValueMap["GrdBAuditCnt1To10million5Yrs"]) +
					Int32.Parse(colValueMap["GrdCAuditCnt1To10million5Yrs"]) +
					Int32.Parse(colValueMap["GrdAAuditCnt200million5Yrs"]) +
					Int32.Parse(colValueMap["GrdAAuditCnt50To200million5Yrs"]) +
					Int32.Parse(colValueMap["GrdAAuditCnt10To50million5Yrs"]) +
					Int32.Parse(colValueMap["GrdAAuditCnt1To10million5Yrs"]);
				if (v2 == 0) return "-1";
				decimal v3 = v1 / v2;
				return v3.ToString();

			});
			processer.declareCol("GrdCAuditNum", excelPosMap, (Dictionary<string, string> colValueMap) =>
			{
				return ( Int32.Parse(colValueMap["GrdBAuditCnt200million5Yrs"]) +
					Int32.Parse(colValueMap["GrdBAuditCnt50To200million5Yrs"]) +
					Int32.Parse(colValueMap["GrdBAuditCnt10To50million5Yrs"]) +
					Int32.Parse(colValueMap["GrdBAuditCnt1To10million5Yrs"])).ToString();

			});


			sql = processer.getInsertSql("PublicWorkFirmResume");
			sql = processer.addDeclareColToInsertSql(sql);

			string updateSql = processer.getUpdateSql("PublicWorkFirmResume"
				, "CorporationName");
			updateSql = processer.addDeclareColToUpdateSql(updateSql);

			int update = db.ExecuteNonQuery(db.GetCommand(updateSql));
			if (update == 0)
			{
				SqlCommand cmd = db.GetCommand(sql);
				db.ExecuteNonQuery(cmd);
			}


		}

		public List<object> getExcelImportFields()
		{
			List<object> list = new List<object>();
			int rowNum = excelHeaderMap.GetLength(0);
			for (int i = 0; i < rowNum; i++)
            {
				list.Add(new { fieldName = excelHeaderMap[i,0], fieldShowName = excelHeaderMap[i,1] });
			}
			return list;
		}
		//public int Delete(int seq)
		//{
		//	string sql = @"
		//		DELETE PublicWorkFirmResume WHERE Seq = @Seq";
		//	SqlCommand cmd = db.GetCommand(sql);
		//	cmd.Parameters.AddWithValue("@Seq", seq);
		//	return db.ExecuteNonQuery(cmd);
		//}
		//public int Add(PublicWorkFirmResumeModel collection)
		//{
		//	string sql = @"
		//		INSERT INTO  PublicWorkFirmResume(
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
		//public int Update(PublicWorkFirmResumeModel collection)
		//{
		//	string sql = @"
		//		UPDATE PublicWorkFirmResume
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
