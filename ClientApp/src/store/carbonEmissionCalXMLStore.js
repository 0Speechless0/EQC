
import Vuex, { Store } from 'vuex'
import Vue from 'vue'
import IPagination  from "./interface/IPagination";
import EngStoreHandler  from './EngStoreHandler';
Vue.use(Vuex)

const carbonEmissionCalXMLStore = new Store({
    state: {
        ...IPagination,
        ...EngStoreHandler.state,
        carbonCalInfo : {
            items : []
        },
        perPage : 30

    },
    actions: {
        ...EngStoreHandler.actions ,
        async uploadXML({state, dispatch}, file)
        {
            var formData = new FormData();
            console.log(file);
            formData.append("file", file);
            let res = await window.myAjax.post("CarbonEmissionCalXML/UploadXML", formData, {
                headers: {
                  "Content-Type": "multipart/form-data"
                }
            });
            console.log(res.data);
            if(res.data.status == "success") {
              alert("匯入成功");
              await dispatch("getCarbonCalInfo", res.data.engId);
            }
            else {
              alert("匯入失敗:"+res.data.status);
            }
        },
        async getCarbonCalInfo({state}, engId)
        {
            let {data} = await window.myAjax.post("CarbonEmissionCalXML/GetCarbonCalInfo",{ 
                id : engId,
                perPage : state.perPage,
                pageIndex : state.pageIndex+1
            });
            state.carbonCalInfo = data;
            state.totalRows = data.totalRows;
        },

    },
    mutations: {
        onPaginationChange(state, pageConfig)
        {   
            console.log(pageConfig);
            state.pageIndex = pageConfig.pInx -1;
            state.perPage = pageConfig.pCount;
        }
    },
    getters:{
        itemList(state)
        {
            return state.carbonCalInfo
                .items.slice(state.page*state.perPage, (state.page+1)*state.perPage);
        },
        dismantlingRate(state)
        {
            return state.engInfo.TotalBudget == 0 ?
            0 :  (state.carbonCalInfo.co2ItemTotal * 100 / state.engInfo.SubContractingBudget).toFixed(0);
        }
    }

});
Vue.use(carbonEmissionCalXMLStore);
export default carbonEmissionCalXMLStore;