using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace AM_i18n.Scripts.Core
{
    public class TextDataIOUtility
    {
        private static string SAVE_FOLDER_KEY => $"{Application.productName}_SaveFolder";

        public static string GetSaveFolderPath()
        {
#if UNITY_EDITOR
            return UnityEditor.EditorPrefs.GetString(SAVE_FOLDER_KEY);
#else
            return PlayerPrefs.GetString(SAVE_FOLDER_KEY);
#endif
        }

        public static void SetSaveFolderPath(string path)
        {
            PlayerPrefs.SetString(SAVE_FOLDER_KEY, path);
#if UNITY_EDITOR
            UnityEditor.EditorPrefs.SetString(SAVE_FOLDER_KEY, path);
#endif
        }

        private int _currentGameLanguageIndex = default;
        public string[] EntryLanguages { get; private set; } = null;
        public string JsonContentFolderPath => Path.Combine(Application.streamingAssetsPath, "Localization");
        public string FilePath(string sourcePath) => Path.Combine(sourcePath, $"{GetCurrentLanguage}.json");
        public string GetCurrentLanguage
        {
            get
            {
                if (EntryLanguages != null && EntryLanguages.Length > _currentGameLanguageIndex)
                {
                    return EntryLanguages[_currentGameLanguageIndex];
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Ex. new TextDataIOUtility("en_US");
        /// </summary>
        /// <param name="currentGameLanguage"></param>
        public TextDataIOUtility(string currentGameLanguage = default)
        {
            EntryLanguages = LanguageEntryDataIOUtility.LoadLanguagesOptions(JsonContentFolderPath);
            if (string.IsNullOrEmpty(currentGameLanguage) == false)
            {
                _currentGameLanguageIndex = Array.IndexOf(EntryLanguages, currentGameLanguage.ToString());
            }
        }

        /// <summary>
        /// Ex. LoadEntryDataCollect("en_US");
        /// </summary>
        /// <param name="currentGameLanguage"></param>
        /// <returns></returns>
        public EntryDataCollection LoadEntryDataCollect(string currentGameLanguage)
        {
            int currentGameLanguageIndex = Array.IndexOf(EntryLanguages, currentGameLanguage);
            return LoadEntryDataCollect(currentGameLanguageIndex);
        }

        public EntryDataCollection LoadEntryDataCollect(int currentGameLanguageIndex)
        {
            if (currentGameLanguageIndex < 0 || currentGameLanguageIndex > EntryLanguages.Length) { return null; }

            _currentGameLanguageIndex = currentGameLanguageIndex;
            string jsonContentPath = FilePath(JsonContentFolderPath);
            if (File.Exists(jsonContentPath))
            {
                string json = File.ReadAllText(jsonContentPath);
                if (string.IsNullOrEmpty(json) == false)
                {
                    return JsonUtility.FromJson<EntryDataCollection>(json);
                }
            }
            return new EntryDataCollection();
        }

        public uint WriteKeyToEnum(string textKey)
        {
            StringBuilder textKeyEntries = new StringBuilder();
            uint newMaxID = ExtractTextKeyContentAndReturnEmptyID(textKeyEntries);
            textKey = textKey.Replace("_", "").Replace(" ", "");
            textKeyEntries.AppendLine($"{textKey}_{newMaxID} = {newMaxID}");

            string textKeyBuildedFile = BuildTextFileFrom(newMaxID, textKeyEntries);

            File.WriteAllText(GetTextKeyPath(), textKeyBuildedFile);
            File.WriteAllText(GetTextKeyPathBackup(), textKeyBuildedFile);

            return newMaxID;
        }

        private uint ExtractTextKeyContentAndReturnEmptyID(StringBuilder textKeyEntries)
        {
            uint maxID = 0;
            TextKey[] enumEntries = (TextKey[])Enum.GetValues(typeof(TextKey));

            foreach (TextKey key in enumEntries)
            {
                string keyText = key.ToString();
                uint keyID = uint.Parse(keyText.Split('_')[1]);

                if (maxID < keyID)
                {
                    maxID = keyID;
                }

                textKeyEntries.Append(keyText).Append(" = ").Append(keyID).AppendLine(",");
            }

            return maxID + 1;
        }

        private string BuildTextFileFrom(uint newMaxID, StringBuilder textKeyEntries)
        {
            string classWarningMessage = $"// Auto generated file, manual changes will be ignored. Max Id = {newMaxID}\r\n";
            return classWarningMessage + "namespace AM_i18n.Scripts.Core\r\n{\r\n\tpublic enum TextKey : uint\r\n\t{\r\n" + textKeyEntries.ToString() + "\r\n\t}\r\n}";
        }

        private static string GetTextKeyPath() => Path.Combine(Directory.GetCurrentDirectory(), "Packages", "com.andersonmarques.i18n", "Scripts", "i18n", "TextKey.cs");

        private string GetTextKeyPathBackup() => Path.Combine(Directory.GetCurrentDirectory(), GetSaveFolderPath(), "TextKeyBackup.cs");

        public void SaveDataToJSON(int entryLanguageIndex, uint keyID, string textValue)
        {
            EntryDataCollection entryDataCollection = LoadEntryDataCollect(entryLanguageIndex);

            if (entryDataCollection.AddTextToExistingEntryData(keyID, textValue) == false)
            {
                EntryData newEntryData = new EntryData
                {
                    KeyID = keyID,
                    Values = new string[] { textValue }
                };
                entryDataCollection.EntriesDatas.Add(newEntryData);
            }

            if (File.Exists(JsonContentFolderPath) == false)
            {
                Directory.CreateDirectory(JsonContentFolderPath);
            }

            File.WriteAllText(FilePath(JsonContentFolderPath), JsonUtility.ToJson(entryDataCollection));
        }

        public void RestoreEntryEnumKeyBackup()
        {
#if UNITY_EDITOR
            bool restore = UnityEditor.EditorUtility.DisplayDialog("Restore Backup", "Do you want to restore the entry enum keys using the backup file?", "Restore", "Cancel");
            if (restore == false) { return; }

            string backupFile = File.ReadAllText(GetTextKeyPathBackup());
            if (string.IsNullOrEmpty(backupFile))
            {
                Debug.LogWarning("Error: No backup file found. Make sure the folder with the TextKeyBackup.cs is set in the \"Tools/Text Manager\" window");
                return;
            }

            File.WriteAllText(GetTextKeyPath(), backupFile);
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }
}
