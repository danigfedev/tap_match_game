using UnityEngine;
using UnityEngine.Serialization;

namespace _AppAssets.Code
{
    [System.Serializable]
    public class RecyclingData
    {
        public RecyclingTypes RecyclingType;
        public Sprite Sprite;
        public Color RecyclingTypeColor;
    }
}