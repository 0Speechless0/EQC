<template>
    <div>
        <SubList v-if="!listMode" v-bind:masterItem="masterItem"  v-on:closeSubList="closeSubList"></SubList>
        <div v-show="listMode">
            <div class="row d-flex bd-highlight mb-3" style="padding-left:10px">
                <label class="btn btn-shadow btn-color11-3">
                    <input v-on:change="fileChange($event)" id="inputFile" type="file" name="file" multiple="" style="display:none;">
                    <i class="fas fa-upload"></i> 批次匯入Excel
                </label>
                <a :href="'ExcelTemplate/CarbonEmissionFactor.xlsx'" class="bd-highlight" download style="padding-left: 15px;">
                    <button role="button" class="btn btn-shadow btn-color11-3 btn-block">
                        下載範例
                    </button>
                </a>
                <div class="d-flex pl-3 pb-2">
                    <input v-model="searchStr" />
                    <button type="button" class="btn btn-outline-success" @click="search()">搜尋</button>
                </div>
                <div class="align-self-center ml-4" style="color:red">
                    可輸入編碼
                </div>
                <!-- div class="ml-auto bd-highlight align-self-center" style="padding-right: 15px;">
                <button @click="download" class="btn btn-color11-1 btn-block">
                    <i class="fas fa-download"></i>碳排管制表
                </button>
            </div -->
            </div>
            <div class="row d-flex bd-highlight mb-3" style="padding-left:10px">
                <div class="ml-auto bd-highlight align-self-center" style="padding-right: 15px;">
                    更新日期:{{getDate(lastUpdate, true)}}
                </div>
            </div>
            <div class="row justify-content-between">
                <comm-pagination class="col-12" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
            </div>
            <div class="table-responsive">
                <table class="table table-responsive-md table-hover table2">
                    <thead class="insearch">
                        <tr>
                            <th><strong>項次</strong></th>
                            <th><strong>編碼</strong></th>
                            <th><strong>工作項目</strong></th>
                            <th><strong>碳排係數(kgCO2e)</strong></th>
                            <th><strong>單位</strong></th>
                            <th><strong>細目編碼</strong></th>
                            <th><strong>備註</strong></th>
                            <th class="text-center"><strong>管理</strong></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                            <td style="min-width: unset;">{{pageRecordCount*(pageIndex-1)+index+1}}</td>
                            <template v-if="item.Seq != editSeq">
                                <td v-html="checkCode(item)"></td>
                                <td>{{item.Item}}</td>
                                <td>{{item.KgCo2e}}</td>
                                <td>{{item.Unit}}</td>
                                <td>{{item.SubCode}}</td>
                                <td>{{item.Memo}}</td>
                                <td style="min-width: unset;">
                                    <div class="d-flex justify-content-center">
                                        <button @click="editDetail(item)" class="btn btn-color11-2 btn-xs sharp m-1" title="碳分析"><i class="fas fa-newspaper"></i></button>
                                        <button @click="onEditRecord(item)" class="btn btn-color11-1 btn-xs sharp m-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                        <button @click="onDelRecord(item)" class="btn btn-color9-1 btn-xs sharp m-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                                    </div>
                                </td>
                            </template>
                            <template v-if="item.Seq == editSeq">
                                <td><input v-model.trim="editRecord.Code" maxlength="20" type="text" class="form-control"></td>
                                <td><input v-model.trim="editRecord.Item" maxlength="50" type="text" class="form-control"></td>
                                <td><input v-model="editRecord.KgCo2e" type="text" class="form-control"></td>
                                <td><input v-model.trim="editRecord.Unit" maxlength="10" type="text" class="form-control"></td>
                                <td><input v-model.trim="editRecord.SubCode" maxlength="20" type="text" class="form-control"></td>
                                <td><textarea v-model="editRecord.Memo" maxlength="100" rows="5" class="form-control"></textarea></td>
                                <td style="min-width: unset;">
                                    <div class="d-flex justify-content-center">
                                        <button @click="onSaveRecord(editRecord)" class="btn btn-color11-2 btn-xs sharp m-1" title="儲存"><i class="fas fa-save"></i></button>
                                        <button @click="onEditCancel" class="btn btn-color9-1 btn-xs sharp m-1" title="取消"><i class="fas fa-times"></i></button>
                                    </div>
                                </td>
                            </template>
                        </tr>
                        <tr>
                            <td></td>
                            <td><input v-model.trim="newItem.Code" maxlength="20" type="text" class="form-control"></td>
                            <td><input v-model.trim="newItem.Item" maxlength="50" type="text" class="form-control"></td>
                            <td><input v-model="newItem.KgCo2e" type="text" class="form-control"></td>
                            <td><input v-model.trim="newItem.Unit" maxlength="10" type="text" class="form-control"></td>
                            <td><input v-model.trim="newItem.SubCode" maxlength="20" type="text" class="form-control"></td>
                            <td><textarea v-model="newItem.Memo" maxlength="100" rows="5" class="form-control"></textarea></td>
                            <td style="min-width: unset;">
                                <div class="d-flex justify-content-center">
                                    <button @click="onSaveRecord(newItem)" class="btn btn-outline-secondary btn-xs sharp m-1" title="新增"><i class="fas fa-plus"></i></button>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</template>
