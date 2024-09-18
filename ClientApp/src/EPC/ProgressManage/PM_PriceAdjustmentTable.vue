<template>
    <table class="table table-responsive-md table-hover VA-middle tableFixHead">
        <thead class="insearch">
            <tr>
                <th><strong>序號dd</strong></th>
                <th><strong>契約項目</strong></th>
                <th><strong>契約金額</strong></th>
                <th><strong>權重</strong></th>
                <th><strong>當月累計工作量</strong></th>
                <th><strong>工作金額</strong></th>
                <th><strong>權重金額</strong></th>
                <th><strong>物價指數%</strong></th>
                <th><strong>物價調整款</strong></th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="(item, index) in items" v-bind:key="item.Seq" class="bg-1-30">
                <td class="text-center"><strong>{{index+1}}</strong></td>
                <td style="font-size: 15px;">
                    {{item.PayItem}}<br />{{item.Description}}<font color="blue">({{item.ItemCode}})</font>
                </td>
                <td>{{item.Amount}}</td>
                <td>{{item.Weights}}</td>
                <td>{{item.MonthQuantity}}</td>
                <td>{{item.WorkAmount}}</td>
                <td>{{item.WeightsAmount}}</td>
                <template v-if="item.Kind == item.AdjKind">
                    <td>{{item.PriceIndex}}</td>
                    <td>{{item.PriceAdjustment}}</td>
                </template>
                <template v-if="item.Kind != item.AdjKind">
                    <td></td>
                    <td></td>
                </template>
            </tr>
        </tbody>
    </table>
</template>
<script>
    export default {
        props: ['items']
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
}
</style>