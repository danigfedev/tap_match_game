using System;

namespace _AppAssets.Code
{
    public interface IPoolable
    {
        event Action<IPoolable> OnSendToPool;
        void InitializePoolable();
        void ResetPoolable();
        void ResetAndSendToPool();
    }
}