using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ShibaInspector.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class HorizontalGroupAttribute : PropertyAttribute
    {
        public string groupId;
        
        public HorizontalGroupAttribute(string groupId = "default") 
        {
            this.groupId = groupId;
        }
    }
}