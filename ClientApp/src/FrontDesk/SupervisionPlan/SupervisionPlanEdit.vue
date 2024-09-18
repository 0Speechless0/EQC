<template>
    <div>
        <!-- shioulo 20220518-2254
        <EngInfo v-bind:engMain="engMain"></EngInfo>
        <p>以下資料系統會自動帶出，請確認是否需要異動或增減，背景灰色表示為使用者自建</p>
    -->
        <h5 class="insearch my-0 py-2">
            <div class="form-row justify-content-start my-1">
                <div class="col-12 col-lg-6 col-xl-8 my-1">
                    工程編號：{{engMain.EngNo}} &nbsp;&nbsp; 工程名稱：{{engMain.EngName}}
                </div>

                <div class="col-6 col-lg-2 col-xl-1">
                    <button v-on:click.stop="unlockItem" v-if="engMain.DocState!=null && engMain.DocState>-1 && engMain.DocState<10" v-bind:disabled="engMain.DocState==1" class="btn btn-color11-3 btn-block"><i class="fas fa-lock"></i>解鎖</button>
                </div>
                <div class="col-6 col-lg-4 col-xl-3">
                    <button class="btn btn-color11-2 btn-block" @click="finalContentUploadModal = true"><i class="fas fa-upload"></i> 上傳定稿</button>
                </div>
                <div class="col-6 col-lg-2 col-xl-1">
                    <button v-on:click.stop="downloadPlan(engMain, 0)" v-if="engMain.DocState>1" class="btn btn-color11-1 btn-block" title="下載"><i class="fas fa-download"></i>下載word</button>
                </div>
                <div class="col-6 col-lg-2 col-xl-1">
                    <button v-on:click.stop="downloadPlan(engMain, 1)" v-if="engMain.DocState>1" class="btn btn-color11-1 btn-block" title="下載"><i class="fas fa-download"></i>下載pdf</button>
                </div>
                <div class="col-6 col-lg-2 col-xl-1">
                    <button v-on:click.stop="downloadPlan(engMain, 2)" v-if="engMain.DocState>1" class="btn btn-color11-1 btn-block" title="下載"><i class="fas fa-download"></i>下載odt</button>
                </div>
            </div>
        </h5>
        <p></p>
        <ul class="nav nav-tabs" role="tablist">
            <li class="nav-item">
                <a v-on:click="selectTab='Chapter5Summary'" ref="Chapter5" class="nav-link" data-toggle="tab" href="#">編輯材料設備送審管制項目(勾選)</a>
            </li>
            <li class="nav-item">
                <a v-on:click="selectTab='Chapter5'" class="nav-link" data-toggle="tab" href="#">編輯材料設備品質管理標準(勾選)</a>
            </li>
            <li v-if="engMain.IsNeedElecDevice" class="nav-item">
                <a v-on:click="selectTab='Chapter6'" class="nav-link" data-toggle="tab" href="#">設備運轉測試標準</a>
            </li>
            <li class="nav-item">
                <a v-on:click="selectTab='Chapter701'" class="nav-link" data-toggle="tab" href="#">施工抽查標準</a>
            </li>
            <li class="nav-item">
                <a v-on:click="selectTab='Chapter702'" class="nav-link" data-toggle="tab" href="#">環境保育標準</a>
            </li>
            <li class="nav-item">
                <a v-on:click="selectTab='Chapter703'" class="nav-link" data-toggle="tab" href="#">職業安全衛生標準</a>
            </li>
        </ul>
        <div>
            <Chapter5Summary v-if="selectTab=='Chapter5Summary'" v-bind:engMain="engMain"></Chapter5Summary>
            <Chapter5 v-if="selectTab=='Chapter5'" v-bind:engMain="engMain"></Chapter5>
            <Chapter6 v-if="selectTab=='Chapter6'" v-bind:engMain="engMain"></Chapter6>
            <Chapter701 v-if="selectTab=='Chapter701'" v-bind:engMain="engMain"></Chapter701>
            <Chapter702 v-if="selectTab=='Chapter702'" v-bind:engMain="engMain"></Chapter702>
            <Chapter703 v-if="selectTab=='Chapter703'" v-bind:engMain="engMain"></Chapter703>
        </div>

        <p class="mt-5 mb-1 mx-2 small-green">* 新增項目後，請務必同步新增流程圖及抽查管理標準</p>
        <p class="mb-1 mx-2 small-green">* 勾選與設置完成後，系統將鎖定各項資料，方能產製監造計畫書</p>
        <p class="mb-1 mx-2 small-green">* 產製計畫書完成後，於word中請先點選ctrl+A更新功能變數</p>
        <p class="mb-1 mx-2 small-red">* 如須重新選擇管制項目，請點選「解鎖」按鈕，新增管制項目後，請點選「勾選與設置完成」，⑤</p>
        <div class="row justify-content-center">
            <div class="col-12 col-sm-5 col-lg-4 col-xl-3 mt-3">
                <button v-on:click.stop="createSupervisionProject" v-bind:disabled="!( engMain.DocState == null)" role="button" class="btn btn-color11-2 btn-block">
                    <i class="fas fa-upload"></i>&nbsp;匯入標案基本資料
                </button>
            </div>
            <div v-if="fCanEdit" class="col-12 col-sm-5 col-lg-4 col-xl-3 mt-3">
                <button v-on:click.stop="settingComplete" role="button" class="btn btn-shadow btn-color1 btn-block">
                    勾選與設置完成
                </button>
            </div>
            <div class="col-12 col-sm-5 col-lg-4 col-xl-3 mt-3">
                <button v-on:click.stop="createPlan" v-bind:disabled="engMain.DocState!=0" v-bind:class="engMain.DocState == 0 ? 'btn-color2' : 'bg-gray'" role="button" class="btn btn-shadow btn-block">
                    產製監造計畫書
                </button>
            </div>
            <div class="form-inline mt-3" v-if="engMain.DocState==1" >
                <span style="color:red;margin: auto;background: lightgreen; ">{{ (engMain.Progress*100).toFixed(0) }} %</span>
            </div>
            <!-- div class="col-12 col-sm-5 col-lg-4 col-xl-3 mt-3">
            <button v-on:click.stop="back()" role="button" class="btn btn-shadow btn-color1 btn-block">
                回上頁
            </button>
        <div -->
            <div v-if="engMain.DocState>1" class="col-12 col-sm-5 col-lg-4 col-xl-3 mt-3">
                <button v-on:click.stop="downloadPlan(engMain, 0)" class="btn btn-color11-1 btn-block" title="下載"><i class="fas fa-download"></i>下載word</button>
            </div>

        </div>
        <FinalContentUpload :modalShow="finalContentUploadModal" :targetItem="engMain" @closeModal=" finalContentUploadModal = false"></FinalContentUpload>
    </div>
