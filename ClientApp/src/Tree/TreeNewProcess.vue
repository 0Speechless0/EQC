<template>

        <div class="card whiteBG mb-4 colorset_B">
                <div class="card-header">
                    <h3 class="card-title font-weight-bold">{{ title }}</h3>
                </div>
                <div class="card-body">
                    <div id="app">
                        <TreeStep1 v-if=" (!treePlantMainEngSeq && !selectTender.Seq  )  && type != 3" 
                            :createType="type"
                            :title="typeTitle"
                            @confirm="afterChange"></TreeStep1>
                        <TreeStep2 
                            ref="step2"
                            v-if="selectTender.Seq  && type != 3 && $refs.TreeStep3"  
                            :Eng="Eng" 
                            :EngType="EngType"
                            :EngTypeOptions="EngTypeOptions"
                            :treePlantMainSeq="treePlantMainSeq "
                            :tender="selectTender"
                            :createType="type"
                            :treeMain="treeMain"
                            @assignToTreeProperty="(name, value) => $refs.TreeStep3.setTreeMainProperty(name, value)"
                            > </TreeStep2>
                        <div v-if="type == 3">
                            <h2>Step1. 請輸入基本資料</h2>
                                    <div class="setFormcontentCenter">
                                        <div class="form-row">
                                            <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                                                <label class="my-2 mx-2">工程類別<span
                                                        class="small-red">&nbsp;*</span></label><select
                                                    class="form-control my-1 mr-0 mr-md-4 WidthasInput"
                                                    style="min-width: 220px;"  v-model="EngType">
                                                    <option v-for="(engTypeOption,index) in EngTypeOptions " :key="index" :value="engTypeOption.Seq">{{engTypeOption.Name }}</option>
            
                                                </select>
                                            </div>
                                        </div>
   
                                        <div class="form-row">
                                            <div class="col-12 form-inline my-2 justify-content-md-between"><label
                                                    class="my-2 mx-2">工程名稱<span
                                                        class="small-red">&nbsp;*</span></label><input type="text"
                                                    placeholder="" class="col-12 col-md-10 form-control" v-model="Eng.EngName"></div>
                                        </div>
                                        <div class="form-row">
                                            <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                                                <label class="my-2 mx-2">工程地點<span
                                                        class="small-red">&nbsp;*</span></label>
                                                <div><select class="form-control my-1 mr-0 mr-sm-1" v-model="Eng.CitySeq">
                                                    <option v-for="(option,index) in cityList " :key="index" :value="option.Seq">{{option.CityName }}</option>
           
                                                    </select><select
                                                        class="form-control my-1 mr-0 mr-md-1 mr-0 mr-md-4" v-model="Eng.EngTownSeq">
                                                        <option v-for="(option,index) in townList " :key="index" :value="option.Seq">{{option.TownName }}</option>

                                                    </select></div>
                                            </div>
                                            <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                                                <label class="my-2 mx-2">工程年度<span
                                                        class="small-red">&nbsp;*</span></label><input type="number"
                                                    class="form-control"  v-model="Eng.EngYear">
                                            </div>
                            
                                                <div class="col-12 col-md-6 form-inline my-2 justify-content-md-start">
                                                    <label class="my-2 mx-2">工程編號<span
                                                            class="small-red">&nbsp;*</span></label><input type="text" 
                                                            v-model="Eng.EngNo"
                                                        class="col-12 col-md-6 form-control ml-4">
                                                </div>

                                        </div>

                                        <div class="form-row">
                                            <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between"><label
                                                    class="my-2 mx-2">主辦機關<span
                                                        class="small-red">&nbsp;*</span></label><select
                                                        :disabled="user.RoleSeq  == 20 || user.RoleSeq  == 3"
                                                    class="form-control my-1 mr-0 mr-md-4 WidthasInput" v-model="Eng.OrganizerUnitSeq">
                                                    <option v-for="(option,index) in unitList " :key="index" :value="option.Value">{{option.Text }}</option>
                  
                                                </select></div>
                                                <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                                                <label class="my-2 mx-2">執行機關<span
                                                        class="small-red">&nbsp;*</span></label><select 
                                                    :disabled="user.RoleSeq  == 20  || user.RoleSeq  == 3"
                                                    
                                                    class="form-control my-1 mr-0 mr-md-4 WidthasInput" v-model="Eng.ExecUnitSeq" >
                                                    <option v-for="(option,index) in unitList " :key="index" :value="option.Value">{{option.Text }}</option>
                  
                                                </select>
                                            </div>

                                        </div>
                                        <div class="form-row">
                                            <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                                                <label class="my-2 mx-2">執行單位<span
                                                        class="small-red">&nbsp;*</span></label><select 
                                                        :disabled="user.RoleSeq  == 20 "
                                                    class="form-control my-1 mr-0 mr-md-4 WidthasInput" v-model="Eng.ExecSubUnitSeq">
                                                    <option v-for="(option,index) in subUnitList " :key="index" :value="option.Value">{{option.Text }}</option>

                                                </select>
                                            </div>
                                            <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                                                <label class="my-2 mx-2">標案建立者<span
                                                        class="small-red">&nbsp;*</span></label><select 
                                                        :disabled="user.RoleSeq  == 20 "
                                                    class="form-control my-1 mr-0 mr-md-4 WidthasInput" v-model="Eng.OrganizerUserSeq">
                                                    <option v-for="(option,index) in userList " :key="index" :value="option.Value">{{option.Text }}</option>
                
                                                </select>
                                            </div>
                                        </div>
   
                                        <!-- <div class="form-row">
                                            <div class="col-12 col-md-7 form-inline my-2 justify-content-md-between">
                                                <label class="my-2 mx-2">契約經費(未發包填核定經費)</label><input type="text"
                                                    placeholder="" class="form-control my-1 mr-0 mr-md-4" v-model="ContractExpenses">
                                            </div>
                                        </div> -->
        
                                    </div>
                        </div>

                            <div v-if="$refs.TreeStep3">
                                <div  class="form-row">
                                    <div class="col-12 col-md-6 form-inline my-2 ">
                                        <label class="my-2 mx-2">地號<span
                                                            class="small-red"></span></label>
                                        <div class="mt-3 col-7 col-md-8  col-lg-9">
                                            <input class="form-control"  style="width:100%" v-model="$refs.TreeStep3.treePlantMain.PlaceNumber"/>
                                        </div>

                            
                                    </div>

                                
                                </div>
                                <div  class="form-row" >
                                    <div class="col-12 col-md-6 form-inline my-2 ">
                                        <label class="my-2 mx-2">種植前現場照片<span
                                                            class="small-red"></span></label>
                                        <div class="mt-3 col-6 col-md-7  col-lg-8">
                                            <b-form-file  multiple v-model="photo.SchTreePhoto" placeholder="上傳種植前現場照片"></b-form-file>
                                        </div>

                            
                                    </div>

                               
                                </div>
                                <downloadSlot    class="mt-3 form-row" route="Tree" :seq="`${fileSeq}.0`"  ref="downloadSlot" :uploadTrigger="photo"/>
                                <div  class="form-row">
                                    <div class="col-12 col-md-6 form-inline my-2 ">
                                    <label class="my-2 mx-2">種植後現場照片<span
                                                        class="small-red"></span></label>
                                    <div class="mt-3 col-6 col-md-7  col-lg-8">
                                        <b-form-file  multiple v-model="photo.AclTreePhoto" placeholder="上傳種植後現場照片"></b-form-file>
                                    </div>

                                    </div>
                                
                                </div>

                                <downloadSlot  class="mt-3 form-row" route="Tree" :seq="`${fileSeq}.1`"  ref="downloadSlot" :uploadTrigger="photo"/>
                               
                            </div>





                    <!-- <downloadList route="Tree" :seq="downloadSeq" :viewImage="true" ref="downloadList"> </downloadList> -->
                                    <TreeStep3 v-if="selectTender.Seq || type == 3" 
                                :createType="type" 
                                :treePlantTypeOptions="TreeTypeOptions"
                                :tender="selectTender"
                                :Eng="Eng"
                                :treePlantMainEngSeq="treePlantMainEngSeq"
                                :treePlantMainSeq="treePlantMainSeq "
   
                                @afterSave="afterSave"
                                @afterCancel="afterCancel"
                                @treeMainLoaded="EngLoad"

                                ref="TreeStep3"> </TreeStep3>
                    </div>
                </div>
        </div>



