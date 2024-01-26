<template>
    <div>

        <div class="table">

            <table class="table table3 min910" border="0" >
                <slot name="table" :items="store.factorList">
                    <thead>
                        <tr>
                            <th>編碼</th>  
                            <th>工作項目</th>    
                            <th>碳排係數(kgCO2e)</th>    
                            <th>單位</th>  
                            <th>細目編碼</th>    
                            <th>備註</th>
                        </tr>
                    </thead>

                    <tbody>
                        <tr v-for="(item, index) in store.factorList" v-bind:key="index">
                            <td>
                                <div >{{item.Code}}</div>
                            </td>
                            <td>
                                <div >{{item.Item}}</div>
                            </td>
                            <td>
                                <div >{{item.KgCo2e}}</div>
                            </td>
                            <td>
                                <div >{{item.Unit}}</div>
                            </td>
                            <td>
                                <div >{{item.SubCode}}</div>
                            </td>
                            <td>
                                <div >{{item.Memo}}</div>
                            </td>

                        </tr>
                    </tbody>
                </slot>

            </table>
        </div>
    </div>
</template>

<script setup>

 import {defineProps, onMounted} from "vue"
 const {route, params} = defineProps(["route", "params"])
 import {store} from "./CarbonFactorViewStore.js";
 onMounted(async() => {
    await store.getFactorList(params.type);
})

</script>