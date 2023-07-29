using System.Collections;
using UnityEngine;

namespace _AppAssets.Code
{
    public class GameManager : MonoBehaviour
    {
        [Range(5, 20)]
        [SerializeField] private int _boardWidth;
        [Range(5, 20)]
        [SerializeField] private int _boardHeight;
        [SerializeField] private GameObject _matchablePrefab;
        [SerializeField] private Camera _mainCamera;
        
        [Range(0, 0.1f)]
        [SerializeField]private float _boardMarginPercentage = 0.025f;

        [SerializeField] private float _boardScreenHeightPercantage = 0.7f;

        private GameObject _board;

        private void Start()
        {
            _board = new GameObject("Board");

            StartCoroutine(SetupGame());
        }

        [ContextMenu("Reset Board")]
        public void ResetBoard()
        {
            ClearBoard();
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
        
        private void ClearBoard()
        {
            var tileCount = _board.transform.childCount;

            for (int i = tileCount - 1; i >= 0; i--)
            {
                Destroy(_board.transform.GetChild(i).gameObject);
            }
        }

        private IEnumerator SetupGame()
        {
            var screenOrientationCoroutine = StartCoroutine(ConfigureScreenOrientation());

            yield return screenOrientationCoroutine;
            
            ConfigureCameraOrtographicSize();

            var boardCenter = BuildGameBoard();
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

        private Vector2 BuildGameBoard()
        {
            var boardCenterPosition = GetBoardAreaCenter();

            // Calculate Lower Left corner
            var boardDimensions = new Vector2(_boardWidth / 2f - 0.5f, _boardHeight / 2f - 0.5f);
            var lowerLeftCorner = boardCenterPosition - boardDimensions;
            _board.transform.position = lowerLeftCorner;

            GameObject matchableInstance;
            for (int row = 0; row < _boardHeight; row++)
            {
                for (int column = 0; column < _boardWidth; column++)
                {
                    matchableInstance = Instantiate(_matchablePrefab, _board.transform);
                    matchableInstance.transform.position += (Vector3)(Vector2.right * column + Vector2.up * row);
                }
            }

            return new Vector2(_boardWidth / 2f - 0.5f, _boardHeight / 2f - 0.5f);
        }
        
        private Vector2 GetBoardAreaCenter()
        {
            var screenHeight = Screen.height;
            var centerHeight = screenHeight * 0.2f + screenHeight * _boardScreenHeightPercantage / 2;
            var boardAreaCenter = new Vector2(Screen.width / 2f, centerHeight);
            var boardCenterPosition = (Vector2)_mainCamera.ScreenToWorldPoint(boardAreaCenter);
            return boardCenterPosition;
        }
    }
}