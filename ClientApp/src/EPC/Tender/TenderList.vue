<template>
    <div class="card-body">
        <form class="form-group insearch mb-3">
            <div class="form-row">
                <div class ="d-flex">
                    <label for="selectYearOption" class="mr-2">發包年度</label>
                    <div>
                        <select id="selectYearOption" class="form-control"  @change="onYearChange($event)" v-model="selectYear">
                            <option v-for="option in selectYearOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                    {{ option.Text }}
                                </option>
                        </select>
                    </div>

                </div>
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                    <select v-model="selectUnit" @change="onUnitChange(selectUnit)" class="form-control">
                        <option v-for="option in selectUnitOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                    <input v-model="keyWord" type="text" placeholder="標案名稱關鍵字" class="form-control">
                </div>
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                    <button v-on:click.stop="onSearch()" type="button" class="btn btn-outline-secondary btn-xs mx-1" data-dismiss="modal">查詢 <i class="fas fa-search"></i></button>
                </div>

            </div>
        </form>
        <div class="row justify-content-between">
            <comm-pagination class="col-12" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        </div>
        <div class="table-responsive">
            <table class="table table-responsive-md table-hover">
                <thead>
                    <tr>
                        <th style="width: 42px;"><strong>項次</strong></th>
                        <th><strong>工程編號</strong></th>
                        <th><strong>工程名稱</strong></th>
                        <th style="width: 120px;"><strong>執行機關</strong></th>
                        <!--
                        <th style="width: 120px;"><strong>設計單位</strong></th>
                        <th><strong>監造單位</strong></th> -->
                        <th style="width: 95px;"><strong>狀態</strong></th>
                        <th><strong>功能</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq" v-bind:class="{'btn-color11-1':item==selectItem}">
                        <td><strong>{{pageRecordCount*(pageIndex-1)+index+1}}</strong></td>
                        <td>{{item.EngNo}}</td>
                        <td>{{item.EngName}}</td>
                        <td>{{item.ExecUnit}}</td>
                        <!--
                        <td>{{item.DesignUnitName}}</td>
                        <td>{{item.SupervisorUnitName}}</td> -->
                        <td><i v-bind:class="getStateCss(item.ExecState)"></i>{{item.ExecState}}</td>
                        <td>
                            <div class="d-flex">
                                <button v-on:click.stop="editTender(item)" v-bind:disabled="item.PrjXMLSeq == null || item.PrjXMLSeq<1" class="btn btn-color11-3 btn-xs mr-1"><i class="fas fa-eye"> 查看標管資料</i></button>
                                <button v-on:click.stop="editTenderPlan(item)" class="btn btn-color11-3 btn-xs mr-1"><i class="fas fa-pencil-alt"> 管考</i></button>
                                <button  v-bind:disabled="item.PrjXMLSeq == null || item.PrjXMLSeq<1" @click="dnDoc2(item)" class="btn btn-color11-1 btn-xs mx-1">
                                    <i class="fas fa-download"></i> 工程資料表
                                </button>
                                <PackageDownload  btnclass="btn btn-color11-3 btn-xs mr-1" :systemType="3" :seq="item.Seq"></PackageDownload>
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
        components :{
            PackageDownload :require("../../FilePackage/Modal.vue").default
        },
        data: function () {
            return {
                //使用者單位資訊
                userUnit: null,
                userUnitName: null,
                keyWord:'',
                initFlag: 0,
                //分頁
                recordTotal: 0,
                pageRecordCount: 30,
                pageIndex: 0,
                //選項
                selectYear: '',
                selectYearOptions: [],
                //機關
                selectUnit: '',
                selectUnitOptions: [],

                items: [],
                selectItem: {},
                tarEdit: "",
            };
        },
        methods: {
            //s20230818
            dnDoc2(item) {
                window.comm.dnFile('/EPCQualityVerify/DnDoc2?id=' + item.PrjXMLSeq);
            },
            getStateCss(state) {
                return window.comm.getEngStateCss(state);
            },
            //取得使用者單位資訊
            getUserUnit() {
                window.myAjax.post('/EPCTender/GetUserUnit')
                    .then(resp => {
                        this.userUnit = resp.data.unit;
                        this.userUnitName = resp.data.unitName;
                        //
                        if (sessionStorage.getItem('selectUnit') == null) {
                            this.selectUnit = this.userUnit;
                            this.onUnitChange(this.selectUnit);
                            this.onSearch();
                        } else {
                            this.selectUnit = sessionStorage.getItem('selectUnit');
                            this.initFlag = 2;
                            this.onUnitChange(this.selectUnit);
                            this.onSearch();
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //工程年分
            async getSelectYearOption() {
                const { data } = await window.myAjax.post('/EPCTender/GetYearOptions');
                this.selectYearOptions = data;
                if (this.selectYearOptions.length > 0) {
                    this.selectYear = this.selectYearOptions[0].Value;
                    this.onYearChange();
                }
            },
            async onYearChange(event) {
                this.items = [];
                //工程機關
                this.selectUnit = '';
                this.selectUnitOptions = [];
                const { data } = await window.myAjax.post('/EPCTender/GetUnitOptions', { year: this.selectYear });
                this.selectUnitOptions = data;
                if (this.userUnit == null) this.getUserUnit();
                //儲存到session
                //sessionStorage.removeItem('selectYear');
                //window.sessionStorage.setItem("selectYear", this.selectYear);
            },
            onUnitChange(unitSeq) {
                if (this.selectUnitOptions.length == 0) return;
                this.items = [];
                //this.pageIndex = 1;
                //this.getList();
                //儲存到session
                /*sessionStorage.removeItem('selectUnit');
                window.sessionStorage.setItem("selectUnit", this.selectUnit);*/

            },
            onSearch() {
                if (this.selectUnitOptions.length == 0) return;
                this.pageIndex = 1;
                this.getList();
                //儲存到session
                /*sessionStorage.removeItem('selectUnit');
                window.sessionStorage.setItem("selectUnit", this.selectUnit);*/

            },
            //計算分頁
            onPaginationChange(pInx, pCount) {
                //console.log("pInx:" + this.$refs['pagination'].pageIndex + " pCount:" + pCount);
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
                this.getList();
            },
            getList() {
                if (this.selectYear == '' || this.selectUnit == '') return;

                this.items = [];
                window.myAjax.post('/EPCTender/GetList'
                    , {
                        year: this.selectYear,
                        unit: this.selectUnit,
                        keyWord: this.keyWord,
                        pageRecordCount: this.pageRecordCount,
                        pageIndex: this.pageIndex
                    })
                    .then(resp => {
                        this.items = resp.data.items;
                        //if (!this.engNameFlag) {
                        //    this.selectSubEngNameItems = resp.data.engNameItems;
                        //}
                        this.recordTotal = resp.data.pTotal;
                        //this.setPagination();
                        //this.engNameFlag = false;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //編輯標案
            editTender(item) {
                window.sessionStorage.setItem(window.epcSelectTrenderSeq, item.Seq);
                window.myAjax.post('/EPCTender/EditTender', { seq: item.PrjXMLSeq, tarEdit: this.tarEdit})
                    .then(resp => {
                        window.location.href = resp.data.Url;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //編輯工程
            editTenderPlan(item) {
                window.sessionStorage.setItem(window.epcSelectTrenderSeq, item.Seq);
                window.myAjax.post('/EPCTender/EditTenderPlan', { seq: item.Seq, tarEdit: this.tarEdit })
                    .then(resp => {
                        window.location.href = resp.data.Url;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        async mounted() {
            let pathSplitation = window.location.pathname.split('/');
            let index = pathSplitation[pathSplitation.length -1 ];
            switch(index) {
                case 'Index9' : this.tarEdit ='Edit9'; break;
                case 'Index3' : this.tarEdit ='Edit3'; break;
            }
            console.log('mounted() 工程標案清單 ' + this.tarEdit);
            if (this.selectYearOptions.length == 0) {
                this.getSelectYearOption();
            }
            
            //s20230517
            if (document.getElementById('leftMenuBody').offsetLeft >= 0) {
                setTimeout(function () {
                    document.getElementById('leftMenuTab').click();
                }, 150);
            }
            document.getElementById('leftMenuBody').style.display = "none"; //s20230524
        }
    }
</script>
