using EQC.Common;
using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class SuperviseCommitteeService : BaseService
    {//委員督導
        //改善前照片
        public List<T> GetChapterPhotos<T>(int engMainSeq)
        {
            string sql = @"
                SELECT
                    UniqueFileName fName,
                    Description Memo
                FROM EngAttachment
                where EngMainSeq=@engMainSeq
                and Chapter=1
                and FileType=1
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@engMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //更新API註解
        public bool UpdateCheckPhoto(ESCommitteePhotoVModel m)
        {
            Null2Empty(m);
            string sql = @"
                update ConstCheckRecFile set 
                    RESTful = @RESTful,
                    ModifyTime = GetDate(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
            try { 
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@RESTful", m.RESTful);

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SuperviseCommitteeService.UpdateCheckPhoto: " + e.Message);
                log.Info(sql);
                return false;
            }
}
        //改善前照片
        public List<T> GetCheckPhotos<T>(int engMainSeq)
        {
            string sql = @"
				SELECT
                    e1.CCRCheckType1,
                    e2.Seq,
                    e2.Memo,
                    e2.RESTful,
                    e2.UniqueFileName fName
                FROM EngMain a
                inner join EngConstruction f on(f.EngMainSeq=a.Seq)
                inner join ConstCheckRec e1 on(e1.EngConstructionSeq=f.Seq)
 		        inner join ConstCheckRecFile e2 on(e2.ConstCheckRecSeq=e1.Seq)
                where a.Seq=@engMainSeq
                order by e1.CCRCheckType1
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@engMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //其他抽查成果
        public List<T> OtherCheck<T>(int engMainSeq)
        {
            string sql = @"
				select count(z1.OrderNo) totalRec,
	                sum(IIF(missingCount=0, 1, 0)) okCount,
	                z1.checkName	
                from (
					select z.CCRCheckType1, z.OrderNo, z.checkName,
                        sum(okCount) okCount, SUM(missingCount) missingCount,
                        z.CCRPosDesc
                    from (	
                    	--設備運轉測試清單				
                        SELECT
                        	e1.CCRCheckType1,
                            e3.ItemName checkName,
                            e3.OrderNo,
                            1 okCount,
                            0 missingCount,
                            e1.CCRPosDesc
                        FROM EngMain a
                        inner join EngConstruction f on(f.EngMainSeq=a.Seq)
                        inner join ConstCheckRec e1 on(e1.EngConstructionSeq=f.Seq and e1.CCRCheckType1=2)
 		                inner join ConstCheckRecResult e2 on(e2.ConstCheckRecSeq=e1.Seq and e2.ResultItem=1 and e2.CCRCheckResult=1)
                        inner join EquOperTestList e3 on(e3.Seq=e1.ItemSeq)
                        where a.Seq=@engMainSeq
                    
                        union all
                    
                        SELECT
                        	e1.CCRCheckType1,
                            e3.ItemName checkName,
                            e3.OrderNo,
                            0 okCount,
                            1 missingCount,
                            e1.CCRPosDesc
                        FROM EngMain a
                        inner join EngConstruction f on(f.EngMainSeq=a.Seq)
                        inner join ConstCheckRec e1 on(e1.EngConstructionSeq=f.Seq and e1.CCRCheckType1=2)
 		                inner join ConstCheckRecResult e2 on(e2.ConstCheckRecSeq=e1.Seq and e2.ResultItem=1 and e2.CCRCheckResult=2)
                        inner join EquOperTestList e3 on(e3.Seq=e1.ItemSeq)
                        where a.Seq=@engMainSeq
                        
                        --職業安全衛生清單
                        union all
                        				
                        SELECT
                        	e1.CCRCheckType1,
                            e3.ItemName checkName,
                            e3.OrderNo,
                            1 okCount,
                            0 missingCount,
                            e1.CCRPosDesc
                        FROM EngMain a
                        inner join EngConstruction f on(f.EngMainSeq=a.Seq)
                        inner join ConstCheckRec e1 on(e1.EngConstructionSeq=f.Seq and e1.CCRCheckType1=3)
 		                inner join ConstCheckRecResult e2 on(e2.ConstCheckRecSeq=e1.Seq and e2.ResultItem=1 and e2.CCRCheckResult=1)
                        inner join OccuSafeHealthList e3 on(e3.Seq=e1.ItemSeq)
                        where a.Seq=@engMainSeq
                    
                        union all
                    
                        SELECT
                        	e1.CCRCheckType1,
                            e3.ItemName checkName,
                            e3.OrderNo,
                            0 okCount,
                            1 missingCount,
                            e1.CCRPosDesc
                        FROM EngMain a
                        inner join EngConstruction f on(f.EngMainSeq=a.Seq)
                        inner join ConstCheckRec e1 on(e1.EngConstructionSeq=f.Seq and e1.CCRCheckType1=3)
 		                inner join ConstCheckRecResult e2 on(e2.ConstCheckRecSeq=e1.Seq and e2.ResultItem=1 and e2.CCRCheckResult=2)
                        inner join OccuSafeHealthList e3 on(e3.Seq=e1.ItemSeq)
                        where a.Seq=@engMainSeq
                        
                        --環境保育清單
                        union all
                        				
                        SELECT
                        	e1.CCRCheckType1,
                            e3.ItemName checkName,
                            e3.OrderNo,
                            1 okCount,
                            0 missingCount,
                            e1.CCRPosDesc
                        FROM EngMain a
                        inner join EngConstruction f on(f.EngMainSeq=a.Seq)
                        inner join ConstCheckRec e1 on(e1.EngConstructionSeq=f.Seq and e1.CCRCheckType1=4)
 		                inner join ConstCheckRecResult e2 on(e2.ConstCheckRecSeq=e1.Seq and e2.ResultItem=1 and e2.CCRCheckResult=1)
                        inner join EnvirConsList e3 on(e3.Seq=e1.ItemSeq)
                        where a.Seq=@engMainSeq
                    
                        union all
                    
                        SELECT
                        	e1.CCRCheckType1,
                            e3.ItemName checkName,
                            e3.OrderNo,
                            0 okCount,
                            1 missingCount,
                            e1.CCRPosDesc
                        FROM EngMain a
                        inner join EngConstruction f on(f.EngMainSeq=a.Seq)
                        inner join ConstCheckRec e1 on(e1.EngConstructionSeq=f.Seq and e1.CCRCheckType1=4)
 		                inner join ConstCheckRecResult e2 on(e2.ConstCheckRecSeq=e1.Seq and e2.ResultItem=1 and e2.CCRCheckResult=2)
                        inner join EnvirConsList e3 on(e3.Seq=e1.ItemSeq)
                        where a.Seq=@engMainSeq
                    ) z
                    group by z.CCRCheckType1,z.OrderNo,z.checkName,z.CCRPosDesc
                ) z1
                group by z1.CCRCheckType1, z1.OrderNo,z1.checkName
                order by z1.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@engMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //施工抽查成果
        public List<T> ConstructionCheck<T>(int engMainSeq)
        {
            string sql = @"
                select count(z1.OrderNo) totalRec,
	                sum(IIF(missingCount=0, 1, 0)) okCount,
	                z1.checkName	
                from (
					select z.OrderNo, z.checkName,
                        sum(okCount) okCount, SUM(missingCount) missingCount,
                        z.CCRPosDesc
                    from (					
                        SELECT
                            e3.ItemName checkName,
                            e3.OrderNo,
                            1 okCount,
                            0 missingCount,
                            e1.CCRPosDesc
                        FROM EngMain a
                        inner join EngConstruction f on(f.EngMainSeq=a.Seq)
                        inner join ConstCheckRec e1 on(e1.EngConstructionSeq=f.Seq and e1.CCRCheckType1=1)
 		                inner join ConstCheckRecResult e2 on(e2.ConstCheckRecSeq=e1.Seq and e2.ResultItem=1 and e2.CCRCheckResult=1)
                        inner join ConstCheckList e3 on(e3.Seq=e1.ItemSeq)
                    
                        where a.Seq=@engMainSeq
                    
                        union all
                    
                        SELECT
                            e3.ItemName checkName,
                            e3.OrderNo,
                            0 okCount,
                            1 missingCount,
                            e1.CCRPosDesc
                        FROM EngMain a
                        inner join EngConstruction f on(f.EngMainSeq=a.Seq)
                        inner join ConstCheckRec e1 on(e1.EngConstructionSeq=f.Seq and e1.CCRCheckType1=1)
 		                inner join ConstCheckRecResult e2 on(e2.ConstCheckRecSeq=e1.Seq and e2.ResultItem=1 and e2.CCRCheckResult=2)
                        inner join ConstCheckList e3 on(e3.Seq=e1.ItemSeq)
                    
                        where a.Seq=@engMainSeq
                    ) z
                    group by z.OrderNo,z.checkName,z.CCRPosDesc
                ) z1
                group by z1.OrderNo,z1.checkName
                order by z1.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@engMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //標案
        public List<SelectIntOptionModel> GetTenderOptions(string superviseDate, string execUnit)
        {
            string sql = @"
                select
                    z.TenderName Text,
                    z.Seq Value
                from (
                    select DISTINCT c.Seq, b.TenderName    
                    from SuperviseEng a
                    inner join PrjXML b on(b.Seq=a.PrjXMLSeq and b.ExecUnitName=@ExecUnitName)
                    inner join EngMain c on(c.PrjXMLSeq=b.Seq)
                    where a.SuperviseDate = @SuperviseDate
                    " + getAuthority() + @"
                ) z
                order by z.TenderName desc
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SuperviseDate", superviseDate);
            cmd.Parameters.AddWithValue("@ExecUnitName", execUnit);
            

            return db.GetDataTableWithClass<SelectIntOptionModel>(cmd);
        }
        //執行機關
        public List<SelectOptionModel> GetExecUnitOptions(string superviseDate)
        {
            string sql = @"
                select
                    z.ExecUnitName Text,
                    z.ExecUnitName Value
                from (
                    select DISTINCT b.ExecUnitName    
                    from SuperviseEng a
                    inner join PrjXML b on(b.Seq=a.PrjXMLSeq)
                    inner join EngMain c on(c.PrjXMLSeq=b.Seq)
                    where a.SuperviseDate = @SuperviseDate
                    " + getAuthority() + @"
                ) z
                order by z.ExecUnitName desc
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SuperviseDate", superviseDate);

            return db.GetDataTableWithClass<SelectOptionModel>(cmd);
        }
        //督導日期
        public List<SelectOptionModel> GetSuperviseDateOptions()
        {
            string sql = @"
                select
                    CONVERT(char(10), z.SuperviseDate,111) Text,
                    CONVERT(char(10), z.SuperviseDate,111) Value
                from (
                    select DISTINCT a.SuperviseDate
                    from SuperviseEng a
                    inner join PrjXML b on(b.Seq=a.PrjXMLSeq)
                    inner join EngMain c on(c.PrjXMLSeq=b.Seq)
                    where a.SuperviseDate >= DATEADD(day, -6, GetDate()) 
                    and a.SuperviseDate <= Cast(GetDate() as DATE)
                    " + getAuthority() + @"
                ) z
                order by z.SuperviseDate desc
				";
            SqlCommand cmd = db.GetCommand(sql);

            return db.GetDataTableWithClass<SelectOptionModel>(cmd);
        }
        //標案權限管控
        public string getAuthority()
        {
            string sql = " and (1=0)";
            System.Web.SessionState.HttpSessionState _session = System.Web.HttpContext.Current.Session;
            if (_session["UserInfo"] == null) return sql;

            UserInfo userInfo = (UserInfo)_session["UserInfo"];
            if (userInfo.UnitSeq1 == null && userInfo.UnitSeq2 == null) return sql;

            if (userInfo.IsAdmin) return " ";//系統管理者

            //
            List<Role> roles = userInfo.Role;
            if (roles.Count == 0) return sql;

            int roleSeq = roles[0].Seq;

            if (roleSeq == ConfigManager.DepartmentAdmin_RoleSeq) //署管理者
            {
                sql = " ";
            }
            else if (roleSeq == ConfigManager.DepartmentUser_RoleSeq) //署使用者
            {
                if (userInfo.UnitName1 == "水利署")
                {
                    sql = " ";
                }
                else
                {//內聘督導委員
                    sql = String.Format(@"
                        and (
                            SELECT STUFF(
                            (SELECT ',' + z.cName
                            FROM (
                                select zb.DisplayName cName, za.OrderNo from InsideCommittee za
                                inner join UserMain zb on(zb.Seq=za.UserMainSeq)
                                where za.SuperviseEngSeq=a.Seq
                            ) z
                            order by z.OrderNo
                            FOR XML PATH('')) ,1,1,'')
                        ) like '%{0}%'
                        ", userInfo.DisplayName);
                }
            }
            else if (roleSeq == ConfigManager.Committee_RoleSeq) //外聘督導委員
            {
                if (!String.IsNullOrEmpty(userInfo.DisplayName))
                {
                    sql = String.Format(@"
                        and (
                            (
                                SELECT STUFF(
                                (SELECT ',' + z.cName FROM (
                                    select zb.ECName cName, za.OrderNo from OutCommittee za
                                    inner join ExpertCommittee zb on(zb.Seq=za.ExpertCommitteeSeq)
                                    where za.SuperviseEngSeq=a.Seq
                                 ) z
                                FOR XML PATH('')) ,1,1,'')
                            ) like '%{0}%'
                            or
                            (
                                SELECT STUFF(
                                (SELECT ',' + z.cName
                                FROM (
                                    select zb.DisplayName cName, za.OrderNo from InsideCommittee za
                                    inner join UserMain zb on(zb.Seq=za.UserMainSeq)
                                    where za.SuperviseEngSeq=a.Seq
                                ) z
                                --order by z.OrderNo
                                FOR XML PATH('')) ,1,1,'')
                            ) like '%{0}%'
                         )
                        ", userInfo.DisplayName);
                }
            }

            return sql;
        }
        //工程已連接PrjXML標案
        public List<T> GetEngLinkTenderBySeq<T>(int seq)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    a.EngNo,
                    a.EngName,
                    a.EngPeriod,
                    ISNULL(dbo.ChtDate2Date(e.ActualStartDate), a.StartDate) StartDate, --s20220810
                    a.SchCompDate,
                    a.PrjXMLSeq,
                    a.SupervisorContact,
                    -- d.DocState,
                    e.TenderNo,
                    e.ExecUnitName,
                    e.TenderName,
                    e.DurationCategory,
                    e.ActualStartDate
                FROM EngMain a
                inner join PrjXML e on(e.Seq=a.PrjXMLSeq)
                /*left outer join SupervisionProjectList d on(
                    d.EngMainSeq=a.Seq
                    and d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                )*/
                where a.Seq=@Seq
                and a.Seq in (
                    select c.Seq
                    from SuperviseEng a
                    inner join PrjXML b on(b.Seq=a.PrjXMLSeq)
                    inner join EngMain c on(c.PrjXMLSeq=b.Seq)
                    where a.SuperviseDate >= DATEADD(day, -6, GetDate()) 
                    and a.SuperviseDate <= Cast(GetDate() as DATE)
                    " + getAuthority() + @"
                )
                ";
                
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}