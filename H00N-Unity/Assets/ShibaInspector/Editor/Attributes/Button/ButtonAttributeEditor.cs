// Editor/ButtonAttributeEditor.cs
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace ShibaInspector.Attributes
{
    // 모든 MonoBehaviour에 적용
    [CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
    public class ButtonAttributeEditor : Editor
    {
        
        // 저장: group name -> list of tab names
    private Dictionary<string, string[]> tabsByGroup;
    // 선택된 탭 인덱스
    private Dictionary<string, int>   selectedTab;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        // 1) Build map once per draw
        BuildTabs();

        // 2) Iterate through properties and collect fields by group & tab
        var props = new Dictionary<(string group, string tab), List<SerializedProperty>>();
        var it = serializedObject.GetIterator();
        if (it.NextVisible(true) && !it.name.Contains("Script"))
        {
            do
            {
                var fi = target.GetType().GetField(it.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (fi == null)
                        {
                            // 기본 노출 필드
                            EditorGUILayout.PropertyField(it, true);
                            continue;
                        }
                var attrs = fi.GetCustomAttributes<TabGroupAttribute>(true).ToArray();
                if (attrs.Length == 0)
                {
                    EditorGUILayout.PropertyField(it, true);
                }
                else
                {
                    foreach (var a in attrs)
                    {
                        var key = (a.group, a.tab);
                        if (!props.ContainsKey(key)) 
                            props[key] = new List<SerializedProperty>();
                        props[key].Add(serializedObject.FindProperty(it.propertyPath));
                    }
                }
            } while (it.NextVisible(false));
        }

        // 3) Draw tabs and fields per group
        foreach (var kv in tabsByGroup)
        {
            var groupName = kv.Key;
            var tabNames  = kv.Value;
            if (!selectedTab.ContainsKey(groupName)) selectedTab[groupName] = 0;

            // 탭 헤더
            selectedTab[groupName] = GUILayout.Toolbar(selectedTab[groupName], tabNames);
            GUILayout.Space(4);

            // 선택된 탭만 렌더링
            var activeTab = tabNames[selectedTab[groupName]];
            var key = (groupName, activeTab);
            if (props.TryGetValue(key, out var list))
            {
                foreach (var p in list)
                    EditorGUILayout.PropertyField(p, true);
            }

            GUILayout.Space(8);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void BuildTabs()
    {
        tabsByGroup = new Dictionary<string, string[]>();
        selectedTab = new Dictionary<string, int>();

        // 리플렉션으로 MonoBehaviour 필드 스캔
        var fields = target.GetType()
                           .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                           .Where(f => f.GetCustomAttributes<TabGroupAttribute>(true).Any());

        foreach (var f in fields)
        {
            foreach (var a in f.GetCustomAttributes<TabGroupAttribute>(true))
            {
                if (!tabsByGroup.TryGetValue(a.group, out var arr))
                {
                    tabsByGroup[a.group] = new[] { a.tab };
                }
                else if (!arr.Contains(a.tab))
                {
                    tabsByGroup[a.group] = arr.Append(a.tab).ToArray();
                }
            }
        }
    }
    }   
}