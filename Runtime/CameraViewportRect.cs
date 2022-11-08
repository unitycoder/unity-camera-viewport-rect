using UnityEngine;
using UnityEngine.EventSystems;

namespace Gilzoide.RectTransformCameraViewport
{
    [RequireComponent(typeof(RectTransform)), ExecuteAlways]
    public class CameraViewportRect : UIBehaviour
    {
        [SerializeField] protected Camera _camera;

        [Tooltip("Toggle Camera.enabled when this script gets enabled/disabled")]
        public bool ToggleCameraEnabled = true;

        public Camera Camera
        {
            get => _camera;
            set
            {
                _camera = value;
                Refresh();
            }
        }

        protected Canvas _canvas;
        protected Vector3[] _worldCorners = new Vector3[4];

        public void Refresh()
        {
            if (Camera && _canvas)
            {
                Camera.pixelRect = GetScreenRect();
            }
        }

        protected virtual void Update()
        {
            if (transform.hasChanged)
            {
                Refresh();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _canvas = FindRootCanvas();
            if (ToggleCameraEnabled && Camera)
            {
                Camera.enabled = true;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (ToggleCameraEnabled && Camera)
            {
                Camera.enabled = false;
            }
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            if (isActiveAndEnabled)
            {
                _canvas = FindRootCanvas();
                Refresh();
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (isActiveAndEnabled)
            {
                Refresh();
            }
        }
#endif

        protected Rect GetScreenRect()
        {
            ((RectTransform) transform).GetWorldCorners(_worldCorners);

            Vector3 bottomLeft = _worldCorners[0];
            Vector3 topRight = _worldCorners[2];
            if (_canvas.renderMode == RenderMode.ScreenSpaceCamera && _canvas.worldCamera != null)
            {
                Camera camera = _canvas.worldCamera;
                bottomLeft = camera.WorldToScreenPoint(bottomLeft);
                topRight = camera.WorldToScreenPoint(topRight);
            }
            return Rect.MinMaxRect(bottomLeft.x, bottomLeft.y, topRight.x, topRight.y);
        }

        protected Canvas FindRootCanvas()
        {
            Canvas canvas = GetComponentInParent<Canvas>(includeInactive: true);
            return canvas != null ? canvas.rootCanvas : null;
        }
    }
}