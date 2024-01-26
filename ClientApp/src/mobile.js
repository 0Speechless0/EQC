import Vue from 'vue';
import axios from 'axios';

Vue.config.devtools = true;//瀏覽器外掛 Vue 監視啟用

Vue.component('mb-eng', require('./Mobile/Eng.vue').default);
Vue.component('mb-list', require('./Mobile/List.vue').default);
window.myAjax = axios.create({ baseURL: window.location.origin });