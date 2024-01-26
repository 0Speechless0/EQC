<template>
<div v-if="!engInfo.EngNo" class="" style="height: 200px;">
    <input type="file" class="btn btn-color11-2 btn-sm mx-1"  @change="(e) => uploadXML(e.target.files[0])" id="importJson" hidden/>
                        <label  class="btn btn-color11-1 btn-sm mx-1"  style="margin-bottom: 0px;" for="importJson">匯入PCCES</label>
</div>
<div class="pageContent" v-else>
    <h5 class="insearch py-2">
        工程編號：{{ engInfo.TenderNo }} ( {{ engInfo.EngNo }}  )<br>工程名稱：{{ engInfo.TenderName }}  ({{ engInfo.EngName }} )
    </h5>

    <div class="tab-content">
        <!-- 一 -->
        <div id="menu01" class="tab-pane active">
            <h5>
                工程天數 {{engInfo.EngPeriod}} 天，工程總價：<span style="color:yellowgreen">{{engInfo.SubContractingBudget}}</span> 元。 <br />
                總碳排放量：<span style="color:blue"> {{co2Total }} </span>KgCo2e。<br />
                可拆解率：<span style="color:red">{{ dismantlingRate }} </span> % 。 <br />
            </h5>
            <form class="form-group insearch mb-3">
                <div class="form-row d-flex">
                    <div class="col "  v-if="!loading">
                        <input type="file" class="btn btn-color11-2 btn-sm mx-1"  @change="(e) => uploadXML(e.target.files[0])" id="importJson" hidden/>
                        <label  class="btn btn-color11-1 btn-sm mx-1 "  style="margin-bottom: 0px;" for="importJson">匯入PCCES</label>
                    </div>
                    <div class="col" v-else>
                        <p style="color:green" class="col" > 檔案處理中，請耐心等候 ....</p>
                    </div>

                </div>
            </form>
            <comm-pagination ref="pagination" :recordTotal="totalRows" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
            <table class="table table-bordered">
                <thead style="background:#f2f2f2">
                    <tr>
                        <th scope="col">#</th>
                        <th scope="col">項目及說明</th>

                        <th scope="col">碳排係數</th>
                        <th scope="col">工項碳排量</th>
                        <th scope="col">數量</th>
                        <th scope="col">單位</th>
                        <th scope="col">編碼(備註)</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in itemList" :key="index">
                        <th scope="row">{{ index +1 }}</th>
                        <td>{{item.Description}}</td>
                        <td>{{item.KgCo2e}}</td>
                        <td>{{item.ItemKgCo2e}}</td>
                        <td>{{ item.Quantity}}</td>
                        <td>{{item.Unit}}</td>
                        <td>{{item.Memo}}</td>

                    </tr>

                </tbody>
            </table>
        </div>
    </div>
</div>

</template>

<script setup>
import store from "../../store/carbonEmissionCalXMLStore";
import {computed, ref, onMounted} from "vue";
window.store = store;
const loading = ref(false);
const engInfo = computed(() => store.state.engInfo);
const itemList = computed(() => store.state.carbonCalInfo.items );
const totalRows = computed(() => store.state.totalRows);
const dismantlingRate = computed(() => store.getters.dismantlingRate);
const co2Total = computed(() => store.state.carbonCalInfo.co2Total);

function onPaginationChange(pInx, pCount)
{
    console.log(pCount);
    store.commit("onPaginationChange", {
        pInx : pInx,
        pCount : pCount
    });
    store.dispatch("getCarbonCalInfo", 1);
}

async function uploadXML(file)
{   
    loading.value = true;
    await store.dispatch("uploadXML", file);
    await store.dispatch("getEng", 1); 
    loading.value = false;
}
onMounted(async() => {
    // await store.dispatch("getEng", 1); 
    // await store.dispatch("getCarbonCalInfo", 1);
});

</script>