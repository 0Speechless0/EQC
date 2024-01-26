<template>
    <div class="modal fade show" id="MyDialog" ref="MyDialog" style="background:rgb(0 0 0 / 50%)" v-bind:style="{display: modalShow ? 'block' : 'none'}" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
        <div class="modal-dialog modal-dialog-scrollable modal-dialog-centered modal-xl" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h6 class="modal-title redText" id="projectUpload">定稿計畫上傳</h6>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close" v-on:click="closeModal()">
                        <span aria-hidden="true">×</span>
                    </button>
                </div>
                <div class="modal-body">
                    <table v-if="targetItem.DocState==-1" class="table table2" border="0">
                        <tbody>
                            <tr>
                                <th class="col-2">檔案上傳</th>
                                <td>
                                    <div>
                                        <div :class="['m-2 p-2', file ? '' : 'btn-color3']">
                                            <div v-if="!file" :class="['dropZone ', dragging ? 'dropZone-over' : '']"
                                                    @dragestart="dragging = true"
                                                    @dragenter="dragging = true"
                                                    @dragleave="dragging = false">
                                                <div class="dropZone-info align-self-center" @drag="onFileChange">
                                                    <span class="dropZone-title" style="margin-top:0px;">拖拉檔案至此區塊 或 點擊此處</span>
                                                </div>
                                                <input type="file" @change="onFileChange" />
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
                                            <button v-on:click.stop="uploadPlan()" class="btn btn-outline-secondary btn-xs mx-1" role="button">
                                                <i class="fas fa-plus"></i> 新增版次
                                            </button>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <th>預算書核定文號</th>
                                <td><input v-model.trim="approveNo" maxlength="20" type="text" class="form-control" placeholder="尚未輸入"></td>
                            </tr>
                            <tr>
                                <th>簡述</th>
                                <td><input v-model="memo" maxlength="50" type="text" class="form-control" /></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="table-responsive">
                        <table class="table table1" border="0">
                            <thead>
                                <tr>
                                    <th class="sort">版次</th>
                                    <th>時間</th>
                                    <th>預算書核定文號</th>
                                    <th>簡述</th>
                                    <th width="80">編輯</th>
                                    <th width="80">檔案</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(item, index) in revisionList" v-bind:key="item.Seq">
                                    <td>{{item.VersionNo}}</td>
                                    <td>{{item.showModifyTime}}</td>
                                    <template v-if="!item.edit">
                                        <td>{{item.ApproveNo}}</td>
                                        <td>{{item.Memo}}</td>
                                        <td><a v-on:click.stop="editItem(item)" v-if="(index==0)" href="#" class="btn btn-color11-3 btn-xs mx-1" title="編輯"><i class="fas fa-pencil-alt"></i> 編輯</a></td>
                                    </template>
                                    <template v-if="item.edit">
                                        <td><input v-model="item.ApproveNo" maxlength="20" type="text" class="form-control" ></td>
                                        <td><input v-model="item.Memo" maxlength="50" type="text" class="form-control"></td>
                                        <td><a v-on:click.stop="saveItem(item)" href="#" class="btn btn-color11-2 btn-xs mx-1" title="儲存"><i class="fas fa-save"></i> 儲存</a></td>
                                    </template>
                                    <!-- td>
                                        <div v-if="!item.edit">{{item.ApproveNo}}</div>
                                        <input v-if="item.edit" type="text" class="form-control" v-model="item.ApproveNo">
                                    </td>
                                    <td>
                                        <div v-if="!item.edit">{{item.Memo}}</div>
                                        <input v-if="item.edit" type="text" class="form-control" v-model="item.Memo">
                                    </td>
                                    <td>
                                        <div v-if="targetItem.DocState==-1" class="row justify-content-center m-0">
                                            <a v-on:click.stop="editItem(item)" v-if="!item.edit && (index==0)" href="#" class="btn btn-color11-3 btn-xs mx-1" title="編輯"><i class="fas fa-pencil-alt"></i> 編輯</a>
                                            <a v-on:click.stop="saveItem(item)" v-if="item.edit" href="#" class="btn btn-color11-2 btn-xs mx-1" title="儲存"><i class="fas fa-save"></i> 儲存</a>
                                        </div>
                                    </td -->
                                    <td>
                                        <div class="row justify-content-center m-0">
                                            <a v-on:click.stop="download(item, 0)" href="#" class="btn btn-color11-1 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> 下載 </a>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    export default{
        props: ["targetItem", "modalShow"],
        emits: ["closeModal"],
        data : () => {
            return {
                dragging: false,
                revisionList :[],
                //targetItem: {},
                memo: '',
                approveNo: '',
                file: null,
                files: new FormData()
            }
        },
        watch:{
            modalShow : {
                handler(value) {
                    if (value == true) {
                        this.getRevisionList(); //this.targetItem = this.item;
                    } else {
                        this.memo = '';
                        this.approveNo = '';
                        this.file = null;
                    }
                    //console.log("modalShow:", value);
                }
            }
        },
        methods:{
            removeFile() {
                this.file = '';
                this.files = new FormData();
            },
            onFileChange(e) {
                // 判斷拖拉上傳或點擊上傳的 event
                var files = e.target.files || e.dataTransfer.files;

                // 預防檔案為空檔
                if (!files.length) {
                    this.dragging = false;
                    return;
                }

                this.createFile(files[0]);
            },
            closeModal() {
                this.$emit("closeModal");
            },
            getRevisionList() {
                window.myAjax.post('/SupervisionPlan/RevisionList', { seq: this.targetItem.Seq })
                    .then(resp => {
                        this.revisionList = resp.data;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            editItem(item) {
                if (this.editFlag) return;

                this.editFlag = true;
                item.edit = this.editFlag;
            },
            saveItem(item) {
                if (window.comm.stringEmpty(item.Memo)) {
                    alert('須輸入資料');
                    return;
                }
                window.myAjax.post('/SupervisionPlan/RevisionSave', { item: item })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            item.edit = false;
                            this.editFlag = false;
                            const resultItem = resp.data.item;
                            item.showModifyTime = resultItem.showModifyTime;
                        }
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            download(item, type) {
                this.downloadFile(`/SupervisionPlan/RevisionDownload?seq=${item.Seq}&type=${type}`);
            },
            downloadFile(tarUrl) {
                //console.log('tarUrl: ' + tarUrl);
                window.comm.dnFile(tarUrl);
            },
            createFile(file) {
                // 附檔名判斷
                //console.log(file);
                if (!file.type.match('application/vnd.openxmlformats-officedocument.wordprocessingml.document') && !file.type.match('application/pdf')) {
                    alert('請選擇 Word docx, pdf 檔案');
                    this.dragging = false;
                    return;
                }
                this.file = file;
                this.dragging = false;
                this.files = new FormData();
                this.files.append("file", this.file, this.file.name);
            },
            uploadPlan() {
                this.files.append("engMain", this.targetItem.Seq);
                this.files.append("m", this.memo);
                this.files.append("aNo", this.approveNo);
                const files = this.files;
                window.myAjax.post('/SupervisionPlan/RevisionUpload', files,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getRevisionList();
                            alert(resp.data.message);
                            this.memo = '';
                            this.approveNo = '';
                            this.file = null;
                        } else {
                            alert(resp.data.message);
                        }
                    }).catch(error => {
                        console.log(error);
                    });
            }
        }

    }

</script>