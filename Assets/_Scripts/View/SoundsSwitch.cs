using CosmicMemory.Helper;
using UnityEngine;
using UnityEngine.UI;

namespace CosmicMemory.View
{
    [RequireComponent(typeof(Image))]
    public sealed class SoundsSwitch : MonoBehaviour
    {
        private enum TypeSound : byte
        {
            Sounds,
            Music
        }

        #region Fields
        [SerializeField] private Sprite _imgOn;
        [SerializeField] private Sprite _imgOff;
        [SerializeField] private TypeSound _typeSound = TypeSound.Sounds;

        private Image _imgView;
        #endregion

        #region Properties
        private bool IsSound => SaveHelper.savesData.isOnSounds;
        private bool IsMusic => SaveHelper.savesData.isOnMusic;
        #endregion

        #region Unity Methods
        private void Start()
        {
            _imgView = GetComponent<Image>();
            SetView();
        }
        #endregion

        #region Public Methods
        public void SwitchMusic()
        {
            switch (_typeSound)
            {
                case TypeSound.Sounds:
                    AudioGame.instance.OnOffSounds();
                    break;
                case TypeSound.Music:
                    AudioGame.instance.OnOffMusic();
                    break;
            }

            SetView();
        }
        #endregion

        #region Private Methods
        private void SetView()
        {
            switch (_typeSound)
            {
                case TypeSound.Sounds:
                    _imgView.sprite = IsSound ? _imgOn : _imgOff;
                    break;
                case TypeSound.Music:
                    _imgView.sprite = IsMusic ? _imgOn : _imgOff;
                    break;
            }
        }
        #endregion
    }
}
