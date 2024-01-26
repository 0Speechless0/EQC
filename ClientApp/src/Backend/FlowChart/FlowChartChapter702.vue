<template>
    <div>
        <div v-if="loading">
            Loading...
        </div>
        <div class="table-responsive">
            <table class="table table1 min910" border="0">
                <thead>
                    <tr>
                        <th class="sort">排序</th>
                        <th class="number">excel編號</th>
                        <th>環境保育清單</th>
                        <th>建立日期</th>
                        <th>更新日期</th>
                        <th>施工項目</th>
                        <th>流程圖編輯</th>
                    </tr>
                </thead>

                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.id">
                        <td>
                            <div v-if="!item.edit">{{ item.OrderNo }}</div>
                            <input v-if="item.edit" type="text" v-model.number="item.OrderNo" class="form-control" />
                        </td>

                        <td>
                            <div v-if="!item.edit">{{ item.ExcelNo }}</div>
                            <input maxlength="10" v-if="item.edit" type="text" v-model.trim="item.ExcelNo"
                                class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{ item.ItemName }}</div>
                            <input maxlength="100" v-if="item.edit" type="text" v-model.trim="item.ItemName"
                                class="form-control" />
                        </td>
                        <td>
                            {{ item.createDate }}
                        </td>
                        <td>
                            {{ item.modifyDate }}
                        </td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <a v-on:click.stop="editItem(item)" v-if="!item.edit" href="#"
                                    class="btn btn-color11-2 btn-xs m-1" title="編輯"><i class="fas fa-pencil-alt"></i>
                                    編輯</a>
                                <a v-on:click.stop="saveItem(item)" v-if="item.edit" href="#"
                                    class="btn btn-color11-3 btn-xs m-1" title="儲存"><i class="fas fa-save"></i> 儲存</a>
                                <a v-on:click.stop="delItem(index, item)" href="#" class="btn btn-color9-1 btn-xs m-1"
                                    title="刪除"><i class="fas fa-trash-alt"></i> 刪除</a>
                            </div>
                        </td>
                        <td>
                            <div class="row justify-content-center m-0">
                               <a v-on:click.stop="openModal(item)" href="#" class="btn btn-color11-2 btn-xs m-1" title="預覽"><i  class="fas fa-eye"></i>  預覽</a>
                            </div>
                            <!-- 大圖 -->
                            <div class="modal fade" id="exampleModal" data-backdrop="static" data-keyboard="false"
                                tabindex="-1" v-bind:aria-labelledby="exampleModalLabel" aria-modal="true"
                                aria-hidden="true">
                                <div class="modal-dialog modal-lg">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <h5 class="modal-title" id="projectUpload">流程圖</h5>
                                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                                <span aria-hidden="true">×</span>
                                            </button>
                                        </div>
                                        <div class="modal-body">
                                            <div class="table-responsive">
                                                <img v-bind:src="getPhotoPath()" class="rounded"
                                                    style="max-height:600px;">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
            <div style="width:99%;" class="row justify-content-center">
                <b-pagination :total-rows="totalRows" :per-page="perPage" v-model="pageIndex">
                </b-pagination>
            </div>
            <FlowChartTpDiagramEdit :title="editDiagramItem.MDName" :editFlowChartTp="editDiagramItem.Seq"
                :modalShow="modalShow" @handleModalShow="modalShow = !modalShow" type="Chapter702"
                @download="download"
                        route="FlowChartTp"
                ></FlowChartTpDiagramEdit>
        </div>
        <!-- <div class="modal fade show" id="MyDialog" ref="MyDialog" style="background:rgb(0 0 0 / 50%)" v-bind:style="{display: modalShow ? 'block' : 'none'}" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header bg-R text-white">
                        <h6 class="modal-title">上傳流程圖</h6>
                    </div>
                    <div class="modal-body">
                        <input id="inputFile" type="file" name="file" multiple="" v-on:change="fileChange($event)" />
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-color9-1 btn-xs mx-1" data-dismiss="modal" v-on:click="closeModal()"><i class="fas fa-times"></i> 關閉</button>
                        <button type="button" class="btn btn-color11-3 btn-xs mx-1" v-on:click.stop="upload()"><i class="fas fa-upload"></i> 上傳</button>
                    </div>
                </div>
            </div>
        </div> -->
    </div>
</template>
<script>
import FlowChartTpDiagramEdit from "./FlowChartTpDiagramEdit.vue";

