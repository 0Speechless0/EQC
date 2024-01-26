<template>
    <div>
        <h5 class="insearch mt-0 py-2">
            <div class="form-row mb-1">
                <div class="col-12 col-sm-4 col-md-2 mb-1">
                    估驗請款日
                </div>
                <table>
                    <tr>
                        <td>
                            <input v-model="addDate" type="date" class="form-control">
                        </td>
                        <td>
                            <button @click="onAddAskPayment" v-bind:disabled="addIsDisabled" role="button" class="btn btn-shadow btn-color11-3 btn-block">
                                新增<i class="fas fa-plus"></i>
                            </button>
                        </td>
                    </tr>
                </table>
            </div>
        </h5>
        <!-- Nav tabs -->
        <ul class="nav nav-tabs" role="tablist">
            <li v-for="(item, index) in dateList" v-bind:key="item.SPDateStr" class="nav-item">
                <a @click="onChangeDate(item)" v-bind:id="'tab'+item.Seq" class="nav-link" data-toggle="tab" href="#menu01">{{item.APDateStr}}</a>
            </li>
        </ul>
        <!-- Tab panes -->
        <div class="tab-content">
            <div class="form-row" role="toolbar">
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                    <button @click="downloadAdj" v-bind:disabled="isDisabled" type="button" class="btn btn-outline-secondary btn-sm" title="檔案下載">下載excel &nbsp;<i class="fas fa-download"></i></button>
                </div>
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                    <label class="btn btn-block btn-outline-secondary btn-sm" v-bind:class="{ 'disabled' : isDisabled}">
                        <input v-on:change="uploadSchProgress($event)" v-bind:disabled="isDisabled" type="file" name="file" multiple="" style="display:none;">
                        上傳請款資料&nbsp;<i class="fas fa-upload"></i>
                    </label>
                </div>
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                    <button @click.stop="onDelItem" v-bind:disabled="isDisabled" type="button" class="btn btn-outline-secondary btn-sm" title="刪除請款">刪除請款 &nbsp;<i class="fas fa-times"></i></button>
                </div>
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                    <button @click.stop="fillCompleted" v-bind:disabled="isDisabled" type="button" class="btn btn-outline-secondary btn-sm" title="請款完成">請款完成 &nbsp;<i class="fas fa-check"></i></button>
                </div>
                <div v-if="activeItem != null" class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                    <button @click="downloadInvoice" v-bind:disabled="activeItem.APState==0" type="button" class="btn btn-outline-secondary btn-sm">估驗請款單 <i class="fas fa-download"></i></button>&nbsp;
                </div>
            </div>
            <div id="menu01" class="tab-pane active">
                <div class="table-responsive">
                    <table class="table table-responsive-md table-hover">
                        <thead class="insearch">
                            <tr>
                                <th><strong>序號</strong></th>
                                <th><strong>項次</strong></th>
                                <th><strong>施工項目</strong></th>
                                <th class="text-right"><strong>單位</strong></th>
                                <th class="text-right"><strong>單價</strong></th>
                                <th class="text-right"><strong>契約數量</strong></th>
                                <th class="text-right"><strong>金額</strong></th>
                                <th class="text-right"><strong>累計完成量</strong></th>
                                <th class="text-right"><strong>累計價值</strong></th>
                                <th class="text-right"><strong>備註</strong></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in items" v-bind:key="item.Seq" class="bg-1-30">
                                <td><strong>{{item.OrderNo}}</strong></td>
                                <td><strong>{{item.PayItem}}</strong></td>
                                <td><strong>{{item.Description}}</strong></td>
                                <td>{{item.Unit}}</td>
                                <td class="text-right"><strong>{{item.Price}}</strong></td>
                                <td class="text-right"><strong>{{item.Quantity}}</strong></td>
                                <td class="text-right"><strong>{{item.Amount}}</strong></td>
                                <td><input v-bind:disabled="item.ItemType==-1" v-model.number="item.AccuQuantity" type="text" class="form-control text-right"></td>
                                <td><input v-bind:disabled="item.ItemType==-1" v-model.number="item.AccuAmount" type="text" class="form-control text-right"></td>
                                <td><input v-bind:disabled="item.ItemType==-1" v-model="item.Memo" maxlength="100" type="text" class="form-control"></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div v-if="activeItem != null" class="card-footer">
                <div class="row justify-content-center">
                    <div v-if="activeItem.APState==0" class="col-12 col-sm-4 col-xl-2 my-2">
                        <button v-on:click="onChangeDate(activeItem)" role="button" class="btn btn-shadow btn-color3 btn-block"> 取消修改 </button>
                    </div>
                    <div v-if="activeItem.APState==0" class="col-12 col-sm-4 col-xl-2 my-2">
                        <button v-on:click="onSaveClick" role="button" class="btn btn-shadow btn-block btn-color11-4">儲存 <i class="fas fa-save"></i></button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['tenderItem'],
        data: function () {
            return {
                dateList: [],
                addDate: '',
                activeItem: null,
                items: [],
            };
        },
        methods: {
            //新增請款日
            onAddAskPayment() {
                if (this.addDate == '') {
                    alert('請選取日期');
                    return;
                }
                if (this.dateList.length > 0) {
                    var i = 0;
                    for (i = 0; i < this.dateList.length; i++) {
                        if (this.dateList[i].APState == 0) {
                            let item = this.dateList[i];
                            alert('請款日: ' + item.ItemDate + ' 尚未設定 [請款完成], 不能再新增請款日');
                            document.getElementById('tab' + item.Seq).click();
                            return;
                        }
                    }
                }
                window.myAjax.post('/ECAskPayment/AddAskPayment', { id: this.tenderItem.Seq, tarDate: this.addDate })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getDateList();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //日期清單
            getDateList() {
                this.activeItem = null;
                this.items = [];
                window.myAjax.post('/ECAskPayment/GetDateList', { id: this.tenderItem.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.dateList = resp.data.items;
                            if (this.dateList.length > 0) {
                                this.$nextTick(function () {
                                    document.getElementById('tab' + this.dateList[0].Seq).click();
                                });
                            }
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //取得清單
            onChangeDate(item) {
                this.activeItem = null;
                this.items = [];
                window.myAjax.post('/ECAskPayment/GetList', { id: item.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.activeItem = item;
                            this.items = resp.data.items;
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //刪除請款日
            onDelItem() {
                if (confirm('刪除 請款日:' + this.activeItem.ItemDate + ' 資料?\n是否確定? ')) {
                    window.myAjax.post('/ECAskPayment/DelAPItem', { id: this.activeItem.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getDateList();
                            }
                            alert(resp.data.msg);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            //填報完成
            fillCompleted() {
                if (confirm('請款日:' + this.activeItem.ItemDate + '已完成請款, 之後將不能修改予刪除\n是否確定? ')) {
                    window.myAjax.post('/ECAskPayment/APCompleted', { id: this.activeItem.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getDateList();
                            }
                            alert(resp.data.msg);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            //儲存
            onSaveClick() {
                window.myAjax.post('/ECAskPayment/APSave', {
                    id: this.activeItem.Seq,
                    items: this.items
                })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.onChangeDate(this.activeItem);
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //下載請款調整
            downloadAdj() {
                this.download('/ECAskPayment/DownloadAdj?id=' + this.activeItem.Seq);
            },
            //下載估驗請款單
            downloadInvoice() {
                this.download('/ECAskPayment/DownloadInvoice?id=' + this.activeItem.Seq);
            },
            download(action) {
                window.myAjax.get(action, { responseType: 'blob' })
                    .then(resp => {
                        const blob = new Blob([resp.data]);
                        const contentType = resp.headers['content-type'];
                        if (contentType.indexOf('application/json') >= 0) {
                            //alert(resp.data);
                            const reader = new FileReader();
                            reader.addEventListener('loadend', (e) => {
                                const text = e.srcElement.result;
                                const data = JSON.parse(text)
                                alert(data.message);
                            });
                            reader.readAsText(blob);
                        } else if (contentType.indexOf('application/blob') >= 0) {
                            var saveFilename = null;
                            const data = decodeURI(resp.headers['content-disposition']);
                            var array = data.split("filename*=UTF-8''");
                            if (array.length == 2) {
                                saveFilename = array[1];
                            } else {
                                array = data.split("filename=");
                                saveFilename = array[1];
                            }
                            if (saveFilename != null) {
                                const url = window.URL.createObjectURL(blob);
                                const link = document.createElement('a');
                                link.href = url;
                                link.setAttribute('download', saveFilename);
                                document.body.appendChild(link);
                                link.click();
                            } else {
                                console.log('saveFilename is null');
                            }
                        } else {
                            alert('格式錯誤下載失敗');
                        }
                    }).catch(error => {
                        console.log(error);
                    });
            },
            
            //匯入(excel)各工項 累計預定進度%
            uploadSchProgress(event) {
                var files = event.target.files || event.dataTransfer.files;
                // 預防檔案為空檔
                if (!files.length) return;
                if (!files[0].type.match('application/vnd.openxmlformats-officedocument.spreadsheetml.sheet')) {// && !files[0].type.match('application/vnd.ms-excel') ) {
                    alert('請選擇 .xlsx Excel檔案');
                    return;
                }
                var uploadfiles = new FormData();
                uploadfiles.append("id", this.activeItem.Seq);
                uploadfiles.append("file", files[0], files[0].name);
                this.upload(uploadfiles, 'UploadAccu');
            },
            upload(uploadfiles, action) {
                window.myAjax.post('/ECAskPayment/' + action, uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            this.getDateList();
                        }
                        alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
        },
        computed: {
            isDisabled: function () {
                if (this.activeItem == null || this.activeItem.APState != 0) return true;
                return false;
            },
            addIsDisabled: function () {
                if (this.addDate == '') return true;
                return false;
            }
        },
        mounted() {
            console.log('mounted 工程變更-估驗請款');
            this.getDateList();
        }
    }
</script>
<style>
    .labelDisabled {
        opacity: .65;
    }
</style>