
using EQC.Services;
using System.Collections.Generic;

namespace EQC.Models
{
    public class NgReportPhotoGroupModel
    {
        public string Text { get; set; }
        public int Value { get; set; }

        //取得 照片清單 
        public List<UploadFileModel> photos = new List<UploadFileModel>();
        public void GetRecResultPhotos(int recSeq)
        {
            photos = new ConstCheckRecImproveFileService().GetPhotos<UploadFileModel>(recSeq, Value);
        }
    }
}
