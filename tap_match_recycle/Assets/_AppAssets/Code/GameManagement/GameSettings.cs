using UnityEngine;

namespace _AppAssets.Code
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "TapMatchRecycle/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Board Settings")]
        [Range(5, 20)]
        public int BoardWidth;
        [Range(5, 20)]
        public int BoardHeight;
        [Range(3, 6)]
        public int NumberOfMatchables;

        [Space]
        [Header("Display Settings")]
        [Range(0.5f, 0.7f)]
        public float BoardHeightScreenPercentage;
        [Range(1,3)]
        public int FooterToHeaderRatio;
        
        public float HeaderHeightScreenPercentage => CalculateHeaderHeightPercentage();

        public float FooterHeightScreenPercentage => CalculateFooterHeightPercentage();

        private float CalculateHeaderHeightPercentage()
        {
            var remainingSpace = 1 - BoardHeightScreenPercentage;
            var headerHeightPercentage = remainingSpace / (1 + FooterToHeaderRatio);

            return headerHeightPercentage;
        }

        private float CalculateFooterHeightPercentage()
        {
            var remainingSpace = 1 - BoardHeightScreenPercentage;
            var footerHeightPercentage = remainingSpace / (1 + 1f / FooterToHeaderRatio);

            return footerHeightPercentage;
        }
        
    }
}