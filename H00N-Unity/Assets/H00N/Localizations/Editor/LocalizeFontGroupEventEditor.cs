using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;

namespace H00N.Localizations
{
    [CustomEditor(typeof(LocalizeFontGroupEvent))]
    [CanEditMultipleObjects]
    public class LocalizeFontGroupEventEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawLocalizeStringEventButton();
        }

        private void DrawLocalizeStringEventButton()
        {
            var localizeFontGroupEvents = targets.OfType<LocalizeFontGroupEvent>().ToArray();
            if (localizeFontGroupEvents.Length <= 0 || localizeFontGroupEvents.Any(x => x.TryGetComponent<LocalizeStringEvent>(out _)))
                return;

            EditorGUILayout.Space(10);
            if (GUILayout.Button("Add LocalizeStringEvent") == false)
                return;

            Undo.RecordObjects(targets, "Add LocalizeStringEvent");
            foreach (var target in targets)
                AddLocalizeStringEvent(target as LocalizeFontGroupEvent);
            Undo.RecordObjects(targets, "Add LocalizeStringEvent Success");
        }

        private void AddLocalizeStringEvent(LocalizeFontGroupEvent localizeFontGroupEvent)
        {
            if(localizeFontGroupEvent == null)
                return;

            LocalizeStringEvent localizeStringEvent = localizeFontGroupEvent.gameObject.AddComponent<LocalizeStringEvent>();
            Component[] components = localizeFontGroupEvent.gameObject.GetComponents<Component>();
            int tmpIndex = System.Array.IndexOf(components, localizeFontGroupEvent);
            int newIndex = System.Array.IndexOf(components, localizeStringEvent);

            while (newIndex > tmpIndex)
            {
                UnityEditorInternal.ComponentUtility.MoveComponentUp(localizeStringEvent);
                newIndex--;
            }

            SerializedObject serializedLocalizeFontGroupEvent = new SerializedObject(localizeFontGroupEvent);
            SerializedProperty textProperty = serializedLocalizeFontGroupEvent.FindProperty("text");
            if(textProperty == null)
                return;

            TMP_Text tmpText = textProperty.objectReferenceValue as TMP_Text;
            if(tmpText == null)
                return;

            MethodInfo setter = typeof(TMP_Text).GetMethod("set_text", BindingFlags.Instance | BindingFlags.Public);
            UnityAction<string> methodDelegate = System.Delegate.CreateDelegate(typeof(UnityAction<string>), tmpText, setter) as UnityAction<string>;
            UnityEditor.Events.UnityEventTools.AddPersistentListener(localizeStringEvent.OnUpdateString, methodDelegate);
            localizeStringEvent.OnUpdateString.SetPersistentListenerState(0, UnityEventCallState.EditorAndRuntime);
            EditorUtility.SetDirty(localizeStringEvent);
        }
    }
}