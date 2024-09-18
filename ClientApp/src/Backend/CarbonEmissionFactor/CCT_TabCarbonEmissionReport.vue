<template>
    <div>
        <form class="form-group insearch mb-3">
            <div class="form-row">
                <div class="col-1 mt-3">
                    <select v-model="selectYear" @change="onYearChange" class="form-control">
                        <option selected="selected" :value="-1"> 全部</option>
                        <option v-for="option in selectYearOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-3 mt-3">
                    <select v-model="selectUnit" @change="onUnitChange" class="form-control">
                        <option selected="selected" :value="-1"> 全部機關</option>
                        <option v-for="option in selectUnitOptions" v-bind:value="option.Value"
                                v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>

            </div>
        </form>
        <div v-if="loading">
            <p  style="color:darkgreen" class="mb-0 ml-2"> 更新中，請稍後 ...</p>
        </div>
        <div v-else-if="items.length>0  " class="d-flex">
            <button class="btn btn-color11-1  col-2" @click="download">
                <i class="fas fa-download"></i>管制填報表
            </button>
            <button class="btn btn-color11-3  col-1" @click="getList(true)"><i class="fas fa-refresh mr-2"></i>更新 </button>

        </div>

        <div v-if="!loading" class="table-responsive">
            <table class="table table1 min910" border="0">
                <thead class="insearch">
                    <tr>
                        <th class="sort" rowspan="2">排序</th>
                        <th class="number" rowspan="2">工程編號</th>
                        <th rowspan="2">工程名稱</th>
                        <th rowspan="2">決標日期</th>
                        <th rowspan="2">執行機關</th>
                        <th rowspan="2">I. 建立標案</th>
                        <th colspan="3">II. 碳排量估算</th>
                        <th colspan="2">III. 材料設備送審管制總表填列情形</th>
                        <th colspan="3">IV. 施工日誌登載情形</th>
                        <th colspan="2">V. 施工抽查表填列情形</th>
                    </tr>
                    <tr>

                        <th>碳排量檢核</th>
                        <th>可拆解率</th>
                        <th>施工優化減碳計算</th>
                        <th>總表填列情形</th>
                        <th>材料設備送審(協力廠商地址)</th>
                        <th>機具人員登載</th>
                        <th>施工日誌登載</th>
                        <th>監造報表登載</th>
                        <th>工地節能減碳抽查表施工抽查建立</th>
                        <th>施工品質抽查情形</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td>{{index+1}}</td>
                        <td>{{item.EngNo}}</td>
                        <td>{{item.EngName}}</td>
                        <td>{{ToROCDate(item.AwardDate) }}</td>
                        <td>{{item.ExecUnitName}}</td>
                        <td>{{item.IsBuild}}</td>
                        <td>{{ item.CECheckResult }}</td>
                        <td>{{item.DismantlingRate}}% </td>
                        <td>{{ item.ReductionResult }}</td>
                        <td>{{item.MaterialSummaryNum}}/{{ item.MaterialSummaryTotal }}</td>
                        <td>{{ item.MetarialAddrResult }}</td>
                        <td v-if="item.MachineLoadingResult != '未開工'"> 
                            {{ item.MachineLoadingResult }}/{{item.FillConstructionDayShouldNum}}/{{item.FillDayTotal }}</td>
                        <td v-else>
                            未開工
                        </td>
                        <td>{{item.FillConstructionDayNum}}/{{item.FillConstructionDayShouldNum}}/{{item.FillDayTotal }} </td>
                        <td>{{item.FillSupervisionDayNum}}/{{item.FillSupervisionDayShouldNum}}/{{item.FillDayTotal }} </td>
                        <td v-if="isNaN(parseInt(item.EnergySavingCarbonResult))" > {{ item.EnergySavingCarbonResult  }}</td>
                        <td v-else> {{ item.EnergySavingCarbonResult }}/{{item.energySavingCheckMustNum}}/{{item.energySavingCheckShouldNum}}</td>
                        <td>{{item.constCheckShouldNum}}/{{ item.neededConstCheckNum }}</td>

                    </tr>
                </tbody>
            </table>
        </div>

    </div>
</template>
<script>
    import Common2 from '../../Common/Common2';
    export default {
        data: function () {
            return {
                loading : false,
                //使用者單位資訊
                userUnit: null,
                userUnitSub: '',

                //選項
                selectYear: '',
                selectYearOptions: [],
                //機關
                selectUnit: '',
                selectUnitOptions: [],
                //機關單位
                selectSubUnit: -1,
                selectSubUnitOptions: [],

                items: [],
                selectedYear: '',
                selectedUnit: '',
                selectedSubUnit: '',
            };
        },
        methods: {
            ToROCDate(date)
            {
                return Common2.ToROCDate(date);
            },
            async download()
            {
                var date = new Date();
                var dateString = date.toISOString();
                var fileData = await window.myAjax.post("TenderCalForm/Download", {itemsStr : JSON.stringify(this.items), year :this.selectYear }, {responseType: 'blob'});  
                console.log(fileData);
                Common2.download2(fileData.data, `水利署碳排管制表(詳細)-[${dateString}].xlsx`, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            },
            //工程年分
            //工程年分

            async getSelectYearOption() {
                const { data } = await window.myAjax.post('/TenderPlan/GetYearOptions');
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
            async onYearChange() {
                this.items = [];
                this.selectedYear = '';
                this.selectedUnit = '';
                this.selectedSubUnit = '';
                //工程機關
                this.selectUnit = '';
                this.selectUnitOptions = [];

                this.selectSubUnit = -1;
                this.selectSubUnitOptions = [];
                const { data } = await window.myAjax.post('/TenderPlan/GetUnitOptions', { year: this.selectYear });
                this.selectUnitOptions = data;
                //if (this.userUnit == null) this.getUserUnit();

                sessionStorage.removeItem('selectYear');
                window.sessionStorage.setItem("selectYear", this.selectYear);
            },
            async onUnitChange() {
                if (this.selectUnitOptions.length == 0) return;

                this.items = [];
                this.selectedYear = '';
                this.selectedUnit = '';
                this.selectedSubUnit = '';

                this.selectSubUnit = -1
                this.selectSubUnitOptions = [];

                //儲存到session
                sessionStorage.removeItem('selectUnit');
                window.sessionStorage.setItem("selectUnit", this.selectUnit);
                this.getList();

            },
            //
            onSubUnitChange(event) {
                //this.getList();
                sessionStorage.removeItem('selectSubUnit');
                window.sessionStorage.setItem("selectSubUnit", this.selectSubUnit);

            },
            async getList(update) {
                this.adjItem = null;
                if (this.selectYear == '' || this.selectUnit == '') return;
                if (this.selectSubUnit == null || this.selectUnit == '') this.selectSubUnit = -1;
                //
                this.selectedYear = '';
                this.selectedUnit = '';
                this.selectedSubUnit = '';
                this.loading = true;
                await window.myAjax.post('/TenderCalForm/GetList'
                    , {
                        year: this.selectYear,
                        uId: this.selectUnit,
                        update : update
                    })
                    .then(resp => {
                        this.items = resp.data;
                        this.loading = false;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            dnGroup() {
                window.comm.dnFile('/CEFactor/dnCReportG?year=' + this.selectYear + "&unit=" + this.selectUnit + "&subUnit=" + this.selectSubUnit);
            }
        },
        mounted() {
            console.log("FFF");
            this.getSelectYearOption();
        }
    }
</script>