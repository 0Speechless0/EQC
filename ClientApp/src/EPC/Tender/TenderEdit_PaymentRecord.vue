<template>
    <div id="menu10" class="tab-pane">
        <div class="row justify-content-between">
            <div class="form-inline col-12 col-md-8 small">
                <label class="my-1 mr-2">
                    <span class="text-danger">※ 貨幣單位為「新台幣千元」</span>
                </label>
            </div>
        </div>
        <div class="table-responsive">
            <table class="table table-responsive-md table-hover">
                <thead class="insearch">
                    <tr>
                        <th rowspan="2"><strong>次別</strong></th>
                        <th rowspan="2"><strong>應付總金額</strong></th>
                        <th><strong>請付日期</strong></th>
                        <th><strong>預定付款日期</strong></th>
                        <th><strong>實際付款日期</strong></th>
                        <th rowspan="2" style="width: 93px;"><strong>累計已付總金額</strong></th>
                        <th rowspan="2" style="min-width: 48px;"><strong>備註</strong></th>
                    </tr>
                    <tr>
                        <th><strong>請付金額</strong></th>
                        <th><strong>預定付款金額</strong></th>
                        <th><strong>實際付款金額</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <template v-for="(item, index) in engMain.PaymentRecord" >
                        <tr v-bind:key="item.Seq">
                            <td rowspan="2"><strong>{{item.PRItemNo}}</strong></td>
                            <td rowspan="2"><input v-model="item.PRTotalAmountPayable" type="text" class="form-control"></td>
                            <td><input v-model="item.PRPayDate" type="text" class="form-control"></td>
                            <td><input v-model="item.PRSchePayDate" type="text" class="form-control"></td>
                            <td><input v-model="item.PRActualPayDate" type="text" class="form-control"></td>
                            <td rowspan="2"><input v-model="item.PRAccuPayAmount" type="text" class="form-control"></td>
                            <td rowspan="2"><textarea v-model="item.PRMemo" rows="3" class="form-control"></textarea></td>
                        </tr>
                        <tr v-bind:key="item.Seq * -1">
                            <td><input v-model="item.PRPayAmount" type="text" class="form-control"></td>
                            <td><input v-model="item.PRSchePayAmount" type="text" class="form-control"></td>
                            <td><input v-model="item.PRActualPayAmount" type="text" class="form-control"></td>
                        </tr>
                    </template>
                </tbody>
            </table>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['engMain'],
        data: function () {
            return {
                item: {},
            };
        },
        /*watch: {
            pageIndex: {
                handler: function (value) {
                    this.getList();
                }
            }
        },
        methods: {
        },*/
        mounted() {
            console.log('mounted() 歷次付款資料');
        }
    }
</script>