<template>
    <div>
        <div v-if="items.length==0" class="table-responsive mb-3">
            <table class="table table1 onepage my-0" border="0">
                <thead>
                    <tr>
                        <th>年度</th>
                        <th>機關/單位</th>
                        <th>匯入碳排量</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>
                            <input v-model.number="engYear" type="number" class="form-control" placeholder="輸入年分" />
                        </td>
                        <td class="text-center">
                            <select v-model="engUnitSeq" class="form-control">
                                <option v-for="(item, index) in unitOptions" v-bind:key="item.Value" :value="item.Value">{{item.Text}}</option>
                            </select>
                        </td>
                        <td style=" display: flex; align-items: center;">
                            <div class="col-8 custom-file b-form-file" id="__BVID__15__BV_file_outer_" style="width: auto;">
                                <input v-on:change="fileChange($event)" type="file" class="custom-file-input" id="__BVID__15" style="z-index: -5;">
                                <label data-browse="選取" class="custom-file-label" for="__BVID__15" style="justify-content: flex-start;">
                                    <span class="d-block form-file-text" style="pointer-events: none;">
                                        {{this.uploadFile == null ? '未選擇任何檔案' :this.uploadFile[0].name}}
                                    </span>
                                </label>
                            </div>
                            <div class="col-4">
                                <button v-if="this.uploadFile != null" @click="upload" type="button" style="color: #fff;background-color: #6c757d;border-color: #6c757d; border-radius: 5px;">匯入</button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div v-if="items.length>0" style="margin: 0px auto; width: 90%;">
            <div style="text-align: center;">
                <table class="table table1">
                    <thead>
                        <tr>
                            <th>工程編號</th>
                            <th>需求碳排量</th>
                            <th>核定碳排量</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in items" v-bind:key="index">
                            <td>{{item.EngNo}}</td>
                            <td>{{item.CarbonDemandQuantity}}</td>
                            <td>{{item.ApprovedCarbonQuantity}}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="form-row justify-content-center" role="toolbar">
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                    <button @click="UpdateImport" type="button" class="btn btn-color11-2 btn-sm" title="確認匯入"><i class="fas fa-check"></i>&nbsp;確認匯入</button>
                </div>
                <div class="bcol-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                    <button @click="items=[]" type="button" class="btn btn-color9-1 btn-sm" title="取消匯入"><i class="fas fa-times"></i>&nbsp;取消匯入</button>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                items: [],
                engYear:null,
                engUnitSeq: -1,
                uploadFile: null,
                unitOptions: [],
            };
        },
        methods: {
            //匯入 excel
            fileChange(event) {
                this.items = [];
                var files = event.target.files || event.dataTransfer.files;
                // 預防檔案為空檔
                if (!files.length) return;

                if (!files[0].type.match('application/vnd.openxmlformats-officedocument.spreadsheetml.sheet')) {// && !files[0].type.match('application/vnd.ms-excel') ) {
                    alert('請選擇 .xlsx Excel檔案');
                    return;
                }
                this.uploadFile = files;
            },
            upload() {
                if (!window.comm.isNumber(this.engYear)) {
                    alert('必須輸入年分');
                    return;
                }
                if (this.engYear == 0) {
                    alert('年分 必須大於0');
                    return;
                }
                if (window.comm.stringEmpty(this.engUnitSeq)) {
                    alert('請選取單位');
                    return;
                }
                this.items = [];
                var uploadfiles = new FormData();
                uploadfiles.append("year", this.engYear);
                uploadfiles.append("uId", this.engUnitSeq);
                uploadfiles.append("file", this.uploadFile[0], this.uploadFile[0].name);
                window.myAjax.post('/CarbonEmissionSetting/Upload', uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                            this.uploadFile = null;
                        } else
                            alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
            UpdateImport() {
                window.myAjax.post('/CarbonEmissionSetting/UpdateImport', { items: this.items }
                    ).then(resp => {
                        if (resp.data.result == 0) {
                            this.items = [];
                        }
                        alert(resp.data.msg);
                    }).catch(error => {
                        console.log(error);
                    });
            },
            //單位清單 s20230502
            getUnitOption() {
                this.unitOptions = [];
                window.myAjax.post('/CarbonEmissionSetting/GetUnitList'
                ).then(resp => {
                    if (resp.data.result == 0) {
                        this.unitOptions = resp.data.items;
                    }
                }).catch(error => {
                    console.log(error);
                });
            }
        },
        mounted() {
            console.log('mounted 碳排量匯入');
            this.getUnitOption();
        }
    }
</script>