<style scoped>
    td {
        text-align: center !important;
    }
    input {
        text-align: center !important;
    }
</style>
<script setup>

import { onMounted, computed, reactive } from 'vue';
const store = reactive(window.carbonReductionCalStore);


const navvyItems = computed( () => store.state.navvyItems);
const truckItems = computed(() => store.state.truckItems);
const energyItems = computed( () => store.state.energyItems );

const targetId = window.sessionStorage.getItem(window.epcSelectTrenderSeq);
console.log("engMainId", targetId);
onMounted(async() => {

    store.commit("computeTypeNumDic");
    store.commit("getCalItem", targetId);
});



// const PC120weightTotal = computed(() => navvyItems.value.reduce((a, c) => a+ c.PC120Weight, 0) )
// const PC200weightTotal = computed(() => navvyItems.value.reduce((a, c) => a+ c.PC200Weight, 0) )
// const PC300weightTotal = computed(() => navvyItems.value.reduce((a, c) => a+ c.PC300Weight, 0) )
// const PC400weightTotal = computed(() => navvyItems.value.reduce((a, c) => a+ c.PC400Weight, 0) )
// const C15TWeightTotal = computed(() => truckItems.value.reduce((a, c) => a+c.C15TWeight, 0))
// const C21TWeightTotal = computed(() => truckItems.value.reduce((a, c) => a+c.C21TWeight, 0))
// const C35TWeightTotal = computed(() => truckItems.value.reduce((a, c) => a+c.C35TWeight, 0))

function computedNavvyReduction(item)
{
    item.TempResult = item.Quantity*(item.PC120Num* item.factor.PC120 + item.PC200Num*item.factor.PC200 + item.PC300Num *item.factor.PC300 + item.PC400Num*item.factor.PC400)
    return item.TempResult;
}
function computedTruckReduction(item)
{

    item.TempResult = item.Quantity*(item.C15TNum * item.factor.C15T + item.C21TNum * item.factor.C21T + item.C35TNum * item.factor.C35T)
    return item.TempResult;
}

function computedEnergyReduction(item)
{
    item.TempResult = (item.Num*item.factor.KgCo2e)
    return item.TempResult;
}
// function computedNavvyReduction(item)
// {

//     let itemNavvyNum = navvyNum.value[item.factor.Type2.trim()] ;

//     if(!itemNavvyNum ) return ;
//     let reduction = -item.Quantity* (
//         item.factor.PC120*item.PC120Weight*itemNavvyNum.PC120Num / PC120weightTotal.value +
//         item.factor.PC200*item.PC200Weight*itemNavvyNum.PC200Num / PC200weightTotal.value+
//         item.factor.PC300*item.PC300Weight*itemNavvyNum.PC300Num / PC300weightTotal.value +
//         item.factor.PC400*item.PC300Weight*itemNavvyNum.PC300Num / PC400weightTotal.value
//     ) /
//     (
//         item.PC120Weight*itemNavvyNum.PC120Num +
//         item.PC200Weight*itemNavvyNum.PC200Num +
//         item.PC300Weight*itemNavvyNum.PC300Num +
//         item.PC400Weight*itemNavvyNum.PC400Num 
//     )
//     item.reduction = reduction;
//     return reduction.toFixed(0);
// }
// function computedTruckReduction(item)
// {

//     let itemTruckNum = truckNum.value[item.factor.Type2.trim() ];
//     if(!itemTruckNum ) return ;
//     let reduction = -item.Quantity* (
//         item.factor.C15T*item.C15TWeight*itemTruckNum.C15TNum / C15TWeightTotal.value +
//         item.factor.C21T*item.C21TWeight*itemTruckNum.C21TNum / C21TWeightTotal.value +
//         item.factor.C35T*item.C35TWeight*itemTruckNum.C35TNum / C35TWeightTotal.value
//     ) /
//     (
//         item.C15TWeight*itemTruckNum.C15T +
//         item.C21TWeight*itemTruckNum.C21T +
//         item.C35TWeight*itemTruckNum.C35T 
//     );
//     item.reduction = reduction;
//     return reduction.toFixed(0);

