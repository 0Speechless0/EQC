<template>
    <div v-if="targetId > 0">
        <h5 class="insearch mt-0 py-2">
            工程：{{tenderItem.EngName}}({{tenderItem.EngNo}})<br>標案：{{tenderItem.TenderName}}({{tenderItem.TenderNo}})
        </h5>
        <p class="text-R">※ 於生態檢核(設計階段)「是否須辦理生態檢核」選擇否者，此處不須上傳文件</p>
        <div v-if="showUpload" class="table-responsive">
            <table class="table table-responsive-md table-hover">
                <thead>
                    <tr class="insearch">
                        <th class="text-left"><strong>資料名稱</strong></th>
                        <th><strong>上傳檔案</strong></th>
                        <th style="width:40px"><strong>下載</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td class="text-left"><strong>主表-公共工程生態檢核自評表</strong></td>
                        <td><b-form-file v-model="SelfEvalFilename" multiple v-bind:placeholder="filePlaceholder(tarRecord.SelfEvalFilename)"></b-form-file></td>
                        <!-- <td><button @click="download(tarRecord,'SelfEvalFilename')" v-if="!strEmpty(tarRecord.SelfEvalFilename)" role="button" class="btn btn-shadow btn-block btn-color11-1"><i class="fas fa-download"></i></button></td> -->
                        <td><button class="btn btn-shadow btn-block btn-color11-1" @click="openDownloadList()"> <i class="fas fa-download"></i></button> </td>
                    </tr>
                    <tr>
                        <td class="text-left"><strong>附表C-01 前置作業資料紀錄表</strong></td>
                        <td><b-form-file v-model="EngDiagram" v-bind:placeholder="filePlaceholder(tarRecord.EngDiagram)"></b-form-file></td>
                        <td><button @click="download(tarRecord,'EngDiagram')" v-if="!strEmpty(tarRecord.EngDiagram)" role="button" class="btn btn-shadow btn-block btn-color11-1"><i class="fas fa-download"></i></button></td>
                    </tr>
                    <tr>
                        <td class="text-left"><strong>附表C-02 現場勘查/會議紀錄表</strong></td>
                        <td><b-form-file v-model="DataCollectDocFilename" v-bind:placeholder="filePlaceholder(tarRecord.DataCollectDocFilename)"></b-form-file></td>
                        <td><button @click="download(tarRecord,'DataCollectDocFilename')" v-if="!strEmpty(tarRecord.DataCollectDocFilename)" role="button" class="btn btn-shadow btn-block btn-color11-1"><i class="fas fa-download"></i></button></td>
                    </tr>
                    <tr>
                        <td class="text-left"><strong>附表C-03 民眾參與紀錄表</strong></td>
                        <td><b-form-file v-model="PlanDesignRecordFilename" v-bind:placeholder="filePlaceholder(tarRecord.PlanDesignRecordFilename)"></b-form-file></td>
                        <td><button @click="download(tarRecord,'PlanDesignRecordFilename')" v-if="!strEmpty(tarRecord.PlanDesignRecordFilename)" role="button" class="btn btn-shadow btn-block btn-color11-1"><i class="fas fa-download"></i></button></td>
                    </tr>

                    <tr>
                        <td class="text-left"><strong>附表 C-04 生態保育措施自主檢查表</strong></td>
                        <td><b-form-file v-model="ChecklistFilename" v-bind:placeholder="filePlaceholder(tarRecord.ChecklistFilename)"></b-form-file></td>
                        <td><button @click="download(tarRecord,'ChecklistFilename')" v-if="!strEmpty(tarRecord.ChecklistFilename)" role="button" class="btn btn-shadow btn-block btn-color11-1"><i class="fas fa-download"></i></button></td>
                    </tr>
                    <tr>
                        <td class="text-left"><strong>附表 C-05 生態保育措施抽查表</strong></td>
                        <td><b-form-file v-model="SOCFilename" v-bind:placeholder="filePlaceholder(tarRecord.SOCFilename)"></b-form-file></td>
                        <td><button @click="download(tarRecord,'SOCFilename')" v-if="!strEmpty(tarRecord.SOCFilename)" role="button" class="btn btn-shadow btn-block btn-color11-1"><i class="fas fa-download"></i></button></td>
                    </tr>
                    <tr>
                        <td class="text-left"><strong>附表 C-06 生態調查評析表 </strong></td>
                        <td><b-form-file v-model="ConservMeasFilename" v-bind:placeholder="filePlaceholder(tarRecord.ConservMeasFilename)"></b-form-file></td>
                        <td><button @click="download(tarRecord,'ConservMeasFilename')" v-if="!strEmpty(tarRecord.ConservMeasFilename)" role="button" class="btn btn-shadow btn-block btn-color11-1"><i class="fas fa-download"></i></button></td>
                    </tr>

                    <tr>
                        <td class="text-left"><strong>附表 C-07 環境生態異常狀況處理表</strong></td>
                        <td><b-form-file v-model="LivePhoto" v-bind:placeholder="filePlaceholder(tarRecord.LivePhoto)"></b-form-file></td>
                        <td><button @click="download(tarRecord,'LivePhoto')" v-if="!strEmpty(tarRecord.LivePhoto)" role="button" class="btn btn-shadow btn-block btn-color11-1"><i class="fas fa-download"></i></button></td>
                    </tr>
                    <tr>
                        <td class="text-left"><strong>附表 C-08 不合格(或環境生態異常狀況)事項報告表</strong></td>
                        <td><b-form-file v-model="Other" v-bind:placeholder="filePlaceholder(tarRecord.Other)"></b-form-file></td>
                        <td><button @click="download(tarRecord,'Other')" v-if="!strEmpty(tarRecord.Other)" role="button" class="btn btn-shadow btn-block btn-color11-1"><i class="fas fa-download"></i></button></td>
                    </tr>
                    <tr>
                        <td class="text-left"><strong>附表 C-09 不合格(或環境生態異常狀況)事項彙整表</strong></td>
                        <td><b-form-file v-model="Other2" v-bind:placeholder="filePlaceholder(tarRecord.Other2)"></b-form-file></td>
                        <td><button @click="download(tarRecord,'Other2')" v-if="!strEmpty(tarRecord.Other2)" role="button" class="btn btn-shadow btn-block btn-color11-1"><i class="fas fa-download"></i></button></td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="card-footer">
            <div v-if="tarRecord.Seq > 0" class="row justify-content-center">
                <!-- div class="col-12 col-sm-4 col-xl-2 my-2">
                <button role="button" class="btn btn-shadow btn-color3 btn-block"> 取消修改 </button>
            </div -->
                <div class="col-12 col-sm-4 col-xl-2 my-2">
                    <button @click="onSaveRecord" role="button" class="btn btn-shadow btn-block btn-color11-2"> 儲存 <i class="fas fa-save"></i></button>
                </div>
            </div>
        </div>
        <downloadList route="EPCEcologicalCheck" :seq="targetId" ref="downloadList"> </downloadList>
    </div>
