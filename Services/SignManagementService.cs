using EQC.Common;
using EQC.EDMXModel;
using EQC.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace EQC.Services
{
    public class SignManagementService
    {
        UserInfo _user;
        public SignManagementService()
        {
           _user =  new SessionManager().GetUser();
           
        }
        public List<ApprovalModuleList> getFormList()
        {
            using (var context = new EQC_NEW_Entities())
            {
                context.Configuration.ProxyCreationEnabled = false;
                return context.ApprovalModuleList.GroupBy(row => row.FormCode)
                    .Select(row => row.FirstOrDefault())
                    .ToList();
            }
        }
        public List<ApprovalModuleList> getFormFlowList(string formCode)
        {
            using (var context = new EQC_NEW_Entities(false))
            {
                var list = context.ApprovalModuleList
                    .Include("ApproverList.Position")
                    .Where(row => row.FormCode == formCode)

                    .ToList();

                return list;
            }
        }

        public Dictionary<int, List<SelectVM> > getTypeUnitList()
        {
            var unitService = new UnitService();
            return new Dictionary<int, List<SelectVM>>()
            {
                {1,  unitService.GetUnitList(1) },
                {2,  unitService.GetUnitList(23) },
                {3,  unitService.GetUnitList(273) },
                {4,  unitService.GetUnitList(16) },
                {5,  unitService.GetUnitList(15) },
                {6,  unitService.GetUnitList(281) },
            };
        }

        //public List<ApproverList> GetApproverLists()
        //{
        //    using (var context = new EQC_NEW_Entities())
        //    {
        //        context.Configuration.ProxyCreationEnabled = false;
        //        var posisionDic = context.Position.ToDictionary(row => row.Seq, row => row.Name);

        //        var list = context.ApproverList.ToList();
        //        list.ForEach(row => row.Name += (posisionDic.ContainsKey((short)(row.PositionSeq ?? 0) ) ? "-" +posisionDic[(short)row.PositionSeq] : ""));
        //        return list;
        //    }
        //}
        public List<ApprovingUnitType> GetApprovingUnitTypes()
        {
            using (var context = new EQC_NEW_Entities())
            {
                return context.ApprovingUnitType.ToList();
            }
        }

        public bool insertForm(ApprovalModuleList m)
        {
            var user = new SessionManager().GetUser();
            using(var context = new EQC_NEW_Entities() )
            {
                if (context.ApprovalModuleList.Where(row => row.FormCode == m.FormCode).Count() > 0) return false ;
                m.ApproverList = new ApproverList()
                {
                    Name = "申請人"
                };
                m.Approver = insertApprover(m.ApproverList);

                m.CreateTime = DateTime.Now;
                m.ModifyTime = DateTime.Now;
                m.CreateUserSeq = user.Seq;
                m.ModifyUserSeq = user.Seq;
                m.ApprovalWorkFlow = 1;
                
                context.ApprovalModuleList.Add(m);
                context.SaveChanges();

                return true;
            }
        }

        public void updateFormName(string formCode, string name)
        {
            using (var context = new EQC_NEW_Entities())
            {
                context.ApprovalModuleList.Where(row => row.FormCode == formCode)
                    .ToList()
                    .ForEach(row => row.FormName = name);

                context.SaveChanges();
            }
        }
        public void deleteForm(string formCode)
        {
            using(var context = new EQC_NEW_Entities())
            {
                context.ApprovalModuleList.RemoveRange(
                    context.ApprovalModuleList.Where(row => row.FormCode == formCode)
                    );
                context.SaveChanges();
            }
        }

        public int insertApprover(ApproverList approver)
        {
            approver = approver ?? new ApproverList() { 
                CreateTime = DateTime.Now,
                CreateUserSeq = _user.Seq
            };
            using (var context = new EQC_NEW_Entities() )
            {
                context.ApproverList.Add(approver);
                context.SaveChanges();
                return approver.Seq;
            }
            
        }
        public void syncFormFlowList(List<ApprovalModuleList> targetList, int[][] flowPositions)
        {
            var user = new SessionManager().GetUser();
            targetList.Reverse();
            string formCode = targetList.FirstOrDefault()?.FormCode ?? "";
            using (var context = new EQC_NEW_Entities(false) )
            {

                var sourceList = context.ApprovalModuleList
                    .Include("ApproverList.Position")
                    .Where(row => row.FormCode == formCode).ToList();

                var insertList = Utils.getInsertList<ApprovalModuleList>(targetList, sourceList);
                var deleteList = Utils.getDeleteList<ApprovalModuleList>(targetList, sourceList);
                int updateCount = Math.Min(targetList.Count, sourceList.Count);
                int i;
                for ( i =0; i < updateCount; i ++)
                {

                    targetList[i].Seq = sourceList[i].Seq;
                    //targetList[i].Approver = targetList[i].Approver ?? insertApprover(targetList[i].ApproverList);
                    targetList[i].ApproverList.ModifyTime = DateTime.Now;
                    targetList[i].ApproverList.ModifyUserSeq = user.Seq;
                    targetList[i].ApproverList.CreateTime = sourceList[i].ApproverList.CreateTime;
                    targetList[i].ModifyTime = DateTime.Now;
                    targetList[i].ModifyUserSeq = user.Seq;
                    

                    context.Entry(sourceList[i])
                        .CurrentValues.SetValues(targetList[i]);
                    targetList[i].ApproverList.Seq = targetList[i].Approver ?? 0;
                    context.Entry(sourceList[i].ApproverList)
                        .CurrentValues.SetValues(targetList[i].ApproverList);

                    sourceList[i].ApproverList.Position.Clear();
                    context
                        .Position
                        .ToList()
                        .Where(row => flowPositions[i].Contains(row.Seq))
                        .ToList()
                        .ForEach(row => sourceList[i].ApproverList.Position.Add(row));
                }
                i = 0;
                foreach (var item in insertList)
                {
                    //int insertedApproverId = insertApprover(item.ApproverList);
                    item.CreateTime = DateTime.Now;
                    item.ModifyTime = DateTime.Now;
                    item.CreateUserSeq = user.Seq;
                    item.ModifyUserSeq = user.Seq;
                    item.ApproverList = item.ApproverList ?? new ApproverList();
                    item.ApproverList.CreateTime = DateTime.Now;
                    item.ApproverList.CreateUserSeq = _user.Seq;

                    //item.Approver = insertedApproverId;

                    item.ApproverList.Position.Clear();
                    context
                        .Position
                        .ToList()
                        .Where(row => flowPositions[i].Contains(row.Seq))
                        .ToList()
                        .ForEach(row => item.ApproverList.Position.Add(row));
                    context.ApprovalModuleList.Add(item);

                    i++;
                }
                foreach (var item in deleteList)
                {
                    item.ApproverList.Position.Clear();
                    context.ApproverList.Remove(item.ApproverList);
                    context.ApprovalModuleList.Remove(item);

                } 

                context.SaveChanges();
            }
        }
    }
}
