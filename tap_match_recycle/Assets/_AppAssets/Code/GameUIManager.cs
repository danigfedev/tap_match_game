using System;
using System.Collections;
using System.Collections.Generic;
using _AppAssets.Code;
using _AppAssets.Code.Settings;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [Header("Main Rect Transforms")]
    [SerializeField] private RectTransform _headerRectTransform;
    [SerializeField] private RectTransform _bodyRectTransform;

    [Space]
    [Header("HEADER")]
    [SerializeField] private Button _showButton;
    [SerializeField] private Button _hideButton;
    [SerializeField] private TMPro.TextMeshProUGUI _turnCountLabel;
    [SerializeField] private TMPro.TextMeshProUGUI _scoreLabel;
    
    [Space]
    [Header("SETTINGS PANEL")]
    [Header("Matchables Variety")] 
    [SerializeField] private TMPro.TextMeshProUGUI _minMatchableVarietyLabel;
    [SerializeField] private TMPro.TextMeshProUGUI _maxMatchableVarietyLabel;
    [SerializeField] private Slider _matchableVarietySlider;
    
    [Space] 
    [Header("Board Width")] 
    [SerializeField] private TMPro.TextMeshProUGUI _minBoardWidthLabel;
    [SerializeField] private TMPro.TextMeshProUGUI _maxBoardWidthLabel;
    [SerializeField] private Slider _boardWidthSlider;
    
    [Header("Board Height")] 
    [SerializeField] private TMPro.TextMeshProUGUI _minBoardHeightLabel;
    [SerializeField] private TMPro.TextMeshProUGUI _maxBoardHeightLabel;
    [SerializeField] private Slider _boardHeightSlider;
    
    [Header("Device Orientation")] 
    [SerializeField] private Image _portraitOrientationImage;
    [SerializeField] private Image _landscapeOrientationImage;
    
    [Header("Buttons")]
    [SerializeField] private Button _resetButton;

    public event Action OnGameSettingsChanged; 
    
    private GameSettings _gameSettings;
    private DisplaySettings _displaySettings;

    private Rect _bodyRect;
    private bool _bodyHidden = false;
    private int _minMatchableVariety;
    private int _maxMatchableVariety;
    private int _minBoardWidth;
    private int _maxBoardWidth;
    private int _minBoardHeight;
    private int _maxBoardHeight;

    public void Initialize(GameSettings gameSettings, DisplaySettings displaySettings)
    {
        _gameSettings = gameSettings;
        _displaySettings = displaySettings;

        AdjustDimensions();

        InitializeHeader();
        
        SetupBodyAnimations();

        //Initialize UI values with game Settings
        CalculateSettingMinAndMaxValues();
        ConfigureSliders();
        
        SubscribeToEvents();
    }

    private void InitializeHeader()
    {
        ToggleShowButtonActive(true);
    }

    private void ToggleShowButtonActive(bool activateShowButton)
    {
        _showButton.gameObject.SetActive(activateShowButton);
        _hideButton.gameObject.SetActive(!activateShowButton);
    }

    private void SetupBodyAnimations()
    {
         _bodyRect = _bodyRectTransform.rect;
         Hide(false);
    }

    private void AdjustDimensions()
    {
        var headerAnchorMinY = 1 - _displaySettings.HeaderHeightScreenPercentage;
        _headerRectTransform.anchorMin = new Vector2(0, headerAnchorMinY);
        _headerRectTransform.anchorMax = new Vector2(1, 1);
        
        _bodyRectTransform.anchorMin = new Vector2(0, 0);
        _bodyRectTransform.anchorMax = new Vector2(1, headerAnchorMinY);
    }
    
    private void CalculateSettingMinAndMaxValues()
    {
        (_minMatchableVariety, _maxMatchableVariety) = _gameSettings.GetRangeAttributeLimits("NumberOfMatchables");
        (_minBoardWidth, _maxBoardWidth) = _gameSettings.GetRangeAttributeLimits("BoardWidth");
        (_minBoardHeight, _maxBoardHeight) = _gameSettings.GetRangeAttributeLimits("BoardHeight");
    }
    
    private void ConfigureSliders()
    {
        //Matchable Variety
        _minMatchableVarietyLabel.text = _minMatchableVariety.ToString();
        _maxMatchableVarietyLabel.text = _maxMatchableVariety.ToString();
        _matchableVarietySlider.minValue = _minMatchableVariety;
        _matchableVarietySlider.maxValue = _maxMatchableVariety;
        _matchableVarietySlider.value = _gameSettings.NumberOfMatchables;
        
        //Board Width
        _minBoardWidthLabel.text = _minBoardWidth.ToString();
        _maxBoardWidthLabel.text = _maxBoardWidth.ToString();
        _boardWidthSlider.minValue = _minBoardWidth;
        _boardWidthSlider.maxValue = _maxBoardWidth;
        _boardWidthSlider.value = _gameSettings.BoardWidth;
        
        //Board Height
        _minBoardHeightLabel.text = _minBoardHeight.ToString();
        _maxBoardHeightLabel.text = _maxBoardHeight.ToString();
        _boardHeightSlider.minValue = _minBoardHeight;
        _boardHeightSlider.maxValue = _maxBoardHeight;
        _boardHeightSlider.value = _gameSettings.BoardHeight;
    }
    
    private void SubscribeToEvents()
    {
        _resetButton.onClick.AddListener(FireOnGameSettingsChangedEvent);
        _showButton.onClick.AddListener(Show);
        _hideButton.onClick.AddListener(() => Hide());
        
        _matchableVarietySlider.onValueChanged.AddListener(OnMatchableVarietySliderChanged);
        _boardWidthSlider.onValueChanged.AddListener(OnBoardWidthSliderChanged);
        _boardHeightSlider.onValueChanged.AddListener(OnBoardHeightSliderChanged);
    }

    private void OnMatchableVarietySliderChanged(float value)
    {
        _gameSettings.NumberOfMatchables = (int)value;
    }

    private void OnBoardWidthSliderChanged(float value)
    {
        _gameSettings.BoardWidth = (int)value;
    }

    private void OnBoardHeightSliderChanged(float value)
    {
        _gameSettings.BoardHeight = (int)value;
    }

    private void FireOnGameSettingsChangedEvent()
    {
        Hide();
        OnGameSettingsChanged?.Invoke();
    }
    
    private void Show()
    {
        if (!_bodyHidden)
        {
            return;
        }
        
        _bodyRectTransform.gameObject.SetActive(true);

        float startValue = _bodyRect.height;//_bodyRectTransform.anchoredPosition.y;
        float endValue = 0;//startValue - _bodyRect.height;
        var duration = 1f;
        
        DOVirtual.Float(startValue,
                endValue,
                duration,
                updatedValue => UpdateBodyPosition(updatedValue))
            .OnComplete(() =>
            {
                ToggleShowButtonActive(false);
            });
            
            _bodyHidden = false;
    }

    private void Hide(bool animate = true)
    {
        if (_bodyHidden)
        {
            return;
        }

        if (animate)
        {
            float startValue = 0;
            float endValue = startValue + _bodyRect.height;
            var duration = 1f;

            DOVirtual.Float(startValue,
                    endValue,
                    duration,
                    updatedValue => UpdateBodyPosition(updatedValue))
                .OnComplete(()=>
                {
                    HideWithoutAnimation();
                    ToggleShowButtonActive(true);
                });
        }
        else
        {
            HideWithoutAnimation();
        }
        
        _bodyHidden = true;
    }

    private void HideWithoutAnimation()
    {
        var xAnchoredPosition = _bodyRectTransform.anchoredPosition.x;
        var newPos = new Vector2(xAnchoredPosition,_bodyRect.height);
        _bodyRectTransform.anchoredPosition += newPos;
            
        _bodyRectTransform.gameObject.SetActive(false);
    }

    private void UpdateBodyPosition(float newHeight)
    {
        var xAnchoredPosition = _bodyRectTransform.anchoredPosition.x;
        var newPos = new Vector2(xAnchoredPosition,newHeight);
        _bodyRectTransform.anchoredPosition = newPos;
    }

    void Start()
    {
        Debug.LogWarning($"Start | Anchored Position: {_headerRectTransform.anchoredPosition}");

        Debug.LogWarning($"Start | Size Delta: {_headerRectTransform.sizeDelta}");

        var rect = _headerRectTransform.rect;
        
        Debug.LogWarning($"Start | Size Delta: {rect.height}");

        //TODO Down animation:
        //anchoredPosition.y: rect.height => 0
        
        //TODO Up animation:
        //anchoredPosition.y: 0 => rect.height
        
        // Code for animation with DoTween
        // DOTween.To(() => startValue, x => currentValue = x, endValue, duration)
        //     .OnUpdate(() => {
        //         // Este callback se llama en cada frame durante la animación
        //         // Puedes usar currentValue aquí para hacer algo con el valor actualizado del float
        //         Debug.Log("Current Value: " + currentValue);
        //     })
        //     .OnComplete(() => {
        //         // Este callback se llama cuando la animación ha terminado
        //         Debug.Log("Animation completed!");
        //     });
        
        //TODO Implement Show()/Hide()
        
        // use bool settingsPanelVisible; Use it in  Show()/Hide() to control whether the animation should be played or not.
        
        // _headerRectTransform.anchoredPosition += Vector2.up * rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"Update | Anchored Position: {_headerRectTransform.anchoredPosition}");

        Debug.Log($"Update | Size Delta: {_headerRectTransform.sizeDelta}");
    }
}
