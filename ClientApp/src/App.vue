<template>
    <div>
        <h5>Hi App.vue</h5>
        <Loading :active.sync="isLoading"
                 :can_cancel="true"
                 :is-full-page="fullPage"></Loading>
                 <PttModal ></PttModal>
        <router-view></router-view>
    </div>
</template>

<script>
    import Loading from 'vue-loading-overlay';
    import PttModal from './components/pttModal.vue';
    //import 'vue-loading-overlay/dist/vue-loading.css';
    export default {
        components: {
            Loading: Loading,
            PttModal : PttModal
        },
        computed: {
            isLoading() {
                return this.$store.state.isLoading;
            }
        },
        methods: {
            onCancel() {
                alert('����Loading...')
            },
        },
        mounted() {
            window.replace('/Login/Login')
        }
    }

</script>

