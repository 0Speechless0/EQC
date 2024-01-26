<template>
    <div>
        <EngInfo v-bind:engMain="engMain"></EngInfo>
        <p>以下資料系統會自動帶出，請確認是否需要異動或增減，背景灰色表示為使用者自建</p>
        <ul class="nav nav-tabs" role="tablist">
            <li class="nav-item">
                <a v-on:click="selectTab='Chapter5Summary'" ref="Chapter5" class="nav-link" data-toggle="tab" href="#">材料設備送審管制總表</a>
            </li>
            <li class="nav-item">
                <a v-on:click="selectTab='Chapter5'" class="nav-link" data-toggle="tab" href="#">材料設備送審管制標準</a>
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
        <div class="row justify-content-center">
            <div v-if="fCanEdit" class="col-12 col-sm-5 col-lg-4 col-xl-3 mt-3">
                <button v-on:click.stop="settingComplete" role="button" class="btn btn-shadow btn-color1 btn-block">
                    勾選與設置完成
                </button>
            </div>
            <div class="col-12 col-sm-5 col-lg-4 col-xl-3 mt-3">
                <button v-on:click.stop="createPlan" v-bind:disabled="engMain.DocState!=0" v-bind:class="(engMain.DocState==0) ? 'btn-color2' : 'bg-gray'" role="button" class="btn btn-shadow btn-block">
                    產製監造計畫書
                </button>
            </div>
            <div class="col-12 col-sm-5 col-lg-4 col-xl-3 mt-3">
                <button v-on:click.stop="back()" role="button" class="btn btn-shadow btn-color1 btn-block">
                    回上頁
                </button>
            </div>
            <div class="col-12 col-sm-5 col-lg-4 col-xl-3 mt-3">
                <button v-on:click.stop="downloadPlan(engMain)" v-if="engMain.DocState>1" class="btn-block mx-2 btn btn-color2" title="下載">下載</button>
            </div>

        </div>
    </div>
</template>
<script type="JavaScript" src="https://cdnjs.cloudflare.com/ajax/libs/vue/1.0.18/vue.min.js"></script>
<script>
    export default {
        data: function () {
            return {
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
                
            };
        },
        components: {
            EngInfo: require('./EngInfo.vue').default,
            Chapter5Summary: require('./SupervisionPlanEdit_Chapter5Summary.vue').default,
            Chapter5: require('./SupervisionPlanEdit_Chapter5.vue').default,
            Chapter6: require('./SupervisionPlanEdit_Chapter6.vue').default,
            Chapter701: require('./SupervisionPlanEdit_Chapter701.vue').default,
            Chapter702: require('./SupervisionPlanEdit_Chapter702.vue').default,
            Chapter703: require('./SupervisionPlanEdit_Chapter703.vue').default
        },
        methods: {
            getItem(isShowCreateMsg) {
                this.fCanEdit = false;
                this.step = 2;
                this.engMain = {};
                window.myAjax.post('/SupervisionPlan/GetEngItem', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engMain = resp.data.item;
                            this.selectTab = 'Chapter5Summary';
                            this.$refs.Chapter5.classList.toggle('active');
                            if (this.engMain.DocState == -1) this.fCanEdit = true;
                            if (isShowCreateMsg == true && this.engMain.DocState == 1); //alert('計畫書產製中，預估需要10 - 20分鐘，請耐心等候...');
                              
                            //解除遮罩
                            $.unblockUI();
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
                                this.getItem();
                            }
                            alert(resp.data.message);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            createPlan() {
                $.blockUI({
                                  message: '<p>計畫書產製中，預估需要10 - 20分鐘，請耐心等候...</p>',
                                  css: {
                                      width: '500px',
                                      height: '40px',
                                      centerX: true,
                                      centerY: true
                                  }
                              });
                window.myAjax.post('/SupervisionPlan/CreatePlan', { engMain: this.targetId })
                    /*.then(resp => {
                        if (resp.data.result == 0) {
                            this.selectTab = '';
                            this.getItem();
                        }
                        alert(resp.data.message);
                    })*/
                    .catch(err => {
                        console.log(err);
                    });
                this.selectTab = '';
                this.getItem(true);
            },
            editEng(item) {
                window.location = "/SupervisionPlan/Edit?id=" + item.Seq;
            },
            back() {
                //window.history.go(-1);
                window.location = "/SuperVisionPlan";
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
            }
        },
        async mounted() {
            console.log('mounted() 監造計畫-編輯' + window.location.href);
            let urlParams = new URLSearchParams(window.location.search);
            if (urlParams.has('id')) {
                this.targetId = parseInt(urlParams.get('id'), 10);
                console.log(this.targetId);
                if (Number.isInteger(this.targetId) && this.targetId > 0) {
                    this.getItem();
                    return;
                }
            }
            window.location = "/FrontDesk";
        }
    }
</script>