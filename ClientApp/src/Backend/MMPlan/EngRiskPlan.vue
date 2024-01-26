<template>
    <div>
        <div v-if="loading">
            Loading...
        </div>
        <div class="row justify-content-between">
            <div class="col mt-3">
                <label for="tableinfo"></label>
            </div>
            <div class="col-12 col-md-4 col-xl-3 mt-3">
                <button v-bind:disabled="!showAdd" v-on:click.stop="openModal()" v-bind:class="{'btn-shadow btn-color1':showAdd, 'bg-gray':!showAdd}" role="button" class="btn btn-block">
                    <i class="fas fa-plus"></i>&nbsp;&nbsp;新增施工風險評估
                </button>
            </div>
        </div>
        <div class="table-responsive">
            <table class="table table1 min910" border="0">
                <thead>
                    <tr>
                        <th >名稱</th>
                        <th >修訂日期</th>
                        <th >功能</th>
                    </tr>
                </thead>

                <tbody>
                    <tr>
                        <td>
                            {{item.Name}}
                        </td>
                        <td class="text-center">
                            {{item.modifyDate}}
                        </td>
                        <td class="sm-2">
                            <div class="row justify-content-center m-0">
                                <button v-on:click.stop="openModal(item)" class="btn btn-color11-3 btn-xs mx-1" title="上傳"><i class="fas fa-upload"></i>上傳</button>
                                <a href="RiskManagement/DownloadTp" download>
                                    <button   class="btn btn-color11-2 btn-xs mx-1" title="下載"><i class="fas fa-download"></i>下載</button>
                                </a>
      
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="modal fade show" id="MyDialog" ref="MyDialog" style="background:rgb(0 0 0 / 50%)" v-bind:style="{display: modalShow ? 'block' : 'none'}" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header bg-R text-white">
                        <h6 class="modal-title">上傳施工風險評估</h6>
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
                item: {},
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
                //v-bind:class="{show:modalShow, sm:modalShow}"
                //this.$refs['MyDialog'].classList.add('show');
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
                alert("檔案較大的話上傳需一段時間");
                window.myAjax.post('RiskManagement/UploadTp', files,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            this.item.Name = this.targetInput.files[0].name;
                            this.item.modifyDate = resp.data.modifyDate;
                                                    alert("上傳成功");
                            this.closeModal();
                        }

                    }).catch(error => {
                        console.log(error);
                    });
            },
            getList() {
                this.loading = true;
                window.myAjax.post('RiskManagement/GetTp').then(resp => {
                    this.loading = false;
                    console.log(resp.data);
                    this.item =  resp.data;
                    this.item.modifyDate = this.item.ModifyTime;
                    this.showAdd = (resp.data == null);
                }).catch(error => {
                    this.loading = false;
                    console.log(error);
                });
                //this.items = data;
                //(this.items.length == 0);
                
            }
        },
        async mounted() {
            console.log('mounted() 施工風險評維護');
            this.getList();
        }
    }
</script>