using System.Collections;
using _AppAssets.Code.Input;
using UnityEngine;

namespace _AppAssets.Code
{
    public class GameManager : MonoBehaviour
    {
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
            _binsManager.Initialize(_gameSettingsProvider.GameSettings, _gameSettingsProvider.DisplaySettings);
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
            _boardManager.FindMatchesAndUpdateBoard(tappedItem);
        }
        
        public void ResetBoard()
        {
            _binsManager.UpdateBinsPanel();
            _boardManager.ClearBoard();
            StartCoroutine(SetupGame());
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