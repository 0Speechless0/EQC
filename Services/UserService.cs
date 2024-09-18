using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebSockets;
using EQC.Common;
using EQC.EDMXModel;
using EQC.ViewModel;

using EQC.Models;
using EQC.ViewModel;
using EQC.ViewModel.Common;
using Newtonsoft.Json.Linq;
using UserMain = EQC.Models.UserMain;
using Role = EQC.Models.Role;

namespace EQC.Services
{
    public class UserService : BaseService
    {
        private DBConn db = new DBConn();

        /// <summary> 取得人員含單位資料 </summary>
        /// <param name="userSeq"> 單位序號 </param>
        /// 
        public IEnumerable<Models.UserMain> GetUserByAccountKeyWord(string keyWord, int role)
        {
            using (var context = new EQC_NEW_Entities())
            {

                context.Configuration.LazyLoadingEnabled = false;
                var userRoleDic =
                    context.UserMain
                    .Include("UserUnitPosition.Role")
                    .ToList()
                    .Join(
                        context.UserUnitPosition,
                        user => user.Seq, pos => pos.UserMainSeq,
                        (user, pos) => pos

                    ).ToDictionary(r => r.UserMainSeq, r => r.Role.FirstOrDefault()?.Seq);

                var list = context.UserMain
                    .ToList()
                    .Where(user => {

                        userRoleDic.TryGetValue(user.Seq, out byte? roleValue);
                        return roleValue == role && user.UserNo.Contains(keyWord);
                    })
                    .Select(r => new Models.UserMain
                    {
                        Seq = r.Seq,
                        DisplayName = r.DisplayName,
                        UserNo = r.UserNo

                    });

                return list;
            }
        }
        public List<VUserMain> GetUserInfo(int userSeq) //20210704 shioulo
        {
            string sql = @"
				SELECT a.[Seq]
                    ,a.[UserNo]                      
                    ,a.[DisplayName]
                    ,a.[TelRegion]
                    ,a.[Tel]
                    ,a.[TelExt]
                    ,a.[Mobile]
                    ,a.[Email]
                    ,a.[IsEnabled]
                    ,a.[IsDelete]
                    ,a.[CreateTime]
                    ,a.[CreateUserSeq]
                    ,a.[ModifyTime]
                    ,a.[ModifyUserSeq]						  
					,ROW_NUMBER() OVER(ORDER BY a.Seq) AS Rows		
					,ISNULL(g.Name,ISNULL(f.Name,c.Name)) as UnitName1
                    ,ISNULL(g.Seq,ISNULL(f.Seq,c.Seq))  as UnitSeq1					  
					,Case When g.Name is null Then (Case When f.name is null Then NULL Else c.name END) ELSE f.Name END  as UnitName2
                    ,Case When g.Seq is null Then (Case When f.Seq is null Then NULL Else c.Seq END) ELSE f.Seq END as UnitSeq2
					,Case When f.Name is null or g.Name is null THEN NULL ELSE c.Name END as UnitName3
                    ,Case When f.Seq is null or g.Seq is null THEN NULL ELSE c.Seq END as UnitSeq3
					,e.Name as RoleName 	
                    ,e.Seq as RoleSeq 
                    ,b.PositionSeq
			FROM UserMain a
			join UserUnitPosition b on a.Seq=b.UserMainSeq
			left join Unit c on b.UnitSeq=c.Seq
			left join UserRole d on b.Seq=d.UserUnitPositionSeq
			left join Role e on d.RoleSeq=e.Seq
            left join Unit f on c.ParentSeq=f.Seq
			left join Unit g on f.ParentSeq=g.Seq
			where a.Seq=@Seq";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", userSeq);
            return db.GetDataTableWithClass<VUserMain>(cmd);
        }


        public int GetDepartmentUserCount(int unitSeq)
        {
            string sql = @"

                    SELECT Count(*)
				FROM UserMain a
				join UserUnitPosition b on a.Seq=b.UserMainSeq
				left join Unit c on b.UnitSeq=c.Seq
				left join UserRole d on b.Seq=d.UserUnitPositionSeq
				left join Role e on d.RoleSeq=e.Seq
                left join Unit f on c.ParentSeq=f.Seq
				left join Unit g on f.ParentSeq=g.Seq
				where  a.IsDelete=0 and (c.Seq Like @unit or f.Seq Like @unit or g.Seq Like @unit)";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@unit", unitSeq);

