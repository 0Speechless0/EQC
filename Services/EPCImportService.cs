using EQC.Common;
using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public  class EPCImportService : BaseService
    {//標案資料匯入
        
        //標案資料是否已存在
        public int GetSeqByTenderNo(string no, string execUnitCd)
        {
            string sql = @"SELECT Seq FROM PrjXML where TenderNo=@TenderNo and ExecUnitCd = @ExecUnitCd";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@TenderNo", no);
            cmd.Parameters.AddWithValue("@ExecUnitCd", execUnitCd);
            DataTable dt = db.GetDataTable(cmd);
            if (dt.Rows.Count == 0)
                return -1;
            else
                return Convert.ToInt32(dt.Rows[0]["Seq"].ToString());
        }

        public bool updatePrjFromXML(TenderModel m)
        {
            int prjSeq = GetSeqByTenderNo(m.TenderNo, m.ExecUnitCd);
            if (prjSeq == -1)
                return AddPrjAll(m);
            else
            {
                m.Seq = prjSeq;
                return updatePrjAll(m);
            }
        }
        public bool AddPrjAll(TenderModel m)
        {
            string sql = "";
            Null2Empty(m);
            db.BeginTransaction();
            try
            {
                sql = @"
                    insert into PrjXML(
                        TenderNo,
                        ExecUnitCd,
                        ExecUnitName,
                        TenderName,
                        PrjName,
                        EngType,
                        TownName,
                        -- CoordX,
                         -- CoordY,
                        Location,
                        PlanOrganizerName,
                        PlanNo,
                        CompetentAuthority,
                        OrganizerName,
                        FundingSourceName,
                        TenderNoticeUnit,
                        ContactName,
                        ContactPhone,
                        ContactEmail,
                        Weights,
                        EngOverview,
                        SteelDemand,
                        ConcreteDemand,
                        EarchworkDemand,
                        DurationCategory,
                        TotalDays,
                        DurationDesc,
                        BudgetAccount,
                        TotalEngBudget,
                        OutsourcingBudget,
                        SupplyMaterialCost,
                        LandPurCompen,
                        EngManageFee,
                        AirPollutionFee,
                        OtherFee,
                        PlanningUnitName,
                        DesignUnitName,
                        DesignFee,
                        DesignMemo,
                        SupervisionUnitName,
                        SupervisionFee,
                        SupervisionMemo,
                        ContractorName1,
                        ContractorName2,
                        InsuranceDate,
                        InsuranceAmount,
                        InsuranceNo,
                        ActualAnnoDate,
                        ScheBidReviewDate,
                        ActualBidReviewDate,
                        ScheBidOpeningDate,
                        ActualBidOpeningDate,
                        ScheBidAwardDate,
                        ActualBidAwardDate,
                        ScheBiddingMethod,
                        ActualBiddingMethod,
                        BidAwardMethod,
                        ContractFeePayMethod,
                        EstimateBasePrice,
                        RendBasePrice,
                        BidAmount,
                        ContractNo,
                        Prepayment,
                        ScheStartDate,
                        ActualStartDate,
                        ScheCompletionDate,
                        ScheCompletCloseDate,
                        QualityControlFee,
                        QualityPlanApproveUnit,
                        QualityPlanApproveDate,
                        QualityPlanApproveNo,
                        SupervisionPlanApproveUnit,
                        SupervisionPlanApproveDate,
                        SupervisionPlanApproveNo,
                        SiteContactMemo,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq,
                        TenderYear
                    ) values (
                        @TenderNo,
                        @ExecUnitCd,
                        @ExecUnitName,
                        @TenderName,
                        @PrjName,
                        @EngType,
                        @TownName,
                         -- @CoordX,
                         -- @CoordY,
                        @Location,
                        @PlanOrganizerName,
                        @PlanNo,
                        @CompetentAuthority,
                        @OrganizerName,
                        @FundingSourceName,
                        @TenderNoticeUnit,
                        @ContactName,
                        @ContactPhone,
                        @ContactEmail,
                        @Weights,
                        @EngOverview,
                        @SteelDemand,
                        @ConcreteDemand,
                        @EarchworkDemand,
                        @DurationCategory,
                        @TotalDays,
                        @DurationDesc,
                        @BudgetAccount,
                        @TotalEngBudget,
                        @OutsourcingBudget,
                        @SupplyMaterialCost,
                        @LandPurCompen,
                        @EngManageFee,
                        @AirPollutionFee,
                        @OtherFee,
                        @PlanningUnitName,
                        @DesignUnitName,
                        @DesignFee,
                        @DesignMemo,
                        @SupervisionUnitName,
                        @SupervisionFee,
                        @SupervisionMemo,
                        @ContractorName1,
                        @ContractorName2,
                        @InsuranceDate,
                        @InsuranceAmount,
                        @InsuranceNo,
                        @ActualAnnoDate,
                        @ScheBidReviewDate,
                        @ActualBidReviewDate,
                        @ScheBidOpeningDate,
                        @ActualBidOpeningDate,
                        @ScheBidAwardDate,
                        @ActualBidAwardDate,
                        @ScheBiddingMethod,
                        @ActualBiddingMethod,
                        @BidAwardMethod,
                        @ContractFeePayMethod,
                        @EstimateBasePrice,
                        @RendBasePrice,
                        @BidAmount,
                        @ContractNo,
                        @Prepayment,
                        @ScheStartDate,
                        @ActualStartDate,
                        @ScheCompletionDate,
                        @ScheCompletCloseDate,
                        @QualityControlFee,
                        @QualityPlanApproveUnit,
                        @QualityPlanApproveDate,
                        @QualityPlanApproveNo,
                        @SupervisionPlanApproveUnit,
                        @SupervisionPlanApproveDate,
                        @SupervisionPlanApproveNo,
                        @SiteContactMemo,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq,
                        @TenderYear
                    )";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@TenderNo", m.TenderNo);
                cmd.Parameters.AddWithValue("@ExecUnitCd", m.ExecUnitCd);
                cmd.Parameters.AddWithValue("@ExecUnitName", m.ExecUnitName);
                cmd.Parameters.AddWithValue("@TenderName", m.TenderName);
                cmd.Parameters.AddWithValue("@PrjName", m.PrjName);
                cmd.Parameters.AddWithValue("@EngType", m.EngType);
                cmd.Parameters.AddWithValue("@TownName", m.TownName);
                //cmd.Parameters.AddWithValue("@CoordX", this.NulltoDBNull(m.CoordX));
                //cmd.Parameters.AddWithValue("@CoordY", this.NulltoDBNull(m.CoordY));
                cmd.Parameters.AddWithValue("@Location", m.Location);
                cmd.Parameters.AddWithValue("@PlanOrganizerName", m.PlanOrganizerName);
                cmd.Parameters.AddWithValue("@PlanNo", m.PlanNo);
                cmd.Parameters.AddWithValue("@CompetentAuthority", m.CompetentAuthority);
                cmd.Parameters.AddWithValue("@OrganizerName", m.OrganizerName);
                cmd.Parameters.AddWithValue("@FundingSourceName", m.FundingSourceName);
                cmd.Parameters.AddWithValue("@TenderNoticeUnit", m.TenderNoticeUnit);
                cmd.Parameters.AddWithValue("@ContactName", m.ContactName);
                cmd.Parameters.AddWithValue("@ContactPhone", m.ContactPhone);
                cmd.Parameters.AddWithValue("@ContactEmail", m.ContactEmail);
                cmd.Parameters.AddWithValue("@Weights", m.Weights);
                cmd.Parameters.AddWithValue("@EngOverview", m.EngOverview);
                cmd.Parameters.AddWithValue("@SteelDemand", this.NulltoDBNull(m.SteelDemand));
                cmd.Parameters.AddWithValue("@ConcreteDemand", this.NulltoDBNull(m.ConcreteDemand));
                cmd.Parameters.AddWithValue("@EarchworkDemand", this.NulltoDBNull(m.EarchworkDemand));
                cmd.Parameters.AddWithValue("@DurationCategory", m.DurationCategory);
                cmd.Parameters.AddWithValue("@TotalDays", this.NulltoDBNull(m.TotalDays));
                cmd.Parameters.AddWithValue("@DurationDesc", m.DurationDesc);
                cmd.Parameters.AddWithValue("@BudgetAccount", m.BudgetAccount);
                cmd.Parameters.AddWithValue("@TotalEngBudget", this.NulltoDBNull(m.TotalEngBudget));
                cmd.Parameters.AddWithValue("@OutsourcingBudget", this.NulltoDBNull(m.OutsourcingBudget));
                cmd.Parameters.AddWithValue("@SupplyMaterialCost", this.NulltoDBNull(m.SupplyMaterialCost));
                cmd.Parameters.AddWithValue("@LandPurCompen", this.NulltoDBNull(m.LandPurCompen));
                cmd.Parameters.AddWithValue("@EngManageFee", this.NulltoDBNull(m.EngManageFee));
                cmd.Parameters.AddWithValue("@AirPollutionFee", this.NulltoDBNull(m.AirPollutionFee));
                cmd.Parameters.AddWithValue("@OtherFee", this.NulltoDBNull(m.OtherFee));
                cmd.Parameters.AddWithValue("@PlanningUnitName", m.PlanningUnitName);
                cmd.Parameters.AddWithValue("@DesignUnitName", m.DesignUnitName);
                cmd.Parameters.AddWithValue("@DesignFee", this.NulltoDBNull(m.DesignFee));
                cmd.Parameters.AddWithValue("@DesignMemo", m.DesignMemo);
                cmd.Parameters.AddWithValue("@SupervisionUnitName", m.SupervisionUnitName);
                cmd.Parameters.AddWithValue("@SupervisionFee", this.NulltoDBNull(m.SupervisionFee));
                cmd.Parameters.AddWithValue("@SupervisionMemo", m.SupervisionMemo);
                cmd.Parameters.AddWithValue("@ContractorName1", m.ContractorName1);
                cmd.Parameters.AddWithValue("@ContractorName2", m.ContractorName2);
                cmd.Parameters.AddWithValue("@InsuranceDate", m.InsuranceDate);
                cmd.Parameters.AddWithValue("@InsuranceAmount", this.NulltoDBNull(m.InsuranceAmount));
                cmd.Parameters.AddWithValue("@InsuranceNo", m.InsuranceNo);
                cmd.Parameters.AddWithValue("@ActualAnnoDate", m.ActualAnnoDate);
                cmd.Parameters.AddWithValue("@ScheBidReviewDate", m.ScheBidReviewDate);
                cmd.Parameters.AddWithValue("@ActualBidReviewDate", m.ActualBidReviewDate);
                cmd.Parameters.AddWithValue("@ScheBidOpeningDate", m.ScheBidOpeningDate);
                cmd.Parameters.AddWithValue("@ActualBidOpeningDate", m.ActualBidOpeningDate);
                cmd.Parameters.AddWithValue("@ScheBidAwardDate", m.ScheBidAwardDate);
                cmd.Parameters.AddWithValue("@ActualBidAwardDate", m.ActualBidAwardDate);
                cmd.Parameters.AddWithValue("@ScheBiddingMethod", m.ScheBiddingMethod);
                cmd.Parameters.AddWithValue("@ActualBiddingMethod", m.ActualBiddingMethod);
                cmd.Parameters.AddWithValue("@BidAwardMethod", m.BidAwardMethod);
                cmd.Parameters.AddWithValue("@ContractFeePayMethod", m.ContractFeePayMethod);
                cmd.Parameters.AddWithValue("@EstimateBasePrice", this.NulltoDBNull(m.EstimateBasePrice));
                cmd.Parameters.AddWithValue("@RendBasePrice", this.NulltoDBNull(m.RendBasePrice));
                cmd.Parameters.AddWithValue("@BidAmount", this.NulltoDBNull(m.BidAmount));
                cmd.Parameters.AddWithValue("@ContractNo", m.ContractNo);
                cmd.Parameters.AddWithValue("@Prepayment", this.NulltoDBNull(m.Prepayment));
                cmd.Parameters.AddWithValue("@ScheStartDate", m.ScheStartDate);
                cmd.Parameters.AddWithValue("@ActualStartDate", m.ActualStartDate);
                cmd.Parameters.AddWithValue("@ScheCompletionDate", m.ScheCompletionDate);
                cmd.Parameters.AddWithValue("@ScheCompletCloseDate", m.ScheCompletCloseDate);
                cmd.Parameters.AddWithValue("@QualityControlFee", this.NulltoDBNull(m.QualityControlFee));
                cmd.Parameters.AddWithValue("@QualityPlanApproveUnit", m.QualityPlanApproveUnit);
                cmd.Parameters.AddWithValue("@QualityPlanApproveDate", m.QualityPlanApproveDate);
                cmd.Parameters.AddWithValue("@QualityPlanApproveNo", m.QualityPlanApproveNo);
                cmd.Parameters.AddWithValue("@SupervisionPlanApproveUnit", m.SupervisionPlanApproveUnit);
                cmd.Parameters.AddWithValue("@SupervisionPlanApproveDate", m.SupervisionPlanApproveDate);
                cmd.Parameters.AddWithValue("@SupervisionPlanApproveNo", m.SupervisionPlanApproveNo);
                cmd.Parameters.AddWithValue("@SiteContactMemo", m.SiteContactMemo);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@TenderYear", this.NulltoDBNull(m.TenderYear));

                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('PrjXML') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);
                m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                AddContractorQualityControl(m.ContractorQualityControl, m.Seq);
                AddSupervisor(m.Supervisor, m.Seq);
                AddFullTimeEngineer(m.FullTimeEngineer, m.Seq);
                AddSiteRelate(m.SiteRelate, m.Seq);
                AddBudgeting(m.Budgeting, m.Seq);
                AddChangeDesignData(m.ChangeDesignData, m.Seq);
                AddProgressData(m.ProgressData, m.Seq);
                AddBackwardData(m.BackwardData, m.Seq);
                AddPaymentRecord(m.PaymentRecord, m.Seq);

                db.TransactionCommit();
                return true;
            } catch(Exception e) {
                db.TransactionRollback();
                m.UpdateMsg = e.Message;
                log.Info("EPCImportService.AddPrjAll:[" + m.TenderNo+"] " + e.Message);
                log.Info(sql);
                return false;
            }
        }
        public bool updatePrjAll(TenderModel m)
        {
            string sql = "";
            Null2Empty(m);
            db.BeginTransaction();
            try
            {
                sql = @"delete from ContractorQualityControl where PrjXMLSeq=@PrjXMLSeq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", m.Seq);
                db.ExecuteNonQuery(cmd);

                sql = @"delete from Supervisor where PrjXMLSeq=@PrjXMLSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", m.Seq);
                db.ExecuteNonQuery(cmd);

                sql = @"delete from FullTimeEngineer where PrjXMLSeq=@PrjXMLSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", m.Seq);
                db.ExecuteNonQuery(cmd);

                sql = @"delete from SiteRelate where PrjXMLSeq=@PrjXMLSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", m.Seq);
                db.ExecuteNonQuery(cmd);

                sql = @"delete from BudgetKind where BudgetingSeq in
                        (select Seq from Budgeting where PrjXMLSeq=@PrjXMLSeq)";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", m.Seq);
                db.ExecuteNonQuery(cmd);

                sql = @"delete from Budgeting where PrjXMLSeq=@PrjXMLSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", m.Seq);
                db.ExecuteNonQuery(cmd);

                sql = @"delete from ChangeDesignData where PrjXMLSeq=@PrjXMLSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", m.Seq);
                db.ExecuteNonQuery(cmd);

                sql = @"delete from ProgressData where PrjXMLSeq=@PrjXMLSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", m.Seq);
                db.ExecuteNonQuery(cmd);

                sql = @"delete from BackwardData where PrjXMLSeq=@PrjXMLSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", m.Seq);
                db.ExecuteNonQuery(cmd);

                sql = @"delete from PaymentRecord where PrjXMLSeq=@PrjXMLSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", m.Seq);
                db.ExecuteNonQuery(cmd);

                sql = @"
                    update PrjXML set
                        TenderNo=@TenderNo,
                        ExecUnitCd=@ExecUnitCd,
                        ExecUnitName=@ExecUnitName,
                        TenderName=@TenderName,
                        PrjName=@PrjName,
                        EngType=@EngType,
                        TownName=@TownName,
                         -- CoordX=@CoordX,
                         -- CoordY=@CoordY,
                        Location=@Location,
                        PlanOrganizerName=@PlanOrganizerName,
                        PlanNo=@PlanNo,
                        CompetentAuthority=@CompetentAuthority,
                        OrganizerName=@OrganizerName,
                        FundingSourceName=@FundingSourceName,
                        TenderNoticeUnit=@TenderNoticeUnit,
                        ContactName=@ContactName,
                        ContactPhone=@ContactPhone,
                        ContactEmail=@ContactEmail,
                        Weights=@Weights,
                        EngOverview=@EngOverview,
                        SteelDemand=@SteelDemand,
                        ConcreteDemand=@ConcreteDemand,
                        EarchworkDemand=@EarchworkDemand,
                        DurationCategory=@DurationCategory,
                        TotalDays=@TotalDays,
                        DurationDesc=@DurationDesc,
                        BudgetAccount=@BudgetAccount,
                        TotalEngBudget=@TotalEngBudget,
                        OutsourcingBudget=@OutsourcingBudget,
                        SupplyMaterialCost=@SupplyMaterialCost,
                        LandPurCompen=@LandPurCompen,
                        EngManageFee=@EngManageFee,
                        AirPollutionFee=@AirPollutionFee,
                        OtherFee=@OtherFee,
                        PlanningUnitName=@PlanningUnitName,
                        DesignUnitName=@DesignUnitName,
                        DesignFee=@DesignFee,
                        DesignMemo=@DesignMemo,
                        SupervisionUnitName=@SupervisionUnitName,
                        SupervisionFee=@SupervisionFee,
                        SupervisionMemo=@SupervisionMemo,
                        ContractorName1=@ContractorName1,
                        ContractorName2=@ContractorName2,
                        InsuranceDate=@InsuranceDate,
                        InsuranceAmount=@InsuranceAmount,
                        InsuranceNo=@InsuranceNo,
                        ActualAnnoDate=@ActualAnnoDate,
                        ScheBidReviewDate=@ScheBidReviewDate,
                        ActualBidReviewDate=@ActualBidReviewDate,
                        ScheBidOpeningDate=@ScheBidOpeningDate,
                        ActualBidOpeningDate=@ActualBidOpeningDate,
                        ScheBidAwardDate=@ScheBidAwardDate,
                        ActualBidAwardDate=@ActualBidAwardDate,
                        ScheBiddingMethod=@ScheBiddingMethod,
                        ActualBiddingMethod=@ActualBiddingMethod,
                        BidAwardMethod=@BidAwardMethod,
                        ContractFeePayMethod=@ContractFeePayMethod,
                        EstimateBasePrice=@EstimateBasePrice,
                        RendBasePrice=@RendBasePrice,
                        BidAmount=@BidAmount,
                        ContractNo=@ContractNo,
                        Prepayment=@Prepayment,
                        ScheStartDate=@ScheStartDate,
                        ActualStartDate=@ActualStartDate,
                        ScheCompletionDate=@ScheCompletionDate,
                        ScheCompletCloseDate=@ScheCompletCloseDate,
                        QualityControlFee=@QualityControlFee,
                        QualityPlanApproveUnit=@QualityPlanApproveUnit,
                        QualityPlanApproveDate=@QualityPlanApproveDate,
                        QualityPlanApproveNo=@QualityPlanApproveNo,
                        SupervisionPlanApproveUnit=@SupervisionPlanApproveUnit,
                        SupervisionPlanApproveDate=@SupervisionPlanApproveDate,
                        SupervisionPlanApproveNo=@SupervisionPlanApproveNo,
                        SiteContactMemo=@SiteContactMemo,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq,
                        TenderYear=@TenderYear
                    where Seq=@Seq";

                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@TenderNo", m.TenderNo);
                cmd.Parameters.AddWithValue("@ExecUnitCd", m.ExecUnitCd);
                cmd.Parameters.AddWithValue("@ExecUnitName", m.ExecUnitName);
                cmd.Parameters.AddWithValue("@TenderName", m.TenderName);
                cmd.Parameters.AddWithValue("@PrjName", m.PrjName);
                cmd.Parameters.AddWithValue("@EngType", m.EngType);
                cmd.Parameters.AddWithValue("@TownName", m.TownName);
                //cmd.Parameters.AddWithValue("@CoordX", this.NulltoDBNull(m.CoordX));
                //cmd.Parameters.AddWithValue("@CoordY", this.NulltoDBNull(m.CoordY));
                cmd.Parameters.AddWithValue("@Location", m.Location);
                cmd.Parameters.AddWithValue("@PlanOrganizerName", m.PlanOrganizerName);
                cmd.Parameters.AddWithValue("@PlanNo", m.PlanNo);
                cmd.Parameters.AddWithValue("@CompetentAuthority", m.CompetentAuthority);
                cmd.Parameters.AddWithValue("@OrganizerName", m.OrganizerName);
                cmd.Parameters.AddWithValue("@FundingSourceName", m.FundingSourceName);
                cmd.Parameters.AddWithValue("@TenderNoticeUnit", m.TenderNoticeUnit);
                cmd.Parameters.AddWithValue("@ContactName", m.ContactName);
                cmd.Parameters.AddWithValue("@ContactPhone", m.ContactPhone);
                cmd.Parameters.AddWithValue("@ContactEmail", m.ContactEmail);
                cmd.Parameters.AddWithValue("@Weights", m.Weights);
                cmd.Parameters.AddWithValue("@EngOverview", m.EngOverview);
                cmd.Parameters.AddWithValue("@SteelDemand", this.NulltoDBNull(m.SteelDemand));
                cmd.Parameters.AddWithValue("@ConcreteDemand", this.NulltoDBNull(m.ConcreteDemand));
                cmd.Parameters.AddWithValue("@EarchworkDemand", this.NulltoDBNull(m.EarchworkDemand));
                cmd.Parameters.AddWithValue("@DurationCategory", m.DurationCategory);
                cmd.Parameters.AddWithValue("@TotalDays", this.NulltoDBNull(m.TotalDays));
                cmd.Parameters.AddWithValue("@DurationDesc", m.DurationDesc);
                cmd.Parameters.AddWithValue("@BudgetAccount", m.BudgetAccount);
                cmd.Parameters.AddWithValue("@TotalEngBudget", this.NulltoDBNull(m.TotalEngBudget));
                cmd.Parameters.AddWithValue("@OutsourcingBudget", this.NulltoDBNull(m.OutsourcingBudget));
                cmd.Parameters.AddWithValue("@SupplyMaterialCost", this.NulltoDBNull(m.SupplyMaterialCost));
                cmd.Parameters.AddWithValue("@LandPurCompen", this.NulltoDBNull(m.LandPurCompen));
                cmd.Parameters.AddWithValue("@EngManageFee", this.NulltoDBNull(m.EngManageFee));
                cmd.Parameters.AddWithValue("@AirPollutionFee", this.NulltoDBNull(m.AirPollutionFee));
                cmd.Parameters.AddWithValue("@OtherFee", this.NulltoDBNull(m.OtherFee));
                cmd.Parameters.AddWithValue("@PlanningUnitName", m.PlanningUnitName);
                cmd.Parameters.AddWithValue("@DesignUnitName", m.DesignUnitName);
                cmd.Parameters.AddWithValue("@DesignFee", this.NulltoDBNull(m.DesignFee));
                cmd.Parameters.AddWithValue("@DesignMemo", m.DesignMemo);
                cmd.Parameters.AddWithValue("@SupervisionUnitName", m.SupervisionUnitName);
                cmd.Parameters.AddWithValue("@SupervisionFee", this.NulltoDBNull(m.SupervisionFee));
                cmd.Parameters.AddWithValue("@SupervisionMemo", m.SupervisionMemo);
                cmd.Parameters.AddWithValue("@ContractorName1", m.ContractorName1);
                cmd.Parameters.AddWithValue("@ContractorName2", m.ContractorName2);
                cmd.Parameters.AddWithValue("@InsuranceDate", m.InsuranceDate);
                cmd.Parameters.AddWithValue("@InsuranceAmount", this.NulltoDBNull(m.InsuranceAmount));
                cmd.Parameters.AddWithValue("@InsuranceNo", m.InsuranceNo);
                cmd.Parameters.AddWithValue("@ActualAnnoDate", m.ActualAnnoDate);
                cmd.Parameters.AddWithValue("@ScheBidReviewDate", m.ScheBidReviewDate);
                cmd.Parameters.AddWithValue("@ActualBidReviewDate", m.ActualBidReviewDate);
                cmd.Parameters.AddWithValue("@ScheBidOpeningDate", m.ScheBidOpeningDate);
                cmd.Parameters.AddWithValue("@ActualBidOpeningDate", m.ActualBidOpeningDate);
                cmd.Parameters.AddWithValue("@ScheBidAwardDate", m.ScheBidAwardDate);
                cmd.Parameters.AddWithValue("@ActualBidAwardDate", m.ActualBidAwardDate);
                cmd.Parameters.AddWithValue("@ScheBiddingMethod", m.ScheBiddingMethod);
                cmd.Parameters.AddWithValue("@ActualBiddingMethod", m.ActualBiddingMethod);
                cmd.Parameters.AddWithValue("@BidAwardMethod", m.BidAwardMethod);
                cmd.Parameters.AddWithValue("@ContractFeePayMethod", m.ContractFeePayMethod);
                cmd.Parameters.AddWithValue("@EstimateBasePrice", this.NulltoDBNull(m.EstimateBasePrice));
                cmd.Parameters.AddWithValue("@RendBasePrice", this.NulltoDBNull(m.RendBasePrice));
                cmd.Parameters.AddWithValue("@BidAmount", this.NulltoDBNull(m.BidAmount));
                cmd.Parameters.AddWithValue("@ContractNo", m.ContractNo);
                cmd.Parameters.AddWithValue("@Prepayment", this.NulltoDBNull(m.Prepayment));
                cmd.Parameters.AddWithValue("@ScheStartDate", m.ScheStartDate);
                cmd.Parameters.AddWithValue("@ActualStartDate", m.ActualStartDate);
                cmd.Parameters.AddWithValue("@ScheCompletionDate", m.ScheCompletionDate);
                cmd.Parameters.AddWithValue("@ScheCompletCloseDate", m.ScheCompletCloseDate);
                cmd.Parameters.AddWithValue("@QualityControlFee", this.NulltoDBNull(m.QualityControlFee));
                cmd.Parameters.AddWithValue("@QualityPlanApproveUnit", m.QualityPlanApproveUnit);
                cmd.Parameters.AddWithValue("@QualityPlanApproveDate", m.QualityPlanApproveDate);
                cmd.Parameters.AddWithValue("@QualityPlanApproveNo", m.QualityPlanApproveNo);
                cmd.Parameters.AddWithValue("@SupervisionPlanApproveUnit", m.SupervisionPlanApproveUnit);
                cmd.Parameters.AddWithValue("@SupervisionPlanApproveDate", m.SupervisionPlanApproveDate);
                cmd.Parameters.AddWithValue("@SupervisionPlanApproveNo", m.SupervisionPlanApproveNo);
                cmd.Parameters.AddWithValue("@SiteContactMemo", m.SiteContactMemo);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@TenderYear", this.NulltoDBNull(m.TenderYear));

                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                db.ExecuteNonQuery(cmd);

                AddContractorQualityControl(m.ContractorQualityControl, m.Seq);
                AddSupervisor(m.Supervisor, m.Seq);
                AddFullTimeEngineer(m.FullTimeEngineer, m.Seq);
                AddSiteRelate(m.SiteRelate, m.Seq);
                AddBudgeting(m.Budgeting, m.Seq);
                AddChangeDesignData(m.ChangeDesignData, m.Seq);
                AddProgressData(m.ProgressData, m.Seq);
                AddBackwardData(m.BackwardData, m.Seq);
                AddPaymentRecord(m.PaymentRecord, m.Seq);

                db.TransactionCommit();
                return true;
            } catch (Exception e)
            {
                db.TransactionRollback();
                m.UpdateMsg = e.Message;
                log.Info("EPCImportService.updatePrjAll:[" + m.TenderNo + "] " + e.Message);
                log.Info(sql);
                return false;
            }
        }
        //歷次付款資料
        private void AddPaymentRecord(List<PaymentRecordModel> models, int prjSeq)
        {
            string sql = @"insert into PaymentRecord(
                    PrjXMLSeq,
                    PRItemNo,
                    PRTotalAmountPayable,
                    PRPayDate,
                    PRPayAmount,
                    PRSchePayDate,
                    PRSchePayAmount,
                    PRActualPayDate,
                    PRActualPayAmount,
                    PRAccuPayAmount,
                    PRMemo,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                )values(
                    @PrjXMLSeq,
                    @PRItemNo,
                    @PRTotalAmountPayable,
                    @PRPayDate,
                    @PRPayAmount,
                    @PRSchePayDate,
                    @PRSchePayAmount,
                    @PRActualPayDate,
                    @PRActualPayAmount,
                    @PRAccuPayAmount,
                    @PRMemo,
                    @OrderNo,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);
            foreach (PaymentRecordModel m in models)
            {
                Null2Empty(m);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", prjSeq);
                cmd.Parameters.AddWithValue("@PRItemNo", this.NulltoDBNull(m.PRItemNo));
                cmd.Parameters.AddWithValue("@PRTotalAmountPayable", this.NulltoDBNull(m.PRTotalAmountPayable));
                cmd.Parameters.AddWithValue("@PRPayDate", m.PRPayDate);
                cmd.Parameters.AddWithValue("@PRPayAmount", this.NulltoDBNull(m.PRPayAmount));
                cmd.Parameters.AddWithValue("@PRSchePayDate", m.PRSchePayDate);
                cmd.Parameters.AddWithValue("@PRSchePayAmount", this.NulltoDBNull(m.PRSchePayAmount));
                cmd.Parameters.AddWithValue("@PRActualPayDate", m.PRActualPayDate);
                cmd.Parameters.AddWithValue("@PRActualPayAmount", this.NulltoDBNull(m.PRActualPayAmount));
                cmd.Parameters.AddWithValue("@PRAccuPayAmount", this.NulltoDBNull(m.PRAccuPayAmount));
                cmd.Parameters.AddWithValue("@PRMemo", m.PRMemo);
                cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
            }
        }
        //落後資料
        private void AddBackwardData(List<BackwardDataModel> models, int prjSeq)
        {
            string sql = @"insert into BackwardData(
                    PrjXMLSeq,
                    BDYear,
                    BDMonth,
                    BDBackwardFactor,
                    BDAnalysis,
                    BDSolution,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                )values(
                    @PrjXMLSeq,
                    @BDYear,
                    @BDMonth,
                    @BDBackwardFactor,
                    @BDAnalysis,
                    @BDSolution,
                    @OrderNo,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);
            foreach (BackwardDataModel m in models)
            {
                Null2Empty(m);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", prjSeq);
                cmd.Parameters.AddWithValue("@BDYear", this.NulltoDBNull(m.BDYear));
                cmd.Parameters.AddWithValue("@BDMonth", this.NulltoDBNull(m.BDMonth));
                cmd.Parameters.AddWithValue("@BDBackwardFactor", m.BDBackwardFactor);
                cmd.Parameters.AddWithValue("@BDAnalysis", m.BDAnalysis);
                cmd.Parameters.AddWithValue("@BDSolution", m.BDSolution);
                cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
            }
        }
        //進度資料
        private void AddProgressData(List<ProgressDataModel> models, int prjSeq)
        {
            string sql = @"insert into ProgressData(
                    PrjXMLSeq,
                    PDYear,
                    PDMonth,
                    PDAccuScheProgress,
                    PDAccuActualProgress,
                    PDAccuScheCloseAmount,
                    PDAccuActualCloseAmount,
                    PDYearAccuScheProgress,
                    PDYearAccuActualProgress,
                    PDYearAccuScheCloseAmount,
                    PDYearAccuActualCloseAmount,
                    PDAccuEstValueAmount,
                    PDEstValueRetentAmount,
                    PDExecState,
                    PDActualExecMemo,
                    PDAccountPayable,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                )values(
                    @PrjXMLSeq,
                    @PDYear,
                    @PDMonth,
                    @PDAccuScheProgress,
                    @PDAccuActualProgress,
                    @PDAccuScheCloseAmount,
                    @PDAccuActualCloseAmount,
                    @PDYearAccuScheProgress,
                    @PDYearAccuActualProgress,
                    @PDYearAccuScheCloseAmount,
                    @PDYearAccuActualCloseAmount,
                    @PDAccuEstValueAmount,
                    @PDEstValueRetentAmount,
                    @PDExecState,
                    @PDActualExecMemo,
                    @PDAccountPayable,
                    @OrderNo,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);
            foreach (ProgressDataModel m in models)
            {
                Null2Empty(m);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", prjSeq);
                cmd.Parameters.AddWithValue("@PDYear", this.NulltoDBNull(m.PDYear));
                cmd.Parameters.AddWithValue("@PDMonth", this.NulltoDBNull(m.PDMonth));
                cmd.Parameters.AddWithValue("@PDAccuScheProgress", this.NulltoDBNull(m.PDAccuScheProgress));
                cmd.Parameters.AddWithValue("@PDAccuActualProgress", this.NulltoDBNull(m.PDAccuActualProgress));
                cmd.Parameters.AddWithValue("@PDAccuScheCloseAmount", this.NulltoDBNull(m.PDAccuScheCloseAmount));
                cmd.Parameters.AddWithValue("@PDAccuActualCloseAmount", this.NulltoDBNull(m.PDAccuActualCloseAmount));
                cmd.Parameters.AddWithValue("@PDYearAccuScheProgress", this.NulltoDBNull(m.PDYearAccuScheProgress));
                cmd.Parameters.AddWithValue("@PDYearAccuActualProgress", this.NulltoDBNull(m.PDYearAccuActualProgress));
                cmd.Parameters.AddWithValue("@PDYearAccuScheCloseAmount", this.NulltoDBNull(m.PDYearAccuScheCloseAmount));
                cmd.Parameters.AddWithValue("@PDYearAccuActualCloseAmount", this.NulltoDBNull(m.PDYearAccuActualCloseAmount));
                cmd.Parameters.AddWithValue("@PDAccuEstValueAmount", this.NulltoDBNull(m.PDAccuEstValueAmount));
                cmd.Parameters.AddWithValue("@PDEstValueRetentAmount", this.NulltoDBNull(m.PDEstValueRetentAmount));
                cmd.Parameters.AddWithValue("@PDExecState", m.PDExecState);
                cmd.Parameters.AddWithValue("@PDActualExecMemo", m.PDActualExecMemo);
                cmd.Parameters.AddWithValue("@PDAccountPayable", this.NulltoDBNull(m.PDAccountPayable));
                cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
            }
        }
        //變更設計資料
        private void AddChangeDesignData(List<ChangeDesignDataModel> models, int prjSeq)
        {
            string sql = @"insert into ChangeDesignData(
                    PrjXMLSeq,
                    CDChangeDate,
                    CDAnnoNo,
                    CDApprovalNo,
                    CDBeforeAmount,
                    CDAfterAmount,
                    CDBeforeCloseDate,
                    CDAfterCloseDate,
                    CDApprovalDays,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                )values(
                    @PrjXMLSeq,
                    @CDChangeDate,
                    @CDAnnoNo,
                    @CDApprovalNo,
                    @CDBeforeAmount,
                    @CDAfterAmount,
                    @CDBeforeCloseDate,
                    @CDAfterCloseDate,
                    @CDApprovalDays,
                    @OrderNo,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);
            foreach (ChangeDesignDataModel m in models)
            {
                Null2Empty(m);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", prjSeq);
                cmd.Parameters.AddWithValue("@CDChangeDate", m.CDChangeDate);
                cmd.Parameters.AddWithValue("@CDAnnoNo", m.CDAnnoNo);
                cmd.Parameters.AddWithValue("@CDApprovalNo", m.CDApprovalNo);
                cmd.Parameters.AddWithValue("@CDBeforeAmount", this.NulltoDBNull(m.CDBeforeAmount));
                cmd.Parameters.AddWithValue("@CDAfterAmount", this.NulltoDBNull(m.CDAfterAmount));
                cmd.Parameters.AddWithValue("@CDBeforeCloseDate", m.CDBeforeCloseDate);
                cmd.Parameters.AddWithValue("@CDAfterCloseDate", m.CDAfterCloseDate);
                cmd.Parameters.AddWithValue("@CDApprovalDays", this.NulltoDBNull(m.CDApprovalDays));
                cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
            }
        }
        //預算編列
        private void AddBudgeting(List<BudgetingModel> models, int prjSeq)
        {
            SqlCommand cmd,cmdBK;
            string sql = @"insert into Budgeting(
                    PrjXMLSeq,
                    BudgetYear,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                )values(
                    @PrjXMLSeq,
                    @BudgetYear,
                    @OrderNo,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                )";

            string sqlBK = @"insert into BudgetKind(
                    BudgetingSeq,
                    BudgetType,
                    BudgetSource,
                    BudgetAccount,
                    LegalBudget,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                )values(
                    @BudgetingSeq,
                    @BudgetType,
                    @BudgetSource,
                    @BudgetAccount,
                    @LegalBudget,
                    @OrderNo,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                )";
            

            foreach (BudgetingModel m in models)
            {
                Null2Empty(m);
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", prjSeq);
                cmd.Parameters.AddWithValue("@BudgetYear", m.BudgetYear);
                cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
                //
                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('Budgeting') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);
                m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
                //
                foreach (BudgetKindModel bk in m.BudgetKindList)
                {
                    Null2Empty(bk);
                    cmdBK = db.GetCommand(sqlBK);
                    cmdBK.Parameters.Clear();
                    cmdBK.Parameters.AddWithValue("@BudgetingSeq", m.Seq);
                    cmdBK.Parameters.AddWithValue("@BudgetType", bk.BudgetType);
                    cmdBK.Parameters.AddWithValue("@BudgetSource", bk.BudgetSource);
                    cmdBK.Parameters.AddWithValue("@BudgetAccount", bk.BudgetAccount);
                    cmdBK.Parameters.AddWithValue("@LegalBudget", this.NulltoDBNull(bk.LegalBudget));
                    cmdBK.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(bk.OrderNo));
                    cmdBK.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmdBK);
                }
            }
        }
        //工地相關人員
        private void AddSiteRelate(List<SiteRelateModel> models, int prjSeq)
        {
            string sql = @"insert into SiteRelate(
                    PrjXMLSeq,
                    SRName,
                    SRLicenseKind,
                    SRLicenseNo,
                    SRStartDate,
                    SREndDate,
                    SRMemo,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                )values(
                    @PrjXMLSeq,
                    @SRName,
                    @SRLicenseKind,
                    @SRLicenseNo,
                    @SRStartDate,
                    @SREndDate,
                    @SRMemo,
                    @OrderNo,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);
            foreach (SiteRelateModel m in models)
            {
                Null2Empty(m);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", prjSeq);
                cmd.Parameters.AddWithValue("@SRName", m.SRName);
                cmd.Parameters.AddWithValue("@SRLicenseKind", m.SRLicenseKind);
                cmd.Parameters.AddWithValue("@SRLicenseNo", m.SRLicenseNo);
                cmd.Parameters.AddWithValue("@SRStartDate", m.SRStartDate);
                cmd.Parameters.AddWithValue("@SREndDate", m.SREndDate);
                cmd.Parameters.AddWithValue("@SRMemo", m.SRMemo);
                cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
            }
        }
        //專任工程人員
        private void AddFullTimeEngineer(List<FullTimeEngineerModel> models, int prjSeq)
        {
            string sql = @"insert into FullTimeEngineer(
                    PrjXMLSeq,
                    FEName,
                    FELicenseKind,
                    FELicenseNo,
                    FEStartDate,
                    FEEndDate,
                    FEMemo,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                )values(
                    @PrjXMLSeq,
                    @FEName,
                    @FELicenseKind,
                    @FELicenseNo,
                    @FEStartDate,
                    @FEEndDate,
                    @FEMemo,
                    @OrderNo,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);
            foreach (FullTimeEngineerModel m in models)
            {
                Null2Empty(m);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", prjSeq);
                cmd.Parameters.AddWithValue("@FEName", m.FEName);
                cmd.Parameters.AddWithValue("@FELicenseKind", m.FELicenseKind);
                cmd.Parameters.AddWithValue("@FELicenseNo", m.FELicenseNo);
                cmd.Parameters.AddWithValue("@FEStartDate", m.FEStartDate);
                cmd.Parameters.AddWithValue("@FEEndDate", m.FEEndDate);
                cmd.Parameters.AddWithValue("@FEMemo", m.FEMemo);
                cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
            }
        }
        //監造單位聘用監工人員
        private void AddSupervisor(List<SupervisorModel> models, int prjSeq)
        {
            string sql = @"insert into Supervisor(
                    PrjXMLSeq,
                    SPName,
                    SPLicenseNo,
                    SPSkill,
                    SPMoveinDate,
                    SPDismissalDate,
                    SPState,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                )values(
                    @PrjXMLSeq,
                    @SPName,
                    @SPLicenseNo,
                    @SPSkill,
                    @SPMoveinDate,
                    @SPDismissalDate,
                    @SPState,
                    @OrderNo,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);
            foreach (SupervisorModel m in models)
            {
                Null2Empty(m);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", prjSeq);
                cmd.Parameters.AddWithValue("@SPName", m.SPName);
                cmd.Parameters.AddWithValue("@SPLicenseNo", m.SPLicenseNo);
                cmd.Parameters.AddWithValue("@SPSkill", m.SPSkill);
                cmd.Parameters.AddWithValue("@SPMoveinDate", m.SPMoveinDate);
                cmd.Parameters.AddWithValue("@SPDismissalDate", m.SPDismissalDate);
                cmd.Parameters.AddWithValue("@SPState", m.SPState);
                cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
            }
        }
        //廠商聘用品管人員
        private void AddContractorQualityControl(List<ContractorQualityControlModel> models, int prjSeq)
        {
            string sql = @"insert into ContractorQualityControl(
                    PrjXMLSeq,
                    QCName,
                    QCLicenseNo,
                    QCSkill,
                    QCMoveinDate,
                    QCDismissalDate,
                    QCState,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                )values(
                    @PrjXMLSeq,
                    @QCName,
                    @QCLicenseNo,
                    @QCSkill,
                    @QCMoveinDate,
                    @QCDismissalDate,
                    @QCState,
                    @OrderNo,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);
            foreach (ContractorQualityControlModel m in models)
            {
                Null2Empty(m);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", prjSeq);
                cmd.Parameters.AddWithValue("@QCName", m.QCName);
                cmd.Parameters.AddWithValue("@QCLicenseNo", m.QCLicenseNo);
                cmd.Parameters.AddWithValue("@QCSkill", m.QCSkill);
                cmd.Parameters.AddWithValue("@QCMoveinDate", m.QCMoveinDate);
                cmd.Parameters.AddWithValue("@QCDismissalDate", m.QCDismissalDate);
                cmd.Parameters.AddWithValue("@QCState", m.QCState);
                cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
            }
        }

        //匯入 結果,統計
        public void SavePrjXMLImport(List<TenderModel> models, int Success, int Failure)
        {
            string sql = "";
            //Null2Empty(m);
            db.BeginTransaction();
            try
            {
                sql = @"insert into PrjXMLImport(
                    Success,
                    Failure,
                    CreateTime,
                    CreateUserSeq
                )values(
                    @Success,
                    @Failure,
                    GetDate(),
                    @ModifyUserSeq
                )";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Success", Success);
                cmd.Parameters.AddWithValue("@Failure", Failure);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
                //
                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('PrjXMLImport') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);
                int importSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                sql = @"insert into PrjXMLImportRecord(
                        PrjXMLImportSeq,
                        TenderNo,
                        ItemState,
                        Msg
                    )values(
                        @PrjXMLImportSeq,
                        @TenderNo,
                        @ItemState,
                        @Msg
                    )";
                cmd = db.GetCommand(sql);

                foreach (TenderModel m in models)
                {
                    if (!String.IsNullOrEmpty(m.TenderNo) || !String.IsNullOrEmpty(m.TenderName))
                    {
                        Null2Empty(m);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@PrjXMLImportSeq", importSeq);
                        cmd.Parameters.AddWithValue("@TenderNo", (String.IsNullOrEmpty(m.TenderNo) ? m.TenderName : m.TenderNo) );
                        cmd.Parameters.AddWithValue("@ItemState", this.NulltoDBNull(m.UpdateState));
                        cmd.Parameters.AddWithValue("@Msg", m.UpdateMsg);
                        db.ExecuteNonQuery(cmd);
                    }
                }
                db.TransactionCommit();
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("EPCImportService.SavePrjXMLImport:[" + e.Message);
                log.Info(sql);
            }
        }
        public int GetPrjXMLImportCount()
        {
            string sql = @"SELECT count(Seq) total FROM PrjXMLImport";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetPrjXMLImportList<T>(int pageRecordCount, int pageIndex)
        {
            
                string sql = @"
                    SELECT
                        a.Success,
                        a.Failure,
                        a.CreateTime,
                        b.DisplayName CreateUser
                    FROM PrjXMLImport a
                    inner join UserMain b on(b.Seq=a.CreateUserSeq)
                    order by a.CreateTime DESC
                    OFFSET @pageIndex ROWS
				    FETCH FIRST @pageRecordCount ROWS ONLY";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex-1));
                cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
                return db.GetDataTableWithClass<T>(cmd);
            
        }
    }
}