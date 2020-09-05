using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField] private Transform pointPrefab;
    [SerializeField] [Range(10, 300)] private int resolution = 50;
    [SerializeField] private EGraphFunctionName function = EGraphFunctionName.Sine;
    
    private Transform[] _points;
    private GraphFunction.Function _graphFunction = GraphFunction.Set(EGraphFunctionName.Sine);
    
    private void Awake()
    {
        var step = 2f / resolution;
        var scale = Vector3.one / 10f;
        
        _points = new Transform[resolution * resolution];
        for (var i = 0; i < _points.Length; i++)
        {
            var point = Instantiate(pointPrefab, transform, false);
            point.localScale = scale;
            _points[i] = point;
        }
    }

    private void Update()
    {
        var t = 2f * Time.time;
        _graphFunction = GraphFunction.Set(function);

        var step = 2f / resolution;
        for (int i = 0, z = 0; z < resolution; z++)
        {
            var v = (z + 0.5f) * step - 1f;
            for (int x = 0; x < resolution; x++, i++)
            {
                var u = (x + 0.5f) * step - 1f;
                _points[i].localPosition = _graphFunction(u, v, t);
            }
        }
    }
}
