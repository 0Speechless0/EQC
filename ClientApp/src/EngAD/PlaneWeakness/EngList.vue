<template>
    <div v-if=" engData.items && engData.items.length > 0">
        <h5>{{engData.title}}</h5>
        <div class="table-responsive">
            <table class="table table-responsive-md table-hover">
                <thead class="insearch">
                    <tr>
                        <th class="text-left"><strong>年度</strong></th>
                        <th><strong>工程名稱</strong></th>
                        <th><strong>執行單位</strong></th>
                        <th><strong>決標經費</strong></th>
                        <th><strong>工程地點</strong></th>
                        <th><strong>品質弱面分析</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in engData.items" v-bind:key="item.Seq">
                        <td class="text-left"><strong>{{item.TenderYear}}</strong></td>
                        <td>{{item.TenderName}}</td>
                        <td>{{item.ExecUnitName}}</td>
                        <td>{{item.BidAmount}}</td>
                        <td>{{item.Location}}</td>
                        <td>{{item.PlaneWeakness}}</td>
                    </tr>
                </tbody>
            </table>
        </div>
        <p class="text-R my-1">
            * 品質弱面分析<br>
            1：重大(或重點防汛)、2：進度落後、3：決標比偏低、4：近年查核成績不佳、5：近5年曾發生重大職安事件、
            6：近5年履約計分偏低(含總分、職安及品質等)、7：近3年曾遭停權、8：近期承攬量能偏高、9：跨區域承攬、10：施工量能偏低、
            11：委外監造工程、12：成績不佳的委外監造廠商、13：高敏感區域工程、14：全民督工
        </p>
    </div>
</template>
<script>
    export default {
        props: ['engData'],
        async mounted() {
            console.log('mounted() 品質弱面追蹤與分析-工程');
        }
    }
</script>
