﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EQC_NEW_Entities : DbContext
    {
        public EQC_NEW_Entities()
            : base("name=EQC_NEW_Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<PttCheckRecord> PttCheckRecord { get; set; }
        public virtual DbSet<PttMain> PttMain { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<UserMain> UserMain { get; set; }
        public virtual DbSet<UserUnitPosition> UserUnitPosition { get; set; }
        public virtual DbSet<TreeList> TreeList { get; set; }
        public virtual DbSet<TreePlantEngType> TreePlantEngType { get; set; }
        public virtual DbSet<TreePlantMain> TreePlantMain { get; set; }
        public virtual DbSet<TreePlantMonth> TreePlantMonth { get; set; }
        public virtual DbSet<TreePlantNumList> TreePlantNumList { get; set; }
        public virtual DbSet<TreePlantType> TreePlantType { get; set; }
        public virtual DbSet<SchProgressHeader> SchProgressHeader { get; set; }
        public virtual DbSet<SchProgressHeaderHistory> SchProgressHeaderHistory { get; set; }
        public virtual DbSet<SchProgressPayItem> SchProgressPayItem { get; set; }
        public virtual DbSet<SupDailyDate> SupDailyDate { get; set; }
        public virtual DbSet<SupPlanOverview> SupPlanOverview { get; set; }
        public virtual DbSet<SchProgressHeaderHistoryProgress> SchProgressHeaderHistoryProgress { get; set; }
        public virtual DbSet<Town> Town { get; set; }
        public virtual DbSet<Unit> Unit { get; set; }
        public virtual DbSet<EngMain> EngMain { get; set; }
        public virtual DbSet<ApprovalModuleList> ApprovalModuleList { get; set; }
        public virtual DbSet<ApprovingUnitType> ApprovingUnitType { get; set; }
        public virtual DbSet<EYCentralLocalAddressList> EYCentralLocalAddressList { get; set; }
        public virtual DbSet<Gravelfieldcoord> Gravelfieldcoord { get; set; }
        public virtual DbSet<NationalSupervisedActivity> NationalSupervisedActivity { get; set; }
        public virtual DbSet<PublicWorkFirmResume> PublicWorkFirmResume { get; set; }
        public virtual DbSet<VendorHireWorkList> VendorHireWorkList { get; set; }
        public virtual DbSet<wraControlPlanNo> wraControlPlanNo { get; set; }
        public virtual DbSet<Country2WRAMapping> Country2WRAMapping { get; set; }
        public virtual DbSet<AmmeterRecord> AmmeterRecord { get; set; }
        public virtual DbSet<Position> Position { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<PCCESPayItem> PCCESPayItem { get; set; }
        public virtual DbSet<PCCESSMain> PCCESSMain { get; set; }
        public virtual DbSet<SchEngProgressHeader> SchEngProgressHeader { get; set; }
        public virtual DbSet<SchEngProgressPayItem> SchEngProgressPayItem { get; set; }
        public virtual DbSet<PCCESWorkItem> PCCESWorkItem { get; set; }
        public virtual DbSet<CarbonEmissionFactor> CarbonEmissionFactor { get; set; }
        public virtual DbSet<EngRiskFrontSubProjectListTp> EngRiskFrontSubProjectListTp { get; set; }
        public virtual DbSet<EngRiskFrontHazardType> EngRiskFrontHazardType { get; set; }
        public virtual DbSet<EngRiskFrontSubProjectDetailTp> EngRiskFrontSubProjectDetailTp { get; set; }
        public virtual DbSet<ConstCheckUser> ConstCheckUser { get; set; }
        public virtual DbSet<ToolPackage> ToolPackage { get; set; }
        public virtual DbSet<ConstCheckRec> ConstCheckRec { get; set; }
        public virtual DbSet<ConstCheckRecFile> ConstCheckRecFile { get; set; }
        public virtual DbSet<ConstCheckRecResult> ConstCheckRecResult { get; set; }
        public virtual DbSet<CarbonReductionFactor> CarbonReductionFactor { get; set; }
        public virtual DbSet<CarbonReductionNavvyFactor> CarbonReductionNavvyFactor { get; set; }
        public virtual DbSet<CarbonReductionTruckFactor> CarbonReductionTruckFactor { get; set; }
        public virtual DbSet<DrainList> DrainList { get; set; }
        public virtual DbSet<EngReportApprove> EngReportApprove { get; set; }
        public virtual DbSet<EngReportEstimatedCost> EngReportEstimatedCost { get; set; }
        public virtual DbSet<EngReportList> EngReportList { get; set; }
        public virtual DbSet<EngReportLocalCommunication> EngReportLocalCommunication { get; set; }
        public virtual DbSet<EngReportMainJobDescription> EngReportMainJobDescription { get; set; }
        public virtual DbSet<EngReportOnSiteConsultation> EngReportOnSiteConsultation { get; set; }
        public virtual DbSet<EngReportOption> EngReportOption { get; set; }
        public virtual DbSet<EngReportType> EngReportType { get; set; }
        public virtual DbSet<ProposalReviewAttributes> ProposalReviewAttributes { get; set; }
        public virtual DbSet<ProposalReviewType> ProposalReviewType { get; set; }
        public virtual DbSet<ReportJobDescriptionList> ReportJobDescriptionList { get; set; }
        public virtual DbSet<RiverList> RiverList { get; set; }
        public virtual DbSet<EngRiskFrontSubProjectDetail> EngRiskFrontSubProjectDetail { get; set; }
        public virtual DbSet<EngRiskFrontSubProjectList> EngRiskFrontSubProjectList { get; set; }
        public virtual DbSet<ApproverList> ApproverList { get; set; }
        public virtual DbSet<UserLoginRecord> UserLoginRecord { get; set; }
        public virtual DbSet<EngMaterialDeviceList> EngMaterialDeviceList { get; set; }
        public virtual DbSet<EngMaterialDeviceSummary> EngMaterialDeviceSummary { get; set; }
        public virtual DbSet<EngMaterialDeviceTestSummary> EngMaterialDeviceTestSummary { get; set; }
        public virtual DbSet<SignatureFile> SignatureFile { get; set; }
        public virtual DbSet<ConstCheckList> ConstCheckList { get; set; }
        public virtual DbSet<EnvirConsList> EnvirConsList { get; set; }
        public virtual DbSet<EquOperTestList> EquOperTestList { get; set; }
        public virtual DbSet<OccuSafeHealthList> OccuSafeHealthList { get; set; }
        public virtual DbSet<EngConstruction> EngConstruction { get; set; }
        public virtual DbSet<CarbonEmissionPayItem> CarbonEmissionPayItem { get; set; }
        public virtual DbSet<constCheckSignatures> constCheckSignatures { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<UserNotification> UserNotification { get; set; }
        public virtual DbSet<ConstCheckAppLock> ConstCheckAppLock { get; set; }
        public virtual DbSet<CarbonEmissionHeader> CarbonEmissionHeader { get; set; }
        public virtual DbSet<PrjXML> PrjXML { get; set; }
        public virtual DbSet<ProgressData> ProgressData { get; set; }
        public virtual DbSet<CarbonReductionCalResult> CarbonReductionCalResult { get; set; }
        public virtual DbSet<EnvironmentVar> EnvironmentVar { get; set; }
        public virtual DbSet<SupDailyReportConstructionEquipment> SupDailyReportConstructionEquipment { get; set; }
        public virtual DbSet<SupDailyReportConstructionMaterial> SupDailyReportConstructionMaterial { get; set; }
        public virtual DbSet<SupDailyReportConstructionPerson> SupDailyReportConstructionPerson { get; set; }
        public virtual DbSet<EC_SupDailyDate> EC_SupDailyDate { get; set; }
        public virtual DbSet<PrjXMLTag> PrjXMLTag { get; set; }
        public virtual DbSet<CECheckTable> CECheckTable { get; set; }
        public virtual DbSet<Suggestion> Suggestion { get; set; }
        public virtual DbSet<SuggestionClass> SuggestionClass { get; set; }
        public virtual DbSet<SuggestionHead> SuggestionHead { get; set; }
        public virtual DbSet<AuditCaseList> AuditCaseList { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<APIRecord> APIRecord { get; set; }
        public virtual DbSet<CarbonReductionCal> CarbonReductionCal { get; set; }
    }
}
