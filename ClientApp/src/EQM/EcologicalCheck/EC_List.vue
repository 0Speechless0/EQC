<template>
    <div v-if="targetId > 0">
        <h5 class="insearch mt-0 py-2">
            工程編號：{{tenderItem.TenderNo}}({{tenderItem.EngNo}})<br>工程名稱：{{tenderItem.TenderName}}({{tenderItem.EngName}})
        </h5>

        <div class="form-group row">
            <label class="col-md-4 col-form-label">是否須辦理生態檢核</label>
            <div class="col-md-8">
                <select v-model="tarRecord.ToDoChecklit" class="form-control">
                    <option value="1">是(新建工程)</option>
                    <option value="2">是(其他)</option>
                    <option value="3">否(災後緊急處理、搶修、搶險)</option>
                    <option value="4">否(災後原地復建)</option>
                    <option value="5">否(原構造物範圍內之整建或改善)</option>
                    <option value="6">否(已開發場所且經自評確認無涉及生態環境保育議題)</option>
                    <option value="7">否(劃取得綠建築標章並納入生態範疇相關指標之建築工程)</option>
                    <option value="8">否(維護相關工程)</option>
                </select>
            </div>
        </div>
        <div v-if="tarRecord.ToDoChecklit >2" class="form-group row">
            <label class="col-md-4 col-form-label">選否：上傳「生態檢核前置作業確認表」檔案</label>
            <div class="col-md-7">
                <b-form-file v-model="ChecklistFilename" v-bind:placeholder="filePlaceholder(tarRecord.ChecklistFilename)"></b-form-file>
            </div>
            <div class="col-md-1">
                <button @click="download(tarRecord,'ChecklistFilename')" v-if="!strEmpty(tarRecord.ChecklistFilename)" role="button" class="btn btn-shadow btn-block btn-color11-1"><i class="fas fa-download"></i></button>
            </div>
        </div>
        <div v-if="tarRecord.ToDoChecklit < 3" class="form-group row">
            <label class="col-md-12 col-form-label">選是:上傳「生態檢核資訊公開內容」檔案</label>
            <div class="col-md-12">
                <div class="table-responsive">
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
                                <td><b-form-file multiple v-model="SelfEvalFilename" v-bind:placeholder="filePlaceholder(tarRecord.SelfEvalFilename)"></b-form-file></td>
                                <!-- <td><button @click="download(tarRecord,'SelfEvalFilename')" v-if="!strEmpty(tarRecord.SelfEvalFilename)" role="button" class="btn btn-shadow btn-block btn-color11-1"><i class="fas fa-download"></i></button></td> -->
                                <td><button class="btn btn-shadow btn-block btn-color11-1" @click="openDownloadList()"> <i class="fas fa-download"></i></button> </td>
                            </tr>
                            <tr>
                                <td class="text-left"><strong>附表D-01 工程生態背景資料表</strong></td>
                                <td><b-form-file v-model="DataCollectDocFilename" v-bind:placeholder="filePlaceholder(tarRecord.DataCollectDocFilename)"></b-form-file></td>
                                <td><button @click="download(tarRecord,'DataCollectDocFilename')" v-if="!strEmpty(tarRecord.DataCollectDocFilename)" role="button" class="btn btn-shadow btn-block btn-color11-1"><i class="fas fa-download"></i></button></td>
                            </tr>
                            <tr>
                                <td class="text-left"><strong>附表D-02 現場勘查/會議紀錄表</strong></td>
                                <td><b-form-file v-model="MemberDocFilename" v-bind:placeholder="filePlaceholder(tarRecord.MemberDocFilename)"></b-form-file></td>
                                <td><button @click="download(tarRecord,'MemberDocFilename')" v-if="!strEmpty(tarRecord.MemberDocFilename)" role="button" class="btn btn-shadow btn-block btn-color11-1"><i class="fas fa-download"></i></button></td>
                            </tr>
                            <tr>
                                <td class="text-left"><strong>附表D-03 生態調查評析表</strong></td>
                                <td><b-form-file v-model="SOCFilename" v-bind:placeholder="filePlaceholder(tarRecord.SOCFilename)"></b-form-file></td>
                                <td><button @click="download(tarRecord,'SOCFilename')" v-if="!strEmpty(tarRecord.SOCFilename)" role="button" class="btn btn-shadow btn-block btn-color11-1"><i class="fas fa-download"></i></button></td>
                            </tr>
                            <tr>
                                <td class="text-left"><strong>附表 D-04 民眾參與紀錄表</strong></td>
                                <td><b-form-file v-model="PlanDesignRecordFilename" v-bind:placeholder="filePlaceholder(tarRecord.PlanDesignRecordFilename)"></b-form-file></td>
                                <td><button @click="download(tarRecord,'PlanDesignRecordFilename')" v-if="!strEmpty(tarRecord.PlanDesignRecordFilename)" role="button" class="btn btn-shadow btn-block btn-color11-1"><i class="fas fa-download"></i></button></td>
                            </tr>


                            <tr>
                                <td class="text-left"><strong>附表D-05 生態保育措施研擬紀錄表</strong></td>
                                <td><b-form-file v-model="ConservMeasFilename" v-bind:placeholder="filePlaceholder(tarRecord.ConservMeasFilename)"></b-form-file></td>
                                <td><button @click="download(tarRecord,'ConservMeasFilename')" v-if="!strEmpty(tarRecord.ConservMeasFilename)" role="button" class="btn btn-shadow btn-block btn-color11-1"><i class="fas fa-download"></i></button></td>
                            </tr>
                            <tr>
                                <td class="text-left"><strong>附表D-06 工程範圍生物名錄</strong></td>
                                <td><b-form-file v-model="EngCreatureNameList" v-bind:placeholder="filePlaceholder(tarRecord.EngCreatureNameList)"></b-form-file></td>
                                <td><button @click="download(tarRecord,'EngCreatureNameList')" v-if="!strEmpty(tarRecord.EngCreatureNameList)" role="button" class="btn btn-shadow btn-block btn-color11-1"><i class="fas fa-download"></i></button></td>
                            </tr>

                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        <div class="card-footer">
            <div class="row justify-content-center">
                <!-- div class="col-12 col-sm-4 col-xl-2 my-2">
                    <button role="button" class="btn btn-shadow btn-color3 btn-block"> 取消修改 </button>
                </div -->
                <div class="col-12 col-sm-4 col-xl-2 my-2">
                    <button @click="onSaveRecord" role="button" class="btn btn-shadow btn-block btn-color11-2"> 儲存 <i class="fas fa-save"></i></button>
                </div>
            </div>
        </div>
        <downloadList route="EQMEcologicalCheck" :seq="targetId" ref="downloadList" > </downloadList>
    </div>
