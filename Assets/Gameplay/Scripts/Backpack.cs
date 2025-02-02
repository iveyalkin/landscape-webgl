using System;
using System.Collections.Generic;
using IV.Gameplay.Interactions;
using UnityEngine;

namespace IV.Gameplay
{
    [CreateAssetMenu(fileName = nameof(Backpack), menuName = "Gameplay/Backpack", order = 0)]
    public class Backpack : ScriptableObject
    {
        public event Action OnPickupAdded = delegate { };

        [SerializeField] private List<Pickup.Data> startingPickups = new();

        [field: NonSerialized] public List<Pickup.Data> Pickups { get; } = new();

        public int PickupCount => Pickups.Count;

        private void OnEnable() =>
            Pickups.AddRange(startingPickups);

        public void AddPickup(Pickup.Data pickup)
        {
            Debug.Log("Backpack: Added pickup: " + pickup.name);

            Pickups.Add(pickup);

            OnPickupAdded();
        }
        
        public bool HasPickup(Pickup.Data pickupData)
        {
            foreach (var pickup in Pickups)
                if (pickup.name == pickupData.name)
                    return true;

            return false;
        }
    }
}