            return (int)db.ExecuteScalar(cmd);
        }
        public List<VUserMain> GetDepartmentUser(int unitSeq, string nameSearch, int? page, int? per_page)
        {

            try
            {
                string sql = @"

                    SELECT a.[Seq]
                      ,a.[UserNo]                      
                      ,a.[DisplayName]
                      ,a.[PassWord]
                      ,a.[TelRegion]
                      ,a.[Tel]
                      ,a.[TelExt]
                      ,a.[Mobile]
                      ,a.[Email]
                      ,a.[IsEnabled]
                      ,a.[IsDelete]
                      ,a.[CreateTime]
                      ,a.[CreateUserSeq]
                      ,a.[ModifyTime]
                      ,a.[ModifyUserSeq]						  
					  ,ROW_NUMBER() OVER(ORDER BY a.Seq) AS Rows		
					  ,ISNULL(g.Name,ISNULL(f.Name,c.Name)) as UnitName1
                      ,ISNULL(g.Seq,ISNULL(f.Seq,c.Seq))  as UnitSeq1					  
					  ,ISNULL('/' + Case When g.Name is null Then (Case When f.name is null Then NULL Else c.name END) ELSE f.Name END, '')  as UnitName2
                      ,Case When g.Seq is null Then (Case When f.Seq is null Then NULL Else c.Seq END) ELSE f.Seq END as UnitSeq2
					  ,ISNULL('/' + Case When f.Name is null or g.Name is null THEN NULL ELSE c.Name END, '') as UnitName3
                      ,Case When f.Seq is null or g.Seq is null THEN NULL ELSE c.Seq END as UnitSeq3
					  ,e.Name as RoleName 	
                      ,e.Seq as RoleSeq 
                      ,b.PositionSeq
                      ,(SELECT COUNT(*) FROM SignatureFile WHERE UserMainSeq = a.Seq) AS SignatureFileCount
				FROM UserMain a
				join UserUnitPosition b on a.Seq=b.UserMainSeq
				left join Unit c on b.UnitSeq=c.Seq
				left join UserRole d on b.Seq=d.UserUnitPositionSeq
				left join Role e on d.RoleSeq=e.Seq
                left join Unit f on c.ParentSeq=f.Seq
				left join Unit g on f.ParentSeq=g.Seq
				where  a.IsDelete=0 and (c.Seq Like @unit or f.Seq Like @unit or g.Seq Like @unit)
                and ( a.IsDelete=0  and (a.DisplayName Like @nameSearch  or a.UserNo Like @nameSearch ))
				ORDER BY CASE @Sort_by
						WHEN 'a.Seq'
							THEN a.Seq
						END OFFSET @Page ROWS
				FETCH FIRST @Per_page ROWS ONLY";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Page", page * per_page ?? 1);
                cmd.Parameters.AddWithValue("@Per_page", per_page ?? 100);
                cmd.Parameters.AddWithValue("@Sort_by", "a.Seq");
                cmd.Parameters.AddWithValue("@nameSearch", '%' + (nameSearch ?? "") + '%' ?? "");
                cmd.Parameters.AddWithValue("@unit", unitSeq);
                return db.GetDataTableWithClass<VUserMain>(cmd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary> 取得人員列表 </summary>
        /// <param name="page"> 頁數 </param>
        /// <param name="per_page"> 跳頁 </param>
        /// <param name="unitSeq"> 單位序號 </param>
        public List<VUserMain> GetList(int unitSeq, string nameSearch, int? page = null, int? per_page = null)
        {
            try
            {
                string sql = @"
					SELECT a.[Seq]
                      ,a.[UserNo]                      
                      ,a.[DisplayName]
                      ,a.[PassWord]
                      ,a.[TelRegion]
                      ,a.[Tel]
                      ,a.[TelExt]
                      ,a.[Mobile]
                      ,a.[Email]
                      ,a.[IsEnabled]
                      ,a.[IsDelete]
                      ,a.[CreateTime]
                      ,a.[CreateUserSeq]
                      ,a.[ModifyTime]
                      ,a.[ModifyUserSeq]						  
					  ,ROW_NUMBER() OVER(ORDER BY a.Seq) AS Rows		
					  ,ISNULL(g.Name,ISNULL(f.Name,c.Name)) as UnitName1
                      ,ISNULL(g.Seq,ISNULL(f.Seq,c.Seq))  as UnitSeq1					  
					  ,ISNULL('/' + Case When g.Name is null Then (Case When f.name is null Then NULL Else c.name END) ELSE f.Name END, '')  as UnitName2
                      ,Case When g.Seq is null Then (Case When f.Seq is null Then NULL Else c.Seq END) ELSE f.Seq END as UnitSeq2
					  ,ISNULL('/' + Case When f.Name is null or g.Name is null THEN NULL ELSE c.Name END, '') as UnitName3
                      ,Case When f.Seq is null or g.Seq is null THEN NULL ELSE c.Seq END as UnitSeq3
					  ,e.Name as RoleName 	
                      ,e.Seq as RoleSeq 
                      ,b.PositionSeq
                      ,(SELECT COUNT(*) FROM SignatureFile WHERE UserMainSeq = a.Seq) AS SignatureFileCount
				FROM UserMain a
				join UserUnitPosition b on a.Seq=b.UserMainSeq
				left join Unit c on b.UnitSeq=c.Seq
				left join UserRole d on b.Seq=d.UserUnitPositionSeq
				left join Role e on d.RoleSeq=e.Seq
                left join Unit f on c.ParentSeq=f.Seq
				left join Unit g on f.ParentSeq=g.Seq
				where( a.IsDelete=0 and ((c.Seq Like @UnitSeq OR g.Seq Like @UnitSeq OR f.Seq Like @UnitSeq )  OR @UnitSeq = 1))
                and ( a.IsDelete=0  and (a.DisplayName Like @nameSearch  or a.UserNo Like @nameSearch ))
				ORDER BY CASE @Sort_by
						WHEN 'a.Seq'
							THEN a.Seq
						END OFFSET @Page ROWS
				FETCH FIRST @Per_page ROWS ONLY";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Sort_by", "a.Seq");
                cmd.Parameters.AddWithValue("@Page", page * per_page ?? 1);
                cmd.Parameters.AddWithValue("@Per_page", per_page ?? 100);
                cmd.Parameters.AddWithValue("@UnitSeq", unitSeq);
                cmd.Parameters.AddWithValue("@nameSearch", '%'+(nameSearch ?? "")+'%' ?? "");
                var l = db.GetDataTableWithClass<VUserMain>(cmd);
                return l;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<VUserMain> GetListV2(int unitSeq, string nameSearch, bool hasConstCheckApp)
        {
            try
            {
                string sql = @"
					SELECT a.[Seq]
                      ,a.[UserNo]                      
                      ,a.[DisplayName]
                      ,a.[PassWord]
                      ,a.[TelRegion]
                      ,a.[Tel]
                      ,a.[TelExt]
                      ,a.[Mobile]
                      ,a.[Email]
                      ,a.[IsEnabled]
                      ,a.[IsDelete]
                      ,a.[CreateTime]
                      ,a.[CreateUserSeq]
                      ,a.[LastLoginTime]  
                      ,a.[ModifyTime]
                      ,a.[ModifyUserSeq]						  
					  ,ROW_NUMBER() OVER(ORDER BY a.Seq) AS Rows		
					  ,ISNULL(g.Name,ISNULL(f.Name,c.Name)) as UnitName1
                      ,ISNULL(g.Seq,ISNULL(f.Seq,c.Seq))  as UnitSeq1					  
					  ,ISNULL('/' + Case When g.Name is null Then (Case When f.name is null Then NULL Else c.name END) ELSE f.Name END, '')  as UnitName2
                      ,Case When g.Seq is null Then (Case When f.Seq is null Then NULL Else c.Seq END) ELSE f.Seq END as UnitSeq2
					  ,ISNULL('/' + Case When f.Name is null or g.Name is null THEN NULL ELSE c.Name END, '') as UnitName3
                      ,Case When f.Seq is null or g.Seq is null THEN NULL ELSE c.Seq END as UnitSeq3
					  ,e.Name as RoleName 	
                      ,e.Seq as RoleSeq 
					  ,e2.Name as RoleName2	
                      ,e2.Seq as RoleSeq2
                      ,b.PositionSeq
                      ,(SELECT COUNT(*) FROM SignatureFile WHERE UserMainSeq = a.Seq) AS SignatureFileCount
                      ,ccal.Seq ConstCheckAppLock
                      ,ccal.CreateTime ConstCheckAppCreateTime
				FROM UserMain a
				join UserUnitPosition b on a.Seq=b.UserMainSeq
				left join Unit c on b.UnitSeq=c.Seq
				left join UserRole d on b.Seq=d.UserUnitPositionSeq
				left join Role e on d.RoleSeq=e.Seq
				left join Role e2 on d.RoleSeq2=e2.Seq
                left join Unit f on c.ParentSeq=f.Seq
				left join Unit g on f.ParentSeq=g.Seq
                left join ConstCheckAppLock ccal on ccal.UserMainSeq = a.Seq
				where( a.IsDelete=0 and ((c.Seq Like @UnitSeq OR g.Seq Like @UnitSeq OR f.Seq Like @UnitSeq )  OR @UnitSeq = 1))
                and ( a.IsDelete=0  and (a.DisplayName Like @nameSearch  or a.UserNo Like @nameSearch ))
                and (ccal.Seq is not null or @hasConstCheckApp = 0 )
				ORDER BY a.Seq";

                SqlCommand cmd = db.GetCommand(sql);
                //cmd.Parameters.AddWithValue("@Sort_by", "a.Seq");
                cmd.Parameters.AddWithValue("@UnitSeq", unitSeq);
                cmd.Parameters.AddWithValue("@hasConstCheckApp", hasConstCheckApp ? 1 : 0 );
                cmd.Parameters.AddWithValue("@nameSearch", '%' + (nameSearch ?? "") + '%' ?? "");
                return db.GetDataTableWithClass<VUserMain>(cmd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal List<VUserMain> GetChildList(int page, int per_page)
        {
            SessionManager sessionManager = new SessionManager();
            UserInfo userInfo = sessionManager.GetUser();
            string sql = @"
					SELECT a.[Seq]
                      ,a.[UserNo]                      
                      ,a.[DisplayName]
                      ,a.[PassWord]
                      ,a.[TelRegion]
                      ,a.[Tel]
                      ,a.[TelExt]
                      ,a.[Mobile]
                      ,a.[Email]
                      ,a.[IsEnabled]
                      ,a.[IsDelete]
                      ,a.[CreateTime]
                      ,a.[CreateUserSeq]
                      ,a.[ModifyTime]
                      ,a.[ModifyUserSeq]						  
					  ,ROW_NUMBER() OVER(ORDER BY a.Seq) AS Rows		
					  ,ISNULL(g.Name,ISNULL(f.Name,c.Name)) as UnitName1
                      ,ISNULL(g.Seq,ISNULL(f.Seq,c.Seq))  as UnitSeq1					  
					  ,ISNULL('/' + Case When g.Name is null Then (Case When f.name is null Then NULL Else c.name END) ELSE f.Name END, '')  as UnitName2
                      ,Case When g.Seq is null Then (Case When f.Seq is null Then NULL Else c.Seq END) ELSE f.Seq END as UnitSeq2
					  ,ISNULL('/' + Case When f.Name is null or g.Name is null THEN NULL ELSE c.Name END, '') as UnitName3
                      ,Case When f.Seq is null or g.Seq is null THEN NULL ELSE c.Seq END as UnitSeq3
					  ,e.Name as RoleName 	
                      ,e.Seq as RoleSeq 
                      ,b.PositionSeq
                      ,(SELECT COUNT(*) FROM SignatureFile WHERE UserMainSeq = a.Seq) AS SignatureFileCount
                from UserMain a 
				join UserUnitPosition b on a.Seq=b.UserMainSeq
				left join Unit c on b.UnitSeq=c.Seq
				left join UserRole d on b.Seq=d.UserUnitPositionSeq
				left join Role e on d.RoleSeq=e.Seq
                left join Unit f on c.ParentSeq=f.Seq
				left join Unit g on f.ParentSeq=g.Seq
				where a.IsDelete=0
				and a.Seq in(
                    Select UserMain.Seq from UserMain 
                    inner join UserUnitPosition up 
                    on up.UserMainSeq = UserMain.CreateUserSeq
                    inner join UserRole ur
                    on ur.UserUnitPositionSeq = up.Seq
                    where (ur.RoleSeq in (
                        Select distinct r.RoleSeq from UserRole r
                        inner join UserUnitPosition up2
                        on up2.Seq = r.UserUnitPositionSeq
                        inner join UserMain um2
                        on um2.Seq = up2.UserMainSeq
                        where um2.Seq = @userSeq
                    )
                and a.UserNo Like @preUserNo)
                or a.Seq = @userSeq
                group by UserMain.Seq) 
				ORDER BY CASE @Sort_by
						WHEN 'a.Seq'
							THEN  a.Seq
						END OFFSET @Page ROWS
				FETCH FIRST @Per_page ROWS ONLY
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Sort_by", "a.Seq");
            cmd.Parameters.AddWithValue("@preUserNo", userInfo.UserNo+"%");
            cmd.Parameters.AddWithValue("@userSeq", userInfo.Seq);
            cmd.Parameters.AddWithValue("@Page", page * per_page);
            cmd.Parameters.AddWithValue("@Per_page", per_page);
            return db.GetDataTableWithClass<VUserMain>(cmd);

        }
        public int GetChildListCount()
        {
            SessionManager sessionManager = new SessionManager();
            UserInfo userInfo = sessionManager.GetUser();
            string sql = @"
                Select Count(*) from UserMain  where Seq in(
                    Select UserMain.Seq from UserMain 
                    inner join UserUnitPosition up 
                    on up.UserMainSeq = UserMain.CreateUserSeq
                    inner join UserRole ur
                    on ur.UserUnitPositionSeq = up.Seq
                    where ur.RoleSeq in (
                        Select distinct r.RoleSeq from UserRole r
                        inner join UserUnitPosition up2
                        on up2.Seq = r.UserUnitPositionSeq
                        inner join UserMain um2
                        on um2.Seq = up2.UserMainSeq
                        where um2.Seq = @userSeq
                    )
                or UserMain.Seq = @userSeq
                group by UserMain.Seq)";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@userSeq", userInfo.Seq);
            return (int)( db.ExecuteScalar(cmd) ?? 0);

        }
        public List<VUserMain> GetUserBelong(int userSeq, bool hasConstCheckApp = false)
        {
            try
            {
                string sql = @"

                    SELECT a.[Seq]
                      ,a.[UserNo]                      
                      ,a.[DisplayName]
                      ,a.[PassWord]
                      ,a.[TelRegion]
                      ,a.[Tel]
                      ,a.[TelExt]
                      ,a.[Mobile]
                      ,a.[Email]
                      ,a.[IsEnabled]
                      ,a.[IsDelete]
                      ,a.[CreateTime]
                      ,a.[CreateUserSeq]
                      ,a.[ModifyTime]
                      ,a.[ModifyUserSeq]						  
					  ,ROW_NUMBER() OVER(ORDER BY a.Seq) AS Rows		
					  ,ISNULL(g.Name,ISNULL(f.Name,c.Name)) as UnitName1
                      ,ISNULL(g.Seq,ISNULL(f.Seq,c.Seq))  as UnitSeq1					  
					  ,ISNULL('/' + Case When g.Name is null Then (Case When f.name is null Then NULL Else c.name END) ELSE f.Name END, '')  as UnitName2
                      ,Case When g.Seq is null Then (Case When f.Seq is null Then NULL Else c.Seq END) ELSE f.Seq END as UnitSeq2
					  ,ISNULL('/' + Case When f.Name is null or g.Name is null THEN NULL ELSE c.Name END, '') as UnitName3
                      ,Case When f.Seq is null or g.Seq is null THEN NULL ELSE c.Seq END as UnitSeq3
					  ,e.Name as RoleName 	
                      ,e.Seq as RoleSeq 
                      ,b.PositionSeq
                      ,ccal.Seq ConstCheckAppLock
                      ,ccal.CreateTime ConstCheckAppCreateTime
                      ,(SELECT COUNT(*) FROM SignatureFile WHERE UserMainSeq = a.Seq) AS SignatureFileCount
				FROM UserMain a
				join UserUnitPosition b on a.Seq=b.UserMainSeq
				left join Unit c on b.UnitSeq=c.Seq
				left join UserRole d on b.Seq=d.UserUnitPositionSeq
				left join Role e on d.RoleSeq=e.Seq
                left join Unit f on c.ParentSeq=f.Seq
				left join Unit g on f.ParentSeq=g.Seq
                left join ConstCheckAppLock ccal on ccal.UserMainSeq = a.Seq
				where  a.IsDelete=0 and (a.Seq = @userSeq or a.CreateUserSeq = @userSeq)
                and (ccal.Seq is not null or @hasConstCheckApp = 0 )
				ORDER BY CASE @Sort_by
						WHEN 'a.Seq'
							THEN a.Seq END";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Sort_by", "a.Seq");
                cmd.Parameters.AddWithValue("@hasConstCheckApp", hasConstCheckApp ? 1 : 0);
                cmd.Parameters.AddWithValue("@userSeq", userSeq);
                return db.GetDataTableWithClass<VUserMain>(cmd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<VUserMain> GetUser(int userSeq)
        {
            try
            {
                string sql = @"

                    SELECT a.[Seq]
                      ,a.[UserNo]                      
                      ,a.[DisplayName]
                      ,a.[PassWord]
                      ,a.[TelRegion]
                      ,a.[Tel]
                      ,a.[TelExt]
                      ,a.[Mobile]
                      ,a.[Email]
                      ,a.[IsEnabled]
                      ,a.[IsDelete]
                      ,a.[CreateTime]
                      ,a.[CreateUserSeq]
                      ,a.[ModifyTime]
                      ,a.[ModifyUserSeq]						  
					  ,ROW_NUMBER() OVER(ORDER BY a.Seq) AS Rows		
					  ,ISNULL(g.Name,ISNULL(f.Name,c.Name)) as UnitName1
                      ,ISNULL(g.Seq,ISNULL(f.Seq,c.Seq))  as UnitSeq1					  
					  ,ISNULL('/' + Case When g.Name is null Then (Case When f.name is null Then NULL Else c.name END) ELSE f.Name END, '')  as UnitName2
                      ,Case When g.Seq is null Then (Case When f.Seq is null Then NULL Else c.Seq END) ELSE f.Seq END as UnitSeq2
					  ,ISNULL('/' + Case When f.Name is null or g.Name is null THEN NULL ELSE c.Name END, '') as UnitName3
                      ,Case When f.Seq is null or g.Seq is null THEN NULL ELSE c.Seq END as UnitSeq3
					  ,e.Name as RoleName 	
                      ,e.Seq as RoleSeq 
                      ,b.PositionSeq
                      ,(SELECT COUNT(*) FROM SignatureFile WHERE UserMainSeq = a.Seq) AS SignatureFileCount
				FROM UserMain a
				join UserUnitPosition b on a.Seq=b.UserMainSeq
				left join Unit c on b.UnitSeq=c.Seq
				left join UserRole d on b.Seq=d.UserUnitPositionSeq
				left join Role e on d.RoleSeq=e.Seq
                left join Unit f on c.ParentSeq=f.Seq
				left join Unit g on f.ParentSeq=g.Seq
				where  a.IsDelete=0 and a.Seq = @userSeq 
				ORDER BY CASE @Sort_by
						WHEN 'a.Seq'
							THEN a.Seq END";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Sort_by", "a.Seq");
                cmd.Parameters.AddWithValue("@userSeq", userSeq);
                return db.GetDataTableWithClass<VUserMain>(cmd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary> 取得總筆數 </summary>
        /// <param name="unitSeq"> unitSeq </param>
        /// <returns> Count </returns>
        public Object GetCount(int unitSeq)
        {
            string sql = @"
				SELECT COUNT(*)
				FROM UserMain a
				join UserUnitPosition b on a.Seq=b.UserMainSeq
				left join Unit c on b.UnitSeq=c.Seq
				left join UserRole d on b.Seq=d.UserUnitPositionSeq
				left join Role e on d.RoleSeq=e.Seq
                left join Unit f on c.ParentSeq=f.Seq
				left join Unit g on f.ParentSeq=g.Seq
				where a.IsDelete=0 and ((c.Seq = @UnitSeq OR g.Seq = @UnitSeq OR f.Seq = @UnitSeq ) OR @UnitSeq = 0 OR @UnitSeq = 1)";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@UnitSeq", unitSeq);
            return db.ExecuteScalar(cmd);
        }

        //shioulo 20220706
        public List<VUserMain> GetUser(string userNo, string passWd)
        {
            string sql = @"SELECT *
                FROM UserMain
                WHERE userNo = @userNo
	                AND PassWord = @password
                    AND IsDelete=0 AND IsEnabled=1";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@userNo", userNo);
            cmd.Parameters.AddWithValue("@password", passWd);
            return db.GetDataTableWithClass<VUserMain>(cmd);
        }
        public int CheckUser(string userNo, string passWd)
        {
            string sql = @"SELECT *
                FROM UserMain
                WHERE userNo = @userNo
	                AND PassWord = @password
                    AND IsDelete=0 AND IsEnabled=1";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@userNo", userNo);
            cmd.Parameters.AddWithValue("@password", passWd);
            return db.GetDataTable(cmd).Rows.Count;
        }

        //android 登入
        public int CheckUser2(string userNo, string Mobile)
        {
            string sql = @"SELECT *
                FROM UserMain
                WHERE userNo = @userNo
	                AND Mobile = @Mobile
                    AND IsDelete=0 AND IsEnabled=1";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@userNo", userNo);
            cmd.Parameters.AddWithValue("@Mobile", Mobile);
            return db.GetDataTable(cmd).Rows.Count;
        }

        //Oauth 登入
        public int CheckUser3(string userNo, string userName)
        {
            //string sql = @"SELECT *
            //    FROM UserMain
            //    WHERE userNo = @userNo
            // AND Email = @Mail
            //    AND DisplayName = @userName";
            string sql = @"SELECT *
                FROM UserMain
                WHERE userNo = @userNo
                AND DisplayName = @userName
                AND IsDelete=0 AND IsEnabled=1";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@userNo", userNo);
            //cmd.Parameters.AddWithValue("@Mail", Mail);
            cmd.Parameters.AddWithValue("@userName", userName);
            return db.GetDataTable(cmd).Rows.Count;
        }

        //MobileAPI 登入
        public int CheckUser4(string userNo, string phone)
        {
            //string sql = @"SELECT *
            //    FROM UserMain
            //    WHERE userNo = @userNo
            // AND Email = @Mail
            //    AND DisplayName = @userName";
            string sql = @"SELECT *
                FROM UserMain
                WHERE userNo = @userNo
                AND Mobile = @phone
                AND IsDelete=0 AND IsEnabled=1";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@userNo", userNo);
            //cmd.Parameters.AddWithValue("@Mail", Mail);
            cmd.Parameters.AddWithValue("@phone", phone);
            return db.GetDataTable(cmd).Rows.Count;
        }

        /// <summary> 新增人員 </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public SaveChangeStatus AddUser(VUserMain vUserMain)
        {
            db.BeginTransaction();
            try
            {
                int userMainSeq = 0;
                SaveChangeStatus saveChangeStatus = new SaveChangeStatus(true, StatusCode.Validation);
                // 檢查帳號是否存在
                string existSql = @"Select Count(*) as Count From UserMain Where UserNo =@UserNo";
                SqlCommand existCmd = db.GetCommand(existSql);
                existCmd.Parameters.AddWithValue("@UserNo", vUserMain.UserNo);
                var userData = db.GetDataTable(existCmd);
                existCmd.Parameters.Clear();
                if (userData.Rows[0].Field<int>("Count") > 0)
                {
                    saveChangeStatus = new SaveChangeStatus(false, StatusCode.Validation);
                    saveChangeStatus.Message = "帳號已存在!";
                }

                if (saveChangeStatus.IsSuccess)
                {
                    saveChangeStatus = new SaveChangeStatus(true, StatusCode.Save);
                    // 新增資料
                    string sql = @"
                    INSERT INTO [UserMain]
                           ([UserNo]
                           ,[PassWord]
                           ,[DisplayName]
                           ,[TelRegion]
                           ,[Tel]
                           ,[TelExt]
                           ,[Mobile]
                           ,[Email]
                           ,[IsEnabled]
                           ,[CreateTime]
                           ,[CreateUserSeq]
                           ,[ModifyTime]
                           ,[ModifyUserSeq])
                     VALUES
                           (@UserNo
                           ,@PassWord
                           ,@DisplayName
                           ,@TelRegion
                           ,@Tel
                           ,@TelExt
                           ,@Mobile
                           ,@Email
                           ,@IsEnabled
                           ,@CreateTime
                           ,@CreateUserSeq
                           ,@ModifyTime
                           ,@ModifyUserSeq)";
                    SqlCommand cmd = db.GetCommand(sql);
                    cmd.Parameters.AddWithValue("@UserNo", vUserMain.UserNo);
                    cmd.Parameters.AddWithValue("@PassWord", vUserMain.PassWord);
                    cmd.Parameters.AddWithValue("@DisplayName", vUserMain.DisplayName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TelRegion", vUserMain.TelRegion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Tel", vUserMain.Tel ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TelExt", vUserMain.TelExt ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Mobile", vUserMain.Mobile ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", vUserMain.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsEnabled", vUserMain.IsEnabled);
                    cmd.Parameters.AddWithValue("@CreateTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@ModifyTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreateUserSeq", new SessionManager().GetUser().Seq);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", new SessionManager().GetUser().Seq);
                    db.ExecuteNonQuery(cmd);
                    cmd.Parameters.Clear();

                    string sql1 = @"SELECT IDENT_CURRENT('UserMain') AS Seq";
                    cmd = db.GetCommand(sql1);
                    DataTable dt = db.GetDataTable(cmd);
                    userMainSeq = Convert.ToInt32(dt.Rows[0]["Seq"].ToString());

                    cmd.Parameters.Clear();
                    string sql2 = @"INSERT INTO [UserUnitPosition]
                                   (UnitSeq 
                                   ,UserMainSeq 
                                   ,PositionSeq 
                                   ,IsEnabled 
                                   ,CreateTime 
                                   ,CreateUserSeq 
                                   ,ModifyTime 
                                   ,ModifyUserSeq)
                                    VALUES
                                   (@UnitSeq
                                   ,@UserMainSeq
                                   ,@PositionSeq
                                   ,@IsEnabled
                                   ,@CreateTime
                                   ,@CreateUserSeq
                                   ,@ModifyTime
                                   ,@ModifyUserSeq)";

                    cmd = db.GetCommand(sql2);
                    short? unitSeq = (vUserMain.UnitSeq3 ?? vUserMain.UnitSeq2) ?? vUserMain.UnitSeq1;
                    cmd.Parameters.AddWithValue("@UnitSeq", unitSeq ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserMainSeq", userMainSeq);
                    cmd.Parameters.AddWithValue("@PositionSeq", vUserMain.PositionSeq == 0 ? (object)DBNull.Value : vUserMain.PositionSeq);
                    cmd.Parameters.AddWithValue("@IsEnabled", true);
                    cmd.Parameters.AddWithValue("@CreateTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@ModifyTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreateUserSeq", new SessionManager().GetUser().Seq);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", new SessionManager().GetUser().Seq);
                    db.ExecuteNonQuery(cmd);
                    cmd.Parameters.Clear();

                    string sql3 = @"SELECT IDENT_CURRENT('UserUnitPosition') AS Seq";
                    cmd = db.GetCommand(sql3);
                    DataTable dt1 = db.GetDataTable(cmd);
                    int UserUnitPositionSeq = Convert.ToInt32(dt1.Rows[0]["Seq"].ToString());
                    cmd.Parameters.Clear();
                    string sql4 = @"INSERT INTO UserRole
                                   (UserUnitPositionSeq
                                   ,RoleSeq
                                   ,RoleSeq2)
                                    VALUES
                                   (@UserUnitPositionSeq
                                   ,@RoleSeq
                                   ,@RoleSeq2)";

                    cmd = db.GetCommand(sql4);
                    cmd.Parameters.AddWithValue("@UserUnitPositionSeq", UserUnitPositionSeq);
                    cmd.Parameters.AddWithValue("@RoleSeq", vUserMain.RoleSeq);
                    cmd.Parameters.AddWithValue("@RoleSeq2", Utils.NulltoDBNull(vUserMain.RoleSeq2) );
                    db.ExecuteNonQuery(cmd);
                    cmd.Parameters.Clear();
                }

                db.TransactionCommit();
                saveChangeStatus.Data = userMainSeq;
                return saveChangeStatus;
            }
            catch (Exception ex)
            {
                db.TransactionRollback();
                return new SaveChangeStatus(false, StatusCode.Save, ex);
            }
        }

        /// <summary> 單簽新增人員 </summary>
        /// <returns></returns>
        public int OauthAddUser(string UserNo, string Mail, string userName, string Phone, string Unit, string UpperUnit1, string UpperUnit2)
        {
            db.BeginTransaction();
            try
            {
                int userMainSeq = 0;
                int unitSeq = 0;
                int unitParentSeq = 0;
                // 檢查帳號是否存在
                string existSql = @"Select Count(*) as Count From UserMain Where UserNo =@UserNo";
                SqlCommand existCmd = db.GetCommand(existSql);
                existCmd.Parameters.AddWithValue("@UserNo", UserNo);
                var userData = db.GetDataTable(existCmd);
                existCmd.Parameters.Clear();
                if (userData.Rows[0].Field<int>("Count") > 0)
                {
                    log.Info("error in OauthAddUser 673 Line");
                    return 0;
                }
                else
                {
                    //上層分分署代號對應名稱設定
                    Dictionary<string, string> unitDictionary = new Dictionary<string, string>
                    {
                        { "wra01", "第一河川分署" },
                        { "wra02", "第二河川分署" },
                        { "wra03", "第三河川分署" },
                        { "wra04", "第四河川分署" },
                        { "wra05", "第五河川分署" },
                        { "wra06", "第六河川分署" },
                        { "wra07", "第七河川分署" },
                        { "wra08", "第八河川分署" },
                        { "wra09", "第九河川分署" },
                        { "wra10", "第十河川分署" },
                        { "wratp", "臺北水源特定區管理分署" },
                        { "wranb", "北區水資源分署" },
                        { "wracb", "中區水資源分署" },
                        { "wrasb", "南區水資源分署" },
                        { "wrapi", "水利規劃分署" }
                    };

                    // 檢查上層名稱是否有對應到key
                    if (unitDictionary.TryGetValue(UpperUnit1.ToLower(), out string unitName))
                    {
                        UpperUnit1 = unitName;
                    }

                    //查詢上層單位Seq
                    string sql = @"SELECT * FROM Unit WHERE Name = @Name ";
                    SqlCommand cmd = db.GetCommand(sql);
                    cmd.Parameters.AddWithValue("@Name", Utils.NulltoDBNull(UpperUnit1));
                    DataTable dt1 = db.GetDataTable(cmd);
                    unitParentSeq = Convert.ToInt32(dt1.Rows[0]["Seq"].ToString());
                    cmd.Parameters.Clear();

                    //查詢單位Seq
                    string sql2 = @"SELECT * FROM Unit WHERE Name = @Name AND ParentSeq = @ParentSeq";
                    SqlCommand cmd2 = db.GetCommand(sql2);
                    cmd2.Parameters.AddWithValue("@Name", Utils.NulltoDBNull(Unit));
                    cmd2.Parameters.AddWithValue("@ParentSeq", Utils.NulltoDBNull(unitParentSeq));
                    DataTable dt2 = db.GetDataTable(cmd2);
                    unitSeq = Convert.ToInt32(dt2.Rows[0]["Seq"].ToString());
                    cmd2.Parameters.Clear();

                    // 新增人員資料
                    string sql3 = @"
                    INSERT INTO [UserMain]
                           ([UserNo]
                           ,[PassWord]
                           ,[DisplayName]
                           ,[Email]
                           ,[IsEnabled]
                           ,[CreateTime])
                     VALUES
                           (@UserNo
                           ,@PassWord
                           ,@DisplayName
                           ,@Email
                           ,@IsEnabled
                           ,@CreateTime)";
                    cmd = db.GetCommand(sql3);
                    cmd.Parameters.AddWithValue("@UserNo", UserNo);
                    cmd.Parameters.AddWithValue("@PassWord", "12345");
                    cmd.Parameters.AddWithValue("@DisplayName", Utils.NulltoDBNull(userName) );
                    cmd.Parameters.AddWithValue("@Email", Utils.NulltoDBNull(Mail) );
                    if ((unitParentSeq < 14 && unitParentSeq != 10) || (unitParentSeq >= 19 && unitParentSeq <= 22) || (unitParentSeq == 135) || (unitParentSeq == 136))
                    {
                        cmd.Parameters.AddWithValue("@IsEnabled", false);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@IsEnabled", true);
                    }
                    cmd.Parameters.AddWithValue("@CreateTime", DateTime.Now);
                    db.ExecuteNonQuery(cmd);
                    cmd.Parameters.Clear();
                    //最新一筆Seq
                    string sql4 = @"SELECT IDENT_CURRENT('UserMain') AS Seq";
                    cmd = db.GetCommand(sql4);
                    DataTable dt4 = db.GetDataTable(cmd);
                    userMainSeq = Convert.ToInt32(dt4.Rows[0]["Seq"].ToString());
                    cmd.Parameters.Clear();

                    //新增人員單位
                    string sql5 = @"INSERT INTO [UserUnitPosition]
                                   (UnitSeq 
                                   ,UserMainSeq 
                                   ,IsEnabled 
                                   ,CreateTime)
                                    VALUES
                                   (@UnitSeq
                                   ,@UserMainSeq
                                   ,@IsEnabled
                                   ,@CreateTime)";

                    cmd = db.GetCommand(sql5);
                    cmd.Parameters.AddWithValue("@UnitSeq", unitSeq);
                    cmd.Parameters.AddWithValue("@UserMainSeq", userMainSeq);
                    cmd.Parameters.AddWithValue("@IsEnabled", true);
                    cmd.Parameters.AddWithValue("@CreateTime", DateTime.Now);
                    db.ExecuteNonQuery(cmd);
                    cmd.Parameters.Clear();
                    //
                    string sql6 = @"SELECT IDENT_CURRENT('UserUnitPosition') AS Seq";
                    cmd = db.GetCommand(sql6);
                    DataTable dt6 = db.GetDataTable(cmd);
                    int UserUnitPositionSeq = Convert.ToInt32(dt6.Rows[0]["Seq"].ToString());
                    cmd.Parameters.Clear();
                    //權限
                    string sql7 = @"INSERT INTO UserRole
                                   (UserUnitPositionSeq
                                   ,RoleSeq)
                                    VALUES
                                   (@UserUnitPositionSeq
                                   ,@RoleSeq)";

                    cmd = db.GetCommand(sql7);
                    cmd.Parameters.AddWithValue("@UserUnitPositionSeq", UserUnitPositionSeq);
                    if (unitParentSeq < 14 || (unitParentSeq >= 19 && unitParentSeq <= 22) || (unitParentSeq == 135) || (unitParentSeq == 136))
                    {
                        cmd.Parameters.AddWithValue("@RoleSeq", "2");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@RoleSeq", "20");
                    }
                    db.ExecuteNonQuery(cmd);
                    cmd.Parameters.Clear();
                }

                db.TransactionCommit();

                return 1;
            }
            catch (Exception ex)
            {
                log.Info($"\nStackTrace:{ex.StackTrace}\n Error: {ex.Message}");
                db.TransactionRollback();
                return 0;
            }
        }

        /// <summary> 刪除使用者 </summary>
        /// <param name="Seq"> Seq </param>
        /// <returns></returns>
        internal SaveChangeStatus DeleteUser(int seq)
        {
            SaveChangeStatus saveChangeStatus = new SaveChangeStatus(true, StatusCode.Save);
            string sql = @"
            UPDATE [UserMain]
               SET [IsDelete] = 1                  
             WHERE Seq = @Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);
            db.ExecuteNonQuery(cmd);
            return saveChangeStatus;
        }

        /// <summary> 取得單位(下拉選單資料)  </summary>
        /// <returns></returns>
        internal List<SelectVM> GetPositionList()
        {
            string sql = @"
				SELECT Cast(Seq as varchar(10)) as Value
                , Name as Text
                FROM Position ";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<SelectVM>(cmd);
        }

        /// <summary> 人員資料修改 </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public SaveChangeStatus Update(VUserMain vUserMain)
        {
            db.BeginTransaction();

            try
            {
                SaveChangeStatus saveChangeStatus = new SaveChangeStatus(true, StatusCode.Save);
                string sql = @"
                UPDATE [UserMain]
                   SET [UserNo] = @UserNo
                      ,[PassWord] = @PassWord
                      ,[DisplayName] = @DisplayName
                      ,[TelRegion] = @TelRegion
                      ,[Tel] = @Tel
                      ,[TelExt] = @TelExt
                      ,[Mobile] = @Mobile
                      ,[Email] = @Email
                      ,[IsEnabled] = @IsEnabled
                      ,[ModifyTime] = @ModifyTime
                      ,[ModifyUserSeq] = @ModifyUserSeq
                      WHERE Seq = @Seq 

                 UPDATE  UserUnitPosition 
                      SET  UnitSeq = @UnitSeq 
                          ,PositionSeq = @PositionSeq
                          ,ModifyTime = @ModifyTime 
                          ,ModifyUserSeq = @ModifyUserSeq 
                      WHERE  UserMainSeq = @UserMainSeq 
 
                 UPDATE UserRole 
                      SET RoleSeq = @RoleSeq,
                      RoleSeq2 = @RoleSeq2
                      WHERE UserUnitPositionSeq = 
                      (Select top 1 Seq 
                      From UserUnitPosition
                      Where UserMainSeq = @UserMainSeq)";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", vUserMain.Seq);
                cmd.Parameters.AddWithValue("@UserNo", vUserMain.UserNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PassWord", vUserMain.PassWord);
                cmd.Parameters.AddWithValue("@DisplayName", vUserMain.DisplayName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TelRegion", vUserMain.TelRegion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Tel", vUserMain.Tel ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TelExt", vUserMain.TelExt ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Mobile", vUserMain.Mobile ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", vUserMain.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsEnabled", vUserMain.IsEnabled);
                cmd.Parameters.AddWithValue("@ModifyTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", new SessionManager().GetUser().Seq);
                short? unitSeq = (vUserMain.UnitSeq3 ?? vUserMain.UnitSeq2) ?? vUserMain.UnitSeq1;
                cmd.Parameters.AddWithValue("@UnitSeq", unitSeq);
                cmd.Parameters.AddWithValue("@PositionSeq", vUserMain.PositionSeq == 0 ? (Object)DBNull.Value : vUserMain.PositionSeq);
                cmd.Parameters.AddWithValue("@RoleSeq", vUserMain.RoleSeq);
                cmd.Parameters.AddWithValue("@RoleSeq2", Utils.NulltoDBNull(vUserMain.RoleSeq2) );
                cmd.Parameters.AddWithValue("@UserMainSeq", vUserMain.Seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                saveChangeStatus.Data = vUserMain.Seq;
                return saveChangeStatus;
            }
            catch (Exception ex)
            {
                db.TransactionRollback();
                return new SaveChangeStatus(false, StatusCode.Save, ex);
            }
        }

        public List<UserInfo> GetUserByAccount(string account)
        {
            //shioulo 20210707-1034
            string sql = @"
				SELECT a.[Seq]
                    ,a.[UserNo]                      
                    ,a.[DisplayName]
                    ,a.[TelRegion]
                    ,a.[Tel]
                    ,a.[TelExt]
                    ,a.[Mobile]
                    ,a.[Email]
                    ,a.[IsEnabled]
                    ,a.[IsDelete]
                    ,a.[CreateTime]
                    ,a.[CreateUserSeq]
                    ,a.[ModifyTime]
                    ,a.[ModifyUserSeq]						  
					,ROW_NUMBER() OVER(ORDER BY a.Seq) AS Rows		
					,ISNULL(g.Name,ISNULL(f.Name,c.Name)) as UnitName1
                    ,ISNULL(g.Seq,ISNULL(f.Seq,c.Seq))  as UnitSeq1					  
					,Case When g.Name is null Then (Case When f.name is null Then NULL Else c.name END) ELSE f.Name END  as UnitName2
                    ,Case When g.Seq is null Then (Case When f.Seq is null Then NULL Else c.Seq END) ELSE f.Seq END as UnitSeq2
                    ,c.code UnitCode2 
					,Case When f.Name is null or g.Name is null THEN NULL ELSE c.Name END as UnitName3
                    ,Case When f.Seq is null or g.Seq is null THEN NULL ELSE c.Seq END as UnitSeq3
					,e.Name as RoleName 	
                    ,e.Seq as RoleSeq 
                    ,b.PositionSeq
			FROM UserMain a
			join UserUnitPosition b on a.Seq=b.UserMainSeq
			left join Unit c on b.UnitSeq=c.Seq
			left join UserRole d on b.Seq=d.UserUnitPositionSeq
			left join Role e on d.RoleSeq=e.Seq
            left join Unit f on c.ParentSeq=f.Seq
			left join Unit g on f.ParentSeq=g.Seq
			where a.UserNo=@UserNo and a.IsDelete = 0";
            /*//shioulo 20210521-1046
            string sql = @"
				SELECT b.UnitSeq, a.*
				FROM UserMain a
                left outer join UserUnitPosition b on(b.UserMainSeq=a.Seq)
				WHERE a.UserNo = @UserNo";*/
            /*string sql = @"
				SELECT *
				FROM UserMain
				WHERE UserNo = @UserNo";*/
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("UserNo", account);
            return db.GetDataTableWithClass<UserInfo>(cmd);
        }

        public object GetMobileUserInfo(string userNo)
        {
            List<UserInfo> data = GetUserByAccount(userNo);

            var user = data.First();

            if(user == null)
            {
                return null;
            }
            RoleService roleService = new RoleService();
            List<Role> userRoleList = roleService.GetListByUserSeq(user.Seq);

            return new
            {
                seq = user.Seq,
                name = user.DisplayName,
                phone = user.Mobile,
                userNo = user.UserNo,
                userRoleList = userRoleList,
                unitInfo = $"{user.UnitName1 ?? ""} {user.UnitName2 ?? ""} {user.UnitName3 ?? ""}"
            };

        }


        public UserMain GetUserBySeq(int seq)
        {
            string sql = @"
				SELECT *
				FROM UserMain
				WHERE Seq = @Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<UserMain>(cmd).FirstOrDefault();
        }

        /// <summary>
        /// 取得人員的角色
        /// </summary>
        /// <param name="account">帳號</param>
        /// <returns></returns>
        public List<Role> GetRoleByAccount(string account)
        {
            string sql = @"
				SELECT [Role].*
				FROM [Role]
				JOIN UserRole ON UserRole.RoleSeq = [Role].Seq
				JOIN UserUnitPosition ON UserUnitPosition.Seq = UserRole.UserUnitPositionSeq
				JOIN UserMain ON UserMain.Seq = UserUnitPosition.UserMainSeq
				WHERE UserNo = @UserNo and UserMain.IsDelete = 0";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@UserNo", account);
            return db.GetDataTableWithClass<Role>(cmd);
        }

        /// <summary> 儲存 簽名檔 </summary>
        /// <param name="signatureFileVM"> 簽名檔 ViewModel </param>
        /// <returns> 儲存結果 </returns>
        public SaveChangeStatus SaveSignatureFile(SignatureFileVM signatureFileVM)
        {
            try
            {
                SaveChangeStatus saveChangeStatus = new SaveChangeStatus(true, StatusCode.Save);

                string sql = @"DELETE SignatureFile WHERE UserMainSeq = @UserMainSeq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@UserMainSeq", signatureFileVM.UserMainSeq);
                db.ExecuteNonQuery(cmd);
                cmd.Parameters.Clear();

                sql = @"
				INSERT INTO SignatureFile (
					UserMainSeq
					,DisplayFileName
					,FileName
					,FilePath
					)
				VALUES (
					@UserMainSeq
					,@DisplayFileName
					,@FileName
					,@FilePath
					)";
                cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@UserMainSeq", signatureFileVM.UserMainSeq);
                cmd.Parameters.AddWithValue("@DisplayFileName", signatureFileVM.DisplayFileName);
                cmd.Parameters.AddWithValue("@FileName", signatureFileVM.FileName);
                cmd.Parameters.AddWithValue("@FilePath", signatureFileVM.FilePath);
                db.ExecuteNonQuery(cmd);
                return saveChangeStatus;
            }
            catch (Exception ex)
            {
                db.TransactionRollback();
                return new SaveChangeStatus(false, StatusCode.Save, ex);
            }
        }

        /// <summary> 取得 簽名檔  </summary>
        /// <param name="userSeq"> UserMain.Seq </param>
        /// <returns></returns>
        public List<SignatureFileVM> GetSignatureFileByUserSeq(int userSeq)
        {
            string sql = @"
				SELECT *
                FROM SignatureFile WHERE UserMainSeq = @UserMainSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@UserMainSeq", userSeq);
            return db.GetDataTableWithClass<SignatureFileVM>(cmd);
        }

        /// <summary> AD登入人員資料修改 </summary>
        /// <returns></returns>
        public SaveChangeStatus OauthUpdateUserData(int Seq,string Mail,string Name,string Phone,string Unit, string UpperUnit1, string UpperUnit2)
        {
            db.BeginTransaction();

            try
            {
                //上層分分署代號對應名稱設定
                Dictionary<string, string> unitDictionary = new Dictionary<string, string>
                {
                    { "wra01", "第一河川分署" },
                    { "wra02", "第二河川分署" },
                    { "wra03", "第三河川分署" },
                    { "wra04", "第四河川分署" },
                    { "wra05", "第五河川分署" },
                    { "wra06", "第六河川分署" },
                    { "wra07", "第七河川分署" },
                    { "wra08", "第八河川分署" },
                    { "wra09", "第九河川分署" },
                    { "wra10", "第十河川分署" },
                    { "wratp", "臺北水源特定區管理分署" },
                    { "wranb", "北區水資源分署" },
                    { "wracb", "中區水資源分署" },
                    { "wrasb", "南區水資源分署" },
                    { "wrapi", "水利規劃分署" }
                };

                // 檢查上層名稱是否有對應到key
                if (unitDictionary.TryGetValue(UpperUnit1.ToLower(), out string unitName))
                {
                    UpperUnit1 = unitName;
                }

                //查詢上層單位Seq
                string sql = @"SELECT * FROM Unit WHERE Name = @Name ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Name", Utils.NulltoDBNull(UpperUnit1));
                DataTable dt1 = db.GetDataTable(cmd);
                var UpperUnitSeq = Convert.ToInt32(dt1.Rows[0]["Seq"].ToString());
                cmd.Parameters.Clear();

                //查詢單位Seq
                string sql2 = @"SELECT * FROM Unit WHERE Name = @Name AND ParentSeq = @ParentSeq";
                SqlCommand cmd2 = db.GetCommand(sql2);
                cmd2.Parameters.AddWithValue("@Name", Utils.NulltoDBNull(Unit));
                cmd2.Parameters.AddWithValue("@ParentSeq", Utils.NulltoDBNull(UpperUnitSeq));
                DataTable dt2 = db.GetDataTable(cmd2);
                var UnitSeq = Convert.ToInt32(dt2.Rows[0]["Seq"].ToString());
                cmd2.Parameters.Clear();

                //寫入更新
                SaveChangeStatus saveChangeStatus = new SaveChangeStatus(true, StatusCode.Save);
                string sql3 = @"
                UPDATE [UserMain]
                   SET [DisplayName] = @DisplayName
                      ,[Mobile] = @Mobile
                      ,[Email] = @Email
                      ,[ModifyTime] = @ModifyTime
                      ,[ModifyUserSeq] = @ModifyUserSeq
                      WHERE Seq = @Seq 

                 UPDATE  UserUnitPosition 
                      SET  UnitSeq = @UnitSeq 
                          ,ModifyTime = @ModifyTime 
                          ,ModifyUserSeq = @ModifyUserSeq 
                      WHERE  UserMainSeq = @UserMainSeq";

                SqlCommand cmd3 = db.GetCommand(sql3);
                cmd3.Parameters.AddWithValue("@Seq", Seq);
                cmd3.Parameters.AddWithValue("@DisplayName", Name);
                cmd3.Parameters.AddWithValue("@Mobile", Phone);
                cmd3.Parameters.AddWithValue("@Email", Mail);
                cmd3.Parameters.AddWithValue("@ModifyTime", DateTime.Now);
                cmd3.Parameters.AddWithValue("@ModifyUserSeq", new SessionManager().GetUser().Seq);
                cmd3.Parameters.AddWithValue("@UnitSeq", UnitSeq);
                cmd3.Parameters.AddWithValue("@UserMainSeq", Seq);
                db.ExecuteNonQuery(cmd3);

                db.TransactionCommit();
                saveChangeStatus.Data = Seq;
                return saveChangeStatus;
            }
            catch (Exception ex)
            {
                db.TransactionRollback();
                return new SaveChangeStatus(false, StatusCode.Save, ex);
            }
        }
    }
}
