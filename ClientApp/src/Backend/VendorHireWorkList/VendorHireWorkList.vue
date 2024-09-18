<template>
	
	<div>
		<div class="d-flex justify-content-center mt-2">
			<div class="row col-2 form-inline">
				<div class="p-1">開始年度</div>
				<input v-model="year" type="number" placeholder="全部" class="form-control col-6"  />
			</div>

			<button role="button" class="btn btn-shadow btn-color11-3 btn-block col-1" @click="importFromPrj"> 從工程會匯入 </button>
		</div>	

		<ImportTemplate route="VendorHireWorkList"
		ref="ImportTemplate"
		:hasSearch="true"
		:hasEdit="true"
		:hasDelete="true"
		searchSign="可輸入承攬廠商或工程名稱查詢"/>
	</div>
</template>


<script >
import ImportTemplate from "../../components/ImportTemplate";
export default {
	components: {
		ImportTemplate
	},
	data : () => {
			return {
				year : null
			}
		},
	methods :{

		async importFromPrj()
		{
			alert("匯入範圍較大時，需較長時間，請耐心等候...")
			let {data : res} = await window.myAjax.post("VendorHireWorkList/ImportFromPrj", {year : this.year})
			if(res == true) {
				alert('匯入成功')
				this.$refs.ImportTemplate.getList();
			}
		}
	}
}
</script>