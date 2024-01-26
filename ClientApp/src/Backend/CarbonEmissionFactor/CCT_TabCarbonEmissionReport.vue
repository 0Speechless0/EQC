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
        <div v-if="items.length>0">
            <button class="btn btn-color11-1 btn-block col-2" @click="download">
                <i class="fas fa-download"></i>管制填報表
            </button>
        </div>
        <div class="table-responsive">
            <table class="table table1 min910" border="0">
                <thead class="insearch">
                    <tr>
                        <th class="sort">排序</th>
                        <th class="number">工程編號</th>
                        <th>工程名稱</th>
                        <th class="number">標案編號</th>
                        <th>標案名稱</th>
                        <th>執行機關</th>
                        <th>I. 建立標案</th>
                        <th>II. 碳排量估算</th>
                        <th>III. 材料設備送審管制總表填列情形</th>
                        <th>III. 檢試驗管制總表填列情形</th>
                        <th>IV. 施工日誌登載情形</th>
                        <th>IV. 監造報表登載情形</th>
                        <th>V. 施工抽查表填列情形</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td>{{index+1}}</td>
                        <td>{{item.EngNo}}</td>
                        <td>{{item.EngName}}</td>
                        <td>{{item.TenderNo}}</td>
                        <td>{{item.TenderName}}</td>
                        <td>{{item.ExecUnitName}}</td>
                        <td>{{item.isBuild}}</td>
                        <td>{{item.TotalKgCo2e}} 
                            &#40;{{item.DismantlingRate}}% &#41;</td>
                        <td>{{item.MaterialSummaryNum}}/{{ item.MaterialSummaryTotal }}</td>
                        <td>{{item.MaterialTestNum}}/{{item.MaterialTestTotal}}</td>
                        <td>{{item.FillConstructionDayNum}}/{{item.FillConstructionDayShouldNum}}/{{item.FillDayTotal }} </td>
                        <td>{{item.FillSupervisionDayNum}}/{{item.FillSupervisionDayShouldNum}}/{{item.FillDayTotal }} </td>
                        <td>{{item.constCheckShouldNum}}/{{ item.neededConstCheckNum }}</td>
                        <td></td>
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
            async download()
            {
                var date = new Date();
                var dateString = date.toISOString();
                var fileData = await window.myAjax.post("TenderCalForm/Download", {items : this.items}, {responseType: 'blob'});  
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
            async getList() {
                this.adjItem = null;
                if (this.selectYear == '' || this.selectUnit == '') return;
                if (this.selectSubUnit == null || this.selectUnit == '') this.selectSubUnit = -1;
                //
                this.selectedYear = '';
                this.selectedUnit = '';
                this.selectedSubUnit = '';
                window.myAjax.post('/TenderCalForm/GetList'
                    , {
                        year: this.selectYear,
                        uId: this.selectUnit,
                    })
                    .then(resp => {
                        this.items = resp.data;
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