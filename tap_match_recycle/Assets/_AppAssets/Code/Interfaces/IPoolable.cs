using System;
using UnityEngine;

namespace _AppAssets.Code.Interfaces
{
    public interface IPoolable
    {
        event Action<IPoolable> OnSendToPool;
        void InitializePoolable(Transform parent);
        void InitializePoolableAtLocalPosition(Transform parent, Vector2 initialPosition);
        void ResetPoolable();
        void ResetAndSendToPool();
    }
}