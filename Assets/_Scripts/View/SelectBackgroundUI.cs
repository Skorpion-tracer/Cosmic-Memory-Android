using UnityEngine;

namespace CosmicMemory.View
{
    public sealed class SelectBackgroundUI : BaseChangeImage
    {
        #region Fields
        private BackgroundLevel _backgroundLevel;
        #endregion

        #region Unity Methods
        protected override void Start()
        {
            base.Start();
            _backgroundLevel = FindFirstObjectByType<BackgroundLevel>();
            Debug.Log($"BackgroundLevel - {_backgroundLevel.name}");
        }
        #endregion

        #region Public Methods
        public override void ChangeToggle(bool change)
        {
            if (change)
            {
                if (_backgroundLevel == null)
                {
                    _backgroundLevel = FindFirstObjectByType<BackgroundLevel>();
                    Debug.Log($"BackgroundLevel был равен Null, переприсвоили - {_backgroundLevel.name}");
                }

                Debug.Log($"BackgroundLevel - {_backgroundLevel.name}");

                _backgroundLevel?.SwitchSprite(IdBack);
                AudioGame.instance.PlayClick();
            }
        }
        #endregion
    }
}