using System.Collections;
using UnityEngine;

namespace _AppAssets.Code
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game rules")]
        [Range(5, 20)]
        [SerializeField] private int _boardWidth;
        [Range(5, 20)]
        [SerializeField] private int _boardHeight;
        [Range(3, 6)]
        [SerializeField] private int _numberOfMatchables;
        
        [Space]
        [SerializeField] private Matchable _matchablePrefab;
        [SerializeField] private Camera _mainCamera;
        
        [Range(0, 0.1f)]
        [SerializeField]private float _boardMarginPercentage = 0.025f;

        [SerializeField] private float _boardScreenHeightPercantage = 0.7f;
        
        public BoardManager BoardManager;

        private void Start()
        {
            //Initialize Object Pools
            BoardManager.InitializeBoard(_boardWidth, _boardHeight, _mainCamera, _boardScreenHeightPercantage);

            StartCoroutine(SetupGame());
        }

        [ContextMenu("Reset Board")]
        public void ResetBoard()
        {
            BoardManager.ClearBoard();
            StartCoroutine(SetupGame());
        }
        
        [ContextMenu("Set Orientation Landscape Left")]
        public void SetOrientationLandscapeLeft()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
        
        [ContextMenu("Set Orientation Portrait")]
        public void SetOrientationPortrait()
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
        
        // private void ClearBoard()
        // {
        //     var tileCount = _board.transform.childCount;
        //
        //     for (int i = tileCount - 1; i >= 0; i--)
        //     {
        //         Destroy(_board.transform.GetChild(i).gameObject);
        //     }
        // }

        private IEnumerator SetupGame()
        {
            var screenOrientationCoroutine = StartCoroutine(ConfigureScreenOrientation());

            yield return screenOrientationCoroutine;
            
            ConfigureCameraOrtographicSize();

            // GetBoardAreaCenter();
            
            BoardManager.BuildGameBoard(_boardWidth, _boardHeight);
            // var boardCenter = BuildGameBoard();
        }

        private IEnumerator ConfigureScreenOrientation()
        {
            var screenOrientation = (float)_boardWidth / _boardHeight > 1
                ? ScreenOrientation.LandscapeLeft
                : ScreenOrientation.Portrait;
            
            Screen.orientation = screenOrientation; //Takes one frame to update. On next frame the width and height values are updated

            yield return new WaitForEndOfFrame();
        }
        
        private void ConfigureCameraOrtographicSize()
        {
            var aspectRatio = (float)Screen.width / Screen.height;
            var horizontalFit = _boardWidth * (1 + _boardMarginPercentage) / aspectRatio;
            var verticalFit = _boardHeight * (1 + _boardMarginPercentage) / _boardScreenHeightPercantage;
            var cameraSizeDoubled = horizontalFit >= verticalFit ? horizontalFit : verticalFit;
            _mainCamera.orthographicSize = cameraSizeDoubled / 2;
        }
    }
}