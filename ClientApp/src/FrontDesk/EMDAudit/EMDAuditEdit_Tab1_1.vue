<template>
    <div>
        <div class="modal fade text-left" v-bind:id="modalId" v-bind:ref="modalId" tabindex="-1" aria-labelledby="infoEdit_01" aria-hidden="true">
            <div class="modal-dialog modal-dialog-scrollable modal-dialog-centered modal-xl" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">協力廠商資料</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close" v-on:click="closeModal()">
                            <span aria-hidden="true">×</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <MaterialInfo v-bind:emdSummary="emdSummary"></MaterialInfo>
                        <h2>協力廠商</h2>
                        <div class="table-responsive">
                            <table border="0" class="table table2 mt-0">
                                <tbody>
                                    <tr>
                                        <th>廠商名稱</th>
                                        <td>
                                            <input type="text" class="form-control" v-model="targetItem.VendorName">
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>廠商統編</th>
                                        <td><input type="text" class="form-control" v-model="targetItem.VendorTaxId"></td>
                                    </tr>
                                    <tr>
                                        <th>廠商地址</th>
                                        <td><input type="text" class="form-control" v-model="targetItem.VendorAddr"></td>
                                    </tr>
                                    <tr>
                                        <th><button :disabled="!targetItem.edit" v-if="targetItem.edit" @click="getGeocodeClick" class="btn btn-block btn-color11-2 btn-sm">取得座標</button></th>
                                        <td>緯度: {{targetItem.VendorLat}} , 經度: {{targetItem.VendorLng}} , 距離: {{targetItem.VendorDistance}}</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div class="modal-footer justify-content-center">
                        <button v-if="targetItem.edit" v-on:click.stop="saveItems" role="button" class="btn btn-color11-4 btn-xs mx-1" title="儲存"><i class="fas fa-save"></i> 儲存</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?key=AIzaSyCqusfoZ66T3BmV04q40fTxsPvJuLmRmIw"></script>
