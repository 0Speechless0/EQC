using EQC.Common;
using EQC.Controllers;
using EQC.EDMXModel;
using EQC.Models;
using EQC.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EQC.Services
{


    public class ChapterItemModel
    {
        public int Seq { get; set; }
        public string ItemName { get; set; }
        public int chapter { get; set; }
    }
    public class ChapterControlModel
    {
        public int Seq { get; set; }
        public string ItemName { get; set; }
        public string ControlInfo { get; set; }
        public int Stage { get; set; }
    }
    public class UploadCheckJsonModel
    {

        public string account { get; set; }

        public string subItem { get; set; }
        public int engSeq { get; set; }
        public int chapter { get; set; }

        //應該是chapterItemSeq
        public int constCheckSeq { get; set; }
        public DateTime? checkDate { get; set; }

        public string checkLocation { get; set; }

        public int projectType { get; set; }

        public Dictionary<int, object[]> standardResult {get;set;}

        public string[][][] imageData { get; set; }

        public constCheckSignatures[] signature { get; set; }

    }

    public class MobileAPIService : BaseService
    {
       private readonly EQC_NEW_Entities context;
        ConstCheckRecService constCheckRecService ;
        public MobileAPIService()
        {
            context = new EQC_NEW_Entities();
            constCheckRecService = new ConstCheckRecService();
        }

        ~MobileAPIService()
        {
            context.Dispose();
        }

        public List<T> GetEngSeqByEngNo<T>(string engNo)
        {
            string sql = @"
                SELECT e.Seq, e.EngName, e.EngNo, e.EngYear, u.Name execUnitName, e.BuildContractorContact, e.SupervisorDirector FROM EngMain e
                inner join Unit u on u.Seq = e.ExecUnitSeq
                where e.EngNo=@engNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@engNo", engNo);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<ChapterControlModel> GetChapterControl(int seq, int chapter, int[] stages = null)
        {
            List<ChapterControlModel> controlList = new List<ChapterControlModel>();
            if (chapter == 6)
            {
                new EquOperControlStService()

                    .GetList<EquOperControlStModel>(seq)
                    .GroupBy(row => $"{row.EPCheckItem1.Replace("\r\n", "")} {row.EPCheckItem2.Replace("\r\n", "")}".Trim())
                    .ToList()
                    .ForEach(group =>
                    {
                        string standardStr = group.Aggregate("", (acc, cur) => acc += cur.EPStand1 + "  ");
                        int itemSeq = group.First().Seq;
                        controlList.Add(new ChapterControlModel
                        {
                            ItemName = group.Key,
                            Seq = itemSeq,
                            ControlInfo = standardStr,
                            Stage = 1

                        });

                    });


            }
            if (chapter == 701)
            {
                new ConstCheckControlStService()
                    .GetList<ConstCheckControlStModel>(seq)
                    .Where(row => stages?.Contains(row.CCFlow1 ?? 0) ?? true)
                    .GroupBy(row => row.CCManageItem1)
                    .ToList()
                    .ForEach(group => {
                        string standardStr = group.Aggregate($"", (acc, cur) => acc += cur.CCCheckStand1+"  ");
                        int itemSeq = group.First().Seq;
                        controlList.Add(new ChapterControlModel
                        {
                            ItemName = group.Key.Replace("\r\n", "").Trim(),
                            Seq = itemSeq,
                            ControlInfo = standardStr,
                            Stage = (byte)group.First().CCFlow1

                        });

                    });

            }
            if (chapter == 702)
            {
                new OccuSafeHealthControlStService()
                    .GetList<OccuSafeHealthControlStModel>(seq)
                    .GroupBy(row => row.OSCheckItem1)
                    .ToList()
                    .ForEach(group => {
                        string standardStr = group.Aggregate($"", (acc, cur) => acc += cur.OSStand1 + "  ");
                        int itemSeq = group.First().Seq;
                        controlList.Add(new ChapterControlModel
                        {
                            ItemName = group.Key.Replace("\r\n", "").Trim(),
                            Seq = itemSeq,
                            ControlInfo = standardStr,
                            Stage = 1

                        });

                    });

            }
            if (chapter == 703)
            {
                new EnvirConsControlStService()
                    .GetList<EnvirConsControlStModel>(seq)
                    .Where(row => stages?.Contains(row.ECCFlow1 ?? 0) ?? true)
                    .GroupBy(row => row.ECCCheckItem1)
                    .ToList()
                    .ForEach(group => {
                        string standardStr = group.Aggregate($"", (acc, cur) => acc += cur.ECCStand1 + "  ");
                        int itemSeq = group.First().Seq;
                        controlList.Add(new ChapterControlModel
                        {
                            ItemName = group.Key.Replace("\r\n", "").Trim(),
                            Seq = itemSeq,
                            ControlInfo = standardStr,
                            Stage = (byte)group.First().ECCFlow1

                        });

                    });
            }
            return controlList;
        }

        public List<object> GetEngListByUser(string userNo, int engYear = 0, string str = null)
        {
            string getRole = @" Select distinct ur.RoleSeq from UserRole ur
                inner join UserUnitPosition uu on uu.Seq = ur.UserUnitPositionSeq
                inner join UserMain u on u.Seq = uu.UserMainSeq
                where u.userNo = @userNo                
            ";
            SqlCommand cmd = db.GetCommand(getRole);
            cmd.Parameters.AddWithValue("@userNo", userNo);
            int role = Convert.ToInt32(db.ExecuteScalar(cmd));

            string sql = @"
                Select *
                from

	                (select 
	                ROW_NUMBER() OVER(PARTITION BY p.PrjXMLSeq ORDER BY p.CreateTime desc) rowNumber,
                    e.Seq, 
                    e.EngNo,  
                    e.EngName,
                    cast(  (select Name from Unit where Unit.Seq = e.ExecUnitSeq ) as varchar(20)) as ExecUnitName  
                    from EngMain e
                    inner join ProgressData p on p.PrjXMLSeq = e.PrjXMLSeq
                    left join ConstCheckUser c on c.EngSeq = e.Seq
                    left join UserMain u on u.Seq = c.UserSeq
                    where 
                
                    p.PDExecState = '施工中' and
            
                    ((e.PrjXMLSeq is not null " + Utils.getAuthoritySql("e.", userNo) + @")
                    or u.userNo= @UserNo)
                    and (e.EngYear = @Year or @Year = 0)
                    and ( e.EngName Like '%'+ @Str+'%' or e.EngNo Like '%'+@Str+'%' or @Str is null)

	                ) c　where c.rowNumber = 1



            ";

            cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@userNo", userNo);
            cmd.Parameters.AddWithValue("@Year", engYear);
            cmd.Parameters.AddWithValue("@Str", (object)str ?? DBNull.Value);

            return db.GetDataTable(cmd).Rows.Cast<DataRow>().Select(row => new
            {
                Seq = row.Field<object>("Seq"),
                EngNo = row.Field<string>("EngNo"),
                EngName = row.Field<string>("EngName"),
                ExecUnitName = row.Field<string>("ExecUnitName")
            }).ToList<object>();

        }
        public List<ChapterItemModel> GetEngChapterItems(string engNo)
        {
            List<ChapterItemModel> list = new List<ChapterItemModel>();
            new EquOperTestListService().GetListByEngNo<ConstCheckListVModel>(engNo)
                               .ForEach(row => {
                                   list.Add(new ChapterItemModel
                                   {
                                       Seq = row.Seq,
                                       ItemName = row.ItemName,
                                       chapter = 6
                                   });
                               });
            new ConstCheckListService().GetListByEngNo<ConstCheckListVModel>(engNo)
                .ForEach(row => {
                    list.Add(new ChapterItemModel
                    {
                        Seq = row.Seq,
                        ItemName = row.ItemName,
                        chapter = 701
                    });
                });
            new OccuSafeHealthListService().GetListByEngNo<OccuSafeHealthListModel>(engNo)
                .ForEach(row => {
                    list.Add(new ChapterItemModel
                    {
                        Seq = row.Seq,
                        ItemName = row.ItemName,
                        chapter = 702
                    });
                });
            new EnvirConsListService().GetListByEngNo<ConstCheckListVModel>(engNo)
                .ForEach(row => {
                    list.Add(new ChapterItemModel
                    {
                        Seq = row.Seq,
                        ItemName = row.ItemName,
                        chapter = 703
                    });
                });
            return list;
        }

        public List<constCheckSignatures> GetCheckSignature(string token)
        {
            var list = context.constCheckSignatures
                .Where(r => r.Token == token && r.ConstCheckSeq == 0)
                .ToList();
            return list.Select(r => new constCheckSignatures
            {
                SignatureRole = r.SignatureRole,
                EngSeq = r.EngSeq,
                ConstCheckSeq = r.ConstCheckSeq,
                SignatureVal = r.SignatureVal
            }).ToList();
            return list;
        }
        public List<object> GetDocument(int engSeq)
        {
            string host = Utils.getHost();

            return  $"MobileConstCheckDocuments/{engSeq}"
                .GetFiles()
                .Select(filePath => new {
                    DownLoadLink = $"{host}/FileUploads/MobileConstCheckDocuments/{engSeq}/{Path.GetFileName(filePath).getEscapeDataString()}",
                    Description = Path.GetFileName(filePath)

                }
                ).ToList<object>();
           
        }
        public Dictionary<string, IEnumerable<object>> GetToolPackage(int engSeq)
        {
            var packageData = context.ToolPackage
                .Where(row => row.EngSeq == engSeq)
                .ToList()
                .GroupBy(row =>
                {
                    switch (row.FileName.Split('.')[1].ToLower())
                    {
                        case "png": return "image";
                        case "jpg": return "image";
                        case "pdf": return "document";

                        default: return "nothing";
                    }
                })
                .ToDictionary(row => row.Key, row => row);

            string host = Utils.getHost();
            var returnPackageData = new Dictionary<string, IEnumerable<object>>();


            returnPackageData.Add("document", packageData.ContainsKey("document") ? packageData["document"]
                .Select(doc => new
                {
                    Description = doc.Description,
                    DownLoadLink = $"{host}/FileUploads/ToolPackage/{engSeq}/{doc.FileName.getEscapeDataString()}",
                    Stage = doc.Stage,
                }) : null );

            returnPackageData.Add("image", packageData.ContainsKey("image") ? packageData["image"]
                .Select(img => new
                {
                    Description = img.Description,
                    Base64 = $"ToolPackage/{engSeq}/{img.FileName}".ConvertToBase64(),
                    Stage = img.Stage,
                }):  null);
            return returnPackageData;
        }

        public void UploadCheckJson(string json, string token)
        {
            //uploadObject.constCheckSeq 其實是章節項目序列號

            var uploadObject = JsonConvert.DeserializeObject<UploadCheckJsonModel>(json);
            var usrSeq = new UserService().GetUserByAccount(uploadObject.account).FirstOrDefault()?.Seq ?? 0;
            var engNo = new EngMainService().GetItemBySeq<EDMXModel.EngMain>(uploadObject.engSeq).FirstOrDefault()?.EngNo;
            Dictionary<string, ChapterControlModel> controlDic = new Dictionary<string, ChapterControlModel>();
            var exceptControlSeqs = new List<int>();
            if (uploadObject.chapter == 6)
            {
                controlDic = new EquOperTestListService()
                    .GetList<EquOperControlStModel>(engNo)
                    .GroupBy(row => (row.EPCheckItem1.Replace("\r\n", "") + " " + row.EPCheckItem2.Replace("\r\n", "")).Trim())
                    .Select(group => {
                        if (group.Count() > 1) exceptControlSeqs.AddRange(group.Skip(1).Select(r => r.Seq));
                        return new ChapterControlModel
                        {
                            Seq = group.First().Seq,
                            ItemName = group.Key
                        };
                    })
                    .ToDictionary(row => row.ItemName);
                
            }
            if (uploadObject.chapter == 701)
            {
                controlDic = new ConstCheckControlStService()
                    .GetList<ConstCheckControlStModel>(uploadObject.constCheckSeq)
                    .GroupBy(row => row.CCManageItem1.Replace("\r\n", "").Trim() )
                    .Select(group => {
                        if (group.Count() > 1) exceptControlSeqs.AddRange(group.Skip(1).Select(r => r.Seq));
                        return new ChapterControlModel
                        {
                            Seq = group.First().Seq,
                            ItemName = group.Key
                        };
                    })
                    .ToDictionary(row => row.ItemName);
            }
            if (uploadObject.chapter == 702)
            {
                controlDic = new OccuSafeHealthControlStService()
                    .GetList<OccuSafeHealthControlStModel>(uploadObject.constCheckSeq)
                    .GroupBy(row => row.OSCheckItem1.Replace("\r\n", "").Trim())
                    .Select(group => {
                        if (group.Count() > 1) exceptControlSeqs.AddRange(group.Skip(1).Select(r => r.Seq));
                        return new ChapterControlModel
                        {
                            Seq = group.First().Seq,
                            ItemName = group.Key
                        };
                    })
                    .ToDictionary(row => row.ItemName);

            }
            if (uploadObject.chapter == 703)
            {
                controlDic = new EnvirConsControlStService()
                    .GetList<EnvirConsControlStModel>(uploadObject.constCheckSeq)
                    .GroupBy(row => row.ECCCheckItem1.Replace("\r\n", "").Trim())
                    .Select(group => {
                        if (group.Count() > 1) exceptControlSeqs.AddRange(group.Skip(1).Select(r => r.Seq));
                        return new ChapterControlModel
                        {
                            Seq = group.First().Seq,
                            ItemName = group.Key
                        };
                    })
                    .ToDictionary(row => row.ItemName);
            }

            var constructionSeqDic = new EngConstructionService()
                .GetListAllByEngMainSeq<ConstCheckListVModel>(uploadObject.engSeq)
                .ToDictionary(row => row.ItemName, row => row.Seq);

            //新增抽查單
            ConstCheckRec constCheck = new ConstCheckRec();
            try
            {
                constCheck = new ConstCheckRec
                {
                    IsManageConfirm = false,
                    FormConfirm = 0,
                    ItemSeq = uploadObject.constCheckSeq,
                    CreateTime = DateTime.Now,
                    CreateUserSeq = usrSeq,
                    CCRCheckType1 = (byte)CheckType(uploadObject.chapter),
                    CCRCheckFlow = (byte)uploadObject.projectType,
                    EngConstructionSeq = constructionSeqDic[uploadObject.subItem],
                    CCRCheckDate = uploadObject.checkDate,
                    CCRPosDesc = uploadObject.checkLocation,

                };
                context.ConstCheckRec.Add(constCheck);
            }
            catch(Exception e)
            {
                BaseService.log.Info($"{e.Message}:{constructionSeqDic.Aggregate("", (a, c) => a+c.Key+",")}<======= {uploadObject.subItem}");
            }

            context.SaveChanges();
            //新增抽查圖檔
            for (int i1 = 0; i1 < uploadObject.imageData.Length; i1++)
            {
                var imageArr = uploadObject.imageData[i1][0];
                string[] controlArr = new string[0];
                if (uploadObject.imageData[i1][1].Length > 1)
                {
                    controlArr = uploadObject.imageData[i1][1];
                }
                else if(uploadObject.imageData[i1][1].Length == 1)
                {
                    controlArr = uploadObject.imageData[i1][1][0].Split(',').Select(str => str.Replace("  ", "").Trim()).ToArray();
                }


                for (int j = 0; j < imageArr.Length; j++)
                {

                    string uniqueName = UploadFilesProcesser.GetUniqueFileName();
                    $"Eng/{uploadObject.engSeq}/{uniqueName}.jpg".SaveImageByBase64(imageArr[j].Replace(@"\", ""));
                    foreach(var controlStr in controlArr)
                    {
                        try
                        {
                            context.ConstCheckRecFile.Add(new ConstCheckRecFile
                            {
                                CreateTime = DateTime.Now,
                                CreateUserSeq = usrSeq,
                                UniqueFileName = uniqueName + ".jpg",
                                ConstCheckRecSeq = constCheck.Seq,
                                ControllStSeq = controlDic[controlStr].Seq

                            });
                        }
                        catch(Exception e)
                        {
                            throw new Exception($"{e.Message}:{controlDic.Aggregate("", (a, c) => a + c.Key + ",")} <====={controlStr}");
                        }

                    }

                }


            }


            //新增抽查結果

            foreach (var pair in uploadObject.standardResult)
            {
                context.ConstCheckRecResult.Add(new ConstCheckRecResult
                {
                    ConstCheckRecSeq = constCheck.Seq,
                    ControllStSeq = pair.Key,
                    CCRRealCheckCond = Convert.ToString(pair.Value[0]),
                    CCRCheckResult = (byte?)Convert.ToInt32(pair.Value[1])
                });
            }
            foreach (var seq in exceptControlSeqs)
            {
                context.ConstCheckRecResult.Add(new ConstCheckRecResult
                {
                    ConstCheckRecSeq = constCheck.Seq,
                    ControllStSeq = seq,
                    CCRRealCheckCond = "",
                    CCRCheckResult = 0
                });
            }
            context.SaveChanges();

            var updateSignatureDic =
                context.constCheckSignatures.Where(row => row.Token == token && row.ConstCheckSeq == 0)
                .GroupBy(r => r.SignatureRole)
                .ToDictionary(r => r.Key, r=> r.OrderBy(rr => rr.CreateTime).First() );


            
            if (uploadObject.signature != null)
            {
                uploadObject.signature.ToList()
                .ForEach(s =>
                {
                    if(updateSignatureDic.TryGetValue(s.SignatureRole, out constCheckSignatures value))
                    {
                        value.SignatureRole = s.SignatureRole;
                        value.SignatureVal = s.SignatureVal;
                        value.ModifyTime = DateTime.Now;
                        value.EngSeq = uploadObject.engSeq;
                        value.ConstCheckSeq = constCheck.Seq;
                        value.Token = token;
                    }
                    else
                    {
                        context.constCheckSignatures.Add(new constCheckSignatures
                        {
                            SignatureRole = s.SignatureRole,
                            SignatureVal = s.SignatureVal,
                            CreateTime = DateTime.Now,
                            EngSeq = uploadObject.engSeq,
                            ConstCheckSeq = constCheck.Seq,
                            Token = token,
                            SignatureImgeBase64 = ""
                        });
                    }

                });
            }
            if (updateSignatureDic.Count() > 0)
            {
                var updateSignature = updateSignatureDic.Select(r => r.Value)
                    .ToList();
                updateSignature.ForEach(s => s.ConstCheckSeq = constCheck.Seq);
                //儲存使用者簽名圖檔
                foreach (var signature in updateSignature)
                {
                    string signatureBase64 = signature.SignatureImgeBase64.Split(',')[1];
                    int? userSeq = context.UserMain.Where(row => row.DisplayName == signature.SignatureVal).FirstOrDefault()?.Seq;
                    if (userSeq != null)
                    {
                        string uniqueName = UploadFilesProcesser.GetUniqueFileName();
                        $"SignatureFiles/{userSeq}/{uniqueName}.png".SaveImageByBase64(signatureBase64.Replace(@"\", ""), 1);
                        SignatureFile signatureFile = context.SignatureFile.Where(r => r.UserMainSeq == userSeq).FirstOrDefault();
                        if (signatureFile == null)
                        {
                            context.SignatureFile.Add(new SignatureFile
                            {
                                UserMainSeq = userSeq,
                                FilePath = $@"\FileUploads\SignatureFiles\{userSeq}",
                                FileName = $"{ uniqueName}.png",
                                DisplayFileName = "簽名.jpg"
                            });
                        }
                        else
                        {
                            context.SignatureFile.Remove(signatureFile);
                            $@"SignatureFiles\{userSeq}\{signatureFile.FileName}".removeFile();
                            context.SignatureFile.Add(new SignatureFile
                            {
                                UserMainSeq = userSeq,
                                FilePath = $@"\FileUploads\SignatureFiles\{userSeq}",
                                FileName = $"{uniqueName}.jpg",
                                DisplayFileName = "簽名.jpg"
                            });
                        }

                    }


                }
            }

            context.SaveChanges();

            //產出PDF ，上傳工具包

            storePDFInToolPackage(uploadObject, constCheck.CCRPosDesc, constCheck.Seq);


            context.SaveChanges();

        }

        private int CheckType(int chapter)
        {
            switch (chapter)
            {
                case 6 : return 2;
                case 701: return 1;
                case 702: return 3;
                case 703: return 4;
            }
            return 0;
        }

        private void storePDFInToolPackage(UploadCheckJsonModel model, string positionDesc, int constCheckRecSeq)
        {
            List<EngConstructionEngInfoVModel> engItems = constCheckRecService.GetEngConstruction<EngConstructionEngInfoVModel>(model.engSeq, CheckType(model.chapter), model.constCheckSeq, false);
            if (engItems.Count == 0) return; 
            SignatureFileService signatureFileService = new SignatureFileService();
            SamplingInspectionRecController samplingInspectionRecController = new SamplingInspectionRecController();
            string uuid = Guid.NewGuid().ToString("B").ToUpper();
            string engItemSubEngName;
            if (engItems.Count() > 0)
                engItemSubEngName = engItems[0].subEngName;
            else return;
            foreach (EngConstructionEngInfoVModel engItem in engItems)
            {
                List<ConstCheckRecSheetModel> recItems = constCheckRecService.GetList1<ConstCheckRecSheetModel>(engItem.subEngNameSeq, model.constCheckSeq);
                var item = recItems.Where(r => r.Seq == constCheckRecSeq).FirstOrDefault();
 
                ConstCheckRecResultService service = new ConstCheckRecResultService();


                if (item != null)
                {
                    var users = context.UserMain.ToList();
                    var itemSignature =
                        context.constCheckSignatures.Where(r => r.ConstCheckSeq == constCheckRecSeq)
                        .GroupBy(r => r.SignatureRole)
                        .ToDictionary(r => r.Key, r => r.First());
                    //監造主任
                    if (itemSignature.ContainsKey(3))
                        item.SupervisorDirectorSeq = users.Where(r => r.DisplayName == itemSignature[3].SignatureVal).First().Seq;
                    //監造現場人員
                    if (itemSignature.ContainsKey(2))
                        item.SupervisorUserSeq = users.Where(r => r.DisplayName == itemSignature[2].SignatureVal).First().Seq;
                    service.Close();

                    item.GetControls(service);

                    samplingInspectionRecController.JoinCell(item.items);
                    if (item.CCRCheckType1 == 1)
                        samplingInspectionRecController.CheckSheet1(engItem, item, uuid, signatureFileService, 2);
                    else if (item.CCRCheckType1 == 2)
                        samplingInspectionRecController.CheckSheet2(engItem, item, uuid, signatureFileService, 2);
                    else if (item.CCRCheckType1 == 3)
                        samplingInspectionRecController.CheckSheet3(engItem, item, uuid, signatureFileService, 2);
                    else if (item.CCRCheckType1 == 4)
                        samplingInspectionRecController.CheckSheet4(engItem, item, uuid, signatureFileService, 2);
                }

            }



            string path = Path.Combine(Path.GetTempPath(), uuid) + "pdf";
            string pdfFilePath = Directory.GetFiles(path).First();
            string distPath = Path.Combine(UploadFilesProcesser.RootPath, $"MobileConstCheckDocuments/{model.engSeq}");
            if (!Directory.Exists(distPath)) Directory.CreateDirectory(distPath);
            File.Move(pdfFilePath, Path.Combine(distPath, Path.GetFileName(pdfFilePath)));
            //using (var context = new EQC_NEW_Entities())
            //{
            //    context.ToolPackage.Add(new ToolPackage
            //    {
            //        FileName = Path.GetFileName(pdfFilePath),
            //        EngSeq = model.engSeq,
            //        CreateTime = DateTime.Now,
            //        Description = $"{engItemSubEngName} {positionDesc}"
            //    });
            //    context.SaveChanges();
            //}

        }

        public class SignatureOption
        {
            public int SignatureRole { get; set; }
            public string SignatureVal { get; set; }
        }
        public List<SignatureOption> GetSignatureOption(string engNo, string userNo = null)
        {
            var eng = GetEngSeqByEngNo<EngMainEditVModel>(engNo).FirstOrDefault();
            int engSeq = eng?.Seq ?? 0;
            var userList =
                new EngMainService()
                .SupervisorUserList<EngSupervisorVModel>(engSeq)
                .Where(r => r.UserKind < 2)
                .Select(r => new EngSupervisorVModel { 
                    UserKind = (byte) (r.UserKind +2),
                    UserName = r.UserName
                    
                })
                .ToList();
            if (eng.BuildContractorContact != null)
            {
                userList.Add(new EngSupervisorVModel
                {
                    UserName = eng.BuildContractorContact,
                    UserKind = 4
                });
            }

            if (eng.SupervisorDirector != null)
            {
                userList.Add(new EngSupervisorVModel
                {
                    UserName = eng.SupervisorDirector,
                    UserKind = 2
                });
            }

            var constCheckUser = context.ConstCheckUser.Where(r => r.EngSeq == engSeq).FirstOrDefault()?.UserMain.DisplayName;
            if (constCheckUser != null)
            {
                userList.Add(new EngSupervisorVModel
                {
                    UserName = constCheckUser,
                    UserKind = 1
                });
            }
            if(userNo != null)
            {
                var user = context.UserMain.Where(r => r.UserNo == userNo).First().DisplayName ;
                userList.Add(new EngSupervisorVModel
                {
                    UserName = user,
                    UserKind = 1
                });
            }
            return userList.Select(r => new SignatureOption
            {
                SignatureRole = r.UserKind == 2 ? 3 : (r.UserKind == 3 ? 2 : r.UserKind),
                SignatureVal = r.UserName
            }).ToList();
        }


    }
}
