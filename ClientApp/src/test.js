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

import {useAccount} from "./testUser.js";
import {switchTestComponent} from "./testComponent.js";


import {useStaticStore } from "./store.js";


const pageGenerate = async () => {

    window.sessionStorage.setItem('EPC_SelectTrenderSeq', 264)
    

    // document.cookie = "ASP.NET_SessionId=i3xikgktcm2ilmrmxlbu5x1f";
    //使用store
    await useStaticStore();
  
    var TestComponent =  switchTestComponent(window.location.pathname);

    console.log("TestComponent ", TestComponent );
    if(TestComponent)
    {
        var vm = new Vue({

            render: h=>h(TestComponent)
        
        }).$mount('#app')
    }


    
    

} ;

if(process.env.NODE_ENV != 'production')
{
    window.sessionStorage.setItem(window.eqSelTrenderPlanSeq, 264);

        //API設定;
    console.log("fff", process.env.NODE_ENV);
    window.myAjax = axios.create({ baseURL: process.env.NODE_ENV != 'production' ? 'https://localhost:44300': window.location.origin });
    window.localStorage.setItem("Role", 1);
    let{ testAccount, testPassword} = useAccount("kbb370381");
        //模擬登入 
    window.myAjax.post('Login/CheckUserForDebug', {
        userNo : testAccount,
        passWd : testPassword
    }).then((resp) => {
        pageGenerate()
        window.localStorage.setItem('isEQCAdmin', "");
        window.localStorage.setItem('isAdmin', "");
        if(resp.data.RoleSeq <=2)
        {
            window.localStorage.setItem('isEQCAdmin', "True");
        }
        if(resp.data.RoleSeq <=1)
        window.localStorage.setItem('isAdmin', "True");

    })
        // pageGenerate()

}




