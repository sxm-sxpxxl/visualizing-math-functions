using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CamerasRenderController : MonoBehaviour
{
    [SerializeField] private Camera masterCamera, slaveCamera;

    [SerializeField]
    private Toggle masterIsActiveToggle,
        masterIsClearDepthToggle,
        slaveIsActiveToggle,
        slaveIsClearDepthToggle;

    [SerializeField] private Dropdown cameraRenderPresetDropdown;

    [SerializeField] private Color disabledToggleTextColor;
    private Color _defaultToggleTextColor;

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

    private Dictionary<Toggle, KeyCode> _mapOfToggleToKeyCode;
    private Dictionary<Toggle, Text> _mapOfToggleToText;

    private void Start()
    {
        _camerasRenderPipelineManager = new CamerasRenderPipelineManager();
        _defaultToggleTextColor = masterIsActiveToggle.GetComponentInChildren<Text>().color;
        cameraRenderPresetDropdown.options = DropdownUtility.GetOptionsForEnum<ECamerasRenderPreset>();

        _mapOfToggleToKeyCode = new Dictionary<Toggle, KeyCode>
        {
            { masterIsActiveToggle, KeyCode.Alpha1 },
            { masterIsClearDepthToggle, KeyCode.Alpha2 },
            { slaveIsActiveToggle, KeyCode.Alpha3 },
            { slaveIsClearDepthToggle, KeyCode.Alpha4 }
        };

        _mapOfToggleToText = new Dictionary<Toggle, Text>();
        foreach (var toggle in _mapOfToggleToKeyCode.Keys)
        {
            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            entry.callback.AddListener(data =>
            {
                if (toggle.interactable == false) return;

                var keyCode = _mapOfToggleToKeyCode[toggle];
                ToggleCameraRenderStateProperty(MapOfKeyCodeToCameraStatePropertyPath[keyCode]);
            });

            toggle.gameObject.AddComponent<EventTrigger>().triggers = new List<EventTrigger.Entry> { entry };
            _mapOfToggleToText[toggle] = toggle.GetComponentInChildren<Text>();
        }

        UpdateCameras();
        UpdateUI();
    }

    private void Update()
    {
        foreach (var key in MapOfKeyCodeToCameraStatePropertyPath.Keys.Where(Input.GetKeyDown))
            ToggleCameraRenderStateProperty(MapOfKeyCodeToCameraStatePropertyPath[key]);
    }

    private void ToggleCameraRenderStateProperty(CameraRenderStatePropertyPath path)
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
        _camerasRenderPipelineManager.SetCamerasRenderPreset((ECamerasRenderPreset)cameraRenderPreset);
        UpdateUI();
        UpdateCameras();
    }

    private void UpdateUI()
    {
        cameraRenderPresetDropdown.value = (int)_camerasRenderPipelineManager.CamerasRenderPreset;

        foreach (var pair in _mapOfToggleToKeyCode)
            UpdateToggle(pair.Key, MapOfKeyCodeToCameraStatePropertyPath[pair.Value]);
    }

    private void UpdateToggle(Toggle target, CameraRenderStatePropertyPath path)
    {
        var propValue = _camerasRenderPipelineManager[path.CameraPriorityType, path.FunctionalPropertyName];
        var isEnabledProp = _camerasRenderPipelineManager[path.CameraPriorityType, path.EnabledPropertyName];

        target.isOn = propValue;
        target.interactable = isEnabledProp;
        _mapOfToggleToText[target].color = isEnabledProp ? _defaultToggleTextColor : disabledToggleTextColor;
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