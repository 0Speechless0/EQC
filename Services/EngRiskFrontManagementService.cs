using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Web;
using EQC.Common;
using EQC.EDMXModel;
using EQC.ViewModel;
using Newtonsoft.Json;
using EQC.Models;
using Org.BouncyCastle.Ocsp;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EngRiskFrontManagementService : BaseService
    {
        private EQC_NEW_Entities dbContext;
        UserInfo _user;
        public EngRiskFrontManagementService()
        {
            dbContext = new EQC_NEW_Entities();
            dbContext.Configuration.LazyLoadingEnabled = false;
            _user = new SessionManager().GetUser();
        }


        ~EngRiskFrontManagementService()
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
        public void GoJsSubProjectJsonSetting(int? engRiskSeq)
        {
            if (engRiskSeq == null) return;
            var subProjectList =
            dbContext
                .EngRiskFrontSubProjectList
                //.Include("EngRiskFrontSubProjectDetail")
                .Where(r => r.EngRiskFrontSeq == engRiskSeq);
            foreach (var project in subProjectList)
            {
                var detailLevelNoList = dbContext.Entry(project)
                        .Collection(e => e.EngRiskFrontSubProjectDetail)
                        .Query()
                        .OrderBy(r => r.StepNo)
                    .GroupBy(row => row.Level)
                    .ToDictionary(row => row.Key, row => row.ToDictionary(r => r.StepNo, r => r))
                    ;
                for(int i = 2; i <4;i ++)
                {

                    if(detailLevelNoList.ContainsKey(i))
                    {
                        detailLevelNoList[i]
                            .ToList()
                            .ForEach(node =>
                            {
                                detailLevelNoList[i + 1]
                                    .Where(child => child.Key.StartsWith(node.Key))
                                    .ToList()
                                    .ForEach(child => child.Value.ParentSeq = node.Value.Seq);
                            });
                    }

                }
            }
            dbContext.SaveChanges();
            foreach (var subProject in subProjectList)
            {

                if (subProject.SubProjectJson == null) continue;
                var subProjectDetail =
                    dbContext.Entry(subProject)
                        .Collection(e => e.EngRiskFrontSubProjectDetail)
                        .Query()
                        .OrderBy(r => r.StepNo)
                        .ToList();

                JObject gojsObject = JObject.Parse(subProject.SubProjectJson);
                var nodeArr = gojsObject.GetValue("nodeDataArray").ToArray()
                    .Where(node => Int32.Parse(node["level"].ToString()) > 1)
                    .OrderBy(node => node["tableSeq"]);
                int i = 0;
                foreach (var node in nodeArr)
                {
                    node["tableSeq"] = subProjectDetail[i++].Seq;
                }
                var str = JsonConvert.SerializeObject(gojsObject);
                subProject.SubProjectJson = str;

            }
            dbContext.SaveChanges();
        }

        public string importDestruction(int subProjectSeq, List<GoJsModel> nodeData, List<GoJsLinkModel> linkData, bool normal)
        {
            string nodeDataCopyJson = JsonConvert.SerializeObject(nodeData);
            if (normal) nodeDataCopyJson = GoJsSubProjectJsonNodeNormalize(nodeDataCopyJson);
            List<GoJsModel> nodeDataCopy = JsonConvert.DeserializeObject<List<GoJsModel>>(nodeDataCopyJson);

            Dictionary<int, GoJsModel> nodeDataCopyDic
                = nodeDataCopy
                .ToDictionary(item => item.key, item => item);

            List<EngRiskFrontSubProjectDetail> targetList = new List<EngRiskFrontSubProjectDetail>();


            linkData = linkData
                .OrderBy(link => nodeDataCopyDic[link.to].level)
                .Where(link => nodeDataCopyDic[link.to].level > 1)
                .ToList();
            foreach (var link in linkData)
            {
                var node = nodeDataCopyDic[link.to];
                var fromNode = nodeDataCopyDic[link.from];
                if (node.level == null) continue;
                targetList.Add(
                    new EngRiskFrontSubProjectDetail
                    {
                        Seq = node.tableSeq,
                        Level = node.level,
                        ParentSeq = fromNode.tableSeq,
                        SubProjectSeq = subProjectSeq,
                        CreateTime = DateTime.Now,
                        CreateUser = _user.Seq,
                        StepName = node.text,
                        StepNo = $"{fromNode.no}.{node.no}"

                    }

                );

                node.no = fromNode.no != "" ? $"{fromNode.no}.{node.no}" : node.no;
            }
            targetList = targetList.OrderBy(item => item.StepNo).ToList();



            dbContext.EngRiskFrontSubProjectDetail
                .insert(targetList.Where(row => row.Seq == 0));

            dbContext.SaveChanges();
            List<EngRiskFrontSubProjectDetail> originList
                = dbContext.EngRiskFrontSubProjectDetail
                    .OrderBy(row => row.StepNo)
            .Where(row => row.SubProjectSeq == subProjectSeq)
            .ToList();


            dbContext.EngRiskFrontSubProjectDetail.delete(targetList, originList, originItem => originItem.Seq);
            dbContext.SaveChanges();




            originList
                = dbContext.EngRiskFrontSubProjectDetail
                    .Where(row => row.SubProjectSeq == subProjectSeq)
                    .OrderBy(item => item.StepNo)
                    .ToList();

            int i = 0;
            var nodeDataTp = nodeDataCopy
                .Where(node => node.level > 1)
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
            var parentSeqDic = linkData.ToDictionary(row => nodeDataCopyDic[row.to].tableSeq, row => nodeDataCopyDic[row.from].tableSeq);

            targetList 
                .Where(row => row.ParentSeq == 0)
                .OrderByDescending(row => row.Level)
                .ToList()
                .ForEach(row =>
                    row.ParentSeq = parentSeqDic[row.Seq]
                );
            i = 0;

            foreach (var target in targetList)
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


            var subProject = dbContext.EngRiskFrontSubProjectList.Find(subProjectSeq);
            subProject.SubProjectJson = diagramJson;
            subProject.ModifyTime = DateTime.Now;
            subProject.ModifyUserSeq = _user.Seq;

            dbContext.SaveChanges();
            return diagramJson;
        }

        public void updateEngRiskSubProjectDetail(List<EngRiskFrontSubProjectDetail> list)
        {
            int? subProjectSeq = list != null ?
                list.Count > 0 ? list.First().SubProjectSeq : 0 : -1;

            List<EngRiskFrontSubProjectDetail> originList =
                dbContext.EngRiskFrontSubProjectDetail
                .Where(row => row.SubProjectSeq == subProjectSeq)
                .OrderBy(row => row.StepNo)
                .ToList();
            dbContext.update(list, originList, row => row.Seq);
            dbContext.SaveChanges();
        }

        public void updateEngRiskSubProject(EngRiskFrontSubProjectList target)
        {
            target.ModifyTime = DateTime.Now;
            target.ModifyUserSeq = _user.Seq;
            dbContext.Entry(
                dbContext.EngRiskFrontSubProjectList.Find(target.Seq)
            ).CurrentValues.SetValues(target);
            dbContext.SaveChanges();
        }

        public List<EngRiskFrontHazardType> getEngRiskHazardType()
        {

            return dbContext.EngRiskFrontHazardType.ToList();


        }

        public List<EngRiskFrontSubProjectDetail> getRiskSubProjectDetailList(int subProjectSeq)
        {
            using (var context = new EQC_NEW_Entities())
            {
                context.Configuration.LazyLoadingEnabled = false;
                context.Configuration.ProxyCreationEnabled = false;
                return context
                    .EngRiskFrontSubProjectDetail
                    .Where(row => row.SubProjectSeq == subProjectSeq)
                    .OrderBy(row => row.StepNo)
                    .ToList();
            }
        }

        public List<EngRiskFrontSubProjectList> getRiskSubProjectList()
        {
            return dbContext.EngRiskFrontSubProjectList.ToList();
        }

        public void insertEngRiskSubProject(EngRiskFrontSubProjectList target)
        {
            target.CreateTime = DateTime.Now;
            target.CreateUserSeq = _user.Seq;
            if (dbContext.EngRiskFrontSubProjectList
                .Where(row => row.ExcelNo == target.ExcelNo && row.EngRiskFrontSeq == target.EngRiskFrontSeq ).FirstOrDefault() == null
            )
                dbContext.EngRiskFrontSubProjectList.Add(target);
            dbContext.SaveChanges();

        }

        public void deleteEngRiskSubProject(int seq)
        {
            //dbContext.Entry(dbContext.EngRiskFrontSubProjectList.Find(seq)).State
            //    = System.Data.Entity.EntityState.Deleted;
            //dbContext.SaveChanges();
            try
            {
                string sql = @"UPDATE [dbo].[EngRiskFrontSubProjectList] SET [IsEnabled]=0 WHERE [Seq]=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                //db.TransactionRollback();
                log.Info("EngRiskFrontManagementService.deleteEngRiskSubProject" + e.Message);
            }
        }
    }
}