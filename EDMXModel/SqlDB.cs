using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace EQC.Services
{

    //要從sql字串追蹤變更有困難，先保留
    public partial class SqlDB
    {

        StringBuilder changeText = new StringBuilder();
        StringBuilder changeTable = new StringBuilder();
        public static Action<string, string> changeTextSetter;

        private string GetTableName(Regex rx, string sql)
        {
            return
                rx.Matches(sql).Cast<Match>()
                .FirstOrDefault()?
                .Groups["tableName"]
                .Value;
        }
        public void Record(SqlCommand cmd)
        {
            try
            {
                Regex rx = null;
                string sql = cmd.CommandText.Trim();
                sql = sql.Replace("[", String.Empty);
                sql = sql.Replace("]", String.Empty);
                sql = sql.Replace("/r/n", String.Empty);
                int wherePosition = sql.ToLower().IndexOf("where");
                string sqlWhere = "";
                if (wherePosition > 0)
                {
                    sqlWhere = sql.Substring(wherePosition);
                    sql = sql.Substring(0, wherePosition);
                }

                rx = new Regex(@"(?<column>\w+)\s?=\s?(?<value>@\w+)");

                foreach (Match match in rx.Matches(sqlWhere))
                {
                    var where_paramter = match.Groups["value"].Value;
                    var where_value = cmd.Parameters[where_paramter].Value.ToString();
                    sqlWhere = sqlWhere.Replace(where_paramter, where_value);
                }

                //MatchCollection conditionMatches = rx?.Matches(sqlWhere);
                var findDeletedTableRx = new Regex(@"delete\s+from\s+(?<tableName>\w+)", RegexOptions.IgnoreCase);
                if (findDeletedTableRx.IsMatch(sql))
                {
                    string changeTableStr = GetTableName(
                        findDeletedTableRx, sql);
                    if (!changeTable.ToString().Contains(changeTableStr + ","))
                        changeTable.Append(changeTableStr + ",");

                    changeText.AppendLine($"\t=== 刪除{changeTableStr} 資料 ======");
                    changeText.Append("條件:");
                    changeText.Append(sqlWhere);


                }
                var findInsertedTableRx = new Regex(@"insert\s+(into)?\s+(?<tableName>\w+)\s?\(", RegexOptions.IgnoreCase);
                if (findInsertedTableRx.IsMatch(sql))
                {
                    string changeTableStr = GetTableName(
                        findInsertedTableRx, sql);
                    if (!changeTable.ToString().Contains(changeTableStr + ","))
                        changeTable.Append(changeTableStr + ",");

                    changeText.AppendLine($"\t === 新增 {changeTableStr} 資料 ======");

                    rx = new Regex(@"[\s?](?<value>@\w+),");
                    var paramterMatches = rx.Matches(sql);

                    //rx = new Regex(@"(?<column>\w+)[,\u0029]\s?");
                    //var columnMatches = rx.Matches(sql);

                    int i = 0;
                    foreach (Match match in paramterMatches)
                    {
                        var key = match.Groups["value"].Value;
                        var currentValue = cmd.Parameters[key].Value;

                        changeText.AppendLine($"\t{key}[{currentValue}],");
                        i++;
                    }

                }

                var findUpdatedTableRx = new Regex(@"update\s+(?<tableName>\w+)\s+set", RegexOptions.IgnoreCase);
                if (findUpdatedTableRx.IsMatch(sql))
                {

                    string changeTableStr = GetTableName(
                        findUpdatedTableRx, sql);

                    if (!changeTable.ToString().Contains(changeTableStr + ","))
                        changeTable.Append(changeTableStr + ",");

                    changeText.AppendLine($"\t === 更新 {changeTableStr} 資料 ======");


                    MatchCollection matches = rx?.Matches(sql);
                    StringBuilder tempString = new StringBuilder();
                    changeText.AppendLine("條件:" + sqlWhere);
                    //foreach (Match match in conditionMatches)
                    //{
                    //    var conditionValue = cmd.Parameters[match.Groups["value"].Value].Value;
                    //    tempString.Append($"{match.Groups["column"].Value}[{conditionValue}],");


                    //}

                    changeText.AppendLine("資料:");
                    foreach (Match match in matches)
                    {
                        var currentValue = cmd.Parameters[match.Groups["value"].Value].Value;
                        changeText.AppendLine($"\t{match.Groups["value"].Value}[{currentValue }],");
                    }




                }
                changeTextSetter?.Invoke(changeText.ToString(), changeTable.ToString());

            }
            catch(Exception e)
            {
               BaseService.log.Info("API Record Error: " + e.Message+ "," + e.StackTrace );
            }


        }
    }
}