using CosmicMemory.Helper;
using System;
using UnityEngine;

namespace CosmicMemory.Models
{
    [CreateAssetMenu(fileName = "Card Pictures", menuName = "GameFiled/Card Pictures", order = 52)]
    public sealed class CardPictures : ScriptableObject
    {
        #region Fields
        private Sprite _pictureBack;

        [SerializeField] private string _picDefault = "pb001";

        [Space(10f)]
        [SerializeField] private Sprite[] _pictures;

        private const string _pathBackground = "CardShirts/";
        #endregion

        #region Properties
        public Sprite[] Pictures => _pictures;
        public Sprite PictureBack => _pictureBack;
        #endregion

        #region Public Methods
        public void SetPictureBack()
        {
            _pictureBack = Resources.Load<Sprite>(_pathBackground + SaveHelper.savesData.idPictureBack);
            if (_pictureBack == null)
            {
                _pictureBack = Resources.Load<Sprite>(_pathBackground + _picDefault);
                throw new NullReferenceException("Не найден путь к спрайту из ресурсов");
            }
        }
        #endregion
    }
}
