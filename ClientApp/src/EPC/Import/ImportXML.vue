 <template>
     <div>
         <form @submit.prevent>
             <div>
                 <div :class="['m-2 p-2', file ? '' : 'btn-color3']">
                     <div v-if="!file" :class="['dropZone', dragging ? 'dropZone-over' : '']"
                          @dragestart="dragging = true"
                          @dragenter="dragging = true"
                          @dragleave="dragging = false">
                         <div class="dropZone-info" @drag="onChange">
                             <span class="fa fa-upload dropZone-title"></span>
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
                                         <i class="fas fa-times"></i> 取消
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
         </form>
         <div class="row justify-content-between">
             <div class="form-inline col-12 col-md-8 mt-3">
                 <label for="tableinfo" class="my-1 mr-2">
                     共
                     <span class="small-red">{{recordTotal}}</span>
                     筆，每頁顯示
                 </label>
                 <select v-model="pageRecordCount" @change="onPageRecordCountChange($event)" class="form-control sort">
                     <option value="30">30</option>
                     <option value="50">50</option>
                     <option value="100">100</option>
                 </select>
                 <label for="tableinfo" class="my-1 mx-2">筆，共<span class="small-red">{{pageTotal}}</span>頁，目前顯示第</label>
                 <select v-model="pageIndex" @change="onPageChange($event)" class="form-control sort">
                     <option v-for="option in pageIndexOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                         {{ option.Text }}
                     </option>
                 </select>
                 <label for="tableinfo" class="my-1 mx-2">頁</label>
             </div>
         </div>
         <div class="table-responsive mt-4">
             <table class="table table-responsive-md table-hover">
                 <thead class="insearch">
                     <tr>
                         <th class="text-left"><strong>帳號</strong></th>
                         <th><strong>轉入時間</strong></th>
                         <th><strong>成功件數</strong></th>
                         <th><strong>失敗件數</strong></th>
                     </tr>
                 </thead>
                 <tbody>
                     <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                         <td>{{item.CreateUser}}</td>
                         <td>{{item.CreateDate}}</td>
                         <td>{{item.Success}}</td>
                         <td>{{item.Failure}}</td>
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
                file: null,//{ name: null, size: null },
                files: new FormData(),
                dragging: false,
                hideHeader: true,
                //分頁
                recordTotal: 0,
                pageRecordCount: 20,
                pageTotal: 0,
                pageIndex: 1,
                pageIndexOptions: [],
                items: []
            };
        },
        methods: {
            //計算分頁
            setPagination() {
                this.pageTotal = Math.ceil(this.recordTotal / this.pageRecordCount);
                //
                this.pageIndexOptions = [];
                if (this.pageTotal == 0) {
                    this.pageIndex = 1;
                } else {
                    for (var i = 1; i <= this.pageTotal; i++) {
                        this.pageIndexOptions.push({ Text: i, Value: i });
                    }
                    if (this.pageIndex > this.pageIndexOptions.length) {
                        this.pageIndex = this.pageIndexOptions.length;
                    }
                }
            },
            onPageRecordCountChange(event) {
                this.setPagination();
                this.getItem();
            },
            onPageChange(event) {
                this.getItem();
            },
            //歷史上傳紀錄
            getItem() {
                window.myAjax.post('/EPCImport/GetImportList'
                    ,{
                        pageRecordCount: this.pageRecordCount,
                        pageIndex: this.pageIndex
                    })
                    .then(resp => {
                        this.items = resp.data.items;
                        this.recordTotal = resp.data.pTotal;
                        this.setPagination();
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //xml 檔案上傳處裡
            onChange(e) {
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
                console.log(file.type);
                if (!file.type.match('text/xml') && !file.type.match('text/plain')) {
                    alert('請選擇 xml,txt 檔案');
                    this.dragging = false;
                    return;
                }
                this.file = file;
                this.dragging = false;
                this.files.append("file", this.file, this.file.name);
            },
            uploadXML(processMode) {
                this.files.set("processMode", processMode);
                const files = this.files;
                window.myAjax.post('/EPCImport/UploadXML', files,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            alert(resp.data.message);
                            this.pageIndex = 1;
                            this.getItem();
                            this.removeFile();
                        } else {
                            this.updateXMLMode = (resp.data.result == -2);
                            alert(resp.data.message);
                        }
                    }).catch(error => {
                        console.log(error);
                    });
            },
            removeFile() {
                this.file = '';
                this.files = new FormData();
                this.updateXMLMode = false;
            },
            onChartFileChange(e) {
                // 判斷拖拉上傳或點擊上傳的 event
                var files = e.target.files || e.dataTransfer.files;

                // 預防檔案為空檔
                if (!files.length) {
                    this.dragging = false;
                    return;
                }
                this.createChartFile(files[0]);
            },
            createChartFile(file) {
                this.file = file;
                this.dragging = false;
                this.files.append("file", this.file, this.file.name);
            }
        },
        computed: {
            // 前端擷取附檔名
            extension() {
                return (this.file) ? this.file.name.split('.').pop() : '';
            }
        },
        async mounted() {
            console.log('mounted() 標案資料匯入');
            this.getItem();
        }
    }
</script>