<template>
    <div>
        <h3>Hi App.vue111255555</h3>
        <!-- <Loading :active.sync="isLoading"
                 :can_cancel="true"
                 :is-full-page="fullPage"></Loading>
                 <PttModal ></PttModal>
        <router-view></router-view> -->
    </div>
</template>

<script>
import { BIconChevronDoubleLeft } from 'bootstrap-vue';

    // import Loading from 'vue-loading-overlay';
    // import PttModal from './components/pttModal.vue';
    //import 'vue-loading-overlay/dist/vue-loading.css';
    export default {
        components: {
            // Loading: Loading,
            // PttModal : PttModal
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
            console.log(process.env.NODE_ENV );
            // window.replace('/Login/Login')
        }
    }

</script>

