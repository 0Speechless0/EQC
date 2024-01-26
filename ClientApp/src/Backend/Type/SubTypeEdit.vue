<template>
    <div class="layout__content">
        <div class="container">
            <!-- START CONTENT -->
            <div class="d-flex justify-content-between mb-4">
                <h4 class="mb-0 align-self-center">次分類設定 - 新增</h4>
            </div>
            <section class="mb-4">
                <div class="card mb-4">
                    <div class="card-body">
                        <form>
                            <div class="form-row">
                                <div class="col-md-12 mb-3">
                                    <label for="town">縣市</label>
                                    <b-form-select id="town" class="form-control" v-model="city_selected" :options="city_options" @change="getMainTypeSelect">
                                    </b-form-select>
                                </div>
                                <div class="col-md-12 mb-3">
                                    <label for="main-category">主分類名稱</label>
                                    <b-form-select id="main-category" class="form-control" v-model="maintype_selected" :options="maintype_options">
                                    </b-form-select>
                                </div>
                                <div class="col-md-12 mb-3">
                                    <label for="sub-category">次分類名稱</label>
                                    <input type="text" id="sub-category" class="form-control" v-model="subTypeName" placeholder="請輸入"/>
                                </div>
                                <div class="col-md-12 mb-3">
                                    <label for="order">次分類排序</label>
                                    <input type="text" id="order" class="form-control" v-model="orderNo"  placeholder="請輸入"/>
                                </div>
                                <!--<div class="col-md-12 d-flex justify-content-end">
                                    <label for="txtDeadLine">逾期天數</label>
                                    <input type="text" id="txtDeadLine" class="form-control" v-model="deadLine" />
                                </div>-->
                                <div class="col-md-12 d-flex justify-content-end">
                                    <b-button id="btnNew" @click="AddSubType" variant="btn btn-primary mr-3">確認</b-button>
                                    <b-button @click="returnToList" variant="btn btn-primary">取消</b-button>
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
    moment.suppressDeprecationWarning = true;

    export default {
        data() {
            return {
                city_selected: [],
                city_options: ['value', 'text'],

                maintype_selected: [],
                maintype_options: ['value', 'text'],

                subTypeName: '',
                orderNo: 0,
                deadLine: 14,   // 20201125-固定給14天
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
            getMainTypeSelect() {
                axios.post('/MainType/GetEnabledMainType', { citySeq: this.city_selected })
                    .then(resp => {
                        console.log(resp.data.maintype)
                        this.maintype_options = resp.data.maintype
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            AddSubType() {
                if (this.city_selected == 0) {
                    alert('請選擇縣市');
                    return;
                }
                if (this.subTypeName == '') {
                    alert('請輸入次類別名稱');
                    return;
                }
                if (this.deadLine == 0) {
                    alert('請輸入逾期天數');
                    return;
                }
                if (this.orderNo == '' || this.orderNo == null) {
                    this.orderNo = 0;
                }
                const formData = new FormData();
                formData.append('_citySeq', this.city_selected);
                formData.append('_maintypeSeq', this.maintype_selected);
                formData.append('_subTypeName', this.subTypeName);
                formData.append('_orderNo', this.orderNo);
                formData.append('_deadLine', this.deadLine);
                axios.post('/SubType/Create', formData)
                    .then(resp => {
                        if (resp.data.result > 0) {
                            alert('新增完成');
                            window.location.replace('/SubType/Index');
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
                window.location.replace('/SubType/Index');
            }
        },
        mounted() {
            this.getCitySelect();
            if (this.city_selected.length == 0) {
                this.maintype_options = null;
            }
        }
    }
</script>