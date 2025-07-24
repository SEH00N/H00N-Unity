using UnityEditor;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace H00N.Localizations
{
    [CustomEditor(typeof(FontGroup))]
    [CanEditMultipleObjects]
    public class FontGroupEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var fontGroups = new List<FontGroup>();
            foreach (var t in targets)
                fontGroups.Add((FontGroup)t);

            // Mixed value 처리
            bool mixedFont = false;
            TMP_FontAsset firstFont = fontGroups[0].font;
            foreach (var fg in fontGroups)
            {
                if (fg.font != firstFont)
                {
                    mixedFont = true;
                    break;
                }
            }

            EditorGUI.showMixedValue = mixedFont;
            EditorGUI.BeginChangeCheck();
            TMP_FontAsset newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Font Asset", firstFont, typeof(TMP_FontAsset), false);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var fg in fontGroups)
                {
                    Undo.RecordObject(fg, "Font Group Font Changed");
                    fg.font = newFont;
                    EditorUtility.SetDirty(fg);
                }
            }

            // 폰트가 null인 오브젝트에 defaultFontAsset 할당
            foreach (var fg in fontGroups)
            {
                if (fg.font == null)
                {
                    fg.font = TMP_Settings.defaultFontAsset;
                    EditorUtility.SetDirty(fg);
                }
            }

            // 모두 폰트가 null이면 종료
            if (fontGroups.TrueForAll(fg => fg.font == null))
                return;

            // 첫 번째 오브젝트 기준으로 머티리얼 프리셋 생성
            string fontAssetPath = AssetDatabase.GetAssetPath(fontGroups[0].font);
            string fontFolderPath = System.IO.Path.GetDirectoryName(fontAssetPath);
            string[] guids = AssetDatabase.FindAssets("t:Material", new[] { fontFolderPath });

            var presets = new List<Material>() { fontGroups[0].font.material };

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

                if (mat == null)
                    continue;

                if (mat == fontGroups[0].font.material)
                    continue;

                string fontName = fontGroups[0].font.name.Replace("_", "").Replace(" ", "").Trim().ToLower();
                string materialName = mat.name.Replace("_", "").Replace(" ", "").Trim().ToLower();
                if (materialName.StartsWith(fontName) == false)
                    continue;

                presets.Add(mat);
            }

            string[] names = presets.ConvertAll(m => m.name).ToArray();
            // 머티리얼 mixed value 처리
            bool mixedMat = false;
            Material firstMat = fontGroups[0].material;
            foreach (var fg in fontGroups)
            {
                if (fg.material != firstMat)
                {
                    mixedMat = true;
                    break;
                }
            }
            int selectedIndex = presets.IndexOf(firstMat);
            if (selectedIndex < 0) selectedIndex = 0;

            // material이 null인 오브젝트에 presets[0] 할당
            foreach (var fg in fontGroups)
            {
                if (fg.material == null && presets.Count > 0)
                {
                    fg.material = presets[0];
                    EditorUtility.SetDirty(fg);
                }
            }

            EditorGUI.showMixedValue = mixedMat;
            EditorGUI.BeginChangeCheck();
            int newSelectedIndex = EditorGUILayout.Popup("Material Preset", selectedIndex, names);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var fg in fontGroups)
                {
                    fg.material = presets[newSelectedIndex];
                    Undo.RecordObject(fg, "Font Group Material Preset Changed");
                    EditorUtility.SetDirty(fg);
                }
            }
        }
    }
}