using UnityEngine.Localization.Components;
using UnityEngine;
using TMPro;

namespace H00N.Localizations
{
    public class LocalizeFontGroupEvent : LocalizedAssetBehaviour<FontGroup, LocalizedFontGroup>
    {
        [SerializeField] TMP_Text text;

        protected override void UpdateAsset(FontGroup localizedAsset)
        {
            if(localizedAsset == null)
                return;

            if (text == null)
                return;

            text.font = localizedAsset.font;
            text.fontSharedMaterial = localizedAsset.material;
        }
    }
}