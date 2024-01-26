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
    public class AuditCaseListService : ExcelImportService
    {
		private DBConn db = new DBConn();
		private string[,] excelHeaderMap = new string[,]
		{
			{"EngName", "工程名稱" },
			{"ContractNo",  "標案編號" },
			{"OrganizerUnit",   "主辦單位" },
			{"FundingSourceName",   "經費來源" },
			{"ActualBidAwardDate",  "實際決標日" },
			{"ContractAmount",  "契約金額" },
			{"AuditDate",   "查核日期" },
			{"AuditUnit",   "查核督導小組" },
			{"InformMethod",    "通知方式" },
			{"InsideCommittee", "內聘委員" },
			{"OutsideCommittee",    "外聘委員" },
			{"JoinPerson",  "參與人員" },
			{"Score",   "評分" },
			{"VendorDeductPoint",   "廠商扣點" },
			{"SupervisionDeductPoint",  "監造扣點" },
			{"PCMDeductionPoint",   "PCM扣點" },
			{"ImproveResult",   "改善結果" },
			{"PSIssueNoDate",   "發文日期文號" },
			{"ApprovalNoDate",  "回文日期文號" },
			{"AuditUnitSugesstion", "查核建議處置" },
			{"ECUnitRealResponse",  "機關實際處置" },
			{"Momo",    "備註" },
			{"EngType", "工程類別" },
			{"BelongPrj",   "歸屬計畫" }

		};
		public List<AuditCaseListModel> GetList(int page, int per_page, string keyWord)
		{
			string sql = @"Select
				Seq,
				EngName,
				ContractNo,
				OrganizerUnit,
				FundingSourceName,
				ActualBidAwardDate,
				ContractAmount,
				AuditDate,
				AuditUnit,
				InformMethod,
				InsideCommittee,
				OutsideCommittee,
				JoinPerson,
				Score,
				VendorDeductPoint,
				SupervisionDeductPoint,
				PCMDeductionPoint,
				ImproveResult,
				PSIssueNoDate,
				ApprovalNoDate,
				AuditUnitSugesstion,
				ECUnitRealResponse,
				Momo,
				EngType,
				BelongPrj
                from AuditCaseList
				where EngName Like @keyWord
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
			return db.GetDataTableWithClass<AuditCaseListModel>(cmd);
		}
		public Object GetCount()
		{
			string sql = @"
				SELECT COUNT(*)
				FROM AuditCaseList";
			SqlCommand cmd = db.GetCommand(sql);
			return db.ExecuteScalar(cmd);
		}


		public void importExcel(ExcelProcesser processer)
		{
			string sql = "";
			string updateSql ="";
			processer.createDbColumnMapToHeader(excelHeaderMap);
			int currentRow = processer.getStartRow();
			int lastRow = processer.getEndRow();
			StringBuilder SqlStringBuilder = new StringBuilder();


			while (currentRow < lastRow)
			{
				try
                {
					if (!processer.checkRowVaild(currentRow))
					{
						currentRow++;
						continue;
					}
					sql = processer.getInsertSql("AuditCaseList", currentRow);
					updateSql = processer.getUpdateSql("AuditCaseList"
						, "EngName", currentRow);
					int update = db.ExecuteNonQuery(db.GetCommand(updateSql));
					if (update == 0)
					{
						SqlCommand cmd = db.GetCommand(sql);
						db.ExecuteNonQuery(cmd);
					}
					currentRow++;
				}
				catch(Exception e)
                {
					throw e;
                }
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
		//		DELETE AuditCaseList WHERE SNo = @SNo";
		//	SqlCommand cmd = db.GetCommand(sql);
		//	cmd.Parameters.AddWithValue("@SNo", SNo);
		//	return db.ExecuteNonQuery(cmd);
		//}
		//public int Add(AuditCaseListModel collection)
		//{
		//	string sql = @"
		//		INSERT INTO  AuditCaseList(
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
		//public int Update(AuditCaseListModel collection)
		//{
		//	string sql = @"
		//		UPDATE AuditCaseList
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
