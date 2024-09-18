<script setup>
import { onMounted, ref, watch, computed} from "vue";
import paginationIndex from "../../components/paginationV2.vue";
import {useSelectionStore} from "../../store/SelectionStore.js";
import usePageItem from "../../store/PageItemStore.js";
import Modal from "../../components/Modal.vue";
const pagination = usePageItem("AlertSetting/GetList", 10, 5);

const modal = ref(null);
const modal2 = ref(null);
const systemList  =  ref([]);
const currentSystemValue  =  ref( parseInt(window.sessionStorage.getItem("AlertSettingSystemType")) );
const currentMenuList  =  ref([]);
const currentMenuName  =  ref([]);
const editIndex = ref("");
const editArrIndex = ref(-1)
const menuStore = useSelectionStore("Menu/GetList");
const lookingItem =ref({});

const host = computed( () => window.location.origin);
onMounted(async () => {
    systemList.value =  await (useSelectionStore("Menu/GetSystemTypeList").Get());
})

watch(currentSystemValue, async (nvalue) => {
    window.sessionStorage.setItem("AlertSettingSystemType", nvalue);
    if(!isNaN(currentSystemValue.value))
    {
        pagination.setParams({
            systemSeq : nvalue
        }).GetPageItem(1, 10);
        menuStore.setParameter({
            systemTypeSeq : nvalue
        });

        currentMenuList.value = (await menuStore.GetWithHandler((resp) => resp.l.map(e => {return {Value : e.Seq, Text : e.Name } })));
    }



    console.log("currentMenuList", currentMenuList)
}, { immediate : true})
function openAlertMemoEdit(item)
{
    modal.value.show = true;
    lookingItem.value  = item;
}


watch(() => modal, (nvalue) => {

    if(!nvalue.value.show)
        editArrIndex.value = -1
})

// watch(() => modal2, (nvalue) => {
//     if(!nvalue.value.show)
//         editArrIndex.value = -1
// })
function addItem()
{
    editIndex.value = "";
    pagination.list.value.push({
        No : "",
        edit : true
    });
}

async function deleteItem(item)
{
    if(item.No!=""){
        let {data :res} = window.myAjax.post("AlertSetting/Delete", { No : item.No })
        if(res)
            pagination.GetPageItem();
    }
    item.No = -1;
    pagination.list.value
    = pagination.list.value.filter(e => e.No != -1);

}

async function  updateBackend(item, insert)
{

    item.Menu = null;
    let {data :res} = await window.myAjax.post("AlertSetting/UpdateOrCreate", { item : item, systemTypeSeq : currentSystemValue.value })
    if(res){
        if(item.MenuSeq)
            res.MenuName = menuStore.Map.value[item.MenuSeq];
         Object.assign(item, res);

         if(insert)
        {
            editItem(item, true)
        }
        else
        {
            editItem(item, false)
        }

        
    }
       
}

const list = computed(() => pagination.list.value);

const listFilted = computed (() => 
    list.value.filter(e => e.No.startsWith(currentSystemValue.value)  || e.No ==""

)    .reverse())

function editItem(item, edit)
{
    item.edit = edit;
    pagination.list.value =  pagination.list.value.map(e => e);
}

</script>


