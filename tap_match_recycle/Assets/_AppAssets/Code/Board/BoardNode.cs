namespace _AppAssets.Code
{
    public class BoardNode
    {
        public BoardCoordinates Coordinates { get; private set; }
        public Matchable Matchable { get; private set; }

        public BoardNode(BoardCoordinates coordinates, Matchable matchable)
        {
            SetNodeData(coordinates, matchable);
        }
        
        public void SetMatchable(Matchable matchable)
        {
            Matchable = matchable;
        }

        public void SetNodeData(BoardCoordinates coordinates, Matchable matchable)
        {
            Coordinates = coordinates;
            Matchable = matchable;
        }
    }
}