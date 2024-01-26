<template>
    <div class="layout__content">
        <div class="container">
            <!-- START CONTENT -->
            <div class="d-flex justify-content-between mb-4">
                <h4 class="mb-0 align-self-center">主分類設定 - 新增</h4>
            </div>
            <section class="mb-4">
                <div class="card mb-4">
                    <div class="card-body">
                        <form>
                            <div class="form-row">
                                <div class="col-md-12 mb-3">
                                    <label for="town">縣市</label>
                                    <b-form-select v-model="city_selected" :options="city_options" class="form-control">
                                    </b-form-select><br />
                                </div>
                                <div class="col-md-12 mb-3">
                                    <label for="main-category">主分類名稱</label>
                                    <input type="text" class="form-control" id="main-category" v-model="mainTypeName" />
                                </div>
                                <div class="col-md-12 mb-3">
                                    <label for="order">主分類排序</label>
                                    <input type="text" class="form-control" id="order" v-model="orderNo" />
                                </div>
                                <div class="col-md-12 d-flex justify-content-end">
                                    <b-btn @click="AddMainType" class="btn btn-primary mr-3">確認</b-btn>
                                    <b-btn @click="returnToList" class="btn btn-secondary">取消</b-btn>
                                </div>
                            </div>
                        </form>
                    </div>
                </div>
            </section>
        </div>
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
                city_selected: [],
                city_options: ['value', 'text'],
                mainTypeName: '',
                orderNo: 0
            }
        },
        methods: {
            getCitySelect() {
                axios.post('/City/GetCitySelect')
                    .then(resp => {
                        console.log(resp.data);
                        this.city_options = resp.data.city;
                        this.city_options.unshift({ value: 0, text: '請選擇' });
                        this.city_selected = 0;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            AddMainType() {
                if (this.city_selected == 0) {
                    alert('請選擇縣市');
                    return;
                }
                if (this.orderNo == '' || this.orderNo == null) {
                    this.orderNo = 0;
                }
                const formData = new FormData();
                formData.append('_citySeq', this.city_selected);
                formData.append('_mainTypeName', this.mainTypeName);
                formData.append('_orderNo', this.orderNo);
                formData.append('_isEnabled', 1);
                axios
                    .post('/MainType/Create', formData)
                    .then(resp => {
                        if (resp.data.result > 0) {
                            alert('新增成功');
                            window.location.replace('/MainType/Index');
                        }
                        else {
                            alert('新增失敗');
                        }
                        console.log(resp);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            returnToList() {
                window.location.replace('/MainType/Index');
            }
        },
        mounted() {
            this.getCitySelect();
        }
    }
</script>