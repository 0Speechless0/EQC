<template>
    <div>
        <div class="table-responsive">
            <table class="table table-hover table2 VA-middle">
                <tbody>
                    <tr>
                        <th style="width: 120px;">查詢年度</th>
                        <td>
                            <div class="form-inline">
                                <select v-model="selStartYear" class="form-control">
                                    <option v-for="option in selectYearOptions" v-bind:value="option.Value"
                                        v-bind:key="option.Value">
                                        {{ option.Text }}
                                    </option>
                                </select>~
                                <select v-model="selEndYear" class="form-control">
                                    <option v-for="option in selectYearOptions" v-bind:value="option.Value"
                                        v-bind:key="option.Value">
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
                                <input v-model="selAll" @change="onAllChange" value="All" type="checkbox"
                                    name="ExecutiveAgencyAll" id="ExecutiveAgency_all" class="custom-control-input">
                                <label for="ExecutiveAgency_all" class="custom-control-label">全部</label>
                            </div>
                            <div v-for="(unit, index) in unitList" :key="index"
                                class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="unit.IsSelected" :value="unit.Text" type="checkbox" name="ExecutiveAgency"
                                    :id="`ExecutiveAgency_${index}`" class="custom-control-input">
                                <label :for="`ExecutiveAgency_${index}`" class="custom-control-label">{{ unit.Text
                                }}</label>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th style="width: 150px;">編碼或名稱</th>
                        <td>

                            <input class="form-control col-5" v-model="refCodeKeyWord" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="row justify-content-center">
            <div class="col-12 col-sm-4 col-xl-2 my-2">
                <button @click="getList" role="button" class="btn btn-shadow btn-block btn-outline-secondary"><i
                        class="fas fa-search"></i> 查詢</button>
            </div>
        </div>
        <h5>查詢結果如下，點選檢視可瀏覽該筆標案各工項之碳排放量</h5>
        <div class="row justify-content-between">
            <comm-pagination class="col-12" :recordTotal="recordTotal"
                v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        </div>
        <div class="table-responsive">
            <table class="table table-responsive-md table-hover">
                <thead class="insearch">
                    <tr>
                        <th style="width: 42px;"><strong>項次</strong></th>
                        <th class="text-left"><strong>年度</strong></th>
                        <th><strong>工程名稱</strong></th>
                        <th><strong>執行單位</strong></th>
                        <th><strong>決標經費</strong></th>
                        <th><strong>碳排放量</strong></th>
                        <th class="text-center"><strong>功能</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in engItems" v-bind:key="item.Seq">
                        <td><strong>{{ pageRecordCount * (pageIndex - 1) + index + 1 }}</strong></td>
                        <td class="text-left"><strong>{{ item.EngYear }}</strong></td>
                        <td>{{ item.EngName }}</td>
                        <td>{{ item.ExecUnit }}</td>
                        <td>{{ item.AwardAmount }}</td>
                        <td>{{ item.Co2Total }}</td>
                        <td>
                            <div class="d-flex justify-content-center">
                                <button @click="openModal(item)" class="btn btn-color2 btn-xs mx-1" title="檢視"
                                    data-toggle="modal" data-target="#view01"><i class="fas fa-eye"></i> 檢視</button>
                            </div>

                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <span class="text-G">合計：{{ recordTotal }}件，碳排放量：{{ co2Total }}</span>

        <!-- 小視窗 檢視各工項之碳排放量 -->
        <div class="modal fade" id="view01" data-backdrop="static" data-keyboard="false">
            <div class="modal-dialog modal-xl modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header bg-0 text-white">
                        <h6 class="modal-title font-weight-bold">碳排放量</h6>
                        <button @click="closeModal" type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">×</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <CarbonEmissionItems v-if="showModal" v-bind:targetId="tarEngSeq" :refCodeKeyWord="refCodeKeyWord"></CarbonEmissionItems>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
import { useSelectionStore } from "../../store/SelectionStore.js";
const store = useSelectionStore();

export default {
    data: function () {
        return {
            selectYearOptions: [],
            selStartYear: '',
            selEndYear: '',
            selAll: false,
            selUnits: [],
            engItems: [],
            unitList: [],
            //分頁
            recordTotal: 0,
            pageRecordCount: 30,
            pageIndex: 1,
            co2Total: 0,
            //
            tarEngSeq: null,
            showModal: false,
            refCodeKeyWord: null

        };
    },
    components: {
        CarbonEmissionItems: require('./CE_Items.vue').default,
    },
    methods: {
        openModal(item) {
            this.tarEngSeq = item.Seq;
            this.showModal = true;
        },
        closeModal() {
            this.showModal = false;
        },
        //
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
        onPaginationChange(pInx, pCount) {
            //console.log("pInx:" + this.$refs['pagination'].pageIndex + " pCount:" + pCount);
            this.pageRecordCount = pCount;
            this.pageIndex = pInx;
            this.getList();
        },
        //清單 
        getList() {
            this.selUnits = this.unitList
                .filter(e => e.IsSelected)
                .map(e => e.Text);

            console.log("selUnits", this.selUnits);
            if (this.selUnits.length == 0) {
                alert('請勾選 執行機關');
                return;
            }
            this.total = 0;
            this.co2Total = 0;
            this.engItems = [];

            window.myAjax.post('/EADCarbonEmission/GetList', {
                units: this.selUnits,
                sYear: this.selStartYear,
                eYear: this.selEndYear,
                pageRecordCount: this.pageRecordCount,
                pageIndex: this.pageIndex,
                refCodeKeyWord: this.refCodeKeyWord
            })
                .then(resp => {
                    this.recordTotal = resp.data.total;
                    this.co2Total = resp.data.co2Total;
                    this.engItems = resp.data.items;
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //工程年分
        async getSelectYearOption() {
            const { data } = await window.myAjax.post('/EADCarbonEmission/GetYearOptions');
            this.selectYearOptions = data;
            if (this.selectYearOptions.length > 0) {
                this.selStartYear = this.selectYearOptions[0].Value;
                this.selEndYear = this.selectYearOptions[0].Value;
            }
        },
    },
    async mounted() {
        console.log('mounted() 水利工程淨零碳排分析');
        this.getSelectYearOption();
        this.unitList = await store.GetSelection("Unit/GetUnitList", "Unit");
        this.unitList = this.unitList.filter(e => e.Text.indexOf("清單") == -1)

    }
}
</script>
