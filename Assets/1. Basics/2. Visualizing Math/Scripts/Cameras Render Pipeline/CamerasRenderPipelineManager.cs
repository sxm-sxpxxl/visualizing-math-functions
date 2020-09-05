using System;
using System.Collections.Generic;
using System.Linq;

public enum ECamerasRenderPreset
{
    MasterCompletelyOverlapSlaveWithoutDistortion,
    SlaveCompletelyOverlapMasterWithoutDistortion,
    MasterOverlapSlaveWithoutDistortion,
    MasterCompletelyOverlapSlaveWithDistortion,
    SlaveCompletelyOverlapMasterWithDistortion,
    MasterOverlapSlaveWithDistortion
}

public enum ECameraPriorityType
{
    Master,
    Slave
}

public struct CameraRenderStatePropertyPath
{
    public ECameraPriorityType CameraPriorityType { get; set; }
    public ECameraRenderStatePropertyName FunctionalPropertyName { get; set; }
    public ECameraRenderStatePropertyName EnabledPropertyName { get; set; }
}

public class CamerasRenderPipelineManager : ICamerasRenderPipelineManager
{
    public CameraRenderState MasterCameraState => _cameraRenderStates[ECameraPriorityType.Master];
    public CameraRenderState SlaveCameraState => _cameraRenderStates[ECameraPriorityType.Slave];
    public ECamerasRenderPreset CamerasRenderPreset { get; private set; }

    public bool this[ECameraPriorityType priorityType, ECameraRenderStatePropertyName propertyName] =>
        _cameraRenderStates[priorityType][propertyName];

    private static readonly Dictionary<string, ECamerasRenderPreset> PresetMasks = new Dictionary<string, ECamerasRenderPreset>
    {
        { "10xx", ECamerasRenderPreset.MasterCompletelyOverlapSlaveWithoutDistortion },
        { "0xx0", ECamerasRenderPreset.SlaveCompletelyOverlapMasterWithoutDistortion },
        { "1110", ECamerasRenderPreset.MasterOverlapSlaveWithoutDistortion },
        { "x10x", ECamerasRenderPreset.MasterCompletelyOverlapSlaveWithDistortion },
        { "0xx1", ECamerasRenderPreset.SlaveCompletelyOverlapMasterWithDistortion },
        { "1111", ECamerasRenderPreset.MasterOverlapSlaveWithDistortion }
    };

    private static readonly Dictionary<int, CameraRenderStatePropertyPath> MapOfPresetMaskToCameraRenderStateProperty =
        new Dictionary<int, CameraRenderStatePropertyPath>
        {
            {
                0, new CameraRenderStatePropertyPath
                {
                    CameraPriorityType = ECameraPriorityType.Master,
                    FunctionalPropertyName = ECameraRenderStatePropertyName.IsActive,
                    EnabledPropertyName = ECameraRenderStatePropertyName.IsEnabledActive
                }
            },
            {
                1, new CameraRenderStatePropertyPath
                {
                    CameraPriorityType = ECameraPriorityType.Master,
                    FunctionalPropertyName = ECameraRenderStatePropertyName.IsClearDepth,
                    EnabledPropertyName = ECameraRenderStatePropertyName.IsEnabledClearDepth
                }
            },
            {
                2, new CameraRenderStatePropertyPath
                {
                    CameraPriorityType = ECameraPriorityType.Slave,
                    FunctionalPropertyName = ECameraRenderStatePropertyName.IsActive,
                    EnabledPropertyName = ECameraRenderStatePropertyName.IsEnabledActive
                }
            },
            {
                3, new CameraRenderStatePropertyPath
                {
                    CameraPriorityType = ECameraPriorityType.Slave,
                    FunctionalPropertyName = ECameraRenderStatePropertyName.IsClearDepth,
                    EnabledPropertyName = ECameraRenderStatePropertyName.IsEnabledClearDepth
                }
            }
        };

    private readonly Dictionary<ECameraPriorityType, CameraRenderState> _cameraRenderStates;
    
    public CamerasRenderPipelineManager()
    {
        _cameraRenderStates = new Dictionary<ECameraPriorityType, CameraRenderState>
        {
            { ECameraPriorityType.Master, new CameraRenderState { IsActive = true, IsClearDepth = false } },
            { ECameraPriorityType.Slave, new CameraRenderState { IsActive = true, IsClearDepth = true } }
        };
        
        UpdatePresetByCamerasRenderPreset();
    }

    public void SetCameraRenderStateFunctionalProperty(CameraRenderStatePropertyPath path, bool value)
    {
        _cameraRenderStates[path.CameraPriorityType][path.FunctionalPropertyName] = value;
        UpdatePresetByCamerasRenderPreset();
    }

    public void SetCamerasRenderPreset(ECamerasRenderPreset preset)
    {
        UpdateCamerasRenderStateByPreset(preset);
        CamerasRenderPreset = preset;
    }

    private void UpdatePresetByCamerasRenderPreset()
    {
        var matchedPreset = GetPresetByCameraRenderState();
        SetCamerasRenderPreset(matchedPreset);
    }

    private void UpdateCamerasRenderStateByPreset(ECamerasRenderPreset preset)
    {
        if (PresetMasks.ContainsValue(preset) == false)
            throw new NullReferenceException("The preset was not found in actual presets");
        
        var matchedPresetMask = PresetMasks.First(mask => mask.Value == preset).Key;

        for (var i = 0; i < matchedPresetMask.Length; i++)
        {
            var targetMap = MapOfPresetMaskToCameraRenderStateProperty[i];
            var targetCameraState = _cameraRenderStates[targetMap.CameraPriorityType];
            
            targetCameraState[targetMap.EnabledPropertyName] = matchedPresetMask[i] != 'x';
            if (matchedPresetMask[i] == 'x') continue;

            var convertedPresetMaskElement = matchedPresetMask[i].ToBoolean();
            targetCameraState[targetMap.FunctionalPropertyName] = convertedPresetMaskElement;
        }
        
        if (SlaveCameraState.IsActive == false)
            MasterCameraState.IsEnabledActive = false;
        
        if (MasterCameraState.IsActive == false)
            SlaveCameraState.IsEnabledActive = false;
        
        if (MasterCameraState.IsActive == false && SlaveCameraState.IsEnabledActive == false)
            SlaveCameraState.IsActive = true;
            
        if (SlaveCameraState.IsActive == false && MasterCameraState.IsEnabledActive == false)
            MasterCameraState.IsActive = true;
    }

    private ECamerasRenderPreset GetPresetByCameraRenderState()
    {
        ECamerasRenderPreset camerasRenderPreset = default;
        
        foreach (var currentPresetMask in PresetMasks.Keys)
        {
            var isMatchPresetMask = true;
            for (var i = 0; i < currentPresetMask.Length; i++)
            {
                if (currentPresetMask[i] == 'x') 
                    continue;

                var targetMap = MapOfPresetMaskToCameraRenderStateProperty[i];
                var matchedProperty = _cameraRenderStates[targetMap.CameraPriorityType][targetMap.FunctionalPropertyName];

                var convertedPresetMaskElement = currentPresetMask[i].ToBoolean();
                isMatchPresetMask &= convertedPresetMaskElement == matchedProperty;
            }

            if (isMatchPresetMask)
            {
                camerasRenderPreset = PresetMasks[currentPresetMask];
                break;
            }
        }

        return camerasRenderPreset;
    }
}