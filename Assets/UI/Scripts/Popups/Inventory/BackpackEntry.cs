using IV.Gameplay.Interactions;
using UnityEngine;
using UnityEngine.UI;

namespace IV.UI.Popups.Inventory
{
    public class BackpackEntry : MonoBehaviour
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Text countText;
        [SerializeField] private Image iconImage;

        public void SetData(Pickup.Data pickup)
        {
            nameText.text = pickup.name;
            countText.text = pickup.value.ToString();

            if (pickup.icon != null)
                iconImage.sprite = pickup.icon;
            else
                Debug.LogWarning($"No icon found for {pickup.name}");
        }
    }
}