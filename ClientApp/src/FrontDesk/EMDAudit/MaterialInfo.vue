<template>
    <div>
        <h2>材料</h2>
        <div class="table-responsive">
            <table border="0" class="table table2 mt-0 min720">
                <tbody>
                    <tr>
                        <th>契約詳細表項次</th>
                        <td>{{emdSummary.ItemNo}}</td>
                        <th>契約數量</th>
                        <td>{{emdSummary.ContactQty}}</td>
                    </tr>
                    <tr>
                        <th>材料/設備名稱</th>
                        <td>{{emdSummary.MDName}}</td>
                        <th>是否取樣試驗</th>
                        <td>
                            <span v-if="emdSummary.IsSampleTest==null || emdSummary.IsSampleTest=='' || emdSummary.IsSampleTest=='0'">否</span>
                            <span v-if="emdSummary.IsSampleTest=='1'">是</span>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['emdSummary'],
        async mounted() {
            console.log('mounted() 材料');
        }
    }
</script>
