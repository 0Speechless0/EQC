using System;
using System.Collections.Generic;

namespace EQC.Models
{
    public class TenderModel:PrjXMLModel
    {//標案資料
        //PrjXMLExt
        public PrjXMLExtModel PrjXMLExt = new PrjXMLExtModel();
        //廠商聘用品管人員
        public List<ContractorQualityControlModel> ContractorQualityControl = new List<ContractorQualityControlModel>();
        //監造單位聘用監工人員
        public List<SupervisorModel> Supervisor = new List<SupervisorModel>();
        //專任工程人員
        public List<FullTimeEngineerModel> FullTimeEngineer = new List<FullTimeEngineerModel>();
        //工地相關人員
        public List<SiteRelateModel> SiteRelate = new List<SiteRelateModel>();
        //預算編列
        public List<BudgetingModel> Budgeting = new List<BudgetingModel> ();
        //變更設計資料
        public List<ChangeDesignDataModel> ChangeDesignData = new List<ChangeDesignDataModel>();
        //進度資料
        public List<ProgressDataModel> ProgressData = new List<ProgressDataModel>();
        //落後資料
        public List<BackwardDataModel> BackwardData = new List<BackwardDataModel>();
        //歷次付款資料
        public List<PaymentRecordModel> PaymentRecord = new List<PaymentRecordModel>();
        //儲存成功與否
        public int UpdateState { get; set; }
        public string UpdateMsg { get; set; }
    }
}
