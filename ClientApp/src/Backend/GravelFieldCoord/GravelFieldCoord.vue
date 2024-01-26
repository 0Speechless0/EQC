<template>
	
	<div>
		<ImportTemplate route="GravelFieldCoord"
		:hasSearch="true"
		:hasEdit="true"
		:hasDelete="true"
		searchSign="可輸入廠商名稱查詢"
		/>
	</div>
</template>


<script>
import ImportTemplate from "../../components/ImportTemplate";
export default {
	components: {
		ImportTemplate
	}
}
</script>