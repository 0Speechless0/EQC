<template>
    <div>
        <form class="form-group">
            <div class="form-row">
                <div class="col-1 mt-3">
                    <select v-model="selectYear" @change="onYearChange($event)" class="form-control">
                        <option selected="selected" :value="-1"> 全部</option>
                        <option v-for="option in selectYearOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-3 mt-3">
                    <select v-model="selectUnit" @change="onUnitChange(selectUnit)" class="form-control">
                        <option v-for="option in selectUnitOptions" v-bind:value="option.Value"
                                v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-2 mt-3">
                    <select v-model="selectSubUnit" @change="onSubUnitChange($event)" class="form-control">
                        <option v-for="option in selectSubUnitOptions" v-bind:value="option.Value"
                                v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-1 col-sm-2 mt-3">
                    <select v-model="selectRptType" @change="onRptTypeChange($event)" class="form-control">
                        <option v-for="option in selectRptTypeOptions" v-bind:value="option.Value"
                                v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="ml-auto bd-highlight align-self-center" style="padding-right: 15px;">
                    <button @click.prevent="onDownloadNeedAssessment" class="btn btn-color11-1 btn-block">
                        <i class="fas fa-download"></i>提案需求評估表
                    </button>
                </div>
                <p style="color: red; padding-top: 15px;">狀態分為：需求評估、需求提案核可、需求重新評估、提案審查填報、提案審查核可、提案審查暫緩、經費核可、已核定案件</p>
            </div>
        </form>
        <div class="row justify-content-between">
            <comm-pagination class="col-10" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        </div>
        <div class="table-responsive">
            <table border="0" class="table table1 min910">
                <thead class="insearch">
                    <tr>
                        <th class="sort">項次</th>
                        <th class="number">年度</th>
                        <th>案由</th>
                        <th>執行機關</th>
                        <th>執行單位</th>
                        <th>負責人</th>
                        <th>狀態</th>
                        <th class="text-center">功能</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td>{{pageRecordCount*(pageIndex-1)+index+1}}</td>
                        <td>{{item.RptYear}}</td>
                        <td><a href="#" :title="item.RptName" v-on:click.stop="onViewEng(item)">{{item.RptName}}</a></td>
                        <td>{{item.ExecUnit}}</td>
                        <td>{{item.ExecSubUnit}}</td>
                        <td>{{item.ExecUser}}</td>
                        <td>{{item.EngState}}</td>
                        <td style="min-width: 105px;">
                            <div v-if="item.IsShow" class="row justify-content-center m-0">
                                <button @click="onEditEng(item)" class="btn btn-color11-3 btn-xs mx-1"
                                        data-toggle="modal" data-target="#prepare_edit01" title="需求審查">
                                    <i class="fas fa-pencil-alt"></i> 需求審查
                                </button>
                            </div>
                        </td>
                    </tr>
                    <tr v-if="items==null||items.length==0">
                        <td colspan="8" class="text-center">--查無資料--</td>
                    </tr>
                </tbody>
            </table>
        </div>
        <!-- 小視窗  -->
        <div class="modal fade" id="prepare_edit01" ref="divEditDialog" style="display:none;">
            <div class="modal-dialog modal-xl modal-dialog-centered " style="max-width: fit-content;">
                <div class="modal-content">
                    <div class="card whiteBG mb-4 pattern-F colorset_1">
                        <div class="tab-content">
                            <div class="tab-pane active">
                                <h5>評估結果</h5>
                                <div class="table table-responsive-md table-hover VA-middle">
                                    <div class="custom-control custom-radio custom-control-inline">
                                        <input v-model="engReport.EvaluationResult" v-bind:value="1" type="radio" name="QA" id="QA01" class="custom-control-input" checked>
                                        <label for="QA01" class="custom-control-label">
                                            確有工程執行需求，接續研擬設計概念<br>
                                            1.	預計工程內容：<input v-model="engReport.ER1_1" type="text" style="border: 1px solid #ced4da;width: 650px;" value="" maxlength="50"><br>
                                            2.	經費：<input v-model="engReport.ER1_2" type="text" style="border: 1px solid #ced4da;" value="">元。
                                        </label>
                                    </div>
                                    <br>
                                    <div class="custom-control custom-radio custom-control-inline" style="padding-top: 10px;">
                                        <input v-model="engReport.EvaluationResult" v-bind:value="2" type="radio" name="QA" id="QA02" class="custom-control-input">
                                        <label for="QA02" class="custom-control-label">
                                            需辦理整體規劃或研究，惟因防洪考量需先辦理短期工程，接續辦理設計概念研擬，另循程序提出規劃需求<br>
                                            1.	預計工程內容：<input v-model="engReport.ER2_1" type="text" style="border: 1px solid #ced4da; width: 82%;" value="" maxlength="50"><br>
                                            2.	預計規劃課題：<input v-model="engReport.ER2_2" type="text" style="border: 1px solid #ced4da;" value="">
                                        </label>
                                    </div>
                                    <br>
                                    <div class="custom-control custom-radio custom-control-inline" style="padding-top: 10px;">
                                        <input v-model="engReport.EvaluationResult" v-bind:value="3" type="radio" name="QA" id="QA03" class="custom-control-input">
                                        <label for="QA03" class="custom-control-label">需辦理整體規劃或研究，另循程序提出需求，預計規劃課題：<input v-model="engReport.ER3" type="text" style="border: 1px solid #ced4da;" value=""></label>
                                    </div>
                                    <br>
                                    <div class="custom-control custom-radio custom-control-inline" style="padding-top: 10px;">
                                        <input v-model="engReport.EvaluationResult" v-bind:value="4" type="radio" name="QA" id="QA04" class="custom-control-input">
                                        <label for="QA04" class="custom-control-label">待解決事項屬管理問題，採管理手段處理，預計管理措施：<input v-model="engReport.ER4" type="text" style="border: 1px solid #ced4da;" value=""></label>
                                    </div>
                                    <br>
                                    <div class="custom-control custom-radio custom-control-inline" style="padding-top: 10px;">
                                        <input v-model="engReport.EvaluationResult" v-bind:value="5" type="radio" name="QA" id="QA05" class="custom-control-input">
                                        <label for="QA05" class="custom-control-label">經評估後無辦理需求</label>
                                    </div>
                                    <br>
                                    <div class="custom-control custom-radio custom-control-inline" style="padding-top: 10px;">
                                        <input v-model="engReport.EvaluationResult" v-bind:value="6" type="radio" name="QA" id="QA06" class="custom-control-input">
                                        <label for="QA06" class="custom-control-label">其他<input v-model="engReport.ER6" type="text" style="border: 1px solid #ced4da;" value=""></label>
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <button type="button" class="btn btn-color3" data-dismiss="modal">關閉</button>
                                    <button @click="onSaveEng" type="button" class="btn btn-color11-1">儲存 <i class="fas fa-save"></i></button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                //使用者單位資訊
                userUnit: null,
                userUnitSub: '',
                userName: '',
                unitName: '',
                unitSubName: '',
                rptYear: '',
                rptName: '',
                initFlag: 0,
                hasCouncilOption: -1,
                //分頁
                recordTotal: 0,
                pageRecordCount: 30,
                pageIndex: 1,
                //pageIndexOptions:[],
                //選項
                selectYear: '',
                selectYearOptions: [],
                //機關
                selectUnit: '',
                selectUnitOptions: [],
                //機關單位
                selectSubUnit: -1,
                selectSubUnitOptions: [],
                //狀態
                selectRptType: '',
                selectRptTypeOptions: [],
                items: [],
                // 工程資料
                engReport: {},
            };
        },
        methods: {
            //取得使用者單位資訊
            getUserUnit() {
                window.myAjax.post('/EngReport/GetUserUnit')
                    .then(resp => {
                        this.userUnit = resp.data.unit;
                        this.userUnitSub = resp.data.unitSub;
                        this.userName = resp.data.userName;
                        this.unitName = resp.data.unitName;
                        this.unitSubName = resp.data.unitSubName;

                        if (sessionStorage.getItem('selectERUnit') == null) {
                            this.selectUnit = this.userUnit;
                            this.onUnitChange(this.selectUnit);
                        } else {
                            this.GetSession();
                        }

                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //工程年度
            async getSelectYearOption() {
                const { data } = await window.myAjax.post('/EngReport/GetYearOptions');
                this.selectYearOptions = data;
                if (this.selectYearOptions.length > 0) {
                    this.selectYear = this.selectYearOptions[0].Value;
                    if (sessionStorage.getItem('selectERYear') != null) {
                        this.selectYear = sessionStorage.getItem('selectERYear');
                    }
                    this.onYearChange();
                }
            },
            async onYearChange(event) {
                this.items = [];
                //工程機關
                this.selectUnit = '';
                this.selectUnitOptions = [];

                this.selectSubUnit = -1;
                this.selectSubUnitOptions = [];
                const { data } = await window.myAjax.post('/EngReport/GetUnitOptions', { year: this.selectYear });
                this.selectUnitOptions = data;
                this.selectUnit = this.selectUnitOptions[0].Value;
                if (this.userUnit == null) this.getUserUnit();

                sessionStorage.removeItem('selectERYear');
                window.sessionStorage.setItem("selectERYear", this.selectYear);
            },
            async onUnitChange(unitSeq) {
                if (this.selectUnitOptions.length == 0) return;

                this.items = [];
                this.selectSubUnit = -1
                this.selectSubUnitOptions = [];
                const { data } = await window.myAjax.post('/EngReport/GetSubUnitOptions', { year: this.selectYear, parentSeq: unitSeq });
                this.selectSubUnitOptions = data;
                this.selectSubUnit = this.selectSubUnitOptions[0].Value;
                if (this.initFlag == 0) {
                    this.initFlag == 1
                    //this.selectSubUnit = this.userUnitSub;
                    this.onSubUnitChange();
                }

                if (this.initFlag == 2) {
                    this.initFlag == 1
                    this.selectSubUnit = sessionStorage.getItem('selectERSubUnit');
                    this.onSubUnitChange();
                }
                //儲存到session
                sessionStorage.removeItem('selectERUnit');
                window.sessionStorage.setItem("selectERUnit", this.selectUnit);
            },
            //
            onSubUnitChange(event) {
                this.pageIndex = 1;
                if (this.selectRptType == '') this.getSelectRptTypeOption(2);
                this.getList();
                sessionStorage.removeItem('selectERERSubUnit');
                window.sessionStorage.setItem("selectERSubUnit", this.selectSubUnit);

            },
            GetSession() {
                //
                this.selectUnit = sessionStorage.getItem('selectERUnit');
                this.onUnitChange(this.selectUnit);
                //
                this.initFlag = 2;
                this.selectSubUnit = sessionStorage.getItem('selectERSubUnit');
                this.onSubUnitChange(this.selectSubUnit);

            },
            //工程狀態
            async getSelectRptTypeOption(funcNo) {
                const { data } = await window.myAjax.post('/EngReport/GetRptTypeOptions', { funcNo: funcNo });
                this.selectRptTypeOptions = data;
                this.selectRptTypeOptions.splice(0, 0, { Text: '全部', Value: '0' });
                if (this.selectRptTypeOptions.length > 0) {
                    this.selectRptType = '2';// this.selectRptTypeOptions[0].Value;
                    if (sessionStorage.getItem('selectRptType') != null) {
                        this.selectRptType = sessionStorage.getItem('selectRptType');
                    }
                    this.onRptTypeChange();
                }
            },
            //
            onRptTypeChange(event) {
                this.pageIndex = 1;
                this.getList();

                sessionStorage.removeItem('selectRptType');
                window.sessionStorage.setItem("selectRptType", this.selectRptType);
            },
            onPaginationChange(pInx, pCount) {
                //console.log("pInx:" + this.$refs['pagination'].pageIndex + " pCount:" + pCount);
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
                this.getList();
            },
            onCouncilOptionChange(event) {
                //console.log("pInx:" + this.$refs['pagination'].pageIndex + " pCount:" + pCount);
                this.getList();
            },
            async getList() {
                if (this.selectYear == '' || this.selectUnit == '' || this.selectRptType == '') return;
                if (this.selectSubUnit == null || this.selectUnit == '') this.selectSubUnit = -1;
                window.myAjax.post('/ERProposalWork/GetList'
                    , {
                        year: this.selectYear,
                        unit: this.selectUnit,
                        subUnit: this.selectSubUnit,
                        rptType: this.selectRptType,
                        pageRecordCount: this.pageRecordCount,
                        pageIndex: this.pageIndex
                    })
                    .then(resp => {
                        this.items = resp.data.items;
                        this.recordTotal = resp.data.pTotal;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //編輯工程
            onEditEng(item) {
                this.tarItem = {};
                window.myAjax.post('/ERNeedAssessment/GetEngReport', { id: item.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engReport = resp.data.item;
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //儲存工程
            onSaveEng() {
                window.myAjax.post('/ERProposalWork/UpdateEngReport', { m: this.engReport })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getList();
                            //document.getElementById('btnCloseEditModal').click();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //下載
            onDownloadNeedAssessment() {
                window.comm.dnFile('/EngReport/DownloadNeedAssessmentVF?year=' + this.selectYear + '&unit=' + this.selectUnit + '&subUnit=' + this.selectSubUnit + '&rptType=' + this.selectRptType);
            },
            onViewEng(item) {
                window.myAjax.post('/ERNeedAssessment/ViewEng', { seq: item.Seq })
                    .then(resp => {
                        window.sessionStorage.setItem(window.eqSelNeedAssessmentSeq, item.Seq);
                        console.log(resp.data.Url);
                        window.location.href = resp.data.Url;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        mounted() {
            console.log('mounted() 提案工作會議123');
            if (this.selectYearOptions.length == 0) {
                this.getSelectYearOption();
            }
        }
    }
</script>