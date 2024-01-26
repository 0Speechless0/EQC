<template>
    <div>
        <!-- 小視窗 設定展延工期 -->
        <div class="" id="date_extend">
            <div class="modal-dialog modal-xl modal-dialog-centered ">
                <div class="modal-content">
                    <div class="modal-header bg-1 text-white">
                        <h6 class="modal-title font-weight-bold">設定展延工期</h6>
                    </div>
                    <div class="modal-body">
                        <div class="table-responsive">
                            <table class="table table2">
                                <tbody>
                                    <tr>
                                        <th width="180">工程開始變更日期</th>
                                        <td>
                                            <div class="form-inline">
                                                <b-input-group>
                                                    <input v-bind:value="engMain.chsStartDate" ref="chsStartDate" @change="onDateChange(engMain.chsStartDate, $event, 'chsStartDate')" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                                                    <b-form-datepicker v-model="chsStartDate" :hide-header="true" button-only right size="sm" @context="onDatePicketChange($event, 'chsStartDate')">
                                                    </b-form-datepicker>
                                                </b-input-group>
                                            </div>
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
                            <table class="table table2">
                                <tbody>
                                    <tr>
                                        <th width="180">展延天數</th>
                                        <td >
                                            <div class="input-group mb-2">
                                                <input v-model.number="reportExtension.ExtendDays" @change="onExtendDaysChange" type="number" class="form-control">
                                                <div class="input-group-append">
                                                    <div class="input-group-text">天</div>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>公文核准文號</th>
                                        <td>
                                            <input v-model="reportExtension.ApprovalNo" maxlength="20" type="text" class="form-control" value="">
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>公文核准日期</th>
                                        <td>
                                            <div class="form-inline">
                                                <b-input-group>
                                                    <input v-model="reportExtension.chsApprovalDate" ref="chsApprovalDate" @change="onDateChange(reportExtension.chsApprovalDate, $event, 'chsApprovalDate')"
                                                           type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                                                    <b-form-datepicker v-model="chsApprovalDate" :hide-header="true"
                                                                       button-only right size="sm" @context="onDatePicketChange($event, 'chsApprovalDate')">
                                                    </b-form-datepicker>
                                                </b-input-group>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>原因</th>
                                        <td class="text-left">
                                            <div class="custom-control custom-radio">
                                                <input v-model="reportExtension.ExtendReason" value="1" type="radio" id="customRadio_reason01" name="customRadio_reason" class="custom-control-input">
                                                <label class="custom-control-label" for="customRadio_reason01">發生第17條第5款不可抗力或不可歸責契約當事人之事故</label>
                                            </div>
                                            <div class="custom-control custom-radio">
                                                <input v-model="reportExtension.ExtendReason" value="2" type="radio" id="customRadio_reason02" name="customRadio_reason" class="custom-control-input">
                                                <label class="custom-control-label" for="customRadio_reason02">因天候影響無法施工</label>
                                            </div>
                                            <div class="custom-control custom-radio">
                                                <input v-model="reportExtension.ExtendReason" value="3" type="radio" id="customRadio_reason03" name="customRadio_reason" class="custom-control-input">
                                                <label class="custom-control-label" for="customRadio_reason03">機關要求全部或部分停工</label>
                                            </div>
                                            <div class="custom-control custom-radio">
                                                <input v-model="reportExtension.ExtendReason" value="4" type="radio" id="customRadio_reason04" name="customRadio_reason" class="custom-control-input">
                                                <label class="custom-control-label" for="customRadio_reason04">機關應辦事項未及時辦妥</label>
                                            </div>
                                            <div class="custom-control custom-radio">
                                                <input v-model="reportExtension.ExtendReason" value="5" type="radio" id="customRadio_reason05" name="customRadio_reason" class="custom-control-input">
                                                <label class="custom-control-label" for="customRadio_reason05">由機關自辦或機關之其他廠商之延誤而影響履約進度者</label>
                                            </div>
                                            <div class="custom-control custom-radio">
                                                <input v-model="reportExtension.ExtendReason" value="6" type="radio" id="customRadio_reason06" name="customRadio_reason" class="custom-control-input">
                                                <label class="custom-control-label" for="customRadio_reason06">機關提供之地質資料，與實際情形有重大差異</label>
                                            </div>
                                            <div class="custom-control custom-radio">
                                                <input v-model="reportExtension.ExtendReason" value="7" type="radio" id="customRadio_reason07" name="customRadio_reason" class="custom-control-input">
                                                <label class="custom-control-label" for="customRadio_reason07">因傳染病或政府之行為</label>
                                            </div>
                                            <div class="custom-control custom-radio">
                                                <input v-model="reportExtension.ExtendReason" value="8" type="radio" id="customRadio_reason08" name="customRadio_reason" class="custom-control-input">
                                                <label class="custom-control-label" for="customRadio_reason08">因機關使用或佔用本工程任何部分，但契約另有規定者，不再此限</label>
                                            </div>
                                            <div class="custom-control custom-radio">
                                                <input v-model="reportExtension.ExtendReason" value="9" type="radio" id="customRadio_reason09" name="customRadio_reason" class="custom-control-input">
                                                <label class="custom-control-label" for="customRadio_reason09">其他非可歸責於廠商之情形，經機關認定者</label>
                                            </div>
                                            <input v-model="reportExtension.ExtendReasonOther" maxlength="100" type="text" class="form-control" value="">
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <div class="row justify-content-center">
                            <div class="col-12 col-sm-4 col-xl-2 my-2">
                                <button v-on:click.stop="onExtensionAdd" role="button" class="btn btn-block btn-color11-3"> 新增 <i class="fas fa-plus-square fa-lg"></i></button>
                            </div>
                            <div class="col-12 col-sm-4 col-xl-2 my-2">
                                <button @click="onCloseClick" role="button" class="btn btn-block btn-color9-1" title="取消"> 取消 <i class="fas fa-times fa-lg"></i></button>
                            </div>
                        </div>
                        <!-- div class="table-responsive mt-3">
                            <table class="table table-responsive-md table-hover">
                                <thead class="insearch">
                                    <tr>
                                        <th class="text-left"><strong>展延天數(天)</strong></th>
                                        <th class="text-left"><strong>公文核准文號</strong></th>
                                        <th class="text-left"><strong>公文核准日期</strong></th>
                                        <th class="text-left"><strong>原因</strong></th>
                                        <th class="text-left"><strong>狀態</strong></th>
                                        <th class="text-center"><strong>功能</strong></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(item, index) in extensionItems" v-bind:key="item.Seq">
                                        <template v-if="item.Seq != editSeq">
                                            <td>{{item.ExtendDays}}</td>
                                            <td>{{item.ApprovalNo}}</td>
                                            <td>{{item.ApprovalDateStr}}</td>
                                            <td>{{getExtendReasonText(item)}}<br />{{item.ExtendReasonOther}}</td>
                                            <td>同意</td>
                                            <td>
                                                <div class="d-flex justify-content-center">
                                                    <button @click="onExtensionEdit(item)" class="btn btn-color11-1 btn-xs sharp m-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                                    <button @click="onExtensionDel(item)" class="btn btn-color9-1 btn-xs sharp m-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                                                </div>
                                            </td>
                                        </template>
                                        <template v-if="item.Seq == editSeq">
                                            <td><input v-model.number="editRecord.ExtendDays" type="number" class="form-control"></td>
                                            <td><input v-model.trim="editRecord.ApprovalNo" maxlength="20" type="text" class="form-control"></td>
                                            <td><input v-model="editRecord.ApprovalDate" type="date" class="form-control"></td>
                                            <td>
                                                <select v-model.trim="editRecord.ExtendReason" class="form-control">
                                                    <option v-for="option in extendReasonOpions" v-bind:value="option.Value" v-bind:key="option.Value">{{option.Text}}</option>
                                                </select>
                                                <input v-model.trim="editRecord.ExtendReasonOther" maxlength="100" type="text" class="form-control">
                                            </td>
                                            <td></td>
                                            <td style="min-width: unset;">
                                                <div class="d-flex justify-content-center">
                                                    <button @click="onExtensionUpdate(editRecord)" class="btn btn-color11-2 btn-xs sharp m-1" title="儲存"><i class="fas fa-save"></i></button>
                                                    <button @click="onExtensionEditCancel" class="btn btn-color9-1 btn-xs sharp m-1" title="取消"><i class="fas fa-times"></i></button>
                                                </div>
                                            </td>
                                        </template>
                                    </tr>
                                </tbody>
                            </table>
                        </div -->
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['tenderItem'],
        data: function () {
            return {
                //設定展延工期
                reportExtension: {},
                extensionItems: [],
                extendReasonOpions: [
                    { Value: 1, Text: "發生第17條第5款不可抗力或不可歸責契約當事人之事故" },
                    { Value: 2, Text: "因天候影響無法施工" },
                    { Value: 3, Text: "機關要求全部或部分停工" },
                    { Value: 4, Text: "機關應辦事項未及時辦妥" },
                    { Value: 5, Text: "由機關自辦或機關之其他廠商之延誤而影響履約進度者" },
                    { Value: 6, Text: "機關提供之地質資料，與實際情形有重大差異" },
                    { Value: 7, Text: "因傳染病或政府之行為" },
                    { Value: 8, Text: "因機關使用或佔用本工程任何部分，但契約另有規定者，不再此限" },
                    { Value: 9, Text: "其他非可歸責於廠商之情形，經機關認定者" },
                ],

                //
                isDataChange: false,

                editSeq: -99,
                editRecord: {},
                //
                chsStartDate: '',
                chsSchCompDate: '',
                engMain: { Seq: -1, ChangeType:2, chsStartDate: '', chsSchCompDate: '' },
                chsApprovalDate: '',
            };
        },
        methods: {
            //展延天數變更 20230524
            onExtendDaysChange() {
                if (window.comm.stringEmpty(this.reportExtension.ExtendDays) || this.reportExtension.ExtendDays <= 0) {
                    alert('展延天數錯誤');
                    return;
                }
                this.chsSchCompDate = window.comm.getMoment(this.chsSchCompDate).add(this.reportExtension.ExtendDays, 'days').toDate();
                //let sd = window.comm.toYearDate(this.tenderItem.SchCompDateStr);
                //this.chsSchCompDate = new Date(sd.getTime() + 86400000 * this.reportExtension.ExtendDays);
            },
            //日期
            onDateChange(srcDate, event, mode) {
                if (event.target.value.length == 0) {
                    if (mode == 'chsStartDate')
                        this.chsStartDate = ''; 
                    else if (mode == 'chsSchCompDate')
                        this.chsSchCompDate = '';
                    else if (mode == 'chsApprovalDate')
                        this.chsApprovalDate = '';
                    return;
                }
                if (!window.comm.isExistDate(event.target.value)) {
                    event.target.value = srcDate;
                    alert("日期錯誤");
                } else {
                    console.log('onDateChange');
                    if (mode == 'chsStartDate')
                        this.chsStartDate = window.comm.toYearDate(event.target.value);
                    else if (mode == 'chsSchCompDate')
                        this.chsSchCompDate = window.comm.toYearDate(event.target.value);
                    else if (mode == 'chsApprovalDate')
                        this.chsApprovalDate = window.comm.toYearDate(event.target.value);
                }
            },
            onDatePicketChange(ctx, mode) {
                //console.log(ctx);
                if (ctx.selectedDate != null) {
                    var d = ctx.selectedDate;
                    var dd = (d.getFullYear() - 1911) + '/' + (d.getMonth() + 1) + '/' + d.getDate();
                    //var y = d.getYear() - 1911;
                    if (mode == 'chsStartDate')
                        this.engMain.chsStartDate = dd;
                    else if (mode == 'chsSchCompDate')
                        this.engMain.chsSchCompDate = dd;
                    else if (mode == 'chsApprovalDate')
                        this.reportExtension.chsApprovalDate = dd;
                }
            },
            //設定展延工期
            getExtensionList() {
                this.extensionItems = [];
                window.myAjax.post('/EPCProgressManage/GetExtensionList', { id: this.tenderItem.Seq })
                    .then(resp => {
                        this.extensionItems = resp.data;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            getExtendReasonText(item) {
                let inx = item.ExtendReason
                if (inx == 1)
                    return "發生第17條第5款不可抗力或不可歸責契約當事人之事故";
                else if (inx == 2)
                    return "因天候影響無法施工";
                else if (inx == 3)
                    return "機關要求全部或部分停工";
                else if (inx == 4)
                    return "機關應辦事項未及時辦妥";
                else if (inx == 5)
                    return "由機關自辦或機關之其他廠商之延誤而影響履約進度者";
                else if (inx == 6)
                    return "機關提供之地質資料，與實際情形有重大差異";
                else if (inx == 7)
                    return "因傳染病或政府之行為";
                else if (inx == 8)
                    return "因機關使用或佔用本工程任何部分，但契約另有規定者，不再此限";
                else if (inx == 9)
                    return "其他非可歸責於廠商之情形，經機關認定者";
                else
                    return "";
            },
            onExtensionAdd() {
                this.engMain.chsStartDate = this.$refs['chsStartDate'].value;
                if (window.comm.stringEmpty(this.engMain.chsStartDate)) {
                    this.$swal("工程開始變更日期 必須輸入");
                    return;
                }
                this.engMain.chsSchCompDate = this.$refs['chsSchCompDate'].value;
                if (window.comm.stringEmpty(this.engMain.chsSchCompDate)) {
                    this.$swal("預計完工日期 必須輸入");
                    return;
                }
                
                if (window.comm.stringEmpty(this.reportExtension.ExtendDays) || this.reportExtension.ExtendDays<=0) {
                    alert('展延天數錯誤');
                    return;
                }
                if (window.comm.stringEmpty(this.reportExtension.ApprovalNo)) {
                    alert('公文核准文號錯誤');
                    return;
                }
                if (window.comm.stringEmpty(this.reportExtension.chsApprovalDate)) {
                    alert('公文核准日期誤');
                    return;
                }
                if (window.comm.stringEmpty(this.reportExtension.ExtendReason)) {
                    alert('展原因錯誤');
                    return;
                }
                this.reportExtension.EngMainSeq = this.tenderItem.Seq;
                window.myAjax.post('/EPCProgressEngChange/AddEngChangeByExtension', { eng: this.engMain, extension: this.reportExtension })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.$emit('onClose', 1, resp.data.id);
                        } else
                            this.$swal(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
                /*window.myAjax.post('/EPCProgressManage/ExtensionAdd', { item: this.reportExtension })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.isDataChange = true;
                            this.initReportExtension();
                            this.getExtensionList();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });*/
            },
            //編輯延展工期紀錄 s20230310
            onExtensionEdit(item) {
                if (this.editSeq > -99) return;
                this.editRecord = Object.assign({}, item);
                this.editRecord.ApprovalDate = this.editRecord.ApprovalDateStr;
                this.editSeq = this.editRecord.Seq;
            },
            onExtensionEditCancel() {
                this.editSeq = -99;
                this.getExtensionList();
            },
            onExtensionUpdate(item) {
                if (window.comm.stringEmpty(item.ExtendDays)) {
                    alert('展延天數錯誤');
                    return;
                }
                if (window.comm.stringEmpty(item.ApprovalNo)) {
                    alert('公文核准文號錯誤');
                    return;
                }
                if (window.comm.stringEmpty(item.ApprovalDate)) {
                    alert('公文核准日期誤');
                    return;
                }
                if (window.comm.stringEmpty(item.ExtendReason)) {
                    alert('展原因錯誤');
                    return;
                }
                window.myAjax.post('/EPCProgressManage/ExtensionUpdate', { item: item })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.isDataChange = true;
                            this.initReportExtension();
                            this.onExtensionEditCancel();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onExtensionDel(item) {
                window.myAjax.post('/EPCProgressManage/ExtensionDel', { id: item.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.isDataChange = true;
                            this.getExtensionList();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            initReportExtension() {
                this.reportExtension = { Seq: -1, EngMainSeq: -1, ExtendDays: null, ApprovalNo: '', chsApprovalDate: '', ExtendReason: null, ExtendReasonOther: '' };
                this.engMain.Seq = this.tenderItem.Seq;
                this.engMain.chsStartDate = '';
                this.engMain.chsSchCompDate = this.tenderItem.SchCompDateStr;
                this.chsSchCompDate = window.comm.toYearDate(this.engMain.chsSchCompDate);
            },
            onExtensionModalOpen() {
                this.getExtensionList();
            },
            //事件
            onCloseClick() {
                this.$emit('onClose', 0);
            },
        },
        mounted() {
            console.log('mounted 設定展延工期');
            this.initReportExtension();
        }
    }
</script>