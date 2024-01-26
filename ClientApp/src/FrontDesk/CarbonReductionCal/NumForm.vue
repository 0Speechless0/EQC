<style scoped>
    td {
        text-align: center !important;
    }
    input {
        text-align: center !important;
    }
</style>

<script setup>
import { onMounted,  watch, reactive, ref, computed } from 'vue';
const store = reactive(window.carbonReductionCalStore);
const navvyNum = computed(() => store.state.navvyNum );
const truckNum = computed( () => store.state.truckNum );
const energyNum = computed( () => store.state.energyNum );

const targetId = window.sessionStorage.getItem(window.epcSelectTrenderSeq);
onMounted(async() => {

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

    store.commit("setNum", {key:"navvy", value: data.navvy});
    store.commit("setNum", { key:"truck", value: data.truck} );
    store.commit("setNum", { key :"energy", value: data.energy});


});


</script>

<template>
    <div class="tab-pane">
        <h5>挖土機</h5>
        <p>單位：台</p>
        <div class="table-responsive">
        <table border="0" class="table table1 min910">
            <thead>
                <tr>
                    <th>項次</th>
                    <th class="col-2">類型 </th>
                    <th class="col-1">單位</th>
                    <th>PC120</th>
                    <th>PC200</th>
                    <th>PC300</th>
                    <th>PC400</th>
                </tr>

            </thead>
            <tbody>
                <tr v-for="(item, index) in navvyNum" :key="index">
                    <td>{{ index+1 }}</td>
                    <td>
                        {{ item.Type2 }}
                    </td>
                    <td>
                        {{ item.Unit }}
                    </td>
                    <td>
                        <input class="form-control" v-model="item.PC120Num"  type="number"/> 
                    </td>
                    <td>
                        <input class="form-control" v-model="item.PC200Num" type="number"/> 
                    </td>
                    <td>
                        <input class="form-control" v-model="item.PC300Num"  type="number" /> 
                    </td>
                    <td>
                        <input class="form-control" v-model="item.PC400Num"  type="number" /> 
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <h5>傾卸貨車</h5>
    <p>單位：台</p>
    <div class="table-responsive">
        <table border="0" class="table table1 min910">
            <thead>
                <tr>
                    <th>項次</th>
                    <th class="col-2">類型</th>
                    <th class="col-1">單位</th>
                    <th>15T</th>
                    <th>21T</th>
                    <th>35T</th>
                </tr>

            </thead>
            <tbody>
                <tr v-for="(item, index) in truckNum" :key="index">
                    <td>{{ index+1 }}</td>
                    <td>
                        {{ item.Type2 }}
                    </td>
                    <td>
                        {{ item.Unit }}
                    </td>
                    <td>
                        <input class="form-control" v-model="item.C15TNum"  type="number"/> 
                    </td>
                    <td>
                        <input class="form-control" v-model="item.C21TNum"  type="number"/> 
                    </td>
                    <td>
                        <input class="form-control" v-model="item.C35TNum"  type="number"/> 
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <h5>能源減碳</h5>
    <p>單位：度</p>
    <div class="table-responsive">
        <table border="0" class="table table1 min910">
            <thead>
                <tr>
                    <th>項次</th>
                    <th class="col-2">類型</th>
                    <th class="col-1">單位</th>
                    <th>度</th>
                </tr>

            </thead>
            <tbody>
                <tr v-for="(item, index) in energyNum" :key="index">
                    <td>{{ index+1 }}</td>
                    <td>
                        {{ item.Type2 }}
                    </td>
                    <td>
                        {{ item.Unit }}
                    </td>
                    <td>
                        <input class="form-control" v-model="item.Num"  type="number"/> 
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    </div>


</template>

