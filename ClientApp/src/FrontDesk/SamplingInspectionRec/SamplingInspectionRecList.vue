<template>
    <div>
        <div class="table-responsive">
            <h5>施工抽查</h5>
            <SITable :tenderItem="tenderItem" :mode="1" :items="ConstCheckItems" v-on:editSIR="editSIR"></SITable>
            <h5>設備運轉測試</h5>
            <SITable :tenderItem="tenderItem" :mode="2" :items="EquOperTestItems" v-on:editSIR="editSIR"></SITable>
            <h5>職業安全衛生</h5>
            <SITable :tenderItem="tenderItem" :mode="3" :items="OccuSafeHealthItems" v-on:editSIR="editSIR"></SITable>
            <h5>環境保育清單</h5>
            <SITable :tenderItem="tenderItem" :mode="4" :items="EnvirConsItems" v-on:editSIR="editSIR"></SITable>
            <!-- 舊格式 -->
            <!-- table class="table table-responsive-md table-hover">
                <thead class="insearch">
                    <tr>
                        <th class="text-left"><strong>分項工程名稱</strong></th>
                        <th><strong>抽查紀錄數</strong></th>
                        <th><strong>缺失個數</strong></th>
                        <th class="text-center"><strong>管理</strong></th>
                        <th><strong>施工抽查紀錄表</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.subEngNameSeq">
                        <td class="text-left">{{item.subEngNameSeq}},{{showSubEngName(item)}}</td>
                        <td>{{item.constCheckRecCount}}</td>
                        <td>{{item.missingCount}}</td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <a v-on:click.stop="editEng(item)" href="#" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i></a>
                            </div>
                        </td>
                        <td>
                            <a v-on:click="getdownloaditem(item,1)" href="#" class="a-view m-1" title="下載" data-toggle="modal" data-target="#wordModal">doc下載</a><br>
                            <a v-on:click="getdownloaditem(item,2)" href="#" class="a-view m-1" title="下載" data-toggle="modal" data-target="#wordModal">pdf下載</a><br>
                            <a v-on:click="getdownloaditem(item,3)" href="#" class="a-view m-1" title="下載" data-toggle="modal" data-target="#wordModal">odt下載</a>
                        </td>
                    </tr>
                </tbody>
            </table -->
        </div>
        <div class="row justify-content-center m-0">
            <!-- 大圖 -->
            <div class="modal fade" id="wordModal" data-backdrop="static" data-keyboard="false" tabindex="-1" aria-labelledby="exampleModalLabel" aria-modal="true" aria-hidden="true">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="projectUpload">下載</h5>
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
                                                <a v-on:click.stop="downloadone(item,downloaditem)" href="#" class="btn-block mx-2 btn btn-color2" title="下載">下載</a>
                                            </div>
                                        </td>

                                    </tr>
                                </tbody>
                            </table>
                            <div class="row justify-content-center m-0">
                                <a v-on:click.stop="download(item)" href="#" class="btn-block mx-2 btn btn-color2" title="全部下載">全部下載</a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>
</template>
<script>
    export default {
        props: ['tenderItem'],
        components: {
            SITable: require('./SamplingInspectionRec_table.vue').default,
        },
        data: function () {
            return {
                //items: [],
                //
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
                window.myAjax.post('/SamplingInspectionRec/GetList', { engMain: this.tenderItem.Seq })
                    .then(resp => {
                        //this.items = resp.data.items; //舊清單格式
                        this.ConstCheckItems = resp.data.cc; //施工抽查清單
                        this.EquOperTestItems = resp.data.eot; //設備運轉測試清單
                        this.OccuSafeHealthItems = resp.data.osh; //職業安全衛生清單
                        this.EnvirConsItems = resp.data.ec;//環境保育清單
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            download(item) {
                window.myAjax.get('/SamplingInspectionRec/SIRDownload?seq=' + this.downloaditem.subEngNameSeq + '&filetype=' + this.downloadtype, { responseType: 'blob' })
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
            downloadone(item, downloaditem) {
                console.log(item);
                console.log(downloaditem);
                var num = this.downloaditems.length;
                window.myAjax.get('/SamplingInspectionRec/SIROneDownload?seq=' + this.downloaditem.subEngNameSeq + '&items=' + this.downloaditems + '&downloaditem=' + downloaditem + '&num=' + num + '&filetype='+this.downloadtype , { responseType: 'blob' })
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
            //s20230520
            editSIR(mode, item) {
                this.$emit('editSIR', mode, item);//item.subEngNameSeq);
            },
            editEng(item) {
                console.log(item.subEngNameSeq);
                this.$emit('editSIR', item.subEngNameSeq, null);
            },
            showSubEngName(item) {
                if (item.subEngNo) {
                    return item.subEngNo + ':' + item.subEngName;
                } else {
                    return item.subEngName;
                }
            },
            getdownloaditem(item,type) {
                this.downloaditem = item;
                this.downloadtype = type;
                window.myAjax.post('/SamplingInspectionRec/GetSIRlist', { seq: item.subEngNameSeq })
                    .then(resp => {
                        this.downloaditems = resp.data;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            }
        },
        async mounted() {
            console.log('mounted() 抽驗紀錄填報');
            if (this.tenderItem != null) this.getList();
        }
    }
</script>
