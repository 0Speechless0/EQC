<template>
    <div>
        <div class="table-responsive">
            <template v-if="ConstCheckItems.length>0">
                <h5>施工抽查</h5>
                <SITable :tenderItem="tenderItem" :mode="1" :items="ConstCheckItems" v-on:editSIRImprove="editSIRImprove"></SITable>
            </template>
            <template v-if="EquOperTestItems.length>0">
                <h5>設備運轉測試</h5>
                <SITable :tenderItem="tenderItem" :mode="2" :items="EquOperTestItems" v-on:editSIRImprove="editSIRImprove"></SITable>
            </template>
            <template v-if="OccuSafeHealthItems.length>0">
                <h5>職業安全衛生</h5>
                <SITable :tenderItem="tenderItem" :mode="3" :items="OccuSafeHealthItems" v-on:editSIRImprove="editSIRImprove"></SITable>
            </template>
            <template v-if="EnvirConsItems.length>0">
                <h5>環境保育清單</h5>
                <SITable :tenderItem="tenderItem" :mode="4" :items="EnvirConsItems" v-on:editSIRImprove="editSIRImprove"></SITable>
            </template>
            <!--
            <h5> 舊格式 分項工程清單</h5>
            <table class="table table-responsive-md table-hover">
                <thead class="insearch">
                    <tr>
                        <th class="text-left"><strong>分項工程名稱</strong></th>
                        <th><strong>缺失個數</strong></th>
                        <th class="text-center"><strong>編輯</strong></th>
                        <th><strong>不符合事項報告</strong></th>
                        <th><strong>NCR程序追蹤改善表</strong></th>
                        <th><strong>改善照片檔</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.subEngNameSeq">
                        <td class="text-left">{{item.subEngNameSeq}},{{item.subEngName}}</td>
                        <td>{{item.missingCount}}</td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <a v-on:click.stop="editEng(item)" href="#" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i></a>
                            </div>
                        </td>
                        <td>
                            <a v-on:click="getdownloaditem(item,44,1)" href="#" class="a-view m-1" title="下載" data-toggle="modal" data-target="#wordModal">doc下載</a><br />
                            <a v-on:click="getdownloaditem(item,44,2)" href="#" class="a-view m-1" title="下載" data-toggle="modal" data-target="#wordModal">pdf下載</a><br />
                            <a v-on:click="getdownloaditem(item,44,3)" href="#" class="a-view m-1" title="下載" data-toggle="modal" data-target="#wordModal">odt下載</a>
                            <div class="row justify-content-center m-0">
                                
                                <div class="modal fade" id="wordModal" data-backdrop="static" data-keyboard="false" tabindex="-1" aria-labelledby="exampleModalLabel" aria-modal="true" aria-hidden="true">
                                    <div class="modal-dialog modal-lg">
                                        <div class="modal-content">
                                            <div class="modal-header">
                                                <h5 class="modal-title" id="projectUpload">不符合事項報告下載</h5>
                                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                                    <span aria-hidden="true">×</span>
                                                </button>
                                            </div>
                                            <div class="modal-body">
                                                <table class="table table1" border="0">
                                                    <thead>
                                                        <tr>
                                                            <th class="sort">檔名</th>
                                                            <th class="number">功能</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr v-for="(downloaditem, index) in downloaditems" v-bind:key="downloaditem">
                                                            <td>{{downloaditem}}</td>
                                                            <td>
                                                                <div class="row justify-content-center m-0">
                                                                    <a v-on:click.stop="downloadone(item,downloaditem,44)" href="#" class="btn-block mx-2 btn btn-color2" title="下載">下載</a>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                                <div class="row justify-content-center m-0">
                                                    <a v-on:click.stop="download(item,44)" href="#" class="btn-block mx-2 btn btn-color2" title="全部下載">全部下載</a>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                        <td>
                            <a v-on:click="getdownloaditem(item,45,1)" href="#" class="a-view m-1" title="下載" data-toggle="modal" data-target="#wordModal2">doc下載</a><br />
                            <a v-on:click="getdownloaditem(item,45,2)" href="#" class="a-view m-1" title="下載" data-toggle="modal" data-target="#wordModal2">pdf下載</a><br />
                            <a v-on:click="getdownloaditem(item,45,3)" href="#" class="a-view m-1" title="下載" data-toggle="modal" data-target="#wordModal2">odt下載</a>
                            <div class="row justify-content-center m-0">
                                
                                <div class="modal fade" id="wordModal2" data-backdrop="static" data-keyboard="false" tabindex="-1" aria-labelledby="exampleModalLabel" aria-modal="true" aria-hidden="true">
                                    <div class="modal-dialog modal-lg">
                                        <div class="modal-content">
                                            <div class="modal-header">
                                                <h5 class="modal-title" id="projectUpload">NCR程序追蹤改善表下載</h5>
                                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                                    <span aria-hidden="true">×</span>
                                                </button>
                                            </div>
                                            <div class="modal-body">
                                                <table class="table table1" border="0">
                                                    <thead>
                                                        <tr>
                                                            <th class="sort">檔名</th>
                                                            <th class="number">功能</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr v-for="(downloaditem, index) in downloaditems" v-bind:key="downloaditem">
                                                            <td>{{downloaditem}}</td>
                                                            <td>
                                                                <div class="row justify-content-center m-0">
                                                                    <a v-on:click.stop="downloadone(item,downloaditem,45)" href="#" class="btn-block mx-2 btn btn-color2" title="下載">下載</a>
                                                                </div>
                                                            </td>

                                                        </tr>
                                                    </tbody>
                                                </table>
                                                <div class="row justify-content-center m-0">
                                                    <a v-on:click.stop="download(item,45)" href="#" class="btn-block mx-2 btn btn-color2" title="全部下載">全部下載</a>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                        <td>
                            <a v-on:click="getdownloaditem(item,46,1)" href="#" class="a-view m-1" title="下載" data-toggle="modal" data-target="#wordModal3">doc下載</a><br />
                            <a v-on:click="getdownloaditem(item,46,2)" href="#" class="a-view m-1" title="下載" data-toggle="modal" data-target="#wordModal3">pdf下載</a><br />
                            <a v-on:click="getdownloaditem(item,46,3)" href="#" class="a-view m-1" title="下載" data-toggle="modal" data-target="#wordModal3">odt下載</a>
                            <div class="row justify-content-center m-0">
                                
                                <div class="modal fade" id="wordModal3" data-backdrop="static" data-keyboard="false" tabindex="-1" aria-labelledby="exampleModalLabel" aria-modal="true" aria-hidden="true">
                                    <div class="modal-dialog modal-lg">
                                        <div class="modal-content">
                                            <div class="modal-header">
                                                <h5 class="modal-title" id="projectUpload">改善照片下載</h5>
                                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                                    <span aria-hidden="true">×</span>
                                                </button>
                                            </div>
                                            <div class="modal-body">
                                                <table class="table table1" border="0">
                                                    <thead>
                                                        <tr>
                                                            <th class="sort">檔名</th>
                                                            <th class="number">功能</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr v-for="(downloaditem, index) in downloaditems" v-bind:key="downloaditem">
                                                            <td>{{downloaditem}}</td>
                                                            <td>
                                                                <div class="row justify-content-center m-0">
                                                                    <a v-on:click.stop="downloadone(item,downloaditem,46)" href="#" class="btn-block mx-2 btn btn-color2" title="下載">下載</a>
                                                                </div>
                                                            </td>

                                                        </tr>
                                                    </tbody>
                                                </table>
                                                <div class="row justify-content-center m-0">
                                                    <a v-on:click.stop="download(item,46)" href="#" class="btn-block mx-2 btn btn-color2" title="全部下載">全部下載</a>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
            -->
        </div>
     </div>
