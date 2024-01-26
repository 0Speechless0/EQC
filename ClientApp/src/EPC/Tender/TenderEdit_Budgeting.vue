<template>
    <div id="menu06" class="tab-pane">
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
                        <th><strong>預算年(民國)</strong></th>
                        <th><strong>預算別</strong></th>
                        <th><strong>預算來源</strong></th>
                        <th><strong>預算科目</strong></th>
                        <th><strong>法定預算數</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in engMain.Budgeting" v-bind:key="item.Seq">
                        <td><input v-model="item.BudgetYear" type="text" class="form-control"></td>
                        <td><input v-model="item.BudgetKindList[0].BudgetType" type="text" class="form-control"></td>
                        <td><input v-model="item.BudgetKindList[0].BudgetSource" type="text" class="form-control"></td>
                        <td><input v-model="item.BudgetKindList[0].BudgetAccount" type="text" class="form-control"></td>
                        <td><input v-model="item.BudgetKindList[0].LegalBudget" type="text" class="form-control"></td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['engMain'],
        /*watch: {
            pageIndex: {
                handler: function (value) {
                    this.getList();
                }
            }
        },
        components: {
            qcstdmodal: require('./QCStdModal.vue').default
        },
        data: function () {
            return {
            };
        },
        methods: {
        },*/
        async mounted() {
            console.log('mounted() 預算編列');
        }
    }
</script>