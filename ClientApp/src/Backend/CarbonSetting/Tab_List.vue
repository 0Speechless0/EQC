<template>
    <div>
        <form class="form-group insearch mb-3">
            <div class="form-row">
                <div class="col-1 mt-3">
                    <select v-model="engYear" class="form-control">
                        <option value="-1"> 全部</option>
                        <option value="112" > 112 </option>
                        <option value="111" selected="selected"> 111 </option>
                        <option value="110"> 110 </option>
                        <option value="109"> 109 </option>
                        <option value="108">108</option>
                        <option value="107">107</option>
                        <option value="106">106</option>
                    </select>
                </div>
                <div class="col-12 col-sm-3 mt-3">
                    <button @click="getResords" type="button" style="color: #fff;background-color: #6c757d;border-color: #6c757d; border-radius: 5px;" >查詢</button>
                </div>
            </div>
        </form>

        <div class="table-responsive">
            <table class="table table-responsive-md table-hover VA-middle">
                <thead class="insearch">
                    <tr>
                        <th style="width: 15%; text-align: center;"><strong>機關/單位</strong></th>
                        <th class="text-right" style="width: 15%; text-align: center;"><strong>需求碳排量</strong></th>
                        <th class="text-right" style="width: 15%; text-align: center;"><strong>核定碳排量</strong></th>
                        <th class="text-right" style="width: 15%; text-align: center;"><strong>設計碳排量</strong></th>
                        <th class="text-right" style="width: 15%; text-align: center;"><strong>施工碳排量</strong></th>
                        <th style="width: 15%; text-align: center;">
                            <strong>功能</strong>
                            <button @click="onNewRecord" role="button" class="btn btn-color11-3 btn-xs sharp mr-1">
                                <i class="fas fa-plus"></i>
                            </button>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-if="newItem != null">
                        <td>
                            <div class="form-row" style="align-items: center">
                                <span class="col-auto">年分</span>
                                <div class="col-9"><input v-model.number="newItem.EngYear" type="number" class="form-control my-1 mr-0 mr-md-4" placeholder="輸入年分" /></div>
                                
                            </div>
                            <select v-model="newItem.EngUnitSeq" class="form-control">
                                <option value="34">北區水資源局</option>
                                <option value="35">中區水資源局</option>
                                <option value="36">南區水資源局</option>
                            </select>
                        </td>
                        <td>
                            <input v-model.number="newItem.CarbonDemandQuantity" type="number" class="form-control my-1 mr-0 mr-md-4">
                        </td>
                        <td>
                            <input v-model.number="newItem.ApprovedCarbonQuantity" type="number" class="form-control my-1 mr-0 mr-md-4">
                        </td>
                        <td></td>
                        <td></td>
                        <td>
                            <button @click="onAddRecord" class="btn btn-color11-2 btn-xs sharp mr-1"><i class="fas fa-save"></i></button>
                            <button @click="newItem=null" class="btn btn-color9-1 btn-xs sharp mr-1"><i class="fas fa-times"></i></button>
                        </td>
                    </tr>
                    <template v-if="newItem == null">
                        <tr v-for="(item, index) in items" v-bind:key="index">
                            <td>{{item.UnitName}}</td>
                            <template v-if="item.Seq != editSeq">
                                <td class="text-right">{{item.CarbonDemandQuantity}}</td>
                                <td class="text-right">{{item.ApprovedCarbonQuantity}}</td>
                                <td class="text-right">{{item.CarbonDesignQuantity}}</td>
                                <td class="text-right">{{item.CarbonConstructionQuantity}}</td>
                                <td style="text-align: center;">
                                    <template v-if="item.Seq>-1">
                                        <button @click="onEditRecord(item)" class="btn btn-color11-1 btn-xs sharp mr-1">
                                            <i class="fas fa-pencil-alt"></i>
                                        </button>
                                        <button @click="onDelRecord(item)" class="btn btn-color9-1 btn-xs sharp mr-1">
                                            <i class="fas fa-trash-alt"></i>
                                        </button>
                                    </template>
                                </td>
                            </template>
                            <template v-if="item.Seq == editSeq">
                                <td>
                                    <input v-model.number="editRecord.CarbonDemandQuantity" type="number" class="form-control my-1 mr-0 mr-md-4">
                                </td>
                                <td>
                                    <input v-model.number="editRecord.ApprovedCarbonQuantity" type="number" class="form-control my-1 mr-0 mr-md-4">
                                </td>
                                <td></td>
                                <td></td>
                                <td style="text-align: center;">
                                    <template v-if="item.Seq>-1">
                                        <button @click="onUpdateRecord" class="btn btn-color11-2 btn-xs sharp mr-1"> <i class="fas fa-save"></i> </button>
                                        <button @click="editSeq=-99" class="btn btn-color9-1 btn-xs sharp mr-1"> <i class="fas fa-times"></i> </button>
                                    </template>
                                </td>
                            </template>
                        </tr>
                    </template>
                </tbody>
            </table>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                editSeq: -99,
                editRecord: {},
                items: [],
                newItem: null,
                engYear:-1,
            };
        },
        methods: {
            //刪除紀錄
            onDelRecord(item) {
                if (this.editSeq > -99) return;
                if (confirm('是否確定刪除資料？')) {
                    window.myAjax.post('/CarbonEmissionSetting/DelRecord', { id: item.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getResords();
                            } else
                                alert(resp.data.msg);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            //編輯紀錄
            onEditRecord(item) {
                if (this.editSeq > -99) return;
                this.editRecord = Object.assign({}, item);
                this.editSeq = this.editRecord.Seq;
            },
            //更新
            onUpdateRecord() {
                window.myAjax.post('/CarbonEmissionSetting/UpdateRecord', { m: this.editRecord })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getResords();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //新增
            onAddRecord() {
                if (!window.comm.isNumber(this.newItem.EngYear)) {
                    alert('必須輸入年分');
                    return;
                }
                if (this.newItem.EngYear == 0) {
                    alert('年分 必須大於0');
                    return;
                }
                if (window.comm.stringEmpty(this.newItem.EngUnitSeq)) {
                    alert('請選取單位');
                    return;
                }
                window.myAjax.post('/CarbonEmissionSetting/UpdateRecord', { m: this.newItem})
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getResords();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onNewRecord() {
                this.newItem = null;
                window.myAjax.post('/CarbonEmissionSetting/NewRecord')
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.newItem = resp.data.item;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //紀錄清單
            getResords() {
                this.editSeq = -99;
                this.newItem = null;
                this.items = [];
                window.myAjax.post('/CarbonEmissionSetting/GetList', { year: this.engYear })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        mounted() {
            console.log('mounted() 清單');
        }
    }
</script>