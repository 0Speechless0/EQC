<template>
    <div id="menu05" class="tab-pane">
        <div class="table-responsive">
            <table class="table table-responsive-md table-hover">
                <thead class="insearch">
                    <tr>
                        <th><strong>序號</strong></th>
                        <th><strong>工地人員</strong></th>
                        <th><strong>職務類別</strong></th>
                        <th><strong>證照號碼</strong></th>
                        <th><strong>受聘本工程起始日期</strong></th>
                        <th><strong>受聘本工程結束日期</strong></th>
                        <th><strong>備註</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in engMain.SiteRelate" v-bind:key="item.Seq">
                        <td class="align-middle">{{index+1}}</td>
                        <td><input v-model="item.SRName" type="text" class="form-control"></td>
                        <td><input v-model="item.SRLicenseKind" type="text" class="form-control"></td>
                        <td><input v-model="item.SRLicenseNo" type="text" class="form-control"></td>
                        <td><input v-model="item.SRStartDate" type="text" class="form-control"></td>
                        <td><input v-model="item.SREndDate" type="text" class="form-control"></td>
                        <td><input v-model="item.SRMemo" type="text" class="form-control"></td>
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
            console.log('mounted() 工地相關人員');
        }
    }
</script>