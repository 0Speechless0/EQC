<template>
    <div class="tab-content">
        <table class="table table-responsive-md table-hover">
            <thead class="insearch">
                <tr>
                    <th style="width: 10%;"><strong>項次</strong></th>
                    <th><strong>變更類型</strong></th>
                    <th><strong>工程開始變更日期</strong></th>
                    <th><strong>變更後預定完工日期</strong></th>
                    <th><strong>變更日期</strong></th>
                    <th style="text-align: center;">
                        <strong>功能</strong>
                    </th>
                    <th style="text-align: center;">
                        <strong>變更歷程</strong>
                    </th>
                    <th v-if="fEngChangeCanAdd" style="text-align: center;">
                        <button v-if="!fEngChangeAdd" @click="onEngChangeAddClick" type="button" class="btn btn-color11-3 btn-xs sharp mx-1" title="新增">
                            <i class="fas fa-plus"></i>
                        </button>
                    </th>
                </tr>
            </thead>
            <tbody>
                <template v-if="fEngChangeAdd">
                    <tr v-show="engChangeType==1 || engChangeType==200" class="bg-1-30">
                        <td></td>
                        <td></td>
                        <td>
                            <b-input-group>
                                <input v-bind:value="engMain.chsStartDate" ref="chsStartDate" @change="onDateChange(engMain.chsStartDate, $event, 'chsStartDate')" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                                <b-form-datepicker v-model="chsStartDate" :hide-header="true" button-only right size="sm" @context="onDatePicketChange($event, 'chsStartDate')">
                                </b-form-datepicker>
                            </b-input-group>
                        </td>
                        <td>
                            <b-input-group>
                                <input v-bind:value="engMain.chsSchCompDate" ref="chsSchCompDate" @change="onDateChange(engMain.chsSchCompDate, $event, 'chsSchCompDate')"
                                       v-bind:disabled="engMain.srcSchCompDate!=null" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                                <b-form-datepicker v-if="engMain.srcSchCompDate==null" v-model="chsSchCompDate" :hide-header="true"
                                                   button-only right size="sm" @context="onDatePicketChange($event, 'chsSchCompDate')">
                                </b-form-datepicker>
                            </b-input-group>
                        </td>
                        <td></td>
                        <td style="text-align: center;width: 10%;">
                            <button @click="onSaveNewEngChangeClick" type="button" class="btn btn-color11-2 btn-xs sharp mx-1" title="儲存"><i class="fas fa-save"></i></button>
                            <button @click="fEngChangeAdd=false" type="button" class="btn btn-color9-1 btn-xs sharp mx-1" title="取消"><i class="fas fa-times"></i></button>
                        </td>
                        <td style="text-align: center;width: 10%;">
                        </td>
                        <td v-if="fEngChangeCanAdd"></td>
                    </tr>
                    <tr>
                        <td colspan="8" class="text-left">
                            選擇工程變更類別
                            <div v-if="engChangeItems.length == 0 || engChangeItems[engChangeItems.length-1].ChangeType < 100">
                                <div class="custom-control custom-radio custom-control-inline">
                                    <input v-model="engChangeType" value="2" type="radio" id="W2" class="custom-control-input">
                                    <label for="W2" class="custom-control-label" style="width:80px">展延工期</label>
                                </div>
                                <div class="custom-control custom-radio custom-control-inline">
                                    <input v-model="engChangeType" value="100" type="radio" id="W100" class="custom-control-input">
                                    <label for="W100" class="custom-control-label">停工</label>
                                </div>
                                <div class="custom-control custom-radio custom-control-inline">
                                    <input v-model="engChangeType" value="200" type="radio" id="W200" class="custom-control-input">
                                    <label for="W200" class="custom-control-label">契約終止及解除</label>
                                </div>
                                <div class="custom-control custom-radio custom-control-inline">
                                    <input v-model="engChangeType" value="1" type="radio" id="W1" class="custom-control-input">
                                    <label for="W1" class="custom-control-label">工程變更設計暨修正施工預算</label>
                                </div>
                            </div>
                            <div v-if="engChangeItems.length > 0 && engChangeItems[engChangeItems.length-1].ChangeType == 100">
                                <div class="custom-control custom-radio custom-control-inline">
                                    <input v-model="engChangeType" value="3" type="radio" id="W3" class="custom-control-input">
                                    <label for="W3" class="custom-control-label">復工</label>
                                </div>
                                <div class="custom-control custom-radio custom-control-inline">
                                    <input v-model="engChangeType" value="200" type="radio" id="W200" class="custom-control-input">
                                    <label for="W200" class="custom-control-label">契約終止及解除</label>
                                </div>
                            </div>
                            <div v-if="engChangeItems.length > 0 && engChangeItems[engChangeItems.length-1].ChangeType == 200">
                                <h5 class="redText">已契約終止及解除, 不能再變更</h5>
                                <button @click="fEngChangeAdd=false" type="button" class="btn btn-color9-1 mx-1" title="取消"><i class="fas fa-times"></i> 取消</button>
                            </div>
                        </td>
                    </tr>
                    <tr class="insearch">
                        <th colspan="8" class="text-left">
                            <div v-if="engChangeType==2" class="pt-2">
                                指工程施工中，非屬廠商因素，造成核定網狀圖中主要徑作業受影響，且符合契約相關規定，辦理展延工期。
                                <ExtendConstruction :tenderItem="tenderItem" v-on:onClose="onCloseEvent"></ExtendConstruction>
                            </div>
                            <div v-if="engChangeType==100" class="pt-2">
                                非可歸責於廠商之因素，致契約全部或部分必須停工，機關得通知廠商停工，停工原因消滅後，機關應立即通知廠商復工。
                                <WorkStopBack :tenderItem="tenderItem" :mode="1" :engChangeType="engChangeType" v-on:onClose="onCloseEvent"></WorkStopBack>
                            </div>
                            <div v-if="engChangeType==3" class="pt-2">
                                非可歸責於廠商之因素，致契約全部或部分必須停工，機關得通知廠商停工，停工原因消滅後，機關應立即通知廠商復工。
                                <WorkStopBack :tenderItem="tenderItem" :mode="2" :engChangeType="engChangeType" :engChange="engChangeItems[engChangeItems.length-1]" v-on:onClose="onCloseEvent"></WorkStopBack>
                            </div>
                            <div v-if="engChangeType==200" class="pt-2">
                                終止契約：係指不使契約繼續進行，廠商已依契約向機關履行部分，仍屬有效。解除契約：係指當事人之一方因行使解除權，而使契約自始歸於消滅，回復契約成立前之狀況，當事人雙方均有回復原狀之義務，包括退物、還錢。
                            </div>
                            <div v-if="engChangeType==1" class="pt-2">
                                有2種情形:<br />
                                1.變更設計：指工程施工中，因故需變更原設計原則以完備原設計標的之功能需求或為因應緊急事項、事實需求及其他必需配合處理等涉及設計原則及預算之變更。<br />
                                2.修正施工預算：指工程施工中，因經費之財源調整、工程項目數量漏列或誤估之調整、地形變動致施工數量不符之調整、其他署辦費用之調整、契約項目規定以實做數量結算之調整、因政府法令、稅捐、規費或管制費率變更之調整及因政策變更須作契約之終止或解除之調整等不涉及設計原則變更所為之預算修正。
                            </div>
                        </th>
                    </tr>
                </template>
                <template v-if="!fEngChangeAdd">
                    <tr v-for="(item, index) in engChangeItems" v-bind:key="item.Seq" v-bind:class="{'bg-1-30':(item.Seq==selEngSeq), 'bg-1-10':(item.Seq!=selEngSeq)}">
                        <td>{{item.Version}}</td>
                        <td>{{item.ChangeTypeStr}}</td>
                        <template v-if="item.SPState==1 || (item.SPState==0 && item.Seq!=selEngSeq)">
                            <td>{{item.ChangeType==200 ? '-' : item.chsStartDate}}</td>
                            <td>{{item.ChangeType==200 ? '-' : item.chsSchCompDate}}</td>
                        </template>
                        <template v-if="item.SPState==0 && item.Seq==selEngSeq">
                            <td>
                                <b-input-group v-if="item.ChangeType!=200">
                                    <input v-bind:value="engMain.chsStartDate" disabled @change="onDateChange(engMain.chsStartDate, $event, 'chsStartDate')" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                                    <b-form-datepicker v-model="chsStartDate" :hide-header="true" button-only right size="sm" @context="onDatePicketChange($event, 'chsStartDate')">
                                    </b-form-datepicker>
                                </b-input-group>
                            </td>
                            <td>
                                <b-input-group v-if="item.ChangeType!=200">
                                    <input v-bind:value="engMain.chsSchCompDate" disabled @change="onDateChange(engMain.chsSchCompDate, $event, 'chsSchCompDate')"
                                           type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                                    <b-form-datepicker v-if="engMain.srcSchCompDate==null" v-model="chsSchCompDate" :hide-header="true"
                                                       button-only right size="sm" @context="onDatePicketChange($event, 'chsSchCompDate')">
                                    </b-form-datepicker>
                                </b-input-group>
                            </td>
                        </template>
                        <td>{{item.chsModifyTime}}</td>
                        <td style="text-align: center; width: 120px;">
                            <button v-if="item.SPState==0 && item.Seq==selEngSeq" @click="onSaveECDate(item)" class="btn btn-color11-2 btn-xs sharp" title="儲存"><i class="fas fa-save"></i></button>
                            <button v-if="item.SPState==0" @click="onEditPayItemClick(item)" type="button" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯">
                                <i class="fas fa-pencil-alt"></i>
                            </button>
                            <button v-if="selEngItem.SPState==1 && item.Seq==selEngSeq" @click="onDnSchProgress" type="button" class="btn btn-outline-secondary btn-sm" title="檔案下載">下載範本<i class="fas fa-download"></i></button>
                        </td>
                        <td style="text-align: center; width: 80px;">
                            <button v-if="item.SPState==1" @click="onEditPayItemClick(item)" type="button" class="btn btn-color11-3 btn-xs sharp mx-1" title="歷程">
                                <i class="fas fa-clock"></i>
                            </button>
                        </td>
                        <td v-if="fEngChangeCanAdd"></td>
                    </tr>
                </template>
            </tbody>
        </table>

        <div v-if="selEngSeq != -1">
            <h5>
                工程總價：{{tenderItem.SubContractingBudget}}元，綠色經費：{{greenFunding}}元，綠色經費占比：{{greenFundingRate}}%<br />
                核定碳排量：{{tenderItem.ApprovedCarbonQuantity}}&nbsp;kgCO2e，設計排放量：{{co2TotalDesign}}&nbsp;kgCO2e，施工碳排量：{{co2Total}}&nbsp;kgCO2e，可拆解率：{{dismantlingRate}}%
            </h5>
            <div v-if="selEngItem.SPState==0" style="display: flex;" class="row">
                <div class="col-10 col-sm-10 mb-3 mb-sm-1 mt-sm-2 mt-md-0">
                    <button type="button" title="填報完成" class="btn btn-outline-secondary btn-sm" data-toggle="modal" data-target="#checkCompleted">
                        填報完成&nbsp;<i class="fas fa-check"></i>
                    </button>
                    <button @click="onDelEngChange(selEngItem)" type="button" title="刪除變更" class="btn btn-color11-4 btn-sm">
                        刪除變更&nbsp;<i class="fas fa-times"></i>
                    </button>
                </div>
                <div class="col-2 col-sm-2 col-md-auto mb-3 mb-sm-1 mt-sm-2 mt-md-0">
                    <!-- s20231017 -->
                    <button v-if="!editAllQuantityItem" @click="onEditAllQuantityItem" type="button" title="數量編輯" class="btn btn-color11-1 btn-sm">
                        數量編輯&nbsp;<i class="fas fa-pencil-alt"></i>
                    </button>
                    <button v-if="editAllQuantityItem" @click="getRecords(selEngSeq)" type="button" title="關閉數量編輯" class="btn btn-color9-1 btn-sm">
                        關閉數量編輯&nbsp;<i class="fas fa-times"></i>
                    </button>
                </div>
            </div>
            <div class="modal fade" id="checkCompleted" style="display: none;" aria-hidden="true">
                <div class="modal-dialog modal-xl modal-dialog-centered " style="max-width: fit-content;">
                    <div class="modal-content">
                        <div class="card whiteBG mb-4 pattern-F colorset_1">
                            <div class="tab-content">
                                <div class="tab-pane active">
                                    <div class="swal2-icon swal2-warning swal2-icon-show" style="display: flex;"><div class="swal2-icon-content">!</div></div>
                                    <h2 style="display: block; ">工程變更, 是否確定?</h2>
                                    <div class="swal2-html-container" style="display: block;">
                                        <div style="text-align:left">工程變更日期: <font color="red">{{selEngItem.chsStartDate}}</font></div>
                                        <div style="text-align:left">預定完工日期: <font color="red">{{selEngItem.chsSchCompDate}}</font></div>
                                        <div style="text-align:left">1. 填報完成後將不能再修改</div>
                                        <div style="text-align:left">2. 所有 {{selEngItem.chsStartDate}}(含) 之後的預定進度及所有估驗請款 全部會被刪除.</div>
                                    </div>
                                    <div class="swal2-actions" style="display: flex;">
                                        <div class="swal2-loader"></div>
                                        <button @click="fillCompleted" type="button" class="swal2-confirm swal2-styled swal2-default-outline" style="display: inline-block; background-color: rgb(48, 133, 214);">是</button>
                                        <button type="button" data-toggle="modal" data-target="#checkCompleted" class="swal2-confirm swal2-styled swal2-default-outline" style="display: inline-block; background-color: rgb(221, 51, 51);">否</button>
                                        <button id="modalClose" type="button" data-toggle="modal" data-target="#checkCompleted" style="display: none"></button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <template v-if="selEngItem.ChangeType==1">
                <comm-pagination ref="pagination" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>

                <div class="table-responsive">
                    <table class="table table-responsive-md table-hover VA-middle">
                        <thead class="insearch">
                            <tr>
                                <th v-if="selEngItem.SPState==0 && editSeq==-99" style="text-align: center; width: 40px;">
                                    <button @click="onDelRecord" class="btn btn-color11-4 btn-xs sharp" title="刪除"><i class="fas fa-trash-alt"></i></button>
                                </th>
                                <th style="text-align: center;width: 50px;"><strong>序號</strong></th>
                                <th><strong>項次</strong></th>
                                <th><strong>施工項目</strong></th>
                                <th><strong>編碼</strong></th>
                                <th class="text-right"><strong>碳係數(kgCO2e)</strong></th>
                                <th><strong>單位</strong></th>
                                <th class="text-right"><strong>契約數量</strong></th>
                                <th class="text-right"><strong>單價(元)</strong></th>
                                <th class="text-right"><strong>金額(元)</strong></th>
                                <th style="text-align: center;width: 80px;">
                                    <strong>功能</strong>
                                </th>

                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in items" v-bind:key="item.Seq" v-bind:class="{'GreenFunding': item.GreenFundingSeq!=null, 'bg-1-30':item.GreenFundingSeq==null }">
                                <td v-if="selEngItem.SPState==0 && editSeq==-99" style="text-align: center;">
                                    <div class="d-flex justify-content-center">
                                        <input v-model="selDelItems" :value="item.Seq" :id="'del_'+item.Seq" type="checkbox">
                                        <!-- div class="custom-control custom-checkbox custom-control-inline">
                <input v-model="selDelItems" :value="item.Seq" :id="'del_'+item.Seq" type="checkbox" name="ExecutiveAgency" class="custom-control-input">
                <label :for="'del_'+item.Seq" class="custom-control-label"></label>
            </div -->
                                    </div>
                                </td>
                                <template v-if="editAllQuantityItem">
                                    <!-- 數量編輯 s20231017 -->
                                    <td class="text-center"><strong>{{pageRecordCount*(pageIndex-1)+index+1}}</strong></td>
                                    <td><strong>{{item.PayItem}}</strong></td>
                                    <td><strong>{{item.Description}}</strong></td>
                                    <td><strong>{{item.Memo}}</strong></td>
                                    <td class="text-right">{{item.KgCo2e}}</td>
                                    <td>{{item.Unit}}</td>
                                    <td class="text-right"><input v-model.number="item.Quantity" type="number" @change="onQuantityChanged(item)" class="form-control text-right"></td>
                                    <td class="text-right">{{numberComma(item.Price)}}</td>
                                    <td class="text-right">{{numberComma(Math.round(item.Quantity*item.Price))}}</td>
                                    <td></td>
                                </template>
                                <template v-if="!editAllQuantityItem && item.Seq != editSeq">
                                    <td class="text-center"><strong>{{pageRecordCount*(pageIndex-1)+index+1}}</strong></td>
                                    <td><strong>{{item.PayItem}}</strong></td>
                                    <td><strong>{{item.Description}}</strong></td>
                                    <td><strong>{{item.Memo}}</strong></td>
                                    <td class="text-right">{{item.KgCo2e}}</td>
                                    <td>{{item.Unit}}</td>
                                    <td class="text-right">{{item.Quantity}}</td>
                                    <td class="text-right">{{numberComma(item.Price)}}</td>
                                    <td class="text-right">{{numberComma(item.Amount)}}</td>
                                    <td style="text-align: center;">
                                        <div v-if="editSeq==-99" class="d-flex justify-content-center">
                                            <span>
                                                <button v-if="item.Seq == item.RootSeq" @click="onGetWList(item)" class="btn btn-color11-2 btn-xs sharp" data-toggle="modal" data-target="#prepare_edit01" title="價格分析"><i class="fas fa-newspaper"></i></button>
                                            </span>
                                            <span v-if="selEngItem.SPState==0">
                                                <button v-if="item.ItemMode == 0" @click="onNewRecordClick(item, index)" class="btn btn-color11-3 btn-xs sharp" title="新增"><i class="fas fa-plus"></i></button>
                                                <button @click="onEditRecord(item)" class="btn btn-color11-1 btn-xs sharp" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                            </span>
                                        </div>
                                    </td>
                                </template>
                                <template v-if="!editAllQuantityItem && item.Seq == editSeq && item.ItemMode == 0">
                                    <td class="text-center"><strong>{{pageRecordCount*(pageIndex-1)+index+1}}</strong></td>
                                    <td><strong>{{item.PayItem}}</strong></td>
                                    <td><strong>{{item.Description}}</strong></td>
                                    <td><strong>{{item.Memo}}</strong></td>
                                    <td class="text-right">{{item.KgCo2e}}</td>
                                    <td>{{item.Unit}}</td>
                                    <td><input v-model.number="editRecord.Quantity" type="number" class="form-control text-right"></td>
                                    <td class="text-right">{{item.Price}}</td>
                                    <td class="text-right"></td>
                                    <td style="text-align: center;">
                                        <div class="d-flex justify-content-center">
                                            <button @click="onSaveRecord" class="btn btn-color11-2 btn-xs sharp mx-1" title="儲存"><i class="fas fa-save"></i></button>
                                            <button @click="editSeq = -99" class="btn btn-color9-1 btn-xs sharp mx-1" title="取消"><i class="fas fa-times"></i></button>
                                        </div>
                                    </td>
                                </template>
                                <template v-if="!editAllQuantityItem && item.Seq == editSeq && item.ItemMode == 1">
                                    <td></td>
                                    <td><input v-model="editRecord.PayItem" maxlength="20" type="text" class="form-control"></td>
                                    <td><input v-model="editRecord.Description" maxlength="200" type="text" class="form-control"></td>
                                    <td><input v-model="editRecord.Memo" maxlength="100" type="text" class="form-control"></td>
                                    <td><input v-model="editRecord.KgCo2e" type="text" class="form-control"></td>
                                    <td><input v-model="editRecord.Unit" maxlength="10" type="text" class="form-control"></td>
                                    <td><input v-model.number="editRecord.Quantity" type="number" class="form-control text-right"></td>
                                    <td><input v-model.number="editRecord.Price" type="number" class="form-control text-right"></td>
                                    <td></td>
                                    <td style="text-align: center;">
                                        <div class="d-flex justify-content-center">
                                            <button @click="onSaveNewRecordClick" class="btn btn-color11-2 btn-xs sharp mx-1" title="儲存"><i class="fas fa-save"></i></button>
                                            <button @click="onCancelNewRecordClick" class="btn btn-color9-1 btn-xs sharp mx-1" title="取消"><i class="fas fa-times"></i></button>
                                        </div>
                                    </td>
                                </template>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </template>
        </div>
        <!-- 價格分析清單 -->
        <div class="modal fade" id="prepare_edit01">
            <div class="modal-dialog modal-xl modal-dialog-centered " style="max-width: fit-content;">
                <div class="modal-content">
                    <div class="modal-header">
                        <h6 class="modal-title redText">單價分析</h6>
                        <button ref="closeCopyModal" type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">×</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <table class="table table-responsive-md table-hover VA-middle">
                            <thead class="insearch">
                                <tr>
                                    <th colspan="3"><strong>工料項目:{{selEngItem.Description}}</strong></th>
                                    <th colspan="2"><strong>單位:{{selEngItem.Unit}}</strong></th>
                                    <th colspan="3"><strong>計價代碼:{{selEngItem.Memo}}</strong></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr class="bg-1-30">
                                    <td><strong>工料名稱</strong></td>
                                    <td class="text-right"><strong>單位</strong></td>
                                    <td class="text-right"><strong>數量</strong></td>
                                    <td class="text-right"><strong>單價</strong></td>
                                    <td class="text-right"><strong>複價</strong></td>
                                    <td><strong>編碼</strong></td>
                                    <td><strong>備註</strong></td>
                                    <td style="text-align: center;width: 90px;">
                                        <div v-if="selEngItem.SPState==0">
                                            功能
                                            <button @click="onNewWItemClick" class="btn btn-color11-3 btn-xs sharp mx-1" title="新增"><i class="fas fa-plus"></i></button>
                                        </div>
                                    </td>
                                </tr>
                                <tr v-if="isNewWItem" class="bg-1-30">
                                    <td><input v-model.trim="newWItem.Description" maxlength="200" type="text" class="form-control"></td>
                                    <td style="width: 90px;"><input v-model.trim="newWItem.Unit" maxlength="10" type="text" class="form-control"></td>
                                    <td style="width: 120px;"><input v-model.number="newWItem.Quantity" type="number" class="form-control"></td>
                                    <td style="width: 100px;"><input v-model.number="newWItem.Price" type="number" class="form-control"></td>
                                    <td style="width: 160px;"><input v-model.number="newWItem.Amount" type="number" class="form-control"></td>
                                    <td><input v-model.trim="newWItem.ItemCode" maxlength="20" type="text" class="form-control"></td>
                                    <td><input v-model.trim="newWItem.Remark" maxlength="100" type="text" class="form-control"></td>
                                    <td style="text-align: center;">
                                        <button @click="onSaveWItemClick(newWItem)" class="btn btn-color11-2 btn-xs sharp" title="儲存"><i class="fas fa-save"></i></button>
                                        <button @click="isNewWItem=false" class="btn btn-color9-1 btn-xs sharp" title="取消"><i class="fas fa-times"></i></button>
                                    </td>
                                </tr>
                                <tr v-for="(item, index) in workItems" v-bind:key="item.Seq" class="bg-1-30">
                                    <template v-if="item.Seq != editWSeq">
                                        <td><strong>{{item.Description}}</strong></td>
                                        <td class="text-right"><strong>{{item.Unit}}</strong></td>
                                        <td class="text-right"><strong>{{item.Quantity}}</strong></td>
                                        <td class="text-right"><strong>{{item.Price}}</strong></td>
                                        <td class="text-right"><strong>{{item.Amount}}</strong></td>
                                        <td><strong>{{item.ItemCode}}</strong></td>
                                        <td><strong>{{item.Remark}}</strong></td>
                                        <td style="text-align: center;">
                                            <div v-if="selEngItem.SPState==0">
                                                <button @click="onEditWItem(item)" class="btn btn-color11-1 btn-xs sharp" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                                <button @click="onDelWItem(item)" class="btn btn-color11-4 btn-xs sharp" title="刪除"><i class="fas fa-trash-alt"></i></button>
                                            </div>
                                        </td>
                                    </template>
                                    <template v-if="item.Seq == editWSeq">
                                        <td><input v-model.trim="editWRecord.Description" maxlength="200" type="text" class="form-control"></td>
                                        <td style="width: 90px;"><input v-model.trim="editWRecord.Unit" maxlength="10" type="text" class="form-control"></td>
                                        <td style="width: 120px;"><input v-model.number="editWRecord.Quantity" type="number" class="form-control"></td>
                                        <td style="width: 100px;"><input v-model.number="editWRecord.Price" type="number" class="form-control"></td>
                                        <td style="width: 160px;"><input v-model.number="editWRecord.Amount" type="number" class="form-control"></td>
                                        <td><input v-model.trim="editWRecord.ItemCode" maxlength="20" type="text" class="form-control"></td>
                                        <td><input v-model.trim="editWRecord.Remark" maxlength="100" type="text" class="form-control"></td>
                                        <td style="text-align: center;">
                                            <button @click="onSaveWItemClick(editWRecord)" class="btn btn-color11-2 btn-xs sharp" title="儲存"><i class="fas fa-save"></i></button>
                                            <button @click="editWSeq=-99" class="btn btn-color9-1 btn-xs sharp" title="取消"><i class="fas fa-times"></i></button>
                                        </td>
                                    </template>
                                </tr>
                                <tr class="bg-1-30">
                                    <td><strong>合計</strong></td>
                                    <td class="text-right"><strong>{{selEngItem.Unit}}</strong></td>
                                    <td class="text-right"><strong>1</strong></td>
                                    <td class="text-right"><strong></strong></td>
                                    <td class="text-right"><strong>{{workItemsAmount}}</strong></td>
                                    <td><strong></strong></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['tenderItem', 'sepHeader'],
        components: {
            ExtendConstruction: require('./CalendarSetting_ExtendConstruction.vue').default,
            WorkStopBack: require('./CalendarSetting_WorkStopBack.vue').default,
        },
        data: function () {
            return {
                targetId: null,
                //
                items: [],
                editInx: -99,
                editSeq: -99,
                editRecord: {},
                //分頁
                recordTotal: 0,
                pageRecordCount: 30,
                pageIndex: 0,
                //
                selEngSeq: -1,
                selEngItem: {},
                //
                chsStartDate: '',
                chsSchCompDate: '',
                engMain: { Seq:-1, chsStartDate: '', chsSchCompDate:'' },
                engChangeItems: [],
                fEngChangeCanAdd: false,
                fEngChangeAdd: false,
                //
                selPayItem: {},
                editWSeq: -99,
                editWRecord: {},
                newWItem: {},
                isNewWItem:false,
                workItems: [],
                workItemsAmount: null,
                engChangeType: -1,
                co2Total: null,
                dismantlingRate: null,//可拆解率
                co2TotalDesign: null, //設計排放量
                approvedCarbonQuantity: null,
                greenFunding: null,
                greenFundingRate: null,
                selDelItems: [],//s20230530
                editAllQuantityItem: false,//s20231017
            };
        },
        methods: {
            //s20231017
            onQuantityChanged(item) {
                console.log(item);
                if (this.strEmpty(item.Quantity)) {
                    alert('數量 必須輸入且不得為0');
                    return;
                }
                window.myAjax.post('/EPCProgressEngChange/UpdateRecord', { m: item })
                    .then(resp => {
                        if (resp.data.result != 0) {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //數量編輯
            onEditAllQuantityItem() {
                this.editSeq = -99;
                this.editAllQuantityItem = true;
            },

            //千分位 s20231014
            numberComma(data) {
                return window.comm.numberComma(data);
            },
            //儲存日期 s20231014
            onSaveECDate(item) {
                //console.log(this.$refs['chsStartDate1']);
                //this.engMain.chsStartDate = this.$refs['chsStartDate1'].value;
                if (window.comm.stringEmpty(this.engMain.chsStartDate)) {
                    this.$swal("工程開始變更日期 必須輸入");
                    return;
                }
                //this.engMain.chsSchCompDate = this.$refs['chsSchCompDate1'].value;
                if (window.comm.stringEmpty(this.engMain.chsSchCompDate)) {
                    this.$swal("預計完工日期 必須輸入");
                    return;
                }
                console.log(this.engMain);
                window.myAjax.post('/EPCProgressEngChange/UpdateEngDate', { eng: this.engMain, id: item.Seq, mode: item.ChangeType })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.selEngItem.chsStartDate = this.engMain.chsStartDate;
                            this.selEngItem.chsSchCompDate = this.engMain.chsSchCompDate;
                        }
                        this.$swal(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //刪除工程變更 s20230927
            onDelEngChange(ecItem) {
                if (confirm('是否確定刪除此工程變更？')) {
                    window.myAjax.post('/EPCProgressEngChange/DelEngChange', { ec: ecItem })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getEngChangeList();
                            } else
                                alert(resp.data.msg);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            //取消
            onCloseEvent(mode, engChangeSeq) {
                this.fEngChangeAdd = false;
                if (mode == 1) {
                    this.getEngChangeList();
                    this.getRecords(engChangeSeq);
                }
            },
            //
            //刪除 價格
            onDelWItem(item) {
                if (confirm('是否確定刪除資料？')) {
                    window.myAjax.post('/EPCProgressEngChange/DelWItem', { id: item.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.onGetWList(this.selPayItem);
                            }
                            alert(resp.data.msg);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            //編輯 價格
            onEditWItem(item) {
                if (this.editWSeq > -99) return;
                this.editWRecord = Object.assign({}, item);
                this.editWSeq = this.editWRecord.Seq;
            },
            //儲存 價格
            onSaveWItemClick(item) {
                if (this.strEmpty(item.Description) || this.strEmpty(item.Unit) || this.strEmpty(item.ItemCode)) {
                    alert('工料名稱,單位,編碼 必須輸入');
                    return;
                }
                if (this.strEmpty(item.Quantity) || this.strEmpty(item.Price) || this.strEmpty(item.Amount)) {
                    alert('數量,單價,複價 必須輸入且不得為0');
                    return;
                }
                window.myAjax.post('/EPCProgressEngChange/UpdateWItem', { m: item })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.isNewWItem = false;
                            this.onGetWList(this.selPayItem);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //新增價格
            onNewWItemClick() {
                this.isNewWItem = false;
                window.myAjax.post('/EPCProgressEngChange/GetNewWItem', { id: this.selPayItem.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.newWItem = resp.data.item;
                            this.isNewWItem = true;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //單價分析清單
            onGetWList(item) {
                this.selPayItem = item;
                this.workItemsAmount = null;
                this.editWSeq = -99;
                this.isNewWItem = false;
                window.myAjax.post('/EPCProgressEngChange/GetWList', { id: item.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.workItems = resp.data.items;
                            if (this.workItems.length > 0) {
                                this.workItemsAmount = 0
                                for (var i = 0; i < this.workItems.length; i++) {
                                    this.workItemsAmount += this.workItems[i].Amount;
                                }
                            }
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //日期
            onDateChange(srcDate, event, mode) {
                if (event.target.value.length == 0) {
                    if (mode == 'chsStartDate')
                        this.chsStartDate = '';
                    else if (mode == 'chsSchCompDate')
                        this.chsSchCompDate = '';
                    return;
                }
                if (!window.comm.isExistDate(event.target.value)) {
                    event.target.value = srcDate;
                    alert("日期錯誤");
                } else {
                    console.log('onDateChange');
                    if (mode == 'chsStartDate') {
                        this.chsStartDate = window.comm.toYearDate(event.target.value);
                    } else if (mode == 'chsSchCompDate')
                        this.chsSchCompDate = window.comm.toYearDate(event.target.value);
                }
            },
            onDatePicketChange(ctx, mode) {
                //console.log(ctx);
                if (ctx.selectedDate != null) {
                    var d = ctx.selectedDate;
                    var dd = (d.getFullYear() - 1911) + '/' + (d.getMonth() + 1) + '/' + d.getDate();
                    //var y = d.getYear() - 1911;
                    if (mode == 'chsStartDate')
                        this.engMain.chsStartDate = dd;
                    else if (mode == 'chsSchCompDate')
                        this.engMain.chsSchCompDate = dd;
                }
            },
            onPaginationChange(pInx, pCount) {
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
                if (this.selEngSeq > 0) this.getRecords(this.selEngSeq);
            },
            strEmpty(str) {
                return window.comm.stringEmpty(str);
            },
            //下載預定進度範本
            onDnSchProgress() {
                window.comm.dnFile('/EPCProgressEngChange/DnSchProgress?id=' + this.selEngItem.Seq);
            },
            //填報完成
            fillCompleted() {
                document.getElementById("modalClose").click();
                
                window.myAjax.post('/EPCProgressEngChange/FillCompleted', { id: this.selEngItem.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getEngChangeList();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //刪除
            onDelRecord() {
                if (confirm('是否確定刪除資料？')) {
                    window.myAjax.post('/EPCProgressEngChange/DelRecords', { ids: this.selDelItems })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getRecords(this.selEngSeq);
                            }
                            alert(resp.data.msg);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            //自行新增
            onNewRecordClick(item, index) {
                this.editInx = index + 1;
                this.editRecord = Object.assign({}, item);
                this.editRecord.Description = '';
                this.editRecord.Memo = '';
                this.editRecord.Unit = '';
                this.editRecord.KgCo2e = null;
                this.editRecord.Quantity = null;
                this.editRecord.Price = null;
                this.editRecord.Seq = -1;
                this.editRecord.ItemMode = 1;
                this.items.splice(this.editInx, 0, this.editRecord);
                this.editSeq = this.editRecord.Seq;
            },
            //取消新增
            onCancelNewRecordClick() {
                if (this.editRecord.Seq == -1) {
                    this.items.splice(this.editInx, 1);
                }
                this.editInx = -99;
                this.editSeq = -99;
            },
            //儲存新增項目
            onSaveNewRecordClick() {
                if ((this.strEmpty(this.editRecord.PayItem) || this.strEmpty(this.editRecord.Description))) {
                    alert('項次,施工項目 必須輸入');
                    return;
                }
                if ((this.strEmpty(this.editRecord.Quantity) || this.strEmpty(this.editRecord.Price))) {
                    alert('數量,單價 必須輸入且不得為0');
                    return;
                }
                if (this.editRecord.Seq == -1) {
                    window.myAjax.post('/EPCProgressEngChange/AddRecord', { id: this.selEngSeq, m: this.editRecord })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getRecords(this.selEngSeq);
                            } else
                                alert(resp.data.msg);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                } else {
                    this.onSaveRecord();
                }
            },
            //儲存
            onSaveRecord() {
                if (this.strEmpty(this.editRecord.Quantity)) {
                    alert('數量 必須輸入且不得為0');
                    return;
                }
                window.myAjax.post('/EPCProgressEngChange/UpdateRecord', { m: this.editRecord })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getRecords(this.selEngSeq);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //編輯紀錄
            onEditRecord(item) {
                if (this.editSeq > -99) return;
                this.editRecord = Object.assign({}, item);
                this.editSeq = this.editRecord.Seq;
            },
            //編輯 Payitem
            onEditPayItemClick(item) {
                this.selEngItem = item;
                if (item.SPState == 0) {//s20231014
                    this.engMain.Seq = this.selEngItem.EngMainSeq;
                    this.engMain.chsStartDate = this.selEngItem.chsStartDate;
                    this.chsStartDate = window.comm.toYearDate(this.engMain.chsStartDate);
                    this.engMain.chsSchCompDate = this.selEngItem.chsSchCompDate;
                    this.chsSchCompDate = window.comm.toYearDate(this.engMain.chsSchCompDate);
                }

                this.selEngSeq = this.selEngItem.Seq;
                this.getRecords(this.selEngSeq);
            },
            //清單
            getRecords(id) {
                this.editAllQuantityItem = false;
                this.editSeq = -99;
                this.items = [];
                window.myAjax.post('/EPCProgressEngChange/GetList', {
                    id: id,
                    eng: this.targetId,
                    pageIndex: this.pageIndex,
                    perPage: this.pageRecordCount
                })
                .then(resp => {
                    if (resp.data.result >= 0) {
                        this.items = resp.data.items;
                        this.recordTotal = resp.data.totalRows;
                        this.co2Total = resp.data.co2Total;
                        this.co2TotalDesign = resp.data.co2TotalDesign;
                        this.dismantlingRate = resp.data.dismantlingRate;
                        this.greenFunding = resp.data.greenFunding;
                        this.greenFundingRate = resp.data.greenFundingRate;
                    } else
                        alert(resp.data.msg);
                })
                .catch(err => {
                    console.log(err);
                });
            },
            //取標案
            getItem() {
                this.pageIndex = 1;
                this.getEngChangeList();
            },
            //工程變更清單
            getEngChangeList() {
                this.editSeq = -99;
                this.items = [];
                this.selEngSeq = -1;
                this.selEngItem = {};
                window.myAjax.post('/EPCProgressEngChange/GetECList', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result >= 0) {
                            this.fEngChangeCanAdd = resp.data.addFlag;
                            this.engChangeItems = resp.data.items;
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //新工程變更
            onEngChangeAddClick() {
                this.engChangeType = -1;//s20231014
                this.selEngSeq = -1;
                this.editSeq = -99;
                this.items = [];
                this.fEngChangeAdd = false;
                this.engMain = { Seq: this.tenderItem.Seq, chsStartDate: '', chsSchCompDate: '', ChangeType: 1 };
                window.myAjax.post('/EPCSchProgress/GetEngItem', { id: this.tenderItem.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engMain.chsSchCompDate = resp.data.item.ScheChangeCloseDateStr;
                            this.chsSchCompDate = window.comm.toYearDate(this.engMain.chsSchCompDate);
                            /* s20230428 工程會資料因素, 開放人工輸入
                            if (window.comm.stringEmpty(this.engMain.chsSchCompDate)) {
                                this.$swal('變更後預定完工日期, 系統尚未設置. 無法作業');
                            } else {
                                this.checkEngChange();
                            }*/
                            this.checkEngChange();
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //檢查是否可新增
            checkEngChange() {
                window.myAjax.post('/EPCProgressEngChange/CheckECState', { id: this.tenderItem.Seq })
                    .then(resp => {
                        if (resp.data.result == 0)
                            this.fEngChangeAdd = true;
                        else
                            this.$swal(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //儲存新工程變更
            onSaveNewEngChangeClick() {
                this.engMain.chsStartDate = this.$refs['chsStartDate'].value;
                if (window.comm.stringEmpty(this.engMain.chsStartDate)) {
                    this.$swal("工程開始變更日期 必須輸入");
                    return;
                }
                this.engMain.chsSchCompDate = this.$refs['chsSchCompDate'].value;
                if (window.comm.stringEmpty(this.engMain.chsSchCompDate)) {
                    this.$swal("預計完工日期 必須輸入");
                    return;
                }
                this.engMain.ChangeType = this.engChangeType;
                console.log(this.engMain);
                window.myAjax.post('/EPCProgressEngChange/AddEngChange', { eng: this.engMain })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.fEngChangeAdd = false;
                            this.getEngChangeList();
                            this.getRecords(resp.data.id);
                        } else
                            this.$swal(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        async mounted() {
            console.log('mounted() 工程變更');
            this.targetId = this.tenderItem.Seq;
            if (this.targetId == null) {
                alert('請先選取標案');
                return;
            }
            this.getItem();
        },
    }
</script>