</template>
<script>
    import downloadList from "../../components/downloadList.vue";
    export default {
        data: function () {
            return {
                targetId: null,
                tenderItem: {},
                tarRecord: {},
                showUpload: false,
                //是
                ChecklistFilename: null,
                SelfEvalFilename: [],
                PlanDesignRecordFilename: null,
                MemberDocFilename: null,
                DataCollectDocFilename: null,
                ConservMeasFilename: null,
                SOCFilename: null,
                LivePhoto: null,
                EngDiagram: null,
                Other: null,
                Other2: null
            };
        },
        components:{
            downloadList: downloadList
        },
        methods: {
            openDownloadList(){
                $("#downloadList").modal("show");
            },
            filePlaceholder(d) {
                if (this.strEmpty(d))
                    return "未選擇任何檔案";
                else
                    return d;
            },
            strEmpty(str) {
                return window.comm.stringEmpty(str);
            },
            //儲存
            onSaveRecord() {
                var uploadfiles = new FormData();
                uploadfiles.append("seq", this.tarRecord.Seq);
                if (this.ChecklistFilename != null) uploadfiles.append("ChecklistFilename", this.ChecklistFilename, this.ChecklistFilename.name);
                if (this.SelfEvalFilename != null) {
                    this.SelfEvalFilename.forEach( (element, index) => {
                        uploadfiles.append(`SelfEvalFilename`, element);
                    });

                }
                if (this.PlanDesignRecordFilename != null) uploadfiles.append("PlanDesignRecordFilename", this.PlanDesignRecordFilename, this.PlanDesignRecordFilename.name);
                if (this.DataCollectDocFilename != null) uploadfiles.append("DataCollectDocFilename", this.DataCollectDocFilename, this.DataCollectDocFilename.name);
                if (this.ConservMeasFilename != null) uploadfiles.append("ConservMeasFilename", this.ConservMeasFilename, this.ConservMeasFilename.name);
                if (this.SOCFilename != null) uploadfiles.append("SOCFilename", this.SOCFilename, this.SOCFilename.name);
                if (this.LivePhoto != null) uploadfiles.append("LivePhoto", this.LivePhoto, this.LivePhoto.name);
                if (this.EngDiagram != null) uploadfiles.append("EngDiagram", this.EngDiagram, this.EngDiagram.name);
                if (this.Other != null) uploadfiles.append("Other", this.Other, this.Other.name);
                if (this.Other2 != null) uploadfiles.append("Other2", this.Other2, this.Other2.name);
                this.upload(uploadfiles);

            },
            //上傳
            upload(uploadfiles) {
                window.myAjax.post('/EPCEcologicalCheck/UpdateRecord', uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            this.getResords();
                            this.$refs.downloadList.getdownloaditem();
                        }
                        alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
            //下載
            download(item, key) {
                window.myAjax.get('/EPCEcologicalCheck/Download?id=' + item.Seq+'&key='+key, { responseType: 'blob' })
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
            getResords() {
                this.showUpload = false;
                window.myAjax.post('/EPCEcologicalCheck/GetList', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.tarRecord = resp.data.item;
                            this.showUpload = true;
                        } else if(resp.data.result == 1) {
                            alert(resp.data.msg);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //取標案
            getItem() {
                if (this.targetId == null) {
                    alert('請先選取標案');
                    return;
                }
                window.myAjax.post('/EPCEcologicalCheck/GetEngMain', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.tenderItem = resp.data.item;
                            this.getResords();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        async mounted() {
            console.log('mounted() 生態檢核(施工階段)上傳');
            this.targetId = window.sessionStorage.getItem(window.epcSelectTrenderSeq);
            this.getItem();
        }
    }
</script>
