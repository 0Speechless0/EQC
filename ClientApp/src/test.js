//用於測試component，在ClientApp下命令列執行 npm run serve啟動測試網站;
//下方的case是路由，不同的路由載入不同component，假如網站架在localhost:8080
//那從瀏覽器跳轉至 localhost:8080/'路由' 可進入該component前端頁
//開發中的component可以在此設定，藉由偵錯伺服器在每次存檔後快速編譯呈現畫面 而不用每次都執行npm run build

import Vue from 'vue';
import axios from 'axios';

//系統管理者
// var testAccount = "guest";
// var testPassword = "a12345b";

//執行者
// var testAccount = "wca02040";
// var testPassword = "12345";

//管理者
// var testAccount = "i610180";
// var testPassword = "12345";
var role = 0;
var account = [
    {
        //系統管理者
        testAccount : "kbb370381",
        testPassword : "a3146"
        
    },
    {
        //執行者
        testAccount : "wca02040",
        testPassword : "1234"
        
    },
    {
        //管理者
        testAccount : "i610180",
        testPassword : "1234"
        
    },
    {
        //施工廠商
        testAccount : "C112-SMC-112-19",
        testPassword : "1234"
        
    }


]
var TestComponent;

//API設定;
window.myAjax = axios.create({ baseURL: process.env.NODE_ENV != 'production' ? 'https://localhost:44300': window.location.origin });
window.localStorage.setItem("Role", 4);
const pageGenerate = () => {

    window.sessionStorage.setItem('EPC_SelectTrenderSeq', 264)
    document.cookie = "ASP.NET_SessionId=i3xikgktcm2ilmrmxlbu5x1f";
    window.sessionStorage.setItem(window.window.epcSelectTrenderSeq, 264)
    switch(window.location.pathname)
    {
        case "/FlowChart" :
            TestComponent = require("./Backend/FlowChart/FlowChart.vue").default;break;
        case "/EQMEcologicalCheck" :
            TestComponent = require('./EQM/EcologicalCheck/EC_List_V2.vue').default;break;
        case "/QCPlaneWeaknessList" :
            TestComponent = require('./EngSupervise/QCPlaneWeakness/QCPlaneWeaknessList.vue').default;break;
        case "/EAD/CarbonEmission" :
            TestComponent = require('./EngAD/CarbonEmission/CarbonEmission.vue').default;break;
        case "/UserManager" :
            TestComponent = require("./Backend/UserManager/Index.vue").default ;break; 
        case "/RiskManagement/lookup" :
            TestComponent = require('./Backend/RiskManagement/Index.vue').default;break;
        case "/RiskManagement/destruct" :
            TestComponent = require('./Backend/RiskManagement/Index.vue').default;break;
        case "/SignManagement/lookup" :
            TestComponent = require("./Backend/SignManagement/Index.vue").default;break;
        case "/QueryArea/ToolPackage" :
            TestComponent = require("./QueryArea/ToolPackage.vue").default;break;
        case "/QueryArea/ConstCheckOwnerSetting" :
            TestComponent = require("./QueryArea/ConstCheckOwnerSetting.vue").default;break;
        case "/EngRiskFront" :
            TestComponent = require("./EngRiskFront/EngRiskFront.vue").default;break;
        case "/Test" :
            TestComponent = require("./Test.vue").default;break;
        case "/MMPlanTp" :
            TestComponent = require("./Backend/MMPlan/MMPlan.vue").default;break;
        case "/EPC/ConstructionCarbonReduction" :
            TestComponent = require("./EPC/ProgressManage/Construction_CarbonReduction_Main.vue").default;break;
        case "/CarbonReduction" :
            TestComponent = require("./Backend/CarbonReduction/CarbonReduction.vue").default;break;
        case "/TreePlant" :
            TestComponent = require("./Tree/TreeNew").default;break;
        case "/CarbonEmissionCalXML" :
            TestComponent = require("./QueryArea/CarbonEmissionCalXML/Index.vue").default;break;
        case "/LoginRecord" :
            TestComponent = require("./Backend/LoginRecord/Index.vue").default;break;
        case "/CCTTable" :
            TestComponent = require("./Backend/CarbonEmissionFactor/CCTIndex.vue").default;break;
        case "/CarbonReductionCal" :
            TestComponent = require("./FrontDesk/CarbonReductionCal/Index.vue").default;break;
        case "/pttModal" :
            TestComponent = require("./components/pttModal.vue").default;break;
        case "/UserNotification" :
            TestComponent = require("./Backend/UserNotification/Index.vue").default;break;
        case "/TrainingImage" :
            TestComponent = require("./EngAD/TrainingImage/Index.vue").default;break;
        case "/SupervisionPlan" :
            TestComponent = require("./FrontDesk/SupervisionPlan/SupervisionPlanEdit.vue").default;break;
        case "/QV_Main" :
            TestComponent = require("./EPC/QualityVerify/QV_Main.vue").default;break;
        case "/EngAnalysisDecision" :
            TestComponent = require("./EngAD/AnalysisDecision.vue").default; break;
        case "/EPCTender/Edit" :
            TestComponent = require("./EPC/Tender/TenderEdit.vue").default;break;
        case "/EPCProgressManage" :
            TestComponent = require('./EPC/ProgressManage/PM_Main.vue').default;break;
        case "/SupervisionPlanEdit" :
            TestComponent = require("./FrontDesk/SupervisionPlan/SupervisionPlanEdit.vue").default;break;
        case "/FlowChartTp" :
            TestComponent = require("./Backend/FlowChart_Test/FlowChart_Test.vue").default;break;
        case "/Backend/APIRecord" :
            TestComponent = require("./Backend/APIRecord/Index.vue").default;break;
        case "/Backend/QualityDeductionPoints" :
            TestComponent = require("./Backend/QualityDeductionPoints/QualityDeductionPoints.vue").default;break;
        
    }
    

    if(TestComponent)
    {
        new Vue({

            render: h=>h(TestComponent)
        
        }).$mount('#app')
    }


    
    

} ;

if(process.env.NODE_ENV != 'production')
{
    window.sessionStorage.setItem(window.eqSelTrenderPlanSeq, 264);
    console.log(account[role]);
    let{ testAccount, testPassword} = account[role];
        //模擬登入 
    window.myAjax.post('Login/CheckUserForDebug', {
        userNo : testAccount,
        passWd : testPassword
    }).then(() => {
        pageGenerate()
    })
        // pageGenerate()

}




