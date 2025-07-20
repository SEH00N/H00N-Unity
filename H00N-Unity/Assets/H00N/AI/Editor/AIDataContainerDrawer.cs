using UnityEditor;
using UnityEngine;

namespace H00N.AI.Editor
{
    [CustomPropertyDrawer(typeof(AIDataContainer))]
    public class AIDataContainerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("aiDataList"), label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("aiDataList"), label);
        }
    }
}
