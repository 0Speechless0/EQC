<template>
    <div class="">
        <h2>工程基本資料</h2>
        <div class="table-responsive">
            <table class="table table2 min910" border="0">
                <tbody>
                    <tr>
                        <th>工程編號</th>
                        <td>{{engMain.EngNo}}</td>
                        <th>工程名稱</th>
                        <td>{{engMain.EngName}}</td>
                    </tr>
                    <tr>
                        <th>主辦機關</th>
                        <td>{{engMain.organizerUnitName}}</td>
                        <th>核定日期</th>
                        <td>{{engMain.chsApproveDate}}</td>
                    </tr>
                    <tr>
                        <th>執行機關</th>
                        <td>{{engMain.execUnitName}}</td>
                        <th>核定文號</th>
                        <td>{{engMain.ApproveNo}}</td>
                    </tr>
                    <tr>
                        <th>監造單位</th>
                        <td>{{engMain.SupervisorUnitName}}</td>
                        <th></th>
                        <td></td>
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
