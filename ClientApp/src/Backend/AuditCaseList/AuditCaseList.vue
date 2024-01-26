<template>

	<div>
		<ImportTemplate route="AuditCaseList" :hasSearch="true" searchSign="可輸入工程名稱查詢" 
		:hasEdit="true"
		:hasDelete="true"
		:detail="{
			ActualBidAwardDate: true,
			ContractAmount : true,
			InformMethod : true,
			EngType : true,
			AuditUnitSugesstion : true,
			ECUnitRealResponse : true,
			PSIssueNoDate : true,
			ApprovalNoDate : true,
			Momo : true
		}" @showModal="showDetail" />
		<div class="modal fade show" id="MyDialog" ref="MyDialog" style="background:rgb(0 0 0 / 50%) ;overflow:auto"
			v-bind:style="{ display: showModal ? 'block' : 'none' }" tabindex="-1" role="dialog"
			aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
			<div class="modal-dialog modal-dialog-centered" role="document">
				<div class="modal-content">
					<div class="modal-header bg-R text-white">
						<h6 class="modal-title">詳細資訊</h6>
					</div>
					<div class="modal-body">

				
							<!-- Form Group (first name)-->
							<div class="row  m-2" style="border:solid 1px">
								<label class="p-2 mb-0" style="background:lightgray;width:30%">實際決標日</label>
								<p class="p-2 mb-0" style="word-wrap:break-word">
									{{
											detail.ActualBidAwardDate
									}} </p>
							</div>
							<!-- Form Group (last name)-->
							<div class="row  m-2" style="border:solid 1px">
								<label class="p-2 mb-0" style="background:lightgray;width:30%">契約金額</label>
								<p class="p-2 mb-0" style="word-wrap:break-word">
									{{ detail.ContractAmount }}
								</p>
							</div>
				
						
							<!-- Form Group (first name)-->
							<div class="row  m-2" style="border:solid 1px">
								<label class="p-2 mb-0" style="background:lightgray;width:30%">通知方式</label>
								<p class="p-2 mb-0" style="word-wrap:break-word">
									{{ detail.InformMethod }}
								</p>
							</div>
							<!-- Form Group (last name)-->
							<div class="row  m-2" style="border:solid 1px">
								<label class="p-2 mb-0" style="background:lightgray;width:30%">工程類別</label>
								<p class="p-2 mb-0" style="word-wrap:break-word">
									{{ detail.EngType }}
								</p>
							</div>
					
						
							<!-- Form Group (first name)-->
							<div class="row  m-2" style="border:solid 1px">
								<label class="p-2 mb-0" style="background:lightgray;width:100%">查核建議處置</label>
								<p class="p-2 mb-0" style="word-wrap:break-word ">
									{{ detail.AuditUnitSugesstion }}
								</p>
							</div>
							<!-- Form Group (last name)-->
							<div class="row  m-2" style="border:solid 1px">
								<label class="p-2 mb-0" style="background:lightgray;width:100%">機關實際處置</label>
								<p class="p-2 mb-0" style="word-wrap:break-word">
									{{ detail.ECUnitRealResponse }}
								</p>
							</div>

			


						<div class="row  m-2" style="border:solid 1px">
							<label class="p-2 mb-0" style="background:lightgray;width:100%">發文日期文號</label>
							<p class="p-2 mb-0" style="word-wrap:break-word">
								{{ detail.PSIssueNoDate }}
							</p>
						</div>
						<div class="row  m-2" style="border:solid 1px">
							<label class="p-2 mb-0" style="background:lightgray;width:100%">回文日期文號</label>
							<p class="p-2 mb-0" style="word-wrap:break-word">
								{{ detail.ApprovalNoDate }}
							</p>
						</div>
						<div class="row  m-2" style="border:solid 1px">
							<label class="p-2 mb-0" style="background:lightgray;width:100%">備註 </label>
							<p class="p-2 mb-0" style="word-wrap:break-word">
								{{ detail.Momo }}
							</p>
						</div>
					</div>
					<div class="modal-footer">
						<button type="button" class="btn btn-color9-1 btn-xs mx-1" data-dismiss="modal"
							@click="showModal = false" v-on:click="showModal = false"><i class="fas fa-times"></i>
							關閉</button>
					</div>
				</div>
			</div>
		</div>
	</div>
</template>


<script>
import { define } from "mime";
import ImportTemplate from "../../components/ImportTemplate";
export default {
	data: () => {
		return {
			showModal: false,
			
			detail: {}
		}
	},
	components: {
		ImportTemplate
	},
	methods: {
		showDetail(item) {
			this.detail = item;
			this.showModal = true;
		}
	}
}
</script>