using UnityEngine;

namespace IV.UI.Popups
{
    public abstract class BasePopup : MonoBehaviour
    {
        [SerializeField] private RaycastInterceptor raycastInterceptor;

        public bool IsShown => gameObject.activeSelf;

        private void Awake()
        {
            Initialize();
            Close();
        }

        private void OnEnable() => Refresh();

        protected virtual void Initialize()
        {
            // no op
        }

        protected abstract void Refresh();

        public void Toggle()
        {
            if (IsShown)
                Close();
            else
                Open();
        }

        public void Close()
        {
            if (!IsShown)
                return;

            gameObject.SetActive(false);
            raycastInterceptor.Deactivate();
        }

        public void Open()
        {
            if (IsShown)
                return;

            raycastInterceptor.Activate();
            gameObject.SetActive(true);
        }
    }
}