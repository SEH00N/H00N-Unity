using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ShibaInspector.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class VerticalGroupAttribute : PropertyAttribute
    {
        public string groupId;

        public VerticalGroupAttribute(string groupId = "defualt")
        {
            this.groupId = groupId;
        }
    }
}
