using EQC.Common;
using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EQMEcologicalStaService : BaseService
    {//生態檢核統計
        //執行階段
        public List<EQMEcologicalStaVModel> GetPlanList()
        {
            string sql = @"
                select
	                z1.OrderNo,
                    z1.execUnitName,
                    z1.needChcek+z1.notChcek engCount, --A
                    z1.notChcek, --B
                    z1.needChcek, --C
                    z1.needChcek execChcek,
                    0 lostChcek,
                    cast(0 as decimal(18,2)) notChcekRate,
                    cast(0 as decimal(18,2)) needChcekRate,
                    cast(0 as decimal(18,2)) execChcekRate,
                    cast(0 as decimal(18,2)) lostChcekRate
                from (
                  select
                      z.OrderNo,
                      z.execUnitName,
                      sum(z.needChcek) needChcek,
                      sum(z.notChcek) notChcek
                  from (
                    select
                        b.OrderNo,
                        b.Name execUnitName,
                        IIF(c.ToDoChecklit<3,1,0) needChcek,
                        IIF(c.ToDoChecklit>2,1,0) notChcek
                    from EngMain a
                    inner join Unit b on(b.ParentSeq is null and b.Seq = a.ExecUnitSeq and b.name != '水利署')
                    inner join EcologicalChecklist c on(c.EngMainSeq=a.Seq and c.Stage=1)
                    where 1=1"
                    + Utils.getAuthoritySql("a.")
                    + @"
                  ) z
                  group by z.OrderNo, z.execUnitName
                ) z1
                order by z1.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
           // cmd.Parameters.AddWithValue("@Stage", stage);
            List<EQMEcologicalStaVModel> lists = db.GetDataTableWithClass<EQMEcologicalStaVModel>(cmd);

            EQMEcologicalStaVModel total = new EQMEcologicalStaVModel() { execUnitName = "總計" };
            foreach (EQMEcologicalStaVModel m in lists)
            {
                total.engCount += m.engCount;
                total.notChcek += m.notChcek;
                total.needChcek += m.needChcek;
                total.execChcek += m.execChcek;
                total.lostChcek += m.lostChcek;
            }
            lists.Add(total);

            return lists;
        }
        //施工階段
        public List<EQMEcologicalStaVModel> GetExecList()
        {
            string sql = @"
                select
	                z1.OrderNo,
                    z1.execUnitName,
                    z1.needChcek+z1.notChcek engCount, --A
                    z1.notChcek, --B
                    z1.needChcek, --C
                    z1.execChcek,
                    0 lostChcek,
                    cast(0 as decimal(18,2)) notChcekRate,
                    cast(0 as decimal(18,2)) needChcekRate,
                    cast(0 as decimal(18,2)) execChcekRate,
                    cast(0 as decimal(18,2)) lostChcekRate
                from (
                  select
                      z.OrderNo,
                      z.execUnitName,
                      sum(z.needChcek) needChcek,
                      sum(z.notChcek) notChcek,
                      sum(z.needChcek) execChcek
                  from (
                    select
                        b.OrderNo,
                        b.Name execUnitName,
                        IIF(d.ToDoChecklit<3,1,0) needChcek,
                        IIF(d.ToDoChecklit>2,1,0) notChcek
                    from EngMain a
                    inner join Unit b on(b.ParentSeq is null and b.Seq = a.ExecUnitSeq and b.name != '水利署')
                    inner join EcologicalChecklist c on(c.EngMainSeq=a.Seq and c.Stage=2)
                    inner join EcologicalChecklist d on(d.EngMainSeq=a.Seq and d.Stage=1)
                    where 1=1"
                    + Utils.getAuthoritySql("a.")
                    + @"
                  ) z
                  group by z.OrderNo, z.execUnitName
                ) z1
                order by z1.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            // cmd.Parameters.AddWithValue("@Stage", stage);
            List<EQMEcologicalStaVModel> lists = db.GetDataTableWithClass<EQMEcologicalStaVModel>(cmd);

            EQMEcologicalStaVModel total = new EQMEcologicalStaVModel() { execUnitName = "總計" };
            foreach (EQMEcologicalStaVModel m in lists)
            {
                total.engCount += m.engCount;
                total.notChcek += m.notChcek;
                total.needChcek += m.needChcek;
                total.execChcek += m.execChcek;
                total.lostChcek += m.lostChcek;
            }
            lists.Add(total);

            return lists;
        }
    }
}