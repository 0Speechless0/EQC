<template>
	
	<div>
		<ImportTemplate route="QualityDeductionPoints"
		searchSign="可輸入缺失編號查詢"
		:hasSearch="true"
		:hasEdit="true"
		:hasAdd="true"
		:hasDelete="true"
				:mutiLineFields="[
			'Content'
		]"/>
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