<template>
    <div>
        <table class="table table-responsive-md table-hover">
            <thead class="insearch">
                <tr>
                    <th class="text-left"><strong>項次</strong></th>
                    <th class="text-left"><strong>施工抽查流程圖名稱</strong></th>
                    <th width="100"><strong>缺失個數</strong></th>
                    <th width="80" class="text-center"><strong>管理</strong></th>
                    <th width="100"><strong>不符合事項報告</strong></th>
                    <th width="100"><strong>NCR程序追蹤改善表</strong></th>
                    <th width="100"><strong>改善照片檔</strong></th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                    <td class="text-left">{{item.OrderNo}}</td>
                    <td class="text-left">{{item.ItemName}}</td>
                    <td>{{item.missingCount}}</td>
                    <td>
                        <div class="row justify-content-center m-0">
                            <button @click="editItem(item)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                        </div>
                    </td>
                    <td>
                        <template v-if="item.improveCount>0">
                            <button v-on:click="SIRDnDoc(item,44,1)" class="btn btn-color12 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> doc</button>
                            <br />
                            <button v-on:click="SIRDnDoc(item,44,2)" class="btn btn-color11-4 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> pdf</button>
                            <br />
                            <button v-on:click="SIRDnDoc(item,44,3)" class="btn btn-color11-1 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> odt</button>
                        </template>
                    </td>
                    <td>
                        <template v-if="item.ncrCount>0">
                            <button v-on:click="SIRDnDoc(item,45,1)" class="btn btn-color12 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> doc</button>
                            <br />
                            <button v-on:click="SIRDnDoc(item,45,2)" class="btn btn-color11-4 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> pdf</button>
                            <br />
                            <button v-on:click="SIRDnDoc(item,45,3)" class="btn btn-color11-1 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> odt</button>
                        </template>
                    </td>
                    <td>
                        <template v-if="item.photoCount>0">
                            <button v-on:click="SIRDnDoc(item,46,1)" class="btn btn-color12 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> doc</button>
                            <br />
                            <button v-on:click="SIRDnDoc(item,46,2)" class="btn btn-color11-4 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> pdf</button>
                            <br />
                            <button v-on:click="SIRDnDoc(item,46,3)" class="btn btn-color11-1 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> odt</button>
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
            editItem(item) {
                this.$emit('editSIRImprove', this.mode, item);
            },
            SIRDnDoc(item, docType, fileType) {
                window.comm.dnFile('/SamplingInspectionRecImprove/SIRDnDoc?mode=' + this.mode + '&seq=' + item.Seq + '&fileType=' + fileType + '&eId=' + this.tenderItem.Seq + '&docType=' + docType)
            }
        },
        async mounted() {
            console.log('mounted 改善抽查項目清單');
        }
    }
</script>
