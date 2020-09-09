using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Transform focus = default;
    [SerializeField, Range(0f, 10f)] float focusDistance = 5f;
    [SerializeField, Range(1f, 5f)] float focusZoomSpeed = 2.5f;
    [SerializeField, Min(0f)] float minFocusDistance = 0f;
    [SerializeField] float maxFocusDistance = 15f;

    [SerializeField, Range(1f, 360f)] private float rotationSpeed = 90f;
    [SerializeField, Range(-89f, 89f)] float minVerticalAngle = -60f, maxVerticalAngle = 60f;
    [SerializeField, Range(0f, 5f)] float dampingRotationDuration = 2.5f;

    private Vector2 _orbitAngles = new Vector2(45f, 0f);
    private Vector2 _lastRotationDirection;
    private float _lastRotationTime;

    private void Awake()
    {
        transform.rotation = Quaternion.Euler(_orbitAngles);
    }

    private void OnValidate()
    {
        if (maxVerticalAngle < minVerticalAngle) maxVerticalAngle = minVerticalAngle;
        if (maxFocusDistance < minFocusDistance) maxFocusDistance = minFocusDistance;
    }

    private void Update()
    {
        HandleRotationInput();
        HandleZoomInput();

        var lookRotation = Quaternion.Euler(_orbitAngles);
        var lookDirection = lookRotation * Vector3.forward;
        var lookPosition = focus.position - lookDirection * focusDistance;

        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }

    private void HandleRotationInput()
    {
        const float eps = 0.001f;

        var keyBoardInput = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        var mouseInput = Input.GetMouseButton(1) ? new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) : Vector2.zero;
        var input = keyBoardInput.sqrMagnitude > mouseInput.sqrMagnitude ? keyBoardInput : mouseInput;

        if (input.sqrMagnitude > eps)
        {
            _orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
            _lastRotationDirection = input;
            _lastRotationTime = Time.unscaledTime;
        }
        else
        {
            var dampingTime = Mathf.Clamp(Time.unscaledTime - _lastRotationTime, 0f, dampingRotationDuration);
            var damping = 1 - dampingTime / dampingRotationDuration;
            if (damping > eps)
            {
                _orbitAngles += damping * rotationSpeed * Time.unscaledDeltaTime * _lastRotationDirection;
            }
        }

        _orbitAngles = ConstrainAngles(_orbitAngles, minVerticalAngle, maxVerticalAngle);
    }

    private void HandleZoomInput()
    {
        var mouseScrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
        var keyBoardInput = Input.GetAxis("Zoom");

        var input = Mathf.Abs(mouseScrollWheelInput) > Mathf.Abs(keyBoardInput) ? mouseScrollWheelInput : keyBoardInput;

        focusDistance += focusZoomSpeed * Time.unscaledDeltaTime * input;
        focusDistance = Mathf.Clamp(focusDistance, minFocusDistance, maxFocusDistance);
    }

    private static Vector2 ConstrainAngles(Vector2 angles, float min, float max)
    {
        angles.x = Mathf.Clamp(angles.x, min, max);

        if (angles.y < 0f)
        {
            angles.y += 360f;
        }
        else if (angles.y >= 360f)
        {
            angles.y -= 360f;
        }
        return angles;
    }
}