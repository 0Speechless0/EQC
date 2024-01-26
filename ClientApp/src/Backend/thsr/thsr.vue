<template>
  
  <div>
    <div class="d-flex flex-wrap form-inline">
      <div class="m-2"> 
        車向:
        <select  v-if="directionOptions" v-model="searchCodition.DirectionText" class="form-control"  >
          <option v-for="(item, index) in directionOptions " :key="index" :value="item">
            {{ item }}
          </option>
        </select>
      </div>
      <div class="m-2"> 
        起站:
        <select  v-if="respData.startOptions" v-model="searchCodition.StartStationName" class="form-control" @change="selectEnd = ''">
          <option value="" selected="selected">
            全部
          </option>
          <option v-for="(item, index) in respData.startOptions" :key="index" :value="item" >
            {{ item }}
          </option>
        </select>
      </div>
      <div class="m-2 "> 
        迄站:
        <select   v-if="respData.endOptions" v-model="searchCodition.EndingStationName" class="form-control" >
          <option value="" selected="selected">
            全部
          </option>
          <option v-for="(item, index) in respData.endOptions " :key="index" :value="item">
            {{ item }}
          </option>
        </select>
      </div>
      <!-- <div class="m-2 ">
        <button type="button" class="btn btn-primary" @click="$refs.Thsr.getList()">搜尋</button>
      </div> -->
      <!-- <div > 
        <button class="btn btn-success align-top" @click="search" >搜尋</button>
      </div> -->
    </div>
    <ImportTemplate route="Thsr" 
      :disableImport="true"
      :pollCodition="true"
      :searchCodition="searchCodition"
      />
  </div>
</template>


<script>
import ImportTemplate from "../../components/ImportTemplate";
import {store} from "../../components/store/importTemplateStore";
import { computed } from "vue";
import axios from "axios";
export default {
  setup(){
    const respData = computed(() => store.respData);
    return {
      store,
      respData
    }
  },
  data : () => {
    return  {
      searchStrList : [ "ddd", "FFF"],
      directionOptions : [ "北上", "南下"],
      searchCodition : {
        StartStationName : "",
        EndingStationName : "",
        DirectionText : ""
      }

    }
  },
  watch:{
    selectStation: {
      handler(n, o)
      {
        console.log("ddd")
      }
      
    },
    selectEnd: {
      handler()
      {

      }
      
    }
  },
  components: {
    ImportTemplate
  },
  methods: {

  },
  mounted (){
    
    // this.GetStationList();
  }
}
</script>