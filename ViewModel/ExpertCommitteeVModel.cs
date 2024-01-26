using System;

namespace EQC.Models
{//專家委員
    public class ExpertCommitteeVModel : ExpertCommitteeModel
    {
        public string ECBirthdayStr {
            get {
                if (ECBirthday.HasValue)
                    return ECBirthday.Value.ToString("yyyy-MM-dd");
                else
                    return null;
            }
        }
    }
}