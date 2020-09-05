using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum ECamerasRenderPreset
{
    MasterCompletelyOverlapSlaveWithoutDistortion,
    SlaveCompletelyOverlapMasterWithoutDistortion,
    MasterOverlapSlaveWithoutDistortion,
    MasterCompletelyOverlapSlaveWithDistortion,
    SlaveCompletelyOverlapMasterWithDistortion,
    MasterOverlapSlaveWithDistortion
}

public enum ECameraRenderStatePropertyName
{
    IsActive,
    IsEnabledActive,
    IsClearDepth,
    IsEnabledClearDepth
}

public enum ECameraPriorityType
{
    Master,
    Slave
}

public struct MapPresetMaskToCameraState
{
    public ECameraPriorityType CameraPriorityType { get; set; }
    public ECameraRenderStatePropertyName FunctionalPropertyName { get; set; }
    public ECameraRenderStatePropertyName EnabledPropertyName { get; set; }
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

public class CamerasRenderPipelineManager
{
    public CameraRenderState MasterCameraState => _cameraRenderStates[ECameraPriorityType.Master];
    public CameraRenderState SlaveCameraState => _cameraRenderStates[ECameraPriorityType.Slave];
    public ECamerasRenderPreset CamerasRenderPreset;

    private readonly Dictionary<ECameraPriorityType, CameraRenderState> _cameraRenderStates;
    
    // todo: why?
    private bool this[ECameraPriorityType priorityType, ECameraRenderStatePropertyName propertyName]
    {
        get => _cameraRenderStates[priorityType][propertyName];
        set => _cameraRenderStates[priorityType][propertyName] = value;
    }
    
    private readonly Dictionary<string, ECamerasRenderPreset> presetMasks = new Dictionary<string, ECamerasRenderPreset>
    {
        { "10xx", ECamerasRenderPreset.MasterCompletelyOverlapSlaveWithoutDistortion },
        { "0xx0", ECamerasRenderPreset.SlaveCompletelyOverlapMasterWithoutDistortion },
        { "1110", ECamerasRenderPreset.MasterOverlapSlaveWithoutDistortion },
        { "x10x", ECamerasRenderPreset.MasterCompletelyOverlapSlaveWithDistortion },
        { "0xx1", ECamerasRenderPreset.SlaveCompletelyOverlapMasterWithDistortion },
        { "1111", ECamerasRenderPreset.MasterOverlapSlaveWithDistortion }
    };

    private readonly MapPresetMaskToCameraState[] presetMaskMaps =
    {
        new MapPresetMaskToCameraState
        {
            CameraPriorityType = ECameraPriorityType.Master,
            FunctionalPropertyName = ECameraRenderStatePropertyName.IsActive,
            EnabledPropertyName = ECameraRenderStatePropertyName.IsEnabledActive
        },
        new MapPresetMaskToCameraState
        {
            CameraPriorityType = ECameraPriorityType.Master,
            FunctionalPropertyName = ECameraRenderStatePropertyName.IsClearDepth,
            EnabledPropertyName = ECameraRenderStatePropertyName.IsEnabledClearDepth
        },
        new MapPresetMaskToCameraState
        {
            CameraPriorityType = ECameraPriorityType.Slave,
            FunctionalPropertyName = ECameraRenderStatePropertyName.IsActive,
            EnabledPropertyName = ECameraRenderStatePropertyName.IsEnabledActive
        },
        new MapPresetMaskToCameraState
        {
            CameraPriorityType = ECameraPriorityType.Slave,
            FunctionalPropertyName = ECameraRenderStatePropertyName.IsClearDepth,
            EnabledPropertyName = ECameraRenderStatePropertyName.IsEnabledClearDepth
        }
    };

    public CamerasRenderPipelineManager()
    {
        _cameraRenderStates = new Dictionary<ECameraPriorityType, CameraRenderState>
        {
            { ECameraPriorityType.Master, new CameraRenderState { IsActive = true, IsClearDepth = false } },
            { ECameraPriorityType.Slave, new CameraRenderState { IsActive = true, IsClearDepth = true } }
        };
        // todo: why?
        UpdateCamerasRenderPreset();
    }

