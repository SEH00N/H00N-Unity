using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

namespace ShibaInspector.Attributes
{
    // 2. Tree Node Definition
    // 2. Tree Node Definition
internal class GroupNode
{
    public string Name;
    public bool IsHorizontal;
    public List<SerializedProperty> Fields = new List<SerializedProperty>();
    public List<GroupNode> Children = new List<GroupNode>();

    public GroupNode(string name, bool isHorizontal)
    {
        Name = name;
        IsHorizontal = isHorizontal;
    }
}

// 3. Custom Editor
[CustomEditor(typeof(MonoBehaviour), true)]
public class GroupAttributeEditor : Editor
{
    private GroupNode root;
    private Dictionary<string, GroupNode> nodeLookup;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        BuildTree();
        DrawGroup(root);

        serializedObject.ApplyModifiedProperties();
    }

    private void BuildTree()
    {
        root = new GroupNode("", true);
        nodeLookup = new Dictionary<string, GroupNode>();
        nodeLookup[string.Empty] = root;

        var prop = serializedObject.GetIterator();
        if (!prop.NextVisible(true))
            return;

        do
        {
            // Check for group attributes via reflection
            var field = target.GetType().GetField(prop.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null)
            {
                // Render default for non-field props
                root.Fields.Add(prop.Copy());
                continue;
            }

            var hg = field.GetCustomAttribute<HorizontalGroupAttribute>(true);
            var vg = field.GetCustomAttribute<VerticalGroupAttribute>(true);

            if (hg == null && vg == null)
            {
                root.Fields.Add(prop.Copy());
                continue;
            }

            string id = hg != null ? hg.groupId : vg.groupId;
            bool isHorizontal = hg != null;
            var segments = string.IsNullOrEmpty(id) ? new[] { field.Name } : id.Split('/');

            string path = string.Empty;
            GroupNode current = root;
            foreach (var seg in segments)
            {
                path = string.IsNullOrEmpty(path) ? seg : path + "/" + seg;
                if (!nodeLookup.TryGetValue(path, out var child))
                {
                    child = new GroupNode(seg, isHorizontal);
                    nodeLookup[path] = child;
                    current.Children.Add(child);
                }
                current = child;
            }
            current.Fields.Add(prop.Copy());

        } while (prop.NextVisible(false));
    }

    private void DrawGroup(GroupNode node)
    {
        if (!string.IsNullOrEmpty(node.Name))
        {
            if (node.IsHorizontal)
                EditorGUILayout.BeginHorizontal("box");
            else
                EditorGUILayout.BeginVertical("box");
        }

        // Draw fields
        foreach (var fieldProp in node.Fields)
        {
            EditorGUILayout.PropertyField(fieldProp, true);
        }

        // Draw children recursively
        foreach (var child in node.Children)
        {
            DrawGroup(child);
        }

        if (!string.IsNullOrEmpty(node.Name))
        {
            if (node.IsHorizontal)
                EditorGUILayout.EndHorizontal();
            else
                EditorGUILayout.EndVertical();
        }
    }
}

}
