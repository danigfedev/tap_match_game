using System.Collections.Generic;
using UnityEngine;

namespace _AppAssets.Code
{
    [CreateAssetMenu(fileName = "MatchablesProvider", menuName = "TapMatchRecycle/Providers/MatchablesProvider")]
    public class MatchablesProvider : ScriptableObject
    {
        // public RecyclingTypeProvider RecyclingTypesPovider;
        
        [Header("Matchables Data")]
        public MatchableData[] Matchables;

        private System.Random _random;
        
        public List<MatchableData> GetRandomMatchables(int matchablesQuantity, int matchablesTypes)
        {
            var randomList = new List<MatchableData>();
            
            _random ??= new System.Random();

            var maxRandomIndex = Mathf.Min(matchablesTypes, Matchables.Length);
            
            for (int i = 0; i < matchablesQuantity; i++)
            {
                // int randomIndex = _random.Next(0, Matchables.Length);
                int randomIndex = _random.Next(0, maxRandomIndex);
                var randomElement = Matchables[randomIndex];
                randomList.Add(randomElement);
            }

            return randomList;
        }
    }
}