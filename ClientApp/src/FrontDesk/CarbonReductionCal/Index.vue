<style scoped>
    td {
        text-align: center !important;
    }
    input {
        text-align: center !important;
    }
</style>
<script setup>

import{onMounted, ref, computed, reactive, watch}   from "vue";
import NumForm from "./NumForm.vue";
import ItemNumForm from "./ItemNumForm.vue";

const selectTab = ref("NumForm");
const tenderItem = ref({});
const targetId = window.sessionStorage.getItem(window.epcSelectTrenderSeq);


const store = reactive(window.carbonReductionCalStore);


onMounted(async () => {
    store.commit("getReduction", targetId);
    tenderItem.value = (await window.myAjax.post("EQMCarbonEmission/GetEngMain", { id : targetId})).data.item;


        console.log("tenderItem", tenderItem.value);

    if(ReductionTotal.value != null)
    {
        await store.dispatch("getTypeList", targetId);
        
        selectTab.value =  'ItemNumForm';
    }
    else{
        carbonReductionCalTag.value = (await window.myAjax.post("CarbonReductionCal/getReductionCalTag", { engSeq : targetId})).data;
    }
})

const ReductionTotal = computed(() => store.state.reductionTotal );

const carbonReductionCalTag = ref(-1);

const step =ref(0);

function changeTab(str)
{
    selectTab.value = str;
    
}

function mergeData()
{
    step.value ++;
    carbonReductionCalTag.value = -1;
    if(selectTab.value == 'ItemNumForm')
    {
        store.dispatch('addReduction', targetId);
    }
    else{
        selectTab.value = 'ItemNumForm';
    }

}
async function setCarbonReductionCalTag(b)
{
    
   let {data} = await window.myAjax.post("CarbonReductionCal/setReductionCalTag", { engSeq : targetId, tag:b });
    if(data)
    {
        carbonReductionCalTag.value = b;
        alert("設定成功");
    }
}



</script>

<template>
    <div>

        <div class="card-body" >
                    <h5 class="insearch mt-0 py-2"> 工程編號：{{tenderItem.TenderNo}}({{tenderItem.EngNo}})<br>工程名稱: {{tenderItem.TenderName}}({{tenderItem.EngName}})<br>
                    工程總減碳效益：{{(ReductionTotal??0).toFixed(2) }} kgCO2e          
                    <button role="button" v-if="carbonReductionCalTag == true" class="btn btn-color9-1 btn-xs mx-1 ml-5" @click="setCarbonReductionCalTag(false)" ><i class="fas fa-circle">&nbsp;不需計算</i></button>
                    <button role="button" v-else-if="carbonReductionCalTag == false" class="btn btn-color11-1 btn-xs mx-1 ml-5" @click="setCarbonReductionCalTag(true)" ><i class="fas fa-circle">&nbsp;需計算</i></button>    
                </h5>
        </div>
        
        <div class="card whiteBG mb-4 pattern-F colorset_R" v-if="carbonReductionCalTag">
            
            <div style="padding-top: 20px; padding-left: 20px; margin-bottom: -5px;">

                <ul class="nav nav-tabs" role="tablist">
                    <li class="nav-item" v-if="ReductionTotal == null"><a @click="changeTab('NumForm')" data-toggle="tab" href="#NumForm" :class="`nav-link ${selectTab == 'NumForm' ? 'active' : ''}`">機具調度分析</a></li>
                    <li  class="nav-item" v-if="step >0  || ReductionTotal != null  "><a @click="changeTab('ItemNumForm')" data-toggle="tab" href="#ItemNumForm"  :class="`nav-link ${selectTab == 'ItemNumForm' ? 'active' : ''}`">機具減碳量畫計算表</a></li>
                    <!-- <button role="button" class="btn btn-color11-3 btn-sm mx-1" id="savenew" @click="store.dispatch('addReduction', targetId)">
                        <i class="fas fa-bolt" >&nbsp;更新</i>
                    </button> -->
                    <!-- <button role="button" class="btn btn-color11-1 btn-sm mx-1" id="savenew">
                        <i class="fas fa-download">&nbsp;下載</i>
                    </button> -->
                </ul>
            </div>
            <div class="tab-content">
                <NumForm   v-if="ReductionTotal == null" id="NumForm" :class="`tab-pane ${selectTab == 'NumForm' ? 'active' : ''}`"></NumForm>
                <ItemNumForm  v-if="selectTab == 'ItemNumForm'" ref="childRef" id="ItemNumForm" :class="`tab-pane ${selectTab == 'ItemNumForm' ? 'active' : ''}`" ></ItemNumForm>
            </div>
      
        </div>
        <div class="row justify-content-center mb-3" v-if="carbonReductionCalTag">
            <button role="button" class="btn btn-color11-2 btn-xs mx-1" @click="mergeData"><i class="fas fa-save">&nbsp;儲存</i></button>
            <div v-if="step == 0 && ReductionTotal == null">
                <p  v-if="selectTab == 'NumForm'" style="color:red; margin-bottom: 0px;">*按儲存顯示機具減碳量計算頁籤</p>
       
            </div>

            <p  v-if="selectTab == 'ItemNumForm'" style="color:blue; margin-bottom: 0px;">*按儲存計算減碳量</p>
        </div>

    </div>
</template>

