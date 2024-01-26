<template>
    <div class="modal fade text-left" v-bind:id="modalId" v-bind:ref="modalId" data-backdrop="static" data-keyboard="false" tabindex="-1" aria-labelledby="flowchart" aria-hidden="true">
        <div class="modal-dialog modal-dialog-scrollable modal-dialog-centered modal-xl" style="max-width: fit-content;" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h6 class="modal-title redText" id="projectUpload">抽查管理標準</h6>
                    <button ref="flowChartModalClose" type="button" class="close" data-dismiss="modal" aria-label="Close" v-on:click="closeModal()">
                        <span aria-hidden="true">×</span>
                    </button>
                </div>
                <div class="modal-body">
                    <p class="bg-gray p-3 mb-1">
                        {{targetItem.ItemName}}
                    </p>
                    <Chapter5 v-if="chapter=='5'" v-bind:engMain="engMain" v-bind:op1="targetItem.Seq"></Chapter5>
                    <Chapter6 v-if="chapter=='6'" v-bind:engMain="engMain" v-bind:op1="targetItem.Seq"></Chapter6>
                    <Chapter701 v-if="chapter=='701'" v-bind:engMain="engMain" v-bind:op1="targetItem.Seq"></Chapter701>
                    <Chapter702 v-if="chapter=='702'" v-bind:engMain="engMain" v-bind:op1="targetItem.Seq"></Chapter702>
                    <Chapter703 v-if="chapter=='703'" v-bind:engMain="engMain" v-bind:op1="targetItem.Seq"></Chapter703>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['modalId', 'engMain', 'targetItem', 'chapter'],
        components: {
            Chapter5: require('./QCStdEdit_Chapter5.vue').default,
            Chapter6: require('./QCStdEdit_Chapter6.vue').default,
            Chapter701: require('./QCStdEdit_Chapter701.vue').default,
            Chapter702: require('./QCStdEdit_Chapter702.vue').default,
            Chapter703: require('./QCStdEdit_Chapter703.vue').default,
        },
        methods: {
            closeHandler: function () {
                this.$emit('onCloseEvent')
            },
            closeModal() {
                this.closeHandler();
            }
        },
        async mounted() {
            console.log('mounted() QCStdModal');
        }
    }
</script>
