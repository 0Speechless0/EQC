<template>
    <div class="tab-content">
        <h5>
            工程總價：{{tenderItem.SubContractingBudget}}元，綠色經費：{{greenFunding}}元，綠色經費占比：{{greenFundingRate}}%<br />
            核定碳排量：{{tenderItem.ApprovedCarbonQuantity}}&nbsp;kgCO2e，設計排放量：{{co2TotalDesign}}&nbsp;kgCO2e，施工碳排量：{{co2Total}}&nbsp;kgCO2e，可拆解率：{{dismantlingRate}}%
        </h5>
        <div>
            <form class="form-group insearch mb-3">
                <div class="form-row">
                    <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                        <select v-model="selectLevel" @change="onLevelChange" class="form-control">
                            <option v-bind:key="index" v-for="(item,index) in selectitems" v-bind:value="item.Value">{{item.Value=='' ? item.Text : item.Value+':'+item.Text}}</option>
                        </select>
                    </div>
                    <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                        <select v-model="selectLevel2" class="form-control">
                            <option v-bind:key="index" v-for="(item,index) in selectitems2" v-bind:value="item.Value">{{item.Value=='' ? item.Text : item.Value}}</option>
                        </select>
                    </div>
                    <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                        <input v-model="keyword" type="text" class="form-control">
                    </div>
                    <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                        <button @click="onSearch" type="button" class="btn btn-outline-secondary btn-sm"><i class="fas fa-search"></i></button>
                    </div>
                </div>
            </form>

            <div v-if="sepHeader.SPState==0" style="display: flex;">
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                    <label class="btn btn-block btn-color11-2 btn-sm">
                        <input v-on:change="fileChange($event)" id="inputFile" type="file" name="file" multiple="" style="display:none;">
                        <i class="fas fa-upload"></i> 匯入PCCES
                    </label>
                </div>
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                    <button @click.stop="fillCompleted" type="button" title="填報完成" class="btn btn-outline-secondary btn-sm" :disabled="sepHeader.SPState != 0">
                        填報完成&nbsp;<i class="fas fa-check"></i>
                    </button>
                </div>
            </div>

            <comm-pagination ref="pagination" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>

            <div class="table-responsive tableFixHead ">
                <table class="table table-responsive-md table-hover VA-middle">
                    <thead class="insearch">
                        <tr>
                            <th><strong>序號</strong></th>
                            <th><strong>項次</strong></th>
                            <th><strong>施工項目</strong></th>
                            <th><strong>編碼</strong></th>
                            <th class="text-right"><strong>碳排係數(kgCO2e)</strong></th>
                            <th><strong>單位</strong></th>
                            <th class="text-right"><strong>契約數量</strong></th>
                            <th class="text-right"><strong>單價(元)</strong></th>
                            <th class="text-right"><strong>金額(元)</strong></th>
                            <th v-if="sepHeader.SPState==0" style="text-align: center;width: 10%;">
                                <strong>功能</strong>
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in items" v-bind:key="item.Seq" v-bind:class="{'GreenFunding': item.GreenFundingSeq!=null, 'bg-1-30':item.GreenFundingSeq==null }">
                            <td><strong>{{pageRecordCount*(pageIndex-1)+index+1}}</strong></td>
                            <td><strong>{{item.PayItem}}</strong></td>
                            <td><strong>{{item.Description}}</strong></td>
                            <td><strong>{{item.Memo}}</strong></td>
                            <td class="text-right"><strong>{{item.KgCo2e}}</strong></td>
                            <template v-if="item.Seq != editSeq">
                                <td><input v-model="item.Unit" disabled type="text" class="form-control"></td>
                                <td><input v-model="item.Quantity" disabled type="text" class="form-control text-right"></td>
                                <td><input v-model="item.Price" disabled type="text" class="form-control text-right"></td>
                                <td class="text-right">{{item.Amount}}</td>
                                <td v-if="sepHeader.SPState==0" style="text-align: center;">
                                    <div class="d-flex justify-content-center">
                                        <button @click="onEditRecord(item)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                        <button @click="onDelRecord(item)" class="btn btn-color11-4 btn-xs sharp mx-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                                    </div>
                                </td>
                            </template>
                            <template v-if="item.Seq == editSeq">
                                <td><input v-model.trim="editRecord.Unit" maxlength="10" type="text" class="form-control"></td>
                                <td><input v-model.number="editRecord.Quantity" type="number" class="form-control text-right"></td>
                                <td><input v-model.number="editRecord.Price" type="number" class="form-control text-right"></td>
                                <td></td>
                                <td style="text-align: center;">
                                    <div class="d-flex justify-content-center">
                                        <button @click="onSaveRecord" class="btn btn-color11-2 btn-xs sharp mx-1" title="儲存"><i class="fas fa-save"></i></button>
                                        <button @click="editSeq = -99" class="btn btn-color9-1 btn-xs sharp mx-1" title="取消"><i class="fas fa-times"></i></button>
                                    </div>
                                </td>
                            </template>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['tenderItem','sepHeader'],
        data: function () {
            return {
                targetId: null,
                selectitems: [],
                selectLevel: '',
                selectitems2: [],
                selectLevel2: '',
                keyword: '',
                co2Total: null,
                dismantlingRate: null,//可拆解率
                //s20230528
                co2TotalDesign: null, //設計排放量
                approvedCarbonQuantity: null,
                greenFunding: null,
                greenFundingRate: null,

                items: [],
                editSeq: -99,
                editRecord: {},
                //分頁
                recordTotal: 0,
                pageRecordCount: 30,
                pageIndex: 0,
            };
        },
        methods: {
            //匯入PCCES 20230417
            fileChange(event) {
                var files = event.target.files || event.dataTransfer.files;
                // 預防檔案為空檔
                if (!files.length) return;
                if (!files[0].type.match('text/xml')) {
                    alert('請選擇 XML 檔案');
                    return;
                }
                var uploadfiles = new FormData();
                uploadfiles.append("id", this.targetId);
                uploadfiles.append("file", files[0], files[0].name);
                this.upload(uploadfiles);
            },
            upload(uploadfiles) {
                window.myAjax.post('/EPCSchEngProgress/UploadXML', uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            this.$emit('reload');
                            this.getItem();
                        }
                        alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
            //填報完成
            fillCompleted(item) {
                if (confirm('填報完成後將不能再修改\n是否確定? ')) {
                    window.myAjax.post('/EPCSchEngProgress/FillCompleted', { id: this.sepHeader.EngMainSeq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.$emit('reload');
                            }
                            alert(resp.data.msg);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            //刪除
            onDelRecord(item) {
                if (confirm('是否確定刪除資料？')) {
                    window.myAjax.post('/EPCSchEngProgress/DelRecord', { id: item.Seq })
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
            onPaginationChange(pInx, pCount) {
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
                this.getResords();
            },
            strEmpty(str) {
                return window.comm.stringEmpty(str);
            },
            //儲存
            onSaveRecord() {
                if ((this.strEmpty(this.editRecord.Quantity) || this.strEmpty(this.editRecord.Price))) {
                    alert('數量,單價 必須輸入且不得為0');
                    return;
                }
                window.myAjax.post('/EPCSchEngProgress/UpdateRecord', { m: this.editRecord })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getResords();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //編輯紀錄
            onEditRecord(item) {
                if (this.editSeq > -99) return;
                this.editRecord = Object.assign({}, item);
                this.editSeq = this.editRecord.Seq;
            },
            onSearch() {
                this.pageIndex = 1;
                this.getResords();
            },
            //清單
            getResords() {
                var fLevel = this.selectLevel;
                if (this.selectLevel2 != '') fLevel = this.selectLevel2;
                this.editSeq = -99;
                this.items = [];
                window.myAjax.post('/EPCSchEngProgress/GetList', {
                    id: this.targetId,
                    pageIndex: this.pageIndex,
                    perPage: this.pageRecordCount,
                    fLevel: fLevel,
                    keyword: this.keyword
                })
                .then(resp => {
                    if (resp.data.result >= 0) {
                        this.items = resp.data.items;
                        this.recordTotal = resp.data.totalRows;
                        this.co2Total = resp.data.co2Total;
                        this.co2TotalDesign = resp.data.co2TotalDesign;
                        this.dismantlingRate = resp.data.dismantlingRate;
                        this.greenFunding = resp.data.greenFunding;
                        this.greenFundingRate = resp.data.greenFundingRate;
                        if (this.selectitems.length == 0) this.getLevel1();
                        if (resp.data.result == 1) this.$emit('reload');
                    } else
                        alert(resp.data.msg);
                })
                .catch(err => {
                    console.log(err);
                });
            },
            //第一階
            getLevel1() {
                this.selectLevel = '';
                this.selectitems = [];
                this.selectLevel2 = '';
                this.selectitems2 = [];
                window.myAjax.post('/EPCSchEngProgress/GetLevel1List', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.selectitems = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //第二階
            onLevelChange() {
                this.selectLevel2 = '';
                this.selectitems2 = [];
                if (this.selectLevel != '') {
                    window.myAjax.post('/EPCSchEngProgress/GetLevel2List', { id: this.targetId, key: this.selectLevel })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.selectitems2 = resp.data.items;
                            }
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
                //this.getResords();
            },
            //取標案
            getItem() {
                if (this.targetId == null) {
                    alert('請先選取標案');
                    return;
                }
                this.getLevel1();
                this.onSearch();
            },
        },
        async mounted() {
            console.log('mounted() 前置作業-碳排清單');
            this.targetId = this.tenderItem.Seq;
            this.getItem();
        },
    }
</script>
<style scoped>
.tableFixHead          { overflow: auto; max-height: 500px;   }
table {
    border-collapse: separate;
    border-spacing: 0;
}
.table {
    margin : 0;
}
.tableFixHead thead  { position: sticky !important ; top: 0 !important ; z-index: 1 !important;     }
th {
    border : 0;
    border-bottom: #ddd solid 1px !important; 
    border-left : 0 !important;
    border-right:0 !important;
}
td {
    z-index: 0;
    position: relative;
}</style>
