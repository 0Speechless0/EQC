<template>
    <div>
        <div v-if="loading">
            Loading...
        </div>
        <div v-if="!loading && this.op1" class="row justify-content-between">
            <div class="col mt-3">
                <label for="tableinfo">點選新增項目，新增資訊</label>
            </div>
            <div class="col-12 col-md-4 col-xl-2 mt-3">
                <a v-on:click.stop="newItem()" role="button" class="btn btn-outline-light btn-block" style="color:black">
                    <i class="fas fa-plus"></i>&nbsp;&nbsp;新增項目
                </a>
            </div>
            <div class="form-inline">
                <button @click="dnStdDoc" role="button" class="btn btn-shadow btn-color11-1 btn-block">
                    <i class="fas fa-download"></i>&nbsp;下載標準
                </button>
            </div>
            <div class="form-inline">
                <label class="btn btn-shadow btn-color11-2 btn-block">
                    <input v-on:change="uploadSchProgress($event)" type="file" name="file" multiple="" style="display:none;">
                    &nbsp;匯入標準<i class="fas fa-upload"></i>
                </label>
            </div>
        </div>
        <div class="table-responsive">
            <table class="table table1 min1920 tableLayoutFixed" border="0">
                <thead>
                    <tr>
                        <th class="sort">順序</th>
                        <th>抽驗項目</th>
                        <th>管理標準</th>
                        <th>檢驗時機</th>
                        <th class="twoInput">檢驗方法</th>
                        <th>檢驗頻率</th>
                        <th>不符合之處置方法</th>
                        <th>管理紀錄</th>
                        <th>型態</th>
                        <th>註記</th>
                        <th>編輯</th>
                    </tr>
                </thead>

                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.id" v-show="!editFlag || item.edit">
                        <td>
                            <div v-if="!item.edit">{{item.OrderNo}}</div>
                            <input v-if="item.edit" type="text" v-model.number="item.OrderNo" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.EPCheckItem1}}</div>
                            <input maxlength="50" v-if="item.edit" type="text" v-model.trim="item.EPCheckItem1" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.EPStand1}}</div>
                            <textarea rows="5" maxlength="100" v-if="item.edit" type="text" v-model.trim="item.EPStand1" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.EPCheckTiming}}</div>
                            <input maxlength="100" v-if="item.edit" type="text" v-model.trim="item.EPCheckTiming" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.EPCheckMethod}}</div>
                            <textarea rows="5" maxlength="100" v-if="item.edit" type="text" v-model.trim="item.EPCheckMethod" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.EPCheckFeq}}</div>
                            <textarea rows="5" maxlength="100" v-if="item.edit" type="text" v-model.trim="item.EPCheckFeq" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.EPIncomp}}</div>
                            <textarea rows="5" maxlength="100" v-if="item.edit" type="text" v-model.trim="item.EPIncomp" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.EPManageRec}}</div>
                            <input maxlength="100" v-if="item.edit" type="text" v-model.trim="item.EPManageRec" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{getTypeText(item.EPType)}}</div>
                            <select v-if="item.edit" v-model.number="item.EPType" class="form-control">
                                <option value="1">文字</option>
                                <option value="2">數字</option>
                                <option value="3">邏輯</option>
                                <option value="4">尺寸</option>
                            </select>
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.EPMemo==true ? 'AR' : ''}}</div>
                            <div v-if="item.edit" class="custom-control custom-checkbox">
                                <input type="checkbox" class="custom-control-input" id="AR" v-model="item.EPMemo">
                                <label class="custom-control-label" for="AR">AR</label>
                            </div>
                        </td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <a v-on:click.stop="editItem(item)" v-if="!item.edit" href="#" class="btn btn-color11-2 btn-xs m-1" title="編輯"><i class="fas fa-pencil-alt"></i> 編輯</a>
                                <a v-on:click.stop="saveItem(item)" v-if="item.edit" href="#" class="btn btn-color11-3 btn-xs m-1" title="儲存"><i class="fas fa-save"></i> 儲存</a>
                                <a v-on:click.stop="delItem(index, item)" href="#" class="btn btn-color9-1 btn-xs m-1" title="刪除"><i class="fas fa-trash-alt"></i> 刪除</a>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>

        </div>
        <div style="width:99%;" class="row justify-content-center">
            <b-pagination :total-rows="totalRows"
                          :per-page="perPage"
                          v-model="pageIndex">
            </b-pagination>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['op1'],
        watch: {
            'op1': function (nval, oval) {
                console.log('watch op1 :' + oval + ' >> ' + nval);
                if (this.op1 != null) { this.getList(); }
            },
            pageIndex: {
                handler: function (value) {
                    this.getList();
                }
            }
        },
        data: function () {
            return {
                editFlag: false,
                loading: false,
                items: [],
                //分頁
                pageIndex: 1,
                perPage: 3,
                totalRows: 0,
            };
        },
        methods: {
            //匯入標準 s20230415
            uploadSchProgress(event) {
                var files = event.target.files || event.dataTransfer.files;
                // 預防檔案為空檔
                if (!files.length) return;
                if (!files[0].type.match('application/vnd.openxmlformats-officedocument.spreadsheetml.sheet')) {
                    alert('請選擇 .xlsx Excel檔案');
                    return;
                }
                var uploadfiles = new FormData();
                uploadfiles.append("id", this.op1);
                uploadfiles.append("mode", 6);
                uploadfiles.append("file", files[0], files[0].name);
                window.myAjax.post('/QCStdTp/UploadStd', uploadfiles,
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
            //下載標準 s20230415
            dnStdDoc() {
                window.comm.dnFile('/QCStdTp/DnStdDoc?mode=6&id=' + this.op1);
            },
            getTypeText(value) {
                if (value == 1) {
                    return '文字';
                } else if (value == 2) {
                    return '數字';
                } else if (value == 3) {
                    return '邏輯';
                } else if (value == 4) {//s20231016
                    return '尺寸';
                } else {
                    return '';
                }
            },
            getList() {
                this.editFlag = false;
                this.loading = true;
                this.items = [];
                let params = { op1: this.op1, pageIndex: this.pageIndex, perPage: this.perPage };
                window.myAjax.post('/QCStdTp/Chapter6', params)
                    .then(resp => {
                        this.items = resp.data.items;
                        this.totalRows = resp.data.pTotal;
                        this.loading = false;
                    })
                    .catch(err => {
                        this.loading = false;
                        console.log(err);
                    });
            },
            editItem(item) {
                if (this.editFlag) return;

                this.editFlag = true;
                item.edit = this.editFlag;
            },
            newItem() {
                if (this.editFlag) return;
                this.editFlag = true;
                var newRow = {
                    Seq: -1 * Date.now(),
                    EquOperTestTpSeq: this.op1,
                    EPCheckItem1:'', EPCheckItem2:'',
                    EPStand1: '', EPStand2: '', EPStand3: '', EPStand4: '', EPStand5: '',
                    EPCheckTiming: '',
                    EPCheckMethod: '',
                    EPCheckFeq: '',
                    EPIncomp: '',
                    EPManageRec: '',
                    EPType: 1,
                    EPMemo: false,
                    EPCheckFields: 0,
                    EPManageFields: 0,
                    OrderNo: 999,
                    edit: true
                };
                this.items.push(newRow);
            },
            saveItem(item) {
                if (item.OrderNo === '') {
                    alert('[順序]欄位須輸入資料');
                    return;
                }
                //console.log(item);
                if (item.Seq < 0) {
                    window.myAjax.post('/QCStdTp/Chapter6Add', { item: item })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                item.Seq = resp.data.Seq;
                                item.edit = false;
                                this.editFlag = false;
                            }
                            this.sortItems();
                            alert(resp.data.message);
                            console.log(resp);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                } else {
                    window.myAjax.post('/QCStdTp/Chapter6Save', { item: item })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                item.edit = false;
                                this.editFlag = false;
                            }
                            this.sortItems();
                            alert(resp.data.message);
                            console.log(resp);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            delItem(index, item) {
                //item.edit = false;
                console.log('index: ' + index + ' seq: ' + item.Seq);
                if (item.Seq < 0) {
                    this.items.splice(index, 1);
                    this.editFlag = false;
                }
                else if (confirm('是否確定刪除?')) {
                    window.myAjax.post('/QCStdTp/Chapter6Del', { seq: item.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                if (item.edit) this.editFlag = false;
                                this.items.splice(index, 1);
                            } else {
                                alert(resp.data.message);
                            }
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            sortItems() {
                this.items.sort(function (a, b) {
                    var nameA = a.OrderNo; // ignore upper and lowercase
                    var nameB = b.OrderNo; // ignore upper and lowercase
                    console.log('nameA: ' + nameA + ' nameB: ' + nameB);
                    if (nameA < nameB) {
                        return -1;
                    }
                    if (nameA > nameB) {
                        return 1;
                    }
                    return 0;
                });
            }
        },
        async mounted() {
            console.log('mounted() 第六章 設備功能運轉測試抽驗程序及標準');
        }
    }
</script>