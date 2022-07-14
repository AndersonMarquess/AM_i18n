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

        public string FilePath => Path.Combine(Application.streamingAssetsPath, "i18n", $"{_currentGameLanguage}.json");

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
            uint maxID = 0;
            StringBuilder textKeyEntries = new StringBuilder();
            TextKey[] enumEntries = (TextKey[])Enum.GetValues(typeof(TextKey));

            foreach (TextKey key in enumEntries)
            {
                string keyText = key.ToString();
                uint keyID = uint.Parse(keyText.Split("_")[1]);

                if (maxID < keyID)
                {
                    maxID = keyID;
                }

                textKeyEntries.Append(keyText).Append(" = ").Append(keyID).AppendLine(",");
            }

            uint newMaxID = maxID + 1;
            textKey = textKey.Replace("_", "").Replace(" ", "");
            textKeyEntries.AppendLine($"{textKey}_{newMaxID} = {newMaxID}");

            string classWarningMessage = $"// Auto generated file. Max Id = {newMaxID}\n";
            string classContent = classWarningMessage + "namespace AM_i18n.Scripts.Core\n{\npublic enum TextKey : uint\n{\n" + textKeyEntries.ToString() + "\n}\n}";
            string textKeyPath = Path.Combine(Directory.GetCurrentDirectory(), "Packages", "com.andersonmarques.i18n", "Scripts", "i18n", "TextKey.cs");
            File.WriteAllText(textKeyPath, classContent);

            return newMaxID;
        }

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

            File.WriteAllText(FilePath, JsonUtility.ToJson(entryDataCollection));
        }
    }
}
