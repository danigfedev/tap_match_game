using System.Collections.Generic;
using UnityEngine;

namespace _AppAssets.Code
{
    public class RecyclingBinsManager : MonoBehaviour
    {
        [SerializeField] private RecyclingBinProvider _binsProvider;
        [SerializeField] private Transform _binCointainer;

        private Dictionary<RecyclingTypes, GameObject> _typeToBinInstanceMap;
        
        [ContextMenu("Initialize")]
        public void Initialize()
        {
            _typeToBinInstanceMap = _binsProvider.InstantiateAllBins(_binCointainer);
            //Maybe configure with the item sprite? To make it easier to identify
        }

        public GameObject GetBinInstanceByType(RecyclingTypes type)
        {
            _typeToBinInstanceMap.TryGetValue(type, out var binInstance);
            return binInstance;
        }
    }
}