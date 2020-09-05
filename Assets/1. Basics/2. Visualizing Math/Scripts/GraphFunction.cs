using UnityEngine;

public enum EGraphFunctionName
{
    Sine,
    SineScaling,
    MultiSine,
    MultiSineMoreComplex,
    Sine2D,
    MultiSine2D,
    MultiSine2DMoreComplex,
    Ripple,
    Cylinder,
    Sphere,
    Torus
}

public static class GraphFunction
{
    public delegate Vector3 Function(float u, float v, float t);
    private const float Pi = Mathf.PI;

    public static Function Set(EGraphFunctionName functionName)
    {
        switch (functionName)
        {
            case EGraphFunctionName.Sine:
                return SineFunction;
            case EGraphFunctionName.SineScaling:
                return SineScalingFunction;
            case EGraphFunctionName.MultiSine:
                return MultiSineFunction;
            case EGraphFunctionName.MultiSineMoreComplex:
                return MultiSineMoreComplexFunction;
            case EGraphFunctionName.Sine2D:
                return Sine2DFunction;
            case EGraphFunctionName.MultiSine2D:
                return MultiSine2DFunction;
            case EGraphFunctionName.MultiSine2DMoreComplex:
                return MultiSine2DMoreComplexFunction;
            case EGraphFunctionName.Ripple:
                return Ripple;
            case EGraphFunctionName.Cylinder:
                return Cylinder;
            case EGraphFunctionName.Sphere:
                return Sphere;
            case EGraphFunctionName.Torus:
                return Torus;
            default:
                return SineFunction;
        }
    }

    #region Functions

    private static Vector3 SineFunction(float x, float z, float t)
    {
        return new Vector3(x, Mathf.Sin(Pi * (x + t)), z);
    }

    private static Vector3 SineScalingFunction(float x, float z, float t)
    {
        return new Vector3(x, Mathf.Sin(Pi * x * t), z);
    }
    
    private static Vector3 Sine2DFunction(float x, float z, float t)
    {
        return new Vector3(x, Mathf.Sin(Pi * (x + z + t)), z);
    }

    private static Vector3 MultiSineFunction(float x, float z, float t)
    {
        var y = Mathf.Sin(Pi * (x + t));
        y += 0.5f * Mathf.Sin(2f * Pi * (x + 2f * t));
        y *= 0.77f;
        return new Vector3(x, y, z);
    }

    private static Vector3 MultiSineMoreComplexFunction(float x, float z, float t)
    {
        var y = Mathf.Sin(Pi * (x + 2f * t));
        y += 0.5f * Mathf.Sin(4f * Pi * (x + t));
        y *= 0.67f;
        return new Vector3(x, y, z);
    }

    private static Vector3 MultiSine2DFunction(float x, float z, float t)
    {
        var y = Mathf.Sin(Pi * (x + t));
        y += Mathf.Sin(Pi * (z + t));
        y *= 0.5f;
        return new Vector3(x, y, z);
    }

    private static Vector3 MultiSine2DMoreComplexFunction(float x, float z, float t)
    {
        var y = 4f * Mathf.Sin(Pi * (x + z + 0.5f * t));
        y += Mathf.Sin(Pi * (x + t));
        y += 0.5f * Mathf.Sin(2f * Pi * (z + 2f * t));
        y *= 1f / 5.5f;
        return new Vector3(x, y, z);
    }

    private static Vector3 Ripple(float x, float z, float t)
    {
        var d = Mathf.Sqrt(x * x + z * z);
        var y = Mathf.Sin(Pi * (4f * d - t));
        y /= 1f + 10f * d;
        return new Vector3(x, y, z);
    }

    private static Vector3 Cylinder(float u, float v, float t)
    {
        var r = 0.8f + Mathf.Sin(Pi * (6f * u + 2f * v + t)) * 0.2f;
        
        var x = r * Mathf.Sin(Pi * u);
        var y = v;
        var z = r * Mathf.Cos(Pi * u);
        
        return new Vector3(x, y, z);
    }

    private static Vector3 Sphere(float u, float v, float t)
    {
        var r = 0.8f + Mathf.Sin(Pi * (6f * u + t)) * 0.1f;
        r += Mathf.Sin(Pi * (4f * v + t)) * 0.1f;
        var s = r * Mathf.Cos(Pi * 0.5f * v);
        
        var x = s * Mathf.Sin(Pi * u);
        var y = r * Mathf.Sin(Pi * 0.5f * v);
        var z = s * Mathf.Cos(Pi * u);

        return new Vector3(x, y, z);
    }

    private static Vector3 Torus(float u, float v, float t)
    {
        var r1 = 0.65f + Mathf.Sin(Pi * (6f * u + t)) * 0.1f;
        var r2 = 0.2f + Mathf.Sin(Pi * (4f * v + t)) * 0.05f;
        var s = r2 * Mathf.Cos(Pi * v) + r1;

        var x = s * Mathf.Sin(Pi * u);
        var y = r2 * Mathf.Sin(Pi * v);
        var z = s * Mathf.Cos(Pi * u);

        return new Vector3(x, y, z);
    }
    
    #endregion
}
