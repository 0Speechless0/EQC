<template>
    <div>
        <h5 class="insearch mt-0 py-2">OO工程有限公司 負責案件清單</h5>
        <comm-pagination ref="pagination" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        <div class="table-responsive" style="padding: 10px;">
            <table class="table table-responsive-md">
                <thead class="insearch">
                    <tr>
                        <th><strong>排序</strong></th>
                        <th><strong>工程編號</strong></th>
                        <th><strong>工程名稱</strong></th>
                        <th><strong>執行機關</strong></th>
                        <th><strong>執行單位</strong></th>
                        <th><strong>碳排放量</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>1</td>
                        <td>111-|-01X8X-0XX-XXX</td>
                        <td>111年度OO海岸更新海堤歲修工程</td>
                        <td>第O河川局</td>
                        <td>工務課</td>
                        <td>765025</td>
                    </tr>
                    <tr>
                        <td>2</td>
                        <td>111-B-01XXX-0XX_01</td>
                        <td>111年度O河局防汛備料補充工程</td>
                        <td>第O河川局</td>
                        <td>管理課</td>
                        <td>1631605</td>
                    </tr>
                    <tr>
                        <td>3</td>
                        <td>111-|-01XXX-0XX-XXX</td>
                        <td>111年度OO溪OO堤段構造物維修改善工程</td>
                        <td>第O河川局</td>
                        <td>工務課</td>
                        <td>1020778</td>
                    </tr>
                    <tr>
                        <td>4</td>
                        <td>111-B-01XXX-01_2-XXX</td>
                        <td>111年度OO溪OO大橋上游河段採售分離計畫--支出(工程標)</td>
                        <td>第O河川局</td>
                        <td>管理課</td>
                        <td>543716</td>
                    </tr>
                    <tr>
                        <td>5</td>
                        <td>111-|-01XAA-0XX-XXX</td>
                        <td>111年度OO第二水庫周邊設施改善工程</td>
                        <td>第O河川局</td>
                        <td>工務課</td>
                        <td>856667</td>
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