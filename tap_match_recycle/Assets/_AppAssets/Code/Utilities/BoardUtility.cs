using System.Collections.Generic;

namespace _AppAssets.Code.Utilities
{
    public static class BoardUtility
    {
        public static bool IsCoordinateInBounds(this BoardNode[,] board, BoardCoordinates coordinates)
        {
            int width = board.GetLength(0);
            int height = board.GetLength(1);
            
            return coordinates.Row >= 0 && coordinates.Row < height && coordinates.Column >= 0 &&
                   coordinates.Column < width;
        }

        public static void DetachFromBoard(this List<Matchable> matches)
        {
            foreach (var matchable in matches)
            {
                matchable.DetachFromBoard();
            }
        }
        
        public static void Animate(this List<Matchable> matches)
        {
            foreach (var matchable in matches)
            {
                matchable.Animate();
            }
        }
    }
}