
import Vuex, { Store } from 'vuex'
import Vue from 'vue'

Vue.use(Vuex)

const EngStoreHandler = {
    state :{
        list : [],
        listCount : 0,
        selectYear : null,
        selectUnit : null,
        selectUnitOptions: [],
        selectYearOptions : [],
        selectSubUnitOptions : [],
        selectSubUnitUserOption : [],
        engInfo : {}
    },

    mutations: {
        onChangeYear(state, selectYear)
        {
                state.selectYear = selectYear
            
        },
        setYearOption(state, options)
        {
            if (options.length > 0) {
                state.selectYearOptions = options
            }
        },
        setUnitOption(state, options)
        {
            if (options.length > 0) {
                state.selectUnitOptions = options
            }
        },
        // onChangeUnit(state, selectUnit)
        // {

        //     state.selectUnit = selectUnit
        // }
    },
    actions :{
        async getEng({state}, engId, route = "/CarbonEmissionCalXML")
        {

            const { data } = await window.myAjax.post( route+'/GetEngMain', { id: engId });
            state.engInfo = data;
        },
        async getSubUnitOption({state}, targetUnit)
        {

            const { data } = await window.myAjax.post('/Unit/GetUnitList', { parentSeq: targetUnit });
            state.selectSubUnitOptions = data;
        },
        async getSubUnitUserOption({state}, targetSubUnit)
        {
            const { data } = await window.myAjax.post('/Users/GetUserBySubUnit', { subUnitSeq: targetSubUnit });
            state.selectSubUnitUserOption = data;
            console.log("d", data);
        }
    }

};

export default EngStoreHandler;