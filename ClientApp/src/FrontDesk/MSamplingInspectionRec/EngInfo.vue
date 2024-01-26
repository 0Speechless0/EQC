<template>
    <div class="">
        <h2>基本資料</h2>
        <div class="table-responsive mb-4">
            <table class="table table2 my-0" border="0">
                <tbody>
                    <tr>
                        <th>工程編號-工程名稱</th>
                        <td>{{engMain.EngNo}}-{{engMain.EngName}}</td>
                    </tr>
                    <tr>
                        <th>分項工程編號-分項工程</th>
                        <td>{{engMain.subEngNameSeq}}-{{engMain.subEngName}}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['engMain'],
        async mounted() {
            console.log('mounted() 工程資訊');
        }
    }
</script>
