<template>
    <div>
        <!-- 各單位在建工程件數 -->
        <div class="card whiteBG mb-4 pattern-F">
            <div class="card-header ">
                <h3 class="card-title font-weight-bold">各單位在建工程件數</h3>
            </div>
            <div class="card-body">
                <div class="row">
                    <div v-for="(item, index) in engCntForUnit" v-bind:key="item.ExecUnitName" class="col-12 col-sm-6 col-md-3 mb-4">
                        <div class="card btn-color2">
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-12">
                                        <p class="card-text">{{item.ExecUnitName}}</p>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-4">
                                        <br />
                                        <a v-on:click="onEngCntForUnitClick(1, item)" href="" data-toggle="modal" data-target="#case_01">
                                            <p class="card-text text-2">{{item.behindCount}}</p>
                                        </a>
                                    </div>
                                    <div class="col-8">
                                        <a v-on:click="onEngCntForUnitClick(2, item)" href="" data-toggle="modal" data-target="#case_01">
                                            <h4 class="fa-3x text-right text-white">{{item.constructionCount}}</h4>
                                        </a>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- 小視窗 案件清單/進度落後案件清單 -->
        <div class="modal fade" id="case_01">
            <div class="modal-dialog modal-xl modal-dialog-centered ">
                <div class="modal-content">
                    <div class="modal-header bg-0 text-white">
                        <h6 class="modal-title font-weight-bold">{{engCntForUnitModelHeader}}</h6>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <h5 class="mt-0">{{engCntForUnitMode}}</h5>
                        <div class="table-responsive">
                            <table class="table table-responsive-md">
                                <thead>
                                    <tr>
                                        <th><strong>排序</strong></th>
                                        <th><strong>工程編號</strong></th>
                                        <th><strong>工程名稱</strong></th>
                                        <th><strong>執行機關</strong></th>
                                        <th><strong>合約金額</strong></th>
                                        <th><strong>品管費用</strong></th>
                                        <th><strong>開工日期</strong></th>
                                        <th><strong>預定完工日期</strong></th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(item, index) in tarEngForUnitItems" v-bind:key="item.Seq">
                                        <td><strong>{{index+1}}</strong></td>
                                        <td>{{item.TenderNo}}</td>
                                        <td>{{item.TenderName}}</td>
                                        <td>{{item.ExecUnitName}}</td>
                                        <td>{{item.BidAmount}}</td>
                                        <td>{{item.QualityControlFee}}</td>
                                        <td>{{chsDateFormat(item.ActualStartDate)}}</td>
                                        <td>{{chsDateFormat(item.ScheCompletionDate)}}</td>
                                        <td>
                                            <div class="d-flex">
                                                <button v-on:click.stop="viewTender(item)" class="btn btn-color11-2 btn-xs sharp mr-1"><i class="fas fa-eye"></i></button>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-color3" data-dismiss="modal">關閉</button>
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
                targetMode: null,
                //各單位在建工程件數
                engCntForUnit: [],
                tarEngForUnitItems: [],
                engCntForUnitModelHeader: '',
                engCntForUnitMode: '',
            };
        },
        methods: {
            //各單位在建工程件數
            getEngCntForUnit() {
                window.myAjax.post('/EngAnalysisDecision/GetEngCntForGovUnitList', { m: this.targetMode })
                    .then(resp => {
                        this.engCntForUnit = resp.data.items;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onEngCntForUnitClick(mode, item) {
                this.tarEngForUnitItems = [];
                if (mode == 1) {
                    this.engCntForUnitModelHeader = '進度落後案件清單';
                    this.engCntForUnitMode = item.ExecUnitName + '之進度落後案件清單'
                } else {
                    this.engCntForUnitModelHeader = '案件清單';
                    this.engCntForUnitMode = item.ExecUnitName + '之案件清單'
                }
                window.myAjax.post('/EngAnalysisDecision/GetEngListForGovUnit', { m: this.targetMode, mode: mode, unit: item.ExecUnitName })
                    .then(resp => {
                        this.tarEngForUnitItems = resp.data.items;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            chsDateFormat(d) {
                if (d == null || d.length != 7) return d;
                return d.substring(0, 3) + '.' + d.substring(3, 5) + '.' + d.substring(5, 7);
            },
            strEmpty(str) {
                return window.comm.stringEmpty(str);
            },
            viewTender(item) {
                window.sessionStorage.setItem(window.epcSelectTrenderSeq, item.Seq);
                window.myAjax.post('/EPCTender/EditTender', { seq: item.Seq, tarEdit: "Edit9" })
                    .then(resp => {
                        window.location.href = resp.data.Url;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //
            init() {
                this.getEngCntForUnit();
            }
        },
        mounted() {
            console.log('mounted() 單位在建工程件數');
            let urlParams = new URLSearchParams(window.location.search);
            if (urlParams.has('m')) {
                this.targetMode = parseInt(urlParams.get('m'), 10);
                console.log(this.targetId);
                if (Number.isInteger(this.targetMode) && this.targetMode > 0) {
                    this.init();
                    return;
                }
            }
        }
    }
</script>
