using System.Collections.Generic;
using _AppAssets.Code.Settings;
using UnityEngine;

namespace _AppAssets.Code
{
    public class RecyclingBinsManager : MonoBehaviour
    {
        [SerializeField] private RecyclingBinProvider _binsProvider;
        [SerializeField] private GameObject _binCointainer;

        private DisplaySettings _displaySettings;
        private RectTransform _rectTransform;
        private Dictionary<RecyclingTypes, GameObject> _typeToBinInstanceMap;
        
        [ContextMenu("Initialize")]
        public void Initialize(DisplaySettings displaySettings)
        {
            _displaySettings = displaySettings;
            _rectTransform = _binCointainer.GetComponent<RectTransform>();

            AdjustDimensions();
            
            _typeToBinInstanceMap = _binsProvider.InstantiateAllBins(_binCointainer.transform);
            
            //Maybe configure with the item sprite? To make it easier to identify
        }

        public GameObject GetBinInstanceByType(RecyclingTypes type)
        {
            _typeToBinInstanceMap.TryGetValue(type, out var binInstance);
            return binInstance;
        }
        
        private void AdjustDimensions()
        {
            _rectTransform.anchorMin = new Vector2(0, 0);
            _rectTransform.anchorMax = new Vector2(1, _displaySettings.FooterHeightScreenPercentage);
        }
    }
}