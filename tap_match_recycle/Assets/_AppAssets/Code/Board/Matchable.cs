using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace _AppAssets.Code
{
    public class Matchable : MonoBehaviour, IPoolable
    {
        public event Action OnMatchableSentToBin; //Maybe this is the same as OnSendToPool? Will be called at the same point
        public event Action<IPoolable> OnSendToPool;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        public BoardNode BoardNode { get; private set; }
        public bool IsMatched { get; private set; }
        public RecyclingTypes Type => _data.RecyclingType;
        public BoardCoordinates Coordinates { get; private set; }

        private MatchableData _data;
        private Transform _bin;
        
        public void SetMatchableData(MatchableData data, BoardNode boardNode, Transform bin)
        {
            _data = data;
            _bin = bin;
            SetBoardNodeData(boardNode);
        }

        public void MarkAsMatched()
        {
            IsMatched = true;
        }

        public void DetachFromBoard()
        { 
            BoardNode.EmptyNode();
            
            transform.DOMove(_bin.position, 1).OnComplete(() =>
            {
                ResetAndSendToPool();
            });
        }

        public void Update(BoardNode newNode)
        {
            SetBoardNodeData(newNode);
            transform.DOLocalMoveY(Coordinates.Row, 0.5f).SetEase(Ease.OutBounce);
        }

        public List<Matchable> GetAdjacentMatchables()
        {
            var adjacentNodes = BoardNode.GetAdjacentNodes();
            return adjacentNodes.Select(node => node.Matchable).ToList();
        }
        
        public void InitializePoolable(Transform parent)
        {
            transform.SetParent(parent);
            transform.localPosition = Vector3.zero;
            var coordinates = BoardNode.Coordinates;
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