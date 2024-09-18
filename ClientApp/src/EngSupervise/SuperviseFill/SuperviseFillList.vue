<template>
    <div>
        <form class="form-group insearch" style="padding-bottom:0px;">
            <div class="form-row">
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                    <input v-model.trim="keyWord" type="text" placeholder="請輸入督導期別" class="form-control">
                </div>
                <div class="d-flex">
                    <button v-on:click.stop="onSearchPhase" type="button" class="btn btn-outline-secondary btn-xs mx-1"><i class="fas fa-search"></i> 查詢</button>
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
        <div class="row justify-content-between">
            <div class="form-inline col-12 col-md-8 small">
                <label class="my-1 mr-2">
                    共
                    <span class="text-danger">{{recordTotal}}</span>
                    筆，每頁顯示
                </label>
                <select v-model="pageRecordCount" @change="onPageRecordCountChange($event)" class="form-control sort form-control-sm">
                    <option value="30">30</option>
                    <option value="50">50</option>
                    <option value="100">100</option>
                </select>
                <label class="my-1 mx-2">筆，共<span class="text-danger">{{pageTotal}}</span>頁，目前顯示第</label>
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
                        <th><strong>監造單位</strong></th>
                        <th style="width: 95px;"><strong>狀態</strong></th>
                        <th style="width: 42px;"><strong>功能</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in targetPhaseItems" v-bind:key="item.Seq">
                        <td><strong>{{pageRecordCount*(pageIndex-1)+index+1}}</strong></td>
                        <td>{{item.EngNo}}</td>
                        <td>{{item.EngName}}</td>
                        <td>{{item.ExecUnit}}</td>
                        <td>{{item.DesignUnitName}}</td>
                        <td>{{item.SupervisionUnitName}}</td>
                        <td><i v-bind:class="getStateCss(item.ExecState)"></i>{{item.ExecState}}</td>
                        <td>
                            <div class="d-flex">
                                <button @click="onEditEng(item)" class="btn btn-color11-1 btn-xs sharp mr-1" data-toggle="modal" data-target="#schedule_01" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                <button @click="onDownloadClick(item)" class="btn btn-color11-3 btn-xs sharp mx-1" title="下載"> <i class="fas fa-download"></i>
                                </button>
                            </div>
                        </td>
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
            };
        },
        methods: {
            onDownloadClick(item) {
                window.comm.dnFile('/ESSuperviseFill/dnSRSheet?id=' + item.Seq);
            },
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
            //編輯工程
            onEditEng(item) {
                window.myAjax.post('/ESSuperviseFill/EditEng', { seq: item.Seq })
                    .then(resp => {
                        window.sessionStorage.setItem(window.eqSelTrenderPlanSeq, item.Seq);
                        window.location.href = resp.data.Url;
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
                window.myAjax.post('/ESSuperviseFill/SearchPhase', { keyWord: this.keyWord })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.targetPhase = resp.data.item;
                            this.yearPhaseItems = resp.data.phaseOption;
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
                window.myAjax.post('/ESSuperviseFill/GetPhaseEngList'
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
            console.log('mounted() 督導填報');
            this.getYearPhases();
            this.keyWord = window.sessionStorage.getItem(window.window.esKeyword);
        }
    }
</script>