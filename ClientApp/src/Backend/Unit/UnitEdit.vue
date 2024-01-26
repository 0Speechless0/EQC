<template>
    <div>
        <form @submit="addUnit">
            <label>父層單位：</label>
            <b-form-select v-model="unit_selected" :options="unit_options" @change="getUnitSelect"></b-form-select>
            <label>單位代碼</label>
            <input type="text" id="txtCode" v-model="code" required /><br />
            <label>單位名稱</label>
            <input type="text" id="txtName" v-model="name" required /><br />
            <label>排列順序(0:自動置於最後)</label>
            <input type="text" id="txtOrderNo" v-model="orderNo" required/><br />
            <b-checkbox v-model="isSubUnit">是否為分派單位</b-checkbox><br />
            <b-checkbox v-model="isRegTable">是否有登記桌</b-checkbox><br />

            <button id="btnNew" type="submit">新增</button>
        </form>
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
                parentSeq: 0,
                code: '',
                name: '',
                orderNo: 0,
                isEnabled: true,
                isSubUnit: false,
                isRegTable: false,

                unit_selected: [],  // 單位
                unit_options: ['value', 'text'],

            }
        },
        methods: {
            addUnit() {
                const formData = new FormData();
                formData.append('_parentSeq', this.unit_selected);
                formData.append('_code', this.code);
                formData.append('_name', this.name);
                formData.append('_orderNo', this.orderNo);
                formData.append('_isEnabled', this.isEnabled);
                formData.append('_isSubUnit', this.isSubUnit);
                formData.append('_isRegTable', this.isRegTable);
                axios
                    .post('/Unit/Create', formData)
                    .then(resp => {
                        console.log(resp);
                        this.getUnitSelect();
                        alert('新增完成');
                        window.location.replace('/Unit/Index');
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            getUnitSelect() {
                this.unit_options = null;
                axios.post('/Unit/GetEnabledUnit')
                    .then(resp => {
                        this.unit_options = resp.data.unit;
                        this.unit_options.unshift({ value: 0, text: '無父層' });
                    })
            },
        },
        mounted() {
            this.getUnitSelect();
        }
    }
</script>