using UnityEngine;

namespace IV.Core.Interactions
{
    public interface IInteractable
    {
        float InteractionTime { get; }

        Vector3 GetInteractionPoint();
        bool AssertInteraction();
        void StartInteraction();
        void CompleteInteraction();
        void CancelInteraction();
    }
}