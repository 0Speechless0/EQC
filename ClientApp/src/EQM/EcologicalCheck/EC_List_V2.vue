<script setup>

// import downloadList from "../../components/downloadList.vue";
import Modal from "../../components/Modal.vue";
import { ref, onMounted } from "vue";
import { useFileStore } from "../../store/FileStore.js";
const store = useFileStore("EQMEcologicalCheck");
const targetId = ref(window.sessionStorage.getItem(window.eqSelTrenderPlanSeq));
const tenderItem = ref({

});
const tarRecord = ref({})

onMounted(async () => {
    let { data } = await window.myAjax.post('/EQMEcologicalCheck/GetEngMain', { id: targetId.value });
    tenderItem.value = data.item;
    await getResords();


})

async function getResords() {
    let { data } = await window.myAjax.post('/EQMEcologicalCheck/GetList', { id: targetId.value })


    tarRecord.value = data.item;

}

function filePlaceholder(d) {
    if (this.strEmpty(d))
        return "未選擇任何檔案";
    else
        return d;
}

function strEmpty(str) {
    return window.comm.stringEmpty(str);
}

const modalList = ref([]);
const modal = ref(null);

function showModal(groupKey)
{
    modal.value.show = true;
    modalList.value =    store.getTempFiliesName(groupKey)

}
</script>


