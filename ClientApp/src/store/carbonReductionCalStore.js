
import Vuex, { Store } from 'vuex'
import Vue from 'vue'
Vue.use(Vuex)

const carbonReductionCalStore = new Store({
    state: {
        navvyNum : [],
        truckNum : [],
        energyNum : [],
        navvyItems : [],
        truckItems: [],
        energyItems : [],
        navvyNumDic : {},
        truckNumDic : {},
        energyNumDic : {},
        reductionTotal : 0,
    },
    actions :{
        async getTypeList({commit }, targetId)
        {

            const {data} = await  window.myAjax.post("CarbonReductionCal/GetTypeList", {
                engSeq : targetId
        
            });
            data.navvy.forEach(element => {
                element.PC120Num = 0;
                element.PC200Num = 0;
                element.PC300Num = 0;
                element.PC400Num = 0;
            });
            data.truck.forEach(element => {
                element.C15TNum = 0;
                element.C21TNum = 0;
                element.C35TNum = 0;
            });
            data.energy.forEach(element => {
                element.Num = 0;
            });

            commit("setNum", {key:"navvy", value: data.navvy});
            commit("setNum", { key:"truck", value: data.truck} );
            commit("setNum", { key :"energy", value: data.energy});
        },
        async addReduction({state, commit}, targetId)
        {
            let calItem = state.navvyItems.concat(state.truckItems.concat(state.energyItems));
            state.reductionTotal  = calItem.reduce((a, c) => c.TempResult+ a, 0);
            calItem.forEach(e => 
            {      
                e.CarbonPayItemSeq =  e.Seq  + "";
                e.Seq  =  e.carbonReductionCal ? e.carbonReductionCal.Seq : null;  
            });

            let {data} = await window.myAjax.post("CarbonReductionCal/saveCalResult", {engSeq : targetId, items : calItem, result : state.reductionTotal});
            if(data == true) {
                alert("更新成功");
            }
            console.log("fff");
            commit("getCalItem", targetId);
        },

    },
    mutations :{
        setNum(state, arg)
        {
            state[arg.key+ "Num"] = arg.value;

        },
        setItems(state, arg)
        {
            state[arg.key+"Items"] = arg.value;
        },
        async getReduction(state, targetId)
        {   

            
            state.reductionTotal = (await window.myAjax.post("CarbonReductionCal/GetCalResult", {engSeq: targetId})).data; 
        },

        computeTypeNumDic(state)
        {
            state.navvyNum.forEach(e => state.navvyNumDic[e.Type2] = e);
            state.truckNum.forEach(e => state.truckNumDic[e.Type2] = e);
            state.energyNum.forEach(e => state.energyNumDic[e.Type2] = e);
        },

        async getCalItem(state, targetId)
        {
            state.navvyItems =  (await window.myAjax.post("CarbonReductionCal/GetList", {
                engSeq : targetId,
                type  : 1
        
            })).data.map(e => {
                e.PC120Num = state.navvyNumDic[e.factor.Type2].PC120Num || !e.carbonReductionCal ? state.navvyNumDic[e.factor.Type2].PC120Num ?? 0  : e.carbonReductionCal.PC120Num ;
                e.PC200Num = state.navvyNumDic[e.factor.Type2].PC200Num || !e.carbonReductionCal ? state.navvyNumDic[e.factor.Type2].PC200Num ?? 0 : e.carbonReductionCal.PC200Num ;
                e.PC300Num = state.navvyNumDic[e.factor.Type2].PC300Num || !e.carbonReductionCal ? state.navvyNumDic[e.factor.Type2].PC300Num ?? 0 : e.carbonReductionCal.PC300Num ;
                e.PC400Num = state.navvyNumDic[e.factor.Type2].PC400Num || !e.carbonReductionCal ? state.navvyNumDic[e.factor.Type2].PC400Num ?? 0 : e.carbonReductionCal.PC400Num ;
                e.CalType = 1;
                return e;
        
            });

            state.truckItems = (await window.myAjax.post("CarbonReductionCal/GetList", {
                engSeq : targetId,
                type  : 2
        
            }) ).data.map(e => {
                e.C15TNum = state.truckNumDic[e.factor.Type2].C15TNum || !e.carbonReductionCal ? state.truckNumDic[e.factor.Type2].C15TNum ?? 0 : e.carbonReductionCal.C15TNum ;
                e.C21TNum = state.truckNumDic[e.factor.Type2].C21TNum || !e.carbonReductionCal ? state.truckNumDic[e.factor.Type2].C21TNum ?? 0 : e.carbonReductionCal.C21TNum ;
                e.C35TNum = state.truckNumDic[e.factor.Type2].C35TNum || !e.carbonReductionCal ? state.truckNumDic[e.factor.Type2].C35TNum ?? 0 : e.carbonReductionCal.C35TNum ;
                e.CalType = 2;
                return e;
        
            });
            state.energyItems = (await window.myAjax.post("CarbonReductionCal/GetList", {
                engSeq : targetId,
                type  : 3
        
            }) ).data.map(e => {
                e.Num = state.energyNumDic[e.factor.Type2].Num || !e.carbonReductionCal ?  state.energyNumDic[e.factor.Type2].Num ?? 0 : e.carbonReductionCal.Num ;
                return e;
        
            });
        }

    }
});
Vue.use(carbonReductionCalStore);
export default carbonReductionCalStore;