export default {
    props: ['addItem'],
    watch: {
        'addItem': function (nval, oval) {
            this.newItem();
        },
        pageIndex: {
            handler: function (value) {
                this.getList();
            }
        }
    },
    data: function () {
        return {
            editFlag: false,
            loading: false,
            items: [],
            modalShow: false,
            targetItem: null,
            targetInput: {},
            files: new FormData(),
            //分頁
            pageIndex: 1,
            perPage: 10,
            totalRows: 0,
            //流程圖路徑
            photo: '',
            editDiagramItem: {}
        };
    },
    methods: {
        openModal(item) {
            this.files = new FormData();
            this.targetItem = item;
            this.modalShow = true;
            this.editDiagramItem = item;
        },
        closeModal() {
            this.targetInput.value = '';
            this.modalShow = false;
        },
        fileChange(event) {
            this.targetInput = event.target;
            this.files.append("file", this.targetInput.files[0], this.targetInput.files[0].name);
            this.files.append("seq", this.targetItem.Seq);
        },
        upload(seq, file) {
                        this.files = new FormData();
            this.files.append("file", file, file.name);
            this.files.append("seq", seq);
            window.myAjax.post('/FlowChartTp/Chapter702Upload', this.files,
                {
                    headers: {
                        'Content-Type': 'multipart/form-data'
                    }
                })
            // .then(resp => {
            //     if (resp.data.result == 0) {
            //         const item = resp.data.item.Data;
            //         this.targetItem.FlowCharOriginFileName = this.targetInput.files[0].name;
            //         this.targetItem.modifyDate = item.modifyDate;
            //         this.closeModal();
            //     } else {
            //         alert(resp.data.message);
            //     }
            // }).catch(error => {
            //     console.log(error);
            // });
        },
        download(Seq) {
            window.myAjax.get('/FlowChartTp/Chapter702Download?seq=' + Seq, { responseType: 'blob' })
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
        getList() {
            this.editFlag = false;
            this.loading = true;
            this.items = [];
            let params = { pageIndex: this.pageIndex, perPage: this.perPage };
            window.myAjax.post('/FlowChartTp/Chapter702', params)
                .then(resp => {
                    this.items = resp.data.items;
                    this.totalRows = resp.data.pTotal;
                    this.loading = false;
                })
                .catch(err => {
                    this.loading = false;
                    console.log(err);
                });
        },
        editItem(item) {
            if (this.editFlag) return;

            this.editFlag = true;
            item.edit = this.editFlag;
        },
        newItem() {
            if (this.editFlag) return;
            window.myAjax.post('/FlowChartTp/Chapter702NewItem')
                .then(resp => {
                    if (resp.data.result == 0) {
                        var item = resp.data.item.Data;
                        item.edit = true;
                        this.editFlag = true;
                        this.items.push(item);
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        saveItem(item) {
            if (item.OrderNo === '' || window.comm.stringEmpty(item.ExcelNo) || window.comm.stringEmpty(item.ItemName)) {
                alert('所有欄位均須輸入資料');
                return;
            }
            window.myAjax.post('/FlowChartTp/Chapter702Save', { item: item })
                .then(resp => {
                    if (resp.data.result == 0) {
                        item.edit = false;
                        this.editFlag = false;
                        const resultItem = resp.data.item.Data;
                        item.modifyDate = resultItem.modifyDate;
                    }
                    this.sortItems();
                    alert(resp.data.message);
                })
                .catch(err => {
                    console.log(err);
                });
        },
        delItem(index, item) {
            console.log('index: ' + index + ' seq: ' + item.Seq);
            if (confirm('是否確定刪除?')) {
                window.myAjax.post('/FlowChartTp/Chapter702Del', { seq: item.Seq })
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
        sortItems() {
            this.items.sort(function (a, b) {
                var nameA = a.OrderNo;
                var nameB = b.OrderNo;
                if (nameA < nameB) {
                    return -1;
                }
                if (nameA > nameB) {
                    return 1;
                }
                return 0;
            });
        },
        showPicture(item) {
            window.myAjax.post('/FlowChartTp/Chapter702Show', { seq: item.Seq })
                .then(resp => {
                    if (resp.data.url != null) {
                        this.photo = resp.data.url;
                    } else {
                        console.log(resp.data.message);
                    }


                    return '/FileUploads/Tp/' + resp.url;
                }).catch(error => {
                    console.log(error);
                });
        },
        getPhotoPath(item) {

            return '/FileUploads/Tp/' + this.photo;
        }
    },
    async mounted() {
        console.log('mounted() 第七章 702 環境保育管理標準');
        this.getList();
    },
    components: {
        FlowChartTpDiagramEdit
    }
}
</script>