<template>
    <div>
        <div class="row d-flex bd-highlight mb-3" >

            <div class="col-12 col-sm-4 col-md-4 col-lg-4 col-xl-2">
                <button @click="onEdit(-1)" role="button" class="btn btn-shadow btn-color11-3 btn-block">
                    <i class="fas fa-plus-square fa-lg"></i> 新增
                </button>
            </div>
            <label class="btn btn-shadow btn-color11-3">
                <input v-on:change="fileChange($event)" id="inputFile" type="file" name="file" multiple="" style="display:none;">
                <i class="fas fa-upload"></i> 批次匯入Excel
            </label>
            <a :href="'ExcelTemplate/ExpertCommittee.xlsx'"  class="bd-highlight" download  style="padding-left: 15px;"> 
                <button  role="button" class="btn btn-shadow btn-color11-3 btn-block">
                     下載範例
                </button>
            </a>
            <a :href="'ExpertCommittee/Download'"  class="bd-highlight" download  style="padding-left: 15px;"> 
                <button  role="button" class="btn btn-shadow btn-color11-3 btn-block">
                     匯出
                </button>
            </a>
            <div class="d-flex pl-3 pb-2"> 
                <input v-model="searchStr" />
                <button type="button" class="btn btn-outline-success" @click="search()" >搜尋</button>
            </div>
            <div class="align-self-end ml-4" style="color:red">
                可輸入委員姓名或主要專長
            </div>
            <div class="ml-auto bd-highlight align-self-center" style="padding-right: 15px;">
                更新日期:{{getDate(lastUpdate, true)}}
            </div>

        </div>
        <div class="row justify-content-between">
            <comm-pagination class="col-12" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        </div>
        <div class="table-responsive">
            <table class="table table-responsive-md">
                <thead class="insearch">
                    <tr>
                        <th><strong>姓名</strong></th>
                        <th><strong>委員種類</strong></th>
                        <th><strong>職稱</strong></th>
                        <th><strong>主要專長</strong></th>
                        <th><strong>聯絡電話</strong></th>
                        <th><strong>E-mail</strong></th>
                        <th class="text-center"><strong>管理</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <!-- 專家01 -->
                    <tr v-for="(item) in items" v-bind:key="item.Seq">
                        <td>{{item.ECName}}</td>
                        <td>{{positionText(item.ECKind)}}</td>
                        <td>{{item.ECPosition}}</td>
                        <td>{{item.ECMainSkill}}</td>
                        <td>{{item.ECTel}}<br>{{item.ECMobile}}</td>
                        <td>{{item.ECEmail}}</td>
                        <td>
                            <div class="d-flex justify-content-center">
                                <button @click="onEdit(item.Seq)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                <button @click="onDel(item.Seq)" class="btn btn-color11-4 btn-xs sharp mx-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                            </div>
                        </td>
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
                recordTotal: 0,
                pageRecordCount : 10,
                pageIndex : 1,
                items: [],
                staticItems: [],
                lastUpdate : null,
                searchStr : null
            };
        },
    
        methods: {
            search(){
                if(this.searchStr){
                    this.items  = this.staticItems.filter(e=> e.ECName.includes(this.searchStr) || e.ECMainSkill.includes(this.searchStr) );
                    this.recordTotal = this.items.length;
                    this.items = this.items.slice(0, this.pageRecordCount);
                
                }
                else{
                    this.recordTotal = this.staticItems.length;
                    this.items = this.staticItems.slice(0, this.pageRecordCount);
                }
            
                 this.pageIndex = 1;
            },
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
            getDate(time, hasTime = false) {

                return Common.ToROCDate(time, hasTime);
            },
            positionText(v) {
                if (v == 1)
                    return '評選委員';
                else if (v == 2)
                    return '督導委員';
                else
                    return '其它';
            },
            getList() {
                window.myAjax.post('/ExpertCommittee/GetList').then(resp => {
                    this.staticItems= resp.data.data;
                    this.recordTotal = this.staticItems.length;
                    this.items = this.staticItems.slice(this.pageRecordCount* (this.pageIndex -1 ), this.pageRecordCount* this.pageIndex);
                    this.lastUpdate = resp.data.lastUpdate;
                    
                }).catch(error => {
                    console.log(error);
                });
            },
            onDel(id) {
                let r = confirm("確定刪除委員資料?");
                if(!r) return;

                window.myAjax.post('/ExpertCommittee/DelCommittee', { seq: id })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getList();
                        } else {
                            alert(resp.data.msg);
                        }
                    }).catch(error => {
                        console.log(error);

                    });
            },
            onEdit(id) {
                window.myAjax.post('/ExpertCommittee/EditCommittee', { seq: id })
                    .then(resp => {
                        console.log(resp.data.Url);
                        window.location.href = resp.data.Url;
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
                window.myAjax.post('/ExpertCommittee/Upload', uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            this.getList();
                        }
                        alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
        },
        async mounted() {
            console.log('mounted() 專家委員維護');
            this.getList();
        }
    }
</script>