using CosmicMemory.View;
using YG;

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
            return YandexGame.savesData.idBackground;
        }

        private static string GetBackImage(SelectBackPicture _)
        {
            return YandexGame.savesData.idPictureBack;
        }
    }
}
