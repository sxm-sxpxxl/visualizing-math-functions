using System.Collections;
using UnityEngine;

public class CatLikeCoding_Fractal : MonoBehaviour
{
    [SerializeField] private Mesh[] meshes;
    [SerializeField] private Material material;

    [SerializeField] [Range(0.1f, 1f)] private float childScale = 0.5f;
    [SerializeField] [Range(1, 5)] private int maxDepth = 4;
    
    [SerializeField] [Range(0f, 1f)] private float spawnProbability = 0.7f;
    [SerializeField] [Range(10f, 60f)] private float maxRotationSpeed = 30f;
    [SerializeField] [Range(5f, 20f)] private float maxTwist = 10f;
    
    private int _currentDepth;
    private Material[,] _materialsPerDepth;
    private float _rotationSpeed;

    private struct ChildInfo
    {
        public Vector3 Direction { get; set; }
        public Quaternion Orientation { get; set; }
    }

    private static readonly ChildInfo[] ChildInfos = {
        new ChildInfo { Direction = Vector3.up, Orientation = Quaternion.identity },
        new ChildInfo { Direction = Vector3.right, Orientation = Quaternion.Euler(0f, 0f, -90f) },
        new ChildInfo { Direction = Vector3.left, Orientation = Quaternion.Euler(0f, 0f, 90f) },
        new ChildInfo { Direction = Vector3.forward, Orientation = Quaternion.Euler(90f, 0f, 0f) },
        new ChildInfo { Direction = Vector3.back, Orientation = Quaternion.Euler(-90f, 0f, 0f) },
    };
    
    private void Start()
    {
        _rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
        transform.Rotate(Random.Range(-maxTwist, maxTwist), 0f, 0f);
        
        if (_materialsPerDepth == null)
        {
            _materialsPerDepth = InitializeMaterialsPerDepth();
        }

        gameObject.AddComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length)];
        // Random.Range returns a number between min [inclusive] and max [exclusive]
        gameObject.AddComponent<MeshRenderer>().material = _materialsPerDepth[_currentDepth, Random.Range(0, 2)];
        
        if (_currentDepth < maxDepth)
        {
            StartCoroutine(CreateChildren());
        }
    }

    private void Update()
    {
        transform.Rotate(0f, _rotationSpeed * Time.deltaTime, 0f);
    }

    private void Initialize(CatLikeCoding_Fractal parent, int childIndex)
    {
        meshes = parent.meshes;
        _materialsPerDepth = parent._materialsPerDepth;
        maxDepth = parent.maxDepth;
        _currentDepth = parent._currentDepth + 1;

        spawnProbability = parent.spawnProbability;
        maxRotationSpeed = parent.maxRotationSpeed;
        maxTwist = parent.maxTwist;
        childScale = parent.childScale;
        transform.parent = parent.transform;

        var childInfo = ChildInfos[childIndex];
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = childInfo.Direction * (0.5f + 0.5f * childScale);
        transform.localRotation = childInfo.Orientation;
    }

    private Material[,] InitializeMaterialsPerDepth()
    {
        var materialsPerDepth = new Material[maxDepth + 1, 2];
        for (var i = 0; i <= maxDepth; i++)
        {
            var t = Mathf.Pow(i / (maxDepth - 1f), 2);
            materialsPerDepth[i, 0] = new Material(material) { color = Color.Lerp(Color.white, Color.yellow, t) };
            materialsPerDepth[i, 1] = new Material(material) { color = Color.Lerp(Color.white, Color.cyan, t) };
        }

        materialsPerDepth[maxDepth, 0].color = Color.magenta;
        materialsPerDepth[maxDepth, 1].color = Color.red;
        return materialsPerDepth;
    }

    private IEnumerator CreateChildren()
    {
        yield return new WaitForSeconds(0.5f);
        // First the new game object is created. Then a new Fractal component is created and added to it.
        // Then Awake and OnEnable is invoked. Then the AddComponent method finishes.
        // Then Initialize is invoked before Start method.
        for (var i = 0; i < ChildInfos.Length; i++)
        {
            if (Random.value < spawnProbability)
            {
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
                new GameObject("Fractal Child").AddComponent<CatLikeCoding_Fractal>()
                    .Initialize(this, i);
            }
        }
        
    }
}
