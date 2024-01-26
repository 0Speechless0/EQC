<template>
    <div>
        <h5 class="insearch mt-0 py-2">材料設備送審</h5>
        <comm-pagination ref="pagination" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        <div class="table-responsive mb-3" style="min-height:320px; padding: 10px;">
            <table class="table table1 onepage my-0" border="0">
                <thead>
                    <tr>
                        <th>項次</th>
                        <th class="text-left">契約詳細表項次<br>材料/設備名稱</th>
                        <th>契約數量</th>
                        <th class="text-right">取樣次數</th>
                        <th class="text-right">送審次數</th>
                        <th class="text-right">驗廠次數</th>
                        <th>協力廠商資料</th>
                        <th>型錄</th>
                        <th>試驗報告</th>
                        <th>審查結果</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="index" @click="onSelectEngClick(item)" v-bind:class="{'bg-1-30':(item.EngMaterialDeviceListSeq==selEngSeq), '':(item.EngMaterialDeviceListSeq!=selEngSeq)}">
                        <td>{{pageRecordCount*(pageIndex-1)+index+1}}</td>
                        <td>{{item.ItemNo}}<br />{{item.MDName}}</td>
                        <td class="text-right">{{item.ContactQty}} {{item.ContactUnit}}
                        </td>
                        <td class="text-right">{{item.IsSampleTestCnt}}</td>
                        <td class="text-right">{{item.IsFactoryInspCnt}}</td>
                        <td class="text-right">{{item.AuditDateCnt}}</td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <button @click="onViewVendorClick(item)" data-toggle="modal" data-target="#refVendorModal"
                                        class="btn-block mx-2 btn btn-color11-3 btn-xs" style="width: 10px;" title="檢視">
                                    <i class="fas fa-eye"></i>
                                </button>
                            </div>
                        </td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <button @click="onViewCatalogClick(item)" data-toggle="modal" data-target="#refCatalogModal"
                                        class="btn-block mx-2 btn btn-color11-3 btn-xs" style="width: 10px;" title="檢視">
                                    <i class="fas fa-eye"></i>
                                </button>
                            </div>
                        </td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <button @click="onViewTestReportClick(item)" data-toggle="modal" data-target="#refCatalogModal"
                                        class="btn-block mx-2 btn btn-color11-3 btn-xs" style="width: 10px;" title="檢視">
                                    <i class="fas fa-eye"></i>
                                </button>
                            </div>
                        </td>
                        <td class="text-center">{{item.AuditResultCnt}}次通過，{{item.AuditDateCnt - item.AuditResultCnt}}次否決</td>
                    </tr>
                </tbody>
            </table>
        </div>
        <!-- 型錄 -->
        <div class="modal fade" id="refCatalogModal" aria-labelledby="refCatalogModal" data-backdrop="static" data-keyboard="false" tabindex="-1" aria-modal="true">
            <div class="modal-dialog modal-lx modal-dialog-centered ">
                <div class="modal-content">
                    <div class="modal-header bg-0 text-white">
                        <h6 class="modal-title" id="projectUpload">{{modelType==1 ? '型錄' : '相關試驗報告'}}</h6>
                    </div>
                    <div class="modal-body">
                        <table border="0" class="table table1 my-0">
                            <thead>
                                <tr>
                                    <th>項次</th>
                                    <th class="text-left">檔案</th>
                                    <th class="text-left">上傳日期</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(item, index) in catalogItems" v-bind:key="item.Seq">
                                    <td>{{index+1}}</td>
                                    <td><a v-on:click.stop="dnCatalog(item)" href="#" class="a-blue underl l mx-2">{{item.OriginFileName}}</a></td>
                                    <td>{{item.CreateTimeStr}}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal" aria-label="Close">
                            關閉
                        </button>
                    </div>
                </div>
            </div>
        </div>
        <!-- 協力廠商資料 -->
        <div class="modal fade" id="refVendorModal" aria-labelledby="refVendorModal" data-backdrop="static" data-keyboard="false" tabindex="-1" aria-modal="true">
            <div class="modal-dialog modal-lx modal-dialog-centered ">
                <div class="modal-content">
                    <div class="modal-header bg-0 text-white">
                        <h6 class="modal-title" id="projectUpload">協力廠商資料</h6>
                    </div>
                    <div class="modal-body">
                        <table class="table table1 onepage my-0" border="0">
                            <thead>
                                <tr>
                                    <th class="text-left">廠商名稱</th>
                                    <th class="text-left">廠商統編</th>
                                    <th class="text-left">廠商地址</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(item, index) in vendorItems" v-bind:key="index">
                                    <td class="text-left">{{item.VendorName}}</td>
                                    <td>{{item.VendorTaxId}}</td>
                                    <td>{{item.VendorAddr}}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal" aria-label="Close">
                            關閉
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <TestControlList v-if="selectEng != null" :engMainSeq="engMainSeq" :emdSeq="selectEng.EngMaterialDeviceListSeq"></TestControlList>
    </div>
</template>
<script>
    export default {
        props: ['engMainSeq'],
        components: {
            TestControlList: require('./TabEng_TestControlList.vue').default,
        },
        watch: {
            engMainSeq: function(val){
                if (this.engMainSeq > -1) this.getList();
            }
        },
        data: function () {
            return {
                items: [],
                selectEng: null,
                selEngSeq: -1,
                //分頁
                recordTotal: 0,
                pageRecordCount: 30,
                pageIndex: 1,

                vendorItems: [],
                catalogItems: [],
                modelType: -1,
            };
        },
        methods: {
            //選取項次
            onSelectEngClick(item) {
                this.selectEng = item;
                this.selEngSeq = this.selectEng.EngMaterialDeviceListSeq;
            },
            //相關試驗報告
            onViewTestReportClick(item) {
                this.modelType = 2;
                this.catalogItems = [];
                window.myAjax.post('/EngHistoryInfo/GetEMDTestReportList', { id: item.EngMaterialDeviceListSeq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.catalogItems = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //型錄
            onViewCatalogClick(item) {
                this.modelType = 1;
                this.catalogItems = [];
                window.myAjax.post('/EngHistoryInfo/GetEMDCatalogList', { id: item.EngMaterialDeviceListSeq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.catalogItems = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            dnCatalog(item) {
                if (this.modelType == 1)
                    window.comm.dnFile('/EMDAudit/ResumeFileDownload?seq=' + item.Seq);
                else
                    window.comm.dnFile('/EMDAudit/AuditFileDownload?seq=' + item.Seq);
            },
            //協力廠商資料
            onViewVendorClick(item) {
                this.vendorItems = [];
                window.myAjax.post('/EngHistoryInfo/GetEMDVendorList', { id: item.EngMaterialDeviceListSeq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.vendorItems = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //
            getList() {
                this.selectEng = null;
                this.selEngSeq = -1;
                this.items = [];
                window.myAjax.post('/EngHistoryInfo/GetEquipmentReviewList', {
                    id: this.engMainSeq,
                    pageRecordCount: this.pageRecordCount,
                    pageIndex: this.pageIndex
                })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                            this.recordTotal = resp.data.total;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //分頁
            onPaginationChange(pInx, pCount) {
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
                this.getList();
            },           
        },
        async mounted() {
            console.log('mounted 負責案件清單');
            if (this.engMainSeq > -1) this.getList();
        }
    }
</script>