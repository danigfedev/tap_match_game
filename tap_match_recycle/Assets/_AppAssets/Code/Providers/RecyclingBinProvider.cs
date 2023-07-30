using Unity.VisualScripting;
using UnityEngine;

namespace _AppAssets.Code
{
    [CreateAssetMenu(fileName = "RecyclingBinProvider", menuName = "TapMatchRecycle/RecyclingBinProvider")]
    public class RecyclingBinProvider : ScriptableObject
    {
        public Sprite BinSprite;
        public RecyclingTypeProvider RecyclingDataProvider;

        public Sprite GetRecyclingBin(RecyclingTypes type) 
        {
            var recyclingData = RecyclingDataProvider.GetRecyclingData(type);
            var instance = Instantiate(BinSprite);
            instance.GetComponent<SpriteRenderer>().color *= recyclingData.RecyclingTypeColor;
            return instance;
        }
    }
}