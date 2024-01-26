<template>
    <div class="layout__content">
        <div class="container">
            <!-- START CONTENT -->
            <div class="d-flex justify-content-between mb-4">
                <h4 class="mb-0 align-self-center">主分類設定</h4>
                <b-btn class="btn btn-primary" @click="newMainType">新增</b-btn>
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
                                    <b-form-select id="town" v-model="city_selected" :options="city_options" @change="getMainTypeSelect"></b-form-select>
                                </div>
                                <div class="col-md-12 mb-3">
                                    <label for="main-category">主分類</label>
                                    <b-form-input v-model="mainTypeName" class="form-control"></b-form-input>
                                </div>
                                <div class="col-md-12 d-flex justify-content-end">
                                    <b-btn @click="getList">查詢</b-btn>
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
                                     :sort-by.sync="sortBy"
                                     :sort-desc.sync="sortDesc"
                                     striped responsive="sm"
                                     variant="table table-bordered table-hover"
                                     head-variant="light">
                                <template v-slot:cell(CityName)="row" slot-scope="{value, item}">
                                    <span v-if="!item.editing">
                                        {{row.value}}
                                    </span>
                                    <span v-else>
                                        <b-form-select v-model="row.item.CitySeq" :options="city_options">
                                        </b-form-select>
                                    </span>
                                </template>
                                <template v-slot:cell(MainTypeName)="row" slot-scope="{value, item}">
                                    <span v-if="!item.editing">
                                        {{row.value}}
                                    </span>
                                    <span v-else>
                                        <b-input v-model="row.item.MainTypeName"></b-input>
                                    </span>
                                </template>
                                <template v-slot:cell(IsEnabled)="row" slot-scope="{value, item}">
                                    <span v-if="!item.editing">
                                        <span v-if="row.value === '是'">✔</span>"
                                        <span v-else>✖</span>
                                    </span>
                                    <span v-else>
                                        <b-checkbox v-model="row.item.IsEnabled"></b-checkbox>
                                    </span>
                                </template>
                                <template v-slot:cell(OrderNo)="row" slot-scope="{value, item}">
                                    <span v-if="!item.editing">
                                        {{row.vlaue}}
                                    </span>
                                    <span v-else>
                                        <b-input v-model="row.item.OrderNo"></b-input>
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
        computed: {
            editableFields() {
                return this.fields.filter(field => {
                    return field.editable == true
                })
            }
        },
        data() {
            return {
                sortBy: 'Seq',
                sortDesc: false,
                fields: [
                    { key: 'Seq', label: '序號', sortable: true, class: 'd-none' },
                    { key: 'CitySeq', label: '縣市序號', sortable: true, class: 'd-none', editable: true },
                    { key: 'CityName', label: '縣市', sortable: true, class: '', editable: true },
                    { key: 'MainTypeName', label: '名稱', sortable: true, editable: true, class: '' },
                    { key: 'IsEnabled', label: '是否啟用', sortable: true, formatter: (value) => { return value ? '是' : '否' }, class: '' },
                    { key: 'OrderNo', label: '排列順序', sortable: true, class: '', editable: true },
                    { key: 'CreateTime', label: '建立時間', sortable: true, formatter: (value) => { return moment(value).format('YYYY/MM/DD HH:mm:ss') }, class: '' },
                    { key: 'ModifyTime', label: '異動時間', sortable: true, formatter: (value) => { return moment(value).format('YYYY/MM/DD HH:mm:ss') }, class: '' },
                    { key: 'actions', label: '', class: 'text-center' }
                ],
                items: ['Seq', 'CitySeq', 'CityName', 'MainTypeName', 'IsEnabled', 'OrderNo', 'CreateTime', 'CreateUser', 'ModifyTime', 'ModifyUser'],
                currentPage: 1,
                perPage: 15,
                totalRows: 0,
                pageOptions: [5, 10, 15],
                item: [],
                city_selected: [],
                city_options: ['value', 'text'],

                mainTypeName: '',
            };
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
                formData.append('mainTypeName', this.mainTypeName);
                axios.post('/MainType/GetList', formData)
                    .then(resp => {
                        console.log(resp.data);
                        this.items = resp.data.l;
                        this.totalRows = resp.data.t;
                        //this.city_options = resp.data.city;
                        //console.log(this.city_options);
                    })
                    .then(err => {
                        console.log(err);
                    });
            },

            getCitySelect() {
                axios.post('/City/GetCitySelect')
                    .then(resp => {
                        console.log(resp.data.city);
                        this.city_options = resp.data.city;
                        this.city_options.unshift({ value: 0, text: '請選擇' });
                        this.city_selected = 0;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },

            doEdit(item) {
                this.$set(item, 'temp', JSON.parse(JSON.stringify(item)))
                this.$set(item, 'editing', true)
            },
            doSave(rowitem, item) {
                this.$set(item, 'editing', false)
                axios.post('/MainType/Update', rowitem)
                    .then(resp => {
                        console.log(resp.data);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            doDelete(rowitem, item) {
                axios.post('/MainType/Delete', rowitem)
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
            newMainType() {
                window.location.replace('/MainType/Create');
            },
            gomycell(key) {
                console.log(`cell(${key})`);
                return `cell(${key})`;
            }
        },
        mounted() {
            this.getCitySelect();
            //this.getList();
            this.items = null;
        },
        watch: {
            currentPage: {
                handler: function (value) {
                    this.getList()
                }
            }
        }
    }
</script>