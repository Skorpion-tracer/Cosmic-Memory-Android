using TMPro;
using UnityEngine;
using YG.Utils.Pay;
using YG;
using System;

namespace CosmicMemory.View
{
    [RequireComponent(typeof(PurchaseYG))]
    public sealed class PriceShow : MonoBehaviour
    {
        #region Fields
        [SerializeField] private TextMeshProUGUI _textMesh;
        #endregion

        #region Unity Methods
        private void OnValidate()
        {
            _textMesh = GetComponentInChildren<TextMeshProUGUI>();
            if (_textMesh == null)
            {
                throw new ArgumentException("У данного объекта должен быть дочерний TextMeshPro!!!");
            }
        }

        private void Start()
        {
            PurchaseYG purchaseYG = GetComponent<PurchaseYG>();
            Purchase purchase = YandexGame.PurchaseByID(purchaseYG.data.id);
            _textMesh.text = purchase.price;
        }
        #endregion
    }
}
