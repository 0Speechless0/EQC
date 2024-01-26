using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EQC.Common;
using EQC.EDMXModel;
using EQC.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EQC.Services
{
    public class RiskManagementService
    {
        private EQC_NEW_Entities dbContext;
        UserInfo _user;
        public RiskManagementService()
        {
            dbContext = new EQC_NEW_Entities();
            dbContext.Configuration.LazyLoadingEnabled = false;
            _user = new SessionManager().GetUser();
        }
        ~RiskManagementService()
        {
            dbContext.Dispose();
        }
        public string GoJsSubProjectJsonNodeNormalize(string json)
        {
            JArray nodeArr = JArray.Parse(json);
            foreach (var node in nodeArr)
            {
                node["tableSeq"] = 0;
            }
            return JsonConvert.SerializeObject(nodeArr);
        }
        public string importDestruction(int subProjectSeq, List<GoJsModel> nodeData, List<GoJsLinkModel> linkData, bool normal)
        {
            string nodeDataCopyJson = JsonConvert.SerializeObject(nodeData);
            if (normal) nodeDataCopyJson = GoJsSubProjectJsonNodeNormalize(nodeDataCopyJson);
            List<GoJsModel> nodeDataCopy = JsonConvert.DeserializeObject<List<GoJsModel>>(nodeDataCopyJson);

            Dictionary<int, GoJsModel> nodeDataCopyDic
                = nodeDataCopy
                .ToDictionary(item => item.key, item => item);

            List<EngRiskFrontSubProjectDetailTp> targetList = new List<EngRiskFrontSubProjectDetailTp>();


            linkData = linkData
                .OrderBy(link => nodeDataCopyDic[link.to].level)
                .Where(link => nodeDataCopyDic[link.to].level > 1)
                .ToList();

            foreach (var link in linkData )
            {
                var node = nodeDataCopyDic[link.to];
                var fromNode = nodeDataCopyDic[link.from];
                if (node.level == null) continue;
                targetList.Add(
                    new EngRiskFrontSubProjectDetailTp
                    {
                        Seq = node.tableSeq,
                        Level = node.level,
                        ParentSeq = fromNode.tableSeq ,
                        SubProjectSeq = subProjectSeq,
                        CreateTime = DateTime.Now,
                        CreateUser = _user.Seq,
                        StepName = node.text,
                        StepNo = $"{fromNode.no}.{node.no}"

                    }

                );
                 
                node.no = fromNode.no != "" ?  $"{fromNode.no}.{node.no}" : node.no;
            }
            targetList = targetList.OrderBy(item => item.StepNo).ToList();



            dbContext.EngRiskFrontSubProjectDetailTp
                .insert(targetList.Where(row => row.Seq == 0));

            dbContext.SaveChanges();
            List<EngRiskFrontSubProjectDetailTp> originList
                = dbContext.EngRiskFrontSubProjectDetailTp
                    .OrderBy(row => row.StepNo)
                    .Where(row => row.SubProjectSeq == subProjectSeq)
                    .ToList();


            dbContext.EngRiskFrontSubProjectDetailTp.delete(targetList, originList, originItem => originItem.Seq );
            dbContext.SaveChanges();




            originList
                = dbContext.EngRiskFrontSubProjectDetailTp
                    .Where(row => row.SubProjectSeq == subProjectSeq)
                    .OrderBy(item => item.StepNo)
                    .ToList();

            int i = 0;
            var nodeDataTp = nodeDataCopy
                .Where(node => node.level > 1 )
                .OrderBy(node => node.no).ToList();
            Dictionary<int, GoJsModel> nodeDataDic
                = nodeData
                .ToDictionary(item => item.key, item => item);
            foreach (var node in nodeDataTp)
            {
                nodeDataDic[node.key].tableSeq = originList[i].Seq;
                node.tableSeq = originList[i].Seq;
                i++;
            }
            var parentSeqDic = linkData.ToDictionary(row => nodeDataCopyDic[row.to].tableSeq, row => nodeDataCopyDic[row.from].tableSeq );

            targetList
                .Where(row => row.ParentSeq == 0)
                .OrderByDescending(row => row.Level)
                .ToList()
                .ForEach(row => 
                    row.ParentSeq = parentSeqDic[row.Seq]
                );
            i = 0;
            
            foreach(var target in  targetList )
            {
                originList[i].ModifyTime = DateTime.Now;
                originList[i].ModifyUser = _user.Seq;
                originList[i].ParentSeq = target.ParentSeq;
                originList[i].StepName = target.StepName;
                originList[i].StepNo = target.StepNo;
                i++;
            }



            string diagramJson = JsonConvert.SerializeObject(
                new
                {
                    @class = "GraphLinksModel",
                    nodeDataArray = nodeData,
                    linkDataArray = linkData
                }
            );


            var subProject = dbContext.EngRiskFrontSubProjectListTp.Find(subProjectSeq);
            subProject.SubProjectJson = diagramJson;
            subProject.ModifyTime = DateTime.Now;
            subProject.ModifyUserSeq = _user.Seq;

            dbContext.SaveChanges();
            return diagramJson;
        }

        public void updateEngRiskSubProjectDetail(List<EngRiskFrontSubProjectDetailTp> list)
        {
            int? subProjectSeq = list != null ? 
                list.Count > 0 ? list.First().SubProjectSeq : 0 : -1;

            List<EngRiskFrontSubProjectDetailTp> originList = 
                dbContext.EngRiskFrontSubProjectDetailTp
                .Where(row => row.SubProjectSeq == subProjectSeq)
                .OrderBy(row => row.StepNo)
                .ToList();
            dbContext.update(list, originList, row => row.Seq);
            dbContext.SaveChanges();
        }

        public List<EngRiskFrontSubProjectListTp> getRiskSubProjectList()
        {
           
            return dbContext.EngRiskFrontSubProjectListTp.ToList();
        }

        public List<EngRiskFrontSubProjectDetailTp> getRiskSubProjectDetailList(int subProjectSeq)
        {
        using(var context = new EQC_NEW_Entities())
            {
                context.Configuration.LazyLoadingEnabled = false;
                context.Configuration.ProxyCreationEnabled = false;
                return context
    .EngRiskFrontSubProjectDetailTp
    .Where(row => row.SubProjectSeq == subProjectSeq)
    .OrderBy(row => row.StepNo)
    .ToList();
            }


        }
        public List<EngRiskFrontHazardType> getEngRiskHazardType()
        {
           
            return dbContext.EngRiskFrontHazardType.ToList();
          

        }

        public void updateEngRiskSubProject(
            EngRiskFrontSubProjectListTp target
        )
        {
            target.ModifyTime = DateTime.Now;
            target.ModifyUserSeq = _user.Seq;
            dbContext.Entry(
                dbContext.EngRiskFrontSubProjectListTp.Find(target.Seq)
            ).CurrentValues.SetValues(target);
            dbContext.SaveChanges();
        }

        public void insertEngRiskSubProject(
            EngRiskFrontSubProjectListTp target
        )
        {
            target.CreateTime = DateTime.Now;
            target.CreateUserSeq = _user.Seq;
            if(dbContext.EngRiskFrontSubProjectListTp
                .Where(row => row.ExcelNo == target.ExcelNo).FirstOrDefault() ==null 
            ) 
                dbContext.EngRiskFrontSubProjectListTp.Add(target);
            dbContext.SaveChanges();

        }

        public void deleteEngRiskSubProject(
            int seq    
        )
        {
            dbContext.Entry(dbContext.EngRiskFrontSubProjectListTp.Find(seq)).State 
                = System.Data.Entity.EntityState.Deleted;
            dbContext.SaveChanges();
        }


    }
}