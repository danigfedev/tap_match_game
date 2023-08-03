using System;
using System.Collections.Generic;
using System.Linq;
using _AppAssets.Code.Utilities;
using UnityEngine;

namespace _AppAssets.Code
{
    public class BoardManager : MonoBehaviourPool<Matchable>
    {
        [SerializeField] private RecyclingDataProvider recyclingDataProvider;

        public event Action OnBoardUpdated;
        
        private GameSettings _gameSettings;
        private DisplayManager _displayManager;
        private RecyclingBinsManager _binsManager;
        private Transform _board;
        private BoardNode[,] _boardNodes;
        private int _totalAnimatedMatchables;
        private int _finishedAnimationsCounter;

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

        public void ClearBoard()
        {
            ResetPool();
        }

        public int FindMatchesAndUpdateBoard(Matchable tappedMatchable)
        {
            //Flow:
            // 1 - Find Matches 
            // 2 - Detach matches from board
            // 3 - Recalculate Board
            // 5 - Trigger animations
            
            var matches = new List<Matchable>();
            FindMatches(tappedMatchable, tappedMatchable.Type, ref matches);
            
            matches.DetachFromBoard();

            var groupedEmptyNodes = _boardNodes.Cast<BoardNode>()
                .Where(node => node.IsEmpty)
                .GroupBy(node => node.Coordinates.Column);

            var matchablesToAnimate = RecalculateBoard(groupedEmptyNodes);
            matchablesToAnimate.AddRange(matches);

            _totalAnimatedMatchables = matchablesToAnimate.Count;
            _finishedAnimationsCounter = 0;
            
            matchablesToAnimate.Animate(OnAnimationEnded);

            return matches.Count;
        }

        private void OnAnimationEnded(Matchable animatedMatchable)
        {
            animatedMatchable.OnAnimationEnded -= OnAnimationEnded;
            _finishedAnimationsCounter++;
            
            if (_finishedAnimationsCounter == _totalAnimatedMatchables)
            {
                OnBoardUpdated?.Invoke();
            }
        }

        private List<Matchable> RecalculateBoard(IEnumerable<IGrouping<int, BoardNode>> groupedMatches)
        {
            var totalMatchablesToUpdate = new List<Matchable>();
            
            //Board nodes grouped by column in the board
            foreach (var nodeGroup in groupedMatches)
            {
                var column = nodeGroup.Key;
                var boardNodesToUpdate = nodeGroup.OrderBy(node => node.Coordinates.Row).ToList();
                
                //Get all nodes that need to be updated
                var firstNotMatchedNodeIndexInBoard = boardNodesToUpdate.Last().Coordinates.Row + 1;
                for (int row = firstNotMatchedNodeIndexInBoard; row < _gameSettings.BoardHeight; row++)
                {
                    boardNodesToUpdate.Add(_boardNodes[column, row]);
                }

                //Get all matchables that remain in the board
                var matchablesForNodesToUpdate = new List<Matchable>();
                
                var remainingMatchablesInBoard = boardNodesToUpdate
                    .Where(n => !n.IsEmpty)
                    .Select(n => n.Matchable);
                
                matchablesForNodesToUpdate.AddRange(remainingMatchablesInBoard);
                
                //Add replacements for the matches that will be removed from the board
                var newMatchables = PopulateMatchablesReplacements(boardNodesToUpdate, matchablesForNodesToUpdate, column);
                matchablesForNodesToUpdate.AddRange(newMatchables);

                //Make each node in the current column points to the corresponding matchable
                for (int i = 0; i < boardNodesToUpdate.Count; i++)
                {
                    boardNodesToUpdate[i].SetMatchable(matchablesForNodesToUpdate[i]);
                }
                
                // Append the matchables from the current column to the list of matchables
                // remaining on the board that will be animated. 
                totalMatchablesToUpdate.AddRange(matchablesForNodesToUpdate);
            }

            return totalMatchablesToUpdate;
        }

        private List<Matchable> PopulateMatchablesReplacements(List<BoardNode> boardNodesToUpdate, List<Matchable> matchablesForNodesToUpdate, int boardColumn)
        {
            var newNodesCount = boardNodesToUpdate.Count - matchablesForNodesToUpdate.Count;
            var randomMatchableData =
                recyclingDataProvider.GetRandomMatchables(newNodesCount, _gameSettings.NumberOfMatchables);
            var newMatchables = new List<Matchable>();
            int auxCounter = 0;
            for (int i = 0; i < newNodesCount; i++)
            {
                var matchableInstance = GetItemFromPool();
                var matchableData = randomMatchableData[auxCounter];
                var matchableBin = _binsManager.GetBinInstanceByType(matchableData.RecyclingType);
                matchableInstance.SetMatchableData(matchableData, matchableBin.transform);

                //Setting matchable at the top of the board
                var newMatchableVerticalPosition = _gameSettings.BoardHeight + auxCounter;
                var newMatachableHorizontalPosition = boardColumn;
                var newPosition = new Vector2(newMatachableHorizontalPosition, newMatchableVerticalPosition);
                matchableInstance.InitializePoolableAtLocalPosition(_board, newPosition);
                
                newMatchables.Add(matchableInstance);
                auxCounter++;
            }

            return newMatchables;
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
        
        private Vector2 CalculateBoardLowerLeftCorner()
        {
            var boardCenterPosition = _displayManager.GetBoardAreaCenter();
            var boardDimensions = new Vector2(_gameSettings.BoardWidth / 2f - 0.5f, _gameSettings.BoardHeight / 2f - 0.5f);
            var lowerLeftCorner = boardCenterPosition - boardDimensions;
            return lowerLeftCorner;
        }
        
        private void PopulateBoard()
        {
            var boardWidth = _gameSettings.BoardWidth;
            var boardHeight = _gameSettings.BoardHeight;
            
            _boardNodes = new BoardNode[boardWidth, boardHeight];

            var randomMatchableData = recyclingDataProvider.GetRandomMatchables(boardWidth * boardHeight, _gameSettings.NumberOfMatchables);
            
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
    }
}