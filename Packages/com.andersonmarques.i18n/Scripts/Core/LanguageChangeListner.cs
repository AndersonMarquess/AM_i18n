using System;
using UnityEngine;
using TMPro;

namespace AM_i18n.Scripts.Core
{
    public class LanguageChangeListner : MonoBehaviour
    {
        public static void AddListnerToText(TMP_Text text, TextKey key, int amount = 0)
        {
            if (text.transform.TryGetComponent(out LanguageChangeListner language))
            {
                language.Text = text;
                language.TextKey = key;
                language.Amount = amount;
                language.UpdateText();
            }
        }

        [field: SerializeField]
        public TextKey TextKey { get; set; } = default;
        [field: SerializeField]
        public TMP_Text Text { get; set; } = null;
        [field: SerializeField]
        public int Amount { get; set; } = 0;
        private TextManager _textManager = null;

        private void Start()
        {
            if (Text == null)
            {
                Text = GetComponent<TMP_Text>();
            }

            _textManager = TextManager.Instance;
            _textManager.OnLanguageChange += OnLanguageChangeHandler;
            UpdateText();
        }

        private void OnDestroy()
        {
            if (_textManager != null)
            {
                _textManager.OnLanguageChange -= OnLanguageChangeHandler;
            }
        }

        private void OnLanguageChangeHandler(object sender, EventArgs e)
        {
            UpdateText();
        }

        public void UpdateText()
        {
            if (Text != null && _textManager != null)
            {
                Text.SetText(_textManager.GetText(TextKey, Amount));
            }
        }
    }
}
