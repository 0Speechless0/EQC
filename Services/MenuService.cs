using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using EQC.Common;
using EQC.Models;
using EQC.ViewModel;
using EQC.ViewModel.Common;


namespace EQC.Services
{
    public class MenuService
    {
        private DBConn db = new DBConn();

        /// <summary> 取得 功能選單 </summary>
        /// <param name="systemTypeSeq"> 系統別序號 </param>
        /// <param name="userMainSeq"> 人員序號 </param>
        /// <returns> 功能選單 </returns>
        public List<VMenu> LoadMenu(byte systemTypeSeq, int userMainSeq)
        {
            string sql = @"
                Exec USP_GetUserMenu @SystemTypeSeq, @UserMainSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SystemTypeSeq", systemTypeSeq);
            cmd.Parameters.AddWithValue("@UserMainSeq", userMainSeq);
            return db.GetDataTableWithClass<VMenu>(cmd);
        }
        /// <summary> 取得 系統清單 </summary>
        /// <param name="userMainSeq"> 人員序號 </param>
        /// <returns> 系統清單 </returns>
        public List<VSystemMenu> LoadSystemMenu(int userMainSeq)
        {
            string sql = @"Select　
                Max(SystemType.Name) as Name, 
                Max(Menu1.PathName) as PathName,
                Max(SystemType.Seq) as SystemType,
                SystemType.OrderNo 
                from UserMain 
                inner Join UserUnitPosition on UserMain.Seq = UserMainSeq
                inner join UserRole on UserUnitPositionSeq　=　UserUnitPosition.Seq
                inner join MenuRole on MenuRole.RoleSeq = UserRole.RoleSeq
                inner join Menu  on MenuSeq = Menu.Seq
                inner join SystemType on SystemTypeSeq = SystemType.Seq
                inner join Menu as Menu1 on Menu1.SystemTypeSeq = SystemType.Seq
                where  Menu1.ParentSeq = 0 and UserMain.Seq = @userMainSeq
                group by SystemType.OrderNo, SystemType.Name
                order by SystemType.OrderNo --s20230517
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@userMainSeq", userMainSeq);
            return db.GetDataTableWithClass<VSystemMenu>(cmd);

        }

        /// <summary> 取得MENU權限列表 </summary>
        /// <param name="systemTypeSeq"></param>
        /// <returns> MenuRoleVM </returns>
        public List<MenuRoleVM> GetList(int systemTypeSeq)
        {
            //shioulo 20220516 add [menuOrder]
            string sql = @"
                Select MenuSeq, menuOrder, Menu.Name as MenuName,
                Case When Role1 is null Then CAST(0 AS BIT) Else CAST(1 AS BIT) END as Role1,
                Case When Role2 is null Then CAST(0 AS BIT) Else CAST(1 AS BIT) END as Role2,
                Case When Role3 is null Then CAST(0 AS BIT) Else CAST(1 AS BIT) END as Role3,
                Case When Role4 is null Then CAST(0 AS BIT) Else CAST(1 AS BIT) END as Role4,
                Case When Role5 is null Then CAST(0 AS BIT) Else CAST(1 AS BIT) END as Role5,
                Case When Role6 is null Then CAST(0 AS BIT) Else CAST(1 AS BIT) END as Role6, --shioulo 20220513
                Case When Role7 is null Then CAST(0 AS BIT) Else CAST(1 AS BIT) END as Role7, --shioulo 20220513
                 Case When Role20 is null Then CAST(0 AS BIT) Else CAST(1 AS BIT) END as Role20 --shioulo 20220513
                from (
                select MenuSeq, menuOrder,[1] as Role1,[2] as Role2 ,[3]as Role3,[4]as Role4,[5]as Role5,[6]as Role6,[7]as Role7,  
                [20] as Role20 from
                (
                    select Seq,Seq as MenuSeq,MenuSeq as MenuSeq2,RoleSeq,a.Name,a.OrderNo as menuOrder from Menu  a
                    left join MenuRole b on b.MenuSeq=a.Seq
                    Where a.ParentSeq>0 and a.SystemTypeSeq=@SystemTypeSeq
                    and IsEnabled=1 -- shioulo 20220516
                ) temp
                PIVOT
                (
                 Max(Name)
                 FOR [RoleSeq] in ([1],[2],[3],[4],[5],[6],[7], [20]) 
                ) AS PivotTable) temp2
                left join Menu on temp2.MenuSeq=menu.Seq
                order by menuOrder --shioulo 20220516";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SystemTypeSeq", systemTypeSeq);
            return db.GetDataTableWithClass<MenuRoleVM>(cmd);
        }

