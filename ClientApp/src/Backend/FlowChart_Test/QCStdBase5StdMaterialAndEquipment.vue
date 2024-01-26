    <template>
    <div>
        <div v-if="loading">
            Loading...
        </div>
        
        <div class="table-responsive" id="showBlock" style="display:none;">
            <table class="table table3 min910">
                <thead>
                    <tr>
                        <th class="sort">順序</th>
                        <th class="twoword">檢驗項目</th>
                        <th class="twoword">檢驗標準</th>
                        <th class="twoword">檢驗時機</th>
                        <th class="twoInput">檢驗方法</th>
                        <th class="twoword">檢驗頻率</th>
                        <th class="twoword">不符合之處置方法</th>
                        <th class="twoword">管理紀錄</th>
                        <th class="twoword">備註</th>
                    </tr>
                </thead>

                <tbody>

                    <tr v-for="(item, index) in items" v-bind:key="item.id" v-show="!editFlag || item.edit">
                        <td>
                            <div v-if="!item.edit">{{item.OrderNo}}</div>
                            <input v-if="item.edit" type="text" v-model.number="item.OrderNo" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.MDTestItem}}</div>
                            <input maxlength="50" v-if="item.edit" type="text" v-model.trim="item.MDTestItem" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.MDTestStand1}}</div>
                            <textarea rows="5" maxlength="100" v-if="item.edit" type="text" v-model.trim="item.MDTestStand1" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.MDTestTime}}</div>
                            <input maxlength="100" v-if="item.edit" type="text" v-model.trim="item.MDTestTime" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.MDTestMethod}}</div>
                            <textarea rows="5" maxlength="100" v-if="item.edit" type="text" v-model.trim="item.MDTestMethod" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.MDTestFeq}}</div>
                            <textarea rows="5" maxlength="100" v-if="item.edit" type="text" v-model.trim="item.MDTestFeq" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.MDIncomp}}</div>
                            <textarea rows="5" maxlength="100" v-if="item.edit" type="text" v-model.trim="item.MDIncomp" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.MDManageRec}}</div>
                            <input maxlength="100" v-if="item.edit" type="text" v-model.trim="item.MDManageRec" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.MDMemo}}</div>
                            <div v-if="item.edit" class="row justify-content-center m-0">
                                <a v-bind:data-target="'#M_Col'+item.Seq" data-toggle="modal" href="#" class="btn btn-color7" title="編輯"><i class="fas fa-edit"></i></a>
                            </div>

                            <div class="modal fade" v-bind:id="'M_Col'+item.Seq" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
                                <div class="modal-dialog modal-dialog-centered" role="document">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <h5 class="modal-title">備註編輯</h5>
                                        </div>
                                        <div class="modal-body">
                                            <textarea class="form-control" maxlength="255" v-if="item.edit" type="text" v-model.trim="item.MDMemo" />
                                        </div>
                                        <div class="modal-footer">
                                            <button type="button" class="btn btn-color3" data-dismiss="modal">Close</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
               
                    </tr>

                </tbody>
            </table>
        </div>
        
    </div>
</template>
<script>
    import $ from 'jquery'
    export default {
        props: ['op1','op2'],
        watch: {
            'op1': function (nval, oval) {
                console.log('watch op1 :' + oval + ' >> ' + nval);
               // if (this.op1 != null) { this.getList(); $("#showBlock").show(); }
            },
            'op2': function (nval, oval) {
                if ((this.op2 % 2) != 0) { this.getList(); $("#showBlock").show();} else { $("#showBlock").hide();}
           },
            pageIndex: {
                handler: function (value) {
                    this.getList();
                }
            }
        },
        data: function () {
            return {
                editFlag: false,
                loading: false,
                items: [],
                //分頁
                pageIndex: 1,
                perPage: 3,
                totalRows: 0,
            };
        },
        methods: {
            getList() {
                this.editFlag = false;
                this.loading = true;
                this.items = [];
                let params = { op1: this.op1, pageIndex: this.pageIndex, perPage: this.perPage };
                console.log(this.op1);
                window.myAjax.post('/QCStdTp/Chapter5_AllList', params)
                    .then(resp => {
                        this.items = resp.data.items;
                        this.totalRows = resp.data.pTotal;
                        this.loading = false;
                    })
                    .catch(err => {
                        this.loading = false;
                        console.log(err);
                    });
            },
            editItem(item) {
                if (this.editFlag) return;

                this.editFlag = true;
                item.edit = this.editFlag;
            },
            newItem() {
                if (this.editFlag) return;
                this.editFlag = true;
                var newRow = {
                    Seq: -1 * Date.now(),
                    EngMaterialDeviceListTpSeq: this.op1,
                    MDTestItem: '',
                    MDTestStand1: '',
                    MDTestStand2: '',
                    MDTestTime: '',
                    MDTestMethod: '',
                    MDTestFeq: '',
                    MDIncomp: '',
                    MDManageRec: '',
                    MDMemo: '',
                    OrderNo: 999,
                    edit: true
                };
                this.items.push(newRow);
            },
            saveItem(item) {
                if (item.OrderNo === '') {
                    alert('[順序]欄位須輸入資料');
                    return;
                }
                //console.log(item);
                if (item.Seq < 0) {
                    window.myAjax.post('/QCStdTp/Chapter5Add', { item: item })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                item.Seq = resp.data.Seq;
                                item.edit = false;
                                this.editFlag = false;
                            }
                            this.sortItems();
                            alert(resp.data.message);
                            console.log(resp);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                } else {
                    window.myAjax.post('/QCStdTp/Chapter5Save', { item: item })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                item.edit = false;
                                this.editFlag = false;
                            }
                            this.sortItems();
                            alert(resp.data.message);
                            console.log(resp);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            delItem(index, item) {
                //item.edit = false;
                console.log('index: ' + index + ' seq: ' + item.Seq);
                if (item.Seq < 0) {
                    this.items.splice(index, 1);
                    this.editFlag = false;
                }
                else if (confirm('是否確定刪除?')) {
                    window.myAjax.post('/QCStdTp/Chapter5Del', { seq: item.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                if (item.edit) this.editFlag = false;
                                this.items.splice(index, 1);
                            } else {
                                alert(resp.data.message);
                            }
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            sortItems() {
                this.items.sort(function (a, b) {
                    var nameA = a.OrderNo; // ignore upper and lowercase
                    var nameB = b.OrderNo; // ignore upper and lowercase
                    console.log('nameA: ' + nameA + ' nameB: ' + nameB);
                    if (nameA < nameB) {
                        return -1;
                    }
                    if (nameA > nameB) {
                        return 1;
                    }
                    return 0;
                });
            }
        },
        async mounted() {
            console.log('mounted() 第五章 材料與設備抽驗程序及標準');
            //document.getElementById('app').classList.remove("container");
            //await this.fetchitems();
        }
    }
</script>
<style>

</style>
