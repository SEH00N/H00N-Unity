using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShibaInspector.Attributes;
using System.Runtime.InteropServices;

public class Test : MonoBehaviour
{
    public bool c;
    [ConditionalField("a con")]
    [SerializeField]
    private int a;
    public bool b;
}
