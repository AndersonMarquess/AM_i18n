using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace AM_i18n.Scripts.Core
{
    public class TextDataIOUtility
    {
        private Language _currentGameLanguage = Language.en_US;

        public string SaveFolderPath => TextManager.GetSaveFolderPath();
        public string FilePath => Path.Combine(SaveFolderPath, $"{_currentGameLanguage}.json");

        public TextDataIOUtility(Language currentGameLanguage)
        {
            _currentGameLanguage = currentGameLanguage;
        }

        public EntryDataCollection LoadEntryDataCollect(Language currentGameLanguage)
        {
            _currentGameLanguage = currentGameLanguage;
            EntryDataCollection entryDataCollection = null;

            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                entryDataCollection = JsonUtility.FromJson<EntryDataCollection>(json);
            }
            else
            {
                entryDataCollection = new EntryDataCollection();
                entryDataCollection.EntriesDatas = new List<EntryData>();
            }

            return entryDataCollection;
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

        private string GetTextKeyPathBackup() => Path.Combine(Directory.GetCurrentDirectory(), SaveFolderPath, "TextKeyBackup.cs");

        public void SaveDataToJSON(Language currentGameLanguage, uint keyID, string textValue)
        {
            EntryDataCollection entryDataCollection = LoadEntryDataCollect(currentGameLanguage);

            if (entryDataCollection.AddTextToExistingEntryData(keyID, textValue) == false)
            {
                EntryData newEntryData = new EntryData
                {
                    KeyID = keyID,
                    Values = new string[] { textValue }
                };
                entryDataCollection.EntriesDatas.Add(newEntryData);
            }

            string jsonFolderPath = SaveFolderPath;
            if (File.Exists(jsonFolderPath) == false)
            {
                Directory.CreateDirectory(jsonFolderPath);
            }

            File.WriteAllText(FilePath, JsonUtility.ToJson(entryDataCollection));
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