        public int AddMenu(FormCollection collection)
        {
            string sql = @"
                INSERT INTO Menu (
	                 ParentSeq
	                , [Name]
	                , [Url]
	                , OrderNo
	                , CanManage
	                , IsEnabled
	                , CreateTime
	                , CreateUer
	                , ModifyTime
	                , ModifyUser
	                )
                VALUES (
	                 @ParentSeq
	                , @Name
	                , @Url
	                , @OrderNo
	                , @CanManage
	                , @IsEnabled
	                , GETDATE()
	                , @CreateUer
	                , GETDATE()
	                , @ModifyUser)";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@ParentSeq", collection["_parentSeq"]);
            cmd.Parameters.AddWithValue("@Name", collection["_name"]);
            cmd.Parameters.AddWithValue("@Url", collection["_url"]);
            cmd.Parameters.AddWithValue("@OrderNo", collection["_orderNo"]);
            cmd.Parameters.AddWithValue("@CanManage", collection["_canManage"]);
            cmd.Parameters.AddWithValue("@IsEnabled", collection["_isEnabled"]);
            cmd.Parameters.AddWithValue("@CreateUer", new SessionManager().GetUser().Seq);
            cmd.Parameters.AddWithValue("@ModifyUser", new SessionManager().GetUser().Seq);
            return db.ExecuteNonQuery(cmd);
        }

        public Object GetCount()
        {
            string sql = @"
                SELECT count(*)
				FROM [Menu]";
            SqlCommand cmd = db.GetCommand(sql);
            return db.ExecuteScalar(cmd);
        }

        public int Update(Menu item)
        {
            string sql = @"
                UPDATE Menu
                SET ParentSeq = @ParentSeq
	                , Name = @Name
	                , Url = @Url
	                , OrderNo = @OrderNo
	                , CanManage = @CanManage
	                , IsEnabled = @IsEnabled
	                , ModifyTime = GETDATE()
	                , ModifyUser = @ModifyUser
                WHERE Seq = @Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", item.Seq);
            cmd.Parameters.AddWithValue("@ParentSeq", item.ParentSeq);
            cmd.Parameters.AddWithValue("@Name", item.Name);
            cmd.Parameters.AddWithValue("@Url", item.Url);
            cmd.Parameters.AddWithValue("@OrderNo", item.OrderNo);
            cmd.Parameters.AddWithValue("@CanManage", item.CanManage);
            cmd.Parameters.AddWithValue("@IsEnabled", item.IsEnabled);
            cmd.Parameters.AddWithValue("@ModifyUser", new SessionManager().GetUser().Seq);
            return db.ExecuteNonQuery(cmd);
        }

        /// <summary> 儲存MenuRole </summary>
        /// <param name="menuSeq"> menuSeq </param>
        /// <param name="roleSeq"> roleSeq </param>
        /// <param name="chk"> chk </param>
        /// <returns> SaveChangeStatus </returns>
        internal SaveChangeStatus Save(int menuSeq, int roleSeq, bool chk)
        {
            SaveChangeStatus result = new SaveChangeStatus(true, StatusCode.Save);
            string sql = string.Empty;
            if (chk)
            {
                sql = @"
                INSERT INTO [MenuRole]
                           ([MenuSeq]
                           ,[RoleSeq]
                           ,[CreateTime]
                           ,[CreateUserSeq]
                           ,[ModifyTime]
                           ,[ModifyUserSeq])
                     VALUES
                           (@MenuSeq
                           ,@RoleSeq
                           ,@CreateTime
                           ,@CreateUserSeq
                           ,@ModifyTime
                           ,@ModifyUserSeq)";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@MenuSeq", menuSeq);
                cmd.Parameters.AddWithValue("@RoleSeq", roleSeq);
                cmd.Parameters.AddWithValue("@CreateTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@CreateUserSeq", new SessionManager().GetUser().Seq);
                cmd.Parameters.AddWithValue("@ModifyTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", new SessionManager().GetUser().Seq);
                db.ExecuteNonQuery(cmd);
            }
            else
            {
                sql = @"
                DELETE FROM [dbo].[MenuRole]
                WHERE [MenuSeq]=@MenuSeq AND [RoleSeq]=@RoleSeq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@MenuSeq", menuSeq);
                cmd.Parameters.AddWithValue("@RoleSeq", roleSeq);
                db.ExecuteNonQuery(cmd);
            }
            return result;
        }

        /// <summary> 取得系統別下拉 </summary>
        /// <returns> SelectVM </returns>
        internal List<SelectVM> GetSystemTypeList()
        {
            string sql = @"
				Select Cast(Seq as varchar(10)) as Value,
				Name as Text
				from SystemType
				Where IsEnabled=1 ";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<SelectVM>(cmd);
        }

        public int Delete(Menu item)
        {
            string sql = @"
                DELETE [Menu] WHERE Seq = @Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", item.Seq);
            return db.ExecuteNonQuery(cmd);
        }

        public int GetMaxOrderNo()
        {
            string sql = @"
                SELECT MAX(OrderNo) AS MaxOrderNo
				FROM [Menu]";
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

        public List<Menu> GetEnabledMenu()
        {
            string sql = @"
                SELECT Menu.Seq --序號
	                , Menu.ParentSeq --父層序號
	                , Menu.[Name] --名稱
	                , Menu.[Url] --網址
	                , Menu.OrderNo --排列順序
	                , Menu.CanManage --是否有管理的功能(0:否, 1:是)
	                , Menu.IsEnabled --是否啟用(0:否, 1:是)
	                , Menu.CreateTime --建立時間
	                , Menu.CreateUer --建立人員序號
	                , Menu.ModifyTime --異動時間
	                , Menu.ModifyUser --異動人員序號
                FROM menu
                WHERE IsEnabled = 1
                ORDER BY OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<Menu>(cmd);
        }

        public string GetMenuParentString(int seq)
        {
            string sql = @"
                SELECT dbo.FN_MenuParentString(@Seq)";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);
            DataTable dt = db.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }



}