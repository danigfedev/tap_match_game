using System.Collections;
using _AppAssets.Code.Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace _AppAssets.Code
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _debugText;
        [SerializeField] private GameSettingsProvider _gameSettingsProvider;
        [SerializeField] private RecyclingBinsManager _binsManager;
        [SerializeField] private BoardManager _boardManager;

        private DisplayManager _displayManager;
        private InputManager<Matchable> _inputManager;

        private void Start()
        {
            _inputManager = InputFactory.CreateInputManager<Matchable>(Application.platform);
            _inputManager.OnItemTapped += OnItemTapped;

            _displayManager = new DisplayManager();
            _displayManager.Initialize(_gameSettingsProvider.DisplaySettings, _gameSettingsProvider.GameSettings);
            _binsManager.Initialize(_gameSettingsProvider.DisplaySettings);
            _boardManager.Initialize(_gameSettingsProvider.GameSettings, _displayManager, _binsManager);
            
            //IDEA: Here the State Machine should start
            
            StartCoroutine(SetupGame());
        }

        private void Update()
        {
            _inputManager.HandleInput();
        }

        private void OnItemTapped(Matchable tappedItem)
        {
            var message = "Object hit: " + tappedItem.Type;
            _debugText.text = message;
            // Debug.Log(message);
            
            _boardManager.FindMatchesAndUpdateBoard(tappedItem);
        }

        [ContextMenu("Reset Board")]
        public void ResetBoard()
        {
            _boardManager.ClearBoard();
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

        private IEnumerator SetupGame()
        {
            var screenOrientationCoroutine = StartCoroutine(_displayManager.ConfigureScreenOrientation());

            yield return screenOrientationCoroutine;
            
            _displayManager.ConfigureCameraOrtographicSize();

            _boardManager.BuildGameBoard();
        }
    }
}