using EQC.Common;
using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EQC.Services
{
    public class ThsrService : BaseService
    {
        private string[,] headerMap = new string[,]
        {
            { "DirectionText", "方向"},
            { "CarNo", "車次"},
            { "StartStationName", "起站"},
            { "DepartureTime", "預計出發時間"},
            { "EndingStationName", "終點站"},
            { "ArrivalTime", "預計到達時間"},
            { "Memo", "備註"}
        };


        public List<THSR> GetList(int page, int per_page, THSR coditions)
        {
            string sql = $@"
                Select
                    Direction,
                    CarNo,
                    StartStationName,
                    DepartureTime,
                    EndingStationName,
                    ArrivalTime,
                    Memo
                from THSR
                where 
                (StartStationName Like '%'+ @StartStationName + '%' or @StartStationName  = '' )and
                (EndingStationName Like '%'+ @EndingStationName +'%' or @EndingStationName  = '')  and
                Memo >= '{DateTime.Now.ToString("yyyy-MM-dd")}' and
                Direction Like @Direction
				ORDER BY DepartureTime
				OFFSET @Page ROWS
				FETCH FIRST @Per_page ROWS ONLY";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Sort_by", "Seq");
            cmd.Parameters.AddWithValue("@StartStationName", coditions.StartStationName  );
            cmd.Parameters.AddWithValue("@EndingStationName", coditions.EndingStationName );
            cmd.Parameters.AddWithValue("@Direction", "%" + coditions.Direction ?? "" + "%");
            cmd.Parameters.AddWithValue("@Page", page * per_page);
            cmd.Parameters.AddWithValue("@Per_page", per_page);
            return db.GetDataTableWithClass<THSR>(cmd);
        }


        public Object GetCount(THSR coditions)
        {
            string sql = $@"
				SELECT COUNT(*)
				FROM THSR
                where 
                (StartStationName Like '%'+ @StartStationName + '%' or @StartStationName  = '' ) and
                (EndingStationName Like '%'+ @EndingStationName  + '%' or @EndingStationName   = '' ) and
                Memo >= '{DateTime.Now.ToString("yyyy-MM-dd")}' and
                Direction Like @Direction
            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@StartStationName", coditions.StartStationName);
            cmd.Parameters.AddWithValue("@EndingStationName", coditions.EndingStationName);
            cmd.Parameters.AddWithValue("@Direction", "%" + coditions.Direction ?? "" + "%");
            return db.ExecuteScalar(cmd);
        }

        internal List<string> getStartStationList(int? Direction)
        {
            string sql = @"select distinct StartStationName from THSR  where Direction Like @Direction";
            var cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Direction", "%" + Direction ?? "" + "%");
            return db.GetDataTable(cmd).Rows.Cast<DataRow>().Select(row => row.Field<string>("StartStationName")).ToList();
            

        }
        internal List<string> getEndingStationList(int? Direction , string StartOption = "")
        {
            string sql = @"select distinct EndingStationName from THSR  where 
                       ( StartStationName = @StartStationName or @StartStationName = '') and
                        Direction Like @Direction";
            var cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@StartStationName", StartOption );
            cmd.Parameters.AddWithValue("@Direction", "%" + Direction ?? "" + "%");
            return db.GetDataTable(cmd).Rows.Cast<DataRow>().Select(row => row.Field<string>("EndingStationName")).ToList();


        }

        public List<object> getFields()
        {
            return getFields(headerMap);
        }
    }
}