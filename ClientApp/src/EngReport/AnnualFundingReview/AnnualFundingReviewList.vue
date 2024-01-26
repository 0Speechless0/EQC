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
            </div>
            <p style="color: red; padding-top: 15px;">狀態分為：需求評估、需求提案核可、需求重新評估、提案審查填報、提案審查核可、提案審查暫緩、經費核可、已核定案件</p>
        </form>
        <div class="row justify-content-between">
            <comm-pagination class="col-10" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        </div>
        <div v-for="(item, index) in items" v-bind:key="item.Seq">
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
                            <th>狀態</th>
                            <th>展開</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>{{pageRecordCount*(pageIndex-1)+index+1}}</td>
                            <td>{{item.RptYear}}</td>
                            <td><a href="#" title="{{item.RptName}}" v-on:click.stop="onViewEng(item)">{{item.RptName}}</a></td>
                            <td>{{item.ExecUnit}}</td>
                            <td>{{item.ExecSubUnit}}</td>
                            <td>{{item.ExecUser}}</td>
                            <td>{{item.EngState}}</td>
                            <td class="d-flex justify-content-center" v-on:click.stop="setShowHide(index,item)">
                                <a v-if="item.RptTypeSeq==5||item.RptTypeSeq==7" href="javascript:void(0)" class="btn btn-color11-3 btn-xs mx-1" title="展開"><i class="fas fa-expand"> 展開/隱藏</i></a>
                            </td>
                        </tr>
                        <tr v-if="items==null||items.length==0">
                            <td colspan="8" class="text-center">--查無資料--</td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div v-if="item.IsShow" class="tab-content" id="searchResult-{{item.Seq}}">
                <div class="tab-pane active">
                    <div class="table-responsive">
                        <table class="table table1" id="addnew998">
                            <thead class="insearch">
                                <tr>
                                    <th style="width: 100px;"><strong>年度</strong></th>
                                    <th style="width: 200px;"><strong>核定經費(元)</strong></th>
                                    <th style="width: 200px;"><strong>需球碳排量(tCO2e)</strong></th>
                                    <th style="width: 200px;"><strong>核定碳排量(tCO2e)</strong></th>
                                    <th style="width: 200px;"><strong>參考碳排量</strong></th>
                                    <th style="width: 200px;"><strong>當年度支用經費(元)</strong></th>
                                    <th style="width: 200px;"><strong>審核意見</strong></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td style="text-align: center;">{{item.RptYear}}</td>
                                    <td style="text-align: center;">
                                        <div v-if="!item.edit">{{item.ApprovedFund}}</div>
                                        <input v-if="item.edit" type="text" v-model.trim="item.ApprovedFund" class="form-control" @click="setRefCarbonEmission(item,index)" v-on:keyup="setRefCarbonEmission(item,index)" style="text-align: right;" />
                                    </td>
                                    <td style="text-align: center;">{{item.DemandCarbonEmissions}}頓</td>
                                    <td style="text-align: center;">
                                        <div v-if="!item.edit">{{item.ApprovedCarbonEmissions}}</div>
                                        <input v-if="item.edit" type="text" v-model.trim="item.ApprovedCarbonEmissions" class="form-control" style="text-align: right;" />
                                    </td>
                                    <td style="text-align: center;">{{item.RefCarbonEmission}}</td>
                                    <td style="text-align: center;">
                                        <div v-if="!item.edit">{{item.Expenditure}}</div>
                                        <input v-if="item.edit" type="text" v-model.trim="item.Expenditure" class="form-control" style="text-align: right;" />
                                    </td>
                                    <td style="text-align: center;">
                                        <div v-if="!item.edit">{{item.ResolutionAuditOpinionName}}</div>
                                        <div v-if="item.edit" style="display: flex; justify-content: center;">
                                            <div class="custom-control custom-radio custom-control-inline" style="padding-top: 10px;">
                                                <input v-model="item.ResolutionAuditOpinion" v-bind:value="1" type="radio" name="QA" id="QA07" class="custom-control-input">
                                                <label for="QA07" class="custom-control-label">核可</label>
                                            </div><br>
                                            <div class="custom-control custom-radio custom-control-inline" style="padding-top: 10px;">
                                                <input v-model="item.ResolutionAuditOpinion" v-bind:value="2" type="radio" name="QA" id="QA08" class="custom-control-input">
                                                <label for="QA08" class="custom-control-label">暫緩</label>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <table class="table table1" id="addnew998">
                            <thead class="insearch">
                                <tr>
                                    <th style="width: 1000px;"><strong>決議</strong></th>
                                    <th style="width: 150px;"><strong>功能</strong></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>
                                        <div v-if="!item.edit">{{item.Resolution}}</div>
                                        <textarea v-if="item.edit" class="col-12 form-control" v-model="item.Resolution" style="min-width: auto;">同意辦理，本案可視需求於契約訂定擴充條款，惟啟動契約擴充前需先行函報本署籌措財源。</textarea>
                                    </td>
                                    <td>
                                        <div class="d-flex justify-content-center">
                                            <a href="#" v-if="!item.edit" v-on:click.stop="item.edit=!item.edit" class="btn btn-color11-3 btn-xs mx-1" title="編輯"><i class="fas fa-pencil-alt"></i> 編輯</a>
                                            <a href="#" v-if="item.edit" v-on:click.stop="onSaveRecord(item)" class="btn btn-color11-2 btn-xs mx-1"><i class="fas fa-save"></i> 儲存</a>
                                            <a href="#" v-if="item.edit" v-on:click.stop="item.edit=false" class="btn btn-color9-1 btn-xs mx-1" title="取消"><i class="fas fa-times"></i> 取消</a>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <p style="color: red;">核定碳排量=需求碳排量 * 0.7</p>
                        <p style="color: red;">參考碳排量=核定經費(元)*回歸曲線0.38*物調(82.156/前一年的平均新指數)</p>
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
                if (this.selectRptType == '') this.getSelectRptTypeOption(4);
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
                    this.selectRptType = '0';// this.selectRptTypeOptions[0].Value;
                    if (sessionStorage.getItem('selectRptType') != null) {
                        this.selectRptType = sessionStorage.getItem('selectRptType');
                    }
                    this.onRptTypeChange();
                }
            },
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
                window.myAjax.post('/ERAnnualFundingReview/GetList'
                    , {
                        year: this.selectYear,
                        hasCouncil: this.hasCouncilOption,
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
            editEng(item) {
                //window.location = "/ERNeedAssessment/Edit?id=" + item.Seq;
                window.myAjax.post('/ERNeedAssessment/EditEng', { seq: item.Seq })
                    .then(resp => {
                        window.sessionStorage.setItem(window.eqSelNeedAssessmentSeq, item.Seq);
                        console.log(resp.data.Url);
                        window.location.href = resp.data.Url;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            setShowHide(index, item, divName) {
                if (item.IsShow) {
                    this.items[index].IsShow = false;
                } else {
                    this.items[index].IsShow = true;
                }
            },
            onSaveRecord(uItem) {
                //console.log(uItem);
                if (this.strEmpty(uItem.ApprovedFund) || this.strEmpty(uItem.ApprovedCarbonEmissions) || this.strEmpty(uItem.Expenditure)) {
                    alert('核定經費(千元), 核定碳排量(tCO2e), 當年度支用經費(千元) 必須輸入!');
                    return;
                }
                window.myAjax.post('/ERAnnualFundingReview/UpdateEngReport', { m: uItem })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            uItem.edit = false;
                            if (uItem.ResolutionAuditOpinion == '1')
                                uItem.ResolutionAuditOpinionName = '核可';
                            else
                                uItem.ResolutionAuditOpinionName = '暫緩';
                            //this.getList();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            strEmpty(str) {
                return window.comm.stringEmpty(str);
            },
            setRefCarbonEmission(item, index) {
                console.log('---A---');
                console.log((item.ApprovedFund * 0.0001));
                console.log(((item.ApprovedFund * 0.0001) * item.RegressionCurve));
                console.log((((item.ApprovedFund * 0.0001) * item.RegressionCurve) * item.PriceAdjustmentIndex).toFixed(2));
                let refCarbonEmission = (((item.ApprovedFund * 0.0001) * item.RegressionCurve) * item.PriceAdjustmentIndex).toFixed(2);
                console.log(refCarbonEmission);
                console.log('---B---');
                this.items[index].RefCarbonEmission = refCarbonEmission;
            },
            onViewEng(item) {
                window.myAjax.post('/ERAnnualFundingReview/ViewEng', { seq: item.Seq })
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
        async mounted() {
            console.log('mounted() 年度經費檢討會議');
            if (this.selectYearOptions.length == 0) {
                this.getSelectYearOption();
            }
        }
    }
</script>
