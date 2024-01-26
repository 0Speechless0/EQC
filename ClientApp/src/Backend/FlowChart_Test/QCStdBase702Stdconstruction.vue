<template>
    <div id="showBlock" style="display:none;">
        <div v-if="loading">
            Loading...
        </div>
      <!--  <div v-if="!loading && this.op1" class="row justify-content-between">
            <div class="col mt-3">
                <label for="tableinfo">點選新增項目，新增資訊</label>
            </div>
            <div class="col-12 col-md-4 col-xl-2 mt-3">
                <a v-on:click.stop="newItem()" role="button" class="btn btn-shadow btn-color1 btn-block">
                    <i class="fas fa-plus"></i>&nbsp;&nbsp;新增項目
                </a>
            </div>
        </div>-->
        <div class="table-responsive">
            <table class="table table3 min910" border="0">
                <thead>
                    <tr>
                        <th class="sort">順序</th>
                        <th>流程</th>
                        <th>抽驗項目</th>
                        <th>管理標準</th>
                        <th>抽查時機</th>
                        <th class="twoInput">抽查方法</th>
                        <th>抽查頻率</th>
                        <th>不符合之處置方法</th>
                        <th>管理紀錄</th>
                        <th>型態</th>
                        <th>註記</th>
                        <!--<th>編輯</th>-->
                    </tr>
                </thead>

                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.id" v-show="!editFlag || item.edit">
                        <td>
                            <div v-if="!item.edit">{{item.OrderNo}}</div>
                            <input v-if="item.edit" type="text" v-model.number="item.OrderNo" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{getFlowText(item.ECCFlow1)}}</div>
                            <select v-if="item.edit" v-model.number="item.ECCFlow1" class="form-control">
                                <option value="1">施工前</option>
                                <option value="2">施工中</option>
                                <option value="3">施工後</option>
                            </select>
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.ECCCheckItem1}}</div>
                            <input maxlength="50" v-if="item.edit" type="text" v-model.trim="item.ECCCheckItem1" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.ECCStand1}}</div>
                            <input maxlength="50" v-if="item.edit" type="text" v-model.trim="item.ECCStand1" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.ECCCheckTiming}}</div>
                            <input maxlength="100" v-if="item.edit" type="text" v-model.trim="item.ECCCheckTiming" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.ECCCheckMethod}}</div>
                            <textarea rows="5" maxlength="100" v-if="item.edit" type="text" v-model.trim="item.ECCCheckMethod" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.ECCCheckFeq}}</div>
                            <textarea rows="5" maxlength="100" v-if="item.edit" type="text" v-model.trim="item.ECCCheckFeq" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.ECCIncomp}}</div>
                            <textarea rows="5" maxlength="100" v-if="item.edit" type="text" v-model.trim="item.ECCIncomp" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.ECCManageRec}}</div>
                            <input maxlength="100" v-if="item.edit" type="text" v-model.trim="item.ECCManageRec" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{getTypeText(item.ECCType)}}</div>
                            <select v-if="item.edit" v-model.number="item.ECCType" class="form-control">
                                <option value="1">文字</option>
                                <option value="2">數字</option>
                                <option value="3">邏輯</option>
                            </select>
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.ECCMemo==true ? 'AR' : ''}}</div>
                            <div v-if="item.edit" class="custom-control custom-checkbox">
                                <input type="checkbox" class="custom-control-input" id="AR" v-model="item.ECCMemo">
                                <label class="custom-control-label" for="AR">AR</label>
                            </div>
                        </td>
                       <!-- <td>
                            <div class="row justify-content-center m-0">
                                <a v-on:click.stop="editItem(item)" v-if="!item.edit" href="#" class="btn-block mx-2 btn btn-color2" title="編輯">編輯</a>
                                <a v-on:click.stop="saveItem(item)" v-if="item.edit" href="#" class="btn-block mx-2 btn btn-color1" title="儲存">儲存</a>
                                <a v-on:click.stop="delItem(index, item)" href="#" class="mx-2 btn-color3 btn btn-block" title="刪除">刪除</a>
                            </div>
                        </td>-->
                    </tr>
                </tbody>
            </table>

        </div>
    </div>
</template>
<script>
    import $ from 'jquery'
    export default {
        props: ['op1', 'op2'],
        watch: {
            'op1': function (nval, oval) {
                console.log('watch op1 :' + oval + ' >> ' + nval);
                //if (this.op1 != null) { this.getList(); $("#showBlock").show();}
            },
            'op2': function (nval, oval) {
                if ((this.op2 % 2) != 0) { this.getList(); $("#showBlock").show(); } else { $("#showBlock").hide(); }
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
            getFlowText(value) {
                if (value == 1) {
                    return '施工前';
                } else if (value == 2) {
                    return '施工中';
                } else if (value == 3) {
                    return '施工後';
                } else {
                    return '';
                }
            },
            getTypeText(value) {
                if (value == 1) {
                    return '文字';
                } else if (value == 2) {
                    return '數字';
                } else if (value == 3) {
                    return '邏輯';
                } else {
                    return '';
                }
            },
            getList() {
                this.editFlag = false;
                this.loading = true;
                this.items = [];
                let params = { op1: this.op1, pageIndex: this.pageIndex, perPage: this.perPage };
                window.myAjax.post('/QCStdTp/Chapter702_AllList', params)
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
                    EnvirConsListSeq: this.op1,
                    ECCFlow1:1,
                    ECCCheckItem1: '', ECCCheckItem2: '',
                    ECCStand1: '', ECCStand2: '', ECCStand3: '', ECCStand4: '', ECCStand5: '',
                    ECCCheckTiming: '',
                    ECCCheckMethod: '',
                    ECCCheckFeq: '',
                    ECCIncomp: '',
                    ECCManageRec: '',
                    ECCType: 1,
                    ECCMemo: false,
                    ECCCheckFields: 0,
                    ECCManageFields: 0,
                    OrderNo: 999,
                    edit: true
                };
                this.items.push(newRow);
            },
            saveItem(item) {
                if (item.OrderNo === '' || item.ECCFlow1 === '') {
                    alert('[順序,流程]欄位須輸入資料');
                    return;
                }
                //console.log(item);
                if (item.Seq < 0) {
                    window.myAjax.post('/QCStdTp/Chapter702Add', { item: item })
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
                    window.myAjax.post('/QCStdTp/Chapter702Save', { item: item })
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
                    window.myAjax.post('/QCStdTp/Chapter702Del', { seq: item.Seq })
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
            console.log('mounted() 第七章 702 環境保育管理標準');
        }
    }
</script>