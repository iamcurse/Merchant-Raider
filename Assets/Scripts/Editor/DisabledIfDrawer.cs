using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DisabledIfAttribute))]
public class DisabledIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get the condition property
        var disabledIfAttribute = (DisabledIfAttribute)attribute;
        SerializedProperty conditionProperty = property.serializedObject.FindProperty(disabledIfAttribute.conditionPropertyName);

        // If the condition is true, disable the field
        GUI.enabled = conditionProperty == null || !conditionProperty.boolValue;

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