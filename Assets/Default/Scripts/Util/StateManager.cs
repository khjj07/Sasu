using UnityEngine;

namespace Default.Scripts.Util
{
    public abstract class StateManager<T> : MonoBehaviour where T : State<T>
    {
       [SerializeField]
        private T currentState;

        public virtual void Start()
        {
            if (currentState != null)
            {
                Change(currentState);
            }
        }

        public virtual void Next()
        {
          
            if (currentState.next != null)
            {
                Change(currentState.next);
            }
        }

        public virtual void Previous()
        {
            if (currentState.previous != null)
            {
                Change(currentState.previous);
            }
        }

        public virtual void Change(T state)
        {
            currentState.OnInactive();
            currentState = state;
            currentState.OnActive();
        }
    }
}
