<template>

<div class="row justify-content-center" style="width: 99%;">
    <ul role="menubar" aria-disabled="false" aria-label="Pagination"
        class="pagination b-pagination justify-content-center">
        <li role="presentation" aria-hidden="true" class="page-item "><span
                role="menuitem" aria-label="Go to first page"
                type="button" tabindex="-1"
                class="page-link"
                @click="pageSectionIndex = 0">«</span></li>
        <li role="presentation" aria-hidden="true" class="page-item "><span
                role="menuitem" aria-label="Go to previous page"
                type="button" tabindex="-1"
                class="page-link"
                @click="pageSectionIndex > 0 ? pageSectionIndex-- : ''">‹</span></li>
        <li role="presentation" :class="`page-item${pageStart +n == page ? 'active' : ''}` " 
            :key="n"
            v-for="n in  (pageCount % pageSectionCount > 0 && pageSectionIndex == pageSectionIndexEnd ?  (pageCount % pageSectionCount ) : pageSectionCount )">
            <button
                class="page-link"
                @click="$emit('pageChange', pageStart +n)">{{ pageStart +n }}</button></li>
        <li role="presentation" class="page-item"><button role="menuitem"
                type="button" tabindex="-1" aria-label="Go to next page"
                class="page-link"
                @click="pageSectionIndex < pageSectionIndexEnd ? pageSectionIndex++ : ''">›</button></li>
        <li role="presentation" class="page-item"><button role="menuitem"
                type="button" tabindex="-1" aria-label="Go to last page"
                class="page-link"
                @click="pageSectionIndex = pageSectionIndexEnd">»</button></li>

    </ul>
</div>
</template>

<script>
export default{
    props: ["page", "pageCount", "pageSectionCount"],
    emits: ["pageChange"],
    data : () => {
        return {
            pageSectionIndex: 0,
        }
    },
    computed:
    {
        pageStart()
        {
            return this.pageSectionIndex*this.pageSectionCount;
        },
        pageSectionIndexEnd()
        {
            console.log(this.pageSectionIndex);
            return Math.floor(this.pageCount % this.pageSectionCount  == 0 ? 
            this.pageCount/this.pageSectionCount -1: this.pageCount/this.pageSectionCount );
        }


    },
    watch:
    {
        page :{
            handler(nValue, oValue)
            {
                console.log(nValue. oValue);
                if(nValue < oValue)
                {
                    if(nValue % this.pageSectionCount  == 0)
                    {
                        this.pageSectionIndex -- ;
                    }
                }
            }
        }
    }
}

</script>