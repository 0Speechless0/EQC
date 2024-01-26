<template>
    <div>
        <form class="form-group">
            <div class="form-row">
                <div class="col-1 mt-3">
                    <select v-model="selectYear" @change="onYearChange($event)" class="form-control">
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
                    <!--<a href="./files/提案需求評估表.docx" download="提案需求評估表.docx">-->
                    <button @click.prevent="onDownloadNeedAssessment" class="btn btn-color11-1 btn-block">
                        <i class="fas fa-download"></i>提案需求評估表
                    </button>
                    <!--</a>-->
                </div>
                <p style="color: red; padding-top: 15px;">狀態分為：需求評估、需求提案核可、需求重新評估、提案審查填報、提案審查核可、提案審查暫緩、經費核可、已核定案件</p>
            </div>
        </form>
        <div class="row justify-content-between">
            <comm-pagination class="col-10" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        </div>
        <div class="table-responsive">
            <table border="0" class="table table1 min910" id="addnew159">
                <thead class="insearch">
                    <tr>
                        <th class="sort">項次</th>
                        <th class="number text-center">年度</th>
                        <th>案由</th>
                        <th>執行機關</th>
                        <th>執行單位</th>
                        <th>負責人</th>
                        <th>狀態</th>
                        <th>
                            <div class="d-flex justify-content-center">
                                <a v-on:click.stop="fAddItem=true" href="##" class="btn btn-color11-3 btn-xs sharp mx-1" title="新增"><i class="fas fa-plus"></i></a>
                            </div>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-if="fAddItem">
                        <td></td>
                        <td><input v-model.trim="newItem.RptYear" maxlength="3" type="text" class="form-control"></td>
                        <td><input v-model.trim="newItem.RptName" maxlength="50" type="text" class="form-control"></td>
                        <th><input type="text" class="form-control" v-model="newItem.ExecUnit" disabled="disabled"></th>
                        <th>
                            <!--<input type="text" class="form-control" v-model="newItem.ExecSubUnit" disabled="disabled">-->
                            <select class="form-control" @change="onChangeUnit2" v-model="newItem.ExecSubUnitSeq">
                                <option v-bind:key="index" v-for="(item,index) in units2" v-bind:value="item.Value">{{item.Text}}</option>
                            </select>
                        </th>
                        <th>
                            <!--<input type="text" class="form-control" v-model="newItem.ExecUser" disabled="disabled">-->
                            <select class="form-control" v-model="newItem.CreateUserSeq">
                                <option v-bind:key="index" v-for="(item,index) in users3" v-bind:value="item.Seq">{{item.DisplayName}}</option>
                            </select>
                        </th>
                        <td>需求評估</td>
                        <td style="min-width: unset;">
                            <div class="d-flex justify-content-center">
                                <button @click="onSaveRecord(newItem)" class="btn btn-color11-2 btn-xs mx-1" title="新增"><i class="fas fa-save"></i></button>
                                <button @click="fAddItem=false" class="btn btn-color11-4 btn-xs mx-1" title="取消"><i class="fas fa-times"></i></button>
                            </div>
                        </td>
                    </tr>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td>{{pageRecordCount*(pageIndex-1)+index+1}}</td>
                        <td>{{item.RptYear}}</td>
                        <td><a href="#" :title="item.RptName" v-on:click.stop="onViewEng(item)">{{item.RptName}}</a></td>
                        <td>{{item.ExecUnit}}</td>
                        <td>{{item.ExecSubUnit}}</td>
                        <td>{{item.ExecUser}}</td>
                        <td>{{item.EngState}}</td>
                        <td style="min-width: 105px;">
                            <div class="row justify-content-center m-0">
                                <button v-if="item.RptTypeSeq==1||item.RptTypeSeq==3" v-on:click.stop="onEditEng(item)" class="btn btn-color11-3 btn-xs mx-1" title="填報">
                                    <i class="fas fa-pencil-alt"></i> 填報
                                </button>
                                <button v-if="item.RptTypeSeq==1||item.RptTypeSeq==3" v-on:click.stop="onDelEng(item)" class="btn btn-color11-4 btn-xs mx-1" title="刪除">
                                    <i class="fas fa-trash-alt"></i> 刪除
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
        <h5>待核/覆核清單</h5>
        <div class="row justify-content-between">
            <comm-pagination class="col-10" :recordTotal="recordTotalB" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        </div>
        <div class="table-responsive">
            <table border="0" class="table table1 min910" id="addnew159">
                <thead class="insearch">
                    <tr>
                        <th class="sort">項次</th>
                        <th class="number text-center">年度</th>
                        <th>案由</th>
                        <th>執行機關</th>
                        <th>執行單位</th>
                        <th>負責人</th>
                        <th>狀態</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in itemsB" v-bind:key="item.Seq">
                        <td>{{pageRecordCountB*(pageIndexB-1)+index+1}}</td>
                        <td>{{item.RptYear}}</td>
                        <td><a href="#" :title="item.RptName" v-on:click.stop="onViewEng(item)">{{item.RptName}}</a></td>
                        <td>{{item.ExecUnit}}</td>
                        <td>{{item.ExecSubUnit}}</td>
                        <td>{{item.ExecUser}}</td>
                        <td>{{item.EngState}}</td>
                        <td style="min-width: 105px;">
                            <div class="row justify-content-center m-0">
                                <button v-if="item.RptTypeSeq==1||item.RptTypeSeq==3" v-on:click.stop="onEditEng(item)" class="btn btn-color11-3 btn-xs mx-1" title="填報">
                                    <i class="fas fa-pencil-alt"></i> 填報
                                </button>
                            </div>
                        </td>
                    </tr>
                    <tr v-if="itemsB==null || itemsB.length==0">
                        <td colspan="8" class="text-center">--查無資料--</td>
                    </tr>
                </tbody>
            </table>
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
                editSeq: -99,
                newItem: {},
                fAddItem: false,
                //分頁
                recordTotal: 0,
                pageRecordCount: 30,
                pageIndex: 1,
                //pageIndexOptions:[],
                //分頁B
                recordTotalB: 0,
                pageRecordCountB: 30,
                pageIndexB: 1,
                //年度
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
                itemsB: [],

                // 執行單位下拉-第二層
                units2: [],
                // 使用者下拉
                users3: [],
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
            onSubUnitChange(event) {
                this.pageIndex = 1;
                if (this.selectRptType == '') this.getSelectRptTypeOption(1);
                this.getList();
                this.getListB();
                sessionStorage.removeItem('selectERSubUnit');
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
                this.getListB();

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
            onNewRecord() {
                window.myAjax.post('/ERNeedAssessment/NewRecord')
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.newItem = resp.data.item;
                            this.newItem.RptYear = '';
                            this.getUnitList2(this.newItem.ExecUnitSeq);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            strEmpty(str) {
                return window.comm.stringEmpty(str);
            },
            //儲存
            onSaveRecord(uItem) {
                //console.log(uItem);
                if (this.strEmpty(uItem.RptYear) || this.strEmpty(uItem.RptName)) {
                    alert('年度, 案由 必須輸入!');
                    return;
                }
                uItem.ExecUnitSeq = this.newItem.ExecUnitSeq;
                uItem.ExecSubUnitSeq = this.newItem.ExecSubUnitSeq;
                uItem.CreateUserSeq = this.newItem.CreateUserSeq;
                window.myAjax.post('/ERNeedAssessment/UpdateEngReport', { m: uItem })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.editSeq = -99;
                            this.getSelectYearOption();
                            //if (this.selectUnitOptions.length == 0) {
                            //    this.getSelectYearOption();
                            //    this.onNewRecord();
                            //}
                            //else 
                            //    this.getList();
                            if (uItem.Seq == -1) this.onNewRecord();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            async getList() {
                this.fAddItem = false;
                if (this.selectYear == '' || this.selectUnit == '' || this.selectRptType == '') return;
                if (this.selectSubUnit == null || this.selectUnit == '') this.selectSubUnit = -1;
                window.myAjax.post('/ERNeedAssessment/GetList'
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
            async getListB() {
                this.fAddItem = false;
                if (this.selectYear == '' || this.selectUnit == '' || this.selectRptType == '') return;
                if (this.selectSubUnit == null || this.selectUnit == '') this.selectSubUnit = -1;
                window.myAjax.post('/ERNeedAssessment/GetListB'
                    , {
                        year: this.selectYear,
                        unit: this.selectUnit,
                        subUnit: this.selectSubUnit,
                        rptType: this.selectRptType,
                        pageRecordCount: this.pageRecordCount,
                        pageIndex: this.pageIndex
                    })
                    .then(resp => {
                        this.itemsB = resp.data.items;
                        this.recordTotalB = resp.data.pTotal;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onDelEng(item) {
                if (!confirm('是否確定刪除資料？')) return;
                window.myAjax.post('/ERNeedAssessment/DelEngReport', { id: item.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getSelectYearOption();
                            this.onNewRecord();
                            //this.getList();// this.engSupervisor = resp.data.items;
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onEditEng(item) {
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
            //下載
            onDownloadNeedAssessment() {
                window.comm.dnFile('/EngReport/DownloadNeedAssessmentVF?year=' + this.selectYear + '&unit=' + this.selectUnit + '&subUnit=' + this.selectSubUnit + '&rptType=' + this.selectRptType);
            },
            // 取得執行單位下拉選單2
            getUnitList2(unitSeq = 0) {
                const self = this;
                let params = { parentSeq: unitSeq };
                window.myAjax.post('/Unit/GetUnitList', params)
                    .then(resp => {
                        self.units2 = resp.data;
                        let obj = new Object();
                        obj.target = new Object();
                        obj.target.value = this.userUnitSub;//this.userUnit;
                        self.onChangeUnit2(obj);
                    })
                    .then(err => {
                        //console.log(err);
                    });
            },
            // 選擇單位第二層
            onChangeUnit2(value) {
                const self = this;
                self.getUserList(value.target.value);
            },
            // 取得人員列表
            getUserList(unitSeq = 0) {
                const self = this;
                if (unitSeq != null) {
                    let params = { page: 0, per_page: 0, unitSeq: unitSeq, nameSearch: '' };
                    window.myAjax.post('/ERNeedAssessment/GetUserList', params)
                        .then(resp => {
                            this.users3 = resp.data.l;
                        })
                        .then(err => {
                            //console.log(err);
                        });
                }
            },
        },
        async mounted() {
            console.log('mounted() 需求評估');
            if (this.selectYearOptions.length == 0) {
                await this.getSelectYearOption();
            }
            this.onNewRecord();
        }
    }
</script>