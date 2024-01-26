<template>
    <div class="">
        <h2>矯正與預防措施執行情形</h2>
        <div class="form-group">
            <label for="NCRdescription01" class="m-2">一、缺失事項</label>
            <textarea v-model="ncr.MissingItem" @change="onDataChange" id="NCRdescription01" class="form-control mb-3" rows="5"></textarea>
        </div>
        <div class="form-group">
            <label for="NCRdescription02" class="m-2">二、原因分析</label>
            <textarea v-model="ncr.CauseAnalysis" @change="onDataChange" id="NCRdescription02" class="form-control mb-3" rows="5"></textarea>
        </div>
        <label class="m-2">三、矯正(改善)及預防措施(品管人員提出)</label>
        <div class="form-group">
            <label for="NCRdescription03-1" class="m-2">(一)矯正措施</label>
            <textarea v-model="ncr.CorrectiveAction" @change="onDataChange" id="NCRdescription03-1" class="form-control mb-3" rows="5"></textarea>
        </div>
        <div class="form-group">
            <label for="NCRdescription03-2" class="m-2">(二)預防措施</label>
            <textarea v-model="ncr.PreventiveAction" @change="onDataChange" id="NCRdescription03-2" class="form-control mb-3" rows="5"></textarea>
        </div>
        <div class="form-group">
            <label for="NCRdescription04" class="m-2">四、矯正預防措施與改善結果</label>
            <textarea v-model="ncr.CorrPrevImproveResult" @change="onDataChange" id="NCRdescription04" class="form-control mb-3" rows="5"></textarea>
        </div>
        <!--
        <h2>審核結果（由原檢查人員認可）</h2>
        <div class="form-inline my-2">
            <label class="m-2">改善審核結果：</label>
            <select v-model="ncr.ImproveAuditResult" class="form-control">
                <option value="1">符合</option>
                <option value="2">需再行改善</option>
            </select>
        </div>
        <div class="form-inline my-2">
            <label for="NCRtrackingDate" class="m-2">計畫追蹤日期：</label>
            <input v-bind:value="ncr.chsProcessTrackDate" ref="chsProcessTrackDate" @change="onDateChange(ncr.chsProcessTrackDate, $event, 'chsProcessTrackDate')" id="NCRtrackingDate" type="text" class="form-control" placeholder="yyy/mm/dd">
            <b-form-datepicker v-model="chsProcessTrackDate" :hide-header="1==1"
                               button-only right size="sm" @context="onDatePicketChange($event, 'chsProcessTrackDate')">
            </b-form-datepicker>
        </div>
        <div class="form-group my-2">
            <label for="NCRtrackingContent" class="m-2">追蹤行動內容：</label>
            <textarea v-model="ncr.TrackCont" @change="onDataChange" id="NCRtrackingContent" class="form-control mb-3" rows="5"></textarea>
        </div>
        <div class="form-inline">
            <label for="NCRagreeCaseClosed" class="m-2">是否同意結案：</label>
            <div class="custom-control custom-radio mx-2">
                <input v-model="ncr.CanClose" @change="onDataChange" type="radio" id="NCRagreeCaseClosedYes" class="custom-control-input" name="ncrCanClose" value="true">
                <label class="custom-control-label" for="NCRagreeCaseClosedYes">是</label>
            </div>
            <div class="custom-control custom-radio mx-2">
                <input v-model="ncr.CanClose" @change="onDataChange" type="radio" id="NCRagreeCaseClosedNo" class="custom-control-input" name="ncrCanClose" value="false">
                <label class="custom-control-label" for="NCRagreeCaseClosedNo">否</label>
            </div>
        </div>
        -->
        <div class="row justify-content-center">
            <button v-on:click="onUpdateReport" v-bind:disabled="ncr.FormConfirm>0" role="button" class="btn btn-color11-4 btn-xs mx-1">
                <i class="fas fa-save"></i> 儲存
            </button>
            <button v-if="ncr.Seq != null" v-on:click="onFormConfirm" v-bind:disabled="ncr.FormConfirm>0" role="button" class="btn btn-color11-3 btn-xs mx-1">
                <i class="fa fa-check"></i> 確認
            </button>
            <!-- div class="col-6 col-lg-3 mt-3">
                <button v-on:click="back" role="button" class="btn btn-shadow btn-block btn-color1">
                    回上頁
                </button>
            </div -->
        </div>
    </div>
</template>
<script>
    export default {
        props: ['engMain', 'si'],
        watch: {
            si: function () {
                if (this.si.Seq > 0)
                    this.getNCR();
                else
                    this.ncr = { };
            }
        },
        data: function () {
            return {
                fResultChange: false,
                ncr: {},
                //chsProcessTrackDate:'',
            }
        },
        methods: {
            //取得 NCR 紀錄
            getNCR() {
                this.fResultChange = false;
                this.ncr = { };
                window.myAjax.post('/SamplingInspectionRecImprove/GetNCR', { rec: this.si })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.ncr = resp.data.ncr;
                            //this.chsProcessTrackDate = this.toYearDate(this.ncr.chsProcessTrackDate);
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
            onDateChange(srcDate, event, mode) {
                if (this.si.Seq > 0) this.fResultChange = true;
                if (event.target.value.length == 0) {
                    //if (mode == 'chsProcessTrackDate') this.chsProcessTrackDate = '';
                    return;
                }
                if (!this.isExistDate(event.target.value)) {
                    event.target.value = srcDate;
                    alert("日期錯誤");
                } else {
                    //if (mode == 'chsProcessTrackDate') this.chsProcessTrackDate = this.toYearDate(event.target.value);
                }
            },
            onDatePicketChange(ctx, mode) {
                if (this.si.Seq > 0) this.fResultChange = true;
                /*if (ctx.selectedDate != null) {
                    var d = ctx.selectedDate;
                    var dd = (d.getFullYear() - 1911) + '/' + (d.getMonth() + 1) + '/' + d.getDate();
                    //var y = d.getYear() - 1911;
                    if (mode == 'chsProcessTrackDate') this.ncr.chsProcessTrackDate = dd;
                }*/
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
                /*if (this.ncr.CanClose == null) {
                    alert('是否同意結案 等項目必須輸入');
                    return;
                }
                this.ncr.chsProcessTrackDate = this.$refs.chsProcessTrackDate.value;*/

                window.myAjax.post('/SamplingInspectionRecImprove/UpdateNCR', { recItem: this.si, ncr: this.ncr })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.fResultChange = false;
                            this.ncr.Seq = resp.data.ncrSeq;
                        }
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //表單確認
            onFormConfirm() {
                if (this.ncr.Seq == null) return;

                if (confirm('資料確認後將無法修改, 是否 確認此NCR程序追蹤改善表?')) {
                    window.myAjax.post('/SamplingInspectionRecImprove/NCRConfirm', { id: this.ncr.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.ncr.FormConfirm = 1;
                            }
                            alert(resp.data.message);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            back() {
                //window.history.go(-1);
                window.location = "/SamplingInspectionRecImprove";
            }
        },
        async mounted() {
            console.log('mounted() NCR程序追蹤改善表');
            /*var d = new Date();
            var dd = d.getFullYear() + '-' + (d.getMonth() + 1) + '-' + d.getDate();
            this.chsProcessTrackDate = dd;*/
        }
    }
</script>