// }
// function computedEnergyReduction(item)
// {
//     let reduction = -item.Quantity* item.factor.KgCo2e;
//     item.reduction = reduction;
//     return reduction.toFixed(0);

// }
</script>
<template>
    <div class="tab-pane">
        <h5>挖土機</h5>
        <div class="table-responsive">
            <table border="0" class="table table1 min910">
                <thead>
                    <tr>
                        <th class="col-1">項次</th>
                        <th class="col-1">類型</th>
                        <th class="col-1">單位</th>
                        <th class="col-2">工作項目</th>
                        <th class="col-2">編碼</th>
                        <th>PC120</th>
                        <th>PC200</th>
                        <th>PC300</th>
                        <th>PC400</th>
                        <th>減碳量評估(kgCO2e)</th>
                    </tr>

                </thead>
                <tbody>
                    <tr v-for="(item, index) in navvyItems" :key="index">
                        <td>{{ index+1 }}</td>
                        <td>
                            {{ item.factor.Type2 }}
                        </td>
                        <td>
                            {{ item.factor.Unit }}
                        </td>
                        <td>
                            {{ item.Description }}
                        </td>
                        <td>
                            {{ item.RefItemCode }}
                        </td>
                        <td>
                            <input class="form-control" v-model="item.PC120Num" type="number"/> 
                        </td>
                        <td>
                            <input class="form-control" v-model="item.PC200Num" type="number"/> 
                        </td>
                        <td>
                            <input class="form-control" v-model="item.PC300Num" type="number" /> 
                        </td>
                        <td>
                            <input class="form-control" v-model="item.PC400Num" type="number" /> 
                        </td>
                        <td>
                            {{computedNavvyReduction(item)}}
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <h5>傾卸貨車</h5>
        <div class="table-responsive">
            <table border="0" class="table table1 min910">
                <thead>
                    <tr>
                        <th class="col-1">項次</th>
                        <th class="col-1">類型</th>
                        <th class="col-1">單位</th>
                        <th class="col-2">工作項目</th>
                        <th class="col-2">編碼</th>
                        <th>15T</th>
                        <th>21T</th>
                        <th>35T</th>
                        <th>減碳量評估(kgCO2e)</th>
                    </tr>

                </thead>
                <tbody>
                    <tr v-for="(item, index) in truckItems" :key="index">
                        <td>{{ index+1 }}</td>
                        <td>
                            {{ item.factor.Type2 }}
                        </td>
                        <td>
                            {{ item.factor.Unit }}
                        </td>
                        <td>
                            {{ item.Description }}
                        </td>
                        <td>
                            {{ item.RefItemCode }}
                        </td>
                        <td>
                            <input class="form-control" v-model="item.C15TNum" type="number"/> 
                        </td>
                        <td>
                            <input class="form-control" v-model="item.C21TNum" type="number"/> 
                        </td>
                        <td>
                            <input class="form-control" v-model="item.C35TNum" type="number" /> 
                        </td>
                        <td>
                            {{computedTruckReduction(item)}}
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <h5>能源減碳</h5>
        <div class="table-responsive">
            <table border="0" class="table table1 min910">
                <thead>
                    <tr>
                        <th class="col-1">項次</th>
                        <th class="col-1">類型</th>
                        <th class="col-1">單位</th>
                        <th class="col-2">工作項目</th>
                        <th class="col-2">編碼</th>
                        <th> </th>
                        <th>減碳量評估</th>
                    </tr>

                </thead>
                <tbody>
                    <tr v-for="(item, index) in energyItems" :key="index">
                        <td>{{ index+1 }}</td>
                        <td>
                            {{ item.factor.Type2 }}
                        </td>
                        <td>
                            {{ item.factor.Unit }}
                        </td>
                        <td>
                            {{ item.factor.Description }}
                        </td>
                        <td>
                            {{ item.RefItemCode }}
                        </td>
                        <td><input class="form-control" v-model="item.Num" type="number" /> </td>
                        <td>
                            {{computedEnergyReduction(item)}}
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>