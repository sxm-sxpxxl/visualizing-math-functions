using UnityEngine;
using UnityEngine.Events;

public class InputListener : MonoBehaviour
{
    [SerializeField] private KeyCode keyCode;
    [SerializeField] private UnityEvent onPress;
        
    private void Update()
    {
        if (Input.GetKeyDown(keyCode))
        {
            onPress.Invoke();
        }
    }
}
