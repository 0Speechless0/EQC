<script setup>

import ProposalReviewEdit from "./ProposalReviewEdit";
import {ref, onMounted} from "vue";
const selectTab  = ref(-1);
const levelIndexs = ref([]);
const selectIndex = ref(-1);
const selectOptions = ref([])
const targetId  = ref(null);
onMounted(async () => {
    let urlParams = new URLSearchParams(window.location.search);
    if (urlParams.has('id')) {
        selectOptions.value.push (parseInt(urlParams.get('id'), 10) );

    }

    selectTab.value = selectOptions.value[0];
    (await getSelectOption(selectTab.value) ).forEach(e => {
        selectOptions.value.push(e);
    })

    console.log(selectOptions.value);
})


async function getSelectOption(tar)
{
    var data = (await window.myAjax.post("ERProposalReview/GetProposalReviwCopyId", {seq : tar})).data
    if(data.length > 0)
    return     data.split(",").map(e => parseInt(e));
    return [];
}
async function BranchProposalReview()
{
    var respId = (await window.myAjax.post("ERProposalReview/BranchProposalReview", {seq : selectTab.value }) ).data;
    if(respId  > 0) {
        alert("完成");
        selectOptions.value.push( respId);
    }
}
async function setProjectStatus(option ,index)
{
    selectTab.value = option;
    selectIndex.value =index;
    // var Seqs = await  getSelectOption(option);
    // console.log("Seqs", Seqs);

    // if(Seqs.length > 0)
    // {

    //     selectOptions.value = selectOptions.value.slice(0, index +1).concat(getSelectOption(option));

    // }
      
}

</script>
<template>

<div >
    <ul class="nav nav-tabs" role="tablist">
                <li class="nav-item" @click="() => {selectTab = selectOptions[0];selectIndex= -1}"><a  data-toggle="tab" href="" class="nav-link active">原案</a></li>
                <li :key="index" v-for="(option, index) in selectOptions.slice(1)" class="nav-item"  @click="setProjectStatus(option, index)" > <a  data-toggle="tab" href="" class="nav-link">分案 {{index+1}}</a></li>  
                <li >
                    <!-- <a @click="BranchProposalReview" download class="btn btn-color11-4 btn-xs mx-1"><font style="vertical-align: inherit;"><font style="vertical-align: inherit;"> (<span v-if="selectIndex >= 0">分案 {{selectIndex+1}} </span> <span v-else>原案 </span>) 分案/分期</font></font></a> -->
            </li>
            </ul>

    <div class="tab-content" >
        <div v-for="(option, index) in selectOptions"  :key="index">
            <ProposalReviewEdit   :tenderId="option" v-if="selectTab == option"  ></ProposalReviewEdit >
        </div>


    </div>
</div>
</template>