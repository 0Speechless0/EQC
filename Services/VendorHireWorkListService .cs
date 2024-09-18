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
    public class VendorHireWorkListService : ExcelImportService
	{
		private DBConn db = new DBConn();
		private string[,] excelHeaderMap = new string[,]
		{
			{"EngName" ,    "工程名稱"} ,
			{"OrganizerUnitName" ,  "主辦單位"} ,
			{"EngType" ,    "工程"}   ,
			{"AwardAmount" ,    "決標金額"} ,
			{"ActualBidAwardDate" , "決標"}   ,
			{"ActualProgress" , "目前"}   ,
			{"ProjectMangement" ,   "專案管理"} ,
			{"SupervisorUnitName" , "監造單位"} ,
			{"ContractorName" , "承攬廠商"},
		};
		public List<VendorHireWorkListModel> GetList(int page, int per_page, string keyWord)
		{
			string sql = @"Select
				Seq,
				EngName,
				OrganizerUnitName,
				EngType,
				AwardAmount,
				ActualBidAwardDate,
				ActualProgress,
				ProjectMangement,
				SupervisorUnitName,
				ContractorName,
				ListDate
                from VendorHireWorkList
				where @keyWord = '' or EngName Like @keyWord or ContractorName Like @keyWord
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
			return db.GetDataTableWithClass<VendorHireWorkListModel>(cmd);
		}
		public Object GetCount()
		{
			string sql = @"
				SELECT COUNT(*)
				FROM VendorHireWorkList";
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
			processer.declareCol("ListDate",new int[] { 1, 0 }, new string[] { "列表日期： ", "年", "月", "日" });

			while (currentRow < lastRow)
			{

				if (!processer.checkRowVaild(currentRow))
				{
					currentRow++;
					continue;
				}
				sql = processer.getInsertSql("VendorHireWorkList", currentRow);
				sql = processer.addDeclareColToInsertSql(sql);
				string updateSql = processer.getUpdateSql("VendorHireWorkList"
					, "EngName", currentRow);
				updateSql = processer.addDeclareColToUpdateSql(updateSql);


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

		public void ImportFromPrj(int? year = null)
        {
			using(var context = new EQC_NEW_Entities())
            {
				var targetList =  context.PrjXML.Where(r => r.TenderYear >= year || year == null).ToList().GroupJoin(
						context.VendorHireWorkList.ToList(),
						r1 => r1.Seq,
						r2 => r2.PrjXMLSeq,
						(r1, r2) =>
						{
							var current= r2.FirstOrDefault();
							if (current == null)
                            {
								return new VendorHireWorkList
								{
									EngName = r1.TenderName,
									OrganizerUnitName = r1.ExecUnitName,
									EngType = r1.EngType,
									AwardAmount = r1.BidAmount,
									ActualBidAwardDate = r1.ActualBidAwardDate,
									ActualProgress = (
									(r1.ProgressData?.LastOrDefault()?.PDAccuActualProgress ?? 0) / 100).ToString(),
									ProjectMangement = r1.PrjXMLExt?.PrjManageUnit,
									SupervisorUnitName = r1.SupervisionUnitName,
									ContractorName = r1.ContractorName1,
									PrjXMLSeq = r1.Seq,
									CreateTime = DateTime.Now
								};
							}

							else
							{

								current.EngName = r1.TenderName;
								current.OrganizerUnitName = r1.ExecUnitName;
								current.EngType = r1.EngType;
								current.AwardAmount = r1.BidAmount;
								current.ActualBidAwardDate = r1.ActualBidAwardDate;
								current.ActualProgress = (
								(r1.ProgressData?.LastOrDefault()?.PDAccuActualProgress ?? 0) / 100).ToString();
								current.ProjectMangement = r1.PrjXMLExt?.PrjManageUnit;
								current.SupervisorUnitName = r1.SupervisionUnitName;
								current.ContractorName = r1.ContractorName1;
								current.ModifyTime = DateTime.Now;
							}

							return null;
						}
					).ToList();
                var insertList = targetList.Where(r => r != null);
                context.VendorHireWorkList.AddRange(insertList);
				context.SaveChanges();
            }
        }  
		//public int Delete(int SNo)
		//{
		//	string sql = @"
		//		DELETE VendorHireWorkList WHERE SNo = @SNo";
		//	SqlCommand cmd = db.GetCommand(sql);
		//	cmd.Parameters.AddWithValue("@SNo", SNo);
		//	return db.ExecuteNonQuery(cmd);
		//}
		//public int Add(VendorHireWorkListModel collection)
		//{
		//	string sql = @"
		//		INSERT INTO  VendorHireWorkList(
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
		//public int Update(VendorHireWorkListModel collection)
		//{
		//	string sql = @"
		//		UPDATE VendorHireWorkList
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
