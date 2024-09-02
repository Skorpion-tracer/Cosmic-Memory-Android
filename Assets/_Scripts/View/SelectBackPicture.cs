using CosmicMemory.Helper;

namespace CosmicMemory.View
{
    public sealed class SelectBackPicture : BaseChangeImage
    {
        #region Public Methods
        public override void ChangeToggle(bool change)
        {
            if (SaveHelper.savesData.idPictureBack.Equals(IdBack)) return;

            if (change)
            {
                SaveHelper.savesData.idPictureBack = IdBack;
                SaveHelper.SaveData();
            }

            AudioGame.instance.PlayClick();
        }
        #endregion
    }
}
