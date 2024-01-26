using EQC.Common;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace EQC.Models
{
    public class ConstCheckRecSheetModel : ConstCheckRecModel
    {//抽驗紀錄
        public string CheckItemName { get; set; } //抽查項目名稱 for itemSeq(清冊的Seq)
        public List<ControlStVModel> items = new List<ControlStVModel>();
        //檢驗紀錄
        public void GetControls(ConstCheckRecResultService service)
        {
            if (CCRCheckType1 == 1)
            {//施工抽查清單
                items = service.GetConstCheckRecResult<ControlStVModel>(Seq);
                List<ConstCheckListModel> sList = new ConstCheckListService().GetItemBySeq<ConstCheckListModel>(ItemSeq);
                if (sList.Count == 1) CheckItemName = sList[0].ItemName;
            }
            else if (CCRCheckType1 == 2)
            {//設備運轉測試清單
                items = service.GetEquOperRecResult<ControlStVModel>(Seq);
            }
            else if (CCRCheckType1 == 3)
            {//職業安全衛生清單
                items = service.GetOccuSafeHealthRecResult<ControlStVModel>(Seq);
            }
            else if (CCRCheckType1 == 4)
            {//環境保育清單
                items = service.GetEnvirConsRecResult<ControlStVModel>(Seq);
            }
            foreach(ControlStVModel m in items)
                Utils.Null2Empty(m);
        }

        private string filePathName = null;
        private string filePatdownloadhName = null;
        public string GetFilename(string uuidFolder)
        {
            if (filePathName == null)
            {
                string uuid = "N";
                if (CCRCheckType1 == 1)
                    uuid = "施工抽查";
                else if (CCRCheckType1 == 2)
                    uuid = "設備運轉";
                else if (CCRCheckType1 == 3)
                    uuid = "職業安全";
                else if (CCRCheckType1 == 4)
                    uuid = "環境保育";
                uuid = String.Format("{0}-{1}{2:00}{3:00}-{4}.docx", uuid, CCRCheckDate.Year-1911, CCRCheckDate.Month, CCRCheckDate.Day, Seq);
                
                filePathName = Path.Combine(Path.GetTempPath(), uuidFolder);
                if(!Directory.Exists(filePathName)) Directory.CreateDirectory(filePathName);
                filePathName = Path.Combine(filePathName, uuid);
            }
            return filePathName;
        }

        public string Getdownloadname(string uuidFolder)
        {
            if (filePatdownloadhName == null)
            {
                string uuid = "N";
                if (CCRCheckType1 == 1)
                    uuid = "施工抽查";
                else if (CCRCheckType1 == 2)
                    uuid = "設備運轉";
                else if (CCRCheckType1 == 3)
                    uuid = "職業安全";
                else if (CCRCheckType1 == 4)
                    uuid = "環境保育";
                uuid = String.Format("{0}-{1}{2:00}{3:00}-{4}.docx", uuid, CCRCheckDate.Year - 1911, CCRCheckDate.Month, CCRCheckDate.Day, Seq);

                filePatdownloadhName = Path.Combine(Path.GetTempPath(), uuidFolder);
                if (!Directory.Exists(filePatdownloadhName)) Directory.CreateDirectory(filePatdownloadhName);
                filePatdownloadhName = uuid;
            }
            return filePatdownloadhName;
        }

    }
}
