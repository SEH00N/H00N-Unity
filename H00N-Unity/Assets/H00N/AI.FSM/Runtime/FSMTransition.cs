using System.Collections.Generic;
using UnityEngine;

namespace H00N.AI.FSM
{
    public abstract class FSMTransition : MonoBehaviour
    {
        protected FSMBrain brain = null;
        protected FSMState state = null;

        private List<FSMDecision> decisions = null;

        public virtual void Init(FSMBrain brain, FSMState state)
        {
            this.brain = brain;
            this.state = state;
            
            decisions = new List<FSMDecision>();
            GetComponents<FSMDecision>(decisions);
            decisions.ForEach(i => i.Init(brain, state));
        }

        public virtual void EnterState()
        {
            decisions.ForEach(i => i.EnterState());
        }

        public virtual void ExitState()
        {
            decisions.ForEach(i => i.ExitState());
        }

        public bool CheckDecisions()
        {
            bool condition = true;

            foreach (FSMDecision decision in decisions)
            {
                condition = decision.MakeDecision();
                if (decision.IsReverse)
                    condition = !condition;
                if (condition == false)
                    break;
            }

            return condition;
        }

        public abstract bool Transition();
    }
}