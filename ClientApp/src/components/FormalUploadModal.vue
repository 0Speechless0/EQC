<template>
    <div class="modal fade show" id="MyDialog" ref="MyDialog" style="background:rgb(0 0 0 / 50%)" v-bind:style="{display: modalShow  ? 'block' : 'none'}" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
        <div class="modal-dialog modal-dialog-scrollable modal-dialog-centered modal-xl" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h6 class="modal-title redText" id="projectUpload">{{ title }}</h6>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close" v-on:click="closeModal()">
                        <span aria-hidden="true">×</span>
                    </button>
                </div>
                <div class="modal-body">
                    <table class="table table2" border="0">
                        <tbody>
                            <tr>
                                <th class="col-2">{{ title }}</th>
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
                                            <button v-on:click.stop="uploadFile()" class="btn btn-outline-secondary btn-xs mx-1" role="button">
                                                <i class="fas fa-plus"></i> 上傳檔案
                                            </button>
                                        </div>
                                    </div>
                                </td>
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
                                    <slot name="thead" >
                                        <th>時間</th>
                                        <th>檔名</th>
                                        <th class="col-4">簡述</th>
                                        <th>檔案</th>
                                    </slot>
                                </tr>
                            </thead>
                            <tbody>
                                <slot name="tbody" :items="fileList">

                                    <tr  v-for="(file, index) in fileList" :key="index">
                                        <td class="text-center">{{ file.CreateTime }}</td>
                                        <td class="text-center">{{ file.Name }}</td>
                                        <td class="text-center">{{ file.Description }}</td>
                                        <td class="text-center">
                                            <div class="row justify-content-center m-0">
                                                <a v-on:click.stop="download(file)" href="#" class="btn btn-color11-1 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> 下載 </a>
                                                <a v-on:click.stop="deleteFileRemote(file)" href="#" class="btn btn-color9-1 btn-xs mx-1" title="刪除"><i class="fas fa-trash-alt"></i> 刪除</a>
                                            </div>
                                        </td>
                                    </tr>
                                </slot>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import Common from "../Common/Common2";
    export default{
        expose :['modalShow'],
        props: ["RemoteDir", "Route", "title"],
        data : () => {
            return {
                dragging: false,
                modalShow : false,
                fileList :[],
                //targetItem: {},
                memo: '',
                approveNo: '',
                file: null,
                files: new FormData()
            }
        },

        computed :{
            fileColumn()
            {
                return this.fileList.length > 0 ? Object.keys(this.fileList[0]) : [];
            },
            fileName()
            {
                var a= this.file.name.split(".");
                var name = a[0];
                var ext = a[1];
                return name.replace(".", "") + "." + this.memo.replace(".", "") + "." + ext;
            }
        },
        methods:{
            async deleteFileRemote(file)
            {
                let _name = file.Name.split(".");

               let {data :res } = await window.myAjax.post(this.Route+"/DeleteFile", { fileName : `${_name[0]}.${file.Description}.${_name[1]}`, dir : this.RemoteDir })
               if(res == true){
                    alert("刪除成功");
                    this.getFileList();
               }
            },
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
                this.modalShow = false;
            },
            getFileList() {
                window.myAjax.post(this.Route+"/GetDownloadFilesList", {dir : this.RemoteDir })
                    .then(resp => {
                        this.fileList = resp.data;
                        console.log("fileList", this.fileList);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            async download(file) {
                let _name = file.Name.split(".");
                let fileData = await window.myAjax.post(`${this.Route}/DownloadFile`, {fileName :   `${_name[0]}.${file.Description}.${_name[1]}`, dir :this.RemoteDir },{responseType: 'blob'});  
                Common.download2(fileData.data, file.Name, fileData.data.type)
            },
            downloadFile(tarUrl) {
                //console.log('tarUrl: ' + tarUrl);
                window.comm.dnFile(tarUrl);
            },
            createFile(file) {
                // 附檔名判斷
                //console.log(file);
                // if (!file.type.match('application/vnd.openxmlformats-officedocument.wordprocessingml.document') && !file.type.match('application/pdf')) {
                //     alert('請選擇 Word docx, pdf 檔案');
                //     this.dragging = false;
                //     return;
                // }
                this.file = file;
                this.dragging = false;
                this.files = new FormData();

            },
            uploadFile() {

                this.files.append("dir", this.RemoteDir);
                this.files.append("file", this.file, this.fileName);
                const files = this.files;
                window.myAjax.post(this.Route+'/UploadFile', files,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    })
                    .then(resp => {
                        if (resp.data == true) {
                            this.getFileList();
                            this.memo = '';
                            this.file = null;
                        } else {
                            alert(resp.data.message);
                        }
                    }).catch(error => {
                        console.log(error);
                    });
            }
        },
        mounted()
        {
            this.getFileList();
        }

    }

</script>