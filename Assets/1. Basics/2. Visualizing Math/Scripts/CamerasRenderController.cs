using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public struct CameraRenderState
{
    public bool IsActive { get; set; }
    public bool IsEnabledActive { get; set; }
    public bool IsClearDepth { get; set; }
    public bool IsEnabledClearDepth { get; set; }
}

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

// todo: refactoring
public class CamerasRenderPipelineManager
{
    public CameraRenderState MasterCameraState;
    public CameraRenderState SlaveCameraState;
    public ECamerasRenderPreset CamerasRenderPreset;

    private readonly Dictionary<string, ECamerasRenderPreset> presetMasks = new Dictionary<string, ECamerasRenderPreset>
    {
        { "10xx", ECamerasRenderPreset.MasterCompletelyOverlapSlaveWithoutDistortion },
        { "0xx0", ECamerasRenderPreset.SlaveCompletelyOverlapMasterWithoutDistortion },
        { "1110", ECamerasRenderPreset.MasterOverlapSlaveWithoutDistortion },
        { "x10x", ECamerasRenderPreset.MasterCompletelyOverlapSlaveWithDistortion },
        { "0xx1", ECamerasRenderPreset.SlaveCompletelyOverlapMasterWithDistortion },
        { "1111", ECamerasRenderPreset.MasterOverlapSlaveWithDistortion }
    };

    public CamerasRenderPipelineManager()
    {
        MasterCameraState = new CameraRenderState { IsActive = true, IsClearDepth = false };
        SlaveCameraState = new CameraRenderState { IsActive = true, IsClearDepth = true };
        UpdateCamerasRenderPreset();
    }

    public void TrySetActive(ECameraPriorityType cameraPriorityType, bool active)
    {
        switch (cameraPriorityType)
        {
            case ECameraPriorityType.Master:
                MasterCameraState.IsActive = active;
                break;
            case ECameraPriorityType.Slave:
                SlaveCameraState.IsActive = active;
                break;
        }

        UpdateCamerasRenderPreset();
    }

    public void TrySetClearDepthBuffer(ECameraPriorityType cameraPriorityType, bool clearDepth)
    {
        switch (cameraPriorityType)
        {
            case ECameraPriorityType.Master:
                MasterCameraState.IsClearDepth = clearDepth;
                break;
            case ECameraPriorityType.Slave:
                SlaveCameraState.IsClearDepth = clearDepth;
                break;
        }

        UpdateCamerasRenderPreset();
    }

    // todo: refactoring
    public void SetCamerasRenderPreset(ECamerasRenderPreset camerasRenderPreset)
    {
        var newCameraRenderStateMask = new[]
        {
            MasterCameraState.IsActive,
            MasterCameraState.IsClearDepth,
            SlaveCameraState.IsActive,
            SlaveCameraState.IsClearDepth
        };
        var currentCameraRenderEnabledStateMask = new []
        {
            MasterCameraState.IsEnabledActive,
            MasterCameraState.IsEnabledClearDepth,
            SlaveCameraState.IsEnabledActive,
            SlaveCameraState.IsEnabledClearDepth
        };
        
        var matchedPresetMask = presetMasks
            .First(mask => mask.Value == camerasRenderPreset).Key;

        for (var i = 0; i < matchedPresetMask.Length; i++)
        {
            if (matchedPresetMask[i] == 'x')
            {
                currentCameraRenderEnabledStateMask[i] = false;
                continue;
            }

            var convertedPresetMaskElement = Convert.ToBoolean(Convert.ToInt32(Convert.ToString(matchedPresetMask[i])));
            newCameraRenderStateMask[i] = convertedPresetMaskElement;
            currentCameraRenderEnabledStateMask[i] = true;
        }
        
        if (matchedPresetMask[0] == '0' && matchedPresetMask[2] == 'x')
        {
            newCameraRenderStateMask[2] = true;
        }
            
        if (matchedPresetMask[2] == '0' && matchedPresetMask[0] == 'x')
        {
            newCameraRenderStateMask[0] = true;
        }
        
        MasterCameraState.IsActive = newCameraRenderStateMask[0];
        MasterCameraState.IsClearDepth = newCameraRenderStateMask[1];
        SlaveCameraState.IsActive = newCameraRenderStateMask[2];
        SlaveCameraState.IsClearDepth = newCameraRenderStateMask[3];

        CamerasRenderPreset = camerasRenderPreset;
        
        MasterCameraState.IsEnabledActive = currentCameraRenderEnabledStateMask[0];
        MasterCameraState.IsEnabledClearDepth = currentCameraRenderEnabledStateMask[1];
        SlaveCameraState.IsEnabledActive = currentCameraRenderEnabledStateMask[2];
        SlaveCameraState.IsEnabledClearDepth = currentCameraRenderEnabledStateMask[3];
    }
    
