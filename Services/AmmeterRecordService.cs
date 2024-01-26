using EQC.EDMXModel;
using EQC.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Services
{
    public class AmmeterRecordService
    {
        public AmmeterRecord[] GetAmmeterRecord(int year, int execUnitSeq)
        {
            using(var context = new EQC_NEW_Entities())
            {
                context.Configuration.ProxyCreationEnabled = false;

                var a = context.AmmeterRecord.ToList();
                var resultList = context.AmmeterRecord
                    .Where(row => row.Year == year && row.ExecUnitSeq == execUnitSeq)
                    .ToArray();

                return resultList;
            }
        }
        public void RecordAmmeter(AmmeterRecord m)
        {
            using(var context = new EQC_NEW_Entities() )
            {

                if (m.Session > 4) return;

                m.Session ++;
                context.Entry(context.AmmeterRecord.Find(m.Seq))
                    .CurrentValues.SetValues(m);
                context.SaveChanges();
            }
        }



        public void EditAmmeter(AmmeterRecord m)
        {
            using (var context = new EQC_NEW_Entities())
            {

                context.Entry(context.AmmeterRecord.Find(m.Seq))
                    .CurrentValues.SetValues(m);


                context.SaveChanges();
            }
        }

        public void addAmmeterRecord(int year , int unitSeq, AmmeterRecord m )
        {

            m.CreateTime = DateTime.Now;
            m.ModifyTime = DateTime.Now;
            m.Session1 = null;
            m.Session2 = null;
            m.Session3 = null;
            m.Session4 = null;
            m.Session = 0;
            m.Year = year;
            m.ExecUnitSeq = (short)unitSeq;
            using(var context =new EQC_NEW_Entities() )
            {
                try
                {

                    context.AmmeterRecord.Add(m);
                    context.SaveChanges();
                }
                catch(Exception e)
                {

                }
            }
        }

        public void deleteAmmeter(int Seq)
        {
            using (var context = new EQC_NEW_Entities())
            {
                context.Entry(context.AmmeterRecord.Find(Seq))
                    .State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }
    }
}