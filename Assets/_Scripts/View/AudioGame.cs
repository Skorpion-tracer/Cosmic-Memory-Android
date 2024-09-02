using CosmicMemory.Helper;
using UnityEngine;

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

        private float _delayWinsPlay = 1.2f;
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

            _music.mute = !SaveHelper.savesData.isOnMusic;
        }
        #endregion

        #region Public Methods
        public void PlayWins()
        {
            if (SaveHelper.savesData.isOnSounds)
            {
                _wins.PlayDelayed(_delayWinsPlay);
            }
        }

        public void PlayClick()
        {
            if (SaveHelper.savesData.isOnSounds)
            {
                _click.Play();
            }
        }

        public void OnOffSounds()
        {
            SaveHelper.savesData.isOnSounds = !SaveHelper.savesData.isOnSounds;
            SaveHelper.SaveData();
        }

        public void OnOffMusic()
        {
            _music.mute = !_music.mute;
            SaveHelper.savesData.isOnMusic = !_music.mute;
            SaveHelper.SaveData();
        }
        #endregion
    }
}