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
                        <th style="width: 120px;">委員</th>
                        <td>
                            <div class="form-inline">
                                <input v-model.trim="committee" type="text" class="form-control">
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
    import {useSelectionStore} from "../store/SelectionStore.js";
    const store = useSelectionStore();
    export default {
        data: function () {
            return {
                selectYearOptions: [],
                selStartYear: '',
                selEndYear: '',
                selAll: false,
                selUnits: [],
                committee: '',
                committeeItems: [],
                unitList: [],
                //
                tarStartYear: '',
                tarEndYear: '',
                engItems: [],
            };
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
            //工程清單
            onEngView(item) {
                window.myAjax.post('/EADCommittee/GetEngList', {
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
                window.myAjax.post('/EADCommittee/GetList', {
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
                const { data } = await window.myAjax.post('/EADCommittee/GetYearOptions');
                this.selectYearOptions = data;
                if (this.selectYearOptions.length > 0) {
                    this.selStartYear = this.selectYearOptions[0].Value;
                    this.selEndYear = this.selectYearOptions[0].Value;
                }
            },
        },
        async mounted() {
            console.log('mounted() 工程採購評選委員分析');
            this.unitList = await store.GetSelection("Unit/GetUnitList", "Unit") ;
            this.unitList  = this.unitList .filter(e => e.Text.indexOf("清單") == -1)
            this.getSelectYearOption();
        }
    }
</script>
