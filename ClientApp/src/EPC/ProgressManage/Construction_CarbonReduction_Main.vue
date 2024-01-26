<template>
<div>

    <div class="card-header">
    <h3 class="card-title font-weight-bold">進度管理</h3>
</div>
 <div class="card-body">
                    <h5 class="insearch mt-0 py-2"> 工程編號：112-I-01020-021125-000(112-I-01020-021125-000)<br>工程名稱：蘭陽溪37~41斷面疏濬工程兼供土石採售分離（支出）(蘭陽溪37~41斷面疏濬工程兼供土石採售分離（支出）)<br>
                    工程總減碳效益：7615973
                    </h5>
                <div class="card whiteBG mb-4 pattern-F colorset_R">
                    <div style="padding-top: 20px; padding-left: 20px; margin-bottom: -5px;">
                        <ul role="tablist" class="nav nav-tabs">
                            <li class="nav-item"><a data-toggle="tab" href="#menu01" class="nav-link active">機具調度分析</a>
                            </li>
                            <li class="nav-item"><a data-toggle="tab" href="#menu02" class="nav-link">機具減碳量化計算表</a></li>
                            <button role="button" class="btn btn-color11-3 btn-sm mx-1" id="savenew">
                                <i class="fas fa-bolt">&nbsp;更新</i>
                            </button>
                            <button role="button" class="btn btn-color11-1 btn-sm mx-1" id="savenew">
                                <i class="fas fa-download">&nbsp;下載</i>
                            </button>
                        </ul>
                        
                    </div>
                    <div class="tab-content">
                        <div id="menu01" class="tab-pane active">
                            <h5>挖土機</h5>
                            <p>單位：台</p>
                            <div class="table-responsive">
                                <table class="table table-responsive-md table-hover">
                                    <thead class="insearch">
                                        <tr>
                                            <th style="width: px;"><strong>項次</strong></th>
                                            <th><strong>類型</strong></th>
                                            <th><strong>單位</strong></th>
                                            <th style="width: px;"><strong>PC120</strong></th>
                                            <th style="width: px;"><strong>PC200</strong></th>
                                            <th style="width: px;"><strong>PC300</strong></th>
                                            <th style="width: px;"><strong>PC400</strong></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><strong>1</strong></td>
                                            <td>土方工作</td>
                                            <td>M3</td>
                                            <td><input type="text" class="form-control" value="0"></td>
                                            <td><input type="text" class="form-control" value="0"></td>
                                            <td><input type="text" class="form-control" value="3"></td>
                                            <td><input type="text" class="form-control" value="0"></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <h5>傾卸貨車</h5>
                            <p>單位：台</p>
                            <div class="table-responsive">
                                <table class="table table-responsive-md table-hover">
                                    <thead class="insearch">
                                        <tr>
                                            <th style="width: px;"><strong>項次</strong></th>
                                            <th><strong>類型</strong></th>
                                            <th><strong>單位</strong></th>
                                            <th style="width: px;"><strong>15T</strong></th>
                                            <th style="width: px;"><strong>21T</strong></th>
                                            <th style="width: px;"><strong>35T</strong></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><strong>1</strong></td>
                                            <td>土方工作</td>
                                            <td>M3</td>
                                            <td><input type="text" class="form-control" value="3"></td>
                                            <td><input type="text" class="form-control" value="0"></td>
                                            <td><input type="text" class="form-control" value="0"></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <h5>能源減碳</h5>
                            <p>單位：台</p>
                            <div class="table-responsive">
                                <table class="table table-responsive-md table-hover">
                                    <thead class="insearch">
                                        <tr>
                                            <th style="width: px;"><strong>項次</strong></th>
                                            <th><strong>類型</strong></th>
                                            <th><strong>單位</strong></th>
                                            <th style="width: px;"><strong>度</strong></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><strong>1</strong></td>
                                            <td>電力</td>
                                            <td>度</td>
                                            <td><input type="text" class="form-control" value="100"></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <div class="row justify-content-center mt-5">
                                <div class="d-flex">
                                    <button role="button" class="btn btn-color11-2 btn-sm mx-1" id="savenew">
                                        <i class="fas fa-save">&nbsp;儲存</i>
                                    </button>
                                </div>
                            </div>
                        </div>
                        <div id="menu02" class="tab-pane ">
                            <h5>挖土機</h5>
                            <div class="table-responsive">
                                <table class="table table-responsive-md table-hover">
                                    <thead class="insearch">
                                        <tr>
                                            <th style="width: px;"><strong>項次</strong></th>
                                            <th><strong>類型</strong></th>
                                            <th><strong>單位</strong></th>
                                            <th><strong>工作項目</strong></th>
                                            <th><strong>編碼</strong></th>
                                            <th style="width: px;"><strong>PC120</strong></th>
                                            <th style="width: px;"><strong>PC200</strong></th>
                                            <th style="width: px;"><strong>PC300</strong></th>
                                            <th style="width: px;"><strong>PC400</strong></th>
                                            <th><strong>減碳量估算</strong></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><strong>1</strong></td>
                                            <td>土方工作</td>
                                            <td>M3</td>
                                            <td>機械拆除，無筋混凝土，未含運費</td>
                                            <td>022010423</td>
                                            <td><input type="text" class="form-control" value="0"></td>
                                            <td><input type="text" class="form-control" value="0"></td>
                                            <td><input type="text" class="form-control" value="3"></td>
                                            <td><input type="text" class="form-control" value="0"></td>
                                            <th><strong>23661</strong></th>
                                        </tr>

                                    </tbody>
                                </table>
                            </div>
                            <h5>傾卸貨車</h5>
                            <div class="table-responsive">
                                <table class="table table-responsive-md table-hover">
                                    <thead class="insearch">
                                        <tr>
                                            <th style="width: px;"><strong>項次</strong></th>
                                            <th><strong>類型</strong></th>
                                            <th><strong>單位</strong></th>
                                            <th><strong>工作項目</strong></th>
                                            <th><strong>編碼</strong></th>
                                            <th style="width: px;"><strong>15T</strong></th>
                                            <th style="width: px;"><strong>21T</strong></th>
                                            <th style="width: px;"><strong>35T</strong></th>
                                            <th><strong>減碳量估算</strong></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><strong>1</strong></td>
                                            <td>土方工作</td>
                                            <td>M3</td>
                                            <td>土方工作，填方</td>
                                            <td>02300D0003</td>
                                            <td><input type="text" class="form-control" value="3"></td>
                                            <td><input type="text" class="form-control" value="0"></td>
                                            <td><input type="text" class="form-control" value="0"></td>
                                            <th><strong>1524</strong></th>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <h5>能源減碳</h5>
                            <div class="table-responsive">
                                <table class="table table-responsive-md table-hover">
                                    <thead class="insearch">
                                        <tr>
                                            <th style="width: px;"><strong>項次</strong></th>
                                            <th><strong>類型</strong></th>
                                            <th><strong>單位</strong></th>
                                            <th><strong>工作項目</strong></th>
                                            <th><strong>編碼</strong></th>
                                            <th style="width: px;"><strong>度</strong></th>
                                            <th><strong>減碳量估算</strong></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><strong>1</strong></td>
                                            <td>電力</td>
                                            <td>度</td>
                                            <td>太陽能發電</td>
                                            <td></td>
                                            <td><input type="text" class="form-control" value="100"></td>
                                            <th><strong>1005</strong></th>
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
<script setup>


</script>