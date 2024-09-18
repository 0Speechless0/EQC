<template>
    <div class="tab-content">
        <div>
            <div class="table-responsive tableFixHead ">
                <table class="table table-responsive-md table-hover VA-middle">
                    <thead class="insearch">
                        <tr>
                            <th><strong>序號</strong></th>
                            <th><strong>項次</strong></th>
                            <th><strong>施工項目</strong></th>
                            <th><strong>編碼</strong></th>
                            <th class="text-right"><strong>碳排係數(kgCO2e)</strong></th>
                            <th><strong>單位</strong></th>
                            <th class="text-right"><strong>契約數量</strong></th>
                            <th class="text-right"><strong>單價(元)</strong></th>
                            <th class="text-right"><strong>金額(元)</strong></th>
                            <th v-if="sepHeader.SPState==0" style="text-align: center;width: 20px;">
                                <strong>功能</strong>
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in items" v-bind:key="item.Seq" v-bind:class="{'GreenFunding': item.GreenFundingSeq!=null, 'bg-1-30':item.GreenFundingSeq==null }">
                            <td><strong>{{pageRecordCount*(pageIndex-1)+index+1}}</strong></td>
                            <td><strong>{{item.PayItem}}</strong></td>
                            <td><strong>{{item.Description}}</strong></td>
                            <td><strong>{{item.Memo}}</strong></td>
                            <td class="text-right"><strong>{{item.KgCo2e}}</strong></td>
                            <td>{{item.Unit}}</td>
                            <td class="text-right">{{item.Quantity}}</td>
                            <td class="text-right">{{item.Price}}</td>
                            <td class="text-right">{{item.Amount}}</td>
                            <td v-if="sepHeader.SPState==0" style="text-align: center;">
                                <div class="d-flex justify-content-center">
                                    <button @click="onUnDelRecord(item)" class="btn btn-color11-3 btn-xs sharp mx-1" title="還原"><i class="fas fa-undo"></i></button>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['tenderItem','sepHeader'],
        data: function () {
            return {
                targetId: null,
                items: [],
                //分頁
                recordTotal: 0,
                pageRecordCount: 30,
                pageIndex: 1,
            };
        },
        methods: {
            //刪除
            onUnDelRecord(item) {
                if (confirm('是否還原資料？')) {
                    window.myAjax.post('/EPCSchEngProgress/UnDelRecord', { id: item.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getResords();
                            }
                            alert(resp.data.msg);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            //清單
            getResords() {
                this.items = [];
                window.myAjax.post('/EPCSchEngProgress/GetDelList', { id: this.targetId })
                .then(resp => {
                    if (resp.data.result >= 0) {
                        this.items = resp.data.items;
                    }
                })
                .catch(err => {
                    console.log(err);
                });
            },
            //取標案
            getItem() {
                if (this.targetId == null) {
                    alert('請先選取標案');
                    return;
                }
                this.getResords();
            },
        },
        async mounted() {
            console.log('mounted() 前置作業-碳排刪除清單');
            this.targetId = this.tenderItem.Seq;
            this.getItem();
        },
    }
</script>
<style scoped>
.tableFixHead          { overflow: auto; max-height: 500px;   }
table {
    border-collapse: separate;
    border-spacing: 0;
}
.table {
    margin : 0;
}
.tableFixHead thead  { position: sticky !important ; top: 0 !important ; z-index: 1 !important;     }
th {
    border : 0;
    border-bottom: #ddd solid 1px !important; 
    border-left : 0 !important;
    border-right:0 !important;
}
td {
    z-index: 0;
    position: relative;
}</style>