
import {ref} from "vue";

import {useSelectionStore} from "./SelectionStore.js";
export  function useLevelSelectionStore(route, levelSourceKey, prefix)
{
    const selectionStore = useSelectionStore(route, prefix);

    const LevelsOfSelection = ref([]);

    const lastLevelNum = ref(0);

    async function GetOneLevelSelection(selectedLevelSourceValue, levelNum, prefix)
    {

        backOneLevelSelection(levelNum)
        
        if(selectedLevelSourceValue =="" && levelNum >= 0)
        {

            return;
        }


        let newGroup = await selectionStore.GetSelection(`${route}?${levelSourceKey}=${selectedLevelSourceValue}`, prefix);

        if(newGroup.value.length > 0 ) LevelsOfSelection.value.push(newGroup.value);
        console.log("LevelsOfSelection.value", LevelsOfSelection.value);
        lastLevelNum.value = levelNum ;
        return LevelsOfSelection.value[levelNum];
    }

    function backOneLevelSelection(levelNum)
    {
        LevelsOfSelection.value = LevelsOfSelection.value.slice(0, levelNum+1);

    }
    return {

        GetOneLevelSelection,
        backOneLevelSelection,
        LevelsOfSelection
    }
} 