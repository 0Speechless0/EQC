using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Web;
using System.Web.Mvc;
using EQC.Common;
using EQC.Models;
using EQC.ViewModel;


namespace EQC.Services
{
    public class TownService
    {
        private DBConn db = new DBConn();

        public List<T> GetTownForOption<T>(int citySeq) //shioulo
        {
            string sql = @"
				SELECT Seq
					, TownName
				FROM Town
				WHERE (CitySeq = @CitySeq OR @CitySeq = 0)
					AND IsEnabled = 1
				ORDER BY OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@CitySeq", citySeq);
            return db.GetDataTableWithClass<T>(cmd);


        }

        public List<VTown> GetList(int page, int per_page, string sort_by, int citySeq)
        {
            string sql = @"
				SELECT Seq
					,TownName
					,CitySeq
					,IsEnabled
					,OrderNo
					,CreateTime
					,CreateUser
					,ModifyTime
					,ModifyUser
				FROM town
				WHERE CitySeq = @CitySeq
				ORDER BY CASE @Sort_by
						WHEN 'Seq'
							THEN Town.Seq
						END OFFSET @Page ROWS

				FETCH FIRST @Per_page ROWS ONLY";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@Sort_by", sort_by);
			cmd.Parameters.AddWithValue("@Page", page * per_page);
			cmd.Parameters.AddWithValue("@Per_page", per_page);
			cmd.Parameters.AddWithValue("CitySeq", citySeq);
			return db.GetDataTableWithClass<VTown>(cmd);
        }

		public int AddTown(FormCollection collection)
        {
			string sql = @"
				INSERT INTO Town (
					TownName
					,CitySeq
					,IsEnabled
					,OrderNo
					,CreateTime
					,CreateUser
					,ModifyTime
					,ModifyUser
					)
				VALUES (
					@TownName
					,@CitySeq
					,@IsEnabled
					,@OrderNo
					,GETDATE()
					,@CreateUser
					,GETDATE()
					,@ModifyUser
					)";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@TownName", collection["_townName"]);
			cmd.Parameters.AddWithValue("@CitySeq", collection["_citySeq"]);
			cmd.Parameters.AddWithValue("@IsEnabled", collection["_isEnabled"]);
			cmd.Parameters.AddWithValue("@OrderNo", collection["_orderNo"]);
			cmd.Parameters.AddWithValue("@CreateUser", new SessionManager().GetUser().Seq);
			cmd.Parameters.AddWithValue("@ModifyUser", new SessionManager().GetUser().Seq);
			return db.ExecuteNonQuery(cmd);
        }

		public Object GetCount(int citySeq)
        {
			string sql = @"				
				SELECT COUNT(*)
				FROM Town
				WHERE CitySeq = @CitySeq";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@CitySeq", citySeq);
			return db.ExecuteScalar(cmd);
		}

		public int Update(VTown item)
        {
			string sql = @"
				UPDATE Town
				SET TownName = @TownName
					,CitySeq = @CitySeq
					,IsEnabled = @IsEnabled
					,OrderNo = @OrderNo
					,ModifyTime = GETDATE()
					,ModifyUser = @ModifyUser
				WHERE Seq = @Seq";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@Seq", item.Seq);
			cmd.Parameters.AddWithValue("@TownName", item.TownName);
			cmd.Parameters.AddWithValue("@CitySeq", item.CitySeq);
			cmd.Parameters.AddWithValue("@IsEnabled", item.IsEnabled);
			cmd.Parameters.AddWithValue("@OrderNo", item.OrderNo);
			cmd.Parameters.AddWithValue("@ModifyUser", new SessionManager().GetUser().Seq);
			return db.ExecuteNonQuery(cmd);
        }

		public int Delete(VTown item)
        {
			string sql = @"
				DELETE Town WHERE Seq = @Seq";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@Seq", item.Seq);
			return db.ExecuteNonQuery(cmd);
        }

		public int GetMaxOrderNo(int citySeq)
        {
			string sql = @"
				SELECT MAX(OrderNo) AS MaxOrderNo
				FROM Town
				WHERE CitySeq = @CitySeq";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@CitySeq", citySeq);
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

		public List<VTown> GetEnabledTown(int citySeq)
        {
			string sql = @"
				SELECT Seq
					, TownName
					, CitySeq
					, IsEnabled
					, OrderNo
					, CreateTime
					, CreateUser
					, ModifyTime
					, ModifyUser
				FROM Town
				WHERE (CitySeq = @CitySeq OR @CitySeq = 0)
					AND IsEnabled = 1
				ORDER BY OrderNo";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@CitySeq", citySeq);
			return db.GetDataTableWithClass<VTown>(cmd);


		}
	}
}