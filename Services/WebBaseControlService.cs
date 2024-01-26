
using EQC.EDMXModel;
using EQC.ViewModel.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EQC.Services
{
    public class WebBaseControlService 
    {
        private EQC_NEW_Entities dbContext;
        private DbSet myDbSet;
        public WebBaseControlService(Func<EQC_NEW_Entities, DbSet> tableSelector)
        {

            dbContext = new EQC_NEW_Entities();
            dbContext.Configuration.LazyLoadingEnabled = false;
            myDbSet = tableSelector.Invoke(dbContext);

        }
        ~WebBaseControlService()
        {
            dbContext.Dispose();
        }

        public  IEnumerable<object> GetList()
        {
            return myDbSet.ToListAsync().Result;
        }
        public int Insert<T>(T m)
            where T : class, miniStandardRecord
        {
            m.CreateTime = DateTime.Now;
            myDbSet.Add(m);
            dbContext.SaveChanges();
            return m.Seq;
        }

        public void Update<T>(int id, T m)
            where T : class, miniStandardRecord
        {
            var editM = (T)myDbSet.Find(id);
            m.ModifyTime = DateTime.Now;
            m.CreateTime = editM.CreateTime;
            dbContext.Entry(editM).CurrentValues.SetValues(m);
            dbContext.SaveChanges();
        }

        public void UpdateOrCreate<T>(T m)
                        where T : class, miniStandardRecord
        {
            var exist = myDbSet.Find(m.Seq);
            if(exist != null)
            {
                Update(m.Seq, m);
            }
            else
            {
                Insert(m);
            }
        }
        public void Delete(int id)
        {
            myDbSet.Remove(myDbSet.Find(id));
            dbContext.SaveChanges();
        }

    }
}