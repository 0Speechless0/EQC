<template>

<div>
    <div class="modal-header">
        <h2 id="projectUpload" class="modal-title"  style="margin: inherit;">Step1. 標案清單:&nbsp;{{ title }}</h2>
        <!-- <button type="button" data-dismiss="modal" aria-label="Close"
            class="close"><span aria-hidden="true">×</span></button> -->

    </div>
    <div class="modal-body">
        <div class="row m-2"><label class="my-1 mr-2"> 共 <span
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
        <div class="row m-2 d-flex">


                    <span class="mr-3 align-self-center " v-if="createType == 1">工程年度:</span>
                    <span class="mr-3 align-self-center " v-if="createType == 2">發包年度:</span>
                    <select class="form-control col-5 col-sm-2 m-2" v-model="year">
                                        <option selected="selected" :value="0" > 全部</option>
                                        <option v-for="(option, index) in data.yearOption" :value="option" :key="index">{{ option }}</option>
                                    </select>
   




                    <span class="mr-3 ml-3 align-self-center ">所屬機關:</span>
                    <select class="form-control col-5 col-sm-2 m-2" v-model="subUnit">
                                        <option selected="selected" value="" > 全部</option>
                                        <option v-for="(option, index) in data.unitOption" :value="option" :key="index">{{ option }}</option>
                                    </select>
                                    <span class="mr-3 ml-3 align-self-center ">標案名稱搜尋:</span>
                    <input class="form-control col-5 col-sm-3 m-2" v-model="engNameKeyWord" />
                    <button title="搜尋" class="btn btn-color11-2 btn-xs mx-1 m-2" @click="getTenderList"><i class="fas fa-search"></i> 搜尋 </button>





                                    
        </div>
        <div class="table-responsive">
            <table class="table table-responsive-md table-hover" style="margin: inherit;">
                <thead class="insearch">
                    <tr>
                        <th>標案編號</th>
                        <th>工程名稱</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(tender, index) in data.list" :key="index" @click="selectTender = tender.TreePlantSeq ==0 ? tender : {}">
                        <td class="text-start">{{ tender.EngNo }}</td>
                        <td class="text-start">{{ tender.EngName }}  
                            <font style="color:blue" v-if="tender.TreePlantSeq !=0"> (已新增) </font>
                            <font style="color:red" v-else-if="selectTender.Seq == tender.Seq"> (勾選) </font></td>
                    </tr>

                    
                </tbody>
            </table>
            <div class="container" style="padding: 10px 0 0 0 ;">
                <div class="row">
                    <div class="col">
                    
                    </div>
                    <div class="col">
                    <button role="button" class="btn btn-shadow btn-block btn-color11-0" style="background-color:#74baf3" @click="confirm()">確定</button>
                    </div>
                    <div class="col">
                    
                    </div>
                </div>
                </div>
            <div>
                <!-- <button role="button" class="btn btn-shadow btn-block btn-color11-o" style="background-color:#97dded" onclick="search('searchResult')">查詢</button> -->
            </div>
        </div>
    </div>
</div>
</template>

<script>
import axios from "axios";
export default{
    data : () => {
        return {
            data : {},
            selectTender: {},
            page : 1,
            perPage : 10,
            year : 0,
            subUnit : "",
            engNameKeyWord : null
        }
    },
    props :["createType", "title"],
    emits: ["confirm"],
    watch:{
        
        year :{
            async handler()
            {
                await this.getTenderList(1)
            },
            flush: 'post'
        },
        page :{
            async handler()
            {
                await this.getTenderList()
            },
            flush: 'post'
        },
        perPage :{
            async handler()
            {
                await this.getTenderList()
            },
            flush: 'post'
        },
        subUnit :{
            async handler()
            {
                await this.getTenderList(1)
            },
            flush: 'post'
        }
    },
    
    methods:{
        confirm()
        {

            if(this.selectTender.Seq) this.$emit('confirm', this.selectTender)
        },
        async getTenderList(page = this.page){
            this.page = page;
            this.data = ( await window.myAjax.post("Tree/GetTenderListByUser", {page: page, perPage : this.perPage, year : this.year, createType : this.createType, subUnit:this.subUnit,engNameKeyWord : this.engNameKeyWord }) ).data; 
        }
    },
    computed : {
        pageCount()
        {
            return this.data.count != undefined ? Math.ceil(this.data.count/this.perPage) : 0;
        }
    },
    async mounted()
    {


        await this.getTenderList();
        
    }

}

</script>
<style scoped>
td:first-child {
    text-align: left !important;
}
</style>