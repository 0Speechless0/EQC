<template>
    <div>
        <div class="" id="date_stopAndstart">
            <div class="modal-dialog modal-xl modal-dialog-centered ">
                <div class="modal-content">
                    <div class="modal-header bg-1 text-white">
                        <h6 class="modal-title font-weight-bold">設定停工 / 復工</h6>
                    </div>
                    <div class="modal-body">
                        <table class="table table2">
                            <tbody>
                                <tr>
                                    <th width="180">工程開始變更日期</th>
                                    <td>
                                        {{engMain.chsStartDate}}
                                    </td>
                                    <th>變更後預定完工日期</th>
                                    <td>
                                        <div class="form-inline">
                                            <b-input-group>
                                                <input v-bind:value="engMain.chsSchCompDate" ref="chsSchCompDate" @change="onDateChange(engMain.chsSchCompDate, $event, 'chsSchCompDate')"
                                                       type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                                                <b-form-datepicker v-model="chsSchCompDate" :hide-header="true"
                                                                   button-only right size="sm" @context="onDatePicketChange($event, 'chsSchCompDate')">
                                                </b-form-datepicker>
                                            </b-input-group>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <!-- 停工 -->
                        <table v-if="mode==1" class="table table2">
                            <tbody>
                                <tr width="180">
                                    <th colspan="2" class="bg-1-30">停工</th>
                                </tr>
                                <tr>
                                    <th>停工期間</th>
                                    <td>
                                        <div class="form-inline">
                                            <b-input-group>
                                                <input v-bind:value="reportWork.chsSStopWorkDate" ref="chsSStopWorkDate" @change="onDateChange(reportWork.chsSStopWorkDate, $event, 'chsSStopWorkDate')" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                                                <b-form-datepicker v-model="chsSStopWorkDate" :hide-header="true" button-only right size="sm" @context="onDatePicketChange($event, 'chsSStopWorkDate')">
                                                </b-form-datepicker>
                                            </b-input-group>&nbsp;~&nbsp;
                                            <b-input-group>
                                                <input v-bind:value="reportWork.chsEStopWorkDate" ref="chsEStopWorkDate" @change="onDateChange(reportWork.chsEStopWorkDate, $event, 'chsEStopWorkDate')" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                                                <b-form-datepicker v-model="chsEStopWorkDate" :hide-header="true" button-only right size="sm" @context="onDatePicketChange($event, 'chsEStopWorkDate')">
                                                </b-form-datepicker>
                                            </b-input-group>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <th>停工原因</th>
                                    <td>
                                        <textarea v-model="reportWork.StopWorkReason" maxlength="250" rows="5" class="form-control"></textarea>
                                        <span class="text-B small">※ 停工原因請在250字內說明(包含標點符號)</span>
                                    </td>
                                </tr>
                                <tr>
                                    <th>停工文號</th>
                                    <td>
                                        <input v-model="reportWork.StopWorkNo" maxlength="20" type="text" class="form-control" value="">
                                    </td>
                                </tr>
                                <tr>
                                    <th>核定公文檔案</th>
                                    <td>
                                        <b-form-file v-model="StopWorkApprovalFile" placeholder="瀏覽檔案"></b-form-file>
                                        <span class="text-B small">※ 建議檔案大小勿超過5MB，最大限制為20MB</span>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <!-- 復工 -->
                        <table v-if="mode==2" class="table table-hover table2">
                            <tbody>
                                <tr>
                                    <th colspan="2" class="bg-1-30">復工</th>
                                </tr>
                                <tr>
                                    <th width="180">復工日期</th>
                                    <td>
                                        <div class="form-inline">
                                            <b-input-group>
                                                <input v-bind:value="reportWork.chsBackWorkDate" ref="chsBackWorkDate" @change="onDateChange(reportWork.chsBackWorkDate, $event, 'chsBackWorkDate')" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                                                <b-form-datepicker v-model="chsBackWorkDate" :hide-header="true" button-only right size="sm" @context="onDatePicketChange($event, 'chsBackWorkDate')">
                                                </b-form-datepicker>
                                            </b-input-group>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <th>復工文號</th>
                                    <td>
                                        <input v-model="reportWork.BackWorkNo" maxlength="20" type="text" class="form-control" value="">
                                    </td>
                                </tr>
                                <tr>
                                    <th>核定公文檔案</th>
                                    <td>
                                        <b-form-file v-model="BackWorkApprovalFile" placeholder="瀏覽檔案"></b-form-file>
                                        <span class="text-B small">※ 建議檔案大小勿超過5MB，最大限制為20MB</span>
                                    </td>
                                </tr>
                            </tbody>
                        </table>

                        <div class="row justify-content-center">
                            <div class="col-12 col-sm-4 col-xl-2 my-2">
                                <button v-on:click.stop="onWorkSave" role="button" class="btn btn-block btn-color11-3"> {{mode == 1 ? '新增':'儲存'}} <i class="fas fa-plus"></i></button>
                            </div>
                            <div class="col-12 col-sm-4 col-xl-2 my-2">
                                <button v-on:click.stop="onCloseClick" role="button" class="btn btn-block btn-color9-1"> 取消 <i class="fas fa-times fa-lg"></i></button>
                            </div>
                        </div>
                        <div v-show="mode==2" class="table-responsive mt-3">
                            <table class="table table-responsive-md table-hover">
                                <thead class="insearch">
                                    <tr>
                                        <th><strong>次數</strong></th>
                                        <th><strong>停工日期</strong></th>
                                        <th><strong>停工文號</strong></th>
                                        <th><strong>停工原因</strong></th>
                                        <th><strong>復工日期</strong></th>
                                        <th><strong>復工文號</strong></th>
                                        <th class="text-center"><strong>瀏覽核可公文</strong></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(item, index) in workItems" v-bind:key="item.Seq" :class="{ 'bg-1-30': (engChange.SupDailyReportWorkSeq==item.Seq) }">
                                        <td>{{index+1}}</td>
                                        <td>{{item.SStopWorkDateStr}}</td>
                                        <td>{{item.StopWorkNo}}</td>
                                        <td>{{item.StopWorkReason}}</td>
                                        <td>{{item.BackWorkDateStr}}</td>
                                        <td>{{item.BackWorkNo}}</td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button v-on:click.stop="onDownload(item, 1)" type="button" v-bind:disabled="item.StopWorkApprovalFile == ''" class="btn btn-outline-secondary btn-sm mx-1">停工 <i class="fas fa-eye"></i></button>
                                                <button v-on:click.stop="onDownload(item, 2)" type="button" v-bind:disabled="item.BackWorkApprovalFile == ''" class="btn btn-outline-secondary btn-sm mx-1">復工 <i class="fas fa-eye"></i></button>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['tenderItem', 'mode', 'engChangeType', 'engChange'],
        data: function () {
            return {
                //停復工
                StopWorkApprovalFile: null,
                BackWorkApprovalFile: null,
                reportWork: { },
                workItems: [],
                //
                isDataChange: false,

                editSeq: -99,
                editRecord: {},
                //
                //chsStartDate: '',
                chsSchCompDate: '',
                engMain: { Seq: -1, engChangeType: -1, chsStartDate: '', chsSchCompDate: '' },
                chsSStopWorkDate: '',
                chsEStopWorkDate: '',
                chsBackWorkDate: '',
            };
        },
        methods: {
            //日期
            onDateChange(srcDate, event, mode) {
                if (event.target.value.length == 0) {
                    if (mode == 'chsSchCompDate')
                        this.chsSchCompDate = '';
                    else if (mode == 'chsApprovalDate')
                        this.chsApprovalDate = '';
                    else if (mode == 'chsSStopWorkDate')
                        this.chsSStopWorkDate = '';
                    else if (mode == 'chsEStopWorkDate')
                        this.chsEStopWorkDate = ''; 
                    else if (mode == 'chsBackWorkDate')
                        this.chsBackWorkDate = '';
                    return;
                }
                if (!window.comm.isExistDate(event.target.value)) {
                    event.target.value = srcDate;
                    alert("日期錯誤");
                } else {
                    console.log('onDateChange');
                    let dd = window.comm.toYearDate(event.target.value);
                    if (mode == 'chsSchCompDate')
                        this.chsSchCompDate = dd;
                    else if (mode == 'chsSStopWorkDate')
                        this.chsSStopWorkDate = dd;
                    else if (mode == 'chsEStopWorkDate')
                        this.chsEStopWorkDate = dd;
                    else if (mode == 'chsBackWorkDate')
                        this.chsBackWorkDate = dd;
                }
            },
            onDatePicketChange(ctx, mode) {
                //console.log(ctx);
                if (ctx.selectedDate != null) {
                    var d = ctx.selectedDate;
                    var dd = (d.getFullYear() - 1911) + '/' + (d.getMonth() + 1) + '/' + d.getDate();
                    //var y = d.getYear() - 1911;
                    if (mode == 'chsSchCompDate')
                        this.engMain.chsSchCompDate = dd;
                    else if (mode == 'chsSStopWorkDate') {
                        this.reportWork.chsSStopWorkDate = dd;
                        this.engMain.chsStartDate = dd;
                    } else if (mode == 'chsEStopWorkDate')
                        this.reportWork.chsEStopWorkDate = dd;
                    else if (mode == 'chsBackWorkDate') {
                        if (this.adjSchCompDate()) {
                            this.reportWork.chsBackWorkDate = dd;
                            this.engMain.chsStartDate = dd;
                        }
                    }
                }
            },
            //變更後預定完工日期 調整
            adjSchCompDate() {
                let days = window.comm.calDays(window.comm.toYearDate(this.engChange.chsStartDate), new Date(window.comm.ToDate(this.chsBackWorkDate)+' 00:00:00'));
                if (days <= 0) {
                    this.chsBackWorkDate = '';
                    alert('復工日期必須大於' + this.engChange.chsStartDate);
                    return false;
                }
                this.chsSchCompDate = window.comm.getMoment(this.chsSchCompDate).add(days, 'days').toDate();
                return true;
            },
            //文件下載
            onDownload(item, mode) {
                var fName = '';
                if (mode == 1)
                    fName = item.StopWorkApprovalFile;
                else if (mode == 2)
                    fName = item.BackWorkApprovalFile;
                window.comm.dnFile('/EPCProgressManage/DocDownload?id=' + this.tenderItem.Seq + "&mode=" + mode + "&fn=" + fName);
            },
            onWorkSave() {
                if (this.mode == 1) {
                    if (window.comm.stringEmpty(this.reportWork.chsSStopWorkDate) || window.comm.stringEmpty(this.reportWork.chsEStopWorkDate)) {
                        alert('停工期間 必須填寫');
                        return;
                    }
                    if (window.comm.stringEmpty(this.reportWork.StopWorkReason) || window.comm.stringEmpty(this.reportWork.StopWorkNo)) {
                        alert('停工文號,停工原因 必須填寫');
                        return;
                    }
                    if (this.reportWork.Seq == -1 && this.StopWorkApprovalFile == null) {
                        alert('停工-核定公文檔案必須上傳');
                        return;
                    }
                } else if (this.mode == 2) {
                    if (window.comm.stringEmpty(this.reportWork.chsBackWorkDate) || window.comm.stringEmpty(this.reportWork.BackWorkNo)) {
                        alert('復工日期,復工文號 必須填寫');
                        return;
                    }
                    if (this.reportWork.Seq == -1) {
                        if (this.BackWorkApprovalFile == null) {
                            alert('復工-核定公文檔案必須上傳');
                            return;
                        }
                    } else {
                        if (window.comm.stringEmpty(this.reportWork.BackWorkApprovalFile) && this.BackWorkApprovalFile == null) {
                            alert('復工-核定公文檔案必須上傳');
                            return;
                        }
                    }
                } else {
                    alert("mode資料異常無法作業");
                    return;
                }

                var files = new FormData();
                files.append("engJSON", JSON.stringify(this.engMain));
                files.append("Seq", this.reportWork.Seq);
                files.append("EngMainSeq", this.reportWork.EngMainSeq);
                if (this.mode == 1) {
                    files.append("chsSStopWorkDate", this.reportWork.chsSStopWorkDate);
                    files.append("chsEStopWorkDate", this.reportWork.chsEStopWorkDate);
                    files.append("StopWorkReason", this.reportWork.StopWorkReason);
                    files.append("StopWorkNo", this.reportWork.StopWorkNo);
                    if (this.StopWorkApprovalFile != null)
                        files.append('StopWorkFile', this.StopWorkApprovalFile, this.StopWorkApprovalFile.name);
                } else if (this.mode == 2) {
                    files.append("chsBackWorkDate", this.reportWork.chsBackWorkDate);
                    files.append("BackWorkNo", this.reportWork.BackWorkNo);
                    if (this.BackWorkApprovalFile != null)
                        files.append('BackWorkFile', this.BackWorkApprovalFile, this.BackWorkApprovalFile.name);
                }
                
                window.myAjax.post('/EPCProgressEngChange/AddEngChangeByWorkStop', files,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.$emit('onClose', 1, resp.data.id);
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            initReportWork() {
                this.StopWorkApprovalFile = null;
                this.BackWorkApprovalFile = null;
                this.reportWork = {
                    Seq: -1, EngMainSeq: this.tenderItem.Seq,
                    SStopWorkDate: null, EStopWorkDate: null, StopWorkReason: '', StopWorkNo: '',
                    BackWorkDate: null, BackWorkNo: '', 
                    chsSStopWorkDate: '', chsEStopWorkDate: '', chsBackWorkDate: ''
                };
                this.engMain.Seq = this.tenderItem.Seq;
                this.engMain.ChangeType = this.engChangeType;
                this.engMain.chsStartDate = '';
                this.engMain.chsSchCompDate = this.tenderItem.SchCompDateStr;
                this.chsSchCompDate = window.comm.toYearDate(this.engMain.chsSchCompDate);
                if (this.mode == 2) {
                    this.getWorkList();
                    this.reportWork.Seq = this.engChange.SupDailyReportWorkSeq;
                }
            },
            getWorkList() {
                this.workItems = [];
                window.myAjax.post('/EPCProgressManage/GetWorkList', { id: this.tenderItem.Seq })
                    .then(resp => {
                        this.workItems = resp.data;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onWorkModalOpen() {
                this.getWorkList();
            },
            //
            formatDateStr(d) {
                return window.comm.formatCDateStr(d);
            },
            onCloseClick() {
                this.$emit('onClose', 0);
            },
        },
        mounted() {
            console.log('mounted 停復工');
            this.initReportWork();
            
        }
    }
</script>