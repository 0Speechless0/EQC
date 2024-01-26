<template>
    <div>
        <div class="row justify-content-between">
            <div class="col-12 col-md-8 col-xl-6 mt-3">
                <select class="form-control" v-model="selected">
                    <option value="Chapter5">第五章 材料與設備抽驗程序及標準</option>
                    <option value="Chapter6">第六章 設備功能運轉測試抽驗程序及標準</option>
                    <option value="Chapter701">第七章 施工抽查程序及標準</option>
                    <option value="Chapter702">第七章 施工抽查程序及標準(環境保育)</option>
                    <option value="Chapter703">第七章 施工抽查程序及標準(職業安全衛生)</option>
                </select>
            </div>
            <div class="col-12 col-md-4 col-xl-4 mt-3">
                <button v-on:click.stop="addItem++" role="button" class="btn btn-outline-secondary btn-xs mx-1">
                    <i class="fas fa-plus"></i>&nbsp;&nbsp;新增
                </button>
            </div>
        </div>
        <div>
            <flow5 v-if="selected=='Chapter5'" v-bind:addItem="addItem"></flow5>
            <flow6 v-if="selected=='Chapter6'" v-bind:addItem="addItem"></flow6>
            <flow701 v-if="selected=='Chapter701'" v-bind:addItem="addItem"></flow701>
            <flow702 v-if="selected=='Chapter702'" v-bind:addItem="addItem"></flow702>
            <flow703 v-if="selected=='Chapter703'" v-bind:addItem="addItem"></flow703>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                selected: 'Chapter5',
                addItem: 0
            };
        },
        components: {
            flow5: require('./FlowChartChapter5.vue').default,
            flow6: require('./FlowChartChapter6.vue').default,
            flow701: require('./FlowChartChapter701.vue').default,
            flow702: require('./FlowChartChapter702.vue').default,
            flow703: require('./FlowChartChapter703.vue').default
        },
        async mounted() {
            console.log('mounted() FlowChart');
            /*if (this.options51.length == 0) {
                this.getOptions51();
            }*/
        }
    }
</script>