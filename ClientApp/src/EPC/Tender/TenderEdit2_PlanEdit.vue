 <template>
     <div>
             <div style="width:800px">
                 <h2>基本資料</h2>
                 <div>
                     <div class="form-row">
                         <div class="col-12 col-md-8 form-inline my-2 justify-content-md-between">
                             <label class="my-2 mx-2">工程名稱<span class="small-red">&nbsp;*</span></label>
                             <input disabled v-model.trim="engMain.EngName" type="text" placeholder="PCCES帶入" class="form-control col-md-8 my-1 mr-0 mr-sm-1">
                         </div>
                     </div>
                     <!-- div class="form-row">
       <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
           <label class="my-2 mx-2">工程地點<span class="small-red">&nbsp;*</span></label>
           <div>
               <select disabled v-model="engMain.CitySeq" @change="onCityChange($event)" class="form-control my-1 mr-0 mr-sm-1">
                   <option v-for="option in cities" v-bind:value="option.Value" v-bind:key="option.Value">
                       {{ option.Text }}
                   </option>
               </select>
               <select disabled v-model="engMain.EngTownSeq" class="form-control my-1 mr-0 mr-md-1 mr-0 mr-md-4">
                   <option v-for="option in towns" v-bind:value="option.Value" v-bind:key="option.Value">
                       {{ option.Text }}
                   </option>
               </select>
           </div>
       </div>
    </div -->
                     <div class="form-row">
                         <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                             <label class="my-2 mx-2">工程編號<span class="small-red">&nbsp;*</span></label>
                             <input disabled v-model.trim="engMain.EngNo" type="text" class="form-control my-1 mr-0 mr-md-4 WidthasInput" placeholder="PCCES帶入">
                         </div>
                         <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                             <label class="my-2 mx-2">工程會標案編號<span class="small-red">&nbsp;*</span></label>
                             <div role="group" class="input-group">
                                 <input disabled v-model.trim="engMain.TenderNo" type="text" class="form-control">
                                 <button v-on:click="onTenderSearch()" v-bind:disabled="(this.engMain.Seq == -1)" title="標案查詢" class="btn btn-sm bg-gray" data-toggle="modal" data-target="#refTenderListModal"><i class="fas fa-search"></i></button>
                                 <button v-on:click="onCancelTenderLink()" title="清除" class="btn btn-sm bg-gray"><i class="fas fa-times"></i></button>
                             </div>
                         </div>
                     </div>
                     <hr class="my-3">
                     <label class="mb-1 mx-2 small-red">* 如設計委外，請務必填寫設計單位、統編、設計人員、聯絡人mail，並發送mail開通帳號，⑤</label>
                     <label class="mb-1 mx-2 small-red">* 如監造委外，請務必填寫監造單位、統編、監造主任、聯絡人mail，並發送mail開通帳號，⑤</label>
                     <label class="mb-1 mx-2 small-red">* 決標後，請務必填寫施工廠商、統編、聯絡人、聯絡人mail，並發送mail開通帳號，⑤</label>
                     <div class="form-row">
                         <div class="col form-inline my-2">
                             <label class="my-2 mx-2">自辦/委辦</label>
                             <div class="custom-control custom-radio mx-2">
                                 <input v-model="engMain.ExecType" @change="onExecTypeChange()" value="1" type="radio" id="self" name="S/C" class="custom-control-input">
                                 <label class="custom-control-label" for="self">自辦</label>
                             </div>
                             <div class="custom-control custom-radio mx-2">
                                 <input v-model="engMain.ExecType" @change="onExecTypeChange()" value="2" type="radio" id="commission" name="S/C" class="custom-control-input">
                                 <label class="custom-control-label" for="commission">委辦</label>
                             </div>
                         </div>
                     </div>
                     <div v-show="engMain.ExecType==2" class="form-row">
                         <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                             <label class="my-2 mx-2">設計單位</label>
                             <input v-model.trim="engMain.DesignUnitName" @change="onChangeDesignUnit" type="text" class="form-control my-1 mr-0 mr-md-4" placeholder="尚未輸入">
                         </div>
                         <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                             <label class="my-2 mx-2">設計單位統編</label>
                             <input v-model.trim="engMain.DesignUnitTaxId" @change="onChangeDesignUnit" type="text" class="form-control my-1 mr-0 mr-md-4" placeholder="尚未輸入">
                         </div>
                     </div>
                     <div v-show="engMain.ExecType==2" class="form-row">
                         <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                             <label class="my-2 mx-2">設計人員</label>
                             <input v-model.trim="engMain.DesignManName" @change="onChangeDesignUnit" type="text" class="form-control my-1 mr-0 mr-md-4" placeholder="尚未輸入">
                         </div>
                         <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                             <label class="my-2 mx-2">聯絡人mail</label>
                             <b-input-group>
                                 <input v-model="engMain.DesignUnitEmail" @change="onChangeDesignUnit" style="width:240px" type="text" class="form-control" placeholder="尚未輸入">
                                 <button v-on:click.stop="onSendMailDesignUnit" v-bind:disabled="!getDesignUnitState" v-bind:class="{'btn-color1':getDesignUnitState, 'bg-gray':!getDesignUnitState}"
                                         class="btn btn-sm" title="發送EMail">
                                     <i class="fas fa-envelope"></i>
                                 </button>
                             </b-input-group>
                             <label class="my-2 mx-2">(資料儲存完後，點擊發送帳號通知Mail，通知MAIL後開啟廠商權限)</label>
                         </div>
                     </div>
                     <!-- div class="form-row">
        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
            <label class="my-2 mx-2">預算書核定文號</label>
            <input v-model.trim="engMain.ApproveNo" type="text" class="form-control my-1 mr-0 mr-md-4" placeholder="尚未輸入">
        </div>
        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
            <label class="my-2 mx-2">預算書核定日期</label>
            <input v-if="!fCanEdit" v-bind:value="engMain.chsApproveDate" ref="chsApproveDate" type="text" class="form-control my-1 mr-0 mr-md-2" placeholder="yyy/mm/dd">
            <b-input-group v-if="fCanEdit">
                <input v-bind:value="engMain.chsApproveDate" ref="chsApproveDate" @change="onDateChange(engMain.chsApproveDate, $event, 'ApproveDate')" type="text" class="form-control mydatewidth mr-md-3" placeholder="yyy/mm/dd">
                <b-form-datepicker v-if="fCanEdit" v-model="chsApproveDate" :hide-header="hideHeader"
                                   button-only right size="sm" @context="onDatePicketChange($event, 'ApproveDate')">
                </b-form-datepicker>
            </b-input-group>
        </div>
    </div -->
                     <!--
    <div class="form-row">
        <div class="col form-inline my-2">
            <label class="my-2 mx-2">工程規模</label>
            <textarea v-model="engMain.ProjectScope" class="form-control col-12 col-md-10 mx-0 mx-md-2" rows="5"></textarea>
        </div>
    </div>-->
                     <!-- label class="my-2 mx-2 small-green">* 以下資料可於決標後填寫，系統會自動發送帳密到施工廠商之聯絡人信箱</label -->
                     <!-- shioulo20221216
    <div class="form-row">
        <div class="col-6 form-inline my-2 justify-content-md-between">
            <label class="my-2 mx-2">決標日期</label>
            <input v-if="!fCanEdit" v-bind:value="engMain.chsAwardDate" ref="chsAwardDate" type="text" class="form-control my-1 mr-0 mr-md-2" placeholder="yyy/mm/dd">
            <b-input-group v-if="fCanEdit">
                <input v-bind:value="engMain.chsAwardDate" ref="chsAwardDate" @change="onDateChange(engMain.chsAwardDate, $event, 'AwardDate')" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                <b-form-datepicker v-if="fCanEdit" v-model="chsAwardDate" :hide-header="hideHeader"
                                   button-only right size="sm" @context="onDatePicketChange($event, 'AwardDate')">
                </b-form-datepicker>
            </b-input-group>
        </div>
        <div v-if="engMain.chsAwardDate !=null && engMain.chsAwardDate != ''" style="color:red">
            ◆  請確認工程類標案已轉入工程會標案管理系統。
            ◆  決標日期完成後須勾稽工程會標案編號。
        </div>
    </div>
     -->
                     <div class="form-row">
                         <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                             <label class="my-2 mx-2">保固到期日</label>
                             <input v-if="!fCanEdit" v-bind:value="engMain.chsWarrantyExpires" ref="chsWarrantyExpires" type="text" class="form-control my-1 mr-0 mr-md-2" placeholder="yyy/mm/dd">
                             <b-input-group v-if="fCanEdit">
                                 <input v-bind:value="engMain.chsWarrantyExpires" ref="chsWarrantyExpires" @change="onDateChange(engMain.chsWarrantyExpires, $event, 'WarrantyExpires')" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                                 <b-form-datepicker v-if="fCanEdit" v-model="chsWarrantyExpires" :hide-header="hideHeader"
                                                    button-only right size="sm" @context="onDatePicketChange($event, 'WarrantyExpires')">
                                 </b-form-datepicker>
                             </b-input-group>
                         </div>
                     </div>
                     <!--
    <div class="form-row">
        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
            <label class="my-2 mx-2">工程期限({{engMain.DurationCategory==null ? '天' : engMain.DurationCategory}})</label>
            <input v-model="engMain.EngPeriod" type="text" class="form-control my-1 mr-0 mr-md-4" placeholder="輸入日曆天">
        </div>
    </div>
    <div class="form-row">
        <div class="col-6 form-inline my-2 justify-content-md-between">
            <label class="my-2 mx-2">開工日期</label>
            <input v-if="!fCanEdit" v-bind:value="engMain.chsStartDate" ref="chsStartDate" type="text" class="form-control my-1 mr-0 mr-md-2" placeholder="yyy/mm/dd">
            <b-input-group v-if="fCanEdit">
                <input v-bind:value="engMain.chsStartDate" ref="chsStartDate" @change="onDateChange(engMain.chsStartDate, $event, 'StartDate')" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                <b-form-datepicker v-if="fCanEdit" v-model="chsStartDate" :hide-header="hideHeader"
                                   button-only right size="sm" @context="onDatePicketChange($event, 'StartDate')">
                </b-form-datepicker>
            </b-input-group>
        </div>
    </div>
    <div class="form-row">
        <div class="col-6 form-inline my-2 justify-content-md-between">
            <label class="my-2 mx-2">預定完工日期</label>
            <input v-if="!fCanEdit" v-bind:value="engMain.chsSchCompDate" ref="chsSchCompDate" type="text" class="form-control my-1 mr-0 mr-md-2" placeholder="yyy/mm/dd">
            <b-input-group v-if="fCanEdit">
                <input v-bind:value="engMain.chsSchCompDate" ref="chsSchCompDate" @change="onDateChange(engMain.chsSchCompDate, $event, 'SchCompDate')" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                <b-form-datepicker v-if="fCanEdit" v-model="chsSchCompDate" :hide-header="hideHeader"
                                   button-only right size="sm" @context="onDatePicketChange($event, 'SchCompDate')">
                </b-form-datepicker>
            </b-input-group>
        </div>
        <div class="col-6 form-inline my-2 justify-content-md-between">
            <label class="my-2 mx-2">展延完工日期</label>
            <input v-if="!fCanEdit" v-bind:value="engMain.chsPostCompDate" ref="chsPostCompDate" type="text" class="form-control my-1 mr-0 mr-md-2" placeholder="yyy/mm/dd">
            <b-input-group v-if="fCanEdit">
                <input v-bind:value="engMain.chsPostCompDate" ref="chsPostCompDate" @change="onDateChange(engMain.chsPostCompDate, $event, 'PostCompDate')" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                <b-form-datepicker v-if="fCanEdit" v-model="chsPostCompDate" :hide-header="hideHeader"
                                   button-only right size="sm" @context="onDatePicketChange($event, 'PostCompDate')">
                </b-form-datepicker>
            </b-input-group>
        </div>
    </div>
    <div class="form-row">
        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
            <label class="my-2 mx-2">變更日期</label>
            <input v-if="!fCanEdit" v-bind:value="engMain.chsEngChangeStartDate" ref="chsEngChangeStartDate" type="text" class="form-control my-1 mr-0 mr-md-2" placeholder="yyy/mm/dd">
            <b-input-group v-if="fCanEdit">
                <input v-bind:value="engMain.chsEngChangeStartDate" ref="chsEngChangeStartDate" @change="onDateChange(engMain.chsEngChangeStartDate, $event, 'EngChangeStartDate')" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                <b-form-datepicker v-if="fCanEdit" v-model="chsEngChangeStartDate" :hide-header="hideHeader"
                                   button-only right size="sm" @context="onDatePicketChange($event, 'EngChangeStartDate')">
                </b-form-datepicker>
            </b-input-group>
        </div>
    </div>
    <div class="form-row">
        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
            <label class="my-2 mx-2">決標金額(元)</label>
            <input v-model="engMain.AwardAmount" type="text" class="form-control my-1 mr-0 mr-md-4" placeholder="尚未輸入">
        </div>
    </div>
    -->
                     <div class="form-row">
                         <div class="col form-inline my-2 justify-content-md-between">
                             <label class="my-2 mx-2">變更設計後契約金額(元)</label>
                             <input v-model="engMain.ContractAmountAfterDesignChange" type="text" class="form-control col-12 col-md-7 mx-0 mx-md-2" placeholder="尚未輸入">
                         </div>
                     </div>
                     <div class="form-row">
                         <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                             <label class="my-2 mx-2">施工廠商</label>
                             <input v-model.trim="engMain.BuildContractorName" @change="onChangeBuildContractor" type="text" class="form-control my-1 mr-0 mr-md-4" placeholder="尚未輸入">
                         </div>
                         <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                             <label class="my-2 mx-2">施工廠商統編</label>
                             <input v-model.trim="engMain.BuildContractorTaxId" @change="onChangeBuildContractor" type="text" class="form-control my-1 mr-0 mr-md-2" placeholder="輸入統編後檢核是否正確">
                         </div>
                     </div>
                     <div class="form-row">
                         <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                             <label class="my-2 mx-2">聯絡人</label>
                             <input v-model.trim="engMain.BuildContractorContact" @change="onChangeBuildContractor" type="text" class="form-control my-1 mr-0 mr-md-4" placeholder="尚未輸入">
                         </div>
                         <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                             <label class="my-2 mx-2">聯絡人mail</label>
                             <b-input-group>
                                 <input v-model="engMain.BuildContractorEmail" @change="onChangeBuildContractor" type="text" style="width:240px" class="form-control" placeholder="尚未輸入">
                                 <button v-on:click.stop="onSendMailBuildContractor" v-bind:disabled="!getBuildContractorState" v-bind:class="{'btn-color1':getBuildContractorState, 'bg-gray':!getBuildContractorState}"
                                         class="btn btn-sm" title="發送EMail">
                                     <i class="fas fa-envelope"></i>
                                 </button>
                             </b-input-group>
                             <label class="my-2 mx-2">(資料儲存完後，點擊發送帳號通知Mail，通知MAIL後開啟廠商權限)</label>
                         </div>
                     </div>
                     <div class="form-row">
                         <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                             <label class="my-2 mx-2">工地主任</label>
                             <input v-model.trim="engMain.ConstructionDirector" disabled type="text" class="form-control my-1 mr-0 mr-md-4" placeholder="尚未輸入">
                             <button role="button" @click="getSupervisionUserList('ConstructionDirector', 'C'+engMain.EngNo, 4)" class="btn btn-color11-3 btn-xs mx-1"><i class="fas fa-pencil-alt"> </i></button>
                            </div>
                     </div>
                     <div class="form-row">
                         <div class="col form-inline my-2 justify-content-md-between">
                             <label class="my-2 mx-2">工地現場人員1</label>
                             <input v-model.trim="engMain.ConstructionPerson1" disabled type="text" class="form-control col-12 col-md-7 mx-0 mx-md-2" placeholder="尚未輸入">
                             <button role="button" @click="getSupervisionUserList('ConstructionPerson1', 'C'+engMain.EngNo, 4)" class="btn btn-color11-3 btn-xs mx-1"><i class="fas fa-pencil-alt"> </i></button>
                         </div>
                     </div>
                     <div class="form-row">
                         <div class="col form-inline my-2 justify-content-md-between">
                             <label class="my-2 mx-2">工地現場人員2</label>
                             <input v-model.trim="engMain.ConstructionPerson2" disabled type="text" class="form-control col-12 col-md-7 mx-0 mx-md-2" placeholder="尚未輸入">
                             <button role="button" @click="getSupervisionUserList('ConstructionPerson2', 'C'+engMain.EngNo, 4)" class="btn btn-color11-3 btn-xs mx-1"><i class="fas fa-pencil-alt"> </i></button>
                         </div>
                     </div>
                     <div class="form-row">
                         <div class="col">
                            <div class="row d-flex">
                                <label class="my-2 mx-2 col-12">現場人員3-品管</label>
                             <input v-model.trim="engMain.SupervisorCommPerson4" disabled maxlength="50" type="text" class="form-control col-12 col-md-7 mx-0 mx-md-2" placeholder="尚未輸入">
                             <button role="button" @click="getSupervisionUserList('SupervisorCommPerson4', 'C'+engMain.EngNo, 4)" class="btn btn-color11-3 btn-xs mx-1"><i class="fas fa-pencil-alt"> </i></button>
                            </div>
                         
                         </div>
                         <div class="col-6 form-inline my-2 justify-content-md-between">
                             <label class="my-2 mx-2">證照到期日</label>
                             <input v-if="!fCanEdit" v-bind:value="engMain.chsSupervisorCommPerson4LicenseExpires" ref="chsSupervisorCommPerson4LicenseExpires" type="text" class="form-control my-1 mr-0 mr-md-2" placeholder="yyy/mm/dd">
                             <b-input-group v-if="fCanEdit">
                                 <input v-bind:value="engMain.chsSupervisorCommPerson4LicenseExpires" ref="chsSupervisorCommPerson4LicenseExpires" @change="onDateChange(engMain.chsSupervisorCommPerson4LicenseExpires, $event, 'SupervisorCommPerson4LicenseExpires')" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                                 <b-form-datepicker v-if="fCanEdit" v-model="chsSupervisorCommPerson4LicenseExpires" :hide-header="hideHeader"
                                                    button-only right size="sm" @context="onDatePicketChange($event, 'SupervisorCommPerson4LicenseExpires')">
                                 </b-form-datepicker>
                             </b-input-group>
                         </div>
                     </div>
                     <div class="form-row">
                         <div class="col  ">
                            <div class="row d-flex">
                                <label class="my-2 mx-2 col-12 ">現場人員4-職安</label>
                             <input v-model.trim="engMain.SupervisorCommPerson3" disabled maxlength="50" type="text" class="form-control col-12 col-md-7 mx-0 mx-md-2" placeholder="尚未輸入">
                             <button role="button" @click="getSupervisionUserList('SupervisorCommPerson3', 'C'+engMain.EngNo, 4)" class="btn btn-color11-3 btn-xs mx-1"><i class="fas fa-pencil-alt"> </i></button>
                            </div>
                            
                         </div>
                         <div class="col-6 form-inline my-2 justify-content-md-between">
                             <label class="my-2 mx-2">證照到期日</label>
                             <input v-if="!fCanEdit" v-bind:value="engMain.chsSupervisorCommPerson3LicenseExpires" ref="chsSupervisorCommPerson3LicenseExpires" type="text" class="form-control my-1 mr-0 mr-md-2" placeholder="yyy/mm/dd">
                             <b-input-group v-if="fCanEdit">
                                 <input v-bind:value="engMain.chsSupervisorCommPerson3LicenseExpires" ref="chsSupervisorCommPerson3LicenseExpires" @change="onDateChange(engMain.chsSupervisorCommPerson3LicenseExpires, $event, 'SupervisorCommPerson3LicenseExpires')" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                                 <b-form-datepicker v-if="fCanEdit" v-model="chsSupervisorCommPerson3LicenseExpires" :hide-header="hideHeader"
                                                    button-only right size="sm" @context="onDatePicketChange($event, 'SupervisorCommPerson3LicenseExpires')">
                                 </b-form-datepicker>
                             </b-input-group>
                         </div>
                     </div>
                     <div class="form-row mt-3">
                         <div class="col form-inline my-2 justify-content-md-between">
                             <label class="my-2 mx-2">現場人員5-土建及機電</label>
                             <input v-model.trim="engMain.SupervisorCommPersion2" disabled maxlength="50" type="text" class="form-control col-12 col-md-7 mx-0 mx-md-2" placeholder="尚未輸入">
                             <button role="button" @click="getSupervisionUserList('SupervisorCommPersion2', 'C'+engMain.EngNo, 4)" class="btn btn-color11-3 btn-xs mx-1"><i class="fas fa-pencil-alt"> </i></button>
                            </div>
                     </div>
                 </div>
                 <hr class="my-5">
                 <h2>監造計畫資料</h2>
                 <div>
                     <div class="form-row">
                         <div class="col form-inline my-2">
                             <label class="my-2 mx-2">自辦/委辦</label>
                             <div class="custom-control custom-radio mx-2">
                                 <input v-model="engMain.SupervisorExecType" @change="onSupervisorExecTypeChange()" value="1" type="radio" id="Supervisor-self" name="Supervisor-S/C" class="custom-control-input">
                                 <label class="custom-control-label" for="Supervisor-self">自辦</label>
                             </div>
                             <div class="custom-control custom-radio mx-2">
                                 <input v-model="engMain.SupervisorExecType" @change="onSupervisorExecTypeChange()" value="2" type="radio" id="Supervisor-commission" name="Supervisor-S/C" class="custom-control-input">
                                 <label class="custom-control-label" for="Supervisor-commission">委辦</label>
                             </div>
                         </div>
                     </div>
                     <div v-show="engMain.SupervisorExecType==2">
                         <div class="form-row">
                             <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                                 <label class="my-2 mx-2">監造單位</label>
                                 <input v-model.trim="engMain.SupervisorUnitName" @change="onChangeSupervisorUnit" type="text" class="form-control my-1 mr-0 mr-md-4" placeholder="尚未輸入">
                             </div>
                             <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                                 <label class="my-2 mx-2">監造單位統編</label>
                                 <input v-model.trim="engMain.SupervisorTaxid" @change="onChangeSupervisorUnit" type="text" class="form-control my-1 mr-0 mr-md-2" placeholder="輸入統編後檢核是否正確">
                             </div>
                         </div>
                         <div class="form-row">
                             <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                                 <label class="my-2 mx-2">監造主任</label>
                                 <input v-model.trim="engMain.SupervisorDirector" @change="onChangeSupervisorUnit" type="text" class="form-control my-1 mr-0 mr-md-4" placeholder="尚未輸入">
                             </div>
                             <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                                 <label class="my-2 mx-2">聯絡人mail</label>
                                 <b-input-group>
                                     <input v-model.trim="engMain.SupervisorContact" @change="onChangeSupervisorUnit" style="width:240px" type="text" class="form-control" placeholder="尚未輸入">
                                     <button v-on:click="onSendMailSupervisorUnit" v-bind:disabled="!getSupervisorUnitState" v-bind:class="{'btn-color1':getSupervisorUnitState, 'bg-gray':!getSupervisorUnitState}"
                                             class="btn btn-sm" title="發送EMail">
                                         <i class="fas fa-envelope"></i>
                                     </button>
                                 </b-input-group>
                                 <label class="my-2 mx-2">(資料儲存完後，點擊發送帳號通知Mail，通知MAIL後開啟廠商權限)</label>
                             </div>
                         </div>
                         <div class="form-row">
                             <div class="col form-inline my-2 justify-content-md-between">
                                 <label class="my-2 mx-2">監造技師(專案或計畫經理)</label>
                                 <input v-model.trim="engMain.SupervisorTechnician" disabled type="text" class="form-control col-12 col-md-8 mx-0 mx-md-2" placeholder="尚未輸入">
                                
                                    <button role="button" @click="getSupervisionUserList('SupervisorTechnician', 'S'+engMain.EngNo, 5)" class="btn btn-color11-3 btn-xs mx-1"><i class="fas fa-pencil-alt"> </i></button>
                                </div>
                         </div>
                         <div class="form-row">
                             <div class="col form-inline my-2 justify-content-md-between">
                                 <label class="my-2 mx-2">監造現場人員1</label>
                                 <input v-model.trim="engMain.SupervisorSelfPerson1" disabled type="text" class="form-control col-12 col-md-8 mx-0 mx-md-2" placeholder="尚未輸入">
                            
                                 <button role="button" @click="getSupervisionUserList('SupervisorSelfPerson1', 'S'+engMain.EngNo, 5)" class="btn btn-color11-3 btn-xs mx-1"><i class="fas fa-pencil-alt"> </i></button>
                                </div>
                         </div>
                         <div class="form-row">
                             <div class="col form-inline my-2 justify-content-md-between">
                                 <label class="my-2 mx-2">監造現場人員2</label>
                                 <input v-model.trim="engMain.SupervisorSelfPerson2" type="text" disabled class="form-control col-12 col-md-8 mx-0 mx-md-2" placeholder="尚未輸入">
                            
                                 <button role="button" @click="getSupervisionUserList('SupervisorSelfPerson2', 'S'+engMain.EngNo, 5)" class="btn btn-color11-3 btn-xs mx-1"><i class="fas fa-pencil-alt"> </i></button>
                                </div>
                         </div>
                     </div>
                 </div>
                 <div class="row justify-content-center mt-5">
                         <button  role="button" v-on:click.stop="updateData()"  class="btn btn-color11-4 btn-xs mx-1" >
                             <i class="fas fa-save"> 儲存</i>
                         </button>
                 </div>
                 <!--
                 <label class="mt-5 mb-1 mx-2 small-green">* 點選儲存後，僅儲存與該主檔有關之資料</label>
                 <label class="my-1 mx-2 small-green">* 點選【匯入標案基本資料】按鈕，系統更新主檔及其相關資料表外，自動將後台預設的材料設備清單、施工管理清單、設備運轉清單、職業安全、環境保育等資料儲存到該標案對應的資料表。</label>
                 -->
             </div>

         <!-- 標案清單 shioulo 20220504 -->
         <div class="modal fade" id="refTenderListModal" data-backdrop="static" data-keyboard="false" tabindex="-1" aria-labelledby="refTenderListModal" aria-modal="true">
             <div class="modal-dialog modal-lg">
                 <div class="modal-content">
                     <div class="modal-header">
                         <h5 class="modal-title" id="projectUpload">標案清單</h5>
                         <button id="closeTenderListModal" type="button" class="close" data-dismiss="modal" aria-label="Close">
                             <span aria-hidden="true">×</span>
                         </button>
                     </div>
                     <div class="modal-body">
                         <div class="table-responsive">
                             <table class="table table-responsive-md table-hover">
                                 <thead class="insearch">
                                     <tr>
                                         <th>標案編號</th>
                                         <th>工程名稱</th>
                                     </tr>
                                 </thead>
                                 <tbody>
                                     <tr v-on:click.stop="onSelectTrender(item)" v-for="(item, index) in tenders" v-bind:key="item.Seq">
                                         <td>{{item.TenderNo}}</td>
                                         <td>{{item.TenderName}}</td>
                                     </tr>
                                 </tbody>
                             </table>
                         </div>
                     </div>
                 </div>
             </div>
         </div>
         <Modal ref="supervisionUserListModal" >
                <template #body>
                    <p style="color:red">*人員需要到後台使用者管理新增</p>
                    <table class="table ">
                        <thead>
                            <tr>
                                <th class="text-left">帳號</th>
                                <th>人員</th>

                                <th>選擇</th>
                            </tr>

                        </thead>
                        <tbody>
                                <tr v-for="(user, index) in  supervisionUserViewList" :key="index">
                                        <td class="text-left"> {{ user.UserNo }}</td>
                                        <td >{{ user.DisplayName }}</td>
                                        <td>
                                            <button role="button" :class="`btn btn-color11-3 btn-xs mx-1`"  @click="changeSupervisionUser(user)"><i class="fas fa-check"> </i></button>
                                        </td>

                        

                                </tr>
                        </tbody>
                    </table>

                </template>
         </Modal>
     </div>
