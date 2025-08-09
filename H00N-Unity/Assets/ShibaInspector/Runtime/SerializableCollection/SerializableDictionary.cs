using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShibaInspector.Collections
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<TKey, TValue>>
    {
        [SerializeField]
        [HideInInspector]
        public List<TKey> keys = new();
        [SerializeField]
        [HideInInspector]
        public List<TValue> values = new();

        public Dictionary<TKey, TValue> myDictionary = new Dictionary<TKey, TValue>();

        public TValue this[TKey key]
        {
            get { return myDictionary[key]; }
            set { myDictionary[key] = value; }
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            // For each key/value pair in the dictionary, add the key to the keys list and the value to the values list
            foreach (var kvp in myDictionary)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            myDictionary = new Dictionary<TKey, TValue>();

            // Loop through the list of keys and values and add each key/value pair to the dictionary
            for (int i = 0; i != Math.Min(keys.Count, values.Count); i++)
            {
                myDictionary.Add(keys[i], values[i]);
            }
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys => myDictionary.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => myDictionary.Values;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => myDictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TKey key, TValue value) => myDictionary.Add(key, value);
        public bool ContainsKey(TKey key) => myDictionary.ContainsKey(key);
        public bool ContainsValue(TValue value) => myDictionary.ContainsValue(value);
        public bool Remove(TKey key) => myDictionary.Remove(key);
        public void Clear() => myDictionary.Clear();
        public bool TryAdd(TKey key, TValue value) => myDictionary.TryAdd(key, value);
        public bool TryGetValue(TKey key, out TValue value) => myDictionary.TryGetValue(key, out value);

        public static implicit operator Dictionary<TKey, TValue>(SerializableDictionary<TKey, TValue> serializableDictionary) => serializableDictionary.myDictionary;
    }
}