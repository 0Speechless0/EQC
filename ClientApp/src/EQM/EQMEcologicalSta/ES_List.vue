<template>
    <div>
        <div class="form-row">
            <div class="col form-inline my-2">
                <div class="custom-control custom-radio mx-2">
                    <input v-model="mode" @change="onGetResords" value="1" type="radio" id="self" class="custom-control-input" name="S/C">
                    <label class="custom-control-label" for="self">規劃設計階段之工程案件-生態檢核</label>
                </div>
                <div class="custom-control custom-radio mx-2">
                    <input v-model="mode" @change="onGetResords" value="2" type="radio" id="commission" class="custom-control-input" name="S/C">
                    <label class="custom-control-label" for="commission">施工階段之工程案件-生態檢核</label>
                </div>
            </div>
        </div>

        <div class="table-responsive">
            <table class="table table-responsive-md table-hover VA-middle">
                <thead class="insearch">
                    <tr>
                        <th>機關別</th>
                        <th>
                            決標總件數<br />
                            (A)<br />
                            (A=B+C)
                        </th>
                        <th>
                            不需辦理生態檢核件數<br />
                            (B)
                        </th>
                        <th>
                            應辦理生態檢核件數(C)<br />
                            (C=D+E)
                        </th>
                        <th>
                            依規定辦理生態檢核件數<br />
                            (D)
                        </th>
                        <th>
                            漏依規定辦理生態檢核件數<br />
                            (E)
                        </th>
                        <th>
                            不需辦理生態檢核件數<br />
                            比率%(F)<br />
                            (F=B/A)
                        </th>
                        <th>
                            應辦理生態檢核件數<br />
                            比率%<br />
                            (G)<br />
                            (G=C/A)
                        </th>
                        <th>
                            依規定辦理生態檢核件數<br />
                            比率%<br />
                            (H)<br />
                            (H=D/C)
                        </th>
                        <th>
                            漏依規定辦理生態檢核件數比率%<br />
                            (I)<br />
                            (F=E/C)
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td>{{item.execUnitName}}</td>
                        <td>{{item.engCount}}</td>
                        <td>{{item.notChcek}}</td>
                        <td>{{item.needChcek}}</td>
                        <td>{{item.execChcek}}</td>
                        <td>{{item.lostChcek}}</td>
                        <td>{{item.notChcekRate}}</td>
                        <td>{{item.needChcekRate}}</td>
                        <td>{{item.execChcekRate}}</td>
                        <td>{{item.lostChcekRate}}</td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div v-if="items.length > 0">
            <button @click="download" class="btn btn-color11-1 btn-block col-2">
                <i class="fas fa-download"></i>生態檢核統計表
            </button>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                mode: 1,
                items: [],
            };
        },
        methods: {
            //下載 水利署碳排管制表
            download() {
                window.myAjax.get('/EQMEcologicalSta/Download?mode='+this.mode, { responseType: 'blob' })
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
            //紀錄清單
            onGetResords() {
                this.canDownload = false;
                this.items = [];
                window.myAjax.post('/EQMEcologicalSta/GetRecords', {mode:this.mode})
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        mounted() {
            console.log('mounted() 生態檢核統計');
            this.onGetResords();
        }
    }
</script>
