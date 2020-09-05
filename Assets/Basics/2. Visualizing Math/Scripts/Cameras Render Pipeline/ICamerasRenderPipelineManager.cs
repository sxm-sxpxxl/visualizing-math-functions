public interface ICamerasRenderPipelineManager
{
    CameraRenderState MasterCameraState { get; }
    CameraRenderState SlaveCameraState { get; }
    ECamerasRenderPreset CamerasRenderPreset { get; }
    
    bool this[ECameraPriorityType priorityType, ECameraRenderStatePropertyName propertyName] { get; }
    void SetCameraRenderStateFunctionalProperty(CameraRenderStatePropertyPath path, bool value);
    void SetCamerasRenderPreset(ECamerasRenderPreset preset);
}
