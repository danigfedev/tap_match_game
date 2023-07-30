using System;

namespace _AppAssets.Code
{
    public interface IPoolable
    {
        event Action<IPoolable> OnSendToPool;
        void Initialize(MatchableData data);
        void Reset();
        void ResetAndSendToPool();
    }
}