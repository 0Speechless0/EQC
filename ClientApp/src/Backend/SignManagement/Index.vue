<template>

        <div>

            <div class="card-body" v-if="route < 2">
                <div id="app">
                    <div>
                        <div class="modal-header">
                            <h2 id="projectUpload" class="modal-title"  style="margin: inherit;">表單新增
                                <a href="javascript:void(0)" role="button" class="btn btn-color11-3 btn-xs sharp mr-1" @click="route =1">
                                                    <i class="fas fa-plus"></i>
                                                </a>
                            </h2>
                            <input  type="text" class="form-control ml-auto" style="width:200px" v-model="dataFilter.search"/>
                            <button @click="getFormList" 
                            type="button" class="ml-1" style="color: rgb(255, 255, 255); background-color: rgb(108, 117, 125); border-color: rgb(108, 117, 125); border-radius: 5px;height: 36px;">查詢</button>
                            <!-- <button type="button" data-dismiss="modal" aria-label="Close"
                                class="close"><span aria-hidden="true">×</span></button> -->

                        </div>
                        <div class="modal-body">

                            <div class="table-responsive">
                                <table class="table table1">
                                    <thead class="insearch">
                                        <tr>
                                            <th><strong>表單代碼</strong></th>
                                            <th><strong>表單名稱</strong></th>
                                            <th>

                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr  v-for="(form, index) in data.FormList" :key="index">

                                                <td style="width:35%" class="align-middle">{{form.FormCode}}</td>
                                                <td style="width:35%" class="align-middle" v-if="edit != form.FormCode">                         
                                                    {{ form.FormName }}


                                                </td>
                                                <td style="width:35%" class="align-middle" v-else>                         
                                                        <input type="text" v-model="form.FormName"  class="form-control"/>


                                                </td>
                                                <td style="width:30%">
                                                    <a href="#" title="編輯" class="btn btn-color11-3 btn-xs m-1" @click="editFormName(form)"><i class="fas fa-pencil-alt" ></i> 修改名稱</a>
                                                    <button type="button" class="btn btn-color11-2 btn-xs mx-1" @click="editFormFlow(form, 2)"  >設定流程</button>

                                                    <a href="#" title="刪除" class="btn btn-color9-1 btn-xs m-1" @click="deleteForm(form)"><i class="fas fa-trash-alt"></i> 刪除</a>
                                                </td>
                                        </tr>
                                        
                                    </tbody>
                                </table>
                            </div>
                            <div class="row justify-content-between">
                                <div class="form-inline col-12 col-md-8 small"><label class="my-1 mr-2"> 共 <span
                                        class="text-danger">{{ data.count}}</span> 筆，每頁顯示 </label><select
                                    class="form-control sort form-control-sm" v-model="dataFilter.perPage" >
                                    <option value="10">10</option>
                                    <option value="20">20</option>
                                    <option value="30">30</option>
                                </select><label class="my-1 mx-2">筆，共<span
                                        class="text-danger">{{ data.pageCount }}</span>頁，目前顯示第</label><select
                                    class="form-control sort form-control-sm" v-model="dataFilter.page">
                                    <option  v-for="n in data.pageCount" :value="n" :key="n"> {{n}} </option>
                                </select><label class="my-1 mx-2">頁</label></div>
                            </div>
                        </div>
                    </div>
                                
                </div>

        </div>
        <div class="card-body" v-else>
                <div id="app">
                    <div>
                        <div class="modal-header">
                            <h2 id="projectUpload" class="modal-title"  style="margin: inherit;">表單流程編輯 </h2>

                            <div class="d-flex justify-content-end">
                                <p class="ml-3 mt-3 mb-0">
                                    表單代碼 : {{editForm.FormCode}}
                                </p>
                                <p class="ml-3 mt-3 mb-0">
                                    表單名稱 : {{editForm.FormName}}
                                </p>
                            </div>

                            <!-- <button type="button" data-dismiss="modal" aria-label="Close"
                                class="close"><span aria-hidden="true">×</span></button> -->

                        </div>
                        <div class="modal-body">

                            <div class="d-flex justify-content-start ml-1 mt-1 mb-1">
                                <div class="d-flex"><button role="button"
                                    class="btn btn-color11-2 btn-xs mx-1" @click="saveSignFlow()"><i
                                        class="fas fa-save" >&nbsp;儲存</i></button>
                                </div>
                                <div class="d-flex"><button role="button" class="btn btn-color9-1 btn-xs mx-1" @click="route =0">
                                <a class="link ms-auto" style="color: white;" >回上頁 </a></button></div>
                                <a href="javascript:void(0)" role="button" class="btn btn-color11-3 btn-xs sharp mr-1 ml-auto" @click="addSignFlow()">
                                    <i class="fas fa-plus"></i>
                                </a>
                                <a href="javascript:void(0)" role="button" class="btn btn-color11-4 btn-xs sharp mr-1" @click="removeSignFlow()">
                                    <i class="fas fa-minus"></i>
                                </a>

                            </div>
                            <div class="table-responsive">
                                <table class="table table1">
                                    <thead class="insearch">
                                        <tr>
                                            <th>流程</th>
                                            <th>簽署機關</th>
                                            <th>簽署單位</th>
                                            <th>職稱</th>
                                            <th>簽署方法</th>
                                            <th>email通知</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr v-for="(flow,index) in data.Flowlist" :key="index">
                                                <td class="align-middle">{{data.Flowlist.length - index }}</td>
                                                <td class="align-middle" >
                                                    <el-select
                                                        v-model="flow.ApprovingUnitSeq"
                                                        filterable
                                                        allow-create
                                                        default-first-option
                                                        placeholder="請輸入簽署機關鍵字"
                                                        :reserve-keyword="false"
                                                        @change="ApprovingUnitChange(flow, index)"
                                                    >
                                                        <el-option
                                                        v-for="(option, index2) in data.ApprovingUnitList"
                                                        :key="index2"
                                                        :label="option.Name"
                                                        :value="option.Seq"

                                                        />
                                                    </el-select>
                                                </td>

                                                <td class="align-middle" v-if="index < data.Flowlist.length -1">
                                                    <el-select
                                                        v-model="flow.ApproverList.Name"
                                                        filterable
                                                        allow-create
                                                        default-first-option
                                                        placeholder="請輸入單位關鍵字"
                                                        :reserve-keyword="false"
                                                        
                                                    >
                                                        <el-option
                                                        v-for="(option, index2) in typeUnitList[flow.ApprovingUnitSeq]"
                                                        :key="index2"
                                                        :label="option.Text"
                                                        :value="option.Text"

                                                        />
                                                    </el-select>
                                                </td>
                                                <td class="align-middle" v-else>
                                                    <el-select  v-model="flow.ApproverList.Name" disabled />
                        
                                                </td>
                                                <td class="align-middle" v-if="index < data.Flowlist.length -1">
                                                    <el-select
                                                        v-model="flowPositions[data.Flowlist.length - index -1]"
                                                        filterable
                                                        allow-create
                                                        default-first-option
                                                        placeholder="請輸入職稱關鍵字"
                                                        multiple
                                                        :reserve-keyword="false"
                                                        
                                                    >
                                                        <el-option
                                                        v-for="(option, index2) in data.positionList"
                                                        :key="index2"
                                                        :label="option.Text"
                                                        :value="parseInt(option.Value)"

                                                        />
                                                    </el-select>
                                                </td>
                                                <td v-else>
                        
                                                </td>
                                                <td class="align-middle">

                                                    
                                                    <!-- <div class="btn-group btn-group-toggle" data-toggle="buttons">
                                                        <label class="btn btn-primary m-2" for="option1">
                                                            <input type="radio" name="options" id="option1"  v-model="flow.ApprovalMethod" :value="0"> Active
                                                        </label>
                                                        <label class="btn btn-primary m-2"  for="option2">
                                                            <input type="radio" name="options" id="option2" v-model="flow.ApprovalMethod" :value="1"> Radio
                                                        </label>
                                                        <label class="btn btn-primary m-2" for="option3">
                                                            <input type="radio" name="options" id="option3" v-model="flow.ApprovalMethod" :value="2"> Radio
                                                        </label>
                                                        </div> -->
                                                        <div class="form-check form-check-inline">
                                                        <input class="form-check-input" type="radio" :id="`inlineCheckbox4`" :name="`ApprovalMethod${index}`" v-model="flow.ApprovalMethod" :value="4">
                                                        <label class="form-check-label" :for="`inlineCheckbox4{index}`">數位簽章</label>
                                                        </div>
                                                    <div class="form-check form-check-inline">
                                                        <input class="form-check-input" type="radio" :id="`inlineCheckbox1`" :name="`ApprovalMethod${index}`" v-model="flow.ApprovalMethod" :value="0">
                                                        <label class="form-check-label" :for="`inlineCheckbox1${index}`">線上簽名</label>
                                                        </div>
                                                        <div class="form-check form-check-inline">
                                                        <input class="form-check-input" type="radio" :id="`inlineCheckbox2`"  :name="`ApprovalMethod${index}`" v-model="flow.ApprovalMethod" :value="1">
                                                        <label class="form-check-label" :for="`inlineCheckbox2${index}`">套用簽名檔</label>
                                                        </div>
                                                        <div class="form-check form-check-inline">
                                                        <input class="form-check-input" type="radio" :id="`inlineCheckbox3${index}`" :name="`ApprovalMethod${index}`"  v-model="flow.ApprovalMethod" :value="2">
                                                        <label class="form-check-label" :for="`inlineCheckbox3${index}`">兩者皆可</label>
                                                    </div>
                                                </td>
                                                <td  class="align-middle">
                                                    <div class="custom-control custom-switch">
                                                        <input v-model="flow.EmailNotification" type="checkbox" class="custom-control-input" :id="`customSwitch${index}`">
                                                        <label class="custom-control-label" :for="`customSwitch${index}`">通知</label>
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
            <NewSignFormModal  :show="route == 1" @afterInsert="(form) => editFormFlow(form, 2)" @close="route = 0"></NewSignFormModal>
        </div>




