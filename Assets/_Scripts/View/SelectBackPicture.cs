using YG;

namespace CosmicMemory.View
{
    public sealed class SelectBackPicture : BaseChangeImage
    {
        #region Public Methods
        public override void ChangeToggle(bool change)
        {
            if (YandexGame.savesData.idPictureBack.Equals(IdBack)) return;

            if (change)
            {
                YandexGame.savesData.idPictureBack = IdBack;
                YandexGame.SaveProgress();
            }

            AudioGame.instance.PlayClick();
        }
        #endregion
    }
}
