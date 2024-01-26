<template>

<div id="searchResult" >
    <h2>Step2. 顯示案件基本資料</h2>
    <div class="setFormcontentCenter" style="width:750px ;">
        <div class="form-row">
            <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                <!-- <label class="my-2 mx-2">工程類別<span
                        class="small-red">&nbsp;*</span></label> -->
                    <select
                    class="form-control my-1 mr-0 mr-md-4 WidthasInput"
                    v-model="Eng.TPEngTypeSeq"
                    @change="assignToTreeProperty('TPEngTypeSeq', Eng.TPEngTypeSeq)"
                    style="min-width: 220px;">
                    <option v-for="(engTypeOption,index) in _EngTypeOptions " :key="index" :value="engTypeOption.Seq">{{engTypeOption.Name }}</option>
                </select>
            </div>
        </div>
        <!-- <div class="form-row" >
            <div class="col-12 form-inline my-2 " >
                <label class="my-2 mx-2">工程類別<span class="small-red">&nbsp;*</span></label>
                <div>
                    <select class="col-12 col-md-10 form-control">
                        <option value="01"> 水資源專案計畫 </option>
                        <option value="02"> 前瞻水環境 </option>
                        <option value="03"> 前瞻水安全 </option>
                    </select>
                </div>
            </div>  
        </div> -->
        <div class="form-row">
            <div class="col-12 form-inline my-2 justify-content-md-between"><label
                    class="my-2 mx-2">工程名稱<span
                        class="small-red">&nbsp;*</span></label>
                        <input  :value="Eng.EngName" disabled="disabled" class="col-12 col-md-10 form-control"></div>
        </div>
        <div class="form-row">
            <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                <label class="my-2 mx-2">工程地點<span
                        class="small-red">&nbsp;*</span></label>
                        <input  :value="Eng.engTownName" disabled="disabled" class="col-12 col-md-10 form-control"></div>
            </div>

            <div class="form-row">
                <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                <label class="my-2 mx-2">工程年度<span
                        class="small-red">&nbsp;*</span></label><input type="number"
                    class="form-control" disabled="disabled" :value="Eng.EngYear">
            </div>
            <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                <label class="my-2 mx-2">工程編號<span
                        class="small-red">&nbsp;*</span></label><input type="text" disabled="disabled"
                        :value="Eng.EngNo"
                    class="form-control my-1 mr-0 mr-md-4 WidthasInput">
            </div>


        </div>
        <div class="form-row">
            <div class="col-12 col-md-4 my-2 justify-content-md-between"><label
                    class="my-2 mx-2">主辦機關<span
                        class="small-red">&nbsp;*</span></label>
                        <input  :value="Eng.organizerUnitName" disabled="disabled" class="col-12 col-md-10 form-control"> </div>
            <!-- <div class="col-8 col-md-4 my-2 justify-content-md-between"><label
                    for="a">工程會標案編號</label><input type="text" disabled="disabled" id="a"
                    class="form-control my-1 mr-0 mr-md-4" :value="tender.EngNo" ></div>
            <div class="col-8 col-md-4 my-2 justify-content-md-between"><label
                    for="a">工程會標案名稱</label><input type="text" disabled="disabled" id="a"
                    class="form-control my-1 mr-0 mr-md-4" :value="tender.EngName"></div> -->
        </div>
        <div class="form-row">
            <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                <label class="my-2 mx-2">執行機關<span
                        class="small-red">&nbsp;*</span></label>
                        <input  :value="Eng.execUnitName" disabled="disabled" class="col-12 col-md-10 form-control">
            </div>
        </div>
        <div class="form-row">
            <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                <label class="my-2 mx-2">執行單位<span
                        class="small-red">&nbsp;*</span></label>
                        <select  :disabled="createType != 2" class="col-12 col-md-10 form-control" v-model="Eng.ExecSubUnitSeq"  @change="assignToTreeProperty('ExecSubUnitSeq', Eng.ExecSubUnitSeq)">
                            <option v-for="(item, index) in subUnitList" :key="index" :value="item.Value">{{  
                                item.Text
                            }}</option>
                        </select>

            </div>
            <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                <label class="my-2 mx-2">標案建立者<span
                        class="small-red">&nbsp;*</span></label>
                        <select  :disabled="createType != 2" class="col-12 col-md-10 form-control" v-model="Eng.OrganizerUserSeq" @change="assignToTreeProperty('OrganizerUserSeq', Eng.OrganizerUserSeq)">
                            <option v-for="(item, index) in userList" :key="index" :value="item.Value">{{  
                                item.Text
                            }}</option>
                        </select>
            </div>
        </div>
        <div class="form-row">
            <div class="col-12 col-md-7 form-inline my-2 justify-content-md-between">
                <label class="my-2 mx-2">工程總預算(元)<span
                        class="small-red">&nbsp;*</span></label><input type="text"
                     class="form-control my-1 mr-0 mr-md-4" disabled="disabled" :value="Eng.TotalBudget">
            </div>
        </div>
        <div class="form-row">
            <div class="col-12 col-md-7 form-inline my-2 justify-content-md-between">
                <label class="my-2 mx-2">發包預算(元)</label><input type="text"
                     class="form-control my-1 mr-0 mr-md-4" disabled="disabled" :value="Eng.SubContractingBudget">
            </div>
        </div>
        <div class="form-row">
            <div class="col form-inline my-2"><label class="my-2 mx-2">採購金額(元)</label>
                <div class="custom-control custom-radio mx-2"><input :value="1"
                        type="radio" id="FiftyMillion" name="ProcurementValue"
                        class="custom-control-input" v-model="EngPurchaseAmount" disabled><label for="FiftyMillion"
                        class="custom-control-label">5000萬以上</label></div>
                <div class="custom-control custom-radio mx-2"><input :value="2"
                        type="radio" id="TenToFifty" name="ProcurementValue"
                        class="custom-control-input" v-model="EngPurchaseAmount" disabled><label for="TenToFifty"
                        class="custom-control-label">1000-5000萬</label></div>
                <div class="custom-control custom-radio mx-2"><input :value="3"
                        type="radio" id="OneToTen" name="ProcurementValue"
                        class="custom-control-input" v-model="EngPurchaseAmount" disabled><label for="OneToTen"
                        class="custom-control-label">100-1000萬</label></div><label
                    class="my-2 mx-2">*依工程總預算判斷</label>
            </div>
        </div>
    </div>
    <!-- </div><label class="mt-5 mb-1 mx-2 small-green">* 點選儲存後，僅儲存與該主檔有關之資料</label><label
        class="my-1 mx-2 small-green">*
        點選【匯入標案基本資料】按鈕，系統更新主檔及其相關資料表外，自動將後台預設的材料設備清單、施工管理清單、設備運轉清單、職業安全、環境保育等資料儲存到該標案對應的資料表。</label> -->

