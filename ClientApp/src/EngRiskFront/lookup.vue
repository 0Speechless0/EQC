<template>
    <div>
        <p  class="p-2">{{ subProjectMain.SubProjectName }}</p>
        <div class="table-responsive">
            <table border="1" class="table table1 min910" style="border-color: #fff;">
                <tbody>
                    <tr>
                        <td colspan="2" style="background-color: #dbdbdb;text-align: center;vertical-align:middle;"><p>作業條件</p></td>
                        <td colspan="2" style="background-color: #dbdbdb;text-align: center;vertical-align:middle;  width: 7%;border-bottom-color: #fff;"><p>作業環境</p></td>
                    </tr>
                    <tr>
                        <td rowspan="2" style="background-color: #dbdbdb;text-align: center;vertical-align:middle;  width: 8%; ">
                            <p>現有防護設施</p>
                        </td>
                        <td rowspan="2" style="vertical-align:middle;  width: 40%;">
                            <textarea rows="5" type="text" class="form-control"
                                      v-model="subProjectMain.ExistingProtectiveFacilities"
                                      value="">河床主要為大卵石、礫石及砂土組成，中間覆蓋較厚，兩岸覆蓋較薄，鄰近構造物有石岡壩、水資源回收中心邊坡護岸、埤豐橋、民宅數棟，降雨多集中在5~8月。</textarea>
                            <!-- <p>河床主要為大卵石、礫石及砂土組成，中間覆蓋較厚，兩岸覆蓋較薄，鄰近構造物有石岡壩、水資源回收中心邊坡護岸、埤豐橋、民宅數棟，降雨多集中在5~8月。
                            </p> -->
                        </td>
                        <td style="background-color: #dbdbdb;text-align: center;vertical-align:middle;  width: 7%; border-bottom-color: #fff;">
                            <p>工程控制</p>
                        </td>
                        <td>
                            <textarea rows="3" type="text" class="form-control" v-model="subProjectMain.Equipment"
                                      value="">施工圍籬、作業區警示及管制號誌、警示帶、水位監控、交通錐及連桿、施工用電設備</textarea>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #dbdbdb;text-align: center;vertical-align:middle; border-bottom-color: #fff;">
                            <p>管理控制</p>
                        </td>
                        <td>
                            <textarea rows="3" type="text" class="form-control" v-model="subProjectMain.EngControl"
                                      value="">交通引導人員、現場監視指揮人員、車輛人員進出管制措施、安全衛生作業標準、救援通報系統</textarea>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #dbdbdb;text-align: center;vertical-align:middle; width: 5%; ">
                            <p>機具設備</p>
                        </td>
                        <td style="vertical-align:middle; ">
                            <textarea rows="3" type="text" class="form-control" v-model="subProjectMain.ManagementControl"
                                      value="">挖土機、推土機、卡車、鑽掘機、抽水機</textarea>
                        </td>
                        <td style="background-color: #dbdbdb;text-align: center;vertical-align:middle; ">
                            <p>個人防護具</p>
                        </td>
                        <td>
                            <textarea rows="3" type="text" class="form-control" v-model="subProjectMain.PersonalProtectiveEquipment"
                                      value="">安全鞋、口罩、耳塞</textarea>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="table-responsive">
            <table border="1" class="table table1 min910" style="border-color: #fff;">
                <tbody>
                    <tr>
                        <td style="text-align: center;vertical-align:middle;background-color: #dbdbdb;border-bottom-color: #fff;"
                            colspan="4">
                            <p>作業內容</p>
                        </td>
                        <td style="text-align: center;vertical-align:middle;background-color: #dbdbdb;background-color: #dbdbdb;border-bottom-color: #fff;"
                            colspan="2">
                            <p>風險辨識</p>
                        </td>
                        <td style="text-align: center;vertical-align:middle;background-color: #dbdbdb;border-bottom-color: #fff;"
                            colspan="4">
                            <p>風險分析</p>
                        </td>
                        <td style="text-align: center;vertical-align:middle;background-color: #dbdbdb;width: 5%;border-bottom-color: #fff;">
                            <p>風險評量</p>
                        </td>
                        <td style="text-align: center;vertical-align:middle;background-color: #dbdbdb;border-bottom-color: #fff;"
                            colspan="4">
                            <p>風險處理</p>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: center;vertical-align:middle;background-color: #dbdbdb;"
                            rowspan="2">
                            <p>編號</p>
                        </td>
                        <td style="text-align: center;vertical-align:middle;background-color: #dbdbdb;width: 7%;"
                            colspan="3" rowspan="2">
                            <p>作業步驟</p>
                        </td>
                        <td style="text-align: center;vertical-align:middle; background-color: #dbdbdb;width: 200px;"
                            rowspan="2">
                            <p>危害類型</p>
                        </td>
                        <td style="text-align: center;vertical-align:middle; background-color: #dbdbdb;width: 12%;"
                            rowspan="2">
                            <p>可能之風險狀況</p>
                        </td>
                        <td style="text-align: center;vertical-align:middle; background-color: #dbdbdb;width: 6%;" rowspan="2">
                            <p>可能性</p><i class="fas fa-lightbulb" data-toggle="modal" data-target="#prepare_edit02_" style="color: #c30000;"></i>
                            <div class="modal fade" id="prepare_edit02_" style="display: none;" aria-hidden="true">
                                <div class="modal-dialog modal-xl modal-dialog-centered " style="max-width: fit-content;">
                                    <div class="modal-content">
                                        <div class="card whiteBG mb-4 pattern-F colorset_1">
                                            <div class="tab-content">
                                                <div class="tab-pane active">
                                                    <h5 style="text-align: left;">風險可能性</h5>
                                                    <img id="image" src="../assets/images/可能性.png" alt="懸浮可能性" style="display: none;display: block;">
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                        <td style="text-align: center;vertical-align:middle; background-color: #dbdbdb;width: 6%;"
                            rowspan="2">
                            <p>嚴重度</p><i class="fas fa-lightbulb" data-toggle="modal" data-target="#prepare_edit12_" style="color: #c30000;"></i>
                            <div class="modal fade" id="prepare_edit12_" style="display: none;" aria-hidden="true">
                                <div class="modal-dialog modal-xl modal-dialog-centered " style="max-width: fit-content;">
                                    <div class="modal-content">
                                        <div class="card whiteBG mb-4 pattern-F colorset_1">
                                            <div class="tab-content">
                                                <div class="tab-pane active">
                                                    <h5 style="text-align: left;">後果嚴重度</h5>
                                                    <img id="image" src="../assets/images/嚴重度.png" alt="懸浮嚴重度.png" style="display: none;display: block;">
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                        <td style="text-align: center;vertical-align:middle;background-color: #dbdbdb;width: 10%;"
                            rowspan="2">
                            <p>風險值／風險等級</p><i class="fas fa-lightbulb" data-toggle="modal" data-target="#prepare_edit147_" style="color: #c30000;"></i>
                            <div class="modal fade" id="prepare_edit147_" style="display: none;" aria-hidden="true">
                                <div class="modal-dialog modal-xl modal-dialog-centered " style="max-width: fit-content;">
                                    <div class="modal-content">
                                        <div class="card whiteBG mb-4 pattern-F colorset_1">
                                            <div class="tab-content">
                                                <div class="tab-pane active">
                                                    <h5 style="text-align: left;">風險值／風險等級</h5>
                                                    <img id="image" src="../assets/images/風險值風險等級.png" alt="懸浮風險值風險等級" style="display: none;display: block;">
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                        <!-- <td style="text-align: center;vertical-align:middle;background-color: #dbdbdb;width: 7%;"
                            rowspan="2">
                            <p></p>
                        </td> -->
                        <td style="text-align: center;vertical-align:middle;background-color: #dbdbdb;width: 7%;"
                            rowspan="2">
                            <p>風險可否接受</p>
                        </td>
                        <td style="text-align: center;vertical-align:middle;background-color: #dbdbdb; width: 13%;">
                            <p>風險對策</p>
                        </td>
                        <td style="text-align: center;vertical-align:middle;background-color: #dbdbdb;"
                            rowspan="2">
                            <p>執行成果摘記</p>
                        </td>
                        <td style="text-align: center;vertical-align:middle;background-color: #dbdbdb;width: 7%;"
                            rowspan="2">
                            <p>成效確認</p>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: center;vertical-align:middle;background-color: #dbdbdb;">
                            <p>處理風險與機會之措施</p>
                        </td>

                    </tr>
                    <!-- <tr>
                        <td style="  text-align: center;vertical-align:middle;">
                            <p>a</p>
                        </td>
                        <td colspan="14">
                            <p>前置作業</p>
                        </td>
                    </tr>
                    <tr  style="background-color: rgb(245, 209, 209);"  id="tr1" onclick="toggleRows('i-id1','i-id2','i-id3')">
                        <td style="  text-align: center;vertical-align:middle;">
                            <p>i</p>
                        </td>
                        <td colspan="14">
                            <p>動員及場地整備</p>
                        </td>
                    </tr> -->
                    <tr v-for="(item, index) in subProjectDetail" :key="index" @click="showSelectDetailSub(item, true)"
                        :style="{
                        'background-color' : item.Level == 3 ? ( item.viewIndex % 2 == 0 ? 'rgb(245, 209, 209)' : 'rgb(187, 226, 241)' ) : 'white'

                    }">

                        <td style="text-align: center; vertical-align: middle;" v-show="item.showDetail">
                            <p>{{ item.StepNo.slice(1, item.StepNo.length) }}</p>
                        </td>
                        <td style=" vertical-align: middle;" v-show="item.showDetail" :colspan="item.Level < 4 ? 14: 3" :style="'text-align:left'">
                            <p>{{ item.StepName }}</p>
                        </td>
                        <td style="text-align: center; vertical-align: middle;" v-show="item.showDetail && item.Level ==4 ">
                            <select class="form-control" v-model="item.HazardTypeSeq">
                                <option v-for="(option, index) in HazardTypeList" :key="index" :value="option.Seq">{{option.TypeName}}</option>

                            </select>
                        </td>
                        <td style="vertical-align: middle; text-align: center;" v-show="item.showDetail && item.Level ==4 ">
                            <div class="d-inline-block col-12" tabindex="0" v-b-tooltip :title="item.PossibleRiskSituation">
                                <label>{{(item.PossibleRiskSituation ?? "").slice(0, 15)}}</label>
                            </div>

                            <button id="0523pen" class="btn btn-color11-3 btn-xs m-1 " title="" data-toggle="modal" :data-target="`#prepare_edit${index}`">
                                <i class="fas fa-pencil-alt"></i>
                            </button>
                            <div class="modal fade" :id="`prepare_edit${index}`" style="display: none;" aria-hidden="true" :ref="`prepare_edit${index}`">
                                <div class="modal-dialog modal-xl modal-dialog-centered " style="max-width: fit-content;">
                                    <div class="modal-content">
                                        <div class="card whiteBG mb-4 pattern-F colorset_1">
                                            <div class="tab-content">
                                                <div class="tab-pane active" style="padding: 15px;">
                                                    <h5 style="text-align: left;">可能之風險狀況</h5>
                                                    <textarea v-model="item.PossibleRiskSituationTemp" id="textarea11" rows="10" cols="50" type="text" class="form-control" style="width: auto;">
                                                        施工機具進場，出入口無專人指揮，或未看到交管指揮人員，致交管指揮人員有被撞之虞。
                                                    </textarea>
                                                    <div class="row justify-content-center mt-5">
                                                        <div class="d-flex">
                                                            <button id="0523save" role="button" class="btn btn-color11-2 btn-xs mx-1" @click="storeTemp(item, 'PossibleRiskSituation', `prepare_edit${index}`)">
                                                                <i class="fas fa-save">&nbsp;儲存</i>
                                                            </button>
                                                            <button role="button" class="btn btn-color9-1 btn-xs mx-1" data-dismiss="modal">
                                                                <i class="fas fa-xmark">&nbsp;關閉</i>
                                                            </button>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                        <td style="text-align: center; vertical-align: middle;" v-show="item.showDetail && item.Level ==4 ">
                            <input v-model="item.Possibility" type="number" class="form-control" value="0" />
                            <!-- <input type="text" class="form-control" value=""> -->
                        </td>
                        <td style="text-align: center; vertical-align: middle;" v-show="item.showDetail && item.Level ==4 ">
                            <input v-model="item.Severity" type="number" class="form-control" value="0" />
                        </td>
                        <td style="text-align: center; vertical-align: middle;" v-show="item.showDetail && item.Level ==4 ">
                            <p>{{ RiskValue(item) }}</p>
                        </td>
                        <td style="text-align: center; vertical-align: middle;" v-show="item.showDetail && item.Level ==4 ">
                            <select class="form-control" v-model="item.IsAcceptable">
                                <option :value="1">是</option>
                                <option :value="0">否</option>
                            </select>
                            <!-- <input type="text" class="form-control" value=""> -->
                        </td>
                        <td style="text-align: center; vertical-align: middle;" v-show="item.showDetail && item.Level ==4 ">


                            <span class="d-inline-block col-12" tabindex="0" v-b-tooltip :title="item.RisksAndOpportunitiesMeasure">
                                <label>{{(item.RisksAndOpportunitiesMeasure ?? "").slice(0, 15)}}</label>
                            </span>

                            <button id="0523pen" class="btn btn-color11-3 btn-xs m-1" title="" data-toggle="modal" :data-target="`#prepare_edit2${index}`">
                                <i class="fas fa-pencil-alt"></i>
                            </button>
                            <div class="modal fade" :id="`prepare_edit2${index}`" style="display: none;" aria-hidden="true">
                                <div class="modal-dialog modal-xl modal-dialog-centered " style="max-width: fit-content;">
                                    <div class="modal-content">
                                        <div class="card whiteBG mb-4 pattern-F colorset_1">
                                            <div class="tab-content">
                                                <div class="tab-pane active" style="padding: 15px;">
                                                    <h5 style="text-align: left;">風險對策-處理風險與機會之措施</h5>
                                                    <textarea v-model="item.RisksAndOpportunitiesMeasureTemp" rows="10" cols="50" type="text" class="form-control" style="width: auto;">編列交通指揮人員、反光背心。</textarea>
                                                    <div class="row justify-content-center mt-5">
                                                        <div class="d-flex">
                                                            <button id="0523save" role="button" class="btn btn-color11-2 btn-xs mx-1" @click="storeTemp(item, 'RisksAndOpportunitiesMeasure', `prepare_edit2${index}`)">
                                                                <i class="fas fa-save">&nbsp;儲存</i>
                                                            </button>
                                                            <button role="button" class="btn btn-color9-1 btn-xs mx-1" data-dismiss="modal">
                                                                <i class="fas fa-xmark">&nbsp;關閉</i>
                                                            </button>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>

                        <td style="text-align: center; vertical-align: middle;" v-show="item.showDetail && item.Level ==4 ">
                            <span class="d-inline-block col-12" tabindex="0" v-b-tooltip :title="item.SummaryOfExecutiveResults">
                                <label>{{(item.SummaryOfExecutiveResults ?? "").slice(0, 15)}}</label>
                            </span>
                            <button id="0523pen" class="btn btn-color11-3 btn-xs m-1" title="" data-toggle="modal" :data-target="`#prepare_edit3${index}`">
                                <i class="fas fa-pencil-alt"></i>
                            </button>
                            <div class="modal fade" :id="`prepare_edit3${index}`" style="display: none;" aria-hidden="true">
                                <div class="modal-dialog modal-xl modal-dialog-centered " style="max-width: fit-content;">
                                    <div class="modal-content">
                                        <div class="card whiteBG mb-4 pattern-F colorset_1">
                                            <div class="tab-content">
                                                <div class="tab-pane active" style="padding: 15px;">
                                                    <h5 style="text-align: left;">執行成果摘記</h5>
                                                    <textarea rows="10" cols="50" type="text" class="form-control" style="width: auto;" v-model="item.SummaryOfExecutiveResultsTemp">預算書編列交通指揮人員(四、23)、反光背心(四、14)</textarea>
                                                    <div class="row justify-content-center mt-5">
                                                        <div class="d-flex">
                                                            <button id="0523save" role="button" class="btn btn-color11-2 btn-xs mx-1" @click="storeTemp(item, 'SummaryOfExecutiveResults', `prepare_edit3${index}`)">
                                                                <i class="fas fa-save">&nbsp;儲存</i>
                                                            </button>
                                                            <button role="button" class="btn btn-color9-1 btn-xs mx-1" data-dismiss="modal">
                                                                <i class="fas fa-xmark">&nbsp;關閉</i>
                                                            </button>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <!-- <input type="text" class="form-control" value=""> -->
                        </td>
                        <td style="  text-align: center;vertical-align:middle;" v-show="item.showDetail && item.Level ==4 ">
                            <select class="form-control" v-model="item.IsEffect">
                                <option :value="1">是</option>
                                <option :value="0">否</option>
                            </select>
                        </td>

                    </tr>

                </tbody>
            </table>
        </div>
        <div class="row justify-content-center mt-5">
            <div class="d-flex">
                <button role="button" class="btn btn-color11-2 btn-sm mx-1" @click="updateRiskSubProjectDetail">
                    <i class="fas fa-save">&nbsp;儲存</i>
                </button>
                <button role="button" class="btn btn-color9-1 btn-sm mx-1" @click="$emit('goBack')"> 回上頁 </button>
            </div>
        </div>
    </div>