</template>


<script>
    import  NewSignFormModal from "./componenets/NewSignFormModal.vue";
    export default{
        watch:{
            dataFilter : {
                handler(value, oldValue)
                {
                    console.log(value);
                    if(value.search == oldValue ) this.getFormList();
                },
                deep:true
            }
        },
        data: () =>{
            return {
                flowPositions : [],
                route : 0,
                edit : null,

                editForm :{

                },
                dataFilter: {
                    page:1,
                    perPage :10,
                    search : ""
                },
                data: {
                    ApprovingUnitList : [],
                    ApproverList : [],
                    Flowlist :[
                        { ApproverList : {Name : "申請人", PositionSeq: 0}, ApproverUnitSeq:23, ApproverMethod:1, EmailNotification : true}
                    ],
                    FormList: [
                        { FormCode: "a1", FormName:"測試表單1" }
                    ],
                },

                typeUnitList: {

                },
                selectUnitName: [],
                selectPositionSeq : []


            }
        },
        components :{
            NewSignFormModal
        },
        methods:{
            ApprovingUnitChange(flow, index)
            {
                if(index < this.data.Flowlist.length - 1) flow.ApproverList = {};
   

            },
            async saveSignFlow()
            {
                this.data.Flowlist.forEach((element, index, arr) => {
                    element.ApprovalWorkFlow = arr.length - index;
                    element.FormCode = this.editForm.FormCode;
                    element.FormName = this.editForm.FormName;
                });
                console.log(this.data.Flowlist);

                let res = (await window.myAjax.post("SignManagement/syncFlowList", {
                    list : this.data.Flowlist,
                    flowPositions : JSON.stringify(this.flowPositions)
                })).data;
                if(res)
                {
                    alert("儲存成功");
                    this.getFlowList();
                }
            },
            addSignFlow()
            {
                this.data.Flowlist.reverse();
                this.data.Flowlist.push({
                    ApproverList : {
                        Name : null
                    }
                });
                this.flowPositions[this.data.Flowlist.length -1] = [];
                this.data.Flowlist.reverse();

            },
            removeSignFlow()
            {
                if(this.data.Flowlist.length == 1)
                {
                    alert("表單至少需要一個流程");
                    return ;
                }
                this.data.Flowlist = this.data.Flowlist.slice(1, this.data.Flowlist.length);
            },
            async editFormFlow(form, route = 0)
            {
                this.editForm = form;
                await this.getFormList();
                await this.getFlowList();
                this.route = route;


            },
            async getFlowList()
            {
                this.data.Flowlist = (await window.myAjax.post("SignManagement/getFlowList", {formCode : this.editForm.FormCode })).data.Flowlist;
                this.data.Flowlist.forEach(e => {
                    e.ApproverList = e.ApproverList ? e.ApproverList : {} 
                    this.flowPositions[e.ApprovalWorkFlow - 1]  = e.ApproverList.Position.map(pos => pos.Seq);
                });
                this.data.Flowlist.reverse();
            },
            async getFormList()
            {
                 Object.assign(this.data, (await window.myAjax.post("SignManagement/getFormList", this.dataFilter)).data);
                 this.typeUnitList = JSON.parse(this.data.typeUnitList)
                console.log("getData", this.data);
                console.log("typeUnitList", this.typeUnitList);

                var eqcUnitList = this.typeUnitList[2];
                eqcUnitList.unshift({ Text : "提案單位主管"});
                eqcUnitList.unshift({ Text : "不限"});
                eqcUnitList = this.typeUnitList[1];
                eqcUnitList.unshift({ Text : "提案單位主管"});
                eqcUnitList.unshift({ Text : "不限"});
                
            },
            async deleteForm(form)
            {
                 (await window.myAjax.post("SignManagement/deleteForm", { formCode : form.FormCode}))
                 this.getFormList();
            },
            async editFormName(form)
            {

                if(this.edit)
                {
                    await window.myAjax.post("SignManagement/updateFormName",{ formCode : form.FormCode, formName : form.FormName })
                    this.edit = null;
                }
                else {
                    this.edit = form.FormCode;
                }
            }
        },
        async mounted(){
            await this.getFormList();
        }
    }
</script>

<style scoped>
.el-input
{
    font-size: 1rem !important;
    font-weight: bolder !important;
}
.el-select-dropdown__item{
    font-size: 1rem !important;
    font-weight: bolder !important;
}
td,  td:first-child{
    text-align: center !important;
}
</style>