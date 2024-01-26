<script setup>
 import { onMounted, ref, computed} from "vue";
 import {useImageLocalStore}  from "../ImageLocalStore.js";
 const store  = useImageLocalStore("TrainingImage/GetDirFileName", "Key", "Files");
 

 
 onMounted(() => {
    store.GetImage("") ;

 })
 const amountInPage = ref(15);
 const pageControlArr =  computed( () => Object.values(store.imageData.value).map((e,i) => {
    console.log(e);
    return {
        pageIndex : 0,
        Content : e.value.groupValueKey,
        ViewContent : ref(e.value.groupValueKey.slice(0, amountInPage.value))

    }
}) )

function handleCurrentChange(val, index)
{

    pageControlArr.value[index].ViewContent.value =  pageControlArr.value[index].Content.slice( (val-1)*amountInPage.value, (val)*amountInPage.value) ;
}
</script>

<template>
    <div class="card whiteBG mb-4 pattern-F ">

        <div class="card-header ">
            <h3 class="card-title font-weight-bold">訓練影像資料庫</h3>
        </div>
        <div class="card-body">
            <div class="mr-1 m-1 ">資料夾查詢</div>
            <div class="justify-content-start form-inline">

                <div  role="group" aria-label="Basic outlined example" >
                    <button type="button" class="btn btn-outline-danger m-1" v-if="store.dirStack.value.length > 1 "  @click="store.GetParentDirImage">上層目錄</button>

                    <button type="button" class="btn btn-outline-success m-1" :key="index" v-for="(dirName, index) in store.groupList.value" @click="store.GetImage(dirName)">{{ dirName }}</button>

                </div>


        </div>
        <div class="justify-content-start form-inline m-1 row ">
            <input type="text" class="form-control ml-1 col-10 col-sm-6 col-lg-6" placeholder="檔名查詢" value="" v-model="store.searchStr.value" />
                <button type="button" class="btn btn-outline-secondary m-1" @click="store.GetImageByKeyWord()"><i
                        class="fas fa-search"></i></button>
        </div>
        <div v-for="(path, index) in Object.values(store.imageData.value)" :key="index">
            <div v-if="path.value.groupValueKey.length > 0 ">
                    <h5> /{{ path.value.groupKey  }} </h5>
                    <div class="mb-3 d-flex justify-content-start" >
                        <el-pagination
                        background
                        layout="prev, pager, next"
                        @current-change="(val) => handleCurrentChange(val, index)"
                        :page-size="amountInPage"
                        :current-page.sync="pageControlArr[index].pageIndex"
                        :total="path.value.groupValueKey.length">
                        </el-pagination>
                        <span class="align-self-middle" style="color:red">
                            {{ pageControlArr[index].Content.length }}
                        </span>
                        張
                    </div>



                <div class="row pics m-1">
                    <div class=" mb-4 mr-3" v-for="(file, index2) in pageControlArr[index].ViewContent.value" :key="index2">
                        <div class="card" style="height:360px">
                            <ul class="list-group list-group-flush small">
                                <li class="list-group-item d-flex">
                                    <div>{{ file.fileName }}</div>
                                                                    <a href="javascript:void(0)" class="card-link a-view ml-auto" data-toggle="modal"
                                    data-target="#modal" title="檢視" @click="store.lookUpImg( { path :`${path.value.groupKey}`, name : file.fileName })"><i
                                        class="fas fa-eye"></i></a>
                                </li>
                                
                            </ul>
                            <img :src="'FileUploads/trainingImage/' + file.relativeUrl"  style="height: 315px; width:auto"/>



                        </div>
                    </div>
                </div>
                <div class="row pics m-1">
                    <div class="col-6">

                    </div>

                </div>

            </div>

        </div>

            <!-- 小視窗 看大圖 -->
            <div class="modal fade"  id="modal" >
                <div class="modal-dialog modal-xl modal-dialog-centered ">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h6 class="modal-title font-weight-bold">檢視資料 - {{ store.selectImg.value.name }}</h6>
                            <button type="button" id="close" class="close" data-dismiss="modal" aria-label="Close" >
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body text-center " style="height:500px;overflow: auto;">
                            <img :src="'FileUploads/trainingImage/' +store.selectedFileUrl.value"
                                        style="width:60%;max-height: 400px;" />
                        </div>
                    </div>
                </div>
            </div>
    </div>
</div>

</template>
<style >

  .el-pager li  {
    background-color: #EEEEEE !important; 
    width: 8%;
  }
  .el-pager li.active
  {
    background-color:  #BDBDBD !important;
  }
</style>