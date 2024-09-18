<template>
    <div>
        123
     </div>
</template>
<script>
    export default {
        methods: {
            editEng(id) {
                window.myAjax.post('/SupervisionPlan/EditEng', { seq: id })
                    .then(resp => {
                        console.log(resp.data.Url);
                        window.location.href = resp.data.Url;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        async mounted() {
            console.log('mounted() 監造計畫');
            let targetId = new URLSearchParams(window.location.search).get("id") ?? window.sessionStorage.getItem(window.eqSelTrenderPlanSeq) ;
            if (targetId == null) {
                alert('請先選取標案');
                return;
            } else {
                this.editEng(targetId);
            }
        }
    }
</script>