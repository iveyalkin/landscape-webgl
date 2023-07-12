using IV.Core.Actions;
using UnityEngine;

namespace IV.Gameplay.Actions
{
    public class RequestMoveAction : GameAction
    {
        private Vector3 destination;

        private NavigationSurface navigationSurface;
        private MoveAction moveAction;

        public RequestMoveAction()
        {
            IsInterruptible = true;
        }

        public RequestMoveAction SetUp(Vector3 destination)
        {
            this.destination = destination;

            return this;
        }

        public override void Start(ActionQueue actionQueue)
        {
            if (navigationSurface == null)
                navigationSurface = Object.FindFirstObjectByType<NavigationSurface>();

            moveAction = navigationSurface.RequestMove(destination);
            moveAction.Start(actionQueue);
        }

        public override void Update()
        {
            moveAction.Update();
            IsCompleted = moveAction.IsCompleted;
        }
        
        public override void Cancel()
        {
            moveAction.Cancel();
            base.Cancel();
        }

        public override void Recycle(ActionQueue aq)
        {
            aq.RecycleAction(moveAction);
            moveAction = null;
            base.Recycle(aq);
        }
    }
}