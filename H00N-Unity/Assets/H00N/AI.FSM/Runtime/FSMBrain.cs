using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace H00N.AI.FSM
{
    public class FSMBrain : MonoBehaviour
    {
        // <prev, new>
        public UnityEvent<FSMState, FSMState> OnStateChangedEvent = null;

        [Space(15f)]
        [SerializeField] AIDataContainer aiDataContainer = null;

        [Space(15f)]
        [SerializeField] FSMState defaultState = null;
        [SerializeField] FSMState anyState = null;
        private FSMState currentState = null;
        public FSMState CurrentState => currentState;

        private bool isActived = false;
        private bool isInitialized = false;

        public virtual void Initialize()
        {
            aiDataContainer.Initialize();

            List<FSMState> states = new List<FSMState>();
            transform.GetComponentsInChildren<FSMState>(states);
            states.ForEach(i => i.Init(this));

            isInitialized = true;
        }

        protected virtual void Update()
        {
            if(isInitialized == false)
                return;

            if(isActived == false)
                return;

            if(currentState != null)
                currentState.UpdateState();

            if(anyState != null)
                anyState.UpdateState();
        }

        public void SetAsDefaultState()
        {
            ChangeState(defaultState);
        }

        public void ChangeState(FSMState targetState)
        {
            OnStateChangedEvent?.Invoke(currentState, targetState);

            if(currentState != null)
                currentState.ExitState();

            currentState = targetState;

            if(currentState != null)
                currentState.EnterState();
        }

        public void SetActive(bool isActive)
        {
            isActived = isActive;
        }

        public T GetAIData<T>() where T : class, IAIData
        {
            return aiDataContainer.GetAIData<T>();
        }
    }
}