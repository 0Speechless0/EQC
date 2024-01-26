<template>
    <div>
        <h1>Test</h1>
        <div class="ml-3">
            此頁用於說明組件及展示功能(前端)
        </div>
        <!-- <div :class="`col-12 mt-3`"  v-for="(selections, levelNum) in store.LevelsOfSelection.value" :key="levelNum">

            <select    class="form-control" @change=" store.GetOneLevelSelection($event.target.value, levelNum)" >
                <option  :value="null" selected  > 全部單位 </option>
                <option v-for="(option, index) in selections" :value="option.Value" :key="index" >{{ option.Text }}</option>
            </select>
        </div> -->
        <div>
            <h2>標式檔案管理視窗</h2>
            <div class="ml-3">
                <Modal title="碳報表檔案上傳" RemoteDir="CarbonReportFile" Route="TempFile" ref="modal">
                    <template #content>

                        <h1> sdfsdfsdf</h1>
                    </template>

                </Modal>
                <button @click="() => { modal.modalShow = true }"> 點我</button>
            </div>

            <h2>標式資料表</h2>
            <div class="ml-3">
                <Table minWidth="1800" :items="[
                {
                    id: 1,
                    title: 'A',
                    content: 'aaaaa',
                    createTime: new Date().toISOString()

                },
                {
                    id: 2,
                    title: 'B',
                    content: 'bbbbb',
                    createTime: new Date().toISOString()
                },
                {
                    id: 3,
                    title: 'C',
                    content: 'ccccc',
                    createTime: new Date().toISOString()
                }]" 
                :columnInfo="[
                    ['標題', 'title', '250'],
                    ['內容', 'content', '350'],
                    ['建立時間', 'createTime', '150']
                ]">

            </Table>
            </div>


        </div>
    </div>
</template>

<script setup>
import { onMounted, ref } from "vue";
import Modal from "./components/FormalUploadModal.vue";

import Table from "./components/FormalTable.vue";
import { useLevelSelectionStore } from "./store/LevelSelectionStore";
// const store = useLevelSelectionStore("Unit/GetUnitList", "parentSeq");
const modal = ref(null);

</script>

