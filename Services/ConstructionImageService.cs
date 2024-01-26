using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EQC.Services
{
    public class ConstructionImageService : BaseService
    {

        internal List<object>  getEngData(string startYear, string endYear)
        {
            string sql = @"select 
	            EngMain.Seq as Seq, 
	            EngMain.EngName as TenderName,
	            EngMain.EngNo as TenderNo,
	            EngMain.EngYear as TenderYear,
                ConstCheckRecFile.Memo as Memo,
	            ConstCheckRecFile.RESTful as HasBee,
	            ConstCheckRecFile.CreateTime as CreateTime,
	            UniqueFileName
            from EngMain 
            inner join EngConstruction on EngMain.Seq = EngMainSeq  
            inner join ConstCheckRec on EngConstruction.Seq = EngConstructionSeq
            inner join ConstCheckRecFile on ConstCheckRec.Seq = ConstCheckRecSeq where EngYear >=" + startYear
            +" and EngYear <=" + endYear;

            return db.GetDataTable(db.GetCommand(sql)).Rows.Cast<DataRow>()
                .Select(row => new
                {
                    TenderName = row.Field<string>("TenderName"),
                    Seq = row.Field<int>("Seq"),
                    TenderNo = row.Field<string>("TenderNo"),
                    TenderYear = row.Field<Int16?>("TenderYear"),
                    HasBee = row.Field<string>("HasBee"),
                    Memo = row.Field<string>("Memo"),
                    CreateTime = row.Field<DateTime>("CreateTime").ToString("yyyy-MM-d"),
                    UniqueFileName = row.Field<string>("UniqueFileName")
                }).ToList<object>();
        }

        internal List<string> getEngYearList()
        {
            string sql = @"select EngYear from EngMain group by EngYear";
            return db.GetDataTable(db.GetCommand(sql)).Rows.Cast<DataRow>().Select(row => row.Field<Int16>("EngYear").ToString() ).ToList<string>();
        }

        internal void updateContent(ConstCheckRecFileModel item)
        {
            string sql = @"update ConstCheckRecFile Set Memo = @Memo, RestFul = @RESTful where UniqueFileName = @UniqueFileName";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Memo", item.Memo);
            cmd.Parameters.AddWithValue("@RESTful", item.RESTful ?? "");
            cmd.Parameters.AddWithValue("@UniqueFileName", item.UniqueFileName);
            db.ExecuteNonQuery(cmd);
        }
        internal void delete(string UniqueFileName)
        {
            string sql = @"delete from ConstCheckRecFile where UniqueFileName='" + UniqueFileName+"'";
            db.ExecuteNonQuery(sql);
        }
    }
}