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
    public class QualityDeductionPointsService : ExcelImportService
    {
		private DBConn db = new DBConn();
		private string[,] excelHeaderMap = new string[,]
		{
			{"UnitName",    "單位" },
			{"UnitCode",    "單位代號" },
			{"DeductionPoint",  "扣點" },
			{"Content", "缺失內容" }

		};
		private string[,] alternativeHeaderMap = new string[,]
		{
			{"MissingNo",    "缺失編號" }

		};
		public List<QualityDeductionPoints> GetList(int page, int per_page, string keyWord)
		{
			string sql = @"Select
				Seq,
				UnitName,
				UnitCode,
				MissingNo,
				DeductionPoint,
				[Content]
                from QualityDeductionPoints
				where MissingNo Like @keyWord
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
			cmd.Parameters.AddWithValue("@keyWord", "%"+keyWord+ "%");
			return db.GetDataTableWithClass<QualityDeductionPoints>(cmd);
		}
		public Object GetCount()
		{
			string sql = @"
				SELECT COUNT(*)
				FROM QualityDeductionPoints";
			SqlCommand cmd = db.GetCommand(sql);
			return db.ExecuteScalar(cmd);
		}


		public void importExcel(ExcelProcesser processer)
		{
			string sql = "";
			processer.createDbColumnMapToHeader(excelHeaderMap);
			processer.declareCol(alternativeHeaderMap);
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
				sql = processer.getInsertSql("QualityDeductionPoints", currentRow);
				sql = processer.addDeclareColToInsertSql(sql, currentRow);
				string updateSql = processer.getUpdateSql("QualityDeductionPoints"
					, "MissingNo", currentRow);
				updateSql = processer.addDeclareColToUpdateSql(updateSql, currentRow);
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
			List<object> list = new List<object>();
			int rowNum = alternativeHeaderMap.GetLength(0);
			for (int i = 0; i < rowNum; i++)
			{
				list.Add(new { fieldName = alternativeHeaderMap[i, 0], fieldShowName = alternativeHeaderMap[i, 1] });
			}
			rowNum = excelHeaderMap.GetLength(0);

			for (int i = 0; i < rowNum; i++)
			{
				list.Add(new { fieldName = excelHeaderMap[i, 0], fieldShowName = excelHeaderMap[i, 1] });
			}


			return list;
		}

        internal void Add(QualityDeductionPoints value)
        {
            string sql = @"Insert INTO QualityDeductionPoints(
                UnitName,
                UnitCode,
                MissingNo,
                DeductionPoint,
                [Content],
                CreateTime,
                CreateUserSeq,
                ModifyTime,
                ModifyUserSeq
            )
            VALUES
                (
                    @UnitName,
                    @UnitCode,
                    @MissingNo,
                    @DeductionPoint,
                    @Content,
                    GetDate(),
                    @CreateUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                )";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@UnitName", value.UnitName);
            cmd.Parameters.AddWithValue("@UnitCode", value.UnitCode);
            cmd.Parameters.AddWithValue("@MissingNo", value.MissingNo);
            cmd.Parameters.AddWithValue("@DeductionPoint", value.DeductionPoint);
            cmd.Parameters.AddWithValue("@Content", value.Content);
            cmd.Parameters.AddWithValue("@CreateUserSeq", new SessionManager().GetUser().Seq);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", new SessionManager().GetUser().Seq);
            db.ExecuteNonQuery(cmd);

           
        }

        internal void Update(QualityDeductionPoints value)
        {
            string sql = @"UPDATE
                QualityDeductionPoints
                SET
                UnitName =@UnitName,
                UnitCode =@UnitCode,
                MissingNo =@MissingNo,
                DeductionPoint =@DeductionPoint,
                [Content] = @Content,
                ModifyTime =GetDate(),
                ModifyUserSeq =@ModifyUserSeq Where Seq=" + value.Seq ;

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@UnitName", value.UnitName);
            cmd.Parameters.AddWithValue("@UnitCode", value.UnitCode);
            cmd.Parameters.AddWithValue("@MissingNo", value.MissingNo);
            cmd.Parameters.AddWithValue("@DeductionPoint", value.DeductionPoint);
            cmd.Parameters.AddWithValue("@Content", value.Content);
            cmd.Parameters.AddWithValue("@CreateUserSeq", new SessionManager().GetUser().Seq);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", new SessionManager().GetUser().Seq);
            db.ExecuteNonQuery(cmd);
        }

        internal void Delete(int id)
        {
            string sql = @"Delete from QualityDeductionPoints where Seq=" + id;
            db.ExecuteNonQuery(sql);
        }
        //public int Delete(int SNo)
        //{
        //	string sql = @"
        //		DELETE QualityDeductionPointst WHERE SNo = @SNo";
        //	SqlCommand cmd = db.GetCommand(sql);
        //	cmd.Parameters.AddWithValue("@SNo", SNo);
        //	return db.ExecuteNonQuery(cmd);
        //}
        //public int Add(QualityDeductionPoints collection)
        //{
        //	string sql = @"
        //		INSERT INTO  QualityDeductionPointst(
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
        //public int Update(QualityDeductionPoints collection)
        //{
        //	string sql = @"
        //		UPDATE QualityDeductionPointst
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