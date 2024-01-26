<template>
    <div>
        <div v-if="loading">
            Loading...
        </div>
        <div class="table-responsive">
            <table class="table table1 min910" border="0">
                <thead>
                    <tr>
                        <th class="sort">排序</th>
                        <th>環境保育清單</th>
                        <th>建立日期</th>
                        <th>更新日期</th>
                        <th>功能</th>
                    </tr>
                </thead>

                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.id">
                        <td>
                            {{item.OrderNo}}
                        </td>
                        <td>
                            {{item.ItemName}}
                        </td>
                        <td>
                            {{item.createDate}}
                        </td>
                        <td>
                            {{item.modifyDate}}
                        </td>
                        <td>
                            <div v-if="item.detailCount>0" class="row justify-content-center m-0">
                                <a v-on:click.stop="download(item)" href="#" class="btn-block mx-2 btn btn-color11-2" title="查看"><i class="fas fa-eye"></i> 查看查看</a>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div style="width:99%;" class="row justify-content-center">
            <b-pagination :total-rows="totalRows"
                          :per-page="perPage"
                          v-model="pageIndex">
            </b-pagination>
        </div>
    </div>
</template>
<script>
    export default {
        watch: {
            pageIndex: {
                handler: function (value) {
                    this.getList();
                }
            }
        },
        data: function () {
            return {
                loading: false,
                items: [],
                //分頁
                pageIndex: 1,
                perPage: 10,
                totalRows: 0,
            };
        },
        methods: {
            download(item) {
                window.myAjax.get('/CheckSheet/Chapter702Download?seq=' + item.Seq, { responseType: 'blob' })
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
            getList() {
                this.loading = true;
                this.items = [];
                let params = { pageIndex: this.pageIndex, perPage: this.perPage };
                window.myAjax.post('/CheckSheet/Chapter702', params)
                    .then(resp => {
                        this.items = resp.data.items;
                        this.totalRows = resp.data.pTotal;
                        this.loading = false;
                    })
                    .catch(err => {
                        this.loading = false;
                        console.log(err);
                    });
            }
        },
        async mounted() {
            console.log('mounted() 第七章 702 環境保育管理標準');
            this.getList();
        }
    }
</script>