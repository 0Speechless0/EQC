<template>
    <div id="menu08" class="tab-pane">
        <!-- 選擇年月份 -->
        <ul class="nav justify-content-start nav-pills mb-3" id="pills-tab" role="tablist">
            <li v-for="(item, index) in engMain.ProgressData" v-bind:key="item.Seq" class="nav-item mr-1">
                <a v-on:click.stop="onChangeItem(item, index)" v-bind:class="setTabCSS(item,index)" data-toggle="pill" href="##" role="tab" aria-controls="pills-home">{{getTabCaption(item)}}</a>
            </li>
        </ul>
        <!-- 顯示進度資料 -->
        <div class="tab-content">
            <div class="tab-pane fade show active" id="pills-A" role="tabpanel" aria-labelledby="pills-home-tab">
                <div class="row justify-content-between">
                    <div class="form-inline col-12 col-md-8 small">
                        <label class="my-2 mr-2">
                            <span class="text-danger">※ 貨幣單位為「新台幣千元」</span>
                        </label>
                    </div>
                </div>
                <form>
                    <h5>總累計</h5>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">總累計預定進度</label>
                        <div class="col-12 col-lg-9">
                            <input v-model="item.PDAccuScheProgress" type="text" class="form-control w-auto">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">總累計實際進度</label>
                        <div class="col-12 col-lg-9">
                            <input v-model="item.PDAccuActualProgress" type="text" class="form-control w-auto">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">總累計預定完成金額</label>
                        <div class="col-12 col-lg-9">
                            <input v-model="item.PDAccuScheCloseAmount" type="text" class="form-control w-auto">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">總累計實際完成金額</label>
                        <div class="col-12 col-lg-9">
                            <input v-model="item.PDAccuActualCloseAmount" type="text" class="form-control w-auto">
                        </div>
                    </div>
                    <h5>年累計</h5>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">年累計預定進度</label>
                        <div class="col-12 col-lg-9">
                            <input v-model="item.PDYearAccuScheProgress" type="text" class="form-control w-auto">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">年累計實際進度</label>
                        <div class="col-12 col-lg-9">
                            <input v-model="item.PDYearAccuActualProgress" type="text" class="form-control w-auto">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">年累計預定完成金額</label>
                        <div class="col-12 col-lg-9">
                            <input v-model="item.PDYearAccuScheCloseAmount" type="text" class="form-control w-auto">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">年累計實際完成金額</label>
                        <div class="col-12 col-lg-9">
                            <input v-model="item.PDYearAccuActualCloseAmount" type="text" class="form-control w-auto">
                        </div>
                    </div>
                    <h5>估驗計價</h5>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">總累計估驗計價金額</label>
                        <div class="col-12 col-lg-9">
                            <input v-model="item.PDAccuEstValueAmount" type="text" class="form-control w-auto">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">估驗計價保留款</label>
                        <div class="col-12 col-lg-9">
                            <input v-model="item.PDEstValueRetentAmount" type="text" class="form-control w-auto">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">待支付金額</label>
                        <div class="col-12 col-lg-9">
                            <input v-model="item.PDAccountPayable" type="text" class="form-control w-auto">
                        </div>
                    </div>
                    <h5>執行</h5>
                    <div class="row">
                        <label class="col-12 col-lg-3 col-form-label">執行狀態</label>
                        <div class="col-12 col-lg-9">
                            <input v-model="item.PDExecState" type="text" class="form-control w-auto" value="39">
                        </div>
                    </div>
                    <div class="row">
                        <label class="col-12 col-lg-3 col-form-label">差異</label>
                        <div class="col-12 col-lg-9">
                            <input v-model="item.DiffProgress" type="text" class="form-control w-auto" value="39">
                        </div>
                    </div>
                    <div class="row">
                        <label class="col-12 col-lg-3 col-form-label">累計天數</label>
                        <div class="col-12 col-lg-9">
                            <input v-model="item.AccuDay" type="text" class="form-control w-auto" value="39">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">實際執行摘要</label>
                        <div class="col-12 col-lg-9">
                            <textarea v-model="item.PDActualExecMemo" rows="2" class="form-control"></textarea>
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">應付未付數</label>
                        <div class="col-12 col-lg-9">
                            <input v-model="item.PDAccountPayable" type="text" class="form-control w-auto">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">預定執行摘要</label>
                        <div class="col-12 col-lg-9">
                            <textarea v-model="item.ScheExecMemo" rows="2" type="text" class="form-control"></textarea>
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">延遲付款原因</label>
                        <div class="col-12 col-lg-9">
                            <textarea v-model="item.LatePaymentReason" rows="2" type="text" class="form-control"></textarea>
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">解約原因</label>
                        <div class="col-12 col-lg-9">
                            <textarea v-model="item.TerminationReason" rows="2" type="text" class="form-control"></textarea>
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">專任工程人員督導次數</label>
                        <div class="col-12 col-lg-9">
                            <input v-model="item.SupervisionTimes" type="text" class="form-control w-auto" value="0">
                        </div>
                    </div>
                </form>
            </div>
            <div class="tab-pane fade" id="pills-B" role="tabpanel" aria-labelledby="pills-home-tab">
                <div class="row justify-content-between">
                    <div class="form-inline col-12 col-md-8 small">
                        <label class="my-2 mr-2">
                            <span class="text-danger">※ 貨幣單位為「新台幣千元」</span>
                        </label>
                    </div>
                </div>
                <form>
                    <h5>總累計</h5>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">總累計預定進度</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="90">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">總累計實際進度</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="80">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">總累計預定完成金額</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="ooo">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">總累計實際完成金額</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="ooo">
                        </div>
                    </div>
                    <h5>年累計</h5>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">年累計預定進度</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="1">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">年累計實際進度</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="0">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">年累計預定完成金額</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="10000">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">年累計實際完成金額</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="0">
                        </div>
                    </div>
                    <h5>估驗計價</h5>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">總累計估驗計價金額</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="1212670">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">估驗計價保留款</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="39">
                        </div>
                    </div>
                    <h5>執行</h5>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">執行狀態</label>
                        <div class="col-12 col-lg-9">
                            <select class="form-control">
                                <option value="施工中">施工中</option>
                            </select>
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">實際執行摘要</label>
                        <div class="col-12 col-lg-9">
                            <textarea rows="2" class="form-control">100/5/15因水庫水位回升至EL+219M以上暫停施作 俟水庫水位狀況再行進場施工</textarea>
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">應付未付數</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="0">
                        </div>
                    </div>
                </form>
            </div>
            <div class="tab-pane fade" id="pills-C" role="tabpanel" aria-labelledby="pills-home-tab">
                <div class="row justify-content-between">
                    <div class="form-inline col-12 col-md-8 small">
                        <label class="my-2 mr-2">
                            <span class="text-danger">※ 貨幣單位為「新台幣千元」</span>
                        </label>
                    </div>
                </div>
                <form>
                    <h5>總累計</h5>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">總累計預定進度</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="100">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">總累計實際進度</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="99">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">總累計預定完成金額</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="1222872">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">總累計實際完成金額</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="1210643">
                        </div>
                    </div>
                    <h5>年累計</h5>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">年累計預定進度</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="1">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">年累計實際進度</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="0">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">年累計預定完成金額</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="10000">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">年累計實際完成金額</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="0">
                        </div>
                    </div>
                    <h5>估驗計價</h5>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">總累計估驗計價金額</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="1212670">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">估驗計價保留款</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="39">
                        </div>
                    </div>
                    <h5>執行</h5>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">執行狀態</label>
                        <div class="col-12 col-lg-9">
                            <select class="form-control">
                                <option value="施工中">施工中</option>
                            </select>
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">實際執行摘要</label>
                        <div class="col-12 col-lg-9">
                            <textarea rows="2" class="form-control">100/5/15因水庫水位回升至EL+219M以上暫停施作 俟水庫水位狀況再行進場施工</textarea>
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-12 col-lg-3 col-form-label">應付未付數</label>
                        <div class="col-12 col-lg-9">
                            <input type="text" class="form-control w-auto" value="0">
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['engMain'],
        data: function () {
            return {
                item: {},
                selectTab:0,
            };
        },
        /*watch: {
            pageIndex: {
                handler: function (value) {
                    this.getList();
                }
            }
        },*/
        
        methods: {
            onChangeItem(item, index) {
                this.item = item;
                this.selectTab = index;
            },
            setTabCSS(item, index) {
                console.log(index);
                var css = "nav-link btn btn-light";
                if (index == this.selectTab) {
                    css = css + " active";
                    this.item = item;
                }
                return css;
            },
            getTabCaption(item) {
                var b = '';
                if (item.PDMonth < 10) b = '0';
                return item.PDYear + b + item.PDMonth;
            }
        },
        mounted() {
            console.log('mounted() 進度資料');
        }
    }
</script>