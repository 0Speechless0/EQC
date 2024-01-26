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
using EQC.ViewModel.Common;

namespace EQC.Services
{
    public class RoleService
    {
        private DBConn db = new DBConn();

        /// <summary> 取得角色列表 </summary>
        /// <param name="page"> 頁數 </param>
        /// <param name="per_page"> 跳頁 </param>
        /// <returns> </returns>
        public List<VRole> GetList(int page, int per_page)
        {
            string sql = @"
				SELECT r.Seq
					,r.[Name]
					,r.RoleDesc
					,r.IsEnabled
					,r.IsDefault
					,r.CreateTime
					,r.CreateUserSeq
					,r.ModifyTime
					,r.ModifyUserSeq						  
					,ROW_NUMBER() OVER(ORDER BY r.Seq) AS Rows	
				FROM [Role] r
                WHERE IsDelete = 0
				ORDER BY CASE @Sort_by
						WHEN 'r.Seq'
							THEN r.Seq
						END OFFSET @Page Rows
				FETCH FIRST @Per_page ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Sort_by", "Seq");
            cmd.Parameters.AddWithValue("@Page", page * per_page);
            cmd.Parameters.AddWithValue("@Per_page", per_page);
            return db.GetDataTableWithClass<VRole>(cmd);
        }

        /// <summary> 取得角色列表 </summary>
        /// <param name="userSeq"> UserMain.Seq </param>
        /// <returns> </returns>
        public List<Role> GetListByUserSeq(int userMainSeq)
        {
            string sql = @"
				SELECT Role.* FROM UserRole
                    JOIN UserUnitPosition ON UserUnitPosition.Seq = UserRole.UserUnitPositionSeq
                    JOIN Role ON Role.Seq = UserRole.RoleSeq
                    WHERE UserUnitPosition.UserMainSeq = @UserMainSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@UserMainSeq", userMainSeq);
            return db.GetDataTableWithClass<Role>(cmd);
        }

        /// <summary> 新增 角色 </summary>
        /// <param name="vRole"></param>
        /// <returns></returns>
        public SaveChangeStatus AddRole(VRole vRole)
        {
            try
            {
                SaveChangeStatus saveChangeStatus = new SaveChangeStatus(true, StatusCode.Validation);
                string sql = @"
				INSERT INTO [Role] (
					[Name]
					,RoleDesc
					,IsEnabled
					,IsDelete
					,IsDefault
					,CreateTime
					,CreateUserSeq
					,ModifyTime
					,ModifyUserSeq
					)
				VALUES (
					@Name
					,@RoleDesc
					,@IsEnabled
					,@IsDelete
					,@IsDefault
					,GETDATE()
					,@CreateUserSeq
					,GETDATE()
					,@ModifyUserSeq
					)";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Name", vRole.Name);
                cmd.Parameters.AddWithValue("@RoleDesc", vRole.RoleDesc);
                cmd.Parameters.AddWithValue("@IsEnabled", true);
                cmd.Parameters.AddWithValue("@IsDelete", false);
                cmd.Parameters.AddWithValue("@IsDefault", false);
                cmd.Parameters.AddWithValue("@CreateUserSeq", new SessionManager().GetUser().Seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", new SessionManager().GetUser().Seq);
                db.ExecuteNonQuery(cmd);
                return saveChangeStatus;
            }
            catch (Exception ex)
            {
                return new SaveChangeStatus(false, StatusCode.Save, ex);
            }
        }

        /// <summary> 取得總筆數 </summary>
        /// <returns> 總筆數 </returns>
        public Object GetCount()
        {
            string sql = @"
				SELECT count(*)
				FROM [Role]
                WHERE IsDelete = 0";
            SqlCommand cmd = db.GetCommand(sql);
            return db.ExecuteScalar(cmd);
        }

        /// <summary> 角色資料修改 </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public SaveChangeStatus UpdateRole(VRole vRole)
        {
            try
            {
                SaveChangeStatus saveChangeStatus = new SaveChangeStatus(true, StatusCode.Save);
                string sql = @"
				UPDATE [Role]
				SET [Name] = @Name
					,RoleDesc = @RoleDesc
					,ModifyTime = GETDATE()
					,ModifyUserSeq = @ModifyUserSeq
				WHERE Seq = @Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", vRole.Seq);
                cmd.Parameters.AddWithValue("@Name", vRole.Name);
                cmd.Parameters.AddWithValue("@RoleDesc", vRole.RoleDesc);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", new SessionManager().GetUser().Seq);
                db.ExecuteNonQuery(cmd);
                return saveChangeStatus;
            }
            catch (Exception ex)
            {
                return new SaveChangeStatus(false, StatusCode.Save, ex);
            }
        }

        /// <summary> 刪除角色 </summary>
        /// <param name="Seq"> Seq </param>
        /// <returns></returns>

        public SaveChangeStatus DeleteRole(int seq)
        {
            try
            {
                SaveChangeStatus saveChangeStatus = new SaveChangeStatus(true, StatusCode.Delete);
                string sql = @"
				UPDATE [Role]
                SET IsDelete = 1,
				ModifyTime = GETDATE(),
				ModifyUserSeq = @ModifyUserSeq
                WHERE Seq = @Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", new SessionManager().GetUser().Seq);
                db.ExecuteNonQuery(cmd);
                return saveChangeStatus;
            }
            catch (Exception ex)
            {
                return new SaveChangeStatus(false, StatusCode.Save, ex);
            }
        }

        /// <summary> 取得角色(下拉選單資料) </summary>
        /// <returns></returns>
        internal List<SelectVM> GetRoleList()
        {
            string sql = @"
				SELECT Cast(Seq as varchar(10)) as Value, 
				Name as Text FROM Role
				WHERE IsDelete = 0 AND IsEnabled = 1";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<SelectVM>(cmd);
        }
    }
}