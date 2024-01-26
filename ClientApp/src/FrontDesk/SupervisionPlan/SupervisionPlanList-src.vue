<template>
    <div>
        <form class="form-group">
            <div class="form-row">
                <div class="col-12 col-sm-3 mt-3">
                    <select v-model="selectYear" @change="onYearChange($event)" class="form-control" >
                        <option v-for="option in selectYearOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-3 mt-3">
                    <select v-model="selectUnit" @change="onUnitChange(selectUnit)" class="form-control">
                        <option v-for="option in selectUnitOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-3 mt-3">
                    <select v-model="selectSubUnit" @change="onSubUnitChange($event)" class="form-control">
                        <option v-for="option in selectSubUnitOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-3 mt-3">
                    <select v-model="selectEngName" @change="onEngNameChange($event)" class="form-control">
                        <option v-for="option in selectEngNameItems" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
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
                    <option value="10">10</option>
                    <option value="20">20</option>
                    <option value="30">30</option>
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
        <div class="table-responsive">
            <table class="table table1 min910" border="0">
                <thead>
                    <tr>
                        <th class="sort">排序</th>
                        <th class="number">工程編號</th>
                        <th>工程名稱</th>
                        <th class="number">執行機關</th>
                        <th>監造單位</th>
                        <th>核定日期</th>
                        <th>監造計畫</th>
                        <th>定稿計畫</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td>{{pageRecordCount*(pageIndex-1)+index+1}}</td>
                        <td>{{item.EngNo}}</td>
                        <td><a v-on:click.stop="editEng(item)" href="#">{{item.EngName}}</a></td>
                        <td>{{item.ExecUnit}}</td>
                        <td>{{item.SupervisorUnitName}}</td>
                        <td>{{item.showApproveDate}}</td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <button v-on:click.stop="editEng(item)" v-if="item.DocState==-1" class="btn-block mx-2 btn btn-color2" title="編輯">編輯</button>
                                <button v-on:click.stop="unlockItem(item)" v-if="item.DocState>-1 && item.DocState<10" v-bind:disabled="item.DocState==1" v-bind:class="(item.DocState==1) ? 'bg-gray' : 'btn-color2'" class="btn-block mx-2 btn" title="解鎖">解鎖</button>
                                <button v-on:click.stop="downloadPlan(item)" v-if="item.DocState>1" class="btn-block mx-2 btn btn-color2" title="下載">下載</button>
                                <p v-if="item.DocState<2" v-bind:title="docStateTitle(item.DocState)" class="a-blue mx-2 p-gray">{{docStateTitle(item.DocState)}}</p>
                            </div>
                        </td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <button v-on:click.stop="openModal(item)" class="mx-2 btn btn-block btn-outline-secondary" title="上傳">上傳</button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
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
                                                    <a v-on:click.stop="uploadPlan()" class="btn btn-shadow btn-color1 btn-block" role="button">
                                                        <i class="fas fa-plus"></i>&nbsp;&nbsp;新增版次
                                                    </a>
                                                </div>
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
                                        <td>{{item.VersionNo}}</td>
                                        <td>{{item.showModifyTime}}</td>
                                        <td>
                                            <div v-if="!item.edit">{{item.Memo}}</div>
                                            <input v-if="item.edit" type="text" class="form-control" v-model="item.Memo">
                                        </td>
                                        <td>
                                            <div class="row justify-content-center m-0">
                                                <a v-on:click.stop="editItem(item)" v-if="!item.edit && (index==0)" href="#" class="btn-block mx-2 btn btn-color2" title="編輯">編輯</a>
                                                <a v-on:click.stop="saveItem(item)" v-if="item.edit" href="#" class="btn-block mx-2 btn btn-color1" title="儲存">儲存</a>
                                            </div>
                                        </td>
                                        <td>
                                            <div class="row justify-content-center m-0">
                                                <a v-on:click.stop="downloadFinalized(item)" href="#" class="mx-2 btn btn-block btn-outline-info" title="下載">下載</a>
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
        data: function () {
            return {
                //使用者單位資訊
                userUnit: null,
                userUnitSub: '',
                initFlag: 0,

                modalShow: false,
                targetItem: {},
                targetName: '',
                file: null,
                files: new FormData(),
                dragging: false,
                revisionList:[],//計劃書版次清單
                //分頁
                recordTotal:0,
                pageRecordCount: 10,
                pageTotal: 0,
                pageIndex: 0,
                pageIndexOptions:[],
                //選項
                selectYear: '',
                selectYearOptions:[],
                //機關
                selectUnit: '',
                selectUnitOptions: [],
                //機關單位
                selectSubUnit: '',
                selectSubUnitOptions: [],
                //工程
                engNameFlag: false,
                selectEngName: -1,
                selectEngNameItems: [],
                items: [],
                //
                editFlag: false,
            };
        },
        methods: {
            openModal(item) {
                this.files = new FormData();
                this.targetName = '';
                this.targetItem = item;
                this.modalShow = true;
                this.getRevisionList();
            },
            closeModal() {
                this.targetItem = {};
                this.modalShow = false;
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
            removeFile() {
                this.file = '';
                this.files = new FormData();
            },
            //取得使用者單位資訊
            getUserUnit() {
                window.myAjax.post('/SupervisionPlan/GetUserUnit')
                    .then(resp => {
                        this.userUnit = resp.data.unit;
                        this.userUnitSub = resp.data.unitSub;

                        if (sessionStorage.getItem('selectUnit') == null) {
                            this.selectUnit = this.userUnit;
                            this.onUnitChange(this.selectUnit);
                        }else{
                            this.selectUnit = sessionStorage.getItem('selectUnit');
                            this.initFlag = 2;
                            this.onUnitChange(this.selectUnit);
                        }

                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //工程年分
            async getSelectYearOption() {
                const { data } = await window.myAjax.post('/SupervisionPlan/GetYearOptions');
                this.selectYearOptions = data;
                if (this.selectYearOptions.length > 0) {

                    if (sessionStorage.getItem('selectYear') == null) {
                        this.selectYear = this.selectYearOptions[0].Value;
                        this.onYearChange();
                    } else {
                        this.selectYear = sessionStorage.getItem('selectYear');
                        this.onYearChange();
                    }

                }
            },
            async onYearChange(event) {
                this.items = [];
                //工程機關
                this.selectUnit = '';
                this.selectUnitOptions = [];
                this.selectSubUnit = '';
                this.selectSubUnitOptions = [];
                this.selectEngName = -1;
                this.selectEngNameItems = [];
                const { data } = await window.myAjax.post('/SupervisionPlan/GetUnitOptions', { year: this.selectYear });
                this.selectUnitOptions = data;
                if (this.userUnit == null) this.getUserUnit();

                sessionStorage.removeItem('selectYear');
                window.sessionStorage.setItem("selectYear", this.selectYear);

            },
            async onUnitChange(unitSeq) {
                console.log(this.initFlag);
                if (this.selectUnitOptions.length == 0) return;
                this.items = [];
                this.selectSubUnit = ''
                this.selectSubUnitOptions = [];
                this.selectEngName = -1;
                this.selectEngNameItems = [];
                const { data } = await window.myAjax.post('/SupervisionPlan/GetSubUnitOptions', { year: this.selectYear, parentSeq: unitSeq });
                this.selectSubUnitOptions = data;
                if (this.initFlag == 0) {
                    this.initFlag = 1;
                    console.log(this.initFlag);
                    this.selectSubUnit = this.userUnitSub;
                    this.onSubUnitChange();
                }
                if (this.initFlag == 2) {
                    this.selectSubUnit = sessionStorage.getItem('selectSubUnit');
                    this.onSubUnitChange();
                }
                //儲存到session
                sessionStorage.removeItem('selectUnit');
                window.sessionStorage.setItem("selectUnit", this.selectUnit);
            },
            //
            async onSubUnitChange(event) {
                this.items = [];
                this.selectEngName = -1;
                this.selectEngNameItems = [];
                this.pageIndex = 1;
                const { data } = await window.myAjax.post('/SupervisionPlan/GetEngNameOptions',{year: this.selectYear,unit: this.selectUnit,subUnit: this.selectSubUnit,engMain: this.selectEngName });
                this.selectEngNameItems = data;
                //有session
                if (this.initFlag == 2) {
                    this.initFlag = 1;
                    if (sessionStorage.getItem('selectEngName') != null) {
                        this.getList();
                        this.onEngNameChange(1);
                    } else {
                        this.initFlag = 1;
                        this.getList();
                    }
                } else {
                    this.initFlag = 1;
                    this.getList();
                }
                sessionStorage.removeItem('selectSubUnit');
                window.sessionStorage.setItem("selectSubUnit", this.selectSubUnit);
            },
            onEngNameChange($event) {
                if ($event == 1) {
                    this.selectEngName = sessionStorage.getItem('selectEngName');
                } 
                this.engNameFlag = true;
                this.pageIndex = 1;
                this.getList();

                sessionStorage.removeItem('selectEngName');
                window.sessionStorage.setItem("selectEngName", this.selectEngName);

            },
            onPageRecordCountChange(event) {
                this.setPagination();
                this.getList();
            },
            onPageChange(event) {
                this.getList();
            },
            //計算分頁
            setPagination() {
                this.pageTotal = Math.ceil(this.recordTotal / this.pageRecordCount);
                //
                this.pageIndexOptions = [];
                if (this.pageTotal == 0) {
                    this.pageIndex = 0;
                } else {
                    for (var i = 1; i <= this.pageTotal; i++) {
                        this.pageIndexOptions.push({ Text: i, Value: i });
                    }
                    if (this.pageIndex > this.pageIndexOptions.length) {
                        this.pageIndex = this.pageIndexOptions.length;
                    }
                }
            },
            async getList() {
                if (this.selectYear == '' || this.selectUnit == '' || this.selectSubUnit == '') return;
                window.myAjax.post('/SupervisionPlan/GetList'
                    ,{
                        year: this.selectYear,
                        unit: this.selectUnit,
                        subUnit: this.selectSubUnit,
                        engMain: this.selectEngName,
                        pageRecordCount: this.pageRecordCount,
                        pageIndex: this.pageIndex
                    })
                    .then(resp => {
                        this.items = resp.data.items;
                       // if (!this.engNameFlag) {
                       //     this.selectEngNameItems = resp.data.engNameItems;
                       // }
                        this.recordTotal = resp.data.pTotal;
                        this.setPagination();
                        this.engNameFlag = false;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            docStateTitle(docState) {
                if (docState == 1) {
                    return '產製中';
                } else if (docState == 2) {
                    return '產製完成';
                } else if (docState == 0) {
                    return '已確認';
                } else {
                    return '無';
                }
            },
            //已上傳計畫書清單
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
            //解鎖工程狀態
            unlockItem(item) {
                window.myAjax.post('/SupervisionPlan/UnlockEng', { item: item })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            const resultItem = resp.data.item;
                            item.DocState = resultItem.DocState;
                            item.showModifyTime = resultItem.showModifyTime;
                        }
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //上傳新版計劃書
            uploadPlan() {
                this.files.append("engMain", this.targetItem.Seq);
                this.files.append("name", this.targetName);
                const files = this.files;
                window.myAjax.post('/SupervisionPlan/RevisionUpload', files,
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
            },
            //定稿計畫
            downloadFinalized(item) {
                this.download('/SupervisionPlan/RevisionDownload?seq=' + item.Seq);
            },
            //監造計畫
            downloadPlan(item) {
                this.download('/SupervisionPlan/PlanDownload?seq=' + item.Seq);
            },
            download(url) {
                //window.open('/FlowChartTp/Chapter5Download' + '?seq=' + item.Seq);
                window.myAjax.get(url, { responseType: 'blob' })
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
            editEng(item) {
                //window.location = "/SupervisionPlan/Edit?id=" + item.Seq;
                window.myAjax.post('/SupervisionPlan/EditEng', { seq: item.Seq })
                    .then(resp => {
                        console.log(resp.data.Url);
                        window.location.href = resp.data.Url;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        async mounted() {
            console.log('mounted() 監造計畫');
            if (this.selectYearOptions.length == 0) {
                this.getSelectYearOption();
            }
        }
    }
</script>