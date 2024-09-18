<template>    
    <div>
        <div v-if="requireDesc" class="form-row">
            <div class="col-12 ">
                <h5>設計超過核定量之碳量檢討及處置結果說明
                    <button @click="defaultDesc" role="button" class="btn btn-color11-2 btn-xs mx-1">預設說明</button>
                </h5>
                <label class="my-2 mx-2"></label>
                <textarea v-model.trim="ceTradeHeader.CarbonTradingDesc" :disabled="ceTradeHeader.State == 1" maxlength="500" class="form-control" style="min-width: 750px;min-height: 100px;"></textarea>
                <!--
                <textarea v-if="ceHeader.CarbonTradingNo != null" v-model="ceHeader.CarbonTradingDesc" disabled class="form-control" style="min-width: 750px;min-height: 100px;"></textarea>
                -->
            </div>
        </div>
        <h5>局內審議核定日期及文號(碳交易)：</h5>
        <div class="table-responsive">
            <table class="table table1">
                <thead class="insearch">
                    <tr>
                        <th style="width:160px"><strong>日期</strong></th>
                        <th><strong>文號</strong></th>
                        <th><strong>檔案上傳</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-if="ceTradeHeader.State != 1">
                        <td style="text-align: center;"><input v-model="editDoc.CarbonTradingApprovedDate" type="date" name="bday"></td>
                        <td><input v-model="editDoc.CarbonTradingNo" maxlength="100" type="text" class="form-control"></td>
                        <td>
                            <div class="custom-file b-form-file" id="__BVID__15__BV_file_outer_">
                                <input v-on:change="fileChange($event)" type="file" class="custom-file-input" id="__BVID__15" style="z-index: -5;">
                                <label data-browse="上傳" class="custom-file-label" for="__BVID__15" style="justify-content: flex-start;">
                                    <span class="d-block form-file-text" style="pointer-events: none;">未選擇任何檔案</span>
                                </label>
                            </div>
                        </td>

                    </tr>
                    <tr v-for="(item, index) in approveDocs" v-bind:key="item.Seq">
                        <td style="text-align: center;">{{item.CarbonTradingApprovedDateStr}}</td>
                        <td>{{item.CarbonTradingNo}}</td>
                        <td>
                            <button @click="dnCarbonTradeDoc(item)" class="btn btn-color11-1 btn-xs mx-1">
                                <i class="fas fa-download"></i>
                            </button>
                            <button @click="delTradeDec(item)" title="刪除" class="btn btn-color11-4 btn-xs mx-1"><i class="fas fa-trash-alt"></i> </button>
                            &nbsp;{{item.OriginFileName}}
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div >
            <div v-if="requireDesc" class="table-responsive" style="padding-top: 20px;">
                <table class="table table1">
                    <thead class="insearch">
                        <tr>
                            <th>案件編號</th>
                            <th>名稱</th>
                            <th>執行單位</th>
                            <th>核定碳排量</th>
                            <th>設計碳排量</th>
                            <th v-if="ceTradeHeader.State != 1">可交易量</th>
                            <th>交易碳排量</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in tradeItems" v-bind:key="index">
                            <td>{{item.EngNo}}</td>
                            <td>{{item.EngName}}</td>
                            <td>{{item.ExecUnitName}}</td>
                            <td>{{item.ApprovedCarbonQuantity}}</td>
                            <td>{{item.CarbonDesignQuantity}}</td>
                            <template v-if="ceTradeHeader.State != 1">
                                <td>{{item.ApprovedCarbonQuantity-item.CarbonDesignQuantity-item.TradingTotalQuantity}}</td>
                                <td><input v-show="item.Seq!=-2" v-model.trim="item.Quantity" type="text" @change="tradeChange=true" class="form-control"></td>
                            </template>
                            <template v-if="ceTradeHeader.State == 1">
                                <td>{{item.Quantity}}</td>
                            </template>

                        </tr>
                    </tbody>
                </table>
            </div>
            <div v-if="ceTradeHeader.State != 1" class="row justify-content-center mt-5">
                <div v-if="requireDesc" class="d-flex">
                    <button @click="onUpdateTrade" role="button" class="btn btn-color11-2 btn-xs mx-1">
                        <i class="fas fa-save">&nbsp;儲存</i>
                    </button>
                </div>
                <div v-if="!tradeChange" class="d-flex">
                    <button @click="onConfirmTrade" role="button" class="btn btn-color11-3 btn-xs mx-1">
                        <i class="fas fa-check">&nbsp;確認</i>
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['tenderItem', 'co2Total'],
        data: function () {
            return {
                tradeQuantity: 0,
                requireDesc:false,
                tradeItems: [],
                tradeChange: false,
                editDoc: { CarbonTradingNo: '', CarbonTradingApprovedDate: '' },
                ceTradeHeader: {},//s20230428
                approveDocs:[],//s20230428
            };
        },
        methods: {
            //確認交易完成
            onConfirmTrade() {
                if (this.approveDocs.length == 0) {//s20230429
                    alert("必須上傳核定文件");
                    return;
                }
                if (this.requireDesc) {
                    var total = 0;
                    for (var i = 0; i < this.tradeItems.length; i++) {
                        var item = this.tradeItems[i];
                        if (item.Quantity > 0) total += item.Quantity;
                    }
                    if ((this.tradeQuantity + total) != 0) {
                        alert("總交易量錯誤, 總和須為 " + this.tradeQuantity*-1);
                        return;
                    }
                }
                if (confirm("是否確定完成交易?\n確定完成後所有碳排量作業就不能再變動!")) {
                    window.myAjax.post('/EQMCarbonEmission/ConfirmTrade', { id: this.tenderItem.Seq})
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.$emit('reload');
                                this.getCEHeader();
                            } else {
                                this.getTradeList1();
                                alert(resp.data.msg);
                            }
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            //下載核定檔案
            dnCarbonTradeDoc(item) {
                window.comm.dnFile('/EQMCarbonEmission/dnCarbonTradeDoc?id=' + item.Seq + "&eId=" + this.tenderItem.Seq);
            },
            async delTradeDec(item)
            {
                let res = await window.myAjax.get('/EQMCarbonEmission/delCarbonTradeDoc?fileName=' + item.OriginFileName + "&eId=" + this.tenderItem.Seq);
                if(res.data == true) {
                    alert("刪除成功");
                    this.getApproveDocList();
                    }
            },
            //更新碳交易工程
            onUpdateTrade() {
                if (this.requireDesc && window.comm.stringEmpty(this.ceTradeHeader.CarbonTradingDesc)) {
                    alert('請輸入 超過核定量之碳量檢討及處置結果說明');
                    return;
                }
                var tItems = [];
                for (var i = 0; i < this.tradeItems.length; i++) {
                    var item = this.tradeItems[i];
                    if (item.Quantity == 0) item.Quantity = null;
                    if (item.Seq > -1 && (item.Quantity == null || window.comm.stringEmpty(item.Quantity))) {
                        item.Quantity = null;
                        tItems.push(item);
                    }
                    else if (item.Quantity != null && !window.comm.stringEmpty(item.Quantity)) {
                        if (!window.comm.isNumber(item.Quantity)) {
                            alert('交易碳排量, 不可輸入非數值資料');
                            return;
                        }
                        if ((item.Seq > -1) || (item.Seq == -1 && item.Quantity > 0)) {
                            if (item.Quantity > (item.ApprovedCarbonQuantity - item.CarbonDesignQuantity - item.TradingTotalQuantity)) {
                                alert(item.EngNo+' 數量超過可交易量');
                                return;
                            }
                            tItems.push(item);
                        }
                    }
                }
                window.myAjax.post('/EQMCarbonEmission/UpdateTrade', { id: this.tenderItem.Seq, items: tItems, desc: this.ceTradeHeader.CarbonTradingDesc })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getTradeList1();
                            this.tradeChange = false;
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //可碳交易工程清單
            getTradeList() {
                if (!this.requireDesc) return;
                this.getTradeList1();
            },
            getTradeList1() {
                this.tradeItems = [];
                window.myAjax.post('/EQMCarbonEmission/GetTradeList', { id: this.tenderItem.Seq } )
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.tradeItems = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //核定文件清單 s20230428
            getApproveDocList() {
                this.approveDocs = [];
                window.myAjax.post('/EQMCarbonEmission/GetApproveDocs', { id: this.ceTradeHeader.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.approveDocs = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //上傳核定文件
            fileChange(event) {
                if (window.comm.stringEmpty(this.editDoc.CarbonTradingNo) || window.comm.stringEmpty(this.editDoc.CarbonTradingApprovedDate)) {
                    alert('請輸入 日期,文號');
                    return;
                }
                
                var files = event.target.files || event.dataTransfer.files;
                // 預防檔案為空檔
                console.log(files[0].type);
                if (!files.length) return;
                if (!files[0].type.match('image/jpeg') && !files[0].type.match('application/pdf')) {
                    alert('請選擇 jpg, pdf 格式檔案');
                    return;
                }
                var uploadfiles = new FormData();
                uploadfiles.append("id", this.tenderItem.Seq);
                uploadfiles.append("docNo", this.editDoc.CarbonTradingNo);
                uploadfiles.append("docDate", this.editDoc.CarbonTradingApprovedDate);
                uploadfiles.append("file", files[0], files[0].name);
                this.upload(uploadfiles);
            },
            upload(uploadfiles) {
                window.myAjax.post('/EQMCarbonEmission/UploadTradeDoc', uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            this.$emit('reload');
                            this.getApproveDocList();
                        }
                        alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
            getCEHeader() {
                this.ceTradeHeader = {};
                window.myAjax.post('/EQMCarbonEmission/GetCETradeHeader', { id: this.tenderItem.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.ceTradeHeader = resp.data.item;
                            this.setInitDesc();
                            this.getApproveDocList();
                            this.getTradeList();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            setInitDesc() {
                if (!this.requireDesc) return;
                if (window.comm.stringEmpty(this.ceTradeHeader.CarbonTradingDesc)) this.defaultDesc();
            },
            //s20230528 預設內文
            defaultDesc() {
                this.ceTradeHeader.CarbonTradingDesc = "本案件" + this.tenderItem.EngName + "(" + this.tenderItem.EngNo
                    + ") 核定設碳排量" + this.tenderItem.ApprovedCarbonQuantity + "、設計碳排量" + this.co2Total + "，不足" + this.tradeQuantity * -1
                    + "，已完成碳交易，符合本案設計碳排量" + this.co2Total + "。";
                let sp = '\n';
                for (var i = 0; i < this.tradeItems.length; i++) {
                    let item = this.tradeItems[i];
                    if (item.Quantity > 0) {
                        this.ceTradeHeader.CarbonTradingDesc += sp + '工程:' + item.EngNo + ' ' + item.EngName + ' 交易碳排量: ' + item.Quantity + ' kgCO2e ';
                    }
                }
            }
        },
        async mounted() {
            console.log('mounted() 碳交易');
            this.tradeQuantity = this.tenderItem.ApprovedCarbonQuantity - this.co2Total;
            this.requireDesc = (this.tradeQuantity < 0);
            this.getCEHeader();
        },
    }
</script>