</div>
</template>

<script>
import axios from "axios";

export default{

    props: ["Eng", "createType", "EngTypeOptions", "tender", "EngType", "treePlantMainSeq", "treeMain"],
    emits : ["selectEngType", "assignToTreeProperty"],
    watch:{
        EngType : {
            handler(value)
            {
                this.engType = value;
            }
        },
        "Eng.ExecSubUnitSeq" : {
            handler(value)
            {
                
                if(value) this.getUserList(value);

            },
            immediate : true
        },
        "Eng.ExecUnitSeq" : {
            handler(value)
            {
                if(value) this.getSubUnitList(value);

            },
            immediate : true
        }

    },

    data : () => {
        return {
            engType: null,
            userList : [],
            selectSubUnit : null,
            subUnitList: []

        }
    },
    computed: {
        EngPurchaseAmount : {
            get(){
                if(this.Eng.PurchaseAmount) return this.Eng.PurchaseAmount;
                if(!this.Eng.TotalBudget || this.Eng.TotalBudget < 1000) return 0;
                
                if(this.Eng.TotalBudget >= 50000) return 1;
                else if (this.Eng.TotalBudget >= 1000) return 2;
                else if(this.Eng.TotalBudget >= 100) return 3;

                return 0 ;
            },
            set(value){

            }

            
        },
        _EngTypeOptions()
        {
            return this.EngTypeOptions;
        }
    },
    methods:{
        assignToTreeProperty(name, value)
        {
            this.$emit("assignToTreeProperty", name, value);
        },
        async getSubUnitList(unitSeq){
            this.subUnitList = (await window.myAjax.post("TenderPlan/GetUnitList", { parentUnit: unitSeq })).data;

        },
        async getUserList(subUnitSeq)
        {
            this.userList = (await window.myAjax.post("Users/GetUserBySubUnit", {
                    subUnitSeq : subUnitSeq
                })).data;

        }
    },
    mounted(){
        console.log("TreeStep2");

    }
    


}

</script>