<template>
    <div>
        <div class="row justify-content-between">
            <div class="col-12 col-md-6 col-xl-4 mt-3">
                <select v-model="selectChapter" @change="onChapterChange($event)" class="form-control">
                    <option v-for="option in selectChapterOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                        {{ option.Text }}
                    </option>
                </select>
            </div>
            <div v-if="!loading && selectChapter!='-1'" class="col-12 col-md-4 col-xl-4 mt-3">
                <a v-on:click.stop="newItem()" role="button" class="btn btn-shadow btn-color1 btn-block">
                    <i class="fas fa-plus"></i>&nbsp;&nbsp;新增 圖或表
                </a>
            </div>
        </div>
        <div v-if="loading">
            Loading...
        </div>
        <div class="table-responsive">
            <table class="table table1 min910 tableLayoutFixed" border="0">
                <thead>
                    <tr>
                        <th class="sort">排序</th>
                        <th>章節</th>
                        <th>編號</th>
                        <th>種類</th>
                        <th>圖表名稱</th>
                        <th>建立日期</th>
                        <th>更新日期</th>
                        <th>編輯或刪除</th>
                        <th>上傳或下載</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.id">
                        <td>
                            <div v-if="!item.edit">{{item.OrderNo}}</div>
                            <input v-if="item.edit" type="text" v-model.number="item.OrderNo" class="form-control" />
                        </td>
                        <td>
                            {{item.ChapterName}}
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.ExcelNo}}</div>
                            <input maxlength="10" v-if="item.edit" type="text" v-model.trim="item.ExcelNo" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{getKindText(item.ChartKind)}}</div>
                            <select v-if="item.edit" v-model.number="item.ChartKind" class="form-control">
                                <option value="1">表</option>
                                <option value="2">圖</option>
                            </select>
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.ChartName}}</div>
                            <input maxlength="50" v-if="item.edit" type="text" v-model.trim="item.ChartName" class="form-control" />
                        </td>
                        <td>
                            {{item.createDate}}
                        </td>
                        <td>
                            {{item.modifyDate}}
                        </td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <a v-on:click.stop="editItem(item)" v-if="!item.edit" href="#" class="btn btn-color11-2 btn-xs m-1" title="編輯"><i class="fas fa-pencil-alt"></i>  編輯</a>
                                <a v-on:click.stop="saveItem(item)" v-if="item.edit" href="#" class="btn btn-color11-3 btn-xs m-1" title="儲存"><i class="fas fa-save"></i> 儲存</a>
                                <a v-on:click.stop="delItem(index, item)" href="#" class="btn btn-color9-1 btn-xs m-1" title="刪除"><i class="fas fa-trash-alt"></i> 刪除</a>
                            </div>
                        </td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <a v-on:click.stop="openModal(item)" href="#" class="btn btn-color11-3 btn-xs m-1" title="上傳"><i class="fas fa-upload"></i>  上傳</a>
                                <a v-if="item.OriginFileName && item.OriginFileName.length>0" v-on:click.stop="download(item)" href="#" class="btn btn-color11-2 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> 下載</a>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div style="width:99%;" class="row justify-content-center">
            <b-pagination :total-rows="totalRows"
                          :per-page="perPage"
                          v-model="pageIndex">
            </b-pagination>
        </div>
        <div class="modal fade show" id="MyDialog" ref="MyDialog" style="background:rgb(0 0 0 / 50%)" v-bind:style="{display: modalShow ? 'block' : 'none'}" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header bg-R text-white">
                        <h6 class="modal-title">上傳 圖/表</h6>
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
        </div>
    </div>
</template>
<script>
    export default {
        watch: {
            pageIndex: {
                handler: function (value) {
                    this.getList();
                }
            }
        },
        data: function () {
            return {
                editFlag: false,
                selectChapter: '-1',
                selectChapterOptions: [],
                //分頁
                pageIndex: 1,
                perPage: 10,
                totalRows: 0,

                loading: false,
                items: [],
                modalShow: false,
                targetItem: null,
                targetInput: {},
                files: new FormData(),
            };
        },
        methods: {
            async getChapterOption() {
                const { data } = await window.myAjax.post('/ChapterChart/GetChapterOptions');
                this.selectChapterOptions = data;
                this.getList();
            },
            onChapterChange(event) {
                this.pageIndex = 1;
                this.getList()
            },
            openModal(item) {
                this.files = new FormData();
                this.targetItem = item;
                this.modalShow = true;
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
            upload() {
                const files = this.files;
                window.myAjax.post('/ChapterChart/ChapterUpload', files,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            const item = resp.data.item.Data;
                            this.targetItem.OriginFileName = this.targetInput.files[0].name;
                            this.targetItem.modifyDate = item.modifyDate;
                            this.closeModal();
                        } else {
                            alert(resp.data.message);
                        }
                    }).catch(error => {
                        console.log(error);
                    });
            },
            download(item) {
                //window.open('/ChapterChart/ChapterDownload' + '?seq=' + item.Seq);
                window.myAjax.get('/ChapterChart/ChapterDownload?seq=' + item.Seq, { responseType: 'blob' })
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
            getKindText(value) {
                if (value == 1) {
                    return '表';
                } else if (value == 2) {
                    return '圖';
                } else {
                    return '';
                }
            },
            async getList() {
                this.editFlag = false;
                this.loading = true;
                let params = { chapter: this.selectChapter, pageIndex: this.pageIndex, perPage: this.perPage };
                window.myAjax.post('/ChapterChart/Chapter', params)
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
                
                window.myAjax.post('/ChapterChart/ChapterNewItem', { chapter: this.selectChapter })
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
                if (item.OrderNo === '' || window.comm.stringEmpty(item.ExcelNo) || window.comm.stringEmpty(item.ChartKind) || window.comm.stringEmpty(item.ChartName) ) {
                    alert('所有欄位均須輸入資料');
                    return;
                }
                window.myAjax.post('/ChapterChart/ChapterSave', { item: item })
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
                    window.myAjax.post('/ChapterChart/ChapterDel', { seq: item.Seq })
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
            }
        },
        async mounted() {
            console.log('mounted() 圖表維護');
            if (this.selectChapterOptions.length == 0) {
                this.getChapterOption();
            }
        }
    }
</script>
