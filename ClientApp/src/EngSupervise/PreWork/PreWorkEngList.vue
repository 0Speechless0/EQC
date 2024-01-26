addOutCommitte
<template>
    <div>
        <form class="form-group insearch" style="padding-bottom:0px;">
            <div class="form-row">
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                    <input v-model.trim="keyWord" type="text" placeholder="請輸入督導期別" class="form-control">
                </div>
                <div class="d-flex">
                    <button v-on:click.stop="onSearchPhase" type="button" class="btn btn-outline-secondary btn-xs mx-1"><i
                            class="fas fa-search"></i> 查詢</button>
                </div>
            </div>
            <div style="padding-top: 10px">
                <ul class="nav nav-tabs" role="tablist">
                    <li v-for="(item, index) in yearPhaseItems" v-bind:key="item.Seq" class="nav-item">
                        <a v-on:click="onTabPhaseSelect(item)" data-toggle="tab" href="##"
                            class="nav-link">{{ item.PhaseCode }}</a>
                    </li>
                </ul>
            </div>
        </form>
        <div class="row justify-content-between">
            <div class="form-inline col-12 col-md-8 small">
                <label class="my-1 mr-2">
                    共
                    <span class="text-danger">{{ recordTotal }}</span>
                    筆，每頁顯示
                </label>
                <select v-model="pageRecordCount" @change="onPageRecordCountChange($event)"
                    class="form-control sort form-control-sm">
                    <option value="30">30</option>
                    <option value="50">50</option>
                    <option value="100">100</option>
                </select>
                <label class="my-1 mx-2">筆，共<span class="text-danger">{{ pageTotal }}</span>頁，目前顯示第</label>
                <select v-model="pageIndex" @change="onPageChange($event)" class="form-control sort form-control-sm">
                    <option v-for="option in pageIndexOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                        {{ option.Text }}
                    </option>
                </select>
                <label class="my-1 mx-2">頁</label>
            </div>
        </div>
        <div class="table-responsive">
            <table class="table table-responsive-md table-hover">
                <thead>
                    <tr>
                        <th style="width: 42px;"><strong>項次</strong></th>
                        <th><strong>工程編號</strong></th>
                        <th><strong>工程名稱</strong></th>
                        <th style="width: 120px;"><strong>執行機關</strong></th>
                        <th style="width: 120px;"><strong>設計單位</strong></th>
                        <th style="width: 120px;"><strong>監造單位</strong></th>
                        <th style="width: 95px;"><strong>狀態</strong></th>
                        <th style="width: 42px;"><strong>功能</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in targetPhaseItems" v-bind:key="item.Seq">
                        <td><strong>{{ pageRecordCount * (pageIndex - 1) + index + 1 }}</strong></td>
                        <td>{{ item.EngNo }}</td>
                        <td>{{ item.EngName }}</td>
                        <td>{{ item.ExecUnit }}</td>
                        <td>{{ item.DesignUnitName }}</td>
                        <td>{{ item.SupervisionUnitName }}</td>
                        <td><i v-bind:class="getStateCss(item.ExecState)"></i>{{ item.ExecState }}</td>
                        <td>
                            <div class="d-flex">
                                <button @click="onEditEng(item)" class="btn btn-color11-1 btn-xs sharp mr-1"
                                    data-toggle="modal" data-target="#prepare_edit01" title="編輯"><i
                                        class="fas fa-pencil-alt"></i></button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <!-- 小視窗 編輯人員 -->
        <div class="modal fade" id="prepare_edit01">
            <div v-if="targetPhase != null" class="modal-dialog modal-xl modal-dialog-centered ">
                <div class="modal-content">
                    <div class="modal-header bg-3 text-white">
                        <h6 class="modal-title font-weight-bold">編輯督導日期及人員</h6>
                        <button id="btnCloseEditModal" type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <h5 class="mt-0 insearch">工程編號：{{ tarItem.EngNo }} &nbsp; 工程名稱：{{ tarItem.EngName }} &nbsp;
                            督導期別：{{ targetPhase.PhaseCode }}</h5>
                        <h5>
                            <div class="form-inline">
                                <label class="my-1 mr-2" for="people_A">列管計畫：</label>
                                <select v-model="tarItem.BelongPrj" class="form-control my-1 mr-sm-2">
                                    <option v-for="option in selCtlPlanOptions" v-bind:value="option.Value"
                                        v-bind:key="option.Text">
                                        {{ option.Text }}
                                    </option>
                                </select>
                            </div>
                        </h5>
                        <h5>
                            <div class="form-inline">
                                <label class="my-1 mr-2" for="selectdate">督導日期：</label>
                                <input v-model="tarItem.SuperviseDateStr" type="date" class="form-control my-1 mr-sm-2"
                                    id="selectdate">
                                <label class="my-1 mr-2" for="selectenddate">~</label>
                                <input v-model="tarItem.SuperviseEndDateStr" type="date" class="form-control my-1 mr-sm-2"
                                    id="selectenddate">
                            </div>
                        </h5>
                        <h5>
                            <div class="form-inline">
                                <label class="my-1 mr-2" for="people_A">督導方式：</label>
                                <select v-model="tarItem.SuperviseMode" class="form-control my-1 mr-sm-2">
                                    <option value="0">工程施工督導</option>
                                    <option value="1">走動式督導</option>
                                    <option value="2">異常工程督導</option>
                                </select>
                            </div>
                        </h5>
                        <h5>
                            <div class="form-inline">
                                <label class="my-1 mr-2" for="people_A">領隊：</label>
                                <select v-model="selPositionItem" @change="onPositionChange"
                                    class="form-control my-1 mr-sm-2">
                                    <option v-for="option in selPositionOptions" v-bind:value="option.Value"
                                        v-bind:key="option.Value">
                                        {{ option.Text }}
                                    </option>
                                </select>
                                <select v-model="selUserItem" class="form-control my-1 mr-sm-2">
                                    <option v-for="option in selUserOptions" v-bind:value="option.Value"
                                        v-bind:key="option.Value">
                                        {{ option.Text }}
                                    </option>
                                </select>
                            </div>
                        </h5>
                        <!-- 督導委員(外聘) -->
                        <h5>
                            <div class="form-inline">
                                <label class="my-1 mr-2" for="people_B">督導委員(外聘)：</label>
                                <input @keyup.enter="getOutCommitte(outCommitteWord)" v-model.trim="outCommitteWord"
                                    type="text" class="form-control my-1 mr-sm-2" id="people_B" placeholder="請輸入姓名搜尋">
                                <!--  label class="my-1 mr-2">宋O永,OOO</label -->
                            </div>
                        </h5>
                        <div class="table-responsive">
                            <table class="table table-responsive-md">
                                <thead class="insearch">
                                    <tr>
                                        <th><strong>姓名</strong></th>
                                        <th><strong>聯絡電話</strong></th>
                                        <th><strong>E-mail</strong></th>
                                        <th class="text-center"><strong>管理</strong></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(item, index) in outCommitte" v-bind:key="item.Seq">
                                        <td>{{ item.CName }}</td>
                                        <td>
                                            {{ item.Tel }}<br>{{ item.Mobile }}
                                        </td>
                                        <td>{{ item.Email }}</td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button @click="addOutCommitte(item)" v-if="item.mode == 1"
                                                    class="btn btn-color11-1 btn-xs sharp mx-1" title="加入"><i
                                                        class="fas fa-plus fa-lg"></i></button>
                                                <button @click="delOutCommitte(item)" v-if="item.mode == 0"
                                                    class="btn btn-color11-2 btn-xs sharp mx-1" title="移除"><i
                                                        class="fas fa-trash-alt"></i></button>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <!-- 督導委員(內聘) -->
                        <h5>
                            <div class="form-inline">
                                <label class="my-1 mr-2" for="people_C">督導委員(內聘)：</label>
                                <input @keyup.enter="getInsideCommitte(insideCommitteWord)"
                                    v-model.trim="insideCommitteWord" type="text" class="form-control my-1 mr-sm-2"
                                    id="people_C" placeholder="請輸入姓名搜尋">
                                <!-- label class="my-1 mr-2">OOO,阿民</label -->
                            </div>
                        </h5>
                        <div class="table-responsive">
                            <table class="table table-responsive-md">
                                <thead class="insearch">
                                    <tr>
                                        <th><strong>姓名</strong></th>
                                        <th><strong>聯絡電話</strong></th>
                                        <th><strong>E-mail</strong></th>
                                        <th class="text-center"><strong>管理</strong></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(item, index) in insideCommitte" v-bind:key="item.Seq">
                                        <td>{{ item.CName }}</td>
                                        <td>
                                            {{ item.Tel }}<br>{{ item.Mobile }}
                                        </td>
                                        <td>{{ item.Email }}</td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button @click="addInsideCommitte(item)" v-if="item.mode == 1"
                                                    class="btn btn-color11-1 btn-xs sharp mx-1" title="加入"><i
                                                        class="fas fa-plus fa-lg"></i></button>
                                                <button @click="delInsideCommitte(item.Seq)" v-if="item.mode == 0"
                                                    class="btn btn-color11-2 btn-xs sharp mx-1" title="移除"><i
                                                        class="fas fa-trash-alt"></i></button>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <!-- 幹事 -->
                        <h5>
                            <div class="form-inline">
                                <label class="my-1 mr-2" for="people_D">幹事：</label>
                                <input @keyup.enter="getOfficerCommitte(officerCommitteWord)"
                                    v-model.trim="officerCommitteWord" type="text" class="form-control my-1 mr-sm-2"
                                    id="people_D" placeholder="請輸入姓名搜尋">
                                <!-- label class="my-1 mr-2">宋O永,OOO</label -->
                            </div>
                        </h5>
                        <div class="table-responsive">
                            <table class="table table-responsive-md">
                                <thead class="insearch">
                                    <tr>
                                        <th><strong>姓名</strong></th>
                                        <th><strong>聯絡電話</strong></th>
                                        <th><strong>E-mail</strong></th>
                                        <th class="text-left"><strong>內聘</strong></th>
                                        <th class="text-center"><strong>管理</strong></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(item, index) in officerCommitte" v-bind:key="item.Seq">
                                        <td>{{ item.CName }}</td>
                                        <td>
                                            {{ item.Tel }}<br>{{ item.Mobile }}
                                        </td>
                                        <td>{{ item.Email }}</td>
                                        <td class="d-flex" >
                                            <div class="form-check" v-if="item.mode == 1 && canAddOfficerCommitte  && item.IsInside != null   ">
                                                <input type="checkbox" :id="`info_${index}`"
                                                    class="form-check-input" v-model="item.IsInside">
                                            </div>
                                            <div v-else>
                                                {{ item.IsInside ? '是' : '否' }}
                                            </div>

                                        </td>
                                        <td>
                                            <div class="d-flex justify-content-center"> 
                                                <button @click="addOfficerCommitte(item)"
                                                    v-if="item.mode == 1 && canAddOfficerCommitte"
                                                    class="btn btn-color11-1 btn-xs sharp mx-1" title="加入"><i
                                                        class="fas fa-plus fa-lg"></i></button>
                                                <button @click="delOfficerCommitte(item)" v-if="item.mode == 0"
                                                    class="btn btn-color11-2 btn-xs sharp mx-1" title="移除"><i
                                                        class="fas fa-trash-alt"></i></button>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <h5>備註：</h5>
                        <textarea v-model.trim="tarItem.Memo" maxlength="500" rows="10" class="form-control"></textarea>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-color3" data-dismiss="modal">關閉</button>
                        <button @click="onSaveEng" type="button" class="btn btn-color11-1">儲存 <i
                                class="fas fa-save"></i></button>
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
            //分頁
            recordTotal: 0,
            pageRecordCount: 30,
            pageTotal: 0,
            pageIndex: 0,
            pageIndexOptions: [],

            yearPhaseItems: [],//年度期別
            keyWord: '',
            targetPhase: null,
            targetPhaseItems: [],

            tarItem: {},//工程
            //督導委員(外聘)
            outCommitteWord: '',
            outCommitte: [],
            //督導委員(內聘)
            insideCommitteWord: '',
            insideCommitte: [],
            //幹事
            canAddOfficerCommitte: true,
            officerCommitteWord: '',
            officerCommitte: [],
            //職稱
            selPositionOptions: [],
            selPositionItem: -1,
            selUserOptions: [],
            selUserItem: -1,
            //水利署列管計畫
            selCtlPlanOptions: [],
        };
    },
    methods: {
        //
        onTabPhaseSelect(item) {
            this.keyWord = item.PhaseCode;
            this.onSearchPhase();
        },
        //水利署列管計畫
        getCtlPlanList() {
            window.myAjax.post('/ESPreWork/GetCtlPlanList')
                .then(resp => {
                    this.selCtlPlanOptions = resp.data.items;
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //當年度期別
        getYearPhases() {
            this.yearPhaseItems = [];
            window.myAjax.post('/ESQCPlaneWeakness/GetPhaseOptions')
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.yearPhaseItems = resp.data.items;
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //職稱清單
        getPositions() {
            window.myAjax.post('/ESPreWork/GetPositions')
                .then(resp => {
                    this.selPositionOptions = resp.data.items;
                })
                .catch(err => {
                    console.log(err);
                });
        },
        onPositionChange() {
            this.selUserItem = -1;
            window.myAjax.post('/ESPreWork/GetPositionsUser', { id: this.selPositionItem })
                .then(resp => {
                    this.selUserOptions = resp.data.items;
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //幹事
        getOfficerCommitte(keyword = this.officerCommitteWord) {
            this.officerCommitte = [];
            this.canAddOfficerCommitte = true;
            window.myAjax.post('/ESPreWork/SearchOfficerCommitte', { id: this.tarItem.Seq, keyword: keyword })
                .then(resp => {
                    this.officerCommitte = resp.data.items;
                    var i;
                    for (i = 0; i < this.officerCommitte.length; i++) {
                        if (this.officerCommitte[i].mode == 0) {
                            // this.canAddOfficerCommitte = false;
                            console.log(this.officerCommitte[i].mode);
                        }
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        addOfficerCommitte(item) {
            window.myAjax.post('/ESPreWork/AddOfficerCommitte', { id: this.tarItem.Seq, committe: item.Seq })
                .then(resp => {
                    if (resp.data.result == 0)
                    {
                        if(item.IsInside) this.addInsideCommitte(item);
                        else this.getOfficerCommitte();
                    }

                    else
                        alert(resp.data.msg);
                })
                .catch(err => {
                    console.log(err);
                });
        },
        delOfficerCommitte(item) {
            window.myAjax.post('/ESPreWork/DelOfficerCommitte', { id: item.Seq })
                .then(resp => {
                    if (resp.data.result == 0)
                    {
                        this.getOfficerCommitte(this.officerCommitteWord);
                        // if(item.IsInside) this.delInsideCommitte(item.IsInside);
                    }
                    else
                        alert(resp.data.msg);
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //督導委員(內聘)
        getInsideCommitte(keyword) {
            this.insideCommitte = [];
            window.myAjax.post('/ESPreWork/SearchInsideCommitte', { id: this.tarItem.Seq, keyword: keyword })
                .then(resp => {
                    this.insideCommitte = resp.data.items;
                    this.getOfficerCommitte();
                })
                .catch(err => {
                    console.log(err);
                });
        },
        addInsideCommitte(item) {
            window.myAjax.post('/ESPreWork/AddInsideCommitte', { id: this.tarItem.Seq, committe: item.Seq })
                .then(resp => {
                    if (resp.data.result == 0)
                    {
                        this.getInsideCommitte(this.insideCommitteWord);
                        this.getOfficerCommitte();
                    }

                    else
                        alert(resp.data.msg);
                })
                .catch(err => {
                    console.log(err);
                });
        },
        delInsideCommitte(seq) {
            window.myAjax.post('/ESPreWork/DelInsideCommitte', { id: seq })
                .then(resp => {
                    if (resp.data.result == 0)
                        this.getInsideCommitte(this.insideCommitteWord);
                    else
                        alert(resp.data.msg);
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //督導委員(外聘)
        getOutCommitte(keyword) {
            this.outCommitte = [];
            window.myAjax.post('/ESPreWork/SearchOutCommitte', { id: this.tarItem.Seq, keyword: keyword })
                .then(resp => {
                    this.outCommitte = resp.data.items;
                })
                .catch(err => {
                    console.log(err);
                });
        },
        addOutCommitte(item) {
            if (item.Id == 'XX') {
                alert('該委員身分證編號有誤，請至後台更新');
                return;
            }

            window.myAjax.post('/ESPreWork/AddOutCommitte', { id: this.tarItem.Seq, committe: item.Seq })
                .then(resp => {
                    if (resp.data.result == 0)
                        this.getOutCommitte(this.outCommitteWord);
                    else
                        alert(resp.data.msg);
                })
                .catch(err => {
                    console.log(err);
                });
        },
        delOutCommitte(item) {
            window.myAjax.post('/ESPreWork/DelOutCommitte', { id: item.Seq })
                .then(resp => {
                    if (resp.data.result == 0)
                        this.getOutCommitte(this.outCommitteWord);
                    else
                        alert(resp.data.msg);
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //編輯工程
        onEditEng(item) {
            this.tarItem = {};
            window.myAjax.post('/ESPreWork/GetEng', { id: item.Seq })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.tarItem = resp.data.item;
                        this.getOutCommitte('');
                        this.getInsideCommitte('');
                        this.getOfficerCommitte('');
                        this.selPositionItem = this.tarItem.PositionSeq;
                        this.onPositionChange();
                        this.selUserItem = this.tarItem.LeaderSeq;
                    } else {
                        alert(resp.data.msg);
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //儲存工程
        onSaveEng() {
            if (this.selUserItem == -1) {
                alert('請選取 領隊');
                return;
            }
            if (this.tarItem.SuperviseMode == null) {
                alert('請選取 督導方式');
                return;
            }
            this.tarItem.LeaderSeq = this.selUserItem;
            this.tarItem.SuperviseDate = this.tarItem.SuperviseDateStr;
            this.tarItem.SuperviseEndDate = this.tarItem.SuperviseEndDateStr;
            window.myAjax.post('/ESPreWork/SaveEng', { m: this.tarItem })
                .then(resp => {
                    if (resp.data.result == 0) {
                        //document.getElementById('btnCloseEditModal').click();
                    }
                    alert(resp.data.msg);
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //期別查詢
        onSearchPhase() {
            if (this.keyWord.length == 0) {
                alert("請輸入監督期別");
                return;
            }
            this.targetPhase = null;
            this.targetPhaseItems = [];
            window.myAjax.post('/ESPreWork/SearchPhase', { keyWord: this.keyWord })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.targetPhase = resp.data.item;
                        this.getPhaseEngItems('');
                        window.sessionStorage.setItem(window.window.esKeyword, this.keyWord);
                    } else {
                        alert(resp.data.msg);
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //取得期別工程清單
        getPhaseEngItems() {
            if (this.targetPhase == null) return;
            if (this.pageIndex < 1) this.pageIndex = 1;
            this.targetPhaseItems = [];
            window.myAjax.post('/ESPreWork/GetPhaseEngList'
                , {
                    id: this.targetPhase.Seq,
                    pageRecordCount: this.pageRecordCount,
                    pageIndex: this.pageIndex
                })
                .then(resp => {
                    this.targetPhaseItems = resp.data.items;
                    this.recordTotal = resp.data.pTotal;
                    this.setPagination();
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //工程狀態
        getStateCss(state) {
            return window.comm.getEngStateCss(state);
        },
        //分頁
        onPageRecordCountChange() {
            this.setPagination();
            this.getList();
        },
        onPageChange() {
            this.getList();
        },
        //計算分頁
        setPagination() {
            this.pageTotal = Math.ceil(this.recordTotal / this.pageRecordCount);
            //
            this.pageIndexOptions = [];
            if (this.pageTotal == 0) {
                this.pageIndex = 0;
            } else {
                for (var i = 1; i <= this.pageTotal; i++) {
                    this.pageIndexOptions.push({ Text: i, Value: i });
                }
                if (this.pageIndex > this.pageIndexOptions.length) {
                    this.pageIndex = this.pageIndexOptions.length;
                }
            }
        },
        getList() {
            this.getPhaseEngItems();
        }
    },
    mounted() {
        console.log('mounted() 前置作業');
        this.getYearPhases();
        this.keyWord = window.sessionStorage.getItem(window.window.esKeyword);
        this.getPositions();
        this.getCtlPlanList();
    }
}
</script>