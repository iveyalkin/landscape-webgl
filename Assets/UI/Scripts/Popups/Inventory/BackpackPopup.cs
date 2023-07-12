using IV.Core.Pools;
using UnityEngine;

namespace IV.UI.Popups.Inventory
{
    public class BackpackPopup : BasePopup
    {
        [SerializeField] private Gameplay.Backpack backpack;

        [SerializeField] private GameObjectPool<BackpackEntry> entryPool = new();

        protected override void Initialize()
        {
            // entryPool.Initialize();
        }

        protected override void Refresh()
        {
            RefreshGrid();
            backpack.OnPickupAdded += RefreshGrid;
        }

        private void OnDisable()
        {
            backpack.OnPickupAdded -= RefreshGrid;
            entryPool.Recycle();
        }

        private void RefreshGrid()
        {
            entryPool.Recycle();

            foreach (var pickup in backpack.Pickups)
            {
                var entry = entryPool.Get();
                entry.SetData(pickup);
            }
        }
    }
}