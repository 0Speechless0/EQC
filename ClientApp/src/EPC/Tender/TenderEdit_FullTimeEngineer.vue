<template>
    <div id="menu04" class="tab-pane">
        <div class="table-responsive">
            <table class="table table-responsive-md table-hover">
                <thead class="insearch">
                    <tr>
                        <th><strong>序號</strong></th>
                        <th><strong>工程人員</strong></th>
                        <th><strong>證照種類</strong></th>
                        <th><strong>證照號碼</strong></th>
                        <th><strong>負責本工程起始日期</strong></th>
                        <th><strong>負責本工程結束日期</strong></th>
                        <th><strong>備註</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in engMain.FullTimeEngineer" v-bind:key="item.Seq">
                        <td class="align-middle">{{index+1}}</td>
                        <td><input v-model="item.FEName" type="text" class="form-control"></td>
                        <td><input v-model="item.FELicenseKind" type="text" class="form-control"></td>
                        <td><input v-model="item.FELicenseNo" type="text" class="form-control"></td>
                        <td><input v-model="item.FEStartDate" type="text" class="form-control"></td>
                        <td><input v-model="item.FEEndDate" type="text" class="form-control"></td>
                        <td><input v-model="item.FEMemo" type="text" class="form-control"></td>
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
            console.log('mounted() 專任工程人員');
        }
    }
</script>