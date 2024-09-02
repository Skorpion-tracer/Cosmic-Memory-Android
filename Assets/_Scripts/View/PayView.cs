using CosmicMemory.Helper;
using System;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace CosmicMemory.View
{
    public sealed class PayView : MonoBehaviour
    {
        #region Fields
        [SerializeField] private Button _btnPay;
        [SerializeField] private RawImage _lock;
        #endregion

        #region Properties
        public Button BtnPay => _btnPay;
        public RawImage Lock => _lock;
        #endregion
    }
}