</template>
<script>

    import downloadList from "../../components/downloadList.vue";
    export default {
        components :{
            downloadList : downloadList
        },
        data: function () {
            return {
                targetId: null,
                tenderItem: {},
                tarRecord: {},
                //否
                ChecklistFilename: null,
                //是
                SelfEvalFilename: [],
                SelfEvalFileName: "",
                PlanDesignRecordFilename: null,
                MemberDocFilename: null,
                DataCollectDocFilename: null,
                ConservMeasFilename: null,
                SOCFilename:null,
            };
        },
        methods: {
            openDownloadList()
            {
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
                console.log(this.SelfEvalFilename);
                var uploadfiles = new FormData();
                uploadfiles.append("seq", this.tarRecord.Seq);
                uploadfiles.append("toDoChecklit", this.tarRecord.ToDoChecklit);
                if (this.ChecklistFilename != null) uploadfiles.append("ChecklistFilename", this.ChecklistFilename, this.ChecklistFilename.name);
                if (this.SelfEvalFilename != null) {
                    this.SelfEvalFilename.forEach( (element, index) => {
                        uploadfiles.append(`SelfEvalFilename`, element);
                    });

                }
                if (this.PlanDesignRecordFilename != null) uploadfiles.append("PlanDesignRecordFilename", this.PlanDesignRecordFilename, this.PlanDesignRecordFilename.name);
                if (this.MemberDocFilename != null) uploadfiles.append("MemberDocFilename", this.MemberDocFilename, this.MemberDocFilename.name);
                if (this.DataCollectDocFilename != null) uploadfiles.append("DataCollectDocFilename", this.DataCollectDocFilename, this.DataCollectDocFilename.name);
                if (this.ConservMeasFilename != null) uploadfiles.append("ConservMeasFilename", this.ConservMeasFilename, this.ConservMeasFilename.name);
                if (this.SOCFilename != null) uploadfiles.append("SOCFilename", this.SOCFilename, this.SOCFilename.name);
                if (this.EngCreatureNameList != null) uploadfiles.append("EngCreatureNameList", this.EngCreatureNameList, this.EngCreatureNameList.name);
                this.upload(uploadfiles);

            },
            //上傳
            upload(uploadfiles) {
                window.myAjax.post('/EQMEcologicalCheck/UpdateRecord', uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            this.$refs.downloadList.getdownloaditem();
                            this.getResords();
                        }
                        alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
            //下載
            download(item, key) {
                window.myAjax.get('/EQMEcologicalCheck/Download?id=' + item.Seq+'&key='+key, { responseType: 'blob' })
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
                window.myAjax.post('/EQMEcologicalCheck/GetList', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.tarRecord = resp.data.item;
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
                window.myAjax.post('/EQMEcologicalCheck/GetEngMain', { id: this.targetId })
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
            console.log('mounted() 生態檢核(設計階段)上傳');
            this.targetId = window.sessionStorage.getItem(window.eqSelTrenderPlanSeq);
            this.getItem();
        }
    }
</script>
