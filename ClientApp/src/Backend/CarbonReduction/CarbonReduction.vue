<template>
        <div >

            <div class="row d-flex bd-highlight mb-3" style="padding-left:10px">
                <label class="btn btn-shadow btn-color11-3">
                    <input v-on:change="tableControl.fileChange($event)" id="inputFile" type="file" name="file" multiple="" style="display:none;">
                    <i class="fas fa-upload"></i> 批次匯入Excel
                </label>
                <a :href="'./CarbonReduction/Download'" class="bd-highlight mr-auto" download style="padding-left: 15px;" >
                    <button role="button" class="btn btn-shadow btn-color11-3 btn-block">
                        下載範例

                    </button>
                </a>



                <div class="d-flex pl-3 pb-2 col-4">
                    <input v-model="data.searchStr" class="form-control col-6"/>
                    <button type="button" class="btn btn-outline-success " @click="tableControl.search()">搜尋</button>
                    <div class="align-self-center ml-2 " style="color:red">
                        可輸入編碼
                    </div>

                </div>

                <div class=" bd-highlight align-self-center  ml-auto" style="padding-right: 15px;">
                        更新日期:{{tableControl.getlastUpdateDate(true)}}
                    </div>

            </div>
            <div class="row d-flex bd-highlight mb-3" style="padding-left:10px">
                <p class="pl-2 mr-auto" style="color:blue;">*匯入的Excel欄位位置不可與範例不同</p>


            </div>

            
            <div class="row justify-content-between mb-2">
                <comm-pagination class="col-12" :recordTotal="data.recordTotal" v-on:onPaginationChange="(pInx, pCount) =>tableControl.onPaginationChange(pInx, pCount)"></comm-pagination>
            </div>
            <div>
            <ul class="nav nav-tabs" role="tablist">
                <li class="nav-item"><a v-on:click="selectTab='NavvyTab'" data-toggle="tab" href="" :class="`nav-link ${data.tabStatus['NavvyTab']}`">挖土機</a></li>
                <li class="nav-item"><a v-on:click="selectTab='TruckTab'" data-toggle="tab" href="" :class="`nav-link ${data.tabStatus['TruckTab']}`">傾卸貨車</a></li>
                <li class="nav-item"><a v-on:click="selectTab='Tab'" data-toggle="tab" href="" :class="`nav-link ${data.tabStatus['Tab']}`">能源減碳</a></li>
            </ul>
        </div>
        <div class="tab-content">
            <NavvyTab v-if="data.selectTab=='NavvyTab'" :TableControl="tableControl"></NavvyTab>
            <TruckTab v-if="data.selectTab=='TruckTab'" :TableControl="tableControl"></TruckTab>
            <Tab v-if="data.selectTab=='Tab'" :TableControl="tableControl"></Tab>
        </div>
            <!-- <div class="table-responsive">
                <table class="table table-responsive-md table-hover table2">
                    <thead class="insearch">
                        <tr>
                            <th><strong>項次</strong></th>
                            <th><strong>編碼</strong></th>
                            <th><strong>工作項目</strong></th>
                            <th><strong>碳排係數(kgCO2e)</strong></th>
                            <th><strong>單位</strong></th>
                            <th><strong>細目編碼</strong></th>
                            <th><strong>備註</strong></th>
                            <th class="text-center"><strong>管理</strong></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                            <td style="min-width: unset;">{{pageRecordCount*(pageIndex-1)+index+1}}</td>
                            <template v-if="item.Seq != editSeq">
                                <td v-html="checkCode(item)"></td>
                                <td>{{item.Item}}</td>
                                <td>{{item.KgCo2e}}</td>
                                <td>{{item.Unit}}</td>
                                <td>{{item.SubCode}}</td>
                                <td>{{item.Memo}}</td>
                                <td style="min-width: unset;">
                                    <div class="d-flex justify-content-center">
                                        <button @click="editDetail(item)" class="btn btn-color11-2 btn-xs sharp m-1" title="碳分析"><i class="fas fa-newspaper"></i></button>
                                        <button @click="onEditRecord(item)" class="btn btn-color11-1 btn-xs sharp m-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                        <button @click="onDelRecord(item)" class="btn btn-color9-1 btn-xs sharp m-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                                    </div>
                                </td>
                            </template>
                            <template v-if="item.Seq == editSeq">
                                <td><input v-model.trim="editRecord.Code" maxlength="20" type="text" class="form-control"></td>
                                <td><input v-model.trim="editRecord.Item" maxlength="50" type="text" class="form-control"></td>
                                <td><input v-model="editRecord.KgCo2e" type="text" class="form-control"></td>
                                <td><input v-model.trim="editRecord.Unit" maxlength="10" type="text" class="form-control"></td>
                                <td><input v-model.trim="editRecord.SubCode" maxlength="20" type="text" class="form-control"></td>
                                <td><textarea v-model="editRecord.Memo" maxlength="100" rows="5" class="form-control"></textarea></td>
                                <td style="min-width: unset;">
                                    <div class="d-flex justify-content-center">
                                        <button @click="onSaveRecord(editRecord)" class="btn btn-color11-2 btn-xs sharp m-1" title="儲存"><i class="fas fa-save"></i></button>
                                        <button @click="onEditCancel" class="btn btn-color9-1 btn-xs sharp m-1" title="取消"><i class="fas fa-times"></i></button>
                                    </div>
                                </td>
                            </template>
                        </tr>
                        <tr>
                            <td></td>
                            <td><input v-model.trim="newItem.Code" maxlength="20" type="text" class="form-control"></td>
                            <td><input v-model.trim="newItem.Item" maxlength="50" type="text" class="form-control"></td>
                            <td><input v-model="newItem.KgCo2e" type="text" class="form-control"></td>
                            <td><input v-model.trim="newItem.Unit" maxlength="10" type="text" class="form-control"></td>
                            <td><input v-model.trim="newItem.SubCode" maxlength="20" type="text" class="form-control"></td>
                            <td><textarea v-model="newItem.Memo" maxlength="100" rows="5" class="form-control"></textarea></td>
                            <td style="min-width: unset;">
                                <div class="d-flex justify-content-center">
                                    <button @click="onSaveRecord(newItem)" class="btn btn-outline-secondary btn-xs sharp m-1" title="新增"><i class="fas fa-plus"></i></button>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div> -->
        </div>
</template>
<script setup>

import {reactive,watch, ref } from 'vue';
import TableControl  from './TableControl';
import NavvyTab from './NavvyFactorTab.vue';
import TruckTab from './TruckFactorTab.vue';
import Tab from './FactorTab.vue';

const tableControl = new TableControl("NavvyTab");
const data = reactive(tableControl);
const selectTab = ref(data.selectTab);

watch(selectTab, (fnew) => {
    console.log("dddd", fnew);
    data.selectTab = fnew;
    data.pageIndex = 1;
} )
</script>