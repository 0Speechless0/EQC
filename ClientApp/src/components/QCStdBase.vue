<template>
    <div>
        <div class="form-row">
            <div class="col-md-auto">
                <select class="form-control" v-model="selected" @change="mainOnChange($event)">
                    <option value="stdBase5">第五章 材料與設備抽驗程序及標準</option>
                    <option value="stdBase6">第六章 設備功能運轉測試抽驗程序及標準</option>
                    <option value="stdBase7">第七章 施工抽查程序及標準</option>
                </select>
            </div>
            <div v-if="selected=='stdBase5'">
                <select class="form-control" v-model="selected51" @change="onChange5($event)">
                    <option v-for="option in options51" v-bind:value="option.Value" v-bind:key="option.Value">
                        {{ option.Text }}
                    </option>
                </select>
            </div>
            <div v-if="selected=='stdBase6'">
                <select class="form-control" v-model="selected61" @change="onChange6($event)">
                    <option v-for="option in options61" v-bind:value="option.Value" v-bind:key="option.Value">
                        {{ option.Text }}
                    </option>
                </select>
            </div>
            <div v-if="selected=='stdBase7'" class="col form-row">
                <div class="col-md-auto">
                    <select class="form-control" v-model="selected71" @change="onChange71($event)">
                        <option class="u3977_input_option" value="C701">施工抽查管理標準</option>
                        <option class="u3977_input_option" value="C702">職業安全衛生管理標準</option>
                        <option class="u3977_input_option" value="C703">環境保育管理標準</option>

                    </select>
                </div>
                <div class="col">
                    <select class="form-control" v-model="selected72" @change="onChange72($event)">
                        <option v-for="option in options72" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
            </div>
        </div>
        <div>
            <qcBase5 v-if="selected=='stdBase5'" v-bind:op1="selected51"></qcBase5>
            <qcBase6 v-if="selected=='stdBase6'" v-bind:op1="selected61"></qcBase6>
            <qcBase701 v-if="selected=='stdBase7' && selected71=='C701'" v-bind:op1="selected72"></qcBase701>
            <qcBase702 v-if="selected=='stdBase7' && selected71=='C702'" v-bind:op1="selected72"></qcBase702>
            <qcBase703 v-if="selected=='stdBase7' && selected71=='C703'" v-bind:op1="selected72"></qcBase703>
        </div>
    </div>
</template>
<script>
    //import axios from 'axios';
    //var myAjax = axios.create({ baseURL: window.location.origin });
    export default {
        data: function () {
            return {
                selected: 'stdBase5',
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
                console.log('selected: ' + this.selected);
                this.selected51 = '';
                this.selected61 = '';
                this.selected71 = '';
                this.selected72 = '';
                if (this.selected == 'stdBase6' && this.options61.length == 0) {
                    this.getOptions61();
                }
            },
            onChange5(event) {
                console.log('onChange6: ' + event.target.value);
                console.log('selected61: ' + this.selected61);
            },
            async getOptions51() {
                const { data } = await window.myAjax.post('/QCStdBase/EMDListTp');
                this.options51 = data;
            },
            onChange6(event) {
                console.log('onChange6: '+event.target.value);
                console.log('selected61: ' + this.selected61);
            },
            async getOptions61() {
                const { data } = await window.myAjax.post('/QCStdBase/EOTListTp');
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
                const { data } = await window.myAjax.post('/QCStdBase/CCListTp');
                this.optionFor71 = data;
                this.options72 = this.optionFor71;
            },
            async getOptions702() {
                const { data } = await window.myAjax.post('/QCStdBase/OSHListTp');
                this.optionFor72 = data;
                this.options72 = this.optionFor72;
            },
            async getOptions703() {
                const { data } = await window.myAjax.post('/QCStdBase/ECListTp');
                this.optionFor73 = data;
                this.options72 = this.optionFor73;
            },
            onChange72(event) {
                console.log('onChange7: ' + event.target.value);
                console.log('selected71: ' + this.selected71 + ' selected72: ' + this.selected72);
            }
         /*   async fetchUsers() {
                //const { data } = await axios.get('https://jsonplaceholder.typicode.com/users');
                //const { data } = await axios.post('http://localhost:49869/Home/SampleData');
                const { data } = await window.myAjax.post('/Home/SampleData');
                this.users = data;
            },
            newItem() {
                console.log('newItem()');
                var newRow = { seq: -1 * Date.now(), age:null, firstName:"", lastName:"", edit:true };
                this.users.push(newRow);
            },
            saveItem(item) {
                item.edit = false;
            },
            delItem(index, id) {
                //item.edit = false;
                console.log('index: ' + index + ' seq: ' + id);
                this.users.splice(index, 1);
            }*/
        },
        async mounted() {
            console.log('mounted() QCStdBase');
            //await this.fetchUsers();
            if (this.options51.length == 0) {
                this.getOptions51();
            }
        }
    }
</script>