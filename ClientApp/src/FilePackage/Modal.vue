<script setup>
    import {defineProps, onMounted, reactive, ref, watch } from "vue";
    const props = defineProps(["systemType", "seq", "btnclass"])
    const modal = ref(null);
    const store = reactive({

        seq : props.seq,
        keyWord : "",
        menuGroupedList : {},
        actions : []
        
    })

    watch(() => props.systemType, (v) =>  getList(store.keyWord, v), {immediate : true});

    function checkboxListChange()
    {
      store.actions = Object.entries(store.menuGroupedList)
        .reduce((a, [key, listObject]) => {
          listObject.list.filter(e => e.inGroup).forEach(e => {
          a.push(e);

        })

        console.log("list", a);
        return a;
      }, []);

      console.log("ffff", store.actions);
    }
    function checkList(listObject)
    {
      console.log("fgfdgdfg", listObject);
      listObject.list.forEach(e => {

        e.inGroup =  listObject._inGroup;
      });
      listObject.list.concat();
      checkboxListChange();
    }
    
    async function getList(keyWord, systemType)
    {
      var originGroupedList = Object.assign({}, store.menuGroupedList);
      store.menuGroupedList = {};
      Object.entries(
      
      (await window.myAjax.post("PackageDownloadAction/GetList", {
          systemType : systemType ?? 3,
          keyWord : keyWord,
          engSeq : props.seq ?? 194
        }) ).data )
        .forEach( ([key, list]) => {
          store.menuGroupedList[key] = {
             list : list ,
             _inGroup : false
          }

        });
      console.log('menuGroupedList', store.menuGroupedList)
      Object.keys(originGroupedList)
      .forEach(key => {
        var a = originGroupedList;
        a[key].list = a[key] ? a[key].list.filter(e => e.inGroup) : []

        if(a[key].list.length > 0)
        {
          
          store.menuGroupedList[key].list = a[key].list.concat(store.menuGroupedList[key].list)
        }
        else if(store.menuGroupedList[key])
          a[key].list.forEach(e => {store.menuGroupedList[key].list.push(e) ; })
          
        
      })


      store.menuGroupedList = Object.assign({}, store.menuGroupedList);
    }

    async function onDownloadAction(item)
    {
        let resp =  (await window.myAjax.post("PackageDownloadAction/ExecuateOne", {
          engSeq : props.seq ?? 264,
          seq : item.Seq,
          StartDate : item.StartDate,
          EndDate : item.EndDate

        }) ).data;
        if(resp) {
          alert(resp.message);
        }
    }

    async function onDownloadActionMuti()
    {
        let resp =  (await window.myAjax.post("PackageDownloadAction/ExecuateMutiple", {
          engSeq : props.seq ?? 264,
          actions : store.actions

        }) ).data;
        if(resp) {
          alert(resp.message);
        }
    }
    onMounted(() => {

        var Div = document.getElementById(`window${props.seq}`).parentNode;
        var button = document.createElement("button");
        Div.appendChild(button);
        button.outerHTML = `
            <button class="${props.btnclass}" data-toggle="modal" data-target="#window${props.seq}" >
                <i class="fas fa-download"></i> 報表
            </button>
        `;
    })
    watch(() => store.keyWord, (v) => {
      getList(v, props.systemType);
    })
    
</script>

<template>
  <div
    :class="`modal fade`"
    :id="`window`+props.seq"
    style=" padding-right: 21px;"
    aria-modal="true"

  >
    <div
      class="modal-dialog modal-xl modal-dialog-centered"
      style="max-width: fit-content ;"
      ref="modal"
    >
      <div class="modal-content">
        <div class="card whiteBG mb-4 pattern-F colorset_1">
          <div class="tab-content">
            <div style="padding-top: 15px">
              <label>關鍵字搜尋 <span style="color:red">(按 "Enter" 查詢)</span></label>
              <input
                v-model.lazy="store.keyWord"
                type="text"
                id="keywordInput"
                class="form-control"
                placeholder="輸入關鍵字"
                onchange.lazy="getList(store.keyWord)"
              />
            </div>
            <div style="padding-top: 15px">
              <p>
                報表的使用說明 :
              </p>
              <p style="margin:10px">
                1. 提供系統內各功能產製的檔案，如碳簡易檢核表、施工日誌等
              </p>
              <p style="margin:10px">
                2. 提供使用者於功能中上傳的檔案，如pcces檔案、生態檢核資料
              </p>
              <p style="margin:10px">
                3. 提供透過系統產製檔案(但需已經在要功能中點選產製檔案)，如監造計畫書、施工風險報告書
              </p>
            </div>
            <div class="tab-pane active">
              <div v-for="([key, listObject], index) in Object.entries(store.menuGroupedList)" :key="index">
                <h5 style="text-align: left; display: block">{{ key }}</h5>
                <table
                  border="0"
                  class="table table1 min910"
                  style="display: table"
                >
                  <thead class="insearch">
                    <tr>
                      <th style="text-align: center; width: 10%">
                        <input
                          type="checkbox"
                          v-model="listObject._inGroup"
                          @change="checkList(listObject)"
                          class="form-control-input"
                        />
                        勾選
                      </th>
                      <th class="sort">排序</th>
                      <th>報表名稱</th>
                      <th class="col-4" style="text-align: center">下載</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(item, index2) in listObject.list" :key="index2">
                      <td>
                        <div>
                          <input
                            type="checkbox"
                            @change="checkboxListChange(item)"
                            v-model="item.inGroup"
                            class="form-control-input"
                          />
                        </div>
                      </td>
                      <td>{{ index2+1 }}</td>
                      <td>{{ item.Name }}</td>
                      <td class="d-flex justify-content-center">
                        <span class="d-flex" v-if="item.hasDateInput">
                          <input class="form-control" type="date" placeholder="yyyy/MM/dd" v-model="item.StartDate"/>~
                          <input class="form-control" type="date" v-model="item.EndDate"/>
                        </span>


                          <button
                          @click="onDownloadAction(item)"
                          title="下載"
                          class="btn btn-color11-1 btn-xs mx-1"
                        >
                          <i class="fas fa-download"></i> 
                        </button>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
          <div style="text-align: right; padding: 10px">

            <button
              title="下載"
              class="btn btn-color11-1 btn-xs mx-1"
              style="margin-left: initial"
              data-dismiss="modal"
              @click="onDownloadActionMuti"
            >
              <i class="fas fa-download"></i>
              下載
            </button>
            <button
              title="關閉"
              class="btn btn-color9-1 btn-xs mx-1"
              style="margin-left: initial"
              data-dismiss="modal"

            >
              <i class="fas fa-xmark"></i>
              關閉
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
