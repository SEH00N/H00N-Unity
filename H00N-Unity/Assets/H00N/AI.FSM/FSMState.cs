using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace H00N.AI.FSM
{
    public class FSMState : MonoBehaviour
    {
        [SerializeField] bool autoTransitioning = true;

        [Space(15f)]
        [SerializeField] UnityEvent onStateEnterEvent = null;
        [SerializeField] UnityEvent onStateExitEvent = null;

        protected FSMBrain brain;

        private List<FSMAction> actions = null;
        private FSMTransitionGroup rootTransitionGroup = null;

        public void Init(FSMBrain brain)
        {
            this.brain = brain;

            actions = new List<FSMAction>();
            GetComponents<FSMAction>(actions);
            actions.ForEach(i => i.Init(brain, this));

            rootTransitionGroup = GetComponent<FSMTransitionGroup>();
            if(rootTransitionGroup != null)
                rootTransitionGroup.Init(brain, this);

            if (autoTransitioning && rootTransitionGroup == null)
                Debug.LogWarning("[FSM] Auto transitioning is set for this state. but no root transition group exist.");
        }

        public void EnterState()
        {
            actions.ForEach(i => i.EnterState());
            if (autoTransitioning)
                rootTransitionGroup?.EnterState();

            onStateEnterEvent?.Invoke();
        }

        public void UpdateState()
        {
            if (autoTransitioning)
            {
                if(CheckTransition())
                    return;
            }

            actions.ForEach(i => i.UpdateState());
        }

        public void ExitState()
        {
            actions.ForEach(i => i.ExitState());
            if (autoTransitioning)
                rootTransitionGroup?.EnterState();

            onStateExitEvent?.Invoke();
        }

        private bool CheckTransition()
        {
            if (rootTransitionGroup == null)
            {
                Debug.LogWarning("[FSM] Auto transitioning is set for this state. but no root transition group exist.");
                return false;
            }

            if (rootTransitionGroup.CheckDecisions() == false)
                return false;

            if (rootTransitionGroup.Transition() == false)
                return false;

            return true;
        }
    }
}