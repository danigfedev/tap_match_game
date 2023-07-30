using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _AppAssets.Code
{
    public class Matchable : MonoBehaviour, IPoolable
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private MatchableData Data;
        //Node
        
        public event Action<IPoolable> OnSendToPool;

        public RecyclingTypes Type => Data.RecyclingType;

        public void Initialize(MatchableData data)
        {
            Data = data;
            _spriteRenderer.sprite = Data.Sprite;
        }

        public void Reset()
        {
            Data = null;
            transform.localPosition = Vector3.zero;
            gameObject.SetActive(false);
        }

        public void ResetAndSendToPool()
        {
            Reset();
            OnSendToPool?.Invoke(this);
        }
    }
}