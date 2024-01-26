using EQC.Models;

namespace EQC.ViewModel
{
    public class RejectCompanyVModel : RVFileModel
    {//拒絕往來廠商
        public string Effective_DateStr
        {
            get {
                if (Effective_Date.HasValue)
                    return Effective_Date.Value.ToString("yyyy-MM-dd");
                else
                    return "";
            }
        }
        public string Expire_DateStr
        {
            get
            {
                if (Expire_Date.HasValue)
                    return Expire_Date.Value.ToString("yyyy-MM-dd");
                else
                    return "";
            }
        }
    }
}
