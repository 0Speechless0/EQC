<template>
    <div>
        <div class="table-responsive">
            <table class="table  table-hover table2 VA-middle">
                <tbody>
                    <tr>
                        <th style="width: 120px;">查詢年度</th>
                        <td>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="allYear" @change="onAllYearChange" v-bind:disabled="this.selectYearOptions.length == 0" type="checkbox" name="time" id="anytime" class="custom-control-input">
                                <label for="anytime" class="custom-control-label">不限</label>
                            </div>
                            <div class="form-inline">
                                <select v-model="selStartYear" @change="onYearChange" class="form-control">
                                    <option v-for="option in selectYearOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                        {{ option.Text }}
                                    </option>
                                </select>~
                                <select v-model="selEndYear" @change="onYearChange" class="form-control">
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
                        <th style="width: 120px;">分析項目</th>
                        <td>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selAnalysisAll" @change="onAnalysisAllChange" value="All" type="checkbox" name="analysisAll" id="analysis_all" class="custom-control-input">
                                <label for="analysis_all" class="custom-control-label">全部</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selAnalysis" value="a01" type="checkbox" name="analysis" id="analysis_1" class="custom-control-input">
                                <label for="analysis_1" class="custom-control-label">巨額工程落後2%</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selAnalysis" value="a09" type="checkbox" name="analysis" id="analysis_9" class="custom-control-input">
                                <label for="analysis_9" class="custom-control-label">5千萬以上未達2億元(落後8%以上)</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selAnalysis" value="a10" type="checkbox" name="analysis" id="analysis_10" class="custom-control-input">
                                <label for="analysis_10" class="custom-control-label">未達查核金額工程(落後8%以上)</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selAnalysis" value="a02" type="checkbox" name="analysis" id="analysis_2" class="custom-control-input">
                                <label for="analysis_2" class="custom-control-label">停工案件</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selAnalysis" value="a03" type="checkbox" name="analysis" id="analysis_3" class="custom-control-input" checked>
                                <label for="analysis_3" class="custom-control-label">完工3個月未完成驗收工程</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selAnalysis" value="a04" type="checkbox" name="analysis" id="analysis_4" class="custom-control-input">
                                <label for="analysis_4" class="custom-control-label">完工4個月未完成驗收工程</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selAnalysis" value="a05" type="checkbox" name="analysis" id="analysis_5" class="custom-control-input">
                                <label for="analysis_5" class="custom-control-label">完工5個月未完成驗收工程</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selAnalysis" value="a06" type="checkbox" name="analysis" id="analysis_6" class="custom-control-input">
                                <label for="analysis_6" class="custom-control-label">超過4個月未估驗工程</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selAnalysis" value="a07" type="checkbox" name="analysis" id="analysis_7" class="custom-control-input">
                                <label for="analysis_7" class="custom-control-label">超過5個月未估驗工程</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selAnalysis" value="a08" type="checkbox" name="analysis" id="analysis_8" class="custom-control-input" checked>
                                <label for="analysis_8" class="custom-control-label">終止解約工程</label>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="row justify-content-center">
            <div class="col-12 col-sm-4 col-xl-2 my-2">
                <button @click="getList" role="button" class="btn btn-shadow btn-block btn-outline-secondary"><i class="fas fa-search"></i> 分析結果</button>
            </div>
        </div>

        <EngList v-for="(item, index) in tarAnalysis" v-bind:key="index" v-bind:engData="engItems[item]" v-bind:selChange="tarChange"
                 v-bind:selUnits="tarUnits" v-bind:selStartYear="tarStartYear" v-bind:selEndYear="tarEndYear"></EngList>
    </div>
