using System.Collections.Generic;
using _AppAssets.Code.Settings;
using UnityEngine;

namespace _AppAssets.Code
{
    public class RecyclingBinsManager : MonoBehaviour
    {
        [SerializeField] private RecyclingBinProvider _binsProvider;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GameObject _binCointainer;

        private GameSettings _gameSettings;
        private DisplaySettings _displaySettings;
        private RectTransform _rectTransform;
        private Dictionary<RecyclingTypes, GameObject> _typeToBinInstanceMap;
        
        [ContextMenu("Initialize")]
        public void Initialize(GameSettings gameSettings, DisplaySettings displaySettings, Camera mainCamera)
        {
            _gameSettings = gameSettings;
            _displaySettings = displaySettings;
            _rectTransform = _binCointainer.GetComponent<RectTransform>();

            _canvas.worldCamera = mainCamera;
            
            AdjustDimensions();
            
            _typeToBinInstanceMap = _binsProvider.InstantiateAllBins(_binCointainer.transform);
            UpdateBinsPanel();
        }

        public void UpdateBinsPanel()
        {
            var data = _binsProvider.RecyclingDataProvider.RecyclingTypeData;
            
            for (int i = 0; i < data.Length; i++)
            {
                var binActiveStatus = i < _gameSettings.NumberOfMatchables;

                var binInstance = GetBinInstanceByType(data[i].RecyclingType);
                binInstance.SetActive(binActiveStatus);
            }
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