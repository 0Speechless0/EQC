<template>
    <div>
        <h2>基本資料</h2>
        <div class="table-responsive mb-4">
            <table class="table table2 min720 my-0" border="0">
                <tbody>
                    <tr>
                        <th>檢查日期</th>
                        <td>{{si.chsCheckDate+' '+si.CCRPosDesc}}</td>
                    </tr>
                    <tr>
                        <th>檢查項目類別</th>
                        <td>
                            <div class="custom-control custom-radio custom-control-inline">
                                <input v-model="report.CheckItemKind" @change="onDataChange" type="radio" disabled="true" name="CheckItemKind" class="custom-control-input" id="checksort01" value="1">
                                <label class="custom-control-label" for="checksort01">1.施工設備</label>
                            </div>
                            <div class="custom-control custom-radio custom-control-inline">
                                <input v-model="report.CheckItemKind" @change="onDataChange" type="radio" disabled="true" name="CheckItemKind" class="custom-control-input" id="checksort02" value="2">
                                <label class="custom-control-label" for="checksort02">2.材料設備</label>
                            </div>
                            <div class="custom-control custom-radio custom-control-inline">
                                <input v-model="report.CheckItemKind" @change="onDataChange" type="radio" disabled="true" name="CheckItemKind" class="custom-control-input" id="checksort03" value="3">
                                <label class="custom-control-label" for="checksort03">3.施工成品</label>
                            </div>
                            <div class="custom-control custom-radio custom-control-inline">
                                <input v-model="report.CheckItemKind" @change="onDataChange" type="radio" disabled="true" name="CheckItemKind" class="custom-control-input" id="checksort04" value="4">
                                <label class="custom-control-label" for="checksort04">4.施工作業</label>
                            </div>
                            <div class="custom-control custom-radio custom-control-inline">
                                <input v-model="report.CheckItemKind" @change="onDataChange" type="radio" disabled="true" name="CheckItemKind" class="custom-control-input" id="checksort05" value="5">
                                <label class="custom-control-label" for="checksort05">5.文件、紀錄</label>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th>不符合事項分類</th>
                        <td>
                            <div class="custom-control custom-radio custom-control-inline">
                                <input v-model="report.IncompKind" @change="onDataChange" type="radio" disabled="true" name="IncompKind" class="custom-control-input" id="nonconformance01" value="1">
                                <label class="custom-control-label" for="nonconformance01">一般缺失立即改善</label>
                            </div>
                            <div class="custom-control custom-radio custom-control-inline">
                                <input v-model="report.IncompKind" @change="onDataChange" type="radio" disabled="true" name="IncompKind" class="custom-control-input" id="nonconformance04" value="2">
                                <label class="custom-control-label" for="nonconformance04">一般缺失追蹤改善</label>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th>檢查者類別</th>
                        <td>
                            <div class="custom-control custom-radio custom-control-inline">
                                <input v-model="report.CheckerKind" @change="onDataChange" type="radio" disabled="true" name="CheckerKind" class="custom-control-input" id="inspectionUnit01" value="1">
                                <label class="custom-control-label" for="inspectionUnit01">施工抽查（監造單位）</label>
                            </div>
                            <div class="custom-control custom-radio custom-control-inline">
                                <input v-model="report.CheckerKind" @change="onDataChange" type="radio" disabled="true" name="CheckerKind" class="custom-control-input" id="inspectionUnit02" value="2">
                                <label class="custom-control-label" for="inspectionUnit02">自主檢查（承攬廠商）</label>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <h2>不符事項說明</h2>
        <p>不符合事項（由檢查人員填寫）</p>
        <div class="table-responsive mb-4">
            <table class="table table1 min910 my-0 tableLayoutFixed" border="1">
                <thead>
                    <tr>                        
                        <th style="width:150px">管理項目</th>
                        <th style="width:200px">抽查標準（定量定性）</th>
                        <th style="width:200px">實際抽查情形</th>
                        <th style="width:120px">抽查結果</th>
                        <th >照片</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.ControllStSeq" v-show="item.rowShow">
                        <td v-if="item.rowSpan>0" v-bind:rowspan="item.rowSpan">{{item.CheckItem1}}</td>
                        <td v-if="item.CheckFields>=0 && item.rowSpanStd1>0" v-bind:rowspan="item.rowSpanStd1">{{item.Stand1}}</td>
                        <td v-if="item.rowSpan>0" v-bind:rowspan="item.rowSpan">
                            <div contenteditable="true" aria-multiline="true">
                                {{item.CCRRealCheckCond}}
                            </div>
                        </td>
                        <td v-if="item.rowSpan>0" v-bind:rowspan="item.rowSpan">
                            <div>
                                <select v-model="item.CCRCheckResult" disabled="true" class="form-control">
                                    <option value="1">檢查合格</option>
                                    <option value="2">有缺失</option>
                                    <option value="3">無此項目</option>
                                </select>
                                <div class="custom-control custom-checkbox">
                                    <input v-model="item.CCRIsNCR" disabled="true" v-bind:id="'check_NCR_'+index" type="checkbox" class="custom-control-input">
                                    <label class="custom-control-label" v-bind:for="'check_NCR_'+index">NCR</label>
                                </div>
                                <!-- button v-if="report.FormConfirm==0" v-on:click="uploadPhotoModal(item)" v-bind:disabled="report.Seq==null" role="button" class="btn btn-shadow btn-color1 btn-block" v-bind:class="report.Seq>0 ? 'btn-color1' : 'btn-color3'">
                                    上傳照片
                                </button -->
                            </div>
                        </td>
                        <td v-if="item.rowSpan>0" v-bind:rowspan="item.rowSpan">
                            <PhotoListItem v-bind:engMain="engMain" v-bind:si="si" v-bind:ctlSeq="item.ControllStSeq" v-bind:formConfirm="report.FormConfirm" v-bind:ref="'photoList'+item.Seq"></PhotoListItem>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="form-inline">
            <label for="LIFDate" class="m-2">限期改善完成日期：</label>
            <input v-bind:value="report.chsImproveDeadline" ref="chsImproveDeadline" @change="onDateChange(report.chsImproveDeadline, $event, 'chsImproveDeadline')" disabled="true" type="text" class="form-control" placeholder="yyy/mm/dd">
            <b-form-datepicker v-show="false" v-model="chsImproveDeadline" :hide-header="1==1"
                               button-only right size="sm" @context="onDatePicketChange($event, 'chsImproveDeadline')">
            </b-form-datepicker>
        </div>
        <h2>缺失改善處理情形說明</h2>
        <div class="form-group">
            <label for="Description01" class="m-2">一、原因分析(得以附件型式附於本報告)</label>
            <textarea v-model="report.CauseAnalysis" @change="onDataChange" disabled="true" id="CauseAnalysis" class="form-control mb-3" rows="5"></textarea>
        </div>
        <div class="form-group">
            <label for="Description02" class="m-2">二、改善措施</label>
            <textarea v-model="report.Improvement" @change="onDataChange" disabled="true" id="Improvement" class="form-control mb-3" rows="5"></textarea>
        </div>
        <div class="form-group">
            <label for="Description03" class="m-2">三、處理結果(責任者填寫)</label>
            <textarea v-model="report.ProcessResult" @change="onDataChange" disabled="true" id="ProcessResult" class="form-control mb-3" rows="5"></textarea>
        </div>
        <h2>審核結果（由原檢查人員認可）</h2>
        <div class="form-inline my-2">
            <label class="m-2">改善審核結果：</label>
            <select v-model="report.ImproveAuditResult" @change="onImproveAuditResultChange" class="form-control">
                <option value="1">符合</option>
                <option value="2">需再行改善</option>
            </select>
        </div>
        <div class="form-inline my-2">
            <label for="TrackingDate" class="m-2">計畫追蹤日期：</label>
            <input v-bind:value="report.chsProcessTrackDate" ref="chsProcessTrackDate" @change="onDateChange(report.chsProcessTrackDate, $event, 'chsProcessTrackDate')" type="text" class="form-control" placeholder="yyy/mm/dd">
            <b-form-datepicker v-model="chsProcessTrackDate" :hide-header="1==1"
                               button-only right size="sm" @context="onDatePicketChange($event, 'chsProcessTrackDate')">
            </b-form-datepicker>
        </div>
        <div class="form-group my-2">
            <label for="TrackingContent" class="m-2">追蹤行動內容：</label>
            <textarea v-model="report.TrackCont" @change="onDataChange" id="TrackingContent" class="form-control mb-3" rows="5"></textarea>
        </div>
        <div class="form-inline">
            <label for="AgreeCaseClosed" class="m-2">是否同意結案：</label>
            <div class="custom-control custom-radio mx-2">
                <input v-model="report.CanClose" @change="onDataChange" v-bind:disabled="report.ImproveAuditResult==1" type="radio" id="ReportAgreeCaseClosedYes" class="custom-control-input" name="reportCanClose" value="true">
                <label class="custom-control-label" for="ReportAgreeCaseClosedYes">是</label>
            </div>
            <div class="custom-control custom-radio mx-2">
                <input v-model="report.CanClose" @change="onDataChange" v-bind:disabled="report.ImproveAuditResult==1" type="radio" id="ReportAgreeCaseClosedNo" class="custom-control-input" name="reportCanClose" value="false">
                <label class="custom-control-label" for="ReportAgreeCaseClosedNo">否</label>
            </div>
        </div>
        <div class="row justify-content-center">
            <div class="col-12 col-lg-6 mt-3">
                <button v-on:click="onUpdateReport" v-bind:disabled="report.FormConfirm!=1" v-bind:class="{'btn-color1':(report.FormConfirm==1), 'btn-color3':(report.FormConfirm!=1)}" role="button" class="btn btn-shadow btn-block">
                    儲存
                </button>
            </div>
            <!-- div v-if="report.Seq != null" class="col-6 col-lg-3 mt-3">
                <button v-on:click="onFormConfirm" v-bind:disabled="report.FormConfirm!=1" v-bind:class="{'btn-color1':(report.FormConfirm==1), 'btn-color3':(report.FormConfirm!=1)}" role="button" class="btn btn-shadow btn-block">
                    確認
                </button>
            </div -->
        </div>
    </div>
