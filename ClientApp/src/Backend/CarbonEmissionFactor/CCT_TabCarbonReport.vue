<template>
    <div>
        <div class="d-flex bd-highlight">
            <div class="bd-highlight pr-1">
                <select v-model="selectYear" @change="onYearChange($event)" class="form-control">
                    <option v-for="option in selectYearOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                        {{ option.Text }}
                    </option>
                </select>
            </div>
            <div class="bd-highlight pr-1">
                <select v-model="selectUnit" @change="onUnitChange(selectUnit)" class="form-control">
                    <option selected="selected" :value="-1"> 全部</option>
                    <option v-for="option in selectUnitOptions" v-bind:value="option.Value"
                            v-bind:key="option.Value">
                        {{ option.Text }}
                    </option>
                </select>
            </div>
            <div class="bd-highlight">
                <select v-model="selectSubUnit" @change="onSubUnitChange($event)" class="form-control">
                    <option v-for="option in selectSubUnitOptions" v-bind:value="option.Value"
                            v-bind:key="option.Value">
                        {{ option.Text }}
                    </option>
                </select>
            </div>
            <div class="bd-highlight d-flex justify-content-between">
                <button v-on:click.stop="getList" type="button" class="btn btn-outline-secondary btn-xs mx-1" data-dismiss="modal">查詢 <i class="fas fa-search"></i></button>
            </div>

            <button @click="$refs.fileModal.modalShow = true" class="btn btn-color9-1 btn-xs mx-1 mr-3 ml-auto">檔案管理</button>
            <div v-if="items.length > 0" class=" bd-highlight d-flex justify-content-between">
                <button @click="dnGroup" class="btn btn-color11-1 btn-xs mx-1 mr-3"><i class="fas fa-download"></i>總表</button>
            </div>
        </div>
        <FileModal Route="TempFile" RemoteDir="CarbonReportFile" title="碳報表檔案管理" ref="fileModal"> </FileModal>
        <Modal :title="`${execUnitOfDetail}碳明細表`" widthRatio="90" ref="modal">
            <template  #body>
                <div class="table-responsive ">
                    <table class="table table1 min910" :style="{'min-width': '1800px'}">
                        <thead>
                            <tr>
                                <th :style="{'min-width': '150px'}">工程名稱</th>   
                                <th>工程預算(千元) </th>
                                <th>提報碳排量(tCO2e)</th>
                                <th>核定碳排量(tCO2e)</th>
                                <th>已發包碳排量(tCO2e)</th>
                                <th>是否發包</th>
                                <th>綠色經費(預算千元)</th>
                                <th :style="{'min-width': '50px'}">綠色經費比例</th>
                                <th :style="{'min-width': '150px'}">樹種(喬木/株)</th>
                                <th :style="{'min-width': '150px'}">樹種(灌木/株)</th>
                                <th>綠化面積(m2)</th>
                                <th>綠化長度(m)</th>
                                <th :style="{'min-width': '150px'}" >綠色經費其他項目</th>
                                <th :style="{'min-width': '250px'}">工程設計減碳策略/長度(m)、範圍(m2)、數量(m3)</th>
                                <th>可拆解率</th>
                                <th>施工計畫(機具優化)減碳量(kgCO2e)</th>
                                <th>備註	</th>
                                <th>是否為疏濬工程</th>

                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in itemsDetail" :key="index">
                                <td class="text-left">{{ item.EngName }}</td>
                                <td class="text-right">{{ item.TotalBudget }} </td>
                                <td class="text-right">{{ item.CarbonDemandQuantity }}</td>
                                <td class="text-right">{{ item.ApprovedCarbonQuantity }}</td>
                                <td class="text-right">{{ item.Co2Total }}</td>
                                <td class="text-left">{{ item.awardStatus }}</td>
                                <td class="text-right">{{ item.GreenFunding }}</td>
                                <td class="text-right">{{ item.greenFundingRate ?? 0 }} %</td>
                                <td class="text-left">{{ item.Tree02931 }}</td>
                                <td class="text-left">{{ item.Tree02932 }}</td>
                                <td class="text-right">{{ item.F1108Area }}</td>
                                <td class="text-right">{{ item.F1109Length }}</td>
                                <td class="text-left"> 
                                    <div class="d-inline-block col-12" tabindex="0" v-b-tooltip :title="item.GreenFundingValueArr[1]">
                                        <a href="" @click.prevent> 再生材料 </a>
                                    </div>
                                <br>
                                <div class="d-inline-block col-12" tabindex="0" v-b-tooltip :title="item.GreenFundingValueArr[3]">
                                    <a href="" @click.prevent> 減碳</a>
                                    </div> <br>
                                    <div class="d-inline-block col-12" tabindex="0" v-b-tooltip :title="item.GreenFundingValueArr[5]">
                                        <a href="" @click.prevent> 營建自動化 </a>
                                    </div><br> 
                                </td>
                                <td class="text-left">{{ item.ReductionStrategy }}</td>
                                <td class="text-right">{{ item.co2TotalRate }}%</td>
                                <td class="text-right">{{item.CarbonReduction}}</td>
                                <td class="text-left">{{ item.Remark }}</td>
                                <td class="text-left">{{ item.DredgingEng ? '是' : '否'}}</td>

                            </tr>
                        </tbody>
                    </table>
                </div>

            </template>
            
        </Modal>
        <div class="table-responsive">
            <table class="table table1 min910" border="0">
                <thead class="insearch">
                    <tr>
                        <th class="sort">排序</th>
                        <th>執行機關</th>
                        <th>工程件數(件)</th>
                        <th>總核定經費(千元)</th>
                        <th>提報碳排量(tCO2e)</th>
                        <th>核定碳排量(tCO2e)</th>
                        <th>已發包(件)</th>
                        <th>已發包碳排量(tCO2e)</th>
                        <th>總綠色經費(千元)</th>
                        <th>綠色經費比例(%)</th>
                        <th>喬木總數量(株)</th>
                        <th>灌木總數量(株)</th>
                        <th>綠化面積(m2)</th>
                        <th>綠化長度(m)</th>
                        <th>平均可拆解率(%)</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td>{{index+1}}</td>
                        <td>{{item.execUnitName}}</td>
                        <td class="text-right"><a href="" @click.prevent="openItemDetail(item)">{{item.engCnt}}</a></td>
                        <td class="text-right">{{item.TotalBudget}}</td>
                        <td class="text-right">{{item.CarbonDemandQuantity}}</td>
                        <td class="text-right">{{item.ApprovedCarbonQuantity}}</td>
                        <td class="text-right"> 
                            <a href="" @click.prevent="openItemDetail(item, true)">
                                {{item.awardCnt}}
                            </a>
                        </td>
                        <td class="text-right">{{item.Co2Total}}</td>
                        <td class="text-right">{{item.GreenFunding}}</td>
                        <td class="text-right">{{item.greenFundingRate}}</td>
                        <td class="text-right">{{item.Tree02931Total}}</td>
                        <td class="text-right">{{item.Tree02932Total}}</td>
                        <td class="text-right">{{item.F1108Area}}</td>
                        <td class="text-right">{{item.F1109Length}}</td>
                        <td class="text-right">{{item.co2TotalRate}}</td>
                        <td></td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>
