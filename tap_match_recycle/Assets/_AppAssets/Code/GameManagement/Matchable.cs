using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

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
        // public BoardCoordinates Coordinates => BoardNode.Coordinates;
        public BoardCoordinates Coordinates { get; private set; }

        private MatchableData _data;
        
        public void SetMatchableData(MatchableData data, BoardNode boardNode)
        {
            _data = data;
            SetBoardNodeData(boardNode);
        }

        public void MarkAsMatched()
        {
            IsMatched = true;
        }

        public void DetachFromBoard()
        {
            //Trigger animation
            
            BoardNode.EmptyNode();

            ResetAndSendToPool();//This should be called when reaches pool.
            // _boardNode = null;
        }

        public void Update(BoardNode newNode)
        {
            SetBoardNodeData(newNode);
            transform.localPosition = new Vector3(Coordinates.Column, Coordinates.Row, 0);
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