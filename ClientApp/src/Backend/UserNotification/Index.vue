<script setup>

    import Emit from './Emit.vue';
    import List from './List.vue';
    import Nav from "../../components/Nav.vue";
</script>

<template>
    <div>
            <Nav :tabs="[
            [ Emit, '發佈'],
            [ List, '檢視']
        ]"  currentTab="Emit" 
        >
            
        </Nav>
    </div>

</template>