<template>
	
	<div>
		<ImportTemplate route="EYCentralLocalAddressList"
			:searchStrList="areas"
			:hasSearch="true"
			:hasEdit="true"
			:hasDelete="true"
			searchSign="可輸入機關名稱查詢"
		
		
		/>
	</div>
</template>


<script>
import ImportTemplate from "../../components/ImportTemplate";
import axios from "axios";
export default {
	components: {
		ImportTemplate
	},
	data : () => {
		return {
			areas:[]
		}
	},
	methods:{
		async getOrganizerList(){
			let res = await axios.get("EYCentralLocalAddressList/GetOrganizerList");
			this.areas = res.data.l;
		}
	},
	mounted() {
		this.getOrganizerList();
	}
}
</script>