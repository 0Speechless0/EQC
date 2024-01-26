<template>

<div>
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
      <label for="way2" class="custom-control-label">手動新增方式</label>
    </div>
  </div>
  <div v-if="type == 0">
    <ImportExcel route="CarbonEmission/excelUpload" @afterSuccess="() => {type = 1}"/>
    <ImportExcelSign route="CarbonEmission" />
  </div>
  <div v-else class="table-responsive">
    <table class="table table-responsive-md table-hover">
      <thead class="insearch">
        <tr>
          <th><strong>項次</strong></th>
          <th><strong>編碼</strong></th>
          <th><strong>工作項目</strong></th>
          <th><strong>碳排係數(kgCO2e)</strong></th>
          <th><strong>單位</strong></th>
          <th class="text-center"><strong>管理</strong></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(item, index) in items" :key="index">
          <td>
            <strong>{{ index+1 }}</strong>
          </td>
          <td  v-if="isEdit != item.Seq">{{ item.Code }}</td>
          <td v-else>
            <input v-model="item.Code" />
          </td>
          <td v-if="isEdit != item.Seq">{{ item.Item }}</td>
          <td v-else>
            <input v-model="item.Item" />
          </td>
          <td v-if="isEdit != item.Seq" class="text-right">{{ item.KgCo2e }}</td>
            <td v-else>
                <input v-model="item.KgCo2e" type="number" />
            </td>
          <td v-if="isEdit != item.Seq">{{ item.Unit }}</td>
            <td v-else>
                <input v-model="item.Unit" />
            </td>
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
        </tr>
        <tr>
          <td>
            #
          </td>
          <td>
            <input v-model="inputCode" />
          </td>
          <td>
            <input v-model="inputItem"/>
          </td>
          <td>
            <input v-model="inputKgCo2e" type="number">
          </td>
          <td>
            <input v-model="inputUnit">
          </td>
          <td>
            <a @click="Add" href="javascript:void(0)" class="btn btn-color11-3 btn-xs sharp mx-1" title="新增">
              <i class="fas fa-plus fa-lg"></i>
            </a>
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
<script>
    import axios from 'axios';
    import ImportExcel from "../../components/ImportExcel.vue";
    import ImportExcelSign from "../../components/importExcelSign.vue";
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
      inputCode : null,
      inputItem : null,
      InputKgCo2e : null,
      InputUnit : null,
      items: [

      ],
      fields:[
      "SSS"
      ]
    };
  },
  methods: {
    Edit(Seq) {
        this.isEdit = Seq;
    },
    async Save(item) {
      let res = await  axios.post(`CarbonEmission/Update`, item);
      console.log("res", res);
      if(res.data.status == "success"){
        console.log("CarbonEmission", res.data.data);
      }
    },
    async Delete(item) {

        if( !confirm("您確定要刪除嗎?") ) return;

        let res = await axios.delete(`CarbonEmission/Delete/${item.Seq}`);
        if(res.data.status == "success") {
          console.log("delete");
          this.getList();
        }
        // this.items = this.items.filter(element => element.Seq != item.Seq);
    },
    async Add() {
      let item = {
          Code : this.inputCode,
          Item : this.inputItem,
          KgCo2e : this.inputKgCo2e,
          Unit : this.inputUnit
      }
      let res = await axios.post("CarbonEmission/Add", item);
      if(res.data.status == "success" ) {
        console.log("add success", res.data.newId);
        item.Seq = res.data.newId;
        this.items.push(item);
        this.isEdit = 0;
        this.inputCode = "";
        this.inputItem = "";
        this.InputKgCo2e = "";
        this.InputUnit = "";
      }


    },
    getList(){
      console.log("getList");
        axios.get("CarbonEmission/GetList",{
          params: {
            page : this.currentPage,
            per_page: this.perPage
          }
        })
        .then(resp => {
            console.log("CarbonEmission", resp.data);
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
