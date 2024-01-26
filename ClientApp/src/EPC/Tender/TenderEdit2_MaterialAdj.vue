<template>
    <div class="tab-pane">
        <div class="form-group my-2">
            <h5>是否係依機關已訂約施工中工程，因應營建物價變動之物價調整補貼原則(下稱補貼原則)辦理契約變更者</h5>
            <div class="custom-control custom-radio">
                <input v-model="item.IsChangeContract" value="true" type="radio" id="customRadio1" class="custom-control-input">
                <label class="custom-control-label" for="customRadio1">是</label>
            </div>
            <div class="custom-control custom-radio">
                <input v-model="item.IsChangeContract" value="false" type="radio" id="customRadio2" class="custom-control-input">
                <label class="custom-control-label" for="customRadio2">否 (填否者係指依現行契約規定)</label>
            </div>
        </div>
        <div class="form-group my-2">
            <h5>依補貼原則辦理契約變更者，變更文件是否已檢附物價調整補貼款處理協議書</h5>
            <small class="text-danger">※ 上列勾否者免填</small>
            <div class="custom-control custom-radio">
                <input v-model="item.IsAppendAgreement" value="true" type="radio" id="customRadio3" class="custom-control-input">
                <label class="custom-control-label" for="customRadio3">是</label>
            </div>
            <div class="custom-control custom-radio">
                <input v-model="item.IsAppendAgreement" value="false" type="radio" id="customRadio4" class="custom-control-input">
                <label class="custom-control-label" for="customRadio4">否 (填否者不可申請)</label>
            </div>
        </div>
        <div class="form-group my-2">
            <h5>申請特別預算地方工程物價調整款，是否已符合97年度中央政府擴大公共建設投資計畫，特別預算地方工程物價調整款支用要點之規定</h5>
            <div class="custom-control custom-radio">
                <input v-model="item.IsForYear97" value="true" type="radio" id="customRadio5" class="custom-control-input">
                <label class="custom-control-label" for="customRadio5">是</label>
            </div>
            <div class="custom-control custom-radio">
                <input v-model="item.IsForYear97" value="false" type="radio" id="customRadio6" class="custom-control-input">
                <label class="custom-control-label" for="customRadio6">否 (填否者不可申請)</label>
            </div>
        </div>
        <div class="form-group my-2">
            <h5>申請特別預算地方工程物價調整款之工程款於97年施作月份</h5>
            <div>
                <small class="text-danger">※ 依補貼原則辦理者，1月份不可申請</small>
            </div>
            <div>
                <small class="text-danger">※ 公營事業或受補助之法人、團體不可申請</small>
            </div>
            <div class="mb-3">
                <small class="text-danger">※ 依補貼原則辦理者，可歸責於廠商之逾期，不可申請</small>
            </div>
            <div class="custom-control custom-checkbox custom-control-inline">
                <input v-model="item.ForMade97MonthArray" value="01" type="checkbox" id="month01" class="custom-control-input">
                <label for="month01" class="custom-control-label">1月</label>
            </div>
            <div class="custom-control custom-checkbox custom-control-inline">
                <input v-model="item.ForMade97MonthArray" value="02" type="checkbox" id="month02" class="custom-control-input">
                <label for="month02" class="custom-control-label">2月</label>
            </div>
            <div class="custom-control custom-checkbox custom-control-inline">
                <input v-model="item.ForMade97MonthArray" value="03" type="checkbox" id="month03" class="custom-control-input">
                <label for="month03" class="custom-control-label">3月</label>
            </div>
            <div class="custom-control custom-checkbox custom-control-inline">
                <input v-model="item.ForMade97MonthArray" value="04" type="checkbox" id="month04" class="custom-control-input">
                <label for="month04" class="custom-control-label">4月</label>
            </div>
            <div class="custom-control custom-checkbox custom-control-inline">
                <input v-model="item.ForMade97MonthArray" value="05" type="checkbox" id="month05" class="custom-control-input">
                <label for="month05" class="custom-control-label">5月</label>
            </div>
            <div class="custom-control custom-checkbox custom-control-inline">
                <input v-model="item.ForMade97MonthArray" value="06" type="checkbox" id="month06" class="custom-control-input">
                <label for="month06" class="custom-control-label">6月</label>
            </div>
            <div class="custom-control custom-checkbox custom-control-inline">
                <input v-model="item.ForMade97MonthArray" value="07" type="checkbox" id="month07" class="custom-control-input">
                <label for="month07" class="custom-control-label">7月</label>
            </div>
            <div class="custom-control custom-checkbox custom-control-inline">
                <input v-model="item.ForMade97MonthArray" value="08" type="checkbox" id="month08" class="custom-control-input">
                <label for="month08" class="custom-control-label">8月</label>
            </div>
            <div class="custom-control custom-checkbox custom-control-inline">
                <input v-model="item.ForMade97MonthArray" value="09" type="checkbox" id="month09" class="custom-control-input">
                <label for="month09" class="custom-control-label">9月</label>
            </div>
            <div class="custom-control custom-checkbox custom-control-inline">
                <input v-model="item.ForMade97MonthArray" value="10" type="checkbox" id="month10" class="custom-control-input">
                <label for="month10" class="custom-control-label">10月</label>
            </div>
            <div class="custom-control custom-checkbox custom-control-inline">
                <input v-model="item.ForMade97MonthArray" value="11" type="checkbox" id="month11" class="custom-control-input">
                <label for="month11" class="custom-control-label">11月</label>
            </div>
            <div class="custom-control custom-checkbox custom-control-inline">
                <input v-model="item.ForMade97MonthArray" value="12" type="checkbox" id="month12" class="custom-control-input">
                <label for="month12" class="custom-control-label">12月</label>
            </div>
        </div>
        <div class="form-group my-2">
            <h5>依補貼原則辦理者，廠商是否於完工結算前提出要求</h5>
            <div class="custom-control custom-radio">
                <input v-model="item.IsAppleBeforeClose" value="true" type="radio" id="customRadio7" class="custom-control-input">
                <label class="custom-control-label" for="customRadio7">是</label>
            </div>
            <div class="custom-control custom-radio">
                <input v-model="item.IsAppleBeforeClose" value="false" type="radio" id="customRadio8" class="custom-control-input">
                <label class="custom-control-label" for="customRadio8">否 (填否者不可申請)</label>
            </div>
        </div>
        <div class="form-group my-2">
            <h5>本期計價之工程物價調整款金額 A(元)</h5>
            <input v-model="item.PriceAdjust" type="text" class="form-control w-auto">
        </div>
        <div class="form-group my-2">
            <h5>現有預算可支應之金額 B(元)</h5>
            <input v-model="item.BudgetAvail" type="text" class="form-control w-auto">
        </div>
        <div class="form-group my-2">
            <h5>本次申請特別預算地方工程物價調整款 A-B(元)</h5>
            {{item.PriceAdjust - item.BudgetAvail}}
        </div>
        <div class="card-footer">
            <div class="row justify-content-center">
                <button @click="updateItem" v-bind:disabled="item.Seq<1" role="button" class="btn btn-color11-4 btn-xs mx-1"> 儲存 <i class="fas fa-save"></i></button>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['engMain'],
        data: function () {
            return {
                targetId: null,
                item: { Seq:-1 },
            };
        },
        methods: {
            updateItem() {
                this.item.ForMade97Month = this.item.ForMade97MonthArray.join(',');
                window.myAjax.post('/EPCEngMaterialAdj/UpdateItem', { item: this.item })
                    .then(resp => {
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            getItem() {
                this.item = { Seq: -1 }
                window.myAjax.post('/EPCEngMaterialAdj/GetItem', { id: this.engMain.PrjXMLSeq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.item = resp.data.item;
                        } else
                            alert(resp.data.mag);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            }
        },
        async mounted() {
            console.log('mounted() 物料調整');
            let urlParams = new URLSearchParams(window.location.search);
            if (urlParams.has('id')) {
                this.targetId = parseInt(urlParams.get('id'), 10);
                console.log(this.targetId);
                if (Number.isInteger(this.targetId)) {
                    if (this.targetId <= 0) {
                        window.history.back();
                    } else {
                        this.getItem();
                    }
                    return;
                }
            }
        }
    }
</script>