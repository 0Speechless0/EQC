<template>
    <div id="menu07" class="tab-pane">
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
                        <th><strong>變更日期</strong></th>
                        <th><strong>公告編號</strong></th>
                        <th><strong>核准文號</strong></th>
                        <th><strong>變更前金額</strong></th>
                        <th><strong>變更後金額</strong></th>
                        <th><strong>變更前竣工日</strong></th>
                        <th><strong>變更後竣工日</strong></th>
                        <th><strong>準延天數</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in engMain.ChangeDesignData" v-bind:key="item.Seq">
                        <td class="align-middle"><strong>{{item.CDChangeDate}}</strong></td>
                        <td><input v-model="item.CDAnnoNo" type="text" class="form-control"></td>
                        <td><input v-model="item.CDApprovalNo" type="text" class="form-control"></td>
                        <td><input v-model="item.CDBeforeAmount" type="text" class="form-control"></td>
                        <td><input v-model="item.CDAfterAmount" type="text" class="form-control"></td>
                        <td><input v-model="item.CDBeforeCloseDate" type="text" class="form-control"></td>
                        <td><input v-model="item.CDAfterCloseDate" type="text" class="form-control"></td>
                        <td><input v-model="item.CDApprovalDays" type="text" class="form-control"></td>
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
            console.log('mounted() 變更設計資料');
        }
    }
</script>