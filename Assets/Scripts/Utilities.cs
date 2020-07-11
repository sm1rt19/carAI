using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Utilities
{
    public static float Step(float currentValue, float targetValue, float delta)
    {
        if (targetValue > currentValue)
        {
            delta = Mathf.Min(delta, targetValue - currentValue);
            currentValue += delta;
        }
        if (targetValue < currentValue)
        {
            delta = Mathf.Min(delta, currentValue - targetValue);
            currentValue -= delta;
        }
        return currentValue;
    }
}
