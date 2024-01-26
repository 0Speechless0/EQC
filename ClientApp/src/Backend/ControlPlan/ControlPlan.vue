<template>
	
	<div>
		<ImportTemplate route="ControlPlan"
			:hasSearch="true"
			:hasEdit="true"
			:hasDelete="true"
			searchSign="可輸入計畫名稱查詢"/>
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