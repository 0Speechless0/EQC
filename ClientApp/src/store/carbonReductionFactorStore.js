
import Vuex, { Store } from 'vuex'
import Vue from 'vue'
Vue.use(Vuex)

const carbonReductionFactorStore = new Store({
    state: {

    },
    actions: {

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
Vue.use(carbonReductionFactorStore);
export default carbonReductionFactoreStore;