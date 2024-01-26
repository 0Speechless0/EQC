<template>
    <div>
        <div class="row d-flex bd-highlight mb-3" style="padding-left:10px">
            <div class="d-flex pl-3 pb-2">
                <input v-model.trim="searchStr" />
                <button type="button" class="btn btn-outline-success" style="color: #fff;background-color: #6c757d;" @click="search()">查詢</button>
            </div>
            <div class="align-self-center ml-2" style="color:red">
                可輸入名稱及規格
            </div>
        </div>
        <div class="row justify-content-between">
            <comm-pagination class="col-12" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        </div>

        <div class="table-responsive">
            <table class="table table-responsive-md table-hover VA-middle">
                <thead class="insearch">
                    <tr>
                        <th>ID</th>
                        <th>類別</th>
                        <th>名稱及規格</th>
                        <th>機具能源耗用率</th>
                        <th>單位</th>
                        <th>燃料類別</th>
                        <th>燃料碳排放係數</th>
                        <th>單位</th>
                        <th>單位碳排量</th>
                        <th>單位</th>
                        <th>備註(參考來源)</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td style="min-width: unset;">{{item.Seq}}</td>
                        <td>{{item.Kind}}</td>
                        <td>{{item.NameSpec}}</td>
                        <td>{{item.ConsumptionRate}}</td>
                        <td>{{item.ConsumptionRateUnit}}</td>
                        <td>{{item.FuelKind}}</td>
                        <td>{{item.FuelKgCo2e}}</td>
                        <td>{{item.FuelUnit}}</td>
                        <td>{{item.KgCo2e}}</td>
                        <td>{{item.Unit}}</td>
                        <td>{{item.Memo}}</td>
                        <td style="min-width: unset;">
                            <div class="d-flex justify-content-center">
                                <a v-on:click="onNewRecord(item)" href="##" class="btn btn-color11-3 btn-xs sharp mx-1" title="新增"><i class="fas fa-plus"></i></a>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="modal fade" v-bind:class="{'show':modelShow}" v-bind:style="{display: modelShow ? 'block' : 'none'}" id="case_01">
            <div class="modal-dialog modal-lg modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header bg-0 text-white">
                        名稱及規格：{{selItem.NameSpec}}<br>單位碳排量：{{selItem.KgCo2e}}<br>單位：{{selItem.Unit}}
                    </div>
                    <div class="modal-body">
                        <div class="row">
                            <div class="col"></div>
                            <div class="col">數量:</div>
                            <div class="col"></div>
                        </div>
                        <div class="row">
                            <div class="col"></div>
                            <div class="col"><input ref="iAmount" v-model.number="addAmount" @focus="$event.target.select()" type="number" class="form-control"></div>
                            <div class="col"></div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button @click="onAddRecord" type="button" class="btn btn-color3">儲存</button>
                        <button @click="modelShow=false" type="button" class="btn btn-color3">關閉</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                addAmount: 0,
                modelShow: false,
                items: [],
                selItem: {},
                //分頁
                recordTotal: 0,
                pageIndex: 1,
                pageRecordCount: 30,
                searchStr: '' //s20230318
            };
        },
        methods: {
            search() {//s20230318
                this.getResords();
            },
            onAddRecord() {
                if (!window.comm.isNumber(this.addAmount)) {
                    alert('必須輸入正確數值資料');
                    this.$refs.iAmount.focus();
                    return;
                }
                if (this.addAmount == 0) {
                    alert('數量 必須大於0');
                    return;
                }
                this.$emit('addSubItem', this.selItem, this.addAmount, '機-' + this.selItem.Seq);
                this.modelShow = false;
            },
            onNewRecord(item) {
                this.selItem = item;
                this.modelShow = true;
                this.$nextTick(() => this.$refs.iAmount.focus());
            },
            //紀錄清單
            getResords() {
                this.items = [];
                window.myAjax.post('/MMFMaintenance/GetMachineRecords', {
                    pageRecordCount: this.pageRecordCount,
                    pageIndex: this.pageIndex,
                    keyWord: this.searchStr
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
            //分頁
            onPaginationChange(pInx, pCount) {
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
                this.getResords();
            }
        },
        mounted() {
            console.log('mounted() 機具-維護功能');
            this.getResords();
        }
    }
</script>