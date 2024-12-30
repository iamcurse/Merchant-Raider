using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class DisabledIfAttribute : PropertyAttribute
{
    public string conditionPropertyName;

    public DisabledIfAttribute(string conditionPropertyName)
    {
        this.conditionPropertyName = conditionPropertyName;
    }
}