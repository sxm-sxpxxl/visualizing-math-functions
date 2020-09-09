using UnityEngine;
using UnityEngine.UI;

public class ResetCameraTransform : MonoBehaviour
{
    [SerializeField] private Button resetCameraButton;

    private struct CameraTransform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
    }

    private CameraTransform _defaultCameraTransform;

    private void Start()
    {
        if (resetCameraButton == null)
            throw new MissingReferenceException($"{nameof(resetCameraButton)} isn't set on ResetCameraTransform component.");

        resetCameraButton.onClick.AddListener(Reset);
        _defaultCameraTransform = new CameraTransform
        {
            Position = transform.position,
            Rotation = transform.rotation,
            Scale = transform.localScale
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(PointerSimulator.Press(resetCameraButton));
        }
    }

    // TODO: Consider making the camera movement smoothly when reset.
    public void Reset()
    {
        transform.position = _defaultCameraTransform.Position;
        transform.rotation = _defaultCameraTransform.Rotation;
        transform.localScale = _defaultCameraTransform.Scale;
    }
}