<template>
    <div>
        <div class="row d-flex bd-highlight mb-3" >
            <label class="btn btn-shadow btn-color11-3">
                <input v-on:change="fileChange($event)" id="inputFile" type="file" name="file" multiple="" style="display:none;">
                <i class="fas fa-upload"></i> 匯入Excel
            </label>
            <a :href="'ExcelTemplate/工程核定資料匯入.xlsx'"  class="bd-highlight" download  style="padding-left: 15px;"> 
                <button  role="button" class="btn btn-shadow btn-color11-3 btn-block">
                     下載範本
                </button>
            </a>
        </div>
        <div class="row justify-content-between">
            <comm-pagination class="col-12" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        </div>
        <div class="table-responsive">
            <table class="table table-responsive-md">
                <thead class="insearch">
                    <tr>
                        <th><strong>工程年度</strong></th>
                        <th><strong>工程編號</strong></th>
                        <th><strong>工程名稱</strong></th>
                        <th class="text-right"><strong>工程總預算(元)</strong></th>
                        <th class="text-right"><strong>發包預算(元)</strong></th>
                        <th class="text-right"><strong>需求碳排量(kgCO2e)</strong></th>
                        <th class="text-right"><strong>核定碳排量(kgCO2e)</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <!-- 專家01 -->
                    <tr v-for="(item) in items" v-bind:key="item.Seq">
                        <td>{{item.EngYear}}</td>
                        <td>{{item.EngNo}}<div style="color:red">{{checkEngNo(item)}}</div></td>
                        <td>{{item.EngName}}</td>
                        <td class="text-right">{{item.TotalBudget}}</td>
                        <td class="text-right">{{item.SubContractingBudget}}</td>
                        <td class="text-right">{{item.CarbonDemandQuantity}}</td>
                        <td class="text-right">{{item.ApprovedCarbonQuantity}}</td>
                    </tr>
                </tbody>
            </table>

        </div>


    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                recordTotal: 0,
                pageRecordCount : 30,
                pageIndex : 1,
                items: [],
            };
        },
        methods: {
            //s20231105
            checkEngNo(item) {
                if (item.engMainSeq == null && item.pccessMainSeq == null) {
                    return "";
                } else if (item.engMainSeq != null) {
                    return item.engExecUnit + " 工程編號已存在";
                } else {
                    return "PCCES 工程編號已存在";
                }
            },
            //分頁
            onPaginationChange(pInx, pCount) {
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
                this.getResords();
            },
            getList() {
                this.items = [];
                window.myAjax.post('/EngApprovalImport/GetList', {
                    pageRecordCount: this.pageRecordCount,
                    pageIndex: this.pageIndex
                })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                            this.recordTotal = resp.data.pTotal;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
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
                window.myAjax.post('/EngApprovalImport/Upload', uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            this.getList();
                        }
                        alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
        },
        async mounted() {
            console.log('mounted 工程核定資料匯入');
            this.getList();
        }
    }
</script>