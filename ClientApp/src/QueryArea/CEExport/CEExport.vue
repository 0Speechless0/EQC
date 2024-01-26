<template>
    <div>
        <div class="form-row">
            <div class="col-1 mt-3">
                <select v-model="selectYear" class="form-control">
                    <option v-for="option in selectYearOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                        {{ option.Text }}
                    </option>
                </select>
            </div>
            <div v-if="selectYear>-1" class="col-12 col-sm-3 mt-3">
                <button @click="dnCEExport" class="btn btn-color11-1 mx-1"><i class="fas fa-download"></i>匯出</button>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                selectYear:-1,
                selectYearOptions: [],
            };
        },
        methods: {
            //工程年分
            async getSelectYearOption() {
                const { data } = await window.myAjax.post('/CEExport/GetYearOptions');
                this.selectYearOptions = data;
            },
            //下載
            dnCEExport() {
                window.comm.dnFile('/CEExport/dnCEExport?year=' + this.selectYear);
                    
            },           
        },
        mounted() {
            console.log('mounted 碳排量計算匯出');
            this.getSelectYearOption();
        }
    }
</script>