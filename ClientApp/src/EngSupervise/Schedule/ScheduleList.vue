<template>
    <div>
        <form class="form-group insearch" style="padding-bottom:0px;">
            <div class="row">
                <div class="col-3 d-flex">
                    <input v-model.trim="keyWord" type="text" placeholder="請輸入督導期別" class="form-control">
                    <button v-on:click.stop="onSearchPhase" type="button" class="btn btn-outline-secondary btn-xs mx-1">
                        <i class="fas fa-search"></i> 查詢
                    </button>
                </div>
                <div class="col-7"></div>
                <div class="col-2 d-flex">

                </div>
            </div>
            <div style="padding-top: 10px">
                <ul class="nav nav-tabs" role="tablist">
                    <li v-for="(item, index) in yearPhaseItems" v-bind:key="item.Seq" class="nav-item">
                        <a v-on:click="onTabPhaseSelect(item)" data-toggle="tab" href="##" class="nav-link">{{item.PhaseCode}}</a>
                    </li>
                </ul>
            </div>
        </form>
        <div class="row justify-content-between">
            <comm-pagination class="col-8" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
            <div v-if="recordTotal > 0" class="col-2 d-flex justify-content-center small">
                <button v-on:click.stop="onDownloadSchedule" type="button" class="btn btn-color11-1 btn-xs mx-1">
                    <i class="fas fa-download"></i>行程表
                </button>
            </div>
        </div>
        <div class="table-responsive">
            <table class="table table-responsive-md table-hover">
                <thead>
                    <tr>
                        <th style="width: 42px;"><strong>項次</strong></th>
                        <th><strong>工程編號</strong></th>
                        <th><strong>工程名稱</strong></th>
                        <th style="width: 120px;"><strong>執行機關</strong></th>
                        <th style="width: 120px;"><strong>設計單位</strong></th>
                        <th style="width: 120px;"><strong>監造單位</strong></th>
                        <th style="width: 95px;"><strong>狀態</strong></th>
                        <th style="width: 42px;"><strong>功能</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in targetPhaseItems" v-bind:key="item.Seq">
                        <td><strong>{{pageRecordCount*(pageIndex-1)+index+1}}</strong></td>
                        <td>{{item.EngNo}}</td>
                        <td>{{item.EngName}}</td>
                        <td>{{item.ExecUnit}}</td>
                        <td>{{item.DesignUnitName}}</td>
                        <td>{{item.SupervisionUnitName}}</td>
                        <td><i v-bind:class="getStateCss(item.ExecState)"></i>{{item.ExecState}}</td>
                        <td>
                            <div class="d-flex">
                                <button @click="onEditEng(item)" class="btn btn-color11-1 btn-xs sharp mr-1"
                                        data-toggle="modal" data-target="#schedule_01" title="編輯">
                                    <i class="fas fa-pencil-alt"></i>
                                </button>
                                <button @click="onDownloadDoc(item)" class="btn btn-color11-3 btn-xs sharp mr-1"
                                        data-toggle="modal" data-target="#download_01" title="下載">
                                    <i class="fas fa-download"></i>
                                </button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <!-- 小視窗 編輯行程 -->
        <div class="modal fade" id="schedule_01" tabindex="-1" role="dialog" data-backdrop="static" data-keyboard="false">
            <div v-if="targetPhase != null" class="modal-dialog modal-xl modal-dialog-centered ">
                <div class="modal-content">
                    <div class="modal-header bg-3 text-white">
                        <h6 class="modal-title font-weight-bold">行程安排</h6>
                        <button id="btnCloseEditModal" type="button" class="close" data-dismiss="modal"
                                aria-label="Close" @click="displayModal(null,null)">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <h5 class="mt-0">
                            工程編號：{{tarItem.EngNo}} &nbsp; 工程名稱：{{tarItem.EngName}} &nbsp;
                            督導期別：{{targetPhase.PhaseCode}}
                        </h5>
                        <div class="table-responsive">
                            <table class="table table-hover table2">
                                <tr>
                                    <th class="bg-3-30" colspan="2">督導委員</th>
                                </tr>
                                <tr>
                                    <th>領隊</th>
                                    <td>
                                        <div class="form-inline">
                                            <label class="my-1 mx-2">{{tarItem.LeaderName}}</label>
                                        </div>
                                    </td>
                                <tr>
                                    <th>外聘</th>
                                    <td>
                                        <div class="form-inline">
                                            <label class="my-1 mx-2">{{tarItem.OutCommittee}}</label>
                                        </div>
                                        <!-- <span class="small text-3">* 多名人員用逗號隔開</span> -->
                                    </td>
                                </tr>
                                <tr>
                                    <th>內聘</th>
                                    <td>
                                        <div class="form-inline">
                                            <label class="my-1 mx-2">{{tarItem.InsideCommittee}}</label>
                                        </div>
                                        <!-- <span class="small text-3">* 多名人員用逗號隔開</span> -->
                                    </td>
                                </tr>
                                <tr>
                                    <th>委員兼幹事</th>
                                    <td>
                                        <div class="form-inline">
                                            <label class="my-1 mx-2">{{tarItem.OfficerName}}</label>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <th class="bg-3-30" colspan="2">行程資訊</th>
                                </tr>
                                <tr>
                                    <th>簡報地點</th>
                                    <td>
                                        <input v-model.trim="tarItem.BriefingPlace" maxlength="50" type="text"
                                               class="form-control">
                                    </td>
                                </tr>
                                <tr>
                                    <th>簡報地址</th>
                                    <td>
                                        <input v-model.trim="tarItem.BriefingAddr" maxlength="100" type="text"
                                               class="form-control">
                                    </td>
                                </tr>
                                <tr>
                                    <th>搭車資訊-署內派車</th>
                                    <td>
                                        <div class="custom-control custom-checkbox">
                                            <input v-model="tarItem.IsVehicleDisp" type="checkbox" name="car-1"
                                                   id="car-1" class="custom-control-input">
                                            <label for="car-1" class="custom-control-label">是否派車</label>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <th>搭車資訊-建議高鐵車次</th>
                                    <td>
                                        <div class="d-flex">
                                            <!-- div class="custom-control custom-radio">
                                                <input v-model="filter.direction" :value="1" type="radio" name="order-1"
                                                       id="order-1" class="custom-control-input">
                                                <label for="order-1" class="custom-control-label">北上</label>
                                            </div>
                                            <div class="custom-control custom-radio">
                                                <input v-model="filter.direction" :value="2" type="radio" name="order-2"
                                                       id="order-2" class="custom-control-input" checked="checked">
                                                <label for="order-2" class="custom-control-label">南下</label>
                                            </div -->
                                            <div>
                                                <button data-toggle="modal" @click="displayModal('#schedule_01', '#THSR_01')" data-target="#THSR_01" type="button"
                                                        class="btn btn-outline-secondary btn-xs mx-1">
                                                    <i class="fas fa-plus"></i> 高鐵資訊
                                                </button>
                                            </div>
                                        </div>
                                        <table class="table table-hover VA-middle">
                                            <thead class="insearch">
                                                <tr>
                                                    <th><strong>序</strong></th>
                                                    <th class="text-left"><strong>車次</strong></th>
                                                    <th class="text-left"><strong>備註</strong></th>
                                                    <th></th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr v-for="(item, index) in engTHSR" v-bind:key="item.Seq">
                                                    <td>{{index+1}}</td>
                                                    <td class="text-left">{{item.CarNo}}</td>
                                                    <td class="text-left">{{item.Memo}}</td>
                                                    <td>
                                                        <div class="d-flex justify-content-center">
                                                            <button @click="delEngTHSR(item)"
                                                                    class="btn btn-color9-1 btn-xs mx-1">
                                                                <i class="fas fa-trash-alt"></i>
                                                            </button>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        署承辦人
                                        <!-- span class="small text-R d-block">自動帶入幹事姓名</span -->
                                    </th>
                                    <td>
                                        <div class="form-row">
                                            <div class="form-group col-md-4 mb-md-0">
                                                <label for="A_name">姓名</label>
                                                <input v-model.trim="tarItem.AdminContact" maxlength="50" type="text"
                                                       class="form-control" id="A_name">
                                            </div>
                                            <div class="form-group col-md-4 mb-md-0">
                                                <label for="A_phone">電話</label>
                                                <input v-model.trim="tarItem.AdminTel" maxlength="50" type="text"
                                                       class="form-control" id="A_phone">
                                            </div>
                                            <div class="form-group col-md-4 mb-md-0">
                                                <label for="A_cellphone">手機</label>
                                                <input v-model.trim="tarItem.AdminMobile" maxlength="20" type="text"
                                                       class="form-control" id="A_cellphone">
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        分署聯絡人
                                        <!-- span class="small text-R d-block">由工程會標案管理系統自動帶入</span -->
                                    </th>
                                    <td>
                                        <div class="form-row">
                                            <div class="form-group col-md-4 mb-md-0">
                                                <label for="B_name">姓名</label>
                                                <input v-model.trim="tarItem.RiverBureauContact" maxlength="50"
                                                       type="text" class="form-control" id="B_name">
                                            </div>
                                            <div class="form-group col-md-4 mb-md-0">
                                                <label for="B_phone">電話</label>
                                                <input v-model.trim="tarItem.RiverBureauTel" maxlength="50" type="text"
                                                       class="form-control" id="B_phone">
                                            </div>
                                            <div class="form-group col-md-4 mb-md-0">
                                                <label for="B_cellphone">手機</label>
                                                <input v-model.trim="tarItem.RiverBureauMobile" maxlength="20"
                                                       type="text" class="form-control" id="B_cellphone">
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        地方政府聯絡人
                                        <!-- span class="small text-R d-block">由工程會標案管理系統自動帶入</span -->
                                    </th>
                                    <td>
                                        <div class="form-row">
                                            <div class="form-group col-md-4 mb-md-0">
                                                <label for="C_name">姓名</label>
                                                <input v-model.trim="tarItem.LocalGovContact" maxlength="50" type="text"
                                                       class="form-control" id="C_name">
                                            </div>
                                            <div class="form-group col-md-4 mb-md-0">
                                                <label for="C_phone">電話</label>
                                                <input v-model.trim="tarItem.LocalGovTel" maxlength="50" type="text"
                                                       class="form-control" id="C_phone">
                                            </div>
                                            <div class="form-group col-md-4 mb-md-0">
                                                <label for="C_cellphone">手機</label>
                                                <input v-model.trim="tarItem.LocalGovMobile" maxlength="20" type="text"
                                                       class="form-control" id="C_cellphone">
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <th>工區至簡報地點車程(分鐘)</th>
                                    <td>
                                        <input v-model="tarItem.ToBriefingDrive" type="text" class="form-control w-auto"
                                               value="30">
                                    </td>
                                </tr>
                                <tr>
                                    <th>督導開始時間/順序</th>
                                    <td>
                                        <div class="row pl-3">
                                            <input v-model="tarItem.SuperviseStartTimeStr" @change="adjScheduleTime" type="time"
                                                   class="form-control w-auto" value="10:00">&nbsp;
                                            <div class="custom-control custom-radio custom-control-inline pt-1">
                                                <input v-model="tarItem.SuperviseOrder" value="0" type="radio"
                                                       name="order-3" id="order-3" class="custom-control-input">
                                                <label for="order-3" class="custom-control-label">會議室簡報優先</label>
                                            </div>
                                            <div class="custom-control custom-radio custom-control-inline pt-1">
                                                <input v-model="tarItem.SuperviseOrder" value="1" type="radio"
                                                       name="order-4" id="order-4" class="custom-control-input">
                                                <label for="order-4" class="custom-control-label">現場督導優先</label>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                                <!-- tr>
                                    <th>督導順序</th>
                                    <td>
                                        <div class="custom-control custom-radio custom-control-inline">
                                            <input v-model="tarItem.SuperviseOrder" value="0" type="radio"
                                                   name="order-3" id="order-3" class="custom-control-input">
                                            <label for="order-3" class="custom-control-label">會議室簡報優先</label>
                                        </div>
                                        <div class="custom-control custom-radio custom-control-inline">
                                            <input v-model="tarItem.SuperviseOrder" value="1" type="radio"
                                                   name="order-4" id="order-4" class="custom-control-input">
                                            <label for="order-4" class="custom-control-label">現場督導優先</label>
                                        </div>
                                    </td>
                                </tr -->
                                <tr v-if="showScheduleForm">
                                    <th>日程表</th>
                                    <td>
                                        <div class="row">
                                            <div class="col-3 list-group-item">時間</div>
                                            <div class="col-2 list-group-item">分配時間(分鐘)</div>
                                            <div class="col-7 list-group-item">摘要</div>
                                        </div>
                                        <draggable :list="scheduleForm">
                                            <div class="" v-for="item in scheduleForm" :key="item.Seq">
                                                <div class="row">
                                                    <div class="col-3 list-group-item">{{ item.StartTime }} ~ {{ item.EndTime }}</div>
                                                    <div class="col-2 list-group-item"><input v-model.number="item.ActivityTime" @change="adjScheduleTime" type="number" class="form-control" /></div>
                                                    <div class="col-7 list-group-item"><textarea  v-model.trim="item.Summary" maxlength="200" :rows="(item.Summary.match(/\n/g) || []).length +1" class="col-12 form-control"></textarea></div>
                                                </div>
                                            </div>
                                        </draggable>
                                        <div class="row">
                                            <div class="col-3 list-group-item">{{scheduleForm.length==0 ? '' : scheduleForm[scheduleForm.length-1].EndTime}}</div>
                                            <div class="col-2 list-group-item"></div>
                                            <div class="col-7 list-group-item">結束會議</div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-color3" data-dismiss="modal" @click="displayModal(null,null)">關閉</button>
                        <button @click="onSaveEng" type="button" class="btn btn-color11-1">
                            儲存 <i class="fas fa-save"></i>
                        </button>
                    </div>
                </div>
            </div>
        </div>
        <!-- 小視窗 高鐵清單 -->
        <div class="modal fade" id="THSR_01" tabindex="-1" role="dialog" data-backdrop="static" data-keyboard="false">
            <div class="modal-dialog modal-lg modal-dialog-centered ">
                <div class="modal-content">
                    <div class="modal-header bg-3 text-white">
                        <h6 class="modal-title font-weight-bold">行程安排</h6>
                        <button type="button" class="close" data-dismiss="modal"
                                aria-label="Close" @click="displayModal('#THSR_01', '#schedule_01')">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="d-flex">
                            <div class="form-row form-inline">
                                <div class="custom-control custom-radio">
                                    <input v-model="filter.direction" :value="1" type="radio" name="order-1"
                                           id="order-1" class="custom-control-input"
                                           @change="DirectionChange">
                                    <label for="order-1" class="custom-control-label" >北上</label>
                                </div>
                                <div class="custom-control custom-radio">
                                    <input v-model="filter.direction" :value="2" type="radio" name="order-2"
                                           id="order-2" class="custom-control-input" checked="checked"
                                           @change="DirectionChange">
                                    <label for="order-2" class="custom-control-label">南下</label>
                                </div>
                                <div class="form-group p-1">
                                    <label for="A" class="mr-1">車次</label><input maxlength="50" type="text" id="A" class="form-control" v-model="filter.carNo">
                                </div>
                                <div class="form-group p-1">
                                    <label for="B" class="mr-1">起站</label>
                                    <select v-model="filter.start" id="B" class="form-control" @change="GetThsrByFilterVM(true)">
                                        <option value="" selected="selected">全部</option>
                                        <option v-for=" item in startStationOptions" :key="item" :value="item">{{item}}</option>
                                    </select>
                                </div>
                                <div class="form-group p-1">
                                    <label for="C" class="mr-1">迄站</label>
                                    <select v-model="filter.end" id="C" class="form-control" @change="GetThsrByFilterVM(true)" >
                                        <option value="" selected="selected">全部</option>
                                        <option v-for=" item in endingStationOptions" :key="item" :value="item">{{item}}</option>
                                    </select>
                                </div>

                            </div>
                            <div>
                                <button type="button" class="btn btn-outline-success mt-1" @click="searchCarNo()">搜尋</button>
                            </div>
                        </div>
                        <div class="table-responsive">
                            <table class="table table-hover VA-middle">
                                <thead class="insearch">
                                    <tr>
                                        <th class="text-left"><strong>車次</strong></th>
                                        <th><strong>備註</strong></th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(item, index) in listTHSR" v-bind:key="index">
                                        <td class="text-left">{{item.CarNo}}</td>
                                        <td class="text-left">{{item.Memo}}</td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button @click="addEngTHSR(item)"
                                                        class="btn btn-color11-3 btn-xs sharp mx-1" title="加入">
                                                    <i class="fas fa-plus"></i>
                                                </button>
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
        <!-- 小視窗 表單下載 -->
        <div class="modal fade" id="download_01">
            <div class="modal-dialog modal-lg modal-dialog-centered ">
                <div class="modal-content">
                    <div class="modal-header bg-3 text-white">
                        <h6 class="modal-title font-weight-bold">行程安排</h6>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <h5 class="mt-0">工程編號：{{tarItem.EngNo}} &nbsp; 工程名稱：{{tarItem.EngName}}</h5>
                        <div class="table-responsive">
                            <table class="table table-hover VA-middle">
                                <thead class="insearch">
                                    <tr>
                                        <th class="text-left"><strong>表單名稱</strong></th>
                                        <th><strong>格式</strong></th>
                                        <th class="text-center"><strong>下載</strong></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td class="text-left">督導行程表</td>
                                        <td>
                                            <select v-model="fileType" class="form-control my-1 mr-sm-2">
                                                <option value="4">docx</option>
                                                <option value="2">pdf</option>
                                                <option value="3">odt</option>
                                            </select>
                                        </td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button @click="onDownloadClick(2)"
                                                        class="btn btn-color11-3 btn-xs sharp mx-1" title="下載">
                                                    <i class="fas fa-download"></i>
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="text-left">委員出席費請領單</td>
                                        <td>
                                            <select v-model="fileType" class="form-control my-1 mr-sm-2">
                                                <option value="4">docx</option>
                                                <option value="2">pdf</option>
                                                <option value="3">odt</option>
                                            </select>
                                        </td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button @click="onDownloadClick(3)"
                                                        class="btn btn-color11-3 btn-xs sharp mx-1" title="下載">
                                                    <i class="fas fa-download"></i>
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="text-left">簽到單</td>
                                        <td>
                                            <select v-model="fileType" class="form-control my-1 mr-sm-2">
                                                <option value="4">docx</option>
                                                <option value="2">pdf</option>
                                                <option value="3">odt</option>
                                            </select>
                                        </td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button @click="onDownloadClick(4)"
                                                        class="btn btn-color11-3 btn-xs sharp mx-1" title="下載">
                                                    <i class="fas fa-download"></i>
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="text-left">督導意見表</td>
                                        <td>
                                            <select v-model="fileType" class="form-control my-1 mr-sm-2">
                                                <option value="4">docx</option>
                                                <option value="2">pdf</option>
                                                <option value="3">odt</option>
                                            </select>
                                        </td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button @click="onDownloadClick(5)"
                                                        class="btn btn-color11-3 btn-xs sharp mx-1" title="下載">
                                                    <i class="fas fa-download"></i>
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="text-left">督導人員紀錄表</td>
                                        <td>
                                            <select v-model="fileType" class="form-control my-1 mr-sm-2">
                                                <option value="4">docx</option>
                                                <option value="2">pdf</option>
                                                <option value="3">odt</option>
                                            </select>
                                        </td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button @click="onDownloadClick(6)"
                                                        class="btn btn-color11-3 btn-xs sharp mx-1" title="下載">
                                                    <i class="fas fa-download"></i>
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="text-left">全部下載</td>
                                        <td>
                                            <select v-model="fileType" class="form-control my-1 mr-sm-2">
                                                <option value="4">docx</option>
                                                <option value="2">pdf</option>
                                                <option value="3">odt</option>
                                            </select>
                                        </td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button @click="onDownloadClick(99)"
                                                        class="btn btn-color11-3 btn-xs sharp mx-1" title="下載">
                                                    <i class="fas fa-download"></i>
                                                </button>
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
    import draggable from "vuedraggable";
    export default {
        components: {
            draggable
        },
        watch: {
            scheduleForm: function (val) {
                this.adjScheduleTime();
            }
        },
        data: function () {
            return {
                scheduleForm: [],//日程表 s20230519

                //分頁
                recordTotal: 0,
                pageRecordCount: 30,
                //pageTotal: 0,
                pageIndex: 0,
                //pageIndexOptions: [],

                yearPhaseItems: [],//年度期別
                keyWord: '',
                targetPhase: null,
                targetPhaseItems: [],

                tarItem: {},//工程
                engTHSR: [],

                listTHSR: [],//高鐵

                fileType: "4",
                startStationOptions: [],
                 endingStationOptions: [],
                filter: {
                    direction: 2,
                    start: "",
                    end: "",
                    carNo: ""
                },
            };
        },
        computed: {
            // startStationOptions() {
            //     return new Set( this.listTHSR.reduce((a, c) =>{
            //         let key = c.CarNo.split(" ")[2].split("-")[0].split("(")[0] ;
            //         a.push(key)
            //         return a;
            //     }, []) );
            // },
            // endingStationOptions() {
            //     return new Set( this.listTHSR.reduce((a, c) =>{
            //         let key = c.CarNo.split(" ")[2].split("-")[1].split("(")[0] ;
            //         a.push(key)
            //         return a;
            //     }, []) );
            // },
            showScheduleForm() {
                if (window.comm.stringEmpty(this.tarItem.SuperviseStartTimeStr)) return false;
                if (this.scheduleForm.length == 0) return false;
                if (this.tarItem.SuperviseOrder == null) return false;
                return true;
            }
        },
        methods: {
            //日期表 s20230519
            DirectionChange()
            {
                this.filter.start = "";
                this.filter.end = "";
                this.endingStationOptions = [];
                this.getStartStation();
                // this.GetThsrByFilterVM();
            },
            getScheduleForm() {
                this.scheduleForm = [];
                window.myAjax.post('/ESSchedule/GetScheduleForm', { id: this.tarItem.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.scheduleForm = resp.data.items;
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //時間設定
            adjScheduleTime() {
                if (window.comm.stringEmpty(this.tarItem.SuperviseStartTimeStr)) return;
                var startDate = new Date('2023-1-1 '+this.tarItem.SuperviseStartTimeStr + ':00');
                console.log(startDate);
                if (this.scheduleForm.length == 0) return;
                for (var i = 0; i < this.scheduleForm.length; i++) {
                    var item = this.scheduleForm[i];
                    item.StartTime = this.timeFormat(startDate);
                    startDate = this.addMinutes(startDate, item.ActivityTime);
                    item.EndTime = this.timeFormat(startDate);
                }
            },
            addMinutes(date, minutes) {
                return new Date(date.getTime() + minutes * 60000);
            },
            timeFormat(d) {
                var hr = d.getHours();
                if (hr < 10) {
                    hr = "0" + hr;
                }
                var min = d.getMinutes();
                if (min < 10) {
                    min = "0" + min;
                }
                return hr + ":" + min;
            },
            //
            onTabPhaseSelect(item) {
                this.keyWord = item.PhaseCode;
                this.onSearchPhase();
            },
            //當年度期別
            getYearPhases() {
                this.yearPhaseItems = [];
                window.myAjax.post('/ESQCPlaneWeakness/GetPhaseOptions')
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.yearPhaseItems = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            async getStartStation() {
    
                let res = await window.myAjax.post("ESSchedule/GetThsrByFilterVM", { filter: this.filter, getOption : true});
                if (res.data.status == "success") {
                    this.startStationOptions = res.data.startOptionList;
                    // this.endingStationOptions = res.data.endOptions;
                }
            },
            async GetThsrByFilterVM(getOption) {
                let res = await window.myAjax.post("ESSchedule/GetThsrByFilterVM", { filter: this.filter, getOption : getOption });
                if (res.data.status == "success") {
                    this.listTHSR = res.data.list;
                    this.startStationOptions = res.data.startOptionList;
                    this.endingStationOptions = res.data.endOptionList;
                //     this.endingStationOptions = new Set( this.listTHSR.reduce((a, c) =>{
                //     let key = c.CarNo.split(" ")[2].split("-")[1].split("(")[0] ;
                //     a.push(key)
                //     return a;
                // }, []) );
                }

            },
            /*displayModal(closeModal, showModal) {
                $(closeModal).modal('hide');
                $(showModal).modal('show');
                $(closeModal).css({"overflow":"scroll"});
                $(showModal).css({"overflow":"scroll"});
                $("body").css({"overflow":"hidden"});
                if(!showModal & !closeModal) {
                    $("body").css({"overflow":"scroll"});
                }
                // this.GetThsrByFilterVM();
            },*/
            //shioulo20230210
            displayModal(closeModal, showModal) {
                if (closeModal != null) closeModal = closeModal.replace('#', '');
                if (showModal != null) showModal = showModal.replace('#', '');
                if (closeModal != null) {
                    document.getElementById(closeModal).classList.remove("show");
                    document.getElementById(closeModal).style.display = "none";
                    document.getElementById(closeModal).style.overflow = "scroll";
                }
                if (showModal != null) {
                    document.getElementById(showModal).classList.add("show");
                    document.getElementById(showModal).style.display = "block";
                    document.getElementById(showModal).style.overflow = "scroll";
                }
                /*document.getElementsByTagName("body").style.overflow = "hidden";
                if (!showModal & !closeModal) {
                    document.getElementsByTagName("body").style.overflow = "scroll";
                }*/
                // this.GetThsrByFilterVM();
            },
            searchCarNo() {
                this.GetThsrByFilterVM();
            },
            //高鐵
            addEngTHSR(item) {
                item.SuperviseEngSeq = this.tarItem.Seq;
                window.myAjax.post('/ESSchedule/AddEngTHSR', { m: item })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getEngTHSR();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            delEngTHSR(item) {
                window.myAjax.post('/ESSchedule/DelEngTHSR', { id: item.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getEngTHSR();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //工程高鐵
            getEngTHSR() {
                this.engTHSR = [];
                window.myAjax.post('/ESSchedule/GetEngTHSR', { id: this.tarItem.Seq })
                    .then(resp => {
                        this.engTHSR = resp.data.items;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //編輯工程
            onEditEng(item) {
                this.tarItem = {};
                window.myAjax.post('/ESSchedule/GetEng', { id: item.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.tarItem = resp.data.item;
                            this.getScheduleForm();
                            this.getEngTHSR();
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //儲存工程
            onSaveEng() {
                if (window.comm.stringEmpty(this.tarItem.SuperviseStartTimeStr)) {
                    alert("督導開始時間, 必須設定");
                    return;
                }
                if (this.tarItem.SuperviseOrder == null) {
                    alert("督導順序, 必須設定");
                    return;
                }
                this.tarItem.SuperviseStartTime = this.tarItem.SuperviseStartTimeStr + ':00';
                window.myAjax.post('/ESSchedule/SaveEng', { m: this.tarItem, sf: this.scheduleForm })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            document.getElementById('btnCloseEditModal').click();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //期別查詢
            onSearchPhase() {
                if (this.keyWord.length == 0) {
                    alert("請輸入監督期別");
                    return;
                }
                this.targetPhase = null;
                this.targetPhaseItems = [];
                window.myAjax.post('/ESSchedule/SearchPhase', { keyWord: this.keyWord })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.targetPhase = resp.data.item;
                            this.yearPhaseItems = resp.data.phaseOption;
                            this.getPhaseEngItems('');
                            window.sessionStorage.setItem(window.window.esKeyword, this.keyWord);
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //取得期別工程清單
            getPhaseEngItems() {
                if (this.targetPhase == null) return;
                if (this.pageIndex < 1) this.pageIndex = 1;
                this.targetPhaseItems = [];
                window.myAjax.post('/ESSchedule/GetPhaseEngList'
                    , {
                        id: this.targetPhase.Seq,
                        pageRecordCount: this.pageRecordCount,
                        pageIndex: this.pageIndex
                    })
                    .then(resp => {
                        this.targetPhaseItems = resp.data.items;
                        this.recordTotal = resp.data.pTotal;
                        //this.setPagination();
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //工程狀態
            getStateCss(state) {
                return window.comm.getEngStateCss(state);
            },
            //下載檔案
            onDownloadDoc(item) {
                this.tarItem = item;
            },
            onDownloadClick(docNo) {
                let action = '/ESSchedule/Download?id=' + this.tarItem.Seq + '&docNo=' + docNo + '&docType=' + this.fileType;
                this.download(action);
            },
            onDownloadSchedule() {
                let action = '/ESSchedule/DnSchedule?id=' + this.targetPhase.Seq;
                this.download(action);
            },
            download(action) {
                window.comm.dnFile(action);
            },
            //分頁
            onPaginationChange(pInx, pCount) {
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
                this.getList();
            },
            getList() {
                this.getPhaseEngItems();
            }
        },
        mounted() {
            console.log('mounted() 行程安排');
            this.getYearPhases();
            this.keyWord = window.sessionStorage.getItem(window.window.esKeyword);
            // this.getTHSR();
            this.getStartStation();
        }
    }
</script>