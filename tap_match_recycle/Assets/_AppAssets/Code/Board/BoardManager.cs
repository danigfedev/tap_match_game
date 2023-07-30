using UnityEngine;

namespace _AppAssets.Code
{
    public class BoardManager : MonoBehaviourPool<Matchable>
    {
        [SerializeField] private MatchablesProvider _matchablesProvider;
        private GameSettings _gameSettings;
        private DisplayManager _displayManager;
        private Transform _board;
        private BoardNode[,] _boardNodes;

        public void Initialize(GameSettings gameSettings, DisplayManager displayManager)
        {
            _gameSettings = gameSettings;
            _displayManager = displayManager;

            _poolSize = CalculatePoolSize();

            _board = new GameObject("Board").transform;
            _board.parent = transform;
            
            Initialize(_poolSize);
        }

        public void BuildGameBoard()
        {
            var newPoolSize = CalculatePoolSize();
            ResizePool(newPoolSize);
            
            var lowerLeftCorner = CalculateBoardLowerLeftCorner();
            _board.transform.position = lowerLeftCorner;
            
            PopulateBoard();
        }

        private Vector2 CalculateBoardLowerLeftCorner()
        {
            var boardCenterPosition = _displayManager.GetBoardAreaCenter();
            var boardDimensions = new Vector2(_gameSettings.BoardWidth / 2f - 0.5f, _gameSettings.BoardHeight / 2f - 0.5f);
            var lowerLeftCorner = boardCenterPosition - boardDimensions;
            return lowerLeftCorner;
        }

        public void ClearBoard()
        {
            ResetPool();
        }

        private void PopulateBoard()
        {
            var boardWidth = _gameSettings.BoardWidth;
            var boardHeight = _gameSettings.BoardHeight;
            
            _boardNodes = new BoardNode[boardWidth, boardHeight];
            
            var randomMatchableData = _matchablesProvider.GetRandomMatchables(boardWidth * boardHeight, _gameSettings.NumberOfMatchables);
            
            var counter = 0;
            
            Matchable matchableInstance;
            for (int row = 0; row < boardHeight; row++)
            {
                for (int column = 0; column < boardWidth; column++)
                {
                    var boardCoordinates = new BoardCoordinates(row, column);
                        
                    matchableInstance = GetItemFromPool();

                    var boardNode = new BoardNode(boardCoordinates, matchableInstance);
                    _boardNodes[column, row] = boardNode;

                    matchableInstance.SetMatchableData(randomMatchableData[counter], boardNode);
                    matchableInstance.InitializePoolable(_board);
                    
                    counter++;
                }
            }
        }

        private int CalculatePoolSize()
        {
            return _gameSettings.BoardWidth * _gameSettings.BoardHeight * 2;
        }
    }
}