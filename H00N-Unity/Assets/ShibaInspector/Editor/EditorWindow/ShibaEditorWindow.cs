using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using ShibaInspector.Attributes;

namespace ShibaInspector.EidtorWindow
{
    public class ShibaEditorWindow : EditorWindow
    {
        public class ShibaEditorContentWrapper : ScriptableObject
        {
            [SerializeReference]
            public object content;
        }

        protected void OnGUIContent(ShibaEditorContentWrapper target)
        {
            object obj = target.content;
            if (obj != null)
            {
                var methods = obj.GetType()
                                 .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                 .Where(m => m.GetCustomAttribute<OnGUIAttribute>() != null);

                foreach (var m in methods)
                {
                    var attr  = m.GetCustomAttribute<OnGUIAttribute>();
                    m.Invoke(obj, null);
                }
            }

            DrawSerializableObject(target);
        }

        private void DrawSerializableObject(ShibaEditorContentWrapper target)
        {
            // draw properties
            var so = new SerializedObject(target);
            so.Update();
            var sp = so.FindProperty("content");
            var it  = sp.Copy();
            var end = it.GetEndProperty();

            // step into first child
            it.NextVisible(true);
            EditorGUI.indentLevel++;

            while (!SerializedProperty.EqualContents(it, end))
            {
                EditorGUILayout.PropertyField(it, true);
                it.NextVisible(false);
            }

            EditorGUI.indentLevel--;
            so.ApplyModifiedProperties();

            // draw functions
            object obj = target.content;
            if (obj != null)
            {
                var methods = obj.GetType()
                                 .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                 .Where(m => m.GetCustomAttribute<ButtonAttribute>() != null);

                foreach (var m in methods)
                {
                    var attr  = m.GetCustomAttribute<ButtonAttribute>();
                    string label = !string.IsNullOrEmpty(attr.Label)
                                   ? attr.Label
                                   : ObjectNames.NicifyVariableName(m.Name);

                    if (GUILayout.Button(label))
                    {
                        m.Invoke(obj, null);
                    }
                }
            }
        }
    }
}