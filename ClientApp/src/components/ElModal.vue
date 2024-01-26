<script setup>
    import { defineExpose, ref, defineProps } from 'vue';
    const show = ref(false);
    const props = defineProps(["title"]);
    defineExpose({
        show

    })
</script>

<template>

<el-dialog :title="props.title" :visible.sync="show" width="80%" >
    <slot name="content">
        error
    </slot>
</el-dialog>
</template>