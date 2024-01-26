<template>
<div class="modal fade" id="addDoor">
    <div class="modal-dialog modal-xl modal-dialog-centered " style="max-width: fit-content;">
        <div class="modal-content" style="width: 1000px;">
            <div class="card mb-4 pattern-F colorset_1">
                <div  style="padding: 0px 0 0px 20px;">
                    <h5>選擇類別</h5>
                    <!-- <h3 class="card-title font-weight-bold" style="background-color: #05a5c0; color: #fff; padding: 10px 0 10px 30px;">選擇類別</h3> -->
                </div>
                <div class="tab-content">
                        <!-- <h5>自行勾稽標管系統</h5> -->
                        <ul class="row justify-content-center BGwidth" >
                            <li class="col-2 col-md-2 col-xl-4 mb-2">
                                <a  @click.prevent="jump(1)">
                                    <div class="card bg-2 text-white">
                                        <div class="card-body">
                                            <h2 class="card-title m-0 p-0 ">透過PCCES匯入XML</h2>
                                        </div>
                                        <!-- <div class="card-img-bottom p-5 img04">
                                        </div> -->
                                    </div>
                                </a>
                            </li>
                            <li class="col-2 col-md-2 col-xl-4 mb-2">
                                <a  @click.prevent="jump(2)" >
                                    <div class="card bg-B text-white">
                                        <div class="card-body">
                                            <h2 class="card-title m-0 p-0 ">勾稽標管系統</h2>
                                        </div>
                                        <!-- <div class="card-img-bottom p-5 img05">
                                        </div> -->
                                    </div>
                                </a>
                            </li>
                            <li class="col-2 col-md-2  col-xl-4 mb-2 ">
                                <a  @click.prevent="jump(3)" >
                                    <div class="card bg-1 text-white">
                                        <div class="card-body">
                                            <h2 class="card-title m-0 p-0 ">自行新增非工程資料</h2>
                                        </div>
                                        <!-- <div class="card-img-bottom p-5 img03">
                                        </div> -->
                                    </div>
                                </a>
                            </li>
                        </ul>
                </div>
            </div>
        </div>
    </div>
</div>
</template>
<script>
export default{
    emits : [ "afterSelect"],
    data: () => {
        return {

        }
    },
    methods:{
        jump(target)
        {

            $("#addDoor").modal('hide');
            this.$emit("afterSelect", target);
        }
    }

}

</script>
<style>

</style>