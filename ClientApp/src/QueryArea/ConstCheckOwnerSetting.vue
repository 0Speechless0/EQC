<template>

    <div >
        <div class="card-header">
            <h3 class="card-title font-weight-bold">設定抽查者</h3>
        </div>
        <div id="app" class="card-body">
            <div id="app">
                <form class="form-group insearch mb-3">
            <div class="form-row">
                <div class ="d-flex">
                    <label for="selectYearOption" class="mr-2">發包年度</label>
                    <div>
                        <select v-model="condition.selectYear" class="form-control"  @change="onYearChange" >
                            <option v-for="option in yearOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                    {{ option.Text }}
                                </option>
                        </select>
                    </div>

                </div>
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                    <select v-model="condition.selectUnit" class="form-control" @change="onUnitChange">
                        <option v-for="option in unitOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                    <input v-model="condition.keyWord" type="text" placeholder="標案名稱關鍵字" class="form-control">
                </div>
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                    <button v-on:click.stop="onSearch()" type="button" class="btn btn-outline-secondary btn-xs mx-1" data-dismiss="modal">查詢 <i class="fas fa-search"></i></button>
                </div>

            </div>
        </form>
            <div class="row justify-content-between">
                <comm-pagination class="col-12" :recordTotal="listCount" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
            </div>
                <div class="table-responsive">
                    <table border="0" class="table table1 min910">
                        <thead class="insearch">
                            <tr>
                                <th class="sort">排序</th>
                                <th class="number">工程編號</th>
                                <th>工程名稱</th>
                                <!-- <th>狀態</th> -->
                                <th>執行機關</th>
                                <th>執行單位</th>
                                <th>選擇抽查者</th>
                                <th class="text-center">功能</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index2) in viewItems" :key="index2">
                                <td>{{ index2+1 }}</td>
                                <td>{{ item.EngNo }}</td>
                                <td>{{ item.EngName }}</td>
                                <!-- <td><i class="fa fa-circle text-success mr-1"></i>{{ item.ExecState }}</td> -->
                                <td>{{ item.ExecUnit }}</td>
                                <td>
                                    <div class="row justify-content-center m-0" v-if="settingIndex == index2">
                                        <select class="form-control " id="selectBox" v-model="checkUserSetting.selectSubUnit" @change="onSubUnitChange(index2)">
                                            <option v-for="(option, index) in subUnitOptions" :key="index" :value="option.Value">{{ option.Text }}  </option>
                                        </select>
                                    </div>
                                    <div v-else>
                                            {{ item.ExecSubUnit }}
                                    </div>
                                </td>
                                <td style="min-width: 105px;">
                                    <div class="row justify-content-center m-0" v-if="settingIndex == index2">
                                        <select class="form-control " v-model="checkUserSetting.selectUser">
                                            <option v-for="(option, index) in selectSubUnitUserOption " :value="option.Value" :key="index"> {{ option.Text }} </option>
 
                                        </select>
                                    </div>
                                    <div v-else>
                                            {{ item.UserName }}
                                    </div>
                                </td>
                                <td style="min-width: 105px;">
                                    <div class="row justify-content-center m-0">
                                        <button title=""  id="editButton" class="btn btn-color9-1 btn-xs mx-1" v-if="settingIndex == index2" @click="SaveSetting(item.Seq)">
                                            <i class="fas fa-check"></i> 確定
                                        </button>
                                        <button  v-else title=""  id="cancelButton" class="btn btn-color11-3 btn-xs mx-1"  @click="selectSettingIndex(index2)">
                                            <i class="fas fa-pencil-alt"></i> 編輯
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>

</template>

<script setup>
import {computed, onMounted, reactive, ref} from  "vue";
const condition = reactive({
    pageCount : 10,
    pageIndex :1,
    selectUnit :null,
    selectYear : null,
    keyWord : null,

});
var originItems;
const viewItems  = ref([]);
const state = reactive(window.epcTenderStore.state);  

const handleOriginItems = async () => {
    const {data } = await window.myAjax.post('ConstCheckUser/GetList', condition) ;
    let tempOriginItems = [];
    data.forEach(element => {
        tempOriginItems[element.EngSeq] = element;
    });
    originItems = tempOriginItems;
}
const settingIndex = ref(null);
const checkUserSetting = reactive({
    selectSubUnit : null,
    selectUser : null,

})


const SaveSetting = async (EngSeq) => {
    const {data} = await window.myAjax.post("ConstCheckUser/Update", {
        EngSeq : EngSeq,
        UserSeq : checkUserSetting.selectUser,
        UnitSeq : checkUserSetting.selectSubUnit
    });
    if(data == true) 
    {
        alert("更新成功!");
        settingIndex.value= null;
        onSearch();
    }
}


const selectSettingIndex = (index) => {
    settingIndex.value = index;
}

const onSearch = async () => {
    await handleOriginItems();
    await window.epcTenderStore.dispatch("getEPCTenterList", condition)
    console.log("list", list);
    viewItems.value = list.value.map(e => {
        return {
            ... e,
            ... originItems[e.Seq]
        }
    })
    console.log("originItems", originItems );
    console.log("viewItems", viewItems.value );
}
const unitOptions = computed(() => state.selectUnitOptions);
const subUnitOptions = computed(() => state.selectSubUnitOptions);
const yearOptions = computed(() => state.selectYearOptions);
const selectSubUnitUserOption = computed(() => state.selectSubUnitUserOption);
// const selectYear = computed(() => state.selectYear);
// const selectUnit = computed(() => state.selectUnit);
const list = computed(() => state.list);
const listCount = computed(() => state.listCount);

onMounted(async () => {
    await window.epcTenderStore.dispatch("getEPCYearOption");
})

const onYearChange = (event) => {
    window.epcTenderStore.dispatch("getEPCUnitOption", condition.selectYear);
} 

const onUnitChange = () => {
    window.epcTenderStore.dispatch("getSubUnitOption", condition.selectUnit);
}

const onSubUnitChange = async (index) => {

    await window.epcTenderStore.dispatch("getSubUnitUserOption", checkUserSetting.selectSubUnit);


}
// const onUnitChange = (event) => {
//     window.epcTenderStore.commit("onChangeUnit", event.target.value);
// }

const onPaginationChange = (pInx, pCount) => {
    //console.log("pInx:" + this.$refs['pagination'].pageIndex + " pCount:" + pCount);
    condition.pageCount = pCount;
    condition.pageIndex = pInx;
}



</script>