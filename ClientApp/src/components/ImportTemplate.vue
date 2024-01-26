<template>
  <div>

    <a :href="'ExcelTemplate/' + route + '.xlsx'" :download="route" v-if="!disableImport">
      <p> 下載範例</p>
    </a>

    <div class="d-flex flex-row" v-if="!disableImport">
      <div class="custom-control custom-radio custom-control-inline align-self-center">
        <input type="radio" name="way" id="way1" :value="0" class="custom-control-input" v-model="type" />
        <label for="way1" class="custom-control-label">批次匯入</label>
      </div>
      <div class="custom-control custom-radio custom-control-inline align-self-center">
        <input type="radio" name="way" id="way2" class="custom-control-input" :value="1" v-model="type" />
        <label for="way2" class="custom-control-label">顯示</label>
      </div>



      <div class="ml-auto p-2">
        最近更新:{{ getDate(updateTime, true) }}
      </div>
    </div>

    <div v-show="type == 0 && !disableImport"  >
      
      <ImportExcel :route="route" @afterSuccess="() => { type = 1; getLastUpdateTime() }" />
      <ImportExcelSign :route="route" v-if="!noHint" />
      <div v-else>

        <b-card style="font-size:25px; background-color:#FFF0F5;width: 99%;margin: auto;">
          <b-card-title style="font-size:50px;color:red;">匯入限制</b-card-title>
          <b-card-text></b-card-text>
          <b-card-text>1.格式請完全參照下載範例</b-card-text>
          <b-card-text>2.不可有空資料，空資料請用--取代</b-card-text>
          <b-card-text>3.冒號請使用全形</b-card-text>
          <b-card-text>9.百分比資料長度包括百分比不大於7</b-card-text>
        </b-card>
      </div>
    </div>
    <div v-show="type == 1 || disableImport" class="table-responsive">
      <div class="row justify-content-between mr-2">
        <comm-pagination :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        <div class="d-flex pl-3 pb-2" v-if="hasSearch">
          <select  v-if="searchStrList" v-model="strOption">
            <option v-for="(item, index) in searchStrList " :key="index" :value="item">
              {{ item }}
            </option>
          </select>
          <input v-model="searchStr" class="form-control" />
          <button type="button" class="btn btn-outline-success" @click="search()">搜尋</button>
          <div class="align-self-center ml-4" style="color:red">
            {{ searchSign }}
          </div>
        </div>
      </div>
      <table class="table table-responsive-md table-hover">
        <thead class="insearch">
          <tr>
            <th v-if="hasEdit || hasDelete || detail">#管理</th>
            <!-- <th><strong>#</strong></th> -->
            <th v-for="(field, index) in fields" :key="'A' + index" v-show=" !(detail && detail[field.fieldName])"><strong >{{ field.fieldShowName }}</strong></th>


            <!-- <th class="text-center"><strong>管理</strong></th> -->
          </tr>
        </thead>
        <tbody>
          <tr v-if="hasAdd">
            <td>

            <div class="d-flex justify-content-center">
              <button @click="Add()" class="btn btn-outline-secondary btn-xs sharp m-1" title="新增"><i
                  class="fas fa-plus"></i></button>
            </div>

            </td>
            <td v-for=" (field, index) in fields" :key="index" v-show=" !(detail && detail[field.fieldName])">
              <textarea class="form-control" v-model="addition[field.fieldName]" v-if="field.mutiLine" style="width:100%" row="5" />
              <textarea class="form-control" v-model="addition[field.fieldName]" v-else />
            </td>

          </tr>
          <tr v-for="(item, index) in items" :key="index">
            <td v-if="hasEdit || hasDelete || detail">
              <div class="d-flex justify-content-center">
                <strong>{{ index + 1 }}</strong>
              </div>
              <div class="d-flex justify-content-center" v-if="item.Seq != isEdit">
                <button v-if="hasEdit" @click="Edit(item.Seq)" class="btn btn-color11-1 btn-xs sharp m-1" title="編輯"><i
                    class="fas fa-pencil-alt"></i></button>
                <button v-if="hasDelete" @click="Delete(item)" class="btn btn-color9-1 btn-xs sharp m-1" title="刪除"><i
                    class="fas fa-trash-alt"></i></button>
                <button v-if="detail" class="btn btn-color11-2 btn-xs sharp m-1" @click="showDetail(item)" title="檢視"><i
                    class="fas fa-eye"></i></button>
              </div>
              <div class="d-flex justify-content-center" v-else>
                <button @click="Save(item)" class="btn btn-color11-2 btn-xs sharp m-1" title="儲存"><i
                    class="fas fa-save"></i></button>
                <button @click="isEdit = null" class="btn btn-color9-1 btn-xs sharp m-1" title="取消"><i
                    class="fas fa-times"></i></button>
              </div>

            </td>
            <!-- <td>

            </td> -->
            <td v-for=" (field, index) in fields" :key="'B' + index" v-show=" !(detail && detail[field.fieldName] )">
              <div >
                <div v-if="item.Seq != isEdit  ">
                  <strong v-if="typeof item[field.fieldName] == 'string'
                  && !isNaN(Number(item[field.fieldName]))
                  && item[field.fieldName].includes('0.')">
                    {{ Number(item[field.fieldName]) * 100 }}%
                  </strong>
                  <strong v-else-if="typeof item[field.fieldName] == 'string'
                  && !isNaN(Number(item[field.fieldName]))
                  && item[field.fieldName].length == 7">
                    {{ formatCDateStr(item[field.fieldName]) }}
                  </strong>
                  <strong v-else>
                    {{ item[field.fieldName] }}
                  </strong>
                </div>
                <div v-else>
                  <textarea v-model="item[field.fieldName]" v-if="field.mutiLine" style="width:100%" row="5" />
                  <input class="form-control" v-model="item[field.fieldName]" v-else />

                </div>
              </div>
            </td>



          </tr>

        </tbody>
      </table>

    </div>

  </div>
