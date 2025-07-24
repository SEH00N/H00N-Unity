using TMPro;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace H00N.Localizations
{
    [CustomEditor(typeof(TextMeshPro), true)]
    [CanEditMultipleObjects]
    public class TextMeshProOverrideEditor : TMPro.EditorUtilities.TMP_EditorPanel
    {
        public override void OnInspectorGUI()
        {
            DrawLocalizeFontGroupEventMissing();
            base.OnInspectorGUI();
        }

        private void DrawLocalizeFontGroupEventMissing()
        {
            var tmpTexts = targets.OfType<TextMeshPro>().ToArray();
            if (tmpTexts.Length <= 0 || tmpTexts.Any(x => x != null && x.TryGetComponent<LocalizeFontGroupEvent>(out _)))
                return;

            EditorGUILayout.HelpBox("LocalizeFontGroupEvent Missing", MessageType.Warning);
            bool addTriggered = GUILayout.Button("Add LocalizeFontGroupEvent");
            EditorGUILayout.Space(10);

            if (addTriggered == false)
                return;

            Undo.RecordObjects(tmpTexts.Select(i => i.gameObject).ToArray(), "Add LocalizeFontGroupEvent");
            foreach (var tmpText in tmpTexts)
                AddLocalizeFontGroupEvent(tmpText);
            Undo.RecordObjects(tmpTexts.Select(i => i.gameObject).ToArray(), "Add LocalizeFontGroupEvent Success");
        }

        private void AddLocalizeFontGroupEvent(TextMeshPro tmpText)
        {
            var newComponent = Undo.AddComponent<LocalizeFontGroupEvent>(tmpText.gameObject);
            var serializedObject = new SerializedObject(newComponent);
            var textProperty = serializedObject.FindProperty("text");
            textProperty.objectReferenceValue = tmpText;
            serializedObject.ApplyModifiedProperties();

            var components = tmpText.gameObject.GetComponents<Component>();
            var tmpIndex = System.Array.IndexOf(components, tmpText);
            var newIndex = System.Array.IndexOf(components, newComponent);

            while (newIndex > tmpIndex)
            {
                UnityEditorInternal.ComponentUtility.MoveComponentUp(newComponent);
                newIndex--;
            }
        }
    }
}