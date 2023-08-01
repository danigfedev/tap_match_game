using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _AppAssets.Code
{
    [CreateAssetMenu(fileName = "RecyclingBinProvider", menuName = "TapMatchRecycle/Providers/RecyclingBinProvider")]
    public class RecyclingBinProvider : ScriptableObject
    {
        public GameObject BinPrefab;
        public RecyclingTypeProvider RecyclingDataProvider;
        public int TotalBinCount => RecyclingDataProvider.RecyclingTypeData.Length;
        
        public Dictionary<RecyclingTypes,GameObject> InstantiateAllBins(Transform parent)
        {
            var binCount = TotalBinCount;
            var recyclingTypeData = RecyclingDataProvider.RecyclingTypeData;
            
            var binDictionary = new Dictionary<RecyclingTypes, GameObject>(binCount);
            
            for(int i = 0; i < binCount; i++)
            {
                var binInstance = InstantiateBin(recyclingTypeData[i].RecyclingType, parent);
                binDictionary.Add(recyclingTypeData[i].RecyclingType, binInstance);
            }

            return binDictionary;
        }
        
        public GameObject InstantiateBin(RecyclingTypes type, Transform parent) 
        {
            var recyclingData = RecyclingDataProvider.GetRecyclingData(type);
            var binInstance = Instantiate(BinPrefab, parent);
            binInstance.GetComponent<Image>().color *= recyclingData.RecyclingTypeColor;
            return binInstance;
        }
    }
}