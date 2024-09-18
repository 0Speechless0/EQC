using EQC.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace EQC.EDMXModel
{
    public partial class PackageDownloadAction
    {
        public int? createUserSeq { get; set; }
        public bool hasDateInput
        {
            get
            {
                if (MainInjection == null) return false;
                var arr = MainInjection.Split(',');
                if (arr.Length < 2) return false;
                if (arr.Length > 3)
                    if (arr[1].Length == 0 || arr[2].Length == 0)
                        return false;

                return arr[1].Length > 0 || arr[2].Length > 0;
            }
        }


        public List<string[]> relateSqlParamter
        {

            get
            {
                var result = new List<string[]>();
                if (RelatedTable == null || RelatedInjection == null)
                    return result;
                int i = 0;
                var a1 = RelatedTable.Split(',');
                
                var a2 = RelatedInjection.Split(',');

                while(i < a1.Length && i < a2.Length)
                {
                    var a1_1 = a1[i].Split(':');
                    if(a1_1.Length == 3 ) result.Add(new string[4] { a1_1[0], a1_1[1], a1_1[2], a2[i] });
                    i++;
                }
                return result;
                    
            }
        }

        public bool inGroup { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public DateTime CreateTime { get; set; }
        public string PackageDistPath { get; set; }

        public int FileCount(DBConn db, object engSeq)
        {
            if (FileTableLinkFromEng == null) return 1;
            string[] StrInLevels = FileTableLinkFromEng.Split(',');

            object[] coditions = new object[] { engSeq };
            string sql = @"
                select {0} from {2} where {1} in ({3})
    
            ";

            StrInLevels.ToList()
                .ForEach(paramterStr =>
                {
                    if (coditions.Length == 0) return;
                    object[] paramters = new object[4];
                    paramterStr.Split(':').CopyTo(paramters, 0);
                    paramters[3] =
                        coditions.Aggregate("", (a, c) => a + ',' + $"'{c}'").Remove(0, 1);
                    sql = String.Format(sql, paramters);
                    coditions =
                        db.GetDataTable(db.GetCommand(sql))
                            .Rows.Cast<DataRow>()
                            .Select(r => r.Field<object>(paramters[0].ToString()))
                            .ToArray();
                });

            if(coditions.Length > 0)
            {
                sql = @"
                    select count(*) from {0} where {1} in ({2}) and {3}
                ";
                sql = String.Format(sql, new object[] {
                    FileTable,
                    FileTableKey,
                    coditions.Aggregate("", (a, c) =>   a + ',' + $"'{c}'" ).Remove(0, 1),
                    FileTableCodition

                });
                return (int)db.ExecuteScalar(db.GetCommand(sql));
            }
            return 0;
        
        }
    }
}