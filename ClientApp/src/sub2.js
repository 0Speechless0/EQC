import Vue from 'vue';
Vue.component("tree-list", require("./Tree/TreeList.vue").default);
Vue.component("tree-new", require("./Tree/TreeNew.vue").default);
Vue.component("sign-management", require("./Backend/SignManagement/Index.vue").default);
Vue.component("ammeter-record", require("./AmmeterRecord/Index.vue").default);
Vue.component("carbon-emission-view", require("./FrontDesk/CarbonEmissionView/Index.vue").default);
Vue.component("risk-management-index", require("./Backend/RiskManagement/Index.vue").default);
Vue.component('tool-package', require("./QueryArea/ToolPackage.vue").default);
Vue.component('const-check-owner-setting', require('./QueryArea/ConstCheckOwnerSetting.vue').default);
Vue.component("carbon-emission-cal-xml", require("./QueryArea/CarbonEmissionCalXML/Index.vue").default);
//工程提報
Vue.component('er-need-assessment-list', require('./EngReport/NeedAssessment/NeedAssessmentList.vue').default);
Vue.component('er-need-assessment-edit', require('./EngReport/NeedAssessment/NeedAssessmentEdit.vue').default);
Vue.component('er-proposal-work-list', require('./EngReport/ProposalWork/ProposalWorkList.vue').default);
Vue.component('er-proposal-review-list', require('./EngReport/ProposalReview/ProposalReviewList.vue').default);
Vue.component('er-proposal-review-edit', require('./EngReport/ProposalReview/EditIndex.vue').default);
Vue.component('er-annual-funding-review-list', require('./EngReport/AnnualFundingReview/AnnualFundingReviewList.vue').default);
Vue.component('er-approved-case-list', require('./EngReport/ApprovedCase/ApprovedCaseList.vue').default);
Vue.component('er-need-assessment-view', require('./EngReport/NeedAssessment/NeedAssessmentView.vue').default);
Vue.component('er-proposal-review-view', require('./EngReport/ProposalReview/ProposalReviewView.vue').default);
Vue.component("carbon-reduction-factor", require("./Backend/CarbonReduction/CarbonReduction.vue").default);
Vue.component("login-record", require("./Backend/LoginRecord/Index.vue").default);
Vue.component('del-eng-report', require('./Backend/DelEngReport/DelEngReport.vue').default);
Vue.component('far-region-import', require('./Backend/ExcelImport/FarRegion/Index.vue').default);

//工程提報 end

//施工風險評估報告產製
Vue.component('eng-risk-front', require('./EngRiskFront/EngRiskFront.vue').default);
//施工風險評估報告產製 end

//施工減碳
Vue.component('carbon-reduction-cal', require('./FrontDesk/CarbonReductionCal/Index.vue').default);

Vue.component('carbon-reduction-factor-view', require("./QueryArea/CarbonReductionFactorView/Index.vue").default);

//訓練影像資料庫
Vue.component('training-image', require('./EngAD/TrainingImage/Index.vue').default);
//系統公告
Vue.component('user-notification', require('./Backend/UserNotification/Index.vue').default);
Vue.component('ptt-modal', require('./components/pttModal.vue').default);

//樹種管理
Vue.component("tree-management", require('./Backend/TreeListManagement/Index.vue').default);

Vue.component("alert-setting", require('./Backend/AlertSetting/Index.vue').default);
Vue.component("api-record", require("./Backend/APIRecord/Index.vue").default);

Vue.component("performance-score-import", require('./Backend/PerformanceScore/PerformanceScore.vue').default);

Vue.component("system-problem-list", require('./SystemProblem/List.vue').default);
Vue.component("system-problem-index", require('./SystemProblem/Index.vue').default);

//工程督導範本維護
Vue.component('estemp-maintain', require('./Backend/ESTemp/ESTemp.vue').default);

import common from './Common/Common2';
window.comm2 = common;
