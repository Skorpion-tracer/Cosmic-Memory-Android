using DG.Tweening;
using UnityEngine;

namespace CosmicMemory.View
{
    public class ScaleBehaviour : MonoBehaviour
    {
        #region Fields
        private Tween _tween;

        [SerializeField] private Vector3 _scale = new Vector3(1.2f, 1.2f);
        [SerializeField] private float _duration = 0.2f;
        [SerializeField] private Ease _ease = Ease.InQuad;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            transform.localScale = Vector3.one;
            _tween = transform.DOScale(_scale, _duration).SetLoops(-1, LoopType.Yoyo).SetEase(_ease);
            _tween?.Play();
        }

        private void OnDisable()
        {
            _tween?.Kill();
        }
        #endregion
    }
}