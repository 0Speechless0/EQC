<template>
    <div>
        <h5 class="insearch mt-0 py-2">鴻嘉營造有限公司 負責案件清單</h5>
        <comm-pagination ref="pagination" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        <div class="table-responsive" style="padding: 10px;">
            <table class="table table-responsive-md">
                <thead class="insearch">
                    <tr>
                        <th><strong>排序</strong></th>
                        <th><strong>年度</strong></th>
                        <th><strong>工程編號</strong></th>
                        <th><strong>工程名稱</strong></th>
                        <th><strong>執行機關</strong></th>
                        <th><strong>執行單位</strong></th>
                        <th><strong>經費</strong></th>
                        <th><strong>施工地點</strong></th>
                        <th><strong>完工日期</strong></th>
                        <th><strong>實際驗收完成日</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>1</td>
                        <td>111</td>
                        <td>111-|-01X8X-0XX-XXX</td>
                        <td>111年度OO海岸更新海堤歲修工程</td>
                        <td>南區水資源局</td>
                        <td>工務課</td>
                        <td>42890</td>
                        <td>高雄市大樹區、屏東縣屏東市、九如鄉</td>
                        <td>1100915</td>
                        <td></td>
                    </tr>
                    <tr>
                        <td>2</td>
                        <td>110</td>
                        <td>111-B-01XXX-0XX_01</td>
                        <td>110年度O河局防汛備料補充工程</td>
                        <td>南區水資源局</td>
                        <td>工務課</td>
                        <td>57822</td>
                        <td>屏東縣九如鄉、高雄市大樹區</td>
                        <td>1090923</td>
                        <td>1101227</td>
                    </tr>
                    <tr>
                        <td>3</td>
                        <td>109</td>
                        <td>111-B-01XXX-0XX_002</td>
                        <td>109年度OO溪OO堤段構造物維修改善工程</td>
                        <td>南區水資源局</td>
                        <td>工務課</td>
                        <td>41484</td>
                        <td>高雄市大樹區、屏東縣九如鄉</td>
                        <td>1101120</td>
                        <td>1110621</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                //分頁
                recordTotal: 0,
                pageRecordCount: 30,
                pageIndex: 0,
            };
        },
        methods: {
            getList() {
                /*this.ceHeader = {};
                window.myAjax.post('/EPCSchEngProgress/GetSEPHeader', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.sepHeader = resp.data.item;
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });*/
            },
            //分頁
            onPaginationChange(pInx, pCount) {
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
                this.getLost();
            },           
        },
        async mounted() {
            console.log('mounted 負責案件清單');
        }
    }
</script>