using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using EQC.Common;
using EQC.ViewModel;

namespace EQC.Services
{
    public class SubTypeService
    {
        private DBConn db = new DBConn();

        //public List<VSubType> GetList(int page, int per_page, string sort_by, int citySeq, int mainTypeSeq, string subTypeName)

		public List<VSubType> GetList(int page, int per_page, string sort_by, FormCollection collection)
		{
			string sql = @"
				SELECT SubType.Seq AS SubTypeSeq
					,City.Seq AS CitySeq
					,City.CityName
					,MainType.Seq AS MainTypeSeq
					,MainTypeName
					,SubTypeName
					,DeadLine
					,SubType.IsEnabled
					,SubType.OrderNo
					,SubType.CreateTime
					,SubType.CreateUser
					,SubType.ModifyTime
					,SubType.ModifyUser
				FROM City
				JOIN MainType ON MainType.CitySeq = City.Seq
				JOIN SubType ON SubType.MainTypeSeq = MainType.Seq
				WHERE (
						City.Seq = @CitySeq
						OR @CitySeq = 0
						)
					AND (
						MainType.Seq = @MainTypeSeq
						OR @MainTypeSeq = 0
						)
					AND (
						SubTypeName LIKE '%' + @SubTypeName + '%'
						OR @SubTypeName = ''
						)
				ORDER BY CASE @Sort_by
					WHEN 'SubTypeSeq'
					THEN SubType.Seq
					END
				OFFSET @Page ROWS
				FETCH FIRST @Per_page ROWS ONLY";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@Sort_by", sort_by);
			cmd.Parameters.AddWithValue("@Page", page * per_page);
			cmd.Parameters.AddWithValue("@Per_page", per_page);
            cmd.Parameters.AddWithValue("@CitySeq", collection["citySeq"]);
            cmd.Parameters.AddWithValue("@MainTypeSeq", collection["mainTypeSeq"]);
            cmd.Parameters.AddWithValue("@SubTypeName", collection["subTypeName"]);
            return db.GetDataTableWithClass<VSubType>(cmd);

		}

		public int AddSubType(FormCollection collection)
        {
			string sql = @"
				INSERT INTO SubType (
					MainTypeSeq
					,SubTypeName
					,DeadLine
					,IsEnabled
					,OrderNo
					,CreateTime
					,CreateUser
					,ModifyTime
					,ModifyUser
					)
				VALUES (
					@MainTypeSeq
					,@SubTypeName
					,@DeadLine
					,1
					,@OrderNo
					,GETDATE()
					,@CreateUser
					,GETDATE()
					,@ModifyUser
					)";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@MainTypeSeq", collection["_maintypeSeq"]);
			cmd.Parameters.AddWithValue("@SubTypeName", collection["_subTypeName"]);
			cmd.Parameters.AddWithValue("@DeadLine", collection["_deadLine"]);
			cmd.Parameters.AddWithValue("@OrderNo", collection["_orderNo"]);
			cmd.Parameters.AddWithValue("@CreateUser", new SessionManager().GetUser().Seq);
			cmd.Parameters.AddWithValue("@ModifyUser", new SessionManager().GetUser().Seq);
			return db.ExecuteNonQuery(cmd);
		}

		public Object GetCount(FormCollection collection)
        {
			string sql = @"
				SELECT COUNT(*)
				FROM City
				JOIN MainType ON MainType.CitySeq = City.Seq
				JOIN SubType ON SubType.MainTypeSeq = MainType.Seq
				WHERE(
						City.Seq = @CitySeq
						OR @CitySeq = 0
						)
					AND(
						MainType.Seq = @MainTypeSeq
						OR @MainTypeSeq = 0
						)
					AND(
						SubTypeName LIKE '%' + @SubTypeName + '%'
						OR @SubTypeName = ''
						)";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@CitySeq", collection["citySeq"]);
			cmd.Parameters.AddWithValue("@MainTypeSeq", collection["mainTypeSeq"]);
			cmd.Parameters.AddWithValue("@SubTypeName", collection["subTypeName"]);
			return db.ExecuteScalar(cmd);
		}

		public int GetMaxOrderNo(int maintypeSeq)
        {
			string sql = @"
				SELECT Max(OrderNo) AS MaxOrderNo
				FROM SubType
				WHERE MainTypeSeq = @MainTypeSeq";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("MainTypeSeq", maintypeSeq);
			DataTable dt = db.GetDataTable(cmd);
			if (dt.Rows.Count == 0) return 0;

			if (dt.Rows[0]["MaxOrderNo"] == DBNull.Value)
            {
				return 0;
            }
			else
            {
				return int.Parse(dt.Rows[0]["MaxOrderNo"].ToString());
            }
		}

		public List<VSubType> GetEnabledSubType(int maintypeSeq)
        {
			string sql = @"
				SELECT Seq AS SubTypeSeq
					,MainTypeSeq
					,SubTypeName
					,DeadLine
					,IsEnabled
					,OrderNo
					,CreateTime
					,CreateUser
					,ModifyTime
					,ModifyUser
				FROM SubType
				WHERE IsEnabled = 1
					AND MainTypeSeq = @MainTypeSeq OR @MainTypeSeq = 0
				ORDER BY OrderNo";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@MainTypeSeq", maintypeSeq);
			return db.GetDataTableWithClass<VSubType>(cmd);
        }

		public int Delete(VSubType item)
        {
			string sql = @"
				DELETE SubType WHERE Seq = @Seq";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@Seq", item.SubTypeSeq);
			return db.ExecuteNonQuery(cmd);
        }
    }
}