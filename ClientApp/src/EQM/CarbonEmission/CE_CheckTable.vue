<template>    
    <div>
        <div class="card whiteBG mb-4 pattern-F colorset_1">
            <h5>規劃、設計階段</h5>
            <table class="table table-responsive-md table-hover VA-middle">
                <thead class="insearch">
                    <tr class="insearch">
                        <th style="width: 150px;">面向</th>
                        <th style="width: 150px;text-align: center;">類別</th>
                        <th style="width: 50px;text-align: center;">編號</th>
                        <th style="width: 800px;text-align: center;">規劃、設計階段</th>
                        <th style="width: 50px;">是</th>
                        <th style="width: 50px;">否</th>
                        <th style="width: 70px;">不適用</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td rowspan="10">綠色環境</td>
                        <td rowspan="10" style="text-align: center;">戶外減碳</td>
                        <td style="text-align: center;">1</td>
                        <td>是否加大工程綠地面積。</td>
                        <td><input v-model="checkTable.F1101" value="1" type="radio" name="F1101" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1101" value="2" type="radio" name="F1101" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1101" value="3" type="radio" name="F1101" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>2</td>
                        <td>是否避開原有老樹設計，施工時保護老樹不受傷害。</td>
                        <td><input v-model="checkTable.F1102" value="1" type="radio" name="F1102" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1102" value="2" type="radio" name="F1102" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1102" value="3" type="radio" name="F1102" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>3</td>
                        <td>是否多數綠地種喬木或複層綠化，少種人工草坪、花圃、灌木。</td>
                        <td><input v-model="checkTable.F1103" value="1" type="radio" name="F1103" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1103" value="2" type="radio" name="F1103" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1103" value="3" type="radio" name="F1103" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>4</td>
                        <td>是否在坡面工或坡腳，以植穴或花台方式，種植喬木。</td>
                        <td><input v-model="checkTable.F1104" value="1" type="radio" name="F1104" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1104" value="2" type="radio" name="F1104" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1104" value="3" type="radio" name="F1104" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>5</td>
                        <td>是否利用多年生蔓藤植物攀爬構造物，以爭取綠化量。</td>
                        <td><input v-model="checkTable.F1105" value="1" type="radio" name="F1105" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1105" value="2" type="radio" name="F1105" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1105" value="3" type="radio" name="F1105" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>6</td>
                        <td>
                            是否盡量利用在地物種或碳儲存效果佳之樹種(如：高固碳樹種<br>
                            <span v-for="(item, index) in checkTable.TreeList" v-bind:key="item.Seq">
                                <template v-if="item.Seq != 9999">
                                    <input v-model="item.Checked" type="checkbox">
                                    {{item.TreeName}}
                                    <input v-model.number="item.Amount" type="number" class="underLineStyle" placeholder="" style="outline: none;border: none;border-bottom: 1px solid #000000;width: 50px;">株，
                                </template>
                                <template v-else>
                                    <input v-model="item.Checked" type="checkbox">其他
                                    <input v-model.trim="item.TreeName" type="text" maxlength="30" class="underLineStyle" placeholder="" style="outline: none;border: none;border-bottom: 1px solid #000000;width: 150px;">樹種
                                    <input v-model.number="item.Amount" type="number" class="underLineStyle" placeholder="" style="outline: none;border: none;border-bottom: 1px solid #000000;width: 50px;">株
                                </template>
                            </span>)
                        </td>
                        <td><input v-model="checkTable.F1106" value="1" type="radio" name="F1106" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1106" value="2" type="radio" name="F1106" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1106" value="3" type="radio" name="F1106" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>7</td>
                        <td>
                            灌木樹種<br>
                            <span v-for="(item, index) in checkTable.ShrubList" v-bind:key="item.Seq">
                                <template v-if="item.Seq != 9999">
                                    <input v-model="item.Checked" type="checkbox">
                                    {{item.TreeName}}
                                    <input v-model.number="item.Amount" type="number" class="underLineStyle" placeholder="" style="outline: none;border: none;border-bottom: 1px solid #000000;width: 50px;">株，
                                </template>
                                <template v-else>
                                    <input v-model="item.Checked" type="checkbox">其他
                                    <input v-model.trim="item.TreeName" type="text" maxlength="30" class="underLineStyle" placeholder="" style="outline: none;border: none;border-bottom: 1px solid #000000;width: 150px;">樹種
                                    <input v-model.number="item.Amount" type="number" class="underLineStyle" placeholder="" style="outline: none;border: none;border-bottom: 1px solid #000000;width: 50px;">株
                                </template>
                            </span>
                        </td>
                        <!-- td style="display: flex; flex-direction: column;">
                    其他：
                    <textarea v-model.trim="checkTable.F1107Desc" maxlength="100" class="form-control"></textarea>
                </td -->
                        <td><input v-model="checkTable.F1107" value="1" type="radio" name="F1107" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1107" value="2" type="radio" name="F1107" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1107" value="3" type="radio" name="F1107" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>8</td>
                        <td style="display: flex; flex-direction: column;">
                            綠化面積(m2)：
                            <input v-model.number="checkTable.F1108Area" type="number" class="form-control">
                        </td>
                        <td><input v-model="checkTable.F1108" value="1" type="radio" name="F1108" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1108" value="2" type="radio" name="F1108" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1108" value="3" type="radio" name="F1108" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>9</td>
                        <td style="display: flex; flex-direction: column;">
                            綠化長度(m)：
                            <input v-model.number="checkTable.F1109Length" type="number" class="form-control">
                        </td>
                        <td><input v-model="checkTable.F1109" value="1" type="radio" name="F1109" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1109" value="2" type="radio" name="F1109" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1109" value="3" type="radio" name="F1109" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>10</td>
                        <td style="display: flex; flex-direction: column;">
                            工程設計減碳策略/長度(m)、範圍(m2)、數量(m3)：
                            <textarea v-model.trim="checkTable.F1110Desc" maxlength="100" class="form-control"></textarea>
                        </td>
                        <td><input v-model="checkTable.F1110" value="1" type="radio" name="F1110" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1110" value="2" type="radio" name="F1110" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1110" value="3" type="radio" name="F1110" @click="onRadioClick($event)"></td>
                    </tr>
                </tbody>
                <tbody>
                    <tr>
                        <td rowspan="6">綠色材料</td>
                        <td rowspan="6" style="text-align: center;">再生材料</td>
                        <td style="text-align: center;">1</td>
                        <td>
                            是否採用礦物摻料預拌混凝土設計<br>
                            (礦物摻料
                            <input v-model="checkTable.F1201Mix" value="1" type="radio" name="F1201Mix" @click="onRadioClick($event)">20%，
                            <input v-model="checkTable.F1201Mix" value="2" type="radio" name="F1201Mix" @click="onRadioClick($event)">30%，
                            <input v-model="checkTable.F1201Mix" value="3" type="radio" name="F1201Mix" @click="onRadioClick($event)">50%，
                            <input v-model="checkTable.F1201Mix" value="0" type="radio" name="F1201Mix" @click="onRadioClick($event)">其他
                            <input v-model.trim="checkTable.F1201Other" maxlength="30" type="text" class="underLineStyle" placeholder="" style="outline: none;border: none;border-bottom: 1px solid #000000;width: 150px;">)
                        </td>
                        <td><input v-model="checkTable.F1201" value="1" type="radio" name="F1201" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1201" value="2" type="radio" name="F1201" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1201" value="3" type="radio" name="F1201" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>2</td>
                        <td>是否採用高性能混凝土設計，以減少水泥使用量。</td>
                        <td><input v-model="checkTable.F1202" value="1" type="radio" name="F1202" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1202" value="2" type="radio" name="F1202" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1202" value="3" type="radio" name="F1202" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>3</td>
                        <td>是否採用再生級配骨材作為混凝土骨材。</td>
                        <td><input v-model="checkTable.F1203" value="1" type="radio" name="F1203" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1203" value="2" type="radio" name="F1203" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1203" value="3" type="radio" name="F1203" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>4</td>
                        <td>是否採用再生磚塊或再生水泥磚作為室外圍牆或花台。</td>
                        <td><input v-model="checkTable.F1204" value="1" type="radio" name="F1204" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1204" value="2" type="radio" name="F1204" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1204" value="3" type="radio" name="F1204" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>5</td>
                        <td>是否採用再生瀝青混凝土。</td>
                        <td><input v-model="checkTable.F1205" value="1" type="radio" name="F1205" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1205" value="2" type="radio" name="F1205" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1205" value="3" type="radio" name="F1205" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>6</td>
                        <td style="display: flex; flex-direction: column;">
                            其他：
                            <textarea v-model.trim="checkTable.F1206Desc" maxlength="100" class="form-control"></textarea>
                        </td>
                        <td><input v-model="checkTable.F1206" value="1" type="radio" name="F1206" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1206" value="2" type="radio" name="F1206" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1206" value="3" type="radio" name="F1206" @click="onRadioClick($event)"></td>
                    </tr>
                </tbody>
                <tbody>
                    <tr>
                        <td rowspan="12">綠色工法</td>
                        <td rowspan="12" style="text-align: center;">減廢</td>
                        <td style="text-align: center;">1</td>
                        <td>是否採用土方平衡設計。</td>
                        <td><input v-model="checkTable.F1301" value="1" type="radio" name="F1301" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1301" value="2" type="radio" name="F1301" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1301" value="3" type="radio" name="F1301" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>2</td>
                        <td>是否運用土方交換規劃。</td>
                        <td><input v-model="checkTable.F1302" value="1" type="radio" name="F1302" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1302" value="2" type="radio" name="F1302" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1302" value="3" type="radio" name="F1302" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>3</td>
                        <td>是否採用預鑄材料設計。</td>
                        <td><input v-model="checkTable.F1303" value="1" type="radio" name="F1303" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1303" value="2" type="radio" name="F1303" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1303" value="3" type="radio" name="F1303" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>4</td>
                        <td>是否採用地工織物材料設計。</td>
                        <td><input v-model="checkTable.F1304" value="1" type="radio" name="F1304" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1304" value="2" type="radio" name="F1304" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1304" value="3" type="radio" name="F1304" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>5</td>
                        <td>棄土，餘方是否近運利用。</td>
                        <td><input v-model="checkTable.F1305" value="1" type="radio" name="F1305" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1305" value="2" type="radio" name="F1305" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1305" value="3" type="radio" name="F1305" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>6</td>
                        <td>
                            是否選擇材料回填，
                            <input v-model="checkTable.F1306Mode" value="1" type="radio" name="F1306Mode" @click="onRadioClick($event)">就地取材
                            <input v-model="checkTable.F1306Mode" value="2" type="radio" name="F1306Mode" @click="onRadioClick($event)">級配粒料
                            <input v-model="checkTable.F1306Mode" value="3" type="radio" name="F1306Mode" @click="onRadioClick($event)">透水材料。
                        </td>
                        <td><input v-model="checkTable.F1306" value="1" type="radio" name="F1306" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1306" value="2" type="radio" name="F1306" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1306" value="3" type="radio" name="F1306" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>7</td>
                        <td>
                            是否採用砌排石工，
                            <input v-model="checkTable.F1307Mode" value="1" type="radio" name="F1307Mode" @click="onRadioClick($event)">乾砌石
                            <input v-model="checkTable.F1307Mode" value="2" type="radio" name="F1307Mode" @click="onRadioClick($event)">鋪石
                            <input v-model="checkTable.F1307Mode" value="3" type="radio" name="F1307Mode" @click="onRadioClick($event)">漿砌石
                            <input v-model="checkTable.F1307Mode" value="4" type="radio" name="F1307Mode" @click="onRadioClick($event)">混凝土排石。
                        </td>
                        <td><input v-model="checkTable.F1307" value="1" type="radio" name="F1307" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1307" value="2" type="radio" name="F1307" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1307" value="3" type="radio" name="F1307" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>8</td>
                        <td>是否採用蛇籠工。</td>
                        <td><input v-model="checkTable.F1308" value="1" type="radio" name="F1308" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1308" value="2" type="radio" name="F1308" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1308" value="3" type="radio" name="F1308" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>9</td>
                        <td>
                            是否採用箱型石籠工，
                            <input v-model="checkTable.F1309Mode" value="1" type="radio" name="F1309Mode" @click="onRadioClick($event)">就地取料
                            <input v-model="checkTable.F1309Mode" value="2" type="radio" name="F1309Mode" @click="onRadioClick($event)">外購石料。
                        </td>
                        <td><input v-model="checkTable.F1309" value="1" type="radio" name="F1309" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1309" value="2" type="radio" name="F1309" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1309" value="3" type="radio" name="F1309" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>10</td>
                        <td>
                            是否採用拋石工，
                            <input v-model="checkTable.F1310Mode" value="1" type="radio" name="F1310Mode" @click="onRadioClick($event)">就地取料
                            <input v-model="checkTable.F1310Mode" value="2" type="radio" name="F1310Mode" @click="onRadioClick($event)">外購石料。
                        </td>
                        <td><input v-model="checkTable.F1310" value="1" type="radio" name="F1310" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1310" value="2" type="radio" name="F1310" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1310" value="3" type="radio" name="F1310" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>11</td>
                        <td>是否採用多孔隙瀝青混凝土鋪面。</td>
                        <td><input v-model="checkTable.F1311" value="1" type="radio" name="F1311" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1311" value="2" type="radio" name="F1311" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1311" value="3" type="radio" name="F1311" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>12</td>
                        <td style="display: flex; flex-direction: column;">
                            其他：
                            <textarea v-model.trim="checkTable.F1312Desc" maxlength="100" class="form-control"></textarea>
                        </td>
                        <td><input v-model="checkTable.F1312" value="1" type="radio" name="F1312" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1312" value="2" type="radio" name="F1312" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1312" value="3" type="radio" name="F1312" @click="onRadioClick($event)"></td>
                    </tr>
                </tbody>
                <tbody>
                    <tr>
                        <td rowspan="4">綠色工法</td>
                        <td rowspan="4" style="text-align: center;">營建自動化</td>
                        <td style="text-align: center;">1</td>
                        <td>是否採用系統模板。</td>
                        <td><input v-model="checkTable.F1401" value="1" type="radio" name="F1401" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1401" value="2" type="radio" name="F1401" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1401" value="3" type="radio" name="F1401" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>2</td>
                        <td>是否採用預鑄外牆、柱、樑。</td>
                        <td><input v-model="checkTable.F1402" value="1" type="radio" name="F1402" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1402" value="2" type="radio" name="F1402" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1402" value="3" type="radio" name="F1402" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>3</td>
                        <td>是否採用清水模板。</td>
                        <td><input v-model="checkTable.F1403" value="1" type="radio" name="F1403" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1403" value="2" type="radio" name="F1403" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1403" value="3" type="radio" name="F1403" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>4</td>
                        <td style="display: flex; flex-direction: column;">
                            其他：
                            <textarea v-model.trim="checkTable.F1404Desc" maxlength="100" class="form-control"></textarea>
                        </td>
                        <td><input v-model="checkTable.F1404" value="1" type="radio" name="F1404" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1404" value="2" type="radio" name="F1404" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F1404" value="3" type="radio" name="F1404" @click="onRadioClick($event)"></td>
                    </tr>
                </tbody>
            </table>
            <h5>施工階段</h5>
            <table class="table table-responsive-md table-hover VA-middle">
                <thead class="insearch">
                    <tr class="insearch">
                        <th style="width: 150px;">面向</th>
                        <th style="width: 150px;text-align: center;">類別</th>
                        <th style="width: 50px;text-align: center;">編號</th>
                        <th style="width: 800px;text-align: center;">施工階段</th>
                        <th style="width: 50px;">是</th>
                        <th style="width: 50px;">否</th>
                        <th style="width: 70px;">不適用</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td rowspan="3">綠色環境</td>
                        <td rowspan="3" style="text-align: center;">生態</td>
                        <td style="text-align: center;">1</td>
                        <td>是否於用地整理時應注意植栽保留，避免多餘之砍伐。</td>
                        <td><input v-model="checkTable.F2101" value="1" type="radio" name="F2101" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F2101" value="2" type="radio" name="F2101" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F2101" value="3" type="radio" name="F2101" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>2</td>
                        <td>是否於施工階段辦理生態檢核。</td>
                        <td><input v-model="checkTable.F2102" value="1" type="radio" name="F2102" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F2102" value="2" type="radio" name="F2102" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F2102" value="3" type="radio" name="F2102" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>3</td>
                        <td style="display: flex; flex-direction: column;">
                            其他：
                            <textarea v-model.trim="checkTable.F2103Desc" maxlength="100" class="form-control"></textarea>
                        </td>
                        <td><input v-model="checkTable.F2103" value="1" type="radio" name="F2103" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F2103" value="2" type="radio" name="F2103" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F2103" value="3" type="radio" name="F2103" @click="onRadioClick($event)"></td>
                    </tr>
                </tbody>
                <tbody>
                    <tr>
                        <td rowspan="4">綠色能源</td>
                        <td rowspan="4" style="text-align: center;">節能</td>
                        <td style="text-align: center;">1</td>
                        <td>是否施工中工區照明、交通維持之警示燈、警示路牌與指示燈、工務所用電及其他用電，配置節能標章產品，以達節能減碳。</td>
                        <td><input v-model="checkTable.F2201" value="1" type="radio" name="F2201" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F2201" value="2" type="radio" name="F2201" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F2201" value="3" type="radio" name="F2201" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>2</td>
                        <td>是否選用低耗能(節能)的施工機具與設備，有助於工程節能減碳。</td>
                        <td><input v-model="checkTable.F2202" value="1" type="radio" name="F2202" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F2202" value="2" type="radio" name="F2202" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F2202" value="3" type="radio" name="F2202" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>3</td>
                        <td>是否選用適當功率之機具工作，降低閒置等候時間，間接降低耗能，以達減碳功效。</td>
                        <td><input v-model="checkTable.F2203" value="1" type="radio" name="F2203" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F2203" value="2" type="radio" name="F2203" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F2203" value="3" type="radio" name="F2203" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>4</td>
                        <td style="display: flex; flex-direction: column;">
                            其他：
                            <textarea v-model.trim="checkTable.F2204Desc" maxlength="100" class="form-control"></textarea>
                        </td>
                        <td><input v-model="checkTable.F2204" value="1" type="radio" name="F2204" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F2204" value="2" type="radio" name="F2204" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F2204" value="3" type="radio" name="F2204" @click="onRadioClick($event)"></td>
                    </tr>
                </tbody>
                <tbody>
                    <tr>
                        <td rowspan="3">綠色工法</td>
                        <td rowspan="3" style="text-align: center;">減廢</td>
                        <td style="text-align: center;">1</td>
                        <td>是否採最短運距以土方挖填，促使土方挖填平衡，降低土方運入或運離所造成之排碳。</td>
                        <td><input v-model="checkTable.F2301" value="1" type="radio" name="F2301" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F2301" value="2" type="radio" name="F2301" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F2301" value="3" type="radio" name="F2301" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>2</td>
                        <td>是否採用就地取材或縮短運距之材料，降低運送過程之中碳排放。</td>
                        <td><input v-model="checkTable.F2302" value="1" type="radio" name="F2302" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F2302" value="2" type="radio" name="F2302" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F2302" value="3" type="radio" name="F2302" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>3</td>
                        <td style="display: flex; flex-direction: column;">
                            其他：<textarea v-model.trim="checkTable.F2303Desc" maxlength="100" class="form-control"></textarea>
                        </td>
                        <td><input v-model="checkTable.F2303" value="1" type="radio" name="F2303" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F2303" value="2" type="radio" name="F2303" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F2303" value="3" type="radio" name="F2303" @click="onRadioClick($event)"></td>
                    </tr>
                </tbody>
            </table>
            <h5>營運階段</h5>
            <table class="table table-responsive-md table-hover VA-middle">
                <thead class="insearch">
                    <tr class="insearch">
                        <th style="width: 150px;">面向</th>
                        <th style="width: 150px;text-align: center;">類別</th>
                        <th style="width: 50px;text-align: center;">編號</th>
                        <th style="width: 800px;text-align: center;">營運階段</th>
                        <th style="width: 50px;">是</th>
                        <th style="width: 50px;">否</th>
                        <th style="width: 70px;">不適用</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td rowspan="4">綠色環境</td>
                        <td rowspan="4" style="text-align: center;">生態</td>
                        <td style="text-align: center;">1</td>
                        <td>是否維持工程完工時之綠覆率，確保計有植栽90%以上存活率。</td>
                        <td><input v-model="checkTable.F3101" value="1" type="radio" name="F3101" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3101" value="2" type="radio" name="F3101" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3101" value="3" type="radio" name="F3101" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>2</td>
                        <td>是否增加原有綠化面積或生態複層面積比例之30%以上。</td>
                        <td><input v-model="checkTable.F3102" value="1" type="radio" name="F3102" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3102" value="2" type="radio" name="F3102" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3102" value="3" type="radio" name="F3102" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>3</td>
                        <td>是否對既有樹種作保護與維持。</td>
                        <td><input v-model="checkTable.F3103" value="1" type="radio" name="F3103" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3103" value="2" type="radio" name="F3103" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3103" value="3" type="radio" name="F3103" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>4</td>
                        <td style="display: flex; flex-direction: column;">
                            其他：<textarea v-model.trim="checkTable.F3104Desc" maxlength="100" class="form-control"></textarea>
                        </td>
                        <td><input v-model="checkTable.F3104" value="1" type="radio" name="F3104" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3104" value="2" type="radio" name="F3104" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3104" value="3" type="radio" name="F3104" @click="onRadioClick($event)"></td>
                    </tr>
                </tbody>
                <tbody>
                    <tr>
                        <td rowspan="4">綠色能源</td>
                        <td rowspan="4" style="text-align: center;">節能</td>
                        <td style="text-align: center;">1</td>
                        <td>是否進行例行性維修與檢測，維持設備高效率或低耗能之功能，或適度更新節能產品。</td>
                        <td><input v-model="checkTable.F3201" value="1" type="radio" name="F3201" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3201" value="2" type="radio" name="F3201" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3201" value="3" type="radio" name="F3201" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>2</td>
                        <td>是否採用替代能源維持設備運轉。</td>
                        <td><input v-model="checkTable.F3202" value="1" type="radio" name="F3202" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3202" value="2" type="radio" name="F3202" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3202" value="3" type="radio" name="F3202" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>3</td>
                        <td>是否增設太陽光電、小水利發電等綠電設施。</td>
                        <td><input v-model="checkTable.F3203" value="1" type="radio" name="F3203" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3203" value="2" type="radio" name="F3203" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3203" value="3" type="radio" name="F3203" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>4</td>
                        <td style="display: flex; flex-direction: column;">
                            其他：
                            <textarea v-model.trim="checkTable.F3204Desc" maxlength="100" class="form-control"></textarea>
                        </td>
                        <td><input v-model="checkTable.F3204" value="1" type="radio" name="F3204" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3204" value="2" type="radio" name="F3204" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3204" value="3" type="radio" name="F3204" @click="onRadioClick($event)"></td>
                    </tr>
                </tbody>
                <tbody>
                    <tr>
                        <td rowspan="4">綠色環境</td>
                        <td rowspan="4" style="text-align: center;">固碳</td>
                        <td style="text-align: center;">1</td>
                        <td>是否於水庫、滯洪池、環境營造周邊，增加種植喬木，可吸附貯存較高之二氧化碳。</td>
                        <td><input v-model="checkTable.F3301" value="1" type="radio" name="F3301" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3301" value="2" type="radio" name="F3301" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3301" value="3" type="radio" name="F3301" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>2</td>
                        <td style="display: flex; flex-direction: column;">
                            其他：
                            <textarea v-model.trim="checkTable.F3302Desc" maxlength="100" class="form-control"></textarea>
                        </td>
                        <td><input v-model="checkTable.F3302" value="1" type="radio" name="F3302" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3302" value="2" type="radio" name="F3302" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3302" value="3" type="radio" name="F3302" @click="onRadioClick($event)"></td>
                    </tr>
                </tbody>
                <tbody>
                    <tr>
                        <td rowspan="4">綠色工法</td>
                        <td rowspan="4" style="text-align: center;">耐久</td>
                        <td style="text-align: center;">1</td>
                        <td>是否執行防沖刷、防淤積等維護要項(含設施與管理)，延長設施壽命。</td>
                        <td><input v-model="checkTable.F3401" value="1" type="radio" name="F3401" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3401" value="2" type="radio" name="F3401" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3401" value="3" type="radio" name="F3401" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>2</td>
                        <td>是否紀錄耐久設計與材料之效益，回饋後續工程之規劃、設計與施工。</td>
                        <td><input v-model="checkTable.F3402" value="1" type="radio" name="F3402" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3402" value="2" type="radio" name="F3402" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3402" value="3" type="radio" name="F3402" @click="onRadioClick($event)"></td>
                    </tr>
                    <tr>
                        <td>3</td>
                        <td style="display: flex; flex-direction: column;">
                            其他：
                            <textarea v-model.trim="checkTable.F3403Desc" maxlength="100" class="form-control"></textarea>
                        </td>
                        <td><input v-model="checkTable.F3403" value="1" type="radio" name="F3403" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3403" value="2" type="radio" name="F3403" @click="onRadioClick($event)"></td>
                        <td><input v-model="checkTable.F3403" value="3" type="radio" name="F3403" @click="onRadioClick($event)"></td>
                    </tr>
                </tbody>
                
            </table>
            <h5>備註</h5>
            <table>
                <tbody>
                    <tr>
                        <td><textarea v-model.trim="checkTable.Remark" maxlength="300" class="form-control"></textarea></td>
                    </tr>
                </tbody>
            </table>
            <h5>設計者簽章：</h5>
            <canvas id="mycanvas" style="background: rgb(238, 238, 238) none repeat scroll 0% 0%; width: 400px; height: 200px;" width="400" height="200"></canvas>
            <div>
                <button @click="onConvertToImageClick" id="convertToImage" class="btn btn-color11-3 btn-xs mx-1">轉圖</button>
                <button id="clear" class="btn btn-color9-1 btn-xs mx-1">清除</button>
            </div>
            <div v-show="checkTable.Signature.length > 0" id="image" style="display: block; width: 400px; height: 200px; max-width: 400px; max-height: 200px; ">
                <img src="">
            </div>

            <div class="row justify-content-center mt-5">
                <div class="form-row">
                    <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                        <button @click="onSaveClick" role="button" class="btn btn-block btn-color11-2 btn-sm">
                            <i class="fas fa-save">&nbsp;儲存</i>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['tenderItem','ceHeader'],
        data: function () {
            return {
                checkTable: { Signature: '' },
                clickItem:null
            };
        },
        methods: {
            /*/下載 節能減碳簡易檢核表 s20230428
            downloadCheckTable() {
                window.comm.dnFile('/EQMCarbonEmission/DnCheckTable?id=' + this.tenderItem.Seq);
            },*/
            //s20230502
            onRadioClick(event) {
                this.checkTable[event.srcElement.name] = null;
            },
            //清單
            getResord() {
                this.checkTable = { Signature: ''};
                window.myAjax.post('/EQMCarbonEmission/GetCheckTable', { id: this.tenderItem.Seq } )
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.checkTable = resp.data.item;
                            if (document.getElementById('image') != null)
                                document.getElementById('image').innerHTML = "<img src='" + this.checkTable.Signature + "' style='width: 400px; height: 200px;'/>";
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //儲存
            onSaveClick() {
                if (this.checkTable.Seq == -1) this.checkTable.CarbonEmissionHeaderSeq = this.ceHeader.Seq;

                window.myAjax.post('/EQMCarbonEmission/SaveCheckTable', { item: this.checkTable })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            if (this.checkTable.Seq == -1) {
                                this.checkTable.Seq = resp.data.id;
                            }
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onConvertToImageClick() {
                console.log("convertToImage")
                var image = canvas.toDataURL("image/png");
                this.checkTable.Signature = image;
                document.getElementById('image').innerHTML = "<img src='" + this.checkTable.Signature + "' style='width: 400px; height: 200px;' />";
            }
        },
        async mounted() {
            console.log('mounted() 碳排放量檢核表');
            this.getResord();

            canvas = document.getElementById('mycanvas');
            ctx = canvas.getContext("2d");
            // 抗鋸齒
            // ref: https://www.zhihu.com/question/37698502
            let width = 400;//canvas.width,
            let height = 200;//canvas.height;
            if (window.devicePixelRatio) {
                canvas.style.width = width + "px";
                canvas.style.height = height + "px";
                canvas.height = height * window.devicePixelRatio;
                canvas.width = width * window.devicePixelRatio;
                ctx.scale(window.devicePixelRatio, window.devicePixelRatio);
            }
            canvas.addEventListener('mousedown', function (evt) {
                var mousePos = getMousePos(canvas, evt);
                ctx.beginPath();
                ctx.moveTo(mousePos.x, mousePos.y);
                evt.preventDefault();
                canvas.addEventListener('mousemove', mouseMove, false);
            });

            canvas.addEventListener('mouseup', function () {
                canvas.removeEventListener('mousemove', mouseMove, false);
            }, false);
            canvas.addEventListener('touchstart', function (evt) {
                // console.log('touchstart')
                // console.log(evt)
                var touchPos = getTouchPos(canvas, evt);
                ctx.beginPath(touchPos.x, touchPos.y);
                ctx.moveTo(touchPos.x, touchPos.y);
                evt.preventDefault();
                canvas.addEventListener('touchmove', touchMove, false);
            });

            canvas.addEventListener('touchend', function () {
                // console.log("touchend")
                canvas.removeEventListener('touchmove', touchMove, false);
            }, false);


            // clear
            document.getElementById('clear').addEventListener('click', function () {
                // console.log("reset")
                ctx.clearRect(0, 0, canvas.width, canvas.height);
            }, false);


            // convertToImage
            /*document.getElementById('convertToImage').addEventListener('click', function () {
                console.log("convertToImage")
                var image = canvas.toDataURL("image/png");
                this.checkTable.Signature = image;
                document.getElementById('image').innerHTML = "<img src='" + this.checkTable.Signature + "' alt='from canvas'/>";
                //$('#').html("<img src='" + image + "' alt='from canvas'/>");
            }, false);*/
            // 簽名製作
        },
    }
    // 簽名製作
    var canvas = document.getElementById('mycanvas');
    var ctx;
    // mouse
    function getMousePos(canvas, evt) {
        var rect = canvas.getBoundingClientRect();
        return {
            x: evt.clientX - rect.left,
            y: evt.clientY - rect.top
        };
    }

    function mouseMove(evt) {
        var mousePos = getMousePos(canvas, evt);
        ctx.lineCap = "round";
        ctx.lineWidth = 2;
        ctx.lineJoin = "round";
        ctx.shadowBlur = 1; // 邊緣模糊，防止直線邊緣出現鋸齒 
        ctx.shadowColor = 'black';// 邊緣顏色
        ctx.lineTo(mousePos.x, mousePos.y);
        ctx.stroke();
    }
    // touch
    function getTouchPos(canvas, evt) {
        var rect = canvas.getBoundingClientRect();
        return {
            x: evt.touches[0].clientX - rect.left,
            y: evt.touches[0].clientY - rect.top
        };
    }

    function touchMove(evt) {
        // console.log("touchmove")
        var touchPos = getTouchPos(canvas, evt);
        // console.log(touchPos.x, touchPos.y)

        ctx.lineWidth = 2;
        ctx.lineCap = "round"; // 繪制圓形的結束線帽
        ctx.lineJoin = "round"; // 兩條線條交匯時，建立圓形邊角
        ctx.shadowBlur = 1; // 邊緣模糊，防止直線邊緣出現鋸齒 
        ctx.shadowColor = 'black'; // 邊緣顏色
        ctx.lineTo(touchPos.x, touchPos.y);
        ctx.stroke();
    }
</script>
