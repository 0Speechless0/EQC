using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using EQC.Common;
using EQC.Models;
namespace EQC.Services
{
    public class WeakTenderPlanEditService
    {
        private DBConn db = new DBConn();
        public int setMajorEng(string tenderNo, string execUnit)
        {
            string getExecUnitCdSql = @"Select distinct PrjXML.ExecUnitCd ,[Name], Unit.Seq
                from Unit inner join PrjXML 
                on Name = PrjXML.ExecUnitName where Unit.Seq=" + execUnit;

            var execUnitCd = db.ExecuteScalar(getExecUnitCdSql);

            string sql = @"
                UPDATE MajorEng
                SET 
                    ExecUnitCd = @execUnit,
                    ModifyTime = GetDate(),
                    ModifyUserSeq = @modifyUser
                WHERE TenderNo = @tenderNo
                IF @@ROWCOUNT = 0
                BEGIN
                  INSERT INTO MajorEng(
                    TenderNo,
                    ExecUnitCd,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                )  
                  Values (@tenderNo, @execUnit, GetDate(), @createdUser, GetDate(), @modifyUser);
                END;
            ";
            SqlCommand cmd = db.GetCommand(sql);
            SessionManager sm = new SessionManager();
            cmd.Parameters.AddWithValue("@tenderNo", tenderNo);
            cmd.Parameters.AddWithValue("@execUnit", execUnitCd);
            cmd.Parameters.AddWithValue("@modifyUser", sm.GetUser().Seq);
            cmd.Parameters.AddWithValue("@createdUser", sm.GetUser().Seq);
            return db.ExecuteNonQuery(cmd);
        }

        public List<object> getUnitListByTenderYear(string tenderYear)
        {
            string sql = $@"Select Max([Name]) [Name], Min(Unit.Seq) Seq from Unit 
                inner join PrjXML 
                on Name = PrjXML.ExecUnitName and PrjXML.TenderYear={tenderYear}
                group by PrjXML.ExecUnitName 
				order by Seq";
            return db.GetDataTableWithClass<Unit>(db.GetCommand(sql)).Select( row => new {Name= row.Name, Seq = row.Seq }).ToList<object>();

        }

        public List<string> getTenderYear()
        {

            string sql = @"Select distinct TenderYear from PrjXML order by TenderYear desc";
            return db.GetDataTable(db.GetCommand(sql)).Rows.Cast<DataRow>().Select(row => row.ItemArray.First().ToString()).ToList();
        }
    

        public List<string> getEngList(int unitSeq, int year)
        {
            string sql = $@"Select distinct TenderNo from PrjXML where ExecUnitName = ( Select distinct Name from Unit where Seq= { unitSeq}) and PrjXML.TenderYear = {year}";

            return db.GetDataTable(db.GetCommand(sql)).Rows.Cast<DataRow>().Select(row => row.ItemArray.First().ToString() ).ToList();
        }
        public Object GetCount()
        {
            string sql = @"
				SELECT COUNT(*)
				FROM MajorEng";
            SqlCommand cmd = db.GetCommand(sql);
            return db.ExecuteScalar(cmd);
        }
        internal List<object> getMajorEng(int page, int per_page)
        {
            string sql = @"
                Select 
                    p.Seq PrjXMLSeq,
                    p.TenderName, 
                    p.TenderNo,
                    p.ExecUnitName UnitName
                from PrjXML p 
                inner join MajorEng mp on (p.TenderNo = mp.TenderNo and p.ExecUnitCd = mp.ExecUnitCd)
                Order By mp.ModifyTime DESC 
                OFFSET @Page ROWS
                FETCH FIRST @Per_page ROWS ONLY";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Page", page*per_page);
            cmd.Parameters.AddWithValue("@Per_page", per_page);

            return db.GetDataTable(cmd).Rows.Cast<DataRow>().Select(row => new { 
                Seq = row.Field<int>("PrjXMLSeq"),
                TenderNo = row.Field<string>("TenderNo"),
                TenderName = row.Field<string>("TenderName"),
                UnitName = row.Field<string>("UnitName")
            
            }).ToList<object>();


        }

        public void deleteByTenderNo(int seq)
        {
            try
            {
                string sql = @"Delete MajorEng where PrjXMLSeq=" + seq;
                db.ExecuteNonQuery(sql);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}