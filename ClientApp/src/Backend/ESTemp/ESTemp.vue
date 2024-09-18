<template>
    <div>
        <div v-if="loading">
            Loading...
        </div>
        <!--<div class="row justify-content-between">
            <div class="col mt-3">
                <label for="tableinfo"></label>
            </div>
            <div class="col-12 col-md-4 col-xl-3 mt-3">
                <button v-bind:disabled="!showAdd" v-on:click.stop="newItem()" v-bind:class="{'btn-shadow btn-outline-secondary':showAdd, 'bg-gray':!showAdd}" class="btn btn-block">
                    <i class="fas fa-plus"></i>&nbsp;&nbsp;新增監造計畫書
                </button>
            </div>
        </div>-->
        <div class="table-responsive">
            <table class="table table1 min910" border="0">
                <thead>
                    <tr>
                        <th>名稱</th>
                        <th>修訂日期</th>
                        <th class="text-center">功能</th>
                    </tr>
                </thead>

                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.id">
                        <td>
                            {{item.Name}}
                        </td>
                        <td>
                            {{item.modifyDate}}
                        </td>
                        <td class="sm-2">
                            <div class="row justify-content-center m-0">
                                <button v-on:click.stop="openModal(item)" class="btn btn-color11-3 btn-xs mx-1" title="上傳"><i class="fas fa-upload"></i>上傳</button>
                                <button v-if="item.OriginFileName && item.OriginFileName.length>0" v-on:click.stop="download(item)" class="btn btn-color11-2 btn-xs mx-1" title="下載"><i class="fas fa-download"></i>下載</button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="modal fade show" id="MyDialog" ref="MyDialog" style="background:rgb(0 0 0 / 50%)" v-bind:style="{display: modalShow ? 'block' : 'none'}" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
            <div v-if="targetItem != null" class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header bg-R text-white">
                        <h6 class="modal-title font-weight-bold">上傳{{targetItem.Name}}</h6>
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
        data: function () {
            return {
                showAdd: false,
                loading: false,
                items: [],
                modalShow: false,
                targetItem: null,
                targetInput: {},
                files: new FormData(),                
            };
        },
        methods: {
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
                window.myAjax.post('/ESTemp/Upload', files,
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
                        }
                        alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
            download(item) {
                window.myAjax.get('/ESTemp/Download?seq='+item.Seq, { responseType: 'blob' })
                    .then(resp => {
                        const blob = new Blob([resp.data]);
                        const contentType = resp.headers['content-type'];
                        if (contentType.indexOf('application/json') >= 0) {
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
                this.loading = true;
                window.myAjax.post('/ESTemp/GetList').then(resp => {
                    this.loading = false;
                    this.items = resp.data;
                    this.showAdd = (this.items.length == 0);
                }).catch(error => {
                    this.loading = false;
                    console.log(error);
                });
            }
        },
        async mounted() {
            console.log('mounted() 工程督導範本維護');
            this.getList();
        }
    }
</script>