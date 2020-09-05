using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] [Range(0.5f, 1f)] private float rotationSpeed = 0.5f;
    [SerializeField] private ZoomInfo zoom;
    
    [Serializable] public class ZoomInfo
    {
        [Range(0.5f, 1f)] public float speed = 0.5f;
        public float minScale = 0.8f;
        public float maxScale = 2f;
    }

    private void Update()
    {
        var normal = 100f;
        var deltaTime = Time.deltaTime;
        
        var inputMouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");
        var deltaZScale = normal * zoom.speed * inputMouseScrollWheel * deltaTime;

        var currentScale = transform.localScale;
        var newZScale = Mathf.Clamp(currentScale.z - deltaZScale, zoom.minScale, zoom.maxScale);
        transform.localScale = new Vector3(currentScale.x, currentScale.y, newZScale);
        
        if (Input.GetMouseButton(0))
        {
            var inputMouseX = Input.GetAxis("Mouse X");
            var inputMouseY = Input.GetAxis("Mouse Y");

            var baseRotation = normal * rotationSpeed * deltaTime;
            var deltaXRotation = inputMouseY * baseRotation;
            var deltaYRotation = inputMouseX * baseRotation;

            transform.localRotation *= Quaternion.Euler(-deltaXRotation, deltaYRotation, 0f);
        }
    }
}
