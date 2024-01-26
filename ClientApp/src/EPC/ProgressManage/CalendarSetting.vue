<template>
    <div>
        <div class="row justify-content-center">
            <div class="col-12 col-md-8">
                <vc-calendar :attributes="attrs" @dayclick="dayClickHandler" @update:from-page="fromPageHandler" :columns="1" :step="1" is-expanded style="max-width: 800px;"></vc-calendar>
            </div>
            <div class="col-12 col-md-4">
                <div class="row small">
                    <div class="col">
                        <span>開工日期：{{formatDateStr(tenderItem.ActualStartDate)}}</span>
                        <div class="form-inline">
                            <div class="d-flex align-items-center m-1"><i class="fas fa-square-full text-success mr-1"></i> 已填寫</div>
                            <div class="d-flex align-items-center m-1"><i class="fas fa-square-full text-warning mr-1"></i> 逾期未填</div>
                            <div class="d-flex align-items-center m-1"><i class="fas fa-square-full text-danger mr-1"></i> 停工</div>
                            <!-- div class="d-flex align-items-center m-1"><i class="fas fa-square-full text-info mr-1"></i> 不計工期</div -->
                        </div>
                    </div>
                </div>
                <div class="row mb-2">
                    <button v-on:click="onWorkModalOpen()" type="button" class="btn btn-color11-4 btn-block btn-sm" data-toggle="modal" data-target="#date_stopAndstart">設定停工 / 復工</button>
                    <button v-on:click="onExtensionModalOpen()" type="button" class="btn btn-color11-4 btn-block btn-sm mb-1" data-toggle="modal" data-target="#date_extend">設定展延工期</button>
                    <!--
        <button v-on:click="onNoDurationModalOpen()" type="button" class="btn btn-color-1 btn-sm mb-1" data-toggle="modal" data-target="#date_NotContainHoliday">設定不計工期</button>
        <button v-on:click="onHolidayModalOpen()" type="button" class="btn btn-color-1 btn-sm mb-1" data-toggle="modal" data-target="#date_ContainHoliday">設定假日計工期</button>
        -->
                </div>
                <div class="row small mb-2">
                    <div class="col-3 pr-0" style="max-width:112px">本日天氣:上午</div>
                    <div class="col-4 pl-0"><input v-model.trim="supDailyDateNote.Weather1" maxlength="30" type="text" class="form-control "></div>
                    <div class="col pl-0 pr-0" style="max-width:35px">下午</div>
                    <div class="col-4 pl-0"><input v-model.trim="supDailyDateNote.Weather2" maxlength="30" type="text" class="form-control"></div>
                </div>
                <div class="row small">
                    <div class="col-3 pr-0" style="max-width:112px">填報日期:</div>
                    <div class="col-4 pl-0" style="min-width:170px;"><input v-model.trim="supDailyDateNote.FillinDate" type="date" class="form-control "></div>
                </div>
            </div>
            
        </div>
        <p>&nbsp;</p>
        <!-- 小視窗 設定停工 / 復工 -->
        <div class="modal fade" id="date_stopAndstart">
            <div class="modal-dialog modal-xl modal-dialog-centered ">
                <div class="modal-content">
                    <div class="modal-header bg-1 text-white">
                        <h6 class="modal-title font-weight-bold">設定停工 / 復工</h6>
                        <button v-on:click="onCloseModal" type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">×</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <h5 class="mt-0">工程編號：{{tenderItem.TenderNo}} &nbsp; 工程名稱：{{tenderItem.TenderName}}</h5>
                        <div class="table-responsive">
                            <table class="table table-hover table2">
                                <tbody>
                                    <tr>
                                        <th colspan="2" class="bg-1-30">停工</th>
                                    </tr>
                                    <tr>
                                        <th>停工期間</th>
                                        <td>
                                            <div class="form-inline">
                                                <input v-model="reportWork.SStopWorkDateStr" :disabled="reportWork.EC_SchEngProgressHeaderSeq != null" type="date" class="form-control my-1 mr-0 mr-sm-1">&nbsp;~&nbsp;
                                                <input v-model="reportWork.EStopWorkDateStr" :disabled="reportWork.EC_SchEngProgressHeaderSeq != null" type="date" class="form-control my-1">
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>停工原因</th>
                                        <td>
                                            <textarea v-model="reportWork.StopWorkReason" maxlength="250" rows="5" class="form-control"></textarea>
                                            <span class="text-B small">※ 停工原因請在250字內說明(包含標點符號)</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>停工文號</th>
                                        <td>
                                            <input v-model="reportWork.StopWorkNo" maxlength="20" type="text" class="form-control" value="">
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>核定公文檔案</th>
                                        <td>
                                            <b-form-file v-model="StopWorkApprovalFile" placeholder="瀏覽檔案"></b-form-file>
                                            <span class="text-B small">※ 建議檔案大小勿超過5MB，最大限制為20MB</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th colspan="2" class="bg-1-30">復工</th>
                                    </tr>
                                    <tr>
                                        <th>復工日期</th>
                                        <td>
                                            <div class="form-inline">
                                                <input v-model="reportWork.BackWorkDateStr" :disabled="reportWork.EC_SchEngProgressHeaderSeq != null" type="date" class="form-control my-1 mr-0 mr-sm-1">
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>復工文號</th>
                                        <td>
                                            <input v-model="reportWork.BackWorkNo" maxlength="20" type="text" class="form-control" value="">
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>核定公文檔案</th>
                                        <td>
                                            <b-form-file v-model="BackWorkApprovalFile" placeholder="瀏覽檔案"></b-form-file>
                                            <span class="text-B small">※ 建議檔案大小勿超過5MB，最大限制為20MB</span>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <div class="row justify-content-center">
                            <div class="col-12 col-sm-4 col-xl-2 my-2">
                                <button v-on:click.stop="onWorkSave" role="button" class="btn btn-shadow btn-block btn-outline-secondary"> {{reportWork.Seq == -1 ? '新增':'儲存'}} <i class="fas fa-plus"></i></button>
                            </div>
                            <div class="col-12 col-sm-4 col-xl-2 my-2">
                                <button v-on:click.stop="initReportWork()" role="button" class="btn btn-shadow btn-color3 btn-block"> 取消 </button>
                            </div>
                        </div>
                        <div class="table-responsive mt-3">
                            <table class="table table-responsive-md table-hover">
                                <thead class="insearch">
                                    <tr>
                                        <th><strong>次數</strong></th>
                                        <th><strong>停工日期</strong></th>
                                        <th><strong>停工文號</strong></th>
                                        <th><strong>停工原因</strong></th>
                                        <th><strong>復工日期</strong></th>
                                        <th><strong>復工文號</strong></th>
                                        <th class="text-center"><strong>瀏覽核可公文</strong></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-on:click.stop="onWorkItemClick(item)" v-for="(item, index) in workItems" v-bind:key="item.Seq">
                                        <td>{{index+1}}</td>
                                        <td>{{item.SStopWorkDateStr}}</td>
                                        <td>{{item.StopWorkNo}}</td>
                                        <td>{{item.StopWorkReason}}</td>
                                        <td>{{item.BackWorkDateStr}}</td>
                                        <td>{{item.BackWorkNo}}</td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button v-on:click.stop="onDownload(item, 1)" type="button" v-bind:disabled="item.StopWorkApprovalFile == ''" class="btn btn-outline-secondary btn-sm mx-1">停工 <i class="fas fa-eye"></i></button>
                                                <button v-on:click.stop="onDownload(item, 2)" type="button" v-bind:disabled="item.BackWorkApprovalFile == ''" class="btn btn-outline-secondary btn-sm mx-1">復工 <i class="fas fa-eye"></i></button>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- 小視窗 設定展延工期 -->
        <div class="modal fade" id="date_extend">
            <div class="modal-dialog modal-xl modal-dialog-centered ">
                <div class="modal-content">
                    <div class="modal-header bg-1 text-white">
                        <h6 class="modal-title font-weight-bold">設定展延工期</h6>
                        <button v-on:click="onCloseModal" type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">×</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <h5 class="mt-0">工程編號：{{tenderItem.TenderNo}} &nbsp; 工程名稱：{{tenderItem.TenderName}}</h5>
                        <div class="table-responsive">
                            <table class="table table-hover table2">
                                <tbody>
                                    <tr>
                                        <th>展延天數</th>
                                        <td>
                                            <div class="input-group mb-2">
                                                <input v-model="reportExtension.ExtendDays" type="text" class="form-control">
                                                <div class="input-group-append">
                                                    <div class="input-group-text">天</div>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>公文核准文號</th>
                                        <td>
                                            <input v-model="reportExtension.ApprovalNo" maxlength="20" type="text" class="form-control" value="">
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>公文核准日期</th>
                                        <td>
                                            <div class="form-inline">
                                                <input v-model="reportExtension.ApprovalDate" type="date" class="form-control my-1">
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>原因</th>
                                        <td>
                                            <div class="custom-control custom-radio">
                                                <input v-model="reportExtension.ExtendReason" value="1" type="radio" id="customRadio_reason01" name="customRadio_reason" class="custom-control-input">
                                                <label class="custom-control-label" for="customRadio_reason01">發生第17條第5款不可抗力或不可歸責契約當事人之事故</label>
                                            </div>
                                            <div class="custom-control custom-radio">
                                                <input v-model="reportExtension.ExtendReason" value="2" type="radio" id="customRadio_reason02" name="customRadio_reason" class="custom-control-input">
                                                <label class="custom-control-label" for="customRadio_reason02">因天候影響無法施工</label>
                                            </div>
                                            <div class="custom-control custom-radio">
                                                <input v-model="reportExtension.ExtendReason" value="3" type="radio" id="customRadio_reason03" name="customRadio_reason" class="custom-control-input">
                                                <label class="custom-control-label" for="customRadio_reason03">機關要求全部或部分停工</label>
                                            </div>
                                            <div class="custom-control custom-radio">
                                                <input v-model="reportExtension.ExtendReason" value="4" type="radio" id="customRadio_reason04" name="customRadio_reason" class="custom-control-input">
                                                <label class="custom-control-label" for="customRadio_reason04">機關應辦事項未及時辦妥</label>
                                            </div>
                                            <div class="custom-control custom-radio">
                                                <input v-model="reportExtension.ExtendReason" value="5" type="radio" id="customRadio_reason05" name="customRadio_reason" class="custom-control-input">
                                                <label class="custom-control-label" for="customRadio_reason05">由機關自辦或機關之其他廠商之延誤而影響履約進度者</label>
                                            </div>
                                            <div class="custom-control custom-radio">
                                                <input v-model="reportExtension.ExtendReason" value="6" type="radio" id="customRadio_reason06" name="customRadio_reason" class="custom-control-input">
                                                <label class="custom-control-label" for="customRadio_reason06">機關提供之地質資料，與實際情形有重大差異</label>
                                            </div>
                                            <div class="custom-control custom-radio">
                                                <input v-model="reportExtension.ExtendReason" value="7" type="radio" id="customRadio_reason07" name="customRadio_reason" class="custom-control-input">
                                                <label class="custom-control-label" for="customRadio_reason07">因傳染病或政府之行為</label>
                                            </div>
                                            <div class="custom-control custom-radio">
                                                <input v-model="reportExtension.ExtendReason" value="8" type="radio" id="customRadio_reason08" name="customRadio_reason" class="custom-control-input">
                                                <label class="custom-control-label" for="customRadio_reason08">因機關使用或佔用本工程任何部分，但契約另有規定者，不再此限</label>
                                            </div>
                                            <div class="custom-control custom-radio">
                                                <input v-model="reportExtension.ExtendReason" value="9" type="radio" id="customRadio_reason09" name="customRadio_reason" class="custom-control-input">
                                                <label class="custom-control-label" for="customRadio_reason09">其他非可歸責於廠商之情形，經機關認定者</label>
                                            </div>
                                            <input v-model="reportExtension.ExtendReasonOther" maxlength="100" type="text" class="form-control" value="">
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <div class="row justify-content-center">
                            <div class="col-12 col-sm-4 col-xl-2 my-2">
                                <button v-on:click.stop="onExtensionAdd" role="button" class="btn btn-shadow btn-block btn-color11-3"> 新增 <i class="fas fa-plus-square fa-lg"></i></button>
                            </div>
                        </div>
                        <div class="table-responsive mt-3">
                            <table class="table table-responsive-md table-hover">
                                <thead class="insearch">
                                    <tr>
                                        <th class="text-left"><strong>展延天數(天)</strong></th>
                                        <th class="text-left"><strong>公文核准文號</strong></th>
                                        <th class="text-left"><strong>公文核准日期</strong></th>
                                        <th class="text-left"><strong>原因</strong></th>
                                        <th class="text-left"><strong>狀態</strong></th>
                                        <th class="text-center"><strong>功能</strong></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(item, index) in extensionItems" v-bind:key="item.Seq">
                                        <template v-if="item.Seq != editSeq">
                                            <td>{{item.ExtendDays}}</td>
                                            <td>{{item.ApprovalNo}}</td>
                                            <td>{{item.ApprovalDateStr}}</td>
                                            <td>{{getExtendReasonText(item)}}<br />{{item.ExtendReasonOther}}</td>
                                            <td>同意</td>
                                            <td>
                                                <div class="d-flex justify-content-center">
                                                    <button @click="onExtensionEdit(item)" class="btn btn-color11-1 btn-xs sharp m-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                                    <button v-if="item.SupDailyReportExtensionSeq == null" @click="onExtensionDel(item)" class="btn btn-color9-1 btn-xs sharp m-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                                                </div>
                                            </td>
                                        </template>
                                        <template v-if="item.Seq == editSeq">
                                            <td><input v-model.number="editRecord.ExtendDays" :disabled="editRecord.SupDailyReportExtensionSeq != null" type="number" class="form-control"></td>
                                            <td><input v-model.trim="editRecord.ApprovalNo" maxlength="20" type="text" class="form-control"></td>
                                            <td><input v-model="editRecord.ApprovalDate" type="date" class="form-control"></td>
                                            <td>
                                                <select v-model.trim="editRecord.ExtendReason" class="form-control">
                                                    <option v-for="option in extendReasonOpions" v-bind:value="option.Value" v-bind:key="option.Value">{{option.Text}}</option>
                                                </select>
                                                <input v-model.trim="editRecord.ExtendReasonOther" maxlength="100" type="text" class="form-control">
                                            </td>
                                            <td></td>
                                            <td style="min-width: unset;">
                                                <div class="d-flex justify-content-center">
                                                    <button @click="onExtensionUpdate(editRecord)" class="btn btn-color11-2 btn-xs sharp m-1" title="儲存"><i class="fas fa-save"></i></button>
                                                    <button @click="onExtensionEditCancel" class="btn btn-color9-1 btn-xs sharp m-1" title="取消"><i class="fas fa-times"></i></button>
                                                </div>
                                            </td>
                                        </template>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- 小視窗 設定不計工期 -->
        <div class="modal fade" id="date_NotContainHoliday">
            <div class="modal-dialog modal-lg modal-dialog-centered ">
                <div class="modal-content">
                    <div class="modal-header bg-1 text-white">
                        <h6 class="modal-title font-weight-bold">設定不計工期</h6>
                        <button v-on:click="onCloseModal" type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">×</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <h5 class="mt-0">工程編號：{{tenderItem.TenderNo}} &nbsp; 工程名稱：{{tenderItem.TenderName}}</h5>
                        <div class="table-responsive">
                            <table class="table table-hover table2">
                                <tbody>
                                    <tr>
                                        <th>起始日期</th>
                                        <td>
                                            <div class="form-inline">
                                                <input v-model="reportNoDuration.StartDate" @change="onNoDurationStartDateChange" type="date" class="form-control my-1">
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>結束日期</th>
                                        <td>
                                            <div class="form-inline">
                                                <input v-model="reportNoDuration.EndDate" type="date" class="form-control my-1">
                                                <span class="text-B small">&nbsp;※ 非必填</span>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>變更設定</th>
                                        <td>
                                            <select v-model="reportNoDuration.DaySet" class="form-control">
                                                <option value="1">休息日</option>
                                                <option value="2">工作日</option>
                                            </select>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>說明</th>
                                        <td><input v-model="reportNoDuration.Descript" type="text" class="form-control" value=""></td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <div class="row justify-content-center">
                            <div class="col-12 col-sm-4 my-2">
                                <button v-on:click.stop="onNoDurationAdd" role="button" class="btn btn-shadow btn-block btn-color11-3"> 新增 <i class="fas fa-plus-square fa-lg"></i></button>
                            </div>
                        </div>
                        <div class="table-responsive mt-3">
                            <table class="table table-responsive-sm table-hover">
                                <thead class="insearch">
                                    <tr>
                                        <th><strong>起始日期</strong></th>
                                        <th><strong>結束日期</strong></th>
                                        <th><strong>變更設定</strong></th>
                                        <th><strong>說明</strong></th>
                                        <th class="text-center"><strong>功能</strong></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(item, index) in noDurationItems" v-bind:key="item.Seq">
                                        <td>{{item.StartDateStr}}</td>
                                        <td>{{item.EndDateStr}}</td>
                                        <td>{{getDaySetText(item.DaySet)}}</td>
                                        <td>{{item.Descript}}</td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <a v-on:click.stop="onNoDurationDel(item)" href="javascript:void(0)" class="a-delete m-1" title="刪除"><i class="fas fa-trash-alt"></i></a>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- 小視窗 設定假日計工期 -->
        <div class="modal fade" id="date_ContainHoliday">
            <div class="modal-dialog modal-lg modal-dialog-centered ">
                <div class="modal-content">
                    <div class="modal-header bg-1 text-white">
                        <h6 class="modal-title font-weight-bold">設定假日計工期</h6>
                        <button v-on:click="onCloseModal" type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">×</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <h5 class="mt-0">工程編號：{{tenderItem.TenderNo}} &nbsp; 工程名稱：{{tenderItem.TenderName}}</h5>
                        <div class="table-responsive">
                            <table class="table table-hover table2">
                                <tbody>
                                    <tr>
                                        <th>起始日期</th>
                                        <td>
                                            <div class="form-inline">
                                                <input v-model="reportHoliday.StartDate" @change="onHolidayStartDateChange" type="date" class="form-control my-1">
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>結束日期</th>
                                        <td>
                                            <div class="form-inline">
                                                <input v-model="reportHoliday.EndDate" type="date" class="form-control my-1">
                                                <span class="text-B small">&nbsp;※ 非必填</span>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>變更設定</th>
                                        <td>
                                            <select v-model="reportHoliday.DaySet" class="form-control">
                                                <option value="1">休息日</option>
                                                <option value="2" selected="">工作日</option>
                                            </select>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>說明</th>
                                        <td><input v-model="reportHoliday.Descript" type="text" class="form-control" value=""></td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <div class="row justify-content-center">
                            <div class="col-12 col-sm-4 my-2">
                                <button v-on:click.stop="onHolidayAdd" role="button" class="btn btn-shadow btn-block btn-color11-3"> 新增 <i class="fas fa-plus-square fa-lg"></i></button>
                            </div>
                        </div>
                        <div class="table-responsive mt-3">
                            <table class="table table-responsive-sm table-hover">
                                <thead class="insearch">
                                    <tr>
                                        <th><strong>起始日期</strong></th>
                                        <th><strong>結束日期</strong></th>
                                        <th><strong>變更設定</strong></th>
                                        <th><strong>說明</strong></th>
                                        <th class="text-center"><strong>功能</strong></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(item, index) in holidayItems" v-bind:key="item.Seq">
                                        <td>{{item.StartDateStr}}</td>
                                        <td>{{item.EndDateStr}}</td>
                                        <td>{{getDaySetText(item.DaySet)}}</td>
                                        <td>{{item.Descript}}</td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <a v-on:click.stop="onHolidayDel(item)" href="javascript:void(0)" class="a-delete m-1" title="刪除"><i class="fas fa-trash-alt"></i></a>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['tenderItem','attrs'],
        data: function () {
            return {
                //設定假日計工期
                reportHoliday: {},
                holidayItems: [],

                //設定不計工期
                reportNoDuration: {},
                noDurationItems: [],

                //設定展延工期
                reportExtension: {},
                extensionItems: [],
                extendReasonOpions: [
                    { Value: 1, Text: "發生第17條第5款不可抗力或不可歸責契約當事人之事故" },
                    { Value: 2, Text: "因天候影響無法施工" },
                    { Value: 3, Text: "機關要求全部或部分停工" },
                    { Value: 4, Text: "機關應辦事項未及時辦妥" },
                    { Value: 5, Text: "由機關自辦或機關之其他廠商之延誤而影響履約進度者" },
                    { Value: 6, Text: "機關提供之地質資料，與實際情形有重大差異" },
                    { Value: 7, Text: "因傳染病或政府之行為" },
                    { Value: 8, Text: "因機關使用或佔用本工程任何部分，但契約另有規定者，不再此限" },
                    { Value: 9, Text: "其他非可歸責於廠商之情形，經機關認定者" },
                ],

                //停復工
                StopWorkApprovalFile: null,
                BackWorkApprovalFile: null,
                reportWork: {},
                workItems: [],

                //
                isDataChange: false,

                editSeq: -99,
                editRecord: {},
                //s20230408
                supDailyDateNote: { Weather1: '', Weather2: '', FillinDate:''}
            };
        },
        methods: {
            //日誌資訊
            setSupDailyDateNote(note) {
                this.supDailyDateNote.Weather1 = note.Weather1;
                this.supDailyDateNote.Weather2 = note.Weather2;
                this.supDailyDateNote.FillinDate = note.FillinDateStr;
            },
            //停復工
            onWorkItemClick(item) {
                this.initReportWork();
                this.reportWork = Object.assign({}, item);
            },
            //文件下載
            onDownload(item, mode) {
                var fName = '';
                if (mode == 1)
                    fName = item.StopWorkApprovalFile;
                else if (mode == 2)
                    fName = item.BackWorkApprovalFile;
                //s20230526
                window.comm.dnFile('/EPCProgressManage/DocDownload?id=' + this.tenderItem.Seq + "&mode=" + mode + "&fn=" + fName);
            },
            getWorkList() {
                this.workItems = [];
                window.myAjax.post('/EPCProgressManage/GetWorkList', { id: this.tenderItem.Seq })
                    .then(resp => {
                        this.workItems = resp.data;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onWorkSave() {
                if (window.comm.stringEmpty(this.reportWork.SStopWorkDateStr) || window.comm.stringEmpty(this.reportWork.EStopWorkDateStr)) {
                    alert('停工期間 必須填寫');
                    return;
                }
                if (window.comm.stringEmpty(this.reportWork.StopWorkReason) || window.comm.stringEmpty(this.reportWork.StopWorkNo)) {
                    alert('停工文號,停工原因 必須填寫');
                    return;
                }
                if (this.reportWork.Seq==-1 && this.StopWorkApprovalFile == null) {
                    alert('停工-核定公文檔案必須上傳');
                    return;
                }
                if (!window.comm.stringEmpty(this.reportWork.BackWorkDateStr) || !window.comm.stringEmpty(this.reportWork.BackWorkNo) || (this.BackWorkApprovalFile != null)) {
                    if (window.comm.stringEmpty(this.reportWork.BackWorkDateStr) || window.comm.stringEmpty(this.reportWork.BackWorkNo)) {
                        alert('復工日期,復工文號 必須填寫');
                        return;
                    }
                    if (this.reportWork.Seq == -1) {
                        if (this.BackWorkApprovalFile == null) {
                            alert('復工-核定公文檔案必須上傳');
                            return;
                        }
                    } else {
                        if (window.comm.stringEmpty(this.reportWork.BackWorkApprovalFile) && this.BackWorkApprovalFile == null) {
                            alert('復工-核定公文檔案必須上傳');
                            return;
                        }
                    }
                }
                var files = new FormData();
                files.append("Seq", this.reportWork.Seq);
                files.append("EngMainSeq", this.tenderItem.Seq);
                files.append("SStopWorkDate", this.reportWork.SStopWorkDateStr);
                files.append("EStopWorkDate", this.reportWork.EStopWorkDateStr);
                files.append("StopWorkReason", this.reportWork.StopWorkReason);
                files.append("StopWorkNo", this.reportWork.StopWorkNo);
                files.append("BackWorkDate", this.reportWork.BackWorkDateStr);
                files.append("BackWorkNo", this.reportWork.BackWorkNo);
                if (this.StopWorkApprovalFile != null)
                    files.append('StopWorkFile', this.StopWorkApprovalFile, this.StopWorkApprovalFile.name);
                if (this.BackWorkApprovalFile != null)
                    files.append('BackWorkFile', this.BackWorkApprovalFile, this.BackWorkApprovalFile.name);
                window.myAjax.post('/EPCProgressManage/WorkSave', files,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.isDataChange = true;
                            this.initReportWork();
                            this.getWorkList();
                            alert(resp.data.msg);
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            initReportWork() {
                this.StopWorkApprovalFile = null;
                this.BackWorkApprovalFile = null;
                this.reportWork = {
                    Seq: -1, EngMainSeq: -1,
                    SStopWorkDate: null, EStopWorkDate: null, StopWorkReason: '', StopWorkNo: '',
                    BackWorkDate: null, BackWorkNo: '', 
                    SStopWorkDateStr: '', EStopWorkDateStr: '', BackWorkDateStr: ''
                };
            },
            onWorkModalOpen() {
                this.getWorkList();
            },
            //設定展延工期
            getExtensionList() {
                this.extensionItems = [];
                window.myAjax.post('/EPCProgressManage/GetExtensionList', { id: this.tenderItem.Seq })
                    .then(resp => {
                        this.extensionItems = resp.data;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            getExtendReasonText(item) {
                let inx = item.ExtendReason
                if (inx == 1)
                    return "發生第17條第5款不可抗力或不可歸責契約當事人之事故";
                else if (inx == 2)
                    return "因天候影響無法施工";
                else if (inx == 3)
                    return "機關要求全部或部分停工";
                else if (inx == 4)
                    return "機關應辦事項未及時辦妥";
                else if (inx == 5)
                    return "由機關自辦或機關之其他廠商之延誤而影響履約進度者";
                else if (inx == 6)
                    return "機關提供之地質資料，與實際情形有重大差異";
                else if (inx == 7)
                    return "因傳染病或政府之行為";
                else if (inx == 8)
                    return "因機關使用或佔用本工程任何部分，但契約另有規定者，不再此限";
                else if (inx == 9)
                    return "其他非可歸責於廠商之情形，經機關認定者";
                else
                    return "";
            },
            onExtensionAdd() {
                if (window.comm.stringEmpty(this.reportExtension.ExtendDays)) {
                    alert('展延天數錯誤');
                    return;
                }
                if (window.comm.stringEmpty(this.reportExtension.ApprovalNo)) {
                    alert('公文核准文號錯誤');
                    return;
                }
                if (window.comm.stringEmpty(this.reportExtension.ApprovalDate)) {
                    alert('公文核准日期誤');
                    return;
                }
                if (window.comm.stringEmpty(this.reportExtension.ExtendReason)) {
                    alert('展原因錯誤');
                    return;
                }
                this.reportExtension.EngMainSeq = this.tenderItem.Seq;
                window.myAjax.post('/EPCProgressManage/ExtensionAdd', { item: this.reportExtension })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.isDataChange = true;
                            this.initReportExtension();
                            this.getExtensionList();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //編輯延展工期紀錄 s20230310
            onExtensionEdit(item) {
                if (this.editSeq > -99) return;
                this.editRecord = Object.assign({}, item);
                this.editRecord.ApprovalDate = this.editRecord.ApprovalDateStr;
                this.editSeq = this.editRecord.Seq;
            },
            onExtensionEditCancel() {
                this.editSeq = -99;
                this.getExtensionList();
            },
            onExtensionUpdate(item) {
                if (window.comm.stringEmpty(item.ExtendDays)) {
                    alert('展延天數錯誤');
                    return;
                }
                if (window.comm.stringEmpty(item.ApprovalNo)) {
                    alert('公文核准文號錯誤');
                    return;
                }
                if (window.comm.stringEmpty(item.ApprovalDate)) {
                    alert('公文核准日期誤');
                    return;
                }
                if (window.comm.stringEmpty(item.ExtendReason)) {
                    alert('展原因錯誤');
                    return;
                }
                window.myAjax.post('/EPCProgressManage/ExtensionUpdate', { item: item })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.isDataChange = true;
                            this.initReportExtension();
                            this.onExtensionEditCancel();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onExtensionDel(item) {
                window.myAjax.post('/EPCProgressManage/ExtensionDel', { id: item.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.isDataChange = true;
                            this.getExtensionList();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            initReportExtension() {
                this.reportExtension = { Seq: -1, EngMainSeq: -1, ExtendDays: null, ApprovalNo: '', ApprovalDate: null, ExtendReason: null, ExtendReasonOther: ''};
            },
            onExtensionModalOpen() {
                this.getExtensionList();
            },
            //設定不計工期
            getNoDurationList() {
                this.noDurationItems = [];
                window.myAjax.post('/EPCProgressManage/GetNoDurationList', { id: this.tenderItem.Seq })
                    .then(resp => {
                        this.noDurationItems = resp.data;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onNoDurationStartDateChange() {
                if (this.reportNoDuration.EndDate == null || this.reportNoDuration.EndDate == '') {
                    this.reportNoDuration.EndDate = this.reportNoDuration.StartDate;
                }
            },
            onNoDurationAdd() {
                if (this.reportNoDuration.StartDate == null || this.reportNoDuration.StartDate == '') {
                    alert('起始日期錯誤');
                    return;
                }
                if (this.reportNoDuration.EndDate == null || this.reportNoDuration.EndDate == '') {
                    this.reportNoDuration.EndDate = this.reportNoDuration.StartDate;
                }
                this.reportNoDuration.EngMainSeq = this.tenderItem.Seq;
                window.myAjax.post('/EPCProgressManage/NoDurationAdd', { item: this.reportNoDuration })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.isDataChange = true;
                            this.initReportNoDuration();
                            this.getNoDurationList();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onNoDurationDel(item) {
                window.myAjax.post('/EPCProgressManage/NoDurationDel', { id: item.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.isDataChange = true;
                            this.getNoDurationList();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            initReportNoDuration() {
                this.reportNoDuration = { Seq: -1, EngMainSeq: -1, StartDate: null, EndDate: null, DaySet: 1, Descript: '' };
            },
            onNoDurationModalOpen() {
                this.getNoDurationList();
            },
            //設定假日計工期
            getHolidayList() {
                this.holidayItems = [];
                window.myAjax.post('/EPCProgressManage/GetHolidayList', { id: this.tenderItem.Seq })
                    .then(resp => {
                        this.holidayItems = resp.data;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onHolidayStartDateChange() {
                if (this.reportHoliday.EndDate == null || this.reportHoliday.EndDate == '') {
                    this.reportHoliday.EndDate = this.reportHoliday.StartDate;
                }
            },
            onHolidayAdd() {
                if (this.reportHoliday.StartDate == null || this.reportHoliday.StartDate == '') {
                    alert('起始日期錯誤');
                    return;
                }
                if (this.reportHoliday.EndDate == null || this.reportHoliday.EndDate == '') {
                    this.reportHoliday.EndDate = this.reportHoliday.StartDate;
                }
                this.reportHoliday.EngMainSeq = this.tenderItem.Seq;
                window.myAjax.post('/EPCProgressManage/HolidayAdd', { item: this.reportHoliday })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.isDataChange = true;
                            this.initReportHoliday();
                            this.getHolidayList();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onHolidayDel(item) {
                window.myAjax.post('/EPCProgressManage/HolidayDel', { id: item.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.isDataChange = true;
                            this.getHolidayList();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            initReportHoliday() {
                this.reportHoliday = { Seq: -1, EngMainSeq: -1, StartDate: null, EndDate: null, DaySet: 2, Descript: '' };
            },
            onHolidayModalOpen() {
                this.getHolidayList();
            },
            getDaySetText(inx) {
                if (inx == 1) {
                    return "休息日"
                } else if (inx == 2) {
                    return "工作日"
                } else
                    return "";
            },
            //
            formatDateStr(d) {
                return window.comm.formatCDateStr(d);
            },
            //月曆事件
            dayClickHandler: function (day) {
                this.$emit('onDayClickEvent', day)
            },
            fromPageHandler: function (fromDay) {
                this.$emit('onFromPageEvent', fromDay)
            },
            onCloseModal() {
                if (this.isDataChange) {
                    this.$emit('onDataChangeEvent');
                    this.isDataChange = false;
                }
            },
        },
        mounted() {
            console.log('mounted() 標案月曆');
            this.initReportHoliday();
            this.initReportNoDuration();
            this.initReportExtension();
            this.initReportWork();
        }
    }
</script>
<style>
    .custom-file-input:lang(en) ~ .custom-file-label::after,
    .custom-file-input:lang(zh) ~ .custom-file-label::after {
        content: '上傳';
    }
    /*已填寫*/
    .cal_filled-in {
        background-color: darkseagreen;
    }
    .cal_filled-in-color {
        color: darkseagreen;
    }
    /*逾期未填*/
    .cal_unfilled {
        border: 2px solid;
        border-color: red;
        border-radius: 2px;
    }
    /*停工*/
    .cal_stopwork {
        background-color: darksalmon;
    }
    .cal_stopwork-color {
        color: darksalmon;
    }
    /*不計工期*/
    .cal_no-duration {
        background-color: deepskyblue;
    }
    .cal_no-duration-color {
        color: deepskyblue;
    }
    .vc-header {
        background-color: #777;
        padding: 7px !important;
    }
    .vc-title {
        color: white !important;
    }
    .vc-svg-icon {
        color: #fff;
    }
    .vc-grid-container {
        background-color: #eee !important;
    }
</style>