using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShibaInspector.Collections
{
    [Serializable]
public class SerializableQueue<T> : Queue<T>, ISerializationCallbackReceiver
{
    [SerializeField] private List<T> list = new();

    public void OnBeforeSerialize()
    {
        list.Clear();
        foreach (T item in this)
        {
            list.Insert(0, item);
        }
    }

    public void OnAfterDeserialize()
    {
        Clear();
        for(int i = list.Count - 1; i >= 0; i--)
        {
            Enqueue(list[i]);
        }
    }
}

}