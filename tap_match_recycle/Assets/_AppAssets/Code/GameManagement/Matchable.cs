using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _AppAssets.Code
{
    public class Matchable : MonoBehaviour, IPoolable
    {
        public bool IsMatched { get; private set; }
        
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        private MatchableData _data;
        private BoardNode _boardNode;

        public event Action<IPoolable> OnSendToPool;

        public RecyclingTypes Type => _data.RecyclingType;

        public void SetMatchableData(MatchableData data, BoardNode boardNode)
        {
            _data = data;
            _boardNode = boardNode;
        }

        public void MarkAsMatched()
        {
            IsMatched = true;
        }
        
        public void InitializePoolable(Transform parent)
        {
            transform.SetParent(parent);
            transform.localPosition = Vector3.zero;
            var coordinates = _boardNode.Coordinates;
            transform.localPosition += (Vector3)(Vector2.right * coordinates.Column + Vector2.up * coordinates.Row);
            
            _spriteRenderer.sprite = _data.Sprite;
        }

        public void ResetPoolable()
        {
            ResetMatchableData();
            
            transform.localPosition = Vector3.zero;
            gameObject.SetActive(false);
        }

        public void ResetAndSendToPool()
        {
            ResetPoolable();
            OnSendToPool?.Invoke(this);
        }
        
        private void ResetMatchableData()
        {
            _data = null;
            _boardNode = null;
            IsMatched = false;
        }
    }
}