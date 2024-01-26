<template>
    <div>
        <div class="d-flex ">
            <h2>抽查缺失改善</h2>
            <div class="col col-lg-3 " style="margin-top: 20px;"  v-if="Object.keys(SuggestionList).length > 0">
                            <button type="button" class="btn btn-info" @click="openSuggestionModal">改善建議</button>
                        </div>
        </div>
   

        <!-- div class="form-inline col-12 col-md-8 my-2">
            <select v-model="selectRecEngConstruction" @change="onRecEngConstructionChange" class="form-control mb-2 mr-sm-2">
                <option v-for="option in recEngConstructions" v-bind:value="option.Value" v-bind:key="option.Value">
                    {{ option.Text }}
                </option>
            </select>
            <select v-if="fDebug" v-model="searchCheckType" disabled class="form-control mb-2 mr-sm-2">
                <option value="1">施工抽查</option>
                <option value="2">設備運轉測試</option>
                <option value="3">職業安全</option>
                <option value="4">生態保育</option>
            </select>
            <select v-model="selectRecItem" @change="onSelectRecItemChange" class="form-control sort">
                <option v-for="option in selectRecItemOption" v-bind:value="option.Value" v-bind:key="option.Value">
                    {{ option.Text }}
                </option>
            </select>
            <button v-on:click="getRecHeader(selectRecItem)" class="btn btn-color3 mb-2 mr-sm-2" type="button"><i class="fas fa-search"></i></button>
        </div -->

        <table class="table table-responsive-md table-hover">
            <thead class="insearch">
                <tr>
                    <th><strong>分項工程</strong></th>
                    <th><strong>檢查日期</strong></th>
                    <th><strong>地點</strong></th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="(item, index) in selectRecItemOption" v-bind:key="item.Seq">
                    <td class="text-left">{{item.ItemName}}</td>
                    <td>{{item.chsCheckDate}}</td>
                    <td>{{item.CCRPosDesc}}</td>
                    <td>
                        <button v-on:click="getRecHeader(item.Seq)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                    </td>
                </tr>
            </tbody>
        </table>
        <div v-show="si.Seq>0">
            <ul class="nav nav-tabs" role="tablist">
                <li class="nav-item">
                    <a v-on:click="selectTab='Report'" ref="reportTab" class="nav-link" data-toggle="tab" href="##" title="不符合事項報告">不符合事項報告</a>
                </li>
                <li class="nav-item">
                    <a v-if="this.hasNCR" v-on:click="selectTab='NCR'" ref="reportNCR" class="nav-link" data-toggle="tab" href="##" title="NCR程序追蹤改善表">NCR程序追蹤改善表</a>
                </li>
                <li class="nav-item">
                    <a v-on:click="selectTab='Photo'" class="nav-link" ref="reportPhoto" data-toggle="tab" href="##" title="改善照片">改善照片</a>
                </li>
            </ul>
            <!-- Tab panes -->
            <div class="tab-content">
                <!-- 不符合事項報告 -->
                <report v-show="selectTab=='Report'" v-bind:engMain="tenderItem.Seq" v-bind:si="si" v-on:ncr="onNCR" ref="report"></report>
                <!-- NCR程序追蹤改善表 -->
                <NCR v-show="selectTab=='NCR'" v-bind:engMain="tenderItem.Seq" v-bind:si="si" ref="ncr"></NCR>
                <!-- 改善照片 -->
                <PhotoGroup v-if="selectTab=='Photo'" v-bind:engMain="tenderItem.Seq" v-bind:si="si"></PhotoGroup>
            </div>
        </div>
        <ImproveSuggestionModal ref="ImproveSuggestionModal" title="改善建議" headerColor="#17a2b8">
            <template #body>
                <div class="card" v-for="([tag, suggestions] , index) in Object.entries(SuggestionList) " :key="index">
                    <div class="card-title">
                        {{ tag }}
                    </div>
                    <div class="card-body">
                        <pre v-for="(suggestion, index2) in suggestions" :key="index2">{{ index2+1 }}. {{ suggestion.Text }}</pre>
                    </div>
                </div>
            </template>
        </ImproveSuggestionModal>
    </div>
