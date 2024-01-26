using EQC.EDMXModel;
using EQC.ViewModel;
using EQC.ViewModel.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using EQC.Common;
namespace EQC.Services
{
    public class CarbonReductionCalService
    {
        EQC_NEW_Entities dbContext ;
        //WebBaseControlService calTempservice;
        public CarbonReductionCalService()
        {
            //calTempservice = new WebBaseControlService(db => db.CarbonReductionCal);
            dbContext = new EQC_NEW_Entities();
        }
        ~CarbonReductionCalService()
        {
            dbContext.Dispose();
        }
        public void saveCalItems(int engSeq, List<CarbonReductionCal> items)
        {
            var calTempItems = dbContext.CarbonReductionCal.Where(r => r.EngMainSeq == engSeq).ToList();
            var itemsArr = items.ToArray();
            int i = 0;
            calTempItems.ForEach(e => {
                itemsArr[i].Seq = e.Seq;
                itemsArr[i].CreateTime = e.CreateTime;
                itemsArr[i].EngMainSeq = engSeq;
                itemsArr[i].ModifyTime = DateTime.Now;
                dbContext.Entry(e).CurrentValues.SetValues(itemsArr[i++]);



            }); 
            var insertList = Utils.getInsertList(items, calTempItems);
            insertList.ForEach(e => {
                e.CreateTime = DateTime.Now;
                e.EngMainSeq = engSeq;
     
            });

            var deleteList = Utils.getDeleteList(items, calTempItems);

            dbContext.CarbonReductionCal.AddRange(insertList);
            dbContext.CarbonReductionCal.RemoveRange(deleteList);
            dbContext.SaveChanges();

        }
        public decimal? GetCalResult(int engSeq)
        {
            return dbContext.CarbonReductionCalResult.Find(engSeq)?.Result;
        }
        public void saveCalResult(int engSeq, decimal result)
        {
            var exist = dbContext.CarbonReductionCalResult.Find(engSeq);
            if (exist != null)
            {
                exist.Result = result;
            }
            else
                dbContext.CarbonReductionCalResult.Add(new CarbonReductionCalResult { EngMainSeq = engSeq, Result = result });
            dbContext.SaveChanges();
        }
        public List<CarbonReductionCalVM<T>>  GetItem<T>(Func<EQC_NEW_Entities, DbSet<T>> setSelector,  int engSeq, int orderNo) where T : class, CarbonReduction
        {
            int? carbonHeaderSeq = dbContext.EngMain.Find(engSeq)
                .CarbonEmissionHeader
                .FirstOrDefault()?.Seq;

            if(carbonHeaderSeq is int _carbonHeaderSeq )
            {
                List<T> reductionFactorCodes =
                    setSelector.Invoke(dbContext)
                    .ToList();
                var matchedPayItem = dbContext.CarbonEmissionPayItem
                    .Where(r => r.CarbonEmissionHeaderSeq == _carbonHeaderSeq)
                    .ToList()
                    .Join(reductionFactorCodes, outter => outter.RefItemCode, inner => inner.Code, 
                    (outter, inner) => new CarbonReductionCalVM<T>{ 
                        Seq = outter.Seq,
                        Description = outter.Description,
                        RefItemCode = outter.RefItemCode,
                        Quantity = outter.Quantity,
                        factor = inner
                    })
                    .ToList();
                return matchedPayItem
                    .GroupJoin(dbContext.CarbonReductionCal,
                        outter => outter.Seq,
                        inner => inner.CarbonPayItemSeq,
                        (outter, inner) =>
                        {
                            if(inner.Count() <= orderNo)
                            {
                                outter.carbonReductionCal = inner.LastOrDefault();
                            }
                            else
                            {
                                outter.carbonReductionCal = inner?.Where(r => r.CalType == orderNo).FirstOrDefault();
                            }

                            return outter;
                        }
                    ).ToList();
                    

            }
            return new List<CarbonReductionCalVM<T>>();
        }

        public List<CarbonReductionCalVM<CarbonReductionFactor>> GetCarbonReductionFactor(int engSeq)
        {
            var factorItem =  dbContext.CarbonReductionFactor.ToList();
            return factorItem.GroupJoin(
                dbContext.CarbonReductionCal.Where(r => r.EngMainSeq == engSeq),
                outter => -outter.Seq,
                inner => inner.CarbonPayItemSeq,
                (outter, inner) =>
                {
                    return new CarbonReductionCalVM<CarbonReductionFactor>
                    {
                        Seq = outter.Seq,
                        factor = outter,
                        carbonReductionCal = inner.FirstOrDefault()
                    };
                }).ToList();
        }
        public List<T> GetFactorTypeDic<T>(Func<EQC_NEW_Entities, DbSet<T>> setSelector, int engSeq) where T : class, CarbonReduction
        {
            int? carbonHeaderSeq = dbContext.EngMain.Find(engSeq)
               .CarbonEmissionHeader?
               .FirstOrDefault()?.Seq;
            if (carbonHeaderSeq is int _carbonHeaderSeq)
            {
                List<T> reductionFactorCodes =
                setSelector.Invoke(dbContext)
                .ToList();
                return dbContext.CarbonEmissionPayItem
                    .Where(r => r.CarbonEmissionHeaderSeq == _carbonHeaderSeq)
                    .ToList()
                    .Join(reductionFactorCodes, outter => outter.RefItemCode, inner => inner.Code,
                    (outter, inner) => inner)
                    .GroupBy(inner => inner.Type2)
                    .Select(r => r.First())
                    .ToList();
;
            }
            return new List<T>();
                
        }
    }
}