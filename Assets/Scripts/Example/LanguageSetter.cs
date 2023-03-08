using UnityEngine;
using UnityEngine.UI;
using AM_i18n.Scripts.Core;

namespace AM_i18n.Scripts.Example
{
    public class LanguageSetter : MonoBehaviour
    {
        [SerializeField]
        private Button _ptBrButton = null;
        [SerializeField]
        private Button _enUsButton = null;

        private void Awake()
        {
            _ptBrButton.onClick.AddListener(() => TextManager.Instance.SetGameLanguage("pt_BR"));
            _enUsButton.onClick.AddListener(() => TextManager.Instance.SetGameLanguage("en_US"));
        }
    }
}
