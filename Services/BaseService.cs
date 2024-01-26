using EQC.Common;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EQC.Services
{
    public class BaseService 
    {
        protected DBConn db = new DBConn();
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger("Service");

        public void Close()
        {
            db.ConnectionClose();
        }

        public int getUserSeq()
        {
            return Utils.getUserSeq();// new SessionManager().GetUser().Seq;
        }

        /// <summary>
        /// null 轉成 DBNull
        /// </summary>
        /// <param name="source"></param>
        public object NulltoDBNull(object value)
        {
            return Utils.NulltoDBNull(value);
            /*if (value==null)
                return DBNull.Value;
            else
                return value;*/
        }
        public string NulltoEmpty(string value)
        {
            return Utils.NulltoEmpty(value);
        }

        /// <summary>
        /// 將物件中字串屬性為null的值轉成string.Empty
        /// </summary>
        /// <param name="source"></param>
        public void Null2Empty(object source)
        {
            Utils.Null2Empty(source);
            /*//設定其他欄位為空字串
            foreach (PropertyInfo prop in source.GetType().GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    string propValue = prop.GetValue(source, null) as string;
                    if (propValue == null)
                        prop.SetValue(source, string.Empty, null);
                }
            }*/
        }
        public DateTime getLastUpdateTime(string tableName)
        {
            string sql = @"Select Max(ModifyTime) as LastUpdate from " + tableName;

            return (DateTime)db.ExecuteScalar(db.GetCommand(sql));

        }
        public List<object> getFields(string[,] excelHeaderMap)
        {
            int rowNum = excelHeaderMap.GetLength(0);
            List<object> list = new List<object>();
            for (int i = 0; i < rowNum; i++)
            {
                list.Add(new { fieldName = excelHeaderMap[i, 0], fieldShowName = excelHeaderMap[i, 1] });
            }

            return list;
        }
        public string ROCDateStrHandler(string rocStr)
        {
            if (rocStr == null || rocStr.Length < 7) return null;
            int rocYear = Int32.Parse (rocStr.Substring(0, 3) );
            return (rocYear + 1911).ToString() + "-" + rocStr.Substring(3, 2) + "-" + rocStr.Substring(5, 2); 
        }
        public string EngDateTime(DateTime? dt)
        {
            if (dt.HasValue)
            {
                DateTime tar = dt.Value;
                return String.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}", tar.Year, tar.Month, tar.Day, tar.Hour, tar.Minute);
            }
            else
            {
                return null;
            }
        }
        //public List<T> getKeyWordData<T>(List<T> data, string[] columns, string keyWord)
        //{
        //    List<T> result = new List<T>();
        //    foreach (string col in columns)

        //    {

        //        PropertyInfo prop = typeof(T).GetProperty(col);
        //        if (prop == null) continue;
        //        foreach (T row in data)
        //        {


        //            string value = prop.GetValue(row, null).ToString();

        //            if (value.Contains(keyWord))
        //            {
        //                result.Add(row);
        //            }

        //        }
        //    }
        //    foreach (string col in columns)
        //    {
        //        result.Sort(delegate (T x, T y)
        //        {

        //            string valueX = typeof(T).GetProperty(col).GetValue(x).ToString();
        //            string valueY = typeof(T).GetProperty(col).GetValue(y).ToString();

        //            //if (valueX == null && y.PartName == null) return 0;
        //            //else if (x.PartName == null) return -1;
        //            //else if (y.PartName == null) return 1;
        //            return valueX.IndexOf(keyWord) - valueY.IndexOf(keyWord);
        //        });
        //    }


        //    return result;

        //}
    }
}