using System.Collections.Generic;
using UnityEngine;
using H00N.Extensions;

namespace H00N.AI.FSM
{
    public class FSMTransitionGroup : FSMTransition
    {
        private List<FSMTransition> transitions = null;

        public override void Init(FSMBrain brain, FSMState state)
        {
            base.Init(brain, state);

            transitions = new List<FSMTransition>(transform.childCount);
            transform.GetComponentsInChildren<FSMTransition>(transitions, false, false);
            transitions.ForEach(i => i.Init(brain, state));

            if(transitions.Count <= 0)
                Debug.LogWarning("[FSM] There are no child transitions in the transition group.");
        }

        public override void EnterState()
        {
            base.EnterState();
            transitions.ForEach(i => i.EnterState());
        }

        public override void ExitState()
        {
            base.ExitState();
            transitions.ForEach(i => i.ExitState());
        }

        public override bool Transition()
        {
            foreach (FSMTransition transition in transitions)
            {
                if (transition.CheckDecisions())
                {
                    transition.Transition();
                    return true;
                }
            }

            return false;
        }
    }
}