    public void SetCameraRenderStateProperty(
        ECameraPriorityType cameraPriorityType,
        ECameraRenderStatePropertyName propertyName,
        bool value)
    {
        this[cameraPriorityType, propertyName] = value;
        UpdateCamerasRenderPreset();
    }

    /// <summary>
    /// SetCamerasRenderPreset устанавливает пресет и подгоняет текущее функ. состояние под него
    /// а затем обновляет состояние отображения для текущего пресета
    /// </summary>
    public void SetCamerasRenderPreset(ECamerasRenderPreset preset)
    {
        CamerasRenderPreset = preset;
        var matchedPresetMask = presetMasks
            .First(mask => mask.Value == preset).Key;

        for (var i = 0; i < matchedPresetMask.Length; i++)
        {
            var targetPresetMaskMap = presetMaskMaps[i];
            this[targetPresetMaskMap.CameraPriorityType, targetPresetMaskMap.EnabledPropertyName] = matchedPresetMask[i] != 'x';
                
            if (matchedPresetMask[i] == 'x') continue;

            var convertedPresetMaskElement = Convert.ToBoolean(Convert.ToInt32(Convert.ToString(matchedPresetMask[i])));
            this[targetPresetMaskMap.CameraPriorityType, targetPresetMaskMap.FunctionalPropertyName] = convertedPresetMaskElement;
        }

        // todo: why?
        if (SlaveCameraState.IsActive == false)
        {
            MasterCameraState.IsEnabledActive = false;
        }
        
        if (MasterCameraState.IsActive == false)
        {
            SlaveCameraState.IsEnabledActive = false;
        }
        
        if (matchedPresetMask[0] == '0' && matchedPresetMask[2] == 'x')
        {
            SlaveCameraState.IsActive = true;
            // this[ECameraPriorityType.Slave, ECameraRenderStatePropertyName.IsActive] = true;
            // this[ECameraPriorityType.Slave, ECameraRenderStatePropertyName.IsEnabledActive] = false;
        }
            
        if (matchedPresetMask[2] == '0' && matchedPresetMask[0] == 'x')
        {
            MasterCameraState.IsActive = true;
            // this[ECameraPriorityType.Master, ECameraRenderStatePropertyName.IsActive] = true;
            // this[ECameraPriorityType.Master, ECameraRenderStatePropertyName.IsEnabledActive] = false;
        }
    }
    
    /// <summary>
    /// UpdateCamerasRenderPreset подгоняет пресет под измененное функ. состояние
    /// и затем обновляет состояние отображение для полученного пресета
    /// </summary>
    private void UpdateCamerasRenderPreset()
    {
        foreach (var currentPresetMask in presetMasks.Keys)
        {
            var isMatchPresetMask = true;
            for (var i = 0; i < currentPresetMask.Length; i++)
            {
                if (currentPresetMask[i] == 'x') continue;

                var targetPresetMaskMap = presetMaskMaps[i];
                var matchedProperty = this[targetPresetMaskMap.CameraPriorityType, targetPresetMaskMap.FunctionalPropertyName];

                var convertedPresetMaskElement = Convert.ToBoolean(Convert.ToInt32(Convert.ToString(currentPresetMask[i])));
                isMatchPresetMask &= convertedPresetMaskElement == matchedProperty;
            }

            if (isMatchPresetMask)
            {
                SetCamerasRenderPreset(presetMasks[currentPresetMask]);
                return;
            }
        }
    }
}

// todo: refactoring
public class CamerasRenderController : MonoBehaviour
{
    [SerializeField] private Camera masterCamera;
    [SerializeField] private Camera slaveCamera;

    [SerializeField] private Text masterIsActiveText,
        masterIsClearDepthText,
        slaveIsActiveText,
        slaveIsClearDepthText;
    
    [SerializeField] private Dropdown cameraRenderPresetDropdown;

    [SerializeField] private Color disabledTextColor;
    private Color defaultTextColor;

    private CamerasRenderPipelineManager _camerasRenderPipelineManager;
    
