using System.Collections;
using _AppAssets.Code.GameManagement.GameModesSystem;
using _AppAssets.Code.Input;
using UnityEngine;

namespace _AppAssets.Code.GameManagement
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameSettingsProvider _gameSettingsProvider;
        [SerializeField] private BoardManager _boardManagerPrefab;
        [SerializeField] private RecyclingBinsManager _binsManagerPrefab;
        [SerializeField] private GameUIManager _gameUIManagerPrefab;

        private BoardManager _boardManager;
        private RecyclingBinsManager _binsManager;
        private GameUIManager _gameUIManager;

        private Camera _mainCamera;
        private GameMode _currentGameMode;
        private GameStates _currentGameState;
        private DisplayManager _displayManager;
        private InputManager _inputManager;
        private Matchable _lastTappedMatchable;

        private int _turnCount;
        private int _score;

        private void Awake()
        {
            _currentGameState = GameStates.UNDEFINED;
            AdjustFrameRate();
            _mainCamera = Camera.main;
        }
        
        private void Start()
        {
            ChangeGameState(GameStates.INITIALIZE_GAME);
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
                    _gameUIManager.ToggleUIBlockerActive(false);
                    break;
                case GameStates.UPDATE_BOARD:
                    _inputManager.ToggleInputBlocked(true);
                    _gameUIManager.ToggleUIBlockerActive(true);
                    var matchesFound = _boardManager.FindMatchesAndUpdateBoard(_lastTappedMatchable);
                    _score += matchesFound;
                    break;
                case GameStates.CHECK_GAME_END:
                    _turnCount++;
                    _gameUIManager.UpdateUI(_turnCount, _score);
                    var endGameStatus = _currentGameMode.CheckEndOfGameStatus();
                    HandleEndOfGame(endGameStatus);
                    break;
                case GameStates.CONFIGURE_GAME:
                    _inputManager.ToggleInputBlocked(true);
                    //TODO Maybe implement BoardManager.HideBoard, so it's not rendered?
                    break;
                case GameStates.RESET_GAME:
                    ResetScores();
                    ResetBoard();
                    break;
            }
        }

        private void AdjustFrameRate()
        {
            var platform = Application.platform;
            
            if(platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
            {
                Application.targetFrameRate = 60;
            }
        }
        
        private void InitializeGame()
        {
            _currentGameMode = GameModeFactory.CreateGameMode(GameModes.DEFAULT);
            _inputManager = InputFactory.CreateInputManager(gameObject, Application.platform);
            _inputManager.Initialize();
            
            _displayManager = new DisplayManager(_mainCamera);
            _displayManager.Initialize(_gameSettingsProvider.DisplaySettings, _gameSettingsProvider.GameSettings);
            
            _boardManager = Instantiate(_boardManagerPrefab);
            _binsManager = Instantiate(_binsManagerPrefab);
            _gameUIManager = Instantiate(_gameUIManagerPrefab);
            
            _binsManager.Initialize(_gameSettingsProvider.GameSettings, _gameSettingsProvider.DisplaySettings, _mainCamera);
            _boardManager.Initialize(_gameSettingsProvider.GameSettings, _displayManager, _binsManager);
            _gameUIManager.Initialize(_gameSettingsProvider.GameSettings, _gameSettingsProvider.DisplaySettings);
            
            ResetScores();
            SubscribeToEvents();
            
            ChangeGameState(GameStates.SET_UP_BOARD);
        }

        private void ResetBoard()
        {
            _binsManager.UpdateBinsPanel();
            _boardManager.ClearBoard();
            StartCoroutine(SetupGame());
        }
        
        private void ResetScores()
        {
            _turnCount = 1;
            _score = 0;
            _gameUIManager.UpdateUI(_turnCount, _score);
        }

        private void SubscribeToEvents()
        {
            _inputManager.OnItemTapped += OnMatchableTapped;
            _boardManager.OnBoardUpdated += () => ChangeGameState(GameStates.CHECK_GAME_END);
            _gameUIManager.OnGameSettingsChanged += OnGameSettingsChanged;
            _gameUIManager.NotifySettingsPanelShown += () => ChangeGameState(GameStates.CONFIGURE_GAME);
            _gameUIManager.NotifySettingsPanelHidden += () => ChangeGameState(GameStates.WAIT_FOR_INPUT);
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

        public void OnGameSettingsChanged()
        {
            ChangeGameState(GameStates.RESET_GAME);
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