    // todo: refactoring
    private void UpdateCamerasRenderPreset()
    {
        var currentCameraRenderStateMask = new []
        {
            MasterCameraState.IsActive,
            MasterCameraState.IsClearDepth,
            SlaveCameraState.IsActive,
            SlaveCameraState.IsClearDepth
        };
        var currentCameraRenderEnabledStateMask = new []
        {
            MasterCameraState.IsEnabledActive,
            MasterCameraState.IsEnabledClearDepth,
            SlaveCameraState.IsEnabledActive,
            SlaveCameraState.IsEnabledClearDepth
        };

        foreach (var currentPresetMask in presetMasks.Keys)
        {
            var isMatchPresetMask = true;
            for (var i = 0; i < currentPresetMask.Length; i++)
            {
                if (currentPresetMask[i] == 'x')
                {
                    currentCameraRenderEnabledStateMask[i] = false;
                    continue;
                }

                var convertedPresetMaskElement = Convert.ToBoolean(Convert.ToInt32(Convert.ToString(currentPresetMask[i])));
                isMatchPresetMask &= convertedPresetMaskElement == currentCameraRenderStateMask[i];
                currentCameraRenderEnabledStateMask[i] = true;
            }

            if (isMatchPresetMask)
            {
                CamerasRenderPreset = presetMasks[currentPresetMask];
                
                MasterCameraState.IsEnabledActive = currentCameraRenderEnabledStateMask[0];
                MasterCameraState.IsEnabledClearDepth = currentCameraRenderEnabledStateMask[1];
                SlaveCameraState.IsEnabledActive = currentCameraRenderEnabledStateMask[2];
                SlaveCameraState.IsEnabledClearDepth = currentCameraRenderEnabledStateMask[3];
                
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

    private CamerasRenderPipelineManager _camerasRenderStateManager;
    
    private void Start()
    {
        _camerasRenderStateManager = new CamerasRenderPipelineManager();
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
            var masterCameraState = _camerasRenderStateManager.MasterCameraState;

            if (masterCameraState.IsEnabledActive == false) return;
            
            _camerasRenderStateManager.TrySetActive(ECameraPriorityType.Master, !masterCameraState.IsActive);
            UpdateUI();
            UpdateCameras();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var masterCameraState = _camerasRenderStateManager.MasterCameraState;
            
            if (masterCameraState.IsEnabledClearDepth == false) return;
            
            _camerasRenderStateManager.TrySetClearDepthBuffer(ECameraPriorityType.Master, !masterCameraState.IsClearDepth);
            UpdateUI();
            UpdateCameras();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            var slaveCameraState = _camerasRenderStateManager.SlaveCameraState;
            
            if (slaveCameraState.IsEnabledActive == false) return;
            
            _camerasRenderStateManager.TrySetActive(ECameraPriorityType.Slave, !slaveCameraState.IsActive);
            UpdateUI();
            UpdateCameras();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            var slaveCameraState = _camerasRenderStateManager.SlaveCameraState;
            
            if (slaveCameraState.IsEnabledClearDepth == false) return;
            
            _camerasRenderStateManager.TrySetClearDepthBuffer(ECameraPriorityType.Slave, !slaveCameraState.IsClearDepth);
            UpdateUI();
            UpdateCameras();
        }
    }

    public void SetCameraRenderPreset(int cameraRenderPreset)
    {
        _camerasRenderStateManager.SetCamerasRenderPreset((ECamerasRenderPreset) cameraRenderPreset);
        UpdateUI();
        UpdateCameras();
    }

    private void UpdateUI()
    {
        var masterCameraState = _camerasRenderStateManager.MasterCameraState;
        var slaveCameraState = _camerasRenderStateManager.SlaveCameraState;
        var getFunctionString = new Func<bool, string>(flag => flag ? "Enabled" : "Disabled");

        cameraRenderPresetDropdown.value = (int) _camerasRenderStateManager.CamerasRenderPreset;

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
        UpdateCamera(masterCamera, _camerasRenderStateManager.MasterCameraState, true);
        UpdateCamera(slaveCamera, _camerasRenderStateManager.SlaveCameraState);
    }

    private static void UpdateCamera(Camera targetCamera, CameraRenderState cameraRenderState, bool toggleActiveForParent = false)
    {
        var targetGameObject = toggleActiveForParent ? targetCamera.transform.parent.gameObject : targetCamera.gameObject;
        targetGameObject.SetActive(cameraRenderState.IsActive);
        targetCamera.clearFlags = cameraRenderState.IsClearDepth ? CameraClearFlags.Depth : CameraClearFlags.SolidColor;
    }
}