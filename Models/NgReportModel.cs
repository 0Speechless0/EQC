using EQC.Common;
using EQC.Services;
using System;
using System.Collections.Generic;

namespace EQC.Models
{
    public class NgReportModel : ConstCheckRecImproveModel
    {//不符合事項報告        
        public byte CCRCheckType1 { get; set; }
        public string CCRPosDesc { get; set; }
        public int? SupervisorUserSeq { get; set; }
        public int? SupervisorDirectorSeq { get; set; }
        public string CCRRealCheckCond { get; set; }//實際抽查情形
        
        public DateTime? CCRCheckDate { get; set;  } //檢查日期

        //取得 照片群組清單
        public List<NgReportPhotoGroupModel> photoGroups = new List<NgReportPhotoGroupModel>();
        public void GetImgGroupOption(ConstCheckRecImproveService constCheckRecImproveService)
        {
            if (CCRCheckType1 == 1)
            {//施工抽查
                photoGroups = constCheckRecImproveService.GetConstCheckPhotoGroupOption<NgReportPhotoGroupModel>(ConstCheckRecSeq);
            }
            else if (CCRCheckType1 == 2)
            {//設備運轉
                photoGroups = constCheckRecImproveService.GetEquOperPhotoGroupOption<NgReportPhotoGroupModel>(ConstCheckRecSeq);
            }
            else if (CCRCheckType1 == 3)
            {//職業安全衛生
                photoGroups = constCheckRecImproveService.GetOccuSafeHealthGroupOption<NgReportPhotoGroupModel>(ConstCheckRecSeq);
            }
            else if (CCRCheckType1 == 4)
            {//環境保育
                photoGroups = constCheckRecImproveService.GetEnvirConsGroupOption<NgReportPhotoGroupModel>(ConstCheckRecSeq);
            }

            foreach(NgReportPhotoGroupModel m in photoGroups)
            {
                m.GetRecResultPhotos(ConstCheckRecSeq);
            }
        }
    }
}
