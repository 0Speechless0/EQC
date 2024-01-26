<template>
<div>
    
</div>
    <!-- 流程圖 -->
    <!-- <div class="modal fade text-left" v-bind:id="modalId" v-bind:ref="modalId" data-backdrop="static" data-keyboard="false" tabindex="-1" aria-labelledby="flowchart" aria-hidden="true">
        div class="modal fade" id="flowChartModal" ref="flowChartModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true"
        <div class="modal-dialog modal-dialog-scrollable modal-dialog-centered modal-xl" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="projectUpload">測驗檢測施工抽查流程圖上傳</h5>
                    <button ref="flowChartModalClose" type="button" class="close" data-dismiss="modal" aria-label="Close" v-on:click="closeModal()">
                        <span aria-hidden="true">×</span>
                    </button>
                </div>
                <div class="modal-body">
                    <h2>流程圖上傳</h2>
                    <table class="table table2" border="0">
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
                                    <td><a v-on:click.stop="ShowHandler" href="#" class="a-blue underl l mx-2" v-bind:title="targetItem.FlowCharOriginFileName">{{targetItem.FlowCharOriginFileName}}</a></td>
                                    <td>{{targetItem.modifyDate}}</td>
                                    <td>
                                        <div class="row justify-content-center m-0">
                                            <a v-on:click.stop="delHandler" href="#" class="a-blue underl a-red l mx-2" title="刪除">刪除</a>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <img v-if="targetItem.FlowCharOriginFileName && targetItem.FlowCharOriginFileName.length > 0" v-bind:src="getPhotoPath()" class="rounded" style="max-height:600px;">
                    </div>
                    
                </div>
            </div>
        </div>
    </div> -->
</template>
<script>
    export default {
        // props: ['modalId', 'engMain', 'targetItem', 'downloadUrl'],
        components: {
            // EngInfo: require('./EngInfo.vue').default,
        },
        data: function () {
            return {
                // file: null,
                // files: new FormData(),
                // dragging: false,
                // photo:'',
            };
        },
        methods: {
            // uploadHandler: function () {
            //     this.$emit('onUploadEvent', this.files)
            // },
            // downloadHandler: function () {
            //     this.$emit('onDownloadEvent')
            // },
            // delHandler: function () {
            //     if (confirm('是否確定刪除?')) {
            //         this.$emit('onDelEvent')
            //     }
            // },
            // closeHandler: function () {
            //     this.$emit('onCloseEvent')
            // },
            // uploadFinish() {
            //     this.file = null;
            //     this.files = new FormData();
            // },
            // closeModal() {
            //     this.photo = '';
            //     this.uploadFinish();
            //     this.closeHandler();
            // },
            // ShowHandler: function () {
            //     this.$emit('onShowEvent')
            // },
            // //流程圖上傳
            // onFileChange(e) {
            //     // 判斷拖拉上傳或點擊上傳的 event
            //     var files = e.target.files || e.dataTransfer.files;

            //     // 預防檔案為空檔
            //     if (!files.length) {
            //         this.dragging = false;
            //         return;
            //     }
 
            //     this.file = files[0];
            //     this.dragging = false;
            //     this.files.append("file", this.file, this.file.name);
            // },
            // removeFile() {
            //     this.file = '';
            //     this.files = new FormData();
            //     this.photo = '';
            // },
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
            // showFile(tarUrl) {
            //     console.log('tarUrl:' + tarUrl);
            //     window.myAjax.post(tarUrl ,{ engMain: this.engMain.Seq, seq: this.targetItem.Seq })
            //         .then(resp => {
            //             if (resp.data.url != null) {
            //                 this.photo = resp.data.url;
            //             } else {
            //                 console.log(resp.data.message);
            //             }


            //             return '/FileUploads/Tp/' + resp.url;
            //         }).catch(error => {
            //             console.log(error);
            //         });
            // },
            // getPhotoPath(item) {

            //     return  this.photo;
            // }

        },
        async mounted() {
            // console.log('mounted() FlowChartModal');
        }
    }
</script>
