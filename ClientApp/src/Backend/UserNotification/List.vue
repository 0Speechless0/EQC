<script setup>
import pageNation from "../../components/paginationV2.vue";
import { useUserNotificationStore } from "../store/UserNotificationStore";
import  pttInsideModal  from "../../components/pttInsideModal.vue";
import { onMounted } from "vue";
import Common from "../../Common/Common2.js";
import ClassicEditor from '@ckeditor/ckeditor5-build-classic';

const store = useUserNotificationStore();
onMounted( async() => await store.pageItem.GetPageItem() )

function downloadJson(notification)
{
    Common.download2(JSON.stringify(notification), `${notification.Title}.json`, "application/json" );
}
function readJson(e)
{
    console.log('dd');
    function uploadJson(e)
    {
        
        var  content = JSON.parse(e); 
        store.isEditJsonImport.value = true;
        store.lookForNotification(content);
        console.log("content", content);
    }

    try{
        Common.readFiles(e, uploadJson)
        .then(() =>  e.target.value = "");
    }   
    catch(e)
    {
        alert("上傳失敗")
    }

}
function submit()
{
    store.detailModalShow.value = false;
    store.submit(store.viewNotification.value)
}

</script>
<template>
    <div>
        <div class="d-flex justify-content-end">
            <p style="color: blue">發送通知(json)</p>
            <input type="file" id="upload" hidden @change="(e) => readJson(e)"/>
            <label title="上傳" for="upload" type="file"
                class="btn btn-color12 btn-xs sharp m-1" >
                    <i class="fas fa-upload"></i>
                </label>
        </div>

        <div class="row">
            <table class="table" v-if="store.pageItem.list.value.length > 0">
                <thead>
                    <tr>
                        <th class="col-4 col-sm-6 text-center">
                            系統公告內容
                        </th>
                        <th class="col-2 col-sm-1 text-center">
                            已讀數
                        </th>
                        <th class="col-3 col-sm-2 text-center">
                            發布日期
                        </th>
                        <th class="col-3 col-sm-2 text-center">
                            發布期限
                        </th>
                        <th class="col-3 col-sm-3 text-center">
                            管理
                        </th>
                    </tr>

                </thead>
                <tbody>
                    <tr v-for="(notification, index) in store.pageItem.list.value" :key="index">
                        <td>
                            {{ notification.Title }}
                        </td>
                        <td class="text-center">
                            {{ notification.ReadedCount }}
                        </td>
                        <td class="text-center">
                            {{ notification.CreateTime }}
                        </td>
                        <td class="text-center">
                            {{ notification.ExpireTime }}
                        </td>
                        <td>
                            <div class="d-flex justify-content-center">
                                <button title="檢視"  @click="store.lookForNotification(notification)"
                                    class="btn btn-color11-2 btn-xs sharp m-1"><i
                                        class="fas fa-eye"></i>
                                </button>
                                <button title="下載"
                                    class="btn btn-color11-1 btn-xs sharp m-1" @click="downloadJson(notification)"><i
                                        class="fas fa-download"></i>
                                </button>
                                <button title="刪除" @click="store.deleteNotification(notification.Seq)"
                                    class="btn btn-color9-1 btn-xs sharp m-1"><i class="fas fa-trash-alt"></i>
                                </button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="row mt-3">
            <pageNation 
                :page="store.pageItem.currentPage.value" 
                :pageCount="store.pageItem.pageCount.value" 
                :page-section-count="5"
                @pageChange="(page)=> store.pageItem.GetPageItem(page)"
                > </pageNation>
        </div>
            <pttInsideModal 
                @onCancel="() => store.detailModalShow.value = false"
                :viewNotification="store.viewNotification.value"  
                :detailModalShow="store.detailModalShow.value" 
                :ClassicEditor="ClassicEditor"
                :isEdit="store.isEditJsonImport.value"
            >

            <template  #footer>
                <el-button v-if="store.isEditJsonImport.value" type="primary" @click="submit">發送 </el-button>
            </template>

            </pttInsideModal>
        <!-- <el-dialog :title="store.viewNotification.value.Title" :visible.sync="store.detailModalShow.value" width="80%">
            <span class="card pl-1" v-html="store.viewNotification.value.EmitContent">

            </span>
            <span slot="footer" class="dialog-footer">
                <el-button @click="showMainModal()">返回</el-button>
            </span>
        </el-dialog> -->
</div>

</template>