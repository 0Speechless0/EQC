// store/index.js

//import { reactive, readonly, inject } from 'vue';

//export const createStore = () => {
//    const state = reactive({
//        age: 0,
//        name: '',
//    })

//    const fetchUserData = () => {
//        return new Promise((resolve) => {
//            setTimeout(_ => {
//                state.age = 18
//                state.name = 'peerone'
//                resolve()
//            }, 1000)
//        })
//    }
//    return {
//        fetchUserData,
//        state: readonly(state)
//    }
//}

//export const state = Symbol('state')

//export const useState = () => inject(state)


//import { createStore } from 'vuex'

//export default createStore({
//    state: {
//    },
//    mutations: {
//    },
//    actions: {
//    },
//    modules: {
//    }
//})

import Vuex, { Store } from 'vuex'
import Vue from 'vue'

Vue.use(Vuex)

const store = new Store({
    state: {
        isLoading: false,
        engSeq : null,
        epcTenderList : [],
        epcTenderListCount : 0,
        selectYear : null,
        selectUnit : null
    },
    mutations: {
        Set_Loading(state, payload) {
            state.isLoading = payload;
        },
        selectEng(state, engSeq)
        {
            state.engSeq = engSeq;
        }

    },
    actions: {
        setLoading(context, payload) {
           
            context.commit('Set_Loading', payload);
        },
        async getEPCTenterList(state, pageCount, pageIndex, selectYear, selectUnit, keyWord)
        {
            if (selectYear == '' || selectUnit == '') return;

            this.items = [];
            const {data} =  await window.myAjax.post('/EPCTender/GetList'
                , {
                    year: selectYear,
                    unit: selectUnit,
                    keyWord: keyWord,
                    pageRecordCount: pageCount,
                    pageIndex: pageIndex
                });
            state.epcTenderList.items = data.items;
            state.epcTenderListCount = data.pTotal;
        },
        async getEPCYearOption()
        {
            const { data } = window.myAjax.post('/EPCTender/GetYearOptions');
            this.selectYearOptions = data;
            if (this.selectYearOptions.length > 0) {
                this.selectYear = this.selectYearOptions[0].Value;
                this.onYearChange();
            }
        },
        async getEPCUnitOption()
        {
            const { data } = window.myAjax.post('/EPCTender/GetUnitOptions');
            this.selectYearOptions = data;
            if (this.selectYearOptions.length > 0) {
                this.selectYear = this.selectYearOptions[0].Value;
                this.onYearChange();
            }
        }
    },
    getters:{
        selectedEngSeq(state)
        {
            return state.engSeq;
        },
        epcTenderList(state)
        {
            return state.epcTenderList;
        }
    }

});

export default store;