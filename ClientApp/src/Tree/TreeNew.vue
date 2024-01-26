<template>
    <div>
        <div class="card whiteBG mb-4 colorset_B" v-show="route == null">
            <div class="card-header">
                <h3 class="card-title font-weight-bold">植樹列表</h3>
                <div class="mt-auto">
                <a style="color: white;" href="https://www.youtube.com/watch?v=EGinhApSUJo&ab_channel=%E6%95%A6%E9%99%BD%E7%A7%91%E6%8A%80" target="_blank">
                                        操作影片
                                    </a>
            </div>
            </div>
            <div class="card-body">
                <div id="app">
                    <div >
                        <form class="form-group">
                            <div class="form-row">
                                <div class="col-1 mt-3"><select class="form-control" v-model="year">
                                        <option selected="selected" :value="0" > 全部</option>
                                        <option v-for="(option, index) in data.yearOptions" :value="option" :key="index">{{ option }}</option>
                                    </select></div>
                                <div class="col-1 mt-3"><select class="form-control" v-model="TPEngTypeSeq">
                                    <option selected="selected" :value="0" > 全部</option>
                                    <option v-for="(option, index) in data.TPEngTypeOption" :value="option.Value" :key="index">{{ option.Text }}</option>
                                </select></div>
                                <!-- <div class="col-12 col-sm-3 mt-3"><select class="form-control">
                                        <option value="23"> 第一河川局 </option>
                                        <option value="36"> 南區水資源局 </option>
                                    </select></div> -->
                                <unitFilter
                                    :newUnitLevelOptions="data.unitOptions" :maxUnitLevel="1" @afterUnitChange="(subUnit) => getEngEditList(subUnit, 1)" class="form-row col-8"></unitFilter>

                                <!-- <div class="col-12 col-sm-3 mt-3"><select class="form-control" v-model="subUnit">
                                    <option value="" > 全部單位 </option>
                                    <option v-for="(option, index) in subUnitOptions" :value="option" :key="index" >{{ option }}</option>
                                </select></div> -->
                                <!-- <div class="col-12 col-sm-3 mt-3" v-for="(unitOptions,index) in unitOptionsArr" :key="index">
                                    <select class="form-control" @change="UnitChange($event, unitOptions, index)">
                                    <option value="" > 全部單位 </option>
                                    <option v-for="(option, index2) in unitOptions" :value="index2" :key="index2">{{ option.Text }}</option>
                                    </select>
                                </div> -->

                                <!-- <div class="col-2 col-sm-2"><label for="council" class="m-2"
                                        style="float: right; padding-top: 3px;">勾稽工程會</label></div>
                                <div class="col-2 col-sm-2 mt-3"><select id="council" class="form-control">
                                        <option value="-1"> 全部</option>
                                        <option value="0"> 有 </option>
                                        <option value="1"> 無 </option>
                                    </select></div> -->
                            </div>
                        </form>
                        <div class="row justify-content-between">
                            <div class="form-inline col-12 col-md-8 small"><label class="my-1 mr-2"> 共 <span
                                        class="text-danger">{{ data.count}}</span> 筆，每頁顯示 </label><select
                                    class="form-control sort form-control-sm" v-model="perPage" >
                                    <option value="10">10</option>
                                    <option value="20">20</option>
                                    <option value="30">30</option>
                                </select><label class="my-1 mx-2">筆，共<span
                                        class="text-danger">{{ pageCount }}</span>頁，目前顯示第</label><select
                                    class="form-control sort form-control-sm" v-model="page">
                                    <option  v-for="n in pageCount" :value="n" :key="n"> {{n}} </option>
                                </select><label class="my-1 mx-2">頁</label></div>
                            <div class="col-12 col-md-4 col-xl-2 mt-3">

                                    <button role="button" class="btn btn-outline-secondary btn-xs mx-1" @click="openAddModal()">
                                        <i class="fas fa-plus"></i>&nbsp;新增工程 
                                    </button>
                            

                            </div>
                        </div>
                        <div class="table-responsive">
                            <table border="0" class="table table1 min910">
                                <thead class="insearch">
                                    <tr>
                                        <th class="sort">排序</th>
                                        <th class="number">工程編號</th>
                                        <th>工程名稱</th>
                                        <th>工程類別</th>
                                        <th>執行機關</th>
                                        <th>執行單位</th>
                                        <!-- <th>勾稽工程會</th>
                                        <th>Pcces檔案</th> -->
                                        <th class="text-center">明細</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(item, index) in data.list" :key="index">
                                        <td>{{index+1}}</td>
                                        <td>{{ item.EngNo }}</td>
                                        <td>{{ item.EngName }}</td>
                                        <td>{{ item.TPEngTypeName }}</td>
                                        <td>{{item.execUnitName }}</td>
                                        <td>{{item.execSubUnitName }}</td>
                                        <!-- <td>無</td>
                                        <td><button class="btn btn-color11-1 btn-xs mx-1"><i
                                                    class="fas fa-download"></i> 下載<br>111/11/21 </button></td> -->
                                        <td style="min-width: 105px;">
                                            <div class="row justify-content-center m-0" >
                                                
                                                <button title="編輯" class="btn btn-color11-3 btn-xs mx-1" @click="editTreeMain(item)">
                                                    <i class="fas fa-pencil-alt" ></i> 編輯 
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>

                </div>
            </div>
        </div>
        <div v-if="route == 1">
            <TreeNewProcess :title="!editTreeMainSeq ? '水利署數位轉型的案件清單' : '編輯' " 
                :type="1" 
                :treePlantMainEngSeq="editTreeMainEngSeq"
                :treePlantMainSeq="editTreeMainSeq"
                @afterSave="afterSave"
                @afterCancel="afterCancel"
                ></TreeNewProcess>
        </div>
        <div v-if="route == 2">
            <TreeNewProcess :title="!editTreeMainSeq ? '自行勾稽標管系統'  : '編輯' " 
            :type="2" 
            :treePlantMainEngSeq="editTreeMainEngSeq" 
            :treePlantMainSeq="editTreeMainSeq"
            @afterSave="afterSave"
            @afterCancel="afterCancel" ></TreeNewProcess>
        </div>
        <div v-if="route == 3">
            <TreeNewProcess :title="!editTreeMainSeq ? '自行新增非工程資料'  : '編輯' " 
            :type="3" :treePlantMainEngSeq="editTreeMainEngSeq" 
            :treePlantMainSeq="editTreeMainSeq"
            @afterSave="afterSave"
            @afterCancel="afterCancel"></TreeNewProcess>
        </div>
        <TreeAddModal @afterSelect="(e) => route = e"></TreeAddModal>
        
    </div>


