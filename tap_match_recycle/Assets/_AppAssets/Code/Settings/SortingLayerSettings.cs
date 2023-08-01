using UnityEngine;

namespace _AppAssets.Code.Settings
{
    [CreateAssetMenu(fileName = "SortingLayerSettings", menuName = "TapMatchRecycle/Settings/SortingLayerSettings")]

    public class SortingLayerSettings : ScriptableObject
    {
        public int MatchableDefault = 0;
        public int MatchableOverlay = 100;
    }
}