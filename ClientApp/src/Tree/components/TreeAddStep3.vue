<template>
<div>

    <h2>Step3. 每月種植株數</h2>
        <div class="form-row">
            <div class="col-4 col-md-4 form-inline  ">
                <label class="my-2 mx-2">植樹方式 </label><select 
                    class="form-control my-1 mr-0 mr-md-4 WidthasInput"
                    style="min-width: 220px;" v-model="selectTreeType">
                    <option v-for="(option , index) in treePlantTypeOptions" :key="index"  :value="option.Seq">{{ option.Name }}</option>
                </select>
            </div>
            <div class="col-4 col-md-4 form-inline  ">
                <label class="my-2 mx-2">預定開始種植日期</label>
                        <!-- <input type="text" name="bday" placeholder="yyy/mm/dd" class="form-control mydatewidth mr-md-3"> -->
                        <input type="date" name="bday" :value="ToDate('ScheduledPlantDate')"
                        @input="treePlantMain.ScheduledPlantDate= $event.target.value "
                        class="form-control">

            </div>
            <div class="col-4 col-md-4 form-inline">
                <label class="my-2 mx-2">預定完成日期</label>
                <input type="date" name="bday" :value="ToDate('ScheduledCompletionDate')" 
                @input="treePlantMain.ScheduledCompletionDate= $event.target.value" class="form-control">

            </div>
        </div>
        <div class="form-row" style="padding-top: 5px;">
            <div class="col-4 col-md-4 form-inline">
                <label class="my-2 mx-2">發包日期(決標日期)</label>
                <input type="date" name="bday" :disabled="createType != 3 && createType != 2" :value="ToDate('ContractDate')" 
                 @input="treePlantMain.ContractDate = $event.target.value" class="form-control">
            </div>
            <div class="col-4 col-md-4 form-inline">
                <label class="my-2 mx-2">預定植樹總面積(公頃)</label>
                <input type="text" class="form-control" v-model.lazy="treePlantMain.ScheduledPlantTotalArea">
            </div>
            <div class="col-4 col-md-4 form-inline  ">
                <label class="my-2 mx-2">實際完成種植日期</label>
                        <!-- <input type="text" name="bday" placeholder="yyy/mm/dd" class="form-control mydatewidth mr-md-3"> -->
                        <input type="date" name="bday" :value="ToDate('ActualCompletionPlantDate')" 
                         @input="treePlantMain.ActualCompletionPlantDate = $event.target.value" class="form-control">
            </div>
        </div>
        <div class="form-row">
            <div class="col-4 col-md-4 form-inline">
                <label class="my-2 mx-2">承攬廠商</label>
                <input type="text" class="form-control"   :disabled="createType != 3" v-model="treePlantMain.Contractor">
            </div>
            <div class="col-4 col-md-4 form-inline">
                <label class="my-2 mx-2">承攬廠商統編</label>
                <input type="text" class="form-control"   :disabled="createType == 1"  v-model="treePlantMain.ContractorUniformNumber ">

            </div>
        </div>
        <div class="form-row" style="padding-top: 5px;">
            <div class="col-4 col-md-4 form-inline">
                <label class="my-2 mx-2">種樹發包商</label>
                <input type="text" class="form-control" v-model="treePlantMain.PlantContractor">
            </div>
            <div class="col-4 col-md-4 form-inline">
                <label class="my-2 mx-2">種樹發包商統編</label>
                <input type="text" class="form-control" v-model="treePlantMain.PlantContractorUniformNumber">
            </div>
        </div>
        <div class="form-row" style="padding-top: 5px;">
            <div class="col-4 col-md-4 form-inline">
                <label class="my-2 mx-2">契約經費(元)</label>
                <input type="text" class="form-control" v-model="treePlantMain.ContractExpenses">
            </div>
            <div class="col-4 col-md-4 form-inline">
                <label class="my-2 mx-2">植樹經費(元)</label>
                <input type="text" class="form-control" v-model="treePlantMain.PlantExpenses" >
            </div>
        </div>
        <div class="form-row" >
            <div class="col-4 col-md-4 form-inline">
                <label class="my-2 mx-2">執行狀況描述</label>
                <textarea class="form-control" style="min-width: 750px;" v-model="treePlantMain.ExecutionStatusDescription"></textarea>
            </div>
         </div>

        <!-- <TreeAddFromXML v-if="createType == 1 " 
            :engSeq="treePlantMainEngSeq ?? tender.Seq" 
            :edit="treePlantMainSeq"
            :AreaTotal="treePlantMain.ScheduledPlantTotalArea" 
            :isSave="save"
            @afterSave="afterSave"
            @afterLoad="(items) => treePlantMain.PlantExpenses = items.reduce((pre, cur) => pre +cur.TreeAmount, 0 )"></TreeAddFromXML> -->

        <TreeAddCustomer
            :engSeq="treePlantMainEngSeq  ?? tender.Seq"
            :AreaTotal="treePlantMain.ScheduledPlantTotalArea" 
            :treePlantMainSeq="treePlantMainSeq"
            :edit="treePlantMainSeq"
            :isSave="save"
            :dataFromPcces="dataFromPcces"
            :createType="createType"
            :treePlantMain="treePlantMain"
            :monthDiff="monthDiff"

            :startMonth="startMonth"
            @afterSave="afterSave" ref="treeCustomer"
            @planteScheduleDateChange="treePlantMain.ScheduledPlantDate = treePlantMain.ScheduledCompletionDate"></TreeAddCustomer >
        <div class="row justify-content-center mt-5">
            <div class="d-flex"><button role="button"
                class="btn btn-color11-2 btn-xs mx-1" @click="saveTreePlantMain()"><i
                    class="fas fa-save" >&nbsp;儲存</i></button>
            </div>
            <div class="d-flex">
                <button href="#" title="刪除" class="btn btn-color11-4 btn-xs mx-1" @click="removeTreePlant()"><i class="fas fa-trash-alt"></i> 刪除</button>
            </div>
            
            <div class="d-flex"><button role="button" @click="() => $emit('afterCancel')" class="btn btn-color9-1 btn-xs mx-1">
            回上頁 </button></div>

        </div>
