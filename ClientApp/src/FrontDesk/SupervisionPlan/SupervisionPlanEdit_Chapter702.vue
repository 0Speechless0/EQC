<template>
    <div>
        <div class="row justify-content-between">
            <div class="col">
                <h2>環境保育標準</h2>
            </div>
            <div v-if="fCanEdit" class="col-12 col-md-5 col-xl-3 mt-3">
                <button v-on:click.stop="newItem()" class="btn btn-shadow btn-outline-secondary btn-block">
                    <i class="fas fa-plus"></i>&nbsp;&nbsp;新增環境保育標準
                </button>
            </div>
        </div>
        <comm-pagination :recordTotal="totalRows" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        <div class="table-responsive tableFixHead  ">
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
                        <th>環境保育清單</th>
                        <th>建立日期</th>
                        <th>更新日期</th>
                        <th>流程圖</th>
                        <th>抽查管理標準</th>
                        <th>抽查紀錄表</th>
                        <th v-if="fCanEdit"></th>
                    </tr>
                </thead>

                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.id" v-bind:class="{'bg-gray':item.DataType>0}">
                        <td class="text-center">
                            <div class="custom-control custom-checkbox">
                                <input :disabled="!fCanEdit" v-model="item.DataKeep" @change="onInputChange(item)" type="checkbox" class="custom-control-input" v-bind:id="'StdCheck'+index">
                                <label class="custom-control-label" v-bind:for="'StdCheck'+index"></label>
                            </div>
                        </td>
                        <td>
                            <input :disabled="!fCanEdit" v-model.number="item.OrderNo" @change="onInputChange(item)" type="text" class="form-control" />
                        </td>
                        <td>
                            <input :disabled="!fCanEdit" v-model.trim="item.ItemName" @change="onInputChange(item)" maxlength="100" type="text" class="form-control" />
                        </td>
                        <td>
                            {{item.createDate}}
                        </td>
                        <td>
                            {{item.modifyDate}}
                        </td>
                        <td class="text-center">
                            <a v-on:click="openFlowChartModal(item)" data-toggle="modal" data-target="#flowChartModal" href="#" class="a-blue" title="流程圖">
                                <i class="fas fa-project-diagram"></i>
                            </a><span v-if="item.FlowCharOriginFileName==null || item.FlowCharOriginFileName==''">無</span>
                        </td>
                        <td class="text-center">
                            <a v-on:click="openQCStdModal(item)" data-toggle="modal" data-target="#qcStdModal" href="#" class="a-blue" title="抽查管理標準">
                                <i class="fas fa-file-alt fa-2x"></i>
                            </a><span v-if="item.stdCount==0">無</span>
                        </td>
                        <td class="text-center">
                            <a v-on:click.stop="onDownloadCheckSheet(item)" href="#" class="a-blue" title="抽查紀錄表">
                                <i class="fas fa-file-alt fa-2x"></i>
                            </a><span v-if="item.stdCount==0">無</span>
                        </td>
                        <td v-if="fCanEdit">
                            <div v-if="item.DataType==1" class="d-flex justify-content-center">
                                <!-- <button @click="delItem(index, item)" class="btn btn-outline-secondary btn-xs sharp m-1" title="刪除"><i class="fas fa-trash-alt"></i></button> -->
                                <button @click="copyItem(item)" class="btn btn-outline-secondary btn-xs sharp m-1" data-toggle="modal" data-target="#prepare_edit01" title="複製"><i class="fas fa-copy"></i></button>
                            </div>
                            <!-- button v-if="item.DataType==1" v-on:click.stop="delItem(index, item)" href="#" class="mx-1 btn-color9-1 btn btn-block" title="刪除"><i class="fas fa-trash-alt"></i></button -->
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
        <!-- 複製流程圖與管理標準 視窗 -->
        <div class="modal fade" id="prepare_edit01">
            <div class="modal-dialog modal-xl modal-dialog-centered " style="max-width: fit-content;">
                <div class="modal-content">
                    <div class="modal-header bg-0 text-white">
                        <h6 class="modal-title">複製流程圖與管理標準</h6>
                        <button ref="closeCopyModal" type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">×</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="card whiteBG mb-4 pattern-F colorset_1">
                            <table>
                                <tr>
                                    <td>後臺管理標準</td>
                                    <td>
                                        <input v-model.trim="stdCode" type="text" placeholder="代號" />
                                    <td>
                                    <td><button @click="importTemplate" class="btn btn-color2 btn-block">複製</button></td>
                                </tr>
                                <tr>
                                    <td>複製工程案號</td>
                                    <td width="500">
                                        <!-- input v-model.trim="searchEngNo" type="text" / -->
                                        <autocomplete :search="search" :get-result-value="getResultValue" @submit="handleSubmit" :debounce-time="300"></autocomplete>
                                    <td>
                                    <td><button @click="onClickSearchEngNo" class="btn btn-color2 btn-block">搜索</button></td>
                                </tr>
                                <tr>
                                    <td>複製項目</td>
                                    <td>
                                        <select v-model="selCopyItem" class="form-control">
                                            <option v-for="option in copyItemOptions" v-bind:value="option.Seq" v-bind:key="option.Seq">
                                                {{option.ItemName}}
                                            </option>
                                        </select>
                                    <td>
                                    <td><button @click="onClickCopyItem" v-bind:disabled="selCopyItem == -1" class="btn btn-color2 btn-block">複製</button></td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- 流程圖 -->
        <!-- <flowChartmodal ref="flowChartModal"
                v-on:onUploadEvent="onUploadFlowChartEvent" v-on:onDownloadEvent="onDownloadFlowChartEvent" v-on:onDelEvent="onDelFlowChartEvent" v-on:onCloseEvent="closeFlowChartModal"
                v-bind:modalId="'flowChartModal'" v-bind:engMain="engMain" v-bind:targetItem="targetItem" v-on:onShowEvent="onShowFlowChartEvent"></flowChartmodal> -->
        <FlowChartTpDiagramEdit :title="editDiagramItem.MDName" :editFlowChartTp="editDiagramItem.Seq"
                                :modalShow="modalShow" @handleModalShow="modalShow = !modalShow" type="Chapter702"
                                @download="onDownloadFlowChartEvent"
                                @upload="flowChartStatusChange"
                                :engSeq="engMain.Seq"
                                route="SupervisionPlan"> </FlowChartTpDiagramEdit>
        <flowChartmodal ref="flowChartModal"></flowChartmodal>
        <qcstdmodal v-on:onCloseEvent="closeQCStdModal"
                    v-bind:modalId="'qcStdModal'" v-bind:engMain="engMain" v-bind:targetItem="targetQCStdItem" v-bind:chapter="targetChapter"></qcstdmodal>
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
                editDiagramItem :{},
                modalShow: false,
                //複製項目 s20230302
                selCopyItem: -1,
                copyTargetItem: {},
                searchEngNo: '',
                copyItemOptions: [],
                stdCode: '', //s20230414
                //
                engResult: null, //s20230527
            };
        },
        methods: {
            //s20230527
            async search(input) {
                if (this.engResult != null && (this.engResult.EngNo + " " + this.engResult.EngName) == input) { return []; }
                if (input.length < 1) { return []; }

                this.engResult = null;
                this.searchEngNo = '';
                const { data } = await window.myAjax.post('/SupervisionPlan/SearchEng', { k: input });
                return data;
            },
            getResultValue(result) {
                return result.EngNo + " " + result.EngName;
            },
            handleSubmit(result) {
                this.searchEngNo = result.EngNo;
                this.engResult = result;
            },

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
                this.downloadFile('/SupervisionPlan/Chapter702DownloadCheckSheet?engMain=' + this.engMain.Seq + '&seq=' + item.Seq);
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
                window.myAjax.post('/SupervisionPlan/Chapter702', params)
                    .then(resp => {
                        this.items = resp.data.items;
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
                if (window.comm.stringEmpty(item.ItemName)) {
                    alert('項目須輸入資料');
                    return true;
                } else {
                    return false;
                }
            },
            // check All
            onKeepAllChange() {
                /*for (let item of this.items) {
                    if (this.isItemEmpty(item)) {
                        return;
                    }
                }*/
                window.myAjax.post('/SupervisionPlan/Chapter702SaveKeep', { items: this.items })
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
            //項目名稱, Keep儲存
            onInputChange(item) {

                if (this.isItemEmpty(item)) {
                    return;
                }
                window.myAjax.post('/SupervisionPlan/Chapter702Save', { item: item })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            const resultItem = resp.data.item.Data;
                            item.modifyDate = resultItem.modifyDate;
                        } else
                        {
                            item.DataKeep = resp.data.DataKeep;
                            alert(resp.data.message);
                        }
                     
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            newItem() {
                window.myAjax.post('/SupervisionPlan/Chapter702NewItem', { engMain: this.engMain.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            var item = resp.data.item.Data;
                            this.items.push(item);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            delItem(index, item) {
                //item.edit = false;
                if (confirm('是否確定刪除?')) {
                    window.myAjax.post('/SupervisionPlan/Chapter702Del', { seq: item.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                if (item.edit) this.editFlag = false;
                                this.items.splice(index, 1);
                            } else {
                                alert(resp.data.message);
                            }
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            //複製後台標準 s20230415
            importTemplate() {
                window.myAjax.post('/QCStdTp/Chapter702_ImportToSt', {
                    code: this.stdCode,
                    seq: this.copyTargetItem.Seq
                })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getList();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //複製項目 s20230302
            copyItem(item) {
                this.copyTargetItem = Object.assign({}, item);
                this.selCopyItem = -1;
                this.searchEngNo = '';
                this.copyItemOptions = [];
            },
            onClickSearchEngNo() {
                if (this.searchEngNo.length == 0) {
                    alert('請輸入工程案號');
                    return;
                }
                this.selCopyItem = -1;
                this.copyItemOptions = [];
                window.myAjax.post('/SupervisionPlan/Chapter702SearchEngNo', { engNo: this.searchEngNo })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.copyItemOptions = resp.data.items;
                        }
                        if (this.copyItemOptions.length == 0) {
                            alert('查無資料');
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onClickCopyItem() {
                this.$refs.closeCopyModal.click();
                if (this.selCopyItem == -1) {
                    alert('請選取複製項目');
                    return;
                }
                if (this.selCopyItem == this.copyTargetItem.Seq) {
                    alert('不能複製自己');
                    return;
                }
                window.myAjax.post('/SupervisionPlan/Chapter702CopyItem', { srcId: this.selCopyItem, tarId: this.copyTargetItem.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getList();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //抽查管理標準
            openQCStdModal(item) {
                this.targetQCStdItem = item;
                this.targetChapter = '702';
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
            onUploadFlowChartEvent: function (formData) {
                formData.append("engMain", this.engMain.Seq);
                formData.append("seq", this.targetItem.Seq);
                window.myAjax.post('/SupervisionPlan/Chapter702Upload', formData,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            const item = resp.data.item.Data;
                            this.targetItem.FlowCharOriginFileName = item.FlowCharOriginFileName;
                            this.targetItem.modifyDate = item.modifyDate;
                            this.$refs.flowChartModal.uploadFinish();
                        }
                        alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
            onDownloadFlowChartEvent(Seq) {
                this.$refs.flowChartModal.downloadFile('/SupervisionPlan/Chapter702DownloadFlowChart?engMain=' + this.engMain.Seq + '&seq=' + Seq);
            },
            onShowFlowChartEvent() {
                this.$refs.flowChartModal.showFile('/SupervisionPlan/Chapter702ShowFlowChart');
            },
            onDelFlowChartEvent() {
                window.myAjax.post('/SupervisionPlan/Chapter702DelFlowChart', { engMain: this.engMain.Seq, seq: this.targetItem.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.targetItem.FlowCharOriginFileName = '';
                        }
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            }
        },
        async mounted() {
            console.log('mounted() 第七章 702 環境保育管理標準');
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