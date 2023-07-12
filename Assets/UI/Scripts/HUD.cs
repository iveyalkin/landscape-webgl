using IV.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace IV.UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private Backpack backpack;

        [SerializeField] private Text pickupCounter;
    
        [SerializeField] private Stats stats;

        private void OnEnable()
        {
            backpack.OnPickupAdded += OnPickupAdded;
            InvalidatePickupCount();
        }

        private void OnPickupAdded()
        {
            Debug.Log("HUD: Pickup added");

            InvalidatePickupCount();
        }

        private void InvalidatePickupCount()
        {
            pickupCounter.text = $"{backpack.PickupCount.ToString()} pcs.";
        }
    }
}