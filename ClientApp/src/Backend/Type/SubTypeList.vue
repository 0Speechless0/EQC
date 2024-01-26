<template>
    <div class="layout__content">
        <div class="container">
            <!-- START CONTENT -->
            <div class="d-flex justify-content-between mb-4">
                <h4 class="mb-0 align-self-center">次分類設定</h4>
                <div class="btn-group-toggle align-self-center">
                    <b-button @click="newSubType" class="btn-primary">新增</b-button>
                </div>
            </div>
            <section class="mb-4">
                <div class="card mb-4">
                    <div class="card-body">
                        <form>
                            <div class="d-flex justify-content-between mb-4">
                                <h5 class="align-self-center">資料篩選</h5>
                            </div>
                            <div class="form-row">
                                <div class="col-md-12 mb-3">
                                    <label for="town">縣市</label>
                                    <b-form-select id="town" v-model="city_selected" :options="city_options" @change="getMainTypeSelect" class="custom-select">
                                    </b-form-select>
                                </div>
                                <div class="col-md-12 mb-3">
                                    <label for="main-category">主分類：</label>
                                    <b-form-select id="main-category" class="form-control" v-model="maintype_selected" :options="maintype_options"></b-form-select>
                                </div>
                                <div class="col-md-12 mb-3">
                                    <label for="sub-category">次分類：</label>
                                    <b-input id="sub-category" class="form-control" v-model="subTypeName"></b-input>
                                </div>
                                <div class="col-md-12 d-flex justify-content-end">
                                    <b-btn id="btnQuery" @click="getList" class="btn btn-secondary">查詢</b-btn>
                                </div>
                            </div>
                        </form>
                    </div>
                </div>
            </section>

            <section>
                <div class="card">
                    <div class="card-body">
                        <div class="table-responsive">
                            <b-table :items="items"
                                     :fields="fields"
                                     :sort_by.sync="sortBy"
                                     :sort-desc.sync="sortDesc"
                                     striped responsive="sm"
                                     variant="table table-bordered table-hover"
                                     head-variant="light">
                                <template v-slot:cell(OrderNo)="row" slot-scope="{value, item}">
                                    <span v-if="!item.editing">
                                        {{row.value}}
                                    </span>
                                    <span v-else>
                                        <b-input v-model="row.item.OrderNo"></b-input>
                                    </span>
                                </template>
                                <template v-slot:cell(CityName)="row" slot-scope="{value, item}">
                                    <span>
                                        {{row.value}}
                                    </span>
                                </template>
                                <template v-slot:cell(MainTypeName)="row" slot-scope="{value, item}">
                                    <span>
                                        {{row.value}}
                                    </span>
                                </template>
                                <template v-slot:cell(SubTypeName)="row" slot_scope="{value, item}">
                                    <span v-if="!item.editing">
                                        {{row.value}}
                                    </span>
                                    <span v-else>
                                        <b-input v-model="row.item.SubTypeName"></b-input>
                                    </span>
                                </template>
                                <!--<template v-slot:cell(DeadLine)="row" slot_scope="{value, item}">
                                    <span v-if="!item.editing">
                                        {{row.value}}
                                    </span>
                                    <span v-else>
                                        <b-input v-model="row.item.DeadLine"></b-input>
                                    </span>
                                </template>-->
                                <template v-slot:cell(CreateTime)="row" slot-scope="{value, item}">
                                    <span v-if="!item.editing">
                                        {{row.value}}
                                    </span>
                                </template>
                                <template v-slot:cell(ModifyTime)="row" slot-scope="{value, item}">
                                    <span v-if="!item.editing">
                                        {{row.value}}
                                    </span>
                                </template>
                                <template v-slot:cell(actions)="row" slot-scope="{item}">
                                    <b-btn v-if="!item.editing"
                                           @click="doEdit(item)" variant="primary">
                                        編輯
                                    </b-btn>
                                    <b-btn v-if="item.editing"
                                           @click="doSave(row.item, item)" variant="success">
                                        儲存
                                    </b-btn>
                                    <b-btn v-if="!item.editing"
                                           @click="doDelete(row.item, item)" variant="danger">
                                        刪除
                                    </b-btn>
                                    <b-btn v-if="item.editing"
                                           @click="doCancel(item)" variant="info">
                                        取消
                                    </b-btn>
                                </template>
                            </b-table>
                        </div>
                        <div class="d-flex justify-content-center mt-4">
                            <b-pagination v-on:change="onPageChange"
                                          :total-rows="totalRows"
                                          :per-page="perPage"
                                          v-model="currentPage">

                            </b-pagination>
                        </div>
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
                sortBy: 'SubTypeSeq',
                sortDesc: false,
                fields: [
                    { key: 'OrderNo', label: '排列順序', sortable: true, class: '', editable: true },
                    { key: 'CitySeq', label: '縣市序號', sortable: false, class: 'd-none', editable: false },
                    { key: 'CityName', label: '縣市', sortable: true, class: '', editable: false },
                    { key: 'MainTypeSeq', label: '主分類序號', sortable: false, class: 'd-none', editable: false },
                    { key: 'MainTypeName', label: '主分類', sortable: true, class: '', editable: false },
                    { key: 'SubTypeSeq', label: '次分類序號', sortable: false, class: 'd-none', editable: false },
                    { key: 'SubTypeName', label: '次分類', sortable: true, class: '', editable: true },
                    //{ key: 'DeadLine', label: '逾期天數', sortable: true, class: '', editable: true },
                    { key: 'CreateTime', label: '建立時間', sortable: true, formatter: (value) => { return moment(value).format('YYYY/MM/DD HH:mm:ss') }, class: '', editable: false },
                    { key: 'ModifyTime', label: '異動時間', sortable: true, formatter: (value) => { return moment(value).format('YYYY/MM/DD HH:mm:ss') }, class: '', editable: false },
                    { key: 'actions', label:'', class: 'text-center' }
                ],
                items: ['OrderNo', 'CitySeq', 'CityName', 'MainTypeSeq', 'MainTypeName', 'SubTypeSeq', 'SubTypeName', 'DeadLine', 'CreateTime', 'CreateUser', 'ModifyTime', 'ModifyUser'],
                currentPage: 1,
                perPage: 15,
                totalRows: 0,
                pageOptions: [5, 10, 15],
                item: [],

                city_selected: [],
                city_options: ['value', 'text'],

                maintype_selected: [],
                maintype_options: ['value', 'text'],

                subTypeName: ''
            }
        },
        methods: {
            getList() {
                var formData = new FormData();
                formData.append('page', this.currentPage);
                formData.append('per_page', this.perPage);

                if (this.sortBy) {
                    formData.append('sort_by', this.sortBy);
                }

                formData.append('citySeq', this.city_selected == null ? 0 : this.city_selected);
                formData.append('mainTypeSeq', this.maintype_selected == null ? 0 : this.maintype_selected);
                formData.append('subTypeName', this.subTypeName);
                //var requestParams = {
                //    page: this.currentPage,
                //    per_page: this.perPage,
                //    citySeq: this.city_selected == null ? 0 : this.city_selected,
                //    mainTypeSeq: this.maintype_selected == null ? 0 : this.maintype_selected,
                //    subTypeName: this.subTypeName
                //};
                //if (this.sortBy) {
                //    requestParams = Object.assign({ sort_by: this.sortBy }, requestParams);
                //}
                axios.post('/SubType/GetList', formData)
                    .then(resp => {
                        console.log(resp.data);
                        this.items = resp.data.l;
                        this.totalRows = resp.data.t;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            getCitySelect() {
                axios.post('/City/GetCitySelect')
                    .then(resp => {
                        console.log(resp.data.city)
                        this.city_options = resp.data.city;
                        this.city_options.unshift({ value: 0, text: '請選擇' });
                        this.city_selected = 0;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            getMainTypeSelect() {
                if (this.city_selected == 0) {
                    this.maintype_options = null;
                    return;
                }
                //var formData = new FormData();
                //formData.append("_citySeq", this.city_selected);
                axios.post('/MainType/GetEnabledMainType', { citySeq: this.city_selected })
                    .then(resp => {
                        console.log(resp.data.maintype)
                        this.maintype_options = resp.data.maintype;
                        this.maintype_options.unshift({ value: 0, text: '請選擇' });
                        this.maintype_selected = 0;
                    });
            },
            doEdit(item) {
                this.$set(item, 'temp', JSON.parse(JSON.stringify(item)))
                this.$set(item, 'editing', true)
            },
            doSave(rowitem, item) {
                this.$set(item, 'editing', false)
                axios.post('/SubType/Update', rowitem)
                    .then(resp => {
                        console.log(resp.data);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            doDelete(rowitem, item) {
                console.log(rowitem);
                axios.post('/SubType/Delete', rowitem)
                    .then(resp => {
                        if (resp.data.result > 0) {
                            alert('刪除成功');
                        }
                        else {
                            alert('刪除失敗');
                        }
                        console.log(resp.data);
                    })
                    .catch(err => {
                        console.log(err);
                    });
                this.$set(item, 'editing', false);
                this.getList();
            },
            doCancel(item) {
                this.$set(item, 'editing', false);
                this.$delete(item, 'temp');
            },
            newSubType() {
                window.location.replace('/SubType/Create');
            }
        },
        mounted() {
            this.getCitySelect();
            if (this.city_selected.length == 0) {
                this.maintype_options = null;
            }
            this.items = null;
        },
        watch: {
            currentPage: {
                handler: function (value) {
                    this.getList();
                }
            }
        }
    }
</script>