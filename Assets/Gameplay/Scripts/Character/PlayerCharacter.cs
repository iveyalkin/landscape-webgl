using System;
using UnityEngine;
using UnityEngine.AI;

namespace IV.Gameplay.Character
{
    public class PlayerCharacter : MonoBehaviour
    {
        private static readonly int velocityParam = Animator.StringToHash("velocity");

        [SerializeField]
        private Backpack backpack;

        [SerializeField]
        private NavMeshAgent navMeshAgent;

        [SerializeField]
        private Animator animator;

        private void Update()
        {
            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            var velocity = navMeshAgent.velocity;
            animator.SetFloat(velocityParam, velocity.magnitude);
        }
    }
}