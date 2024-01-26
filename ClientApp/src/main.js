import Vue, { createApp } from 'vue'

import VueAxios from 'vue-axios'
import axios from 'axios'
//import axios from '@/js/axios'

import ElementUI from 'element-ui';
import 'element-ui/lib/theme-chalk/index.css';
Vue.use(ElementUI);

import VueRouter from 'vue-router'
import App from './App.vue'



//import datePicker from 'vue-bootstrap-datetimepicker'
//import router from './router'

import { BootstrapVue, BootstrapVueIcons, BTable} from 'bootstrap-vue'


import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'

import VCalendar from 'v-calendar';     // �ޤJ��䴡��

// �ޤJ���J������
import Loading from 'vue-loading-overlay'
// �ɤJ�˦�
import 'vue-loading-overlay/dist/vue-loading.css'

import '@/assets/css/layout.css';
import '@/assets/css/all.css';



import './mobile.js' // �����
//API設定

import CKEditor from '@ckeditor/ckeditor5-vue2';

Vue.use( CKEditor );

// import './assets/js/jquery.blockUI.js'

import VueResource from 'vue-resource'

import VueSweetalert2 from 'vue-sweetalert2';
import 'sweetalert2/dist/sweetalert2.min.css';
Vue.use(VueSweetalert2);
// ��l�ƴ���
//Vue.use(Loading);
Vue.use(Loading, {
    canCancel: true,
    onCancel: oncancel,
    color: '#000000',
    loader: 'dots',     //spinner/dots/bars
    width: 50,
    height: 50,
    backgroundColor: '#ffffff',
    isFullPage: true,
    opactity: 0.8
});
function oncancel() {
    alert('����Loading...')
}

// ��ƾ�
Vue.use(VCalendar, {
    componentPrefix: 'vc',
})

//import Vuex, { Store } from 'vuex'
//Vue.use(Vuex);

// idle-vue
//import IdleVue from 'idle-vue';
//import App from './App.vue';
//Vue.use(Vuex);
//import { state, createStore } from './store';

//const eventsHub = Vue;

// ���Ψ禡-Common


import common from './Common/Common'
Vue.prototype.common = common

Vue.config.productionTip = false


// Install BootstrapVue
Vue.use(BootstrapVue)


//Vue.use(VueRouter)

Vue.component('b-table', BTable)

// Optionally install the BootstrapVue icon component plugin
Vue.use(BootstrapVueIcons)


//Vue.component('App', require('./App.vue').default);
// �e�x
// ��x

Vue.component('Login', require('./Backend/Login/Login.vue').default);
Vue.component('user-list', require('./Backend/UserManager/Index.vue').default);
Vue.component('role-list', require('./Backend/Role/RoleList.vue').default);
Vue.component('menu-list', require('./Backend/Menu/MenuList.vue').default);

//�������u
Vue.component('national-supervised-activity', require('./Backend/NationalSupervisedActivity/NationalSupervisedActivity.vue').default);
Vue.component('gravel-field-coord', require('./Backend/GravelFieldCoord/GravelFieldCoord.vue').default);
Vue.component('audit-case-list', require('./Backend/AuditCaseList/AuditCaseList.vue').default);
Vue.component('vendor-hire-work-list', require('./Backend/VendorHireWorkList/VendorHireWorkList.vue').default);
Vue.component('ey-central-local-address-list', require('./Backend/EYCentralLocalAddressList/EYCentralLocalAddressList.vue').default);
Vue.component('public-work-firm-resume', require('./Backend/PublicWorkFirmResume/PublicWorkFirmResume.vue').default);
Vue.component('weak-tender-plan-edit', require('./Backend/WeakTenderPlanEdit/WeakTenderPlanEdit.vue').default);
Vue.component('control-plan', require('./Backend/ControlPlan/ControlPlan.vue').default);
Vue.component('unit-cities', require('./Backend/UnitCities/UnitCities.vue').default);
Vue.component('quality-deduction-points', require('./Backend/QualityDeductionPoints/QualityDeductionPoints.vue').default);
//�������u end

//�I�u�v����Ʈw
Vue.component('construction-image', require('./EngAD/ConstructionImage/ConstructionImage.vue').default);
//�P�M��
Vue.component('court-verdict', require('./Backend/CourtVerdict/CourtVerdict.vue').default);
//���K
Vue.component('thsr', require('./Backend/thsr/thsr.vue').default);
Vue.component('technical-main-page', require('./Technical/TechnicalMainPage.vue').default);

// ����
Vue.component('s-identify', require('./components/SIdentify.vue').default);

//Vue.component('list-item', {
//    template: '\
//            {{ title }}\
//            <button v-on:click="$emit(\'remove\')">�R��</button>\
//            ',
//            props:['title']
//}),
Vue.use(VueAxios, axios);


Vue.config.debug = true;
Vue.config.devtools = true;

window.Vue = Vue;   // ���쳣�i�H���oVue
//window.Vuex = Vuex;
//window.VueRouter = VueRouter;




window.App = App;


//window.router = router;

// response �d�I��
var loader = null;



axios.interceptors.request.use(
    (config) => {
        //console.log('�d�I�ШD');
        //store.commit('Set_Loading', true);
        if (!loader) {
            loader = Vue.$loading.show();
        }
        //alert('�d�I�ШD');
        return config;
    },
    (error) => {
        //console.log('�d�I��request����');
        hideLoader();
        //alert('�d�I��request����')
        return Promise.reject(error);
    }
);

function hideLoader() {
    loader && loader.hide();
    loader = null;
}

axios.interceptors.response.use(
    (response) => {
        //console.log('�d�I�^��');
        hideLoader();
        //store.commit('Set_Loading', false);
        //alert('�d�I�^��')
        if (response.data.result == -1919) {
            window.location.href = response.data.url;
            return null;
        }
        return response;
    },
    (error) => {
        //console.log('�d�I��response����')
        hideLoader();
        //alert('�d�I��response����')
        return Promise.reject(error);
    }
); 



//這裡設定路由，避免衝突，不同人使用不同檔案引入
import './sub1.js'; // �Ф�ʥ[�J shioulo, alex
import './sub2.js'; // �Ф�ʥ[�J alex




import {useStaticStore } from "./store.js";
if(process.env.NODE_ENV == 'production')
{
    window.useStaticStore = useStaticStore;
}


import './test.js';

