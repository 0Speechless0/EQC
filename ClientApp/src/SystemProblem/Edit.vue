<script setup>
import { reactive, onMounted, defineProps, defineEmits} from "vue";

const props = defineProps(["admin", 'targetItemSeq'] );

const emit = defineEmits(["oncancel", "onsave"]);
const editTarget = reactive({
    targetItem: {
        ProblemTypeSeq : null,
        uploadFiles : []
    },
    engMain: null,
    userMain: {},
    systemProblemOption : [],
    MenuName : "",
    lastUrl : ""
});
onMounted(async () => {
    let engId = window.sessionStorage.getItem("EPC_SelectTrenderSeq") ?? 0;

    if(!props.admin)
    {
        editTarget.lastUrl = new URL(window.sessionStorage.getItem("funcOriginLocation") ).pathname;
        editTarget.userMain = (await window.myAjax.post("Users/GetUserInfo")).data.userInfo;
        if(engId != 0)
            editTarget.engMain = (await window.myAjax.post("TenderPlan/GetEngItem", { id: engId })).data.item;
        editTarget.targetItem.MenuSeq = (await window.myAjax.post("Menu/GetMenuSeqByUrl", { url : editTarget.lastUrl})).data ?? 0;
        console.log("editTarget.targetItem.MenuSeq", editTarget.targetItem.MenuSeq);
        editTarget.MenuName = (await window.myAjax.post("Menu/GetMenuName", { seq :  editTarget.targetItem.MenuSeq  })).data;
    }
    else if(props.targetItemSeq)
    {
        editTarget.targetItem = (await window.myAjax.post("SystemProblem/GetOneProblem", { Seq : props.targetItemSeq})).data;
        editTarget.engMain = editTarget.targetItem.EngMain;
        editTarget.userMain = (await window.myAjax.post("Users/GetUserInfo", {Seq : editTarget.targetItem.UserMainSeq})).data.userInfo;
        if(editTarget.targetItem.Menu != null)
        {
            editTarget.MenuName = editTarget.targetItem.Menu.Name;
            editTarget.lastUrl = "/"  + editTarget.targetItem.Menu.PathName;
        }        

    }

    editTarget.systemProblemOption = (await window.myAjax.post("SystemProblem/GetTypeOtion")).data;

});

async function adminSave()
{
    let res = (await window.myAjax.post("SystemProblem/AdimResponse", {

        systemProblem : editTarget.targetItem

        }) ).data
    if(res == true)
    {
        alert("回覆成功");
        emit("oncancel")
    }
}

async function save()
{
    if(editTarget.engMain != null)
        editTarget.targetItem.EngSeq = editTarget.engMain.Seq;
    editTarget.targetItem.UserMainSeq = editTarget.userMain.Seq;

    var form = new FormData();
    var fileTooBig =  [] ;
    if(editTarget.targetItem.Title == null || editTarget.targetItem.Descriptoin == null)
    {
        alert("標題和說明必填");
        return ;
    }
    editTarget.targetItem.uploadFiles.forEach( file => {
        if(file.size > 5242880)
        {
        
            fileTooBig.push(file);
            return;
        }
        form.append("files", file);
    })
    if(fileTooBig.length > 0)
    {
        alert("部分檔案超過限制無法上傳:" + fileTooBig.reduce((a, c) => a + c.name + ",", "") );
        return ;
    }


    let insertSeq = (await window.myAjax.post("SystemProblem/UserSave", {

        systemProblem : editTarget.targetItem,
        userMain : editTarget.userMain

    }) ).data

    if( isNaN(insertSeq) ) return ;

    form.append("id", insertSeq);


    let res = (await window.myAjax.post("SystemProblem/UploadUserFile", form, {
        headers: { 'Content-Type': 'multipart/form-data' }
    }) ).data
    if(res != true) alert("檔案上傳失敗")

    alert("回報成功")

}

</script>

