using SolarAR.Planets;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SolarAR.UI
{
    public sealed class PlanetPreviewDragSurface : MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        [SerializeField] private PlanetPreviewController _previewController;
        [SerializeField, Min(0.01f)] private float _sensitivity = 0.18f;

        public void OnBeginDrag(PointerEventData eventData)
        {
            _previewController?.BeginPreviewInteraction();
        }

        public void OnDrag(PointerEventData eventData)
        {
            _previewController?.RotatePreview(eventData.delta * _sensitivity);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _previewController?.EndPreviewInteraction();
        }

        private void OnDisable()
        {
            _previewController?.EndPreviewInteraction();
        }
    }
}
