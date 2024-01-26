<template>
    <div>
        <div class="table-responsive">
            <table class="table table-responsive-md table2 VA-middle">
                <tr>
                    <th colspan="6" class="bg-R-30"><strong>專家基本資訊</strong></th>
                </tr>
                <tr>
                    <th><strong>姓名</strong></th>
                    <td><input v-model.trim="item.ECName" maxlength="50" type="text" class="form-control"></td>
                    <th>生日</th>
                    <td><input v-model.trim="item.ECBirthdayStr" type="date" class="form-control"></td>
                    <th>身分證</th>
                    <td><input v-model.trim="item.ECId" maxlength="14" type="text" class="form-control"></td>
                </tr>
                <tr>
                    <th>委員種類</th>
                    <td>
                        <select v-model="item.ECKind" class="form-control">
                            <option value="1">評選委員</option>
                            <option value="2">督導委員</option>
                            <option value="3">其他</option>
                        </select>
                    </td>
                    <th>職稱</th>
                    <td><input v-model.trim="item.ECPosition" maxlength="10" type="text" class="form-control"></td>
                    <th>機關 / 單位</th>
                    <td><input v-model.trim="item.ECUnit" maxlength="30" type="text" class="form-control"></td>
                </tr>
                <tr>
                    <th colspan="6" class="bg-R-30"><strong>聯絡資料</strong></th>
                </tr>
                <tr>
                    <th>e-mail</th>
                    <td colspan="5"><input v-model.trim="item.ECEmail" maxlength="100" type="text" class="form-control"></td>
                </tr>
                <tr>
                    <th>電話</th>
                    <td><input v-model.trim="item.ECTel" maxlength="50" type="text" class="form-control"></td>
                    <th>手機</th>
                    <td><input v-model.trim="item.ECMobile" maxlength="20" type="text" class="form-control"></td>
                    <th>傳真</th>
                    <td><input v-model.trim="item.ECFax" maxlength="15" type="text" class="form-control"></td>
                </tr>
                <tr>
                    <th>通訊地址</th>
                    <td colspan="5"><input v-model.trim="item.ECAddr1" maxlength="100" type="text" class="form-control"></td>
                </tr>
                <tr>
                    <th>戶籍地址</th>
                    <td colspan="5"><input v-model.trim="item.ECAddr2" maxlength="100" type="text" class="form-control"></td>
                </tr>
                <tr>
                    <th colspan="6" class="bg-R-30"><strong>詳細資訊</strong></th>
                </tr>
                <tr>
                    <th>主要專長</th>
                    <td colspan="5"><input v-model.trim="item.ECMainSkill" maxlength="50" type="text" class="form-control"></td>
                </tr>
                <tr>
                    <th>次要專長</th>
                    <td colspan="5"><input v-model.trim="item.ECSecSkill" maxlength="50" type="text" class="form-control"></td>
                </tr>
                <tr>
                    <th>銀行帳號</th>
                    <td><input v-model.trim="item.ECBankNo" maxlength="100" type="text" class="form-control"></td>
                    <th>葷 / 素</th>
                    <td>
                        <div class="custom-control custom-radio custom-control-inline">
                            <input v-model="item.ECDiet" value="1" type="radio" name="eatA" id="eatA01" class="custom-control-input">
                            <label for="eatA01" class="custom-control-label">葷</label>
                        </div>
                        <div class="custom-control custom-radio custom-control-inline">
                            <input v-model="item.ECDiet" value="2" type="radio" name="eatA" id="eatA02" class="custom-control-input">
                            <label for="eatA02" class="custom-control-label">素</label>
                        </div>
                    </td>
                    <th>特殊需求</th>
                    <td><input v-model.trim="item.ECNeed" maxlength="50" type="text" class="form-control"></td>
                </tr>
                <tr>
                    <th>備註</th>
                    <td colspan="5">
                        <textarea v-model.trim="item.ECMemo" maxlength="500" rows="5" class="form-control"></textarea>
                    </td>
                </tr>
            </table>
        </div>
        <div class="card-footer">
            <div class="row justify-content-center">
                <div class="col-12 col-sm-4 col-xl-2 my-2">
                    <button @click="onCancel" role="button" class="btn btn-shadow btn-color3 btn-block"> 離開 </button>
                </div>
                <div class="col-12 col-sm-4 col-xl-2 my-2">
                    <button @click="onSave" role="button" class="btn btn-shadow btn-block btn-color11-3"> 儲存 <i class="fas fa-save"></i></button>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                targetId:null,
                item: {},
            };
        },
        methods: {
            onCancel() {
                window.location = "/ExpertCommittee";
            },
            strEmpty(v) {
                return window.comm.stringEmpty(v);
            },
            onSave() {
                if (this.item.ECKind == null) {
                    alert('請設定 委員種類');
                    return;
                }
                if (this.strEmpty(this.item.ECName) || this.strEmpty(this.item.ECEmail) || this.strEmpty(this.item.ECTel)
                    || this.strEmpty(this.item.ECMobile)) {
                    alert('姓名,E-mail,電話,手機 必須填寫');
                    return;
                }
                this.item.ECBirthday = this.item.ECBirthdayStr;
                window.myAjax.post('/ExpertCommittee/Save', { m: this.item })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            if (this.item.Seq == -1) this.item.Seq = resp.data.id;
                        }
                        alert(resp.data.msg);
                    }).catch(error => {
                        console.log(error);
                    });
            },
            getItem() {
                this.loading = true;
                window.myAjax.post('/ExpertCommittee/GetCommittee', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.item = resp.data.item;
                        } else {
                            alert(resp.data.message);
                        }
                    }).catch(error => {
                        console.log(error);
                    });
            }
        },
        async mounted() {
            console.log('mounted() 專家委員維護-編輯');
            let urlParams = new URLSearchParams(window.location.search);
            if (urlParams.has('id')) {
                this.targetId = parseInt(urlParams.get('id'), 10);
                console.log(this.targetId);
                if (Number.isInteger(this.targetId)) {
                    this.getItem();
                    return;
                }
            }
            this.onCancel();
        }
    }
</script>