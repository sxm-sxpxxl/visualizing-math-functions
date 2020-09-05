using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graph))]
public class GraphFunctionSelector : MonoBehaviour
{
    [SerializeField] private Dropdown functionsDropdown;

    private Graph _graph;

    private void Start()
    {
        if (functionsDropdown == null)
            throw new MissingReferenceException($"{nameof(functionsDropdown)} isn't set on GraphFunctionSelector component.");

        _graph = GetComponent<Graph>();

        functionsDropdown.options = DropdownUtility.GetOptionsForEnum<EGraphFunctionName>();
        functionsDropdown.value = (int) _graph.functionName;
        functionsDropdown.onValueChanged.AddListener(SetGraphFunction);
    }

    private void SetGraphFunction(int functionName)
    {
        _graph.functionName = (EGraphFunctionName) functionName;
    }
}
