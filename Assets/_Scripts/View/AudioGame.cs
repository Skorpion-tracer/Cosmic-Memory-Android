using UnityEngine;
using YG;

namespace CosmicMemory.View
{
    public sealed class AudioGame : MonoBehaviour
    {
        #region Singleton
        public static AudioGame instance;
        #endregion

        #region Fields
        [SerializeField] private AudioSource _wins;
        [SerializeField] private AudioSource _click;
        [SerializeField] private AudioSource _music;

        private float _delaywinsPlay = 1.2f;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            _music.mute = !YandexGame.savesData.isOnMusic;
        }
        #endregion

        #region Public Methods
        public void PlayWins()
        {
            if (YandexGame.savesData.isOnSounds)
            {
                _wins.PlayDelayed(_delaywinsPlay);
            }
        }

        public void PlayClick()
        {
            if (YandexGame.savesData.isOnSounds)
            {
                _click.Play();
            }
        }

        public void OnOffSounds()
        {
            YandexGame.savesData.isOnSounds = !YandexGame.savesData.isOnSounds;
            YandexGame.SaveProgress();
        }

        public void OnOffMusic()
        {
            _music.mute = !_music.mute;
            YandexGame.savesData.isOnMusic = !_music.mute;
            YandexGame.SaveProgress();
        }
        #endregion
    }
}