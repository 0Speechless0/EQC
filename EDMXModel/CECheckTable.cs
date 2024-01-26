//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace EQC.EDMXModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class CECheckTable
    {
        public int Seq { get; set; }
        public int CarbonEmissionHeaderSeq { get; set; }
        public Nullable<byte> F1101 { get; set; }
        public Nullable<byte> F1102 { get; set; }
        public Nullable<byte> F1103 { get; set; }
        public Nullable<byte> F1104 { get; set; }
        public Nullable<byte> F1105 { get; set; }
        public Nullable<byte> F1106 { get; set; }
        public string F1106TreeJson { get; set; }
        public Nullable<byte> F1107 { get; set; }
        public string F1107Desc { get; set; }
        public Nullable<byte> F1201 { get; set; }
        public Nullable<byte> F1201Mix { get; set; }
        public string F1201Other { get; set; }
        public Nullable<byte> F1202 { get; set; }
        public Nullable<byte> F1203 { get; set; }
        public Nullable<byte> F1204 { get; set; }
        public Nullable<byte> F1205 { get; set; }
        public Nullable<byte> F1206 { get; set; }
        public string F1206Desc { get; set; }
        public Nullable<byte> F1301 { get; set; }
        public Nullable<byte> F1302 { get; set; }
        public Nullable<byte> F1303 { get; set; }
        public Nullable<byte> F1304 { get; set; }
        public Nullable<byte> F1305 { get; set; }
        public Nullable<byte> F1306 { get; set; }
        public Nullable<byte> F1306Mode { get; set; }
        public Nullable<byte> F1307 { get; set; }
        public Nullable<byte> F1307Mode { get; set; }
        public Nullable<byte> F1308 { get; set; }
        public Nullable<byte> F1309 { get; set; }
        public Nullable<byte> F1309Mode { get; set; }
        public Nullable<byte> F1310 { get; set; }
        public Nullable<byte> F1310Mode { get; set; }
        public Nullable<byte> F1311 { get; set; }
        public Nullable<byte> F1312 { get; set; }
        public string F1312Desc { get; set; }
        public Nullable<byte> F1401 { get; set; }
        public Nullable<byte> F1402 { get; set; }
        public Nullable<byte> F1403 { get; set; }
        public Nullable<byte> F1404 { get; set; }
        public string F1404Desc { get; set; }
        public Nullable<byte> F2101 { get; set; }
        public Nullable<byte> F2102 { get; set; }
        public Nullable<byte> F2103 { get; set; }
        public string F2103Desc { get; set; }
        public Nullable<byte> F2201 { get; set; }
        public Nullable<byte> F2202 { get; set; }
        public Nullable<byte> F2203 { get; set; }
        public Nullable<byte> F2204 { get; set; }
        public string F2204Desc { get; set; }
        public Nullable<byte> F2301 { get; set; }
        public Nullable<byte> F2302 { get; set; }
        public Nullable<byte> F2303 { get; set; }
        public string F2303Desc { get; set; }
        public Nullable<byte> F3101 { get; set; }
        public Nullable<byte> F3102 { get; set; }
        public Nullable<byte> F3103 { get; set; }
        public Nullable<byte> F3104 { get; set; }
        public string F3104Desc { get; set; }
        public Nullable<byte> F3201 { get; set; }
        public Nullable<byte> F3202 { get; set; }
        public Nullable<byte> F3203 { get; set; }
        public Nullable<byte> F3204 { get; set; }
        public string F3204Desc { get; set; }
        public Nullable<byte> F3301 { get; set; }
        public Nullable<byte> F3302 { get; set; }
        public string F3302Desc { get; set; }
        public Nullable<byte> F3401 { get; set; }
        public Nullable<byte> F3402 { get; set; }
        public Nullable<byte> F3403 { get; set; }
        public string F3403Desc { get; set; }
        public string Signature { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<int> CreateUserSeq { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
        public Nullable<int> ModifyUserSeq { get; set; }
        public Nullable<byte> F1108 { get; set; }
        public Nullable<int> F1108Area { get; set; }
        public Nullable<byte> F1109 { get; set; }
        public Nullable<int> F1109Length { get; set; }
        public Nullable<byte> F1110 { get; set; }
        public string F1110Desc { get; set; }
        public string F1107TreeJson { get; set; }
        public int F1107TreeTotal { get; set; }
        public string Remark { get; set; }
    
        public virtual CarbonEmissionHeader CarbonEmissionHeader { get; set; }
    }
}