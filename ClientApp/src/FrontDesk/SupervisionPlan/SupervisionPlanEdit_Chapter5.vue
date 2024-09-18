<template>
    <div>
        <div class="row justify-content-between align-items-end">
            <div class="col">
                <h2>料設備送審管制標準</h2>
            </div>
            <div class="col">
                ***下方材料設備名稱如需修改，請至材料設備送審管制總表編修
            </div>

        </div>
        <comm-pagination :recordTotal="totalRows" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        <div class="table-responsive tableFixHead">
            <table class="table table1 min910" border="0">
                <thead>
                    <tr>
                        <th class="sort">
                            <div v-if="fCanEdit" class="custom-control custom-checkbox">
                                <input @change="onCheckAllChange($event)" type="checkbox" class="custom-control-input" id="StdCheck_All" value="true">
                                <label class="custom-control-label" for="StdCheck_All">All</label>
                            </div>
                        </th>
                        <th class="sort">項次</th>
                        <th>種契約詳細表項次</th>
                        <th>材料/設備名稱</th>
                        <th>建立日期</th>
                        <th>更新日期</th>
                        <!-- th>流程圖</th -->
                        <th>抽查管理標準</th>
                        <th>抽查紀錄表</th>
                    </tr>
                </thead>

                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq" v-bind:class="{'bg-gray':item.DataType>0}">
                        <td class="text-center">
                            <div class="custom-control custom-checkbox">
                                <input :disabled="!fCanEdit" v-model="item.DataKeep" @change="onInputChange(item)" type="checkbox" class="custom-control-input" v-bind:id="'StdCheck'+index">
                                <label class="custom-control-label" v-bind:for="'StdCheck'+index"></label>
                            </div>
                        </td>
                        <td>
                            {{item.OrderNo}}
                        </td>
                        <td>
                            {{item.ItemNo}}
                        </td>
                        <td>
                            {{item.MDName}}
                        </td>
                        <td>
                            {{item.createDate}}
                        </td>
                        <td>
                            {{item.modifyDate}}
                        </td>
                        <!-- td class="text-center">
                        <a v-on:click="openFlowChartModal(item)" data-toggle="modal" data-target="#flowChartModal" href="#" class="a-blue" title="流程圖">
                            <i class="fas fa-project-diagram"></i>
                        </a><span v-if="item.FlowCharOriginFileName==null || item.FlowCharOriginFileName==''">無</span>
                    </td -->
                        <td class="text-center">
                            <a v-on:click="openQCStdModal(item)" data-toggle="modal" data-target="#qcStdModal" href="#" class="a-blue" title="抽查管理標準">
                                <i class="fas fa-file-alt fa-2x"></i>
                            </a>
                            <!--
                        <a v-on:click.stop="editQC(item)" href="#" class="a-blue" title="抽查管理標準">
                            <i class="fas fa-file-alt fa-2x"></i>
                        </a>-->
                            <span v-if="item.stdCount==0">無</span>
                        </td>
                        <td class="text-center">
                            <a v-on:click.stop="onDownloadCheckSheet(item)" href="#" class="a-blue" title="抽查紀錄表">
                                <i class="fas fa-file-alt fa-2x"></i>
                            </a><span v-if="item.stdCount==0">無</span>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <comm-pagination :recordTotal="totalRows" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        <!-- div style="width:99%;" class="row justify-content-center">
            <b-pagination :total-rows="totalRows"
                          :per-page="perPage"
                          v-model="pageIndex">
            </b-pagination>
        </div -->
        <!-- 流程圖 -->
        <flowChartmodal ref="flowChartModal"></flowChartmodal>
        <qcstdmodal v-on:onCloseEvent="closeQCStdModal"
                    v-bind:modalId="'qcStdModal'" v-bind:engMain="engMain" v-bind:targetItem="targetQCStdItem" v-bind:chapter="targetChapter"></qcstdmodal>
        <FlowChartTpDiagramEdit :title="editDiagramItem.MDName" :editFlowChartTp="editDiagramItem.Seq"
                                :modalShow="modalShow" @handleModalShow="modalShow = !modalShow" type="Chapter5" :engSeq="engMain.Seq"
                                @download="onDownloadFlowChartEvent"
                                @upload="flowChartStatusChange"
                                route="SupervisionPlan"> </FlowChartTpDiagramEdit>
    </div>
