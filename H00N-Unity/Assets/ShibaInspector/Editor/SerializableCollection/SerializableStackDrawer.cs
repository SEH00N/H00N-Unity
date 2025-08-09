#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace ShibaInspector.Collections
{
    [CustomPropertyDrawer(typeof(SerializableStack<>), true)]
public class SerializableStackDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var listProp = property.FindPropertyRelative("list");

        label.text += " (stack)";
        EditorGUI.PropertyField(position, listProp, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var listProp = property.FindPropertyRelative("list");
        return EditorGUI.GetPropertyHeight(listProp, label, true);
    }
}
}

#endif