<template>
            <div class="card-body" >
                <div id="app">
                    <div>

                        <div class="modal-body">
                            <form class="form-group insearch mb-3" v-if="!record"> 
                                <div class="form-row">
                                    <div class="d-flex col-sm-12 col-md-3 col-lg-1 mt-3">
                                        <input type="number" step="1" style="height:32px" class="form-control "  v-model="selectYear" />
                                    <!-- <span class="m-2" style="color:red"> 清空此項查詢全部</span> -->
                                    </div>
                                    <div class="col-12 col-sm-12 col-md-3 mt-3">
                                        <select class="form-control" v-model="selectUnit">
                                            <option v-for="(option,index) in unitList" :key="index" :value="option.Value">{{ option.Text }}</option>
                                        </select>
                                        
                                    </div>


                                <div class="col-12 col-sm-3 ml-3 mt-3"><button @click="searchAmmeter" type="button" style="color: rgb(255, 255, 255); background-color: rgb(108, 117, 125); border-color: rgb(108, 117, 125); border-radius: 5px;">查詢</button></div>
                                </div>
                            </form>
                            <div v-if="record">

                                <!-- <h5 class="insearch mt-0"> 
                                    設備種類：{{record.MachineType}}<br>
                                    設置地點：{{record.Place}} <br>
                                    容量 : {{record.Capacity}} KW   
                                </h5> -->
                                <table class="table">
                                    <thead>
                                        <tr>
                                            <th scope="col" class="col-4"></th>
                                            <th scope="col" class="col-2">第一季</th>
                                            <th scope="col" class="col-2">第二季</th>
                                            <th scope="col" class="col-2">第三季</th>
                                            <th scope="col" class="col-2">第四季</th>
                                        </tr>

                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td  class="form-inline row justify-content-center borderfix">
                                                <span class="m-1" >第{{record.Session +1 }}季:</span>
                                                <input class="form-control col-2" v-model="record.Record"
                                                        />
                                                <span class="m-1">度</span>
                                                <button @click="RecordAmmeter(record)" href="#" title="紀錄" class="btn btn-shadow btn-color11-3  btn-xs m-1"><i class="fas fa-pencil-alt"></i> 紀錄</button>


                                            </td>
                                            <td >
                                            {{record.Session1}}
                                            </td>

                                            <td >
                                            {{ record.Session2 }}
                                            </td>

                                            <td >
                                            {{ record.Session3 }}
                                            </td>

                                            <td >
                                            {{ record.Session4 }}
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                                <div class="row justify-content-center mt-5">
                                    
                                    <div class="d-flex"><button role="button" @click="record=null" class="btn btn-color9-1 btn-xs mx-1" >
                                    <a class="link" style="color: white;" >回上頁 </a></button>
                                    </div>

                                </div>
                            </div>
                            <div v-else-if="search">
                                <div class="row justify-content-between mb-3 pl-3 pr-3">
                                        <div class="form-inline col-12 col-md-8 small justify-content-start"><label class="my-1 mr-2"> 共 <span
                                                    class="text-danger">{{ data.count}}</span> 筆，每頁顯示 </label><select
                                                class="form-control sort form-control-sm" v-model="dataFilter.perPage" >
                                                <option value="10">10</option>
                                                <option value="20">20</option>
                                                <option value="30">30</option>
                                            </select><label class="my-1 mx-2">筆，共<span
                                                    class="text-danger">{{ data.pageCount }}</span>頁，目前顯示第</label><select
                                                class="form-control sort form-control-sm" v-model="dataFilter.page">
                                                <option  v-for="n in data.pageCount" :value="n" :key="n"> {{n}} </option>
                                            </select><label class="my-1 mx-2">頁</label>
                                        </div>
                                        <button @click="NewAmmeterRecord" role="button" class="btn btn-outline-secondary btn-xs mx-1 "><i class="fas fa-plus"></i>&nbsp;新增紀錄 </button>
                                    </div>
                                <table class="table" >
                                <thead>
                                    <!-- <tr>
                                    <th scope="col">
                                        <button type="button" class="btn btn-outline-secondary" @click="NewAmmeterRecord" >+</button>
                                    </th>
                                    <th scope="col" rowspan="1"> </th>
                                    <th scope="col" rowspan="1"> </th>
                                    <th scope="col" rowspan="1" > </th>
                                    <th scope="col" rowspan="1" > </th>
                                    <th scope="col" rowspan="2" align="center" class="important">
                                        紀錄
                                    </th>
                                    
                                    <th scope="col" class="text-center" colspan="4" width="500">發電量(度)</th>
                                    </tr> -->
                                        <tr>
                                    <th scope="col">#</th>
                                    <th scope="col" rowspan="1"> 設備種類</th>
                                    <th scope="col" rowspan="1"> 設置地點</th>
                                    <th scope="col" rowspan="1" > 容量(KW)</th>
                                    <th scope="col" rowspan="1"> 管理</th>
                                    <th scope="col" v-if="!edit" >

                                        
                                    </th>
                                    <!-- <th scope="col">第一季</th>
                                    <th scope="col">第二季</th>
                                        <th scope="col">第三季</th>
                                        <th scope="col">第四季</th> -->

                                    </tr>
                                </thead>
                                <tbody  >
                                <tr v-for="(item, index) in data.list" :key="index">
                                    <th scope="row">{{ index+1 }}</th>
                                    <td v-if="edit != index">{{ item.MachineType }}</td>
                                    <td v-else>
                                        <select  type="text" class="form-control" v-model="item.MachineType">
                                            <option value="太陽能">
                                                太陽能
                                            </option>    
                                            <option value="小水利">
                                                小水利
                                            </option>
                                        </select>
                                    </td>
                                    <td v-if="edit != index">{{ item.Place }}</td>
                                    <td v-else>
                                        <input  type="text" class="form-control" v-model="item.Place"/>
                                    </td>
                                    <td v-if="edit != index">{{ item.Capacity }}</td>
                                    <td v-else>
                                        <input type="number" class="form-control" v-model="item.Capacity"/>
                                    </td>
                                    <td class="justify-content-center borderfix">
                                 
                                        <button v-if="edit != index" title="編輯" class="btn btn-color11-3 btn-xs sharp mx-1" @click="edit = index "><i class="fas fa-pencil-alt"></i></button>
                                        <button v-else @click="EditAmmeterRecord(item)" title="編輯" class="btn btn-color11-2 btn-xs sharp mx-1" ><i class="fas fa-save"></i></button>
                                        <button v-if="edit != index" @click="DeleteAmmeterRecord(item.Seq)" title="刪除" class="btn btn-color11-4 btn-xs sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                                        <button v-if="edit != index" @click="CopyAmmeterRecord(item)" title="複製" class="btn btn-color11-2 btn-xs sharp mx-1"><i class="fas fa-copy"></i></button>

                                    </td>
                                    <td v-if="!edit">
                                        <button @click="record = item" href="#" title="紀錄" class="btn btn-shadow btn-color11-3  btn-xs m-1"><i class="fas fa-pencil-alt"></i> 紀錄</button>
                                    </td>
                                            <!-- <td v-if="item.Session == 4 || edit == index">

                                            </td>
                                        <td v-else class="form-inline row justify-content-center borderfix">
                                            <span class="m-1" >第{{item.Session +1 }}季:</span>
                                            <input class="form-control col-2" v-model="item.Record"
                                                    />
                                            <span class="m-1">度</span>
                                            <button @click="RecordAmmeter(item)" href="#" title="編輯" class="btn btn-shadow btn-color11-3  btn-xs m-1"><i class="fas fa-pencil-alt"></i> 紀錄</button>

                                        
                                        </td>
                                <td >
                                    {{item.Session1}}
                                </td>

                                <td >
                                    {{ item.Session2 }}
                                </td>

                                <td >
                                    {{ item.Session3 }}
                                </td>

                                <td >
                                    {{ item.Session4 }} -->
                                <!-- </td> -->


                                    </tr> 

                                </tbody>

                            </table>

                            </div>


                        </div>
                    </div>
                </div>
            </div>

