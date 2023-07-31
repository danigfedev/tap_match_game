using UnityEngine;

namespace _AppAssets.Code
{
    [CreateAssetMenu(fileName = "RecyclingBinProvider", menuName = "TapMatchRecycle/Providers/RecyclingBinProvider")]
    public class RecyclingBinProvider : ScriptableObject
    {
        public GameObject BinPrefab;
        public RecyclingTypeProvider RecyclingDataProvider;

        public GameObject GetRecyclingBin(RecyclingTypes type) 
        {
            var recyclingData = RecyclingDataProvider.GetRecyclingData(type);
            var instance = Instantiate(BinPrefab);
            instance.GetComponent<SpriteRenderer>().color *= recyclingData.RecyclingTypeColor;
            return BinPrefab;
        }
    }
}