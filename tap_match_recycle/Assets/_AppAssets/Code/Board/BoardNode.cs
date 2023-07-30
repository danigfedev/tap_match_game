using System.Collections.Generic;
using _AppAssets.Code.Utilities;

namespace _AppAssets.Code
{
    public class BoardNode
    {
        public BoardCoordinates Coordinates { get; private set; }
        public Matchable Matchable { get; private set; }
        
        private BoardNode[,] _boardNodes;

        public BoardNode(BoardCoordinates coordinates, Matchable matchable, BoardNode[,] boardContext)
        {
            SetNodeData(coordinates, matchable, boardContext);
        }
        
        public void SetMatchable(Matchable matchable)
        {
            Matchable = matchable;
        }

        public void SetNodeData(BoardCoordinates coordinates, Matchable matchable, BoardNode[,] boardNodes)
        {
            Coordinates = coordinates;
            Matchable = matchable;
            _boardNodes = boardNodes;
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