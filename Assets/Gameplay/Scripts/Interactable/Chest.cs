using IV.Core.Actions;
using IV.Core.Interactions;
using IV.Gameplay.Actions;
using UnityEngine;

namespace IV.Gameplay.Interactions
{
    [RequireComponent(typeof(BaseInteractable))]
    public partial class Chest : MonoBehaviour
    {
        [SerializeField] private ActionQueue actionQueue;

        public void OnRequest()
        {
            if (!baseInteractable.AssertInteraction())
            {
                baseInteractable.ShowHint();
                return;
            }

            var destination = baseInteractable.GetInteractionPoint();
            var requestMoveAction = actionQueue.GetAction<RequestMoveAction>().SetUp(destination);
            actionQueue.QueueAction(requestMoveAction);

            var interactAction = actionQueue.GetAction<InteractAction>();
            interactAction.interactable = baseInteractable;
            actionQueue.QueueAction(interactAction, false);
        }

        public void OnOpen()
        {
            Debug.Log("Chest opened");
        }
    }
}