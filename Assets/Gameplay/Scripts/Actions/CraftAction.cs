using UnityEngine;
using IV.Core.Actions;

namespace IV.Gameplay.Actions
{
    public class CraftAction : GameAction
    {
        private string recipeId;
        private float craftingTime;

        private float startTime;

        public CraftAction()
        {
            IsInterruptible = true;
        }

        public CraftAction SetUp(string recipeId, float raftingTime = 2f)
        {
            this.recipeId = recipeId;
            this.craftingTime = raftingTime;

            return this;
        }

        public override void Start(ActionQueue _)
        {
            startTime = Time.time;
            // Start crafting animation/effects
        }

        public override void Update()
        {
            if (Time.time - startTime >= craftingTime)
            {
                // Complete crafting and spawn item
                IsCompleted = true;
            }
        }
    }
}