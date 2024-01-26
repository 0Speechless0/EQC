<script setup>
    import {ref, defineExpose, defineProps, watch} from "vue";
    const show = ref(false);
    const props = defineProps(["title", "widthRatio", "headerColor"]);

    const modalClass =ref("modal fade");
    watch(() => show.value, (value) => {
        
        if(value)
        {
            modalClass.value = modalClass.value.concat(" show");
        }
        else{
            modalClass.value = modalClass.value.slice(0, 10);
        }

    })
    defineExpose({
        show
    })
</script>
<template>

<div  :style="{'display': show ? 'block' : 'none', background: 'rgb(0 0 0 / 50%)', overflow:'auto' }" 
    :class="modalClass"  data-backdrop="static"  data-keyboard="false" tabindex="-1" aria-labelledby="exampleModalLabel" aria-modal="true" aria-hidden="true">
        <div class="modal-dialog modal-lg" :style="widthRatio ? {'max-width': widthRatio +'%'} : {}">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 id="projectUpload" class="modal-title" :style="{color: props.headerColor}">{{ props.title }}</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close" @click="show = false"> 
                        <span aria-hidden="true" >×</span>
                    </button>
                </div>
                <div class="modal-body">
                    <slot name="body">
                            ㄎㄎ
                    </slot>
                </div>
                <div class="modal-footer" >
                    <slot name="footer">
                        
                    </slot>
                </div>
            </div>
        </div>
    </div>

</template>

<style scoped>
    body {
        overflow-y: hidden !important;
    }
</style>