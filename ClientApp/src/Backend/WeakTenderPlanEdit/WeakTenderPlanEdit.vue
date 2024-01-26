<template>

	<div>

		<div class="row">
			<div class="col-2">
				<div class="input-group">
					<label class="input-group-text" for="inputGroupSelect02">年度</label>
					<select class="form-control" v-model="selectedTenderYear" id="inputGroupSelect02">
						<option v-for="option in TenderYearList" v-bind:value="option" v-bind:key="option">
							{{ option }}
						</option>
					</select>
				</div>
			</div>
			<div class="col-4">
				<div class="input-group">
					<label class="input-group-text" for="inputGroupSelect01">所屬機關</label>
					<select class="form-control" id="inputGroupSelect01" v-model="selectedUnit">
						<option v-for="option in unitList" v-bind:value="option.Seq" v-bind:key="option">
							{{ option.Name }}
						</option>
					</select>
				</div>
			</div>
			<div class="col-3">
				<input type="text" class="form-control" placeholder="請輸入標案編號" aria-describedby="button-addon2"
					v-model="searchEng">

			</div>
			<div class="col-3 ">
				<button class="btn btn-primary" type="button" id="button-addon2" @click="Save()">設定</button>
			</div>
		</div>
		<div class="row" style="margin-top:20px">
			<select class="form-control" multiple aria-label="multiple select example" style="width:100%">
				<option v-for="option in engViewList" v-bind:value="option" @click="selectEng(option)"
					v-bind:key="option">
					{{ option }}
				</option>

			</select>
		</div>
		<div class="row">
			<table class="table table-responsive-md table-hover">
				<thead>
					<td>#</td>
					<td>標案編號</td>
					<td>標案名稱</td>
					<td>所屬機構</td>
					<td>刪除</td>
				</thead>
				<tbody>
					<tr v-for="(item, index) in engUnitList" :key="item">
						<td>{{ index + 1 }}</td>
						<td>{{ item.TenderNo }}</td>
						<td>{{ item.TenderName }}</td>
						<td>{{ item.UnitName }}</td>
						<td>
							<a @click="Delete(item)" class="btn btn-color11-4 btn-xs sharp mx-1" title="刪除"><i
									class="fas fa-trash-alt"></i></a>
						</td>
					</tr>
				</tbody>
			</table>
		</div>
		<div style="width:99%; margin-top:50px" class="row justify-content-center">
			<!--v-on:change="onPageChange"-->
			<b-pagination :total-rows="totalRows" :per-page="perPage" v-model="currentPage">
			</b-pagination>
			<!--<div>
              目前的排序鍵值: <b>{{ sortBy }}</b>, 排序方式:
              <b>{{ sortDesc ? 'Descending' : 'Ascending' }}</b>
          </div>-->
		</div>
	</div>

</template>

<script>


import axios from "axios";
export default {
	watch: {
		searchEng: {
			handler: function (value) {
				if (value == "") this.engViewList = this.engList;
				this.engViewList = this.engList.filter((element) => element.includes(value));
				this.engViewList.sort((a, b) => a.indexOf(value) - b.indexOf(value));
			}
		},
		currentPage: {
			handler: function (value) { this.getList(); }
		},
		selectedUnit: {
			handler: function (value) {
				console.log("selectedUnit", value)
				this.getEngList(value)
			}
		},
		selectedTenderYear: {
			handler: function (value) {
				this.getUnitListByTenderYear(value);
			}
		}
	}
	,
	data: () => {

		return {
			isEdit: 0,
			totalRows: 0,
			currentPage: 1,
			perPage: 10,
			unitList: [],
			engList: [],
			engViewList: [],
			searchEng: null,
			selectedUnit: null,
			engUnitList: [],
			selectedTenderYear: null,
			TenderYearList: []
		}
	},
	methods: {
		async Delete(item) {

			if (!confirm("您確定要刪除嗎?")) return;

			let res = await window.myAjax.delete(`WeakTenderPlanEdit/Delete/${item.Seq}`);
			if (res.data.status == "success") {
				console.log("delete");
				this.getList();
			}
			// this.items = this.items.filter(element => element.Seq != item.Seq);
		},
		selectEng(eng) {
			this.searchEng = eng;
		},
		async Save() {
			let form = new FormData();
			form.append("tenderNo", this.searchEng);
			form.append("execUnit", this.selectedUnit);
			if (!this.searchEng || !this.selectedUnit || !this.engViewList.includes(this.searchEng)) {
				alert("設定失敗");
				return;
			}
			let res = await window.myAjax.post("WeakTenderPlanEdit/Add", form);

			if (res.data.status == "success") {
				alert("設定成功");
				this.currentPage = 1;
				this.getList();
			}

		},
		async getList() {
			let res = await window.myAjax.post("WeakTenderPlanEdit/getMajorEng", {


					page: this.currentPage,
					per_page: this.perPage
				
			});
			if (res.data.status == "success") {

				this.engUnitList = res.data.l;
				this.totalRows = res.data.t;
			}
		},
		async getEngList(unitSeq = "") {

			let res = await window.myAjax.post("WeakTenderPlanEdit/getEngList", {
				
					unitSeq: unitSeq,
					year : this.selectedTenderYear
			});

			if (res.data.status == "success") {
				this.engList = res.data.data;
				this.engViewList = this.engList;
				console.log("eng", this.engList);
			}
		},
		async getUnitListByTenderYear(value) {

			let res = await window.myAjax.post("WeakTenderPlanEdit/getUnitListByTenderYear", {
				
					tenderYear: value
				
			});
			if (res.data.status == "success") {

				this.unitList = res.data.data;
				console.log("unit", this.unitList);
			}
		}

	},
	async mounted() {
		let res = await window.myAjax.post("WeakTenderPlanEdit/GetTenderYearList");

		if (res.data.status == "success") {

			this.TenderYearList = res.data.data;
			console.log("unit", this.TenderYearList);
		}
		this.getList();


	}

}

</script>