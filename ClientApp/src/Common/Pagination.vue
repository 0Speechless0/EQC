addOutCommitte
<template>
    <div class="form-inline small">
        <label class="my-1 mr-2">
            共
            <span class="text-danger">{{recordTotal}}</span>
            筆，每頁顯示
        </label>
        <select v-model="pageRecordCount" @change="onPageRecordCountChange($event)" class="form-control sort form-control-sm">
            <option v-for="option in pageRecordCountOption" v-bind:value="option" v-bind:key="option">
                {{ option }}
            </option>
            <!-- option value="30">30</option>
            <option value="50">50</option>
            <option value="100">100</option -->
        </select>
        <label class="my-1 mx-2">筆，共<span class="text-danger">{{pageTotal}}</span>頁，目前顯示第</label>
        <select v-model="pageIndex" @change="onPageChange($event)" class="form-control sort form-control-sm">
            <option v-for="option in pageIndexOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                {{ option.Text }}
            </option>
        </select>
        <label class="my-1 mx-2">頁</label>
    </div>
</template>
<script>
    export default {
        props: ['recordTotal','pageRecordList'],
        watch: {
            'recordTotal': function (nval, oval) {
                this.onPageRecordCountChange();
            },
        },
        data: function () {
            return {
                //分頁
                //recordTotal: 0,
                pageRecordCountOption:[30, 50, 100], //s20230530
                pageRecordCount: 30,
                pageTotal: 0,
                pageIndex: 1,
                pageIndexOptions: [],
                fInit:false,//s20230627
            };
        },
        methods: {
            //分頁
            onPageRecordCountChange() {
                this.setPagination();
                this.onChange();
            },
            onPageChange() {
                this.onChange();
            },
            //計算分頁
            setPagination() {
                this.pageTotal = Math.ceil(this.recordTotal / this.pageRecordCount);
                //
                this.pageIndexOptions = [];
                if (this.pageTotal == 0) {
                    this.pageIndex = 1;
                } else {
                    for (var i = 1; i <= this.pageTotal; i++) {
                        this.pageIndexOptions.push({ Text: i, Value: i });
                    }
                    if (this.pageIndex > this.pageIndexOptions.length) {
                        this.pageIndex = this.pageIndexOptions.length;
                    }
                }
            },
            onChange() {
                this.$emit('onPaginationChange', this.pageIndex, this.pageRecordCount);
            }
        },
        mounted() {
            console.log('mounted() 分頁模組');
            if (this.pageRecordList != null) {//s20230530
                this.pageRecordCountOption = this.pageRecordList;
                if (this.pageRecordCountOption.length > 0) this.pageRecordCount = this.pageRecordCountOption[0];
            }
        }
    }
</script>