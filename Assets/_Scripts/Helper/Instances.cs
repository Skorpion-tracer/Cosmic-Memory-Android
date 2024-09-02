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
            return string.Empty; //YandexGame.savesData.idBackground; //TODO переделать на сохранения
        }

        private static string GetBackImage(SelectBackPicture _)
        {
            return string.Empty; //YandexGame.savesData.idPictureBack; //TODO переделать на сохранения
        }
    }
}