</template>

<script>

import TreeStep1 from "./components/TreeAddStep1.vue";
import TreeStep2 from "./components/TreeAddStep2.vue";
import TreeStep3 from "./components/TreeAddStep3.vue";
import downloadSlot from "../components/downloadSlot.vue";
import {useLevelSelectionStore} from "../store/LevelSelectionStore.js";
import {useSelectionStore} from "../store/SelectionStore";
const userSelectionStore = useSelectionStore("TenderPlan/GetUserList");
const citySelectionStore = useSelectionStore("TenderPlan/GetCityList");
const townSelectionStore = useSelectionStore("TenderPlan/GetTownList");



export default{

    props : ["type", "title", "treePlantMainEngSeq", "treePlantMainSeq" ],
    emits : ['afterSave'],
    components : {
        TreeStep1, 
        TreeStep2, 
        TreeStep3,
        downloadSlot
    },

    data: () => {
        return {
            selectTender: {},
            insertCode : "",
            Eng : {
                ExecUnitSeq :null,
                ExecSubUnitSeq : null,
                OrganizerUserSeq : null,
                EngYear : new Date().getFullYear() - 1911

            },
            EngType : null,
            TreeType : null,
            EngTypeOptions : [],
            TreeTypeOptions : [],
            newInsertTreeMainSeq :0,
            unitList : [],
            subUnitList : [], 
            cityList : [],
            userList : [],
            townList : [],
            photo :{
                SchTreePhoto : [],
                AclTreePhoto : []
            },
            upload : 0,
            downloadListType: null,
            user: {},
            treeMain : null
        }
    },
    watch : {
        photo: {
            async handler(value){
                if(Object.keys(this.photo).length == 0) return ;
                var uploadFiles = new FormData();
                uploadFiles.append("seq", this.fileSeq );

                this.imageAppend(uploadFiles, "SchTreePhoto");
                this.imageAppend(uploadFiles, "AclTreePhoto");
                var resp = (await window.myAjax.post("Tree/UploadFiles", uploadFiles, 
                
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    })).data;
                this.photo = {};
            },
            deep : true,
            flush : 'post'
            
        },
        selectTender : {
            async handler(tender){
                if(tender.Seq && !this.treePlantMainSeq ){
                    let data = ( await window.myAjax.post("Tree/GetEng", { engSeq : tender.Seq, createType: this.type }) ).data;
                    this.EngTypeOptions = data.engTypeOption;
                    this.TreeTypeOptions = data.treeTypOption;
                    this.Eng =  data.eng ?? {};
                }

            }
        },
        EngType :{
            handler(value)
            {
            }
        },
        //作用於編輯模式、和新增模式的後續
        "Eng.ExecUnitSeq" : {
            async handler(value, oldValue){
                if(value != null && this.type == 3){
                    this.subUnitList = (await window.myAjax.post("TenderPlan/GetUnitList", { parentUnit: value})).data;
                    this.userList = [];
                }

            }

        },
        //作用於編輯模式、和新增模式的後續
        "Eng.CitySeq" :{
            async handler(value, oldValue){
                if(value != null  && this.type == 3)this.townList = await townSelectionStore.setParameter({id :value}).Get();
            }
        },
        //作用於編輯模式、和新增模式的後續
        "Eng.ExecSubUnitSeq" : {
            async handler(value, oldValue){
                if(value != null && this.type == 3) this.userList = (await window.myAjax.post("Users/GetUserBySubUnit", {
                    subUnitSeq : value
                })).data;
            }
        }
    },
    methods :{
        openDownloadList(type)
        {
            // this.downloadListType = type;
            // $("#downloadList").modal("show");
        },
        imageAppend(uploadFiles, img){
            var noVaild = false;
            if(this.photo[img]) {
                this.photo[img].forEach(e => {

                    if(e.type.split('/')[0] !='image') {
                        noVaild = true;
                        return ;
                    }
                    uploadFiles.append("photo."+img, e, e.name);
                })

            }  
            if(noVaild)
            {
                alert("請上傳圖檔");
            }
        },
        async afterSave(treeMainSeq)
        {
            if(!this.treePlantMainSeq )
            {
                await window.myAjax.post("Tree/RenamePhotoDir", {source :`${this.fileSeq}.0`, target: `${treeMainSeq}.0`});
                await window.myAjax.post("Tree/RenamePhotoDir", {source :`${this.fileSeq}.1`, target : `${treeMainSeq}.1`} );
            }

            this.$emit("afterSave", treeMainSeq);
        },
        afterCancel()
        {
            this.selectTender= {};
            this.$emit("afterCancel");
        },
        async EngLoad(treeMain)
        {
            this.EngType = treeMain.TPEngTypeSeq;
            this.treeMain = treeMain;
            if(this.type == 3 )
                this.Eng =  treeMain ;
            else{
                let data = ( await window.myAjax.post("Tree/GetEng", { engSeq : this.treePlantMainEngSeq , createType: this.type }) ).data;
                this.Eng = data.eng;
                this.treeMain.execUnitName = this.Eng.execUnitName; 
                this.EngTypeOptions = data.engTypeOption;
                this.TreeTypeOptions = data.treeTypOption;
                this.Eng.ExecSubUnitSeq = treeMain.ExecSubUnitSeq;
                this.Eng.OrganizerUserSeq = treeMain.OrganizerUserSeq;
                this.Eng.TPEngTypeSeq = treeMain.TPEngTypeSeq;

            }
               
        },
        afterChange(tender)
        {
            this.selectTender = tender;
            console.log("this.selectTender", this.selectTender);

        },

    },
    computed : {
        fileSeq()
        {
            return this.treePlantMainSeq ?  `${this.treePlantMainSeq}` :`temp.${this.insertCode }`;
        },
        typeTitle() {
            if(this.type == 1)
                return '***以下為"水利署"數位轉型的案件清單*** ';
            if(this.type == 2)
                return '***以下為工程會-標案管理系統***';
            return "";
        }
    },
    async mounted() {
    

        if(this.treePlantMainEngSeq)
        {
            this.selectTender =   
                ( await window.myAjax.post("Tree/GetSelectTender", {engSeq :this.treePlantMainEngSeq, createType : this.type}) )
                .data; 
            
        }

        if(this.type == 3)
        {
            let data = ( await window.myAjax.post("Tree/GetEng", { engSeq : 0, createType: this.type }) ).data;

            this.EngTypeOptions = data.engTypeOption;
            this.TreeTypeOptions = data.treeTypOption;
            this.user = (await window.myAjax.post("Users/GetUserInfo")).data.userInfo;
            const store = useLevelSelectionStore("Unit/GetUnitList", "parentSeq");
            this.unitList = await   store.GetOneLevelSelection(null, 0);
            this.cityList = await citySelectionStore.Get();
            if(!this.Eng.Seq)
            {
      
                this.Eng.ExecUnitSeq = this.user.UnitSeq1 ;
                this.Eng.ExecSubUnitSeq = this.user.UnitSeq2;
                this.Eng.OrganizerUserSeq = this.user.Seq;
                this.Eng.OrganizerUnitSeq = this.user.UnitSeq1;
                console.log("insert mode");
                
            }
            else
            {
                console.log("update mode");
                return ;
                
            }





        }
        if(!this.treePlantMainSeq)
        {
            this.insertCode = new Date().valueOf();
        }


    }
}

</script>