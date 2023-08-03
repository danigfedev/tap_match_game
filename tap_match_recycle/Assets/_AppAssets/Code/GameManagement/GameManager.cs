using System.Collections;
using _AppAssets.Code.GameManagement.GameModesSystem;
using _AppAssets.Code.Input;
using UnityEngine;

namespace _AppAssets.Code.GameManagement
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameSettingsProvider _gameSettingsProvider;
        [SerializeField] private RecyclingBinsManager _binsManager;
        [SerializeField] private BoardManager _boardManager;
        [SerializeField] private GameUIManager _gameUIManager;

        private GameMode _currentGameMode;
        private GameStates _currentGameState;
        private DisplayManager _displayManager;
        private InputManager _inputManager;
        private Matchable _lastTappedMatchable;

        private void Awake()
        {
            _currentGameState = GameStates.UNDEFINED;
        }
        
        private void Start()
        {
            ChangeGameState(GameStates.INITIALIZE_GAME);
        }
        
        public void ResetBoard()
        {
            _binsManager.UpdateBinsPanel();
            _boardManager.ClearBoard();
            StartCoroutine(SetupGame());
        }

        private void ChangeGameState(GameStates newState)
        {
            _currentGameState = newState;

            switch (_currentGameState)
            {
                case GameStates.INITIALIZE_GAME:
                    InitializeGame();
                    break;
                case GameStates.SET_UP_BOARD:
                    StartCoroutine(SetupGame());
                    break;
                case GameStates.WAIT_FOR_INPUT:
                    _inputManager.ToggleInputBlocked(false);
                    break;
                case GameStates.UPDATE_BOARD:
                    _inputManager.ToggleInputBlocked(true);
                    _boardManager.FindMatchesAndUpdateBoard(_lastTappedMatchable);
                    break;
                case GameStates.CHECK_GAME_END:

                    var endGameStatus = _currentGameMode.CheckEndOfGameStatus();
                    HandleEndOfGame(endGameStatus);
                    break;
                case GameStates.CONFIGURE_GAME:
                    break;
            }
        }

        private void InitializeGame()
        {
            _currentGameMode = GameModeFactory.CreateGameMode(GameModes.DEFAULT);
            _inputManager = InputFactory.CreateInputManager(gameObject, Application.platform);
            _inputManager.Initialize();

            _displayManager = new DisplayManager();
            _displayManager.Initialize(_gameSettingsProvider.DisplaySettings, _gameSettingsProvider.GameSettings);
            _binsManager.Initialize(_gameSettingsProvider.GameSettings, _gameSettingsProvider.DisplaySettings);
            _boardManager.Initialize(_gameSettingsProvider.GameSettings, _displayManager, _binsManager);
            _gameUIManager.Initialize(_gameSettingsProvider.GameSettings, _gameSettingsProvider.DisplaySettings);
            
            SubscribeToEvents();
            
            ChangeGameState(GameStates.SET_UP_BOARD);
        }

        private void SubscribeToEvents()
        {
            _inputManager.OnItemTapped += OnMatchableTapped;
            _boardManager.OnBoardUpdated += OnBoardUpdated;
            _gameUIManager.OnGameSettingsChanged += ResetBoard;
        }
        
        private IEnumerator SetupGame()
        {
            var screenOrientationCoroutine = StartCoroutine(_displayManager.ConfigureScreenOrientation());

            yield return screenOrientationCoroutine;
            
            _displayManager.ConfigureCameraOrtographicSize();

            _boardManager.BuildGameBoard();
            
            ChangeGameState(GameStates.WAIT_FOR_INPUT);
        }

        private void OnMatchableTapped(GameObject tappedObject)
        {
            var tappedMatchable = tappedObject.GetComponent<Matchable>();

            if (tappedMatchable == null)
            {
                return;
            }
            
            _lastTappedMatchable = tappedMatchable;
            ChangeGameState(GameStates.UPDATE_BOARD);
        }
        
        private void OnBoardUpdated()
        {
            ChangeGameState(GameStates.CHECK_GAME_END);
        }
        
        private void HandleEndOfGame(EndGameStatus endGameStatus)
        {
            switch (endGameStatus)
            {
                case EndGameStatus.WIN:
                    break;
                case EndGameStatus.LOSE:
                    break;
                case EndGameStatus.KEEP_PLAYING:
                default:
                    ChangeGameState(GameStates.WAIT_FOR_INPUT);
                    break;
            }
        }
    }
}