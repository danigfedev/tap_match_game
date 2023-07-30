namespace _AppAssets.Code.Utilities
{
    public static class BoardUtility
    {
        public static bool IsCoordinateInBounds(this BoardNode[,] board, BoardCoordinates coordinates)
        {
            int width = board.GetLength(0);
            int height = board.GetLength(1);
            
            return coordinates.Row > 0 && coordinates.Row < height && coordinates.Column > 0 &&
                   coordinates.Column < width;
        }
    }
}