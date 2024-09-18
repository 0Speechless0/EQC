<template>
    <div>
        <form class="form-group insearch" style="padding-bottom:0px;">
            <div class="form-row">
                <div class="col-12 col-sm-3 mb-3 mb-sm-0">
                    <input v-model.trim="keyWord" type="text" placeholder="期別前五碼年月(YYYMM)" class="form-control">
                </div>
                <div class="d-flex">
                    <button v-on:click.stop="onSearchPhase" type="button" class="btn btn-outline-secondary btn-xs mx-1"><i class="fas fa-search"></i> 查詢</button>
                    <button v-on:click.stop="onNewPhase" type="button" class="btn btn-outline-secondary btn-xs mx-1"><i class="fas fa-plus"></i> 新增</button>
                </div>
            </div>
            <div style="padding-top: 10px">
                <ul class="nav nav-tabs" role="tablist">
                    <li v-for="(item, index) in yearPhaseItems" v-bind:key="item.Seq" class="nav-item">
                        <a v-on:click="onTabPhaseSelect(item)" data-toggle="tab" href="##" class="nav-link">{{item.PhaseCode}}</a>
                    </li>
                </ul>
            </div>
        </form>

        <div v-if="targetPhase!=null" class="card whiteBG mb-4">
            <div class="card-header">
                <div class="form-row" style="width:100%">
                    <div class="col">
                        <h3 class="card-title font-weight-bold">督導期別:{{targetPhase.PhaseCode}}</h3>
                    </div>
                    <div class="col-3">
                        <button v-on:click.stop="onDelPhase" type="button" class="btn btn-color11-1 btn-xs mx-1"><i class="fas fa-trash-alt"></i> 刪除期別</button>
                        <button data-toggle="modal" data-target="#MyDialog" type="button" class="btn btn-color11-1 btn-xs mx-1"><i class="fas fa-plus"></i> 加入工程</button>
                    </div>
                </div>
            </div>
            <div class="card-body">
                <div class="table-responsive">
                    <table class="table table-responsive-md table-hover">
                        <thead>
                            <tr>
                                <th><strong>項次</strong></th>
                                <th><strong>工程資訊</strong></th>
                                <th><strong>督導資訊</strong></th>
                                <th>1</th>
                                <th>2</th>
                                <th>3</th>
                                <th>4</th>
                                <th>5</th>
                                <th>6</th>
                                <th>7</th>
                                <th>8</th>
                                <th>9</th>
                                <th>10</th>
                                <th>11</th>
                                <th>12</th>
                                <th>13</th>
                                <th>14</th>
                                <th><strong>功能</strong></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in targetPhaseItems" v-bind:key="item.Seq">
                                <td><strong>{{index+1}}</strong></td>
                                <td>{{item.EngNo}}<br />{{item.EngName}}<br />{{item.ExecUnit}}</td>
                                <td>{{recDateFmt(item.RecDate)}}</td>
                                <td>{{weakNessText(item.W1)}}</td>
                                <td>{{weakNessText(item.W2)}}</td>
                                <td>{{weakNessText(item.W3)}}</td>
                                <td>{{weakNessText(item.W4)}}</td>
                                <td>{{weakNessText(item.W5)}}</td>
                                <td>{{weakNessText(item.W6)}}</td>
                                <td>{{weakNessText(item.W7)}}</td>
                                <td>{{weakNessText(item.W8)}}</td>
                                <td>{{weakNessText(item.W9)}}</td>
                                <td>{{weakNessText(item.W10)}}</td>
                                <td>{{weakNessText(item.W11)}}</td>
                                <td>{{weakNessText(item.W12)}}</td>
                                <td>{{weakNessText(item.W13)}}</td>
                                <td>{{weakNessText(item.W14)}}</td>
                                <td>
                                    <div class="row justify-content-center m-0">
                                        <button @click="delEng(item)" type="button" class="btn btn-color9-1 btn-xs mx-1"><i class="fas fa-trash-alt"></i></button>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>

        <div class="modal fade" id="MyDialog" ref="MyDialog" >
            <div class="modal-dialog modal-dialog-scrollable modal-dialog-centered modal-xl" role="document">
                <div class="modal-content">
                    <div class="modal-header bg-3 text-white">
                        <h6 class="modal-title font-weight-bold" id="projectUpload">工程查詢</h6>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">×</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="form-group insearch" style="padding:5px">
                            <div class="row">
                                <div class="col-10">
                                    <button v-on:click.stop="onEngNameChange(1)" type="button" class="btn btn-primary btn-xs mx-1">所屬機關</button>
                                    <button v-on:click.stop="onEngNameChange(2)" type="button" class="btn btn-primary btn-xs mx-1">縣市政府</button>
                                    <button v-on:click.stop="onEngNameChange(3)" type="button" class="btn btn-primary btn-xs mx-1">其他補助</button>
                                </div>
                                <div class="col-2">
                                    <button v-on:click.stop="onDownloadClick" type="button" class="btn btn-color11-1 btn-xs mx-1">品質管制弱面檔案</button>
                                </div>
                            </div>
                            <div class="form-row mt-1">
                                <div class="col-5 col-sm-3 mb-3 mb-sm-0">
                                    <select v-model.trim="filterEngUnit" class="form-control">
                                        <option v-for="option in engUnitOptions" v-bind:value="option.Value" v-bind:key="option.Value">{{option.Text}}</option>
                                    </select>
                                </div>
                                <div class="col-3 col-sm-3 mb-3 mb-sm-0">
                                    <input v-model.trim="filterEngName" type="text" placeholder="工程名稱(關鍵字)" class="form-control">
                                </div>
                                <div class="d-flex">
                                    <button @click="onFilterEng" type="button" class="btn btn-outline-secondary btn-xs mx-1"><i class="fas fa-search"></i> 查詢</button>
                                </div>
                            </div>
                        </div>
                        <div class="form-group insearch mb-3">
                            <div class="form-row">
                                說明:1重大(或重點防汛)，2進度落後，3決標比偏低，4施工廠商近年查核成績不佳，5曾發生重大職安事件之標案，6履約計分偏低標案，
                                7近三年曾遭停權之施工廠商，8施工廠商近期承攬能量偏高，9施工廠商跨區域承攬，10施工量能偏低，11委外監造之工程，
                                12成績不佳的委外監造廠商，13高敏感區域工程，14全民督工
                            </div>
                        </div>
                        <div class="row justify-content-between">
                            <comm-pagination class="col-12" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
                        </div>
                        <div class="table-responsive">
                            <table class="table table-responsive-md table-hover">
                                <thead>
                                    <tr>
                                        <th class="sort">序</th>
                                        <th>機關單位</th>
                                        <th>工程資訊</th>
                                        <th>督導資訊</th>
                                        <th>1</th>
                                        <th>2</th>
                                        <th>3</th>
                                        <th>4</th>
                                        <th>5</th>
                                        <th>6</th>
                                        <th>7</th>
                                        <th>8</th>
                                        <th>9</th>
                                        <th>10</th>
                                        <th>11</th>
                                        <th>12</th>
                                        <th>13</th>
                                        <th>14</th>
                                        <th>功能</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(item, index) in items" v-bind:key="index">
                                        <td>{{pageRecordCount*(pageIndex-1)+index+1}}</td>
                                        <th>{{item.ExecUnit}}</th>
                                        <td v-html="showEngName(item)"></td>
                                        <th>{{recDateFmt(item.RecDate)}}</th>
                                        <td>{{weakNessText(item.W1)}}</td>
                                        <td>{{weakNessText(item.W2)}}</td>
                                        <td>{{weakNessText(item.W3)}}</td>
                                        <td>{{weakNessText(item.W4)}}</td>
                                        <td>{{weakNessText(item.W5)}}</td>
                                        <td>{{weakNessText(item.W6)}}</td>
                                        <td>{{weakNessText(item.W7)}}</td>
                                        <td>{{weakNessText(item.W8)}}</td>
                                        <td>{{weakNessText(item.W9)}}</td>
                                        <td>{{weakNessText(item.W10)}}</td>
                                        <td>{{weakNessText(item.W11)}}</td>
                                        <td>{{weakNessText(item.W12)}}</td>
                                        <td>{{weakNessText(item.W13)}}</td>
                                        <td>{{weakNessText(item.W14)}}</td>
                                        <td>
                                            <div class="row justify-content-center m-0">
                                                <button v-show="canAddEng(item)" @click="addEng(item)" type="button" class="btn btn-color11-1 btn-xs mx-1"><i class="fas fa-plus"></i></button>
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
    import {useSelectionStore} from "../../store/SelectionStore";
    const store = useSelectionStore();
    export default {
        data: function () {
            return {
                //使用者單位資訊
                unitList :[],
                userUnit: null,
                userUnitSub: '',
                //分頁
                recordTotal: 0,
                pageRecordCount: 30,
                pageIndex: 0,

                //選項
                tarMode:1,

                //工程
                selectEngName: "",
                selectEngNameItems: [],
                items: [],
                //
                initFlag: 0,

                yearPhaseItems: [],//年度期別
                keyWord: '',
                targetPhase: null,
                targetPhaseItems: [],
                //s20230310
                filterEngName: '',
                filterEngUnit: '',
                engUnitOptions: [],
            };
        },
        methods: {
            //
            onTabPhaseSelect(item) {
                this.keyWord = item.PhaseCode;
                this.onSearchPhase();
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
            //期別查詢
            onSearchPhase() {
                if (this.keyWord.length == 0) {
                    alert("請輸入監督期別");
                    return;
                }
                this.targetPhase = null;
                this.targetPhaseItems = [];
                window.myAjax.post('/ESQCPlaneWeakness/SearchPhase', { keyWord: this.keyWord })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.targetPhase = resp.data.item;
                            this.yearPhaseItems = resp.data.phaseOption;
                            this.getPhaseEngItems();
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //新增期別
            onNewPhase() {
                if (this.keyWord.length < 5) {
                    alert("請輸入監督期別,前五碼為年月(YYYMM)");
                    return;
                }
                this.targetPhase = null;
                this.targetPhaseItems = [];
                window.myAjax.post('/ESQCPlaneWeakness/NewPhase', { keyWord: this.keyWord })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.targetPhase = resp.data.item;
                            this.getYearPhases();
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //刪除期別
            onDelPhase() {
                if (confirm('是否確定刪除目前期別資料？')) {
                    window.myAjax.post('/ESQCPlaneWeakness/DelPhase', { id: this.targetPhase.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.targetPhase = null;
                                this.targetPhaseItems = [];
                                this.getYearPhases();
                            }
                            alert(resp.data.msg);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            //取得期別工程清單
            getPhaseEngItems() {
                if (this.targetPhase == null) return;

                this.targetPhaseItems = [];
                window.myAjax.post('/ESQCPlaneWeakness/GetPhaseEngList', { id: this.targetPhase.Seq })
                    .then(resp => {
                        this.targetPhaseItems = resp.data.items;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //工程加入到期別
            addEng(item) {
                window.myAjax.post('/ESQCPlaneWeakness/AddEng', { phaseSeq: this.targetPhase.Seq, engSeq: item.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getPhaseEngItems();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //從期別移除工程
            delEng(item) {
                window.myAjax.post('/ESQCPlaneWeakness/DelEng', { id: item.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getPhaseEngItems();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //是否需勾稽
            requireLinkEng(item) {
                return this.unitList.find(e => e.Text.indexOf(item.ExecUnit) > -1) ;
            },
            //工程是否勾稽
            showEngName(item) {
                if (item.EngMainSeq == null) {
                    if (this.requireLinkEng(item))
                        return item.EngNo + '<br />' + item.EngName + '<font color="red">(未勾稽)</font>';//<br />' + item.ExecUnit;
                    else
                        return item.EngNo + '<br />' + item.EngName;// + '<br />' + item.ExecUnit;
                } else
                    return item.EngNo + '<br />' + item.EngName + '<font color="blue">(已勾稽)</font>';//<br />' + item.ExecUnit;
            },
            //工程是否可加入期別
            canAddEng(item) {
                // if (this.requireLinkEng(item) ) return false;
                // if (this.requireLinkEng(item) && item.EngMainSeq == null) return false;
                //if (item.Seq == 0) return true;
                var i;
                for (i = 0; i < this.targetPhaseItems.length; i++) {
                    if (item.Seq == this.targetPhaseItems[i].PrjXMLSeq) return false;
                }
                return true;
            },
            //督導資訊格式
            recDateFmt(str) {
                if (str == null || str.length == 0)
                    return '';
                else
                    return str + ' 督導';
            },
            onEngNameChange(mode) {
                this.filterEngName = '';
                this.filterEngUnit = '';
                this.engUnitOptions = [],
                this.pageIndex = 1;
                this.getList(mode);
            },
            onFilterEng() {
                this.pageIndex = 1;
                this.getList(this.tarMode);
            },
            onPageRecordCountChange() {
                this.setPagination();
                this.getList(this.tarMode);
            },
            onPageChange() {
                this.getList(this.tarMode);
            },
            //分頁變更
            onPaginationChange(pInx, pCount) {
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
                this.getList(this.tarMode);
            },
            getList(mode) {
                this.tarMode = mode;
                window.myAjax.post('/ESQCPlaneWeakness/GetEngList'
                    ,{
                        mode:mode,
                        pageRecordCount: this.pageRecordCount,
                        pageIndex: this.pageIndex,
                        fName: this.filterEngName,
                        fUnit: this.filterEngUnit
                    })
                    .then(resp => {
                        this.items = resp.data.items;
                        this.recordTotal = resp.data.pTotal;
                        if (this.items.length > 0 && this.engUnitOptions.length == 0) {
                            this.getEngUnitOptions();
                        }
                        //this.setPagination();
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //工程單位清單 s20230310
            getEngUnitOptions() {
                this.engUnitOptions = [];
                window.myAjax.post('/ESQCPlaneWeakness/GetEngUnitList', { mode: this.tarMode })
                    .then(resp => {
                        this.engUnitOptions = resp.data.items;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            weakNessText(v) {
                if (v > 0)
                    return "V";
                else
                    return "";
            },
            onDownloadClick() {
                window.comm.dnFile('/ESQCPlaneWeakness/dnQCPlaneWeakness?mode=' + this.tarMode);
            }
        },
        async mounted() {
            console.log('mounted() 品質管制弱面篩選');
            this.unitList = await store.GetSelection("Unit/GetUnitList", "Unit") ;
            this.unitList  = this.unitList .filter(e => e.Text.indexOf("清單") == -1);
            this.getYearPhases();
        }
    }
</script>