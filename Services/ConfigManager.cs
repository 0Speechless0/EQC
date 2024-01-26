using System;
using System.Configuration;

namespace EQC.Services
{
    public class ConfigManager
    {
        /// <summary>外聘委員 上層單位 seq 
        /// shioulo 20220812
        /// </summary>
        public static Int16 OutCommitteeUnit_UnitParentSeq
        {
            get
            {
                try
                {
                    return Convert.ToInt16(ConfigurationManager.AppSettings["OutCommitteeUnit_UnitParentSeq"].ToString());
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }
        /// <summary>委外設計 role seq
        /// </summary>
        public static byte Committee_RoleSeq
        {//shioulo 20220511
            get
            {
                try
                {
                    return Convert.ToByte(ConfigurationManager.AppSettings["Committee_RoleSeq"].ToString());
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }
        /// <summary>設計單位 上層單位 seq 
        /// shioulo 20220706
        /// </summary>
        public static Int16 DesignUnit_UnitParentSeq
        {
            get
            {
                try
                {
                    return Convert.ToInt16(ConfigurationManager.AppSettings["DesignUnit_UnitParentSeq"].ToString());
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }
        /// <summary>委外設計 role seq
        /// </summary>
        public static byte OutsourceDesign_RoleSeq
        {//shioulo 20220511
            get
            {
                try
                {
                    return Convert.ToByte(ConfigurationManager.AppSettings["OutsourceDesign_RoleSeq"].ToString());
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }
        /// <summary>署管理者 role seq
        /// </summary>
        public static byte DepartmentAdmin_RoleSeq
        {
            get
            {
                try
                {
                    return Convert.ToByte(ConfigurationManager.AppSettings["DepartmentAdmin_RoleSeq"].ToString());
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }
        /// <summary>署使用者 role seq
        /// </summary>
        public static byte DepartmentUser_RoleSeq
        {
            get
            {
                try
                {
                    return Convert.ToByte(ConfigurationManager.AppSettings["DepartmentUser_RoleSeq"].ToString());
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }
        /// <summary>施工廠商 上層單位 seq
        /// </summary>
        public static Int16 BuildContractor_UnitParentSeq
        {
            get
            {
                try
                {
                    return Convert.ToInt16(ConfigurationManager.AppSettings["BuildContractor_UnitParentSeq"].ToString());
                }catch(Exception)
                {
                    return 0;
                }
            }
        }
        /// <summary>施工廠商 role seq
        /// </summary>
        public static byte BuildContractor_RoleSeq
        {
            get
            {
                try
                {
                    return Convert.ToByte(ConfigurationManager.AppSettings["BuildContractor_RoleSeq"].ToString());
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }
        /// <summary>監造單位 上層單位 seq
        /// </summary>
        public static Int16 SupervisorUnit_UnitParentSeq
        {
            get
            {
                try
                {
                    return Convert.ToInt16(ConfigurationManager.AppSettings["SupervisorUnit_UnitParentSeq"].ToString());
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }
        /// <summary>監造單位 role seq
        /// </summary>
        public static byte SupervisorUnit_RoleSeq
        {
            get
            {
                try
                {
                    return Convert.ToByte(ConfigurationManager.AppSettings["SupervisorUnit_RoleSeq"].ToString());
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }
        /// <summary>mail address
        /// </summary>
        public static string MailAdress
        {
            get
            {
                return ConfigurationManager.AppSettings["MailAdress"].ToString();
            }
        }
        /// <summary>SMTP Host
        /// </summary>
        public static string SMTP
        {
            get
            {
                return ConfigurationManager.AppSettings["SMTP"].ToString();
            }
        }
        /// <summary>Mail server port
        /// </summary>
        public static int MailPort
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["MailPort"].ToString());
            }
        }
        /// <summary>email 密碼
        /// </summary>
        public static string MailPd
        {
            get
            {
                return ConfigurationManager.AppSettings["MailPd"].ToString();
            }
        }
        /// <summary>email EnableSsl
        /// </summary>
        public static bool EnableSsl
        {
            get
            {
                return ConfigurationManager.AppSettings["EnableSsl"].ToString()=="Y";
            }
        }
        /// <summary>email UseDefaultCredentials
        /// </summary>
        public static bool UseDefaultCredentials
        {
            get
            {
                return ConfigurationManager.AppSettings["UseDefaultCredentials"].ToString() == "Y";
            }
        }
        /// <summary>停用登入email驗證碼 shioulo 20220707
        /// </summary>
        public static bool LoingEmailCodeDisabled
        {
            get
            {
                return ConfigurationManager.AppSettings["LoingEmailCodeDisabled"].ToString() == "Y";
            }
        }
        /// <summary>ICDF_Accounting_ConnString
        /// </summary>
        public static string EQC_ConnString
        {
            get
            {
                try
                {
                    return ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>入口網
        /// </summary>
        public static string PortalSite
        {
            get
            {
                return ConfigurationManager.AppSettings["PortalSite"].ToString();
            }
        }

        /// <summary>入口網 localhost
        /// </summary>
        public static string PortalSiteLocalhostPort
        {
            get
            {
                return ConfigurationManager.AppSettings["PortalSiteLocalhostPort"].ToString();
            }
        }

        /// <summary>DefaultDeadLine
        /// </summary>
        public static int DefaultDeadLine
        {
            get
            {
                if (ConfigurationManager.AppSettings["DefaultDeadLine"] == null)
                    return 0;
                else 
                    return Convert.ToInt32(ConfigurationManager.AppSettings["DefaultDeadLine"].ToString());
            }
        }

        public static string EngAPIToken
        {
            get
            {
                return ConfigurationManager.AppSettings["EngAPIToken"] ?? "77771234";
            }
        }
    }
}