</template>
<script>
    import {useSelectionStore} from "../../store/SelectionStore.js";
    const store = useSelectionStore();
    export default {
        data: function () {
            return {
                allYear:false,
                selectYearOptions: [],
                selStartYear: '',
                selEndYear: '',
                unitList: [],
                selAll: false,
                selUnits: [],
                selAnalysisAll: false,
                selAnalysis: [],

                tarStartYear: '',
                tarEndYear: '',
                tarUnits: [],
                tarAnalysis: [],
                tarChange:null,
                engItems: {
                    a01: { title: '巨額工程落後2%', key: 1 },
                    a09: { title: '5千萬以上未達2億元(落後8%以上)', key: 9 },
                    a10: { title: '未達查核金額工程 (落後8%以上)', key: 10 },
                    a02: { title: '停工案件', key: 2 },
                    a03: { title: '完工3個月未完成驗收工程', key: 3 },
                    a04: { title: '完工4個月未完成驗收工程', key: 4 },
                    a05: { title: '完工5個月未完成驗收工程', key: 5 },
                    a06: { title: '超過4個月未估驗工程', key: 6 },
                    a07: { title: '超過5個月未估驗工程', key: 7 },
                    a08: { title: '終止解約工程', key: 8 },
                },
            };
        },
        components: {
            EngList: require('./EngList.vue').default,
        },
        methods: {
            onAllChange() {
                let elements = this.unitList;
                this.selUnits = [];
                var i;
                for (i = 0; i < elements.length; i++) {
                    elements[i].IsSelected = this.selAll;
                    // if (this.selAll) {
                    //     this.selUnits.push(elements[i].value);
                    // }
                }
            },
            onAnalysisAllChange() {
                let elements = document.getElementsByName("analysis");
                this.selAnalysis = [];
                var i;
                for (i = 0; i < elements.length; i++) {
                    elements[i].checked = this.selAnalysisAll;
                    if (this.selAnalysisAll) {
                        this.selAnalysis.push(elements[i].value);
                    }
                }
            },
            //工程清單
            onEngView(item) {
                window.myAjax.post('/EADRisk/GetEngList', {
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
                    .filter(e => e.IsSelected && !e.other)
                    .map(e => e.Text) ; 
                var otherUnits = this.unitList
                .filter(e => e.IsSelected && e.other )
                .map(e => e.Value) ;
                this.selUnits = this.selUnits.concat(otherUnits);
                if (this.selUnits.length == 0) {
                    alert('請勾選 執行機關');
                    return;
                }
                if (this.selAnalysis.length == 0) {
                    alert('請勾選 分析項目');
                    return;
                }
                if (this.selStartYear == '' || this.selEndYear == '') {
                    alert('請勾選 查詢年度');
                    return;
                }
                this.tarStartYear = this.selStartYear;
                this.tarEndYear = this.selEndYear;
                this.tarUnits = Object.assign([], this.selUnits);
                //
                this.tarAnalysis = [];
                this.tarAnalysis = Object.assign([], this.selAnalysis);
                this.tarAnalysis.sort();
                this.tarChange = new Date();
            },
            //工程年分
            onAllYearChange() {
                if (this.allYear) {
                    this.selStartYear = -1;
                    this.selEndYear = -1;
                } else if (this.selectYearOptions.length > 0) {
                    this.selStartYear = this.selectYearOptions[0].Value;
                    this.selEndYear = this.selectYearOptions[0].Value;
                } else {
                    this.selStartYear = '';
                    this.selEndYear = '';
                }
            },
            onYearChange() {
                if (this.allYear) {
                    this.allYear = false;
                    if (this.selectYearOptions.length > 0) {
                        if (this.selStartYear == -1) this.selStartYear = this.selectYearOptions[0].Value;
                        if (this.selEndYear == -1) this.selEndYear = this.selectYearOptions[0].Value;
                    } else {
                        this.selStartYear = '';
                        this.selEndYear = '';
                    }
                }
            },
            async getSelectYearOption() {
                const { data } = await window.myAjax.post('/EADRisk/GetYearOptions');
                this.selectYearOptions = data;
                if (this.selectYearOptions.length > 0) {
                    this.selStartYear = this.selectYearOptions[0].Value;
                    this.selEndYear = this.selectYearOptions[0].Value;
                }
            },
        },
        async mounted() {
            console.log('mounted() 水利工程履約風險分析');
            this.unitList = await store.GetSelection("Unit/GetUnitList", "Unit") ;
            this.unitList  = this.unitList .filter(e => e.Text.indexOf("清單") == -1);
            this.unitList = this.unitList.concat([
                { Text : "縣市政府" , Value: 0, IsSelected :false, other :true},
                { Text : "補助單位" , Value: 1, IsSelected :false, other :true}
            ]);
            this.getSelectYearOption();
        }
    }
</script>
