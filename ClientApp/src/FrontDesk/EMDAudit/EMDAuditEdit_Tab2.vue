<template>
    <div>
        <div class="row justify-content-between">
            <div class="form-inline col-12 col-md-11 mt-3">
            </div>
            <!-- shioulo 20220519
        <div class="form-inline col-12 col-md-1 mt-3">
            <a v-on:click="goback()" href="#" class="a-blue" title="返回">返回</a>
        </div>
        -->
        </div>
        <div class="table-responsive mb-3" style="min-height:320px;">
            <table border="0" class="table table1 onepage my-0">
                <thead>
                    <tr>
                        <th rowspan="2">項次</th>
                        <th class="min200">契約詳細表項次</th>
                        <th width="180px">預定進場日期</th>
                        <th rowspan="2">進場數量</th>
                        <th width="180px">抽樣日期</th>
                        <th rowspan="2">規定抽(取)樣頻率</th>
                        <th>累積進場數量</th>
                        <th rowspan="2" class="number">抽試驗結果</th>
                        <th rowspan="2">抽驗及會同人員</th>
                        <th rowspan="2" class="min75">備註(歸檔編號)</th>
                        <th rowspan="2" class="sort min75"></th>
                    </tr>
                    <tr>
                        <th>材料/設備名稱</th>
                        <th>實際進場日期</th>
                        <th>抽樣數量</th>
                        <th>累積抽樣數量</th>
                    </tr>
                </thead>
                <tbody v-for="(item, index) in items" v-bind:key="item.Seq" v-show="!editFlag || item.edit">
                    <tr>
                        <td rowspan="2">{{perPage*(pageIndex-1)+index+1}}</td>
                        <td>{{item.ItemNo}}</td>
                        <td>
                            <div v-if="!item.edit">{{item.chsSchTestDate}}</div>
                            <b-input-group v-if="item.edit">
                                <input v-bind:value="item.chsSchTestDate" @change="onDateInputChange($event, item, 'SchTestDate')" type="text" class="form-control" placeholder="yyy/mm/dd">
                                <b-form-datepicker v-model="item.SchTestDate" :hide-header="hideHeader"
                                                   button-only right class="mb-2" size="sm" @context="onDatePicketChange($event, index, 'SchTestDate')">
                                </b-form-datepicker>
                            </b-input-group>
                        </td>
                        <td rowspan="2">
                            <div v-if="!item.edit">{{item.TestQty}}</div>
                            <input v-if="item.edit" type="text" class="form-control text-right" v-model.number="item.TestQty">
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.chsSampleDate}}</div>
                            <b-input-group v-if="item.edit">
                                <input v-bind:value="item.chsSampleDate" @change="onDateInputChange($event, item, 'SampleDate')" type="text" class="form-control" placeholder="yyy/mm/dd">
                                <b-form-datepicker v-model="item.SampleDate" :hide-header="hideHeader"
                                                   button-only right class="mb-2" size="sm" @context="onDatePicketChange($event, index, 'SampleDate')">
                                </b-form-datepicker>
                            </b-input-group>
                        </td>
                        <td rowspan="2" class="twoInput">
                            {{item.SampleFeq}}
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.AccTestQty}}</div>
                            <input v-if="item.edit" type="text" class="form-control text-right" v-model.number="item.AccTestQty">
                        </td>
                        <td rowspan="2">
                            <span v-if="!item.edit && item.TestResult==2">否決</span>
                            <span v-if="!item.edit && item.TestResult==1">通過</span>
                            <select v-if="item.edit" v-model.number="item.TestResult" class="form-control">
                                <option value="1">通過</option>
                                <option value="2">否決</option>
                            </select>
                        </td>
                        <td rowspan="2">
                            <div v-if="!item.edit">{{item.Coworkers}}</div>
                            <input v-if="item.edit" type="text" class="form-control" v-model="item.Coworkers">
                        </td>
                        <td rowspan="2">
                            <div v-if="!item.edit">{{item.ArchiveNo}}</div>
                            <input v-if="item.edit" type="text" class="form-control" v-model="item.ArchiveNo">
                            <div>
                                <a v-if="!item.edit" v-on:click="openEMDAuditEditModal_3(item)" data-toggle="modal" data-target="#emdauditdModal_3" href="#" class="a-blue" title="檢視">檢視</a>
                                <a v-if="item.edit" v-on:click="openEMDAuditEditModal_3(item)" data-toggle="modal" data-target="#emdauditdModal_3" href="#" class="a-blue" title="編輯">編輯</a>
                            </div>
                        </td>
                        <td rowspan="2">
                            <div v-if="!item.edit" class="d-flex justify-content-center">
                                <button @click="editItem(item)" class="btn btn-color11-3 btn-xs sharp m-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                <!-- button v-if="item.ItemType==0" @click="addItem(item)" class="btn btn-color11-1 btn-xs sharp m-1" title="新增"><i class="fas fa-plus"></i></!--button -->
                                <button v-if="item.ItemType==1" @click="delItem(item)" class="btn btn-color9-1 btn-xs sharp m-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                            </div>
                            <div v-if="item.edit" class="d-flex justify-content-center">
                                <button @click="saveItem(item)" class="btn btn-color11-4 btn-xs sharp m-1" title="儲存"><i class="fas fa-save"></i></button>
                                <button @click="cancelItem(item)" class="btn btn-color9-1 btn-xs sharp m-1" title="取消"><i class="fas fa-times"></i></button>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>{{item.MDName}}</td>
                        <td>
                            <div v-if="!item.edit">{{item.chsRealTestDate}}</div>
                            <b-input-group v-if="item.edit">
                                <input v-bind:value="item.chsRealTestDate" @change="onDateInputChange($event, item, 'RealTestDate')" type="text" class="form-control" placeholder="yyy/mm/dd">
                                <b-form-datepicker v-model="item.RealTestDate" :hide-header="hideHeader"
                                                   button-only right class="mb-2" size="sm" @context="onDatePicketChange($event, index, 'RealTestDate')">
                                </b-form-datepicker>
                            </b-input-group>
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.SampleQty}}</div>
                            <input v-if="item.edit" type="text" class="form-control text-right" v-model.number="item.SampleQty">
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.AccSampleQty}}</div>
                            <input v-if="item.edit" type="text" class="form-control text-right" v-model.number="item.AccSampleQty">
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div style="width:99%;" class="row justify-content-center">
            <b-pagination :total-rows="totalRows"
                          :per-page="perPage"
                          v-model="pageIndex">
            </b-pagination>
        </div>
        <UploadReport v-if="LoadNo=='3'" v-on:onCloseEvent="closeEMDAuditEditModal_3"
                          v-bind:modalId="'emdauditdModal_3'" v-bind:emdSummary="emdSummary" v-bind:targetItem="targetItem" v-bind:engMainSeq="engMain.Seq"></UploadReport>
    </div>
