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
// using MvcVue1.Services;

namespace EQC.Services
{
    public class MBEngService
	{
        private DBConn db = new DBConn();

		public List<VItem> checkUser(string account, string mobile)
		{
			string sql = @"
				SELECT
					a.[Seq] as seq,
					a.[Mobile] as item
				FROM
					[UserMain] a
				WHERE
					a.[UserNo] =  @account
				AND
					a.[Mobile] =  @mobile";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@account", account);
			cmd.Parameters.AddWithValue("@mobile", mobile);
			return db.GetDataTableWithClass<VItem>(cmd);
		}

		public List<EngMain> getEngMain(string mobile)
        {
			string sql = @"
				SELECT
					c.[Seq],
					c.[EngName] 
				FROM
					UserMain a
					LEFT JOIN UserUnitPosition b ON a.Seq= b.UserMainSeq
					LEFT JOIN EngMain c ON b.UnitSeq= c.ExecUnitSeq 
				WHERE
					a.Mobile = @mobile 
				ORDER BY
					c.EngNo";
            SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@mobile", mobile);
		
            return db.GetDataTableWithClass<EngMain>(cmd);
        }

		public List<EngConstruction> getEngConstruction(int engMainSeq)
		{
			string sql = @"
				SELECT
					[Seq],
					[ItemName]
				FROM
					[EngConstruction] 
				WHERE
					[EngMainSeq] = @engMainSeq
				ORDER BY
					[OrderNo]";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@engMainSeq", engMainSeq);

