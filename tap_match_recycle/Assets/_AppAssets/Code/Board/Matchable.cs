using System;
using System.Collections.Generic;
using System.Linq;
using _AppAssets.Code.Interfaces;
using _AppAssets.Code.Settings;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace _AppAssets.Code
{
    public class Matchable : MonoBehaviour, IPoolable
    {
        public event Action<Matchable> OnAnimationEnded;
        public event Action<IPoolable> OnSendToPool;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private SortingLayerSettings _sortingLayerSettings;
        
        [FormerlySerializedAs("_animationDuration")]
        [Space]
        [Header("Animation Parameters")] 
        [SerializeField] private float _fallingAnimationDuration = 0.5f;
        [SerializeField] private Ease _fallingAnimationEase = Ease.OutBounce;
        [SerializeField] private float _toBinAnimationDuration = 0.5f;
        [SerializeField] private Ease _toBinAnimationEase = Ease.Linear;

        public BoardNode BoardNode { get; private set; }
        public bool IsMatched { get; private set; }
        public RecyclingTypes Type => _data.RecyclingType;
        public BoardCoordinates Coordinates { get; private set; }

        private RecyclingData _data;
        private Transform _bin;

        private void Update()
        {
            if (BoardNode != null && transform.localPosition.y >= BoardNode.BoardHeight - 1)
            {
                var heightDiff = transform.localPosition.y - (BoardNode.BoardHeight - 0.5f);
                var color = _spriteRenderer.color;
                color.a = 1 - heightDiff;
                _spriteRenderer.color = color;
            }
        }
        
        public void InitializePoolable(Transform parent)
        {
            var coordinates = BoardNode.Coordinates;
            var initialPosition = Vector2.right * coordinates.Column + Vector2.up * coordinates.Row;

            InitializePoolableAtLocalPosition(parent, initialPosition);
        }

        public void InitializePoolableAtLocalPosition(Transform parent, Vector2 initialPosition)
        {
            transform.SetParent(parent);
            transform.localPosition = initialPosition;
            
            _spriteRenderer.sprite = _data.Sprite;
            _spriteRenderer.sortingOrder = _sortingLayerSettings.MatchableDefault;
        }

        public void ResetAndSendToPool()
        {
            ResetPoolable();
            OnSendToPool?.Invoke(this);
        }
        
        public void ResetPoolable()
        {
            ResetMatchableData();
            
            transform.localPosition = Vector3.zero;
            _spriteRenderer.sortingOrder = _sortingLayerSettings.MatchableDefault;
            gameObject.SetActive(false);
        }
        
        public void SetMatchableData(RecyclingData data, Transform bin)
        {
            _data = data;
            _bin = bin;
        }
        
        public void SetMatchableData(RecyclingData data, BoardNode boardNode, Transform bin)
        {
            _data = data;
            _bin = bin;
            SetBoardNodeData(boardNode);
        }
        
        public void SetBoardNodeData(BoardNode node)
        {
            BoardNode = node;
            Coordinates = BoardNode.Coordinates;
        }

        public void MarkAsMatched()
        {
            IsMatched = true;
        }

        public void DetachFromBoard()
        { 
            BoardNode.EmptyNode();
            BoardNode = null;
        }

        public List<Matchable> GetAdjacentMatchables()
        {
            var adjacentNodes = BoardNode.GetAdjacentNodes();
            return adjacentNodes.Select(node => node.Matchable).ToList();
        }
        
        public void Animate()
        {
            if (BoardNode == null)
            {
                MoveToBin();
            }
            else
            {
                MoveToBoardNode();
            }
        }

        private void MoveToBin()
        {
            _spriteRenderer.sortingOrder = _sortingLayerSettings.MatchableOverlay;
            transform.DOMove(_bin.position, _toBinAnimationDuration)
                .SetEase(_toBinAnimationEase)
                .OnComplete(() =>
            {
                ResetAndSendToPool();
                FireOnAnimationEndedEvent();
            });
        }
        
        private void MoveToBoardNode()
        {
            transform.DOLocalMoveY(Coordinates.Row, _fallingAnimationDuration)
                .SetEase(_fallingAnimationEase)
                .OnComplete(FireOnAnimationEndedEvent);
        }

        private void FireOnAnimationEndedEvent()
        {
            OnAnimationEnded?.Invoke(this);
        }
        
        private void ResetMatchableData()
        {
            _data = null;
            BoardNode = null;
            IsMatched = false;
        }
    }
}