<template>
    <div v-if="targetId > 0">
        <h5 class="insearch mt-0 py-2">
            工程編號：{{ tenderItem.TenderNo }}({{ tenderItem.EngNo }})<br>工程名稱：{{ tenderItem.TenderName }}({{ tenderItem.EngName }})
        </h5>

        <div class="form-group row">
            <label class="col-md-4 col-form-label">是否須辦理生態檢核</label>
            <div class="col-md-8">
                <select v-model="tarRecord.ToDoChecklit" class="form-control">
                    <option value="1">是(新建工程)</option>
                    <option value="2">是(其他)</option>
                    <option value="3">否(災後緊急處理、搶修、搶險)</option>
                    <option value="4">否(災後原地復建)</option>
                    <option value="5">否(原構造物範圍內整建或改善之工程，且經上級機關審查確認)</option>
                    <option value="6">否(已開發場所之工程，且經上級機關審查確認)</option>
                    <option value="7">否(劃取得綠建築標章並納入生態範疇相關指標之建築工程)</option>
                    <option value="8">否(維護相關工程)</option>
                </select>
            </div>
        </div>
        <div v-if="tarRecord.ToDoChecklit > 2" class="form-group row">
            <label class="col-md-4 col-form-label">選否：上傳「上級機關文書」檔案</label>
            <div class="col-md-7">
                <b-form-file v-model="ChecklistFilename"
                    v-bind:placeholder="filePlaceholder(tarRecord.ChecklistFilename)"></b-form-file>
            </div>
            <div class="col-md-1">
                <button @click="download(tarRecord, 'ChecklistFilename')" v-if="!strEmpty(tarRecord.ChecklistFilename)"
                    role="button" class="btn btn-shadow btn-block btn-color11-1"><i class="fas fa-download"></i></button>
            </div>
        </div>
        <div v-if="tarRecord.ToDoChecklit < 3" class="form-group row">
            <label class="col-md-12 col-form-label">選是:上傳「生態檢核資訊公開內容」檔案</label>
            <Modal ref="modal" title="檔案清單">
                <template #body>
                    <table class="table">
                        <thead>
                            <tr>
                                <th>#</th>
                                <th scope="col">檔名</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(file,index) in modalList" :key="index">
                                <td>{{ index +1 }}</td>
                                <td > 
                                    {{ file.name }}
                                </td>

                            </tr>
                        </tbody>
                    </table>
                </template>
            </Modal>
            <div class="col-md-12">
                <div class="table-responsive">
                    <table class="table table-responsive-md table-hover">
                        <thead>
                            <tr class="insearch">
                                <th class="text-left"><strong>資料名稱</strong></th>
                                <th><strong>上傳檔案</strong></th>
                                <th style="width:40px"><strong>下載</strong></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td class="text-left"><strong>主表-公共工程生態檢核自評表</strong></td>
                                <td><b-form-file multiple v-bind:placeholder="filePlaceholder(tarRecord.SelfEvalFilename)"
                                        @change="store.onFileChange($event, 'SelfEvalFilename')"></b-form-file></td>
                                <!-- <td><button @click="download(tarRecord,'SelfEvalFilename')" v-if="!strEmpty(tarRecord.SelfEvalFilename)" role="button" class="btn btn-shadow btn-block btn-color11-1"><i class="fas fa-download"></i></button></td> -->
                                <td>
                                    <button class="btn btn-shadow btn-block btn-color11-1" @click="openDownloadList()"> <i
                                            class="fas fa-download"></i></button> 
                                    <button class="btn btn-shadow btn-block btn-color11-3" @click="showModal('SelfEvalFilename')"> <i
                                            class="fas fa-eye"></i></button> 
                                </td>
                            </tr>
                            <tr>
                                <td class="text-left"><strong>附表D-01 工程生態背景資料表</strong></td>
                                <td><b-form-file
                                        v-bind:placeholder="filePlaceholder(tarRecord.DataCollectDocFilename)"></b-form-file>
                                </td>
                                <td><button @click="download(tarRecord, 'DataCollectDocFilename')"
                                        v-if="!strEmpty(tarRecord.DataCollectDocFilename)" role="button"
                                        class="btn btn-shadow btn-block btn-color11-1"><i
                                            class="fas fa-download"></i></button></td>
                            </tr>
                            <tr>
                                <td class="text-left"><strong>附表D-02 現場勘查/會議紀錄表</strong></td>
                                <td><b-form-file v-bind:placeholder="filePlaceholder(tarRecord.MemberDocFilename)"
                                        @change="store.onFileChange($event, 'MemberDocFilename')"></b-form-file></td>
                                <td><button @click="download(tarRecord, 'MemberDocFilename')"
                                        v-if="!strEmpty(tarRecord.MemberDocFilename)" role="button"
                                        class="btn btn-shadow btn-block btn-color11-1"><i
                                            class="fas fa-download"></i></button></td>
                            </tr>
                            <tr>
                                <td class="text-left"><strong>附表D-03 生態調查評析表</strong></td>
                                <td><b-form-file v-bind:placeholder="filePlaceholder(tarRecord.SOCFilename)"></b-form-file>
                                </td>
                                <td><button @click="download(tarRecord, 'SOCFilename')"
                                        v-if="!strEmpty(tarRecord.SOCFilename)" role="button"
                                        class="btn btn-shadow btn-block btn-color11-1"><i
                                            class="fas fa-download"></i></button></td>
                            </tr>
                            <tr>
                                <td class="text-left"><strong>附表 D-04 民眾參與紀錄表</strong></td>
                                <td><b-form-file
                                        v-bind:placeholder="filePlaceholder(tarRecord.PlanDesignRecordFilename)"></b-form-file>
                                </td>
                                <td><button @click="download(tarRecord, 'PlanDesignRecordFilename')"
                                        v-if="!strEmpty(tarRecord.PlanDesignRecordFilename)" role="button"
                                        class="btn btn-shadow btn-block btn-color11-1"><i
                                            class="fas fa-download"></i></button></td>
                            </tr>


                            <tr>
                                <td class="text-left"><strong>附表D-05 生態保育措施研擬紀錄表</strong></td>
                                <td><b-form-file
                                        v-bind:placeholder="filePlaceholder(tarRecord.ConservMeasFilename)"></b-form-file>
                                </td>
                                <td><button @click="download(tarRecord, 'ConservMeasFilename')"
                                        v-if="!strEmpty(tarRecord.ConservMeasFilename)" role="button"
                                        class="btn btn-shadow btn-block btn-color11-1"><i
                                            class="fas fa-download"></i></button></td>
                            </tr>
                            <tr>
                                <td class="text-left"><strong>附表D-06 工程範圍生物名錄</strong></td>
                                <td><b-form-file
                                        v-bind:placeholder="filePlaceholder(tarRecord.EngCreatureNameList)"></b-form-file>
                                </td>
                                <td><button @click="download(tarRecord, 'EngCreatureNameList')"
                                        v-if="!strEmpty(tarRecord.EngCreatureNameList)" role="button"
                                        class="btn btn-shadow btn-block btn-color11-1"><i
                                            class="fas fa-download"></i></button></td>
                            </tr>

                    </tbody>
                </table>
            </div>
        </div>
    </div>
    <div class="card-footer">
        <div class="row justify-content-center">
            <!-- div class="col-12 col-sm-4 col-xl-2 my-2">
                    <button role="button" class="btn btn-shadow btn-color3 btn-block"> 取消修改 </button>
                </div -->
            <div class="col-12 col-sm-4 col-xl-2 my-2">
                <button @click="store.uploadFilies()" role="button" class="btn btn-shadow btn-block btn-color11-2"> 儲存 <i
                        class="fas fa-save"></i></button>
            </div>
        </div>
    </div>
    <!-- <downloadList route="EQMEcologicalCheck" :seq="targetId" ref="downloadList"> </downloadList> -->
</div></template>
