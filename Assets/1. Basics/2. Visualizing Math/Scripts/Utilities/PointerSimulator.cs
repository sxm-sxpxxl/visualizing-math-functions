using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class PointerSimulator
{
    public static IEnumerator Press(Button button, float durationPressing = 0.1f)
    {
        var pointerEventData = new PointerEventData(EventSystem.current);
        
        button.OnPointerDown(pointerEventData);
        yield return new WaitForSeconds(durationPressing);
        
        button.OnPointerClick(pointerEventData);
        button.OnPointerUp(pointerEventData);
    }
}
