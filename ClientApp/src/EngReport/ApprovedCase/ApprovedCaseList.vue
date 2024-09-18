<template>
    <div>
        <form class="form-group">
            <div class="form-row">
                <div class="col-1 mt-3">
                    <select v-model="selectYear" @change="onYearChange($event)" class="form-control">
                        <option :value="-1">
                            全部
                        </option>
                        <option v-for="option in selectYearOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-3 mt-3">
                    <select v-model="selectUnit" @change="onUnitChange(selectUnit)" class="form-control">
                        <option selected="selected" :value="-1"> 全部</option>
                        <option v-for="option in selectUnitOptions" v-bind:value="option.Value"
                                v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-2 mt-3" v-if="selectUnit >0">
                    <select v-model="selectSubUnit" @change="onSubUnitChange($event)" class="form-control">
                        <option v-for="option in selectSubUnitOptions" v-bind:value="option.Value"
                                v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-1 col-sm-2 mt-3" >
                    <select v-model="selectRptType" @change="onRptTypeChange($event)" class="form-control">
                        <option v-for="option in selectRptTypeOptions" v-bind:value="option.Value"
                                v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-1 col-sm-2 mt-3 fomr-inline">
                    <input class="form-control" v-model="keyWord" placeholder="可輸入關鍵字查詢工程名稱"/> 
                </div>
                <div class="col-1 col-sm-2 mt-3 fomr-inline">
                    <button class="btn btn-color11-3 btn-xs sharp mx-1 mt-2" @click.prevent="getList">
                        <i class="fas fa-search"></i>
                    </button>
                </div>
                <div class="ml-auto bd-highlight align-self-center" style="padding-right: 15px;">
                    <!--<a href="./files/112先期檢討報部v4_核定.xlsx" download="112先期檢討報部v4_核定.xlsx">-->
                    <a class="p-1" :href="`../EngReport/GetApprovedCollection?year=${this.selectYear}&unit=${this.selectUnit}&subUnit=${this.selectSubUnit}&rptType=${this.selectRptType}`" download>
                        <button type="button" class="btn btn-color11-1 btn-block">
                            <i class="fas fa-download"></i>先期檢討報部_核定
                        </button>
                    </a>
                    <!--</a>-->
                    <!--<button @click="onDownloadPreviousReviewMeeting" type="button" class="btn btn-color11-1 btn-block" style="width: max-content;"><i class="fas fa-download"></i>先期檢討報部_核定</button>-->
                </div>
                <p style="color: red; padding-top: 15px;">狀態分為：需求評估、需求提案核可、需求重新評估、提案審查填報、提案審查核可、提案審查暫緩、經費核可、已核定案件</p>
            </div>
        </form>
        <div class="row justify-content-between">
            <comm-pagination class="col-10" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        </div>
        <div class="table-responsive">
            <table border="0" class="table table1 min910" id="addnew369">
                <thead class="insearch">
                    <tr>
                        <th class="sort">項次</th>
                        <th style="width:40px;"></th>
                        <th class="number">年度</th>
                        <th>工程名稱</th>
                        <th>執行機關</th>
                        <th>執行單位</th>
                        <th>負責人</th>
                        <th>狀態</th>
                        <th>工程案號</th>
                        <th>功能</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td>{{pageRecordCount*(pageIndex-1)+index+1}}</td>
                        <td><input v-if="item.IsTransfer==0" type="checkbox" v-model.trim="item.IsCheck" v-bind:id="item.Seq"></td>
                        <td>{{item.RptYear}}</td>
                        <td><a href="#" :title="item.RptName" v-on:click.stop="onViewEng(item)">{{item.RptName}}</a></td>
                        <td>{{item.ExecUnit}}</td>
                        <td>{{item.ExecSubUnit}}</td>
                        <td>{{item.ExecUser}}</td>
                        <td>{{item.EngState}}</td>
                        <td>
                            <div v-if="!item.edit">{{item.EngNo}}</div>
                            <input v-if="item.edit" type="text" v-model.trim="item.EngNo" class="form-control" />
                        </td>
                        <td style="min-width: 105px;">
                            <a href="#" v-if="!item.edit&&item.IsTransfer==0" v-on:click.prevent="item.edit=!item.edit" class="btn btn-color11-3 btn-xs mx-1" title="編輯"><i class="fas fa-pencil-alt"></i> 編輯</a>
                            <a href="#" v-if="item.edit" v-on:click.prevent="onSaveRecord(item)" class="btn btn-color11-2 btn-xs mx-1"><i class="fas fa-save"></i> 儲存</a>
                            <a href="#" v-if="item.edit" v-on:click.prevent="item.edit=false" class="btn btn-color9-1 btn-xs mx-1" title="取消"><i class="fas fa-times"></i> 取消</a>
                            <a href="#" v-if="!item.edit&&item.IsTransfer==0" class="btn btn-color11-2 btn-xs mx-1" v-on:click.prevent="onTransferRecord(item);" title="轉入建立案件"><i class="fas fa-file-import"></i> 轉入 </a>
                            <button href="#" v-else class="btn btn-color11-2 btn-xs mx-1" :disabled="true" title="轉入建立案件"><i class="fas fa-file-import"></i> 已轉入 </button>
                        </td>
                    </tr>
                    <tr v-if="items==null||items.length==0">
                        <td colspan="10" class="text-center">--查無資料--</td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="card-footer">
            <div class="row justify-content-center">
                <div class="col-12 col-sm-4 col-xl-2 my-2">
                    <a href="javascript:void(0)" class="btn btn-shadow btn-block btn-color11-2" v-on:click.stop="onTransfer();" title="轉入建立案件"><i class="fas fa-file-import"></i>轉入建立案件 </a>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                keyWord : "",
                //使用者單位資訊
                userUnit: null,
                userUnitSub: '',
                userName: '',
                unitName: '',
                unitSubName: '',
                rptYear: '',
                rptName: '',
                initFlag: 0,
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
                this.onUnitChange(this.selectUnit)
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
                if (this.selectRptType == '') this.getSelectRptTypeOption(5);
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
            //工程狀態-變動事件
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
                window.myAjax.post('/ERApprovedCase/GetList'
                    , {
                        year: this.selectYear,
                        unit: this.selectUnit,
                        subUnit: this.selectSubUnit,
                        rptType: this.selectRptType,
                        pageRecordCount: this.pageRecordCount,
                        pageIndex: this.pageIndex,
                        keyWord : this.keyWord
                    })
                    .then(resp => {
                        this.items = resp.data.items;
                        this.recordTotal = resp.data.pTotal;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onSaveRecord(uItem) {
                //console.log(uItem);
                if (this.strEmpty(uItem.EngNo)) {
                    alert('工程案號 必須輸入!');
                    return;
                }
                window.myAjax.post('/ERApprovedCase/UpdateEngReport', { m: uItem })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            uItem.edit = false;
                            //this.getList();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onTransferRecord(uItem) {
                //console.log(uItem);
                if (!confirm('是否確定轉入建立案件？')) return;
                if (this.strEmpty(uItem.EngNo)) {
                    alert('工程案號 必須輸入!');
                    return;
                }
                window.myAjax.post('/ERApprovedCase/UpdateEngReportTransferRecord', { m: uItem })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            uItem.edit = false;
                            this.getList();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onTransfer() {
                if (!confirm('是否確定轉入建立案件？\n請確認「工程案號」是否填寫正確，一旦轉入則無法再編輯！')) return;
                window.myAjax.post('/ERApprovedCase/UpdateEngReportTransfer', { list: this.items.filter(e => ! e.IsTransfer ) })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getList();
                            alert(resp.data.msg);
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
            onViewEng(item) {
                window.myAjax.post('/ERApprovedCase/ViewEng', { seq: item.Seq })
                    .then(resp => {
                        window.sessionStorage.setItem(window.eqSelNeedAssessmentSeq, item.Seq);
                        console.log(resp.data.Url);
                        window.location.href = resp.data.Url;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //下載
            onDownloadPreviousReviewMeeting() {
                window.comm.dnFile('/EngReport/GetApprovedCollection');
            },
        },
        async mounted() {
            console.log('mounted() 核定案件');
            if (this.selectYearOptions.length == 0) {
                this.getSelectYearOption();
            }
        }
    }
</script>
