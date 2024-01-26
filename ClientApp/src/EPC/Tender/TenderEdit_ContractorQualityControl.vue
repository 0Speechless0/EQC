<template>
    <div id="menu02" class="tab-pane">
        <div class="table-responsive">
            <table class="table table-responsive-md table-hover">
                <thead class="insearch">
                    <tr>
                        <th><strong>序號</strong></th>
                        <th><strong>品管人員姓名</strong></th>
                        <th><strong>證照號碼</strong></th>
                        <th><strong>專長</strong></th>
                        <th><strong>進駐工地日期</strong></th>
                        <th><strong>解職日期</strong></th>
                        <th style="width: 128px;"><strong>目前狀況</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in engMain.ContractorQualityControl" v-bind:key="item.Seq">
                        <td class="align-middle">{{index+1}}</td>
                        <td><input v-model="item.QCName" type="text" class="form-control"></td>
                        <td><input v-model="item.QCLicenseNo" type="text" class="form-control"></td>
                        <td><input v-model="item.QCSkill" type="text" class="form-control"></td>
                        <td><input v-model="item.QCMoveinDate" type="text" class="form-control"></td>
                        <td><input v-model="item.QCDismissalDate" type="text" class="form-control"></td>
                        <td><input v-model="item.QCState" type="text" class="form-control"></td>
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
            console.log('mounted() 廠商聘用品管人員');
        }
    }
</script>