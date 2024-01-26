using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Office2010.Excel;
using EQC.Common;
using EQC.ViewModel;
using EQC.ViewModel.Common;

namespace EQC.Services
{
    public class UnitService
    {
        public const int type_BuildContractor = 4;//"施工廠商
        public const int type_SupervisorUnit = 5;//"監造單位
        public const int type_DesignUnit = 6;//"設計單位
        public const int type_OutCommitteeUnit = 7;//"外聘委員單位
        //取得鄉鎮清單 shioulo
        public List<T> GetListForOption<T>(int parentUnit)
        {
            string sql = @"
                SELECT
                    Seq,
                    [Name]
                from Unit
                where (@parentSeq=-1 and parentSeq is null)
                or (parentSeq = @parentSeq)
                order by OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@parentSeq", parentUnit);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //取得鄉鎮 Seq shioulo
        public int? GetUnitSeq(string pccessCode)
        {
            string sql = @"SELECT Seq from Unit where PCCESSCode=@PCCESSCode";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@PCCESSCode", pccessCode);
            DataTable dt = db.GetDataTable(cmd);
            if (dt.Rows.Count == 1)
            {
                return Convert.ToInt32(dt.Rows[0]["Seq"].ToString());
            }
            else
            {
                return null;
            }
        }
        //取得上層單位 Seq shioulo
        public int GetParentUnitSeq(int seq)
        {
            string sql = @"SELECT ParentSeq from Unit where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            DataTable dt = db.GetDataTable(cmd);
            if (dt.Rows.Count == 1)
            {
                //System.Diagnostics.Debug.WriteLine(dt.Rows[0]["ParentSeq"].ToString());
                if (dt.Rows[0]["ParentSeq"].ToString() == "")
                    return -1;
                else
                    return Convert.ToInt32(dt.Rows[0]["ParentSeq"].ToString());
            }
            else
            {
                return -1;
            }
        }

        private DBConn db = new DBConn();
        public List<VUnit> GetList(int page, int per_page, string sort_by)
        {
            string sql = @"
				SELECT Seq
					,ParentSeq
					,Code
					,Name
					,OrderNo
					,IsEnabled
					,IsSubUnit
					,IsRegTable
					,CreateTime
					,CreateUser
					,ModifyTime
					,ModifyUser
				FROM Unit
				ORDER BY CASE @Sort_by
						WHEN 'Seq'
							THEN Unit.Seq
						END OFFSET @Page ROWS

				FETCH FIRST @Per_page ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Sort_by", sort_by);
            cmd.Parameters.AddWithValue("@Page", page * per_page);
            cmd.Parameters.AddWithValue("@Per_page", per_page);
            return db.GetDataTableWithClass<VUnit>(cmd);
        }

        public int AddUnit(FormCollection collection)
        {
            string sql = @"
				INSERT INTO Unit (
					ParentSeq
					,Code
					,[Name]
					,OrderNo
					,IsEnabled
					,IsSubUnit
					,IsRegTable
					,CreateTime
					,CreateUser
					,ModifyTime
					,ModifyUser
					)
				VALUES (
					@ParentSeq
					,@Code
					,@Name
					,@OrderNo
					,@IsEnabled
					,@IsSubUnit
					,@IsRegTable
					,GETDATE()
					,@CreateUser
					,GETDATE()
					,@ModifyUser
					)";
            SqlCommand cmd = db.GetCommand(sql);
            if (Convert.ToInt32(collection["_parentSeq"]) == 0)
            {
                cmd.Parameters.AddWithValue("@ParentSeq", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@ParentSeq", collection["_parentSeq"]);
            }
            cmd.Parameters.AddWithValue("@Code", collection["_code"]);
            cmd.Parameters.AddWithValue("@Name", collection["_name"]);
            cmd.Parameters.AddWithValue("@OrderNo", collection["_orderNo"]);
            cmd.Parameters.AddWithValue("@IsEnabled", collection["_isEnabled"]);
            cmd.Parameters.AddWithValue("@IsSubUnit", collection["_isSubUnit"]);
            cmd.Parameters.AddWithValue("@IsRegTable", collection["_isRegTable"]);
            cmd.Parameters.AddWithValue("@CreateUser", new SessionManager().GetUser().Seq);
            cmd.Parameters.AddWithValue("@ModifyUser", new SessionManager().GetUser().Seq);
            return db.ExecuteNonQuery(cmd);
        }

        public Object GetCount()
        {
            string sql = @"
				SELECT COUNT(*)
				FROM Unit
				";
            SqlCommand cmd = db.GetCommand(sql);
            return db.ExecuteScalar(cmd);
        }

        public int Update(VUnit item)
        {
            string sql = @"
				UPDATE Unit
				SET ParentSeq = @ParentSeq
					,Code = @Code
					,Name = @Name
					,OrderNo = @OrderNo
					,IsEnabled = @IsEnabled
					,IsSubUnit = @IsSubUnit
					,IsRegTable = @IsRegTable
					,ModifyTime = GETDATE()
					,ModifyUser = @ModifyUser
				WHERE Seq = @Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("Seq", item.Seq);
            if (Convert.ToInt32(item.ParentSeq) == 0)
            {
                cmd.Parameters.AddWithValue("@ParentSeq", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@ParentSeq", item.ParentSeq);
            }
            cmd.Parameters.AddWithValue("@Code", item.Code);
            cmd.Parameters.AddWithValue("@Name", item.Name);
            cmd.Parameters.AddWithValue("@OrderNo", item.OrderNo);
            cmd.Parameters.AddWithValue("@IsEnabled", item.IsEnabled);
            cmd.Parameters.AddWithValue("@IsSubUnit", item.IsSubUnit);
            cmd.Parameters.AddWithValue("@IsRegTable", item.IsRegTable);
            cmd.Parameters.AddWithValue("@ModifyUser", new SessionManager().GetUser().Seq);
            return db.ExecuteNonQuery(cmd);
        }

        public int Delete(VUnit item)
        {
            string sql = @"
				DELETE Unit WHERE Seq = @Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", item.Seq);
            return db.ExecuteNonQuery(cmd);
        }

        public int GetMaxOrderNo()
        {
            string sql = @"
				SELECT MAX(OrderNo) AS MaxOrderNo
				FROM [Unit]";
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

        /// <summary> 取得單位(下拉選單資料) </summary>
        /// <returns></returns>
        /// 
        internal List<string> GetUnitList(string[] subUnit, int unitLevelCount)
        {
            if (unitLevelCount < 1) return null;
            string unitName = $"e0.ParentSeq, e0.Name Name0,";
            string joinStr = "";
            int i = 0;
            for (i = 0; i < unitLevelCount - 1; i++)
            {
                unitName += $"e{i + 1}.Name Name{i + 1},";
                joinStr += $" left join Unit e{i + 1} on e{i}.Seq = e{i + 1}.ParentSeq ";
            }

            unitName = unitName.Remove(unitName.Length - 1, 1);

            string sql = @"
                select
				" + unitName + @"
                from Unit e0" + @"
                " + joinStr;

            SqlCommand cmd = db.GetCommand(sql);


            var l = db.GetDataTable(cmd).Rows.Cast<DataRow>()
                .Where(row => row.Field<Int16?>("ParentSeq") == null)
                .Select(row => row.ItemArray )
                .ToList();
            i = 0;
            return subUnit.Aggregate(l, (result, unit) =>
            {

                if (unit == "") return result;
                i++;
                return  result.Where(row => row[i]?.ToString() == unit).ToList();
            }, (result) => {
                return ( i == unitLevelCount) ? new List<string>() : result.Select(row => row[i+1]?.ToString());
            })?.Distinct().ToList();
        }
        internal List<SelectVM> GetUnitList(int? parentSeq)
        {
            string sql = @"
				Select Cast(Seq as varchar(10)) as Value,
				Name as Text
				from Unit
                {0}";
            if (parentSeq == null)
            {
                sql = string.Format(sql, " Where ParentSeq is null ");
            }
            else
            {
                sql = string.Format(sql, " Where ParentSeq=@ParentSeq ");
            }
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@ParentSeq", parentSeq ?? (object)DBNull.Value);
            return db.GetDataTableWithClass<SelectVM>(cmd);
        }

        public List<VUnit> GetEnabledUnit()
        {
            string sql = @"
				SELECT Seq
					,ParentSeq
					,Code
					,[Name]
					,OrderNo
					,IsEnabled
					,IsSubUnit
					,IsRegTable
					,CreateTime
					,CreateUser
					,ModifyTime
					,ModifyUser
				FROM Unit
				WHERE IsEnabled = 1
				ORDER BY OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<VUnit>(cmd);
        }

        /// <summary> 取得單位(下拉選單資料)(施工風險) </summary>
        /// <returns></returns>
        internal List<SelectVM> GetUnitListForRisk(int id)
        {
            string sql = @"
				SELECT Cast([Seq] as varchar(10)) as Value
                      ,[Name] as Text
                FROM [dbo].[Unit] 
                WHERE ISNULL([ParentSeq],0)=0 
	                --AND Seq IN (23,24,25,26,27,2829,30,31,32,33,34,35,36,178)
                    AND Seq IN (SELECT ExecUnitSeq FROM EngMain WHERE Seq = @id)
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@id", id);
            return db.GetDataTableWithClass<SelectVM>(cmd);
        }
    }
}