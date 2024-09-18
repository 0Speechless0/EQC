<template>
    <div>
        <div class="row">
            <div class="col-6 d-flex ">
                <input class="form-control col-3 ml-2"  type="date" v-model="dateFrom"/>
                <div  style="font-size: 22px;" class="mr-2">
                    開始，
                </div>
                <div  style="font-size: 22px;color:blue" class="mr-2">
                    前
                </div>
                <input class="form-control col-1 ml-2"  type="number" v-model="days"/>
                <div  style="font-size: 22px;" class="mr-2">
                    天的資料
                </div>
            </div>
            <a :href="`LoginRecord/DownloadUserRecord?from=${dateFrom}&days=${days}`" download class="btn btn-color11-1 btn-xs mx-1"><i class="fas fa-download"></i><font style="vertical-align: inherit;"><font style="vertical-align: inherit;">下載</font></font></a>
        </div>

        <div class="table-responsive">

            <table class="table table-responsive-md">
                <thead>
                    <tr class="insearch">
                        <th style="text-align: start;"><strong>登入帳號</strong></th>
                        <th><strong>名稱</strong></th>
                        <th><strong>所屬單位</strong></th>
                        <th><strong>職稱</strong></th>
                        <th><strong>連線位置</strong></th>
                        <th><strong>登入時間</strong></th>
                    </tr>

                </thead>
                <tbody>
                    <tr v-for="(record, index) in data.list" :key="index">
                        <td style="text-align: start;">{{ record.UserNo }}</td>
                        <td>{{ record.DisplayName }}</td>

                        <td v-if="record.Unit">
                            {{ record.Unit }}
                        </td>
                        <td v-else></td>
                        <td v-if="record.Position" >{{ record.Position.Name }}</td>
                        <td v-else></td>
                        <td>{{ record.OriginIP }}</td>
                        <td>{{ record.CreateTime }}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>



</template>
<script setup>

import { watchEffect, ref } from 'vue';
const data = ref({});
const unitDic = ref([]);
const days =ref(1);
const now = new Date();
const dateFrom = ref(`${now.getFullYear()}-${now.getMonth()+1}-${now.getDate()}`);
console.log("dateFrom", dateFrom.value);

watchEffect(async() => {
    if(days.value == "") return;
    data.value = (await window.myAjax.post("LoginRecord/GetList", {
        from : dateFrom.value ?? new Date().toDateString(),
        days : days.value
    })).data;
    data.value.unitList.forEach(element => {
        unitDic.value[element.Seq] = element.Name;
    });
})

</script>