<script>
    export default {
        props: ['modalId', 'emdSummary', 'targetItem', 'engMain', "isEdit"],
        components: {
            MaterialInfo: require('./MaterialInfo.vue').default,
        },
        data: function () {
            return {
                initGeo: false,
                geocoder: null,
                hasGeo: false,// 存放目前是否已獲得座標的狀態
                vendor: {
                    lat: "",
                    lng: "",
                    distance: ""
                },
                engLatLng:null, //x,y
            };
        },
        methods: {
            closeHandler: function () {
                this.$emit('onCloseEvent')
            },
            closeModal() {
                this.closeHandler();
            },
            saveItems() {
                window.myAjax.post('/EMDAudit/UpdateEMDSummary_1', { item: this.targetItem })
                    .then(resp => {
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //google map s20230503
            async initGeocoder() {
                if (!this.initGeo) {
                    this.geocoder = new google.maps.Geocoder();
                }
                this.initGeo = true;
            },
            getGeocodeClick() {
                if (window.comm.stringEmpty(this.targetItem.VendorAddr) || this.targetItem.VendorAddr.length<10) {
                    alert('請輸入完整地址資料');
                    return;
                }
                this.getLatLng();
            },
            getLatLng() {
                this.initGeocoder();
                this.vendor = {lat: "", lng: "", distance: ""},
                this.hasGeo = false;
                this.errormsg = ""
                const address = this.targetItem.VendorAddr;
                this.geocoder.geocode(
                    {
                        address: address,
                        componentRestrictions: { country: "TW" }
                    },
                    (results, status) => {
                        console.log(status);
                        if (status === "OK") { //轉換成功
                            this.vendor.lat = results[0].geometry.location.lat();
                            this.vendor.lng = results[0].geometry.location.lng();
                            this.targetItem.VendorLat = this.vendor.lat;
                            this.targetItem.VendorLng = this.vendor.lng;
                            this.getEngLatLng();
                            this.hasGeo = true;
                        } else {//轉換失敗
                            this.errormsg = status;
                        }
                    }
                );
            },
            //計算距離
            getDistance() {
                if (this.engLatLng == null || this.engLatLng.CoordX == null || this.engLatLng.CoordY == null) {
                    this.vendor.distance = '沒有工程座標資料,無法計算';
                    return;
                }
                var engLat = this.engLatLng.CoordX;//緯度
                var engLng = this.engLatLng.CoordY;//經度
                if (engLat > 200 || engLng > 200) {//s20230509
                    let A = 0.00001549;
                    let B = 0.000006521;
                    let twd97x = this.engLatLng.CoordX + 807.8 + A * this.engLatLng.CoordX + B * this.engLatLng.CoordY;
                    let twd97y = this.engLatLng.CoordY - 248.6 + A * this.engLatLng.CoordY + B * this.engLatLng.CoordX;
                    let engLatLng = this.twd97ToLatlng(twd97x, twd97y);
                    engLat = engLatLng.lat;
                    engLng = engLatLng.lng;
                }

                this.vendor.distance = "";
                var origin1 = new google.maps.LatLng(engLat, engLng);
                var destinationA = new google.maps.LatLng(this.vendor.lat, this.vendor.lng);
                var service = new google.maps.DistanceMatrixService();
                service.getDistanceMatrix(
                    {
                        origins: [origin1],
                        destinations: [destinationA],
                        travelMode: 'DRIVING',
                    }, (response, status) => {
                        //console.log(response);
                        if (status == 'OK' && response.rows.length > 0) {
                            let el = response.rows[0].elements[0];
                            if (el.status == 'OK') {
                                this.vendor.distance = el.distance.text;
                                this.targetItem.VendorDistance = this.vendor.distance;
                            } else
                                this.vendor.distance = el.status;
                        } else
                            this.vendor.distance = status;
                    });
            },
            //工程座標
            getEngLatLng() {
                this.engLatLng = null;
                window.myAjax.post('/EMDAudit/GetEngLatLng', { id: this.engMain.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engLatLng = resp.data.item;
                            this.getDistance();
                        } else
                            alert('無法取得工程座標資料');
                    })
                    .catch(err => {
                        console.log(err);
                    });
     
            },
            twd97ToLatlng($x, $y) {
                var pow = Math.pow, M_PI = Math.PI;
                var sin = Math.sin, cos = Math.cos, tan = Math.tan;
                var $a = 6378137.0, $b = 6356752.314245;
                var $lng0 = 121 * M_PI / 180, $k0 = 0.9999, $dx = 250000, $dy = 0;
                var $e = pow((1 - pow($b, 2) / pow($a, 2)), 0.5);

                $x -= $dx;
                $y -= $dy;

                var $M = $y / $k0;

                var $mu = $M / ($a * (1.0 - pow($e, 2) / 4.0 - 3 * pow($e, 4) / 64.0 - 5 * pow($e, 6) / 256.0));
                var $e1 = (1.0 - pow((1.0 - pow($e, 2)), 0.5)) / (1.0 + pow((1.0 - pow($e, 2)), 0.5));

                var $J1 = (3 * $e1 / 2 - 27 * pow($e1, 3) / 32.0);
                var $J2 = (21 * pow($e1, 2) / 16 - 55 * pow($e1, 4) / 32.0);
                var $J3 = (151 * pow($e1, 3) / 96.0);
                var $J4 = (1097 * pow($e1, 4) / 512.0);

                var $fp = $mu + $J1 * sin(2 * $mu) + $J2 * sin(4 * $mu) + $J3 * sin(6 * $mu) + $J4 * sin(8 * $mu);

                var $e2 = pow(($e * $a / $b), 2);
                var $C1 = pow($e2 * cos($fp), 2);
                var $T1 = pow(tan($fp), 2);
                var $R1 = $a * (1 - pow($e, 2)) / pow((1 - pow($e, 2) * pow(sin($fp), 2)), (3.0 / 2.0));
                var $N1 = $a / pow((1 - pow($e, 2) * pow(sin($fp), 2)), 0.5);

                var $D = $x / ($N1 * $k0);

                var $Q1 = $N1 * tan($fp) / $R1;
                var $Q2 = (pow($D, 2) / 2.0);
                var $Q3 = (5 + 3 * $T1 + 10 * $C1 - 4 * pow($C1, 2) - 9 * $e2) * pow($D, 4) / 24.0;
                var $Q4 = (61 + 90 * $T1 + 298 * $C1 + 45 * pow($T1, 2) - 3 * pow($C1, 2) - 252 * $e2) * pow($D, 6) / 720.0;
                var $lat = $fp - $Q1 * ($Q2 - $Q3 + $Q4);

                var $Q5 = $D;
                var $Q6 = (1 + 2 * $T1 + $C1) * pow($D, 3) / 6;
                var $Q7 = (5 - 2 * $C1 + 28 * $T1 - 3 * pow($C1, 2) + 8 * $e2 + 24 * pow($T1, 2)) * pow($D, 5) / 120.0;
                var $lng = $lng0 + ($Q5 - $Q6 + $Q7) / cos($fp);

                $lat = ($lat * 180) / M_PI;
                $lng = ($lng * 180) / M_PI;

                return {
                    lat: $lat,
                    lng: $lng
                };
            }
        },
        async mounted() {
            console.log('mounted() 協力廠商資料');
            var scripts = [
                "https://maps.googleapis.com/maps/api/js?key=" + window.googleMapKey,
            ];
            scripts.forEach(script => {
                let tag = document.createElement("script");
                tag.setAttribute("src", script);
                document.head.appendChild(tag);
            });
            //this.initGeocoder();
        }
    }

</script>

