<template>
    <div>
        <div class="row justify-content-between">
            <div class="col-12 col-md-6 col-xl-4 mt-3">
                <select class="form-control" v-model="selected">
                    <option value="Chapter5">第五章 材料設備抽驗程序及標準</option>
                    <option value="Chapter6">第六章 設備功能運轉測試抽驗程序及標準</option>
                    <option value="Chapter701">第七章 施工抽查程序及標準</option>
                    <option value="Chapter702">第七章 施工抽查程序及標準(環境保育)</option>
                    <option value="Chapter703">第七章 施工抽查程序及標準(職業安全衛生)</option>
                </select>
            </div>
        </div>
        <div>
            <flow5 v-if="selected=='Chapter5'"></flow5>
            <flow6 v-if="selected=='Chapter6'"></flow6>
            <flow701 v-if="selected=='Chapter701'"></flow701>
            <flow702 v-if="selected=='Chapter702'"></flow702>
            <flow703 v-if="selected=='Chapter703'"></flow703>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                selected: 'Chapter5',
            };
        },
        components: {
            flow5: require('./CheckSheet5.vue').default,
            flow6: require('./CheckSheet6.vue').default,
            flow701: require('./CheckSheet701.vue').default,
            flow702: require('./CheckSheet702.vue').default,
            flow703: require('./CheckSheet703.vue').default
        },
        async mounted() {
            console.log('mounted() CheckSheet');
            /*if (this.options51.length == 0) {
                this.getOptions51();
            }*/
        }
    }
</script>