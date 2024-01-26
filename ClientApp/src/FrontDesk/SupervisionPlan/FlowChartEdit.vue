<template>
    <!-- 流程圖 -->
    <div>
        <!-- div class="modal fade" id="flowChartModal" ref="flowChartModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true" -->
        <div class="modal-dialog modal-dialog-scrollable modal-dialog-centered modal-xl" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="projectUpload">流程圖上傳</h5>
                </div>
                <div class="modal-body">
                    <EngInfo v-bind:engMain="engMain"></EngInfo>
                    <h2>上傳項目</h2>
                    <p class="bg-gray p-3 mb-5">
                        {{targetItem.ItemName}}
                    </p>
                    <h2 v-if="fCanEdit">流程圖上傳</h2>
                    <table v-if="fCanEdit" class="table table2" border="0">
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
                                                <a v-on:click.stop="uploadHandler" class="btn btn-shadow btn-color1 btn-block" role="button">
                                                    <i class="fas fa-plus"></i>&nbsp;&nbsp;上傳
                                                </a>
                                            </div>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    
                    <h2>您目前上傳的流程圖</h2>
                    <span>（若重新上傳將進行覆蓋）</span>
                    <div class="table-responsive">
                        <table class="table table1" border="0">
                            <thead>
                                <tr>
                                    <th>檔案</th>
                                    <th>上傳日期</th>
                                    <th>刪除</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-if="targetItem.FlowCharOriginFileName && targetItem.FlowCharOriginFileName.length > 0">
                                    <td><a v-on:click.stop="downloadHandler" href="#" class="a-blue underl l mx-2" v-bind:title="targetItem.FlowCharOriginFileName">{{targetItem.FlowCharOriginFileName}}</a></td>
                                    <td>{{targetItem.modifyDate}}</td>
                                    <td>
                                        <div v-if="fCanEdit" class="row justify-content-center m-0">
                                            <a v-on:click.stop="delHandler" href="#" class="a-blue underl a-red l mx-2" title="刪除">刪除</a>
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
    export default {
        components: {
            EngInfo: require('./EngInfo.vue').default,
        },
        data: function () {
            return {
                fCanEdit: false,
                chapter:'',
                actionUrl:'',
                engMainSeq:-1,
                engMain: {},
                targetItem: { ItemName: '', FlowCharOriginFileName: '' },
                downloadUrl: '',

                file: null,
                formData: new FormData(),
                dragging: false,
            };
        },
        methods: {
            uploadHandler: function () {
                //this.$emit('onUploadEvent', this.files)
                this.formData.append("engMain", this.engMainSeq);
                this.formData.append("seq", this.targetItem.Seq);
                window.myAjax.post(this.actionUrl +'Upload', this.formData,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            const item = resp.data.item.Data;
                            this.targetItem.FlowCharOriginFileName = item.FlowCharOriginFileName;
                            this.targetItem.modifyDate = item.modifyDate;
                            this.uploadFinish();

                        }
                        alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
            downloadHandler: function () {
                //this.$emit('onDownloadEvent')
                this.downloadFile(this.actionUrl +'DownloadFlowChart?engMain=' + this.engMainSeq + '&seq=' + this.targetItem.Seq);
            },
            delHandler: function () {
                if (confirm('是否確定刪除?')) {
                    //this.$emit('onDelEvent')
                    window.myAjax.post(this.actionUrl +'DelFlowChart', { engMain: this.engMainSeq, seq: this.targetItem.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.targetItem.FlowCharOriginFileName = '';
                            }
                            alert(resp.data.message);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            closeHandler: function () {
                this.$emit('onCloseEvent')
            },
            uploadFinish() {
                this.file = null;
                this.formData = new FormData();
            },
            closeModal() {
                this.uploadFinish();
                this.closeHandler();
            },
            //流程圖上傳
            onFileChange(e) {
                // 判斷拖拉上傳或點擊上傳的 event
                var files = e.target.files || e.dataTransfer.files;

                // 預防檔案為空檔
                if (!files.length) {
                    this.dragging = false;
                    return;
                }
 
                this.file = files[0];
                this.dragging = false;
                this.formData.append("file", this.file, this.file.name);
            },
            removeFile() {
                this.file = '';
                this.formData = new FormData();
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
            //工程專案
            getEngMain() {
                this.fCanEdit = false;
                this.engMain = {};
                window.myAjax.post('/SupervisionPlan/GetEngItem', { id: this.engMainSeq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engMain = resp.data.item;
                            if (this.engMain.DocState == -1) this.fCanEdit = true;
                        } else {
                            alert(resp.data.message);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //管制項目
            getItem(flowChartSeq) {
                this.editFlag = false;
                this.targetItem = { ItemName: '', FlowCharOriginFileName: '' };
                let params = { seq: flowChartSeq };
                window.myAjax.post(this.actionUrl+'Item', params)
                    .then(resp => {
                        this.targetItem = resp.data;
                        if (this.chapter==='5') this.targetItem.ItemName = this.targetItem.MDName;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        async mounted() {
            console.log('mounted() 監造計畫-流程圖');
            let urlParams = new URLSearchParams(window.location.search);
            if (urlParams.has('chapter') && urlParams.has('engMain') && urlParams.has('seq')) {
                this.chapter = urlParams.get('chapter');
                this.actionUrl = '/SupervisionPlan/Chapter' + this.chapter;
                this.engMainSeq = parseInt(urlParams.get('engMain'), 10);
                var flowChartSeq = parseInt(urlParams.get('seq'), 10);
                this.getEngMain();
                this.getItem(flowChartSeq);
                return;
            }
            window.location = "/FrontDesk";
        }
    }
</script>