    private void Start()
    {
        _camerasRenderPipelineManager = new CamerasRenderPipelineManager();
        defaultTextColor = masterIsActiveText.color;
        cameraRenderPresetDropdown.options = Enum.GetValues(typeof(ECamerasRenderPreset))
            .Cast<ECamerasRenderPreset>()
            .Select(f => new Dropdown.OptionData(f.ToString().SplitByUppercaseLetters()))
            .ToList();
        
        UpdateCameras();
        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var masterCameraState = _camerasRenderPipelineManager.MasterCameraState;

            if (masterCameraState.IsEnabledActive == false) return;
            
            _camerasRenderPipelineManager.SetCameraRenderStateProperty(ECameraPriorityType.Master, ECameraRenderStatePropertyName.IsActive, !masterCameraState.IsActive);
            UpdateUI();
            UpdateCameras();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var masterCameraState = _camerasRenderPipelineManager.MasterCameraState;
            
            if (masterCameraState.IsEnabledClearDepth == false) return;
            
            _camerasRenderPipelineManager.SetCameraRenderStateProperty(ECameraPriorityType.Master, ECameraRenderStatePropertyName.IsClearDepth, !masterCameraState.IsClearDepth);
            UpdateUI();
            UpdateCameras();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            var slaveCameraState = _camerasRenderPipelineManager.SlaveCameraState;
            
            if (slaveCameraState.IsEnabledActive == false) return;
            
            _camerasRenderPipelineManager.SetCameraRenderStateProperty(ECameraPriorityType.Slave, ECameraRenderStatePropertyName.IsActive, !slaveCameraState.IsActive);
            UpdateUI();
            UpdateCameras();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            var slaveCameraState = _camerasRenderPipelineManager.SlaveCameraState;
            
            if (slaveCameraState.IsEnabledClearDepth == false) return;
            
            _camerasRenderPipelineManager.SetCameraRenderStateProperty(ECameraPriorityType.Slave, ECameraRenderStatePropertyName.IsClearDepth, !slaveCameraState.IsClearDepth);
            UpdateUI();
            UpdateCameras();
        }
    }

    public void SetCameraRenderPreset(int cameraRenderPreset)
    {
        _camerasRenderPipelineManager.SetCamerasRenderPreset((ECamerasRenderPreset) cameraRenderPreset);
        UpdateUI();
        UpdateCameras();
    }

    private void UpdateUI()
    {
        var masterCameraState = _camerasRenderPipelineManager.MasterCameraState;
        var slaveCameraState = _camerasRenderPipelineManager.SlaveCameraState;
        var getFunctionString = new Func<bool, string>(flag => flag ? "Enabled" : "Disabled");

        cameraRenderPresetDropdown.value = (int) _camerasRenderPipelineManager.CamerasRenderPreset;

        masterIsActiveText.text = $"Key 1. {getFunctionString(masterCameraState.IsActive)} IsActive";
        masterIsActiveText.color = masterCameraState.IsEnabledActive ? defaultTextColor : disabledTextColor;
        
        masterIsClearDepthText.text = $"Key 2. {getFunctionString(masterCameraState.IsClearDepth)} IsClearDepth";
        masterIsClearDepthText.color = masterCameraState.IsEnabledClearDepth ? defaultTextColor : disabledTextColor;
        
        slaveIsActiveText.text = $"Key 3. {getFunctionString(slaveCameraState.IsActive)} IsActive";
        slaveIsActiveText.color = slaveCameraState.IsEnabledActive ? defaultTextColor : disabledTextColor;
        
        slaveIsClearDepthText.text = $"Key 4. {getFunctionString(slaveCameraState.IsClearDepth)} IsClearDepth";
        slaveIsClearDepthText.color = slaveCameraState.IsEnabledClearDepth ? defaultTextColor : disabledTextColor;
    }

    private void UpdateCameras()
    {
        UpdateCamera(masterCamera, _camerasRenderPipelineManager.MasterCameraState, true);
        UpdateCamera(slaveCamera, _camerasRenderPipelineManager.SlaveCameraState);
    }

    private static void UpdateCamera(Camera targetCamera, CameraRenderState cameraRenderState, bool toggleActiveForParent = false)
    {
        var targetGameObject = toggleActiveForParent ? targetCamera.transform.parent.gameObject : targetCamera.gameObject;
        targetGameObject.SetActive(cameraRenderState.IsActive);
        targetCamera.clearFlags = cameraRenderState.IsClearDepth ? CameraClearFlags.Depth : CameraClearFlags.SolidColor;
    }
}