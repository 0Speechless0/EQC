<template>
    <div>
        <table class="table table-responsive-md table-hover">
            <thead class="insearch">
                <tr>
                    <th class="text-left"><strong>項次</strong></th>
                    <th class="text-left"><strong>施工抽查流程圖名稱</strong></th>
                    <th width="100"><strong>抽查紀錄數</strong></th>
                    <th width="100"><strong>缺失個數</strong></th>
                    <th width="80" class="text-center"><strong>管理</strong></th>
                    <th width="200" class="text-center"><strong>施工抽查紀錄表下載</strong></th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                    <td class="text-left">{{item.OrderNo}}</td>
                    <td class="text-left">{{item.ItemName}}</td>
                    <td>{{item.constCheckRecCount}}</td>
                    <td>{{item.missingCount}}</td>
                    <td>
                        <div class="row justify-content-center m-0">
                            <button @click="editSIR(item)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                        </div>
                    </td>
                    <td>
                        <template v-if="item.constCheckRecCount > 0">
                            <button v-on:click="SIRDnDoc(item,1)" class="btn btn-color12 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> doc</button>
                            <button v-on:click="SIRDnDoc(item,2)" class="btn btn-color11-4 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> pdf</button>
                            <button v-on:click="SIRDnDoc(item,3)" class="btn btn-color11-1 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> odt</button>
                        </template>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</template>
<script>
    export default {
        props: ['tenderItem', 'mode', 'items'],
        data: function () {
            return {
            };
        },
        methods: {
            editSIR(item) {
                this.$emit('editSIR', this.mode, item);
            },
            SIRDnDoc(item, fileType) {
                window.comm.dnFile('/SamplingInspectionRec/SIRDnDoc?mode=' + this.mode + '&seq=' + item.Seq + '&fileType=' + fileType + '&eId=' + this.tenderItem.Seq)
            }
        },
        async mounted() {
            console.log('mounted() 抽查項目清單');
        }
    }
</script>
