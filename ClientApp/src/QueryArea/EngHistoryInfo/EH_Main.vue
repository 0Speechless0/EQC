 <template>
     <div>
         <ul class="nav nav-tabs" role="tablist">
             <li class="nav-item">
                 <button v-on:click="selectTab='TabEng'" class="nav-link active" data-toggle="tab" href="##">工程名稱</button>
             </li>
             <li class="nav-item">
                 <button v-on:click="selectTab='TabCompany'" class="nav-link" data-toggle="tab" href="##">廠商名稱</button>
             </li>  
         </ul>
         <TabEng v-if="selectTab=='TabEng'"></TabEng>
         <TabCompany v-if="selectTab=='TabCompany'"></TabCompany>
     </div>
</template>
<script>
    export default {
        data: function () {
            return {
                selectTab:'TabEng',
            };
        },
        components: {
            TabEng: require('./TabEng.vue').default,
            TabCompany: require('./TabCompany.vue').default,
        },
        methods: {
        },
        async mounted() {
            console.log('mounted 水利工程履歷管理');
        }
    }
</script>