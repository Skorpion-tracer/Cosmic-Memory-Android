using CosmicMemory.View;

namespace CosmicMemory.Helper
{
    public static class Instances
    {
        public static string GetBackImage(BaseChangeImage changeImage)
        {
            return changeImage is SelectBackgroundUI sb ? GetBackImage(sb) : GetBackImage((SelectBackPicture)changeImage);
        }

        private static string GetBackImage(SelectBackgroundUI _)
        {
            return SaveHelper.savesData.idBackground;
        }

        private static string GetBackImage(SelectBackPicture _)
        {
            return SaveHelper.savesData.idPictureBack;
        }
    }
}