</template>
<style scoped>
    th, td{
    text-align:center !important;
    vertical-align:middle !important;
    }
    .borderfix{
        /* border-top : none !important; */
    }
    .important {
    font-size: 28px !important;
    }
</style>
<script>
export default{
    watch : {
        dataFilter:{
            handler()
            {
                this.getData()
            },
            flush :"post",
            deep : true
        }
    },
    data : () => {
        return {
            search : false,
            edit: null,
            record : null,
            data : {
                count : 0,
                list : [],
                pageCount: 0,
            },
            selectUnit:"",
            selectYear: null ,
            dataFilter : {
                page : 1,
                perPage : 30
            },
            unitList : [],
            year : 0,
            items: [
                {
                    Capacity : 0,
                    Place : "sdsd",
                    MachineType : "A",
                    Session1 : 11,
                    Session2 : null,
                    Session3 : null,
                    Session4 : null,
                },
                {
                    Capacity : 0,
                    Place : "assdsds",
                    MachineType : "B",
                    Session1 : 11,
                    Session2 : null,
                    Session3 : null,
                    Session4 : null,
                }
            ]
        }
    },
    methods:{
        async searchAmmeter()
        {
            await this.getData();
            console.log(this.data);
            this.search = true;
            this.page = 1;
        },
        async getData()
        {
            this.data = (await window.myAjax.post("AmmeterRecord/GetAmmeter", {
                page : this.dataFilter.page,
                perPage : this.dataFilter.perPage,
                selectUnit : this.selectUnit,
                selectYear :this.selectYear
            }) ).data 
        },
        async CopyAmmeterRecord(item)
        {
            await window.myAjax.post("AmmeterRecord/NewAmmeter",{year : this.selectYear, unitSeq : this.selectUnit, m: item});
            await this.getData();
        }, 
        async NewAmmeterRecord()
        {
            await window.myAjax.post("AmmeterRecord/NewAmmeter",{year : this.selectYear, unitSeq : this.selectUnit});
            await this.getData();
            this.edit = 0;
        },
        async DeleteAmmeterRecord(seq)
        {
            await window.myAjax.post("AmmeterRecord/DeleteAmmeter", {id : seq})
            this.getData();
        },
        async EditAmmeterRecord(item)
        {
            await window.myAjax.post("AmmeterRecord/EditAmmeter",{ m : item})
            this.edit = null;
            this.getData();
        },
        async RecordAmmeter(item)
        {
            item[`Session${item.Session+1}`] =  item.Record;
            await window.myAjax.post("AmmeterRecord/RecordAmmeter", {m:item});
            this.getData();
        }
    },
    async mounted()
    {
        // var userInfo = (await myAjax.post("Users/GetUserInfo", {userSeq : localStorage.getItem("userSeq")}) ).data;
        this.unitList = (await window.myAjax.post("Unit/GetUnitList", { parentSeq : null })).data;

        this.selectYear = new Date().getFullYear() - 1911;
    }
}

</script>