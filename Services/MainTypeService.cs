using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using EQC.Common;
using EQC.Models;
using EQC.ViewModel;


namespace EQC.Services
{
    public class MainTypeService
    {
        private DBConn db = new DBConn();

        public List<VMainType> GetList(int page, int per_page, string sort_by, FormCollection collection)
        {
            string sql = @"
              SELECT MainType.Seq
	                ,MainType.CitySeq
	                ,City.CityName
	                ,MainType.MainTypeName
	                ,MainType.IsEnabled
	                ,MainType.OrderNo
	                ,MainType.CreateTime
	                ,MainType.CreateUser
	                ,MainType.ModifyTime
	                ,MainType.ModifyUser
                FROM MainType
                LEFT JOIN City ON City.Seq = MainType.CitySeq
                WHERE (
                    City.Seq = @CitySeq
                    OR @CitySeq = 0
                    )
                    AND (
                        MainTypeName LIKE '%' + @MainTypeName + '%'
                        OR @MainTypeName = ''
                        )
				ORDER BY CASE @Sort_by
					WHEN 'Seq'
					THEN MainType.Seq
					END
				OFFSET @Page ROWS
				FETCH FIRST @Per_page ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Sort_by", sort_by);
            cmd.Parameters.AddWithValue("@Page", page * per_page);
            cmd.Parameters.AddWithValue("@Per_page", per_page);
            cmd.Parameters.AddWithValue("@CitySeq", collection["citySeq"]);
            cmd.Parameters.AddWithValue("@MainTypeName", collection["mainTypeName"]);
            return db.GetDataTableWithClass<VMainType>(cmd);
        }

        public int AddMainType(FormCollection collection)
        {
            string sql = @"
				INSERT INTO MainType (
					CitySeq
					,MainTypeName
					,IsEnabled
					,OrderNo
					,CreateTime
					,CreateUser
					,ModifyTime
					,ModifyUser
					)
				VALUES (
					@CitySeq
					,@MainTypeName
					,@IsEnabled
					,@OrderNo
					,GETDATE()
					,@CreateUser
					,GETDATE()
					,@ModifyUser
					)";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@CitySeq", collection["_citySeq"]);
            cmd.Parameters.AddWithValue("@MainTypeName", collection["_mainTypeName"]);
            cmd.Parameters.AddWithValue("@IsEnabled", collection["_isEnabled"]);
            cmd.Parameters.AddWithValue("@OrderNo", collection["_orderNo"]);
            cmd.Parameters.AddWithValue("@CreateUser", new SessionManager().GetUser().Seq);
            cmd.Parameters.AddWithValue("@ModifyUser", new SessionManager().GetUser().Seq);
            return db.ExecuteNonQuery(cmd);
        }

        public Object GetCount(FormCollection collection)
        {
            string sql = @"
				SELECT count(*)
                FROM MainType
                JOIN City ON City.Seq = MainType.CitySeq
                WHERE (
		                City.Seq = @CitySeq
		                OR @CitySeq = 0
		                )
	                AND MainTypeName LIKE '%' + @MainTypeName + '%'
	                OR @MainTypeName = ''";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@CitySeq", collection["citySeq"]);
            cmd.Parameters.AddWithValue("@MainTypeName", collection["mainTypeName"]);
            return db.ExecuteScalar(cmd);
        }

        public int Update(VMainType item)
        {
            string sql = @"
				UPDATE MainType
				SET CitySeq = @CitySeq
					,MainTypeName = @MainTypeName
					,IsEnabled = @IsEnabled
					,OrderNo = @OrderNo
					,ModifyTime = GETDATE()
					,ModifyUser = @ModifyUser
				WHERE Seq = @Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", item.Seq);
            cmd.Parameters.AddWithValue("@CitySeq", item.CitySeq);
            cmd.Parameters.AddWithValue("@MainTypeName", item.MainTypeName);
            cmd.Parameters.AddWithValue("@IsEnabled", item.IsEnabled);
            cmd.Parameters.AddWithValue("@OrderNo", item.OrderNo);
            cmd.Parameters.AddWithValue("@ModifyUser", new SessionManager().GetUser().Seq);
            return db.ExecuteNonQuery(cmd);
        }

        public int Delete(VMainType item)
        {
            string sql = @"
				DELETE MainType WHERE Seq = @Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", item.Seq);
            return db.ExecuteNonQuery(cmd);
        }
        public int GetMaxOrderNo(string citySeq)
        {
            string sql = @"
				SELECT Max(OrderNo) AS MaxOrderNo
				FROM MainType
				WHERE CitySeq = @CitySeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@CitySeq", int.Parse(citySeq));
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

        public List<MainType> GetEnabledMainType(int CitySeq)
        {
            string sql = @"
                SELECT Seq
	                ,CitySeq
	                ,MainTypeName
	                ,IsEnabled
	                ,OrderNo
	                ,CreateTime
	                ,CreateUser
	                ,ModifyTime
	                ,ModifyUser
                FROM MainType
                WHERE IsEnabled = 1
                    AND (CitySeq = @CitySeq OR @CitySeq = 0)
                ORDER BY OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@CitySeq", CitySeq);
            return db.GetDataTableWithClass<MainType>(cmd);
        }
    }
}