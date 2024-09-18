<script setup>
import ImportTemplate from "../../../components/ImportTemplate";
</script>
<template>
    <div>
        <div>
		<ImportTemplate route="FarRegion"
		searchSign="可輸入地區關鍵字查詢"
		:hasSearch="true"
		:hasAdd="true"
		:hasDelete="true"
        primaryKey="TownCity"
        />
	</div>
    </div>
</template>