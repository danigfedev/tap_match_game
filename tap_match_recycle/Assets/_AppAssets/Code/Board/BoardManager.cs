using System.Collections.Generic;
using System.Linq;
using _AppAssets.Code.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace _AppAssets.Code
{
    public class BoardManager : MonoBehaviourPool<Matchable>
    {
        [SerializeField] private MatchablesProvider _matchablesProvider;
        private GameSettings _gameSettings;
        private DisplayManager _displayManager;
        private RecyclingBinsManager _binsManager;
        private Transform _board;
        private BoardNode[,] _boardNodes;

        public void Initialize(GameSettings gameSettings, DisplayManager displayManager, RecyclingBinsManager binsManager)
        {
            _gameSettings = gameSettings;
            _displayManager = displayManager;
            _binsManager = binsManager;

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

                    var boardNode = new BoardNode(boardCoordinates, matchableInstance, _boardNodes);
                    _boardNodes[column, row] = boardNode;

                    var matchableData = randomMatchableData[counter];
                    var matchableBin = _binsManager.GetBinInstanceByType(matchableData.RecyclingType);
                    
                    matchableInstance.SetMatchableData(matchableData, boardNode, matchableBin.transform);
                    matchableInstance.InitializePoolable(_board);
                    
                    counter++;
                }
            }
        }

        private int CalculatePoolSize()
        {
            return _gameSettings.BoardWidth * _gameSettings.BoardHeight * 2;
        }

        public void FindMatchesAndUpdateBoard(Matchable tappedMatchable)
        {
            var matches = new List<Matchable>();
            FindMatches(tappedMatchable, tappedMatchable.Type, ref matches);
            
            var matchingNodes = matches.Select(matchable => matchable.BoardNode).ToList();
            var groupedNodes = matchingNodes.GroupBy(node => node.Coordinates.Column);
            
            //Flow should be:
            // 1 - FInd Matches 
            // 2 - Detach matches from board
            // 3 - Recalculate Board (update existing data)
            // 5 - Update nodes

            matches.DetachFromBoard();
            
            //TODO Refactor this. Maybe thi can return the list of updated nodes
            UpdateBoard(groupedNodes);//Rename this to RecaulculateBoard or something like that
            
            //nodesToUpdate.Update();
        }

        private void UpdateBoard(IEnumerable<IGrouping<int, BoardNode>> groupedMatches)
        {
            //Maybe do this insde a SelectMany method?
            foreach (var nodeGroup in groupedMatches)
            {
                var column = nodeGroup.Key;
                var boardNodesToUpdate = nodeGroup.OrderBy(node => node.Coordinates.Row).ToList();
                
                var firstNotMatchedNodeIndexInBoard = boardNodesToUpdate.Last().Coordinates.Row + 1;
                for (int row = firstNotMatchedNodeIndexInBoard; row < _gameSettings.BoardHeight; row++)
                {
                    boardNodesToUpdate.Add(_boardNodes[column, row]);
                }

                //New implementation:
                var matchablesForNodesToUpdate = new List<Matchable>();
                var remainingMatchablesInBoard = boardNodesToUpdate.Where(n => !n.IsEmpty).Select(n => n.Matchable);
                matchablesForNodesToUpdate.AddRange(remainingMatchablesInBoard);

                //Get new matchables from pool ==============================
                var newNodesCount = boardNodesToUpdate.Count - matchablesForNodesToUpdate.Count;
                var randomMatchableData = _matchablesProvider.GetRandomMatchables(newNodesCount, _gameSettings.NumberOfMatchables);
                Matchable matchableInstance;
                int auxCounter = 0;
                for (int i = 0; i < newNodesCount; i++)
                {
                    matchableInstance = GetItemFromPool();
                    var matchableData = randomMatchableData[auxCounter];
                    var matchableBin = _binsManager.GetBinInstanceByType(matchableData.RecyclingType);
                    matchableInstance.SetMatchableData(matchableData, matchableBin.transform);

                    //Setting matchable position at board's top
                    var newMatchableVerticalPosition = _gameSettings.BoardHeight + auxCounter;
                    var newMatachableHorizontalPosition = column;
                    // var newMatachableHorizontalPosition = nodeToUpdate.Coordinates.Column;
                    var newPosition = new Vector2(newMatachableHorizontalPosition, newMatchableVerticalPosition);
                    matchableInstance.InitializePoolableAtLocalPosition(_board, newPosition);
                    
                    //Adding to list
                    matchablesForNodesToUpdate.Add(matchableInstance);
                    auxCounter++;
                }

                for (int i = 0; i < boardNodesToUpdate.Count; i++)
                {
                    boardNodesToUpdate[i].SetMatchable(matchablesForNodesToUpdate[i]);
                }
                
                foreach (var nodeToUpdate in boardNodesToUpdate)
                {
                    nodeToUpdate.Update();
                }
            }
        }

        private void FindMatches(Matchable tappedMatchable, RecyclingTypes typeToMatch, ref List<Matchable> matches)
        {
            if (tappedMatchable == null) //This check should be unnecessary when whole game is done
            {
                return;
            }
            
            if (tappedMatchable.Type != typeToMatch)
            {
                return;
            }

            if (tappedMatchable.IsMatched)
            {
                return;
            }

            matches.Add(tappedMatchable);
            tappedMatchable.MarkAsMatched();
            
            var adjacentMatchables = tappedMatchable.GetAdjacentMatchables();
            foreach (var adjacentMatchable in adjacentMatchables)
            {
                FindMatches(adjacentMatchable, typeToMatch, ref matches);
            }
        }
    }
}