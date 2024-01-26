<template>
    <div>
        <b-table :items="items"
                 :fields="fields"
                 :sort-by.sync="sortBy"
                 :sort-desc.sync="sortDesc"
                 stripted responsive="sm">
            <template v-slot:cell(Code)="row" slot-scope="{value, item}">
                <span v-if="!item.editing">
                    {{row.value}}
                </span>
                <span v-else>
                    <b-input v-model="row.item.Code"></b-input>
                </span>
            </template>
            <template v-slot:cell(Name)="row" slot-scope="{value, item}">
                <span v-if="!item.editing">
                    {{row.value}}
                </span>
                <span v-else>
                    <b-input v-model="row.item.Name"></b-input>
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
            <template v-slot:cell(IsEnabled)="row" slot-scope="{value, item}">
                <span v-if="!item.editing">
                    <span v-if="row.value === '是'">✔</span>"
                    <span v-else>✖</span>
                </span>
                <span v-else>
                    <b-checkbox v-model="row.item.IsEnabled"></b-checkbox>
                </span>
            </template>
            <template v-slot:cell(IsSubUnit)="row" slot-scope="{value, item}">
                <span v-if="!item.editing">
                    {{row.value}}
                </span>
                <span v-else>
                    <b-checkbox v-model="row.item.IsSubUnit"></b-checkbox>
                </span>
            </template>
            <template v-slot:cell(IsRegTable)="row" slot-scope="{value, item}">
                <span v-if="!item.editing">
                    {{row.value}}
                </span>
                <span v-else>
                    <b-checkbox v-model="row.item.IsRegTable"></b-checkbox>
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
                    return field.editable == true
                })
            }
        },
        data() {
            return {
                sortBy: 'Seq',
                sortDesc: false,

                fields: [
                    { key: 'Seq', label: '序號', sortable: false, class: 'd-none', editable: false },
                    { key: 'ParentSeq', label: '父層序號', sortable: false, class: 'd-none', editable: false },
                    {key: 'Code', label: '單位代碼', sortable:true, class: '', editable:true},
                    { key: 'Name', label: '角色名稱', sortable: true, class: '', editable: true },
                    { key: 'OrderNo', label: '排列順序', sortable: true, class: '', editable: true },
                    { key: 'IsEnabled', label: '是否啟用', sortable: true, class: '', editable: true },
                    { key: 'IsSubUnit', label: '是否為分派單位', sortable: true, class: '', editable: true },
                    { key: 'IsRegTable', label: '是否為登記桌', sortable: true, class: '', editable: true },
                    { key: 'CreateTime', label: '建立時間', sortable: true, formatter: (value) => { return moment(value).format('YYYY/MM/DD HH:mm:ss') }, class: '' },
                    { key: 'ModifyTime', label: '異動時間', sortable: true, formatter: (value) => { return moment(value).format('YYYY/MM/DD HH:mm:ss') }, class: '' },
                    { key: 'actions', class: 'text-center' }
                ],
                items: ['Seq', 'ParentSeq', 'Code', 'Name', 'OrderNo', 'IsEnabled', 'IsSubUnit', 'IsRegTable', 'CreateTime', 'CreateUser', 'ModifyTime', 'ModifyUser'],
                currentPage: 1,
                perPage: 15,
                totalRows: 0,
                pageOptions: [5, 10, 15],
                item: [],
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
                axios.post('/Unit/GetList', formData)
                    .then(resp => {
                        console.log(resp.data);
                        this.items = resp.data.l;
                        this.totalRows = resp.data.t;
                    })
                    .then(err => {
                        console.log(err);
                    });
            },
            doEdit(item) {
                this.$set(item, 'temp', JSON.parse(JSON.stringify(item)))
                this.$set(item, 'editing', true)
            },
            doSave(rowitem, item) {
                this.$set(item, 'editing', false)
                axios.post('/Unit/Update', rowitem)
                    .then(resp => {
                        console.log(resp.data);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            doDelete(rowitem, item) {
                this.$set(item, 'editing', false);
                axios.post('/Unit/Delete', rowitem)
                    .then(resp => {
                        this.getList();
                        alert('刪除成功');
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
            this.getList();
            this.items = null;
        },
        watch: {
            currentPage: {
                handler: function () {
                    this.getList();
                }
            }
        }
    }
</script>