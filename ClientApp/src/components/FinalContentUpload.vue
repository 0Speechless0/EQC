<template>

<div class="modal fade show" id="MyDialog" ref="MyDialog" style="background:rgb(0 0 0 / 50%)" v-bind:style="{display: modalShow ? 'block' : 'none'}" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
            <div class="modal-dialog modal-dialog-scrollable modal-dialog-centered modal-lg" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="projectUpload">定稿計畫上傳</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close" v-on:click="closeModal()">
                            <span aria-hidden="true">×</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <table v-if="targetItem.DocState==-1" class="table table2" border="0">
                            <tbody>
                                <tr>
                                    <th class="col-1">檔案上傳</th>
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
                                    <th>簡述</th>
                                    <td><input v-model="targetName" maxlength="50" type="text" class="form-control" /></td>
                                </tr>
                            </tbody>
                        </table>
                        <div class="table-responsive">
                            <table class="table table1" border="0">
                                <thead>
                                    <tr>
                                        <th class="sort">版次</th>
                                        <th>時間</th>
                                        <th>簡述</th>
                                        <th>編輯</th>
                                        <th>檔案</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(item, index) in revisionList" v-bind:key="item.Seq">
                                        <td>{{item.RevisionNo}}</td>
                                        <td>{{item.showModifyTime}}</td>
                                        <td>
                                            <div v-if="!item.edit">{{item.Name}}</div>
                                            <input v-if="item.edit" type="text" class="form-control" v-model="item.Name">
                                        </td>
                                        <td>
                                            <div v-if="targetItem.DocState==-1" class="row justify-content-center m-0">
                                                <a v-on:click.stop="editItem(item)" v-if="!item.edit && (index==0)" href="#" class="btn btn-color11-3 btn-xs mx-1" title="編輯"><i class="fas fa-pencil-alt"></i> 編輯</a>
                                                <a v-on:click.stop="saveItem(item)" v-if="item.edit" href="#" class="btn btn-color11-2 btn-xs mx-1" title="儲存"><i class="fas fa-save"></i> 儲存</a>
                                            </div>
                                        </td>
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
        props: ["item", "modalShow"],
        emits: ["closeModal"],
        data : () => {
            return {
                dragging: false,
                revisionList :[],
                targetItem : {},
                file: null,
                files: new FormData()
            }
        },
        watch:{
            modalShow : {
                handler(value) {
                    if(value == true )this.targetItem = this.item;
                    console.log("modalShow", this.item);
                }
            },
            targetItem : {
                handler() {
                    this.getRevisionList();
                },
                flush: 'post'
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
                window.myAjax.post('/QualityPlan/RevisionList', { seq: this.targetItem.Seq })
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
                if (window.comm.stringEmpty(item.Name)) {
                    alert('須輸入資料');
                    return;
                }
                window.myAjax.post('/QualityPlan/RevisionSave', { item: item })
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
                this.downloadFile(`/QualityPlan/RevisionDownload?seq=${item.Seq}&type=${type}`);
            },
            downloadFile(tarUrl) {
                console.log('tarUrl: ' + tarUrl);
                window.myAjax.get(tarUrl, { responseType: 'blob' })
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
            createFile(file) {
                // 附檔名判斷
                console.log(file);
                /*if (!file.type.match('text/xml')) {
                    alert('請選擇 xml 檔案');
                    this.dragging = false;
                    return;
                }*/
                this.file = file;
                this.dragging = false;

                this.files.append("file", this.file, this.file.name);
            },
            uploadPlan() {
                this.files.append("engMain", this.targetItem.Seq);
                this.files.append("name", this.targetName);
                const files = this.files;
                window.myAjax.post('/QualityPlan/RevisionUpload', files,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            alert(resp.data.message);
                            this.getRevisionList();
                            this.targetName = '';
                            this.file = '';
                            this.files = new FormData();
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