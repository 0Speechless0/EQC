<template>
    <div>
        <div class="nav nav-tabs border-0 ">
            <div class="nav nav-tabs border-0 col-12" v-for="(codeOption, index2) in codeOptionStack" :key="index2">
                <div class="nav-item" v-for="(option, index) in codeOption" :key="index">
                    <button  data-toggle="tab" v-on:click.stop="OnChange(option, index2)" href="#" :class="`btn-block  btn btn-color4 ${optionSelectedStack[index2] == option ?'active' : '' }`" title="第五章 材料與設備抽驗程序及標準">{{ option }}</button>
                </div>
            </div>


        </div>
        <div>
            <CarbonFactorView :CodePrefix="prefix">

            </CarbonFactorView>
        </div>

    </div>
</template>
<script>
    import CarbonFactorView from "./CarbonFactorView.vue";
    export default {
        data: function () {
            return {
                optionSelectedStack: [],
                codeOptionStack: [],
                lastSelectedLevel : 0,
                prefix : null,
                codePrefix :null
            };
        },
        components: {
            CarbonFactorView: CarbonFactorView,

        },
        methods: {
            async OnChange(optionSelected, index2) {

                if(this.lastSelectedLevel > index2)
                {
                    this.optionSelectedStack.forEach((selectedOpt, index) => {
                        if(index2 < index) this.optionSelectedStack[index] = "";
                    })
                }
                this.optionSelectedStack[index2] = optionSelected;
                this.prefix = optionSelected;
                if(optionSelected.length != 5)
                {

                    var newOptions = (await window.myAjax.post("CarbonEmissionView/getCarbonEmissionCodeOption", { prefix : optionSelected })).data;
                    this.codeOptionStack[index2+1] = newOptions;
                    this.codeOptionStack = this.codeOptionStack.slice(0, index2+2);
                }
                console.log(this.optionSelectedStack);
                this.lastSelectedLevel = index2;
            }

        },
        async mounted() {
            console.log('mounted() FlowChart');
            var newOptions = (await window.myAjax.post("CarbonEmissionView/getCarbonEmissionCodeOption")).data;
            this.codeOptionStack.push(newOptions);
            /*if (this.options51.length == 0) {
                this.getOptions51();
            }*/
        }
    }
</script>