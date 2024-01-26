using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using EQC.Models;

namespace EQC.Services
{
    public class CourtVerdictService : BaseService
    {

        public class CourtVerdictVM : CourtVerdict
        {
            public string JText
            {
                get
                {
                    if(JFull.Length > 30)
                        return JFull
                            .Replace(" ", String.Empty)
                            .Replace("\r\n", String.Empty)
                            .Substring(0, 30) + "...";
                    return JFull;
                }
            }
        }
        private string[,] headerMap = new string[,]
        {
            {"JID", "判決書ID" },
            {"JYear", "年度" },
            {"JCase", "字號" },
            {"JNo", "號次" },
            {"JDate", "審判日" },
            {"JTitle", "案由" },
            {"JFull", "全文" },
            {"JText", "全文" }
        };


        private string sqlCoditionSwitch(int _type)
        {
            switch(_type)
            {

                case 1 : return "JTitle"; break;
                case 2 : return "JFull"; break;
                default: return "''";break;
            }
            
        } 
       public List<CourtVerdictVM> GetList(int page, int per_page, SearchCodition coditions)
       {
            string coditionCol = sqlCoditionSwitch(coditions._type);
            string sql = @"
                Select
                JID,
                JYear,
                JCase,
                JNo,
                JDate,
                JTitle,
                JFull
                from CourtVerdict
                where "+ coditionCol + @" Like @searchStr
				ORDER BY CASE @Sort_by
					WHEN 'Seq'
					THEN Seq
					END
				OFFSET @Page ROWS
				FETCH FIRST @Per_page ROWS ONLY";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Sort_by", "Seq");
            cmd.Parameters.AddWithValue("@Page", page * per_page);
            cmd.Parameters.AddWithValue("@Per_page", per_page);
            cmd.Parameters.AddWithValue("@searchStr", "%"+coditions.Text+ "%"  ?? "");
            return db.GetDataTableWithClass<CourtVerdictVM>(cmd);
       }
        public Object GetCount()
        {
            string sql = @"
				SELECT COUNT(*)
				FROM CourtVerdict";
            SqlCommand cmd = db.GetCommand(sql);
            return db.ExecuteScalar(cmd);
        }
        public List<object> getFields()
        {
            return getFields(headerMap);
        }


    }
}