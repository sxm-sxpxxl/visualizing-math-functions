using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private int frameRange = 60;
    
    public int AverageFPS { get; private set; }
    public int HighestFPS { get; private set; }
    public int LowestFPS { get; private set; }

    private int[] _fpsBuffer;
    private int _fpsBufferIndex;

    private void Update()
    {
        if (_fpsBuffer == null || _fpsBuffer.Length != frameRange)
        {
            InitializeBuffer();
        }

        UpdateBuffer();
        CalculateFPS();
    }

    private void InitializeBuffer()
    {
        if (frameRange <= 0)
        {
            frameRange = 1;
        }

        _fpsBuffer = new int[frameRange];
        _fpsBufferIndex = 0;
    }

    private void UpdateBuffer()
    {
        _fpsBuffer[_fpsBufferIndex++] = (int) (1f / Time.unscaledDeltaTime);
        if (_fpsBufferIndex >= frameRange)
        {
            _fpsBufferIndex = 0;
        }
    }

    private void CalculateFPS()
    {
        var sum = 0;
        var highest = 0;
        var lowest = int.MaxValue;
        
        for (var i = 0; i < frameRange; i++)
        {
            var fps = _fpsBuffer[i];
            sum += fps;

            if (fps > highest)
            {
                highest = fps;
            }

            if (fps < lowest)
            {
                lowest = fps;
            }
        }

        AverageFPS = sum / frameRange;
        HighestFPS = highest;
        LowestFPS = lowest;
    }
}
