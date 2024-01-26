
import Vuex, { Store } from 'vuex'
import Vue from 'vue'
import EngStoreHandler from './EngStoreHandler';
Vue.use(Vuex)

const epcTenderStore = new Store({
    state: {
        ...EngStoreHandler.state
    },
    actions: {
        ...EngStoreHandler.actions,
        async getEPCYearOption({commit})
        {
            const { data } = await window.myAjax.post('/EPCTender/GetYearOptions');

            commit("setYearOption", data);

        },
        async getEPCUnitOption({commit}, targetYear)
        {

            commit("onChangeYear", targetYear);
            const { data } = await window.myAjax.post('/EPCTender/GetUnitOptions', { year: targetYear });
            commit("setUnitOption", data);
        },
        async getEPCTenterList({commit, state}, condition)
        {
            if (state.selectYear == '' || state.selectUnit == '') return;

            this.items = [];
            const {data} =  await window.myAjax.post('/EPCTender/GetList'
                , {
                    year: condition.selectYear,
                    unit: condition.selectUnit,
                    keyWord: condition.keyWord,
                    pageRecordCount: condition.pageCount,
                    pageIndex: condition.pageIndex
                });
            state.list = data.items;
            state.listCount = data.pTotal;
        }
    },
    mutations: {
        ...EngStoreHandler.mutations ,
    },
    getters:{
        epcTenderList(state)
        {
            return state.epcTenderList;
        },
        selectYearOptions(state)
        {
            return state.selectYearOptions;
        }
    }

});
Vue.use(epcTenderStore);
export default epcTenderStore;