<template>
	
	<div>
		<ImportTemplate route="PublicWorkFirmResume"
		:noHint="true"
		:hasEdit="true"
		:hasSearch="true"
		searchSign="請輸入廠商名稱"
		:hasDelete="true"
		:FieldNameChange="{
			CorporationName : '廠商'

		}"
/>
	</div>
</template>


<script>
import ImportTemplate  from "../../components/ImportTemplate";
export default {
	components: {
		ImportTemplate
	},
	
}
</script>