<template>
    <div>

        <b-table :items="items"
                 :fields="fields"
                 :sort-by.sync="sortBy"
                 :sort-desc.sync="sortDesc"
                 striped responsive="sm">

            <!--<template v-slot:[gomycell(b)]="row" v-for="b in Fields" slot-scope="{value, item}">
                <span v-if="!item.editing">
                </span>
                <span v-else>
                    <b-input v-model="item.temp[row.item.Seq]"></b-input>
                </span>

            </template>-->
            <template v-slot:cell(CityName)="row" slot-scope="{value, item}">
                <span v-if="!item.editing">
                    {{row.value}}
                </span>
                <span v-else>
                    <b-input v-model="row.item.CityName"></b-input>
                </span>
            </template>
            <template v-slot:cell(IsEnabled)="row" slot-scope="{ value, item }">
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
                    return field.editable === true
                })
            }
        },
        data() {
            return {
                sortBy: 'Seq',
                sortDesc: false,
                fields: [
                    { key: 'Seq', label: '序號', sortable: true, class: 'd-none' },
                    { key: 'CityName', label: '縣市名稱', sortable: true, class: '', editable: true },
                    { key: 'IsEnabled', label: '是否啟用', sortable: true, formatter: (value) => { return value ? '是' : '否' }, class: '' },
                    { key: 'OrderNo', label: '排列順序', sortable: true, class: '', editable: true },
                    { key: 'CreateTime', label: '建立時間', sortable: true, formatter: (value) => { return moment(value).format('YYYY/MM/DD HH:mm:ss') }, class: '' },
                    { key: 'ModifyTime', label: '異動時間', sortable: true, formatter: (value) => { return moment(value).format('YYYY/MM/DD HH:mm:ss') }, class: '' },
                    { key: 'actions', class: 'text-center' }
                ],
                items: ['Seq', 'CityName', 'IsEnabled', 'OrderNo', 'CreateTime', 'ModifyTime'],
                currentPage: 1,
                perPage: 15,
                totalRows: 0,
                pageOptions: [5, 10, 15],
                item: []
            };
        },
        methods: {
            getList() {
                var requestParams = {
                    page: this.currentPage,
                    per_page: this.perPage
                };
                if (this.sortBy) {
                    requestParams = Object.assign({ sort_by: this.sortBy }, requestParams);
                }
                axios.get('/City/GetList', { params: requestParams })
                    .then(resp => {
                        console.log(resp.data);
                        this.items = resp.data.l;
                        this.totalRows = resp.data.t;
                    });
            },
            doEdit(item) {
                this.$set(item, 'temp',
                    JSON.parse(JSON.stringify(item)))
                this.$set(item, 'editing', true)
            },
            doSave(rowitem, item) {
                this.$set(item, 'editing', false)
                axios.post('/City/Update', rowitem)
                    .then(resp => {
                        console.log(resp.data);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            doDelete(rowitem, item) {
                this.$set(item, 'editing', true)
                axios.post('/City/Delete', rowitem)
                    .then(resp => {
                        console.log(resp.data);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            doCancel(item) {
                this.$set(item, 'editing', false)
                this.$delete(item, 'temp')
            },
            gomycell(key) {
                console.log(`cell(${key})`);
                return `cell(${key})`;
            }
        },

        mounted() {

            this.getList()
            //.catch(error => {
            //    console.error(error)
            //});
        },
        watch: {
            currentPage: {
                handler: function (value) {
                    this.getList()
                    //.catch(error => {
                    //    console.error(error)
                    //})
                }
            }
        }
    }
</script>

