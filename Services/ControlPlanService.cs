using EQC.Common;
using EQC.EDMXModel;
using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace EQC.Services
{
    public class ControlPlanService : ExcelImportService
    {
		private DBConn db = new DBConn();
		private string[,] excelHeaderMap = new string[,]
		{
			{ "ProcuringEntityId", "主辦機關代碼"},
			{ "ProjectNo", "計畫編號"},
			{ "ProjectName", "計畫名稱"},
			{ "PlanOrganizerName", "計畫主辦機關名稱"},
			{ "Memo", "備註"},
			{ "SupervisionUnit", "督導單位/計畫主辦"},
			{ "PlanningSection", "計畫組室"},
			{ "hostingPerson", "計畫主辦"}
		};
		public List<wraControlPlanNo> GetList(int page, int per_page, string keyWord)
		{
			string sql = @"Select
				Seq,
				ProcuringEntityId,
				ProjectNo,
				ProjectName,
				PlanOrganizerName,
				Memo,
				SupervisionUnit,
				PlanningSection,
				hostingPerson
                from wraControlPlanNo
				where ProjectName Like @keyWord 
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
			return db.GetDataTableWithClass<wraControlPlanNo>(cmd);
		}
		public Object GetCount()
		{
			string sql = @"
				SELECT COUNT(*)
				FROM wraControlPlanNo";
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
				//using (var context = new EQC_NEW_Entities())
				//{
					try
					{
						if (!processer.checkRowVaild(currentRow))
						{
							currentRow++;
							continue;
						}
						sql = processer.getInsertSql("wraControlPlanNo", currentRow);
						string updateSql = processer.getUpdateSql("wraControlPlanNo"
							, "ProjectName", currentRow);

						//var rx = new Regex(@"(?<column>\w+)\s?=\s?(?<value>@\w+)");
						//var projectNameMatch = rx.Matches(updateSql).Cast<Match>().ToList()
						//	.Where(r => r.Groups["column"].Value == "ProjectName").FirstOrDefault();
						//if (projectNameMatch != null)
						//{
						//	var value = projectNameMatch.Groups["value"].Value;
							
						//	context.PrjXML.Where(r => r.ManualBelongPrj.Contains(value) )
						//		.ToList()


						//}
						int update = db.ExecuteNonQuery(db.GetCommand(updateSql));
						if (update == 0)
						{
							SqlCommand cmd = db.GetCommand(sql);
							db.ExecuteNonQuery(cmd);
						}
						currentRow++;
					}
					catch (Exception e)
					{

					}

				//}
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
		//		DELETE ControlPlan WHERE SNo = @SNo";
		//	SqlCommand cmd = db.GetCommand(sql);
		//	cmd.Parameters.AddWithValue("@SNo", SNo);
		//	return db.ExecuteNonQuery(cmd);
		//}
		//public int Add(ControlPlan collection)
		//{
		//	string sql = @"
		//		INSERT INTO  ControlPlan(
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
		//public int Update(ControlPlan collection)
		//{
		//	string sql = @"
		//		UPDATE ControlPlan
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
