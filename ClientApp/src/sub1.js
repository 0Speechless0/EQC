// ************************************************
//
// 公司內部工程師 要增加項目請至 main.js 內進行管理作業
//
// 此檔案主要是提供 外部工程師 使用與管理
// 除非有特殊需求, 或是已屬於該檔案內的模組功能擴充
//
// 2023-4-24 shioulo
//
// ************************************************

import Vue from 'vue';
import axios from 'axios';

import Highcharts from "highcharts/highcharts";
import exporting from "highcharts/modules/exporting";
exporting(Highcharts);
import exportingData from "highcharts/modules/export-data";
exportingData(Highcharts);
import HighchartsMore from 'highcharts/highcharts-more';
HighchartsMore(Highcharts);
import wordcloud from "highcharts/modules/wordcloud";
wordcloud(Highcharts);

import HighchartsVue from 'highcharts-vue'
Vue.use(HighchartsVue)

import Autocomplete from '@trevoreyre/autocomplete-vue' //s20230526
import '@trevoreyre/autocomplete-vue/dist/style.css'
Vue.use(Autocomplete)

Vue.config.devtools = true;//瀏覽器外掛 Vue 監視啟用
Vue.component('comm-pagination', require('./Common/Pagination.vue').default);
//查詢專區 20230531
Vue.component('ce-export', require('./QueryArea/CEExport/CEExport.vue').default);
//分局 - 儀表板
Vue.component('pd-dashboard', require('./PortalDepUser/PortalDepUser.vue').default);
//署管理者 - 工程資訊彙整分析及決策資訊儀表板
Vue.component('ead-analysis-decision', require('./EngAD/AnalysisDecision.vue').default);
Vue.component('ead-uniteng-list', require('./EngAD/UnitEngList.vue').default);
Vue.component('ead-carbon-emission', require('./EngAD/CarbonEmission/CarbonEmission.vue').default);
Vue.component('ead-committee', require('./EngAD/CommitteeAnalysis.vue').default);
Vue.component('ead-plane-weakness', require('./EngAD/PlaneWeakness/PlaneWeaknessAnalysis.vue').default);
Vue.component('ead-manufacturer', require('./EngAD/ManufacturerAnalysis.vue').default);
Vue.component('ead-risk', require('./EngAD/Risk/RiskAnalysis.vue').default);
Vue.component('ead-weakness-analysis', require('./EngAD/WeaknessAnalysis/WeaknessAnalysis.vue').default); //s20230316

//工程品管
Vue.component('eqm-carbonemission-list', require('./EQM/CarbonEmission/CE_List.vue').default);
Vue.component('eqm-constriskeval-list', require('./EQM/ConstRiskEval/CRE_List.vue').default);
Vue.component('eqm-ecologicalcheck-list', require('./EQM/EcologicalCheck/EC_List.vue').default);
Vue.component('eqm-ecologicalchecksta-list', require('./EQM/EQMEcologicalSta/ES_List.vue').default);
//工程品管 end

//工程督導
Vue.component('es-planeweakness-list', require('./EngSupervise/QCPlaneWeakness/QCPlaneWeaknessList.vue').default);
Vue.component('es-preworkeng-list', require('./EngSupervise/PreWork/PreWorkEngList.vue').default);
Vue.component('es-schedule-list', require('./EngSupervise/Schedule/ScheduleList.vue').default);
Vue.component('es-supervisefill-list', require('./EngSupervise/SuperviseFill/SuperviseFillList.vue').default);
Vue.component('es-supervisefill-edit', require('./EngSupervise/SuperviseFill/SuperviseFillEdit.vue').default);
Vue.component('es-supervisesta-list', require('./EngSupervise/SuperviseSta/SuperviseStaList.vue').default);
Vue.component('es-committee-home', require('./EngSupervise/Committee/CommitteeHome.vue').default);
Vue.component('es-chapter-photo', require('./EngSupervise/Committee/ViewChapterPhoto.vue').default);
Vue.component('es-sir-edit', require('./EngSupervise/Committee/SIRRecEdit.vue').default);
//工程督導 end

//工程履約子系統
Vue.component('epc-importxml', require('./EPC/Import/ImportXML.vue').default);
Vue.component('epc-tender-list', require('./EPC/Tender/TenderList.vue').default);
Vue.component('epc-tender-edit', require('./EPC/Tender/TenderEdit.vue').default);
Vue.component('epc-tender-edit2', require('./EPC/Tender/TenderEdit2.vue').default);
Vue.component('epc-progress-manage', require('./EPC/ProgressManage/PM_Main.vue').default);
Vue.component('epc-quality-verify', require('./EPC/QualityVerify/QV_Main.vue').default);
Vue.component('epc-ecologicalcheck-list', require('./EPC/EcologicalCheck/EC_List.vue').default);
Vue.component('epc-progress-report', require('./EPC/ProgressReport/PR_Main.vue').default);
Vue.component('epc-progress-engchange', require('./EPC/ProgressManage/PM_Main_EngChange.vue').default); //s20230402

Vue.component('eng-history-info', require('./QueryArea/EngHistoryInfo/EH_Main.vue').default); //s20230601

//工程履約子系統 end

Vue.component('priceindex-edit', require('./Backend/PriceIndex/PriceIndexEdit.vue').default);

Vue.component('mmfm-main', require('./Backend/MMFMaintenance/MMFM_Main.vue').default);
Vue.component('carbonemission-factor-edit', require('./Backend/CarbonEmissionFactor/CEFactor_Edit.vue').default);
Vue.component('ce-control-table', require('./Backend/CarbonEmissionFactor/CCTIndex.vue').default);

