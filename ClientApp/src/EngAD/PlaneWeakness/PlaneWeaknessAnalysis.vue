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
                                <input v-model.trim="minBid" type="text" class="form-control">&nbsp;千元&nbsp;~&nbsp;
                                <input v-model.trim="maxBid" type="text" class="form-control">&nbsp;千元
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th style="width: 140px;">品質管制弱面</th>
                        <td>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeaknessAll" @change="onPlaneWeaknessChange" type="checkbox" name="analysis_all" id="analysis_all" class="custom-control-input">
                                <label for="analysis_all" class="custom-control-label">全部</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="w1" type="checkbox" name="analysis" id="analysis_1" class="custom-control-input" checked>
                                <label for="analysis_1" class="custom-control-label">重大(或重點防汛)</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="w2" type="checkbox" name="analysis" id="analysis_2" class="custom-control-input" checked>
                                <label for="analysis_2" class="custom-control-label">進度落後</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="w3" type="checkbox" name="analysis" id="analysis_3" class="custom-control-input" checked>
                                <label for="analysis_3" class="custom-control-label">標比偏低</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="w4" type="checkbox" name="analysis" id="analysis_4" class="custom-control-input" checked>
                                <label for="analysis_4" class="custom-control-label">近年查核成績不佳</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="w5" type="checkbox" name="analysis" id="analysis_5" class="custom-control-input" checked>
                                <label for="analysis_5" class="custom-control-label">近5年曾發生重大職安事件</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="w6" type="checkbox" name="analysis" id="analysis_6" class="custom-control-input" checked>
                                <label for="analysis_6" class="custom-control-label">近5年履約計分偏低(含總分、職安及品質等)</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="w7" type="checkbox" name="analysis" id="analysis_7" class="custom-control-input" checked>
                                <label for="analysis_7" class="custom-control-label">近3年曾遭停權</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="w8" type="checkbox" name="analysis" id="analysis_8" class="custom-control-input" checked>
                                <label for="analysis_8" class="custom-control-label">近期承攬量能偏高</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="w9" type="checkbox" name="analysis" id="analysis_9" class="custom-control-input" checked>
                                <label for="analysis_9" class="custom-control-label">跨區域承攬</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="w10" type="checkbox" name="analysis" id="analysis_10" class="custom-control-input" checked>
                                <label for="analysis_10" class="custom-control-label">施工量能偏低</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="w11" type="checkbox" name="analysis" id="analysis_11" class="custom-control-input" checked>
                                <label for="analysis_11" class="custom-control-label">委外監造工程</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="w12" type="checkbox" name="analysis" id="analysis_12" class="custom-control-input" checked>
                                <label for="analysis_12" class="custom-control-label">成績不佳的委外監造廠商</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="w13" type="checkbox" name="analysis" id="analysis_13" class="custom-control-input" checked>
                                <label for="analysis_13" class="custom-control-label">高敏感區域工程</label>
                            </div>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="selPlaneWeakness" value="w14" type="checkbox" name="analysis" id="analysis_14" class="custom-control-input" checked>
                                <label for="analysis_14" class="custom-control-label">全民督工</label>
                            </div>
                        </td>
                    <tr>
                        <th style="width: 120px;">其他</th>
                        <td>
                            <div class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="isSupervise" value="true" type="checkbox" name="other" id="other_01" class="custom-control-input" checked>
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
                <button @click="getList(false)" role="button" class="btn btn-shadow btn-block btn-outline-secondary"><i class="fas fa-search"></i> 分析結果</button>
            </div>
            <button style="padding: 5px;margin: 5px; " 
                @click="getList(true)"
                class="btn btn-color11-1 btn-xs mx-1" id="capture6"><i class="fas fa-download"></i><font style="vertical-align: inherit;"><font style="vertical-align: inherit;">下載</font></font></button>
        </div>
        <EngList v-for="(item, index) in selPlaneWeakness" v-bind:key="index" v-bind:engData="engItems[item]"></EngList>
    </div>
