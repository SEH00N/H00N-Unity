using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using AIDataWrapType = H00N.AI.AIDataContainer.AIDataWrapper.AIDataWrapType;

namespace H00N.AI.Editor
{
    [CustomPropertyDrawer(typeof(AIDataContainer.AIDataWrapper))]
    public class AIDataWrapperDrawer : PropertyDrawer
    {
        private enum EUnityObjectType
        {
            None,
            MonoBehaviour,
            ScriptableObject
        }

        private static readonly Dictionary<string, EUnityObjectType> unityObjectTypeCache = new();
        private static Type[] serializableObjectTypes = null;
        private static string[] serializableObjectTypeNames = null;

        private bool isInitialized = false;

        private void InitializeSerializableObjectTypes()
        {
            var types = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            types.Add(null);
            foreach (var assembly in assemblies)
                types.AddRange(assembly.GetTypes().Where(t => typeof(IAIData).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && !t.IsGenericType));

            serializableObjectTypes = types.ToArray();
            serializableObjectTypeNames = types.Select(i => i?.FullName ?? "None").ToArray();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(isInitialized == false)
            {
                isInitialized = true;
                InitializeSerializableObjectTypes();
            }

            EditorGUI.BeginProperty(position, label, property);
            Rect wrapTypeDropdownRect = new Rect(position.x, position.y, position.width / 2 - 5, EditorGUIUtility.singleLineHeight);
            Rect objectFieldRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
            Rect objectTypeDropdownRect = new Rect(position.x + position.width / 2 + 5, position.y, position.width / 2 - 5, EditorGUIUtility.singleLineHeight);

            AIDataWrapType wrapType = DrawWrapTypeDropdown(wrapTypeDropdownRect, property);
            switch (wrapType)
            {
                case AIDataWrapType.UnityObject:
                    EUnityObjectType unityObjectType = DrawUnityObjectTypeDropdown(objectTypeDropdownRect, property);
                    DrawUnityObjectField(objectFieldRect, property, unityObjectType);
                    break;
                case AIDataWrapType.SerializableObject:
                    DrawSerializableObjectTypeDropdown(objectTypeDropdownRect, property);
                    DrawSerializableObjectField(objectFieldRect, property);
                    break;
                case AIDataWrapType.None:
                    break;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float baseHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
            var wrapType = (AIDataWrapType)property.FindPropertyRelative("wrapType").enumValueIndex;
            switch (wrapType)
            {
                case AIDataWrapType.SerializableObject:
                    var serializableObjectProp = property.FindPropertyRelative("serializableObjectReference");
                    if (serializableObjectProp.managedReferenceValue != null)
                    {
                        if (serializableObjectProp.isExpanded)
                            return baseHeight + EditorGUI.GetPropertyHeight(serializableObjectProp, true) + EditorGUIUtility.standardVerticalSpacing;
                        else
                            return baseHeight + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }
                    else
                    {
                        return baseHeight;
                    }
                case AIDataWrapType.UnityObject:
                    if (GetUnityObjectType(property) == EUnityObjectType.None)
                        return baseHeight;
                    else
                        return baseHeight + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                case AIDataWrapType.None:
                    return baseHeight;
            }

            return baseHeight;
        }

        // WrapType

        private AIDataWrapType DrawWrapTypeDropdown(Rect wrapTypeDropdownRect, SerializedProperty property)
        {
            SerializedProperty wrapTypeProperty = property.FindPropertyRelative("wrapType");
            AIDataWrapType previousWrapType = (AIDataWrapType)wrapTypeProperty.enumValueIndex;
            AIDataWrapType currentWrapType = DrawEnumDropdown(wrapTypeDropdownRect, previousWrapType);

            if (currentWrapType != previousWrapType)
            {
                Undo.RecordObject(property.serializedObject.targetObject, "AIDataWrapper Change WrapType");
                
                wrapTypeProperty.enumValueIndex = (int)currentWrapType;
                property.serializedObject.ApplyModifiedProperties();
                ClearCache(property);

                Undo.RecordObject(property.serializedObject.targetObject, "AIDataWrapper Change WrapType End");
            }

            return currentWrapType;
        }

        private void ClearCache(SerializedProperty property)
        {
            unityObjectTypeCache.Remove(GetGlobalPropertyKey(property));
            property.FindPropertyRelative("unityObjectReference").objectReferenceValue = null;
            property.FindPropertyRelative("serializableObjectReference").managedReferenceValue = null;
        }

        // UnityObject

        private EUnityObjectType DrawUnityObjectTypeDropdown(Rect objectTypeDropdownRect, SerializedProperty property)
        {
            SerializedProperty unityObjectReferenceProperty = property.FindPropertyRelative("unityObjectReference");
            EUnityObjectType previousUnityObjectType = GetUnityObjectType(property);
            EUnityObjectType currentUnityObjectType = DrawEnumDropdown(objectTypeDropdownRect, previousUnityObjectType);

            if (currentUnityObjectType != previousUnityObjectType)
            {
                Undo.RecordObject(property.serializedObject.targetObject, "AIDataWrapper Change UnityObjectType");
                
                unityObjectReferenceProperty.objectReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
                unityObjectTypeCache[GetGlobalPropertyKey(property)] = currentUnityObjectType;
                
                Undo.RecordObject(property.serializedObject.targetObject, "AIDataWrapper Change UnityObjectType End");
            }

            return currentUnityObjectType;
        }

        private void DrawUnityObjectField(Rect objectFieldRect, SerializedProperty property, EUnityObjectType unityObjectType)
        {
            SerializedProperty unityObjectReferenceProperty = property.FindPropertyRelative("unityObjectReference");
            Object previousUnityObject = unityObjectReferenceProperty.objectReferenceValue;
            Object selectedUnityObject = unityObjectType switch {
                EUnityObjectType.MonoBehaviour => EditorGUI.ObjectField(objectFieldRect, previousUnityObject, typeof(MonoBehaviour), true),
                EUnityObjectType.ScriptableObject => EditorGUI.ObjectField(objectFieldRect, previousUnityObject, typeof(ScriptableObject), false),
                EUnityObjectType.None => null,
                _ => null
            };


            if (selectedUnityObject != previousUnityObject)
            {
                Undo.RecordObject(property.serializedObject.targetObject, "AIDataWrapper Change UnityObject");

                unityObjectReferenceProperty.objectReferenceValue = selectedUnityObject;
                property.serializedObject.ApplyModifiedProperties();

                Undo.RecordObject(property.serializedObject.targetObject, "AIDataWrapper Change UnityObject End");
            }
        }

        // SerializableObject

        private void DrawSerializableObjectTypeDropdown(Rect objectTypeDropdownRect, SerializedProperty property)
        {
            SerializedProperty serializableObjectReferenceProperty = property.FindPropertyRelative("serializableObjectReference");
            Type previousSerializableObjectType = serializableObjectReferenceProperty.managedReferenceValue?.GetType();

            int currentSerializableObjectTypeIndex = Array.IndexOf(serializableObjectTypeNames, previousSerializableObjectType?.FullName);
            int selectedSerializableObjectTypeIndex = EditorGUI.Popup(objectTypeDropdownRect, currentSerializableObjectTypeIndex != -1 ? currentSerializableObjectTypeIndex : 0, serializableObjectTypeNames);
            Type currentSerializableObjectType = serializableObjectTypes[selectedSerializableObjectTypeIndex];

            if (currentSerializableObjectType != previousSerializableObjectType)
            {
                Undo.RecordObject(property.serializedObject.targetObject, "AIDataWrapper Change SerializableObjectType");
                
                if (currentSerializableObjectType != null)
                    serializableObjectReferenceProperty.managedReferenceValue = Activator.CreateInstance(currentSerializableObjectType);
                else
                    serializableObjectReferenceProperty.managedReferenceValue = null;

                property.serializedObject.ApplyModifiedProperties();

                Undo.RecordObject(property.serializedObject.targetObject, "AIDataWrapper Change SerializableObjectType End");
            }
        }

        private void DrawSerializableObjectField(Rect objectFieldRect, SerializedProperty property)
        {
            SerializedProperty serializableObjectReferenceProperty = property.FindPropertyRelative("serializableObjectReference");
            if(serializableObjectReferenceProperty.managedReferenceValue == null)
                return;

            EditorGUI.PropertyField(objectFieldRect, serializableObjectReferenceProperty, new GUIContent(serializableObjectReferenceProperty.managedReferenceValue.GetType().FullName), true);
        }

        // Utility

        private static TEnum DrawEnumDropdown<TEnum>(Rect dropdownRect, TEnum currentEnumValue) where TEnum : struct, Enum
        {
            TEnum[] enumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();
            string[] enumNames = enumValues.Select(t => t.ToString()).ToArray();

            int currentEnumIndex = Array.IndexOf(enumValues, currentEnumValue);
            int selectedIndex = EditorGUI.Popup(dropdownRect, currentEnumIndex != -1 ? currentEnumIndex : 0, enumNames);

            return enumValues[selectedIndex];
        }

        private static EUnityObjectType GetUnityObjectType(SerializedProperty property)
        {
            SerializedProperty unityObjectReferenceProperty = property.FindPropertyRelative("unityObjectReference");
            if (unityObjectReferenceProperty.objectReferenceValue == null)
            {
                if (unityObjectTypeCache.TryGetValue(GetGlobalPropertyKey(property), out EUnityObjectType cachedUnityObjectType))
                    return cachedUnityObjectType;
                else
                    return EUnityObjectType.None;
            }
            
            if (unityObjectReferenceProperty.objectReferenceValue.GetType().IsSubclassOf(typeof(MonoBehaviour)))
                return EUnityObjectType.MonoBehaviour;

            if(unityObjectReferenceProperty.objectReferenceValue.GetType().IsSubclassOf(typeof(ScriptableObject)))
                return EUnityObjectType.ScriptableObject;

            return EUnityObjectType.None;
        }

        // private static Type GetSerializableObjectType(SerializedProperty property)
        // {
        //     SerializedProperty serializableObjectReferenceProperty = property.FindPropertyRelative("serializableObjectReference");
        //     if (serializableObjectReferenceProperty.managedReferenceValue != null)
        //         return serializableObjectReferenceProperty.managedReferenceValue.GetType();

        //     serializableObjectTypeNameCache.TryGetValue(GetGlobalPropertyKey(property), out string cachedSerializableObjectType);
        //     if(string.IsNullOrEmpty(cachedSerializableObjectType))
        //         return null;
        //     else
        //         return Type.GetType(cachedSerializableObjectType);
        // }

        private static string GetGlobalPropertyKey(SerializedProperty property)
        {
            int instanceId = property.serializedObject.targetObject.GetInstanceID();
            return $"{instanceId}_{property.propertyPath}";
        }
    }
}
