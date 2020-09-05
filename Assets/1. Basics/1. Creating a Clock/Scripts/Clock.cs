using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    private const float
        DegreesPerHour = 30f,
        DegreesPerMinute = 6f,
        DegreesPerSecond = 6f;
    
    [SerializeField] private Transform hoursTransform, minutesTransform, secondsTransform;

    [SerializeField] private bool continuous;

    private void Update()
    {
        if (continuous)
            UpdateContinuous();
        else
            UpdateDiscrete();
    }
    
    private void UpdateContinuous()
    {
        var timeOfDayNow = DateTime.Now.TimeOfDay;
        Debug.Log($"Total Hours: {timeOfDayNow.TotalHours} | " +
                  $"Total Minutes: {timeOfDayNow.TotalMinutes} | " +
                  $"Total Seconds: {timeOfDayNow.TotalSeconds}");
        
        hoursTransform.localRotation = Quaternion.Euler(0f, (float) timeOfDayNow.TotalHours * DegreesPerHour, 0f);
        minutesTransform.localRotation = Quaternion.Euler(0f, (float) timeOfDayNow.TotalMinutes * DegreesPerMinute, 0f);
        secondsTransform.localRotation = Quaternion.Euler(0f, (float) timeOfDayNow.TotalSeconds * DegreesPerSecond, 0f);
    }

    private void UpdateDiscrete()
    {
        var timeNow = DateTime.Now;
        Debug.Log($"Hours: {timeNow.Hour} | Minutes: {timeNow.Minute} | Seconds: {timeNow.Second}");
        
        hoursTransform.localRotation = Quaternion.Euler(0f, timeNow.Hour * DegreesPerHour, 0f);
        minutesTransform.localRotation = Quaternion.Euler(0f, timeNow.Minute * DegreesPerMinute, 0f);
        secondsTransform.localRotation = Quaternion.Euler(0f, timeNow.Second * DegreesPerSecond, 0f);
    }
}
