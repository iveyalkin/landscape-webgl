using System;
using IV.Gameplay.Actions;
using IV.Core.Actions;
using IV.Core.Interactions;
using UnityEngine;

namespace IV.Gameplay.Interactions
{
    [RequireComponent(typeof(BaseInteractable))]
    public partial class Pickup : MonoBehaviour
    {
        [SerializeField] private ActionQueue actionQueue;
        [SerializeField] private Backpack backpack;

        public Data data;

        public void OnRequest()
        {
            var destination = baseInteractable.GetInteractionPoint();
            var requestMoveAction = actionQueue.GetAction<RequestMoveAction>().SetUp(destination);
            actionQueue.QueueAction(requestMoveAction);

            var interactAction = actionQueue.GetAction<InteractAction>();
            interactAction.interactable = baseInteractable;
            actionQueue.QueueAction(interactAction, false);
        }

        public void OnPickUp() => backpack.AddPickup(data);

        [Serializable]
        public struct Data
        {
            public string name;
            public Sprite icon;
            public int value;
        }
    }
}