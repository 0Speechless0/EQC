<template>
  <div>
    <!-- div class="form-row mb-1">
            <div class="col-12 col-sm-4 col-md-2 mb-1">
                <label class="my-2 mx-2 float-right">決標日期</label>
            </div>
            <div class="col-12 col-sm-4 col-md-3 mb-1">
                <b-input-group>
                    <input v-bind:value="engMain.chsAwardDate" ref="chsAwardDate" @change="onDateChange(engMain.chsAwardDate, $event, 'AwardDate')"
                           v-bind:disabled="engMain.srcAwardDate!=null" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                    <b-form-datepicker v-if="engMain.srcAwardDate==null" v-model="chsAwardDate" :hide-header="true"
                                       button-only right size="sm" @context="onDatePicketChange($event, 'AwardDate')">
                    </b-form-datepicker>
                </b-input-group>
            </div>
            <div v-if="engMain.srcAwardDate==null" class="col-12 col-sm-4 col-md-2 mb-1">
                <button v-on:click.stop="updateAwardDate()" role="button" class="btn btn-color11-4 btn-xs m-1">
                    <i class="fas fa-save"> 儲存</i>
                </button>
            </div>
            <div class="col-12 col-sm-4 col-md-6 mb-1">
            </div>
        </div -->
    <!-- Nav tabs -->
    <ul class="nav nav-tabs" role="tablist">
      <li class="nav-item">
        <a
          @click="onSetMidKine"
          class="nav-link"
          data-toggle="tab"
          href="#menu00"
          >設定中類</a
        >
      </li>
      <template v-for="(item, index) in dateList">
        <li
          v-if="showDateListTab"
          v-bind:key="item.AdjMonthStr"
          class="nav-item"
        >
          <a
            @click="onChangeDate(item)"
            class="nav-link"
            data-toggle="tab"
            href="#menu01"
            >{{ item.AdjMonthStrYM }}</a
          >
        </li>
      </template>
    </ul>
    <div v-if="setMidKind" class="tab-content">
      <div id="menu01" class="tab-pane active">
        <h5 class="insearch mt-0 py-2">
          <div class="form-row mb-1">
            <div class="table-responsive tableFixHead">
              <h5 style="margin: 0">設定中類</h5>
              <table class="table table-responsive-md table-hover VA-middle">
                <thead class="insearch">
                  <tr>
                    <th><strong>序號</strong></th>
                    <th class="text-center"><strong>契約項目</strong></th>
                    <th class="text-center"><strong>契約金額</strong></th>
                    <th class="text-center"><strong>物價調整款</strong></th>
                  </tr>
                </thead>
                <tbody>
                  <tr
                    v-for="(item, index) in midKinds"
                    v-bind:key="item.Seq"
                    class="bg-1-30"
                  >
                    <td class="text-center">
                      <strong>{{ index + 1 }}</strong>
                    </td>
                    <td style="font-size: 15px">
                      {{ item.Description }}({{ item.ItemCode }})
                    </td>
                    <td>{{ item.Amount }}</td>
                    <td>
                      <select
                        v-model="item.PriceIndexKindId"
                        class="form-control"
                      >
                        <option value="201">水泥及其製品類指數</option>
                        <option value="202">金屬製品類指數</option>
                        <option value="203">砂石及級配類指數</option>
                        <option value="204">瀝青及其製品類指數</option>
                        <option value="999">其他</option>
                      </select>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
          <div v-if="midKinds.length > 0" class="d-flex justify-content-center">
            <div class="col-12 col-sm-4 col-xl-2 my-2">
              <button
                @click="saveMidKind"
                role="button"
                class="btn btn-shadow btn-block btn-color11-4"
              >
                儲存 <i class="fas fa-save"></i>
              </button>
            </div>
          </div>
        </h5>
      </div>
    </div>

    <!-- Tab panes -->
    <div v-if="itemGroup" class="card whiteBG mb-4 pattern-F colorset_1">
      <div class="form-row" role="toolbar">
        <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
          <button
            @click="onCalPriceIndexClick"
            type="button"
            class="btn btn-outline-secondary btn-sm"
            title="重新計算"
          >
            重新計算 &nbsp;<i class="fas fa-check"></i>
          </button>
        </div>
      </div>
      <div class="card-body">
        <h5 class="insearch mt-0 py-2">
          <div class="form-row mb-1">
            <div class="table-responsive">
              <h5>個別-預拌混擬土(M03310)</h5>
              <wiTable v-bind:items="itemGroup.M03310"></wiTable>
              <h5>個別-鋼筋(M03210)</h5>
              <wiTable v-bind:items="itemGroup.M03210"></wiTable>
              <h5>個別-瀝青混擬土(02742)</h5>
              <wiTable v-bind:items="itemGroup.M02742"></wiTable>
              <h5>中分類-水泥及其製品</h5>
              <wiTable v-bind:items="itemGroup.M03"></wiTable>
              <h5>中分類-金屬製品類)</h5>
              <wiTable v-bind:items="itemGroup.M05"></wiTable>
              <h5>中分類-砂石及級配類</h5>
              <wiTable v-bind:items="itemGroup.M02319"></wiTable>
              <h5>中分類-瀝青及其製品類</h5>
              <wiTable v-bind:items="itemGroup.M027_0296"></wiTable>
              <h5>總類</h5>
              <wiTable v-bind:items="itemGroup.Mxxx"></wiTable>
            </div>
          </div>
        </h5>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  props: ["tenderItem", "spHeader"],
  data: function () {
    return {
      //設定中類
      midKinds: [],
      setMidKind: false,
      //
      showDateListTab: false,
      dateList: [],
      itemGroup: null,
      activeItem: null,
      //
      btnDisabled: false,
      //決標日期 shioulo 20221216
      engMain: {},
      chsAwardDate: "",
    };
  },
  components: {
    wiTable: require("./PM_PriceAdjustmentTable.vue").default,
  },
  methods: {
    //決標日期 shioulo 20221216
    onDateChange(srcDate, event, mode) {
      if (event.target.value.length == 0) {
        if (mode == "AwardDate") this.chsAwardDate = "";
        return;
      }
      if (!this.isExistDate(event.target.value)) {
        event.target.value = srcDate;
        alert("日期錯誤");
      } else {
        if (mode == "AwardDate")
          this.chsAwardDate = this.toYearDate(event.target.value);
      }
    },
    onDatePicketChange(ctx, mode) {
      //console.log(ctx);
      if (ctx.selectedDate != null) {
        var d = ctx.selectedDate;
        var dd =
          d.getFullYear() - 1911 + "/" + (d.getMonth() + 1) + "/" + d.getDate();
        //var y = d.getYear() - 1911;
        if (mode == "AwardDate") this.engMain.chsAwardDate = dd;
      }
    },
    //中曆轉西元
    toYearDate(dateStr) {
      if (dateStr == null || dateStr == "") return null;
      var dateObj = dateStr.split("/"); // yyy/mm/dd
      return new Date(
        parseInt(dateObj[0]) + 1911,
        parseInt(dateObj[1]) - 1,
        parseInt(dateObj[2])
      );
    },
    //日期檢查
    isExistDate(dateStr) {
      var dateObj = dateStr.split("/"); // yyy/mm/dd
      if (dateObj.length != 3) return false;

      var limitInMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

      var theYear = parseInt(dateObj[0]);
      if (theYear != dateObj[0]) return false;
      var theMonth = parseInt(dateObj[1]);
      if (theMonth != dateObj[1]) return false;
      var theDay = parseInt(dateObj[2]);
      if (theDay != dateObj[2]) return false;
      if (new Date(theYear + 1911, 1, 29).getDate() === 29) {
        // 是否為閏年?
        limitInMonth[1] = 29;
      }
      return theDay <= limitInMonth[theMonth - 1];
    },
    //
    getEngMain() {
      this.engMain = {};
      window.myAjax
        .post("/ECPriceAdjustment/GetEngItem", { id: this.tenderItem.Seq })
        .then((resp) => {
          if (resp.data.result == 0) {
            this.engMain = resp.data.item;
            this.chsAwardDate = this.toYearDate(this.engMain.chsAwardDate);
          } /* else {
                            alert(resp.data.msg);
                        }*/
        })
        .catch((err) => {
          console.log(err);
        });
    },
    //更新 決標日期
    updateAwardDate() {
      this.engMain.chsAwardDate = this.$refs["chsAwardDate"].value;
      window.myAjax
        .post("/ECPriceAdjustment/UpdateEngAwardDate", {
          engMain: this.engMain,
        })
        .then((resp) => {
          if (resp.data.result == 0) {
            this.getEngMain();
            alert(resp.data.message);
          } else {
            alert(resp.data.message);
          }
        })
        .catch((err) => {
          console.log(err);
        });
    },
    //shioulo 20221216 end
    //中類設定清單
    onSetMidKine() {
      this.itemGroup = null;
      this.setMidKind = true;
    },
    //
    saveMidKind() {
      if (!this.checkModKind()) {
        alert("物價調整項需全數設定");
        return;
      }
      window.myAjax
        .post("/ECPriceAdjustment/SaveMidKind", { items: this.midKinds })
        .then((resp) => {
          if (resp.data.result == 0) {
            this.showDateListTab = true;
          }
          alert(resp.data.msg);
        })
        .catch((err) => {
          console.log(err);
        });
    },
    //檢查中類是否設定
    checkModKind() {
      if (this.midKinds.length == 0) {
        return false;
      }
      var i = 0;
      for (i = 0; i < this.midKinds.length; i++) {
        if (this.midKinds[i].PriceIndexKindId == null) {
          return false;
        }
      }
      return true;
    },
    //重新計算
    onCalPriceIndexClick() {
      if (this.activeItem == null) return;

      window.myAjax
        .post("/ECPriceAdjustment/CalPriceIndex", {
          id: this.tenderItem.Seq,
          tarDate: this.activeItem.AdjMonthStr,
        })
        .then((resp) => {
          if (resp.data.result == 0) {
            this.onChangeDate(this.activeItem);
          }
          alert(resp.data.msg);
        })
        .catch((err) => {
          console.log(err);
        });
    },
    //取得清單
    onChangeDate(item) {
      this.setMidKind = false;
      this.activeItem = item;
      this.itemGroup = null;
      window.myAjax
        .post("/ECPriceAdjustment/GetList", {
          id: this.tenderItem.Seq,
          tarDate: item.AdjMonthStr,
        })
        .then((resp) => {
          if (resp.data.result == 0) {
            this.itemGroup = resp.data.group;
          } else alert(resp.data.msg);
        })
        .catch((err) => {
          console.log(err);
        });
    },
    //日期清單
    getDateList() {
      this.dateList = [];
      window.myAjax
        .post("/ECPriceAdjustment/GetDateList", { id: this.tenderItem.Seq })
        .then((resp) => {
          if (resp.data.result == 0) {
            this.dateList = resp.data.items;
            this.midKinds = resp.data.wItems;
            this.showDateListTab = this.checkModKind();
          } else alert(resp.data.msg);
        })
        .catch((err) => {
          console.log(err);
        });
    },
    //*******************************************************************
    //工程變更
    onEngChangeClick() {
      window.myAjax
        .post("/ECPriceAdjustment/CheckEngChangeDate", {
          id: this.tenderItem.Seq,
        })
        .then((resp) => {
          if (resp.data.result == 0) {
            this.$swal({
              title: "工程變更, 是否確定?",
              html: resp.data.msg,
              icon: "warning",
              showCancelButton: true,
              confirmButtonColor: "#3085d6",
              cancelButtonColor: "#d33",
              confirmButtonText: "是",
              cancelButtonText: "否",
            }).then(this.execChangeConfirm);
          } else {
            this.$swal(resp.data.msg);
          }
        })
        .catch((err) => {
          console.log(err);
        });
    },
    //工程變更執行
    execChangeConfirm(result) {
      if (!result.value) return;

      window.myAjax
        .post("/ECPriceAdjustment/ExecEngChangeDate", {
          id: this.tenderItem.Seq,
        })
        .then((resp) => {
          if (resp.data.result == 0) {
            this.getDateList();
          }
          alert(resp.data.msg);
          this.$emit("reload");
        })
        .catch((err) => {
          console.log(err);
        });
    },
  },
  mounted() {
    console.log("mounted 工程變更-物價調整款");
    this.getDateList();
    this.getEngMain();
  },
};
</script>
<style scoped>
.labelDisabled {
  opacity: 0.65;
}
.tableFixHead {
  overflow: auto;
  max-height: 500px;
}
table {
  border-collapse: separate;
  border-spacing: 0;
}
.table {
  margin: 0;
}
.tableFixHead thead {
  position: sticky !important ;
  top: 0 !important ;
  z-index: 1 !important;
}
th {
  border: 0;
  border-bottom: #ddd solid 1px !important;
  border-left: 0 !important;
  border-right: 0 !important;
}
td {
  z-index: 0;
  position: relative;
}
</style>
