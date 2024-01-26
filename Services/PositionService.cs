using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Common;
using EQC.Models;
using EQC.ViewModel;

namespace EQC.Services
{
    public class PositionService
    {
        private DBConn db = new DBConn();

        public List<VPosition> GetList(int page, int per_page, string sort_by)
        {
			string sql = @"
				SELECT Seq
					,Name
					,OrderNo
					,CreateTime
					,CreateUser
					,ModifyTime
					,ModifyUser
				FROM Position
				ORDER BY CASE @Sort_by
						WHEN 'Seq'
							THEN Position.Seq
						END OFFSET @Page ROWS

				FETCH FIRST @Per_page ROWS ONLY
				";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@Sort_by", sort_by);
			cmd.Parameters.AddWithValue("@Page", page * per_page);
			cmd.Parameters.AddWithValue("@Per_page", per_page);
			return db.GetDataTableWithClass<VPosition>(cmd);
		}

		public int AddPosition(FormCollection collection)
        {
			string sql = @"
				INSERT INTO Position (
					[Name]
					,OrderNo
					,IsEnabled
					,CreateTime
					,CreateUser
					,ModifyTime
					,ModifyUser
					)
				VALUES (
					@Name
					,@OrderNo
					,@IsEnabled
					,GETDATE()
					,@CreateUser
					,GETDATE()
					,@ModifyUser
					)";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@Name", collection["_name"]);
			cmd.Parameters.AddWithValue("@OrderNo", collection["_orderNo"]);
			cmd.Parameters.AddWithValue("@IsEnabled", collection["_isEnabled"]);
			cmd.Parameters.AddWithValue("@CreateUser", new SessionManager().GetUser().Seq);
			cmd.Parameters.AddWithValue("@ModifyUser", new SessionManager().GetUser().Seq);
			return db.ExecuteNonQuery(cmd);
		}

		public Object GetCount()
        {
			string sql = @"
				SELECT COUNT(*)
				FROM Position";
			SqlCommand cmd = db.GetCommand(sql);
			return db.ExecuteScalar(cmd);
		}

		public int Update(VPosition item)
        {
			string sql = @"
				UPDATE Position
				SET Name = @Name
					,OrderNo = @OrderNo
					,IsEnabled = @IsEnabled
					,ModifyTime = GETDATE()
					,ModifyUser = @ModifyUser
				WHERE Seq = @Seq";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@Seq", "Seq");
			cmd.Parameters.AddWithValue("@Name", "Name");
			cmd.Parameters.AddWithValue("@OrderNo", "OrderNo");
			cmd.Parameters.AddWithValue("@IsEnabled", "IsEnabled");
			cmd.Parameters.AddWithValue("@ModifyUser", new SessionManager().GetUser().Seq);
			return db.ExecuteNonQuery(cmd);
		}

		public int Delete(VPosition item)
        {
			string sql = @"
				DELETE Position WHERE Seq = @Seq";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@Seq", item.Seq);
			return db.ExecuteNonQuery(cmd);
        }

		public int GetMaxOrderNo()
        {
			string sql = @"
				SELECT MAX(OrderNo) AS MaxOrderNo
				FROM Position";
			SqlCommand cmd = db.GetCommand(sql);
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

		public List<VPosition> GetEnabledPosition()
        {
			string sql = @"
				SELECT Seq
					,[Name]
					,OrderNo
					,IsEnabled
					,CreateTime
					,CreateUser
					,ModifyTime
					,ModifyUser
				FROM Position
				WHERE IsEnabled = 1
				ORDER BY OrderNo";
			SqlCommand cmd = db.GetCommand(sql);
			return db.GetDataTableWithClass<VPosition>(cmd);

		}


    }
}