<template>
    <div>
        <table class="table table-responsive-md table2 VA-middle">

                <tbody >
                    <tr>
                        <th class="col-2">問題類別<span class="small-red">&nbsp;*</span></th>
                        <td class="col-12" style="display: flex;">
                            <select class="form-control col-4" id="mySelect" v-model="editTarget.targetItem.ProblemTypeSeq" :disabled="props.admin">
                                <option :value="null" >選擇</option>
                                <option v-for="(option, index) in editTarget.systemProblemOption" :value="option.Value" :key="index">{{ option.Text }}</option>

                            </select>
                            <div class="col-8">
                                <label id="select01"
                                    style="display: none; padding-top: 5px;">&nbsp;※工程資料刪除、工程進度刪除、角色權限變更、解除監造計畫鎖定</label>
                                <label id="select02" style="display: none; padding-top: 5px;">&nbsp;※請詳細說明</label>
                                <label id="select03"
                                    style="display: none; padding-top: 5px;">&nbsp;※請詳細說明異常問題，並附上檔案或圖片</label>
                                <label id="select04" style="display: none; padding-top: 5px;">&nbsp;※歡迎提出您的建議</label>
                            </div>
                        </td>
                    </tr>
                    <tr v-if="editTarget.engMain">
                        <th>工程編號</th>
                        <td><input type="text" class="form-control" :value="editTarget.engMain.EngNo" disabled=""></td>
                    </tr>
                    <tr v-if="editTarget.engMain">
                        <th>工程名稱</th>
                        <td><input type="text" class="form-control" :value="editTarget.engMain.EngName" disabled=""></td>
                    </tr>
                    <tr >
                        <th>功能網址</th>
                        <td><input type="text" class="form-control" v-model="editTarget.lastUrl" disabled=""></td>
                    </tr>
                    <tr >
                        <th>功能項目</th>
                        <td><input type="text" class="form-control" v-model="editTarget.MenuName" disabled="">
                        </td>
                    </tr>
                    <tr>
                        <th>問題標題<span class="small-red">&nbsp;*</span></th>
                        <td><input type="text" class="form-control" v-model="editTarget.targetItem.Title" :disabled="props.admin"></td>
                    </tr>
                    <tr>
                        <th>問題說明<span class="small-red">&nbsp;*</span></th>
                        <td>
                            <textarea    rows="5" class="form-control" v-model="editTarget.targetItem.Descriptoin" :disabled="props.admin">請問...</textarea>
                        </td>
                    </tr>
                    <tr>
                        <th>上傳附件</th>
                        <td v-if="!props.admin">

                            <b-form-file multiple v-model="editTarget.targetItem.uploadFiles"  :disabled="props.admin"
                            :placeholder=" editTarget.targetItem.uploadFilesName  ?? '未選擇任何檔案' "></b-form-file>
                            <p class="mb-1 mx-2 small-red" >*附件上傳大小限制為5MB
                            </p>
  
                        </td>
                        <td v-else  >
                            <div class="row ">
                                <div class="col-10 text-center">
                                {{ editTarget.targetItem.uploadFilesName  }}
                            </div>
                            <div class="col-1 text-right" v-if="editTarget.targetItem.uploadFilesName " >
                                <a  class="btn btn-block btn-color11-1 btn-sm" :href="`../SystemProblem/Download?id=${editTarget.targetItem.Seq}`"  download><i
                                        class="fas fa-download"></i> 下載</a>
                            </div>
                            <div class="col-1 text-right" v-else-if="props.admin" >
                                    <span style="color:red" class="text-center">無 </span>
                            </div>
                            </div>


                        </td>
                    </tr>

                </tbody>

        </table>
        <h5>申請人資料</h5>
        <div class="table-responsive">
            <table class="table table-responsive-md table2 VA-middle">
                <tbody>
                    <tr>
                        <th>機關<span class="small-red">&nbsp;*</span></th>
                        <td><input type="text" class="form-control" :value="editTarget.userMain.UnitName1" disabled="">
                        </td>
                        <th>單位<span class="small-red">&nbsp;*</span></th>
                        <td><input type="text" class="form-control" :value="editTarget.userMain.UnitName2" disabled="">
                        </td>
                        <th>帳號<span class="small-red">&nbsp;*</span></th>
                        <td><input type="text" class="form-control" :value="editTarget.userMain.UserNo" disabled="">
                        </td>
                    </tr>
                    <tr>
                        <th>姓名<span class="small-red">&nbsp;*</span></th>
                        <td><input type="text" class="form-control" :value="editTarget.userMain.DisplayName" disabled="">
                        </td>
                        <th>職稱<span class="small-red">&nbsp;*</span></th>
                        <td><input type="text" class="form-control" :value="editTarget.userMain.PositionName" disabled="">
                        </td>
                        <th>信箱<span class="small-red">&nbsp;*</span></th>
                        <td><input type="text" class="form-control" v-model="editTarget.userMain.Email" :disabled="props.admin"></td>
                    </tr>
                    <tr>
                        <th>電話<span class="small-red">&nbsp;*</span></th>
                        <td><input type="text" class="form-control" v-model="editTarget.userMain.Tel" :disabled="props.admin"></td>
                        <th>分機</th>
                        <td><input type="text" class="form-control" v-model="editTarget.userMain.TelExt" :disabled="props.admin">
                        </td>
                        <th>手機<span class="small-red">&nbsp;*</span></th>
                        <td><input type="text" class="form-control" v-model="editTarget.userMain.Mobile" :disabled="props.admin"></td>
                    </tr>
                </tbody>
            </table>
        </div>
        <h5 v-if="props.admin">回復</h5>
        <div v-if="props.admin">
            <textarea class="form-control" rows="4" cols="50" v-model="editTarget.targetItem.Anwser"></textarea>

        </div>
        <div class="row justify-content-center mt-5 mb-5">
                <div class="d-flex">
                    <button v-if="props.admin" role="button" id="goBtn" class="btn btn-color11-2 btn-xs mx-1" @click="adminSave()"><i
                            class="far fa-paper-plane">&nbsp;送出</i></button>
                    <button v-else role="button" id="goBtn" class="btn btn-color11-2 btn-xs mx-1" @click="save()" ><i
                            class="far fa-paper-plane">&nbsp;回報</i></button>
                        </div>
                <div class="d-flex" v-if="props.admin"><a href="#" role="button"
                        class="btn btn-color9-1 btn-xs mx-1" @click="emit('oncancel')">
                        返回 </a></div>
            </div>
    </div>
</template>