</template>
<script>
    import {useSelectionStore} from "../../store/SelectionStore.js";
    import Com from "../../Common/Common2";
    const store = useSelectionStore();
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
                isSupervise: true,
                minBid: '',
                maxBid: '',
                unitList: [],
                //
                selPlaneWeaknessAll:false, //s20230726
                selPlaneWeakness: [],
                engItems: {
                    w1: { title: '重大(或重點防汛)', items: [] },
                    w2: { title: '進度落後', items: [] },
                    w3: { title: '決標比偏低', items: [] },
                    w4: { title: '近年查核成績不佳', items: [] },
                    w5: { title: '近5年曾發生重大職安事件', items: [] },
                    w6: { title: '近5年履約計分偏低(含總分、職安及品質等)', items: [] },
                    w7: { title: '近3年曾遭停權', items: [] },
                    w8: { title: '近期承攬量能偏高', items: [] },
                    w9: { title: '跨區域承攬', items: [] },
                    w10: { title: '施工量能偏低', items: [] },
                    w11: { title: '委外監造工程', items: [] },
                    w12: { title: '成績不佳的委外監造廠商', items: [] },
                    w13: { title: '高敏感區域工程', items: [] },
                    w14: { title: '全民督工', items: [] },
                },
            };
        },
        components: {
            EngList: require('./EngList.vue').default,
        },
        methods: {
            //品質管制弱面
            initEngItems() {
                for (const key of Object.keys(this.engItems)) {
                    this.engItems[key].items = [];
                }
            },
            //執行機關
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
            onPlaneWeaknessChange() {
                let elements = document.getElementsByName("analysis");
                this.selPlaneWeakness = []
                var i;
                for (i = 0; i < elements.length; i++) {
                    elements[i].checked = this.selPlaneWeaknessAll;
                    if (this.selPlaneWeaknessAll) {
                        this.selPlaneWeakness.push(elements[i].value);
                    }
                }
            },

            //清單
            async getList(downloadExcel) {

                this.selUnits = this.unitList
                    .filter(e => e.IsSelected && !e.other )
                    .map(e => e.Text) ; 
                var otherUnits = this.unitList
                    .filter(e => e.IsSelected && e.other )
                    .map(e => e.Value) ;
                this.selUnits = this.selUnits.concat(otherUnits);
                if (this.selUnits.length == 0) {
                    alert('請勾選 執行機關');
                    return;
                }
                if ((this.minBid == '' && this.maxBid != '') || (this.minBid != '' && this.maxBid == '') ) {
                    alert('承攬金額輸入錯誤');
                    return;
                }
                if (this.minBid != '' && this.maxBid != '') {
                    var min = parseInt(this.minBid);
                    var max = parseInt(this.maxBid);
                    if (isNaN(min) || isNaN(max) ) {
                        alert('承攬金額 僅能輸入數字');
                        return;
                    } else if (min > max) {
                        alert('承攬金額 範圍錯誤');
                        return;
                    }
                }
                if (this.selPlaneWeakness.length == 0) {
                    alert('請勾選 品質管制弱面');
                    return;
                }
                this.initEngItems();
                this.selPlaneWeakness.sort();
                if(downloadExcel)
                {
                    Com.dnFile(`/EADPlaneWeakness/ExportExcel?${new URLSearchParams({
                        units: this.selUnits,
                        sYear: this.selStartYear,
                        eYear: this.selEndYear,
                        places: this.selPlaces,
                        isSupervise: this.isSupervise,
                        minBid: this.minBid,
                        maxBid: this.maxBid
                    }).toString()}`);
                }
                else
                {
                    for (const key of this.selPlaneWeakness) {
                    await window.myAjax.post('/EADPlaneWeakness/GetList', {
                        units: this.selUnits,
                        sYear: this.selStartYear,
                        eYear: this.selEndYear,
                        places: this.selPlaces,
                        isSupervise: this.isSupervise,
                        pw: key,
                        minBid: this.minBid,
                        maxBid: this.maxBid
                    })
                        .then(resp => {
                            this.engItems[key].items = resp.data.items;
                        })
                        .catch(err => {
                            console.log(err);
                        });
                    }
                }

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
            strEmpty(str) {
                return window.comm.stringEmpty(str);
            },
        },
        async mounted() {
            console.log('mounted() 品質管制弱面追蹤與分析');
            console.log(this.engItems['w1']);
            this.getSelectYearOption();
            this.unitList = await store.GetSelection("Unit/GetUnitList", "Unit") ;
            this.unitList  = this.unitList .filter(e => e.Text.indexOf("清單") == -1);
            this.unitList = this.unitList.concat([
                { Text : "縣市政府" , Value: 0, IsSelected :false, other:true},
                { Text : "補助單位" , Value: 1, IsSelected :false,  other:true}
            ])
        }
    }
</script>
