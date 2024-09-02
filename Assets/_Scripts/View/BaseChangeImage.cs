using System;
using UnityEngine;
using UnityEngine.UI;

namespace CosmicMemory.View
{
    [RequireComponent(typeof(Toggle))]
    public abstract class BaseChangeImage : MonoBehaviour
    {
        #region Fields
        [SerializeField] private string _idBack;
        [SerializeField] private Image _background;

        protected Toggle _toggle;
        private const float _alphaLock = 0.66f;
        #endregion

        #region Properties
        public string IdBack => _idBack;
        #endregion

        #region Unity Methods
        private void OnValidate()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.transition = Selectable.Transition.None;
            if (transform.parent != null)
                _toggle.group = transform.parent.gameObject.GetComponent<ToggleGroup>();

            _background = gameObject.GetComponentInChildren<Image>();
            if (_background == null)
            {
                throw new ArgumentException("Необходимо добавить дочерний обьект Image с дочерним обьектом Image!!!");
            }
            _background.maskable = false;
        }

        protected virtual void Start()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.isOn = false;
            _toggle.onValueChanged.AddListener(ChangeToggle);
        }

        private void OnDestroy()
        {
            _toggle.onValueChanged.RemoveListener(ChangeToggle);
        }
        #endregion

        #region Public Methods
        public abstract void ChangeToggle(bool change);

        public void Switch(bool isOn)
        {
            if (_toggle == null)
            {
                _toggle = GetComponent<Toggle>();
            }
            _toggle.isOn = isOn;
        }
        #endregion
    }
}
