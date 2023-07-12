using IV.Core.Actions;
using IV.Gameplay.Actions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace IV.Gameplay
{
    public class NavigationSurface : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private ActionQueue actionQueue;

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            var destination = eventData.pointerCurrentRaycast.worldPosition;
            var action = RequestMove(destination);
            actionQueue.QueueAction(action);
        }

        public MoveAction RequestMove(Vector3 destination)
        {
            var action = actionQueue.GetAction<MoveAction>();
            action.SetUp(destination, navMeshAgent);
            return action;
        }
    }
}