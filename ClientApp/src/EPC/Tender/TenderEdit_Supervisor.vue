<template>
    <div id="menu03" class="tab-pane">
        <div class="table-responsive">
            <table class="table table-responsive-md table-hover">
                <thead class="insearch">
                    <tr>
                        <th><strong>序號</strong></th>
                        <th><strong>監工人員姓名</strong></th>
                        <th><strong>證照號碼</strong></th>
                        <th><strong>專長</strong></th>
                        <th><strong>進駐工地日期</strong></th>
                        <th><strong>解職日期</strong></th>
                        <th style="width: 128px;"><strong>目前狀況</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in engMain.Supervisor" v-bind:key="item.Seq">
                        <td class="align-middle">{{index+1}}</td>
                        <td><input v-model="item.SPName" type="text" class="form-control"></td>
                        <td><input v-model="item.SPLicenseNo" type="text" class="form-control"></td>
                        <td><input v-model="item.SPSkill" type="text" class="form-control"></td>
                        <td><input v-model="item.SPMoveinDate" type="text" class="form-control"></td>
                        <td><input v-model="item.SPDismissalDate" type="text" class="form-control"></td>
                        <td><input v-model="item.SPState" type="text" class="form-control"></td>
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
            console.log('mounted() 監造單位聘用監工人員');
        }
    }
</script>