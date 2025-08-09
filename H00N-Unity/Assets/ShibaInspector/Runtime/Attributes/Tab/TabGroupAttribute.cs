using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ShibaInspector.Attributes
{
// 1. TabGroup Attribute
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class TabGroupAttribute : PropertyAttribute
{
    public string group;
    public string tab;
    public TabGroupAttribute(string group, string tab)
    {
        this.group = group;
        this.tab   = tab;
    }
}
}