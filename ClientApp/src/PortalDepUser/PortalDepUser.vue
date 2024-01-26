<template>
    <div>
        <div class="card whiteBG mb-4 pattern-F">
            <div class="card-body pb-0">
                <div class="row">
                    <div v-for="(item, index) in engStateSta" v-bind:key="item.mode" class="col-12 col-sm-6 col-md-3 mb-4">
                        <div class="card" v-bind:class="getLevelCss(item.mode)">
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-6">
                                        <a v-on:click="onEngStateClick(item.mode)" href="" data-toggle="modal" data-target="#case_01">
                                            <span class="fa-2x text-white">{{item.engCount}}</span>
                                        </a>
                                        <p class="card-text mt-10">{{item.level}}</p>
                                    </div>
                                    <div class="col-6 m-auto">
                                        <h4 class="fa-3x text-right"><i v-bind:class="getLevelIcon(item.mode)"></i></h4>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- 待處理事項 -->
        <div class="card whiteBG mb-4">
            <div class="card-header">
                <h3 class="card-title font-weight-bold">待處理事項</h3>
            </div>
            <div class="card-body">
                <div class="basic-list-group">
                    <ul class="list-group list-group-flush">
                        <li v-for="(item, index) in importantEventSta" v-bind:key="item.mode" class="list-group-item">
                            {{item.level}} <a v-on:click="onImportantEventClick(item.mode, item.level)" href="" data-toggle="modal" data-target="#case_01">
                            <span class="text-danger">{{item.engCount}}件</span>
                            <span v-if="item.fillTag" style="color:blue">
                                (請至工程會標案管理系統填報)
                            </span>
                        </a>
                        </li>
                    </ul>
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
                                        <th v-if="Role == 1">排除/列入</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(item, index) in tarEngForUnitItemsView" v-bind:key="item.TenderNo">
                                        <td><strong>{{index+1}}</strong></td>
                                        <td>{{item.TenderNo}}</td>
                                        <td>{{item.TenderName}}</td>
                                        <td>{{item.ExecUnitName}}</td>
                                        <td>{{item.BidAmount}}</td>
                                        <td>{{item.QualityControlFee}}</td>
                                        <td>{{chsDateFormat(item.ActualStartDate)}}</td>
                                        <td>{{chsDateFormat(item.ScheCompletionDate)}}</td>
                                        <td>
                                            <div v-if="item.Seq > 0" class="d-flex">
                                                <button v-on:click.stop="viewTender(item)" class="btn btn-color11-2 btn-xs sharp mr-1"><i class="fas fa-eye"></i></button>
                                            </div>
                                        </td>
                                        <td v-if="Role == 1">
                                            <div class="d-flex">
                                                <button v-if="!item.exclude" v-on:click.stop="excludeTender(item)" class="btn btn-color11-4 btn-xs sharp mr-1"><i class="fas fa-times"></i></button>
                                                <button v-else v-on:click.stop="includeTender(item)" class="btn btn-color11-1 btn-xs sharp mr-1"><i class="fas fa-check"></i></button>
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

        <!-- 案件進度清單 -->
        <div class="card whiteBG mb-4">
            <div class="card-header ">
                <h3 class="card-title font-weight-bold">案件清單</h3>
            </div>
            <div class="card-body">
                <div class="form-row">
                    <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                        <select v-model="selectState" class="form-control">
                            <option v-for="option in selectStateOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                {{ option.Text }}
                            </option>
                        </select>
                    </div>
                    <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                        <input v-model.trim="engKeyword" type="text" placeholder="工程名稱關鍵字" class="form-control">
                    </div>
                    <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                        <button @click="getList" type="button" class="btn btn-outline-secondary btn-xs mx-1" data-dismiss="modal">查詢 <i class="fas fa-search"></i></button>
                    </div>
                </div>
                <div class="row justify-content-between">
                    <comm-pagination class="col-12" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
                </div>
                <div class="table-responsive">
                    <table class="table table-responsive-md">
                        <thead>
                            <tr>
                                <th><strong>排序</strong></th>
                                <th><strong>工程編號</strong></th>
                                <th><strong>工程名稱</strong></th>
                                <th style="width: 105px;"><strong>執行機關</strong></th>
                                <th style="width: 105px;"><strong>設計單位</strong></th>
                                <th><strong>監造單位</strong></th>
                                <th style="width: 95px;"><strong>狀態</strong></th>
                                <th><strong>功能</strong></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in engItems" v-bind:key="item.Seq">
                                <td><strong>{{pageRecordCount*(pageIndex-1)+index+1}}</strong></td>
                                <td>{{item.TenderNo}}</td>
                                <td>{{item.TenderName}}</td>
                                <td>{{item.ExecUnitName}}</td>
                                <td>{{item.DesignUnitName}}</td>
                                <td>{{item.SupervisionUnitName}}</td>
                                <td><i v-bind:class="getStateCss(item.ExecState)"></i>{{item.ExecState}}</td>
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
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                //工程統計
                engStateSta: [],
                tarEngForUnitItems: [],
                engCntForUnitModelHeader: '',
                engCntForUnitMode: '',
                //分頁
                engItems: [],
                recordTotal: 0,
                pageIndex: 1,
                pageRecordCount: 30,
                //filter eng
                selectStateOptions: [],
                selectState: '',
                engKeyword: '',
                //待處理事項
                importantEventSta:[],
            };
        },
        computed: {
            tarEngForUnitItemsView()
            {
                if(this.Role == 1)
                {
                    return this.tarEngForUnitItems;
                }
                else
                {
                    return this.tarEngForUnitItems.filter(e => !e.exclude );
                }
            }
        },
        methods: {
            async excludeTender(item)
            {   
                let {data} = await window.myAjax.post("EngAnalysisDecision/setImportListTag", {  
                    tag: {
                    prjXMLSeq : item.Seq,
                    ExcludeTag : true,
                    Mode :3

                }});
                console.log(data) ;
                if(data) 
                {
                    alert("已排除");
                    item.exclude = true;
                    // this.onImportantEventClick(3)
                    
                }
        
                
            },
            async includeTender(item)
            {   
                let {data} = await window.myAjax.post("EngAnalysisDecision/setImportListTag", { 
                    tag: {
                        prjXMLSeq : item.Seq,
                        ExcludeTag : false,
                        Mode :3

                    }

                });
                if(data) 
                {
                    alert("已列入");
                    item.exclude = false;
                    // this.onImportantEventClick(3)
                }
        
                
            },
            //待處理事項
            getImportantEventSta() {
                window.myAjax.post('/PortalDepUser/GetImportantEventSta')
                    .then(resp => {
                        this.importantEventSta = resp.data.items;
                        this.importantEventSta = resp.data.items;
                        [0, 1, 2, 3, 4 ,5].forEach(e => {
                            this.importantEventSta[e].fillTag = true;

                        })
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },

            onImportantEventClick(mode, level) {
                this.tarEngForUnitItems = [];
                this.engCntForUnitModelHeader = '案件清單';
                this.engCntForUnitMode = level ? level+ '之案件清單' : this.engCntForUnitMode;
                window.myAjax.post('/PortalDepUser/GetImportantEventList', { mode: mode })
                    .then(resp => {
                        this.tarEngForUnitItems = resp.data.items;
                        this.Role = resp.data.roleSeq;
                    })
            },
            //工程狀態統計
            getLevelIcon(mode) {
                if (mode == 1)
                    return 'fas fa-users';
                else if (mode == 2)
                    return 'fas fa-toolbox';
                else if (mode == 3)
                    return 'fas fa-tools';
                else
                    return 'fas fa-clipboard-check'
            },
            getLevelCaption(mode) {
                if (mode == 1)
                    return '發包前';
                else if (mode == 2)
                    return '施工前';
                else if (mode == 3)
                    return '施工中';
                else
                    return '結案'
            },
            getLevelCss(mode) {
                if (mode == 1)
                    return 'btn-color11-1';
                else if (mode == 2)
                    return 'btn-color11-2';
                else if (mode == 3)
                    return 'btn-color11-3';
                else
                    return 'btn-color11-4'
            },
            onEngStateClick(mode) {
                this.engCntForUnitModelHeader = '案件清單';
                this.engCntForUnitMode = this.getLevelCaption(mode);
                window.myAjax.post('/PortalDepUser/GetEngStateList', { mode: mode})
                .then(resp => {
                    this.tarEngForUnitItems = resp.data.items;
                })
                .catch(err => {
                    console.log(err);
                });
            },
            getEngStateSta() {
                window.myAjax.post('/PortalDepUser/GetEngStateSta')
                    .then(resp => {
                        this.engStateSta = resp.data.items;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },

            chsDateFormat(d) {
                if (d == null || d.length != 7) return d;
                return d.substring(0, 3) + '.' + d.substring(3, 5) + '.' + d.substring(5, 7);
            },
            //施工中清單
            getListState() {
                this.engItems = [];
                window.myAjax.post('/PortalDepUser/GetStateList')
                    .then(resp => {
                        this.selectStateOptions = resp.data.items;
                        this.getList();
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            getList() {
                this.engItems = [];
                window.myAjax.post('/PortalDepUser/GetList', {
                    pageRecordCount: this.pageRecordCount,
                    pageIndex: this.pageIndex,
                    state: this.selectState,
                    engKeyword: this.engKeyword
                })
                .then(resp => {
                    this.engItems = resp.data.items;
                    this.recordTotal = resp.data.pTotal;
                })
                .catch(err => {
                    console.log(err);
                });
            },
            getStateCss(state) {
                return window.comm.getEngStateCss(state);
            },
            onPaginationChange(pInx, pCount) {
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
                this.getList();
            },
            viewTender(item) {
                window.sessionStorage.setItem(window.epcSelectTrenderSeq, item.Seq);
                window.myAjax.post('/EPCTender/EditTender', { seq: item.Seq, tarEdit: "Edit3" })
                    .then(resp => {
                        window.location.href = resp.data.Url;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //
            init() {
                this.getEngStateSta();
                this.getImportantEventSta();
                this.getListState();

            }
        },
        mounted() {
            console.log('mounted() 局資訊儀表板');
            this.init();
        }
    }
</script>
