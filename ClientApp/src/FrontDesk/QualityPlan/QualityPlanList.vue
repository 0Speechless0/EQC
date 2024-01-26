<template>
    <div>
        <div class="table-responsive">
            <table class="table table1 min910" border="0">
                <thead>
                    <tr>
                     
                        <th class="number">工程編號</th>
                        <th>工程名稱</th>
                        <th class="number">執行機關</th>
                        <th>監造單位</th>
                        <th>核定日期</th>
                        <th>品質計畫</th>
                        <th>定稿計畫</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                       
                        <td>{{item.EngNo}}</td>
                        <td>{{item.EngName}}</td>
                        <td>{{item.ExecUnit}}</td>
                        <td>{{item.SupervisorUnitName}}</td>
                        <td>{{item.showApproveDate}}</td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <a v-if="item.DocState==-1" v-on:click.stop="createQualityPlan(item)" href="#" class="btn btn-color11-2 btn-xs mx-1" title="產製"><i class="fas fa-save"></i> 產製</a>
                                <a v-if="item.DocState>-1 && item.DocState<10" v-on:click.stop="unlockItem(item)" href="#" class="btn btn-color11-3 btn-xs mx-1" title="解鎖"><i class="fas fa-lock"></i> 解鎖</a>
                                <a v-if="item.DocState>1" v-on:click.stop="downloadQualityPlan(item, 0)" href="#" class="btn btn-color11-1 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> word</a>
                                <a v-if="item.DocState>1" v-on:click.stop="downloadQualityPlan(item, 1)" href="#" class="btn btn-color11-1 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> pdf</a>
                                <a v-if="item.DocState>1" v-on:click.stop="downloadQualityPlan(item, 2)" href="#" class="btn btn-color11-1 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> odt</a>
                                <p class="a-blue mx-2 p-gray" v-bind:title="docStateTitle(item.DocState)">{{docStateTitle(item.DocState)}}</p>
                            </div>
                        </td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <a v-on:click.stop="openModal(item)" href="#" class="btn btn-color11-2 btn-xs mx-1" title="上傳"><i class="fas fa-upload"></i> 上傳</a>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <FinalContentUpload :modalShow ="modalShow" :item="targetItem" @closeModal="modalShow = false" >

        </FinalContentUpload>
     </div>
