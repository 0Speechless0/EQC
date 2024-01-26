<template>
  <div>

    <a href="ExcelTemplate/NationalSupervisedActivity.xlsx" download="NationalSupervisedActivity" > 
      <p> 下載範例</p>
  </a>

  <div class="mb-2">
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
<div v-if="type == 0">

    <ImportExcel :route="NSActivity/excelUpload" @afterSuccess="() => {type = 1}"/>
    <ImportExcelSign route="NSActivity" />
</div>
  <div v-else class="table-responsive">
    <table class="table table-responsive-md table-hover">
      <thead class="insearch">
        <tr>
          <th><strong>#</strong></th>
          <th><strong>通報案件編號</strong></th>
  <th><strong>通報日期</strong></th>
  <th><strong>通報人</strong></th>
  <th><strong>通報工程</strong></th>
  <th><strong>通報主題</strong></th>
  <th><strong>分文日期</strong></th>
  <th><strong>目前處理機關</strong></th>
  <th><strong>應結案日期</strong></th>
  <th><strong>結案日期</strong></th>
  <th><strong>照片數</strong></th>

          <!-- <th class="text-center"><strong>管理</strong></th> -->
        </tr>
      </thead>
      <tbody>
        <tr v-for="(item, index) in items" :key="index">
          <td>
            <strong>{{ index+1 }}</strong>
          </td>
          <td>
            <strong>{{item.PublicBulletinNo}}</strong>
          </td>
<!--           <td v-else>
            <input v-model="item.PublicBulletinNo"/>
          </td> -->
          <td>
            <strong>{{formatCDateStr(item.PublicBulletinDate)}}</strong>
          </td>
<!--           <td v-else>
            <input v-model="item.PublicBulletinDate"  type="date" />
          </td> -->
          <td>
            <strong>{{item.PublicBulletinPerson}}</strong>
          </td>
<!--           <td v-else>
            <input v-model="item.PublicBulletinPerson"/>
          </td> -->
          <td>
            <strong>{{item.ConstructionName}}</strong>
          </td>
<!--           <td v-else>
            <input v-model="item.ConstructionName"/>
          </td> -->
          <td>
            <strong>{{item.ConstructionSubject}}</strong>
          </td>
<!--           <td v-else>
            <input v-model="item.ConstructionSubject"/>
          </td> -->
          <td>
            <strong>{{formatCDateStr(item.AssignDate)}}</strong>
          </td>
<!--           <td v-else>
            <input :value="getDate(item.AssignDate)" type="date" />
          </td> -->
          <td>
            <strong>{{item.ProcessingECUnit}}</strong>
          </td>
<!--           <td v-else>
            <input v-model="item.ProcessingECUnit"/>
          </td> -->
          <td>
            <strong>{{formatCDateStr(item.ExpectDateCaseClosed)}}</strong>
          </td>
<!--           <td v-else>
            <input v-model="item.ExpectDateCaseClosed" type="date" />
          </td> -->
          <td>
            <strong>{{formatCDateStr(item.ActualDateCaseClosed)}}</strong>
          </td>
<!--           <td v-else>
            <input v-model="item.ActualDateCaseClosed" type="date"  />
          </td> -->
          <td>
            <strong>{{item.PhotoQuantity}}</strong>
          </td>
<!--           <td v-else>
            <input v-model="item.PhotoQuantity" type="number" />
          </td> -->
<!--           <td>

            <a
              v-if="isEdit == item.Seq"
              @click="Save(item)"
              title="儲存"
              class="btn btn-color11-3 btn-xs sharp mx-1"
            >
              <i class="fas fa-save"></i>
            </a>
            <a
              v-else
              @click="Edit(item.Seq)"
              class="btn btn-color11-3 btn-xs sharp mx-1"
              title="編輯"
              ><i class="fas fa-pencil-alt"></i
            ></a>
            <a
              @click="Delete(item)"
              class="btn btn-color11-4 btn-xs sharp mx-1"
              title="刪除"
              ><i class="fas fa-trash-alt"></i
            ></a>
          </td> -->
        </tr>
<!--         <tr>
          <td>
            #
          </td>
          <td>
            <input v-model="addition.PublicBulletinNo"/>
          </td>
          <td>
            <input v-model="addition.PublicBulletinDate" type="date" />
          </td>
          <td>
            <input v-model="addition.PublicBulletinPerson"/>
          </td>
          <td>
            <input v-model="addition.ConstructionName"/>
          </td>
          <td>
            <input v-model="addition.ConstructionSubject"/>
          </td>
          <td>
            <input v-model="addition.AssignDate" type="date"/>
          </td>
          <td>
            <input v-model="addition.ProcessingECUnit"/>
          </td>
          <td>
            <input v-model="addition.ExpectDateCaseClosed" type="date"/>
          </td>
          <td>
            <input v-model="addition.ActualDateCaseClosed" type="date"/>
          </td>
          <td>
            <input v-model="addition.PhotoQuantity" type="number" />
          </td>
          <td>
            <a @click="Add" href="javascript:void(0)" class="btn btn-color11-3 btn-xs sharp mx-1" title="新增">
              <i class="fas fa-plus fa-lg"></i>
            </a>
          </td>
        </tr> -->
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

</div>
</template>

<script >
import axios from "axios";
import ImportExcel from "../../components/ImportExcel";
import ImportExcelSign from "../../components/importExcelSign";
import Common from "../../Common/Common.js";
export default  {
  watch:{
    type:{
      handler: function(value) {this.getList();}
    },
    currentPage:{
      handler : function(value)  {this.getList();}
    }

  },
  components : {
    ImportExcel,
    ImportExcelSign
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
    getDate(time) {

      return Common.ToDate(time);
    },
    formatCDateStr(str){
      return Common.formatCDateStr(str);
    },
    Edit(Seq) {
        this.isEdit = Seq;
    },
    async Save(item) {
      let res = await  axios.post(`NSActivity/Update`, item);
      console.log("res", res);
      if(res.data.status == "success"){
        console.log("NSActivity", res.data.data);
      }
    },
    async Delete(item) {

        if( !confirm("您確定要刪除嗎?") ) return;

        let res = await axios.delete(`NSActivity/Delete/${item.Seq}`);
        if(res.data.status == "success") {
          console.log("delete");
          this.getList();
        }
        // this.items = this.items.filter(element => element.Seq != item.Seq);
    },
    async Add() {

      let res = await axios.post("NSActivity/Add", this.addition);
      if(res.data.status == "success" ) {
        console.log("add success", res.data.newId);
        this.addition.Seq = res.data.newId;
        this.items.push(this.addition);
        this.isEdit = 0;
      }


    },
    getList(){
      console.log("getList");
        axios.get("NSActivity/GetList",{
          params: {
            page : this.currentPage,
            per_page: this.perPage
          }
        })
        .then(resp => {
            console.log("NSActivity", resp.data);
            this.items = resp.data.l;
            this.totalRows = resp.data.t;
        });
    }
  },
  async mounted() {
    this.getList();
  }
}

</script>