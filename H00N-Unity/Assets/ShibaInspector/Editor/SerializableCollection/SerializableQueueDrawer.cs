#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace ShibaInspector.Collections
{
    [CustomPropertyDrawer(typeof(SerializableQueue<>), true)]
public class SerializableQueueDrawer : PropertyDrawer
{
    private bool foldout = true;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var listProp = property.FindPropertyRelative("list");

        label.text += " (queue)";
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