using IV.Core.Actions;
using IV.Core.Interactions;
using UnityEngine;

namespace IV.Gameplay.Actions
{
    public class InteractAction : GameAction
    {
        public IInteractable interactable;

        private float startTime;

        public InteractAction()
        {
            IsInterruptible = false;
        }

        public override void Start(ActionQueue _)
        {
            startTime = interactable.InteractionTime;
            interactable.StartInteraction();
        }

        public override void Update()
        {
            startTime -= Time.deltaTime;
            if (startTime > 0f) return;

            IsCompleted = true;
            interactable.CompleteInteraction();
        }

        public override void Cancel()
        {
            base.Cancel();
            interactable.CancelInteraction();
        }

        public override void Recycle(ActionQueue aq)
        {
            interactable = null;
            base.Recycle(aq);
        }
    }
}