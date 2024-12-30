using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnabledIfAttribute))]
public class EnabledIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get the condition property
        var enabledIfAttribute = (EnabledIfAttribute)attribute;
        SerializedProperty conditionProperty = property.serializedObject.FindProperty(enabledIfAttribute.conditionPropertyName);

        // If the condition is false, disable the field (invert logic)
        GUI.enabled = conditionProperty != null && conditionProperty.boolValue;

        // Draw the property (enabled or disabled based on the condition)
        EditorGUI.PropertyField(position, property, label);

        // Restore the GUI enabled state
        GUI.enabled = true;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}