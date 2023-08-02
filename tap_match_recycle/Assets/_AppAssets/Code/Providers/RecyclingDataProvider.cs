using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _AppAssets.Code
{
    [CreateAssetMenu(fileName = "RecyclingDataProvider", menuName = "TapMatchRecycle/Providers/RecyclingDataProvider")]
    public class RecyclingDataProvider : ScriptableObject
    {
        public RecyclingData[] RecyclingTypeData;

        private System.Random _random;
        
        public List<RecyclingData> GetRandomMatchables(int matchablesQuantity, int matchablesTypes)
        {
            var randomList = new List<RecyclingData>();
            
            _random ??= new System.Random();

            var maxRandomIndex = Mathf.Min(matchablesTypes, RecyclingTypeData.Length);
            
            for (int i = 0; i < matchablesQuantity; i++)
            {
                int randomIndex = _random.Next(0, maxRandomIndex);
                var randomElement = RecyclingTypeData[randomIndex];
                randomList.Add(randomElement);
            }

            return randomList;
        }
        
        public RecyclingData GetRecyclingData(RecyclingTypes type)
        {
            return RecyclingTypeData.FirstOrDefault(recycleData => recycleData.RecyclingType == type);
        }
    }
}