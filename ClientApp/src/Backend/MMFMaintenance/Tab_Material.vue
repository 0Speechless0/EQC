<template>
    <div>
        <h5 style="margin: inherit;">材料維護功能</h5><p></p>
        <div class="row d-flex bd-highlight mb-3" style="padding-left:10px">
            <label class="btn btn-shadow btn-color11-3">
                <input v-on:change="fileChange($event)" id="inputFile" type="file" name="file" multiple="" style="display:none;">
                批次匯入Excel
            </label>
            <a :href="'ExcelTemplate/材料機具燃料維護-材料.xlsx'" class="bd-highlight" download style="padding-left: 15px;">
                <button role="button" class="btn btn-shadow btn-color11-3 btn-block">
                    下載範例
                </button>
            </a>
            <div class="d-flex pl-3 pb-2">
                <input v-model="searchStr" />
                <button type="button" class="btn btn-outline-success" style="color: #fff;background-color: #6c757d;" @click="search()">查詢</button>
            </div>
            <div class="align-self-center ml-2" style="color:red">
                可輸入名稱及規格
            </div>
            <div class="ml-auto bd-highlight align-self-center" style="padding-right: 15px;">
                <button @click="download" class="btn btn-color11-1 btn-block" style="color: rgb(255, 255, 255);background-color: #4472C4;">
                    下載Excel
                </button>
            </div>
        </div>
        <div class="row d-flex bd-highlight mb-3" style="padding-left:10px">
            <div class="ml-auto bd-highlight align-self-center" style="padding-right: 15px;">
                更新日期:{{lastUpdate}}
            </div>
        </div>
        <div class="row justify-content-between">
            <comm-pagination class="col-12" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        </div>

        <div class="table-responsive">
            <table class="table table-responsive-md table-hover VA-middle">
                <thead class="insearch">
                    <tr>
                        <th>ID</th>
                        <th>類別</th>
                        <th>名稱及規格</th>
                        <th>單位碳排量</th>
                        <th>單位</th>
                        <th>備註(參考來源)</th>
                        <!-- th class="text-center">管理</th -->
                        <th>
                            <div class="d-flex justify-content-center">
                                <a v-on:click.stop="fAddItem=true" href="##" class="btn btn-color11-3 btn-xs sharp mx-1" title="新增"><i class="fas fa-plus"></i></a>
                            </div>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-if="fAddItem">
                        <td></td>
                        <td><input v-model.trim="newItem.Kind" maxlength="50" type="text" class="form-control"></td>
                        <td><input v-model.trim="newItem.NameSpec" maxlength="200" type="text" class="form-control"></td>
                        <td><input v-model.number="newItem.KgCo2e" type="number" class="form-control"></td>
                        <td><input v-model.trim="newItem.Unit" maxlength="30" type="text" class="form-control"></td>
                        <td><input v-model.trim="newItem.Memo" maxlength="100" type="text" class="form-control"></td>
                        <td style="min-width: unset;">
                            <div class="d-flex justify-content-center">
                                <button @click="onSaveRecord(newItem)" class="btn btn-outline-secondary btn-xs sharp m-1" title="新增"><i class="fas fa-save"></i></button>
                                <button @click="fAddItem=false" class="btn btn-outline-secondary btn-xs sharp m-1" title="取消"><i class="fas fa-times"></i></button>
                            </div>
                        </td>
                    </tr>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td style="min-width: unset;">{{item.Seq}}</td>
                        <template v-if="item.Seq != editSeq">
                            <td>{{item.Kind}}</td>
                            <td>{{item.NameSpec}}</td>
                            <td>{{item.KgCo2e}}</td>
                            <td>{{item.Unit}}</td>
                            <td>{{item.Memo}}</td>
                            <td style="min-width: unset;">
                                <div class="d-flex justify-content-center">
                                    <button @click="onEditRecord(item)" class="btn btn-color11-1 btn-xs sharp m-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                    <button @click="onDelRecord(item)" class="btn btn-color9-1 btn-xs sharp m-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                                </div>
                            </td>
                        </template>
                        <template v-if="item.Seq == editSeq">
                            <td><input v-model.trim="editRecord.Kind" maxlength="50" type="text" class="form-control"></td>
                            <td><input v-model.trim="editRecord.NameSpec" maxlength="200" type="text" class="form-control"></td>
                            <td><input v-model.number="editRecord.KgCo2e" type="number" class="form-control"></td>
                            <td><input v-model.trim="editRecord.Unit" maxlength="30" type="text" class="form-control"></td>
                            <td><input v-model.trim="editRecord.Memo" maxlength="100" type="text" class="form-control"></td>
                            <td style="min-width: unset;">
                                <div class="d-flex justify-content-center">
                                    <button @click="onSaveRecord(editRecord)" class="btn btn-color11-2 btn-xs sharp m-1" title="儲存"><i class="fas fa-save"></i></button>
                                    <button @click="onEditCancel" class="btn btn-color9-1 btn-xs sharp m-1" title="取消"><i class="fas fa-times"></i></button>
                                </div>
                            </td>
                        </template>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                items: [],
                editSeq: -99,
                editRecord: {},
                newItem: {},
                fAddItem: false,
                //分頁
                recordTotal: 0,
                pageIndex: 1,
                pageRecordCount: 30,
                lastUpdate: null,
                searchStr:null
            };
        },
        methods: {
            //下載碳排表
            download() {
                window.myAjax.get('/MMFMaintenance/dnMaterial', { responseType: 'blob' })
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
            search() {
                this.getResords();
            },
            onNewRecord() {
                window.myAjax.post('/MMFMaintenance/NewMaterialRecord')
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.newItem = resp.data.item;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //刪除紀錄
            onDelRecord(item) {
                if (this.editSeq > -99) return;
                if (confirm('是否確定刪除資料？')) {
                    window.myAjax.post('/MMFMaintenance/DelMaterialRecord', { id: item.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getResords();
                            }
                            alert(resp.data.msg);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            strEmpty(str) {
                return window.comm.stringEmpty(str);
            },
            //儲存
            onSaveRecord(uItem) {
                //console.log(uItem);
                if (this.strEmpty(uItem.Kind) || this.strEmpty(uItem.NameSpec) || this.strEmpty(uItem.Unit) || !window.comm.isNumber(uItem.KgCo2e) ) {
                    alert('類別,名稱及規格,單位碳排量,單位 必須輸入!');
                    return;
                }
                window.myAjax.post('/MMFMaintenance/UpdateMaterialRecord', { m: uItem })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.editSeq = -99;
                            this.getResords();
                            if (uItem.Seq == -1) this.onNewRecord();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //取消編輯
            onEditCancel() {
                this.editSeq = -99;
                this.getResords();
            },
            //編輯紀錄
            onEditRecord(item) {
                if (this.editSeq > -99) return;
                this.editRecord = Object.assign({}, item);
                this.editSeq = this.editRecord.Seq;
            },
            //紀錄清單
            getResords() {
                this.fAddItem = false;
                this.items = [];
                window.myAjax.post('/MMFMaintenance/GetMaterialRecords', {
                        pageRecordCount: this.pageRecordCount,
                        pageIndex: this.pageIndex,
                        keyWord : this.searchStr ?? ""
                    })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                            this.recordTotal = resp.data.pTotal;
                            this.lastUpdate = resp.data.lastUpdate;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //分頁
            onPaginationChange(pInx, pCount) {
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
                this.getResords();
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
                window.myAjax.post('/MMFMaintenance/MaterialUpload', uploadfiles,
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
            console.log('mounted() 材料-維護功能');
            this.getResords();
            this.onNewRecord();
        }
    }
</script>