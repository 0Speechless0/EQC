<template>
    <div>
        <div class="row d-flex bd-highlight mb-3" style="padding-left:10px">
            <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                <label class="btn btn-shadow btn-color11-3">
                    <input v-on:change="fileChange($event)" id="inputFile" type="file" name="file" multiple="" style="display:none;">
                    批次匯入Excel&nbsp;<i class="fas fa-upload"></i>
                </label>
            </div>
            <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                <button @click="download" type="button" class="btn btn-shadow btn-color11-3" title="檔案下載">下載範本&nbsp;<i class="fas fa-download"></i></button>
            </div>
        </div>
        <div class="row">
            <div class="col-8">
                <table class="table">
                    <thead class="insearch">
                        <tr>
                            <th style="width:50px"><strong>分類</strong></th>
                            <th style="width:200px"><strong>說明</strong></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in kindList" @click="onItemClick(item)" v-bind:key="item.Id" v-bind:class="{'selectKindItem':selectKind.Id==item.Id}" >
                            <td>{{item.MCode}}</td>
                            <td>{{item.PS}}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="col-4">
                <table class="table table-hover">
                    <thead class="insearch">
                        <tr>
                            <th><strong>月份</strong></th>
                            <th><strong>物價指數</strong></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                            <td>{{item.PIDateStr}}</td>
                            <td>{{item.PriceIndex}}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                selectKind: {},
                kindList: [],
                items: [],
            };
        },
        methods: {
            strEmpty(str) {
                return window.comm.stringEmpty(str);
            },
            //儲存
            onSaveRecord(uItem) {
                //console.log(uItem);
                if (this.strEmpty(uItem.Code) || this.strEmpty(uItem.Item) || uItem.KgCo2e == null ) {
                    alert('編碼,工作項目,碳排係數 必須輸入!');
                    return;
                }
                if (uItem.Code.length < 10) {
                    alert('編碼必須輸入至少10碼');
                    return;
                }
                window.myAjax.post('/PriceIndex/UpdateRecords', { m: uItem })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.editSeq = -99;
                            this.getResords();
                            if (uItem.Seq == -1) this.onNewRecord();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //紀錄清單
            getResords() {
                this.items = [];
                window.myAjax.post('/PriceIndex/GetRecords', { kind: this.selectKind.Id })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                            //this.recordTotal = resp.data.pTotal;
                            //this.lastUpdate = resp.data.lastUpdate;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //分類清單
            getKindList() {
                this.kindList = [];
                window.myAjax.post('/PriceIndex/GetKindList')
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.kindList = resp.data.items;
                    }
                })
                .catch(err => {
                    console.log(err);
                });
            },
            onItemClick(item) {
                this.selectKind = item;
                this.getResords();
            },
            //匯入 excel
            fileChange(event) {
                var files = event.target.files || event.dataTransfer.files;
                // 預防檔案為空檔
                if (!files.length) return;

                //application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
                //application/vnd.ms-excel
                if (!files[0].type.match('application/vnd.openxmlformats-officedocument.spreadsheetml.sheet')) {// && !files[0].type.match('application/vnd.ms-excel') ) {
                    alert('請選擇 .xlsx Excel檔案');
                    return;
                }
                var uploadfiles = new FormData();
                uploadfiles.append("file", files[0], files[0].name);
                this.upload(uploadfiles);
            },
            upload(uploadfiles) {
                window.myAjax.post('/PriceIndex/Upload', uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            this.getResords();
                        }
                        alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
            //下載
            download() {
                window.myAjax.get('/PriceIndex/Download', { responseType: 'blob' })
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
        },
        mounted() {
            console.log('mounted() 物價指數維護');
            this.getKindList();
        }
    }
</script>
<style scoped>
    .selectKindItem {
        background-color:darkturquoise;
    }
</style>