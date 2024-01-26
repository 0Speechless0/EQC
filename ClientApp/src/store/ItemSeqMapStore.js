import { reactive, ref} from "vue";


export const ItemSeqMapStore = reactive({
    ItemSeqMap : {},

    async syncRemote(route, params)
    {
        let res = await window.myAjax.post(route, params)
        this.ItemSeqMap[route] = res.data.reduce( (map ,e) => {
            map[e.Value] = e.Text;
            return map;
        }, {}) 
    },
    getMap(route)
    {
        console.log(this.ItemSeqMap);
        return this.ItemSeqMap[route] ?? {};
    }
})


