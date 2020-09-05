using UnityEngine;
using UnityEngine.UI;

public class InputButtonSimulator : MonoBehaviour
{
    private Button _button;
    
    private void Start()
    {
        _button = GetComponent<Button>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            StartCoroutine(PointerSimulator.Press(_button));
        }
    }
}
