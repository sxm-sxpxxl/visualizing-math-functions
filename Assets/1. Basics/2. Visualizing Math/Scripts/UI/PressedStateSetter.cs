using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PressedStateSetter : MonoBehaviour
{
    private Button _button;
    private Color _unpressedButtonColor, _pressedButtonColor;

    private void Start()
    {
        _button = GetComponent<Button>();
        var colors = _button.colors;
        _unpressedButtonColor = colors.normalColor;
        _pressedButtonColor = colors.pressedColor;
    }

    public void SetPressedState()
    {
        var currentColor = _button.colors.normalColor;

        var nextColor = currentColor == _pressedButtonColor ? _unpressedButtonColor : _pressedButtonColor;
        SetButtonColor(nextColor);
    }
    
    private void SetButtonColor(Color color)
    {
        var currentColors = _button.colors;
        
        currentColors.normalColor = color;
        _button.colors = currentColors;
    }
}
