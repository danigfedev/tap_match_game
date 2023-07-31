using System;
using UnityEngine;

namespace _AppAssets.Code
{
    public interface IPoolable
    {
        event Action<IPoolable> OnSendToPool;
        void InitializePoolable(Transform parent);
        void ResetPoolable();
        void ResetAndSendToPool();
    }
}