</template>
<script type="JavaScript" src="https://cdnjs.cloudflare.com/ajax/libs/vue/1.0.18/vue.min.js"></script>
<script>
    import FinalContentUpload from "../../components/FinalContentUpload.vue";
    export default {
        components:{
            FinalContentUpload
        },
        data: function () {
            return {
                fileStatus : {},
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
                pageRecordCount: 30,
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
                //選項
                engMain: {},
                targetId: -1,
          

            };
        },
        methods: {
            openModal(item) {
                this.files = new FormData();
                this.targetName = '';
                this.targetItem = item;
                this.modalShow = true;
                this.getRevisionList();
            }
,
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
            //取得使用者單位資訊
            getUserUnit() {
                window.myAjax.post('/QualityPlan/GetUserUnit')
                    .then(resp => {
                        this.userUnit = resp.data.unit;
                        this.userUnitSub = resp.data.unitSub;

                        if (sessionStorage.getItem('selectUnit') == null) {
                            this.selectUnit = this.userUnit;
                            this.onUnitChange(this.selectUnit);
                        } else {
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
                const { data } = await window.myAjax.post('/QualityPlan/GetYearOptions');
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
                const { data } = await window.myAjax.post('/QualityPlan/GetUnitOptions', { year: this.selectYear });
                this.selectUnitOptions = data;
                if (this.userUnit == null) this.getUserUnit();

                sessionStorage.removeItem('selectYear');
                window.sessionStorage.setItem("selectYear", this.selectYear);

            },
            async onUnitChange(unitSeq) {
                if (this.selectUnitOptions.length == 0) return;
                this.items = [];
                this.selectSubUnit = ''
                this.selectSubUnitOptions = [];
                this.selectEngName = -1;
                this.selectEngNameItems = [];
                const { data } = await window.myAjax.post('/QualityPlan/GetSubUnitOptions', { year: this.selectYear, parentSeq: unitSeq });
                this.selectSubUnitOptions = data;
                if (this.initFlag == 0) {
                    this.initFlag == 1
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
                const { data } = await window.myAjax.post('/SupervisionPlan/GetEngNameOptions', { year: this.selectYear, unit: this.selectUnit, subUnit: this.selectSubUnit, engMain: this.selectEngName });
                this.selectEngNameItems = data;
                //有session
                if (this.initFlag == 2) {
                    this.initFlag = 1;
                    if (sessionStorage.getItem('selectEngName') != null) {
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

                //儲存到session
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

                window.myAjax.post('/QualityPlan/GetEng'
                    ,{
                        engMain: this.targetId
  
                    })
                    .then(resp => {
                        this.items = resp.data.items;
                        //if (!this.engNameFlag) {
                        //    this.selectEngNameItems = resp.data.engNameItems;
                        //}
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
            //解鎖工程狀態
            unlockItem(item) {
                window.myAjax.post('/QualityPlan/UnlockEng', { item: item })
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
            },
            //產製品質計畫書
            createQualityPlan(item) {
                //alert("尚未實作");
                this.settingComplete();
                this.targetId = item.Seq;
                
             //alert("尚未實作")
                // $.blockUI({
                //     message: '<p>計畫書產製中，預估需要2 - 5分鐘，請耐心等候...</p>',
                //     css: {
                //         width: '500px',
                //         height: '40px',
                //         centerX: true,
                //         centerY: true
                //     }
                // });;
                window.myAjax.post('/QualityPlan/CreatePlan', { engMain: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getList();
                        }
                        // $.unblockUI();
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        $.unblockUI();
                        console.log(err);
                    });
                    alert(resp.data.message);
                //this.getItem(true);
            },
            async getFileStatus(seq) {
                await window.myAjax.post("/QualityPlan/getFileStatus", {engMain : seq })
                    .then( resp => {

                        if(resp.data.result == 0) this.fileStatus = resp.data.fileStatus;
                    });
            },
            getItem(isShowCreateMsg) {
                this.fCanEdit = false;
                this.step = 2;
                this.engMain = {};
                window.myAjax.post('/SupervisionPlan/GetEngItem', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engMain = resp.data.item;
                            this.$refs.classList.toggle('active');
                            if (this.engMain.DocState == -1) this.fCanEdit = true;
                            if (isShowCreateMsg == true && this.engMain.DocState == 1); 
                        } else {
                            //alert(resp.data.message);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            settingComplete() {
                if (/*confirm('勾選與設置完成後，系統將鎖定各項資料，方能產製監造計畫書\n\n是否確定?')*/true) {
                    window.myAjax.post('/QualityPlan/SettingComplete', { engMain: this.targetId })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.selectTab = '';
                                this.getItem();
                            }
                            //alert(resp.data.message);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            download(item) {
                this.downloadFile('/QualityPlan/RevisionDownload?seq=' + item.Seq);
            },
            downloadQualityPlan(item, type) {
                if( (type == 1 && !this.fileStatus.PdfExist) || (type == 2 && !this.fileStatus.OdtExist) ) {

                    if(!confirm("若第一次下載，需要先產製會等比較久，確定要下載嗎?")) return;
                }
                this.downloadFile(`/QualityPlan/DownloadQualityPlan?seq=${item.Seq}&type=${type}`);
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
                        this.getFileStatus(this.targetId);
                    }).catch(error => {
                        console.log(error);
                    });
            },

        },
        async mounted() {
            console.log('mounted() 品質計畫');
            this.targetId = window.sessionStorage.getItem(window.eqSelTrenderPlanSeq);
            await this.getFileStatus(this.targetId);
            console.log("targetId", this.targetId);
            if (this.selectYearOptions.length == 0) {
                this.getSelectYearOption();
            }
            console.log(sessionStorage.getItem('selectYear'));
        }
    }
</script>