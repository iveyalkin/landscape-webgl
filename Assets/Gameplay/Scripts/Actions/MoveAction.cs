using UnityEngine;
using UnityEngine.AI;
using IV.Core.Actions;

namespace IV.Gameplay.Actions
{
    public class MoveAction : GameAction
    {
        private NavMeshAgent agent;
        private Vector3 destination;

        private const float arrivalDistance = 0.1f;

        public MoveAction()
        {
            IsInterruptible = true;
        }

        public MoveAction SetUp(Vector3 destination, NavMeshAgent agent)
        {
            this.agent = agent;
            this.destination = destination;

            return this;
        }

        public override void Start(ActionQueue actionQueue)
        {
            agent.SetDestination(destination);
        }

        public override void Update()
        {
            if (!agent.pathPending && agent.remainingDistance < arrivalDistance)
            {
                IsCompleted = true;
            }
        }

        public override void Cancel()
        {
            base.Cancel();
            // Could add stopping the agent here if needed
        }

        public override void Recycle(ActionQueue aq)
        {
            agent = null;
            base.Recycle(aq);
        }
    }
}