</template>
<!-- script type="JavaScript" src="https://cdnjs.cloudflare.com/ajax/libs/vue/1.0.18/vue.min.js"></script -->
<script>
    //import FinalContentUpload from "../../components/FinalContentUpload.vue";
    //import axios from "axios";
    export default {
        data: function () {
            return {

                docStatus : 0,
                fCanEdit: false,    
                saveFlag: false,
                selectTab: '',
                modalShow: false,
                targetItem: { Name: '' },
                file: null,
                files: new FormData(),
                dragging: false,
                //選項
                targetId: -1,
                engMain: {},
                items: [],
                finalContentUploadModal :false,
                qualityPlanItem : {},
                fileStatus : {}
                
            };
        },
        components: {
            FinalContentUpload: require('./SupervisionPlanUpload.vue').default,
            //EngInfo: require('./EngInfo.vue').default,
            Chapter5Summary: require('./SupervisionPlanEdit_Chapter5Summary.vue').default,
            Chapter5: require('./SupervisionPlanEdit_Chapter5.vue').default,
            Chapter6: require('./SupervisionPlanEdit_Chapter6.vue').default,
            Chapter701: require('./SupervisionPlanEdit_Chapter701.vue').default,
            Chapter702: require('./SupervisionPlanEdit_Chapter702.vue').default,
            Chapter703: require('./SupervisionPlanEdit_Chapter703.vue').default
        },
        methods: {
            //shioulo 20220710
            createSupervisionProject() {
                if (confirm('系統將更新主檔及其相關資料表外，如材料設備清單、施工管理清單、設備運轉清單、職業安全、環境保育等資料\n\n是否確定?')) {
                    let old = this.selectTab;
                    this.selectTab = "";
                    window.myAjax.post('/TenderPlan/CreateSupervisionProject', { engMain: this.engMain.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {

                                this.engMain.DocState = 0;
                            
                                window.location.href += "&docStatus=1";
                                
                            }
                            alert(resp.data.message);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            //
            getItem(isShowCreateMsg) {
                this.fCanEdit = false;
                this.step = 2;
                this.engMain = {};
                window.myAjax.post('/SupervisionPlan/GetEngItem', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engMain = resp.data.item;
                            this.engMain.Progress = resp.data.Progress;
                            this.selectTab = 'Chapter5Summary';
                            this.$refs.Chapter5.classList.toggle('active');
                            if(this.engMain.DocState == -1) this.docState = 1; 
                            if (this.engMain.DocState == -1) this.fCanEdit = true;
                            if (isShowCreateMsg == true && this.engMain.DocState == 1); //alert('計畫書產製中，預估需要2 - 5分鐘，請耐心等候...');
                              
                            //解除遮罩
                            //$.unblockUI();
                        } else {
                            alert(resp.data.message);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            openModal(item) {
                this.files = new FormData();
                this.targetItem = item;
                this.modalShow = true;
            },
            closeModal() {
                this.targetItem = { Name: '' };
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
            /*async getQualityPlanDoc() {
                window.myAjax.post('/SupervisionPlan/RevisionList'
                    , {
                        seq: this.targetId

                    })
                    .then(resp => {
                        if (resp.data.items.length > 0) {
                            this.qualityPlanItem = resp.data.items[0]
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },*/
            /*async getQualityPlanDoc() {
                window.myAjax.post('/QualityPlan/GetEng'
                    ,{
                        engMain: this.targetId
  
                    })
                    .then(resp => {
                        if(resp.data.items.length >0) {
                            this.qualityPlanItem = resp.data.items[0]
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },*/
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
            //工程年分
            async getSelectYearOption() {
                const { data } = await window.myAjax.post('/SupervisionPlan/GetYearOptions');
                this.selectYearOptions = data;
            },
            onYearChange(event) {
                this.items = [];
                this.getSelectUnitOption();
            },
            //工程主辦單位
            async getSelectUnitOption() {
                this.selectUnit = ''
                this.selectUnitOptions = [];
                const { data } = await window.myAjax.post('/SupervisionPlan/GetUnitOptions', { year: this.selectYear});
                this.selectUnitOptions = data;
            },
            onUnitChange(event) {
                this.pageIndex = 1;
                this.getList();
            },

            async getList() {
                if (this.selectYear == '' || this.selectUnit == '') return;

                window.myAjax.post('/SupervisionPlan/GetList'
                    ,{
                        year: this.selectYear,
                        unit: this.selectUnit,
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
            settingComplete() {
                if (confirm('勾選與設置完成後，系統將鎖定各項資料，方能產製監造計畫書\n\n是否確定?')) {
                    window.myAjax.post('/SupervisionPlan/SettingComplete', { engMain: this.targetId })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.selectTab = '';
                                this.docStatus ++ ;
                                this.getItem();
                            }
                            alert(resp.data.message);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            async createPlan() {
                window.myAjax.post('/SupervisionPlan/CreatePlan', { engMain: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.selectTab = '';
                            this.getItem();
                        }
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
                this.selectTab = '';
                this.getItem(true);
            },
            editEng(item) {
                window.location = "/SupervisionPlan/Edit?id=" + item.Seq;
            },
            /*back() {
                //window.history.go(-1);
                window.location = "/SuperVisionPlan";
            },*/
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
            //監造計畫
            async downloadPlan(item, type) {
                if( (type == 1 && !this.fileStatus.PdfExist) || (type == 2 && !this.fileStatus.OdtExist) ) {

                    if(!confirm("若第一次下載，需要先產製會等比較久，確定要下載嗎?")) return;
                }
                await this.download(`/SupervisionPlan/PlanDownload?seq=${item.Seq}&type=${type}`);
            },
            async getFileStatus(seq) {
                await window.myAjax.post("/SupervisionPlan/getFileStatus", {engMain : seq })
                    .then( resp => {

                        if(resp.data.result == 0) this.fileStatus = resp.data.fileStatus;
                    });
            },
            async download(url) {
                window.comm.dnFile(url);
                /*window.myAjax.get(url, { responseType: 'blob' })
                    .then(resp => {
                        const blob = new Blob([resp.data]);
                        const contentType = resp.headers['content-type'];
                        if (contentType.indexOf('application/json') >= 0) {
                            //alert(resp.data);
                            this.fileStatis = res.data.fileCreateStatus;
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
                    });*/
            },
            //解鎖工程狀態
            async unlockItem() {
                await window.myAjax.post('/SupervisionPlan/UnlockEng', { item: this.engMain })
                    .then(resp => {
                        alert(resp.data.message);
                    })
 

                await window.myAjax.post('/QualityPlan/UnlockEng', { item: this.qualityPlanItem })
                .then(resp => {
                    this.editEng(this.engMain)
                })

            }
        },
        async mounted() {
            
            console.log('mounted() 監造計畫-編輯' + window.location.href);
            let urlParams = new URLSearchParams(window.location.search);
            if (urlParams.has('id')) {
                this.targetId = parseInt(urlParams.get('id'), 10);
                this.docStatus =  parseInt(urlParams.get('docStatus'), 10) ; 
                await this.getFileStatus(this.targetId);
                console.log(this.targetId, this.docStatus);
                if (Number.isInteger(this.targetId) && this.targetId > 0) {
                    this.getItem();
                    //this.getQualityPlanDoc();
                    return;
                }
            }
            // window.location = "/EngQualityManage";
        }
    }
</script>
<style scoped>

</style>