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
    public class MBListService
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

		public List<EngConstructionList> getEngConstruction(int engMainSeq)
		{
			string sql = @"
				SELECT 
					b.ItemName as 'construction',
					d.item,
					e.CCRCheckDate as 'checkDate',
					checkFlow =  
					  CASE e.CCRCheckFlow  
						 WHEN 1 THEN '施工前'  
						 WHEN 2 THEN '施工中'  
						 WHEN 3 THEN '施工後'  
						 ELSE null  
					  END
				FROM
					EngMain a 
				LEFT JOIN EngConstruction b ON a.Seq = b.EngMainSeq
				LEFT JOIN ConstCheckList c ON a.Seq = c.EngMainSeq
				LEFT JOIN (SELECT
					a.ConstCheckListSeq,
					MIN (a.Seq) as 'itemSeq',
					a.[CCManageItem1] AS 'item' 
				FROM
					ConstCheckControlSt a 
				GROUP BY
					a.ConstCheckListSeq,
					a.[CCManageItem1]) d ON c.Seq = d.ConstCheckListSeq
					LEFT JOIN ConstCheckRec e ON b.Seq = e.EngConstructionSeq AND d.itemSeq = e.ControllStSeq
				WHERE
					a.Seq = @engMainSeq AND 
					d.item IS NOT NULL
				ORDER BY
					b.Seq,d.itemSeq";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@engMainSeq", engMainSeq);

			return db.GetDataTableWithClass<EngConstructionList>(cmd);
		}

	}
}