</template>
<script>
    export default {
        props: ['engMain'],
        components: {
            UploadReport: require('./EMDAuditEdit_Tab2_UploadReport.vue').default,
        },
        watch: {
            pageIndex: {
                handler: function (value) {
                    this.getList();
                }
            }
        },
        data: function () {
            return {
                hideHeader:true,
                fCanEdit: false,
                items: [],

                targetItem: { ItemName: '', FlowCharOriginFileName: '' },

                //分頁
                pageIndex: 1,
                perPage: 3,
                totalRows: 0,
                //s20230626
                LoadNo: '',
                emdSummary: {},
            };
        },
        methods: {
            //s20230626
            openEMDAuditEditModal_3(item) {
                this.emdSummary = item;
                this.targetItem = item;
                this.LoadNo = '3';
            },
            closeEMDAuditEditModal_3() {
                this.emdSummary = {};
                this.targetItem = { ItemName: '', FlowCharOriginFileName: '' };
                this.LoadNo = '';
            },
            //日期變更 20230222
            onDateInputChange(event, item, mode) {
                var dateStr = event.target.value;
                var dateObj = dateStr.split('/'); // yyy/mm/dd
                if (dateObj.length != 3) {
                    this.clearDate(item, mode);
                } else {
                    console.log(dateStr);
                    dateStr = this.toYearDateStr(dateStr);
                    if (!this.isExistDate(dateStr)) {
                        this.clearDate(item, mode);
                    } else {
                        dateStr = dateStr.replaceAll('/', '-')
                        if (mode == 'SchTestDate') {
                            item.SchTestDate = dateStr;
                        } else if (mode == 'RealTestDate') {
                            item.RealTestDate = dateStr;
                        } else if (mode == 'SampleDate') {
                            item.SampleDate = dateStr;
                        }
                    }
                }
            },
            clearDate(item, mode) {
                if (mode == 'SchTestDate') {
                    item.SchTestDate = '';
                    item.chsSchTestDate = '';
                } else if (mode == 'RealTestDate') {
                    item.RealTestDate = '';
                    item.chsRealTestDate = '';
                } else if (mode == 'SampleDate') {
                    item.SampleDate = '';
                    item.chsSampleDate = '';
                }
            },
            //中曆轉西元
            toYearDateStr(dateStr) {
                if (dateStr == null) return null;
                var dateObj = dateStr.split('/'); // yyy/mm/dd
                //return new Date(parseInt(dateObj[0]) + 1911, parseInt(dateObj[1]) - 1, parseInt(dateObj[2]));
                return (parseInt(dateObj[0]) + 1911).toString() + '/' + (parseInt(dateObj[1])).toString() + '/' + (parseInt(dateObj[2])).toString();
            },
            //清單
            getList() {
                console.log('getList() ' + this.engMain.Seq);
                this.editFlag = false;
                this.items = [];
                let params = {id: this.engMain.Seq, pageIndex: this.pageIndex, perPage: this.perPage};
                window.myAjax.post('/EMDAudit/GetEMDTestSummaryList', params)
                    .then(resp => {
                        this.items = resp.data.items;
                        this.totalRows = resp.data.pTotal;
                        for (let item of this.items) {
                            item.SchTestDate = this.toYearDate(item.chsSchTestDate);
                            item.RealTestDate = this.toYearDate(item.chsRealTestDate);
                            item.SampleDate = this.toYearDate(item.chsSampleDate);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            addItem(item) {//s20230308
                window.myAjax.post('/EMDAudit/AddEMDTestSummary', { item: item })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getList();
                        }
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            delItem(item) {//s20230308
                if (confirm('是否確定刪除資料？')) {
                    window.myAjax.post('/EMDAudit/DelEMDTestSummary', { id: item.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getList();
                            } else
                                alert(resp.data.msg);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            cancelItem(item) {//s20230308
                this.editFlag = false;
                item.edit = this.editFlag;
            },
            editItem(item) {
                if (this.editFlag) return;

                this.editFlag = true;
                item.edit = this.editFlag;
            },
            //項目名稱, Keep儲存
            saveItem(item) {
                console.log(item);
                window.myAjax.post('/EMDAudit/UpdateEMDTestSummary', { item: item })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            item.edit = false;
                            this.editFlag = false;
                            //const resultItem = resp.data.item.Data;
                            //item.modifyDate = resultItem.modifyDate;
                        }
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onDateChange(event, itemA, index, mode) {
                if (event.target.value.length == 0) {
                    itemA = '';
                    if (mode == 'SchTestDate')
                        this.items[index].SchTestDate = '';
                    else if (mode == 'RealTestDate')
                        this.items[index].RealTestDate = '';
                    else if (mode == 'SampleDate')
                        this.items[index].SampleDate = '';
                    return;
                }
                if (!this.isExistDate(event.target.value)) {
                    event.target.value = itemA;
                    alert("日期錯誤");
                } else {
                    itemA = this.toYearDate(event.target.value);
                    if (mode == 'SchTestDate')
                        this.items[index].SchTestDate = event.target.value;
                    else if (mode == 'RealTestDate')
                        this.items[index].RealTestDate = event.target.value;
                    else if (mode == 'SampleDate')
                        this.items[index].SampleDate = event.target.value;
                }
            },
            onDatePicketChange(ctx, index, mode) {
                //console.log(ctx);
                if (ctx.selectedDate != null) {
                    var d = ctx.selectedDate;
                    var dd = (d.getFullYear() - 1911) + '/' + (d.getMonth() + 1) + '/' + d.getDate();
                    if (mode == 'SchTestDate')
                        this.items[index].chsSchTestDate = dd;
                    else if (mode == 'RealTestDate')
                        this.items[index].chsRealTestDate = dd;
                    else if (mode == 'SampleDate')
                        this.items[index].chsSampleDate = dd;
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
            goback() {
                window.location = "/EMDAudit/EMDAuditList";
            },
            goPage(engId) {
                //window.location = "/SupervisionPlan/Edit?id=" + engId;
                window.open('/SupervisionPlan/Edit?id=' + engId, '_blank');
            },
        },
        async mounted() {
            console.log('mounted() 材料設備檢(試)驗管制總表');
            this.getList();
        }
    }
</script>