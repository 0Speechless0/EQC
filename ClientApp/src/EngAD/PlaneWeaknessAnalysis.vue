<template>
    <div>
        <div class="table-responsive">
            <table class="table  table-hover table2 VA-middle">
                <tbody>
                    <tr>
                        <th style="width: 120px;">查詢年度</th>
                        <td>
                            <div class="form-inline">
                                <select v-model="selStartYear" class="form-control">
                                    <option v-for="option in selectYearOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                        {{ option.Text }}
                                    </option>
                                </select>~
                                <select v-model="selEndYear" class="form-control">
                                    <option v-for="option in selectYearOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                        {{ option.Text }}
                                    </option>
                                </select>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th style="width: 120px;">執行機關</th>
                        <td>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selAll" @change="onAllChange" value="All" type="checkbox" name="ExecutiveAgencyAll" id="ExecutiveAgency_all" class="custom-control-input">
                                <label for="ExecutiveAgency_all" class="custom-control-label">全部</label>
                            </div>
                            <div v-for="(unit, index) in unitList" :key="index" 
                                class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="unit.IsSelected" :value="unit.Text" type="checkbox" name="ExecutiveAgency" :id="`ExecutiveAgency_${index}`" class="custom-control-input">
                                <label :for="`ExecutiveAgency_${index}`" class="custom-control-label">{{ unit.Text }}</label>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th style="width: 120px;">工程地點</th>
                        <td>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaceAll" @change="onPlaceAllChange" type="checkbox" name="place_all" id="place_all" class="custom-control-input">
                                <label for="place_all" class="custom-control-label">全部</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="臺北市" type="checkbox" name="place" id="place_1" class="custom-control-input">
                                <label for="place_1" class="custom-control-label">臺北市</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="新北市" type="checkbox" name="place" id="place_2" class="custom-control-input">
                                <label for="place_2" class="custom-control-label">新北市</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="桃園市" type="checkbox" name="place" id="place_3" class="custom-control-input">
                                <label for="place_3" class="custom-control-label">桃園市</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="臺中市" type="checkbox" name="place" id="place_4" class="custom-control-input">
                                <label for="place_4" class="custom-control-label">臺中市</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="臺南市" type="checkbox" name="place" id="place_5" class="custom-control-input">
                                <label for="place_5" class="custom-control-label">臺南市</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="高雄市" type="checkbox" name="place" id="place_6" class="custom-control-input">
                                <label for="place_6" class="custom-control-label">高雄市</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="新竹縣" type="checkbox" name="place" id="place_7" class="custom-control-input">
                                <label for="place_7" class="custom-control-label">新竹縣</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="苗栗縣" type="checkbox" name="place" id="place_8" class="custom-control-input">
                                <label for="place_8" class="custom-control-label">苗栗縣</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="彰化縣" type="checkbox" name="place" id="place_9" class="custom-control-input">
                                <label for="place_9" class="custom-control-label">彰化縣</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="南投縣" type="checkbox" name="place" id="place_10" class="custom-control-input">
                                <label for="place_10" class="custom-control-label">南投縣</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="雲林縣" type="checkbox" name="place" id="place_11" class="custom-control-input">
                                <label for="place_11" class="custom-control-label">雲林縣</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="嘉義縣" type="checkbox" name="place" id="place_12" class="custom-control-input">
                                <label for="place_12" class="custom-control-label">嘉義縣</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="屏東縣" type="checkbox" name="place" id="place_13" class="custom-control-input">
                                <label for="place_13" class="custom-control-label">屏東縣</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="宜蘭縣" type="checkbox" name="place" id="place_14" class="custom-control-input">
                                <label for="place_14" class="custom-control-label">宜蘭縣</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="花蓮縣" type="checkbox" name="place" id="place_15" class="custom-control-input">
                                <label for="place_15" class="custom-control-label">花蓮縣</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="東縣" type="checkbox" name="place" id="place_16" class="custom-control-input">
                                <label for="place_16" class="custom-control-label">臺東縣</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="澎湖縣" type="checkbox" name="place" id="place_17" class="custom-control-input">
                                <label for="place_17" class="custom-control-label">澎湖縣</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="金門縣" type="checkbox" name="place" id="place_18" class="custom-control-input">
                                <label for="place_18" class="custom-control-label">金門縣</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="連江縣" type="checkbox" name="place" id="place_19" class="custom-control-input">
                                <label for="place_19" class="custom-control-label">連江縣</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="基隆市" type="checkbox" name="place" id="place_20" class="custom-control-input">
                                <label for="place_20" class="custom-control-label">基隆市</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="新竹市" type="checkbox" name="place" id="place_21" class="custom-control-input">
                                <label for="place_21" class="custom-control-label">新竹市</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaces" value="嘉義市" type="checkbox" name="place" id="place_22" class="custom-control-input">
                                <label for="place_22" class="custom-control-label">嘉義市</label>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th style="width: 120px;">承攬金額</th>
                        <td>
                            <div class="form-inline">
                                <input type="text" class="form-control">&nbsp;元&nbsp;~&nbsp;
                                <input type="text" class="form-control">&nbsp;元
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th style="width: 140px;">品質管制弱面</th>
                        <td>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="W2" type="checkbox" name="analysis" id="analysis_all" class="custom-control-input" checked>
                                <label for="analysis_all" class="custom-control-label">進度落後</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="W3" type="checkbox" name="analysis" id="analysis_1" class="custom-control-input" checked>
                                <label for="analysis_1" class="custom-control-label">標比偏低</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="W4" type="checkbox" name="analysis" id="analysis_2" class="custom-control-input" checked>
                                <label for="analysis_2" class="custom-control-label">近年查核成績不佳</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="W5" type="checkbox" name="analysis" id="analysis_3" class="custom-control-input" checked>
                                <label for="analysis_3" class="custom-control-label">近5年曾發生重大職安事件</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="W6" type="checkbox" name="analysis" id="analysis_4" class="custom-control-input" checked>
                                <label for="analysis_4" class="custom-control-label">近5年履約計分偏低(含總分、職安及品質等)</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="W7" type="checkbox" name="analysis" id="analysis_5" class="custom-control-input" checked>
                                <label for="analysis_5" class="custom-control-label">近3年曾遭停權</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="W8" type="checkbox" name="analysis" id="analysis_6" class="custom-control-input" checked>
                                <label for="analysis_6" class="custom-control-label">近期承攬量能偏高</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="W9" type="checkbox" name="analysis" id="analysis_7" class="custom-control-input" checked>
                                <label for="analysis_7" class="custom-control-label">跨區域承攬</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="W10" type="checkbox" name="analysis" id="analysis_8" class="custom-control-input" checked>
                                <label for="analysis_8" class="custom-control-label">施工量能偏低</label>
                            </div>
                        </td>
                    <tr>
                        <th style="width: 120px;">其他</th>
                        <td>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input type="checkbox" name="other" id="other_01" class="custom-control-input" checked>
                                <label for="other_01" class="custom-control-label">未受查核督導</label>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <p class="text-R">* 查核案件定義：查核欄位有資料<br>* 督導案件定義：系統內已有工程督導案件</p>
        <div class="row justify-content-center">
            <div class="col-12 col-sm-4 col-xl-2 my-2">
                <button @click="getList" role="button" class="btn btn-shadow btn-block btn-outline-secondary"><i class="fas fa-search"></i> 分析結果</button>
            </div>
        </div>

        <h5>委員參與排行榜</h5>
        <div class="table-responsive">
            <table class="table table-responsive-md">
                <thead class="insearch">
                    <tr>
                        <th><strong>排序</strong></th>
                        <th><strong>委員姓名</strong></th>
                        <th><strong>任職單位</strong></th>
                        <th><strong>參與標案數</strong></th>
                        <th><strong>出席率</strong></th>
                        <th class="text-center"><strong>功能</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in committeeItems" v-bind:key="item.Seq">
                        <td>{{index+1}}</td>
                        <td>{{item.CName}}</td>
                        <td>{{item.Profession}}</td>
                        <td>{{item.totalCount}}</td>
                        <td>{{item.presenceRate}}</td>
                        <td>
                            <div class="d-flex justify-content-center">
                                <button @click="onEngView(item)" class="btn btn-color2 btn-xs mx-1" role="button" title="檢視"><i class="fas fa-eye"></i> 檢視</button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div v-if="engItems.length > 0">
            <div class="card card-body p-0">
                <div class="table-responsive">
                    <table class="table table-responsive-md">
                        <thead class="insearch">
                            <tr>
                                <th class="text-center"><strong>年度</strong></th>
                                <th><strong>工程名稱</strong></th>
                                <th><strong>執行單位</strong></th>
                                <th><strong>決標經費</strong></th>
                                <th><strong>工程地點</strong></th>
                                <th><strong>參與委員</strong></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in engItems" v-bind:key="item.Seq">
                                <td>{{item.TenderYear}}</td>
                                <td>{{item.TenderName}}</td>
                                <td>{{item.ExecUnitName}}</td>
                                <td>{{item.BidAmount}}</td>
                                <td>{{item.Location}}</td>
                                <td>{{item.committees}}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                selectYearOptions: [],
                selStartYear: '',
                selEndYear: '',
                selAll: false,
                selUnits: [],
                selPlaceAll: false,
                selPlaces: [],
                selPlaneWeakness: [],
                committee: '',
                committeeItems: [],
                unitList: [],
                //
                tarStartYear: '',
                tarEndYear: '',
                engItems: [],
                //
                tarEngSeq: null,
                showModal: false,
            };
        },
        methods: {
            //執行機關
            onAllChange() {
                let elements = this.unitList;
                this.selUnits = []
                var i;
                for (i = 0; i < elements.length; i++) {
                    elements[i].IsSelected = this.selAll;
                    // if (this.selAll) {
                    //     this.selUnits.push(elements[i].value);
                    // }
                }
            },
            //工程地點
            onPlaceAllChange() {
                let elements = document.getElementsByName("place");
                this.selPlaces = []
                var i;
                for (i = 0; i < elements.length; i++) {
                    elements[i].checked = this.selPlaceAll;
                    if (this.selPlaceAll) {
                        this.selPlaces.push(elements[i].value);
                    }
                }
            },
            //工程清單
            onEngView(item) {
                window.myAjax.post('/EADPlaneWeakness/GetEngList', {
                    name: item.CName,
                    kind: item.Kind,
                    sYear: this.tarStartYear,
                    eYear: this.tarEndYear
                })
                    .then(resp => {
                        this.engItems = resp.data.items;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //委員清單
            getList() {
                this.selUnits = this.unitList
                    .filter(e => e.IsSelected )
                    .map(e => e.Text) ; 
                if (this.selUnits.length == 0) {
                    alert('請勾選 執行機關');
                    return;
                }
                this.committeeItems = [];
                this.engItems = [];
                this.tarStartYear = this.selStartYear;
                this.tarEndYear = this.selEndYear;
                window.myAjax.post('/EADPlaneWeakness/GetList', {
                    units: this.selUnits,
                    sYear: this.selStartYear,
                    eYear: this.selEndYear,
                    committee: this.committee
                })
                    .then(resp => {
                        this.committeeItems = resp.data.items;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //工程年分
            async getSelectYearOption() {
                const { data } = await window.myAjax.post('/EADPlaneWeakness/GetYearOptions');
                this.selectYearOptions = data;
                if (this.selectYearOptions.length > 0) {
                    this.selStartYear = this.selectYearOptions[0].Value;
                    this.selEndYear = this.selectYearOptions[0].Value;
                }
            },
        },
        async mounted() {
            console.log('mounted() 品質管制弱面追蹤與分析');
            this.unitList = await store.GetSelection("Unit/GetUnitList", "Unit") ;
            this.unitList  = this.unitList .filter(e => e.Text.indexOf("清單") == -1);
            this.getSelectYearOption();
        }
    }
</script>
