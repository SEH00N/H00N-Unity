using UnityEngine;

namespace H00N.Extensions
{
    public static class TransformExtensions
    {
        public static void SetAnchoredRect(this RectTransform rectTransform, AnchoredRect anchoredRect)
        {
            rectTransform.position = anchoredRect.position;
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 0);
            rectTransform.sizeDelta = anchoredRect.size;
            rectTransform.pivot = anchoredRect.pivot;
        }
    }
}
