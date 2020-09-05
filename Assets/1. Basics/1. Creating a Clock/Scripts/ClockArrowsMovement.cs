using System;
using UnityEngine;

public class ClockArrowsMovement : MonoBehaviour
{
    [SerializeField] private Transform clockArrowHour;
    [SerializeField] private Transform clockArrowMinute;
    [SerializeField] private Transform clockArrowSecond;

    private const int DegreePerSecond = 6;
    private const int DegreePerMinute = 6;
    private const int DegreePerHour = 30;

    private const int SecondsInMinute = 60;
    private const int SecondsInHour = 3600;
    
    private void Start()
    {
        var timeNow = DateTime.Now;

        clockArrowSecond.localEulerAngles = Vector3.up * (timeNow.Second * DegreePerSecond);
        clockArrowMinute.localEulerAngles = Vector3.up * (timeNow.Minute * DegreePerMinute);
        clockArrowHour.localEulerAngles = Vector3.up * (timeNow.Hour * DegreePerHour);
    }
    
    private void FixedUpdate()
    {
        Debug.Log(DateTime.Now.TimeOfDay);
        
        clockArrowSecond.localEulerAngles += Vector3.up * (DegreePerSecond * Time.fixedDeltaTime);
        clockArrowMinute.localEulerAngles += Vector3.up * (DegreePerMinute * Time.fixedDeltaTime / 60);
        clockArrowHour.localEulerAngles += Vector3.up * (DegreePerHour * Time.fixedDeltaTime / SecondsInHour);
    }
}
