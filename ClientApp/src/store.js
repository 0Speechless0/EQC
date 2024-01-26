
import epcTenderStore from './store/epcTenderStore.js';  
import carbonReductionCalStore from './store/carbonReductionCalStore.js';

window.epcTenderStore = epcTenderStore;
window.carbonReductionCalStore  =  carbonReductionCalStore;


//使用store
// import {ItemSeqMapStore} from "./store/ItemSeqMapStore.js";

export  async function useStaticStore()
{
    // await ItemSeqMapStore.syncRemote("Users/GetSelfUnitList");
} 

