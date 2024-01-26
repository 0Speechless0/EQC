﻿using EQC.Common;
using System;

namespace EQC.Models
{
    public class ConstCheckRecModel
    {//抽驗紀錄填報
        public int Seq { get; set; }
        public int EngConstructionSeq { get; set; }
        public byte CCRCheckType1 { get; set; }
        public int ItemSeq { get; set; }
        public byte CCRCheckFlow { get; set; }
        public DateTime CCRCheckDate { get; set; }
        public decimal? CCRPosLati { get; set; }
        public decimal? CCRPosLong { get; set; }
        public string CCRPosDesc { get; set; }
        public bool? IsManageConfirm { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
        public byte? FormConfirm { get; set; }
        public int? SupervisorUserSeq { get; set; }
        public int? SupervisorDirectorSeq { get; set; }

        public string chsCheckDate
        {
            get
            {
                return Utils.ChsDate(this.CCRCheckDate);
            }
        }
    }
}
