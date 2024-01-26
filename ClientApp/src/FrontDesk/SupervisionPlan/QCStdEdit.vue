<template>
    <div>
        <h2>抽查管理標準</h2>

        <Chapter5 v-if="chapter=='5'" v-bind:engMain="engMain" v-bind:op1="seq"></Chapter5>
        <Chapter6 v-if="chapter=='6'" v-bind:engMain="engMain" v-bind:op1="seq"></Chapter6>
        <Chapter701 v-if="chapter=='701'" v-bind:engMain="engMain" v-bind:op1="seq"></Chapter701>
        <Chapter702 v-if="chapter=='702'" v-bind:engMain="engMain" v-bind:op1="seq"></Chapter702>
        <Chapter703 v-if="chapter=='703'" v-bind:engMain="engMain" v-bind:op1="seq"></Chapter703>

    </div>
</template>
<script>
    export default {
        components: {
            Chapter5: require('./QCStdEdit_Chapter5.vue').default,
            Chapter6: require('./QCStdEdit_Chapter6.vue').default,
            Chapter701: require('./QCStdEdit_Chapter701.vue').default,
            Chapter702: require('./QCStdEdit_Chapter702.vue').default,
            Chapter703: require('./QCStdEdit_Chapter703.vue').default,
            
        },
        data: function () {
            return {
                chapter:'',
                engMain:-1,
                seq:-1
            };
        },
        methods: {
        },
        async mounted() {
            console.log('mounted() 監造計畫-抽查管理標準');
            let urlParams = new URLSearchParams(window.location.search);
            if (urlParams.has('chapter') && urlParams.has('engMain') && urlParams.has('seq')) {
                this.chapter = urlParams.get('chapter');
                this.engMain = parseInt(urlParams.get('engMain'), 10);
                this.seq = parseInt(urlParams.get('seq'), 10);
                return;
            }
            window.location = "/FrontDesk";
        }
    }
</script>
