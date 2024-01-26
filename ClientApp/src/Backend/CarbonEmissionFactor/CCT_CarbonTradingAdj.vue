<template>    
    <div>
        <h5 class="insearch mt-0 py-2" style="padding: 10px;">
            工程名稱：{{adjItem.EngName}}<br>
            核定碳排量：{{adjItem.ApprovedCarbonQuantity}}   設計碳排量：{{adjItem.CarbonDesignQuantity}}
        </h5>
        <h5>碳交易案件清單</h5>
        <div>
            <div class="table-responsive" style="padding-top: 20px;">
                <table class="table table1">
                    <thead class="insearch">
                        <tr>
                            <th>案件編號</th>
                            <th>名稱</th>
                            <th>執行單位</th>
                            <th>核定碳排量</th>
                            <th>設計碳排量</th>
                            <th>可交易量</th>
                            <th>交易碳排量</th>
                            <th>功能<a v-if="ceTradeHeader.State == 1" v-on:click.stop="onAddItem" href="##" class="btn btn-color11-3 btn-xs sharp mx-1" title="新增"><i class="fas fa-plus"></i></a>
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-if="fAddItem">
                            <td>
                                <select v-model="selectTradeEng" @change="onSelectTradeEngChange" class="form-control">
                                    <option v-for="option in tradeEngItems" v-bind:value="option.Value" v-bind:key="option.Value">
                                        {{ option.Text }}
                                    </option>
                                </select>
                            </td>
                            <template v-if="newItem != null">
                                <td>{{newItem.EngName}}</td>
                                <td>{{newItem.ExecUnitName}}</td>
                                <td>{{newItem.ApprovedCarbonQuantity}}</td>
                                <td>{{newItem.CarbonDesignQuantity}}</td>
                                <td>{{newItem.ApprovedCarbonQuantity-newItem.CarbonDesignQuantity-newItem.TradingTotalQuantity}}</td>
                                <td><input v-model.number="newItem.Quantity" type="number" class="form-control"></td>
                            </template>
                            <template v-if="newItem == null">
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </template>
                            <td style="min-width: unset;">
                                <div class="d-flex justify-content-center">
                                    <button v-if="newItem != null" @click="onUpdateTrade(newItem)" class="btn btn-outline-secondary btn-xs sharp m-1" title="新增"><i class="fas fa-save"></i></button>
                                    <button @click="fAddItem=false" class="btn btn-outline-secondary btn-xs sharp m-1" title="取消"><i class="fas fa-times"></i></button>
                                </div>
                            </td>
                        </tr>
                        <tr v-for="(item, index) in tradeItems" v-bind:key="index">
                            <td class="text-left">{{item.EngNo}}</td>
                            <td>{{item.EngName}}</td>
                            <td>{{item.ExecUnitName}}</td>
                            <td>{{item.ApprovedCarbonQuantity}}</td>
                            <td>{{item.CarbonDesignQuantity}}</td>
                            <td>{{item.ApprovedCarbonQuantity-item.CarbonDesignQuantity-item.TradingTotalQuantity}}</td>
                            <template v-if="item.Seq != editSeq">
                                <td><input v-model.trim="item.Quantity" type="text" disabled class="form-control"></td>
                                <td style="min-width: unset;">
                                    <div v-if="ceTradeHeader.State == 1" class="d-flex justify-content-center">
                                        <button @click="onEditRecord(item)" class="btn btn-color11-1 btn-xs sharp m-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                        <button @click="onDelRecord(item)" class="btn btn-color9-1 btn-xs sharp m-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                                    </div>
                                </td>
                            </template>
                            <template v-if="item.Seq == editSeq">
                                <td><input v-model.trim="editRecord.Quantity" type="text" class="form-control"></td>
                                <td style="min-width: unset;">
                                    <div class="d-flex justify-content-center">
                                        <button @click="onUpdateTrade(editRecord)" class="btn btn-color11-2 btn-xs sharp m-1" title="儲存"><i class="fas fa-save"></i></button>
                                        <button @click="onEditCancel" class="btn btn-color9-1 btn-xs sharp m-1" title="取消"><i class="fas fa-times"></i></button>
                                    </div>
                                </td>
                            </template>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div v-if="ceTradeHeader.State == 1" class="row justify-content-center mt-5">
                <!-- div class="d-flex">
                    <button @click="onUpdateTrade" role="button" class="btn btn-color11-2 btn-xs mx-1">
                        <i class="fas fa-save">&nbsp;儲存</i>
                    </button>
                </div -->
                <div class="d-flex">
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
        props: ['adjItem'],
        data: function () {
            return {
                editSeq: -99,
                editRecord: {},
                newItem: null,
                fAddItem: false,
                tradeItems: [],
                ceTradeHeader: {},
                tradeEngItems: [],//可交易工程
                selectTradeEng: '',
                //------------------
                tradeQuantity: 0,
                tradeChange: false,
            };
        },
        methods: {
            //編輯交易工程
            onEditRecord(item) {
                if (this.editSeq > -99) return;
                this.editRecord = Object.assign({}, item);
                this.editSeq = this.editRecord.Seq;
            },
            //取消編輯
            onEditCancel() {
                this.editSeq = -99;
            },
            //儲存新碳交易工程
            onUpdateTrade(item) {
                if (!window.comm.isNumber(item.Quantity)) {
                    alert('交易碳排量, 不可輸入非數值資料');
                    return;
                }
                if (item.Quantity == 0) {
                    alert('交易碳排量, 必須>0');
                    return;
                }
                if (item.Quantity > (item.ApprovedCarbonQuantity - item.CarbonDesignQuantity - item.TradingTotalQuantity)) {
                    alert(item.EngNo + ' 數量超過可交易量');
                    return;
                }
                window.myAjax.post('/EQMCarbonEmission/UpdateTradeAdj', { id: this.adjItem.Seq, item: item })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.fAddItem = false;
                            this.newItem = null;
                            this.getTradeList();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //選取交易工程
            onSelectTradeEngChange() {
                this.newItem = null;
                window.myAjax.post('/EQMCarbonEmission/GetTradeEng', { eNo: this.selectTradeEng })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.newItem = resp.data.item;
                    } else
                        alert(resp.data.msg);
                })
                .catch(err => {
                    console.log(err);
                });
            },
            //新增交易
            onAddItem() {
                this.fAddItem = false;
                this.selectTradeEng = '';
                this.tradeEngItems = [];
                window.myAjax.post('/EQMCarbonEmission/GetTradeEngs', { id: this.adjItem.Seq})
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.tradeEngItems = resp.data.items;
                        }
                        if (this.tradeEngItems.length == 0) {
                            alert("無可交易工程");
                        } else {
                            this.fAddItem = true;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //刪除交易工程
            onDelRecord(item) {
                if (this.editSeq > -99) return;
                if (confirm('是否確定刪除資料？')) {
                    window.myAjax.post('/EQMCarbonEmission/DelTradeEng', { id: item.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getTradeList();
                            } else
                                alert(resp.data.msg);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            //確認交易完成
            onConfirmTrade() {
                let tradeQuantity = this.adjItem.CarbonDesignQuantity - this.adjItem.ApprovedCarbonQuantity;
                var total = 0;
                for (var i = 0; i < this.tradeItems.length; i++) {
                    var item = this.tradeItems[i];
                    if (item.Quantity > 0) total += item.Quantity;
                }
                if (tradeQuantity != total) {
                    alert("總交易量錯誤, 總和須為 " + tradeQuantity);
                    return;
                }
                if (confirm("是否確定完成調整?\n確定完成後就不能再變動!")) {
                    window.myAjax.post('/EQMCarbonEmission/ConfirmTradeAdj', { id: this.adjItem.Seq})
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.$emit('reload');
                            } else {
                                this.getTradeList();
                                alert(resp.data.msg);
                            }
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            //可碳交易工程清單
            getTradeList() {
                this.getTradeList1();
            },
            getTradeList1() {
                this.fAddItem = false;
                this.newItem = null;
                this.editSeq = -99;
                this.tradeItems = [];
                window.myAjax.post('/EQMCarbonEmission/GetTradeAdjList', { id: this.adjItem.Seq } )
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.tradeItems = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            getCEHeader() {
                this.ceTradeHeader = {};
                window.myAjax.post('/EQMCarbonEmission/GetCETradeHeader', { id: this.adjItem.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.ceTradeHeader = resp.data.item;
                            this.getTradeList();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        async mounted() {
            console.log('mounted() 碳交易調整');
            this.getCEHeader();
        },
    }
</script>
