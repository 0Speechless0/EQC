<template>
    <div id="menu01" class="tab-pane active">
        <div class="setFormcontentCenter">
            <h5>工程基本資料</h5>
            <div class="row">
                <div class="form-group row col-12">
                    <label class="col-12 col-lg-2 my-2">執行機關<span class="small-red">*</span></label>
                    <div class="col-12 col-lg-10 my-1">
                        <input v-model="engMain.ExecUnitName" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12">
                    <label class="col-12 col-lg-2 my-2">標案名稱<span class="small-red">*</span></label>
                    <div class="col-12 col-lg-10 my-1">
                        <input v-model="engMain.TenderName" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12">
                    <label class="col-12 col-lg-2 my-2">專案項目<span class="small-red">*</span></label>
                    <div class="col-12 col-lg-10 my-1">
                        <input v-model="engMain.PrjName" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">工程類別<span class="small-red">*</span></label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.EngType" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">縣市鄉鎮<span class="small-red">*</span></label>
                    <div class="col-12 col-lg-8 form-inline">
                        <input v-model="engMain.TownName" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12">
                    <label class="col-12 col-lg-2 col-form-label">緯度</label>
                    <div class="col-4 col-lg-4">
                        <input v-model="engMain.CoordX" type="text" class="form-control">
                    </div>
                    <label class="col-12 col-lg-2 col-form-label">經度</label>
                    <div class="col-4 col-lg-4">
                        <input v-model="engMain.CoordY" type="text" class="form-control">
                    </div>
                </div>

            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">施工地點</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.Location" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">計畫主辦機關<span class="small-red">*</span></label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PlanOrganizerName" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">計畫編號<span class="small-red">*</span></label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PlanNo" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">主管機關<span class="small-red">*</span></label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.CompetentAuthority" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">主辦機關<span class="small-red">*</span></label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.OrganizerName" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">經費來源機關<span class="small-red">*</span></label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.FundingSourceName" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">招標公告上線單位</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.TenderNoticeUnit" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">歸屬計畫<span class="small-red">*</span></label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.BelongPrj" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <h5>聯絡人員</h5>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">機關首長</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.Chief" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">姓名</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ContactName" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">電話</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ContactPhone" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">電子郵件信箱</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ContactEmail" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">權重</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.Weights" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12">
                    <label class="col-12 col-lg-2 my-2">工程概要</label>
                    <div class="col-12 col-lg-10 my-1">
                        <textarea v-model="engMain.EngOverview" rows="5" class="form-control"></textarea>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">鋼材需求</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.SteelDemand" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">混凝土需求</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ConcreteDemand" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">填方砂石需求</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.EarchworkDemand" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">工期類別</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.DurationCategory" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">總天數</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.TotalDays" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12">
                    <label class="col-12 col-lg-2 my-2">工期說明</label>
                    <div class="col-12 col-lg-10 my-1">
                        <input v-model="engMain.DurationDesc" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">預算科目</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.BudgetAccount" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">工程總預算</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.TotalEngBudget" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">發包預算</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.OutsourcingBudget" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">供給材料費</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.SupplyMaterialCost" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">本年可用預算</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.YearBudget" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">購地補償費</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.LandPurCompen" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">工程管理費</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.EngManageFee" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">空污費</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.AirPollutionFee" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">其他費用</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.OtherFee" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">空污徵收管制編號</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.AirPollutionNo" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">生態檢核</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.EcologicalCheck" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">建築執照號碼</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.BuildingLicenseNo" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">建照取得日期</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.BuildingLicenseDate" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <h5>規劃</h5>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">單位名稱</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PlanningUnitName" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">規劃費用</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PlanningFee" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <h5>設計</h5>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">單位名稱</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.DesignUnitName" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">費用</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.DesignFee" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12">
                    <label class="col-12 col-lg-2 my-2">備註</label>
                    <div class="col-12 col-lg-10 my-1">
                        <input v-model="engMain.DesignMemo" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <h5>監造</h5>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">單位名稱</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.SupervisionUnitName" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">費用</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.SupervisionFee" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12">
                    <label class="col-12 col-lg-2 my-2">備註</label>
                    <div class="col-12 col-lg-10 my-1">
                        <input v-model="engMain.SupervisionMemo" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <h5>承攬</h5>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">廠商名稱</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ContractorName1" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">廠商名稱2</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ContractorName2" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">專案管理單位</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.PrjManageUnit" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <h5>保險</h5>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">保險日期</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.InsuranceDate" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">保險金額</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.InsuranceAmount" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12">
                    <label class="col-12 col-lg-2 col-form-label">保險號碼</label>
                    <div class="col-12 col-lg-10">
                        <input v-model="engMain.InsuranceNo" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">懲罰機制</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.PunishmentMechanism" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">後續加保狀況</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.InsuranceStatus" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">查核(督導)</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.AuditDate" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">分數</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.Score" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <h5>標案基本資料</h5>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">預定完成設計日期</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.SchePerformDesignDate" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">實際完成設計日期</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.ActualPerformDesignDate" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">預計公告日期</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.ScheAnnoDate" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">實際公告日期</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ActualAnnoDate" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">預定審標日期</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ScheBidReviewDate" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">實際審標日期</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ActualBidReviewDate" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">預定開標日期</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ScheBidOpeningDate" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">實際開標日期</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ActualBidOpeningDate" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">預定決標日期</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ScheBidAwardDate" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">實際決標日期</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ActualBidAwardDate" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">預定招標方式</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ScheBiddingMethod" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">實際招標方式</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ActualBiddingMethod" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">實際決標方式</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.BidAwardMethod" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">契約費用給付方式</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ContractFeePayMethod" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">實際公告次數</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.NumOfAnnounce" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">預估底價</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.EstimateBasePrice" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">會合底價</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.RendBasePrice" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">決標金額</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.BidAmount" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">決標差異分析</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.BidAwardDiff" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">變更設計後契約金額</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.DesignChangeContractAmount" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">合約編號</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ContractNo" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">預付款</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.Prepayment" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">預定開工日期</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ScheStartDate" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">實際開工日期</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ActualStartDate" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">預定完工日期</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ScheCompletionDate" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">預定竣工決算日期</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.ScheCompletCloseDate" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">變更後預定完工日</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.ScheChangeCloseDate" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">實際完工日期</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.ActualCompletionDate" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">實際驗收完成日期</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.ActualAacceptanceCompletionDate" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">驗收扣款</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.AcceptanceDeduction" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">結算金額</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.SettlementAmount" type="text" class="form-control">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">預定決算日期</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.ScheSettlementDate" type="text" class="form-control">
                    </div>
                </div>
                <div class="form-group row col-12 col-lg-6">
                    <label class="col-12 col-lg-4 col-form-label">實際決算日期</label>
                    <div class="col-12 col-lg-8">
                        <input v-model="engMain.PrjXMLExt.ActualSettlementDate" type="text" class="form-control">
                    </div>
                </div>
            </div>
        </div>
        <h5>計劃書基本資料</h5>
        <div class="row">
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">品管費用</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.QualityControlFee" type="text" class="form-control">
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">品質計畫書核定單位</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.QualityPlanApproveUnit" type="text" class="form-control">
                </div>
            </div>
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">品質計畫書核定日期</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.QualityPlanApproveDate" type="text" class="form-control">
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">品質計畫書核定文號</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.QualityPlanApproveNo" type="text" class="form-control">
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">監造計畫書核定單位</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.SupervisionPlanApproveUnit" type="text" class="form-control">
                </div>
            </div>
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">監造計畫書核定日期</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.SupervisionPlanApproveDate" type="text" class="form-control">
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">監造計畫書核定文號</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.SupervisionPlanApproveNo" type="text" class="form-control">
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-group row col-12">
                <label class="col-12 col-lg-2 col-form-label">工地聯絡備註</label>
                <div class="col-12 col-lg-10">
                    <textarea v-model="engMain.SiteContactMemo" rows="5" class="form-control"></textarea>
                </div>
            </div>
        </div>
        <h5>工程材料內容</h5>
        <div class="row">
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">水泥砂漿(M3)</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.PrjXMLExt.CementMortar" type="text" class="form-control">
                </div>
            </div>
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">機拌混凝土(M3)</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.PrjXMLExt.MachineMixConcrete" type="text" class="form-control">
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">預拌混凝土(M3)</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.PrjXMLExt.ReadyMixedConcrete" type="text" class="form-control">
                </div>
            </div>
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">瀝青混凝土(M3)</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.PrjXMLExt.AsphaltConcrete" type="text" class="form-control">
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">砂(M3)</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.PrjXMLExt.Sand" type="text" class="form-control">
                </div>
            </div>
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">級配(M3)</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.PrjXMLExt.Gradation" type="text" class="form-control">
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">CLSM(M3)</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.PrjXMLExt.CLSM" type="text" class="form-control">
                </div>
            </div>
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">土石方(M3)</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.PrjXMLExt.EarthWork" type="text" class="form-control">
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">AC刨除料(M3)</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.PrjXMLExt.ACReduce" type="text" class="form-control">
                </div>
            </div>
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">焚化爐底渣(M3)</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.PrjXMLExt.BottomAsh" type="text" class="form-control">
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">電弧爐氧化碴(M3)</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.PrjXMLExt.EAF" type="text" class="form-control">
                </div>
            </div>
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">轉爐石爐碴(M3)</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.PrjXMLExt.BOF" type="text" class="form-control">
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">鋼筋(噸)</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.PrjXMLExt.Rebar" type="text" class="form-control">
                </div>
            </div>
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">鋼版及型鋼(M3)</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.PrjXMLExt.SteelPlateSection" type="text" class="form-control">
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">模板</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.PrjXMLExt.Template" type="text" class="form-control">
                </div>
            </div>
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">AC刨除料產出(M3)</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.PrjXMLExt.ACReduceOutput" type="text" class="form-control">
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-group row col-12 col-lg-6">
                <label class="col-12 col-lg-4 col-form-label">剩餘土石方產出(M3)</label>
                <div class="col-12 col-lg-8">
                    <input v-model="engMain.PrjXMLExt.EarthWorkOutput" type="text" class="form-control">
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['engMain'],
        /*watch: {
            pageIndex: {
                handler: function (value) {
                    this.getList();
                }
            }
        },
        components: {
            qcstdmodal: require('./QCStdModal.vue').default
        },
        data: function () {
            return {
            };
        },
        methods: {
        },*/
        async mounted() {
            console.log('mounted() 基本資料');
        }
    }
</script>