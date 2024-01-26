<template>
    <div>
        <form class="form-group">
            <div class="form-row">
                <div class="col-1 mt-3">
                    <select v-model="selectYear" @change="onYearChange($event)" class="form-control">
                        <!--<option selected="selected" :value="-1"> 全部</option>-->
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
                <div class="col-1-sm-3 mt-3 ml-auto bd-highlight align-self-center">
                    <div class="row">
                        <div class="col-6" style="padding-right: 60px;">
                            <a class="p-1" :href="`../EngReport/GetCollection?year=${this.selectYear}&unit=${this.selectUnit}&subUnit=${this.selectSubUnit}&rptType=${this.selectRptType}`" download>
                                <button type="button" class="btn btn-color11-1 btn-block" style="width: max-content;"><i class="fas fa-download"></i>先前檢討會議_提報</button>
                            </a>
                            <!--<button @click="onDownloadPreviousReviewMeeting" type="button" class="btn btn-color11-1 btn-block" style="width: max-content;"><i class="fas fa-download"></i>先前檢討會議_提報</button>-->
                        </div>
                        <div class="col-6">
                            <button @click.prevent="onDownloadNeedAssessment" class="btn btn-color11-3 btn-block" style="width: max-content;"><i class="fas fa-download"></i>提案審查表</button>
                        </div>

                    </div>
                </div>
                <p style="color: red; padding-top: 15px;">狀態分為：需求評估、需求提案核可、需求重新評估、提案審查填報、提案審查核可、提案審查暫緩、經費核可、已核定案件</p>
            </div>
        </form>
        <!-- Nav tabs -->
        <ul class="nav nav-tabs" role="tablist">
            <li class="nav-item"><a class="nav-link active" data-toggle="tab" href="#menu01">全部案件清單</a></li>
            <li v-if="isAdmin==true||isEQCAdmin==true" class="nav-item"><a class="nav-link" data-toggle="tab" href="#menu02">提案填報清單</a></li>
            <li class="nav-item"><a class="nav-link" data-toggle="tab" href="#menu03">優先順序清單</a></li>
            <li class="nav-item"><a class="nav-link" data-toggle="tab" href="#menu04">暫緩清單</a></li>
        </ul>
        <div class="tab-content">
            <div id="menu01" class="tab-pane active">
                <div class="row justify-content-between">
                    <comm-pagination class="col-10" :recordTotal="recordTotalA" v-on:onPaginationChange="onPaginationChangeA"></comm-pagination>
                </div>
                <div class="table-responsive">
                    <table border="0" class="table table1 min910" id="sort">
                        <thead class="insearch">
                            <tr>
                                <th class="sort">項次</th>
                                <th class="number">年度</th>
                                <th>工程名稱</th>
                                <th>執行機關</th>
                                <th>執行單位</th>
                                <th>負責人</th>
                                <th>狀態</th>
                                <th class="text-center">功能</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in itemsA" v-bind:key="item.Seq">
                                <td>{{pageRecordCountA*(pageIndexA-1)+index+1}}</td>
                                <td>{{item.RptYear}}</td>
                                <td><a href="#" title="{{item.RptName}}" v-on:click.stop="onViewEng(item)">{{item.RptName}}</a></td>
                                <td>{{item.ExecUnit}}</td>
                                <td>{{item.ExecSubUnit}}</td>
                                <td>{{item.ExecUser}}</td>
                                <td>{{item.EngState}}</td>
                                <td style="min-width: 105px;">
                                    <div class="row justify-content-center m-0">
                                        <button v-if="item.RptTypeSeq==2||item.RptTypeSeq==4||item.RptTypeSeq==5||item.RptTypeSeq==6" v-on:click.stop="onEditEng(item)" class="btn btn-color11-3 btn-xs mx-1" title="提案審查">
                                            <i class="fas fa-pencil-alt"></i> 提案審查
                                        </button>
                                    </div>
                                </td>
                            </tr>
                            <tr v-if="itemsA==null||itemsA.length==0">
                                <td colspan="8" class="text-center">--查無資料--</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div id="menu02" class="tab-pane">
                <div class="row justify-content-between">
                    <comm-pagination class="col-10" :recordTotal="recordTotalB" v-on:onPaginationChange="onPaginationChangeB"></comm-pagination>
                </div>
                <div class="table-responsive">
                    <table border="0" class="table table1 min910">
                        <thead class="insearch">
                            <tr>
                                <th class="sort">項次</th>
                                <th class="number">年度</th>
                                <th>工程名稱</th>
                                <th>執行機關</th>
                                <th>執行單位</th>
                                <th>負責人</th>
                                <th class="text-center">審查結果</th>
                                <th class="text-center">明細</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in itemsB" v-bind:key="item.Seq">
                                <td>{{pageRecordCountB*(pageIndexB-1)+index+1}}</td>
                                <td>{{item.RptYear}}</td>
                                <td><a href="#" title="{{item.RptName}}" v-on:click.stop="onViewEng(item)">{{item.RptName}}</a></td>
                                <td>{{item.ExecUnit}}</td>
                                <td>{{item.ExecSubUnit}}</td>
                                <td>{{item.ExecUser}}</td>
                                <td>{{item.EngState}}</td>
                                <td style="min-width: 105px;">
                                    <div class="row justify-content-center m-0">
                                        <button v-if="item.IsShow" v-on:click.stop="onEditEng(item)" class="btn btn-color11-3 btn-xs mx-1" title="提案審查">
                                            <i class="fas fa-pencil-alt"></i> 提案審查
                                        </button>
                                    </div>
                                </td>
                            </tr>
                            <tr v-if="itemsB==null||itemsB.length==0">
                                <td colspan="8" class="text-center">--查無資料--</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div id="menu03" class="tab-pane">
                <div class="row justify-content-between">
                    <comm-pagination class="col-10" :recordTotal="recordTotalC" v-on:onPaginationChange="onPaginationChangeC"></comm-pagination>
                </div>
                <div class="table-responsive">
                    <table border="0" class="table table1 min910" id="TableC">
                        <thead class="insearch">
                            <tr>
                                <th class="sort">項次</th>
                                <th style="width:40px;"></th>
                                <th style="width:60px;">順序</th>
                                <th style="width:80px;" class="number">年度</th>
                                <th>工程名稱</th>
                                <th>執行機關</th>
                                <th>執行單位</th>
                                <th>負責人</th>
                                <th class="text-center" style="width:120px;">審查結果</th>
                                <th class="text-center" style="width:50px;">明細</th>
                                <th class="text-center" style="width:50px;">填寫</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in itemsC" v-bind:key="item.Seq">
                                <td>{{pageRecordCountC*(pageIndexC-1)+index+1}}</td>
                                <td><input type="checkbox" v-model.trim="item.IsCheck" v-bind:id="item.IsShow" class="cbItemChk"></td>
                                <td><input v-model.trim="item.ReviewSort" type="text" class="cbItemSort form-control" /></td>
                                <td>{{item.RptYear}}</td>
                                <td><a href="#" title="{{item.RptName}}" v-on:click.stop="onViewEng(item)">{{item.RptName}}</a></td>
                                <td>{{item.ExecUnit}}</td>
                                <td>{{item.ExecSubUnit}}</td>
                                <td>{{item.ExecUser}}</td>
                                <td>{{item.EngState}}</td>
                                <td style="min-width: 105px;">
                                    <div class="row justify-content-center m-0">
                                        <button v-if="item.IsShow" v-on:click.stop="onEditEng(item)" class="btn btn-color11-3 btn-xs mx-1" title="提案審查">
                                            <i class="fas fa-pencil-alt"></i> 提案審查
                                        </button>
                                    </div>
                                </td>
                                <td style="min-width: 105px;">
                                    <div v-if="item.IsShow" class="row justify-content-center m-0">
                                        <button @click="onShowEng(item)" class="btn btn-color11-3 btn-xs mx-1"
                                                data-toggle="modal" data-target="#prepare_edit01" title="填寫">
                                            <i class="fas fa-pencil-alt"></i> 填寫
                                        </button>
                                    </div>
                                </td>
                            </tr>
                            <tr v-if="itemsC==null||itemsC.length==0">
                                <td colspan="11" class="text-center">--查無資料--</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div class="row justify-content-center mt-5">
                    <div class="d-flex">
                        <button v-on:click.stop="onSave()" role="button" class="btn btn-color11-2 btn-xs mx-1">
                            <i class="fas fa-save">&nbsp;儲存順序</i>
                        </button>
                    </div>
                </div>
            </div>
            <div id="menu04" class="tab-pane">
                <div class="row justify-content-between">
                    <comm-pagination class="col-10" :recordTotal="recordTotalD" v-on:onPaginationChange="onPaginationChangeD"></comm-pagination>
                </div>
                <div class="table-responsive">
                    <table border="0" class="table table1 min910">
                        <thead class="insearch">
                            <tr>
                                <th class="sort">項次</th>
                                <th class="number">年度</th>
                                <th>工程名稱</th>
                                <th>執行機關</th>
                                <th>執行單位</th>
                                <th>負責人</th>
                                <th class="text-center">審查結果</th>
                                <th class="text-center">明細</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in itemsD" v-bind:key="item.Seq">
                                <td>{{pageRecordCountD*(pageIndexD-1)+index+1}}</td>
                                <td>{{item.RptYear}}</td>
                                <td><a href="#" title="{{item.RptName}}" v-on:click.stop="onViewEng(item)">{{item.RptName}}</a></td>
                                <td>{{item.ExecUnit}}</td>
                                <td>{{item.ExecSubUnit}}</td>
                                <td>{{item.ExecUser}}</td>
                                <td>{{item.EngState}}</td>
                                <td style="min-width: 105px;">
                                    <div class="row justify-content-center m-0">
                                        <button v-if="item.IsShow" v-on:click.stop="onEditEng(item)" class="btn btn-color11-3 btn-xs mx-1" title="提案審查">
                                            <i class="fas fa-pencil-alt"></i> 提案審查
                                        </button>
                                    </div>
                                </td>
                            </tr>
                            <tr v-if="itemsD==null||itemsD.length==0">
                                <td colspan="8" class="text-center">--查無資料--</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>

        <!-- 小視窗  -->
        <div class="modal fade" id="prepare_edit01" ref="divEditDialog" style="display:none;">
            <div class="modal-dialog modal-xl modal-dialog-centered " style="max-width: fit-content;">
                <div class="modal-content">
                    <div class="card whiteBG mb-4 pattern-F colorset_1">
                        <div class="tab-content">
                            <div class="tab-pane active">
                                <h5></h5>
                                <div class="table table-responsive-md table-hover VA-middle">
                                    預估當年度可支用經費：<input v-model="engReport.EstimatedExpenditureCurrentYear" type="text" style="border: 1px solid #ced4da;" value="">元。
                                    <br>
                                    跨以後年度經費：<input v-model="engReport.ExpensesSubsequentYears" type="text" style="border: 1px solid #ced4da;" value="">元。
                                    <br>
                                    預定辦理期程：<input v-model="engReport.BookingProcess_SY" type="text" style="border: 1px solid #ced4da; width: 50px;" maxlength="3">年 <input v-model="engReport.BookingProcess_SM" type="text" style="border: 1px solid #ced4da; width: 50px; " maxlength="2">月 ~ <input v-model="engReport.BookingProcess_EY" type="text" style="border: 1px solid #ced4da; width: 50px; " maxlength="3">年 <input v-model="engReport.BookingProcess_EM" type="text" style="border: 1px solid #ced4da; width: 50px;" maxlength="2">月
                                    <br>
                                    備註：<input v-model="engReport.Remark" type="text" style="border: 1px solid #ced4da;" value="">
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
                isAdmin: false,
                isEQCAdmin: false,
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
                //分頁A
                recordTotalA: 0,
                pageRecordCountA: 30,
                pageIndexA: 1,
                //分頁B
                recordTotalB: 0,
                pageRecordCountB: 30,
                pageIndexB: 1,
                //分頁C
                recordTotalC: 0,
                pageRecordCountC: 30,
                pageIndexC: 1,
                //分頁D
                recordTotalD: 0,
                pageRecordCountD: 30,
                pageIndexD: 1,
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
                itemsA: [],
                itemsB: [],
                itemsC: [],
                itemsD: [],
                // 新增工程資料
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
                    console.log(this.selectYearOptions[0].Value);
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
                this.onUnitChange(this.selectUnitOptions[0].Value);

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
                }
                if (this.initFlag == 2) {
                    this.initFlag == 1
                    this.selectSubUnit = sessionStorage.getItem('selectERSubUnit');
                }
                this.onSubUnitChange();
                //儲存到session
                sessionStorage.removeItem('selectERUnit');
                window.sessionStorage.setItem("selectERUnit", this.selectUnit);
            },
            //
            onSubUnitChange(event) {
                this.pageIndexA = 1;
                this.pageIndexB = 1;
                this.pageIndexC = 1;
                this.pageIndexD = 1;
                if (this.selectRptType == '') this.getSelectRptTypeOption(3);
                this.getListA();
                this.getListB();
                this.getListC();
                this.getListD();
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
                    this.selectRptType = '0';// this.selectRptTypeOptions[0].Value;
                    if (sessionStorage.getItem('selectRptType') != null) {
                        this.selectRptType = sessionStorage.getItem('selectRptType');
                    }
                    this.onRptTypeChange();
                }
            },
            //
            onRptTypeChange(event) {
                this.pageIndexA = 1;
                this.pageIndexB = 1;
                this.pageIndexC = 1;
                this.pageIndexD = 1;
                this.getListA();
                this.getListB();
                this.getListC();
                this.getListD();

                sessionStorage.removeItem('selectRptType');
                window.sessionStorage.setItem("selectRptType", this.selectRptType);
            },
            onPaginationChangeA(pInx, pCount) {
                //console.log("pInx:" + this.$refs['pagination'].pageIndex + " pCount:" + pCount);
                this.pageRecordCountA = pCount;
                this.pageIndexA = pInx;
                this.getListA();
            },
            onPaginationChangeB(pInx, pCount) {
                //console.log("pInx:" + this.$refs['pagination'].pageIndex + " pCount:" + pCount);
                this.pageRecordCountB = pCount;
                this.pageIndexB = pInx;
                this.getListB();
            },
            onPaginationChangeC(pInx, pCount) {
                //console.log("pInx:" + this.$refs['pagination'].pageIndex + " pCount:" + pCount);
                this.pageRecordCountC = pCount;
                this.pageIndexC = pInx;
                this.getListC();
            },
            onPaginationChangeD(pInx, pCount) {
                //console.log("pInx:" + this.$refs['pagination'].pageIndex + " pCount:" + pCount);
                this.pageRecordCountD = pCount;
                this.pageIndexD = pInx;
                this.getListD();
            },
            async getListA() {
                if (this.selectYear == '' || this.selectUnit == '' || this.selectRptType == '') return;
                if (this.selectSubUnit == null || this.selectUnit == '') this.selectSubUnit = -1;
                window.myAjax.post('/ERProposalReview/GetListA'
                    , {
                        year : this.selectYear,
                        unit: this.selectUnit,
                        subUnit: this.selectSubUnit,
                        rptType: this.selectRptType,
                        pageRecordCount: this.pageRecordCountA,
                        pageIndex: this.pageIndexA
                    })
                    .then(resp => {
                        this.itemsA = resp.data.items;
                        this.recordTotalA = resp.data.pTotal;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            async getListB() {
                if (this.selectYear == '' || this.selectUnit == '' || this.selectRptType == '') return;
                if (this.selectSubUnit == null || this.selectUnit == '') this.selectSubUnit = -1;
                window.myAjax.post('/ERProposalReview/GetListB'
                    , {
                        year: this.selectYear,
                        unit: this.selectUnit,
                        subUnit: this.selectSubUnit,
                        rptType: this.selectRptType,
                        pageRecordCount: this.pageRecordCountB,
                        pageIndex: this.pageIndexB
                    })
                    .then(resp => {
                        this.itemsB = resp.data.items;
                        this.recordTotalB = resp.data.pTotal;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            async getListC() {
                if (this.selectYear == '' || this.selectUnit == '' || this.selectRptType == '') return;
                if (this.selectSubUnit == null || this.selectUnit == '') this.selectSubUnit = -1;
                window.myAjax.post('/ERProposalReview/GetListC'
                    , {
                        year: this.selectYear,
                        unit: this.selectUnit,
                        subUnit: this.selectSubUnit,
                        rptType: this.selectRptType,
                        pageRecordCount: this.pageRecordCountC,
                        pageIndex: this.pageIndexC
                    })
                    .then(resp => {
                        this.itemsC = resp.data.items;
                        this.recordTotalC = resp.data.pTotal;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            async getListD() {
                if (this.selectYear == '' || this.selectUnit == '' || this.selectRptType == '') return;
                if (this.selectSubUnit == null || this.selectUnit == '') this.selectSubUnit = -1;
                window.myAjax.post('/ERProposalReview/GetListD'
                    , {
                        year: this.selectYear,
                        unit: this.selectUnit,
                        subUnit: this.selectSubUnit,
                        rptType: this.selectRptType,
                        pageRecordCount: this.pageRecordCountD,
                        pageIndex: this.pageIndexD
                    })
                    .then(resp => {
                        this.itemsD = resp.data.items;
                        this.recordTotalD = resp.data.pTotal;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onEditEng(item) {
                //window.location = "/ERProposalReview/EditEng?id=" + item.Seq;
                window.myAjax.post('/ERProposalReview/EditEng', { seq: item.Seq })
                    .then(resp => {
                        window.sessionStorage.setItem(window.eqSelNeedAssessmentSeq, item.Seq);
                        console.log(resp.data.Url);
                        window.location.href = resp.data.Url;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //顯示工程
            onShowEng(item) {
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
                window.myAjax.post('/ERProposalReview/UpdateEngReportForPRO', { m: this.engReport })
                    .then(resp => {
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },

            //下載
            onDownloadPreviousReviewMeeting() {
                window.comm.dnFile('/EngReport/GetCollection');
            },
            //下載
            onDownloadNeedAssessment() {
                window.comm.dnFile('/EngReport/DownloadNeedAssessment?year=' + this.selectYear + '&unit=' + this.selectUnit + '&subUnit=' + this.selectSubUnit + '&rptType=' + this.selectRptType);
            },
            onSave() {
                window.myAjax.post('/ERProposalReview/UpdateEngReportSort', { list: this.itemsC })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            alert(resp.data.msg);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onViewEng(item) {
                window.myAjax.post('/ERProposalReview/ViewEng', { seq: item.Seq })
                    .then(resp => {
                        window.sessionStorage.setItem(window.eqSelNeedAssessmentSeq, item.Seq);
                        console.log(resp.data.Url);
                        window.location.href = resp.data.Url;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            strEmpty(str) {
                return window.comm.stringEmpty(str);
            },
        },
        async mounted() {
            console.log('mounted() 提案審查');
            this.isAdmin = localStorage.getItem('isAdmin') == 'True' ? true : false;
            this.isEQCAdmin = localStorage.getItem('isEQCAdmin') == 'True' ? true : false;
            if (this.selectYearOptions.length == 0) {
                this.getSelectYearOption();
            }
    }
}
</script>
