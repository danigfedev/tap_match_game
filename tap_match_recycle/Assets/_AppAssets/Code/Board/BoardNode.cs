using System.Collections.Generic;
using _AppAssets.Code.Utilities;

namespace _AppAssets.Code
{
    public class BoardNode
    {
        public BoardCoordinates Coordinates { get; private set; }
        public Matchable Matchable { get; private set; }

        public bool IsEmpty => Matchable == null;

        public int BoardWidth => _boardNodes.GetLength(0);
        public int BoardHeight => _boardNodes.GetLength(1);
        
        private BoardNode[,] _boardNodes;

        public BoardNode(BoardCoordinates coordinates, Matchable matchable, BoardNode[,] boardContext)
        {
            SetNodeData(coordinates, matchable, boardContext);
        }
        
        public void SetNodeData(BoardCoordinates coordinates, Matchable matchable, BoardNode[,] boardNodes)
        {
            Coordinates = coordinates;
            Matchable = matchable;
            _boardNodes = boardNodes;
        }
        
        public void SetMatchable(Matchable matchable)
        {
            Matchable = matchable;
            Matchable.SetBoardNodeData(this);
        }

        public void EmptyNode()
        {
            Matchable = null;
        }
        
        public List<BoardNode> GetAdjacentNodes()
        {
            var validAdjacentNodes = new List<BoardNode>();
            var adjacentCoords = GetAdjacentCoordinates();
            
            foreach (var coordinates in adjacentCoords)
            {
                if (_boardNodes.IsCoordinateInBounds(coordinates))
                {
                    var node = _boardNodes[coordinates.Column, coordinates.Row];
                    validAdjacentNodes.Add(node);
                }
            }

            return validAdjacentNodes;
        }

        private BoardCoordinates[] GetAdjacentCoordinates()
        {
            return new BoardCoordinates[]
            {
                Coordinates.Up,
                Coordinates.Down,
                Coordinates.Right,
                Coordinates.Left
            };
        }
    }
}