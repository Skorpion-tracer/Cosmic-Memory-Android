using System.IO;
using UnityEngine;

namespace CosmicMemory.Helper
{
    public static class SaveHelper
    {
        #region Fields
        private const string _pathSaves = "Saves/SaveCosmicMemory.json";
        private static readonly string _path;

        public static Saves savesData;
        #endregion

        #region Contructor
        static SaveHelper()
        {
            savesData = new();

            string dataPath;

#if UNITY_EDITOR
            dataPath = Application.dataPath;
#else
            dataPath = Application.persistentDataPath;
#endif

            _path = $"{dataPath}/{_pathSaves}";

            string directory = Path.GetDirectoryName(_path);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (!File.Exists(_path))
                File.Create(_path);
        }
#endregion

        #region Public Methods
        public static void LoadData()
        {
            if (File.Exists(_path))
            {
                string json = File.ReadAllText(_path);
                    if (string.IsNullOrEmpty(json)) return;
                savesData = JsonUtility.FromJson<Saves>(json);
            }
        }

        public static void SaveData()
        {
            string json = JsonUtility.ToJson(savesData, true);

            File.WriteAllText(_path, json);
        }
        #endregion
    }
}
