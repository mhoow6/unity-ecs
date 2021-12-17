using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceIndicatorUnity
{
    float time;

    public void Begin()
    {
        time = Time.realtimeSinceStartup;
    }

    public void End()
    {
        float endTimeMillsecond = (Time.realtimeSinceStartup - time) * 1000f;
        Debug.Log($"{endTimeMillsecond} ms");
    }
}
