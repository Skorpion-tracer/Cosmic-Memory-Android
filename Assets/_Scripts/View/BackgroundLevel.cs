using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using YG;

namespace CosmicMemory.View
{
    public sealed class BackgroundLevel : MonoBehaviour
    {
        #region Fields
        [SerializeField] private SpriteRenderer[] _backgrounds;
        #endregion

        #region Unity Methods
        private void Start()
        {
            SetSprite(YandexGame.savesData.idBackground);
        }
        #endregion

        #region Public Methods
        public void SwitchSprite(string idBack)
        {
            SetSprite(idBack);
            YandexGame.savesData.idBackground = idBack;
            YandexGame.SaveProgress();
        }
        #endregion

        #region Private Methods
        private bool SetSprite(string idBack)
        {
            foreach (SpriteRenderer sprite in _backgrounds)
            {
                sprite.gameObject.SetActive(false);
            }

            SpriteRenderer targetSprite = _backgrounds.FirstOrDefault(e => e.sprite.name == idBack);

            if (targetSprite == null)
            {
                Debug.Log("Не найден спрайт");
                SpriteRenderer currentSprite = _backgrounds.FirstOrDefault(e => e.sprite.name == YandexGame.savesData.idBackground);
                currentSprite.gameObject.SetActive(true);

                Debug.Log($"Установлен последний активный спрайт: {currentSprite.sprite.name}");
                return false;
            }
            else
            {
                Debug.Log($"Найден спрайт: {targetSprite.sprite.name}");
                targetSprite.gameObject.SetActive(true);
                return true;
            }
        }
        #endregion
    }
}
