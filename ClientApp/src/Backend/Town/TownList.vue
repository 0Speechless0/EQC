<template>
    <div>
        <div>
            <label>縣市：</label>
            <b-form-select v-model="city_selected" :options="city_options" @change="getTownSelect"></b-form-select>
        </div>
        <div>
            <b-table :items="items"
                     :fields="fields"
                     :sort_by.sync="sortBy"
                     :sort-desc.sync="sortDesc"
                     striped responsive="sm">
                <template v-slot:cell(TownName)="row" slot-scope="{value, item}">
                    <span v-if="!item.editing">
                        {{row.value}}
                    </span>
                    <span v-else>
                        <b-input v-model="row.item.TownName"></b-input>
                    </span>
                </template>
                <template v-slot:cell(IsEnabled)="row" slot-scope="{value, item}">
                    <span v-if="!item.editing">
                        <span v-if="row.value === '是'">✔</span>
                        <span v-else>✖</span>
                    </span>
                    <span v-else>
                        <b-checkbox v-model="row.item.IsEnabled"></b-checkbox>
                    </span>
                </template>
                <template v-slot:cell(OrderNo)="row" slot-scope="{value, item}">
                    <span v-if="!item.editing">
                        {{row.value}}
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
            <b-pagination v-on:change="onPageChange"
                          :total-rows="totalRows"
                          :per-page="perPage"
                          v-model="currentPage">

            </b-pagination>
            <div>
                目前的排序鍵值: <b>{{ sortBy }}</b>, 排序方式:
                <b>{{ sortDesc ? 'Descending' : 'Ascending' }}</b>
            </div>
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
                sortBy: 'Seq',
                sortDesc: false,
                fields: [
                    { key: 'Seq', label: '序號', sortable: true, class: 'd-none', editable: false },
                    { key: 'TownName', label: '鄉鎮市名稱', sortable: true, class: '', editable: true },
                    { key: 'CitySeq', label: '縣市序號', sortable: false, class: 'd-none', editable: false },
                    { key: 'IsEnabled', label: '是否啟用', sortable: true, class: '', editable: true },
                    { key: 'OrderNo', label: '排列順序', sortable: true, class: '', editable: true },
                    { key: 'CreateTime', label: '建立時間', sortable: true, formatter: (value) => { return moment(value).format('YYYY/MM/DD HH:mm:ss') }, class: '', editable: false },
                    { key: 'ModifyTime', label: '異動時間', sortable: true, formatter: (value) => { return moment(value).format('YYYY/MM/DD HH:mm:ss') }, class: '', editable: false },
                    { key: 'actions', class: 'text-center' }
                ],
                items: ['Seq', 'TownName', 'CitySeq', 'IsEnabled', 'OrderNo', 'CreateTime', 'CreateUser', 'ModifyTime', 'ModifyUser'],
                currentPage: 1,
                perPage: 15,
                totalRows: 0,
                pageOptions: [5, 10, 15],
                item: [],

                city_selected: [],      // 預設嘉義縣
                city_options: ['value', 'text'],

            }
        },
        methods: {
            getTownSelect() {
                var formData = new FormData();
                formData.append('page', this.currentPage);
                formData.append('per_page', this.perPage);
                if (this.sortBy) {
                    formData.append('sort_by', this.sortBy);
                }
                formData.append('citySeq', this.city_selected);
                axios.post('/Town/GetList', formData)
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
                        console.log(resp.data.city);
                        this.city_options = resp.data.city;
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
                axios.post('/Role/Update', rowitem)
                    .then(resp => {
                        console.log(resp.data);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            doDelete(rowitem, item) {
                this.$set(item, 'editing', true);
                axios.post('/Role/Delete', rowitem)
                    .then(resp => {
                        console.log(resp.data);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            doCancel(item) {
                this.$set(item, 'editing', false);
                this.$delete(item, 'temp');
            },
            gomycell(key) {
                console.log(`cell(${key})`);
                return `cell(${key})`;
            }
        },
        mounted() {
            this.getCitySelect();
            this.city_selected = 12;
            this.getTownSelect();
            this.items = null;
        },
        watch: {
            currentPage: {
                handler: function () {
                    this.getTownSelect();
                }
            }
        }
    }
</script>