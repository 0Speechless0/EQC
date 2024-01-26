<template>
    <div class="modal" :id="`toolPackageModal${props.engSeq}`" style="display: none;"
            aria-hidden="true">
            <div class="modal-dialog modal-xl modal-dialog-centered "
            style="max-width: fit-content;">
            <div class="modal-content">
            <div class="card whiteBG mb-4 pattern-F colorset_1">
                <div class="tab-content">
                    <div class="tab-pane active">
                        <h5>設定工具包</h5>
                        <p  class="" style="color: red;">格式說明： .jpg、.jpeg、.png、.pdf</p>

                        <div class="table-responsive">
                            <table border="0" class="table table1 min910" id="addnew061401">
                                <thead class="insearch">
                                    <tr>
                                        <th class="sort">排序</th>
                                        <th>說明</th>
                                        <!-- <th>階段</th> -->
                                        <th class="text-center">上傳</th>
                                        <th>功能</th>
                                        <th style="width: 50px;">示意圖</th>
                                        <th>
                                            <div class="d-flex justify-content-center">
                                                <!-- <button title="編輯" class="btn btn-color11-1 btn-xs sharp m-1">
                                                    <i class="fas fa-pencil-alt"></i>
                                                </button>
                                                <button title="刪除" class="btn btn-color9-1 btn-xs sharp m-1">
                                                    <i class="fas fa-trash-alt"></i>
                                                </button> -->
                                                <button title="新增" class="btn btn-outline-secondary btn-xs sharp m-1" @click="addToolPackage">
                                                    <i class="fas fa-plus"></i>
                                                </button>
                                            </div>
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(item, index) in items" :key="index">
                                        <td>{{index+1}}</td>
                                        <td v-if="editIndex == index || !item.FileName"><input type="text" class="form-control" v-model="item.Description"></td>
                                        <td v-else >{{ item.Description }}</td>
                                        
                                        <!-- <td v-if="editIndex == index || !item.FileName" >
                                            <div class="form-check">
                                                <input v-model="item.Stage" class="form-check-input" type="radio" :value="0" :name="`flexCheckChecked${index+1}`" :id="`flexCheckChecked1${index+1}`" checked>
                                                <label class="form-check-label" :for="`flexCheckChecked1${index+1}`">
                                                    施工前
                                                </label>
                                            </div>
                                            <div class="form-check">
                                                <input v-model="item.Stage" class="form-check-input" type="radio" :value="1" :name="`flexCheckChecked${index+1}`" :id="`flexCheckChecked2${index+1}`" >
                                                <label class="form-check-label" :for="`flexCheckChecked2${index+1}`">
                                                    施工中
                                                </label>
                                            </div>
                                            <div class="form-check">
                                                <input v-model="item.Stage" class="form-check-input" type="radio" :value="2" :name="`flexCheckChecked${index+1}`" :id="`flexCheckChecked3${index+1}`" >
                                                <label class="form-check-label" :for="`flexCheckChecked3${index+1}`">
                                                    施工後
                                                </label>
                                            </div>
                                        </td>
                                        <td v-else>
                                            {{ computeStageText(item.Stage) }}
                                        </td> -->
                                        <td v-if="!item.FileName">
                                            <div class="custom-file b-form-file" id="__BVID__16__BV_file_outer_">
                                                <input type="file" class="custom-file-input" id="__BVID__16" style="z-index: -5;" @change="FileChange($event, item)">
                                                <label data-browse="上傳" class="custom-file-label" for="__BVID__16" style="justify-content: flex-start;">
                                                    <span class="d-block form-file-text" style="pointer-events: none;">未選擇任何檔案</span>
                                                </label>
                                            </div>
                                        </td>
                                        <td v-else>
                                            <p style="color:blue">上傳成功</p>
                                        </td>
                                        <td v-if="item.FileName">
                                            <div class="d-flex justify-content-center">
                                                    <button v-if="editIndex != index"  title="編輯" class="btn btn-color11-3 btn-xs sharp m-1" @click="editToolPackage(index)">
                                                        <i class="fas fa-pencil-alt"></i>
                                                    </button>
                                                    <button  v-else @click="saveToolPackage(item)" id="editButton" class="btn btn-color11-2 btn-xs sharp m-1" >
                                                        <i class="fas fa-save"></i> 
                                                    </button>
 
                                                <button  @click="deleteToolPackage(item.FileName)" title="刪除" class="btn btn-color9-1 btn-xs sharp m-1">
                                                    <i class="fas fa-trash-alt"></i>
                                                </button>
                                            </div>
                                        </td>
                                        <td v-else>

                                        </td>
                                        <td v-if="item.FileName && item.FileName.indexOf('.pdf') == -1" >
                                            <img :src="`../FileUploads/ToolPackage/${item.EngSeq}/${item.FileName}`" width="50" height="50">
                                        </td>
                                        <td v-else>
                                            <i class="fa fa-file" aria-hidden="true"></i>
                                        </td>
                                        <td></td>
                                    </tr>
                                
                                </tbody>
                            </table>
                            
                        </div>

                    </div>
                </div>
            </div>
            </div>
        </div>
    </div>
</template>
<script setup>
    import { onMounted,ref, computed, defineProps } from 'vue';
    const items = ref([]); 
    const props = defineProps(['engSeq']);
    const editIndex = ref(null);

    onMounted(async () =>{
        await getToolPackage();
    })

    const getToolPackage = async () => 
    {
        const {data} = await window.myAjax.post("ToolPackage/GetList", { engSeq : props.engSeq } );
        items.value = data;
    } 

    const editToolPackage = (index) =>
    {
        editIndex.value = index;
    }

    const saveToolPackage = async (m) =>
    {
        const {data} = await window.myAjax.post("ToolPackage/Update", { m : m} );
        if(data == true) 
        {
            alert("儲存成功");
            editIndex.value = null;
        }

    }

    const deleteToolPackage = async (fileName) => 
    {
        const {data} = await window.myAjax.post("ToolPackage/Delete", {fileName : fileName, engSeq : props.engSeq } );
        if(data == true) 
        {
            alert("刪除成功");
            getToolPackage();
        }
    }
    const addToolPackage = async () =>
    {
        items.value.push({
            EngSeq : props.engSeq,
            Description :"",
            Stage : 0,
        })
    } 
    const computeStageText = (num) => {
        switch(num)
        {
            case 2 : return "施工後";
            case 1 : return "施工中";
            case 0 : return "施工前";
            default : return "";
        }
    }
    const FileChange = async (event, m) => 
    {
        var formData = new FormData();
        var file = event.target.files[0];
        console.log(file);
        formData.append("m", JSON.stringify(m) );
        formData.append("file", file, file.name);
        if(file.type != "application/pdf" && file.type != "image/jpeg" && file.type != "image/png") 
        {
            alert("格式錯誤，請上傳正確格式");
            return ;
        }
        const {data} = await window.myAjax.post("ToolPackage/UploadFile", 
            formData,                     
            {
                headers: { 'Content-Type': 'multipart/form-data' }
        });
        if(data == true)
        {
            alert("上傳成功");
            await getToolPackage();
        }
    }
    const filePath = computed(() => window.location.host +"/FileUploads/ToolPackage" );

</script>
<style scoped>
    td {
        text-align: center !important;
    }
</style>