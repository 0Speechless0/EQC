<template>
    <div>
        <div class="modal fade text-left" v-bind:id="modalId" v-bind:ref="modalId" tabindex="-1" aria-labelledby="catalogEdit" aria-hidden="true">
            <div class="modal-dialog modal-dialog-scrollable modal-dialog-centered modal-xl" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">相關試驗報告</h5>
                        <button @click="closeModal" type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="table-responsive">
                            <table border="0" class="table table2 mt-0 min720">
                                <tbody>
                                    <tr>
                                        <th>契約詳細表項次</th>
                                        <td>{{emdSummary.ItemNo}}</td>
                                        <th>材料/設備名稱</th>
                                        <td>{{emdSummary.MDName}}</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <h2 v-if="targetItem.edit">相關試驗報告上傳</h2>
                        <div v-if="targetItem.edit" class="m-2 p-2 btn-color3 mb-5">
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
                                                <button @click="removeFile" type="button" class="col-2 btn btn-shadow btn-color1">
                                                    取消選取
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div v-if="file" class="row justify-content-center my-3">
                                <div class="col-12 col-sm-4">
                                    <a v-if="!uploading" v-on:click.stop="uploadFile()" class="btn btn-color11-2 btn-xs mx-1" role="button">
                                        <i class="fas fa-upload"></i>&nbsp;&nbsp;上傳
                                    </a>
                                </div>
                            </div>
                        </div>
                        <h2>您已上傳的相關試驗報告</h2>
                        <div class="table-responsive mb-3">
                            <table border="0" class="table table1 my-0">
                                <thead>
                                    <tr>
                                        <th>項次</th>
                                        <th>檔案</th>
                                        <th>上傳日期</th>
                                        <th v-if="targetItem.edit">刪除</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(item, index) in targetItems" v-bind:key="item.Seq">
                                        <td class="text-center">{{index+1}}</td>
                                        <td><a v-on:click.stop="download(item)" href="#" class="a-blue underl l mx-2">{{item.OriginFileName}}</a></td>
                                        <td>{{item.showCreateTime}}</td>
                                        <td v-if="targetItem.edit">
                                            <div class="row justify-content-center m-0">
                                                <button v-on:click.stop="deleteAuditFileItem(item)" class="btn btn-color9-1 btn-xs mx-1" title="刪除"><i class="fas fa-trash-alt"></i> 刪除</button>
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
    </div>
</template>
<script>
    export default {
        props: ['modalId', 'emdSummary', 'targetItem', 'engMainSeq'],
        data: function () {
            return {
                docType: 1,
                loading: false,
                file: null,
                files: new FormData(),
                dragging: false,
                targetItems: [],
                uploading: false,
            };
        },
        methods: {
            closeHandler: function () {
                this.$emit('onCloseEvent')
            },
            closeModal() {
                this.closeHandler();
            },
            //
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
            createFile(file) {
                // 附檔名判斷
                //console.log(file);
                if (!file.type.match('application/pdf') && !file.type.match('image/jpeg')) {
                    alert('請選擇 pdf, jpg 檔案');
                    this.dragging = false;
                    return;
                }
                this.file = file;
                this.dragging = false;

                this.files.append("file", this.file, this.file.name);
            },
            removeFile() {
                this.uploading = false;
                this.file = '';
                this.files = new FormData();
            },
            //已上傳相關試驗報告清單
            getUploadAuditFileList() {
                this.targetItems = [];
                window.myAjax.post('/EMDAudit/GetUploadAuditFileResultList', { seq: this.targetItem.Seq, fileType: this.docType })
                    .then(resp => {
                        this.targetItems = resp.data.items;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //上傳相關試驗報告
            uploadFile() {
                if (this.uploading)
                    return;
                this.uploading = true;
                this.files.append("engMain", this.engMainSeq);
                this.files.append("id", this.targetItem.Seq);
                this.files.append("fileType", this.docType);
                const files = this.files;
                window.myAjax.post('/EMDAudit/UploadResult_FileType', files,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            alert(resp.data.message);
                            this.targetName = '';
                            this.file = '';
                            this.files = new FormData();
                        } else {
                            alert(resp.data.message);
                        }
                        this.getUploadAuditFileList();
                    }).catch(error => {
                        console.log(error);
                    });
                this.uploading = false;
            },
            deleteAuditFileItem(item) {
                if (confirm('是否確定刪除?')) {
                    window.myAjax.post('/EMDAudit/DelUploadAuditFileResult', { seq: item.Seq, eId: this.engMainSeq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getUploadAuditFileList();
                            }
                            alert(resp.data.message);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            download(item) {
                window.comm.dnFile('/EMDAudit/AuditFileResultDownload?seq=' + item.Seq + '&eId=' + this.engMainSeq);
            },
        },
        mounted() {
            console.log('mounted 相關試驗報告');
            this.getUploadAuditFileList();
        }
    }
</script>
