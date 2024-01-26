<template>
    <div>
        <form class="form-group insearch mb-3">
            <div class="form-row">
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                    <div class="form-inline">
                        <label for="keyword" class="mr-md-2">廠商名稱關鍵字</label>
                        <input v-model.trim="keyword" type="text" id="keyword" class="form-control ml-md-2" >
                    </div>
                </div>
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                    <button @click="onSearch" type="button" class="btn btn-outline-secondary btn-sm"><i class="fas fa-search"></i> 開始查詢</button>
                </div>
            </div>
        </form>
        
        <h5>{{manufacturer}} 履歷資料</h5>
        <div class="table-responsive">
            <table class="table table-responsive-md">
                <thead class="insearch">
                    <tr>
                        <th><strong>年度</strong></th>
                        <th><strong>執行機關</strong></th>
                        <th><strong>工程名稱</strong></th>
                        <th><strong>經費</strong></th>
                        <th><strong>施工地點</strong></th>
                        <th><strong>完工日期</strong></th>
                        <th><strong>實際驗收完成日</strong></th>
                        <th><strong>履約總分</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in engItems" v-bind:key="item.Seq">
                        <td>{{item.TenderYear}}</td>
                        <td>{{item.ExecUnitName}}</td>
                        <td>{{item.TenderName}}</td>
                        <td>{{item.BidAmount}}</td>
                        <td>{{item.Location}}</td>
                        <td>{{item.ActualPerformDesignDate}}</td>
                        <td>{{item.ActualAacceptanceCompletionDate}}</td>
                        <td>{{item.PSTotalScore}}</td>
                    </tr>
                </tbody>
            </table>
        </div>
        <p class="text-right">合計{{engItems.length}}件，{{totalBidAmount}}千元，履約平均：{{avgScore}}分</p>
        <!--
        <h5>履約計分 <small>最近五年廠商履約情形計分(依發文日期排序)</small></h5>
        <p class="mb-0">{{manufacturer}}</p>
        <div class="table-responsive">
            <table class="table table-responsive-md">
                <thead class="insearch">
                    <tr>
                        <th><strong></strong></th>
                        <th><strong>執行機關</strong></th>
                        <th><strong>工程名稱</strong></th>
                        <th><strong>決標金額</strong></th>
                        <th><strong>實際驗收完成日期</strong></th>
                        <th><strong>總分</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in engScoreItems" v-bind:key="item.Seq">
                        <td>{{index+1}}</td>
                        <td>{{item.ExecUnitName}}</td>
                        <td>{{item.TenderName}}</td>
                        <td>{{item.BidAmount}}</td>
                        <td>{{item.ActualAacceptanceCompletionDate}}</td>
                        <td>{{item.PSTotalScore}}</td>
                    </tr>
                </tbody>
            </table>
        </div>
        <p class="text-right">平均：{{avgScore}}分&nbsp;</p>
        -->
        <h5>重大事故 <small>重大事故資料</small></h5>
        <p class="mb-0">{{manufacturer}}</p>
        <div class="table-responsive">
            <table class="table table-responsive-md">
                <thead class="insearch">
                    <tr>
                        <th><strong></strong></th>
                        <th><strong>執行機關</strong></th>
                        <th><strong>工程名稱</strong></th>
                        <th><strong>死亡</strong></th>
                        <th><strong>重傷</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in engSafetyItems" v-bind:key="item.Seq">
                        <td>{{index+1}}</td>
                        <td>{{item.ExecUnitName}}</td>
                        <td>{{item.TenderName}}</td>
                        <td>{{item.WSDeadCnt}}</td>
                        <td>{{item.WSHurtCnt}}</td>
                    </tr>
                </tbody>
            </table>
        </div>
        <h5>司法院裁判書 [CourtVerdict]<small></small></h5>
        <p class="mb-0">{{manufacturer}}</p>
        <div class="table-responsive">
            <table class="table table-responsive-md">
                <thead class="insearch">
                    <tr>
                        <th><strong></strong></th>
                        <th><strong>執行機關</strong></th>
                        <th><strong>工程名稱</strong></th>
                        <th><strong>件數</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in engVerdictItems" v-bind:key="item.Seq">
                        <td>{{index+1}}</td>
                        <td>{{item.ExecUnitName}}</td>
                        <td>{{item.TenderName}}</td>
                        <td>?</td>
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
                keyword: '',
                manufacturer: '',
                engItems: [],
                engScoreItems: [],
                engSafetyItems: [],
                engVerdictItems: [],
            };
        },
        methods: {
            //廠商
            onSearch() {
                if (this.keyword.length < 1) {
                    alert('關鍵字輸入長度不足');
                    return
                }
                window.myAjax.post('/EADManufacturer/Search', { key: this.keyword } )
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.manufacturer = resp.data.m;
                            this.getEngList();
                        } else {
                            alert(resp.data.msg);
                            return
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //履歷資料
            getEngList() {
                this.engItems = [];
                window.myAjax.post('/EADManufacturer/GetEngList', { m: this.manufacturer })
                    .then(resp => {
                        this.engItems = resp.data.items;
                        //this.getEngScoreList();
                        this.getEngSafetyList();
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            /*//履約計分
            getEngScoreList() {
                this.engScoreItemss = [];
                window.myAjax.post('/EADManufacturer/GetEngScoreList', { m: this.manufacturer })
                    .then(resp => {
                        this.engScoreItems = resp.data.items;
                        this.getEngSafetyList();
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },*/
            //重大事故
            getEngSafetyList() {
                this.engSafetyItems = [];
                window.myAjax.post('/EADManufacturer/GetEngSafetyList', { m: this.manufacturer })
                    .then(resp => {
                        this.engSafetyItems = resp.data.items;
                        //this.getEngVerdictList();
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //司法院裁判書
            getEngVerdictList() {
                this.engVerdictItems = [];
                window.myAjax.post('/EADManufacturer/GetEngVerdictList', { m: this.manufacturer })
                    .then(resp => {
                        this.engVerdictItems = resp.data.items;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        computed: {
            //履歷資料 總金額
            totalBidAmount: function () {
                var i;
                var total = 0;
                for (i = 0; i < this.engItems.length; i++) {
                    let item = this.engItems[i];
                    if (item.BidAmount != null) {
                        total = total + item.BidAmount;
                    }
                }
                
                return total;
            },
            //履約計分 平均
            avgScore: function () {
                var i;
                var total = 0;
                var cnt = 0;
                for (i = 0; i < this.engItems.length; i++) {
                    let item = this.engItems[i];
                    if (item.PSTotalScore != null) {
                        total = total + item.PSTotalScore;
                        cnt++;
                    }
                }
                if (cnt == 0) return 0;
                return Math.round(total*100/cnt)/100;
            }
        },
        mounted() {
            console.log('mounted() 廠商履歷評估分析');
        }
    }
</script>