			return db.GetDataTableWithClass<EngConstruction>(cmd);
		}

		public List<VItem> getConstCheckList(int engMainSeq)
		{
			string sql = @"
				SELECT
					a.[Seq] as seq,
					a.[ItemName] as item
				FROM
					ConstCheckList a
				
				WHERE
					a.EngMainSeq =  @engMainSeq";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@engMainSeq", engMainSeq);
			return db.GetDataTableWithClass<VItem>(cmd);
		}

		public List<VItem> getCCManageItem(int constCheckListSeq, int ccFlow)
		{
			string sql = @"
				SELECT MIN
					( a.[Seq] ) as 'seq',
					a.[CCManageItem1] as 'item',
					a.[CCManageItem2] as 'item2'
				FROM
					ConstCheckControlSt a 
				WHERE
					a.ConstCheckListSeq =  @constCheckListSeq
				AND
					a.CCFlow1 = @ccFlow
				GROUP BY
					a.[CCManageItem1],
					a.[CCManageItem2]";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@constCheckListSeq", constCheckListSeq);
			cmd.Parameters.AddWithValue("@ccFlow", ccFlow);
			return db.GetDataTableWithClass<VItem>(cmd);
		}

		public List<VCCCheckStand> drawCCManageItem(int constCheckListSeq, int ccFlow, string ccManageItem1, string ccManageItem2)
		{
			string sql = @"
				SELECT 
					a.[Seq]  as 'seq',
					a.[CCCheckStand1] as 'Stand1',
					a.[CCCheckStand2] as 'Stand2'
				FROM
					ConstCheckControlSt a 
				WHERE
					a.ConstCheckListSeq = @constCheckListSeq  
					AND a.CCFlow1 = @ccFlow  
					AND a.CCManageItem1 LIKE @ccManageItem1
					AND a.CCManageItem2 LIKE @ccManageItem2";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@constCheckListSeq", constCheckListSeq);
			cmd.Parameters.AddWithValue("@ccFlow", ccFlow);
			cmd.Parameters.AddWithValue("@ccManageItem1", ccManageItem1);
			cmd.Parameters.AddWithValue("@ccManageItem2", ccManageItem2);
			return db.GetDataTableWithClass<VCCCheckStand>(cmd);
		}

		public List<VItem> getEquOperTestList(int engMainSeq)
		{
			string sql = @"
				SELECT
					a.[Seq] as seq,
					a.[ItemName] as item
				FROM
					[EquOperTestList] a
				WHERE
					a.EngMainSeq =  @engMainSeq";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@engMainSeq", engMainSeq);
			return db.GetDataTableWithClass<VItem>(cmd);
		}

		public List<VItem> getEPCheckItem(int constCheckListSeq)
		{
			string sql = @"
				SELECT MIN
					( a.[Seq] ) as 'seq',
					a.[EPCheckItem1] as 'item',
					a.[EPCheckItem2] as 'item2'
				FROM
					EquOperControlSt a 
				WHERE
					a.[EquOperTestStSeq] =  @constCheckListSeq
			
				GROUP BY
					a.[EPCheckItem1],
					a.[EPCheckItem2]";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@constCheckListSeq", constCheckListSeq);
			return db.GetDataTableWithClass<VItem>(cmd);
		}

		public List<VCCCheckStand> drawEPCheckItem(int constCheckListSeq, string ccManageItem1, string ccManageItem2)
		{
			string sql = @"
				SELECT 
					a.[Seq]  as 'seq',
					a.[EPStand1] as 'Stand1',
					a.[EPStand2] as 'Stand2',
					a.[EPStand3] as 'Stand3',
					a.[EPStand4] as 'Stand4',
					a.[EPStand5] as 'Stand5'
				FROM
					EquOperControlSt a 
				WHERE
					a.[EquOperTestStSeq] = @constCheckListSeq  
					AND a.[EPCheckItem1] LIKE @ccManageItem1
					AND a.[EPCheckItem2] LIKE @ccManageItem2";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@constCheckListSeq", constCheckListSeq);
			cmd.Parameters.AddWithValue("@ccManageItem1", ccManageItem1);
			cmd.Parameters.AddWithValue("@ccManageItem2", ccManageItem2);
			return db.GetDataTableWithClass<VCCCheckStand>(cmd);
		}

		public List<VItem> getOccuSafeHealthList(int engMainSeq)
		{
			string sql = @"
				SELECT
					a.[Seq] as seq,
					a.[ItemName] as item
				FROM
					[OccuSafeHealthList] a
				WHERE
					a.EngMainSeq =  @engMainSeq";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@engMainSeq", engMainSeq);
			return db.GetDataTableWithClass<VItem>(cmd);
		}

		public List<VItem> getOSCheckItem(int constCheckListSeq)
		{
			string sql = @"
				SELECT MIN
					( a.[Seq] ) as 'seq',
					a.[OSCheckItem1] as 'item',
					a.[OSCheckItem2] as 'item2'
				FROM
					[OccuSafeHealthControlSt] a 
				WHERE
					a.[OccuSafeHealthListSeq] =  @constCheckListSeq
			
				GROUP BY
					a.[OSCheckItem1],
					a.[OSCheckItem2]";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@constCheckListSeq", constCheckListSeq);
			return db.GetDataTableWithClass<VItem>(cmd);
		}

		public List<VCCCheckStand> drawOSCheckItem(int constCheckListSeq, string ccManageItem1, string ccManageItem2)
		{
			string sql = @"
				SELECT 
					a.[Seq]  as 'seq',
					a.[OSStand1] as 'Stand1',
					a.[OSStand2] as 'Stand2',
					a.[OSStand3] as 'Stand3',
					a.[OSStand4] as 'Stand4',
					a.[OSStand5] as 'Stand5'
				FROM
					[OccuSafeHealthControlSt] a 
				WHERE
					a.[OccuSafeHealthListSeq] = @constCheckListSeq  
					AND a.[OSCheckItem1] LIKE @ccManageItem1
					AND a.[OSCheckItem2] LIKE @ccManageItem2";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@constCheckListSeq", constCheckListSeq);
			cmd.Parameters.AddWithValue("@ccManageItem1", ccManageItem1);
			cmd.Parameters.AddWithValue("@ccManageItem2", ccManageItem2);
			return db.GetDataTableWithClass<VCCCheckStand>(cmd);
		}

		public List<VItem> getEnvirConsList(int engMainSeq)
		{
			string sql = @"
				SELECT
					a.[Seq] as seq,
					a.[ItemName] as item
				FROM
					[EnvirConsList] a
				WHERE
					a.EngMainSeq =  @engMainSeq";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@engMainSeq", engMainSeq);
			return db.GetDataTableWithClass<VItem>(cmd);
		}

		public List<VItem> getECCCheckItem(int constCheckListSeq, int ccFlow)
		{
			string sql = @"
				SELECT MIN
					( a.[Seq] ) as 'seq',
					a.[ECCCheckItem1] as 'item',
					a.[ECCCheckItem2] as 'item2'
				FROM
					[EnvirConsControlSt] a 
				WHERE
					a.[EnvirConsListSeq] =  @constCheckListSeq
				AND
					a.[ECCFlow1] = @ccFlow
				GROUP BY
					a.[ECCCheckItem1],
					a.[ECCCheckItem2]";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@constCheckListSeq", constCheckListSeq);
			cmd.Parameters.AddWithValue("@ccFlow", ccFlow);
			return db.GetDataTableWithClass<VItem>(cmd);
		}

		public List<VCCCheckStand> drawECCCheckItem(int constCheckListSeq, int ccFlow, string ccManageItem1, string ccManageItem2)
		{
			string sql = @"
				SELECT 
					a.[Seq]  as 'seq',
					a.[ECCStand1] as 'Stand1',
					a.[ECCStand2] as 'Stand2',
					a.[ECCStand3] as 'Stand3',
					a.[ECCStand4] as 'Stand4',
					a.[ECCStand5] as 'Stand5'
				FROM
					[EnvirConsControlSt] a 
				WHERE
					a.[EnvirConsListSeq] = @constCheckListSeq  
					AND a.[ECCFlow1] = @ccFlow  
					AND a.[ECCCheckItem1] LIKE @ccManageItem1
					AND a.[ECCCheckItem2] LIKE @ccManageItem2";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@constCheckListSeq", constCheckListSeq);
			cmd.Parameters.AddWithValue("@ccFlow", ccFlow);
			cmd.Parameters.AddWithValue("@ccManageItem1", ccManageItem1);
			cmd.Parameters.AddWithValue("@ccManageItem2", ccManageItem2);
			return db.GetDataTableWithClass<VCCCheckStand>(cmd);
		}
	}
}