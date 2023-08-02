using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _AppAssets.Code
{
    [CreateAssetMenu(fileName = "RecyclingBinProvider", menuName = "TapMatchRecycle/Providers/RecyclingBinProvider")]
    public class RecyclingBinProvider : ScriptableObject
    {
        public GameObject BinPrefab;
        public RecyclingDataProvider RecyclingDataProvider;
        public int TotalBinCount => RecyclingDataProvider.RecyclingTypeData.Length;

        public Dictionary<RecyclingTypes, GameObject> InstantiateAllBins(Transform parent)
        {
            return InstantiateBins(TotalBinCount, parent);
        }
        
        public Dictionary<RecyclingTypes,GameObject> InstantiateBins(int binCount, Transform parent)
        {
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
            
            binInstance.GetComponent<Bin>().Initialize(recyclingData);
            // binInstance.GetComponent<Image>().color *= recyclingData.RecyclingTypeColor;
            
            return binInstance;
        }
    }
}