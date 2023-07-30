namespace _AppAssets.Code
{
    public struct BoardCoordinates
    {
        public int Column; //Horizontal coordinate
        public int Row; //Vertical coordinate

        public BoardCoordinates(int row, int column)
        {
            Column = column;
            Row = row;
        }

        public BoardCoordinates Up => new BoardCoordinates(Row + 1, Column);
        public BoardCoordinates Down => new BoardCoordinates( Row - 1, Column);
        public BoardCoordinates Right => new BoardCoordinates(Row, Column + 1);
        public BoardCoordinates Left => new BoardCoordinates(Row, Column - 1);
    }
}