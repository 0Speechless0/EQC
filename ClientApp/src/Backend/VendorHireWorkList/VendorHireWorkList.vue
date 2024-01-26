<template>
	
	<div>
		<ImportTemplate route="VendorHireWorkList"
		:hasSearch="true"
		:hasEdit="true"
		:hasDelete="true"
		searchSign="可輸入承攬廠商或工程名稱查詢"/>
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