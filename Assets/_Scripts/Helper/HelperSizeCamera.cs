using UnityEngine;

namespace CosmicMemory.Helper
{
    public static class HelperSizeCamera
    {
        #region Fields
        private static Vector2 _referenceResolution = new(1920, 1080);
        #endregion

        #region Public Methods
        public static void ResizeCamera(Camera camera)
        {
            float targetAspect = _referenceResolution.x / _referenceResolution.y;
            float initialSize = camera.orthographicSize;

            if (targetAspect < camera.aspect)
            {
                camera.orthographicSize = initialSize * (targetAspect / camera.aspect) / 1f;
            }
            else
            {
                camera.orthographicSize = initialSize / 1f;
            }
        }
        #endregion
    }
}
