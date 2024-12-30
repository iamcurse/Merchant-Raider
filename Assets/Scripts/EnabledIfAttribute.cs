using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class EnabledIfAttribute : PropertyAttribute
{
    public string conditionPropertyName;

    public EnabledIfAttribute(string conditionPropertyName)
    {
        this.conditionPropertyName = conditionPropertyName;
    }
}