<script>
import Common from "../../Common/Common.js";
    export default {
        data: function () {
            return {
                listMode: true,
                targetId: null,
                items: [],
                totalScore: -1,
                editSeq: -99,
                editRecord: {},
                newItem: {},
                masterItem: {},
                //分頁
                recordTotal: 0,
                pageIndex: 1,
                pageRecordCount: 30,
                lastUpdate: null,
                searchStr:null
            };
        },
        components: {
            SubList: require('./CEFactor_SubList.vue').default,
        },
        methods: {
            editDetail(item) {
                this.masterItem = item;
                this.listMode = false
            },
            closeSubList() {
                this.listMode = true;
                this.getResords();
            },
            /*/下載 水利署碳排管制表
            download() {
                window.myAjax.get('/CEFactor/dnCCT', { responseType: 'blob' })
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
            },*/
            search() {
                this.getResords();
            },
            getDate(time, hasTime = false) {

                return Common.ToROCDate(time, hasTime);
            },
            checkCode(item) {
                if (item.KeyCode2 == '-1')
                    return item.Code + '<br /><font color="red">(待修正)</font>'
                else
                    return item.Code;
            },
            onNewRecord() {
                window.myAjax.post('/CEFactor/NewRecord')
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.newItem = resp.data.item;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //刪除紀錄
            onDelRecord(item) {
                if (this.editSeq > -99) return;
                if (confirm('是否確定刪除資料？')) {
                    window.myAjax.post('/CEFactor/DelRecord', { id: item.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getResords();
                            }
                            alert(resp.data.msg);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
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
                window.myAjax.post('/CEFactor/UpdateRecords', { m: uItem })
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
            //取消編輯
            onEditCancel() {
                this.editSeq = -99;
                this.getResords();
            },
            //編輯紀錄
            onEditRecord(item) {
                if (this.editSeq > -99) return;
                this.editRecord = Object.assign({}, item);
                this.editSeq = this.editRecord.Seq;
            },
            //紀錄清單
            getResords() {
                this.items = [];
                window.myAjax.post('/CEFactor/GetRecords', {
                        pageRecordCount: this.pageRecordCount,
                        pageIndex: this.pageIndex,
                        keyWord : this.searchStr ?? ""
                    })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                            this.recordTotal = resp.data.pTotal;
                            this.lastUpdate = resp.data.lastUpdate;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //分頁
            onPaginationChange(pInx, pCount) {
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
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
                window.myAjax.post('/CEFactor/Upload', uploadfiles,
                    {
                        headers : { 'Content-Type': 'multipart/form-data' }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            this.getResords();
                        }
                        alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
        },
        mounted() {
            console.log('mounted() 碳排係數維護');
            this.getResords();
            this.onNewRecord();
        }
    }
</script>