</template>
<script>
    export default {
        props: ['engMain'],
        watch: {
            pageIndex: {
                handler: function (value) {
                    this.getList();
                }
            }
        },
        components: {
            flowChartmodal: require('./FlowChartModal.vue').default,
            qcstdmodal: require('./QCStdModal.vue').default,
            FlowChartTpDiagramEdit : require("../../Backend/FlowChart/FlowChartTpDiagramEdit.vue").default
        },
        data: function () {
            return {
                fCanEdit: false, 
                items: [],

                targetItem: { ItemName: '', FlowCharOriginFileName: '' },
                targetQCStdItem: { ItemName: '', FlowCharOriginFileName: '' },
                targetChapter: '',
                //分頁
                pageIndex: 1,
                perPage: 30,
                totalRows: 0,
                modalShow : false,
                editDiagramItem :{}
                
            };
        },
        methods: {
            flowChartStatusChange(Seq) {
                this.editDiagramItem.FlowCharOriginFileName = true;

            },
            onCheckAllChange(event) {
                for (let item of this.items) {
                    item.DataKeep = event.target.checked;
                }
                this.onKeepAllChange();
            },
            //抽查紀錄表
            onDownloadCheckSheet(item) {
                this.downloadFile('/SupervisionPlan/Chapter5DownloadCheckSheet?engMain=' + this.engMain.Seq + '&seq=' + item.Seq);
            },
            downloadFile(tarUrl) {
                console.log('tarUrl: ' + tarUrl);
                window.myAjax.get(tarUrl, { responseType: 'blob' })
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
            //清單
            getList() {
                if (this.engMain.DocState == -1)
                    this.fCanEdit = true;
                else
                    this.fCanEdit = false;
                this.editFlag = false;
                this.items = [];
                let params = { engMain: this.engMain.Seq, pageIndex: this.pageIndex, perPage: this.perPage };
                window.myAjax.post('/SupervisionPlan/Chapter5', params)
                    .then(resp => {
                        this.items = resp.data.items;
                        console.log("items", this.items);
                        this.totalRows = resp.data.pTotal;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onPaginationChange(pInx, pCnt) {//s20230818
                this.pageIndex = pInx;
                this.perPage = pCnt;
                this.getList();
            },
            isItemEmpty(item) {
                /*if (window.comm.stringEmpty(item.MDName)) {
                    alert('項目須輸入資料');
                    return true;
                } else {
                    return false;
                }*/
            },
            //項目名稱, Keep儲存
            onInputChange(item) {

                if (this.isItemEmpty(item)) {
                    return;
                }
                window.myAjax.post('/SupervisionPlan/Chapter5Save', { item: item })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            //const resultItem = resp.data.item.Data;
                            //item.modifyDate = resultItem.modifyDate;
                        } else
                        {
                            item.DataKeep = resp.data.DataKeep;
                            // alert(resp.data.message);
                        }
    
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            // check All
            onKeepAllChange() {
                /*for (let item of this.items) {
                    if (this.isItemEmpty(item)) {
                        return;
                    }
                }*/
                window.myAjax.post('/SupervisionPlan/Chapter5SaveKeep', { items: this.items })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getList();
                        }
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //抽查管理標準
            openQCStdModal(item) {
                this.targetQCStdItem = item;
                this.targetQCStdItem.ItemName = item.MDName;
                this.targetChapter = '5';
            },
            closeQCStdModal() {
                this.targetChapter = '';
            },
            //流程圖上傳
            openFlowChartModal(item) {
                this.editDiagramItem = item;
                this.modalShow = true;
            },
            closeFlowChartModal() {
            },
            // onUploadFlowChartEvent: function (formData) {
            //     formData.append("engMain", this.engMain.Seq);
            //     formData.append("seq", this.targetItem.Seq);
            //     window.myAjax.post('/SupervisionPlan/Chapter5Upload', formData,
            //         {
            //             headers: {
            //                 'Content-Type': 'multipart/form-data'
            //             }
            //         }).then(resp => {
            //             if (resp.data.result == 0) {
            //                 const item = resp.data.item.Data;
            //                 this.targetItem.FlowCharOriginFileName = item.FlowCharOriginFileName;
            //                 this.targetItem.modifyDate = item.modifyDate;
            //                 this.$refs.flowChartModal.uploadFinish();
            //             }
            //             alert(resp.data.message);
            //         }).catch(error => {
            //             console.log(error);
            //         });
            // },
            onDownloadFlowChartEvent(Seq) {
                this.$refs.flowChartModal.downloadFile('/SupervisionPlan/Chapter5DownloadFlowChart?engMain=' + this.engMain.Seq + '&seq=' + Seq);
            },
            onShowFlowChartEvent() {
                this.$refs.flowChartModal.showFile('/SupervisionPlan/Chapter5ShowFlowChart');
            },
            onDelFlowChartEvent() {
                window.myAjax.post('/SupervisionPlan/Chapter5DelFlowChart', { engMain: this.engMain.Seq, seq: this.targetItem.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.targetItem.FlowCharOriginFileName = '';
                        }
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            /*editFlowChart(item) {
                window.open('/SupervisionPlan/FlowChartEdit?chapter=5&engMain=' + this.engMain.Seq + '&seq=' + item.Seq);
            },
            editQC(item) {
                //window.location = '/SupervisionPlan/QCEdit?chapter=5&engMain=' + this.engMain.Seq + '&seq=' + item.Seq;
                window.open('/SupervisionPlan/QCEdit?chapter=5&engMain=' + this.engMain.Seq + '&seq=' + item.Seq);
            }*/
        },
        async mounted() {
            console.log('mounted() 第五章 材料設備送審管制標準');
            this.getList();
        }
    }
</script>
<style scoped>
.tableFixHead          { overflow: auto; max-height: 500px;   }
table {
    border-collapse: separate;
    border-spacing: 0;
}
.table {
    margin : 0;
}
.tableFixHead thead  { position: sticky !important ; top: 0 !important ; z-index: 1 !important;     }
th {
    border : 0;
    border-bottom: #ddd solid 1px !important; 
    border-left : 0 !important;
    border-right:0 !important;
}
td {
    z-index: 0;
    position: relative;
}
</style>