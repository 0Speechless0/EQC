<template>
    <div>
        <div class="nav nav-tabs border-0 ">
            <div class="nav nav-tabs border-0 col-12 d-flex justify-content-around">
                <div class="nav-item" >
                    <button  data-toggle="tab" v-on:click.stop="selectTab = 0" href="#" :class="`btn-block  btn btn-color4 ${ selectTab == 0 ? 'active' : ''}`" >挖土機  </button>
                </div>
                <div class="nav-item" >
                    <button  data-toggle="tab" v-on:click.stop="selectTab = 1" href="#" :class="`btn-block  btn btn-color4 ${ selectTab == 1 ? 'active' : ''}`" >傾卸貨車  </button>
                </div>
                <div class="nav-item" >
                    <button  data-toggle="tab" v-on:click.stop="selectTab = 2" href="#" :class="`btn-block  btn btn-color4 ${ selectTab == 2 ? 'active' : ''}`" >能源減碳  </button>
                </div>
            </div>


        </div>
        <div class="nav nav-tabs border-0 ">
            <div class="nav nav-tabs border-0 col-12 d-flex justify-content-center" v-for="(codeOption, index2) in store.codeOptionStack" :key="index2">
                <div class="nav-item" v-for="(option, index) in codeOption" :key="index">
                    <button  data-toggle="tab" v-on:click.stop="store.getNewOption(index2+2, option)" href="#" :class="`btn-block  btn btn-color4 ${store.optionSelectedStack[index2] == option ?'active' : '' }`" >{{ option }}</button>
                </div>
            </div>


        </div>
        <div>
            <CarbonFactorView :route="'CarbonReduction/GetList'"  :params="{ type : selectTab }"  >
                <template #table="{items}">
                    <thead v-if="selectTab == 0">
                        <tr>
                            <th>編碼</th>  
                            <th>工作項目</th>    
                            <th>類型1</th>    
                            <th>類型2</th>
                            <th>PC120</th>
                            <th>PC200</th>
                            <th>PC300</th>
                            <th>PC400</th>  
                            <th>單位</th>    
                        </tr>
                    </thead>
                    <thead v-if="selectTab == 1">
                        <tr>
                            <th>編碼</th>  
                            <th>工作項目</th>    
                            <th>類型1</th>    
                            <th>類型2</th>
                            <th>15T</th>
                            <th>21T</th>
                            <th>35T</th>  
                            <th>單位</th>    
                        </tr>
                    </thead>
                    <thead v-if="selectTab == 2">
                        <tr>
                            <th>編碼</th>  
                            <th>工作項目</th>    
                            <th>類型1</th>    
                            <th>類型2</th> 
                            <th>KgCo2e</th>
                            <th>單位</th>    
                        </tr>
                    </thead>
                    <tbody v-if="selectTab == 0">
                        <tr v-for="(item, index) in items" v-bind:key="index">
                            <td>
                                <div >{{item.Code}}</div>
                            </td>
                            <td>
                                <div >{{item.Description}}</div>
                            </td>
                            <td>
                                <div >{{item.Type1}}</div>
                            </td>
                            <td>
                                <div >{{item.Type2}}</div>
                            </td>
                            <td>
                                <div >{{item.PC120}}</div>
                            </td>
                            <td>
                                <div >{{item.PC200}}</div>
                            </td>
                            <td>
                                <div >{{item.PC300}}</div>
                            </td>
                            <td>
                                <div >{{item.PC400}}</div>
                            </td>
                            <td>
                                <div >{{item.Unit}}</div>
                            </td>
                        </tr>
                    </tbody>
                    <tbody v-if="selectTab == 1">
                        <tr v-for="(item, index) in items" v-bind:key="index">
                            <td>
                                <div >{{item.Code}}</div>
                            </td>
                            <td>
                                <div >{{item.Description}}</div>
                            </td>
                            <td>
                                <div >{{item.Type1}}</div>
                            </td>
                            <td>
                                <div >{{item.Type2}}</div>
                            </td>
                            <td>
                                <div >{{item.C15T}}</div>
                            </td>
                            <td>
                                <div >{{item.C21T}}</div>
                            </td>
                            <td>
                                <div >{{item.C35T}}</div>
                            </td>
                            <td>
                                <div >{{item.Unit}}</div>
                            </td>

                        </tr>
                    </tbody>
                    <tbody v-if="selectTab == 2">
                        <tr v-for="(item, index) in items" v-bind:key="index">
                            <td>
                                <div >{{item.Code}}</div>
                            </td>
                            <td>
                                <div >{{item.Description}}</div>
                            </td>
                            <td>
                                <div >{{item.Type1}}</div>
                            </td>
                            <td>
                                <div >{{item.Type2}}</div>
                            </td>
                            <td>
                                <div >{{item.KgCo2e}}</div>
                            </td>
                            <td>
                                <div >{{item.Unit}}</div>
                            </td>

                        </tr>
                    </tbody>
                </template>
            </CarbonFactorView>
        </div>

    </div>
</template>
<script setup>
    import CarbonFactorView from "./CarbonFactorView.vue";
    import {store} from "./CarbonFactorViewStore.js";
    import {ref, watch} from "vue";
    const selectTab = ref(0);
    watch(selectTab, (newValue) => {
        store.getFactorList(newValue);
    })

</script>