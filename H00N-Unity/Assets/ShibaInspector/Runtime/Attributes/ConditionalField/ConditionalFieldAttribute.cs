using UnityEngine;

namespace ShibaInspector.Attributes
{
    public class ConditionalFieldAttribute : PropertyAttribute
    {
        public string ConditionalSourceField; // 조건 기준이 되는 다른 필드명
        public object CompareValue;           // 비교 값 (값이 제공되지 않으면 기본적으로 bool true 혹은 null 여부로 판단)
        public bool HideInInspector;          // 조건이 만족하지 않을 때 Inspector에서 완전히 숨길지 여부

        public ConditionalFieldAttribute(string conditionalSourceField)
        {
            this.ConditionalSourceField = conditionalSourceField;
            this.HideInInspector = false;
        }

        // 비교 값을 지정할 수 있는 생성자. 예를 들어, enum이나 int 등과 비교하고 싶을 때 사용.
        public ConditionalFieldAttribute(string conditionalSourceField, object compareValue, bool hideInInspector = false)
        {
            this.ConditionalSourceField = conditionalSourceField;
            this.CompareValue = compareValue;
            this.HideInInspector = hideInInspector;
        }
    }
}