using CosmicMemory.Helper;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace CosmicMemory.View
{
    [RequireComponent(typeof(Toggle))]
    public abstract class BaseChangeImage : MonoBehaviour
    {
        #region Fields
        [SerializeField] private string _idBack;
        [SerializeField] private TypeSwitchImage _typeSwitchImage = TypeSwitchImage.Free;
        [SerializeField] private PayView _payView;
        [SerializeField] private Image _background;

        protected Toggle _toggle;
        private PurchaseYG _purchaseYG;
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

            if (_typeSwitchImage == TypeSwitchImage.Paid)
            {
                _purchaseYG = gameObject.GetComponent<PurchaseYG>();

                if (_purchaseYG == null)
                {
                    throw new ArgumentException("Необходимо добавить компонент PurchaseYG для совершения покупки!!!");
                }

                _purchaseYG.data.id = _idBack;
                _purchaseYG.showCurrencyCode = true;

                TextMeshProUGUI priceText = _payView.BtnPay.GetComponentInChildren<TextMeshProUGUI>();
                _purchaseYG.textMP.priceValue = priceText;
            }
        }

        protected virtual void Start()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.isOn = false;
            _toggle.onValueChanged.AddListener(ChangeToggle);

            CheckPaid();
        }

        private void OnEnable()
        {
            CheckPaid();
        }

        private void OnDestroy()
        {
            if (_payView != null)
                _payView.BtnPay.onClick.RemoveAllListeners();

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

        public void SuccessPurchase()
        {
            _toggle.interactable = true;
            _payView.BtnPay.gameObject.SetActive(false);
            _payView.Lock.gameObject.SetActive(false);
            _payView.BtnPay.onClick.RemoveListener(_purchaseYG.BuyPurchase);
            _background.color = new Color(_background.color.r, _background.color.g, _background.color.b, 255f);
            YandexGame.savesData.AddBuy(_purchaseYG.data.id);
        }
        #endregion

        #region Private Methods
        private void SetParamsForPay()
        {
            if (_toggle == null) return;

            _toggle.interactable = false;
            if (!YandexGame.auth)
            {
                _payView.Lock.gameObject.SetActive(true);
                _payView.BtnPay.gameObject.SetActive(false);
            }
            else
            {
                _payView.BtnPay.gameObject.SetActive(true);
                _payView.Lock.gameObject.SetActive(false);
                _payView.BtnPay.onClick.AddListener(_purchaseYG.BuyPurchase);
                _payView.BtnPay.onClick.AddListener(AudioGame.instance.PlayClick);
            }

            _background.color = new Color(_background.color.r, _background.color.g, _background.color.b, _alphaLock);
        }

        private void CheckPaid()
        {
            Debug.Log($"{_typeSwitchImage}; id: {_idBack}");

            if (_typeSwitchImage == TypeSwitchImage.Paid)
            {
                if (_purchaseYG == null)
                {
                    Debug.Log($"Обьект покупки не найден: {_idBack}. Ищем компонент снова");
                    _purchaseYG = GetComponent<PurchaseYG>();
                    if (_purchaseYG == null)
                    {
                        Debug.Log($"Обьект не найден: {_idBack}");
                        gameObject.SetActive(false);
                        return;
                    }
                    //_purchaseYG.UpdateEntries();
                    Debug.Log($"Найдена покупка: {_purchaseYG.data.id}|{_purchaseYG.data.title}; Consumed: {_purchaseYG.data.consumed}; Price: {_purchaseYG.data.price}");
                }

                bool consumed;
                consumed = YandexGame.savesData.BuyContains(_purchaseYG.data.id);

#if UNITY_EDITOR
                consumed = _purchaseYG.data.consumed;
#endif

                if (!consumed)
                {
                    SetParamsForPay();
                }
            }
        }
        #endregion
    }
}