</template>

<script>
    export default {

        props: ["subProject"],
        data: () => {
            return {
                selectedNo: "",
                subProjectDetail: [],
                subProjectMain: {}
            }
        },
        methods:
        {
            async updateRiskSubProjectDetail() {
                let res = (await window.myAjax.post("EngRiskFrontManagement/updateEngRiskSubProjectDetail", {
                    list: this.subProjectDetail,

                    main: this.subProjectMain
                })
                ).data;
                if (res == true) alert("儲存成功");
            },
            async getRiskSubProjectDeatil() {
                this.subProjectMain = this.subProject;
                console.log(this.subProjectMain);
                var subProjectDetailData = (
                    await window.myAjax.post("EngRiskFrontManagement/getEngRiskSubProjectDetail",
                        { subProjectSeq: this.subProject.Seq })
                ).data;

                if (subProjectDetailData.list.length == 0) {
                    alert("無資料，應該要先進行分項工程拆解!");
                    return;
                }
                this.subProjectDetail = subProjectDetailData.list;

                this.subProjectDetail
                    .filter(e => e.Level == 2)
                    .forEach(e => e.showDetail = true);
                this.subProjectDetail
                    .filter(e => e.Level == 4)
                    .forEach(e => {
                        e.PossibleRiskSituationTemp = e.PossibleRiskSituation;
                        e.RisksAndOpportunitiesMeasureTemp = e.RisksAndOpportunitiesMeasure;
                        e.SummaryOfExecutiveResultsTemp = e.SummaryOfExecutiveResults;

                    })
                this.HazardTypeList = subProjectDetailData.hazardTypeList;

                this.setLevel3Index();
                console.log("a", this.subProjectDetail);
            },
            storeTemp(item, col, modalId) {
                item[col] = item[col + "Temp"];
                window.closeModal(`#${modalId}`);
            },
            RiskValue(item) {
                var value = item.Possibility * item.Severity;
                var hint;
                if (value >= 6) {
                    hint = "高";
                }
                else if (value >= 3) {
                    hint = "中"
                }
                else {
                    hint = "低";
                }
                return `${item.Possibility * item.Severity}/${hint}`
            },
            showSelectDetailSub(selectItem, origin = false) {

                var updateDetail = this.subProjectDetail
                    .filter(e => e.StepNo.startsWith(selectItem.StepNo) && e.Level == selectItem.Level + 1);

                if (updateDetail.length > 0) {
                    updateDetail.forEach(e => {


                        if (origin) e.showDetail = !e.showDetail;
                        else e.showDetail = selectItem.showDetail;

                        if (e.showDetail == false) this.showSelectDetailSub(e);

                    })
                }

                // var newSubProjectDetail =
                //     this.subProjectDetail.map(e =>{
                //         e.showDetail = ( e.StepNo.startsWith(selectItem.StepNo) && e.Level == selectItem.Level+1 )
                //         ? !e.showDetail :e.showDetail
                //     return e;
                // });


                if (origin) this.subProjectDetail = this.subProjectDetail.slice();
            },
            setLevel3Index() {
                var level3Group = [];
                this.subProjectDetail.forEach(
                    e => {
                        if (e.Level != 3) return;
                        else if (!level3Group[e.ParentSeq]) level3Group[e.ParentSeq] = [];

                        level3Group[e.ParentSeq].push(e)
                    }
                )
                level3Group.forEach(group => {
                    var i = 0;
                    group.forEach(e => e.viewIndex = i++)

                })

                this.subProjectDetail = this.subProjectDetail.slice();
                console.log(this.subProjectDetail);
            }
        },
        mounted() {
            this.getRiskSubProjectDeatil();
        }
    }

</script>
<style scoped>
    td {
        padding: 1rem !important;
    }
</style>