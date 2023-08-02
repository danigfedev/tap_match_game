using UnityEngine;

namespace _AppAssets.Code
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "TapMatchRecycle/Settings/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [Range(5, 20)]
        public int BoardWidth = 5;
        [Range(5, 20)] 
        public int BoardHeight = 5;
        [Range(3, 6)] 
        public int NumberOfMatchables = 3;
        public BoardCoordinates BoardDimensions => new BoardCoordinates(BoardWidth, BoardHeight);
    }
}