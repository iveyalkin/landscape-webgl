using System.Collections.Generic;
using System.Reflection;
using IV.Core.URP;
using IV.Core.URP.FullscreenBlur;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace IV.UI
{
    public class RaycastInterceptor : Graphic, IPointerClickHandler
    {
        private int refCounter;

        [SerializeField] private UnityEvent OnClick;
        [SerializeField] private GraphicsController graphicsController;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            // todo calculate and prepare fullscreen mesh
            // Clear the mesh - we don't want to render anything
            vh.Clear();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }

        public void Deactivate()
        {
            refCounter = Mathf.Max(0, refCounter - 1);

            if (refCounter > 0) return;
            if (!gameObject.activeSelf) return;

            gameObject.SetActive(false);
        }

        public void Activate()
        {
            refCounter = 0;
            if (gameObject.activeSelf) return;

            gameObject.SetActive(true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            graphicsController.SetFullscreenBlur(true);
        }

        protected override void OnDisable()
        {
            graphicsController.SetFullscreenBlur(false);
            base.OnDisable();
        }
    }
}