</template>

<script >
import axios from "axios";
import ImportExcel from "./ImportExcel";
import ImportExcelSign from "./importExcelSign";
import Common from "../Common/Common.js";
import {  watch, reactive,ref } from "vue";
import {store} from "./store/importTemplateStore.js";
export default {
  setup(){

    return {
      store,
    }
  },
  watch: {
    searchCodition : {
      handler(value) {
        if(!this.pollCodition) return;
        
        this.getList(value)


      },
      deep : true,
      
    }

  },
  emits :[
    "showDetail",
    "afterGetList",
  ],
  props: [
    "route",
    "hasAdd",
    "hasDelete",
    "hasEdit",
    "detail",
    "hasSearch",
    "searchSign",
    "searchField",
    "searchCodition",
    "searchStrList",
    "FieldNameChange",
    "noHint",
    "mutiLineFields",
    "detailFields",
    "disableImport",
    "pollCodition"
  ],
  components: {
    ImportExcel,
    ImportExcelSign
  },
  data: () => {
    return {
      searchStr: null,
      isEdit: -1,
      recordTotal: 0,
      pageIndex: 1,
      pageRecordCount: 30,
      type: "1",
      updateTime: null,
      strOption: "",
      staticItems:[],
      _detailFields : {},
      addition: {

      },
      items: [

      ],
      fields: [

      ]

    };
  },

  methods: {
    showDetail(item){

      this.$emit("showModal", item);
    },
    onPaginationChange(pInx, pCount) {
      this.pageRecordCount = pCount;
      this.pageIndex = pInx;
      this.getList();
    },
    search() {
      this.getList();
    },
    getDate(time, hasTime = false) {

      return Common.ToROCDate(time, hasTime);
    },
    formatCDateStr(str) {
      return Common.formatCDateStr(str);
    },
    Edit(Seq) {
      this.isEdit = Seq;
    },
    async Save(item) {
      let res = await window.myAjax.post(`${this.route}/Update`, { m :item });
      console.log("res", res);
      if (res.data) {
        this.isEdit = -1;
      
      }
    },
    async Delete(item) {

      if (!confirm("您確定要刪除嗎?")) return;

      let res = await window.myAjax.post(`${this.route}/Delete`, {id :item.Seq });
      if (res.data) {
        console.log("delete");
        this.getList();
      }
      // this.items = this.items.filter(element => element.Seq != item.Seq);
    },
    async Add() {

      let res = await window.myAjax.post(this.route + "/Add", this.addition);
      if (res.data.status == "success") {
        console.log("add success", res.data.newId);
        this.addition.Seq = res.data.newId;
        this.items.push(this.addition);
        this.isEdit = -1;
      }


    },
    async getList(searchCodition = this.searchCodition) {
      console.log("getList", searchCodition);
      await window.myAjax.post(this.route + "/GetList", {
        
          page: this.pageIndex,
          per_page: this.pageRecordCount,
          keyWord: this.searchStr,
          strOption: this.strOption,
          coditions  : JSON.stringify(searchCodition ?? {})
        
      })
        .then(resp => {
          console.log(this.route, resp.data);
          this.items = resp.data.l;
          this.recordTotal = resp.data.t;
          this.staticItems = this.items;
           this.store.respData = resp.data ;
          console.log("afterGetList", resp.data );
        });
    },
    async getFields() {
      console.log("getFields");
      let fields;
      await window.myAjax.get(this.route + "/getDemandFields")
        .then(resp => {
          console.log("this.route", resp.data);
          fields = resp.data.data;


        });
      let fieldObjects = {};
      fields.forEach(f => {
        fieldObjects[f.fieldName] = f;
        // this.addition[f.fieldName] =null;
      });

      console.log("fields", fields);
      if (this.FieldNameChange) {
        let changeColumn = Object.keys(this.FieldNameChange);
        console.log(fields);


        changeColumn.forEach(e => {
          fieldObjects[e].fieldShowName = this.FieldNameChange[e];
        })
        console.log("fieldObjects", fieldObjects);
      }

      if (this.mutiLineFields) {

        this.mutiLineFields.forEach(f => {
          fieldObjects[f].mutiLine = true;
        })
        console.log("fieldObjects", fieldObjects);
        
      }
      fields =fields.filter((e ) => {
        return !this._detailFields[e.fieldName];
      })
      this.fields = fields;
    },
    async getLastUpdateTime() {

      let res = await window.myAjax.get(this.route + "/getLastUpdateTime");
      this.getList();
      this.updateTime = res.data.lastUpdateTime;
    },
    // filterDataByCodition() {

    //    var coditions = Object.keys(this.searchCodition);
    //    this.items = this.staticItems;
    //    console.log(coditions);
    //    coditions.forEach ( field => {
    //     this.items = this.items.filter( e => {
    //       if(this.searchCodition[field] =="全部" || !this.searchCodition[field]) return true;
          
    //       return e[field] == this.searchCodition[field];
    //     })
    //    })
    // }
  },

  async mounted() {
    console.log(this.searchCodition);
    this._detailFields = this.detailFields ?? {};
    this.getList();
    this.getFields();
    this.disableImport ?? this.getLastUpdateTime();



  },

}

</script>