</template>
<script>
    export default {
        props: ['tenderItem'],
        components: {
            SITable: require('./SamplingInspectionRecImprove_table.vue').default,
        },
        data: function () {
            return {
                items: [],
                //
                editFlag: false,
                //下載項目(type:1(docx),2(pdf),3(odt))
                downloaditems: [],
                downloaditem: null,
                downloadtype: 1,
                //s20230520
                ConstCheckItems: [], //施工抽查清單
                EquOperTestItems: [], //設備運轉測試清單
                OccuSafeHealthItems: [], //職業安全衛生清單
                EnvirConsItems: [], //環境保育清單
            };
        },
        methods: {
            getList() {
                window.myAjax.post('/SamplingInspectionRecImprove/GetList', { engMain: this.tenderItem.Seq })
                    .then(resp => {
                        //this.items = resp.data.items;
                        this.ConstCheckItems = resp.data.cc; //施工抽查清單
                        this.EquOperTestItems = resp.data.eot; //設備運轉測試清單
                        this.OccuSafeHealthItems = resp.data.osh; //職業安全衛生清單
                        this.EnvirConsItems = resp.data.ec;//環境保育清單
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            download(item, mode) {
                window.comm.dnFile('/SamplingInspectionRecImprove/SIRDownload?seq=' + this.downloaditem.subEngNameSeq + '&mode=' + mode + '&filetype=' + this.downloadtype);
            },
            downloadone(item,downloaditem,mode) {
                var num = this.downloaditems.length;
                window.comm.dnFile('/SamplingInspectionRecImprove/SIROneDownload?seq=' + this.downloaditem.subEngNameSeq + '&items=' + this.downloaditems + '&downloaditem=' + downloaditem + '&num=' + num + '&filetype=' + this.downloadtype + '&mode=' + mode);
            },
            //s20230520
            editSIRImprove(mode, item) {
                this.$emit('editSIRImprove', mode, item);
            },
            editEng(item) {
                this.$emit('editSIRImprove', item.subEngNameSeq);
                /*window.myAjax.post('/SamplingInspectionRecImprove/EditEng', { seq: item.subEngNameSeq })
                    .then(resp => {
                        console.log(resp.data.Url);
                        window.location.href = resp.data.Url;
                    })
                    .catch(err => {
                        console.log(err);
                    });*/
            },
            getdownloaditem(item, mode, type) {
                this.downloaditem = item;
                this.downloadtype = type;
                window.myAjax.post('/SamplingInspectionRecImprove/GetSIRlist', { seq: item.subEngNameSeq, mode:mode })
                    .then(resp => {
                        this.downloaditems = resp.data;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            }
        },
        async mounted() {
            console.log('mounted() 抽驗缺失改善');
            if (this.tenderItem != null) {
                this.getList();
            }
        }
    }
</script>
