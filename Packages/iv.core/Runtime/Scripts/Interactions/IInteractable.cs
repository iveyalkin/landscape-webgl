using UnityEngine;

namespace IV.Core.Interactions
{
    public interface IInteractable
    {
        float InteractionTime { get; }

        Vector3 GetInteractionPoint();
        void StartInteraction();
        void CompleteInteraction();
        void CancelInteraction();
    }
}