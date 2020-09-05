using UnityEngine;

public class SmoothedMovement : MonoBehaviour
{
    [SerializeField] private float durationMovement = 1f;
    [SerializeField] [Range(0f, 1f)] private float rotationSpeed = 1f;
    
    private Quaternion _startRotation;
    private Quaternion _endRotation;
    private float _currentDuration;

    private void Start()
    {
        _startRotation = transform.localRotation;
        _endRotation = _startRotation;
    }

    private void Update()
    {
        var deltaTime = Time.deltaTime;

        DrawCubeAxis();
        
        if (Input.GetMouseButton(0))
        {
            var delta = 100f * rotationSpeed * deltaTime;
            var inputMouseX = Input.GetAxis("Mouse X");
            var inputMouseY = Input.GetAxis("Mouse Y");

            var deltaXRotation = inputMouseY * delta;
            var deltaYRotation = inputMouseX * delta;
            var deltaRotationVec = new Vector3(0f, -deltaYRotation, deltaXRotation);

            var deltaRotation = Quaternion.Euler(deltaRotationVec);
            var deltaRotationForEnd = Quaternion.Euler(1f * deltaRotationVec);

            transform.localRotation *= deltaRotation;

            _startRotation = transform.localRotation;
            _endRotation = _startRotation * deltaRotationForEnd;
            _currentDuration = 0f;
            
            Debug.Log($"EndRotation: {_endRotation.eulerAngles}");
        }
        else
        {
            if (_currentDuration > durationMovement)
            {
                return;
            }
            
            var t = EaseOutCubic(_currentDuration / durationMovement);
            var deltaRotationForEnd = Quaternion.Slerp(_startRotation, _endRotation, t);
            _currentDuration += deltaTime;
            
            transform.localRotation *= deltaRotationForEnd * Quaternion.Euler(-transform.localEulerAngles);

            
            /////
            
            // transform.localRotation *= deltaRotation * Quaternion.Euler(-transform.localEulerAngles);

            // if (_currentDuration > durationMovement)
            //     return;
            //
            // var t = EaseLinear01(_currentDuration / durationMovement);
            //
            // // var deltaRotation = Vector3.Lerp(_startRotation, _endRotation, t);
            // // deltaRotation -= transform.localEulerAngles;
            //
            // var newRotation = Quaternion.Slerp(_startRotation, _endRotation, t).eulerAngles;
            // var deltaRotation = Quaternion.Euler(newRotation - transform.localEulerAngles);
            //
            // _currentDuration += Time.deltaTime;
            //
            // transform.localRotation = deltaRotation;
        }
        
        // if (_currentDuration > durationMovement)
        // {
        //     _currentDuration = 0f;
        //     transform.localRotation = Quaternion.Euler(_startRotation);
        // }
        //
        // var deltaRotation = Vector3.Lerp(_startRotation, _endRotation, EaseOutCubic(_currentDuration / durationMovement));
        // deltaRotation -= transform.localEulerAngles;
        //
        // _currentDuration += Time.deltaTime;
        //
        // transform.Rotate(deltaRotation);
    }

    private void DrawCubeAxis()
    {
        var startPos = transform.position;
        
        Debug.DrawLine(startPos, transform.forward, Color.blue);
        Debug.DrawLine(startPos, transform.up, Color.green);
        Debug.DrawLine(startPos, transform.right, Color.red);
    }

    private float EaseLinear01(float t) => t;

    private float EaseInQuad01(float t) => t * t;

    private float EaseOutCubic(float t) => --t * t * t + 1;
}