</template>
<script>
    export default {
        props: ['tenderItem', 'constCheckMode', 'constCheckItem'],
        data: function () {
            return {
                //s20230520
                //fDebug: false,
                //selectRecEngConstruction: -1,
                //recEngConstructions: [], //有檢驗單之分項工程
                //
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
                SuggestionList : []
                //
                //targetId: -1,
                //engMain: {},
            };
        },
        components: {
            //EngInfo: require('../SamplingInspectionRec/EngInfo.vue').default,
            report: require('./Report.vue').default,
            NCR: require('./NCR.vue').default,
            PhotoGroup: require('./PhotoGroup.vue').default,
            ImproveSuggestionModal: require('../../components/Modal.vue').default,
        },
        methods: {
            async getSuggestion(recItem)
            {
                console.log(this.constCheckItem);
                let {data} = await window.myAjax.post("Suggestion/GetSuggestion", { function : recItem.CCRCheckType1, functionSeq : recItem.ItemSeq });
                this.SuggestionList  = data;
            },
            openSuggestionModal()
            {
                this.$refs.ImproveSuggestionModal.show = true;
            },
            /*/有檢驗單之分項工程清單 s20230523
            getRecEngConstruction() {
                this.recEngConstructions = [];
                this.targetId = -1;
                this.selectRecEngConstruction = -1;
                window.myAjax.post('/SamplingInspectionRecImprove/GetRecEngConstruction',
                    {
                        mode: this.constCheckMode,
                        eId: this.tenderItem.Seq,
                        rId: this.constCheckItem.Seq
                    })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.recEngConstructions = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //檢驗單之分項工程 s20230523
            onRecEngConstructionChange() {
                this.targetId = this.selectRecEngConstruction;
                this.si.EngConstructionSeq = this.targetId;
                var Today = new Date();
                this.si.CCRCheckDate = Today;
                this.chsRecDate = Today.getFullYear() - 1911 + "/" + (Today.getMonth() + 1) + "/" + Today.getDate();
                this.onSearchCheckTypeChange();
            },*/
            /* 20230523 mark /工程資訊
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
            },*/
            //分項工程變更
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
                //s20230920
                window.myAjax.post('/SamplingInspectionRecImprove/GetRecOptionByCheckType1', { id: this.tenderItem.Seq, checkTypeSeq: this.searchCheckType, itemSeq: this.constCheckItem.Seq , itemName : this.constCheckItem.ItemName })
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
                //console.log(id);
                this.si.Seq = -1;
                this.selectTab = 'Report';
                this.$refs.reportPhoto.classList.remove('active');
                if (this.hasNCR) this.$refs.reportNCR.classList.remove('active');
                //if (this.$refs.reportTab.classList.indexOf('active') == -1)
                    this.$refs.reportTab.classList.add('active');
                this.hasNCR = false;
                let that = this;
                window.myAjax.post('/SamplingInspectionRecImprove/GetRec', { recSeq: id})
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.si = resp.data.item;
                            this.chsRecDate = this.si.chsCheckDate;
                            this.si.CCRCheckDate = window.comm.toYearDate(this.chsRecDate);
                            this.srcSi = JSON.parse(JSON.stringify(this.si));
                            this.getSuggestion(resp.data.item);
                                
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
            onNCR(state) {
                this.hasNCR = state;
            }
        },
        async mounted() {
            console.log('mounted() 抽查缺失改善-編輯' + window.location.href);
            this.searchCheckType = this.constCheckMode;
            //this.getRecEngConstruction();//s20230523
            this.onSearchCheckTypeChange();//s20230524
            /*if (this.subEngNameSeq > 0) {
                this.targetId = this.subEngNameSeq;
                this.si.EngConstructionSeq = this.targetId;
                var Today = new Date();
                this.si.CCRCheckDate = Today;
                this.chsRecDate = Today.getFullYear() - 1911 + "/" + (Today.getMonth() + 1) + "/" + Today.getDate();
                this.getEngItem();
                return;
            }*/
        }
    }
</script>