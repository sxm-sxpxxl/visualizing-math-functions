using System.Collections.Generic;

public enum ECameraRenderStatePropertyName
{
    IsActive,
    IsEnabledActive,
    IsClearDepth,
    IsEnabledClearDepth
}

public class CameraRenderState
{
    public bool IsActive { get => this[ECameraRenderStatePropertyName.IsActive]; set => this[ECameraRenderStatePropertyName.IsActive] = value; }
    public bool IsEnabledActive { get => this[ECameraRenderStatePropertyName.IsEnabledActive]; set => this[ECameraRenderStatePropertyName.IsEnabledActive] = value; }
    public bool IsClearDepth { get => this[ECameraRenderStatePropertyName.IsClearDepth]; set => this[ECameraRenderStatePropertyName.IsClearDepth] = value; }
    public bool IsEnabledClearDepth { get => this[ECameraRenderStatePropertyName.IsEnabledClearDepth]; set => this[ECameraRenderStatePropertyName.IsEnabledClearDepth] = value; }

    private readonly Dictionary<ECameraRenderStatePropertyName, bool> _cameraProperties = new Dictionary<ECameraRenderStatePropertyName, bool>
    {
        { ECameraRenderStatePropertyName.IsActive, false },
        { ECameraRenderStatePropertyName.IsEnabledActive, false },
        { ECameraRenderStatePropertyName.IsClearDepth, false },
        { ECameraRenderStatePropertyName.IsEnabledClearDepth, false }
    };
    
    public bool this[ECameraRenderStatePropertyName propertyName]
    {
        get => _cameraProperties[propertyName];
        set => _cameraProperties[propertyName] = value;
    }
}