<script>
    import FileModal from "../../components/FormalUploadModal.vue";
    import Modal from "../../components/Modal.vue";
    export default {
        components : {
            FileModal : FileModal,
            Modal : Modal
        },
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
                itemsDetail : [],
                execUnitOfDetail : ""
            };
        },
        methods: {
            //工程年分
            //工程年分
            async openItemDetail(item , award)
            {
                const {data} = await window.myAjax.post("CEFactor/GetCarbonReoprtDetail", {year :this.selectYear, unit :item.ExecUnitSeq, subUnit :this.selectSubUnit, award : award });
                data.items.forEach(e => e.GreenFundingValueArr = e.GreenFundingValue.split('\n') );
                data.items.forEach(e => e.Tree02931 = e.Tree02931 ? e.Tree02931.replace(',', '\n') : "" );
                data.items.forEach(e => e.Tree02932 = e.Tree02932 ? e.Tree02932.replace(',', '\n') : "" );
                this.itemsDetail = data.items;
                this.execUnitOfDetail = item.execUnitName;
                this.$refs.modal.show = true;

            },

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
            async onYearChange(event) {
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
            async onUnitChange(unitSeq) {
                if (this.selectUnitOptions.length == 0) return;

                this.items = [];
                this.selectedYear = '';
                this.selectedUnit = '';
                this.selectedSubUnit = '';

                this.selectSubUnit = -1
                this.selectSubUnitOptions = [];
                const { data } = await window.myAjax.post('/TenderPlan/GetSubUnitOptions', { year: this.selectYear, parentSeq: unitSeq });
                this.selectSubUnitOptions = data;
                //儲存到session
                sessionStorage.removeItem('selectUnit');
                window.sessionStorage.setItem("selectUnit", this.selectUnit);

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
                window.myAjax.post('/CEFactor/GetCarbonReoprt'
                    , {
                        year: this.selectYear,
                        unit: this.selectUnit,
                        subUnit: this.selectSubUnit
                    })
                    .then(resp => {
                        this.items = resp.data.items;
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
            console.log('mounted() 管制填報表');
            this.getSelectYearOption();
        }
    }
</script>
<style scoped>
    th {
        border-color: #ddd !important;

    }
</style>