</div>
</template>
<script>

// import TreeAddFromXML from './TreeAddFromXML.vue';

import console from 'console';
import Common from "../../Common/Common2.js";
import TreeAddCustomer from './TreeAddCustomer.vue';
export default{
    data : () => {
        return {
            dataFromPcces : {},
            selectTreeType :null,
            treePlantMain :{
                ScheduledCompletionDate : new Date().toString(),
                ScheduledPlantDate : new Date().toString()
            },
            treeList : [],
            save :false,
            AreaDetect : {
                overArea: false,
                AccArea : 0
            },
            AreaDetectColor : null,

        };

    },
    // type 0 : pccex匯入XML 、自行勾稽標管， type 1 : 自行新增
    props : [ "createType", "treePlantTypeOptions", "tender", "treePlantMainEngSeq", "EngType", "Eng" , "treePlantMainSeq"],
    emits : ["afterSave", "treeMainLoaded", "afterCancel"],
    components:{
        TreeAddCustomer, 
        // TreeAddFromXML

    },
    computed: {

        startMonth()
        {
            return  new Date(this.treePlantMain.ScheduledPlantDate).getMonth() +1;
        },
        monthDiff()
        {
            console.log("aaa");
            return  Common.monthDiff(new Date( this.treePlantMain.ScheduledPlantDate ), 
            new Date( this.treePlantMain.ScheduledCompletionDate)) ;
        }
    },
    watch:{
        Eng:{
           async  handler(value)
            {

                console.log("init dataFromPcces", this.dataFromPcces)
                if(this.createType == 1 && !this.treePlantMainSeq) 
                {

                    let res = await window.myAjax.post("Tree/getTreePlantMainFromPcces", {engSeq : value.Seq  })

                    this.dataFromPcces = res.data;
                    this.treePlantMain.ScheduledPlantDate = res.data.treePlantMain.ScheduledPlantDate;
                    this.treePlantMain.ScheduledCompletionDate = res.data.treePlantMain.ScheduledCompletionDate;
                    this.treePlantMain = Object.assign(res.data.treePlantMain, this.treePlantMain);

                    this.$refs.treeCustomer.GetDataFromPcces(this.dataFromPcces);

                    console.log("init dataFromPcces", 123)
                }
                this.treePlantMain.ContractExpenses =  this.treePlantMain.ContractExpenses ?? value.SubContractingBudget;

            },
            deep: true
        },
        treePlantMainSeq: 
        {
            async handler(value)
            {
                this.GetTreePlanMain(value)
            }
        },
        EngType : {
            handler(value)
            {
                this.treePlantMain.TPEngTypeSeq = value;
            }
        }


    },
    methods : {
        setTreeMainProperty(name, value)
        {
            console.log("123");
            this.treePlantMain[name] = value;
        },
        async removeTreePlant()
        {
            let r = confirm("確定要刪除?");
            if(r) var res = (await window.myAjax.post("Tree/removeTreeMain", {id : this.treePlantMainSeq})).data;
            if(res)
            {
                window.location = "new";
            }
        },

        ToDate(col)
        {
            this.treePlantMain[col] = !this.treePlantMain[col] ? "" : Common.ToDate(this.treePlantMain[col]);
            return this.treePlantMain[col];
        },
        async saveTreePlantMain()
        {

            this.treePlantMain.TreePlantTypeSeq = this.selectTreeType;
            this.treePlantMain.EngCreatType =this.createType;
            console.log(this.Eng);
            this.treePlantMain.EngSeq = this.Eng.Seq;

 
            // if(!this.treePlantMain.ScheduledCompletionDateStr || !this.treePlantMain.ScheduledPlantDateStr )
            // {
            //     alert("請輸入預定種植日和預定完成日");
            //     return ;
            // }
            if( !this.Eng.EngNo || this.Eng.EngNo == "")
            {
                alert("工程編號不該空白!");
                return ;
            }
            
            // if(this.createType != 1 )
            // {
            //     if( this.$refs.treeCustomer.treeMonths > 0 &&
            //         this.$refs.treeCustomer.treeMonths[this.$refs.treeCustomer.treeMonths.length-1].AccScheduledArea > this.treePlantMain.ScheduledPlantTotalArea)
            //     {
            //         alert("超出預定植樹總面積(公頃)，請調整清單");
            //         return ;
            //     }
            // } 
            if(this.createType == 3) 
            {
                let treeMainClone = Object.assign({}, this.treePlantMain);

                this.treePlantMain = Object.assign(treeMainClone, this.Eng);
                this.treePlantMain.EngSeq = null;

            }
            else {
                let EngClone = Object.assign({}, this.Eng);
                this.treePlantMain = Object.assign(EngClone, this.treePlantMain);
            }
            console.log('this.treePlantMain ', this.treePlantMain );
            var insertSeq;
            if(this.treePlantMainSeq)
            {
                this.treePlantMain.Seq = this.treePlantMainSeq;
                await window.myAjax.post("Tree/updateTreeMain", { m : this.treePlantMain});
            }
            else if(this.createType != 3){
                insertSeq  = (await window.myAjax.post("Tree/insertTreeMain", { m : this.treePlantMain, engSeq: this.tender.Seq })).data.insertSeq;
            }
            else {
                insertSeq = (await window.myAjax.post("Tree/insertTreeMainSelf", { m : this.treePlantMain })).data.insertSeq;
            }

        
            this.save = insertSeq ? insertSeq : this.treePlantMain.Seq;

        },
        async afterSave()
        {

            if( !this.treePlantMainSeq){

                alert("新增成功");


            } 
            else alert("儲存成功")
            console.log("gggg", this.treePlantMain);
            this.$emit('afterSave', this.save);
            this.save = 0;
        },
        async GetTreePlanMain(treePlantSeq)
        {
            let res = await window.myAjax.post("Tree/getTreePlantMain", {createType: this.createType, treePlantMainEngSeq : this.treePlantMainEngSeq, treeMainSeq : treePlantSeq});
            this.treePlantMain = res.data == "" ? {} : res.data ;
            this.selectTreeType = this.treePlantMain.TreePlantTypeSeq;
            this.$refs.treeCustomer.SetTreeScheduledStart(this.treePlantMain);
            this.$emit("treeMainLoaded", this.treePlantMain);
        }
    },
    async mounted()
    {


        if(this.treePlantMainSeq)
        {


            await this.GetTreePlanMain(this.treePlantMainSeq);
        }
        else {



            this.treePlantMain.Contractor = this.tender.BuildContractorName;
            this.treePlantMain.ContractorUniformNumber = this.tender.BuildContractorTaxId;
            this.treePlantMain.ContractDate =this.tender.ActualBidAwardDate ? Common.ToDate(this.tender.ActualBidAwardDate) : "";


        }

    }

}

</script>