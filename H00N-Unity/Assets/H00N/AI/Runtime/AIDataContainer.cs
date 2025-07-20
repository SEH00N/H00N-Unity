using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace H00N.AI
{
    [Serializable]
    public class AIDataContainer
    {
        [Serializable]
        public class AIDataWrapper
        {
            public enum AIDataWrapType
            {
                None,
                UnityObject,
                SerializableObject
            }

            public AIDataWrapType wrapType = AIDataWrapType.None;
            public Object unityObjectReference = null;
            [SerializeReference] public object serializableObjectReference = null;

            public IAIData GetAIData()
            {
                return wrapType switch {
                    AIDataWrapType.UnityObject => unityObjectReference as IAIData,
                    AIDataWrapType.SerializableObject => serializableObjectReference as IAIData,
                    _ => null
                };
            }
        }

        [SerializeField] List<AIDataWrapper> aiDataList = null;
        private Dictionary<Type, IAIData> aiDataDictionary = null;

        private bool isInitialized = false;

        public void Initialize()
        {
            aiDataDictionary = new Dictionary<Type, IAIData>();
            aiDataList.ForEach(i => {
                if(i == null)
                    return;

                IAIData aiData = i.GetAIData();
                if(aiData == null)
                    return;

                Type type = aiData.GetType();
                if(aiDataDictionary.ContainsKey(type))
                    return;

                aiDataDictionary.Add(type, aiData.Initialize());
            });

            isInitialized = true;
        }

        public T GetAIData<T>() where T : class, IAIData
        {
            if(isInitialized == false)
                Initialize();

            Type type = typeof(T);
            aiDataDictionary.TryGetValue(type, out IAIData aiData);
            return aiData as T;
        }
    }
}
