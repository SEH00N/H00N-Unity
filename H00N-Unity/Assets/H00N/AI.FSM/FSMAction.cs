using UnityEngine;

namespace H00N.AI.FSM
{
    public abstract class FSMAction : MonoBehaviour
    {
        protected FSMBrain brain;
        protected FSMState state;

        public virtual void Init(FSMBrain brain, FSMState state)
        {
            this.brain = brain;
            this.state = state;
        }

        public virtual void EnterState() { }
        public virtual void UpdateState() { }
        public virtual void ExitState() { }
    }
}