</template>
<script>
    export default {
        props: ['engMain', 'si'],
        components: {
            PhotoListItem: require('../SamplingInspectionRecImprove/PhotoListItem.vue').default,
        },
        watch: {
            si: function () {
                if (this.si.Seq > 0) {
                    this.getRecResult();                    
                } else
                    this.items = [];
            }
        },
        data: function () {
            return {
                hasNCR: false,
                fResultChange: false,
                selectStdItem: -1,
                selectStdItemOption: [],
                items: [],
                report: {},//不符合事項報告
                chsImproveDeadline:'',
                chsProcessTrackDate:'',
            }
        },
        methods: {
            //取得檢驗單紀錄
            getRecResult() {
                this.hasNCR = false,
                this.fResultChange = false;
                this.items = [];
                window.myAjax.post('/SIRIApprove/GetRecResult', { rec: this.si })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.item;
                            var i = 0;
                            for (i = 0; i < this.items.length; i++) {
                                var item = this.items[i];
                                if (item.CCRCheckResult == 2 && item.CCRIsNCR) {
                                    this.hasNCR = true;
                                    this.$emit('ncr', this.hasNCR);
                                    break;
                                }
                            }
                            this.report = resp.data.report;
                            this.chsImproveDeadline = this.toYearDate(this.report.chsImproveDeadline);
                            this.chsProcessTrackDate = this.toYearDate(this.report.chsProcessTrackDate);
                            this.$nextTick(function () {
                                this.fResultChange = false;
                            });
                        } else {
                            alert(resp.data.message);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
                this.fGetRecMode = false;
            },
            isDataChange() {
                return this.fResultChange;
            },
            cleanDataChange() {
                this.fResultChange = false;
            },
            onDataChange() {
                this.fResultChange = true;
            },
            onResultChange(item) {
                if (item) item.ResultItem = 1;
                this.fResultChange = true;
            },
            //改善審核結果
            onImproveAuditResultChange() {
                this.report.CanClose = (this.report.ImproveAuditResult == 1);
            },
            onDateChange(srcDate, event, mode) {
                if (this.si.Seq>0) this.fResultChange = true;
                if (event.target.value.length == 0) {
                    if (mode == 'chsImproveDeadline')
                        this.chsImproveDeadline = '';
                    else if (mode == 'chsProcessTrackDate')
                        this.chsProcessTrackDate = '';
                    return;
                }
                if (!this.isExistDate(event.target.value)) {
                    event.target.value = srcDate;
                    alert("日期錯誤");
                } else {
                    if (mode == 'chsImproveDeadline')
                        this.chsImproveDeadline = this.toYearDate(event.target.value);
                    else if (mode == 'chsProcessTrackDate')
                        this.chsProcessTrackDate = this.toYearDate(event.target.value);

                }
            },
            onDatePicketChange(ctx, mode) {
                if (ctx.selectedDate != null) {
                    if (this.si.Seq > 0) this.fResultChange = true;
                    var d = ctx.selectedDate;
                    var dd = (d.getFullYear() - 1911) + '/' + (d.getMonth() + 1) + '/' + d.getDate();
                    //var y = d.getYear() - 1911;
                    if (mode == 'chsImproveDeadline')
                        this.report.chsImproveDeadline = dd;
                    else if (mode == 'chsProcessTrackDate')
                        this.report.chsProcessTrackDate = dd;
                }
            },
            //中曆轉西元
            toYearDate(dateStr) {
                if (dateStr == null) return null;
                var dateObj = dateStr.split('/'); // yyy/mm/dd
                return new Date(parseInt(dateObj[0]) + 1911, parseInt(dateObj[1]) - 1, parseInt(dateObj[2]));
            },
            //日期檢查
            isExistDate(dateStr) {
                var dateObj = dateStr.split('/'); // yyy/mm/dd
                if (dateObj.length != 3) return false;

                var limitInMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

                var theYear = parseInt(dateObj[0]);
                if (theYear != dateObj[0]) return false;
                var theMonth = parseInt(dateObj[1]);
                if (theMonth != dateObj[1]) return false;
                var theDay = parseInt(dateObj[2]);
                if (theDay != dateObj[2]) return false;
                if (new Date(theYear + 1911, 1, 29).getDate() === 29) { // 是否為閏年?
                    limitInMonth[1] = 29;
                }
                return theDay <= limitInMonth[theMonth - 1];
            },
            onUpdateReport() {
                if (this.report.CanClose == null || this.report.ImproveAuditResult == null) {
                    alert('改善審核結果 / 是否同意結案 等項目必須輸入');
                    return;
                }
                this.report.chsImproveDeadline = this.$refs.chsImproveDeadline.value;
                this.report.chsProcessTrackDate = this.$refs.chsProcessTrackDate.value;

                window.myAjax.post('/SIRIApprove/UpdateReportApprove', { report: this.report })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.fResultChange = false;
                            this.report.FormConfirm = resp.data.formConfirm;
                        }
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        async mounted() {
            console.log('mounted()審查 不符合事項報告');
            var d = new Date();
            var dd = d.getFullYear() + '-' + (d.getMonth() + 1) + '-' + d.getDate();
            this.chsImproveDeadline = dd;
            this.chsProcessTrackDate = dd;
        }
    }
</script>
