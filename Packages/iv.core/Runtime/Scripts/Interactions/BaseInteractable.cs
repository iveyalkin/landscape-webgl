using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace IV.Core.Interactions
{
    [RequireComponent(typeof(Collider))]
    public partial class BaseInteractable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
        IPointerClickHandler, IInteractable
    {
        [SerializeField] private GameObject revealEffect;
        [SerializeField] private GameObject pickupEffect;
        [SerializeField] private Transform interactionPoint;

        [SerializeField] private UnityEvent onInteractionRequest;
        [SerializeField] private UnityEvent onInteractionComplete;
        [SerializeField] private UnityEvent onInteractionCancel;

        [SerializeField] private Condition[] conditions;

        [field: Min(0.1f)]
        [field: SerializeField]
        public float InteractionTime { get; private set; } = 1f;

        protected virtual void OnEnable()
        {
            ResetUI();
        }

        protected virtual void OnDisable()
        {
            ResetUI();
        }

        private void ResetUI()
        {
            if (revealEffect)
            {
                revealEffect.SetActive(false);
            }

            if (pickupEffect)
            {
                pickupEffect.SetActive(false);
            }
        }

        public Vector3 GetInteractionPoint()
        {
            return interactionPoint.position;
        }

        public bool AssertInteraction()
        {
            if (conditions == null)
                return true;

            foreach (var condition in conditions)
            {
                if (!condition.Validate())
                    return false;
            }

            return true;
        }

        public void StartInteraction()
        {
            if (!AssertInteraction()) return;

            Debug.Log($"{gameObject.name}: Interaction Started");

            if (pickupEffect)
            {
                pickupEffect.SetActive(true);
            }
        }

        public void CompleteInteraction()
        {
            Debug.Log($"{gameObject.name}: Interaction Completed");

            // disable component to prevent further interaction
            enabled = false;

            onInteractionComplete.Invoke();
        }

        public void CancelInteraction()
        {
            Debug.Log($"{gameObject.name}: Interaction Cancelled");

            if (pickupEffect)
            {
                pickupEffect.SetActive(false);
            }

            onInteractionCancel.Invoke();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log($"{gameObject.name}: Pointer Enter");

            if (revealEffect)
            {
                revealEffect.SetActive(true);
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            Debug.Log($"{gameObject.name}: Pointer Exit");

            if (revealEffect)
            {
                revealEffect.SetActive(false);
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"{gameObject.name}: Pointer Click");

            if (revealEffect)
            {
                revealEffect.SetActive(false);
            }

            onInteractionRequest.Invoke();
        }

        public void ShowHint()
        {
            Debug.Log("Insufficient conditions to interact");
            // todo: show hint with requirements to interact
        }
    }
}