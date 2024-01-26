using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using EQC.Common;
using EQC.Models;
using EQC.ViewModel;


namespace EQC.Services
{
    public class CityService
    {
        private DBConn db = new DBConn();

        public List<T> GetCityForOption<T>() //shioulo
        {
            string sql = @"
				SELECT Seq
					,CityName
				FROM City
				WHERE IsEnabled = 1
				ORDER BY OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<T>(cmd);

        }

        public List<City> GetList(int page, int per_page, string sort_by)
        {
			string sql = @"
				SELECT Seq
					,CityName
					,IsEnabled
					,OrderNo
					,CreateTime
					,CreateUser
					,ModifyTime
					,ModifyUser
				FROM City
				ORDER BY CASE @Sort_by
					WHEN 'Seq'
					THEN Seq
					END
				OFFSET @Page ROWS
				FETCH FIRST @Per_page ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@Sort_by", sort_by);
			cmd.Parameters.AddWithValue("@Page", page * per_page);
			cmd.Parameters.AddWithValue("@Per_page", per_page);
            return db.GetDataTableWithClass<City>(cmd);
        }

		public int AddCity(FormCollection collection)
        {
			string sql = @"
				INSERT INTO city (
					CityName
					,IsEnabled
					,OrderNo
					,CreateTime
					,CreateUser
					,ModifyTime
					,ModifyUser
					)
				VALUES (
					@CityName
					,@IsEnabled
					,@OrderNo
					,GETDATE()
					,@CreateUser
					,GETDATE()
					,@ModifyUser
					)";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@CityName", collection["cityName"]);
			cmd.Parameters.AddWithValue("@IsEnabled", collection["isEnabled"]);
			cmd.Parameters.AddWithValue("@OrderNo", collection["orderNo"]);
			cmd.Parameters.AddWithValue("@CreateUser", new SessionManager().GetUser().Seq);
			cmd.Parameters.AddWithValue("@ModifyUser", new SessionManager().GetUser().Seq);
			return db.ExecuteNonQuery(cmd);
		}

		public Object GetCount()
        {
			string sql = @"
				SELECT COUNT(*)
				FROM City";
			SqlCommand cmd = db.GetCommand(sql);
			return db.ExecuteScalar(cmd);
        }

		public int Update(VCity item)
        {
			string sql = @"
				UPDATE City
				SET CityName = @CityName
					,IsEnabled = @IsEnabled
					,OrderNo = @OrderNo
					,ModifyTime = GETDATE()
					,ModifyUser = @ModifyUser
				WHERE Seq = @Seq";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@Seq", item.Seq);
			cmd.Parameters.AddWithValue("@CityName", item.CityName);
			cmd.Parameters.AddWithValue("@IsEnabled", item.IsEnabled);
			cmd.Parameters.AddWithValue("@OrderNo", item.OrderNo);
			cmd.Parameters.AddWithValue("@ModifyUser", new SessionManager().GetUser().Seq);

			return db.ExecuteNonQuery(cmd);

		}

		public int Delete(VCity item)
        {
			string sql = @"
				DELETE City WHERE Seq = @Seq";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@Seq", item.Seq);
			return db.ExecuteNonQuery(cmd);
        }

		public int GetMaxOrderNo(int Seq)
        {
			string sql = @"
				SELECT MAX(OrderNo) AS MaxOrderNo
				FROM City
				WHERE Seq = @Seq";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@Seq", Seq);
			DataTable dt = db.GetDataTable(cmd);
			if (dt.Rows.Count == 0)
				return 0;
			if (dt.Rows[0]["MaxOrderNo"] == DBNull.Value)
            {
				return 0;
            } else
            {
				return int.Parse(dt.Rows[0]["MaxOrderNo"].ToString());
            }
        }

		public List<City> GetEnabledCities()
        {
			string sql = @"
				SELECT Seq
					,CityName
					,IsEnabled
					,OrderNo
					,CreateTime
					,CreateUser
					,ModifyTime
					,ModifyUser
				FROM City
				WHERE IsEnabled = 1
				ORDER BY OrderNo";
			SqlCommand cmd = db.GetCommand(sql);
			return db.GetDataTableWithClass<City>(cmd);

		}
    }
}