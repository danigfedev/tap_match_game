using System.Collections.Generic;
using UnityEngine;

namespace _AppAssets.Code
{
    [CreateAssetMenu(fileName = "MatchablesProvider", menuName = "TapMatchRecycle/MatchablesProvider")]
    public class MatchablesProvider : ScriptableObject
    {
        // public RecyclingTypeProvider RecyclingTypesPovider;
        
        [Header("Matchables Data")]
        public MatchableData[] Matchables;

        private System.Random _random;
        
        public List<MatchableData> GetRandomMatchables(int N)
        {
            var randomList = new List<MatchableData>();
            
            _random ??= new System.Random();

            for (int i = 0; i < N; i++)
            {
                int randomIndex = _random.Next(0, Matchables.Length);
                var randomElement = Matchables[randomIndex];
                randomList.Add(randomElement);
            }

            return randomList;
        }
    }
}