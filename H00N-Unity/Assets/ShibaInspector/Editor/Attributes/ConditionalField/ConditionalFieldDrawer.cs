using UnityEngine;
using UnityEditor;

namespace ShibaInspector.Attributes
{
    [CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
    public class ConditionalFieldDrawer : PropertyDrawer
    {
        // 시뮬레이션 값 저장용 (에디터 내부에서만 유지됨)
        private static readonly System.Collections.Generic.Dictionary<string, bool> conditionSimStates = new();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ConditionalFieldAttribute condAttrib = (ConditionalFieldAttribute)attribute;
            SerializedProperty sourceProperty = property.serializedObject.FindProperty(condAttrib.ConditionalSourceField);

            bool sourceExists = sourceProperty != null;
            bool toggleTemp = false;
            string conditionKey = $"{property.serializedObject.targetObject.GetInstanceID()}_{condAttrib.ConditionalSourceField}";
            GUIContent labelTemp = new GUIContent(label);
            bool conditionMet = true;
            if (sourceExists)
            {
                conditionMet = EvaluateCondition(sourceProperty, condAttrib);
            }
            else
            {
                string path = AssetDatabase.GetAssetPath(property.serializedObject.targetObject);
                string guid = AssetDatabase.AssetPathToGUID(path);
                string editorPrefsKey = $"ConditionalSim_{guid}_{property.propertyPath}_{property.serializedObject.targetObject.name}";
                // 조건 필드가 없으면 시뮬레이션 토글값을 사용
                if (!conditionSimStates.ContainsKey(conditionKey))
                    conditionSimStates[conditionKey] = false;

                conditionSimStates[conditionKey] = EditorPrefs.GetBool(editorPrefsKey, false);
                
                toggleTemp = conditionSimStates[conditionKey];
                Rect toggleRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                conditionSimStates[conditionKey] = EditorGUI.ToggleLeft(toggleRect,
                    $"{condAttrib.ConditionalSourceField}", conditionSimStates[conditionKey]);

                if (toggleTemp != conditionSimStates[conditionKey])
                {
                    EditorPrefs.SetBool(editorPrefsKey, conditionSimStates[conditionKey]);
                }
                //Debug.Log(conditionSimStates[conditionKey]);
                conditionMet = conditionSimStates[conditionKey] == (bool)condAttrib.CompareValue;
            }

            if (condAttrib.HideInInspector == false)
            {
                position.y += EditorGUIUtility.singleLineHeight + 2f;
                EditorGUI.BeginProperty(position, labelTemp, property);
                if (!conditionMet)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.PropertyField(position, property, labelTemp, true);
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    EditorGUI.PropertyField(position, property, labelTemp, true);
                }
                EditorGUI.EndProperty();
            }
            else
            {
                if (conditionMet)
                {
                    position.y += EditorGUIUtility.singleLineHeight + 2f;
                    EditorGUI.BeginProperty(position, labelTemp, property);
                    if (!conditionMet)
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUI.PropertyField(position, property, labelTemp, true);
                        EditorGUI.EndDisabledGroup();
                    }
                    else
                    {
                        EditorGUI.PropertyField(position, property, labelTemp, true);
                    }
                    EditorGUI.EndProperty();
                }
            }
            // else if (condAttrib.HideInInspector && !conditionMet)
            //     return;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            string path = AssetDatabase.GetAssetPath(property.serializedObject.targetObject);
            string guid = AssetDatabase.AssetPathToGUID(path);
            string editorPrefsKey = $"ConditionalSim_{guid}_{property.propertyPath}_{property.serializedObject.targetObject.name}";

            ConditionalFieldAttribute condAttrib = (ConditionalFieldAttribute)attribute;
            SerializedProperty sourceProperty = property.serializedObject.FindProperty(condAttrib.ConditionalSourceField);

            bool sourceExists = sourceProperty != null;
            bool conditionMet = sourceExists ? EvaluateCondition(sourceProperty, condAttrib) 
                : EditorPrefs.GetBool(editorPrefsKey, false) == (bool)condAttrib.CompareValue;
            
            if (condAttrib.HideInInspector && !conditionMet)
            {
                return 16f;
            }
            
            float height = EditorGUI.GetPropertyHeight(property, label, true);
            height += EditorGUIUtility.singleLineHeight + 3f;

            return height;
        }

        private bool EvaluateCondition(SerializedProperty sourceProperty, ConditionalFieldAttribute condAttrib)
        {
            if (condAttrib.CompareValue != null)
            {
                switch (sourceProperty.propertyType)
                {
                    case SerializedPropertyType.Boolean:
                        return sourceProperty.boolValue.Equals(condAttrib.CompareValue);
                    case SerializedPropertyType.Enum:
                        int targetIndex = (condAttrib.CompareValue is int) ? (int)condAttrib.CompareValue : System.Convert.ToInt32(condAttrib.CompareValue);
                        return sourceProperty.enumValueIndex == targetIndex;
                    case SerializedPropertyType.Integer:
                        return sourceProperty.intValue.Equals(condAttrib.CompareValue);
                    case SerializedPropertyType.Float:
                        return Mathf.Approximately(sourceProperty.floatValue, System.Convert.ToSingle(condAttrib.CompareValue));
                    case SerializedPropertyType.String:
                        return sourceProperty.stringValue.Equals((string)condAttrib.CompareValue);
                    default:
                        return false;
                }
            }
            else
            {
                if (sourceProperty.propertyType == SerializedPropertyType.Boolean)
                    return sourceProperty.boolValue;
                else if (sourceProperty.propertyType == SerializedPropertyType.ObjectReference)
                    return sourceProperty.objectReferenceValue != null;

                return true;
            }
        }
    }
}