</template>

<script>

import TreeAddModal from "./components/TreeAddModal.vue";
import TreeNewProcess from "./TreeNewProcess.vue";
import unitFilter from "../components/unitFilter.vue";
export default{
    data : () => {
        return {
            route : null,
            editTreeMainEngSeq: null,
            editTreeMainSeq : null,
            lastSubUnitIndex : 0,
            data : {},
            TPEngTypeSeq : 0,
            year : 0,
            page : 1,
            perPage : 10,
            subUnit : [ "", ""]

        }
    },
    components:{
        TreeAddModal,
        TreeNewProcess,
        unitFilter
    },
    watch :{

        TPEngTypeSeq :{
            async handler()
            {
                await this.getEngEditList(this.subUnit, 1)
            },
            flush: 'post'
        },
        perPage: {
            async handler()
            {
                await this.getEngEditList()

            },
            flush: 'post'
        },
        page: {
            async handler()
            {
                await this.getEngEditList()

            },
            flush: 'post'
        },
        year :{
            async handler()
            {
                await this.getEngEditList(this.subUnit, 1)
            },
            flush: 'post'
        }
    },
    computed : {
        pageCount()
        {
            return this.data.count != undefined ? Math.ceil(this.data.count/this.perPage) : 0;
        }
    },
    methods : {
        afterCancel()
        {
            this.getEngEditList();
            this.editTreeMainSeq = null;
            this.editTreeMainEngSeq = null;
            this.route = null;
        },
        afterSave(treeMainSeq)
        {  
            this.editTreeMainSeq = treeMainSeq;
        },
        openAddModal()
        {
            window.openModal("#addDoor");
        },
        async getEngEditList(subUnit = this.subUnit, page = this.page){
            

            this.data = (await window.myAjax.post("Tree/GetEngEditList",
                    {
                        page: page, 
                        perPage : this.perPage,
                        year : this.year,
                        subUnit : subUnit,
                        TPEngTypeSeq  : this.TPEngTypeSeq
                    })).data
            // this.subUnit[0] = this.data.userUnitName ;
            this.subUnit = subUnit;
            this.page = page;

        },
        // UnitChange(e, unitOptions, j)
        // {
            
        //     console.log(j);
        //     var unit;
        //     if(e.target.value != "")
        //     {
        //          unit = unitOptions[e.target.value];
        //         this.subUnit = unit.Text;
        //     }
        //     else if( j > 0){
        //        this.subUnit = this.unitOptionsArr[j-1][this.lastSubUnitIndex].Text;
        //        unit.Value = this.unitOptionsArr[j-1][this.lastSubUnitIndex].Value;
        //     }
        //     else 
        //     {
        //         unit.Value = null;
        //     }
        //     this.getUnit(unit.Value, e.target.value, j);
        // },
        // async getUnit(seq, i, j)
        // {
        //     if(!this.hasUnitOptions[j]){
        //         var Options = (await window.myAjax.post('Unit/GetUnitList',{parentSeq : seq })).data;
        //         if(Options.length > 0)
        //         {
        //             this.unitOptionsArr.push(Options);
        //             this.hasUnitOptions[j] = true;
        //             this.lastSubUnitIndex = i;

        //         }
                

        //     }
        //     else{
        //         this.unitOptionsArr = this.unitOptionsArr.slice(0, j+1);
        //         this.hasUnitOptions = this.hasUnitOptions.slice(0, j+1);
        //         this.getUnit(seq,i ,j);
        //     }

        // },
        async editTreeMain(item)
        {
            console.log("GGG", item);
            this.editTreeMainEngSeq= item.EngSeq == null ? -1 : item.EngSeq ;
            console.log("edit treeMain Seq", item.Seq);
            this.editTreeMainSeq = item.Seq;
            this.route= item.EngCreatType;
        }
    
    },
    async mounted()
    {
     

        await this.getEngEditList();
        console.log(this.data);
    }

}
</script>
