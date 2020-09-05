using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
    private Color _defaultTextColor;

    private ICamerasRenderPipelineManager _camerasRenderPipelineManager;

    private static readonly Dictionary<KeyCode, CameraRenderStatePropertyPath> MapOfKeyCodeToCameraStatePropertyPath = 
        new Dictionary<KeyCode, CameraRenderStatePropertyPath>
    {
        {
            KeyCode.Alpha1, new CameraRenderStatePropertyPath
            {
                CameraPriorityType = ECameraPriorityType.Master,
                FunctionalPropertyName = ECameraRenderStatePropertyName.IsActive,
                EnabledPropertyName = ECameraRenderStatePropertyName.IsEnabledActive
            }
        },
        {
            KeyCode.Alpha2, new CameraRenderStatePropertyPath
            {
                CameraPriorityType = ECameraPriorityType.Master,
                FunctionalPropertyName = ECameraRenderStatePropertyName.IsClearDepth,
                EnabledPropertyName = ECameraRenderStatePropertyName.IsEnabledClearDepth
            }
        },
        {
            KeyCode.Alpha3, new CameraRenderStatePropertyPath
            {
                CameraPriorityType = ECameraPriorityType.Slave,
                FunctionalPropertyName = ECameraRenderStatePropertyName.IsActive,
                EnabledPropertyName = ECameraRenderStatePropertyName.IsEnabledActive
            }
        },
        {
            KeyCode.Alpha4, new CameraRenderStatePropertyPath
            {
                CameraPriorityType = ECameraPriorityType.Slave,
                FunctionalPropertyName = ECameraRenderStatePropertyName.IsClearDepth,
                EnabledPropertyName = ECameraRenderStatePropertyName.IsEnabledClearDepth
            }
        },
    };

    private Dictionary<Text, KeyCode> _mapOfTextToKeyCode;

    private void Start()
    {
        _camerasRenderPipelineManager = new CamerasRenderPipelineManager();
        _defaultTextColor = masterIsActiveText.color;
        cameraRenderPresetDropdown.options = DropdownUtility.GetOptionsForEnum<ECamerasRenderPreset>();
        
        _mapOfTextToKeyCode = new Dictionary<Text, KeyCode>
        {
            { masterIsActiveText, KeyCode.Alpha1 },
            { masterIsClearDepthText, KeyCode.Alpha2 },
            { slaveIsActiveText, KeyCode.Alpha3 },
            { slaveIsClearDepthText, KeyCode.Alpha4 }
        };

        UpdateCameras();
        UpdateUI();
    }

    private void Update()
    {
        foreach (var key in MapOfKeyCodeToCameraStatePropertyPath.Keys.Where(Input.GetKeyDown))
            ChangeProperty(MapOfKeyCodeToCameraStatePropertyPath[key]);
    }

    private void ChangeProperty(CameraRenderStatePropertyPath path)
    {
        var isPropertyEnabled = _camerasRenderPipelineManager[path.CameraPriorityType, path.EnabledPropertyName];
        if (isPropertyEnabled == false) return;
        
        var currentValue = _camerasRenderPipelineManager[path.CameraPriorityType, path.FunctionalPropertyName];
        var nextValue = !currentValue;
        
        _camerasRenderPipelineManager.SetCameraRenderStateFunctionalProperty(path, nextValue);
        UpdateUI();
        UpdateCameras();
    }

    public void SetCameraRenderPreset(int cameraRenderPreset)
    {
        _camerasRenderPipelineManager.SetCamerasRenderPreset((ECamerasRenderPreset) cameraRenderPreset);
        UpdateUI();
        UpdateCameras();
    }

    private void UpdateUI()
    {
        cameraRenderPresetDropdown.value = (int) _camerasRenderPipelineManager.CamerasRenderPreset;

        foreach (var pair in _mapOfTextToKeyCode)
            ChangeText(pair.Key, MapOfKeyCodeToCameraStatePropertyPath[pair.Value], pair.Value);
    }

    private void ChangeText(Text target, CameraRenderStatePropertyPath path, KeyCode keyCode)
    {
        var key = string.Empty;
        switch (keyCode)
        {
            case KeyCode.Alpha1: key = "1"; break;
            case KeyCode.Alpha2: key = "2"; break;
            case KeyCode.Alpha3: key = "3"; break;
            case KeyCode.Alpha4: key = "4"; break;
        }
        
        var getFunctionString = new Func<bool, string>(flag => flag ? "Enabled" : "Disabled");

        var propValue = _camerasRenderPipelineManager[path.CameraPriorityType, path.FunctionalPropertyName];
        var isEnabledProp = _camerasRenderPipelineManager[path.CameraPriorityType, path.EnabledPropertyName];

        target.text = $"Key {key}. {getFunctionString(propValue)} {propValue.GetType().Name}";
        target.color = isEnabledProp ? _defaultTextColor : disabledTextColor;
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