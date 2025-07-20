using UnityEngine;

namespace H00N.AI.FSM
{
    public abstract class FSMDecision : MonoBehaviour
    {
        [SerializeField] protected bool isReverse = false;
        public bool IsReverse => isReverse;

        protected FSMBrain brain = null;
        protected FSMState state = null;

        public virtual void Init(FSMBrain brain, FSMState state)
        {
            this.brain = brain;
            this.state = state;
        }

        public virtual void EnterState() { }
        public virtual void ExitState() { }

        public abstract bool MakeDecision();
    }
}