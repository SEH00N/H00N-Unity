using TMPro;
using UnityEngine;

namespace H00N.Localizations
{
    [CreateAssetMenu(menuName = "Localization/Font Group")]
    public class FontGroup : ScriptableObject
    {
        public TMP_FontAsset font;
        public Material material;
    }
}