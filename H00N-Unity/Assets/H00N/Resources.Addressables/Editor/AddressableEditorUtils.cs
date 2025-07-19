using System;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using Object = UnityEngine.Object;

namespace H00N.Resources.Addressables.Editor
{
    public static class AddressableEditorUtils
    {
        public static Object GetAssetFromKey(string key, Type type)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            if (type == null)
                return null;

            if (type.IsSubclassOf(typeof(Object)) == false)
                return null;

            // AddressableAssetSettings 인스턴스 가져오기
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
                return null;

            // 모든 그룹을 순회하며 key에 해당하는 엔트리 찾기
            foreach (var group in settings.groups)
            {
                if (group == null) continue;

                foreach (var entry in group.entries)
                {
                    if (entry.address == key)
                    {
                        if (type.IsSubclassOf(typeof(Component)))
                        {
                            if (entry.MainAsset is GameObject asset)
                            {
                                if (asset.TryGetComponent(type, out Component component))
                                    return component;
                            }
                        }
                        else
                        {
                            if (type == typeof(Sprite) && entry.MainAsset is Texture2D texture2D)
                            {
                                var subAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(texture2D));
                                foreach (var subAsset in subAssets)
                                {
                                    if (subAsset is Sprite sprite)
                                        return sprite;
                                }
                            }
                            else
                            {
                                Type assetType = entry.MainAsset.GetType();
                                if (assetType == type || assetType.IsSubclassOf(type))
                                    return entry.MainAsset;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static string GetKeyFromAsset(Object asset)
        {
            if (asset == null)
                return null;

            // 에셋의 GUID 가져오기
            string assetPath = AssetDatabase.GetAssetPath(asset);
            string guid = AssetDatabase.AssetPathToGUID(assetPath);

            if (string.IsNullOrEmpty(guid))
                return null;

            // AddressableAssetSettings 인스턴스 가져오기
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
                return null;

            // 모든 그룹을 순회하며 GUID에 해당하는 엔트리 찾기
            foreach (var group in settings.groups)
            {
                if (group == null) continue;

                foreach (var entry in group.entries)
                {
                    if (entry.guid == guid)
                    {
                        // Addressables key 반환
                        return entry.address;
                    }
                }
            }

            return null;
        }
    }
}