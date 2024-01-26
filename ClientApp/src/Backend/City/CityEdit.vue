<template>
    <div>
        <form id="cityForm">
            <label>縣市名稱</label><input id="txtCityName" v-model="cityName" /><br />
            <label>排列順序</label><input id="intOrderNo" v-model="orderNo" /><br />
            <label>是否啟用</label>
            <select id="isEnabled" v-model="isEnabled">
                <option value="true">是</option>
                <option value="false">否</option>
            </select><br />

            <button id="btnNew" @click="AddCity">新增</button>
            <button id="btnCancel" @click="Cancel">取消</button>
        </form>
    </div>
</template>
<script>
    import axios from 'axios';

    export default {
        data() {
            return {
                cityName: '',
                orderNo: 0,
                isEnabled: true
            };
        },
        methods: {
            AddCity() {
                const formData = new FormData();
                formData.append('cityName', this.cityName);
                formData.append('orderNo', this.orderNo);
                formData.append('isEnabled', this.isEnabled);
                axios
                    .post('/City/Create', formData)
                    .then(resp => {
                        console.log(resp.data);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            }
        }
    }
</script>