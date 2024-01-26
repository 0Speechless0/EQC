<template>
    <div>
        <div class="table-responsive">
            <table class="table table-responsive-lg table-hover">
                <thead class="insearch">
                    <tr>
                        <th style="width: 42px;"><strong>編號</strong></th>
                        <th><strong>日期</strong></th>
                        <th style="width: 120px;"><strong>督導形式</strong></th>
                        <th><strong>主要(缺失)內容概述</strong></th>
                        <th><strong>預定改善日期</strong></th>
                        <th><strong>實際改善日期</strong></th>
                        <th><strong>結案日期</strong></th>
                        <th><strong>督導紀錄</strong></th>
                        <th class="text-center"><strong>編輯</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td>{{index+1}}</td>
                        <td>{{item.SuperviseDateStr}}</td>
                        <td>{{item.SuperviseModeStr}}</td>
                        <td>{{item.Missing}}</td>
                        <template v-if="item.Seq != editSeq">
                            <td>{{item.SchImprovDateStr}}</td>
                            <td>{{item.ActImprovDateStr}}</td>
                            <td>{{item.CloseDateStr}}</td>
                            <td>
                                <div class="d-flex justify-content-center">
                                    <!-- select v-model="fileType" class="form-control">
                                        <option value="4">docx</option>
                                        <option value="2">pdf</option>
                                        <option value="3">odt</option>
                                    </select -->
                                    <button @click="onDownloadDoc(item)" class="btn btn-color11-1 btn-xs sharp mx-1" title="下載"><i class="fas fa-download"></i></button>
                                </div>
                            </td>
                            <td>
                                <div class="d-flex justify-content-center">
                                    <button @click="onEditRecord(item)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                </div>
                            </td>
                        </template>
                        <template v-if="item.Seq == editSeq">
                            <td><input v-model="editRecord.SchImprovDateStr" type="date" class="form-control"></td>
                            <td><input v-model="editRecord.ActImprovDateStr" type="date" class="form-control"></td>
                            <td><input v-model="editRecord.CloseDateStr" type="date" class="form-control"></td>
                            <td>
                            </td>
                            <td>
                                <div class="d-flex justify-content-center">
                                    <button @click="onSaveRecord(editRecord)" class="btn btn-color11-1 btn-xs sharp mx-1" title="儲存"><i class="fas fa-save"></i></button>
                                    <button @click="onEditCancel" class="btn btn-color11-4 btn-xs sharp mx-1" title="取消"><i class="fas fa-times"></i></button>
                                </div>
                            </td>
                        </template>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['tenderItem'],
        data: function () {
            return {
                items: [],
                editSeq: -99,
                editRecord: {},
                fileType:2,//pdf
            };
        },
        methods: {
            //取消編輯
            onEditCancel() {
                this.editSeq = -99;
                this.getList();
            },
            //編輯紀錄
            onEditRecord(item) {
                if (this.editSeq > -99) return;
                this.editRecord = Object.assign({}, item);
                this.editSeq = this.editRecord.Seq;
            },
            strEmpty(str) {
                return window.comm.stringEmpty(str);
            },
            //儲存
            onSaveRecord(uItem) {
                uItem.SchImprovDate = uItem.SchImprovDateStr;
                uItem.ActImprovDate = uItem.ActImprovDateStr;
                uItem.CloseDate = uItem.CloseDateStr;
                window.myAjax.post('/EPCQualityVerify/UpdateItem', { m: uItem })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.editSeq = -99;
                            this.getList();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //取清單
            getList() {
                window.myAjax.post('/EPCQualityVerify/GetFillList', { id: this.tenderItem.PrjXMLSeq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //
            onDownloadDoc(item) {
                window.myAjax.get('/ESSchedule/Download?id=' + item.SEngSeq +'&docNo=6&docType=' + this.fileType, { responseType: 'blob' })
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
            console.log('mounted() 督導');
            this.getList();
        }
    }
</script>