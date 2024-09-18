<template>
    <div>
        <ul class="nav nav-tabs" role="tablist">
            <li class="nav-item">
                <button v-on:click="selectTab='EngProgress'" class="nav-link active" data-toggle="tab" href="#menu01">轉入碳排量清單</button>
            </li>
            <li class="nav-item">
                <button v-on:click="selectTab='SubEngProgress'" v-bind:disabled="sepHeader.SPState == null || sepHeader.SPState<1" class="nav-link" data-toggle="tab" href="#menu02">設定分項工程</button>
            </li>
            <li class="nav-item">
                <button v-on:click="selectTab='EngDelProgress'" class="nav-link" data-toggle="tab" href="#menu03">刪除碳排量清單</button>
            </li>
        </ul>
        <EngProgress v-if="selectTab=='EngProgress'" v-bind:tenderItem="tenderItem" v-bind:sepHeader="sepHeader" v-on:reload="reload"></EngProgress>
        <SubEngProgress v-if="selectTab=='SubEngProgress'" v-bind:tenderItem="tenderItem" v-bind:sepHeader="sepHeader" v-on:reload="reload"></SubEngProgress>
        <EngDelProgress v-if="selectTab=='EngDelProgress'" v-bind:tenderItem="tenderItem" v-bind:sepHeader="sepHeader"></EngDelProgress>
    </div>
</template>
<script>
    export default {
        props: ['tenderItem', 'sepHeader'],
        components: {
            EngProgress: require('./PM_SchEng_Progress.vue').default,
            SubEngProgress: require('./PM_SchEng_SubEngProgress.vue').default,
            EngDelProgress: require('./PM_SchEng_DelProgress.vue').default,
        },
        data: function () {
            return {
                selectTab:'EngProgress',
            };
        },
        methods: {
            reload() {
                this.$emit('reload');
            },
        },
        async mounted() {
            console.log('mounted() 前置作業');
        },
    }
</script>
<style scoped>
    button:disabled {
        cursor: not-allowed;
        pointer-events: all !important;
    }
    
</style>