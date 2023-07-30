using System.Linq;
using UnityEngine;

namespace _AppAssets.Code
{
    [CreateAssetMenu(fileName = "RecyclingTypeProvider", menuName = "TapMatchRecycle/Providers/RecyclingTypeProvider")]
    public class RecyclingTypeProvider : ScriptableObject
    {
        public RecyclingTypeData[] RecyclingTypeData;

        public RecyclingTypeData GetRecyclingData(RecyclingTypes type)
        {
            return RecyclingTypeData.FirstOrDefault(recycleData => recycleData.RecyclingType == type);
        }
    }
}