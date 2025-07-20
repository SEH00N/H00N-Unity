using UnityEngine;
using H00N.AI;

public class AIDataContainerTest : MonoBehaviour
{
    [System.Serializable]
    public class TestAIData : IAIData
    {
        public int testInt = 0;

        public IAIData Initialize()
        {
            return this;
        }
    }

    [SerializeField] AIDataContainer aiDataContainer = null;

    private void Start()
    {
        aiDataContainer.Initialize();
    }
}
