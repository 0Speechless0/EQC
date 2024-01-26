<template>
  
    <div>
      <div class="d-flex flex-wrap form-inline">
          <div class="m-2">
              查詢:
            <select   v-model="searchType" class="form-control" >
              <option :value="0">全部</option>
              <option :value="1">
                案由
              </option>
              <option :value="2">
                全文
              </option>
            </select>
          </div>
          <div class="m-2">
              <input class="form-control"  width="200" :disabled="disableInput" value ="" v-model="searchText"/>
          </div>
          <div class="m-2">
            <button type="button" class="btn btn-outline-success" @click="search()">搜尋</button>
          </div>
      </div>
      <ImportTemplate route="CourtVerdict"
         ref="ImportTemplte"
        :disableImport="true"
        :pollCodition="false"
        :searchCodition="{
          Text : searchText,
          _type : searchType
        }"
        :detailFields="
        {
          JFull :true
        }"
        :detail = "true"
       @showModal="showModal" 
        />
            <div class="modal fade text-left show"  style="overflow:auto;background:rgb(0 0 0 / 50%)" v-bind:style="{display: modalShow ? 'block' : 'none'}" data-backdrop="static" data-keyboard="false" tabindex="-1" aria-labelledby="flowchart" aria-hidden="true">
        <div class="modal-dialog modal-dialog-scrollable modal-dialog-centered modal-xl" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="projectUpload">{{modalItem.JTitle }}全文</h5>
                        <button ref="flowChartModalClose" type="button" class="close" data-dismiss="modal" aria-label="Close" v-on:click="closeModal()">
                            <span aria-hidden="true">×</span>
                        </button>
                </div>
                <div class="modal-body">
                    <pre class="bg-gray p-3 mb-5">
                        {{modalItem.JFull}}
                    </pre>

                </div>
            </div>
        </div>
    </div>
    </div>
  </template>
  
  
  <script>
  import ImportTemplate from "../../components/ImportTemplate";
  import axios from "axios";
  export default {
    data: () => {
      return {
        searchText : "",
        searchType : 0,
        modalItem : {},
        modalShow  : false
      }
    },
    watch: {
      searchType :{
        handler() {
          this.searchText = "";
        }
      }
    },
    computed : {

      disableInput() {
        return this.searchType == 0 ;
      }
    },
    methods : {
      search() {
        this.$refs.ImportTemplte.getList();
      },
      showModal(item) {
        this.modalItem = item;
      
        this.modalShow = true;
      },
      closeModal() {
        this.modalShow = false;
      }
    },
    components: {
      ImportTemplate
    }
  }
  </script>