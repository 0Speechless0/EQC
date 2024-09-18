using EQC.Common;
using EQC.Detection;
using EQC.EDMXModel;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    public class PackageDownloadActionController : MyController
    {

        public PackageDownloadActionController()
        {

        }

        public static Dictionary<int, int> MenuOrders = new Dictionary<int, int>() {
            {111,1 },
            {112,2 },
            {115,3 },
            {126,4 },
            {114,5 },
            {155,6 },
        };
        public void GetList(int systemType, int? engSeq = null ,string keyWord = null)
        {
            DBConn db = new DBConn();
            using (var context = new EQC_NEW_Entities())
            {
               
                Dictionary<string, List<PackageDownloadAction>> menuGroupedList = context.PackageDownloadAction
                    .Where(r => r.Menu.SystemTypeSeq == systemType)
                    .Where(r => r.Name.Contains(keyWord) || keyWord == null)
                    .ToList()
                    .Where(rr => rr.FileCount(db, engSeq) > 0)
                    .OrderBy(r => MenuOrders.ContainsKey(r.Menu.Seq) ? MenuOrders[r.Menu.Seq] : 0 )
                    .GroupBy(r => r.Menu.Name)

                    .ToDictionary(r => r.Key, r => r.Select(r1 => new PackageDownloadAction{ 
                        Name =r1.Name,
                        Seq = r1.Seq,
                        Url = r1.Url,
                        MainInjection = r1.MainInjection,
                        RelatedInjection = r1.RelatedInjection,
                        RelatedTable = r1.RelatedTable
                    })
                    .ToList()
                    )
                    ;
                ResponseJson(menuGroupedList);
            }
        }

        public void ExecuateOne(int seq,int engSeq, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            Utils.WebRootPath = HttpContext.Server.MapPath("~");
            var createUser = Utils.getUserSeq();
            new System.Threading.Tasks.Task(() =>
            {
                DownloadOneAction(new PackageDownloadAction { createUserSeq =  createUser}, seq, engSeq, StartDate, EndDate);
            }).Start();
            ResponseJson(new
            {
                message = "檔案已於背景下載中..."
            });
        }

        public void ExecuateMutiple(PackageDownloadAction[] actions, int engSeq)
        {
            Utils.WebRootPath = HttpContext.Server.MapPath("~");
            var pacakgeDistPath = Utils.GetTempFolderForUser($@"PackageTemp-{DateTime.Now.ToString("yyyyMMddHHmmss")}");
            var createUser = Utils.getUserSeq();

            new System.Threading.Tasks.Task(() =>
            {
                foreach (var action in actions)
                {
                    action.createUserSeq = createUser;
                    action.PackageDistPath = pacakgeDistPath;
                    DownloadOneAction(action, action.Seq, engSeq, action.StartDate, action.EndDate);
                }
                DownloadTaskDetection.AddTaskQueneToRun(() =>
                {
                    var fileName = $@"Package-{DateTime.Now.ToString("yyyyMMddHHmmss")}";
                    ZipFile.CreateFromDirectory(pacakgeDistPath, Path.Combine(Utils.GetTempFolderForUser(fileName, createUser), $"{fileName}.zip"));
                    System.IO.Directory.Delete(pacakgeDistPath, true);

                }, createUser);
                 

            }).Start();

            ResponseJson(new
            {
                message = "檔案已於背景下載中..."
            });

        }
        private void DownloadOneAction(PackageDownloadAction action = null,  params object[] p)
        {
           using(var context = new EQC_NEW_Entities())
           {
                try
                {
                    PackageDownloadAction target = null;
                    target = context.PackageDownloadAction.Find(p[0]);
                    target.inGroup = action?.inGroup ?? false;
                    target.PackageDistPath = action?.PackageDistPath;
                    target.createUserSeq = action?.createUserSeq ?? Utils.getUserSeq();
                    object instance = null;
                    var methodInfo = GetActionMethod(target.Url, ref instance);

                    var paramtersList = GetActionParamsDic(target, methodInfo, p);

                    DownloadArgExtension downloadExtensionArg = null;

                     
                    target.CreateTime = DateTime.Now;
                            
                    paramtersList.ForEach(paramters =>
                    {

                        DownloadTaskDetection.AddTaskQueneToRun(() =>
                        {
                            downloadExtensionArg = ((DownloadArgExtension)paramters.First().Value);
                            downloadExtensionArg.setDistFilePath(target);

                            methodInfo.InvokeWithNamedParameters(
                                instance, paramters);

                        }, target.createUserSeq ?? 0);

                    });

                    DownloadTaskDetection.AddTaskQueneToRun(() =>
                    {
                        downloadExtensionArg?.finalHandle(target);
                    }, target.createUserSeq ?? 0);

                }
                catch (Exception e)
                {

                }


            }
        }

        private List<Dictionary<string ,object>> GetActionParamsDic(
            PackageDownloadAction action,
            MethodInfo methodInfo,
            params object[] mainParamter
        )
        {
            var downloadExtensionParamterName = methodInfo.GetParameters().LastOrDefault()?.Name;
            var result = new Dictionary<string, object>()
            {
                {downloadExtensionParamterName, new DownloadArgExtension() }
            };
            int i = 1;

            action.MainInjection?.Split(',').ToList().ForEach(paramterName =>
            {
                result.Add(paramterName, mainParamter[i++]);
            });

            action.StaticInjection?.Split(',').ToList().ForEach(injectionItem =>
            {
                if(injectionItem.Contains('='))
                {
                    var div = injectionItem.Split('=');
                    if(Int32.TryParse(div[1], out int int_value) )
                    {
                        result.Add(div[0], int_value);
                    }
                    else
                    {
                        result.Add(div[0], div[1]);
                    }
         
                    
                }


            });
            var listDic = new List<Dictionary<string, object>>();
            listDic.Add(result);
            listDic = SetRelateParamter(mainParamter[1], action, listDic);

            return listDic;
        }

        private List<Dictionary<string, object>> SetRelateParamter(object relateOrgSeq, PackageDownloadAction action, List<Dictionary<string, object>> dics)
        {
            if (relateOrgSeq == null)
                return null;
            DBConn db = new DBConn();
            string sql;
            object nextSeq = relateOrgSeq;
            int i = 0;
            List<Dictionary<string, object>> current_dics = null;
            current_dics = new List<Dictionary<string, object>>();
            action.relateSqlParamter.ForEach(paramters =>
            {
                i++;
                current_dics = new List<Dictionary<string, object>>();

                sql = $@"select b.{paramters[0]} from {paramters[2]} b
                {action.WhereClasue ?? "where 1=1"}  and 
                    b.{paramters[1]} in ({nextSeq})
                ";
                var cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@nextSeq", nextSeq);
                var rows  = db.GetDataTable(cmd).Rows.Cast<DataRow>().Select(r => r.Field<object>(paramters[0])).ToList();



                if(rows.Count() > 1)
                {
                    int j = 1;
                    dics.ForEach(dic =>
                    {
                        
                        rows.ForEach(row =>
                        {
                            if (paramters[3] == "") return;
                            var ndic = dic.ToDictionary(r => r.Key, r => r.Value);
                            ndic[paramters[3]] = row;
                            current_dics.Add(ndic);
                        });
                    });
                    dics = current_dics;

                }
                else if(paramters[3] != "")
                {
                    dics[0].Add(paramters[3], (object)rows.FirstOrDefault());
                    current_dics = dics;
                }
                    

                if(rows.Count > 0 )
                    nextSeq = rows.Aggregate(" ", (a, c) => a + ',' + $"'{c}'")
                        .Trim().Remove(0, 1);

            });

            return current_dics.Count() > 0 ? current_dics : dics ;
        }

        
        private MethodInfo GetActionMethod(string url,ref object instance)
        {
            string[] div = url.Split('/');
            
            Type t = Type.GetType($"EQC.Controllers.{div[0]}Controller");
            instance = Activator.CreateInstance(t);
            return t.GetMethod(div[1]);

        }
    }
}