Vue.component('reject-company-list', require('./Backend/RejectCompany/RC_List.vue').default);

Vue.component('expert-committee-list', require('./Backend/ExpertCommittee/EC_List.vue').default);
Vue.component('expert-committee-edit', require('./Backend/ExpertCommittee/EC_Edit.vue').default);

Vue.component('qc-stdbase-maintain', require('./Backend/QCStdBase/QCStdBase.vue').default);
Vue.component('flowchart-maintain', require('./Backend/FlowChart/FlowChart.vue').default);
Vue.component('mmplan-maintain', require('./Backend/MMPlan/MMPlan.vue').default);
Vue.component('qualityplan-maintain', require('./Backend/QualityPlan/QualityPlan.vue').default);
Vue.component('checksheet-maintain', require('./Backend/CheckRecordSheet/CheckSheet.vue').default);
Vue.component('chapter-chart-maintain', require('./Backend/ChapterChart/ChapterChartMaintain.vue').default);
Vue.component('del-eng', require('./Backend/DelEng/DelEng.vue').default);
Vue.component('supervise_unitcode-list', require('./Backend/SuperviseUnitCode/SuperviseUnitCode_List.vue').default);

Vue.component('tenderplan-list', require('./FrontDesk/TenderPlan/TenderPlanList.vue').default);
Vue.component('tenderplan-edit', require('./FrontDesk/TenderPlan/TenderPlanEdit.vue').default);

Vue.component('supervisionplan-list', require('./FrontDesk/SupervisionPlan/SupervisionPlanList.vue').default);
Vue.component('supervisionplan-edit', require('./FrontDesk/SupervisionPlan/SupervisionPlanEdit.vue').default);
Vue.component('supervisionplan-flowchart-edit', require('./FrontDesk/SupervisionPlan/FlowChartEdit.vue').default);
Vue.component('supervisionplan-qcstd-edit', require('./FrontDesk/SupervisionPlan/QCStdEdit.vue').default);
Vue.component('qualityplan-list', require('./FrontDesk/QualityPlan/QualityPlanList.vue').default);
Vue.component('sampling-inspection-rec-list', require('./FrontDesk/SamplingInspectionRec/SamplingInspectionRecList.vue').default);
Vue.component('sampling-inspection-rec-edit', require('./FrontDesk/SamplingInspectionRec/SamplingInspectionRecEdit.vue').default);
Vue.component('sir-approve-list', require('./FrontDesk/SIRApprove/SIRApproveList.vue').default);

Vue.component('m-sampling-inspection-rec-list', require('./FrontDesk/MSamplingInspectionRec/SamplingInspectionRecList.vue').default);
Vue.component('m-sampling-inspection-rec-edit', require('./FrontDesk/MSamplingInspectionRec/SamplingInspectionRecEdit.vue').default);

Vue.component('sampling-inspection-recimprove-list', require('./FrontDesk/SamplingInspectionRecImprove/SamplingInspectionRecImproveList.vue').default);
Vue.component('sampling-inspection-recimprove-edit', require('./FrontDesk/SamplingInspectionRecImprove/SamplingInspectionRecImproveEdit.vue').default);
Vue.component('siri-approve-list', require('./FrontDesk/SIRIApprove/SIRIApproveList.vue').default);
Vue.component('siri-approve-edit', require('./FrontDesk/SIRIApprove/SIRIApproveEdit.vue').default);

Vue.component('emdaudit-list', require('./FrontDesk/EMDAudit/EMDAuditList.vue').default);
Vue.component('emdaudit-edit', require('./FrontDesk/EMDAudit/EMDAuditEdit.vue').default);

Vue.component('flowchart-test-maintain', require('./Backend/FlowChart_Test/FlowChart_Test.vue').default);

Vue.component('appinstall', require('./FrontDesk/AppInstall/AppInstall.vue').default);


Vue.component("carbon-cal", require("./FrontDesk/CarbonCal/CarbonCal.vue").default);

Vue.component("carbon-setting", require("./Backend/CarbonSetting/CS_Main.vue").default); //s20230420
Vue.component("eng-approvalimport-list", require("./Backend/EngApprovalImport/EA_List.vue").default); //s20231006

//更告欄modal
Vue.component("ptt-modal", require("./components/pttModal.vue").default )

window.myAjax = axios.create({ baseURL: window.location.origin });
// response 攔截器
window.myLoader = null;
//攔截請求
window.myAjax.interceptors.request.use(
    (config) => {
        //console.log('攔截請求');

        if (!window.myLoader) {
            if(config.url != "/SupervisionPlan/CreatePlan")
                window.myLoader = Vue.$loading.show();
        }
        return config;
    },
    (error) => {
        //console.log('攔截器request失敗');
        myhideLoader();
        return Promise.reject(error);
    }
);

function myhideLoader() {
    window.myLoader && window.myLoader.hide();
    window.myLoader = null;
}
//攔截回應
window.myAjax.interceptors.response.use(
    (response) => {
        //console.log('攔截回應');
        //console.log(response);
        myhideLoader();
        if (response.data.result == -1919) {
            window.location.href = response.data.url;
            return null;
        }
        return response;
    },
    (error) => {
        //console.log('攔截器response失敗')
        myhideLoader();
        return Promise.reject(error);
    }
);

import common from './Common/Common';
window.comm = common;
window.epcSelectTrenderSeq = 'EPC_SelectTrenderSeq';//PrjXML 標案
window.eqSelTrenderPlanSeq = 'EQ_SelectTrenderPlanSeq';//系統建立的標案
window.esKeyword = 'ES_Keyword';//工程督導
import './google_map_api.js';