using System;
using System.Collections.Generic;
using System.Linq;
using _AppAssets.Code.Settings;
using DG.Tweening;
using UnityEngine;

namespace _AppAssets.Code
{
    public class Matchable : MonoBehaviour, IPoolable
    {
        public event Action OnMatchableSentToBin; //Maybe this is the same as OnSendToPool? Will be called at the same point
        public event Action<IPoolable> OnSendToPool;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private SortingLayerSettings _sortingLayerSettings;
        
        public BoardNode BoardNode { get; private set; }
        public bool IsMatched { get; private set; }
        public RecyclingTypes Type => _data.RecyclingType;
        public BoardCoordinates Coordinates { get; private set; }

        private RecyclingData _data;
        private Transform _bin;

        private void Update()
        {
            //TODO check matchable bounciness
            //TODO Optimize this to only affect matchables from above
            var heightDiff = transform.localPosition.y - (BoardNode.BoardHeight - 0.5f);
            var color = _spriteRenderer.color;
            color.a = 1 - heightDiff;
            _spriteRenderer.color = color;
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
        
        public void Update(BoardNode newNode)
        {
            SetBoardNodeData(newNode);
            transform.DOLocalMoveY(Coordinates.Row, 0.5f).SetEase(Ease.OutBounce);
        }

        public void MarkAsMatched()
        {
            IsMatched = true;
        }

        public void DetachFromBoard()
        { 
            BoardNode.EmptyNode();
            
            _spriteRenderer.sortingOrder = _sortingLayerSettings.MatchableOverlay;
            transform.DOMove(_bin.position, 0.5f).OnComplete(() =>
            {
                ResetAndSendToPool();
            });
        }

        public List<Matchable> GetAdjacentMatchables()
        {
            var adjacentNodes = BoardNode.GetAdjacentNodes();
            return adjacentNodes.Select(node => node.Matchable).ToList();
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
            transform.localPosition = Vector3.zero;

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

        private void SetBoardNodeData(BoardNode node)
        {
            BoardNode = node;
            Coordinates = BoardNode.Coordinates;
        }
        
        private void ResetMatchableData()
        {
            _data = null;
            BoardNode = null;
            IsMatched = false;
        }
    }
}