<template>
    <div>
        <label>職位名稱</label>
        <input type="text" id="txtName" v-model="name" /><br />
        <label>排列順序</label>
        <input type="text" id="txtOrderNo" v-model="orderNo" /><br />
        <label>是否啟用</label>
        <select id="isEnabled" v-model="isEnabled">
            <option value="true">是</option>
            <option value="false">否</option>
        </select><br />
        <button id="btnNew" @click="AddPosition">新增</button>
    </div>
</template>
<script>
    import axios from 'axios';
    import moment from 'moment';

    // Suppress the warnings
    moment.suppressDeprecationWarnings = true;

    export default {
        data() {
            return {
                name: '',
                orderNo: 0,
                isEnabled: true
            }
        },
        methods: {
            AddPosition() {
                const formData = new FormData();
                formData.append('_name', this.name);
                formData.append('_orderNo', this.orderNo);
                formData.append('_isEnabled', this.isEnabled);
                axios
                    .post('/Position/Create', formData)
                    .then(resp => {
                        console.log(resp);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            }
        },
    }
</script>