#region 
// using UnityEngine;
// using UnityEditor;

// namespace ShibaInspector.Attributes
// {
//     [CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
//     public class ConditionalFieldDrawer : PropertyDrawer
//     {
//         // 실제 그리기
//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         {
//             ConditionalFieldAttribute condAttrib = (ConditionalFieldAttribute)attribute;
//             SerializedProperty sourceProperty = property.serializedObject.FindProperty(condAttrib.ConditionalSourceField);

//             if (sourceProperty == null)
//             {
//                 // 조건 기준 필드가 없으면 경고 메시지를 출력
//                 EditorGUI.PropertyField(position, property, label, true);
//                 EditorGUI.HelpBox(position, $"[ConditionalField] 에 지정한 필드 '{condAttrib.ConditionalSourceField}'를 찾을 수 없습니다.", MessageType.Warning);
//                 return;
//             }

//             // 조건 비교 결정
//             bool conditionMet = EvaluateCondition(sourceProperty, condAttrib);

//             // 조건이 충족하지 않고 HideInInspector가 true이면 그리기를 건너뛰기
//             if (condAttrib.HideInInspector && !conditionMet)
//             {
//                 return;
//             }

//             EditorGUI.BeginProperty(position, label, property);
//             // 조건이 만족하지 않을 경우에는 값은 보이되 Disabled 상태로 표시함 (HideInInspector가 false인 경우)
//             if (!conditionMet)
//             {
//                 EditorGUI.BeginDisabledGroup(true);
//                 EditorGUI.PropertyField(position, property, label, true);
//                 EditorGUI.EndDisabledGroup();
//             }
//             else
//             {
//                 EditorGUI.PropertyField(position, property, label, true);
//             }
//             EditorGUI.EndProperty();
//         }

//         // 높이 계산 : 조건이 HideInInspector이면 높이를 0으로 반환합니다.
//         public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//         {
//             ConditionalFieldAttribute condAttrib = (ConditionalFieldAttribute)attribute;
//             SerializedProperty sourceProperty = property.serializedObject.FindProperty(condAttrib.ConditionalSourceField);

//             if (sourceProperty == null)
//             {
//                 return EditorGUI.GetPropertyHeight(property, label, true);
//             }

//             bool conditionMet = EvaluateCondition(sourceProperty, condAttrib);
//             if (condAttrib.HideInInspector && !conditionMet)
//             {
//                 return 0f;
//             }
//             else
//             {
//                 return EditorGUI.GetPropertyHeight(property, label, true);
//             }
//         }

//         // 조건 평가 함수 : 비교 값이 제공된 경우 해당 값과 비교, 없으면 bool형의 true 여부를 판단
//         private bool EvaluateCondition(SerializedProperty sourceProperty, ConditionalFieldAttribute condAttrib)
//         {
//             if (condAttrib.CompareValue != null)
//             {
//                 // 비교 값이 있을 경우, 소스 프로퍼티의 타입에 따라 비교
//                 switch (sourceProperty.propertyType)
//                 {
//                     case SerializedPropertyType.Boolean:
//                         return sourceProperty.boolValue.Equals(condAttrib.CompareValue);
//                     case SerializedPropertyType.Enum:
//                         // compareValue가 int 혹은 enum값으로 들어올 수 있음
//                         int targetIndex = (condAttrib.CompareValue is int) ? (int)condAttrib.CompareValue : System.Convert.ToInt32(condAttrib.CompareValue);
//                         return sourceProperty.enumValueIndex == targetIndex;
//                     case SerializedPropertyType.Integer:
//                         return sourceProperty.intValue.Equals(condAttrib.CompareValue);
//                     case SerializedPropertyType.Float:
//                         return Mathf.Approximately(sourceProperty.floatValue, System.Convert.ToSingle(condAttrib.CompareValue));
//                     case SerializedPropertyType.String:
//                         return sourceProperty.stringValue.Equals((string)condAttrib.CompareValue);
//                     default:
//                         return false;
//                 }
//             }
//             else
//             {
//                 // 비교 값이 없으면 bool형일 경우 true, 그 외엔 null 여부
//                 if (sourceProperty.propertyType == SerializedPropertyType.Boolean)
//                 {
//                     return sourceProperty.boolValue;
//                 }
//                 else if (sourceProperty.propertyType == SerializedPropertyType.ObjectReference)
//                 {
//                     return sourceProperty.objectReferenceValue != null;
//                 }
//                 return true;
//             }
//         }
//     }
// }
#endregion