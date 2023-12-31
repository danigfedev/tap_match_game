using System;
using System.Collections;
using _AppAssets.Code;
using _AppAssets.Code.Settings;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [Header("Main UI Elements")]
    [SerializeField] private RectTransform _headerRectTransform;
    [SerializeField] private RectTransform _bodyRectTransform;
    [SerializeField] private GameObject _uiBlocker;
    [SerializeField] private float _showAnimationDuration = 0.5f;
    [SerializeField] private float _hideAnimationDuration = 0.5f;
    
    [Header("Mobile animation (DOTween not working)")]
    [SerializeField] private AnimationCurve _showAnimationCurve;
    [SerializeField] private AnimationCurve _hideAnimationCurve;

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
    [SerializeField] private TMPro.TextMeshProUGUI _matchableVarietyLabel;
    [SerializeField] private Slider _matchableVarietySlider;
    
    [Space] 
    [Header("Board Width")] 
    [SerializeField] private TMPro.TextMeshProUGUI _minBoardWidthLabel;
    [SerializeField] private TMPro.TextMeshProUGUI _maxBoardWidthLabel;
    [SerializeField] private TMPro.TextMeshProUGUI _boardWidthLabel;
    [SerializeField] private Slider _boardWidthSlider;
    
    [Header("Board Height")] 
    [SerializeField] private TMPro.TextMeshProUGUI _minBoardHeightLabel;
    [SerializeField] private TMPro.TextMeshProUGUI _maxBoardHeightLabel;
    [SerializeField] private TMPro.TextMeshProUGUI _boardHeightLabel;
    [SerializeField] private Slider _boardHeightSlider;
    
    [Header("Device Orientation")] 
    [SerializeField] private CanvasGroup _portraitOrientationGroup;
    [SerializeField] private CanvasGroup _landscapeOrientationGroup;
    
    [Header("Buttons")]
    [SerializeField] private Button _resetButton;

    public event Action NotifySettingsPanelShown;
    public event Action NotifySettingsPanelHidden;
    public event Action OnGameSettingsChanged;
    
    private GameSettings _gameSettings;
    private DisplaySettings _displaySettings;

    private CanvasScaler _uiCanvasScaler;
    private Rect _bodyRect; //This throws wrong values on device => _bodyRect.height = 6.341049E+32
    private bool _bodyHidden = false;
    private int _minMatchableVariety;
    private int _maxMatchableVariety;
    private int _minBoardWidth;
    private int _maxBoardWidth;
    private int _minBoardHeight;
    private int _maxBoardHeight;

    private string _matchableVarietyValueLabel;
    private string _boardWidthValueLabel;
    private string _boardHeightValueLabel;

    private int _widthSliderValue; 
    private int _heightSliderValue;
    private float _bodyHeight;
    private float _elapsedTime;

    public void Initialize(GameSettings gameSettings, DisplaySettings displaySettings)
    {
        _gameSettings = gameSettings;
        _displaySettings = displaySettings;
        _uiCanvasScaler = GetComponent<CanvasScaler>();
        _bodyHeight = (_displaySettings.FooterHeightScreenPercentage + _displaySettings.BoardHeightScreenPercentage) *
                      _uiCanvasScaler.referenceResolution.y;
        
        AdjustDimensions();
        
        InitializeHeader();
        ToggleUIBlockerActive(false);
        
        SetupBodyAnimations();

        InitializeOrientationPanel();
        CalculateSettingMinAndMaxValues();
        ConfigureSliders();
        
        SubscribeToButtonEvents();
    }

    public void UpdateUI(int turnCount, int scoreCount)
    {
        _turnCountLabel.text = turnCount.ToString();
        _scoreLabel.text = scoreCount.ToString();
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

    public void ToggleUIBlockerActive(bool activateBlocker)
    {
        _uiBlocker.SetActive(activateBlocker);
    } 

    private void SetupBodyAnimations()
    {
         _bodyRect = _bodyRectTransform.rect;
         HideBody(false);
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
        _matchableVarietyValueLabel = _matchableVarietyLabel.text;
        
        //Board Width
        _minBoardWidthLabel.text = _minBoardWidth.ToString();
        _maxBoardWidthLabel.text = _maxBoardWidth.ToString();
        _boardWidthSlider.minValue = _minBoardWidth;
        _boardWidthSlider.maxValue = _maxBoardWidth;
        _boardWidthSlider.value = _gameSettings.BoardWidth;
        _boardWidthValueLabel = _boardWidthLabel.text;
        
        //Board Height
        _minBoardHeightLabel.text = _minBoardHeight.ToString();
        _maxBoardHeightLabel.text = _maxBoardHeight.ToString();
        _boardHeightSlider.minValue = _minBoardHeight;
        _boardHeightSlider.maxValue = _maxBoardHeight;
        _boardHeightSlider.value = _gameSettings.BoardHeight;
        _boardHeightValueLabel = _boardHeightLabel.text;
        
        ResetSliders();
    }
    
    private void SubscribeToButtonEvents()
    {
        _resetButton.onClick.AddListener(OnApplySettingsClicked);
        _showButton.onClick.AddListener(ShowBody);
        _hideButton.onClick.AddListener(() => HideBody());
    }

    private void SubscribeToSliderEvents()
    {
        _matchableVarietySlider.onValueChanged.AddListener(OnMatchableVarietySliderChanged);
        _boardWidthSlider.onValueChanged.AddListener(OnBoardWidthSliderChanged);
        _boardHeightSlider.onValueChanged.AddListener(OnBoardHeightSliderChanged);
    }

    private void UnsubscribeFromSliderEvents()
    {
        _matchableVarietySlider.onValueChanged.RemoveAllListeners();
        _boardWidthSlider.onValueChanged.RemoveAllListeners();
        _boardHeightSlider.onValueChanged.RemoveAllListeners();
    }

    private void ResetSliders()
    {
        _matchableVarietyLabel.text = string.Format(_matchableVarietyValueLabel, _gameSettings.NumberOfMatchables);
        _boardWidthLabel.text = string.Format(_boardWidthValueLabel, _gameSettings.BoardWidth);
        _boardHeightLabel.text = string.Format(_boardHeightValueLabel, _gameSettings.BoardHeight);

        _matchableVarietySlider.value = _gameSettings.NumberOfMatchables;
        _boardWidthSlider.value = _gameSettings.BoardWidth;
        _boardHeightSlider.value = _gameSettings.BoardHeight;
    }

    private void ApplySettings()
    {
        _gameSettings.NumberOfMatchables = (int)_matchableVarietySlider.value;
        _gameSettings.BoardWidth = (int)_boardWidthSlider.value;
        _gameSettings.BoardHeight = (int)_boardHeightSlider.value;
    } 
    private void OnMatchableVarietySliderChanged(float value)
    {
        _matchableVarietyLabel.text = string.Format(_matchableVarietyValueLabel, value);
    }

    private void OnBoardWidthSliderChanged(float value)
    {
        _widthSliderValue = (int) value;
        _boardWidthLabel.text = string.Format(_boardWidthValueLabel, _widthSliderValue);
        UpdateOrientationPanel();
    }

    private void OnBoardHeightSliderChanged(float value)
    {
        _heightSliderValue = (int)value;
        _boardHeightLabel.text = string.Format(_boardHeightValueLabel, _heightSliderValue);
        UpdateOrientationPanel();
    }

    private void OnApplySettingsClicked()
    {
        HideBody();
        ApplySettings();
        OnGameSettingsChanged?.Invoke();
    }
    
    private void ShowBody()
    {
        if (!_bodyHidden)
        {
            return;
        }

        NotifySettingsPanelShown?.Invoke();
        ToggleShowButtonActive(false);
        
        _bodyRectTransform.gameObject.SetActive(true);
        ToggleUIBlockerActive(true);
        ResetSliders();
        SubscribeToSliderEvents();
        InitializeOrientationPanel();
        
        _bodyHidden = false;
        
        float startValue = _bodyHeight;
        float endValue = 0;
        AnimateScreenPanel(startValue, endValue, _showAnimationDuration, UpdateBodyPosition, OnShowAnimationComplete,
            _showAnimationCurve, Ease.OutBounce);
    }
    
    private void HideBody(bool animate = true)
    {
        if (_bodyHidden)
        {
            return;
        }

        ToggleShowButtonActive(true);
        UnsubscribeFromSliderEvents();
        
        if (animate)
        {
            ToggleUIBlockerActive(true);
            
            float startValue = 0;
            float endValue = _bodyHeight;
            
            AnimateScreenPanel(startValue, endValue, _hideAnimationDuration, UpdateBodyPosition, OnHideAnimationComplete, _hideAnimationCurve, Ease.OutCubic);
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
        var newPos = new Vector2(xAnchoredPosition,_bodyHeight);
        _bodyRectTransform.anchoredPosition = newPos;
            
        _bodyRectTransform.gameObject.SetActive(false);
    }
    
    private void AnimateScreenPanel(float startValue, float endValue, float duration, Action<float> onUpdateCallback, Action onCompletecallback, AnimationCurve curve, Ease doTweenEase)
    {
        AnimateDoTween(startValue, endValue, duration, onUpdateCallback, onCompletecallback, doTweenEase);
        
        //Tests derived from _bodyRect throwing wrong values on device
        // if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        // {
        //     StartCoroutine(AnimateCoroutine(
        //         startValue,
        //         endValue,
        //         duration,
        //         curve,
        //         onUpdateCallback, 
        //         onCompletecallback));
        // }
        // else
        // {
        //     AnimateDoTween(startValue, endValue, duration, onUpdateCallback, onCompletecallback, doTweenEase);
        // }
    }
    
    private void AnimateDoTween(float startValue, float endValue, float duration, Action<float> onUpdateCallback, Action onCompletecallback, Ease ease)
    {
        DOVirtual.Float(startValue,
                endValue,
                duration,
                updatedValue => onUpdateCallback?.Invoke(updatedValue))
            .SetEase(ease)
            .OnComplete(() => onCompletecallback?.Invoke());
    }
    
    // Implemented because of errors with _bodyRect on device.
    // Not needed since this didn't fix anything. Leaving it here for the effort it took. 
    private IEnumerator AnimateCoroutine(float startValue, float endValue, float duration, AnimationCurve curve, Action<float> onUpdateCallback, Action onCompletecallback)
    {
        _elapsedTime = 0;
       
        while (_elapsedTime <= 1)
        {
            var t = curve.Evaluate(_elapsedTime);
            var newYPosition = Mathf.Lerp(startValue, endValue, t);
            onUpdateCallback?.Invoke(newYPosition);
            
            _elapsedTime += Time.deltaTime / duration;
            
            yield return null;

            newYPosition = endValue;
            onUpdateCallback?.Invoke(newYPosition);
        }

        onCompletecallback?.Invoke();
    }

    private void OnShowAnimationComplete()
    {
        ToggleUIBlockerActive(false);
    }
    
    private void OnHideAnimationComplete()
    {
        HideWithoutAnimation();
        ToggleUIBlockerActive(false);
        NotifySettingsPanelHidden?.Invoke();
    }

    private void UpdateBodyPosition(float newHeight)
    {
        var xAnchoredPosition = _bodyRectTransform.anchoredPosition.x;
        var newPos = new Vector2(xAnchoredPosition,newHeight);
        _bodyRectTransform.anchoredPosition = newPos;
    }
    
    private void InitializeOrientationPanel()
    {
        _widthSliderValue = _gameSettings.BoardWidth;
        _heightSliderValue = _gameSettings.BoardHeight;
        UpdateOrientationPanel();
    }

    private void UpdateOrientationPanel()
    {
        var isForcedLandscape = _gameSettings.ShouldForceLandscape(_widthSliderValue, _heightSliderValue);

        if (isForcedLandscape)
        {
            _landscapeOrientationGroup.alpha = 1f;
            _portraitOrientationGroup.alpha = 0.5f;
        }
        else
        {
            _landscapeOrientationGroup.alpha = 0.5f;
            _portraitOrientationGroup.alpha = 1f;
        }
    }
}
