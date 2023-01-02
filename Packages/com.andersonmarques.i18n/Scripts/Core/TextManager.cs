using System;
using System.Collections.Generic;
using UnityEngine;

namespace AM_i18n.Scripts.Core
{
    public class TextManager : MonoBehaviour
    {
        private static TextManager _instance = null;
        public static TextManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    Initialize();
                }
                return _instance;
            }
        }

        private static void Initialize()
        {
            _instance = GameObject.FindObjectOfType<TextManager>();
            if (_instance == null)
            {
                GameObject newGb = new GameObject("[TextManager]");
                _instance = newGb.AddComponent<TextManager>();
                _instance.SetGameLanguage(Language.en_US);
            }
            else
            {
                _instance.LoadContentIfNull();
            }
        }

        private const string TEXT_NOT_FOUND = "TEXT_NOT_FOUND";
        public event EventHandler OnLanguageChange = null;
        [SerializeField]
        private Language _currentGameLanguage = Language.en_US;
        private TextDataIOUtility _textDataIOUtility = null;
        private Dictionary<TextKey, string[]> _textDataJson = null;

        public void SetGameLanguage(Language language)
        {
            _currentGameLanguage = language;
            LoadLanguageContent();
            NotifyOnLanguageChange();
        }

        private void LoadContentIfNull()
        {
            if (_textDataIOUtility == null)
            {
                LoadLanguageContent();
                NotifyOnLanguageChange();
            }
        }

        private void LoadLanguageContent()
        {
            _textDataIOUtility ??= new TextDataIOUtility(_currentGameLanguage);
            _textDataJson ??= new Dictionary<TextKey, string[]>();
            _textDataJson?.Clear();

            EntryDataCollection entryDataCollection = _textDataIOUtility.LoadEntryDataCollect(_currentGameLanguage);

            for (int i = 0; i < entryDataCollection.EntriesDatas.Count; i++)
            {
                EntryData entryData = entryDataCollection.EntriesDatas[i];
                _textDataJson[(TextKey)entryData.KeyID] = entryData.Values;
            }

            entryDataCollection.EntriesDatas?.Clear();
        }

        private void NotifyOnLanguageChange()
        {
            OnLanguageChange?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Key - The reference to text content. <br>
        /// Amount (optional) - The phrase index, use this to handle plural version. <br>
        /// 
        /// Json {
        ///     "ABC" : [ "No Abc available", "One abc available", "{0} Abcs available" ]
        /// }<br>
        /// 
        /// GetText(TextKey.ABC, 53) => "53 Abcs available" <br>
        /// GetText(TextKey.ABC) => "No Abc available"
        /// </summary>
        public string GetText(TextKey key, int amount, bool autoFormat = true)
        {
            if (_textDataJson.ContainsKey(key))
            {
                int pluralPhraseIndex = amount > 1 ? 1 : 0;
                if (autoFormat)
                {
                    return string.Format(_textDataJson[key][pluralPhraseIndex], amount);
                }
                return _textDataJson[key][pluralPhraseIndex];
            }
            return TEXT_NOT_FOUND;
        }

        public string GetText(TextKey key)
        {
            if (_textDataJson.ContainsKey(key))
            {
                return _textDataJson[key][0];
            }

            return TEXT_NOT_FOUND;
        }

        public string[] GetAllText(TextKey key)
        {
            if (_textDataJson.ContainsKey(key))
            {
                return _textDataJson[key];
            }

            return null;
        }
    }
}
