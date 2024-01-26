<template>
	
	<div>
		<ImportTemplate route="UnitCities"
			:hasSearch="true"
			searchSign="可輸入單位或縣市查詢"/>
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