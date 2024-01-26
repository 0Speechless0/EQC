<template>
    <div>
        <h5 class="insearch my-0 py-2">
            <div class="form-row justify-content-start my-1">
                <div class="col-12 col-lg-6 col-xl-8 my-1"> 工程編號：{{tenderItem.TenderNo}}({{tenderItem.EngNo}})<br>工程名稱：{{tenderItem.TenderName}}({{tenderItem.EngName}}) </div>
                <div class="col-6 col-lg-2 col-xl-1"><!----></div>
                <div class="col-6 col-lg-4 col-xl-3">
                    <button v-if="riskItem.LockState==2" v-on:click.stop="unlockItem(0)" class="btn btn-color11-3 btn-block" style="width: 200px;"><i class="fas fa-lock"></i>解鎖</button>
                    <button class="btn btn-color11-2 btn-block" style="width: 200px;"><i class="fas fa-upload"></i> 上傳定稿</button>
                </div>
                <div class="col-6 col-lg-2 col-xl-1"><!----></div>
                <div class="col-6 col-lg-2 col-xl-1"><!----></div>
                <div class="col-6 col-lg-2 col-xl-1"><!----></div>
            </div>
        </h5>
        <div style="padding-top: 10px;">
            <ul role="tablist" class="nav nav-tabs">
                <li class="nav-item"><a data-toggle="tab" href="#menu01" class="nav-link active">工程計畫概要</a></li>
                <li class="nav-item"><a data-toggle="tab" href="#menu03" class="nav-link">準備作業</a></li>
                <li class="nav-item"><a data-toggle="tab" href="#menu04" class="nav-link">設計方案評選</a></li>
                <li class="nav-item"><a data-toggle="tab" href="#menu05" class="nav-link" @click="title='menu05'">設計成果摘要說明</a></li>
                <li class="nav-item"><a data-toggle="tab" href="#menu06" class="nav-link" @click="title='menu06'">設計成果施工風險評估</a></li>
                <li class="nav-item"><a data-toggle="tab" href="#menu07" class="nav-link">設計階段施工風險評估成果之運用</a></li>
                <li class="nav-item"><a data-toggle="tab" href="#menu08" class="nav-link">風險資訊傳遞及風險追蹤管理</a></li>
                <li class="nav-item"><a data-toggle="tab" href="#menu09" class="nav-link">結論</a></li>
            </ul>
        </div>
        <div class="tab-content">
            <div id="menu01" class="tab-pane active">
                <div class="tab-content" id="menu01-tab">
                    <h5>計畫緣起與目標</h5>
                    <div>
                        <textarea rows="8" class="form-control" v-model.trim="riskItem.PlanOriginAndTarget"></textarea>
                    </div>
                    <h5>計畫範圍</h5>
                    <div>
                        <textarea rows="5" class="form-control" v-model.trim="riskItem.PlanScope"></textarea>
                    </div>
                    <div style="padding-top: 10px;width: 40%;display: flex;">
                        <label style="width: 35%;vertical-align:middle;">計畫範圍圖</label>
                        <label class="btn btn-shadow btn-color11-3">
                            <input v-on:change="fileChange($event,'A1')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>上傳
                        </label>
                        <button @click="onDownload('A1')" class="btn btn-color11-1" title="下載"><i class="fas fa-download"></i>下載</button>
                        {{riskItem.PlanScopeFileName}}
                    </div>
                    <p data-v-04c7975c="" style="color: red;">格式說明： .jpg、.jpeg、.png</p>
                    <h5>計畫環境</h5>
                    <div>
                        <textarea rows="5" class="form-control" v-model.trim="riskItem.PlanEnvironment"></textarea>
                    </div>
                    <h5>規劃設計構想</h5>
                    <div>
                        <textarea rows="5" class="form-control" v-model.trim="riskItem.DesignConcept"></textarea>
                    </div>
                    <div style="padding-top: 10px;width: 40%;display: flex;">
                        <label style="width: 35%;vertical-align:middle;">平面佈置圖</label>
                        <label class="btn btn-shadow btn-color11-3">
                            <input v-on:change="fileChange($event,'A2')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>上傳
                        </label>
                        <button @click="onDownload('A2')" class="btn btn-color11-1" title="下載"><i class="fas fa-download"></i>下載</button>
                        {{riskItem.DesignConceptFileName}}
                    </div>
                    <p data-v-04c7975c="" style="color: red;">格式說明： .jpg、.jpeg、.png</p>
                    <h5>工程功能需求</h5>
                    <div class="table-responsive">
                        <table class="table table1 min910" border="0" id="addnew051501">
                            <thead class="insearch">
                                <tr>
                                    <th style="text-align: center; width: 5%;">項次</th>
                                    <th style="text-align: left;width:20% ;">工程功能</th>
                                    <th>說明</th>
                                    <th style="text-align: center; width: 10%;">功能</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(item, index) in itemsA" v-bind:key="item.Seq">
                                    <template v-if="item.Seq != editSeqA">
                                        <td>{{index+1}}</td>
                                        <td>{{item.EngFunction}}</td>
                                        <td>{{item.EngMemo}}</td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button @click="onEditRecord(1,item)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                                <button @click="onDelRecord(1,item)" class="btn btn-color9-1 btn-xs sharp mx-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                                            </div>
                                        </td>
                                    </template>
                                    <template v-if="item.Seq == editSeqA">
                                        <td>{{index+1}}</td>
                                        <td><textarea v-model.trim="item.EngFunction" rows="1" type="text" class="form-control"></textarea></td>
                                        <td><textarea v-model.trim="item.EngMemo" rows="3" type="text" class="form-control"></textarea></td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button @click="onSaveRecordA(item)" class="btn btn-color11-2 btn-xs sharp mx-1"><i class="fas fa-save"></i></button>
                                                <button @click="editSeqA = -99" class="btn btn-color9-1 btn-xs sharp mx-1" title="取消"><i class="fas fa-times"></i></button>
                                            </div>
                                        </td>
                                    </template>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td><textarea v-model.trim="newRecordA.EngFunction" rows="1" type="text" class="form-control"></textarea></td>
                                    <td><textarea v-model.trim="newRecordA.EngMemo" rows="3" type="text" class="form-control"></textarea></td>
                                    <td>
                                        <div class="d-flex justify-content-center">
                                            <button @click="onNewRecordA" class="btn btn-outline-secondary btn-xs sharp mx-1" title="新增"><i class="fas fa-plus fa-lg"></i></button>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <h5>工址環境現況</h5>
                    <div class="table-responsive">
                        <table class="table table-responsive-md table-hover VA-middle" id="addnew051502">
                            <thead class="table table1 min910">
                                <tr class="insearch">
                                    <th style="text-align: center; width: 5%;">項次</th>
                                    <th style="text-align: left;width:20% ;">工址環境</th>
                                    <th>說明</th>
                                    <th style="text-align: center; width: 10%;">功能</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(item, index) in itemsB" v-bind:key="item.Seq">
                                    <template v-if="item.Seq != editSeqB">
                                        <td>{{index+1}}</td>
                                        <td>{{item.SiteEnvironment}}</td>
                                        <td>{{item.EngMemo}}</td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button @click="onEditRecord(2,item)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                                <button @click="onDelRecord(2,item)" class="btn btn-color9-1 btn-xs sharp mx-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                                            </div>
                                        </td>
                                    </template>
                                    <template v-if="item.Seq == editSeqB">
                                        <td>{{index+1}}</td>
                                        <td><textarea v-model.trim="item.SiteEnvironment" rows="1" type="text" class="form-control"></textarea></td>
                                        <td><textarea v-model.trim="item.EngMemo" rows="3" type="text" class="form-control"></textarea></td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button @click="onSaveRecordB(item)" class="btn btn-color11-2 btn-xs sharp mx-1"><i class="fas fa-save"></i></button>
                                                <button @click="editSeqB = -99" class="btn btn-color9-1 btn-xs sharp mx-1" title="取消"><i class="fas fa-times"></i></button>
                                            </div>
                                        </td>
                                    </template>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td><textarea v-model.trim="newRecordB.SiteEnvironment" rows="1" type="text" class="form-control"></textarea></td>
                                    <td><textarea v-model.trim="newRecordB.EngMemo" rows="3" type="text" class="form-control"></textarea></td>
                                    <td>
                                        <div class="d-flex justify-content-center">
                                            <button @click="onNewRecordB" class="btn btn-outline-secondary btn-xs sharp mx-1" title="新增"><i class="fas fa-plus fa-lg"></i></button>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div id="menu02" class="tab-pane ">
            </div>
            <div id="menu03" class="tab-pane ">
                <div style="font-size: 1.2rem ;font-weight: bold ;clear: both ;padding: 13px 15px ;background: -webkit-linear-gradient(right, #f5bdbd, #f19999) ;color: #fff ;border-radius: 5px ;">潛在危害辨識</div>
                <div>
                    <h5>施工風險評估小組</h5>
                    <div class="table-responsive">
                        <table class="table table-responsive-md table-hover VA-middle">
                            <thead class="table table1 min910">
                                <tr class="insearch">
                                    <th style="text-align: center; width: 5%;">項次</th>
                                    <th style="text-align: left;width:18% ;">職稱</th>
                                    <th style="text-align: left;width:20% ;">機關</th>
                                    <th style="text-align: left;width:20% ;">單位</th>
                                    <th style="text-align: left;width:15% ;">姓名</th>
                                    <th>備註</th>
                                    <th style="text-align: center; width: 10%;">功能</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(item, index) in itemsC" v-bind:key="item.Seq">
                                    <template v-if="item.Seq != editSeqC">
                                        <td>{{index+1}}</td>
                                        <td>{{item.JobTitle}}</td>
                                        <td>{{item.OrganizerUnitName}}</td>
                                        <td>{{item.UnitName}}</td>
                                        <td>{{item.PrincipalName}}</td>
                                        <td>{{item.Memo}}</td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button @click="onEditRecord(3,item)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                            </div>
                                        </td>
                                    </template>
                                    <template v-if="item.Seq == editSeqC">
                                        <td>{{index+1}}</td>
                                        <td>{{item.JobTitle}}</td>
                                        <td>
                                            <select class="form-control" @change="onChangeUnit1" v-model="item.OrganizerUnitSeq">
                                                <option v-bind:key="index" v-for="(item,index) in units1" v-bind:value="item.Value">{{item.Text}}</option>
                                            </select>
                                        </td>
                                        <td>
                                            <select class="form-control" @change="onChangeUnit2" v-model="item.UnitSeq">
                                                <option v-bind:key="index" v-for="(item,index) in units2" v-bind:value="item.Value">{{item.Text}}</option>
                                            </select>
                                        </td>
                                        <td>
                                            <select class="form-control" v-model="item.PrincipalSeq">
                                                <option v-bind:key="index" v-for="(item,index) in users1" v-bind:value="item.Seq">{{item.DisplayName}}</option>
                                            </select>
                                        </td>
                                        <td><input v-model.trim="item.Memo" type="text" class="form-control"></td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button @click="onSaveRecordC(item)" class="btn btn-color11-2 btn-xs sharp mx-1"><i class="fas fa-save"></i></button>
                                                <button @click="editSeqC = -99" class="btn btn-color9-1 btn-xs sharp mx-1" title="取消"><i class="fas fa-times"></i></button>
                                            </div>
                                        </td>
                                    </template>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <h5>工程功能需求</h5>
                    <div class="table-responsive">
                        <table class="table table-responsive-md table-hover VA-middle" id="addnew051001">
                            <thead class="table table1 min910">
                                <tr class="insearch">
                                    <th style="text-align: center; width: 5%;">項次</th>
                                    <th style="text-align: left;width:15% ;">工程功能</th>
                                    <th style="text-align: left;width:20% ;">潛在危害</th>
                                    <th style="text-align: left;width:20% ;">危害對策</th>
                                    <th>對策處置人員</th>
                                    <th style="text-align: left;width:15% ;">備註</th>
                                    <th style="text-align: center; width: 10%;">功能</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(item, index) in itemsD" v-bind:key="item.Seq">
                                    <template v-if="item.Seq != editSeqD">
                                        <td>{{index+1}}</td>
                                        <td>{{item.EngFunction}}</td>
                                        <td>{{item.PotentialHazard}}</td>
                                        <td>{{item.HazardCountermeasures}}</td>
                                        <td>{{item.PrincipalName}}</td>
                                        <td>{{item.EngMemo}}</td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button @click="onEditRecord(4,item)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                            </div>
                                        </td>
                                    </template>
                                    <template v-if="item.Seq == editSeqD">
                                        <td>{{index+1}}</td>
                                        <td>{{item.EngFunction}}</td>
                                        <td><textarea v-model.trim="item.PotentialHazard" rows="2" type="text" class="form-control"></textarea></td>
                                        <td><textarea v-model.trim="item.HazardCountermeasures" rows="2" type="text" class="form-control"></textarea></td>
                                        <td>
                                            <select class="form-control" v-model="item.PrincipalSeq">
                                                <option v-bind:key="index" v-for="(item,index) in users2" v-bind:value="item.Value">{{item.Text}}</option>
                                            </select>
                                        </td>
                                        <td><textarea v-model.trim="item.EngMemo" rows="2" type="text" class="form-control"></textarea></td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button @click="onSaveRecordD(item)" class="btn btn-color11-2 btn-xs sharp mx-1"><i class="fas fa-save"></i></button>
                                                <button @click="editSeqD = -99" class="btn btn-color9-1 btn-xs sharp mx-1" title="取消"><i class="fas fa-times"></i></button>
                                            </div>
                                        </td>
                                    </template>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <h5>工址環境現況</h5>
                    <div class="table-responsive">
                        <table class="table table-responsive-md table-hover VA-middle" id="addnew051002">
                            <thead class="table table1 min910">
                                <tr class="insearch">
                                    <th style="text-align: center; width: 5%;">項次</th>
                                    <th style="text-align: left;width:15% ;">工址環境</th>
                                    <th style="text-align: left;width:20% ;">潛在危害</th>
                                    <th style="text-align: left;width:20% ;">危害對策</th>
                                    <th>對策處置人員</th>
                                    <th style="text-align: left;width:15% ;">備註</th>
                                    <th style="text-align: center; width: 10%;">功能</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(item, index) in itemsE" v-bind:key="item.Seq">
                                    <template v-if="item.Seq != editSeqE">
                                        <td>{{index+1}}</td>
                                        <td>{{item.SiteEnvironment}}</td>
                                        <td>{{item.PotentialHazard}}</td>
                                        <td>{{item.HazardCountermeasures}}</td>
                                        <td>{{item.PrincipalName}}</td>
                                        <td>{{item.EngMemo}}</td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button @click="onEditRecord(5,item)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                            </div>
                                        </td>
                                    </template>
                                    <template v-if="item.Seq == editSeqE">
                                        <td>{{index+1}}</td>
                                        <td>{{item.SiteEnvironment}}</td>
                                        <td><textarea v-model.trim="item.PotentialHazard" rows="2" type="text" class="form-control"></textarea></td>
                                        <td><textarea v-model.trim="item.HazardCountermeasures" rows="2" type="text" class="form-control"></textarea></td>
                                        <td>
                                            <select class="form-control" v-model="item.PrincipalSeq">
                                                <option v-bind:key="index" v-for="(item,index) in users2" v-bind:value="item.Value">{{item.Text}}</option>
                                            </select>
                                        </td>
                                        <td><textarea v-model.trim="item.EngMemo" rows="2" type="text" class="form-control"></textarea></td>
                                        <td>
                                            <div class="d-flex justify-content-center">
                                                <button @click="onSaveRecordE(item)" class="btn btn-color11-2 btn-xs sharp mx-1"><i class="fas fa-save"></i></button>
                                                <button @click="editSeqE = -99" class="btn btn-color9-1 btn-xs sharp mx-1" title="取消"><i class="fas fa-times"></i></button>
                                            </div>
                                        </td>
                                    </template>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>

            </div>
            <div id="menu04" class="tab-pane ">
                <h5>設計階段施工方法檢討評選表</h5>
                <div class="table-responsive">
                    <table class="table table-responsive-md table-hover VA-middle" id="addnew052301">
                        <thead class="table table1 min910">
                            <tr>
                                <td style="text-align: left;vertical-align:middle;background-color: #efeded;border-bottom-color: #fff;width: 25%;">
                                    <p>設計方案研擬背景</p>
                                </td>
                                <td style="text-align: left;vertical-align:middle;">
                                    <input type="text" class="form-control" v-model.trim="riskItem.DesignStudy">
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: left;vertical-align:middle;background-color: #efeded;border-bottom-color: #fff;">
                                    <p>優選設計方案潛在危害及施工安全衛生應注意事項</p>
                                </td>
                                <td style="text-align: left;vertical-align:middle;">
                                    <input type="text" class="form-control" v-model.trim="riskItem.DesignPrecautions">
                                </td>
                            </tr>
                        </thead>
                    </table>
                </div>
                <div class="table-responsive">
                    <table class="table table-responsive-md table-hover VA-middle" id="addnew052301">
                        <tbody class="table table1 min910">
                            <tr>
                                <th rowspan="3" style="text-align: center;vertical-align:middle;background-color: #dbdbdb;border-bottom-color: #fff;">
                                    <p>方案概述</p>
                                </th>
                                <th colspan="10" style="text-align: center;vertical-align:middle;background-color: #dbdbdb;border-bottom-color: #fff;">
                                    <p>方案項目權重</p>
                                </th>
                            </tr>
                            <tr v-for="(item, index) in itemsF" v-bind:key="item.Seq">
                                <template v-if="item.PSType==1 && item.Seq != editSeqF">
                                    <td>{{item.Weight1}}</td>
                                    <td>{{item.Weight2}}</td>
                                    <td>{{item.Weight3}}</td>
                                    <td>{{item.Weight4}}</td>
                                    <td>{{item.Weight5}}</td>
                                    <td>{{item.Weight6}}</td>
                                    <td>{{item.Weight7}}</td>
                                    <td style="width: 5%;">評分</td>
                                    <td style="width: 5%;">排序</td>
                                    <td style="width: 5%;text-align: center;display: flex;">
                                        <div class="d-flex justify-content-center">
                                            <button @click="onEditRecord(6,item)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                        </div>
                                    </td>
                                </template>
                                <template v-if="item.PSType==2 && item.Seq != editSeqF">
                                    <td>{{item.Weight1}}</td>
                                    <td>{{item.Weight2}}</td>
                                    <td>{{item.Weight3}}</td>
                                    <td>{{item.Weight4}}</td>
                                    <td>{{item.Weight5}}</td>
                                    <td>{{item.Weight6}}</td>
                                    <td>{{item.Weight7}}</td>
                                    <td style="width: 5%;"></td>
                                    <td style="width: 5%;"></td>
                                    <td style="width: 5%;text-align: center;display: flex;">
                                        <div class="d-flex justify-content-center">
                                            <button @click="onEditRecord(6,item)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                        </div>
                                    </td>
                                </template>
                                <template v-if="item.PSType>2 && item.Seq != editSeqF">
                                    <td>{{item.PlanOverview}}</td>
                                    <td>{{item.Weight1}}</td>
                                    <td>{{item.Weight2}}</td>
                                    <td>{{item.Weight3}}</td>
                                    <td>{{item.Weight4}}</td>
                                    <td>{{item.Weight5}}</td>
                                    <td>{{item.Weight6}}</td>
                                    <td>{{item.Weight7}}</td>
                                    <td style="width: 5%;">{{item.TWeight}}</td>
                                    <td style="width: 5%;">{{item.WeightSort}}</td>
                                    <td style="width: 5%;text-align: center;display: flex;">
                                        <div class="d-flex justify-content-center">
                                            <button @click="onEditRecord(6,item)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                            <button @click="onDelRecord(6,item)" class="btn btn-color9-1 btn-xs sharp mx-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                                        </div>
                                    </td>
                                </template>
                                <template v-if="item.PSType==1 && item.Seq == editSeqF">
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight1"></td>
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight2"></td>
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight3"></td>
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight4"></td>
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight5"></td>
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight6"></td>
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight7"></td>
                                    <td style="width: 5%;">評分</td>
                                    <td style="width: 5%;">排序</td>
                                    <td style="width: 5%;text-align: center;display: flex;">
                                        <div class="d-flex justify-content-center">
                                            <button @click="onSaveRecordF(item)" class="btn btn-color11-2 btn-xs sharp mx-1"><i class="fas fa-save"></i></button>
                                            <button @click="editSeqF = -99" class="btn btn-color9-1 btn-xs sharp mx-1" title="取消"><i class="fas fa-times"></i></button>
                                        </div>
                                    </td>
                                </template>
                                <template v-if="item.PSType==2 && item.Seq == editSeqF">
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight1"></td>
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight2"></td>
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight3"></td>
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight4"></td>
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight5"></td>
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight6"></td>
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight7"></td>
                                    <td style="width: 5%;"></td>
                                    <td style="width: 5%;"></td>
                                    <td style="width: 5%;text-align: center;display: flex;">
                                        <div class="d-flex justify-content-center">
                                            <button @click="onSaveRecordF(item)" class="btn btn-color11-2 btn-xs sharp mx-1"><i class="fas fa-save"></i></button>
                                            <button @click="editSeqF = -99" class="btn btn-color9-1 btn-xs sharp mx-1" title="取消"><i class="fas fa-times"></i></button>
                                        </div>
                                    </td>
                                </template>
                                <template v-if="item.PSType>2 && item.Seq == editSeqF">
                                    <td style="text-align: left;width: 20%;"><input type="text" class="form-control" v-model.trim="item.PlanOverview"></td>
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight1" @keyup="setITWeight(index,item)"></td>
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight2" @keyup="setITWeight(index,item)"></td>
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight3" @keyup="setITWeight(index,item)"></td>
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight4" @keyup="setITWeight(index,item)"></td>
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight5" @keyup="setITWeight(index,item)"></td>
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight6" @keyup="setITWeight(index,item)"></td>
                                    <td><input type="text" class="form-control" v-model.trim="item.Weight7" @keyup="setITWeight(index,item)"></td>
                                    <td style="width: 5%;">{{item.TWeight}}</td>
                                    <td style="width: 5%;">{{item.WeightSort}}</td>
                                    <td style="width: 5%;text-align: center;display: flex;">
                                        <div class="d-flex justify-content-center">
                                            <button @click="onSaveRecordF(item)" class="btn btn-color11-2 btn-xs sharp mx-1"><i class="fas fa-save"></i></button>
                                            <button @click="editSeqF = -99" class="btn btn-color9-1 btn-xs sharp mx-1" title="取消"><i class="fas fa-times"></i></button>
                                        </div>
                                    </td>
                                </template>
                            </tr>
                            <tr>
                                <td style="text-align: left;width: 20%;"><input type="text" class="form-control" v-model.trim="newRecordF.PlanOverview"></td>
                                <td><input type="text" class="form-control" v-model.trim="newRecordF.Weight1" @keyup="setTWeight()"></td>
                                <td><input type="text" class="form-control" v-model.trim="newRecordF.Weight2" @keyup="setTWeight()"></td>
                                <td><input type="text" class="form-control" v-model.trim="newRecordF.Weight3" @keyup="setTWeight()"></td>
                                <td><input type="text" class="form-control" v-model.trim="newRecordF.Weight4" @keyup="setTWeight()"></td>
                                <td><input type="text" class="form-control" v-model.trim="newRecordF.Weight5" @keyup="setTWeight()"></td>
                                <td><input type="text" class="form-control" v-model.trim="newRecordF.Weight6" @keyup="setTWeight()"></td>
                                <td><input type="text" class="form-control" v-model.trim="newRecordF.Weight7" @keyup="setTWeight()"></td>
                                <td style="width: 5%;">{{newRecordF.TWeight}}</td>
                                <td style="width: 5%;"></td>
                                <td style="width: 5%;text-align: center;display: flex;">
                                    <div class="d-flex justify-content-center">
                                        <button @click="onNewRecordF" class="btn btn-outline-secondary btn-xs sharp mx-1" title="新增"><i class="fas fa-plus fa-lg"></i></button>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <h5>設計方案評選</h5>
                <editor v-model="riskItem.DesignSelection" :init="init"></editor>
                <div class="table-responsive" style="width: fit-content;">
                    <table class="table table-responsive-md table-hover VA-middle">
                        <thead class="table table1 min910">
                            <tr class="insearch">
                                <th style="text-align: left; vertical-align: middle; width: 30%;">設計方案評選</th>
                                <td style="display: flex;">
                                    <label class="btn btn-shadow btn-color11-3">
                                        <input v-on:change="fileChangeF($event,1)" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>上傳
                                    </label>
                                </td>
                                <td><p data-v-04c7975c="" style="color: red;">格式說明： .jpg、.jpeg、.png</p></td>
                            </tr>
                        </thead>
                    </table>
                </div>
                <div class="table-responsive">
                    <table class="table table1" id="addnew000">
                        <thead class="insearch">
                            <tr>
                                <th style="width: 50px;"><strong>項次</strong></th>
                                <th><strong>檔案名稱</strong></th>
                                <th style="text-align: center; width: 200px;">
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in itemsF1" v-bind:key="item.Seq">
                                <td>{{index+1}}</td>
                                <td>
                                    <div>{{item.FileName}}</div>
                                </td>
                                <td style="min-width: 105px;">
                                    <button @click="downloadF(item)" class="btn btn-color11-1 btn-xs sharp mx-1" title="下載"><i class="fas fa-download"></i>下載</button>
                                    <a href="#" v-on:click.stop="onDelF(index, item.Seq,1)" class="btn btn-color9-1 btn-xs mx-1" title="刪除"><i class="fas fa-trash-alt"></i> 刪除</a>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div id="menu05" class="tab-pane ">
                <h5>施工風險內容及施作順序</h5>
                <div v-show="title=='menu05'" class="table-responsive">
                    <table border="0" class="table table1 min910" id="addnew054">
                        <thead>
                            <tr>
                                <th class="sort">排序</th>
                                <th class="number">excel編號</th>
                                <th>分項工程名稱</th>
                                <th>建立日期</th>
                                <th>更新日期</th>
                                <th>功能</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in itemsG" v-bind:key="item.Seq">
                                <td>{{index+1}}</td>
                                <td>
                                    <div v-if="editIndex  != index ">{{ item.ExcelNo }}</div>
                                    <input v-else class="form-control" v-model="item.ExcelNo" />
                                </td>
                                <td>
                                    <div v-if="editIndex  != index ">{{ item.SubProjectName }}</div>
                                    <input v-else class="form-control" v-model="item.SubProjectName" />
                                </td>
                                <td> {{ computedDate(item, 'CreateTime') }}</td>
                                <td> {{ computedDate(item, 'ModifyTime') }}</td>
                                <td>
                                    <div class="row justify-content-center m-0">
                                        <a @click="editIndex = index"
                                           v-if="editIndex != index"
                                           href="javascript:void(0)" class="btn btn-color11-2 btn-xs m-1" title="編輯" id="edit-btn">
                                            <i class="fas fa-pencil-alt"></i> 編輯
                                        </a>
                                        <a @click="updateEngRiskSubProject(item)"
                                           v-if="editIndex == index"
                                           href="javascript:void(0)" class="btn btn-color11-1 btn-xs m-1" title="儲存" id="save-btn">
                                            <i class="fas fa-save"></i> 儲存
                                        </a>
                                        <a @click="deleteEngRiskSubProject(item)" v-if="editIndex != index"
                                           href="#" title="刪除" class="btn btn-color9-1 btn-xs m-1">
                                            <i class="fas fa-trash-alt"></i> 刪除
                                        </a>
                                        <div class="row justify-content-center m-0" v-if="editIndex != index">
                                            <a title="工程拆解" class="btn btn-color11-3 btn-xs m-1" @click="editSubProject = item; title = 2;">
                                                <i class="fas fa-plus"></i> 工程拆解
                                            </a>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div>#new </div>
                                </td>
                                <td>
                                    <input class="form-control" v-model="insertSubProject.ExcelNo" />
                                </td>
                                <td>
                                    <input class="form-control" v-model="insertSubProject.SubProjectName" />
                                </td>
                                <td> </td>
                                <td> </td>
                                <td>
                                    <a @click="insertEngRiskSubProject()"
                                       href="javascript:void(0)" class="btn btn-color11-1 btn-xs m-1" title="儲存" id="save-btn">
                                        <i class="fas fa-save"></i> 儲存
                                    </a>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <ConstructionTree v-if="title==2" @goBack="(jsonStr) => {title= 'menu05'; editSubProject.SubProjectJson = jsonStr;}" :subProject="editSubProject"></ConstructionTree>

                <div id="sample" style="display: none;">
                    <div id="myDiagramDiv" style="border: 1px solid black; width: 100%; height: 350px; position: relative; -webkit-tap-highlight-color: rgba(255, 255, 255, 0); cursor: auto; font: 12px sans-serif;">
                        <canvas tabindex="0" width="1589" height="498" style="position: absolute; top: 0px; left: 0px; z-index: 2; user-select: none; touch-action: none; width: 1589px; height: 498px; cursor: auto;">This text is displayed if your browser does not support the Canvas HTML element.</canvas>
                        <div style="position: absolute; overflow: auto; width: 1589px; height: 498px; z-index: 1;font-size: larger;">
                            <div style="position: absolute; width: 1px; height: 1px;"></div>
                        </div>
                    </div>
                    <div style="display: flex;padding: 10px;" class="row">
                        <button id="SaveButton" onclick="save()" class="btn btn-color11-2 btn-sm mx-1" disabled="">儲存</button>
                        <button onclick="layout()" class="btn btn-color11-3 btn-sm mx-1">自動排版</button>
                    </div>
                </div>
            </div>
            <div id="menu06" class="tab-pane ">
                <h5>施工風險評估</h5>
                <div v-show="title=='menu06'" class="table-responsive">
                    <table border="0" class="table table1 min910" id="addnew054">
                        <thead>
                            <tr>
                                <th class="sort">排序</th>
                                <th class="number">excel編號</th>
                                <th>分項工程名稱</th>
                                <th>建立日期</th>
                                <th>更新日期</th>
                                <th>功能</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in itemsG" v-bind:key="item.Seq">
                                <td>{{index+1}}</td>
                                <td>{{item.ExcelNo}}</td>
                                <td>{{item.SubProjectName}}</td>
                                <td>{{item.CreateTime}}</td>
                                <td>{{item.ModifyTime}}</td>
                                <td>
                                    <div class="row justify-content-center m-0">
                                        <a title="風險評估" class="btn btn-color11-3 btn-xs m-1" @click="editSubProject = item; title = 1;">
                                            <i class="fas fa-eye"></i>風險評估
                                        </a>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>

                </div>
                <Lookup v-if="title == 1" @goBack="title= 'menu06'" :subProject="editSubProject"></Lookup>

            </div>
            <div id="menu07" class="tab-pane ">
                <h5>設計階段施工風險評估成果之運用</h5>
                <editor v-model="riskItem.DesignStageRiskResult" :init="init"></editor>
                <div class="table-responsive" style="width: fit-content;">
                    <table class="table table-responsive-md table-hover VA-middle">
                        <thead class="table table1 min910">
                            <tr class="insearch">
                                <th style="text-align: left; vertical-align: middle; width: 30%;">設計階段施工風險評估成果之運用</th>
                                <td style="display: flex;">
                                    <label class="btn btn-shadow btn-color11-3">
                                        <input v-on:change="fileChangeF($event,2)" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>上傳
                                    </label>
                                </td>
                                <td><p data-v-04c7975c="" style="color: red;">格式說明： .jpg、.jpeg、.png</p></td>
                            </tr>
                        </thead>
                    </table>
                </div>
                <div class="table-responsive">
                    <table class="table table1" id="addnew000">
                        <thead class="insearch">
                            <tr>
                                <th style="width: 50px;"><strong>項次</strong></th>
                                <th><strong>檔案名稱</strong></th>
                                <th style="text-align: center; width: 200px;">
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in itemsF2" v-bind:key="item.Seq">
                                <td>{{index+1}}</td>
                                <td>
                                    <div>{{item.FileName}}</div>
                                </td>
                                <td style="min-width: 105px;">
                                    <button @click="downloadF(item)" class="btn btn-color11-1 btn-xs sharp mx-1" title="下載"><i class="fas fa-download"></i>下載</button>
                                    <a href="#" v-on:click.stop="onDelF(index, item.Seq,2)" class="btn btn-color9-1 btn-xs mx-1" title="刪除"><i class="fas fa-trash-alt"></i> 刪除</a>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div id="menu08" class="tab-pane ">
                <h5>風險資訊傳遞及風險追蹤管理</h5>
                <editor v-model="riskItem.RiskTracking" :init="init"></editor>
                <div class="table-responsive" style="width: fit-content;">
                    <table class="table table-responsive-md table-hover VA-middle">
                        <thead class="table table1 min910">
                            <tr class="insearch">
                                ft
                                <th style="text-align: left; vertical-align: middle; width: 30%;">風險資訊傳遞及風險追蹤管理</th>
                                <td style="display: flex;">
                                    <label class="btn btn-shadow btn-color11-3">
                                        <input v-on:change="fileChangeF($event,3)" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>上傳
                                    </label>
                                </td>
                                <td><p data-v-04c7975c="" style="color: red;">格式說明： .jpg、.jpeg、.png</p></td>
                            </tr>
                        </thead>
                    </table>
                </div>
                <div class="table-responsive">
                    <table class="table table1" id="addnew000">
                        <thead class="insearch">
                            <tr>
                                <th style="width: 50px;"><strong>項次</strong></th>
                                <th><strong>檔案名稱</strong></th>
                                <th style="text-align: center; width: 200px;">
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in itemsF3" v-bind:key="item.Seq">
                                <td>{{index+1}}</td>
                                <td>{{item.FileName}}</td>
                                <td style="min-width: 105px;">
                                    <button @click="downloadF(item)" class="btn btn-color11-1 btn-xs sharp mx-1" title="下載"><i class="fas fa-download"></i>下載</button>
                                    <a href="#" v-on:click.stop="onDelF(index, item.Seq,3)" class="btn btn-color9-1 btn-xs mx-1" title="刪除"><i class="fas fa-trash-alt"></i> 刪除</a>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div id="menu09" class="tab-pane ">
                <h5>結論</h5>
                <editor v-model="riskItem.Conclusion" :init="init"></editor>
                <div class="table-responsive" style="width: fit-content;">
                    <table class="table table-responsive-md table-hover VA-middle">
                        <thead class="table table1 min910">
                            <tr class="insearch">
                                <th style="text-align: left; vertical-align: middle; width: 10%;">結論</th>
                                <td style="display: flex;">
                                    <label class="btn btn-shadow btn-color11-3">
                                        <input v-on:change="fileChangeF($event,4)" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>上傳
                                    </label>
                                </td>
                                <td><p data-v-04c7975c="" style="color: red;">格式說明： .jpg、.jpeg、.png</p></td>
                            </tr>
                        </thead>
                    </table>
                </div>
                <div class="table-responsive">
                    <table class="table table1" id="addnew000">
                        <thead class="insearch">
                            <tr>
                                <th style="width: 50px;"><strong>項次</strong></th>
                                <th><strong>檔案名稱</strong></th>
                                <th style="text-align: center; width: 200px;">
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in itemsF4" v-bind:key="item.Seq">
                                <td>{{index+1}}</td>
                                <td>{{item.FileName}}</td>
                                <td style="min-width: 105px;">
                                    <button @click="downloadF(item)" class="btn btn-color11-1 btn-xs sharp mx-1" title="下載"><i class="fas fa-download"></i>下載</button>
                                    <a href="#" v-on:click.stop="onDelF(index, item.Seq,4)" class="btn btn-color9-1 btn-xs mx-1" title="刪除"><i class="fas fa-trash-alt"></i> 刪除</a>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <br>
            <div class="row justify-content-center">
                <div v-if="riskItem.IsFinish == 0&&riskItem.LockState==0" class="col-12 col-sm-4 col-lg-2 col-xl-2 mt-2">
                    <button v-on:click.stop="onSaveRiskItem()" role="button" class="btn btn-color11-2 btn-block">
                        <i class="fas fa-save">&nbsp;暫存</i>
                    </button>
                </div>
                <div v-if="downloadStatus != -1&&riskItem.LockState==0" class="col-12 col-sm-4 col-lg-2 col-xl-2 mt-2">
                    <button v-on:click.stop="onSaveRiskItemFinish()" role="button" class="btn btn-color11-2 btn-block">
                        <i class="fas fa-upload"></i>&nbsp;資料填寫完成
                    </button>
                </div>
                <div v-if="riskItem.IsFinish == 1&&riskItem.LockState==1" class="col-12 col-sm-4 col-lg-2 col-xl-2 mt-2">
                    <button v-if="downloadStatus >= 0" role="button" class="btn btn-shadow btn-block btn-color2" style="width: 200px;" @click="createDocument()"> 產製施工風險評估報告</button>
                </div>
                <div v-if="riskItem.IsFinish == 1&&riskItem.LockState==2" class="col-12 col-sm-4 col-lg-2 col-xl-2 mt-2">
                    <!-- <button v-if="downloadStatus == -1" disalbed role="button" class="btn btn-shadow btn-block bg-gray" >   產製施工風險評估報告 </button> -->
                    <button v-if="downloadStatus < 2" role="button" class="btn btn-shadow btn-block bg-gray" disalbed>
                        <i class="fas fa-upload"></i>&nbsp;文件產製中...
                    </button>
                    <a v-if="downloadStatus >= 2" :href="`EngRiskFront/DownloadDocument?id=${targetId}`" download>
                        <button title="下載" class="btn btn-color11-1 btn-block" style="width: 200px;"><i class="fas fa-download"></i>下載word</button>
                    </a>
                </div>
            </div>
            <br />
            <p data-v-04c7975c="" style="color: red;">* 產製施工風險評估報告按鈕點選後，系統會背景處理，使用者無須等待，可於3-5方鐘後再點選此功能確認。</p>
            <p data-v-04c7975c="" style="color: red;">* 評估報告產製完成後，於word中請先點選ctrl+A更新功能變數。</p>
        </div>
    </div>
