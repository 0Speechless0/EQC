<template>
    <table class="table table-responsive-md table-hover VA-middle">
        <thead class="insearch">
            <tr>
                <th><strong>序號</strong></th>
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