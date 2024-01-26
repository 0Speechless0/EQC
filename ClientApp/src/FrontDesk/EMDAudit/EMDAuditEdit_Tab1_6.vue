<template>
    <div>
        <div class="modal fade text-left" v-bind:id="modalId" v-bind:ref="modalId" tabindex="-1" aria-labelledby="infoEdit_01" aria-hidden="true">
            <div class="modal-dialog modal-dialog-scrollable modal-dialog-centered modal-xl" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">備註</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close" v-on:click="closeModal()">
                            <span aria-hidden="true">×</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <MaterialInfo v-bind:emdSummary="emdSummary"></MaterialInfo>
                        <h2>備註</h2>
                        <div class="table-responsive">
                            <table border="0" class="table table2 mt-0">
                                <tbody>
                                    <tr>
                                        <th>備註(歸檔編號)</th>
                                        <td><input type="text" class="form-control" v-model="targetItem.ArchiveNo"></td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div class="modal-footer  justify-content-center">
                        <button v-if="targetItem.edit" v-on:click.stop="saveItems" role="button" class="btn btn-color11-4 btn-xs mx-1">
                            <i class="fas fa-save"></i> 儲存
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['modalId', 'emdSummary', 'targetItem'],
        components: {
            MaterialInfo: require('./MaterialInfo.vue').default,
        },
        data: function () {
            return {
            };
        },
        methods: {
            closeHandler: function () {
                this.$emit('onCloseEvent')
            },
            closeModal() {
                this.closeHandler();
            },
            saveItems() {
                window.myAjax.post('/EMDAudit/UpdateEMDSummary_2', { item: this.targetItem })
                    .then(resp => {
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            }
        },
        async mounted() {
            console.log('mounted() 協力廠商資料');
        }
    }
</script>

