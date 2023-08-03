using System.Collections;
using _AppAssets.Code.Settings;
using UnityEngine;

namespace _AppAssets.Code
{
    public class DisplayManager
    {
        public Camera MainCamera { get; private set; }
        private DisplaySettings _displaySettings;
        private GameSettings _gameSettings;

        public DisplayManager(Camera mainCamera)
        {
            MainCamera = mainCamera;
        }

        public void Initialize(DisplaySettings displaySettings, GameSettings gameSettings)
        {
            _displaySettings = displaySettings;
            _gameSettings = gameSettings;
        }
        
        public IEnumerator ConfigureScreenOrientation()
        {
            var newScreenOrientation = (float)_gameSettings.BoardWidth / _gameSettings.BoardHeight > 1
                ? ScreenOrientation.LandscapeLeft
                : ScreenOrientation.Portrait;

            var previousScreenOrientation = Screen.orientation; 
            Screen.orientation = newScreenOrientation; //Orientation change may take several frames
            
            if (previousScreenOrientation == newScreenOrientation)
            {
                yield break;
            }
            
            var initialScreenWidth = Screen.width;
            while (Screen.height != initialScreenWidth)
            {
                yield return null;
            }
        }
        
        public void ConfigureCameraOrtographicSize()
        {
            var aspectRatio = (float)Screen.width / Screen.height;
            var boardMarginPercentage = _displaySettings.BoardMarginPercentage;
            
            var horizontalFit = _gameSettings.BoardWidth * (1 + boardMarginPercentage) / aspectRatio;
            var verticalFit = _gameSettings.BoardHeight * (1 + boardMarginPercentage) / _displaySettings.BoardHeightScreenPercentage;
            var cameraSizeDoubled = horizontalFit >= verticalFit ? horizontalFit : verticalFit;
            MainCamera.orthographicSize = cameraSizeDoubled / 2;
        }
        
        public Vector2 GetBoardAreaCenter()
        {
            var screenHeight = Screen.height;
            var footerHeight = screenHeight * _displaySettings.FooterHeightScreenPercentage;
            var centerHeight = footerHeight + screenHeight * _displaySettings.BoardHeightScreenPercentage / 2;
            var boardAreaCenter = new Vector2(Screen.width / 2f, centerHeight);
            var boardCenterPosition = (Vector2)MainCamera.ScreenToWorldPoint(boardAreaCenter);
            return boardCenterPosition;
        }
    }
}