<template>

    <div>
        <Modal ref="modal" title="提示訊息">
            <template #body  >
                <textarea v-if="lookingItem" type="text" class="form-control" v-model="lookingItem.AlertMemo" >
                                    
                </textarea>
                <div class="text-center mt-4">
                    <button class="btn btn-color11-2 btn-xs m-1" rows="7" @click="modal.show = false">
                        儲存
                        <i class="fas fa-save">

                        </i>
                    </button>
                </div>
            </template>

        </Modal>
        <Modal ref="modal2" :title="`${lookingItem.No}_影片`" >
            <template #body  >
                <p style="color:black">
                    {{ lookingItem.AlertMemo }}
                </p>
                <iframe  v-if="modal2 && modal2.show"
                style="width:100%; height: 350px;"
                
                :src="lookingItem.VideoUrl"
                title="YouTube video player"
                frameborder="0"
                allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                allowfullscreen></iframe>

            </template>

        </Modal>
        <div class="row justify-content-between">
            <div class="col-12 col-md-3 mt-3">
                <select class="form-control" v-model="currentSystemValue">
                    <!-- <option selected="selected" :value="-1"> 全部</option> -->
                    <option v-for="(system, index) in  systemList" :key="index" :value="system.Value"> 
                        {{ system.Text }}
                    </option>

                </select></div>
            <div class="col-12 col-md-1  mt-3"><button role="button" @click="addItem()"
                    class="btn btn-outline-secondary btn-xs mx-1"><i class="fas fa-plus"></i>&nbsp;&nbsp;新增 </button>
            </div>
        </div>
        <div>

            <div class="table-responsive">
                <table border="0" class="table table1 min910" id="addnew2024032701">
                    <thead>
                        <tr>
                            <th class="number">編號</th>
                            <th style="width: 20%;">子系統</th>
                            <th>提示訊息</th>
                            <th>影片</th>
                            <th style="width: 10%;">建立日期</th>
                            <th style="width: 10%;">更新日期</th>
                            <th style="width: 10%;">功能</th>
                        </tr>
                    </thead>
                    <tbody >
                        <tr v-for="(item, index) in listFilted" :key="index">
                            <td v-if="item.No == ''">
                                <div style="color:green"> 新增... </div><!---->
                            </td>
                            <td v-else>
                                <div>{{ item.No }}</div><!---->
                            </td>
                            <td v-if="item.No != editIndex ">
                                    <div  class="d-flex row">
                                        <div class="col-6 text-left">
                                            {{ item.MenuName }}
                                        </div>
                
                       
                   
                                        <div v-if="item.Position != null" style="color:blue"  class="col-6 text-left">
                                            設定完成
                                        </div>
                                        <div v-else style="color:red"  class="col-6 text-left">
                                            未設定
                                        </div>
                                        <div class="col-12 text-center">                                            
                                            <a class="btn btn-color9-1 btn-xs m-1  " :href="`./AlertSetting/StartPosSetting?No=${item.No}`" >
                                                設定位置 <i class="fas fa-circle">

                                                </i>
                                            </a>

                                        </div>

                                    </div>

                                        <!-- <a class="btn btn-color9-1 btn-xs m-1" :href="`/AlertSetting/StartPosSetting?No=${item.No}`" target="_blank">
                                            設定位置 <i class="fas fa-circle">

                                            </i>
                                        </a> -->

                            </td>
                            <td v-else >
                                <div class="d-flex justify-content-center">
                                    <select class="form-control col-6"   v-model="item.MenuSeq"  :disabled="item.Position"> 
                                        <option v-for="(menu, index2) in currentMenuList" :key="index2" :value="menu.Value" > 
                                            {{ menu.Text}}
                                        </option>   
                                    </select>

                                </div><!---->
                            </td> 
                            <td v-if="item.edit" class="text-center">
                              
                                   <button @click="openAlertMemoEdit(item)" class="btn btn-color11-3 btn-xs m-1"><i
                                            class="fas fa-pencil-alt"></i> 編輯</button> 
                          
                            </td>
                            <td v-else class="text-center">
                                {{item.AlertMemo != null ? item.AlertMemo.slice(0, 20) : "" }} ...
                            </td>
                            <td v-if="item.edit && item.No != ''" >
                                <!-- <el-upload
                                accept="video/mp4"
                                class="upload-demo  text-center"
        
                                :action="`${host}/AlertSetting/UploadVideo?No=${item.No}`">
                                <el-button size="small" type="primary" class="btn btn-color11-3 btn-xs m-1">
                                    <i
                                            class="fas fa-upload"></i> 

                                </el-button>
                                </el-upload> -->
                                <div>
                                    <input  type="text" class="form-control" v-model="item.VideoUrl" >
                                </div>
                            </td>
                            <td v-else class="text-center">
                                <div v-if="item.VideoUrl">
                                    {{item.VideoUrl.slice(0, 30)}} ... 
                                </div>
                                <!-- <div  style="color:blue" v-if="item.VideoUrl">已上傳
                                   
                                </div>
                                <div style="color:red" v-else>未上傳</div> -->
                                <!-- <a :href="item.VideoUrl">
                                    {{item.VideoUrl != null ? item.VideoUrl.slice(0, 20) : "" }} ...
                                </a> -->
         
                            </td>
                            <td  class="text-center"> {{ item.CreateTime }}</td>
                            <td  class="text-center"> {{ item.ModifyTime }}</td>
                            <td class=" justify-content-center" >
                                <div class="row justify-content-center m-0">
                                    <button v-if="!item.edit"  class="btn btn-color11-2 btn-xs m-1" @click="() => {lookingItem = item;  modal2.show = true; }"><i class="fas fa-eye"></i> </button>
                                    <button v-if="!item.edit" @click="editItem(item, true)" class="btn btn-color11-3 btn-xs m-1"><i
                                            class="fas fa-pencil-alt"></i> 編輯</button>
                                    <button  v-else-if="item.No != ''"   @click="updateBackend(item)" class="btn btn-color11-2 btn-xs m-1"
                                        ><i class="fas fa-save"></i>
                                         儲存 </button>
                                    <button  v-else   @click="updateBackend(item, true)" class="btn btn-color11-2 btn-xs m-1"
                                    ><i class="fas fa-plus"></i>
                                    新增</button>
                                    <a @click="deleteItem(item)" href="#" title="刪除" class="btn btn-color9-1 btn-xs m-1"><i
                                            class="fas fa-trash-alt"></i> 刪除</a>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="row justify-content-center" style="width: 99%;">
                <paginationIndex 
                    v-if=" listFilted.length > 0"
                    :page="pagination.currentPage.value" 
                    :pageCount="pagination.pageCount.value" 
                    :pageSectionCount="5"
                    @pageChange="(p) => pagination.GetPageItem(p)"
                    >
                    
                </paginationIndex>
            </div>
        </div>
    </div>
</template>