<template>
    <div class="">
        <div v-for="option in selectStdItemOption" v-bind:key="option.Value" >
            <div class="" style="display:block">
                <h1 class="photogroup">{{ option.Text }}</h1>
                <PhotoGroupList v-bind:engMain="engMain" v-bind:si="si" v-bind:stdItem="option.Value"></PhotoGroupList>
                <!-- div class="row justify-content-center">
                    <div class="col-6 col-lg-2 mt-3">
                        <button v-on:click.stop="back" role="button" class="btn btn-shadow btn-color1 btn-block">
                            回上頁
                        </button>
                    </div>
                </div -->
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['engMain', 'si'],
        components: {
            PhotoGroupList: require('./PhotoGroupList.vue').default,
        },
        data: function () {
            return {
                selectStdItemOption: [],
            }
        },
        methods: {
            //群組清單
            getOption() {
                this.selectStdItemOption = [];
                if (this.si.Seq > 0) {
                    this.items = [];
                    window.myAjax.post('/SamplingInspectionRecImprove/GetImgGroupOption', { recSeq: this.si.Seq, checkType: this.si.CCRCheckType1 })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.selectStdItemOption = resp.data.item;
                            }
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            back() {
                //window.history.go(-1);
                window.location = "/SamplingInspectionRecImprove";
            }
        },
        async mounted() {
            console.log('mounted() 缺失改善相片');
            this.getOption();
        }
    }
</script>
