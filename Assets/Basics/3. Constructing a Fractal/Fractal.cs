using System.Collections;
using UnityEngine;

public class Fractal : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;
    [SerializeField] [Range(0f, 1f)] private float childScale = 0.5f;
    [SerializeField] [Range(0.1f, 1f)] private float creationDelay = 0.25f;
    [SerializeField] [Range(1, 5)] private int maxDepth = 4;
    
    private int _currentDepth;
    private Vector3 _creationDirection;
    
    private void Start()
    {
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;

        if (_currentDepth < maxDepth)
        {
            StartCoroutine(CreateChildFractals());
            StartCoroutine(DestroyChildFractals());
        }
    }
    
    private void Initialize(Fractal parent, Vector3 birthDirection)
    {
        mesh = parent.mesh;
        material = parent.material;
        childScale = parent.childScale;
        
        _currentDepth = parent._currentDepth + 1;
        _creationDirection = birthDirection;

        transform.parent = parent.transform;
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = birthDirection * (0.5f + 0.5f * childScale);
    }

    private IEnumerator CreateChildFractals()
    {
        var directions = new[]
        {
            Vector3.up,
            Vector3.down,
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right,
        };

        while (true)
        {
            foreach (var direction in directions)
            {
                if (-direction == _creationDirection) continue;
            
                yield return new WaitForSeconds(creationDelay);
                new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, direction);
            }
        }
    }

    private IEnumerator DestroyChildFractals()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 3f));

            var countDestroyedChildren = Random.Range(1, transform.childCount);
            for (var i = 0; i < countDestroyedChildren; i++)
                Destroy(transform.GetChild(Random.Range(0, transform.childCount)).gameObject);
        }
    }
}
