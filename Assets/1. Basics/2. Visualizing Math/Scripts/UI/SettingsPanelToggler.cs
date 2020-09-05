using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SettingsPanelToggler : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    private static readonly int IsOpenParameterHash = Animator.StringToHash("isOpen");
    private Animator _settingsPanelAnimator;
    
    private Button _settingsButton;
    private Color _unpressedSettingsButtonNormalColor, _pressedSettingsButtonNormalColor;

    private void Start()
    {
        _settingsButton = GetComponent<Button>();
        var colors = _settingsButton.colors;
        _unpressedSettingsButtonNormalColor = colors.normalColor;
        _pressedSettingsButtonNormalColor = colors.pressedColor;
        
        _settingsPanelAnimator = settingsPanel.GetComponent<Animator>();
        if (_settingsPanelAnimator == null)
            throw new MissingComponentException($"{settingsPanel.name} doesn't has Animator component.");

        _settingsPanelAnimator.SetBool(IsOpenParameterHash, false);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            StartCoroutine(PointerSimulator.Press(_settingsButton));
        }
    }
    
    public void Toggle()
    {
        var currentOpenState = _settingsPanelAnimator.GetBool(IsOpenParameterHash);
        var nextOpenState = !currentOpenState;
        
        _settingsPanelAnimator.SetBool(IsOpenParameterHash, nextOpenState);
        
        var nextNormalColor = nextOpenState ? _pressedSettingsButtonNormalColor : _unpressedSettingsButtonNormalColor;
        SetSettingsButtonNormalColor(nextNormalColor);
    }

    private void SetSettingsButtonNormalColor(Color color)
    {
        var currentColors = _settingsButton.colors;
        
        currentColors.normalColor = color;
        _settingsButton.colors = currentColors;
    }
}
