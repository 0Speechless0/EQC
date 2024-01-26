using System;

namespace EQC.Models
{
    public class AppInstallModel
    {
        public int Seq { get; set; }
        public int PhoneType { get; set; }//0：安卓 1：IOS 
        public string Link { get; set; }
    }
}
