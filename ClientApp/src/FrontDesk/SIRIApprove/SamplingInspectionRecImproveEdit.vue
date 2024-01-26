<template>
    <div>
        <EngInfo v-bind:engMain="engMain"></EngInfo>

        <h2>抽查缺失改善</h2>
        <div class="form-inline col-12 col-md-8 my-2">
            <label id="InspectionDate" class="my-1 mr-2">抽查日期：</label>
            <select v-model="searchCheckType" @change="onSearchCheckTypeChange" class="form-control mb-2 mr-sm-2">
                <option v-for="option in recCheckTypeOption" v-bind:value="option.Value" v-bind:key="option.Value">
                    {{ option.Value==1 ? '施工抽查' : (option.Value==2 ? '設備運轉測試' : (option.Value==3 ? '職業安全' : '生態保育'))  }}
                </option>
            </select>
            <select v-model="selectRecItem" @change="onSelectRecItemChange" class="form-control sort">
                <option v-for="option in selectRecItemOption" v-bind:value="option.Value" v-bind:key="option.Value">
                    {{ option.Text }}
                </option>
            </select>
            <button v-on:click="getRecHeader(selectRecItem)" class="btn btn-color3 mb-2 mr-sm-2" type="button"><i class="fas fa-search"></i></button>
        </div>
        <form @submit.prevent>
            <ul class="nav nav-tabs" role="tablist">
                <li class="nav-item">
                    <a v-on:click="selectTab='Report'" ref="reportTab" class="nav-link" data-toggle="tab" href="#menu01" title="不符合事項報告">不符合事項報告</a>
                </li>
                <li class="nav-item">
                    <a v-if="this.hasNCR" v-on:click="selectTab='NCR'" class="nav-link" data-toggle="tab" href="#menu02" title="NCR程序追蹤改善表">NCR程序追蹤改善表</a>
                </li>
                <li class="nav-item">
                    <a v-on:click="selectTab='Photo'" class="nav-link" data-toggle="tab" href="#menu03" title="改善照片">改善照片</a>
                </li>
            </ul>
            <!-- Tab panes -->
            <div v-show="si.Seq>0" class="tab-content">
                <div id="menu01" ref="reportTabPane" class="tab-pane">
                    <!-- 不符合事項報告 -->
                    <report v-bind:engMain="engMain.Seq" v-bind:si="si" v-on:ncr="onNCR" ref="report"></report>
                </div>
                <div id="menu02" class="tab-pane">
                    <!-- NCR程序追蹤改善表 -->
                    <NCR v-bind:engMain="engMain.Seq" v-bind:si="si" ref="ncr"></NCR>
                </div>
                <div id="menu03" class="tab-pane">
                    <!-- 改善照片 -->
                    <PhotoList v-if="selectTab=='Photo'" v-bind:engMain="engMain.Seq" v-bind:si="si"></PhotoList>
                </div>
            </div>
            <div class="row justify-content-center">
                <div class="col-6 col-lg-2 mt-3">
                    <button v-on:click.stop="back()" role="button" class="btn btn-shadow btn-color1 btn-block">
                        回上頁
                    </button>
                </div>
            </div>
        </form>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                selectTab: 'Report',
                //選項
                chsRecDate: '',
                hasNCR:false,
                recCheckTypeOption:[],
                ItemSeqOption: [], //檢驗標準項目清單
                
                //表頭資料
                si: {
                    Seq:-1,
                    EngConstructionSeq:-1,
                    CCRCheckDate: null,
                    CCRCheckType1: -1,//檢驗項目
                    ItemSeq: -1,//檢驗標準項目
                    CCRCheckFlow: 1,////施工流程
                    CCRPosLati: '',
                    CCRPosLong: '',
                    CCRPosDesc: ''
                },
                srcSi: {
                    Seq: -1,
                    EngConstructionSeq: -1,
                    CCRCheckDate: null,
                    CCRCheckType1: -1,//檢驗項目
                    ItemSeq: -1,//檢驗標準項目
                    CCRCheckFlow: 1,////施工流程
                    CCRPosLati: '',
                    CCRPosLong: '',
                    CCRPosDesc: ''
                },
                //檢驗單
                searchCheckType: -1,
                searchCheckTypeOld: -1,
                selectRecItem: -1,
                selectRecItemOption: [],
                //
                targetId: -1,
                engMain: {},
            };
        },
        components: {
            EngInfo: require('../SamplingInspectionRec/EngInfo.vue').default,
            report: require('./Report.vue').default,
            NCR: require('./NCR.vue').default,
            PhotoList: require('./PhotoList.vue').default,
        },
        methods: {
            //工程資訊
            getEngItem() {
                this.step = 2;
                this.engMain = {};
                window.myAjax.post('/SamplingInspectionRecImprove/GetEngItem', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engMain = resp.data.item;
                            this.getRecCheckTypeOption();
                            this.$refs.reportTab.classList.toggle('active');
                            this.$refs.reportTabPane.classList.toggle('active');
                        } else {
                            alert(resp.data.message);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //已有檢驗單之檢驗項目
            async getRecCheckTypeOption() {
                this.recCheckTypeOption = [];
                const { data } = await window.myAjax.post('/SamplingInspectionRecImprove/GetRecCheckTypeOption', { id: this.targetId });
                this.recCheckTypeOption = data;
            },
            //檢驗日期變更
            onSearchCheckTypeChange() {
                if (!this.noSaveAlert()) {
                    this.searchCheckType = this.searchCheckTypeOld;
                    return;
                }
                this.searchCheckTypeOld = this.searchCheckType;
                this.hasNCR = false;
                this.si.Seq = -1;
                this.selectRecItem = -1;
                this.selectRecItemOption = [];
                window.myAjax.post('/SamplingInspectionRecImprove/GetRecOptionByCheckType', { constructionSeq: this.targetId, checkTypeSeq: this.searchCheckType })
                    .then(resp => {
                        this.selectRecItemOption = resp.data;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onSelectRecItemChange() {
                this.getRecHeader(this.selectRecItem);
            },
            //檢驗單表頭資訊
            getRecHeader(id) {
                if (id == -1) return;
                this.hasNCR = false;
                window.myAjax.post('/SamplingInspectionRecImprove/GetRec', { recSeq: id })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.si = resp.data.item;
                            this.chsRecDate = this.si.chsCheckDate;
                            this.si.CCRCheckDate = this.toYearDate(this.chsRecDate);
                            this.srcSi = JSON.parse(JSON.stringify(this.si));
                        } else {
                            alert(resp.data.message);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            noSaveAlert() {
                var fChange = this.$refs.report.isDataChange() || this.$refs.ncr.isDataChange();
                console.log('fChange:' + fChange);
                if (fChange) {
                    var result = confirm('目前資料將不會儲存,是否確定?');
                    if (result) {
                        this.$refs.report.cleanDataChange();
                        this.$refs.ncr.cleanDataChange();
                    }
                    return result;
                }
                return true;
            },
            //中曆轉西元
            toYearDate(dateStr) {
                if (dateStr == null) return null;
                var dateObj = dateStr.split('/'); // yyy/mm/dd
                return new Date(parseInt(dateObj[0]) + 1911, parseInt(dateObj[1]) - 1, parseInt(dateObj[2]));
            },
            //日期檢查
            isExistDate(dateStr) {
                var dateObj = dateStr.split('/'); // yyy/mm/dd
                if (dateObj.length != 3) return false;

                var limitInMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

                var theYear = parseInt(dateObj[0]);
                if (theYear != dateObj[0]) return false;
                var theMonth = parseInt(dateObj[1]);
                if (theMonth != dateObj[1]) return false;
                var theDay = parseInt(dateObj[2]);
                if (theDay != dateObj[2]) return false;
                if (new Date(theYear + 1911, 1, 29).getDate() === 29) { // 是否為閏年?
                    limitInMonth[1] = 29;
                }
                return theDay <= limitInMonth[theMonth - 1];
            },
            onNCR(state) {
                console.log(state);
                this.hasNCR = state;
            },
            back() {
                //window.history.go(-1); 
                window.location = "/SIRIApprove";
            }
        },
        async mounted() {
            console.log('mounted() 抽查缺失改善-編輯' + window.location.href);
            let urlParams = new URLSearchParams(window.location.search);
            if (urlParams.has('id')) {
                this.targetId = parseInt(urlParams.get('id'), 10);
                this.si.EngConstructionSeq = this.targetId;
                console.log(this.targetId);
                if (Number.isInteger(this.targetId) && this.targetId > 0) {
                    var Today = new Date();
                    this.si.CCRCheckDate = Today;
                    this.chsRecDate = Today.getFullYear()-1911 + "/" + (Today.getMonth() + 1) + "/" + Today.getDate();
                    this.getEngItem();
                    return;
                }
            }
            window.location = "/FrontDesk";
        }
    }
</script>