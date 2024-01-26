<template>
    <div id="menu09" class="tab-pane">
        <!-- 選擇年月份 -->
        <ul class="nav justify-content-start nav-pills mb-3" id="pills-tab" role="tablist">
            <li v-for="(item, index) in engMain.BackwardData" v-bind:key="item.Seq" class="nav-item mr-1">
                <a v-on:click.stop="onChangeItem(item, index)" v-bind:class="setTabCSS(item,index)" data-toggle="pill" href="##" role="tab" aria-controls="pills-home">{{getTabCaption(item)}}</a>
            </li>
        </ul>
        <!-- 顯示進度資料 -->
        <div class="tab-content">
            <div class="tab-pane fade show active" id="pills-AA" role="tabpanel" aria-labelledby="pills-home-tab">
                <form>
                    <div class="form-group row">
                        <label class="col-md-2 col-form-label">落後因素</label>
                        <div class="col-md-10">
                            <input v-model="item.BDBackwardFactor" type="text" class="form-control">
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-md-2 col-form-label">原因分析</label>
                        <div class="col-md-10">
                            <textarea v-model="item.BDAnalysis" rows="2" class="form-control"></textarea>
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-md-2 col-form-label">解決辦法</label>
                        <div class="col-md-10">
                            <textarea v-model="item.BDSolution" rows="2" class="form-control"></textarea>
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-md-2 col-form-label">改進期限</label>
                        <div class="col-md-10">
                            <input v-model="item.ImproveDeadline" type="text" class="form-control">
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['engMain'],
        data: function () {
            return {
                item: {},
                selectTab:0,
            };
        },
        /*watch: {
            pageIndex: {
                handler: function (value) {
                    this.getList();
                }
            }
        },*/
        
        methods: {
            onChangeItem(item, index) {
                this.item = item;
                this.selectTab = index;
            },
            setTabCSS(item, index) {
                console.log(index);
                var css = 'nav-link btn btn-light';
                if (index == this.selectTab) {
                    css = css + " active";
                    this.item = item;
                }
                return css;
            },
            getTabCaption(item) {
                var b = '';
                if (item.BDMonth < 10) b = '0';
                return item.BDYear + b + item.BDMonth;
            }
        },
        mounted() {
            console.log('mounted() 落後資料');
        }
    }
</script>