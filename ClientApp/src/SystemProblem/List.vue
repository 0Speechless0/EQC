<script setup>
import pagination  from "../components/paginationV2.vue";
import usePageItem from "../store/PageItemStore";
import Edit from "./Edit.vue";
import Com from "../Common/Common2";
import { onMounted,ref, defineProps, watch } from "vue";
const processingPageItemStore = usePageItem("SystemProblem/GetProcessingList", 10);
const donePageItemStore = usePageItem("SystemProblem/GetDoneList", 10);

const linkItem = ref(null);
const props = defineProps(["admin"]);

const admin =  ref(props.admin ?? window.location.pathname.includes("Admin") ? true : false ) ;
onMounted(async() => {
    getList();

})

async function getList()
{
    processingPageItemStore
    .setParams({type : admin.value ? 1 : 0}).GetPageItem(1);
    donePageItemStore
    .setParams({type : admin.value ? 1 : 0}).GetPageItem(1);
}
async function AdminCheck(id)
{
    let res = (await window.myAjax.post("SystemProblem/AdminCheck",{ id: id})).data;
    if(res == true)
    {
        getList();  
    } 

}
watch(linkItem, (v) => {
    getList();
})

</script>
<template>
    <div v-if="linkItem">
        <Edit :admin="true" :targetItemSeq="linkItem.Seq" @oncancel="linkItem = null"></Edit>
    </div>
    <div v-else>
        <!-- <div style="text-align: end;">
            <button role="button" class="btn btn-color11-3 btn-xs mx-1" id="updateStatusBtn">更新狀態</button>
        </div> -->
        <div>
            <!-- <h5 class="insearch mt-0 py-2">
                                            工程編號：A109007(109-B-01010-001-000)<br>工程名稱：水利圖資與雲端運用中心辦公廳舍整修工程(第1期)(水利工資工率參考案件)
                                        </h5> -->
            <h5>待處理</h5>
            <div class="table-responsive" id="open0" style="">
                <table class="table table-responsive-md table-hover table-responsive" id="pendingF1">
                    <thead>
                        <tr class="insearch">
                            <th style="width: 5%;"><strong>排序</strong></th>
                            <th style="width: 7%;"><strong>建立時間</strong></th>
                            <th style="width: 7%;"><strong>問題類別</strong></th>
                            <th><strong>問題標題</strong></th>
                            <th><strong>問題說明</strong></th>
                            <th v-if="admin"><strong>提問人</strong></th>
                                <th v-if="admin"><strong>電話</strong></th>
                            <th style="width: 5%;" v-if="!admin"> <strong>附件</strong></th>
                            <th style="width: 5%;" v-else><strong>問題連結</strong></th>
                            <th class="text-center" style="width: 7%;"><strong>狀態</strong>
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in processingPageItemStore.list.value" :key="index">
                            <td><strong>{{ index +1 }}</strong></td>
                            <td>{{ item.CreateTIme }}</td>
                            <td>{{ item.SystemProblemType == null ? "" : item.SystemProblemType.Name }}</td>
                            <td>{{ item.Title }}</td>
                            <td>{{ item.Descriptoin }}</td>
                            <td v-if="admin">{{ item.UserMain != null ? item.UserMain.DisplayName : '' }}</td>
                            <td v-if="admin">{{ item.UserMain != null ? item.UserMain.Tel ??  item.UserMain.Mobile  : ''}} <span v-if="item.UserMain &&  item.UserMain.TelExt">分機:{{ item.UserMain.TelExt }}</span></td>
                            <td v-if="!admin">
                                <a  v-if="item.uploadFilesName != null" class="btn btn-block btn-color11-1 btn-sm" :href="`SystemProblem/Download?id=${item.Seq}`"  download><i
                                        class="fas fa-download"></i> 下載</a>
                                <span v-else style="color:red"> 無</span>
    
                            </td>
                            <td v-else>
                                <a  @click="linkItem = item" type="button" class="btn btn-xs mt-1" style="background-color: rgb(172, 0, 0);color: rgb(255, 255, 255);"><i class="fas fa-location-arrow"></i> 連結</a>
                            </td>
                            <td style="text-align: center;">
                                <div class="status-label" style="color: red;" v-if="!admin">待處理</div>
                                <button  v-else-if="item.Anwser" class=" form-control confirm-btn" style=" background-color:rgb(0, 170, 23);color: rgb(255, 255, 255)" @click="AdminCheck(item.Seq)">已完成</button>
                            </td>
                        </tr>

                    </tbody>
                </table>
                <pagination v-if="processingPageItemStore.list.value.length > 0 "
                    :page="processingPageItemStore.currentPage.value" 
                    :pageCount="processingPageItemStore.pageCount.value"
                    :pageSectionCount="5"
                    @pageChange="(e) => processingPageItemStore.GetPageItem(e) "
                ></pagination>
            </div>
            <div>
                <h5>已處理</h5>
                <div class="table-responsive" id="open1" style="display: block;">
                    <table class="table table-responsive-md table-hover table-responsive" id="pendingF2">
                        <thead>
                            <tr class="insearch">
                                <th style="width: 5%;"><strong>排序</strong></th>
                                <th style="width: 7%;"><strong>完成時間</strong></th>
                                <!-- <th style="width: 5%;"><strong>是否緊急</strong></th> -->
                                <th style="width: 7%;"><strong>問題類別</strong></th>
                                <th><strong>問題標題</strong></th>
                                <th><strong>問題說明</strong></th>
                                <th v-if="admin"><strong>提問人</strong></th>
                                <th v-if="admin"><strong>電話</strong></th>
                                <th style="width: 5%;"><strong>詳細資料</strong></th>
                                <th class="text-center" style="width: 7%;">
                                    <strong>狀態</strong>
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                        <tr v-for="(item, index) in donePageItemStore.list.value" :key="index">
                            <td><strong>{{ index +1 }}</strong></td>
                            <td>{{ item.CreateTIme }}</td>
                            <td>{{ item.SystemProblemType == null ? "" : item.SystemProblemType.Name }}</td>
                            <td>{{ item.Title }}</td>
                            <td>{{ item.Descriptoin }}</td>
                            <td v-if="admin">{{ item.UserMain != null ? item.UserMain.DisplayName: '' }}</td>
                            <td v-if="admin">{{ item.UserMain != null ? item.UserMain.Tel ??  item.UserMain.Mobile : '' }} <span v-if="item.UserMain && item.UserMain.TelExt">分機:{{ item.UserMain.TelExt }}</span></td>
                            <td>
                                    <button title="查看" style="text-align: center;" data-toggle="modal"
                                        :data-target="`#window${index}`" class="btn btn-color11-3 btn-xs mx-1 mt-1"> <i
                                            class="fas fa-eye"></i> 查看 </button>
                                    <div class="modal" :id="`window${index}`" style="display: none;" aria-hidden="true">
                                        <div class="modal-dialog modal-xl modal-dialog-centered "
                                            style="max-width: fit-content;">
                                            <div class="modal-content">
                                                <div class="card whiteBG mb-4 pattern-F colorset_1">
                                                    <div class="tab-content">
                                                        <div class="tab-pane active">
                                                            <h5>
                                                                問題回復
                                                            </h5>
                                                            <label>{{ item.Anwser }}</label>
                                                        </div>
                                                    </div>
                                                    <div class="tab-content" style="padding-top: 10px;text-align: end;">
                                                        <button title="確認" class="btn btn-color11-3 btn-xs mx-1 ml-auto"
                                                            data-dismiss="modal">
                                                            <i class="fas fa-check"></i>
                                                            確認
                                                        </button>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </td>
                                <td style="text-align: center;">
                                    <div class="status-label" style="color: rgb(0, 170, 23);">已完成
                                    </div>
                                </td>
                        </tr>

                    </tbody>
                    </table>
                </div>
                <pagination
                    :page="donePageItemStore.currentPage.value" 
                    :pageCount="donePageItemStore.pageCount.value"
                    :pageSectionCount="5"
                    @pageChange="(e) => donePageItemStore.GetPageItem(e) "
                ></pagination>
            </div>

        </div>
    </div>
</template>