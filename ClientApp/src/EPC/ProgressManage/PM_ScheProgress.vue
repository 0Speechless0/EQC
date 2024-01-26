<template>
    <div>
        <div v-if="engChanging">
            <h5>工程變更作業中..暫停操作</h5>
        </div>
        <div v-if="!changeCompDate" class="form-row">
            <div class="col-12 col-sm-4 col-md-2 mb-1">
                <label class="my-2 mx-2 float-right">開工日期</label>
            </div>
            <div class="col-12 col-sm-4 col-md-3 mb-1">
                <b-input-group>
                    <input v-bind:value="engMain.chsStartDate" ref="chsStartDate" @change="onDateChange(engMain.chsStartDate, $event, 'StartDate')"
                           v-bind:disabled="engMain.srcStartDate!=null" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                    <b-form-datepicker v-if="engMain.srcStartDate==null" v-model="chsStartDate" :hide-header="true"
                                       button-only right size="sm" @context="onDatePicketChange($event, 'StartDate')">
                    </b-form-datepicker>
                </b-input-group>
            </div>
            <div class="col-12 col-sm-4 col-md-2 mb-1">
                <label class="my-2 mx-2 float-right">預定完工日期</label>
            </div>
            <div class="col-12 col-sm-4 col-md-3 mb-1">
                <b-input-group>
                    <input v-bind:value="engMain.chsSchCompDate" ref="chsSchCompDate" @change="onDateChange(engMain.chsSchCompDate, $event, 'SchCompDate')"
                           v-bind:disabled="engMain.srcSchCompDate!=null" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                    <b-form-datepicker v-if="engMain.srcSchCompDate==null" v-model="chsSchCompDate" :hide-header="true"
                                       button-only right size="sm" @context="onDatePicketChange($event, 'SchCompDate')">
                    </b-form-datepicker>
                </b-input-group>
            </div>
            <div v-if="engMain.srcStartDate==null || engMain.srcSchCompDate==null" class="col-12 col-sm-4 col-md-2 mb-1">
                <button v-on:click.stop="updateEngDates()" role="button" class="btn btn-color11-4 btn-xs m-1">
                    <i class="fas fa-save"> 儲存</i>
                </button>
            </div>
            <div class="col-12 col-sm-4 col-md-6 mb-1">
            </div>
        </div>
        <!-- 工程變更 20221228 -->
        <div v-if="changeCompDate" class="form-row">
            <div class="col-12 col-sm-4 col-md-3 mb-1">
                <label class="my-2 mx-2 float-right">工程開始變更日期</label>
            </div>
            <div class="col-12 col-sm-4 col-md-3 mb-1">
                <b-input-group>
                    <input v-bind:value="engMain.chsEngChangeStartDate" ref="chsEngChangeStartDate" @change="onDateChange(engMain.chsEngChangeStartDate, $event, 'EngChangeStartDate')" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                    <b-form-datepicker v-model="chsEngChangeStartDate" :hide-header="true"
                                       button-only right size="sm" @context="onDatePicketChange($event, 'EngChangeStartDate')">
                    </b-form-datepicker>
                </b-input-group>
            </div>
            <div class="col-12 col-sm-1 col-md-1 mb-1">
                <button v-on:click.stop="onEngChangeClick" role="button" class="btn btn-color11-4 btn-xs m-1">
                    <i class="fas fa-save"> 確定</i>
                </button>
            </div>
            <div class="col-12 col-sm-1 col-md-1 mb-1">
                <button v-on:click.stop="changeCompDate=false" role="button" class="btn btn-color3 btn-xs m-1">
                    <i class="fas fa-times"> 取消</i>
                </button>
            </div>
            <div class="col-12 col-sm-4 col-md-6 mb-1">
            </div>
        </div>
        <div v-if="changeCompDate" class="form-row">
            <div class="col-12 col-sm-4 col-md-3 mb-1">
                <label class="my-2 mx-2 float-right">變更後預定完工日期</label>
            </div>
            <div class="col-12 col-sm-4 col-md-3 mb-1">
                <input v-bind:value="engMain.ScheChangeCloseDateStr" disabled type="text" class="form-control">
            </div>
        </div>

        <div v-if="!changeCompDate" class="form-row" role="toolbar">
            <div v-if="spHeader.EngChangeCount==0" class="bcol-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                <button v-if="engChangeItems.length == 0" @click="onDelProgressClick" v-bind:disabled="isDisabled" type="button" class="btn btn-outline-secondary btn-sm">刪除進度&nbsp;<i class="fas fa-times"></i></button>
            </div>
            <!-- div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                <label class="btn btn-block btn-outline-secondary btn-sm" v-bind:class="{ 'disabled' : isDisabled}">
                    <input v-on:change="fileChange($event)" v-bind:disabled="isDisabled" type="file" name="file" multiple="" style="display:none;">
                    更新決標後PCCES&nbsp;<i class="fas fa-upload"></i>
                </label>
            </div -->
            <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                <button @click="download" type="button" class="btn btn-outline-secondary btn-sm" title="檔案下載">下載工程範本(excel) &nbsp;<i class="fas fa-download"></i></button>
            </div>
            <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                <label class="btn btn-block btn-outline-secondary btn-sm" v-bind:class="{ 'disabled' : (isDisabled || engChanging)}">
                    <input v-on:change="uploadSchProgress($event)" v-bind:disabled="isDisabled  || engChanging" type="file" name="file" multiple="" style="display:none;">
                    匯入(excel)各工項預定完成量&nbsp;<i class="fas fa-upload"></i>
                </label>
                <!-- button type="button" class="btn btn-outline-secondary btn-sm" title="檔案匯入">匯入(excel)各工項預定完成量 &nbsp;<i class="fas fa-upload"></i></button -->
            </div>
            <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                <button @click.stop="fillCompleted" v-bind:disabled="isDisabled || engChanging" type="button" class="btn btn-outline-secondary btn-sm" title="填報完成">填報完成 &nbsp;<i class="fas fa-check"></i></button>
            </div>
            <!-- s20230428 取消功能
            <div class="bcol-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                <button v-if="engChangeItems.length == 0" @click="changeCompDate=true" type="button" class="btn btn-outline-secondary btn-sm">工程變更&nbsp;<i class="fas fa-pen"></i></button>
            </div>
            -->
        </div>
        <!-- p class="text-R small">※ 更新決標後pcces，請務必確認代碼不可變更，系統僅更新單價</p -->
        <!-- Nav tabs -->
        <ul class="nav nav-tabs" role="tablist">
            <li v-for="(item, index) in dateList" v-bind:key="index" class="nav-item">
                <a @click="onChangeDate(item)" class="nav-link" data-toggle="tab" href="#menu01" v-bind:style="{ color:(item.Version==0 ? '': 'blue')  }" >{{item.SPDateStr}}</a>
            </li>
        </ul>
        <!-- Tab panes -->
        <div class="tab-content">
            <div id="menu01" class="tab-pane active">
                <div class="table-responsive">
                    <table class="table table-responsive-md table-hover">
                        <thead class="insearch">
                            <tr>
                                <th><strong>序號</strong></th>
                                <th><strong>項次</strong></th>
                                <th><strong>施工項目</strong></th>
                                <th class="text-right"><strong>單位</strong></th>
                                <th class="text-right"><strong>單價</strong></th>
                                <th class="text-right"><strong>契約數量</strong></th>
                                <th class="text-right"><strong>金額</strong></th>
                                <th colspan="2" class="text-right"><strong>累計預定進度%</strong></th>
                                <!-- th class="text-right"><strong>日進度%</strong></th-->
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in items" v-bind:key="item.Seq" class="bg-1-30">
                                <td><strong>{{item.OrderNo}}</strong></td>
                                <td><strong>{{item.PayItem}}</strong></td>
                                <td><strong>{{item.Description}}</strong></td>
                                <td class="text-right">{{item.Unit}}</td>
                                <td class="text-right">{{item.Price}}</td>
                                <td class="text-right">{{item.Quantity}}</td>
                                <td class="text-right">{{item.Amount}}</td>
                                <template v-if="item.Seq != editSeq">
                                    <td class="text-right">{{item.SchProgress}}</td>
                                    <td class="text-right">
                                        <button @click="onEditRecord(item)" v-if="progressEdit && !(isDisabled || engChanging)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                    </td>
                                </template>
                                <template v-if="item.Seq == editSeq">
                                    <td><input v-model.number="editRecord.SchProgress" maxlength="10" type="text" class="form-control"></td>
                                    <td style="text-align: center;">
                                        <div class="d-flex justify-content-center">
                                            <button @click="onSaveRecord" class="btn btn-color11-2 btn-xs sharp mx-1" title="儲存"><i class="fas fa-save"></i></button>
                                            <button @click="editSeq = -99" class="btn btn-color9-1 btn-xs sharp mx-1" title="取消"><i class="fas fa-times"></i></button>
                                        </div>
                                    </td>
                                </template>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['tenderItem', 'spHeader'],
        data: function () {
            return {
                activeItem: null,
                items: [],
                dateList: [],
                btnDisabled: false,
                //attrs: [],
                engMain: {Seq:-1},
                chsStartDate: '',
                chsSchCompDate: '',
                chsEngChangeStartDate: '',
                changeCompDate: false,
                //s20230406
                engChangeItems: [],
                engChanging: false,
                //s20230720
                editSeq: -99,
                tarRecord: {},
                editRecord: {},
                progressEdit: false,
            };
        },
        /*components: {
            CalendarSetting: require('./CalendarSetting.vue').default,
        },*/
        methods: {
            //編輯紀錄 s20230720
            onEditRecord(item) {
                if (this.editSeq > -99) return;
                this.tarRecord = item;
                this.editRecord = Object.assign({}, item);
                this.editSeq = this.editRecord.Seq;
            },
            //儲存 s20230720
            onSaveRecord() {
                window.myAjax.post('/EPCSchProgress/UpdateProgesss', { m: this.editRecord, id: this.tenderItem.Seq, tarDate: this.activeItem.ItemDate, ver: this.activeItem.Version })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.editSeq = -99;
                            this.tarRecord.SchProgress = this.editRecord.SchProgress;
                        }
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },

            //決標日期 shioulo 20221216
            onDateChange(srcDate, event, mode) {
                if (event.target.value.length == 0) {
                    if (mode == 'chsStartDate')
                        this.chsStartDate = '';
                    else if (mode == 'chsSchCompDate')
                        this.chsSchCompDate = '';
                    else if (mode == 'chsEngChangeStartDate')
                        this.chsEngChangeStartDate = '';
                    return;
                }
                if (!this.isExistDate(event.target.value)) {
                    event.target.value = srcDate;
                    alert("日期錯誤");
                } else {
                    if (mode == 'chsStartDate')
                        this.chsStartDate = this.toYearDate(event.target.value);
                    else if (mode == 'chsSchCompDate')
                        this.chsSchCompDate = this.toYearDate(event.target.value);
                    else if (mode == 'chsEngChangeStartDate')
                        this.chsEngChangeStartDate = this.toYearDate(event.target.value);
                }
            },
            onDatePicketChange(ctx, mode) {
                //console.log(ctx);
                if (ctx.selectedDate != null) {
                    var d = ctx.selectedDate;
                    var dd = (d.getFullYear() - 1911) + '/' + (d.getMonth() + 1) + '/' + d.getDate();
                    //var y = d.getYear() - 1911;
                    if (mode == 'StartDate')
                        this.engMain.chsStartDate = dd;
                    else if (mode == 'SchCompDate')
                        this.engMain.chsSchCompDate = dd;
                    else if (mode == 'EngChangeStartDate')
                        this.engMain.chsEngChangeStartDate = dd;
                }
            },
            //中曆轉西元
            toYearDate(dateStr) {
                if (dateStr == null || dateStr == '') return null;
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
            //
            getEngMain() {
                this.engMain = { Seq: this.tenderItem.Seq, chsStartDate: '', chsSchCompDate:''};
                window.myAjax.post('/EPCSchProgress/GetEngItem', { id: this.tenderItem.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engMain = resp.data.item;
                            this.chsStartDate = this.toYearDate(this.engMain.chsStartDate);
                            this.chsSchCompDate = this.toYearDate(this.engMain.chsSchCompDate);
                        }/* else {
                            alert(resp.data.message);
                        }*/
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //更新 日期
            updateEngDates() {
                this.engMain.chsStartDate = this.$refs['chsStartDate'].value;
                this.engMain.chsSchCompDate = this.$refs['chsSchCompDate'].value;
                window.myAjax.post('/EPCSchProgress/UpdateEngDates', { engMain: this.engMain })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getEngMain();
                            this.getDateList();
                            alert(resp.data.message);
                        } else {
                            alert(resp.data.message);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //shioulo 20221216 end
            //刪除進度資料
            onDelProgressClick() {
                if (confirm('是否確定刪除目前進度資料? ')) {
                    window.myAjax.post('/EPCSchProgress/DelProgress', { id: this.tenderItem.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getEngMain();
                                this.getDateList();
                            }
                            alert(resp.data.msg);
                            this.$emit('reload');
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            //工程變更
            onEngChangeClick() {
                this.engMain.chsEngChangeStartDate = this.$refs['chsEngChangeStartDate'].value;
                if (this.engMain.chsEngChangeStartDate.length == 0) {
                    this.$swal("工程開始變更日期 必須輸入");
                    return;
                }
                window.myAjax.post('/EPCSchProgress/CheckEngChangeDate', { id: this.tenderItem.Seq, engMain: this.engMain })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.$swal({
                                title: '工程變更, 是否確定?',
                                html: resp.data.msg,
                                icon: 'warning',
                                showCancelButton: true,
                                confirmButtonColor: '#3085d6',
                                cancelButtonColor: '#d33',
                                confirmButtonText: '是',
                                cancelButtonText: '否',
                            }).then(this.execChangeConfirm);
                        } else {
                            this.$swal(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //工程變更執行
            execChangeConfirm(result) {
                if (!result.value) return;

                window.myAjax.post('/EPCSchProgress/ExecEngChangeDate', { id: this.tenderItem.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getDateList();
                        }
                        alert(resp.data.msg);
                        this.$emit('reload');
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },

            //填報完成
            fillCompleted() {
                if (confirm('填報完成後將不能再修改\n是否確定? ')) {
                    //setTimeout(
                    window.myAjax.post('/EPCSchProgress/FillCompleted', { id: this.tenderItem.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.$emit('reload');
                            }
                            alert(resp.data.msg);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                    //, 10);
                }
            },
            //取得清單
            onChangeDate(item) {
                this.activeItem = item;
                this.items = [];
                this.progressEdit = false;
                this.editSeq = -99,
                this.tarRecord = { },
                this.editRecord = { },
                window.myAjax.post('/EPCSchProgress/GetList', { id: this.tenderItem.Seq, tarDate: item.ItemDate, ver: item.Version })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                            this.progressEdit = resp.data.canEdit;
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //日期清單
            getDateList() {
                this.dateList = [];
                window.myAjax.post('/EPCSchProgress/GetDateList', { id: this.tenderItem.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.dateList = resp.data.items;
                            console.log("empty 0");
                            if (this.dateList.length > 0 && JSON.stringify(this.spHeader) === '{}') {
                                console.log("empty");
                                this.$emit('reload');
                            }
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //下載
            download() {
                window.comm.dnFile('/EPCSchProgress/Download?id=' + this.tenderItem.Seq);
            },
            //匯入(excel)各工項 累計預定進度%
            uploadSchProgress(event) {
                var files = event.target.files || event.dataTransfer.files;
                // 預防檔案為空檔
                if (!files.length) return;
                if (!files[0].type.match('application/vnd.openxmlformats-officedocument.spreadsheetml.sheet')) {// && !files[0].type.match('application/vnd.ms-excel') ) {
                    alert('請選擇 .xlsx Excel檔案');
                    return;
                }
                var uploadfiles = new FormData();
                uploadfiles.append("id", this.tenderItem.Seq);
                uploadfiles.append("file", files[0], files[0].name);
                this.upload(uploadfiles, 'UploadSchProgress');
            },
            //匯入 更新PCCES
            fileChange(event) {
                var files = event.target.files || event.dataTransfer.files;
                // 預防檔案為空檔
                if (!files.length) return;
                if (!files[0].type.match('text/xml')) {
                    alert('請選擇 XML 檔案');
                    return;
                }
                var uploadfiles = new FormData();
                uploadfiles.append("id", this.tenderItem.Seq);
                uploadfiles.append("file", files[0], files[0].name);
                this.upload(uploadfiles, 'UploadXML');
            },
            upload(uploadfiles, action) {
                window.myAjax.post('/EPCSchProgress/' + action, uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            if (this.activeItem != null) this.onChangeDate(this.activeItem);
                            this.$emit('reload');
                            alert("匯入完成\n請至進度報表確認上傳之預定進度是否符合契約");
                        } else {
                            alert(resp.data.message);
                        }
                    }).catch(error => {
                        console.log(error);
                    });
            },
            //工程變更清單 s20230406
            getEngChangeList() {
                this.engChangeItems = [];
                window.myAjax.post('/EPCProgressEngChange/GetECList', { id: this.tenderItem.Seq })
                    .then(resp => {
                        if (resp.data.result >= 0) {
                            this.engChangeItems = resp.data.items;
                            if (this.engChangeItems.length > 0) {
                                this.engChanging = (this.engChangeItems[this.engChangeItems.length - 1].SPState != 1);
                            }
                        }
                        this.getEngMain();
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        computed: {
            isDisabled: function () {
                if (this.dateList.length == 0) return true;
                if (this.spHeader.SPState > 0) return true;
                return false;
            },
            isUploadPcces: function () {
                return !window.comm.stringEmpty(this.spHeader.PccesXMLDateStr)
            },
        },
        mounted() {
            console.log('mounted() 預定進度');
            this.getEngChangeList();
            this.getDateList();
        }
    }
</script>
<style>
    .labelDisabled {
        opacity: .65;
    }
</style>