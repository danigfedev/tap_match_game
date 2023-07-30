using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _AppAssets.Code
{
    public class BoardManager : MonoBehaviourPool<Matchable>
    {
        [SerializeField] private MatchablesProvider _matchablesProvider;
        private int _boardWidth;
        private int _boardHeight;
        private Camera _mainCamera;
        private Transform _board;
        private float _boardScreenHeightPercantage;

        public void InitializeBoard(int boardWidth, int boardHeight, Camera mainCamera, float boardScreenHeightPercantage)
        {
            _boardWidth = boardWidth;
            _boardHeight = boardHeight;
            
            _mainCamera = mainCamera;
            _boardScreenHeightPercantage = boardScreenHeightPercantage;

            _poolSize = CalculatePoolSize();

            _board = new GameObject("Board").transform;
            _board.parent = transform;
            
            Initialize(_poolSize);
        }

        public void BuildGameBoard(int boardWidth, int boardHeight)
        {
            _boardWidth = boardWidth;
            _boardHeight = boardHeight;

            var newPoolSize = CalculatePoolSize();
            ResizePool(newPoolSize);
            
            var lowerLeftCorner = CalculateBoardLowerLeftCorner();
            _board.transform.position = lowerLeftCorner;
            
            PopulateBoard();
        }

        private Vector2 CalculateBoardLowerLeftCorner()
        {
            var boardCenterPosition = GetBoardAreaCenter();
            var boardDimensions = new Vector2(_boardWidth / 2f - 0.5f, _boardHeight / 2f - 0.5f);
            var lowerLeftCorner = boardCenterPosition - boardDimensions;
            return lowerLeftCorner;
        }

        public void ClearBoard()
        {
            ResetPool();
        }

        private void PopulateBoard()
        {
            var randomMatchableData = _matchablesProvider.GetRandomMatchables(_boardHeight * _boardWidth);
            var counter = 0;
            
            Matchable matchableInstance;
            for (int row = 0; row < _boardHeight; row++)
            {
                for (int column = 0; column < _boardWidth; column++)
                {
                    matchableInstance = GetItemFromPool();
                    var matchableTransform = matchableInstance.transform;
                    matchableTransform.parent = _board;
                    matchableTransform.localPosition = Vector3.zero;
                    matchableTransform.localPosition += (Vector3)(Vector2.right * column + Vector2.up * row);
                    
                    matchableInstance.Initialize(randomMatchableData[counter]);
                    counter++;
                }
            }
        }

        private int CalculatePoolSize()
        {
            return _boardWidth * _boardHeight * 2;
        }

        private Vector2 GetBoardAreaCenter()
        {
            var screenHeight = Screen.height;
            var footerHeight = screenHeight * 0.2f; //TODO: This percentage should be defined somewhere else
            var centerHeight = footerHeight + screenHeight * _boardScreenHeightPercantage / 2;
            var boardAreaCenter = new Vector2(Screen.width / 2f, centerHeight);
            var boardCenterPosition = (Vector2)_mainCamera.ScreenToWorldPoint(boardAreaCenter);
            return boardCenterPosition;
        }
    }
}