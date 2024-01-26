<template>
<div>

    <a :href="'ExcelTemplate/'+route+'.xlsx'" :download="route" > 
      <p> #下載範例</p>
  </a>

  <div class="d-flex flex-row">
    <div class="custom-control custom-radio custom-control-inline">
      <input type="radio" name="way" id="way1" value="0" class="custom-control-input" v-model="type" />
      <label for="way1" class="custom-control-label">批次匯入</label>
    </div>
    <div class="custom-control custom-radio custom-control-inline">
      <input
        type="radio"
        name="way"
        id="way2"
        class="custom-control-input"
        value="1"
        v-model="type"
      />
      <label for="way2" class="custom-control-label">顯示</label>
    </div>
    <div class="ml-auto p-2">
        最近更新:{{getDate(updateTime, true)}}
    </div>
  </div>
  <div v-if="type == 0">

      <ImportExcel :route="route" @afterSuccess="() => {type = 1;getLastUpdateTime();}"/>
      <div>

        <b-card   style="font-size:25px; background-color:#FFF0F5;width: 99%;margin: auto;" >
          <b-card-title style="font-size:50px;color:red;">匯入限制</b-card-title>
          <b-card-text></b-card-text>
          <b-card-text>1.格式請完全參照下載範例</b-card-text>
          <b-card-text>2.不可有空資料，空資料請用--取代</b-card-text>
          <b-card-text>3.冒號請使用請使用</b-card-text>
          <b-card-text>9.百分比資料長度包括百分比不大於7</b-card-text>
        </b-card>
      </div>
  </div>
    <div v-else class="table-responsive">
      <table class="table table-responsive-md table-hover">
        <thead class="insearch">
          <tr >
            <th><strong>#</strong></th>
            <th v-for="(field, index) in fields" :key="'A'+index"><strong>{{field.fieldShowName}}</strong></th>

            <!-- <th class="text-center"><strong>管理</strong></th> -->
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, index) in items" :key="index">
            <td>
              <strong>{{ index+1 }}</strong>
            </td>
            <td v-for=" (field, index) in fields" :key="'B'+index">
                <strong 
                v-if=" typeof item[field.fieldName] == 'string' 
                && !isNaN(Number(item[field.fieldName]))
                && item[field.fieldName].includes('0.')">
                    {{Number(item[field.fieldName])*100}}%
                </strong>
                <strong 
                v-else-if=" typeof item[field.fieldName] == 'string' 
                && !isNaN(Number(item[field.fieldName]))
                && item[field.fieldName].length == 7">
                    {{formatCDateStr(item[field.fieldName])}}
                </strong>
                <strong v-else>
                  {{item[field.fieldName]}}
                </strong>

            </td>
          </tr>

        </tbody>
      </table>
      <div style="width:99%; margin-top:50px" class="row justify-content-center" >
          <!--v-on:change="onPageChange"-->
          <b-pagination :total-rows="totalRows"
                        :per-page="perPage"
                        v-model="currentPage">
          </b-pagination>
          <!--<div>
              目前的排序鍵值: <b>{{ sortBy }}</b>, 排序方式:
              <b>{{ sortDesc ? 'Descending' : 'Ascending' }}</b>
          </div>-->
      </div>
    </div>

</div>
</template>

<script >
import axios from "axios";
import ImportExcel from "./ImportExcel";
import Common from "../Common/Common.js";
export default  {
  watch:{
    type:{
      handler: function(value) {this.getList();}
    },
    currentPage:{
      handler : function(value)  {this.getList();}
    }

  },
  props : ["route"],
  components : {
    ImportExcel
  },
  data: () => {
    return {
      isEdit: 0,
      totalRows:0,
      currentPage : 1,
      perPage : 10,
      type : "1",
      addition : {

      },
      items: [

      ],
      fields:[
      "SSS"
      ]
    };
  },

  methods: {
    getDate(time, hasTime=false) {

      return Common.ToROCDate(time, hasTime);
    },
    formatCDateStr(str){
      return Common.formatCDateStr(str);
    },
    Edit(Seq) {
        this.isEdit = Seq;
    },
    async Save(item) {
      let res = await  axios.post(`${this.route}/Update`, item);
      console.log("res", res);
      if(res.data.status == "success"){
        console.log(this.route, res.data.data);
      }
    },
    async Delete(item) {

        if( !confirm("您確定要刪除嗎?") ) return;

        let res = await axios.delete(`${this.route}/Delete/${item.Seq}`);
        if(res.data.status == "success") {
          console.log("delete");
          this.getList();
        }
        // this.items = this.items.filter(element => element.Seq != item.Seq);
    },
    async Add() {

      let res = await axios.post(this.route+"/Add", this.addition);
      if(res.data.status == "success" ) {
        console.log("add success", res.data.newId);
        this.addition.Seq = res.data.newId;
        this.items.push(this.addition);
        this.isEdit = 0;
      }


    },
    getList(){
      console.log("getList");
        axios.get(this.route+"/GetList",{
          params: {
            page : this.currentPage,
            per_page: this.perPage
          }
        })
        .then(resp => {
            console.log(this.route, resp.data);
            this.items = resp.data.l;
            this.totalRows = resp.data.t;
        });
    },
    getFields(){
            console.log("getFields");
            window.myAjax.get(this.route+"/getDemandFields")
        .then(resp => {
            console.log("this.route", resp.data);
            this.fields = resp.data.data;
        });
    },
    async getLastUpdateTime() {

      let res = await axios.get(this.route+"/getLastUpdateTime");
      this.updateTime = res.data.lastUpdateTime;
    }
  },

  async mounted() {
    this.getList();
    this.getFields();
        this.getLastUpdateTime();
  }
}

</script>