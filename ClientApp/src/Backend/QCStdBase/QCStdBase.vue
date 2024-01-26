<template>
    <div>
        <div class="form-row">
            <div class="col-12 col-md-4 col-lg-3 mt-3">
                <select v-model="selected" @change="mainOnChange($event)" class="form-control">
                    <option value="Chapter5">第五章 材料與設備抽驗程序及標準</option>
                    <option value="Chapter6">第六章 設備功能運轉測試抽驗程序及標準</option>
                    <option value="Chapter7">第七章 施工抽查程序及標準</option>
                </select>
            </div>
            <div v-if="selected=='Chapter5'" class="col-12 col-md-4 col-lg-3 mt-3">
                <select v-model="selected51" class="form-control">
                    <option v-for="option in options51" v-bind:value="option.Value" v-bind:key="option.Value">
                        {{ option.Text }}
                    </option>
                </select>
            </div>
            <div v-if="selected=='Chapter6'" class="col-12 col-md-4 col-lg-3 mt-3">
                <select v-model="selected61" class="form-control" >
                    <option v-for="option in options61" v-bind:value="option.Value" v-bind:key="option.Value">
                        {{ option.Text }}
                    </option>
                </select>
            </div>
                <div v-if="selected=='Chapter7'" class="col-12 col-md-4 col-lg-3 mt-3">
                    <select v-model="selected71" @change="onChange71($event)" class="form-control">
                        <option value="C701">施工抽查管理標準</option>
                        <option value="C702">環境保育管理標準</option>
                        <option value="C703">職業安全衛生管理標準</option>

                    </select>
                </div>
                <div v-if="selected=='Chapter7'" class="col-12 col-md-4 col-lg-3 mt-3">
                    <select v-model="selected72"  class="form-control" >
                        <option v-for="option in options72" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
        </div>
        <div>
            <qcBase5 v-if="selected=='Chapter5'" v-bind:op1="selected51"></qcBase5>
            <qcBase6 v-if="selected=='Chapter6'" v-bind:op1="selected61"></qcBase6>
            <qcBase701 v-if="selected=='Chapter7' && selected71=='C701'" v-bind:op1="selected72"></qcBase701>
            <qcBase702 v-if="selected=='Chapter7' && selected71=='C702'" v-bind:op1="selected72"></qcBase702>
            <qcBase703 v-if="selected=='Chapter7' && selected71=='C703'" v-bind:op1="selected72"></qcBase703>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                selected: 'Chapter5',
                selected51: '',
                options51: [],
                selected61: '',
                options61: [],
                selected71: '',
                selected72: '',
                options72: [],
                optionFor71: [],
                optionFor72: [],
                optionFor73:[],
            };
        },
        components: {
            qcBase5: require('./QCStdBase5StdMaterialAndEquipment.vue').default,
            qcBase6: require('./QCStdBase6StdEquipmentFunction.vue').default,
            qcBase701: require('./QCStdBase701Stdconstruction.vue').default,
            qcBase702: require('./QCStdBase702Stdconstruction.vue').default,
            qcBase703: require('./QCStdBase703Stdconstruction.vue').default
        },
        methods: {
            mainOnChange(event) {
                console.log('selected: ' + event.target.value);
                //console.log('selected: ' + this.selected);
                this.selected51 = '';
                this.selected61 = '';
                this.selected71 = '';
                this.selected72 = '';
                if (this.selected == 'Chapter6' && this.options61.length == 0) {
                    this.getOptions61();
                }
            },
            async getOptions51() {
                const { data } = await window.myAjax.post('/QCStdTp/EMDListTp');
                this.options51 = data;
            },
            async getOptions61() {
                const { data } = await window.myAjax.post('/QCStdTp/EOTListTp');
                this.options61 = data;
            },
            onChange71(event) {
                console.log('onChange7: ' + event.target.value);
                console.log('selected71: ' + this.selected71 + ' selected72: ' + this.selected72);
                this.options72 = [];
                this.selected72 = '';
                if (this.selected71 == 'C701') {
                    if (this.optionFor71.length > 0) {
                        this.options72 = this.optionFor71;
                    } else {
                        this.getOptions701();
                    }
                } else if (this.selected71 == 'C702') {
                    if (this.optionFor72.length > 0) {
                        this.options72 = this.optionFor72;
                    } else {
                        this.getOptions702();
                    }
                } else if (this.selected71 == 'C703') {
                    if (this.optionFor73.length > 0) {
                        this.options72 = this.optionFor73;
                    } else {
                        this.getOptions703();
                    }
                }
                /*if (this.options72.length > 0) {
                    this.selected72 = this.options72[0].value;
                }*/
            },
            async getOptions701() {
                const { data } = await window.myAjax.post('/QCStdTp/CCListTp');
                this.optionFor71 = data;
                this.options72 = this.optionFor71;
            },
            async getOptions702() {
                const { data } = await window.myAjax.post('/QCStdTp/ECListTp');
                this.optionFor72 = data;
                this.options72 = this.optionFor72;
            },
            async getOptions703() {
                const { data } = await window.myAjax.post('/QCStdTp/OSHListTp');
                this.optionFor73 = data;
                this.options72 = this.optionFor73;
            }
        },
        async mounted() {
            console.log('mounted() QCStdBase');
            if (this.options51.length == 0) {
                this.getOptions51();
            }
        }
    }
</script>