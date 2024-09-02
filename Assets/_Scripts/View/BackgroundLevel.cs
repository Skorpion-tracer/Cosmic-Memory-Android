using CosmicMemory.Helper;
using System.Linq;
using UnityEngine;

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
            SetSprite(SaveHelper.savesData.idBackground);
        }
        #endregion

        #region Public Methods
        public void SwitchSprite(string idBack)
        {
            SetSprite(idBack);

            SaveHelper.savesData.idBackground = idBack;
            SaveHelper.SaveData();
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
                SpriteRenderer currentSprite = _backgrounds.FirstOrDefault(e => e.sprite.name == SaveHelper.savesData.idBackground);
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
