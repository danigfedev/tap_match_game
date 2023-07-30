using UnityEngine;

namespace _AppAssets.Code.Settings
{
    [CreateAssetMenu(fileName = "DisplaySettings", menuName = "TapMatchRecycle/Settings/DisplaySettings")]
    public class DisplaySettings : ScriptableObject
    {
        [Range(0.5f, 0.7f)]
        public float BoardHeightScreenPercentage = 0.7f;
        [Range(1, 3)]
        public int FooterToHeaderRatio = 2;
        [Range(0, 0.1f)]
        public float BoardMarginPercentage = 0.025f;
        
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