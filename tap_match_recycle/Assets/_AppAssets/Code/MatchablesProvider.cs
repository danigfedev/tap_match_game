using UnityEngine;

namespace _AppAssets.Code
{
    [CreateAssetMenu(fileName = "MatchablesProvider", menuName = "TapMatchRecycle/MatchablesProvider")]
    public class MatchablesProvider : ScriptableObject
    {
        // public RecyclingTypeProvider RecyclingTypesPovider;
        
        [Header("Matchables Data")]
        public MatchableData[] Matchables;
    }
}