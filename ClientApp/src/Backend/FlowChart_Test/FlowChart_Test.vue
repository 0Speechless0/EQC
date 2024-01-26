<template>
    <div>
        <div class="nav nav-tabs border-0">
            <div class="nav nav-tabs border-0">
                <div class="nav-item">
                    <button v-if="selected=='Chapter5'" data-toggle="tab" v-on:click.stop="OnChange('Chapter5')" href="#" class="btn-block  btn btn-color4 active" title="第五章 材料與設備抽驗程序及標準">第五章 材料與設備抽驗程序及標準</button>
                    <button v-if="selected!='Chapter5'" data-toggle="tab" v-on:click.stop="OnChange('Chapter5')" href="#" class="btn-block  btn btn-color4 " title="第五章 材料與設備抽驗程序及標準">第五章 材料與設備抽驗程序及標準</button>
                </div>
                <div class="nav-item">
                    <button v-if="selected=='Chapter6'" data-toggle="tab" v-on:click.stop="OnChange('Chapter6')" href="#" class="btn-block  btn btn-color4 active" title="第六章 設備功能運轉測試抽驗程序及標準">第六章 設備功能運轉測試抽驗程序及標準</button>
                    <button v-if="selected!='Chapter6'" data-toggle="tab" v-on:click.stop="OnChange('Chapter6')" href="#" class="btn-block  btn btn-color4 " title="第六章 設備功能運轉測試抽驗程序及標準">第六章 設備功能運轉測試抽驗程序及標準</button>
                </div>
            </div>
            <div class="nav nav-tabs border-0">
                <div class="nav-item">
                    <button v-if="selected=='Chapter701'" data-toggle="tab" v-on:click.stop="OnChange('Chapter701')" href="#" class="btn-block  btn btn-color4 active" title="第七章 施工抽查程序及標準">第七章 施工抽查程序及標準</button>
                    <button v-if="selected!='Chapter701'" data-toggle="tab" v-on:click.stop="OnChange('Chapter701')" href="#" class="btn-block  btn btn-color4 " title="第七章 施工抽查程序及標準">第七章 施工抽查程序及標準</button>
                </div>
                <div class="nav-item">
                    <button v-if="selected=='Chapter702'" data-toggle="tab" v-on:click.stop="OnChange('Chapter702')" href="#" class="btn-block  btn btn-color4 active" title="第七章 施工抽查程序及標準(環境保育)">第七章 施工抽查程序及標準(環境保育)</button>
                    <button v-if="selected!='Chapter702'" data-toggle="tab" v-on:click.stop="OnChange('Chapter702')" href="#" class="btn-block  btn btn-color4 " title="第七章 施工抽查程序及標準(環境保育)">第七章 施工抽查程序及標準(環境保育)</button>
                </div>
                <div class="nav-item">
                    <button v-if="selected=='Chapter703'" data-toggle="tab" v-on:click.stop="OnChange('Chapter703')" href="#" class="btn-block  btn btn-color4 active" title="第七章 施工抽查程序及標準(職業安全衛生)">第七章 施工抽查程序及標準(職業安全衛生)</button>
                    <button v-if="selected!='Chapter703'" data-toggle="tab" v-on:click.stop="OnChange('Chapter703')" href="#" class="btn-block  btn btn-color4 " title="第七章 施工抽查程序及標準(職業安全衛生)">第七章 施工抽查程序及標準(職業安全衛生)</button>
                </div>
                <!-- <div class="nav-item">
                    <a href="https://forms.gle/p6zv83nNZ8MZYaiV9" target="_blank">
                     <button class="btn btn-outline-danger">我要加分</button>
                    
                    </a> 
                </div> -->
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
            flow5: require('./FlowChartChapter5_Test.vue').default,
            flow6: require('./FlowChartChapter6_Test.vue').default, 
            flow701: require('./FlowChartChapter701_Test.vue').default,
            flow702: require('./FlowChartChapter702_Test.vue').default,
            flow703: require('./FlowChartChapter703_Test.vue').default
        },
        methods: {
            OnChange(msg) {
                //console.log(msg);
                this.selected = msg;

            }

        },
        async mounted() {
            console.log('mounted() FlowChart');
            /*if (this.options51.length == 0) {
                this.getOptions51();
            }*/
        }
    }
</script>