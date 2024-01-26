<template>
    <div class="tab-pane">
        <h5>評選者請填列</h5>
        <div>
            <div :class="['m-2 p-2', file ? '' : 'btn-color3']">
                <div v-if="!file" :class="['dropZone', dragging ? 'dropZone-over' : '']"
                     @dragestart="dragging = true"
                     @dragenter="dragging = true"
                     @dragleave="dragging = false">
                    <div class="dropZone-info" @drag="onChange">
                        <span class="fa fa-cloud-upload dropZone-title"></span>
                        <span class="dropZone-title">拖拉檔案至此區塊 或 點擊此處</span>
                        <div class="dropZone-upload-limit-info">
                            <!-- div>許可附屬檔名: xml</div -->
                        </div>
                    </div>
                    <input type="file" @change="onChange" />
                </div>
                <div v-if="file" class="form-row justify-content-center">
                    <div class="dropZone-uploaded">
                        <div class="dropZone-uploaded-info">
                            <span class="dropZone-title">選取的檔案: {{ file.name }}</span>
                            <div class="uploadedFile-info">
                                <button @click="removeFile" type="button" class="btn btn-color9-1 btn-xs mx-1">
                                    <i class="fas fa-times"></i> 取消選取
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div v-if="file" class="row justify-content-center my-3">
                <button @click="uploadXML(0)" type="button" class="btn btn-color11-2 btn-xs mx-1">
                    <i class="fas fa-upload"></i> 確認
                </button>
            </div>
        </div>
        <div class="table-responsive mt-4">
            <table class="table table-responsive-md table-hover">
                <thead class="insearch">
                    <tr>
                        <th><strong>項次</strong></th>
                        <th><strong>委員姓名</strong></th>
                        <th><strong>內/外部委員</strong></th>
                        <th><strong>出席</strong></th>
                        <th><strong>職業</strong></th>
                        <th><strong>學經歷</strong></th>
                        <th class="text-center"><strong>管理</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td>{{index+1}}</td>
                        <template v-if="item.Seq != editSeq">

                            <td>{{item.CName}}</td>
                            <td>{{item.Kind==1 ? '外部' : '內部'}}</td>
                            <td>{{item.IsPresence ? '出席' : ''}}</td>
                            <td>{{item.Profession}}</td>
                            <td><pre>{{item.Experience}}</pre></td>
                            <td>
                                <div class="d-flex justify-content-center">
                                    <button @click="onEditRecord(item)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                    <button @click="onDelRecord(item)" class="btn btn-color11-4 btn-xs sharp mx-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                                </div>
                            </td>
                        </template>
                        <template v-if="item.Seq == editSeq">
                            <td><input v-model="editRecord.CName" maxlength="50" type="text" class="form-control" value=""></td>
                            <td>
                                <div class="custom-control custom-radio custom-control-inline">
                                    <input v-model="editRecord.Kind" value="1" type="radio" id="editRecordW1" class="custom-control-input">
                                    <label for="editRecordW1" class="custom-control-label">外部</label>
                                </div>
                                <div class="custom-control custom-radio custom-control-inline">
                                    <input v-model="editRecord.Kind" value="2" type="radio" id="editRecordW2" class="custom-control-input">
                                    <label for="editRecordW2" class="custom-control-label">內部</label>
                                </div>
                            </td>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="editRecord.IsPresence" value="true" type="checkbox" name="IsPresence" id="IsPresence" class="custom-control-input">
                                <label for="IsPresence" class="custom-control-label">出席</label>
                            </div>
                            <td><input v-model="editRecord.Profession" maxlength="100" type="text" class="form-control" value=""></td>
                            <td>
                                <textarea v-model="editRecord.Experience" maxlength="500" rows="5" class="form-control"></textarea>
                            </td>
                            <td>
                                <div class="d-flex justify-content-center">
                                    <button @click="onUpdateItem" class="btn btn-color11-2 btn-xs sharp mx-1"><i class="fas fa-save"></i></button>
                                    <button @click="editSeq = -99" class="btn btn-color9-1 btn-xs sharp mx-1" title="取消"><i class="fas fa-times"></i></button>
                                </div>
                            </td>
                        </template>
                    </tr>
                    <tr>
                        <td></td>
                        <td><input v-model="newItem.CName" maxlength="50" type="text" class="form-control" value=""></td>
                        <td>
                            <div class="custom-control custom-radio custom-control-inline">
                                <input v-model="newItem.Kind" value="1" type="radio" id="W1" class="custom-control-input">
                                <label for="W1" class="custom-control-label">外部</label>
                            </div>
                            <div class="custom-control custom-radio custom-control-inline">
                                <input v-model="newItem.Kind" value="2" type="radio" id="W2" class="custom-control-input">
                                <label for="W2" class="custom-control-label">內部</label>
                            </div>
                        </td>
                        <div class="custom-control custom-checkbox custom-control-inline">
                            <input v-model="newItem.IsPresence" value="true" type="checkbox" name="newItemIsPresence" id="newItemIsPresence" class="custom-control-input">
                            <label for="newItemIsPresence" class="custom-control-label">出席</label>
                        </div>
                        <td><input v-model="newItem.Profession" maxlength="100" type="text" class="form-control" value=""></td>
                        <td>
                            <textarea v-model="newItem.Experience" maxlength="500" rows="5" class="form-control"></textarea>
                        </td>
                        <td>
                            <div class="d-flex justify-content-center">
                                <button @click="onAddItem" class="btn btn-color11-3 btn-xs sharp mx-1" title="新增"><i class="fas fa-plus fa-lg"></i></button>
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
        props: ['engMain'],
        data: function () {
            return {
                file: null,//{ name: null, size: null },
                files: null,
                dragging: false,

                items: [],
                newItem: { PrjXMLSeq: this.engMain.PrjXMLSeq, CName: '', IsPresence:false ,Kind: 1, Profession: '', Experience: '' },
                editSeq: -99,
                editRecord: {},
            };
        },
        methods: {
            //刪除委員
            onDelRecord(item) {
                if (confirm('是否確定刪除?')) {
                    window.myAjax.post('/EPCTender/DelCommittee', { id: item.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getList();
                            }
                            alert(resp.data.msg);
                        }).catch(error => {
                            console.log(error);
                        });
                }
            },
            //編輯委員
            onEditRecord(item) {
                if (this.editSeq > -99) return;

                this.editRecord = Object.assign({}, item);
                this.editSeq = this.editRecord.Seq;
            },
            //更新委員
            onUpdateItem() {
                window.myAjax.post('/EPCTender/UpdateCommittee', { item: this.editRecord })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getList();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //新增委員
            onAddItem() {
                window.myAjax.post('/EPCTender/AddCommittee', { item: this.newItem })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getList();
                            this.newItem = { PrjXMLSeq: this.engMain.PrjXMLSeq, CName: '', IsPresence:false, Kind: 1, Profession: '', Experience: '' };
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            getList() {
                this.editSeq = -99;
                window.myAjax.post('/EPCTender/GetCommitteeList', { id: this.engMain.PrjXMLSeq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //xml 檔案上傳處裡
            onChange(e) {
                // 判斷拖拉上傳或點擊上傳的 event
                var upfile = e.target.files || e.dataTransfer.files;

                // 預防檔案為空檔
                if (!upfile.length) {
                    this.dragging = false;
                    return;
                }
                var file = upfile[0];
                if (!file.type.match('text/xml')) { // 附檔名判斷
                    alert('請選擇 xml 檔案');
                    this.dragging = false;
                    return;
                }

                this.createFile(file);
            },
            createFile(file) {
                this.file = file;
                this.dragging = false;
                this.files = new FormData();
                this.files.append("id", this.engMain.Seq);
                this.files.append("file", this.file, this.file.name);
            },
            uploadXML() {
                const files = this.files;
                window.myAjax.post('/EPCTender/UploadCommitteeXML', files,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            alert(resp.data.message);
                            this.removeFile();
                            this.getList();
                        } else {
                            alert(resp.data.message);
                        }
                    }).catch(error => {
                        console.log(error);
                    });
            },
            removeFile() {
                this.file = '';
                this.files = new FormData();
            },
        },
        async mounted() {
            console.log('mounted() 委員資料');
            this.getList();
        }
    }
</script>