</template>
<script>
    export default {
        data: function () {
            return {
                fSendMail: false,//防止連續發送
                fSendMailSupervisorUnit: false,
                fSendMailBuildContractor: false,
                fSendMailDesignUnit: false,
                fCanEdit: true, // 20220916 先取消管制 false,
                saveFlag: false,
                targetId: null,
                

                //使用者單位資訊
                userUnit: null,
                userUnitName: '',
                userUnitSub: null,
                userUnitSubName: '',

                users: [],//人員清單
                units: [],//機關清單
                organizerSubUnits: [],//主辦機關單位清單
                execSubUnits: [],//執行機關單位清單
                cities: [],//行政區(縣市)清單
                towns: [],//行政區(鄉鎮)清單
                engMain: { OrganizerUnitSeq: -1, ExecUnitSeq: -1, citySeq: -1, EngTownSeq: -1, OrganizerSubUnitSeq: -1 },

                //for datepicket
                //chsStartDate: '',
                //chsSchCompDate: '',
                //chsPostCompDate: '',
                //chsApproveDate: '',
                //chsAwardDate: '', //決標日期 shioulo 20221218
                //s20220825
                chsSupervisorCommPerson4LicenseExpires: '',
                chsSupervisorCommPerson3LicenseExpires: '',
                //chsEngChangeStartDate: '',
                chsWarrantyExpires: '',
                hideHeader: true,
                tenders:[], //標案清單 shioulo 20220504
                supervisionUserList : [] ,
                userInfo : {},
                editPersonType : "",
                editPersonRole : null
                
            };
        },
        components :{
            Modal : require("../../components/Modal.vue").default
        },
        methods: {
            //
            async getSupervisionUserList(typeName, keyWord, role)
            {


                this.editPersonType = typeName;
     
                this.$refs.supervisionUserListModal.show = true;
                if(this.editPersonRole != role) 
                {
                    this.supervisionUserList = [];
                    this.supervisionUserList = (await window.myAjax.post("Users/GetUserByAccountKeyWord", { keyWord : keyWord, role : role } )).data;
                    this.supervisionUserList.forEach(e => e.IsSelected = 
                        e.DisplayName == this.engMain.SupervisorSelfPerson1 ||
                        e.DisplayName == this.engMain.SupervisorSelfPerson2 ||
                        e.DisplayName == this.engMain.SupervisorTechnician ||
                        e.DisplayName == this.engMain.ConstructionDirector ||
                        e.DisplayName == this.engMain.ConstructionPerson1 ||
                        e.DisplayName == this.engMain.ConstructionPerson2 ||
                        e.DisplayName == this.engMain.SupervisorCommPerson4 ||
                        e.DisplayName == this.engMain.SupervisorCommPerson3 ||
                        e.DisplayName == this.engMain.SupervisorCommPerson2 
                                    
                    );
                    this.editPersonRole = role;
                }

            },
            changeSupervisionUser(user)
            {
                this.engMain[this.editPersonType] = user.DisplayName;
                this.$refs.supervisionUserListModal.show = false;
                user.IsSelected = !user.IsSelected;

            },
            onExecTypeChange() {
                console.log('onExecTypeChange()');
                if (this.engMain.ExecType == 1 && this.engMain.DesignUnitName == '') {
                    this.engMain.DesignUnitName = this.engMain.execUnitName;
                } 
            },
            onSupervisorExecTypeChange() {
                console.log('onSupervisorExecTypeChange()');
                if (this.engMain.SupervisorExecType == 1 && this.engMain.SupervisorUnitName == '') {
                    this.engMain.SupervisorUnitName = this.engMain.execUnitName;
                }
            },
            getItem() {
                this.fCanEdit = true;// 20220916 先取消管制 this.fCanEdit = false;
                this.engMain = {};
                window.myAjax.post('/TenderPlan/GetEngItem', {id:this.targetId})
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engMain = resp.data.item;
                            this.getCityTown();
                            //this.getOrganizerSubUnit(this.engMain.OrganizerUnitSeq);
                            //this.getUsers();
                            //this.getExecSubUnit(this.engMain.ExecUnitSeq);
                            if (this.engMain.DocState == null || this.engMain.DocState == -1) {
                                this.fCanEdit = true;
                                //this.chsStartDate = this.toYearDate(this.engMain.chsStartDate);
                                //this.chsSchCompDate = this.toYearDate(this.engMain.chsSchCompDate);
                                //this.chsPostCompDate = this.toYearDate(this.engMain.chsPostCompDate);
                                //this.chsApproveDate = this.toYearDate(this.engMain.chsApproveDate);
                                //this.chsAwardDate = this.toYearDate(this.engMain.chsAwardDate);
                                this.chsSupervisorCommPerson4LicenseExpires = this.toYearDate(this.engMain.chsSupervisorCommPerson4LicenseExpires);
                                this.chsSupervisorCommPerson3LicenseExpires = this.toYearDate(this.engMain.chsSupervisorCommPerson3LicenseExpires);
                                //this.chsEngChangeStartDate = this.toYearDate(this.engMain.chsEngChangeStartDate);
                                this.chsWarrantyExpires = this.toYearDate(this.engMain.chsWarrantyExpires);
                                
                            }
                        } else {
                            alert(resp.data.message);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onChangeSupervisorUnit() {
                this.fSendMailSupervisorUnit = false;
            },
            onChangeBuildContractor() {
                this.fSendMailBuildContractor = false;
            },
            onChangeDesignUnit() {
                this.fSendMailDesignUnit = false;
            },

            editEng(engSeq) {
                window.myAjax.post('/TenderPlan/EditEng', { seq: engSeq })
                    .then(resp => {
                        console.log(resp.data.Url);
                        window.location.href = resp.data.Url;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //取得使用者單位資訊
            getUserUnit() {
                window.myAjax.post('/TenderPlan/GetUserUnit')
                    .then(resp => {
                        this.userUnit = resp.data.unit;
                        this.userUnitName = resp.data.unitName;
                        this.userUnitSub = resp.data.unitSub;
                        this.userUnitSubName = resp.data.unitSubName;

                        this.selectUnit = this.userUnit;
                        //this.onUnitChange(this.selectUnit)
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },

            //行政區(縣市)
            async getCities() {
                this.cities = [];
                const { data } = await window.myAjax.post('/TenderPlan/GetCityList');
                this.cities = data;
            },
            onCityChange(event) {
                this.getCityTown();
            },
            //行政區(鄉鎮)
            async getCityTown() {
                if (this.engMain.CitySeq > 0) {
                    this.twons = [];
                    const { data } = await window.myAjax.post('/TenderPlan/GetTownList', { id: this.engMain.CitySeq });
                    this.towns = data;
                }
            },
            onDateChange(srcDate, event, mode) {
                if (event.target.value.length == 0) {
                    /*if (mode == 'StartDate')
                        this.chsStartDate = '';
                    else if (mode == 'SchCompDate')
                        this.chsSchCompDate = '';
                    else if (mode == 'PostCompDate')
                        this.chsPostCompDate = '';
                    else if (mode == 'ApproveDate')
                        this.chsApproveDate = '';
                    else if (mode == 'AwardDate')
                        this.chsAwardDate = '';
                    else*/ if (mode == 'SupervisorCommPerson4LicenseExpires')
                        this.chsSupervisorCommPerson4LicenseExpires = '';
                    else if (mode == 'SupervisorCommPerson3LicenseExpires')
                        this.chsSupervisorCommPerson3LicenseExpires = '';
                    //else if (mode == 'EngChangeStartDate')
                    //    this.chsEngChangeStartDate = '';
                    else if (mode == 'WarrantyExpires')
                        this.chsWarrantyExpires = '';
                    
                    return;
                }
                if (!this.isExistDate(event.target.value)) {
                    event.target.value = srcDate;
                    alert("日期錯誤");
                } else {
                    /*if (mode == 'StartDate')
                        this.chsStartDate = this.toYearDate(event.target.value);
                    else if (mode == 'SchCompDate')
                        this.chsSchCompDate = this.toYearDate(event.target.value);
                    else if (mode == 'PostCompDate')
                        this.chsPostCompDate = this.toYearDate(event.target.value);
                    else if (mode == 'ApproveDate')
                        this.chsApproveDate = this.toYearDate(event.target.value);
                    else if (mode == 'AwardDate')
                        this.chsAwardDate = this.toYearDate(event.target.value);
                    else*/ if (mode == 'SupervisorCommPerson4LicenseExpires')
                        this.chsSupervisorCommPerson4LicenseExpires = this.toYearDate(event.target.value);
                    else if (mode == 'SupervisorCommPerson3LicenseExpires')
                        this.chsSupervisorCommPerson3LicenseExpires = this.toYearDate(event.target.value);
                    //else if (mode == 'EngChangeStartDate')
                    //    this.chsEngChangeStartDate = this.toYearDate(event.target.value);
                    else if (mode == 'WarrantyExpires')
                        this.chsWarrantyExpires = this.toYearDate(event.target.value);
                }
            },
            onDatePicketChange(ctx, mode) {
                //console.log(ctx);
                if (ctx.selectedDate != null) {
                    var d = ctx.selectedDate;
                    var dd = (d.getFullYear() - 1911) + '/' + (d.getMonth() + 1) + '/' + d.getDate();
                    //var y = d.getYear() - 1911;
                    /*if (mode == 'StartDate')
                        this.engMain.chsStartDate = dd;
                    else if (mode == 'SchCompDate')
                        this.engMain.chsSchCompDate = dd;
                    else if (mode == 'PostCompDate')
                        this.engMain.chsPostCompDate = dd;
                    else if (mode == 'ApproveDate')
                        this.engMain.chsApproveDate = dd;
                    else if (mode == 'AwardDate')
                        this.engMain.chsAwardDate = dd;
                    else*/ if (mode == 'SupervisorCommPerson4LicenseExpires')
                        this.engMain.chsSupervisorCommPerson4LicenseExpires = dd;
                    else if (mode == 'SupervisorCommPerson3LicenseExpires')
                        this.engMain.chsSupervisorCommPerson3LicenseExpires = dd;
                    //else if (mode == 'EngChangeStartDate')
                    //    this.engMain.chsEngChangeStartDate = dd;
                    else if (mode == 'WarrantyExpires')
                        this.engMain.chsWarrantyExpires = dd;
                }
            },
            //中曆轉西元
            toYearDate(dateStr) {
                if (dateStr == null || dateStr == '') return null;
                var dateObj = dateStr.split('/'); // yyy/mm/dd
                return new Date( parseInt(dateObj[0]) + 1911, parseInt(dateObj[1])-1, parseInt(dateObj[2]) );
            },
            //日期檢查
            isExistDate(dateStr) {
                var dateObj = dateStr.split('/'); // yyy/mm/dd
                if (dateObj.length != 3) return false;

                var limitInMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

                var theYear = parseInt(dateObj[0]);
                if (theYear != dateObj[0]) return false;
                var theMonth = parseInt(dateObj[1]);
                if (theMonth != dateObj[1]) return false;
                var theDay = parseInt(dateObj[2]);
                if (theDay != dateObj[2]) return false;
                if (new Date(theYear + 1911, 1, 29).getDate() === 29) { // 是否為閏年?
                    limitInMonth[1] = 29;
                }
                return theDay <= limitInMonth[theMonth - 1];
            },
            //
            updateData() {
                this.editPersonRole = null;
                /*if (this.engMain.EngName == null || this.engMain.EngName.length == 0
                    || this.engMain.CitySeq == null || this.engMain.EngTownSeq == null
                    || this.engMain.EngNo == null || this.engMain.EngNo.length == 0
                    || this.engMain.OrganizerUnitSeq == null
                    || this.engMain.ExecUnitSeq == null || this.engMain.ExecSubUnitSeq == null) {
                    alert('* 欄位必須選填');
                    return;
                }*/
                /*//s20220914 取消
                if (this.engMain.BuildContractorContact.length == 0 || this.engMain.BuildContractorName.length == 0 || this.engMain.BuildContractorEmail.length == 0) {
                    alert('施工廠商 名稱, 聯絡人, Email 必須填寫');
                    return;
                }*/
                if (this.engMain.BuildContractorTaxId !=null && this.engMain.BuildContractorTaxId.length > 0 && !this.checkCompanyNo(this.engMain.BuildContractorTaxId)) {
                    alert('施工廠商統編錯誤');
                    return;
                }
                /*if (this.engMain.SupervisorExecType == 2) {
                    if (this.engMain.SupervisorDirector.length == 0 || this.engMain.SupervisorUnitName.length == 0
                        || this.engMain.SupervisorTaxid.length == 0 || this.engMain.SupervisorContact.length == 0) {
                        alert('監造單位 名稱, 統編, 聯絡人, Email 必須填寫');
                        return;
                    }
                }*/
                //shioulo 20221229
                if (this.engMain.SupervisorTaxid !=null && this.engMain.SupervisorTaxid.length > 0 && !this.checkCompanyNo(this.engMain.SupervisorTaxid)) {
                    alert('監造單位統編錯誤');
                    return;
                }
                /*if (this.engMain.ExecType == 2) {
                    if (this.engMain.DesignUnitName.length == 0 || this.engMain.DesignManName.length == 0
                        || this.engMain.DesignUnitTaxId.length == 0 || this.engMain.DesignUnitEmail.length == 0) {
                        alert('設計單位 名稱, 統編, 聯絡人, Email 必須填寫');
                        return;
                    }
                }*/
                //shioulo 20221229
                if (this.engMain.DesignUnitTaxId != null && this.engMain.DesignUnitTaxId.length > 0 && !this.checkCompanyNo(this.engMain.DesignUnitTaxId)) {
                    alert('設計單位統編錯誤');
                    return;
                }
                //this.engMain.chsStartDate = this.$refs['chsStartDate'].value;
                //this.engMain.chsSchCompDate = this.$refs['chsSchCompDate'].value;
                //this.engMain.chsPostCompDate = this.$refs['chsPostCompDate'].value;
                //this.engMain.chsApproveDate = this.$refs['chsApproveDate'].value;
                //this.engMain.chsAwardDate = this.$refs['chsAwardDate'].value;
                this.engMain.chsSupervisorCommPerson4LicenseExpires = this.$refs['chsSupervisorCommPerson4LicenseExpires'].value;
                this.engMain.chsSupervisorCommPerson3LicenseExpires = this.$refs['chsSupervisorCommPerson3LicenseExpires'].value;
                //this.engMain.chsEngChangeStartDate = this.$refs['chsEngChangeStartDate'].value;
                this.engMain.chsSchWarrantyExpires = this.$refs['chsWarrantyExpires'].value;
                //shioulo 20221218
                /*if (this.engMain.chsAwardDate != null && this.engMain.chsAwardDate.length > 0 && this.engMain.PrjXMLSeq == null) {
                    if (!confirm("決標日期已填寫, 但工程會標案編號 未設定")) return;
                }*/

                if(this.engMain.ExecType == 1)
                {
                    this.engMain.DesignManName = "";
                    this.engMain.DesignUnitEmail = "";
                    this.engMain.DesignUnitName = '';
                    this.engMain.DesignUnitTaxId = '';
                }
                if(this.engMain.SupervisorExecType == 1)
                {
                    this.engMain.SupervisorContact = '';
                    this.engMain.SupervisorDirector = '';
                    this.engMain.SupervisorSelfPerson1 = '';
                    this.engMain.SupervisorSelfPerson2 = '';
                    this.engMain.SupervisorTaxid = '';
                    this.engMain.SupervisorTechnician = '';
                    this.engMain.SupervisorUnitName = '';
                }   
                window.myAjax.post('/TenderPlan/UpdateTenderPlan', { engMain: this.engMain })
                    .then(resp => {
                        this.saveFlag = false;
                        if (resp.data.result == 0) {
                            this.saveFlag = true;
                            //this.engMain = resp.data.item;
                            this.fSendMailSupervisorUnit = true;
                            this.fSendMailBuildContractor = true;
                            this.fSendMailDesignUnit = true;
                            alert(resp.data.message);
                        } else {
                            alert(resp.data.message);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });

                window.myAjax.post("TenderPlan2/UpdateDirectorUserName", {
                    directorName :this.engMain.BuildContractorContact, 
                    engSeq : this.engMain.Seq,
                    keyWord : "C"+ this.engMain.EngNo

                });

                window.myAjax.post("TenderPlan2/UpdateDirectorUserName", {
                    directorName : this.engMain.SupervisorDirector,
                    engSeq : this.engMain.Seq,
                    keyWord : "S"+ this.engMain.EngNo

                });

            },
            createSupervisionProject() {
                if (!this.saveFlag) return;

                if (confirm('系統將更新主檔及其相關資料表外，如材料設備清單、施工管理清單、設備運轉清單、職業安全、環境保育等資料\n\n是否確定?')) {
                    window.myAjax.post('/TenderPlan/CreateSupervisionProject', { engMain: this.engMain.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.engMain.DocState = 0;
                            }
                            alert(resp.data.message);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            //檢查公司統一編號是否正確
            checkCompanyNo(idvalue) {
                var tmp = new String("12121241");
                var sum = 0;
                var re = /^\d{8}$/;
                if (!re.test(idvalue)) {
                    //alert("格式不對！");
                    return false;
                }
                var i = 0;
                for (i = 0; i < 8; i++) {
                    var s1 = parseInt(idvalue.substr(i, 1));
                    var s2 = parseInt(tmp.substr(i, 1));
                    sum += this.cal(s1 * s2);
                }
                if (!this.valid(sum)) {
                    if (idvalue.substr(6, 1) == "7") return (this.valid(sum + 1));
                }
                return (this.valid(sum));
            },
            valid(n) {
                return (n % 10 == 0) ? true : false;
            },
            cal(n) {
                var sum = 0;
                while (n != 0) {
                    sum += (n % 10);
                    n = (n - n % 10) / 10;  // 取整數
                }
                return sum;
            },

            //設計單位發送mail
            onSendMailDesignUnit() {
                if (!this.checkCompanyNo(this.engMain.DesignUnitTaxId)) {
                    alert('設計單位統編錯誤');
                    return;
                }
                if (!this.validateEmail(this.engMain.DesignUnitEmail)) {
                    alert('聯絡人mail錯誤');
                    return;
                }
                if (this.engMain.DesignUnitName && this.engMain.DesignUnitName.length > 0
                    && this.engMain.DesignManName && this.engMain.DesignManName.length > 0) {
                    this.fSendMail = true;
                    window.myAjax.post('/TenderPlan/SendMailToDesignUnit', { engMain: this.engMain.Seq })
                        .then(resp => {
                            alert(resp.data.message);
                            this.fSendMail = false;
                        })
                        .catch(err => {
                            console.log(err);
                            this.fSendMail = false;
                        });

                } else {
                    alert('設計單位名稱,設計單位連絡人 資料錯誤');
                }
            },
            //施工廠商發送mail
            onSendMailBuildContractor() {
                if (!this.checkCompanyNo(this.engMain.BuildContractorTaxId)) {
                    alert('施工廠商統編錯誤');
                    return;
                }
                if (!this.validateEmail(this.engMain.BuildContractorEmail)) {
                    alert('聯絡人mail錯誤');
                    return;
                }
                if (this.engMain.BuildContractorName && this.engMain.BuildContractorName.length > 0
                    && this.engMain.BuildContractorContact && this.engMain.BuildContractorContact.length > 0) {
                    this.fSendMail = true;
                    window.myAjax.post('/TenderPlan/SendMailToBuildContractor', { engMain: this.engMain.Seq })
                        .then(resp => {
                            alert(resp.data.message);
                            this.fSendMail = false;
                        })
                        .catch(err => {
                            console.log(err);
                            this.fSendMail = false;
                        });

                } else {
                    alert('施工廠商名稱,施工廠商連絡人 資料錯誤');
                }
                
            },
            //監造單位發送mail
            onSendMailSupervisorUnit() {
                console.log('onSendMailSupervisorUnit()');
                if (!this.checkCompanyNo(this.engMain.SupervisorTaxid)) {
                    alert('監造單位統編錯誤');
                    return;
                }
                if (!this.validateEmail(this.engMain.SupervisorContact)) {
                    alert('聯絡人mail錯誤');
                    return;
                }
                if (this.engMain.SupervisorUnitName && this.engMain.SupervisorUnitName.length > 0
                    && this.engMain.SupervisorDirector && this.engMain.SupervisorDirector.length > 0) {
                    this.fSendMail = true;
                    window.myAjax.post('/TenderPlan/SendMailToSupervisor', { engMain: this.engMain.Seq})
                        .then(resp => {
                            alert(resp.data.message);
                            this.fSendMail = false;
                        })
                        .catch(err => {
                            console.log(err);
                            this.fSendMail = false;
                        });

                } else {
                    alert('監造單位,SupervisorDirector 資料錯誤');
                }
            },
            validateEmail(email) {
                const re = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                return re.test(String(email).toLowerCase());
            },
            back(){
                //window.history.go(-1);
                window.location = "/TenderPlan/TenderPlanList";
            },

            //標案查詢 shioulo 20220504
            onTenderSearch() {
                this.tenders = [];
                if (this.engMain.ExecUnitSeq > 0) {
                    window.myAjax.post('/TenderPlan/GetTenderList', { id: this.engMain.Seq })
                        .then(resp => {
                            this.tenders = resp.data;
                        })
                        .catch(err => {
                            console.log(err);
                        });
                } else {
                    alert('沒有執行單位');
                }
            },
            //工程連結標案 shioulo 20220518
            onSelectTrender(item) {
                window.myAjax.post('/TenderPlan/SetEngLinkTender', { id: this.engMain.Seq, prj: item.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engMain.PrjXMLSeq = item.Seq;
                            this.engMain.TenderNo = item.TenderNo;
                            document.getElementById("closeTenderListModal").click();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //工程連結標案取消 shioulo 20220519
            onCancelTenderLink() {
                window.myAjax.post('/TenderPlan/CancelEngLinkTender', { id: this.engMain.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engMain.PrjXMLSeq = null;
                            this.engMain.TenderNo = "";
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            }
        },
        computed: {
            supervisionUserViewList()
            {
                return this.supervisionUserList;
            },
            getSupervisorUnitState() {
                return this.fSendMailSupervisorUnit && !this.fSendMail;
            },
            getBuildContractorState() {
                return this.fSendMailBuildContractor && !this.fSendMail;
            },
            getDesignUnitState() {
                return this.fSendMailDesignUnit && !this.fSendMail;
            }
        },
        async mounted() {
            console.log('mounted() 工程查詢-標案編輯 ' + window.location.href);
            let urlParams = new URLSearchParams(window.location.search);
            if (urlParams.has('id')) {
                //if (this.units.length == 0) this.getUnits();
                if (this.cities.length == 0) this.getCities();
                //if (this.users.length == 0) this.getUsers();
                
                this.targetId = parseInt(urlParams.get('id'), 10);
                console.log(this.targetId);
                if (Number.isInteger(this.targetId)) {
                    if (this.targetId <= 0) {
                        window.history.back();
                    } else {
                        this.getUserUnit();
                        this.getItem();
                    }
                    return;
                }
                
            }
            window.history.back();
        }
    }
</script>