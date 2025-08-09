using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShibaInspector.Collections
{
    [Serializable]
public class SerializableStack<T> : Stack<T>, ISerializationCallbackReceiver
{
    [SerializeField] private List<T> list = new();

    public void OnBeforeSerialize()
    {
        list.Clear();
        foreach(T elem in this)
        {
            list.Add(elem);
        }
    }

    public void OnAfterDeserialize()
    {
        Clear();
        for(int i = list.Count - 1; i >= 0; i--)
        {
            Push(list[i]);
        }
    }
}
}