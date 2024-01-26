<template>
    <div>
        <div v-if="loading">
            Loading...
        </div>
        <div v-if="!loading && this.op1">
            <div class="row">
                <div class="col-12 col-md-7 col-xl-7 mt-1">
                    <p class="mb-1 mx-2 small-green">* 系統會預設帶出水利署現有(後台)的標準清單，各單位可自行修改或新增</p>
                </div>
                <div class="col-12 col-md-3 col-xl-3 mt-1 flex-row-reverse form-inline" style="padding-right:0px">
                    <input type="text" class="form-control" v-model="code" />代碼:&nbsp;
                </div>
                <div class="col-12 col-md-2 col-xl-2 mt-1 form-inline ">
                    <button v-on:click.stop="importTemplate()" role="button" class="btn btn-shadow btn-color1 btn-block">
                        <i class="fas fa-plus"></i>&nbsp;帶入管理標準
                    </button>
                </div>
            </div>
            <div class="row mt-1" >
                <div class="form-inline" style="margin-left: auto;margin-right: 0;">
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
                <div class="col-2 form-inline" >
                    <button v-on:click.stop="newItem()" role="button" class="btn btn-shadow btn-color1 btn-block">
                        <i class="fas fa-plus"></i>&nbsp;新增項目
                    </button>
                </div>
            </div>
        </div>
        <div class="table-responsive">
            <table class="table table1 min1920 tableLayoutFixed" border="0">
                <thead>
                    <tr>
                        <th class="sort">
                            <div v-if="fCanEdit" class="custom-control custom-checkbox">
                                <input @change="onCheckAllChange($event)" type="checkbox" class="custom-control-input" id="check_All" value="true">
                                <label class="custom-control-label" for="check_All">All</label>
                            </div>
                        </th>
                        <th class="sort">順序</th>
                        <th colspan="2">施工流程</th>
                        <th>管理項目</th>
                        <th>抽查標準</th>
                        <th>抽查時機</th>
                        <th>抽查方法</th>
                        <th>抽查頻率</th>
                        <th>不符合之處置方法</th>
                        <th>管理紀錄</th>
                        <th>型態</th>
                        <!--  th>註記</th -->
                    </tr>
                </thead>

                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq" v-bind:class="{'bg-gray':item.DataType>0}">
                        <td class="text-center">
                            <div class="custom-control custom-checkbox">
                                <input :disabled="!fCanEdit" v-model="item.DataKeep" @change="onDataChange" type="checkbox" class="custom-control-input" v-bind:id="'sort'+index">
                                <label class="custom-control-label" v-bind:for="'sort'+index"></label>
                            </div>
                        </td>
                        <td>
                            <input :disabled="!fCanEdit" v-model.number="item.OrderNo" @change="onDataChange" type="text" class="form-control" />
                            <button v-if="fCanEdit && item.DataType==1" v-on:click.stop="delItem(index, item)" class="mx-0 btn-color3 btn btn-block" title="刪除"><i class="fas fa-trash-alt"></i></button>
                        </td>
                        <td>
                            <select :disabled="!fCanEdit" v-model.number="item.CCFlow1" @change="onDataChange" class="form-control">
                                <option value="1">施工前</option>
                                <option value="2">施工中</option>
                                <option value="3">施工後</option>
                            </select>
                        </td>
                        <td>
                            <textarea :disabled="!fCanEdit" rows="3" maxlength="50" type="text" v-model.trim="item.CCFlow2" @change="onDataChange" class="form-control" />
                        </td>
                        <td>
                            <textarea :disabled="!fCanEdit" rows="3" maxlength="100" type="text" v-model.trim="item.CCManageItem1" @change="onDataChange" class="form-control" />
                        </td>
                        <td>
                            <textarea :disabled="!fCanEdit" rows="3" maxlength="100" type="text" v-model.trim="item.CCCheckStand1" @change="onDataChange" class="form-control" />
                        </td>
                        <td>
                            <textarea :disabled="!fCanEdit" rows="3" maxlength="100" type="text" v-model.trim="item.CCCheckTiming" @change="onDataChange" class="form-control" />
                        </td>
                        <td>
                            <textarea :disabled="!fCanEdit" rows="3" maxlength="100" type="text" v-model.trim="item.CCCheckMethod" @change="onDataChange" class="form-control" />
                        </td>
                        <td>
                            <textarea :disabled="!fCanEdit" rows="3" maxlength="100" type="text" v-model.trim="item.CCCheckFeq" @change="onDataChange" class="form-control" />
                        </td>
                        <td>
                            <textarea :disabled="!fCanEdit" rows="3" maxlength="100" type="text" v-model.trim="item.CCIncomp" @change="onDataChange" class="form-control" />
                        </td>
                        <td>
                            <textarea :disabled="!fCanEdit" rows="3" maxlength="100" type="text" v-model.trim="item.CCManageRec" @change="onDataChange" class="form-control" />
                        </td>
                        <td>
                            <select :disabled="!fCanEdit" v-model.number="item.CCType" @change="onDataChange" class="form-control">
                                <option value="1">文字</option>
                                <option value="2">數字</option>
                                <option value="3">邏輯</option>
                                <option value="4">尺寸</option>
                            </select>
                        </td>
                        <!--  td>
                            <div class="custom-control custom-checkbox">
                                <input :disabled="!fCanEdit" type="checkbox" class="custom-control-input" v-bind:id="'AR'+item.Seq" v-model="item.CCMemo" @change="onDataChange">
                                <label class="custom-control-label" v-bind:for="'AR'+item.Seq">AR</label>
                            </div>
                        </td -->
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
        <div v-if="fCanEdit" class="row justify-content-center">
            <div class="col-12 col-sm-5 col-lg-4 col-xl-3">
                <button v-on:click.stop="saveItems" role="button" class="btn btn-shadow btn-color1 btn-block">
                    儲存
                </button>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['engMain','op1'],
        watch: {
            pageIndex: {
                handler: function (value) {
                    this.getList();
                }
            }
        },
        data: function () {
            return {
                fCanEdit: false,
                editFlag: false,
                loading: false,
                items: [],
                //分頁
                pageIndex: 1,
                perPage: 4,
                totalRows: 0,
                code : null,
            };
        },
        methods: {
            //匯入標準 s20230414
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
                uploadfiles.append("mode", 701);
                uploadfiles.append("file", files[0], files[0].name);
                window.myAjax.post('/QCStdSt/UploadStd', uploadfiles,
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
            //下載標準 s20230414
            dnStdDoc() {
                window.comm.dnFile('/QCStdSt/DnStdDoc?mode=701&id=' + this.op1);
            },
            async importTemplate() {
                window.myAjax.post('/QCStdTp/Chapter701_ImportToSt', {
                    code :this.code,
                    seq : this.op1
                })
                    .then(resp => {
                        if(resp.data.result == 0) {
                            this.getList() 
                        }
                        else {
                            alert("失敗");
                        }

                    })
                    .catch(err => {
                        this.loading = false;
                        console.log(err);
                    });
            },
            onCheckAllChange(event) {
                for (let item of this.items) {
                    item.DataKeep = event.target.checked;
                }
                this.editFlag = true;
            },
            onDataChange(nval, oval) {
                console.log('watch op1 :' + oval + ' >> ' + nval);
                this.editFlag = true;
            },
            getList() {
                if (this.engMain.DocState == -1)
                    this.fCanEdit = true;
                else
                    this.fCanEdit = false;
                this.editFlag = false;
                this.loading = true;
                this.items = [];
                let params = { op1: this.op1, pageIndex: this.pageIndex, perPage: this.perPage };
                window.myAjax.post('/QCStdSt/Chapter701', params)
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
            newItem() {
                window.myAjax.post('/QCStdSt/Chapter701NewItem', { op1: this.op1 })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            var item = resp.data.item.Data;
                            this.items.push(item);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            saveItems() {
                for (let item of this.items) {
                    console.log(item.CCType);
                    if (item.CCFlow1 == null) {/*item.CCType == null s20230906*/
                        alert('[流程] 必須輸入');
                        return;
                    }
                }
                window.myAjax.post('/QCStdSt/Chapter701Save', { items: this.items })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.editFlag = false;
                            this.getList();
                        }
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            delItem(index, item) {
                //item.edit = false;
                if (confirm('是否確定刪除?')) {
                    window.myAjax.post('/QCStdSt/Chapter701Del', { seq: item.Seq })
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
        },
        async mounted() {
            console.log('mounted() 第七章 701 環境保育標準');
            this.getList();
        }
    }
</script>