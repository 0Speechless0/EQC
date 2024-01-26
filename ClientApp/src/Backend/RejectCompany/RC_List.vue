<template>
    <div>
        <div class="row d-flex bd-highlight mb-3" style="padding-left:10px">
            <label class="btn btn-shadow btn-color11-3">
                <input v-on:change="fileChange($event)" id="inputFile" type="file" name="file" multiple="" style="display:none;">
                <i class="fas fa-upload"></i> 批次匯入Excel
            </label>
            <a :href="'ExcelTemplate/RejectCompany.xls'"  class="bd-highlight" download  style="padding-left: 15px;"> 
                <button  role="button" class="btn btn-shadow btn-color11-3 btn-block">
                     下載範例
                </button>
            </a>
            <div class="d-flex pl-3 pb-2"> 
                <input v-model="searchStr" />
                <button type="button" class="btn btn-outline-success" @click="search()" >搜尋</button>
            </div>
            <div class="align-self-end ml-4" style="color:red">
                可輸廠商名稱查詢
            </div>
            <div class="ml-auto bd-highlight align-self-center" style="padding-right: 15px;">
                更新日期:{{getDate(lastUpdate, true)}}
            </div>

        </div>
        <div class="row justify-content-between">
            <comm-pagination class="col-12" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        </div>
        <div class="table-responsive">
            <table class="table table-responsive-md table-hover">
                <thead class="insearch">
                    <tr>
                        <th><strong>廠商代號</strong></th>
                        <th><strong>廠商名稱</strong></th>
                        <th><strong>標案案號</strong></th>
                        <th><strong>標案名稱</strong></th>
                        <th><strong>拒絕往來生效日</strong></th>
                        <th><strong>拒絕往來截止日</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td>{{item.Corporation_Number}}</td>
                        <td>{{item.Corporation_Name}}</td>
                        <td>{{item.Case_no}}</td>
                        <td>{{item.Case_Name}}</td>
                        <td>{{item.Effective_DateStr}}</td>
                        <td>{{item.Expire_DateStr}}</td>
                    </tr>
                </tbody>
            </table>
        </div>

    </div>
</template>
<script>
import Common from "../../Common/Common.js";
    export default {
        data: function () {
            return {
                items: [],
                staticItems:[],
                lastUpdate: null,
                recordTotal: 0,
                pageIndex: 1,
                pageRecordCount: 30,
                searchStr: null

            };
        },
        watch:{

        },
        methods: {
            onPaginationChange(pInx, pCount) {
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
                    if(this.searchStr) {
                      this.items=   this.staticItems.filter(e=> e.ECName.includes(this.searchStr) || e.ECMainSkill.includes(this.searchStr) );
                        this.items = this.items.slice(this.pageRecordCount* (pInx -1 ), this.pageRecordCount* pInx);
                    }
                    else {
                        this.items = this.staticItems.slice(this.pageRecordCount* (pInx -1 ), this.pageRecordCount* pInx);
                        this.recordTotal = this.staticItems.length;
                    }

            },
            search(){
                if(this.searchStr){
                    this.items  = this.staticItems.filter(e=> e.Corporation_Name.includes(this.searchStr) || e.Case_Name.includes(this.searchStr) );
                    this.recordTotal = this.items.length;
                    this.items = this.items.slice(0, this.pageRecordCount);
                
                }
                else{
                    this.recordTotal = this.staticItems.length;
                    this.items = this.staticItems.slice(0, this.pageRecordCount);
                }
                 this.items.sort((a, b) => a.Case_Name.indexOf(this.searchStr) - b.Case_Name.indexOf(this.searchStr));
                 this.pageIndex = 1;
            },
            getDate(time, hasTime = false) {

                return Common.ToROCDate(time, hasTime);
            },
            //紀錄清單
            getResords() {
                this.items = [];
                window.myAjax.post('/RejectCompany/GetRecords')
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.staticItems = resp.data.items;
                            this.recordTotal = this.staticItems.length;
                            this.items = this.staticItems.slice(this.pageRecordCount* (this.pageIndex -1 ), this.pageRecordCount* this.pageIndex);
                            
                            this.lastUpdate = resp.data.lastUpdate;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //匯入 excel
            fileChange(event) {
                var files = event.target.files || event.dataTransfer.files;
                // 預防檔案為空檔
                if (!files.length) return;

                //application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
                //application/vnd.ms-excel
                if (!files[0].type.match('application/vnd.openxmlformats-officedocument.spreadsheetml.sheet')) {// && !files[0].type.match('application/vnd.ms-excel') ) {
                    alert('請選擇 .xlsx Excel檔案');
                    return;
                }
                var uploadfiles = new FormData();
                uploadfiles.append("file", files[0], files[0].name);
                this.upload(uploadfiles);
            },
            upload(uploadfiles) {
                window.myAjax.post('/RejectCompany/Upload', uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            this.getResords();
                        }
                        alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
        },
        mounted() {
            console.log('mounted() 拒絕往來廠商');
            this.getResords();
    
        }
    }
</script>