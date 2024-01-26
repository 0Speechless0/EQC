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
            <button @click="download" class="btn btn-color11-1 btn-block col-2">
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
                        <th>發包狀態</th>
                        <th>建立標案</th>
                        <th>I. 系統匯入PCCES建立標案</th>
                        <th>II. 碳排量估算及檢核</th>
                        <th>III. 材料設備送審管制總表及檢試驗管制總表填列</th>
                        <th>IV. 監造報表及施工日誌登載</th>
                        <th>V. 施工抽查表填報及照片上傳</th>
                        <th>特殊情況</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td>{{index+1}}</td>
                        <td>{{item.EngNo}}</td>
                        <td>{{item.EngName}}</td>
                        <td>{{item.TenderNo}}</td>
                        <td>{{item.TenderName}}</td>
                        <td>{{item.execUnitName}}</td>
                        <td>{{item.awardStatus}}</td>
                        <td>{{item.createEng}}</td>
                        <td>{{item.pccesXML}}</td>
                        <td>{{item.detachableRate}}</td>
                        <td>{{item.engMaterialDeviceCount == 0 || item.engMaterialDeviceSummaryCount > 0 ? 1 : 0 }}</td>
                        <td>{{item.supDaily}}</td>
                        <td>{{item.checkRec}}</td>
                        <td></td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                items: [],
                //選項
                selectYear: '',
                selectYearOptions: [],
                //機關
                selectUnit: '',
                selectUnitOptions: [],
            };
        },
        methods: {
            //工程年分
            async getSelectYearOption() {
                const { data } = await window.myAjax.post('/CEFactor/GetYearOptions');
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
                //工程機關
                this.selectUnit = '';
                this.selectUnitOptions = [];

                //this.selectSubUnit = -1;
                //this.selectSubUnitOptions = [];
                const { data } = await window.myAjax.post('/CEFactor/GetUnitOptions', { year: this.selectYear });
                this.selectUnitOptions = data;
                //if (this.userUnit == null) this.getUserUnit();

                sessionStorage.removeItem('selectYear');
                window.sessionStorage.setItem("selectYear", this.selectYear);
            },
            onUnitChange() {
                if (this.selectUnitOptions.length == 0) return;
                if (this.selectUnit == '') return;

                this.items = [];
                window.myAjax.post('/CEFactor/GetCCTList', { year: this.selectYear, uId: this.selectUnit})
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });

            },
            download() {
                window.comm.dnFile('/CEFactor/dnCCT?year=' +this.selectYear+"&uId="+ this.selectUnit);
            }
        },
        mounted() {
            console.log('mounted() 管制填報表');
            this.getSelectYearOption();
        }
    }
</script>