<template>
    <div v-if="targetId > 0">
        <h5 class="insearch mt-0 py-2">
            工程編號：{{tenderItem.TenderNo}}({{tenderItem.EngNo}})<br>工程名稱：{{tenderItem.TenderName}}({{tenderItem.EngName}})
        </h5>
        <div class="table-responsive">
            <table class="table table-responsive-md table-hover">
                <thead>
                    <tr class="insearch">
                        <th><strong>核定日期</strong></th>
                        <th><strong>文件說明</strong></th>
                        <th><strong>上傳檔案</strong></th>
                        <th class="text-center"><strong>功能</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <template v-if="item.Seq != editSeq">
                            <td><strong>{{item.ApprovedDateStr}}</strong></td>
                            <td>{{item.Descr}}</td>
                            <td>{{item.FileName}}</td>
                            <td>
                                <div class="d-flex justify-content-center">
                                    <button @click="onEditRecord(item)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                    <button @click="download(item)" class="btn btn-color11-1 btn-xs sharp mx-1" title="下載"><i class="fas fa-download"></i></button>
                                    <button @click="onDelRecord(item)" class="btn btn-color9-1 btn-xs sharp mx-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                                </div>
                            </td>
                        </template>
                        <template v-if="item.Seq == editSeq">
                            <td><strong>
                                <input v-model.trim="editRecord.ApprovedDateStr" type="date" class="form-control">
                                </strong>
                            </td>
                            <td>
                                <input v-model.trim="editRecord.Descr" type="text" class="form-control" id="fileInfo" placeholder="請輸入文件說明">
                            </td>
                            <td>
                                <b-form-file v-model="editFile" placeholder="未選擇任何檔案" ></b-form-file>      
                            </td>
                            <td>
                                <div class="d-flex justify-content-center">
                                    <button @click="onSaveRecord" class="btn btn-color11-2 btn-xs sharp mx-1"><i class="fas fa-save"></i></button>
                                    <button @click="editSeq = -99" class="btn btn-color9-1 btn-xs sharp mx-1" title="取消"><i class="fas fa-times"></i></button>
                                </div>
                            </td>
                        </template>
                    </tr>
                    <tr>
                        <td><strong><input v-model="newRecord.approvedDate" type="date" class="form-control" value=""></strong></td>
                        <td>
                            <input v-model="newRecord.desc" type="text" class="form-control" id="fileInfo" placeholder="請輸入文件說明" value="">
                        </td>
                        <td>
                            <b-form-file v-model="newFile" placeholder="未選擇任何檔案"></b-form-file>
                        </td>
                        <td>
                            <div class="d-flex justify-content-center">
                                <button @click="onNewRecord" class="btn btn-outline-secondary btn-xs sharp mx-1" title="新增"><i class="fas fa-plus fa-lg"></i></button>
                            </div>
                        </td>
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
                targetId: null,
                tenderItem: {},
                items: [],

                editSeq: -99,
                editRecord: {},
                editFile: null,
                //new Item
                newRecord: { approvedDate: null, desc: null },
                newFile: null,
            };
        },
        methods: {
            //新增
            onNewRecord() {
                if (this.strEmpty(this.newRecord.approvedDate) || this.strEmpty(this.newRecord.desc) || this.newFile == null) {
                    alert('核定日期, 文件說明, 上傳檔案 必須輸入!');
                    return;
                }
                var uploadfiles = new FormData();
                uploadfiles.append("EngMainSeq", this.targetId);
                uploadfiles.append("Seq", -1);
                uploadfiles.append("ApprovedDate", this.newRecord.approvedDate);
                uploadfiles.append("Descr", this.newRecord.desc);
                uploadfiles.append("file", this.newFile, this.newFile.name);
                this.upload(uploadfiles);
            },
            strEmpty(str) {
                return window.comm.stringEmpty(str);
            },
            //儲存
            onSaveRecord() {
                if (this.strEmpty(this.editRecord.ApprovedDateStr) || this.strEmpty(this.editRecord.Descr)) {
                    alert('核定日期, 文件說明 必須輸入!');
                    return;
                }
                var uploadfiles = new FormData();
                uploadfiles.append("EngMainSeq", this.targetId);
                uploadfiles.append("Seq", this.editRecord.Seq);
                uploadfiles.append("ApprovedDate", this.editRecord.ApprovedDateStr);
                uploadfiles.append("Descr", this.editRecord.Descr);
                if (this.editFile != null) uploadfiles.append("file", this.editFile, this.editFile.name);
                this.upload(uploadfiles);
            },
            //編輯紀錄
            onEditRecord(item) {
                if (this.editSeq > -99) return;

                this.editFile = null;
                this.editRecord = Object.assign({}, item);
                this.editSeq = this.editRecord.Seq;
            },
            //刪除紀錄
            onDelRecord(item) {
                if (confirm('是否確定刪除?')) {
                    window.myAjax.post('/EQMConstRiskEval/DelRecord', { id:item.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getResords();
                            }
                            alert(resp.data.msg);
                        }).catch(error => {
                            console.log(error);
                        });
                }
            },
            //上傳
            upload(uploadfiles) {
                window.myAjax.post('/EQMConstRiskEval/UpdateRecord', uploadfiles,
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
            //下載
            download(item) {
                window.myAjax.get('/EQMConstRiskEval/Download?id=' + item.Seq, { responseType: 'blob' })
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
            //清單
            getResords() {
                this.editSeq = -99;
                this.items = [];
                this.editFile = null;
                this.newFile = null;
                window.myAjax.post('/EQMConstRiskEval/GetList', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //取標案
            getItem() {
                if (this.targetId == null) {
                    alert('請先選取標案');
                    return;
                }
                window.myAjax.post('/EQMConstRiskEval/GetEngMain', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.tenderItem = resp.data.item;
                            this.getResords();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        async mounted() {
            console.log('mounted() 設計階段施工風險評估');
            this.targetId = window.sessionStorage.getItem(window.eqSelTrenderPlanSeq);
            this.getItem();
        }
    }
</script>