</template>
<script>
    import ConstructionTree from './ConstructionTree.vue';
    import Lookup from "./lookup.vue";
    import Common2 from "../Common/Common2";

    import tinymce from 'tinymce/tinymce.js'
    // 外觀
    import 'tinymce/skins/ui/oxide/skin.css'
    import 'tinymce/themes/silver'
    // Icon
    import 'tinymce/icons/default'
    // 用到的外掛
    import 'tinymce/plugins/emoticons'
    import 'tinymce/plugins/emoticons/js/emojis.js'
    import 'tinymce/plugins/table'
    import 'tinymce/plugins/quickbars'
    // 語言包
    import 'tinymce-i18n/langs5/zh_TW.js'
    // TinyMCE-Vue
    import Editor from '@tinymce/tinymce-vue'

    export default {
        components: {
            ConstructionTree,
            Lookup,
            Editor,
        },
        data: function () {
            return {
                init: {
                    language: 'zh_TW',
                    height: 500,
                    menubar: false,
                    content_css: false,
                    skin: false,
                    /*toolbar_mode: "wrap",*/
                    //valid_elements:
                    //    "p[*],span[*],div[*],input[*],ul,ol,li,strong/b,em,h1,h2,h3,h4,h5,h6,table[*],tbody[*],tr[*],td[*],img[*],video[*]",
                    //content_style: "p {margin: 5px 0;}",
                    plugins: 'table ',
                    toolbar: 'undo redo restoredraft | \
                        forecolor backcolor bold italic underline strikethrough | \
                        alignleft aligncenter alignright alignjustify outdent indent | \
                        formatselect fontselect fontsizeselect | table ',
                    quickbars_insert_toolbar: false,
                    branding: false,
                    fontsize_formats: "12px 14px 16px 18px 20px 22px 24px 28px 32px 36px 48px 56px 72px",
                    font_formats: "新細明體=新細明體;標楷體=標楷體;",
                    //setup: function (editor) { //預設值
                    //    editor.on('init', function (e) {
                    //        this.getBody().style.fontSize = '12px';
                    //        this.getBody().style.color = '#000';
                    //        this.getBody().style.fontFamily = '新細明體';
                    //    });
                    //},
                },

                interPointA: 'destruct',
                interPointB: 'lookup',
                title: null,
                editSubProject: {},

                targetId: null,
                tenderItem: {},
                riskItem: { ConclusionFileName:''},
                itemsA: [],
                itemsB: [],
                itemsC: [],
                itemsD: [],
                itemsE: [],
                itemsF: [],
                itemsG: [],
                itemsH: [],

                // 單位下拉-第一層
                units1: [],
                // 單位下拉-第二層
                units2: [],
                // 使用者下拉
                users1: [],
                users2: [],

                editSeqA: -99,
                newRecordA: { Seq: -1, EngRiskFrontSeq: 0, EngFunction: null, PotentialHazard: null, HazardCountermeasures: null, PrincipalSeq: null, EngMemo: null },

                editSeqB: -99,
                newRecordB: { Seq: -1, EngRiskFrontSeq: 0, SiteEnvironment: null, PotentialHazard: null, HazardCountermeasures: null, PrincipalSeq: null, EngMemo: null },

                editSeqC: -99,
                editSeqD: -99,
                editSeqE: -99,

                editSeqF: -99,
                newRecordF: {
                    Seq: -1, EngRiskFrontSeq: 0, PSType: 3, PlanOverview: null
                    , Weight1: null, Weight2: null, Weight3: null, Weight4: null, Weight5: null, Weight6: null, Weight7: null, WeightSort: 0, TWeight: 0
                    , NWeight1: 0, NWeight2: 0, NWeight3: 0, NWeight4: 0, NWeight5: 0, NWeight6: 0, NWeight7: 0
                },

                editSeqG: -99,
                editSeqH: -99,
                downloadStatus: 0,

                itemsF1: [], itemsF2: [], itemsF3: [], itemsF4: [],
                newFileDS: null,
                editFileDS: null,

                insertSubProject: {},
                //editSubProject: null,
                editIndex: null,

                userUnit: null,
                userUnitSub: null,
                userName: null,
                unitName: null,
                unitSubName: null,
            }
        },
        methods: {
            unlockItem(lockState) {
                window.myAjax.post('/EngRiskFront/UnLock', { Seq: this.targetId, LockState: lockState })
                    .then(resp => {
                        this.getItem();
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            emptyEditor() {
                this.riskItem.Conclusion = 'AAA';
            },
            onBlur(editor) {
                console.log(editor);
                console.log(editor.getData());
            },
            onFocus(editor) {
                console.log(editor);
            },
            onCKChange(editor) {
                console.log(editor);
                console.log(this.riskItem.Conclusion);
            },
            async createDocument()
            {
               let {data }  = await window.myAjax.post("/EngRiskFront/CreateDocument", { id :this.targetId});
                if(data.status > 0)
                {
                    this.unlockItem(2);
                    alert(data.message);
                    this.downloadStatus = -1;
                }
            },
            async checkDocument()
            {
               let { data }  = await window.myAjax.post("/EngRiskFront/CheckDocument", { id :this.targetId});
               this.downloadStatus = data;

            },
            async downloadDocument(action)
            {
                try{
                

                    Common2.download3(action);
                }
                catch
                {
                    alert("下載失敗");
                }

            },
            computedDate(item, col) {
                var date = Common2.ToDate(item[col]);
                item[col] = date;
                return date;
            },
            //取標案
            getItem() {
                if (this.targetId == null) {
                    alert('請先選取標案');
                    return;
                }
                window.myAjax.post('/EngRiskFront/GetEngMain', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.tenderItem = resp.data.item;
                            this.getEngRiskItem();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //取施工風險
            getEngRiskItem() {
                window.myAjax.post('/EngRiskFront/GetEngRiskMain', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.riskItem = resp.data.item;
                            console.log(this.riskItem.LockState);
                            this.getSubList(1);
                            this.getSubList(2);
                            this.getSubList(3);
                            this.getSubList(4);
                            this.getSubList(5);
                            this.getSubList(6);
                            this.getSubList(7);
                            this.getSubList(8);
                            this.getUserPrincipalList();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //取施工風險
            getEngRiskItemUploadFile() {
                window.myAjax.post('/EngRiskFront/GetEngRiskMain', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.riskItem.PlanScopeFileName = resp.data.item.PlanScopeFileName;
                            this.riskItem.DesignConceptFileName = resp.data.item.DesignConceptFileName;
                            this.riskItem.DesignSelectionFileName = resp.data.item.DesignSelectionFileName;
                            this.riskItem.DesignStageRiskResultFileName = resp.data.item.DesignStageRiskResultFileName;
                            this.riskItem.RiskTrackingFileName = resp.data.item.RiskTrackingFileName;
                            this.riskItem.ConclusionFileName = resp.data.item.ConclusionFileName;
                            this.riskItem.FinishFileName = resp.data.item.FinishFileName;
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //儲存施工風險
            onSaveRiskItem() {
                //var data = CKEditor4.instances.ckeditor41.getData();
                //console.log(data);
                window.myAjax.post('/EngRiskFront/UpdateEngRiskMain', { m: this.riskItem })
                    .then(resp => {
                        this.saveFlag = false;
                        //if (resp.data.result == 0) {
                        //    alert(resp.data.msg);
                        //} else {
                        //    alert(resp.data.msg);
                        //}
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //儲存施工風險
            onSaveRiskItemFinish() {
                this.riskItem.IsFinish = 1;
                this.onSaveRiskItem();
                this.unlockItem(1);
            },
            //上傳-檔案異動事件
            fileChange(event, type) {
                var files = event.target.files || event.dataTransfer.files;
                // 預防檔案為空檔
                if (!files.length) return;
                var uploadfiles = new FormData();
                uploadfiles.append("file", files[0], files[0].name);
                uploadfiles.append("Seq", this.targetId);
                uploadfiles.append("fileType", type);
                this.upload(uploadfiles);
            },
            //上傳
            upload(uploadfiles) {
                window.myAjax.post('/EngRiskFront/UploadAttachment', uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            this.getEngRiskItemUploadFile();
                        }
                        //alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
            //下載
            onDownload(fileNo) {
                window.myAjax.get('/EngRiskFront/Download?id=' + this.targetId + '&fileNo=' + fileNo, { responseType: 'blob' })
                    .then(resp => {
                        const blob = new Blob([resp.data]);
                        const contentType = resp.headers['content-type'];
                        if (contentType.indexOf('application/json') >= 0) {
                            //alert(resp.data);
                            const reader = new FileReader();
                            reader.addEventListener('loadend', (e) => {
                                const text = e.srcElement.result;
                                const data = JSON.parse(text)
                                alert(data.message);
                            });
                            reader.readAsText(blob);
                        } else if (contentType.indexOf('application/blob') >= 0) {
                            var saveFilename = null;
                            const data = decodeURI(resp.headers['content-disposition']);
                            var array = data.split("filename*=UTF-8''");
                            if (array.length == 2) {
                                saveFilename = array[1];
                            } else {
                                array = data.split("filename=");
                                saveFilename = array[1];
                            }
                            if (saveFilename != null) {
                                const url = window.URL.createObjectURL(blob);
                                const link = document.createElement('a');
                                link.href = url;
                                link.setAttribute('download', saveFilename);
                                document.body.appendChild(link);
                                link.click();
                            } else {
                                console.log('saveFilename is null');
                            }
                        } else {
                            alert('格式錯誤下載失敗');
                        }
                    }).catch(error => {
                        console.log(error);
                    });
            },
            strEmpty(str) {
                return window.comm.stringEmpty(str);
            },
            //=============================
            //取得使用者單位資訊
            getUserUnit() {
                window.myAjax.post('/EngReport/GetUserUnit')
                    .then(resp => {
                        this.userUnit = resp.data.unit;
                        this.userUnitSub = resp.data.unitSub;
                        this.userName = resp.data.userName;
                        this.unitName = resp.data.unitName;
                        this.unitSubName = resp.data.unitSubName;

                        if (sessionStorage.getItem('selectERUnit') == null) {
                            this.selectUnit = this.userUnit;
                            this.onUnitChange(this.selectUnit);
                        } else {
                            this.GetSession();
                        }

                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            // 取得單位下拉選單1
            getUnitList1() {
                const self = this;
                let params = { id: this.targetId };
                window.myAjax.post('/Unit/GetUnitListForRisk', params)
                    .then(resp => {
                        self.units1 = resp.data;
                        console.log(self.units1[0].Value);
                        console.log(self.userUnit);
                        //self.unitSeq1 = self.units1[0].Value;
                        let obj = new Object();
                        obj.target = new Object();
                        //obj.target.value = self.unitSeq1;
                        //obj.target.value = self.userUnit;
                        obj.target.value = self.units1[0].Value;
                        self.onChangeUnit1(obj);
                    })
                    .then(err => {
                        //console.log(err);
                    });
            },
            // 取得單位下拉選單2
            getUnitList2(unitSeq = 0) {
                console.log(unitSeq);
                const self = this;
                let params = { parentSeq: unitSeq };
                window.myAjax.post('/Unit/GetUnitList', params)
                    .then(resp => {
                        self.units2 = resp.data;
                        //self.units2.splice(0, 0, { Text: '----', Value: '0' });
                        let obj = new Object();
                        obj.target = new Object();
                        //obj.target.value = self.unitSeq1;
                        //obj.target.value = self.userUnitSub;
                        obj.target.value = self.units2[0].Value;
                        self.onChangeUnit2(obj);
                    })
                    .then(err => {
                        //console.log(err);
                    });
            },
            // 選擇單位第一層
            onChangeUnit1(value) {
                const self = this;
                self.getUnitList2(value.target.value);
            },
            // 選擇單位第二層
            onChangeUnit2(value) {
                console.log(value.target.value);
                const self = this;
                self.getUserList(value.target.value);
            },
            // 取得人員列表
            getUserList(unitSeq = 0) {
                console.log(unitSeq);
                const self = this;
                if (unitSeq != null) {
                    let params = { page: 0, per_page: 0, unitSeq: unitSeq, nameSearch: '' };
                    window.myAjax.post('/EngRiskFront/GetUserList', params)
                        .then(resp => {
                            this.users1 = resp.data.l;
                        })
                        .then(err => {
                            //console.log(err);
                        });
                }
            },
            //=============================
            getSubList(type) {
                window.myAjax.post('/EngRiskFront/GetSubList', { Type: type, Seq: this.riskItem.Seq })
                    .then(resp => {
                        if (type == 1) { this.itemsA = resp.data.items; this.editSeqA = -99; }
                        else if (type == 2) { this.itemsB = resp.data.items; this.editSeqB = -99; }
                        else if (type == 3) {
                            this.itemsC = resp.data.items;
                            this.editSeqC = -99;
                        }
                        else if (type == 4) { this.itemsD = resp.data.items; this.editSeqD = -99; }
                        else if (type == 5) { this.itemsE = resp.data.items; this.editSeqE = -99; }
                        else if (type == 6) {
                            this.itemsF = resp.data.items;
                            resp.data.items.forEach((element) => {
                                if (element.PSType == 2) {
                                    this.newRecordF.NWeight1 = element.Weight1.replace('%', '');
                                    this.newRecordF.NWeight2 = element.Weight2.replace('%', '');
                                    this.newRecordF.NWeight3 = element.Weight3.replace('%', '');
                                    this.newRecordF.NWeight4 = element.Weight4.replace('%', '');
                                    this.newRecordF.NWeight5 = element.Weight5.replace('%', '');
                                    this.newRecordF.NWeight6 = element.Weight6.replace('%', '');
                                    this.newRecordF.NWeight7 = element.Weight7.replace('%', '');
                                }
                            });
                        } 
                        else if (type == 7) this.itemsG = resp.data.items;
                        else if (type == 8) this.itemsH = resp.data.items;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //編輯紀錄
            onEditRecord(type, item) {
                console.log('type:' + type);
                console.log('Seq:' + item.Seq);
                console.log('editSeqD:' + this.editSeqD);
                if (type == 1) { if (this.editSeqA > -99) return; this.editSeqA = item.Seq; }
                else if (type == 2) { if (this.editSeqB > -99) return; this.editSeqB = item.Seq; }
                else if (type == 3) { if (this.editSeqC > -99) return; this.editSeqC = item.Seq; }
                else if (type == 4) { if (this.editSeqD > -99) return; this.editSeqD = item.Seq; }
                else if (type == 5) { if (this.editSeqE > -99) return; this.editSeqE = item.Seq; }
                else if (type == 6) { if (this.editSeqF > -99) return; this.editSeqF = item.Seq; }
            },
            //刪除紀錄
            onDelRecord(type, item) {
                if (confirm('是否確定刪除?')) {
                    //if (type == 1) { if (this.editSeqA > -99) return; this.editSeqA = item.Seq; }
                    //else if (type == 2) { if (this.editSeqB > -99) return; this.editSeqB = item.Seq; }
                    //else if (type == 3) { if (this.editSeqC > -99) return; this.editSeqC = item.Seq; }
                    //else if (type == 4) { if (this.editSeqD > -99) return;this.editSeqD = item.Seq;  }
                    //else if (type == 5) { if (this.editSeqE > -99) return; this.editSeqE = item.Seq;  }
                    //else if (type == 6) { if (this.editSeqF > -99) return; this.editSeqF = item.Seq; }

                    window.myAjax.post('/EngRiskFront/DelRecord', { Type: type, id: item.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                if (type == 1) this.getSubList(1);
                                else if (type == 2) this.getSubList(2);
                                else if (type == 3) this.getSubList(3);
                                else if (type == 4) this.getSubList(4);
                                else if (type == 5) this.getSubList(5);
                                else if (type == 6) this.getSubList(6);
                            }
                            //alert(resp.data.msg);
                        }).catch(error => {
                            console.log(error);
                        });
                }
            },
            //對策處置人員
            getUserPrincipalList() {
                window.myAjax.post('/EngRiskFront/GetUserPrincipalList', { Seq: this.riskItem.Seq })
                    .then(resp => {
                        console.log(resp.data);
                        this.users2 = resp.data;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },

            //=== 工程計畫概要 - 工程功能需求
            //新增
            onNewRecordA() {
                if (this.strEmpty(this.newRecordA.EngFunction) || this.strEmpty(this.newRecordA.EngMemo)) {
                    alert('工程功能, 說明 必須輸入!');
                    return;
                }
                this.newRecordA.EngRiskFrontSeq = this.riskItem.Seq;
                window.myAjax.post('/EngRiskFront/UpdateRecordA', { m: this.newRecordA })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.editSeqA = -99;
                            this.newRecordA.EngFunction = '';
                            this.newRecordA.EngMemo = '';
                            this.getSubList(1);
                            this.getSubList(4);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //儲存
            onSaveRecordA(uItem) {
                //console.log(uItem);
                if (this.strEmpty(uItem.EngFunction) || this.strEmpty(uItem.EngMemo)) {
                    alert('工程功能, 說明 必須輸入!');
                    return;
                }
                window.myAjax.post('/EngRiskFront/UpdateRecordA', { m: uItem })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.editSeqA = -99;
                            this.getSubList(1);
                            this.getSubList(4);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },

            //=== 工程計畫概要 - 工址環境現況
            //新增
            onNewRecordB() {
                if (this.strEmpty(this.newRecordB.SiteEnvironment) || this.strEmpty(this.newRecordB.EngMemo)) {
                    alert('工址環境, 說明 必須輸入!');
                    return;
                }
                this.newRecordB.EngRiskFrontSeq = this.riskItem.Seq;
                window.myAjax.post('/EngRiskFront/UpdateRecordB', { m: this.newRecordB })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.editSeqB = -99;
                            this.newRecordB.SiteEnvironment = '';
                            this.newRecordB.EngMemo = '';
                            this.getSubList(2);
                            this.getSubList(5);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //儲存
            onSaveRecordB(uItem) {
                //console.log(uItem);
                if (this.strEmpty(uItem.SiteEnvironment) || this.strEmpty(uItem.EngMemo)) {
                    alert('工址環境, 說明 必須輸入!');
                    return;
                }
                window.myAjax.post('/EngRiskFront/UpdateRecordB', { m: uItem })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.editSeqB = -99;
                            this.getSubList(2);
                            this.getSubList(5);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },

            //=== 準備作業 - 施工風險評估小組
            //儲存
            onSaveRecordC(uItem) {
                //console.log(uItem);
                if (uItem.OrganizerUnitSeq == null
                    || uItem.UnitSeq == null
                    || uItem.PrincipalSeq == null
                ) {
                    alert('機關、單位、姓名 必須輸入!');
                    return;
                }
                window.myAjax.post('/EngRiskFront/UpdateRecordC', { m: uItem })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.editSeqC = -99;
                            this.getSubList(3);
                            this.getUserPrincipalList();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },

            //=== 準備作業 - 工程功能需求
            //儲存
            onSaveRecordD(uItem) {
                //console.log(uItem);
                if (this.strEmpty(uItem.EngFunction)
                    || this.strEmpty(uItem.PotentialHazard)
                    || this.strEmpty(uItem.HazardCountermeasures)
                    || uItem.PrincipalSeq == null
                ) {
                    alert('工址環境、潛在危害、危害對策、對策處置人員 必須輸入!');
                    return;
                }
                window.myAjax.post('/EngRiskFront/UpdateRecordD', { m: uItem })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.editSeqD = -99;
                            this.getSubList(4);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },

            //=== 準備作業 - 工址環境現況
            //儲存
            onSaveRecordE(uItem) {
                //console.log(uItem);
                if (this.strEmpty(uItem.SiteEnvironment)
                    || this.strEmpty(uItem.PotentialHazard)
                    || this.strEmpty(uItem.HazardCountermeasures)
                    || uItem.PrincipalSeq == null
                ) {
                    alert('工址環境、潛在危害、危害對策、對策處置人員 必須輸入!');
                    return;
                }
                window.myAjax.post('/EngRiskFront/UpdateRecordE', { m: uItem })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.editSeqE = -99;
                            this.getSubList(5);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },

            //=== 設計方案評選 - 方案項目權重
            //新增
            onNewRecordF() {
                //if (this.strEmpty(this.newRecordF.Weight1)
                //    || this.strEmpty(this.newRecordF.Weight2)
                //    || this.strEmpty(this.newRecordF.Weight3)
                //    || this.strEmpty(this.newRecordF.Weight4)
                //    || this.strEmpty(this.newRecordF.Weight5)
                //    || this.strEmpty(this.newRecordF.Weight6)
                //    || this.strEmpty(this.newRecordF.Weight7)
                //) {
                //    alert('方案項目權重 必須輸入!');
                //    return;
                //}
                this.newRecordF.EngRiskFrontSeq = this.riskItem.Seq;
                window.myAjax.post('/EngRiskFront/UpdateRecordF', { m: this.newRecordF })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.editSeqF = -99;
                            this.getSubList(6);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //儲存
            onSaveRecordF(uItem) {
                //console.log(uItem);
                //if (this.strEmpty(uItem.Weight1)
                //    || this.strEmpty(uItem.Weight2)
                //    || this.strEmpty(uItem.Weight3)
                //    || this.strEmpty(uItem.Weight4)
                //    || this.strEmpty(uItem.Weight5)
                //    || this.strEmpty(uItem.Weight6)
                //    || this.strEmpty(uItem.Weight7)
                //) {
                //    alert('方案項目權重 必須輸入!');
                //    return;
                //}
                window.myAjax.post('/EngRiskFront/UpdateRecordF', { m: uItem })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.editSeqF = -99;
                            this.getSubList(6);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            setITWeight(index, item) {
                let Total = parseInt(0);
                let TW = parseInt(0);
                let NW = parseInt(0);
                this.itemsF.forEach(r => {
                    if (r.PSType == 2) {
                        console.log("-A-");
                        TW = (this.strEmpty(r.Weight1) ? 0 : parseInt(r.Weight1)) * 0.01;
                        NW = (this.strEmpty(item.Weight1) ? 0 : parseInt(item.Weight1))
                        Total += NW * TW;
                        TW = (this.strEmpty(r.Weight2) ? 0 : parseInt(r.Weight2)) * 0.01;
                        NW = (this.strEmpty(item.Weight2) ? 0 : parseInt(item.Weight2))
                        Total += NW * TW;
                        TW = (this.strEmpty(r.Weight3) ? 0 : parseInt(r.Weight3)) * 0.01;
                        NW = (this.strEmpty(item.Weight3) ? 0 : parseInt(item.Weight3))
                        Total += NW * TW;
                        TW = (this.strEmpty(r.Weight4) ? 0 : parseInt(r.Weight4)) * 0.01;
                        NW = (this.strEmpty(item.Weight4) ? 0 : parseInt(item.Weight4))
                        Total += NW * TW;
                        TW = (this.strEmpty(r.Weight5) ? 0 : parseInt(r.Weight5)) * 0.01;
                        NW = (this.strEmpty(item.Weight5) ? 0 : parseInt(item.Weight5))
                        Total += NW * TW;
                        TW = (this.strEmpty(r.Weight6) ? 0 : parseInt(r.Weight6)) * 0.01;
                        NW = (this.strEmpty(item.Weight6) ? 0 : parseInt(item.Weight6))
                        Total += NW * TW;
                        TW = (this.strEmpty(r.Weight7) ? 0 : parseInt(r.Weight7)) * 0.01;
                        NW = (this.strEmpty(item.Weight7) ? 0 : parseInt(item.Weight7))
                        Total += NW * TW;
                        this.itemsF[index].TWeight = Total;
                        console.log(Total);
                    }
                });
            },
            setTWeight() {
                let Total = parseInt(0);
                let TW = parseInt(0);
                let NW = parseInt(0);
                this.itemsF.forEach(r => {
                    if (r.PSType == 2) {
                        console.log("-A-");
                        TW = (this.strEmpty(r.Weight1) ? 0 : parseInt(r.Weight1)) * 0.01;
                        NW = (this.strEmpty(this.newRecordF.Weight1) ? 0 : parseInt(this.newRecordF.Weight1))
                        Total += NW * TW;
                        TW = (this.strEmpty(r.Weight2) ? 0 : parseInt(r.Weight2)) * 0.01;
                        NW = (this.strEmpty(this.newRecordF.Weight2) ? 0 : parseInt(this.newRecordF.Weight2))
                        Total += NW * TW;
                        TW = (this.strEmpty(r.Weight3) ? 0 : parseInt(r.Weight3)) * 0.01;
                        NW = (this.strEmpty(this.newRecordF.Weight3) ? 0 : parseInt(this.newRecordF.Weight3))
                        Total += NW * TW;
                        TW = (this.strEmpty(r.Weight4) ? 0 : parseInt(r.Weight4)) * 0.01;
                        NW = (this.strEmpty(this.newRecordF.Weight4) ? 0 : parseInt(this.newRecordF.Weight4))
                        Total += NW * TW;
                        TW = (this.strEmpty(r.Weight5) ? 0 : parseInt(r.Weight5)) * 0.01;
                        NW = (this.strEmpty(this.newRecordF.Weight5) ? 0 : parseInt(this.newRecordF.Weight5))
                        Total += NW * TW;
                        TW = (this.strEmpty(r.Weight6) ? 0 : parseInt(r.Weight6)) * 0.01;
                        NW = (this.strEmpty(this.newRecordF.Weight6) ? 0 : parseInt(this.newRecordF.Weight6))
                        Total += NW * TW;
                        TW = (this.strEmpty(r.Weight7) ? 0 : parseInt(r.Weight7)) * 0.01;
                        NW = (this.strEmpty(this.newRecordF.Weight7) ? 0 : parseInt(this.newRecordF.Weight7))
                        Total += NW * TW;
                        this.newRecordF.TWeight = Total;
                        console.log(Total);
                    }
                });
            },

            //=== 檔案上傳
            async getFileList(Seq, ERFType) {
                window.myAjax.post('/EngRiskFront/GetEngRiskFrontFileList', { Seq: Seq, ERFType: ERFType })
                    .then(resp => {
                        if (ERFType == 1)
                            this.itemsF1 = resp.data.items;
                        else if (ERFType == 2)
                            this.itemsF2 = resp.data.items;
                        else if (ERFType == 3)
                            this.itemsF3 = resp.data.items;
                        else if (ERFType == 4)
                            this.itemsF4 = resp.data.items;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            fileChangeF(event,ERFType) {
                var files = event.target.files || event.dataTransfer.files;
                // 預防檔案為空檔
                if (!files.length) return;
                var uploadfiles = new FormData();
                uploadfiles.append("file", files[0], files[0].name);
                uploadfiles.append("EngRiskFrontSeq", this.targetId);
                uploadfiles.append("ERFType", ERFType);
                this.onAddRecordFile(uploadfiles, ERFType);
            },
            onAddRecordFile(uploadfiles, ERFType) {
                window.myAjax.post('/EngRiskFront/AddEngRiskFrontFile', uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getFileList(this.targetId, ERFType);
                            //this.onNewRecord(this.targetId, 2);
                            //this.filesDS = new FormData();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onDelF(index, id, ERFType) {
                if (!confirm('是否確定刪除資料？')) return;
                window.myAjax.post('/EngRiskFront/DelEngRiskFrontFile', { id: id })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            if (ERFType == 1) this.itemsF1.splice(index, 1);
                            else if (ERFType == 2) this.itemsF2.splice(index, 1);
                            else if (ERFType == 3) this.itemsF3.splice(index, 1);
                            else if (ERFType == 4) this.itemsF4.splice(index, 1);
                            alert(resp.data.msg);
                            console.log(resp);
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //下載-設計方案評選附件
            downloadF(item) {
                window.myAjax.get('/EngRiskFront/DownloadEngRiskFrontFile?id=' + item.Seq, { responseType: 'blob' })
                    .then(resp => {
                        const blob = new Blob([resp.data]);
                        const contentType = resp.headers['content-type'];
                        if (contentType.indexOf('application/json') >= 0) {
                            //alert(resp.data);
                            const reader = new FileReader();
                            reader.addEventListener('loadend', (e) => {
                                const text = e.srcElement.result;
                                const data = JSON.parse(text)
                                alert(data.message);
                            });
                            reader.readAsText(blob);
                        } else if (contentType.indexOf('application/blob') >= 0) {
                            var saveFilename = null;
                            const data = decodeURI(resp.headers['content-disposition']);
                            var array = data.split("filename*=UTF-8''");
                            if (array.length == 2) {
                                saveFilename = array[1];
                            } else {
                                array = data.split("filename=");
                                saveFilename = array[1];
                            }
                            if (saveFilename != null) {
                                const url = window.URL.createObjectURL(blob);
                                const link = document.createElement('a');
                                link.href = url;
                                link.setAttribute('download', saveFilename);
                                document.body.appendChild(link);
                                link.click();
                            } else {
                                console.log('saveFilename is null');
                            }
                        } else {
                            alert('格式錯誤下載失敗');
                        }
                    }).catch(error => {
                        console.log(error);
                    });
            },

            async insertEngRiskSubProject() {
                this.insertSubProject.EngRiskFrontSeq = this.riskItem.Seq;
                let res = (await window.myAjax.post('EngRiskFrontManagement/insertEngRiskSubProject', { m: this.insertSubProject })).data;
                if (res == -1) alert("exceNo編碼重複!");
                else {
                    await this.getSubList(7);
                    this.insertSubProject = {};
                    this.page = this.pageCount;
                }
            },
            async updateEngRiskSubProject(item) {
                await window.myAjax.post('EngRiskFrontManagement/updateEngRiskSubProject', { m: item });
                this.getSubList(7);
                this.editIndex = null;
            },
            async deleteEngRiskSubProject(item) {
                await window.myAjax.post("/EngRiskFrontManagement/deleteEngRiskSubProject", { subProjectSeq: item.Seq })
                this.getSubList(7);
            }

        },
        async mounted() {
            console.log('mounted() 施工風險評估報告產製');
            this.targetId = window.sessionStorage.getItem(window.eqSelTrenderPlanSeq);
            console.log('eqSelTrenderPlanSeq:' + this.targetId);
            console.log("subProject", this.subProject);
            this.getUserUnit();
            this.getUnitList1();
            this.getItem();
            this.getFileList(this.targetId,1);
            this.getFileList(this.targetId,2);
            this.getFileList(this.targetId,3);
            this.getFileList(this.targetId,4);
            this.checkDocument();
        }
    }
</script>
<style>
    .ck-editor__editable_inline {
        min-height: 250px;
    }
</style>