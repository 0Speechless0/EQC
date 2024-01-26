<template>
    <div>
        <form @submit.prevent>
            <div>
                <!-- hr class="my-5" -->
                <div class="setFormcontentCenter">
                    <div class="form-row">
                        <div class="col-12 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">工程名稱<span class="small-red">&nbsp;*</span></label>
                            <input v-model.trim="engMain.EngName" type="text" class="col-12 col-md-10 form-control">
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">工程地點</label>
                            <div>
                                <select v-model="engMain.CitySeq" @change="onCityChange($event)"
                                        class="form-control my-1 mr-0 mr-sm-1">
                                    <option v-for="option in cities" v-bind:value="option.Value"
                                            v-bind:key="option.Value">
                                        {{ option.Text }}
                                    </option>
                                </select>
                                <select v-model="engMain.EngTownSeq"
                                        class="form-control my-1 mr-0 mr-md-1 mr-0 mr-md-4">
                                    <option v-for="option in towns" v-bind:value="option.Value"
                                            v-bind:key="option.Value">
                                        {{ option.Text }}
                                    </option>
                                </select>
                            </div>
                        </div>
                        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">工程年度<span class="small-red">&nbsp;*</span></label>
                            <input v-model="engMain.EngYear" type="number" class="form-control" />
                        </div>
                    </div>
                    <div class="form-row">

                        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">工程編號<span class="small-red">&nbsp;*</span></label>
                            <input v-model.trim="engMain.EngNo" type="text"
                                   class="form-control my-1 mr-0 mr-md-4 WidthasInput" placeholder="PCCES帶入">
                        </div>
                        <!-- div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">工程會標案編號</label>
                        <div role="group" class="input-group">
                            <input v-model.trim="engMain.TenderNo" disabled type="text" class="form-control">
                            <button v-on:click="onTenderSearch()" v-bind:disabled="(this.engMain.Seq == -1)"
                                    title="標案查詢" class="btn btn-sm bg-gray" data-toggle="modal"
                                    data-target="#refTenderListModal">
                                <i class="fas fa-search"></i>
                            </button>
                            <button v-on:click="onCancelTenderLink()" title="清除" class="btn btn-sm bg-gray">
                                <i class="fas fa-times"></i>
                            </button>
                        </div>

                    </div -->
                    </div>
                    <div class="form-row">
                        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">主辦機關<span class="small-red">&nbsp;*</span></label>
                            <select v-model="engMain.OrganizerUnitSeq"
                                    @change="getOrganizerSubUnit(engMain.OrganizerUnitSeq)"
                                    class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                                <option v-for="option in units" v-bind:value="option.Value" v-bind:key="option.Value">
                                    {{ option.Text }}
                                </option>
                            </select>
                        </div>
                        <!-- div class="col-8 col-md-4  my-2 justify-content-md-between">
                        <label for="a">工程會標案編號</label><input type="text" disabled
                                                             class="form-control my-1 mr-0 mr-md-4" id="a" v-model="engMain.TenderNo">

                    </div>
                    <div class="col-8 col-md-4  my-2 justify-content-md-between">
                        <label for="a">工程會標案名稱</label><input type="text" disabled
                                                             class="form-control my-1 mr-0 mr-md-4" id="a" v-model="engMain.TenderName">

                    </div -->
                    </div>
                    <div class="form-row">
                        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">執行機關<span class="small-red">&nbsp;*</span></label>
                            <select v-model="engMain.ExecUnitSeq" @change="getExecSubUnit(engMain.ExecUnitSeq)"
                                    ref="refExecUnit" class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                                <option v-for="option in units" v-bind:value="option.Value" v-bind:key="option.Value">
                                    {{ option.Text }}
                                </option>
                            </select>
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">執行單位<span class="small-red">&nbsp;*</span></label>
                            <select v-model="engMain.ExecSubUnitSeq" @change="getUsers()"
                                    class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                                <option v-for="option in execSubUnits" v-bind:value="option.Value"
                                        v-bind:key="option.Value">
                                    {{ option.Text }}
                                </option>
                            </select>
                        </div>
                        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">標案建立者<span class="small-red">&nbsp;*</span></label>
                            <select v-model="engMain.OrganizerUserSeq" 
                                    class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                                <option v-for="option in users" v-bind:value="option.Value" v-bind:key="option.Value">
                                    {{ option.Text }}
                                </option>
                            </select>
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">工程總預算(元) (來自工程提報)</label>
                            <input v-model="engMain.TotalBudget" v-bind:disabled="!isAdmin" @change="onTotalBudgetChange()" type="text"
                                   class="form-control my-1 mr-0 mr-md-4" placeholder="預設發包預算">
                        </div>
                        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">發包預算(元) (來自預算書XML)</label>
                            <input v-model="engMain.SubContractingBudget" v-bind:disabled="!isAdmin" type="text"
                                   class="form-control my-1 mr-0 mr-md-4" placeholder="PCCES帶入">
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="col-12 col-md-7 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">決標金額 (來自工程會標案管理系統)</label>
                            <input v-model="engMain.BidAmount" v-bind:disabled="!isAdmin" type="text"
                                   class="form-control my-1 mr-0 mr-md-4" placeholder="來自工程會標案管理系統">
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">決標日期</label>
                            <b-input-group>
                                <input v-bind:value="engMain.chsAwardDate" ref="chsAwardDate" @change="onDateChange(engMain.chsAwardDate, $event, 'AwardDate')" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                                <b-form-datepicker v-model="chsAwardDate" :hide-header="hideHeader"
                                                   button-only right size="sm" @context="onDatePicketChange($event, 'AwardDate')">
                                </b-form-datepicker>
                            </b-input-group>
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="col-12 col-md-6 form-inline my-2">
                            <label class="my-2 mx-2">需求碳排量(kgCO2e)</label>
                            <input v-model="engMain.CarbonDemandQuantity" v-bind:disabled="engMain.OfficialApprovedCarbonQuantity" type="text"
                                   class="form-control my-1 ml-1 mr-0 mr-md-4" style="width:120px;">
                        </div>
                        <div class="col-12 col-md-6 form-inline my-2">
                            <label class="my-2 mx-2">核定碳排量(kgCO2e)</label>
                            <input v-model="engMain.ApprovedCarbonQuantity" v-bind:disabled="engMain.OfficialApprovedCarbonQuantity" type="text"
                                   class="form-control my-1 ml-1 mr-0 mr-md-4" style="width:120px;">
                        </div>
                    </div>
                </div>
            </div>
        </form>
        <div class="row justify-content-center mt-5">
            <div class="d-flex">
                <button v-on:click.stop="updateData()" role="button" class="btn btn-color11-2 btn-xs mx-1">
                    <i class="fas fa-save">&nbsp;儲存</i>
                </button>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['engApprovalItem'],
        data: function () {
            return {
                fSendMail: false,//防止連續發送
                fSendMailSupervisorUnit: false,
                fSendMailBuildContractor: false,
                fCanEdit: false,
                saveFlag: false,
                updateXMLMode: false,//xml 工程編號存在是否覆蓋
                targetId: null,
                isAdd: false,
                file: null,//{ name: null, size: null },
                files: new FormData(),
                dragging: false,
                step: -1,

                //使用者單位資訊
                isAdmin: false,
                userUnit: null,
                userUnitName: '',
                userUnitSub: null,
                userUnitSubName: '',

                users: [],//人員清單
                units: [],//機關清單
                organizerSubUnits: [],//主辦機關單位清單
                execUnitName: '',
                execSubUnits: [],//執行機關單位清單
                cities: [],//行政區(縣市)清單
                towns: [],//行政區(鄉鎮)清單
                engMain: {
                    Seq: -1, CitySeq: null, EngTownSeq: null, chsAwardDate: null, BidAmount:null,
                    EngNo: '', EngName: '', EngYear: '',
                    OrganizerUnitSeq: -1, 
                    ExecUnitSeq: -1, ExecSubUnitSeq: -1, OrganizerUserSeq: -1,
                    TotalBudget: null, SubContractingBudget: null, CarbonDemandQuantity: null, ApprovedCarbonQuantity:null
                },

                engConstruction: { Seq: -1, EngMainSeq: -1, ItemName: '', ItemQty: 0, ItemUnit: '', edit: false },//工程主要施工項目及數量
                engConstructionList: [],//工程主要施工項目及數量 清單

                engAttachmentList: [], //上傳監造計畫附件 清單
                selectChapter: 1, //上傳監造計畫附件 章
                selectFileType: 1,
                engAttachmentDescription: '',

                chsAwardDate: '', //決標日期 shioulo 20220618
                hideHeader: true,
                tenders: [], //標案清單 shioulo 20220504
                //自辦監造人員
                supervisorKind: 1,
                supervisorSubUnitSeq: -1,
                supervisorUserSeq: -1,
                supervisorUsers: [],
                engSupervisor: [],
                //s20230327 可自行新增工程會工程
                fAddPrjXMLEng: false,
                prjXMLEng: { Seq: -1, TenderNo: '', TenderYear: -1, TenderName: '', ExecUnitName: '' },
                engImportMode: "0", //s20230830
                userRole:-1, //s20231006
            };
        },
        methods: {
            updateData() {
                ///*this.engMain.OrganizerSubUnitSeq == null ||
                if (this.engMain.EngName == null || this.engMain.EngName.length == 0
                    || this.engMain.EngNo == null || this.engMain.EngNo.length == 0
                    || this.engMain.OrganizerUnitSeq == null || this.engMain.OrganizerUserSeq == null
                    || this.engMain.ExecUnitSeq == null || this.engMain.ExecSubUnitSeq == null) {
                    alert('* 欄位必須選填');
                    return;
                }

                this.engMain.chsAwardDate = this.$refs['chsAwardDate'].value;
                this.$emit('createEng', this.engMain);
            },
        //s20230327 可自行新增工程會工程
        addPrjXMLEng() {
            var execUnitName = ''
            if (this.$refs.refExecUnit.options.selectedIndex >= 0) {
                execUnitName = this.units[this.$refs.refExecUnit.options.selectedIndex].Text;
            }
            if (!window.comm.stringEmpty(this.execUnitName)) {

                this.prjXMLEng = { Seq: -1, TenderNo: '', TenderYear: -1, TenderName: '', ExecUnitName: '' };
                this.prjXMLEng.ExecUnitName = execUnitName;
                this.prjXMLEng.TenderYear = this.engMain.EngYear;
                this.prjXMLEng.TenderNo = this.engMain.EngNo;
                this.prjXMLEng.TenderName = this.engMain.EngName;
                this.fAddPrjXMLEng = true;
            }
            
        },
        savePrjXMLEng() {
            window.myAjax.post('/TenderPlan/AddPrjXMLEng', { eng: this.prjXMLEng })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.onTenderSearch();
                        this.fAddPrjXMLEng = false;
                    } else {
                        alert(resp.data.msg);
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //自辦監造人員
        async getSupervisorUsers() {
            this.supervisorUsers = [];
            const { data } = await window.myAjax.post('/TenderPlan/GetUserList', { organizerUnitSeq: this.engMain.ExecUnitSeq, organizerSubUnitSeq: this.supervisorSubUnitSeq });
            this.supervisorUsers = data;
        },
        addSupervisorUser() {
            window.myAjax.post('/TenderPlan/SupervisorUserAdd', { eng: this.targetId, kind: this.supervisorKind, subUnit: this.supervisorSubUnitSeq, id: this.supervisorUserSeq })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.engSupervisor = resp.data.items;
                    } else {
                        alert(resp.data.msg);
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        delEngSupervisor(item) {
            window.myAjax.post('/TenderPlan/SupervisorUserDel', { eng: this.targetId, id: item.Seq })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.engSupervisor = resp.data.items;
                    } else {
                        alert(resp.data.msg);
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        getEngSupervisor() {
            this.engSupervisor = [];
            window.myAjax.post('/TenderPlan/GetSupervisorUser', { id: this.targetId })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.engSupervisor = resp.data.items
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        kindCaption(kind) {
            if (kind == 0)
                return "監造主任";
            else if (kind == 2)
                return "標案建立者";
            else if (kind == 3)
                return "設計人員";
            else 
                return "現場人員";
        },
        //
        onExecTypeChange() {
            //console.log('onExecTypeChange()');
            if (this.userUnitName != '水利署') return;

            if (this.engMain.DesignUnitName == '') {
                this.engMain.DesignUnitName = this.userUnitSubName;
            }
            if (this.engMain.SupervisorUnitName == '') {
                this.engMain.SupervisorUnitName = this.userUnitSubName;
            }
        },
        getItem() {
            this.fCanEdit = true;
            this.step = 2;
            this.engMain = {};
            this.userRole = -1;
            window.myAjax.post('/TenderPlan/GetEngItem', { id: this.targetId })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.engMain = resp.data.item;
                        this.userRole = resp.data.ur;
                        this.getCityTown();
                        this.getEngConstructionList();
                        this.getEngAttachmentList();
                        this.getOrganizerSubUnit(this.engMain.OrganizerUnitSeq);
                        this.getUsers();
                        this.getExecSubUnit(this.engMain.ExecUnitSeq);
                        this.getEngSupervisor();
                        if (this.engMain.DocState == null || this.engMain.DocState == -1) {
                            this.fCanEdit = true;
                            //this.chsStartDate = this.toYearDate(this.engMain.chsStartDate);
                            //this.chsSchCompDate = this.toYearDate(this.engMain.chsSchCompDate);
                            //this.chsPostCompDate = this.toYearDate(this.engMain.chsPostCompDate);
                            //this.chsApproveDate = this.toYearDate(this.engMain.chsApproveDate);//shioulo 20220
                            this.chsAwardDate = this.toYearDate(this.engMain.chsAwardDate);
                        }
                    } else {
                        alert(resp.data.message);
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        onChangeSupervisorUnit() {
            this.fSendMailSupervisorUnit = false;
        },
        onChangeBuildContractor() {
            this.fSendMailBuildContractor = false;
        },
        //xml 檔案上傳處裡
        onChange(e) {
            // 判斷拖拉上傳或點擊上傳的 event
            var files = e.target.files || e.dataTransfer.files;

            // 預防檔案為空檔
            if (!files.length) {
                this.dragging = false;
                return;
            }

            this.createFile(files[0]);
        },
        createFile(file) {
            // 附檔名判斷
            //console.log(file);
            if (!file.type.match('text/xml')) {
                alert('請選擇 xml 檔案');
                this.dragging = false;
                return;
            }
            this.file = file;
            this.dragging = false;

            this.files.append("file", this.file, this.file.name);
        },
        uploadXML(processMode) {
            this.files.set("processMode", processMode);
            const files = this.files;
            window.myAjax.post('/TenderPlan/UploadXML', files,
                {
                    headers: {
                        'Content-Type': 'multipart/form-data'
                    }
                })
                .then(resp => {
                    if (resp.data.result == 0) {
                        alert(resp.data.message);
                        this.targetId = resp.data.item;
                        this.editEng(this.targetId);
                        window.sessionStorage.setItem(window.eqSelTrenderPlanSeq, this.targetId);
                        //this.getItem();
                        //this.removeFile();
                    } else {
                        this.updateXMLMode = (resp.data.result == -2);
                        alert(resp.data.message);
                    }
                }).catch(error => {
                    console.log(error);
                });
        },
        editEng(engSeq) {
            window.myAjax.post('/TenderPlan/EditEng', { seq: engSeq })
                .then(resp => {
                    console.log(resp.data.Url);
                    window.location.href = resp.data.Url;
                })
                .catch(err => {
                    console.log(err);
                });
        },
        removeFile() {
            this.file = '';
            this.files = new FormData();
            this.updateXMLMode = false;
        },
        //取得使用者單位資訊
        getUserUnit() {
            window.myAjax.post('/TenderPlan/GetUserUnit')
                .then(resp => {
                    this.userUnit = resp.data.unit;
                    this.userUnitName = resp.data.unitName;
                    this.userUnitSub = resp.data.unitSub;
                    this.userUnitSubName = resp.data.unitSubName;

                    this.selectUnit = this.userUnit;
                    //this.onUnitChange(this.selectUnit)
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //人員清單
        async getUsers() {
            this.users = [];
            //const { data } = await window.myAjax.post('/TenderPlan/GetUserList', { organizerUnitSeq: this.engMain.OrganizerUnitSeq, organizerSubUnitSeq: this.engMain.OrganizerSubUnitSeq });
            //20220707 改為執行機關聯絡人
            const { data } = await window.myAjax.post('/TenderPlan/GetUserList', { organizerUnitSeq: this.engMain.ExecUnitSeq, organizerSubUnitSeq: this.engMain.ExecSubUnitSeq });
            this.users = data;
        },
        //機關清單
        async getUnits() {
            this.units = [];
            const { data } = await window.myAjax.post('/TenderPlan/GetUnitList', { parentUnit: -1 });
            this.units = data;
        },
        //主辦機關-單位清單
        async getOrganizerSubUnit(unitSeq) {
            this.organizerSubUnits = [];
            /*this.users = [];
            const { data } = await window.myAjax.post('/TenderPlan/GetUnitList', { parentUnit: unitSeq });
            this.organizerSubUnits = data;*/
        },
        //執行機關-單位清單
        async getExecSubUnit(unitSeq) {
            this.execSubUnits = [];
            this.users = [];
            const { data } = await window.myAjax.post('/TenderPlan/GetUnitList', { parentUnit: unitSeq });
            this.execSubUnits = data;
        },
        //執行機關-名稱
        getExecUnitName() {
            console.log(this.$refs.refExecUnit);
        },
        //行政區(縣市)
        async getCities() {
            this.cities = [];
            const { data } = await window.myAjax.post('/TenderPlan/GetCityList');
            this.cities = data;
        },
        onCityChange(event) {
            this.getCityTown();
        },
        //行政區(鄉鎮)
        async getCityTown() {
            if (this.engMain.CitySeq > 0) {
                this.twons = [];
                const { data } = await window.myAjax.post('/TenderPlan/GetTownList', { id: this.engMain.CitySeq });
                this.towns = data;
            }
        },
        //工程總預算變更
        onTotalBudgetChange() {
            if (this.engMain.TotalBudget > 50000000) {
                this.engMain.PurchaseAmount = 1;
            } else if (this.engMain.TotalBudget > 10000000) {
                this.engMain.PurchaseAmount = 2;
            } else if (this.engMain.TotalBudget > 1000000) {
                this.engMain.PurchaseAmount = 3;
            } else {
                this.engMain.PurchaseAmount = 0;
            }
        },
        onDateChange(srcDate, event, mode) {
            if (event.target.value.length == 0) {
                /*if (mode == 'SchCompDate') this.chsSchCompDate = '';
                else if (mode == 'PostCompDate') this.chsPostCompDate = '';
                else if (mode == 'StartDate') this.chsStartDate = '';
                else if (mode == 'ApproveDate') this.chsApproveDate = '';
                else */if (mode == 'AwardDate') this.chsAwardDate = '';
                return;
            }
            if (!this.isExistDate(event.target.value)) {
                event.target.value = srcDate;
                alert("日期錯誤");
            } else {
                /*if (mode == 'SchCompDate') this.chsSchCompDate = this.toYearDate(event.target.value);
                else if (mode == 'PostCompDate') this.chsPostCompDate = this.toYearDate(event.target.value);
                else if (mode == 'StartDate') this.chsStartDate = this.toYearDate(event.target.value);
                else if (mode == 'ApproveDate') this.chsApproveDate = this.toYearDate(event.target.value);
                else */if (mode == 'AwardDate') this.chsAwardDate = this.toYearDate(event.target.value);
            }
        },
        onDatePicketChange(ctx, mode) {
            //console.log(ctx);
            if (ctx.selectedDate != null) {
                var d = ctx.selectedDate;
                var dd = (d.getFullYear() - 1911) + '/' + (d.getMonth() + 1) + '/' + d.getDate();
                //var y = d.getYear() - 1911;
                /*if (mode == 'SchCompDate') this.engMain.chsSchCompDate = dd;
                else if (mode == 'PostCompDate') this.engMain.chsPostCompDate = dd;
                //else if (mode == 'StartDate') this.engMain.chsStartDate = dd;
                //else if (mode == 'ApproveDate') this.engMain.chsApproveDate = dd;
                //else */if (mode == 'AwardDate') this.engMain.chsAwardDate = dd;
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
        async getEngConstructionList() {
            const { data } = await window.myAjax.post('/TenderPlan/GetECList', { engMain: this.engMain.Seq });
            this.engConstructionList = data;
        },
        //新增 工程主要施工項目及數量
        addConstruction() {
            this.engConstruction.EngMainSeq = this.engMain.Seq;
            window.myAjax.post('/TenderPlan/ConstructionAdd', { item: this.engConstruction })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.engConstruction.Seq = resp.data.Seq;
                        //alert(resp.data.message);
                        this.engConstructionList.push(this.engConstruction);
                        //
                        this.engConstruction = { Seq: -1, EngMainSeq: -1, ItemName: '', ItemQty: 0, ItemUnit: '', edit: false };
                    } else {
                        alert(resp.data.message);
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //更新 工程主要施工項目及數量
        saveConstruction(item) {
            window.myAjax.post('/TenderPlan/ConstructionUpdate', { item: item })
                .then(resp => {
                    if (resp.data.result == 0) {
                        item.edit = false;
                    } else
                        alert(resp.data.message);
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //刪除
        delConstruction(index, id) {
            window.myAjax.post('/TenderPlan/ConstructionDel', { seq: id })
                .then(resp => {
                    this.engConstructionList.splice(index, 1);
                    alert(resp.data.message);
                    console.log(resp);
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //監造計畫附件 
        onChapterChange(event) {
            this.getEngAttachmentList();
        },
        //監造計畫附件 清單
        async getEngAttachmentList() {
            const { data } = await window.myAjax.post('/TenderPlan/EngAttachmentList', { engMain: this.engMain.Seq, chapter: this.selectChapter });
            this.engAttachmentList = data;
            if (this.engAttachmentList.length > 0) {
                this.selectFileType = this.engAttachmentList[0].FileType;
            }
        },
        //xml 檔案上傳處裡
        onChartFileChange(e) {
            // 判斷拖拉上傳或點擊上傳的 event
            var files = e.target.files || e.dataTransfer.files;
            // 預防檔案為空檔
            if (!files.length) {
                this.dragging = false;
                return;
            }

            this.createChartFile(files[0]);
        },
        createChartFile(file) {
            console.log(file);
            if(file.name.split('.')[1] != 'jpg') {
                file = null;
                alert("請上傳JPG格式");
            }
            /*if (!file.type.match('text/xml')) {// 附檔名判斷
                alert('請選擇 xml 檔案');
                this.dragging = false;
                return;
            }*/
            this.file = file;
            this.dragging = false;

            this.files.append("file", this.file, this.file.name);
        },
        //上傳圖表
        uploadChart() {
            this.files.append("engMain", this.engMain.Seq);
            this.files.append("chapter", this.selectChapter);
            this.files.append("fileType", this.selectFileType);
            this.files.append("description", this.engAttachmentDescription);
            const files = this.files;
            window.myAjax.post('/TenderPlan/EngAttachmentUpload', files,
                {
                    headers: {
                        'Content-Type': 'multipart/form-data'
                    }
                })
                .then(resp => {
                    if (resp.data.result == 0) {
                        alert(resp.data.message);
                        this.targetId = resp.data.item;
                        this.getEngAttachmentList();
                        this.file = '';
                        this.files = new FormData();
                    } else {
                        alert(resp.data.message);
                    }
                }).catch(error => {
                    console.log(error);
                });
        },
        //更新 上傳監造計畫附件
        saveAttachment(item) {
            window.myAjax.post('/TenderPlan/EngAttachmentUpdate', { item: item })
                .then(resp => {
                    if (resp.data.result == 0) {
                        item.edit = false;
                    }
                    alert(resp.data.message);
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //刪除 上傳監造計畫附件
        delAttachment(index, id) {
            window.myAjax.post('/TenderPlan/EngAttachmentDel', { seq: id })
                .then(resp => {
                    this.engAttachmentList.splice(index, 1);
                    alert(resp.data.message);
                    console.log(resp);
                })
                .catch(err => {
                    console.log(err);
                });
        },
        download(item) {
            window.myAjax.get('/TenderPlan/EngAttachmentDownload?seq=' + item.Seq, { responseType: 'blob' })
                .then(resp => {
                    const blob = new Blob([resp.data]);
                    const contentType = resp.headers['content-type'];
                    if (contentType.indexOf('application/json') >= 0) {
                        //alert(resp.data);
                        const reader = new FileReader();
                        reader.addEventListener('loadend', (e) => {
                            const text = e.srcElement.result;
                            const data = JSON.parse(text)
                            alert(data.message);
                        });
                        reader.readAsText(blob);
                    } else if (contentType.indexOf('application/blob') >= 0) {
                        var saveFilename = null;
                        const data = decodeURI(resp.headers['content-disposition']);
                        var array = data.split("filename*=UTF-8''");
                        if (array.length == 2) {
                            saveFilename = array[1];
                        } else {
                            array = data.split("filename=");
                            saveFilename = array[1];
                        }
                        if (saveFilename != null) {
                            const url = window.URL.createObjectURL(blob);
                            const link = document.createElement('a');
                            link.href = url;
                            link.setAttribute('download', saveFilename);
                            document.body.appendChild(link);
                            link.click();
                        } else {
                            console.log('saveFilename is null');
                        }
                    } else {
                        alert('格式錯誤下載失敗');
                    }
                }).catch(error => {
                    console.log(error);
                });
        },
        //
        createSupervisionProject() {
            if (!this.saveFlag) return;

            //if (confirm('系統將更新主檔及其相關資料表外，如材料設備清單、施工管理清單、設備運轉清單、職業安全、環境保育等資料\n\n是否確定?')) {
                window.myAjax.post('/TenderPlan/CreateSupervisionProject', { engMain: this.engMain.Seq, im: this.engImportMode })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engMain.DocState = 0;
                            document.getElementById('closeImportModal').click();
                        }
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            //}
        },
        //檢查公司統一編號是否正確
        checkCompanyNo(idvalue) {
            var tmp = new String("12121241");
            var sum = 0;
            var re = /^\d{8}$/;
            if (!re.test(idvalue)) {
                //alert("格式不對！");
                return false;
            }
            var i = 0;
            for (i = 0; i < 8; i++) {
                var s1 = parseInt(idvalue.substr(i, 1));
                var s2 = parseInt(tmp.substr(i, 1));
                sum += this.cal(s1 * s2);
            }
            if (!this.valid(sum)) {
                if (idvalue.substr(6, 1) == "7") return (this.valid(sum + 1));
            }
            return (this.valid(sum));
        },
        valid(n) {
            return (n % 10 == 0) ? true : false;
        },
        cal(n) {
            var sum = 0;
            while (n != 0) {
                sum += (n % 10);
                n = (n - n % 10) / 10;  // 取整數
            }
            return sum;
        },
        //施工廠商發送mail
        onSendMailBuildContractor() {
            if (!this.checkCompanyNo(this.engMain.BuildContractorTaxId)) {
                alert('施工廠商統編錯誤');
                return;
            }
            if (!this.validateEmail(this.engMain.BuildContractorEmail)) {
                alert('聯絡人mail錯誤');
                return;
            }
            if (this.engMain.BuildContractorName && this.engMain.BuildContractorName.length > 0
                && this.engMain.BuildContractorContact && this.engMain.BuildContractorContact.length > 0) {
                this.fSendMail = true;
                window.myAjax.post('/TenderPlan/SendMailToBuildContractor', { engMain: this.engMain.Seq })
                    .then(resp => {
                        alert(resp.data.message);
                        this.fSendMail = false;
                    })
                    .catch(err => {
                        console.log(err);
                        this.fSendMail = false;
                    });

            } else {
                alert('施工廠商名稱,施工廠商連絡人 資料錯誤');
            }

        },
        //監造單位發送mail
        onSendMailSupervisorUnit() {
            console.log('onSendMailSupervisorUnit()');
            if (!this.checkCompanyNo(this.engMain.SupervisorTaxid)) {
                alert('監造單位統編錯誤');
                return;
            }
            if (!this.validateEmail(this.engMain.SupervisorContact)) {
                alert('聯絡人mail錯誤');
                return;
            }
            if (this.engMain.SupervisorUnitName && this.engMain.SupervisorUnitName.length > 0
                && this.engMain.SupervisorDirector && this.engMain.SupervisorDirector.length > 0) {
                this.fSendMail = true;
                window.myAjax.post('/TenderPlan/SendMailToSupervisor', { engMain: this.engMain.Seq })
                    .then(resp => {
                        alert(resp.data.message);
                        this.fSendMail = false;
                    })
                    .catch(err => {
                        console.log(err);
                        this.fSendMail = false;
                    });

            } else {
                alert('監造單位,監造主任 資料錯誤');
            }
        },
        validateEmail(email) {
            const re = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return re.test(String(email).toLowerCase());
        },
        back() {
            //window.history.go(-1);
            window.location = "/TenderPlan/TenderPlanList";
        },
        //標案查詢 shioulo 20220504
        onTenderSearch() {
            this.tenders = [];
            if (this.engMain.ExecUnitSeq > 0) {
                if (this.$refs.refExecUnit.options.selectedIndex >= 0)
                    this.execUnitName = this.units[this.$refs.refExecUnit.options.selectedIndex].Text;
                else
                    this.execUnitName = '';

                window.myAjax.post('/TenderPlan/GetTenderList', { id: this.engMain.Seq })
                    .then(resp => {
                        this.tenders = resp.data;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            } else {
                alert('沒有執行單位');
            }
        },
        //工程連結標案 shioulo 20220518
        onSelectTrender(item) {
            window.myAjax.post('/TenderPlan/SetEngLinkTender', { id: this.engMain.Seq, prj: item.Seq })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.engMain.PrjXMLSeq = item.Seq;
                        this.engMain.TenderNo = item.TenderNo;
                        this.engMain.TenderName = item.TenderName;
                        document.getElementById("closeTenderListModal").click();
                    }
                    alert(resp.data.msg);
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //工程連結標案取消 shioulo 20220519
        onCancelTenderLink() {
            window.myAjax.post('/TenderPlan/CancelEngLinkTender', { id: this.engMain.Seq })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.engMain.PrjXMLSeq = null;
                        this.engMain.TenderNo = "";
                    }
                    alert(resp.data.msg);
                })
                .catch(err => {
                    console.log(err);
                });
        }
    },
    computed: {
        // 前端擷取附檔名
        extension() {
            return (this.file) ? this.file.name.split('.').pop() : '';
        },
        getSupervisorUnitState() {
            return this.fSendMailSupervisorUnit && !this.fSendMail;
        },
        getBuildContractorState() {
            return this.fSendMailBuildContractor && !this.fSendMail;
        },
    },
    async mounted() {
        console.log('mounted 新標案編輯 ');
        if (this.units.length == 0) this.getUnits();
        if (this.cities.length == 0) this.getCities();
        this.isAdmin = localStorage.getItem('isAdmin') == 'True' ? true : false;
        this.engMain.OrganizerUnitSeq = this.engApprovalItem.ExecUnitSeq;
        this.engMain.ExecUnitSeq = this.engApprovalItem.ExecUnitSeq;
        this.engMain.ExecSubUnitSeq = this.engApprovalItem.ExecSubUnitSeq;
        this.getExecSubUnit(this.engMain.ExecUnitSeq);
        this.engMain.OrganizerUserSeq = this.engApprovalItem.OrganizerUserSeq;
        this.getUsers();
        this.engMain.EngNo = this.engApprovalItem.EngNo;
        this.engMain.EngName = this.engApprovalItem.EngName;
        this.engMain.EngYear = this.engApprovalItem.EngYear;
        this.engMain.TotalBudget = this.engApprovalItem.TotalBudget;
        this.engMain.SubContractingBudget = this.engApprovalItem.SubContractingBudget;
        this.engMain.CarbonDemandQuantity = this.engApprovalItem.CarbonDemandQuantity;
        this.engMain.ApprovedCarbonQuantity = this.engApprovalItem.ApprovedCarbonQuantity;
        /*let urlParams = new URLSearchParams(window.location.search);
        if (urlParams.has('id')) {
            if (this.units.length == 0) this.getUnits();
            if (this.cities.length == 0) this.getCities();
            //if (this.users.length == 0) this.getUsers();
           
            this.targetId = parseInt(urlParams.get('id'), 10);
            // console.log(this.targetId);
            if (Number.isInteger(this.targetId)) {
                if (this.targetId <= 0) {
                    this.isAdd = true;
                    this.step = 1;
                } else {
                    this.getUserUnit();
                    this.getItem();
                }
                return;
            }

        }*/
    }
}
</script>